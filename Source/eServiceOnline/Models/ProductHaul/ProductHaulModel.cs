using eServiceOnline.Models.Commons;
using Sanjel.Common.BusinessEntities.Mdd;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment;
using Sesi.SanjelData.Entities.Common.BusinessEntities.HumanResources;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Operation;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Organization;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Products;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using eServiceOnline.Data;
using eServiceOnline.Gateway;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ResourceSchedule;
using Sesi.SanjelData.Entities.Common.Model.Scheduling;
using eServiceOnline.BusinessProcess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sesi.SanjelData.Entities.Common.BusinessEntities.WellSite;

namespace eServiceOnline.Models.ProductHaul
{
    public class ProductHaulModel :ModelBase<Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul>
    {
        public int ProductHaulId { get; set; }
        public int ProductHaulLoadId { get; set; }
        public int CallSheetNumber { get; set; }
        public string BaseBlend { get; set; }
        public int BaseBlendSectionId { get; set; }
        public string Category { get; set; }
        public int Tractor { get; set; }
        public string Bulker { get; set; }
        public string Driver { get; set; }
        public string Comments { get; set; }
        public string Status { get; set; }
        public double MixWater { get; set; }

        public double SackWeight { get; set; }
        public double Yield { get; set; }

        public double BulkVolume { get; set; }
        public double Density { get; set; }

        public bool IsTotalBlendTonnage { get; set; }
        public double Amount { get; set; }
        public double TotalBlendWeight { get; set; }
        public double BaseBlendWeight { get; set; }
        public string BlendChemicalDescription { get; set; }
        public int BaseBlendChemicalId { get; set; }
        public int ServicePointId { get; set; }
        public string ServicePointName { get; set; }
        public Collection<ProductLoadSectionModel> ProductLoadSections { get; set; }
        public Collection<ProductLoadSectionModel> BaseProductLoadSections { get; set; }
        public Collection<ProductLoadSectionModel> AdditiveProductLoadSections { get; set; }
        public Collection<ProductLoadSectionModel> AdditionalProductLoadSections { get; set; }
        public DateTime OnLocationTime { get; set; }
        public Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaulStatus ProductHaulLifeStatus { get; set; }
        public int BinId { get; set; }
        public string BinNumber { get; set; }
        public int OrigBinInformationId { set; get; }
        public int BinInformationId { set; get; }
        public string BinInformationName { set; get; }
        public int DriverId{ get; set; }
        public string DriverFName{ get; set; }
        public string DriverMName { get; set; }
        public string DriverLName { get; set; }
        public int BulkUnitId { get; set; }
        public string BulkUnitName { get; set; }
        public string PreferedName { get; set; }
        public int TractorUnitId { get; set; }
        public string TractorUnitName { get; set; }
        public int SupplierCompanyId { get; set; }
        public string SupplierCompanyName{ get; set; }
        public string ThirdPartyUnitNumber { get; set; }
        public string SupplierContactName { get; set; }
        public string SupplierContactNumber { get; set; }
        public int Driver2Id { get; set; }
        public int PodIndex { set; get; }
        public string Driver2FName { get; set; }
        public string Driver2MName { get; set; }
        public string Driver2LName { get; set; }
        public string Driver2PreferedName { get; set; }
        public int ExistingProductHaulId { get; set; }
        public int OriginalProductHaulId { get; set; }
        public string ShortExpOlTime { get; set; }
        public string WellLocation { get; set; }
        public bool IsSameLocation { get; set; }
        public bool UseOriginalHaul { get; set; }
        public int OrigCrewId { get; set; }
        public string CrewName { get; set; }
        public int OrigThirdPartyBulkerCrewId { get; set; }

        public int RigJobId { get; set; }
        public int ScheduleId { get; set; }
        public List<ProductHaulLoad> ProductHaulLoads { get; set; }
        public SanjelCrewSchedule CrewSchedule { get; set; }

        public DateTime PumperCrewScheduleEndTime { get; set; }
        public string BaseBlendUnit { get; set; }
        public bool IsBlendTest { get; set; }
        public int OrigBulkPlantId { get; set; }
        public int BulkPlantId { get; set; }
        public string BulkPlantName { get; set; }
        public int CustomerId { get; set; }
        public string ClientName { get; set; }
        public int ProgramId { get; set; }
        public string ProgramNumber { get; set; }
        public int JobTypeId { get; set; }
        public string JobTypeName { get; set; }
        public double BlendAmount { get; set; }
        public double TransferAmount { get; set; }
        public int DestinationStorageNumber { get; set; }
        public int RigId { get; set; }
        public string RigName { get; set; }
        public string ProductHaulDescription { get; set; }
        
        public ProductHaulInfoModel ProductHaulInfoModel { get; set; } = new ProductHaulInfoModel();
        public ProductLoadInfoModel ProductLoadInfoModel { get; set; } = new ProductLoadInfoModel();
        public override void PopulateTo(Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul productHaul)
        {
            if (productHaul != null)
            {
                productHaul.ModifiedUserName = this.LoggedUser;
                productHaul.Id = this.ProductHaulId;
                productHaul.Comments = this.Comments;
                productHaul.IsThirdParty = this.ProductHaulInfoModel.IsThirdParty;
                productHaul.IsGoWithCrew = this.ProductHaulInfoModel.IsGoWithCrew;
                productHaul.ProductHaulLifeStatus = this.ProductHaulLifeStatus;
                productHaul.EstimatedLoadTime = this.ProductHaulInfoModel.EstimatedLoadTime;
                productHaul.ExpectedOnLocationTime = this.ProductHaulInfoModel.ExpectedOnLocationTime;
                productHaul.EstimatedTravelTime = this.ProductHaulInfoModel.EstimatedTravelTime;
                productHaul.Description = this.ProductHaulDescription;

                productHaul.ContractorCompany = new ContractorCompany() {Id=this.SupplierCompanyId, Name = this.SupplierCompanyName};
                productHaul.ThirdPartyUnitNumber = this.ThirdPartyUnitNumber;
                productHaul.SupplierContactName = this.SupplierContactName;
                productHaul.SupplierContactNumber = this.SupplierContactNumber;

            }
        }
        public void PopulateToHaulLoad(Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaulLoad productHaulLoad)
        {
            if (productHaulLoad != null)
            {
                productHaulLoad.ModifiedUserName = this.LoggedUser;
                productHaulLoad.Id = this.ProductHaulLoadId;
                productHaulLoad.BlendSectionId = this.BaseBlendSectionId;
                productHaulLoad.CallSheetNumber = this.CallSheetNumber;
                productHaulLoad.MixWater = this.MixWater;
                productHaulLoad.Comments = this.Comments;
                productHaulLoad.IsTotalBlendTonnage = this.IsTotalBlendTonnage;

                productHaulLoad.Yield = this.Yield;
                productHaulLoad.PodIndex = this.PodIndex;
                productHaulLoad.SackWeight = this.SackWeight;
                productHaulLoad.Density = this.Density;
                productHaulLoad.BulkVolume = this.BulkVolume;
                productHaulLoad.IsBlendTest = this.IsBlendTest;
                productHaulLoad.ProgramId=this.ProgramNumber ; 
                if (this.IsTotalBlendTonnage)
                {
                    productHaulLoad.TotalBlendWeight = this.Amount * 1000;
                }
                else
                {
                    productHaulLoad.BaseBlendWeight = this.Amount * 1000;
                }

                if (productHaulLoad.IsBlendTest)
                {
                    productHaulLoad.OnLocationTime = DateTime.MinValue;
                    productHaulLoad.ExpectedOnLocationTime = DateTime.MinValue;
                    productHaulLoad.IsGoWithCrew = false;
                    productHaulLoad.WellLocation = string.Empty;
                }
                else
                {
                    productHaulLoad.OnLocationTime = this.OnLocationTime;
                    /*
                    if (!this.IsGoWithCrew) 
                        productHaulLoad.ExpectedOnLocationTime = this.ExpectedOlTime;
                    else 
                        */
                    productHaulLoad.ExpectedOnLocationTime = this.ProductHaulInfoModel.ExpectedOnLocationTime;
                    productHaulLoad.EstmatedLoadTime = this.ProductHaulInfoModel.EstimatedLoadTime;
                    productHaulLoad.IsGoWithCrew = this.ProductHaulInfoModel.IsGoWithCrew;
                    productHaulLoad.WellLocation = this.WellLocation;
                }

                if (productHaulLoad.Bin == null)
                {
                    productHaulLoad.Bin = new Bin { Id = this.BinId, Name = this.BinNumber };
                }

                if (productHaulLoad.ServicePoint == null)
                {
                    productHaulLoad.ServicePoint = new ServicePoint() { Id = this.ServicePointId, Name = this.ServicePointName };
                }
                if(productHaulLoad.JobType==null)
                {
                    productHaulLoad.JobType = new JobType() { Id = this.JobTypeId };
                }
                if(productHaulLoad.BulkPlant==null)
                {
                    productHaulLoad.BulkPlant = new Rig() { Id = this.BulkPlantId };
                }
            }
        }

        public override void PopulateFrom(Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul productHaul)
        {
            if (productHaul != null)
            {
                this.ProductHaulId = productHaul.Id;
                this.Comments = productHaul.Comments;
                this.ProductHaulInfoModel.IsThirdParty = productHaul.IsThirdParty;
                this.ProductHaulInfoModel.IsGoWithCrew = productHaul.IsGoWithCrew;
                this.ProductHaulLifeStatus = productHaul.ProductHaulLifeStatus;
//                this.ScheduleId = productHaul.Schedule?.Id ?? 0;
                this.ProductHaulInfoModel.EstimatedLoadTime = productHaul.EstimatedLoadTime==DateTime.MinValue?DateTime.Now : productHaul.EstimatedLoadTime;
                this.ProductHaulInfoModel.EstimatedTravelTime = productHaul.EstimatedTravelTime;
                this.ProductHaulInfoModel.ExpectedOnLocationTime = productHaul.ExpectedOnLocationTime;
                this.ProductHaulInfoModel.CrewId = productHaul.Crew.Id;

                if (!this.ProductHaulInfoModel.IsThirdParty)
                {
                    if (productHaul.BulkUnit != null)
                    {
                        this.BulkUnitId = productHaul.BulkUnit.Id;
                        this.BulkUnitName = productHaul.BulkUnit.Name;
                        this.Bulker = productHaul.BulkUnit.Name + (productHaul.TractorUnit != null
                                          ? (" / " + productHaul.TractorUnit.Name)
                                          : string.Empty);
                    }

                    if (productHaul.Driver != null)
                    {
                        this.DriverId = productHaul.Driver.Id;
                        this.DriverFName = productHaul.Driver.FirstName;
                        this.DriverMName = productHaul.Driver.MiddleName;
                        this.DriverLName = productHaul.Driver.LastName;
                        this.PreferedName = productHaul.Driver.PreferedFirstName;
                        this.Driver = productHaul.Driver.Name;
                    }

                    if (productHaul.Driver2 != null)
                    {
                        this.Driver2Id = productHaul.Driver2.Id;
                        this.Driver2FName = productHaul.Driver2.FirstName;
                        this.Driver2MName = productHaul.Driver2.MiddleName;
                        this.Driver2LName = productHaul.Driver2.LastName;
                        this.Driver2PreferedName = productHaul.Driver2.PreferedFirstName;
                    }

                    if (productHaul.TractorUnit != null)
                    {
                        this.TractorUnitId = productHaul.TractorUnit.Id;
                        this.TractorUnitName = productHaul.TractorUnit.Name;
                    }
                }
                else
                {
                    this.Bulker = productHaul.ContractorCompany.Name + "/" + productHaul.ThirdPartyUnitNumber;
                    this.Driver = productHaul.SupplierContactName;
                }

/*
                this.SupplierCompanyId = productHaul.ContractorCompany.Id;
                this.SupplierCompanyName = productHaul.ContractorCompany.Name;
                this.ThirdPartyUnitNumber = productHaul.ThirdPartyUnitNumber;
                this.SupplierContactName = productHaul.SupplierContactName;
                this.SupplierContactNumber = productHaul.SupplierContactNumber;
*/
            }
        }

        public void PopulateFromHaulLoad(ProductHaulLoad productHaulLoad)
        {
            if (productHaulLoad != null)
            {
                this.ProductHaulLoadId = productHaulLoad.Id;
                this.CallSheetNumber = productHaulLoad.CallSheetNumber;
                this.BaseBlendSectionId = productHaulLoad.BlendSectionId;
                BlendChemical blendChemical = productHaulLoad.BlendChemical;
                this.BaseBlend = blendChemical == null ? "" : (string.IsNullOrWhiteSpace(blendChemical.Description) ? blendChemical.Name : blendChemical.Description);
                this.Category = productHaulLoad.BlendCategory == null ? "" : (string.IsNullOrEmpty(productHaulLoad.BlendCategory.Description) ? productHaulLoad.BlendCategory.Name : productHaulLoad.BlendCategory.Description);
                this.MixWater = productHaulLoad.MixWater;
                double enteredBlendWeight = productHaulLoad.IsTotalBlendTonnage ? productHaulLoad.TotalBlendWeight : productHaulLoad.BaseBlendWeight;
                this.Amount = enteredBlendWeight / 1000 ;
                this.TotalBlendWeight = productHaulLoad.TotalBlendWeight;
                this.BaseBlendWeight = productHaulLoad.BaseBlendWeight;
                this.BlendChemicalDescription = productHaulLoad?.BlendChemical?.Description;
                this.IsTotalBlendTonnage = productHaulLoad.IsTotalBlendTonnage;
                this.Comments = productHaulLoad.Comments;
                this.OnLocationTime = productHaulLoad.OnLocationTime;
                this.ProductHaulInfoModel.ExpectedOnLocationTime = productHaulLoad.ExpectedOnLocationTime;
                this.ProductHaulInfoModel.EstimatedLoadTime = productHaulLoad.EstmatedLoadTime;
                this.ProductHaulInfoModel.IsGoWithCrew = productHaulLoad.IsGoWithCrew;
                this.WellLocation = productHaulLoad.WellLocation;
                this.PodIndex = productHaulLoad.PodIndex;
                this.ProgramNumber = productHaulLoad.ProgramId;
                
                this.Yield = productHaulLoad.Yield;
                this.SackWeight = productHaulLoad.SackWeight;
                this.Density = productHaulLoad.Density;
                this.BulkVolume = productHaulLoad.BulkVolume;
                this.BaseBlendUnit = (productHaulLoad.Unit == null||string.IsNullOrEmpty(productHaulLoad.Unit.Description))?"t":productHaulLoad.Unit.Description;
                this.IsBlendTest = productHaulLoad.IsBlendTest;
                this.RigName = productHaulLoad.Rig?.Name;
                this.RigId = productHaulLoad.Rig==null?0:productHaulLoad.Rig.Id;
                this.ClientName = productHaulLoad.Customer?.Name;

                if (productHaulLoad.ServicePoint != null)
                {
                    this.ServicePointId = productHaulLoad.ServicePoint.Id;
                    this.ServicePointName = productHaulLoad.ServicePoint.Name??string.Empty;
                }

                var blendSection = productHaulLoad.CallSheetNumber>0?eServiceWebContext.Instance.GetBlendSectionByBlendSectionId(this.BaseBlendSectionId):eServiceOnlineGateway.Instance.GetProgramBlendSectionByBlendSectionId(this.BaseBlendSectionId);

                Collection<ProductLoadSection> productLoadList = ProductLoadList(productHaulLoad.AllProductLoadList==null?new Collection<ProductLoadSection>():new Collection<ProductLoadSection>(productHaulLoad.AllProductLoadList));
                if (productLoadList != null)
                {
                    this.ProductLoadSections = this.ProductLoadSections ?? new Collection<ProductLoadSectionModel>();
                    foreach (ProductLoadSection productLoadSection in productLoadList)
                    {
                        ProductLoadSectionModel productLoadSectionModel = new ProductLoadSectionModel();
                        productLoadSectionModel.PopulateFrom(productLoadSection);
                        productLoadSectionModel.ProductHaulId = this.ProductHaulId;

                        this.ProductLoadSections.Add(productLoadSectionModel);
                    }
                }

                Collection<ProductLoadSection> baseProductLoadList = BaseProductLoadList(productHaulLoad.AllProductLoadList == null ? new Collection<ProductLoadSection>() : new Collection<ProductLoadSection>(productHaulLoad.AllProductLoadList));
                if (baseProductLoadList != null)
                {
                    this.BaseProductLoadSections = this.BaseProductLoadSections ?? new Collection<ProductLoadSectionModel>();
                    foreach (ProductLoadSection productLoadSection in baseProductLoadList)
                    {
                        ProductLoadSectionModel productLoadSectionModel = new ProductLoadSectionModel();
                        productLoadSectionModel.PopulateFrom(productLoadSection);
                        productLoadSectionModel.ProductHaulId = this.ProductHaulId;
                        this.BaseProductLoadSections.Add(productLoadSectionModel);
                    }
                }
                Collection<ProductLoadSection> additiveProductLoadList =AdditiveProductLoadList(new Collection<ProductLoadSection>(productHaulLoad.AllProductLoadList == null ? new Collection<ProductLoadSection>() : new Collection<ProductLoadSection>(productHaulLoad.AllProductLoadList)));
                if (additiveProductLoadList != null)
                {
                    this.AdditiveProductLoadSections = this.AdditiveProductLoadSections ?? new Collection<ProductLoadSectionModel>();
                    foreach (ProductLoadSection productLoadSection in additiveProductLoadList)
                    {
                        ProductLoadSectionModel productLoadSectionModel = new ProductLoadSectionModel();
                        productLoadSectionModel.PopulateFrom(productLoadSection);
                        var percentage = Math.Round(productLoadSectionModel.RequiredAmount / this.BaseBlendWeight * 100, 2, MidpointRounding.AwayFromZero);
                        var additiveSection = blendSection.BlendAdditiveSections.FirstOrDefault(p =>
                             p.AdditiveType.Name.Trim() == productLoadSectionModel.BlendChemicalModel.Name.Trim());
                        if (additiveSection != null)
                            productLoadSectionModel.BlendChemicalModel.Name =
                                additiveSection.AdditiveType.Name + " @ " + additiveSection.Amount +
                                additiveSection.AdditiveAmountUnit.Description;
                        productLoadSectionModel.ProductHaulId = this.ProductHaulId;
                        this.AdditiveProductLoadSections.Add(productLoadSectionModel);
                    }
                }
                Collection<ProductLoadSection> additionalProductLoadList =AdditionalProductLoadList(new Collection<ProductLoadSection>(productHaulLoad.AllProductLoadList == null ? new Collection<ProductLoadSection>() : new Collection<ProductLoadSection>(productHaulLoad.AllProductLoadList)));
                if (additionalProductLoadList != null)
                {
                    this.AdditionalProductLoadSections = this.AdditionalProductLoadSections ?? new Collection<ProductLoadSectionModel>();
                    foreach (ProductLoadSection productLoadSection in additionalProductLoadList)
                    {
                        ProductLoadSectionModel productLoadSectionModel = new ProductLoadSectionModel();
                        productLoadSectionModel.PopulateFrom(productLoadSection);
                        var percentage = Math.Round(productLoadSectionModel.RequiredAmount / this.BaseBlendWeight * 100, 2, MidpointRounding.AwayFromZero);
                        var additiveSection = blendSection.BlendAdditiveSections.FirstOrDefault(p =>
                            p.AdditiveType.Name.Trim() == productLoadSectionModel.BlendChemicalModel.Name.Trim());
                        if (additiveSection != null)
                            productLoadSectionModel.BlendChemicalModel.Name =
                                additiveSection.AdditiveType.Name + " @ " + additiveSection.Amount +
                                additiveSection.AdditiveAmountUnit.Description;
                        productLoadSectionModel.ProductHaulId = this.ProductHaulId;
                        this.AdditionalProductLoadSections.Add(productLoadSectionModel);
                    }
                }

                if (productHaulLoad.Bin != null)
                {
                    this.BinId = productHaulLoad.Bin.Id;
                    this.BinNumber = productHaulLoad.Bin.Name;
                }

                if (productHaulLoad.BulkPlant != null && productHaulLoad.BulkPlant.Id>0)
                {
                    this.OrigBulkPlantId = productHaulLoad.BulkPlant.Id;
                    this.BulkPlantId = productHaulLoad.BulkPlant.Id;
                    this.BulkPlantName = productHaulLoad.BulkPlant.Name;
                }
                if(productHaulLoad.JobType!=null && productHaulLoad.JobType.Id>0)
                {
                    this.JobTypeId = productHaulLoad.JobType.Id;
                    this.JobTypeName = productHaulLoad.JobType.Name;
                }

            }
        }

        public static Collection<ProductLoadSection> ProductLoadList(Collection<ProductLoadSection> AllProductLoadList)
        {

            if (AllProductLoadList != null && AllProductLoadList.Count > 0)
            {
                return GetProductLoadListFromAll(AllProductLoadList);
            }
            return null;


        }
        public static Collection<ProductLoadSection> GetProductLoadListFromAll(Collection<ProductLoadSection> productLoadList)
        {
            List<ProductLoadSection> list = new List<ProductLoadSection>();

            Dictionary<BlendChemical, Collection<ProductLoadSection>> dictionary = GetBlendChemicalDictionary(productLoadList);

            foreach (KeyValuePair<BlendChemical, Collection<ProductLoadSection>> keyValuePair in dictionary)
            {
                double chemicalAmount = 0.0;
                ProductLoadSection productLoadSection = new ProductLoadSection() { BlendChemical = keyValuePair.Key };
                bool hasPreblend = false;
                foreach (ProductLoadSection item in keyValuePair.Value)
                {
                    if (item.AdditiveBlendMethod.Name.Equals("Preblend"))
                    {
                        chemicalAmount += item.RequiredAmount;
                        if (productLoadSection.BlendAdditiveMeasureUnit == null)
                        {
                            productLoadSection.BlendAdditiveMeasureUnit = item.BlendAdditiveMeasureUnit;
                            productLoadSection.AdditiveBlendMethod = item.AdditiveBlendMethod;
                        }
                        hasPreblend = true;
                    }
                }
                productLoadSection.RequiredAmount = chemicalAmount;

                if (hasPreblend)
                    list.Add(productLoadSection);
            }

            return new Collection<ProductLoadSection>(list);
        }

        public static Dictionary<BlendChemical, Collection<ProductLoadSection>> GetBlendChemicalDictionary(Collection<ProductLoadSection> allProductLoadList)
        {
            if (allProductLoadList == null) return null;

            Dictionary<BlendChemical, Collection<ProductLoadSection>> dictionary = new Dictionary<BlendChemical, Collection<ProductLoadSection>>();

            foreach (ProductLoadSection item in allProductLoadList)
            {
                List<int> keyIds = new List<BlendChemical>(dictionary.Keys).Select(p => p.Id).ToList();
                if (!keyIds.Contains(item.BlendChemical.Id))
                {
                    dictionary.Add(item.BlendChemical, new Collection<ProductLoadSection>() { item });
                }
                else
                {
                    BlendChemical blendChemical = new List<BlendChemical>(dictionary.Keys).Find(p => p.Id.Equals(item.BlendChemical.Id));
                    Collection<ProductLoadSection> value;
                    dictionary.TryGetValue(blendChemical, out value);
                    if (value != null) value.Add(item);
                    dictionary[blendChemical] = value;
                }
            }

            return dictionary;
        }
        public static Collection<ProductLoadSection> BaseProductLoadList(Collection<ProductLoadSection> AllProductLoadList)
        {

            if (AllProductLoadList != null && AllProductLoadList.Count > 0)
            {
                return CommonEntityBase.FindItems(AllProductLoadList, p => p.IsFromBase);
            }
            return null;

        }

        public static Collection<ProductLoadSection> AdditiveProductLoadList(Collection<ProductLoadSection> AllProductLoadList)
        {

            if (AllProductLoadList != null && AllProductLoadList.Count > 0)
            {
                return CommonEntityBase.FindItems(AllProductLoadList, p => p.IsFromBase == false && p.AdditiveBlendMethod.Name.Equals("Preblend"));
            }
            return null;

        }

        public static Collection<ProductLoadSection> AdditionalProductLoadList(Collection<ProductLoadSection> AllProductLoadList)
        {

            if (AllProductLoadList != null && AllProductLoadList.Count > 0)
            {
                return CommonEntityBase.FindItems(AllProductLoadList, p => p.IsFromBase == false && (p.AdditiveBlendMethod.Name.Equals("Prehydrated") || p.AdditiveBlendMethod.Name.Equals("AddedOnTheFly")));
            }
            return null;

        }
    }
}
