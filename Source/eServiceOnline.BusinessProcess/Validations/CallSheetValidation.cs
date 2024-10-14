using System.Collections.Generic;
using System.Collections.ObjectModel;
using MetaShare.Common.Foundation.Entities;
using Sanjel.BusinessEntities;
using Sanjel.BusinessEntities.CallSheets;
using Sanjel.BusinessEntities.Sections.Common;
using Sanjel.BusinessEntities.Sections.Header;
using Sanjel.BusinessEntities.Sections.Pumping;
using Sanjel.Common.BusinessEntities.Lookup;
using Sanjel.Common.BusinessEntities.Reference;
using Sanjel.Common.Core.Utilities;
using Sanjel.Common.EService.Sections.Common;
using MetaShare.Common.Foundation.EntityBases;

namespace eServiceOnline.BusinessProcess
{
    public class CallSheetValidation
    {
        public static bool ValidateEntity(CallSheet callSheet)
        {
            ValidationBase validation = CreateMandatoryFieldCheckForCallSheet(callSheet);
            if (validation.CheckIsSuccessful())
            {
                callSheet.Status = EServiceEntityStatus.Ready;
                return true;
            }

            callSheet.Status = EServiceEntityStatus.InProgress;
            return false;
        }

        public static bool ValidateEntityForScheduled(CallSheet callSheet)
        {
            ValidationBase validation = CreateMandatoryFieldCheckExcludeCallOutInfo(callSheet);
            if (validation.CheckIsSuccessful())
            {
                callSheet.Status = EServiceEntityStatus.Ready;
                return true;
            }

            callSheet.Status = EServiceEntityStatus.InProgress;
            return false;
        }


        public static ValidationBase CreateMandatoryFieldCheckForCallSheet(CallSheet callSheet)
        {
            ValidationBase validation = new ValidationBase(callSheet);

            validation.AddValidateRule(CheckCallSheetDirectionToLocation);
            validation.AddValidateRule(CheckCallSheetWellboreFluid);
            validation.AddValidateRule(CheckCallSheetBinSections);
            validation.AddValidateRule(CheckCallSheetSalesPersonSections);
            validation.AddValidateRule(CheckCallSheetCallOutInformation);
            validation.AddValidateRule(CheckCallSheetProductSections);
            validation.AddValidateRule(CheckCallSheetUnitsPersonnel);
            validation.AddValidateRule(CheckCrewAndUnits);

            return validation;
        }

        public static ValidationBase CreateMandatoryFieldCheckExcludeCallOutInfo(CallSheet callSheet)
        {
            ValidationBase validation = new ValidationBase(callSheet);

            validation.AddValidateRule(CheckCallSheetDirectionToLocation);
            validation.AddValidateRule(CheckCallSheetWellboreFluid);
            validation.AddValidateRule(CheckCallSheetBinSections);
            validation.AddValidateRule(CheckCallSheetSalesPersonSections);
            validation.AddValidateRule(CheckCallSheetProductSections);
            validation.AddValidateRule(CheckCallSheetUnitsPersonnel);
            validation.AddValidateRule(CheckCrewAndUnits);
            validation.AddValidateRule(CheckOneClient);
            validation.AddValidateRule(CheckOneCompanyToBeInvoiced);
            validation.AddValidateRule(CheckCompanyId);

            return validation;
        }

        private static ValidateRuleBase.CheckEntity CheckCallSheetDirectionToLocation
            = delegate(ObjectBase entity)
            {
                CallSheet selectedCallSheet = entity as CallSheet;
                if (selectedCallSheet != null)
                {
                    WellLocationInformation wellLocationInformation = selectedCallSheet.Header.HeaderDetails.WellLocationInformation;
                    bool flag = string.IsNullOrEmpty(wellLocationInformation.DirectionToLocation) || wellLocationInformation.DirectionToLocation.Trim().Length == 0;

                    return flag;
                }
                return true;
            };


        public static readonly ValidateRuleBase.CheckEntity CheckCallSheetWellboreFluid
            = delegate(ObjectBase entity)
            {
                CallSheet selectedCallSheet = entity as CallSheet;
                if (selectedCallSheet != null)
                {
                    CallSheetHeaderWellInformation wellInformation = selectedCallSheet.Header.WellInformation;
                    return selectedCallSheet.ServiceLineType == ServiceLineType.Pumping
                           && (wellInformation == null || wellInformation.WellboreFluidSections.Count == 0);
                }
                return true;
            };


        public static readonly ValidateRuleBase.CheckEntity CheckCallSheetBinSections
            = delegate(ObjectBase entity)
            {
                CallSheet selectedCallSheet = entity as CallSheet;
                if (selectedCallSheet == null) return true;
                Collection<UnitSection> unitSections = selectedCallSheet.CommonSection.UnitPersonnel.UnitSections;
                if (selectedCallSheet.ServiceLineType.Equals(ServiceLineType.Pumping) && selectedCallSheet.Header.BusinessUnit.Equals(BusinessUnit.Canada))
                {
                    if (isProductHaulChecked(unitSections))
                    {
                        Collection<BinSection> binSections =
                            selectedCallSheet.CommonSection.UnitPersonnel.BinSections;

                        return binSections == null || binSections.Count == 0 || isNullVolumePumped(binSections);
                    }
                }
                return false;
            };

        public static readonly ValidateRuleBase.CheckEntity CheckCallSheetSalesPersonSections
            = delegate(ObjectBase entity)
            {
                CallSheet selectedCallSheet = entity as CallSheet;
                if (selectedCallSheet == null) return true;

                if (selectedCallSheet.CommonSection.ContactSection.SalesPersonUnknown == false)
                {
                    Collection<EmployeeSection> employeeSections = selectedCallSheet.CommonSection.ContactSection.SalespersonSections;
                    return (employeeSections == null || employeeSections.Count == 0);
                }

                return false;
            };

        public static readonly ValidateRuleBase.CheckEntity CheckCallSheetCallOutInformation
            = delegate(ObjectBase entity)
            {
                CallSheet selectedCallSheet = entity as CallSheet;
                if (selectedCallSheet == null) return true;

                FirstCall firstCall = selectedCallSheet.Header.HeaderDetails.FirstCall;
                return firstCall.IsWillCallback || firstCall.IsExpectedTimeOnLocation;
            };

        public static readonly ValidateRuleBase.CheckEntity CheckCallSheetProductSections
            = delegate(ObjectBase entity)
            {
                CallSheet selectedCallSheet = entity as CallSheet;
                if (selectedCallSheet == null) return true;
                if (selectedCallSheet.ServiceLineType.Equals(ServiceLineType.Pumping) && selectedCallSheet.Header.BusinessUnit.Equals(BusinessUnit.Canada))
                {
                    ProductSection productSection = ((PumpingCallSheet) selectedCallSheet).PumpingSection.ProductSection;

                    if (productSection.BlendSections != null)
                        foreach (BlendSection blendSection in productSection.BlendSections)
                            if (blendSection != null && blendSection.Quantity == null)
                                return true;
                }
                return false;
            };

        public static ValidateRuleBase.CheckEntity CheckCallSheetUnitsPersonnel
            = delegate(ObjectBase entity)
            {
                CallSheet selectedCallSheet = entity as CallSheet;
                if (selectedCallSheet != null && selectedCallSheet.CommonSection != null)
                {
                    UnitPersonnel unitPersonnel = selectedCallSheet.CommonSection.UnitPersonnel;
                    if (unitPersonnel != null && unitPersonnel.IsNeedBin && unitPersonnel.NumberOfBin > unitPersonnel.BinSectionCount)
                    {
                        return true;
                    }

                    return false;
                }
                return true;
            };

        public static ValidateRuleBase.CheckEntity CheckCrewAndUnits
            = delegate(ObjectBase entity)
            {
                List<int> pumperIds = new List<int>() { 205, 288, 238, 71, 62, 291, 276 };

                CallSheet selectedCallSheet = entity as CallSheet;
                if (selectedCallSheet != null && selectedCallSheet.CommonSection != null && selectedCallSheet.CommonSection.UnitPersonnel != null)
                {
                    foreach (UnitSection item in selectedCallSheet.CommonSection.UnitPersonnel.UnitSections)
                    {
                        if ((item.ProductHaulId==null || item.ProductHaulId==0) && (pumperIds.Contains(item.TractorSubtype.Id) ||pumperIds.Contains(item.UnitSubtype.Id)))
                        {
                            return false;
                        }
                    }
                }
                return true;
            };

        public static ValidateRuleBase.CheckEntity CheckOneClient
	        = delegate(ObjectBase entity)
	        {
		        CallSheet selectedCallSheet = entity as CallSheet;
		        if (selectedCallSheet != null && selectedCallSheet.Header != null &&
		            selectedCallSheet.Header.HeaderDetails.CompanyInformation != null)
		        {
			        int clientCount = 0;
			        foreach (CompanyInfoSection item in selectedCallSheet.Header.HeaderDetails.CompanyInformation
				                 .CompanyInfoSections)
			        {
				        if (item.IsClient) clientCount++;
			        }

			        if (clientCount == 1) return false;
		        }

		        return true;
	        };

        public static ValidateRuleBase.CheckEntity CheckOneCompanyToBeInvoiced
	        = delegate (ObjectBase entity)
            {
	            CallSheet selectedCallSheet = entity as CallSheet;
                if (selectedCallSheet != null && selectedCallSheet.Header != null && selectedCallSheet.Header.HeaderDetails.CompanyInformation != null)
                {
                    int toBeInvoicedCount = 0;
                    foreach (CompanyInfoSection item in selectedCallSheet.Header.HeaderDetails.CompanyInformation.CompanyInfoSections)
                    {
                        if (item.IsToBeInvoiced) toBeInvoicedCount++;
                    }

                    if (toBeInvoicedCount == 0) return true;
                    if (toBeInvoicedCount == 1) return false;
                    if (toBeInvoicedCount > 1) return true;
                }
                return true;
            };
        public static ValidateRuleBase.CheckEntity CheckCompanyId
	        = delegate (ObjectBase entity)
            {
	            CallSheet selectedCallSheet = entity as CallSheet;
                if (selectedCallSheet != null && selectedCallSheet.Header != null && selectedCallSheet.Header.HeaderDetails.CompanyInformation != null)
                {
                    bool noId = false;
                    foreach (CompanyInfoSection item in selectedCallSheet.Header.HeaderDetails.CompanyInformation.CompanyInfoSections)
                    {
                        if (item.Company == null || item.Company.Id == 0) return true;
                    }

                }
                return false;
            };
        private static bool isProductHaulChecked(Collection<UnitSection> unitSections)
        {
            if (unitSections != null)
            {
                foreach (UnitSection unitSection in unitSections)
                {
                    if (unitSection.IsProductHaul) return true;
                }
            }
            return false;
        }

        private static bool isNullVolumePumped(Collection<BinSection> binSections)
        {
            if (binSections != null)
            {
                foreach (BinSection binSection in binSections)
                {
                    if (binSection.BinType.Equals("Bulk Bin") && binSection.InitialVolume == null) return true;
                }
            }
            return false;
        }
    }
}