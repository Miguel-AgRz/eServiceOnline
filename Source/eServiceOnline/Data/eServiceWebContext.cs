using eServiceOnline.BusinessProcess;
using eServiceOnline.BusinessProcess.Interface;
using eServiceOnline.Gateway;
using eServiceOnline.Models.ProductHaul;
using eServiceOnline.Models.RigBoard;
using MetaShare.Common.Foundation.Logging;
using MetaShare.Common.Foundation.Permissions;
using MetaShare.Common.ServiceModel.Services;
using Sanjel.BusinessEntities;
using Sanjel.BusinessEntities.CallSheets;
using Sanjel.BusinessEntities.Jobs;
using Sanjel.BusinessEntities.Sections.Header;
using Sanjel.Common.BusinessEntities;
using Sanjel.Common.BusinessEntities.Mdd;
using Sanjel.Common.EService.Sections.Common;
using Sanjel.EService.MicroService.Interfaces;
using Sanjel.Services.Interfaces;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Crew;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ResourceSchedule;
using Sesi.SanjelData.Entities.BusinessEntities.WellSite;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment;
using Sesi.SanjelData.Entities.Common.BusinessEntities.HumanResources;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Organization;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Products;
using Sesi.SanjelData.Entities.Common.BusinessEntities.WellSite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using eServiceOnline.Models.NubbinBoard;
using eServiceOnline.Models.SwedgeBoard;
using eServiceOnline.Models.WitsBoxBoard;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Operation;
using Sesi.SanjelData.Entities.Common.Model.Scheduling;
using Sesi.SanjelData.Services.Interfaces.BusinessEntities.Operation.Dispatch;
using sesi.SanjelLibrary.BlendLibrary;
using Bin = Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment.Bin;
using BlendAdditiveSection = Sesi.SanjelData.Entities.BusinessEntities.BaseEntites.BlendAdditiveSection;
using CallSheet = Sanjel.BusinessEntities.CallSheets.CallSheet;
using ClientConsultant = Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch.ClientConsultant;
using Rig = Sesi.SanjelData.Entities.Common.BusinessEntities.WellSite.Rig;
using RigStatus = Sesi.SanjelData.Entities.Common.BusinessEntities.WellSite.RigStatus;
using TruckUnit = Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment.TruckUnit;
using UnitSection = Sanjel.BusinessEntities.Sections.Common.UnitSection;
using Utility = eServiceOnline.Controllers.Utility;
using BlendSection = Sesi.SanjelData.Entities.BusinessEntities.BaseEntites.BlendSection;
using ICallSheetService = Sanjel.Services.Interfaces.ICallSheetService;

namespace eServiceOnline.Data
{
    public class eServiceWebContext : IeServiceWebContext
    {
        #region Constructors

        static eServiceWebContext()
        {
        }

        #endregion

        #region DeleteProductHaul Method

        /*
        public void DeleteProductHaul(ProductHaulModel model)
        {
            ProductHaul productHaul = eServiceOnlineGateway.Instance.GetProductHaulById(model.ProductHaulId);
            if (productHaul == null) return;

            productHaul.ModifiedUserName = model.LoggedUser;

            ProductHaulProcess.DeleteProductHaul(productHaul, true, TODO);
        }
        */

        public void DeleteProductHaul(CancelProductViewModel model)
        {
            ProductHaul productHaul = eServiceOnlineGateway.Instance.GetProductHaulById(model.ProductHaulId);
            if (productHaul == null) return;

            productHaul.ModifiedUserName = model.LoggedUser;
            var crewId = productHaul.Crew.Id;

            //3. Delete all shipping loads related shipping loads
            foreach (var shippingLoadSheetModel in model.CheckShipingLoadSheetModels)
            {
                if (shippingLoadSheetModel.IsChecked)
                {
                    var shippingLoadSheet =
                        productHaul.ShippingLoadSheets.Find(
                            p => p.Id == shippingLoadSheetModel.ShippingLoadSheetModel.Id);
                    //If a blend request was scheduled together, delete it if it hasn't blended.
                    if (shippingLoadSheet != null)
                    {
	                    productHaul.ShippingLoadSheets.Remove(shippingLoadSheet);
                        ProductHaulProcess.DeleteProductHaulLoadScheduledTogether(shippingLoadSheet);
                        eServiceOnlineGateway.Instance.DeleteShippingLoadSheet(shippingLoadSheet);
                    }
                }
            }
            //Only after all shipping load sheets are canceled
            if (productHaul.ShippingLoadSheets.Count == 0)
            {
	            ProductHaulProcess.DeleteProductHaul(productHaul, true);
	            CrewProcess.UpdateBulkerCrewStatus(productHaul.Crew.Id, productHaul.IsThirdParty, model.LoggedUser);
            }
        }
        #endregion

        #region Update JobDate

        public RigJob GetRigJobByCallsheetNumber(int callsheetNumber)
        {
            RigJob rigJob = eServiceOnlineGateway.Instance.GetRigJobByCallSheetNumber(callsheetNumber);

            return rigJob;
        }

        #endregion

        public int DeleteRigJob(RigJob rigJob)
        {
            return RigBoardProcess.DeleteRigJob(rigJob);
        }

        public bool ActivateARig(int rigId)
        {
            return RigBoardProcess.ActivateARig(rigId);
        }
        public bool DeactivateRig(int rigId)
        {
            return RigBoardProcess.DeactivateRig(rigId);
        }

        #region CovertToBlendChemicalFromBlendSection Method

        private BlendChemical CovertToBlendChemicalFromBlendSection(BlendSection blendSection)
        {
            DateTime startDateTime = DateTime.Now;
            if (blendSection == null) return null;

            /*
            Collection<BlendChemical> blendChemicals = new Collection<BlendChemical>();

            foreach (var blendChemical in this.BlendChemicals)
            {
                var copyChemical = blendChemical.DeepClone() as BlendChemical;
                blendChemicals.Add(copyChemical);

            }
            */
            Debug.WriteLine("\t\tCovertToBlendChemicalFromBlendSection - {0,21}", DateTime.Now.Subtract(startDateTime));

            return BlendCalculator.CovertToBlendChemicalFromBlendSection(CacheData.BlendChemicals, blendSection,CacheData.AdditionMethods, CacheData.AdditiveBlendMethods, CacheData.BlendAdditiveMeasureUnits, CacheData.BlendRecipes);
        }

        #endregion

        #region GetCalculatedProductHaul Method

        public ProductHaulLoad GetCalculatedProductHaulLoad(BlendChemical blendChemical,
	        ProductHaulLoad originalProductHaulLoad, bool isTotalBlendTonnage)
        {
            DateTime startDateTime = DateTime.Now;

            if (blendChemical == null) return originalProductHaulLoad;
            if (originalProductHaulLoad == null) return null;
            double? amount = originalProductHaulLoad.IsTotalBlendTonnage
                ? originalProductHaulLoad.TotalBlendWeight/1000
                : originalProductHaulLoad.BaseBlendWeight/1000;
            BlendAdditiveMeasureUnit blendUnit = blendChemical.BlendRecipe?.Unit;

            if ((amount != null) && (amount > 0))
            {
                Collection<BlendChemicalSection> allBlendBreakDowns = null;
                Collection<BlendChemicalSection> baseBlendBreakDowns = null;
                Collection<BlendChemicalSection> additiveBlendBreakDowns = null;
                Collection<BlendChemicalSection> additionalBlendBreakDowns = null;

                double totalBlendWeight;
                double baseBlendWeight;
                bool isAirborneHarzard = false;
                //default unit of measure for product haul is from blend section, breakdown unit of measure should be from recipe, but default should by kg
                BlendCalculator.GetAllBlendBreakDown1(CacheData.BlendChemicals, CacheData.BlendRecipes, blendChemical.BlendRecipe, (double) amount, blendChemical.Yield,
                    isTotalBlendTonnage, originalProductHaulLoad.MixWater, blendUnit, out allBlendBreakDowns,
                    out baseBlendBreakDowns, out additiveBlendBreakDowns, out additionalBlendBreakDowns,
                    out totalBlendWeight, out baseBlendWeight, out isAirborneHarzard);

                //Dec 28, 2023 AW Fix_Reschedule_ProductHaul: Remove duplicate value assignments to remove blendsection parameter
                originalProductHaulLoad.BulkVolume =  blendChemical.BulkDensity > 0 ? totalBlendWeight / blendChemical.BulkDensity:0;

                Collection<ProductLoadSection> allProductLoadSection = new Collection<ProductLoadSection>();
                this.GetProductLoadSectionCollection(allProductLoadSection, baseBlendBreakDowns, true);
                this.GetProductLoadSectionCollection(allProductLoadSection, additiveBlendBreakDowns, false);
                this.GetProductLoadSectionCollection(allProductLoadSection, additionalBlendBreakDowns, false);
                originalProductHaulLoad.BlendChemical = blendChemical;
                originalProductHaulLoad.AllProductLoadList = new List<ProductLoadSection>(allProductLoadSection);
                originalProductHaulLoad.BaseBlendWeight = baseBlendWeight*1000;
                originalProductHaulLoad.TotalBlendWeight = totalBlendWeight*(blendUnit?.Description=="t"?1000:1);
                originalProductHaulLoad.Unit = blendUnit;
                originalProductHaulLoad.IsAirborneHazard = isAirborneHarzard;
                return originalProductHaulLoad;
            }

            Debug.WriteLine("\t\tGetCalculatedProductHaul - {0,21}", DateTime.Now.Subtract(startDateTime));

            return originalProductHaulLoad;
        }

        #endregion

        #region Common Methods

        public List<T> ListOrderBy<T>(List<T> tList, Func<T, string> element)
        {
            List<T> ts = new List<T>();
            if (tList != null)
            {
                IOrderedEnumerable<T> modelList = tList.OrderBy(element);
                foreach (T t in modelList)
                    ts.Add(t);
                return ts;
            }

            return null;
        }

        #endregion

        #region  Instance Variables

        private static readonly Collection<BlendChemical> blendChemicals = CacheData.BlendChemicals;

        private static Collection<BlendAdditiveMeasureUnit> blendAdditiveMeasureUnits =
            CacheData.BlendAdditiveMeasureUnits;

        private static readonly int getCallSheetStartDayFromNow = -1000;
        private static IeServiceWebContext instance;


        public static IeServiceWebContext Instance
        {
            get
            {
                if (instance == null)
                    instance = new eServiceWebContext();
                return instance;
            }
        }

        #endregion

        #region  Instance Properties
        //This bulk plant job ids should be kept in cache
        public Collection<int> GetBulkPlantRigJobIds()
        {
                 Collection<int> bulkPlantRigJobIds = new Collection<int>(){66468, 66469, 66470, 66471, 66472, 66473, 66474, 66475};
                /*
                if (bulkPlantRigJobIds.Count==0)
                {
                    var bulkPlants = eServiceOnlineGateway.Instance.GetBulkPlants();
                    foreach (var bulkPlant in bulkPlants)
                    {
                        var bulkPlantRigJob = eServiceOnlineGateway.Instance.GetRigJobsByQuery(null, rigJob=>rigJob.Rig.Id == bulkPlant.Id).FirstOrDefault();
                        if(bulkPlantRigJob != null) bulkPlantRigJobIds.Add(bulkPlantRigJob.Id);
                    }
                }
                */

                return bulkPlantRigJobIds;
        }

        public Collection<ServicePoint> ServicePoints
        {
            get
            {
                return new Collection<ServicePoint>(eServiceOnlineGateway.Instance.GetServicePoints());
            }
        }

        public Collection<BlendChemical> BlendChemicals
        {
            get { return blendChemicals; }
        }

        public Collection<BlendChemical> BaseBlendChemicals
        {
            get
            {
                if ((this.BlendChemicals != null) && (this.BlendChemicals.Count > 0))
                {
                    Collection<BlendChemical> collection = CommonEntityBase.FindItems(this.BlendChemicals,
                        p => p.IsBaseEligible);
                    IOrderedEnumerable<BlendChemical> blendChemicals = collection.OrderBy(p => p.Name);
                    Collection<BlendChemical> blendChemicalList = new Collection<BlendChemical>();
                    foreach (BlendChemical entity in blendChemicals)
                        blendChemicalList.Add(entity);

                    return blendChemicalList;
                }

                return new Collection<BlendChemical>();
            }
        }

        #endregion

        #region Get ServicePoint Methods

        public List<DistrictModel> GetDistrictsWithAll()
        {
            List<DistrictModel> districtModels = this.GetDistricts();
            if (districtModels != null)
                districtModels.Insert(0, new DistrictModel { Id = 0, Name = "ALL" });
            return districtModels;
        }

        public List<DistrictModel> GetDistricts()
        {
            List<DistrictModel> districtModels = new List<DistrictModel>();

            if (this.ServicePoints != null)
                foreach (ServicePoint servicePoint in this.ServicePoints)
                {
                    DistrictModel districtModel = new DistrictModel();
                    districtModel.PopulateFrom(servicePoint);
                    districtModels.Add(districtModel);
                }
            List<DistrictModel> districtModelList = this.ListOrderBy(districtModels, p => p.Name);

            return districtModelList;
        }

        #endregion

        #region Get CallSheet Method


        public List<RigJobModel> GetRigJobsFromCallSheet(Collection<CallSheet> callSheets)
        {
            List<RigJobModel> rigJobViewModels = new List<RigJobModel>();

            foreach (CallSheet callSheet in callSheets)
            {
                RigJobModel rigJobmModel = new RigJobModel();
                rigJobmModel.PopulateFrom(callSheet);
                rigJobViewModels.Add(rigJobmModel);
            }
            return rigJobViewModels;
        }


        public RigJobModel GetRigJobFromCallSheet(CallSheet callSheet)
        {
            RigJobModel rigJobModel = new RigJobModel();
            if (callSheet != null)
                rigJobModel.PopulateFrom(callSheet);
            return rigJobModel;
        }


        public CallSheetHeader GetCallSheetHeaderByCallSheetNumber(int callSheetNumber)
        {
            DateTime startDateTime = DateTime.Now;
            ICallSheetService callSheetService =
                ServiceFactory.Instance.GetService(typeof(ICallSheetService)) as ICallSheetService;
            CallSheetHeader callSheetHeader = callSheetService.GetCallSheetHeaderByCallSheetNumber(callSheetNumber);
            Debug.WriteLine("GetCallSheetHeaderByCallSheetNumber - {0,21} S", DateTime.Now.Subtract(startDateTime));
            return callSheetHeader;
        }

        public CallSheet GetCallSheetByNumber(int callSheetNumber)
        {
            CallSheetHeader callSheetHeader = this.GetCallSheetHeaderByCallSheetNumber(callSheetNumber);
            if (callSheetHeader != null)
                return new CallSheet {Id = callSheetHeader.Id, Header = callSheetHeader};
            return null;
        }

        #endregion

        #region GetBlendSection Methods


        public List<BlendSectionModel> GetBlendSectionsFromCallSheetId(int callSheetId)
        {
            DateTime startDateTime = DateTime.Now;
            List<BlendSectionModel> blendSectionModels = new List<BlendSectionModel>();

            {
                var orgBlendSections = GetBlendSectionsByCallSheetId(callSheetId);

                if (orgBlendSections != null)
                {
                    IEnumerable<BlendSection> blendSections =
                        orgBlendSections.Where(p => BlendCalculator.IsAmountUnitTonsCategory(p.BlendCategory));

                    foreach (BlendSection blendSection in blendSections)
                    {
                        BlendSectionModel blendSectionModel = new BlendSectionModel();
                        blendSectionModel.PopulateFrom(blendSection);
                        blendSectionModel.CallSheetNumber = callSheetId;
                        blendSectionModels.Add(blendSectionModel);
                    }
                }
            }

            Debug.WriteLine("GetBlendSectionsFromCallSheetId - {0,21} S", DateTime.Now.Subtract(startDateTime));

            return blendSectionModels;
        }

        public List<BlendSection> GetBlendSectionsByCallSheetId(int callSheetId)
        {
            ICallSheetBlendSectionService callSheetBlendSectionService= MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ICallSheetBlendSectionService>();
            List<CallSheetBlendSection> callSheetBlendSections = null;
            if (callSheetBlendSectionService == null) return null;
            try
            {
                callSheetBlendSections =  callSheetBlendSectionService.SelectByJoin(p=>p.CallSheet.Id==callSheetId, blend=>blend.CallSheetBlendAdditiveSections != null);
            }
            catch (Exception ex)
            {
                ;
            }

            List<BlendSection> blendSections = new List<BlendSection>();
            if (callSheetBlendSections != null)
            {
                foreach (var callSheetBlendSection in callSheetBlendSections)
                {
                    BlendSection blendSection = (BlendSection)callSheetBlendSection;

                    if (blendSection.BlendAdditiveSections == null)
                        blendSection.BlendAdditiveSections = new List<BlendAdditiveSection>();

                    foreach (var callSheetBlendAdditiveSection in callSheetBlendSection.CallSheetBlendAdditiveSections)
                    {
                        blendSection.BlendAdditiveSections.Add((BlendAdditiveSection)callSheetBlendAdditiveSection);
                    }

                    blendSections.Add(blendSection);
                }
            }

            //            return blendSections==null? new List<BlendSection>() : blendSections.ConvertAll(x=>(BlendSection)x);
//            return callSheetBlendSections==null? new List<BlendSection>() : callSheetBlendSections.Cast<BlendSection>().ToList();

            return blendSections;
        }

        //Nov 1, 2023 AW P45_Q4_105: Change get blend section method to improve the performance
        public List<CallSheetBlendSection> GetBlendSectionsByCallSheetIdsAndBlendName(List<int> callSheetIds, string blendName)
        {
	        ICallSheetBlendSectionService callSheetBlendSectionService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ICallSheetBlendSectionService>();
	        List<CallSheetBlendSection> callSheetBlendSections = null;

	        callSheetBlendSections = callSheetBlendSectionService.SelectAll();
	        callSheetBlendSections = callSheetBlendSections.FindAll(p => callSheetIds.Contains(p.CallSheet.Id) && p.BlendFluidType.Name.Equals(blendName));
	        return callSheetBlendSections;
        }

        #endregion

        #region Create And Update ProductHaul Methods

        public BlendChemical GetBlendChemicalByBaseBlendChemicalId(int baseBlendChemicalId)
        {
            if (baseBlendChemicalId <= 0) throw new ArgumentOutOfRangeException(nameof(baseBlendChemicalId));

            BlendChemical baseBlendChemical = CommonEntityBase.FindItem(this.BaseBlendChemicals,
                p => p.Id == baseBlendChemicalId);
            BlendChemical blendChemical = MddTypeUtilities.FindTemplateByBase(this.BlendChemicals, baseBlendChemical);
            if (blendChemical == null) throw new Exception("blendChemical can not be null.");
            blendChemical.Description = baseBlendChemical.Name;

            return blendChemical;
        }

        public (BlendChemical, BlendCategory) GetBlendChemicalByBlendSection(BlendSection blendSection)
        {
            BlendChemical blendChemical = this.CovertToBlendChemicalFromBlendSection(blendSection);
            if (blendChemical == null) throw new Exception("blendChemical can not be null.");
            return (blendChemical, blendSection.BlendCategory);
        }

	    public void RescheduleBlendRequest(ProductLoadInfoModel model,bool? IsRigJobBin=false)
        {
            ProductHaulLoad originalProductHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(model.ProductHaulLoadId);
            if (originalProductHaulLoad == null) throw new Exception("product haul load id is not passed in or passed wrongly.");
            bool IsChangedAmount = false;
            if (model.IsTotalBlendTonnage != originalProductHaulLoad.IsTotalBlendTonnage || (model.IsTotalBlendTonnage && Math.Abs(model.Amount * 1000 - originalProductHaulLoad.TotalBlendWeight) > 0.1) || (!model.IsTotalBlendTonnage && Math.Abs(model.Amount * 1000 - originalProductHaulLoad.BaseBlendWeight) > 0.1))
            {
                originalProductHaulLoad.IsTotalBlendTonnage = model.IsTotalBlendTonnage;
                if (originalProductHaulLoad.IsTotalBlendTonnage)
                {
                    originalProductHaulLoad.TotalBlendWeight = model.Amount * 1000;
                }
                else
                {
                    originalProductHaulLoad.BaseBlendWeight = model.Amount * 1000;
                }
                IsChangedAmount = true;
            }



            if (Math.Abs(model.MixWater - originalProductHaulLoad.MixWater) > 0.001)
            {
                originalProductHaulLoad.MixWater = model.MixWater;
                IsChangedAmount = true;
            }

            originalProductHaulLoad.BulkPlant = eServiceOnlineGateway.Instance.GetRigById(model.BulkPlantId);
            if (model.BinInformationId == 0)
            {
                if (originalProductHaulLoad.Bin.Id != 0) 
					originalProductHaulLoad.Bin = new Bin();
                originalProductHaulLoad.PodIndex = 0;
            }
            else
            {
                var binInformation = eServiceOnlineGateway.Instance.GetBinInformationById(model.BinInformationId);
                if (binInformation == null) throw new Exception("Bin information doesn't exist.");
                if (originalProductHaulLoad.Bin.Id != binInformation.Bin.Id || originalProductHaulLoad.PodIndex != binInformation.PodIndex)
                {
                    originalProductHaulLoad.Bin = binInformation.Bin;
                    originalProductHaulLoad.PodIndex = binInformation.PodIndex;
                }
            }

            if(IsChangedAmount)
            {
                BlendSection blendSection = null;
                if (originalProductHaulLoad.CallSheetNumber > 0)
                {
                    blendSection = this.GetBlendSectionByBlendSectionId(originalProductHaulLoad.BlendSectionId);
                }
                else
                {
                    blendSection = this.GetProgramBlendSectionByBlendSectionId(originalProductHaulLoad.BlendSectionId);
                }
                BlendChemical blendChemical = null;
                BlendCategory blendCategory = null;
                (blendChemical, blendCategory) = this.GetBlendChemicalByBlendSection(blendSection);
                originalProductHaulLoad = this.GetCalculatedProductHaulLoad(blendChemical, originalProductHaulLoad,
                    originalProductHaulLoad.IsTotalBlendTonnage);
            }

            originalProductHaulLoad.ModifiedUserName = model.LoggedUser;
            originalProductHaulLoad.ExpectedOnLocationTime = model.ExpectedOnLocationTime;

            originalProductHaulLoad.IsBlendTest = model.IsBlendTest;
            originalProductHaulLoad.BlendTestingStatus = BlendTestingStatus.None;
            originalProductHaulLoad.Comments = model.Comments;
            //if(originalProductHaulLoad.IsBlendTest && originalProductHaulLoad.BlendTestingStatus==BlendTestingStatus.None)
            //{
            //    originalProductHaulLoad.BlendTestingStatus = BlendTestingStatus.Requested;
            //}

            originalProductHaulLoad.RemainsAmount = originalProductHaulLoad.TotalBlendWeight;
            if(IsRigJobBin.Value)
            {
                originalProductHaulLoad.RemainsAmount = 0;
            }
            originalProductHaulLoad.Description = ProductHaulProcess.BuildProductHaulLoadComments(originalProductHaulLoad);
            
            eServiceOnlineGateway.Instance.UpdateProductHaulLoad(originalProductHaulLoad, IsChangedAmount);

            if (IsChangedAmount && IsRigJobBin.Value)
            {
                List<int> productHaulLoadIds = new List<int>();
                productHaulLoadIds.Add(originalProductHaulLoad.Id);
                ShippingLoadSheet shippingLoadSheet = eServiceOnlineGateway.Instance.GetShippingLoadSheetByProductHaulLoads(productHaulLoadIds).FirstOrDefault();
                shippingLoadSheet = eServiceOnlineGateway.Instance.GetShippingLoadSheetById(shippingLoadSheet.Id,true);
                shippingLoadSheet.BulkPlant = originalProductHaulLoad.BulkPlant;
                shippingLoadSheet.LoadAmount = originalProductHaulLoad.TotalBlendWeight;
                //shippingLoadSheet.ProductHaulLoad = originalProductHaulLoad;
                eServiceOnlineGateway.Instance.UpdateShippingLoadSheet(shippingLoadSheet,false);
            }
           
            
        }

        public void CreateHaulBlend(int rigJobId, string loggedUser, ProductLoadInfoModel productLoadInfoModel, ProductHaulInfoModel productHaulInfoModel, ShippingLoadSheetModel shippingLoadSheetModel,bool? isProductHaul=false)
        {
            ProductHaulLoad originalProductHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productLoadInfoModel.ProductHaulLoadId);
            originalProductHaulLoad.RemainsAmount -= shippingLoadSheetModel.LoadAmount * 1000;
            if (isProductHaul.Value) { originalProductHaulLoad.RemainsAmount = 0; }
            if (originalProductHaulLoad.RemainsAmount > (0.1*1000))
            {
                originalProductHaulLoad.BlendShippingStatus = BlendShippingStatus.ParitialHaulScheduled;
            }
            else
            {
                originalProductHaulLoad.BlendShippingStatus = BlendShippingStatus.HaulScheduled;
            }
          
            eServiceOnlineGateway.Instance.UpdateProductHaulLoad(originalProductHaulLoad);

            BuildProductHaulDetails(loggedUser, productLoadInfoModel, productHaulInfoModel, shippingLoadSheetModel, originalProductHaulLoad, rigJobId);
        }

        //Dec 06, 2023 Tongtao 212_PR_UpdateRigNotesBinInformation: The blend in Bulk Plant bin can be hauled to a rig bin. ShippingLoadSheet will have the reference of blend from the last blend request information(LastProductHaulLoadId) of the bulk plant bin.
        public void HaulBlendFromBulkPlantBin(string loggedUser, ProductLoadInfoModel productLoadInfoModel, ProductHaulInfoModel productHaulInfoModel, ShippingLoadSheetModel shippingLoadSheetModel, BulkPlantBinLoadModel bulkPlantBinLoadModel)
        {
            var binInformation = eServiceOnlineGateway.Instance.GetBinInformationById(productLoadInfoModel.BinInformationId);

            ProductHaulLoad originalProductHaulLoad;
            if (binInformation.LastProductHaulLoadId == 0)
            {
	            originalProductHaulLoad = new ProductHaulLoad
	            {
		            BlendChemical = binInformation.BlendChemical,
		            CallSheetNumber = bulkPlantBinLoadModel.CallSheetNumber,
		            Customer = EServiceReferenceData.Data.ClientCompanyCollection.FirstOrDefault(p =>
			            p.Id == bulkPlantBinLoadModel.ClientId),
		            BlendTestingStatus = binInformation.BlendTestingStatus
	            };
            }
            else
	            originalProductHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(binInformation.LastProductHaulLoadId);

            BuildProductHaulDetails(loggedUser, productLoadInfoModel, productHaulInfoModel, shippingLoadSheetModel, originalProductHaulLoad);
        }

        private void BuildProductHaulDetails(string loggedUser, ProductLoadInfoModel productLoadInfoModel,
	        ProductHaulInfoModel productHaulInfoModel, ShippingLoadSheetModel shippingLoadSheetModel,
	        ProductHaulLoad originalProductHaulLoad,  int rigJobId = 0,HaulBackFromBinModel haulBackFromBinModel = null)
        {
            ProductHaul productHaul;
            SanjelCrew sanjelCrew = null;
            var callSheetNumber = productLoadInfoModel.CallSheetNumber;
            ThirdPartyBulkerCrew thirdPartyBulkerCrew = null;
            bool isThirdParty = productHaulInfoModel.IsThirdParty;
            var rigJob = rigJobId !=0? eServiceOnlineGateway.Instance.GetRigJobById(rigJobId):(callSheetNumber !=0? eServiceWebContext.instance.GetRigJobByCallsheetNumber(callSheetNumber):null);

            if (productHaulInfoModel.IsExistingHaul)
            {
                productHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulInfoModel.ProductHaulId);
            }
            else
            {
                productHaul = new ProductHaul() { ShippingLoadSheets = new List<ShippingLoadSheet>() };
                productHaulInfoModel.PopulateToProductHaul(productHaul);
                productHaul.ModifiedUserName = loggedUser;
                productHaul.ProductHaulLifeStatus = ProductHaulStatus.Scheduled;
                
                int bulkPlantId = productHaulInfoModel.BulkPlantId!=0? productHaulInfoModel.BulkPlantId : shippingLoadSheetModel.BulkPlantId;

                productHaul.BulkPlant = EServiceReferenceData.Data.RigCollection.FirstOrDefault(p => p.Id == bulkPlantId); 
                if (!productHaulInfoModel.IsThirdParty)
                {
                    sanjelCrew = eServiceOnlineGateway.Instance.GetCrewById(productHaulInfoModel.CrewId, true);
                    if (sanjelCrew == null) throw new Exception("SanjelCrew can not be null.");
                    productHaul.Crew = (Crew)sanjelCrew;
                    ProductHaulProcess.SetDriverAndBulk(productHaul, sanjelCrew);
                }
                else
                {
                    thirdPartyBulkerCrew =
                        eServiceOnlineGateway.Instance.GetThirdPartyBulkerCrewById(productHaulInfoModel.ThirdPartyBulkerCrewId);
                    productHaul.Crew = (Crew)thirdPartyBulkerCrew;
                    ProductHaulProcess.SetThirdPartyDriver(productHaul, thirdPartyBulkerCrew);
                }

                productHaul.ExpectedOnLocationTime = productHaulInfoModel.ExpectedOnLocationTime;
                productHaul.EstimatedTravelTime = productHaulInfoModel.EstimatedTravelTime;
                productHaul.Name = ProductHaulProcess.BuildProductHaulName(productHaul);
            }

            ShippingLoadSheet shippingLoadSheet = new ShippingLoadSheet();
            shippingLoadSheet.ModifiedUserName = loggedUser;
            shippingLoadSheet.ProductHaul = productHaul;
            shippingLoadSheet.ShippingStatus = ShippingStatus.Scheduled;
            shippingLoadSheet.BlendSectionId = productLoadInfoModel.BaseBlendSectionId;
            shippingLoadSheet.CallSheetNumber = rigJob?.CallSheetNumber??0;
            shippingLoadSheet.CallSheetId = rigJob?.CallSheetId??0;

            shippingLoadSheet.ClientName =string.IsNullOrEmpty(shippingLoadSheetModel.ClientName) ? rigJob?.ClientCompany.Name: shippingLoadSheetModel.ClientName;
            //Nov 6, 2023 AW P45_Q4_105: Prevent null rigjob caused exception
            shippingLoadSheet.ClientRepresentative = string.IsNullOrEmpty(shippingLoadSheetModel.ClientRepresentative) ? (rigJob!=null?Utility.GetClientRepresentative(rigJob):String.Empty): shippingLoadSheetModel.ClientRepresentative;
            shippingLoadSheet.IsGoWithCrew = shippingLoadSheetModel.IsGoWithCrew;
            //Nov 2, 2023 AW P45_Q4_105: Fix wrong value assigned

            shippingLoadSheet.ProgramId = productLoadInfoModel.ProgramNumber;
            shippingLoadSheet.LoadAmount = shippingLoadSheetModel.LoadAmount * 1000;
            shippingLoadSheet.ExpectedOnLocationTime = productHaulInfoModel.ExpectedOnLocationTime;
            // Jan 29, 2024 zhangyuan 195_PR_Haulback: Add haulback logical division
            if (haulBackFromBinModel != null && haulBackFromBinModel.SourceBinInformationId > 0)
            {
	            shippingLoadSheet.Name = haulBackFromBinModel.SourceBaseBlend;
	            shippingLoadSheet.BlendDescription = haulBackFromBinModel.SourceBlendChemicalDescription;
	            shippingLoadSheet.ProductHaulLoad = null;
	            shippingLoadSheet.BlendTestStatus = BlendTestingStatus.None;
                shippingLoadSheet.Rig = EServiceReferenceData.Data.RigCollection.FirstOrDefault(p => p.Id == shippingLoadSheetModel.RigId);
                shippingLoadSheet.BulkPlant =  EServiceReferenceData.Data.RigCollection.FirstOrDefault(p => p.Id == haulBackFromBinModel.sourceRigId);
                shippingLoadSheet.Destination = shippingLoadSheet.Rig.Name;
                shippingLoadSheet.SourceStorage = new BinInformation()
                {
                    Id = haulBackFromBinModel.SourceBinInformationId,
                    Name = haulBackFromBinModel.SourceBinInformationName
                };

                List<BlendUnloadSheet> blendUnLoadSheets = new List<BlendUnloadSheet>();

                if (haulBackFromBinModel.DestinationBinInformationId != 0)
                {
	                if (Math.Abs(shippingLoadSheetModel.LoadAmount) > 0.01)
	                {
		                var blendUnLoadSheet = new BlendUnloadSheet();
		                blendUnLoadSheet.UnloadAmount = shippingLoadSheetModel.LoadAmount * 1000;
		                blendUnLoadSheet.ModifiedUserName = loggedUser;
		                blendUnLoadSheet.ShippingLoadSheet = shippingLoadSheet;
		                blendUnLoadSheet.DestinationStorage =
			                eServiceOnlineGateway.Instance.GetBinInformationById(haulBackFromBinModel
				                .DestinationBinInformationId);
		                blendUnLoadSheet.Description = ProductHaulProcess.BuildBlendUnloadSheetComments(
			                blendUnLoadSheet, shippingLoadSheet.BlendDescription,
			                shippingLoadSheet.IsGoWithCrew,
			                shippingLoadSheet.ExpectedOnLocationTime);
		                blendUnLoadSheets.Add(blendUnLoadSheet);
	                }
                }

                shippingLoadSheet.BlendUnloadSheets = blendUnLoadSheets;

                productHaul.ShippingLoadSheets.Add(shippingLoadSheet);
                shippingLoadSheet.Description =
                    ProductHaulProcess.BuildShippingLoadSheetComments(shippingLoadSheet);

            }
            else
            {
	            shippingLoadSheet.Name = originalProductHaulLoad.BlendChemical.Name;
	            shippingLoadSheet.BlendDescription = originalProductHaulLoad.BlendChemical.Description;
	            shippingLoadSheet.ProductHaulLoad = originalProductHaulLoad;
	            shippingLoadSheet.BlendTestStatus = originalProductHaulLoad.BlendTestingStatus;
                shippingLoadSheet.BulkPlant = EServiceReferenceData.Data.RigCollection.FirstOrDefault(p => p.Id == shippingLoadSheetModel.BulkPlantId);
                shippingLoadSheet.Rig = EServiceReferenceData.Data.RigCollection.FirstOrDefault(p => p.Id == shippingLoadSheetModel.RigId);

                shippingLoadSheet.Destination = string.IsNullOrEmpty(shippingLoadSheetModel.RigName) ? rigJob?.WellLocation : shippingLoadSheetModel.RigName;
                shippingLoadSheet.SourceStorage = new BinInformation()
                    { Id = productLoadInfoModel.BinInformationId, Name = productLoadInfoModel.BinInformationName };


                if (productHaulInfoModel.IsGoWithCrew == false)
                {
                    List<BlendUnloadSheet> blendUnLoadSheets = new List<BlendUnloadSheet>();
                    if (shippingLoadSheetModel.BlendUnloadSheetModels != null && shippingLoadSheetModel.BlendUnloadSheetModels.Count > 0)
                    {
                        foreach (var blendUnloadSheetModel in shippingLoadSheetModel.BlendUnloadSheetModels)
                        {
                            if (Math.Abs(blendUnloadSheetModel.UnloadAmount) > 0.01)
                            {
                                var blendUnLoadSheet = new BlendUnloadSheet();
                                blendUnLoadSheet.UnloadAmount = blendUnloadSheetModel.UnloadAmount * 1000;
                                blendUnLoadSheet.ModifiedUserName = loggedUser;
                                blendUnLoadSheet.ShippingLoadSheet = shippingLoadSheet;
                                blendUnLoadSheet.DestinationStorage =
                                    eServiceOnlineGateway.Instance.GetBinInformationById(blendUnloadSheetModel
                                        .DestinationStorage.Id);
                                blendUnLoadSheet.Description = ProductHaulProcess.BuildBlendUnloadSheetComments(
                                    blendUnLoadSheet, shippingLoadSheet.BlendDescription,
                                    shippingLoadSheet.IsGoWithCrew,
                                    shippingLoadSheet.ExpectedOnLocationTime);
                                blendUnLoadSheets.Add(blendUnLoadSheet);
                            }
                        }
                    }

                    shippingLoadSheet.BlendUnloadSheets = blendUnLoadSheets;
                }

                shippingLoadSheet.Description = ProductHaulProcess.BuildShippingLoadSheetComments(shippingLoadSheet);
                productHaul.ShippingLoadSheets.Add(shippingLoadSheet);
            }
            productHaul.Description = ProductHaulProcess.BuildProductHaulDescription(productHaul);


            if (productHaulInfoModel.IsExistingHaul)
            {
                eServiceOnlineGateway.Instance.UpdateProductHaul(productHaul, false);
            }
            else
            {
	            eServiceOnlineGateway.Instance.CreateProductHaul(productHaul, false);
	            CrewProcess.AssignBulkerCrewToProductHaul(rigJob, productHaul, sanjelCrew, thirdPartyBulkerCrew, loggedUser, RigJobCrewSectionStatus.Scheduled);
            }
            shippingLoadSheet.OwnerId = productHaul.Id;
            eServiceOnlineGateway.Instance.CreateShippingLoadSheet(shippingLoadSheet);
            var podLoads = productHaulInfoModel.PodLoadModels;
            if (productHaulInfoModel.IsExistingHaul)
            {
                var originalPodLoads =
                    eServiceOnlineGateway.Instance.GetPodLoadsByProductHaul(productHaulInfoModel.ProductHaulId);
                foreach (var podLoad in originalPodLoads)
                {
                    if (podLoad.LoadAmount == 0)
                    {
                        var podLoadModel = podLoads.FirstOrDefault(p => p.PodIndex == podLoad.PodIndex);
                        if (podLoadModel.LoadAmount > 0)
                        {
                            podLoad.LoadAmount = podLoadModel.LoadAmount * 1000;
                            podLoad.ShippingLoadSheet = shippingLoadSheet;
                            eServiceOnlineGateway.Instance.UpdatePodLoad(podLoad);
                        }
                    }
                }
            }
            else
            {
                foreach (var podLoad in podLoads)
                {
                    podLoad.ModifiedUserName = loggedUser;
                    podLoad.LoadAmount = podLoad.LoadAmount * 1000;
                    podLoad.ProductHaul = productHaul;
                    if (podLoad.LoadAmount > 0)
                    {
                        podLoad.ShippingLoadSheet = shippingLoadSheet;
                    }
                    else
                    {
                        podLoad.ShippingLoadSheet = new ShippingLoadSheet();
                    }

                    eServiceOnlineGateway.Instance.CreatePodLoad(podLoad);
                }
            }

            CrewProcess.CreateOrUpdateThirdPartyUnitSections(productHaul);

        }


        private int GetBlendSectionId(int callSheetNumber, string blendDescription)
        {
            string sortedBlendDescription = SortBlendDescription(blendDescription);
            if (!string.IsNullOrEmpty(blendDescription))
            {
                var blendSections =
                    eServiceOnlineGateway.Instance.GetBlendSectionsByCallSheetAndBlend(callSheetNumber);
                if (blendSections == null || !blendSections.Any()) return 0;
                foreach (var callSheetBlendSection in blendSections)
                {
                    var callSheetBlendDescription = GetBlendDescription(callSheetBlendSection);
                    if (callSheetBlendDescription == sortedBlendDescription)
                    {
                        return callSheetBlendSection.Id;
                    }
                }
            }

            return 0;
        }

        private string SortBlendDescription(string blendDescription)
        {
            string sortedBlendDescription = string.Empty;
            string[] blendComponentes = blendDescription.Split('+');
            string baseBlend = blendComponentes[0].Trim();
            BlendSection blendSection = new BlendSection()
                { BlendFluidType = new BlendFluidType() { Name = baseBlend } };
            for (int i = 1; i < blendComponentes.Length; i++)
            {
                string additivestring = blendComponentes[i];
                var m = Regex.Match(additivestring, @"(?<Amount>[0-9\.]+)(?<UOM>%|Kg\/m3|l\/m3|%BWOW)\s(?<Name>[a-zA-Z\d\-\s]+)");
                var blendAdditiveSection = new BlendAdditiveSection()
                    { AdditiveType = new AdditiveType() { Name = m.Groups["Name"].Captures[0].Value.Trim(),}, AdditiveAmountUnit = new BlendAdditiveMeasureUnit(){Description = m.Groups["UOM"].Captures[0].Value.Trim()} , Amount = double.Parse(m.Groups["Amount"].Captures[0].Value.Trim())};

                blendSection.BlendAdditiveSections.Add(blendAdditiveSection);
            }

            sortedBlendDescription = GetBlendDescription(blendSection);

            return sortedBlendDescription;

        }

        private string GetBlendDescription(CallSheetBlendSection callSheetBlendSection)
        {
            string description = callSheetBlendSection.BlendFluidType.Name;
            if (callSheetBlendSection.CallSheetBlendAdditiveSections.Count == 1 &&
                callSheetBlendSection.CallSheetBlendAdditiveSections.First() == null) return description;
            List<CallSheetBlendAdditiveSection> blendAdditiveSections =
                callSheetBlendSection.CallSheetBlendAdditiveSections.OrderBy(p=>p.AdditiveType.Name).ToList();

            foreach (var callSheetBlendAdditiveSection in blendAdditiveSections)
            {
                 description += " + " + string.Format("{0:N2}", callSheetBlendAdditiveSection.Amount) + 
                                callSheetBlendAdditiveSection.AdditiveAmountUnit.Description + " " +
                                callSheetBlendAdditiveSection.AdditiveType.Name;
            }
            return description;
        }

        private string GetBlendDescription(BlendSection blendSection)
        {
            string description = blendSection.BlendFluidType.Name;
            List<BlendAdditiveSection> blendAdditiveSections =
                blendSection.BlendAdditiveSections.OrderBy(p=>p.AdditiveType.Name).ToList();

            foreach (var callSheetBlendAdditiveSection in blendAdditiveSections)
            {
                description += " + " + string.Format("{0:N2}", callSheetBlendAdditiveSection.Amount) + 
                               callSheetBlendAdditiveSection.AdditiveAmountUnit.Description + " " +
                               callSheetBlendAdditiveSection.AdditiveType.Name;
            }
            return description;
        }

        private bool CheckProductHaulCrewChange(ProductHaulInfoModel model, string userName,
	        ProductHaul originalProductHaul, RigJob rigJob)
        {
	        bool isCrewChange = false;
	        if (model.IsThirdParty != originalProductHaul.IsThirdParty ||
	            (!model.IsThirdParty && model.CrewId != originalProductHaul.Crew.Id) ||
	            (model.IsThirdParty && model.ThirdPartyBulkerCrewId != originalProductHaul.Crew.Id))
	        {
		        RigJobCrewSectionStatus assignmentStatus = ProductHaulProcess.ReleaseProductHaulCrew(originalProductHaul, userName);
                //Update original crew status
                originalProductHaul.IsThirdParty = model.IsThirdParty;
                originalProductHaul.Crew = new Crew()
	                { Id = originalProductHaul.IsThirdParty ? model.ThirdPartyBulkerCrewId : model.CrewId };
                CrewProcess.AssignBulkerCrewToProductHaul(rigJob, originalProductHaul, assignmentStatus, userName);
                isCrewChange = true;
	        }

	        return isCrewChange;
        }

        //TODO: This logic needs review
        private static bool  CheckProductHaulTimeChange(ProductHaulInfoModel model, ProductHaul originalProductHaul)
        {
	        bool isHeaderChanged = false;
	        if (model.IsGoWithCrew)
	        {
		        if (!originalProductHaul.IsGoWithCrew)
		        {
			        originalProductHaul.IsGoWithCrew = model.IsGoWithCrew;
			        originalProductHaul.ExpectedOnLocationTime =
				        ProductHaulProcess.UpdateShippingLoadSheetExpectedOnLocationTime(originalProductHaul,
					        model.ExpectedOnLocationTime, model.EstimatedLoadTime);
			        originalProductHaul.EstimatedLoadTime = model.EstimatedLoadTime;
                    //TODO: travel time needs to be synced with Pumper crew's travel time
			        originalProductHaul.EstimatedTravelTime = model.EstimatedTravelTime;
			        isHeaderChanged = true;
		        }
		        else
		        {
			        if (originalProductHaul.EstimatedLoadTime != model.EstimatedLoadTime)
			        {
				        originalProductHaul.EstimatedLoadTime = model.EstimatedLoadTime;
				        isHeaderChanged = true;
                    }
                }

            }
	        else
	        {
		        if (!originalProductHaul.IsGoWithCrew)
		        {
			        if (originalProductHaul.ExpectedOnLocationTime != model.ExpectedOnLocationTime ||
			            originalProductHaul.EstimatedLoadTime != model.EstimatedLoadTime ||
			            Math.Abs(originalProductHaul.EstimatedTravelTime - model.EstimatedTravelTime) > 0.1)
			        {
				        originalProductHaul.ExpectedOnLocationTime = model.ExpectedOnLocationTime;
				        originalProductHaul.EstimatedLoadTime = model.EstimatedLoadTime;
				        originalProductHaul.EstimatedTravelTime = model.EstimatedTravelTime;
				        isHeaderChanged = true;
			        }
		        }
                else
		        {
			        originalProductHaul.ExpectedOnLocationTime = model.ExpectedOnLocationTime;
			        originalProductHaul.EstimatedLoadTime = model.EstimatedLoadTime;
			        originalProductHaul.EstimatedTravelTime = model.EstimatedTravelTime;
                    //Jan 16, 2024 zhangyuan 266_PR_GoWithCrewMenu: Modify need record IsGoWithCrew in ProductHaul
                    originalProductHaul.IsGoWithCrew = model.IsGoWithCrew;
                    isHeaderChanged = true;
		        }
            }

	        return isHeaderChanged;
        }

        public void RescheduleProductHaul1(RescheduleProductHaulViewModel model, int rigJobId, string userName)
        {

	        //1. Get origianl product Haul data
	        ProductHaul originalProductHaul =
		        eServiceOnlineGateway.Instance.GetProductHaulById(model.ProductHaulInfoModel.ProductHaulId);
	        if (originalProductHaul == null) { return; }
	        originalProductHaul.ModifiedUserName = userName;
	        
	        //2. Delete Shipping Load Sheet if flag is off, checking from detail to header
            bool isShippingLoadSheetChanged = CheckHaulLoadChange(model, originalProductHaul);
            

            //3. Update Product Haul Header if changed
            //3.1 Check Time Changes
            isShippingLoadSheetChanged |= CheckProductHaulTimeChange(model.ProductHaulInfoModel, originalProductHaul);
            //3.2 Check Crew Changes
            //3.3 Sync description
            if (isShippingLoadSheetChanged)
            {
	            originalProductHaul.Description = ProductHaulProcess.BuildProductHaulDescription(originalProductHaul);
            }
            
            RigJob rigJob = null;

            if (rigJobId == 0)
            {
	            var callSheetNumber = 0;
	            if(model.PodLoadAndBendUnLoadModels != null && model.PodLoadAndBendUnLoadModels.Count > 0 && model.PodLoadAndBendUnLoadModels.FirstOrDefault() != null)
				            callSheetNumber = model.PodLoadAndBendUnLoadModels.FirstOrDefault().CallSheetNumber;
	            if (callSheetNumber != 0)
		            rigJob = eServiceOnlineGateway.Instance.GetRigJobByCallSheetNumber(callSheetNumber);
            }
            else
            {
	            rigJob = eServiceOnlineGateway.Instance.GetRigJobById(rigJobId);
            }    
	            
            bool isCrewChange = CheckProductHaulCrewChange(model.ProductHaulInfoModel, userName, originalProductHaul, rigJob);
            //4. Update kept shipping load sheet if changed
            //Feb 02, 2024 tongtao 264_PR_GoWithCrewError: save ExpectedOnLocationTime and EstimatedLoadTime when these changed
            if (isShippingLoadSheetChanged)
            {
	            foreach (var shippingLoadSheet in originalProductHaul.ShippingLoadSheets)
	            {
		            shippingLoadSheet.ProductHaul = originalProductHaul;

                    shippingLoadSheet.ExpectedOnLocationTime = originalProductHaul.ExpectedOnLocationTime;

                    shippingLoadSheet.EstimatedLoadTime = originalProductHaul.EstimatedLoadTime;

                    eServiceOnlineGateway.Instance.UpdateShippingLoadSheet(shippingLoadSheet, true);
	            }
            }
            //5. Align crew information and save
            if (isShippingLoadSheetChanged || isCrewChange)
            {
	            CrewProcess.CreateOrUpdateThirdPartyUnitSections(originalProductHaul);
            }

            eServiceOnlineGateway.Instance.UpdateProductHaul(originalProductHaul);

        }

        private bool CheckHaulLoadChange(RescheduleProductHaulViewModel model, ProductHaul originalProductHaul)
        {
	        bool isShippingLoadSheetChanged = false;
        
	        foreach (var podLoadAndBendUnLoadModel in model.PodLoadAndBendUnLoadModels)
	        {
		        if (podLoadAndBendUnLoadModel.IsCheckShippingLoadSheet == false)
		        {
                    //Remove uncheck shipping load sheet from product haul
                    RemoveShippingLoadSheetFromProductHaul(originalProductHaul,
	                    podLoadAndBendUnLoadModel.ShippingLoadSheetId);
                    isShippingLoadSheetChanged = true;
		        }
		        else
		        {
                    //check shipping Load Sheet change
                    isShippingLoadSheetChanged |= CheckShippingLoadSheetChange(podLoadAndBendUnLoadModel, originalProductHaul);
		        }
	        }

	        return isShippingLoadSheetChanged;
        }

        private bool CheckShippingLoadSheetChange(PodLoadAndBendUnLoadModel podLoadAndBendUnLoadModel, ProductHaul originalProductHaul)
        {
	        bool isShippingLoadSheetChanged = false;
	        var shippingLoadSheet =
		        originalProductHaul.ShippingLoadSheets.Find(p => p.Id == podLoadAndBendUnLoadModel.ShippingLoadSheetId);
	        shippingLoadSheet.ModifiedUserName = originalProductHaul.ModifiedUserName;

            //Jan 16, 2024 tongtao 267_PR_ChangeBinUnloadGetDescriptionWrong: Not get old BlendUnloadSheets info from Db,use data from web page.
            //shippingLoadSheet.BlendUnloadSheets =
            //    eServiceOnlineGateway.Instance.GetBlendUnloadSheetByshippingLoadSheetId(shippingLoadSheet.Id);
            if (Math.Abs(shippingLoadSheet.LoadAmount - podLoadAndBendUnLoadModel.LoadAmount * 1000) > 1)
	        {
		        shippingLoadSheet.LoadAmount = podLoadAndBendUnLoadModel.LoadAmount * 1000;
		        isShippingLoadSheetChanged = true;
	        }

	        if (shippingLoadSheet.EstimatedLoadTime != originalProductHaul.EstimatedLoadTime)
	        {
		        shippingLoadSheet.EstimatedLoadTime = originalProductHaul.EstimatedLoadTime;
		        isShippingLoadSheetChanged = true;
            }
	        if (shippingLoadSheet.ExpectedOnLocationTime != originalProductHaul.ExpectedOnLocationTime)
	        {
		        shippingLoadSheet.ExpectedOnLocationTime = originalProductHaul.ExpectedOnLocationTime;
		        isShippingLoadSheetChanged = true;
	        }
            if (shippingLoadSheet.IsGoWithCrew && !podLoadAndBendUnLoadModel.IsGoWithCrew)
	        {

                //Jan 5, 2024 tongtao 251_PR_RescheduleProductHaulVaildError: When rig has not bin for unload,BlendUnloadSheetModels info is null,therefore, it should be checked first before processing.
                if (podLoadAndBendUnLoadModel.BlendUnloadSheetModels != null)
                {
                    //Was GWC, not GWC now, add bin load information
                    foreach (var blendUnloadSheetModel in podLoadAndBendUnLoadModel.BlendUnloadSheetModels)
                    {
                        AddBlendUnloadSheetToShippingLoadSheet(blendUnloadSheetModel, shippingLoadSheet);
                    }

                    isShippingLoadSheetChanged = true;
                }
	        }
            else if (!shippingLoadSheet.IsGoWithCrew && podLoadAndBendUnLoadModel.IsGoWithCrew)
	        {
		        //Was GWC, not GWC now, remove bin load information
		        //shippingLoadSheet.BlendUnloadSheets.Clear();
		        isShippingLoadSheetChanged = true;
	        }
            else if (!shippingLoadSheet.IsGoWithCrew && !podLoadAndBendUnLoadModel.IsGoWithCrew)
	        {
                //Jan 5, 2024 tongtao 251_PR_RescheduleProductHaulVaildError: When rig has not bin for unload,BlendUnloadSheetModels info is null,therefore, it should be checked first before processing.
                if (podLoadAndBendUnLoadModel.BlendUnloadSheetModels != null)
                {
                    //Jan 16, 2024 tongtao 267_PR_ChangeBinUnloadGetDescriptionWrong: Clear BlendUnloadSheets When change BlendUnload Info
                    //shippingLoadSheet.BlendUnloadSheets.Clear();
                    //Was not GWC, not GWC now, check bin load information
                    foreach (var blendUnloadSheetModel in podLoadAndBendUnLoadModel.BlendUnloadSheetModels)
                    {
                        if (Math.Abs(blendUnloadSheetModel.UnloadAmount) > 0.1)
                        {
                            AddBlendUnloadSheetToShippingLoadSheet(blendUnloadSheetModel, shippingLoadSheet);
                            isShippingLoadSheetChanged = true;
                        }

                    }
                }
	        }

	        shippingLoadSheet.IsGoWithCrew = podLoadAndBendUnLoadModel.IsGoWithCrew;

	        if (shippingLoadSheet.Rig.Id != podLoadAndBendUnLoadModel.RigId)
	        {
		        shippingLoadSheet.Rig =
			        EServiceReferenceData.Data.RigCollection.FirstOrDefault(p => p.Id == podLoadAndBendUnLoadModel.RigId);
		        isShippingLoadSheetChanged = true;
	        }

	        if (shippingLoadSheet.CallSheetNumber != podLoadAndBendUnLoadModel.CallSheetNumber)
	        {
		        shippingLoadSheet.CallSheetNumber = podLoadAndBendUnLoadModel.CallSheetNumber;
		        shippingLoadSheet.CallSheetId = podLoadAndBendUnLoadModel.CallSheetId;
		        isShippingLoadSheetChanged = true;
	        }

	        if (isShippingLoadSheetChanged)
                shippingLoadSheet.Description = ProductHaulProcess.BuildShippingLoadSheetComments(shippingLoadSheet);
            //if (shippingLoadSheet.ProductHaulLoad == null || shippingLoadSheet.ProductHaulLoad.Id == 0)
            //{
            //    shippingLoadSheet.Description = ProductHaulProcess.BuildHaulBackShippingLoadSheetComments(shippingLoadSheet);
            //}
            //else
            //{
            //    shippingLoadSheet.Description = ProductHaulProcess.BuildShippingLoadSheetComments(shippingLoadSheet);
            //}

            foreach (var modelPodLoad in podLoadAndBendUnLoadModel.PodLoadModels)
	        {
                //Only update own page
                if (modelPodLoad.ShippingLoadSheet.Id == -1 || (modelPodLoad.ShippingLoadSheet.Id !=0 && modelPodLoad.ShippingLoadSheet.Id != shippingLoadSheet.Id)) continue;

		        modelPodLoad.LoadAmount = modelPodLoad.LoadAmount * 1000;
		        var origPodLoad = originalProductHaul.PodLoad.Find(p => p.PodIndex == modelPodLoad.PodIndex);

		        if (Math.Abs(modelPodLoad.LoadAmount) < 0.001)
		        {
			        origPodLoad.LoadAmount = 0;
			        origPodLoad.ModifiedUserName = shippingLoadSheet.ModifiedUserName;
			        origPodLoad.ShippingLoadSheet = new ShippingLoadSheet();
		        }
		        else
		        {
			        origPodLoad.LoadAmount = modelPodLoad.LoadAmount;
			        origPodLoad.ShippingLoadSheet = shippingLoadSheet;
			        origPodLoad.ModifiedUserName = shippingLoadSheet.ModifiedUserName;
		        }
	        }

            return isShippingLoadSheetChanged;
        }

        private static void AddBlendUnloadSheetToShippingLoadSheet(BlendUnloadSheet blendUnloadSheetModel,
	        ShippingLoadSheet shippingLoadSheet)
        {
	        if (Math.Abs(blendUnloadSheetModel.UnloadAmount) > 0.001)
	        {
		        BlendUnloadSheet blendUnloadSheet = new BlendUnloadSheet();
		        blendUnloadSheet.ModifiedUserName = shippingLoadSheet.ModifiedUserName;
		        blendUnloadSheet.UnloadAmount = blendUnloadSheetModel.UnloadAmount * 1000;
		        blendUnloadSheet.DestinationStorage =
			        eServiceOnlineGateway.Instance.GetBinInformationById(blendUnloadSheetModel
				        .DestinationStorage.Id);
		        blendUnloadSheet.ShippingLoadSheet = shippingLoadSheet;
                blendUnloadSheet.Description = ProductHaulProcess.BuildBlendUnloadSheetComments(
	                blendUnloadSheet, shippingLoadSheet.BlendDescription,
	                shippingLoadSheet.IsGoWithCrew,
	                shippingLoadSheet.ExpectedOnLocationTime);
                shippingLoadSheet.BlendUnloadSheets.Add(blendUnloadSheet);
	        }
        }

        private void RemoveShippingLoadSheetFromProductHaul(ProductHaul originalProductHaul, int shippingLoadSheetId)
        {
            //Empty associated pod loads
	        foreach (var podLoad in originalProductHaul.PodLoad)
	        {
		        if(podLoad.ShippingLoadSheet.Id == shippingLoadSheetId)
                    EmptyPod(podLoad);
	        }
	        originalProductHaul.ShippingLoadSheets.RemoveAll(p => p.Id == shippingLoadSheetId);
	        var shippingLoadSheet = eServiceOnlineGateway.Instance.GetShippingLoadSheetById(shippingLoadSheetId, true);
            //BlendUnload Sheets can be removed with shippingLoadSheet together
            eServiceOnlineGateway.Instance.DeleteShippingLoadSheet(shippingLoadSheet);
        }

        private static void EmptyPod(PodLoad podLoad)
        {
	        podLoad.ShippingLoadSheet = new ShippingLoadSheet();
	        podLoad.BaseTonnage = 0.0;
	        podLoad.BaseTonnageUnit = new BlendAdditiveMeasureUnit();
	        podLoad.LoadAmount = 0.0;
	        podLoad.LoadAmountUnit = new BlendAdditiveMeasureUnit();
        }

        public void ReschedulePodLoadAndBlendUnLoad(List<PodLoadAndBendUnLoadModel> podLoadAndBendUnLoadModels, int productHaulId, string userName)
        {
            var productHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);
            //productHaul.ShippingLoadSheets = new List<ShippingLoadSheet>();
            bool isChangeProductHaul = false;
            var originalRidId = 0;
            foreach (var podLoadAndBendUnLoadModel in podLoadAndBendUnLoadModels)
            {
//                var shippingLoadSheet = eServiceOnlineGateway.Instance.GetShippingLoadSheetById(podLoadAndBendUnLoadModel.ShippingLoadSheetId, true);
                var shippingLoadSheet =
                    productHaul.ShippingLoadSheets.Find(p => p.Id == podLoadAndBendUnLoadModel.ShippingLoadSheetId);
                originalRidId = shippingLoadSheet.Rig?.Id ?? 0;
                if (podLoadAndBendUnLoadModel.IsCheckShippingLoadSheet == false)
                {
//                    var podLoads=eServiceOnlineGateway.Instance.GetPodLoadsByProductHaul(shippingLoadSheet.ProductHaul.Id);
                    var podLoads = productHaul.PodLoad;
                    foreach(var podLoad in podLoads)
                    {
                        if(podLoad.ShippingLoadSheet.Id==shippingLoadSheet.Id)
                        {
                            podLoad.ShippingLoadSheet = new ShippingLoadSheet();
                            podLoad.LoadAmount = 0;
                            eServiceOnlineGateway.Instance.UpdatePodLoad(podLoad);
                        }
                    }
                    eServiceOnlineGateway.Instance.DeleteShippingLoadSheet(shippingLoadSheet);
                    isChangeProductHaul = true;
                    continue;
                }
//                productHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);
                podLoadAndBendUnLoadModel.LoadAmount = podLoadAndBendUnLoadModel.LoadAmount * 1000;
                bool isChangedShippingLoadSheet = false;
                if(shippingLoadSheet.IsGoWithCrew!=podLoadAndBendUnLoadModel.IsGoWithCrew)
                {
                    shippingLoadSheet.IsGoWithCrew = podLoadAndBendUnLoadModel.IsGoWithCrew;
                    isChangedShippingLoadSheet = true;
                }
                if(shippingLoadSheet.LoadAmount!=podLoadAndBendUnLoadModel.LoadAmount)
                {
                    shippingLoadSheet.LoadAmount=podLoadAndBendUnLoadModel.LoadAmount;
                    isChangedShippingLoadSheet = true;
                }
                //Dec 7, 2023 zhangyuan 224_PR_ShowCallSheetList: Add Logic Save CallSheet And Rig
//select CallsheetId 
                    if (shippingLoadSheet.CallSheetNumber != podLoadAndBendUnLoadModel.CallSheetNumber)
                    {
                        if (podLoadAndBendUnLoadModel.CallSheetNumber > 0)
                        {
                            RigJob job = eServiceWebContext.instance.GetRigJobByCallsheetNumber(
                                podLoadAndBendUnLoadModel
                                    .CallSheetNumber);
                            shippingLoadSheet.CallSheetId = job?.CallSheetId ?? 0;
                            shippingLoadSheet.CallSheetNumber = job?.CallSheetNumber ?? 0;
                        }
                        else
                        {
                            shippingLoadSheet.CallSheetId = 0;
                            shippingLoadSheet.CallSheetNumber = 0;
                        }

                        isChangedShippingLoadSheet = true;
                    }

                    if (podLoadAndBendUnLoadModel.RigId > 0)
                    {
                        shippingLoadSheet.Rig = new Rig()
                            { Id = podLoadAndBendUnLoadModel.RigId, Name = podLoadAndBendUnLoadModel.RigName };
                        isChangedShippingLoadSheet = true;
                    }

                    shippingLoadSheet.BlendUnloadSheets = eServiceOnlineGateway.Instance.GetBlendUnloadSheetByQuery(p =>
                        p.ShippingLoadSheet.Id == shippingLoadSheet.Id);

                if (podLoadAndBendUnLoadModel.IsGoWithCrew == false)
                {
                    if (shippingLoadSheet.ExpectedOnLocationTime != productHaul.ExpectedOnLocationTime)
                    {
                        shippingLoadSheet.ExpectedOnLocationTime = productHaul.ExpectedOnLocationTime;
                        isChangedShippingLoadSheet = true;
                    }

                    if (shippingLoadSheet.BlendUnloadSheets == null)
                    {
                        shippingLoadSheet.BlendUnloadSheets = new List<BlendUnloadSheet>();
                    }
                    else
                    {
                        //change different rig delete need DestinationStorage
                        if (originalRidId!=podLoadAndBendUnLoadModel.RigId && podLoadAndBendUnLoadModel.RigId>0)
                        {
                            shippingLoadSheet.BlendUnloadSheets.Clear();
                            isChangedShippingLoadSheet = true;
                        }
                    }
                    if (podLoadAndBendUnLoadModel.BlendUnloadSheetModels?.Count > 0)
                    {
                        foreach (var blendLoadSheet in podLoadAndBendUnLoadModel.BlendUnloadSheetModels)
                        {
                            blendLoadSheet.UnloadAmount = blendLoadSheet.UnloadAmount * 1000;
                            if(blendLoadSheet.UnloadAmount>0)
                            {
                                if (blendLoadSheet.Id == 0)
                                {
                                    blendLoadSheet.ShippingLoadSheet = shippingLoadSheet;
                                    blendLoadSheet.ModifiedUserName = userName;
                                    //Dec 7, 2023 zhangyuan 224_PR_ShowCallSheetList: if change rig, add miss field 
                                    blendLoadSheet.DestinationStorage =
                                        eServiceOnlineGateway.Instance.GetBinInformationById(blendLoadSheet
                                            .DestinationStorage.Id);
                                    blendLoadSheet.Description = ProductHaulProcess.BuildBlendUnloadSheetComments(
                                        blendLoadSheet, shippingLoadSheet.BlendDescription,
                                        shippingLoadSheet.IsGoWithCrew,
                                        shippingLoadSheet.ExpectedOnLocationTime);

                                    shippingLoadSheet.BlendUnloadSheets.Add(blendLoadSheet);
                                    isChangedShippingLoadSheet = true;
                                }
                                else
                                {
                                    var origBlendUnLoadSheet = shippingLoadSheet.BlendUnloadSheets.FirstOrDefault(p => p.Id == blendLoadSheet.Id);
                                    if (origBlendUnLoadSheet.ShippingLoadSheet.Id == shippingLoadSheet.Id && Math.Abs( origBlendUnLoadSheet.UnloadAmount-blendLoadSheet.UnloadAmount)>0.1)
                                    {
                                        origBlendUnLoadSheet.ModifiedUserName = userName;
                                        origBlendUnLoadSheet.UnloadAmount = blendLoadSheet.UnloadAmount; ;
                                        isChangedShippingLoadSheet = true;
                                    }
                                    //Dec 11, 2023 zhangyuan 224_PR_ShowCallSheetList: change Logic RigId need test
                                    if (origBlendUnLoadSheet.ShippingLoadSheet.Id == shippingLoadSheet.Id && origBlendUnLoadSheet.DestinationStorage.Id != blendLoadSheet.DestinationStorage.Id)
                                    {
                                        origBlendUnLoadSheet.ModifiedUserName = userName;
                                        origBlendUnLoadSheet.DestinationStorage = eServiceOnlineGateway.Instance.GetBinInformationById(blendLoadSheet
                                            .DestinationStorage.Id); ;
                                        isChangedShippingLoadSheet = true;
                                    }
                                }
                            }
                            else
                            {
                                if(blendLoadSheet.Id>0)
                                {
	                                var currentOffloadSheet =
		                                shippingLoadSheet.BlendUnloadSheets.Find(p => p.Id == blendLoadSheet.Id);
	                                shippingLoadSheet.BlendUnloadSheets.Remove(currentOffloadSheet);
                                    isChangedShippingLoadSheet = true;
                                }
                            }
                            
                        }
                    }
                    //Dec 11, 2023 zhangyuan 224_PR_ShowCallSheetList: Remove Logic BlendUnloadSheets
                    else
                    {
                        for(int i=0;i< shippingLoadSheet.BlendUnloadSheets.Count();i++)
                        {
                            shippingLoadSheet.BlendUnloadSheets.Remove(shippingLoadSheet.BlendUnloadSheets[i]);
                            isChangedShippingLoadSheet = true;
                        }
                    }

                }
                else
                {
                    if (shippingLoadSheet.BlendUnloadSheets!=null&&shippingLoadSheet.BlendUnloadSheets.Count > 0)
                    {
                        shippingLoadSheet.BlendUnloadSheets = new List<BlendUnloadSheet>();
                        isChangedShippingLoadSheet = true;
                    }

                }
                if(isChangedShippingLoadSheet)
                {
                    shippingLoadSheet.ModifiedUserName = userName;
                    eServiceOnlineGateway.Instance.UpdateShippingLoadSheet(shippingLoadSheet);
//                    shippingLoadSheet = eServiceOnlineGateway.Instance.GetShippingLoadSheetById(shippingLoadSheet.Id,true);
                }
                foreach(var podLoad in podLoadAndBendUnLoadModel.PodLoadModels)
                {
                    podLoad.LoadAmount = podLoad.LoadAmount * 1000;
                    var origPodLoad = productHaul.PodLoad.FirstOrDefault(p => p.Id == podLoad.Id);

                    if (origPodLoad.LoadAmount != 0 && podLoad.LoadAmount == 0 &&
                              origPodLoad.ShippingLoadSheet.Id == podLoadAndBendUnLoadModel.ShippingLoadSheetId)
                    {
                        origPodLoad.LoadAmount = podLoad.LoadAmount;
                        origPodLoad.ModifiedUserName = userName;
                        origPodLoad.ShippingLoadSheet = new ShippingLoadSheet();
                        isChangeProductHaul = true;
                    }
                    else if (origPodLoad.LoadAmount == 0 && podLoad.LoadAmount != 0)
                    {
                        origPodLoad.LoadAmount = podLoad.LoadAmount;
                        origPodLoad.ShippingLoadSheet = shippingLoadSheet;
                        origPodLoad.ModifiedUserName = userName;
                        isChangeProductHaul = true;
                    }
                    else if (origPodLoad.LoadAmount != podLoad.LoadAmount &&
                             origPodLoad.ShippingLoadSheet.Id == podLoadAndBendUnLoadModel.ShippingLoadSheetId)
                    {
                        origPodLoad.LoadAmount = podLoad.LoadAmount;
                        origPodLoad.ModifiedUserName = userName;
                        isChangeProductHaul = true;
                    }
                    
                }
                
            }
            if(isChangeProductHaul)
            {
//                productHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);
                productHaul.ModifiedUserName = userName;
                eServiceOnlineGateway.Instance.UpdateProductHaul(productHaul);
//                productHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaul.Id);
                if(productHaul.ShippingLoadSheets==null||productHaul.ShippingLoadSheets.Count==0)
                {
                    if (productHaul.IsThirdParty)
                    {
                        RigJobThirdPartyBulkerCrewSection bulkerCrewSection = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionByProductHaul(productHaul.Id);
                        ThirdPartyBulkerCrewSchedule thirdPartyBulkerCrewSchedule = eServiceOnlineGateway.Instance.GetThirdPartyCrewScheduleByRigJobThirdPartyBulkerCrewSection(bulkerCrewSection);
                        eServiceOnlineGateway.Instance.DeletethirdPartyBulkerCrewSchedule(thirdPartyBulkerCrewSchedule.Id);
                        eServiceOnlineGateway.Instance.DeleteRigJobThirdPartyBulkerCrewSection(bulkerCrewSection.Id);
                    }
                    else
                    {
                        RigJobSanjelCrewSection sanjelCrewSection = eServiceOnlineGateway.Instance.GetRigJobCrewSectionsByProductHaul(productHaul.Id);
                        SanjelCrewSchedule sanjelCrewSchedule = eServiceOnlineGateway.Instance.GetSanjelCrewSchedulesByRigJobSanjelCrewSection(sanjelCrewSection);
                        eServiceOnlineGateway.Instance.DeleteCrewSchedule(sanjelCrewSchedule.Id, true);
                        eServiceOnlineGateway.Instance.DeleteRigJobCrewSection(sanjelCrewSection);
                    }
                    eServiceOnlineGateway.Instance.DeleteProductHaul(productHaul);
                }
            }
        }
        public List<ProductHaulLoad> GetProductHaulLoadsByShippingLoadSheets(List<ShippingLoadSheet> shippingLoadSheets)
        {
            List<int> productHaulLoadIds = shippingLoadSheets.Select(p => p.ProductHaulLoad.Id).Distinct().ToList();
            return eServiceOnlineGateway.Instance.GetProductHaulLoadByIds(productHaulLoadIds);
        }


        public ProductLoadInfoModel CreateBlendRequest(ProductLoadInfoModel productLoadInfoModel)
        {

            var productHaulLoad = BuildProductHaulLoadFromProductLoadInfoModel(productLoadInfoModel);

         

            productHaulLoad.ModifiedUserName = productLoadInfoModel.LoggedUser;
//            productLoadInfoModel.ProductHaulDescription = RigBoardProcess.BuildProductHaulInfo(productHaulLoad);

            //1. Create Product Haul Load
//            productHaulLoad.ProductHaulLoadLifeStatus = ProductHaulLoadStatus.Scheduled;
            productHaulLoad.RemainsAmount = productHaulLoad.TotalBlendWeight;
            
            
            productHaulLoad.ProductHaul = null;
            productHaulLoad.Description = ProductHaulProcess.BuildProductHaulLoadComments(productHaulLoad);
            //if (productHaulLoad.IsBlendTest) { productHaulLoad.BlendTestingStatus = BlendTestingStatus.Requested; }
            productHaulLoad.BlendTestingStatus = BlendTestingStatus.None;
            RigBoardProcess.CreateProductHaulLoad(productHaulLoad);

            
            //2. Update eService unit section
            if (productLoadInfoModel.CallSheetNumber > 0)
            {
                CallSheetHeader callSheetHeader = this.GetCallSheetHeaderByCallSheetNumber(productLoadInfoModel.CallSheetNumber);
                productHaulLoad.CallSheetId = callSheetHeader.Id;
            }

            productLoadInfoModel.ProductHaulLoadId = productHaulLoad.Id;


            return productLoadInfoModel;
        }
        private ProductHaulLoad CreateProductHaulLoad(ProductHaulLoad productHaulLoad)
        {
            eServiceOnlineGateway.Instance.CreateProductHaulLoad(productHaulLoad);
            CreateProductLoadSections(productHaulLoad);
            return productHaulLoad;
        }




        private ProductHaulLoad BuildProductHaulLoadFromProductLoadInfoModel(ProductLoadInfoModel productHaulModel)
        {
            BlendSection blendSection;
            //1. BuildProductHaulLoad
            ProductHaulLoad productHaulLoad = new ProductHaulLoad();
            productHaulModel.PopulateToHaulLoad(productHaulLoad);


            if (productHaulModel.BinInformationId != 0)
            {
//                var binInformation = eServiceOnlineGateway.Instance.GetBinInformationById(productHaulModel.BinInformationId);
                var binInformation = CacheData.BinInformations.First(p=>p.Id == productHaulModel.BinInformationId);
                productHaulLoad.Bin = binInformation.Bin;
                productHaulLoad.PodIndex = binInformation.PodIndex;
            }

            productHaulLoad.BulkPlant = CacheData.Rigs.First(p => p.Id == productHaulModel.BulkPlantId);
            productHaulLoad.Rig = CacheData.Rigs.First(p => p.Id == productHaulModel.RigId);
            productHaulLoad.DispatchBy = productHaulModel.LoggedUser;

            if (productHaulModel.CallSheetNumber > 0)
            {
                RigJob rigJob =
                    eServiceOnlineGateway.Instance.GetRigJobByCallSheetNumber(productHaulModel.CallSheetNumber);
//                CallSheetHeader callSheetHeader = this.GetCallSheetHeaderByCallSheetNumber(productHaulModel.CallSheetNumber);
//                CopyFromCallSheetHeader(productHaulLoad, callSheetHeader);
                    CopyFromRigJob(productHaulLoad, rigJob);
                blendSection = this.GetBlendSectionByBlendSectionId(productHaulModel.BaseBlendSectionId);
            }
            else
            {
                blendSection = this.GetProgramBlendSectionByBlendSectionId(productHaulLoad.BlendSectionId);

                //Nov 7, 2023 tongtao P45_Q4_167: When the ProgramNumber contains a revision value, extract the programId from it for use, otherwise, use the ProgramNumber directly.

                var program = eServiceOnlineGateway.Instance.GetJobDesignByProgramIdAndRevision(productHaulLoad.ProgramId, productHaulLoad.ProgramVersion);
                productHaulLoad.ServicePoint =
                    EServiceReferenceData.Data.ServicePointCollection.FirstOrDefault(p =>
                        p.Id == productHaulModel.ServicePointId);
                productHaulLoad.Customer =
                    EServiceReferenceData.Data.ClientCompanyCollection.FirstOrDefault(p => p.Id==productHaulModel.CustomerId);
                productHaulLoad.WellLocation = program.WellLocation;

                productHaulLoad.JobType = EServiceReferenceData.Data.JobTypeCollection.FirstOrDefault(p => p.Id == productHaulModel.JobTypeId);

                productHaulLoad.JobDate = DateTime.Now;
            }

            if (blendSection == null) throw new Exception("blendSection can not be null.");
            productHaulLoad.ProductHaulLoadLifeStatus = ProductHaulLoadStatus.Scheduled;

            //4. build product load section
            BlendChemical blendChemical = null;
            (blendChemical, productHaulLoad.BlendCategory) = this.GetBlendChemicalByBlendSection(blendSection);

            productHaulLoad = this.GetCalculatedProductHaulLoad(blendChemical, productHaulLoad,
                productHaulModel.IsTotalBlendTonnage);

            return productHaulLoad;
        }
        private ProductHaulLoad BuildProductHaulLoad(ProductHaulModel productHaulModel)
        {
            BlendSection blendSection;
            //1. BuildProductHaulLoad
            ProductHaulLoad productHaulLoad = new ProductHaulLoad();
            productHaulModel.PopulateToHaulLoad(productHaulLoad);

            if (productHaulModel.BinInformationId != 0)
            {
                var binInformation = eServiceOnlineGateway.Instance.GetBinInformationById(productHaulModel.BinInformationId);
                productHaulLoad.Bin = binInformation.Bin;
                productHaulLoad.PodIndex = binInformation.PodIndex;
            }

            productHaulLoad.BulkPlant = eServiceOnlineGateway.Instance.GetRigById(productHaulModel.BulkPlantId);
            productHaulLoad.Rig = eServiceOnlineGateway.Instance.GetRigById(productHaulModel.RigId);
            productHaulLoad.DispatchBy = productHaulModel.LoggedUser;

            if (productHaulModel.CallSheetNumber > 0)
            {
                CallSheetHeader callSheetHeader = this.GetCallSheetHeaderByCallSheetNumber(productHaulModel.CallSheetNumber);
                CopyFromCallSheetHeader(productHaulLoad, callSheetHeader);
                blendSection = this.GetBlendSectionByBlendSectionId(productHaulModel.BaseBlendSectionId);
            }
            else
            {
                blendSection = this.GetProgramBlendSectionByBlendSectionId(productHaulLoad.BlendSectionId);
                var program = eServiceOnlineGateway.Instance.GetProgramByProgramId(productHaulModel.ProgramNumber);
                productHaulLoad.ServicePoint = CacheData.ServicePointCollections.FirstOrDefault(p=>p.Id==program.JobData.ServicePoint.Id);
                productHaulLoad.Customer = CacheData.AllClientCompanies.FirstOrDefault(p => p.Id == productHaulModel.CustomerId);
                productHaulLoad.WellLocation = program.WellLocationInformation.WellLocation;
                productHaulLoad.JobType= CacheData.JobTypes.FirstOrDefault(p=>p.Id == productHaulModel.JobTypeId);

                productHaulLoad.JobDate = DateTime.Now;
            }

            if (blendSection == null) throw new Exception("blendSection can not be null.");
            productHaulLoad.ProductHaulLoadLifeStatus = ProductHaulLoadStatus.Scheduled;

            //4. build product load section
            BlendChemical blendChemical = null;
            (blendChemical, productHaulLoad.BlendCategory) = this.GetBlendChemicalByBlendSection(blendSection);

            productHaulLoad = this.GetCalculatedProductHaulLoad(blendChemical, productHaulLoad,
                productHaulModel.IsTotalBlendTonnage);

            return productHaulLoad;
        }



        public BlendChemical GetBlendChemicalByBlendSectionId(int blendSectionId, int callSheetNumber)
        {
            var blendSection = callSheetNumber!=0 ? this.GetBlendSectionByBlendSectionId(blendSectionId): this.GetProgramBlendSectionByBlendSectionId(blendSectionId);

            (BlendChemical blendChemical, var blendCategory) = this.GetBlendChemicalByBlendSection(blendSection);

            return blendChemical;
        }
        private DateTime GetScheduleEndTime(ProductHaulModel productHaulModel)
        {
            DateTime endTime;
            if (productHaulModel.ProductHaulInfoModel.IsGoWithCrew)
            {
                endTime = this.GetRigJobAssginCrewEndTime(productHaulModel.RigJobId);
                if (endTime == DateTime.MinValue)
                {
                    productHaulModel.ProductHaulInfoModel.EstimatedTravelTime = 8;
                    endTime = productHaulModel.ProductHaulInfoModel.EstimatedLoadTime.AddHours(8);
                }
                else
                {
                    productHaulModel.ProductHaulInfoModel.EstimatedTravelTime = (endTime - productHaulModel.ProductHaulInfoModel.EstimatedLoadTime).TotalHours;
                }
            }
            else
            {
                endTime = productHaulModel.ProductHaulInfoModel.EstimatedLoadTime.AddHours(productHaulModel.ProductHaulInfoModel.EstimatedTravelTime);
            }

            return endTime;
        }

        private DateTime GetScheduleEndTime(int rigJobId,ProductHaulInfoModel model)
        {
            DateTime endTime;
            if (model.IsGoWithCrew)
            {
                endTime = this.GetRigJobAssginCrewEndTime(rigJobId);
                if (endTime == DateTime.MinValue)
                {
                    model.EstimatedTravelTime = 8;
                    endTime = model.EstimatedLoadTime.AddHours(8);
                }
                else
                {
                    model.EstimatedTravelTime = (endTime - model.EstimatedLoadTime).TotalHours;
                }
            }
            else
            {
                endTime = model.EstimatedLoadTime.AddHours(model.EstimatedTravelTime);
            }
            return endTime;
        }
        private void CreateProductLoadSections(ProductHaulLoad productHaulLoad)
        {
            if (productHaulLoad.AllProductLoadList.Count!=0)
            {
                foreach (var productLoadSection in productHaulLoad.AllProductLoadList)
                {
                    productLoadSection.ModifiedUserName = productHaulLoad.ModifiedUserName;
                    productLoadSection.ProductHaulLoad = productHaulLoad;
                    eServiceOnlineGateway.Instance.CreateProductLoadSection(productLoadSection);
                }
            }
        }

        private Collection<ProductLoadSection> GetProductLoadSectionCollection(
            Collection<ProductLoadSection> allProductLoadSection, Collection<BlendChemicalSection> blendChemicalSections,
            bool isFromBase)
        {
            if (blendChemicalSections == null) return null;

            foreach (BlendChemicalSection blendChemicalSection in blendChemicalSections)
            {
                ProductLoadSection productLoadSection = new ProductLoadSection
                {
                    BlendChemical = blendChemicalSection.BlendChemical,
                    RequiredAmount = blendChemicalSection.Amount,
                    IsFromBase = isFromBase,
                    AdditiveBlendMethod = blendChemicalSection.AdditiveBlendMethod,
                    BlendAdditiveMeasureUnit = blendChemicalSection.Unit
                };

                allProductLoadSection.Add(productLoadSection);
            }
            return allProductLoadSection;
        }

        #endregion

        #region Get ProductHaul Methods

        public List<ProductHaulModel> GetProductHaulCollectionByCallSheetNumber(int callSheetNumber)
        {
            List<ProductHaulModel> productHaulModels = new List<ProductHaulModel>();
            // this code maybe delete 2019/03/22
            //            productHaulModels = this.GetProducthaulModelsByProducthaulCollection(productHaulService.GetProductHaulCollectionByCallSheetNumber(callSheetNumber));
            return productHaulModels;
        }

        private List<ProductHaulModel> GetProducthaulModelsByProducthaulCollection(
            Collection<ProductHaul> productHaulCollection)
        {
            List<ProductHaulModel> productHaulModels = new List<ProductHaulModel>();
            if (productHaulCollection != null)
                foreach (ProductHaul entity in productHaulCollection)
                {
                    ProductHaulModel productHaulModel = new ProductHaulModel();
                    productHaulModel.PopulateFrom(entity);
                    productHaulModels.Add(productHaulModel);
                }

            return productHaulModels;
        }

        public Collection<ProductHaulModel> GetProductHaulModelCollection()
        {
            Collection<ProductHaulModel> productHaulModels = new Collection<ProductHaulModel>();
            List<ProductHaul> productHauls = eServiceOnlineGateway.Instance.GetProductHauls();
            if (productHauls != null)
                foreach (ProductHaul entity in productHauls)
                {
                    ProductHaulModel productHaulModel = new ProductHaulModel();
                    productHaulModel.PopulateFrom(entity);
                    productHaulModels.Add(productHaulModel);
                }

            return productHaulModels;
        }

        public ProductHaul GetProductHaulById(int productHaulId)
        {
            ProductHaul productHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);

            return productHaul;
        }
        public List<ProductHaul> GetProductHaulCollectionByProductHaulIds(Collection<int> productHaulIds)
        {
            if (productHaulIds.Count > 0)
            {
                List<ProductHaul> productHauls =
                    eServiceOnlineGateway.Instance.GetProductHaulLoadsByProductHaulIds(productHaulIds);

                return productHauls;
            }
            else
            {
                return null;
            }
        }



        #endregion

        #region Search By ServicePoint Methods


        /*
        public void UpdateProductHaulAndHaulLoads(ProductHaul productHaul, bool isGoWithCrew,
            DateTime expectedOnLocationTime, int rigJobId,int crewId, bool originalHaulIsThirdParty)
        {
            RigBoardProcess.UpdateProductHaulAndHaulLoads(productHaul, isGoWithCrew, expectedOnLocationTime, rigJobId,crewId, originalHaulIsThirdParty);
        }
        */

        public ProductHaul GetProductHaulInfoById(int id)
        {
            ProductHaul productHaul = eServiceOnlineGateway.Instance.GetProductHaulById(id);
            if (productHaul == null) return null;
            if (productHaul.IsThirdParty) { }
            else
            {
                RigJobSanjelCrewSection sanjelCrewSection =
                    eServiceOnlineGateway.Instance.GetRigJobCrewSectionsByProductHaul(productHaul.Id);
                if (sanjelCrewSection != null)
                {
                    SanjelCrew sanjelCrew = eServiceOnlineGateway.Instance.GetCrewById(sanjelCrewSection.SanjelCrew.Id , true);
                    productHaul.Driver = sanjelCrew.SanjelCrewWorkerSection.FirstOrDefault()?.Worker;
                    List<SanjelCrewTruckUnitSection> unitSections =
                        eServiceOnlineGateway.Instance.GetTruckUnitSectionsByCrew(sanjelCrew.Id);
                    if (unitSections != null)
                    {
                        foreach (var sanjelCrewTruckUnitSection in unitSections)
                        {
                            TruckUnit unit = GetTruckUnitById(sanjelCrewTruckUnitSection.TruckUnit.Id);
                            if (unit == null) { throw new Exception("TruckUnit can not be null."); }
                            if (unit.UnitSubType.Id == 15 || unit.UnitSubType.Id == 101)
                            {
                                productHaul.BulkUnit = unit;
                            }
                            else if (unit.UnitSubType.Id == 276)
                            {
                                productHaul.TractorUnit = unit;
                            }
                        }
                    }

                    List<SanjelCrewWorkerSection> workerSections = GetCrewWorkerSections(sanjelCrew.Id);
                    if(workerSections != null)
                    {
                        productHaul.Driver = workerSections.FirstOrDefault()?.Worker;
                    }
                }
            }

            return productHaul;
        }

        public WellLocationInformation GetWellLocationInfoByCallSheetNumber(int callSheetNumber)
        {
            ICallSheetMicroService callSheetMicroService =
                ServiceFactory.Instance.GetService(typeof(ICallSheetMicroService)) as ICallSheetMicroService;
            if (callSheetMicroService == null) throw new Exception("callSheetMicroService can not be null.");
            CallSheet callSheet = callSheetMicroService.GetCallSheetByCallSheetNumber(callSheetNumber);
            WellLocationInformation wellLocationInfo = callSheet?.Header?.HeaderDetails?.WellLocationInformation;

            return wellLocationInfo;
        }

        //Validate callsheet to inprogress or ready status,example
        public void UpdateWellLocationInfoByCallSheet(int rigJobId, int callSheetNumber,
            WellLocationInformation wellLocationInformation)
        {
            ICallSheetMicroService callSheetMicroService =
                ServiceFactory.Instance.GetService(typeof(ICallSheetMicroService)) as ICallSheetMicroService;
            ICallSheetService callSheetService =
                ServiceFactory.Instance.GetService(typeof(ICallSheetService)) as ICallSheetService;
            if (callSheetMicroService == null) throw new Exception("callSheetMicroService can not be null.");
            if (callSheetService == null) throw new Exception("callSheetService can not be null.");

            CallSheet oldcallSheet = callSheetService.GetCallSheetByCallSheetNumber(callSheetNumber);
            if (oldcallSheet?.Header?.HeaderDetails?.WellLocationInformation != null)
            {
                CallSheet newcallSheet = oldcallSheet;
                newcallSheet.Header.HeaderDetails.WellLocationInformation = wellLocationInformation;

                callSheetMicroService.UpdateCallSheet(newcallSheet, oldcallSheet);
                this.UpateRigJobByWellInfo(rigJobId, wellLocationInformation);
            }
        }

        public void UpateRigJobByWellInfo(int rigJobId, WellLocationInformation wellLocationInformation)
        {
            RigJob originalVersion = eServiceOnlineGateway.Instance.GetRigJobById(rigJobId);

            if (originalVersion != null)
            {
                RigJob currentVersion = originalVersion;

                if ((wellLocationInformation.HasDownHoleWellLocation != null) &&
                    wellLocationInformation.HasDownHoleWellLocation.Value)
                    currentVersion.WellLocation = wellLocationInformation.DownHoleWellLocation;
                else
                    currentVersion.SurfaceLocation = wellLocationInformation.WellLocation;
                currentVersion.Directions = wellLocationInformation.DirectionToLocation;

                RigBoardProcess.UpdateRigJob(currentVersion);
            }
        }

        public bool AdjustJobDuration(int rigJobId, int estJobDuration)
        {
            var rigJob = eServiceWebContext.Instance.GetRigJobById(rigJobId);
            rigJob.JobDuration = estJobDuration * 60;
            eServiceOnlineGateway.Instance.UpdateRigJob(rigJob);

            UpdateRigJobCrewSchedule(rigJob);

            return true;
        }

        public void ReleaseCrew(int rigJobId, bool isCompleteJob, DateTime jobCompleteTime)
        {
            RigBoardProcess.ReleaseCrew(rigJobId, isCompleteJob, jobCompleteTime);
            if(!isCompleteJob)
                AlignPumperCrewAssignment(rigJobId);

        }

        /*
        public void UpdateCrewScheduleByRigJob(RigJob rigJob)
        {
            RigBoardProcess.UpdateCrewScheduleByRigJob(rigJob);
        }
        */

        public SanjelCrewSchedule GetCrewScheduleByJobCrewSection(int jobCrewSetionId)

        {
            return RigBoardProcess.GetCrewScheduleByJobCrewSection(jobCrewSetionId);
        }

        public List<SanjelCrewWorkerSection> GetCrewWorkerSections(int id)
        {
            return RigBoardProcess.GetCrewWorkerSections(id);
        }


        public Collection<BinSection> GetBinSectionCollctionByRootId(int rootId)
        {
            IBinSectionMicroService binSectionMicroService = ServiceFactory.Instance.GetService(typeof(IBinSectionMicroService)) as IBinSectionMicroService;
            if (binSectionMicroService == null) throw new Exception("binSectionMicroService can not be null.");
            Collection<BinSection> binSections = binSectionMicroService.GetBinSectionCollctionByRootId(rootId);

            return binSections;
        }

        public CallSheet GetCallSheetByIdForBin(int id)
        {
            ICallSheetMicroService callSheetMicroService = ServiceFactory.Instance.GetService(typeof(ICallSheetMicroService)) as ICallSheetMicroService;
            if (callSheetMicroService == null) throw new Exception("callSheetMicroService can not be null.");
            CallSheet callSheet = callSheetMicroService.GetCallSheetById(id);

            return callSheet;
        }

        public CallSheet GetCallSheetByNumberForRigJob(int callSheetNumber)
        {
            ICallSheetMicroService callSheetMicroService =
                ServiceFactory.Instance.GetService(typeof(ICallSheetMicroService)) as ICallSheetMicroService;
            if (callSheetMicroService == null) throw new Exception("callSheetMicroService can not be null.");
            CallSheet callSheet = callSheetMicroService.GetCallSheetByCallSheetNumber(callSheetNumber);

            return callSheet;
        }

        public CallSheet UpdateCallSheet(CallSheet currentVersion, CallSheet originalVersion = null, bool isMicroService = true)
        {
            CallSheet callSheet = null;
            if (isMicroService)
            {
                ICallSheetMicroService callSheetMicroService =
                    ServiceFactory.Instance.GetService(typeof(ICallSheetMicroService)) as ICallSheetMicroService;
                if (callSheetMicroService == null) throw new Exception("callSheetMicroService can not be null.");
                callSheet = callSheetMicroService.UpdateCallSheet(currentVersion, originalVersion);
            }
            else
            {
                throw new Exception("Never used logic.");
                //June 17 - Never used
                /*
                ICallSheetService callSheetService =
                    ServiceFactory.Instance.GetService(typeof(ICallSheetService)) as ICallSheetService;
                if (callSheetService == null) throw new Exception("callSheetService can not be null.");
                callSheet = callSheetService.UpdateCallSheet(currentVersion, originalVersion);
*/
            }


            return callSheet;
        }

        #endregion

        #region Create And Update Rig Methods

        public ClientCompany GetClientCompanyById(int id)
        {
            return eServiceOnlineGateway.Instance.GetClientCompanyById(id);
        }

        public Bin GetBinById(int binId)
        {
          

            return eServiceOnlineGateway.Instance.GetBinById(binId);
        }

        public void CreateRig(Rig rig)
        {
            eServiceOnlineGateway.Instance.CreateRig(rig);
        }

        public Rig GetRigInfoByRigId(int id)
        {
            Rig rig = eServiceOnlineGateway.Instance.GetRigById(id);

            return rig;
        }

        public void UpdateRigInfo(Rig rig)
        {
            RigBoardProcess.UpdateRigInfo(rig);
        }

        public bool UpdateRigStatus(int rigId, RigStatus newStatus)
        {
            return RigBoardProcess.UpdateRigStatus(rigId, newStatus);
        }


        #endregion

        #region Create And Update Consultants

        public List<ShiftType> GetWorkShiftInfo()
        {
            return eServiceOnlineGateway.Instance.GetWorkShiftInfo();
        }

        public ShiftType GetWorkShiftById(int id)
        {
            return eServiceOnlineGateway.Instance.GetWorkShiftById(id);
        }

        public void CreateConsultant(ClientConsultant clientConsultant)
        {
            eServiceOnlineGateway.Instance.CreateConsultant(clientConsultant);
        }

        public FirstCall GetConsultantContactsByCallSheetNumber(int callSheetNumber)
        {
            ICallSheetMicroService callSheetMicroService = ServiceFactory.Instance.GetService(typeof(ICallSheetMicroService)) as ICallSheetMicroService;
            if (callSheetMicroService == null) throw new Exception("callSheetMicroService can not be null.");
            CallSheet callSheet = callSheetMicroService.GetCallSheetByCallSheetNumber(callSheetNumber);
            FirstCall consultantContacts = callSheet?.Header?.HeaderDetails?.FirstCall;

            return consultantContacts;
        }

        public ClientConsultant GetConsultantById(int id)
        {
            return eServiceOnlineGateway.Instance.GetConsultantById(id);
        }

        public Collection<ClientConsultant> GetClientConsultantCollection()
        {
            return eServiceOnlineGateway.Instance.GetClientConsultantCollection();
        }

        public int DeleteClientConsultant(ClientConsultant clientConsultant)
        {
            return eServiceOnlineGateway.Instance.DeleteClientConsultant(clientConsultant);
        }

        public void UpdateConsultantInfo(int callSheetNumber, FirstCall consultantContacts)
        {
            ICallSheetMicroService callSheetMicroService = ServiceFactory.Instance.GetService(typeof(ICallSheetMicroService)) as ICallSheetMicroService;
            if (callSheetMicroService == null) throw new Exception("callSheetMicroService can not be null.");

            CallSheet oldcallSheet = callSheetMicroService.GetCallSheetByCallSheetNumber(callSheetNumber);
            if (oldcallSheet?.Header?.HeaderDetails?.FirstCall != null)
            {
                CallSheet newcallSheet = oldcallSheet;
                newcallSheet.Header.HeaderDetails.FirstCall = consultantContacts;
                callSheetMicroService.UpdateCallSheet(newcallSheet, oldcallSheet);
            }
        }

        public void UpdateClientConsultant(bool isUpdateRigJob, ClientConsultant clientConsultant)
        {
            RigBoardProcess.UpdateClientConsultant(isUpdateRigJob, clientConsultant);
            EServiceReferenceData.Data.ClientConsultantCollection = eServiceOnlineGateway.Instance.GetClientConsultantCollection();
        }

        public RigJob CreateRigJobByCallSheet(int callsheetNumber)
        {
            ICallSheetMicroService callSheetMicroService = ServiceFactory.Instance.GetService(typeof(ICallSheetMicroService)) as ICallSheetMicroService;
            if (callSheetMicroService == null) throw new Exception("ICallSheetMicroService can not be null.");

            CallSheet item = callSheetMicroService.GetCallSheetByCallSheetNumber(callsheetNumber);
            RigJob rigJob = new RigJob {CallSheetNumber = callsheetNumber, JobLifeStatus = this.GetRigJobStatus(item)};

            return RigBoardProcess.CreateRigJob(rigJob);
        }

        public Collection<BlendSection> GetBlendSectionCollectionByRootIdIsCallSheetId(int rootId)
        {
            return new Collection<BlendSection>(eServiceWebContext.instance.GetBlendSectionsByCallSheetId(rootId).OrderBy(p=>p.Id).ToList()); 
        }

        #endregion

        #region Update Notes

        public RigJob GetRigJobById(int id)
        {
            RigJob rigJob = eServiceOnlineGateway.Instance.GetRigJobById(id);

            return rigJob;
        }

        public RigJob UpdateRigJob(RigJob currentVersion, RigJob originalVersion)
        {
            return RigBoardProcess.UpdateRigJob(currentVersion);
        }

        #endregion

        #region Refactor new implement

        public CallSheet GetCallSheetById(int id, bool isMicroService = false)
        {
            CallSheet callSheet = null;
            if (!isMicroService)
            {
                ICallSheetService callSheetService =
                    ServiceFactory.Instance.GetService(typeof(ICallSheetService)) as ICallSheetService;
                if (callSheetService == null) throw new Exception("callSheetService can not be null.");
                callSheet = callSheetService.GetCallSheetById(id);
            }
            else
            {
                ICallSheetMicroService callSheetMicroService =
                    ServiceFactory.Instance.GetService(typeof(ICallSheetMicroService)) as ICallSheetMicroService;
                if (callSheetMicroService == null) throw new Exception("callSheetMicroService can not be null.");
                callSheet = callSheetMicroService.GetCallSheetById(id);
            }


            return callSheet;
        }

        public CallSheet UpdateMicroCallSheet(CallSheet currentVersion, CallSheet originalVersion)
        {
            ICallSheetMicroService callSheetMicroService =
                ServiceFactory.Instance.GetService(typeof(ICallSheetMicroService)) as ICallSheetMicroService;
            if (callSheetMicroService == null) throw new Exception("callSheetMicroService can not be null.");
            CallSheet callSheet = callSheetMicroService.UpdateCallSheet(currentVersion, originalVersion);

            return callSheet;
        }

        public RigJob GetMicroRigJobByCallSheetNumber(int callSheetNumber)
        {
            RigJob rigJob = eServiceOnlineGateway.Instance.GetRigJobByCallSheetNumber(callSheetNumber);

            return rigJob;
        }


        public RigJob UpdateMicroRigJob(RigJob currentVersion, RigJob originalVersion)
        {
            return RigBoardProcess.UpdateRigJob(currentVersion);
        }

        public RigJob CreateMicroRigJob(RigJob rigJob)
        {
            RigJob createRigJob = RigBoardProcess.CreateRigJob(rigJob);

            return createRigJob;
        }

        public Collection<JobPackage> GetJobPackageByCallSheetNumber(int callSheetNumber, bool isMicroService = true)
        {
            Collection<JobPackage> jobPackages = null;
            if (!isMicroService)
            {
                IJobService jobService = ServiceFactory.Instance.GetService(typeof(IJobService)) as IJobService;
                if (jobService == null) throw new Exception("jobService must be registered in service factory.");

                jobPackages = jobService.GetJobPackageByCallSheetNumber(callSheetNumber);
            }
            return jobPackages;
        }

        public Sanjel.BusinessEntities.Sections.Common.BlendSection GetBlendSectionById(int id)
        {
            IBlendSectionMicroService blendSectionMicroService =
                ServiceFactory.Instance.GetService(typeof(IBlendSectionMicroService)) as IBlendSectionMicroService;
            if (blendSectionMicroService == null) throw new Exception("blendSectionMicroService can not be null.");
            Sanjel.BusinessEntities.Sections.Common.BlendSection blendSection = blendSectionMicroService.GetBlendSectionById(id);

            return blendSection;
        }
        public BinSection CreateBinSection(BinSection binSection)
        {
            IBinSectionMicroService binSectionMicroService = ServiceFactory.Instance.GetService(typeof(IBinSectionMicroService)) as IBinSectionMicroService;
            if (binSectionMicroService == null) throw new Exception("binSectionMicroService can not be null.");
            BinSection createBlendSection = binSectionMicroService.CreateBinSection(binSection);

            return createBlendSection;
        }

        public Sanjel.BusinessEntities.Sections.Common.BlendSection UpdateBlendSection(Sanjel.BusinessEntities.Sections.Common.BlendSection newBlendSection, Sanjel.BusinessEntities.Sections.Common.BlendSection oldBlendSection)
        {
            IBlendSectionMicroService blendSectionMicroService =
                ServiceFactory.Instance.GetService(typeof(IBlendSectionMicroService)) as IBlendSectionMicroService;
            if (blendSectionMicroService == null) throw new Exception("blendSectionMicroService can not be null.");
            Sanjel.BusinessEntities.Sections.Common.BlendSection blendSection = blendSectionMicroService.UpdateBlendSection(newBlendSection, newBlendSection);

            return blendSection;
        }

        public bool ChangeRigJobStatusToComplete(int id, DateTime jobCompleteTime)
        {
            eServiceWebContext.Instance.ReleaseCrew(id, true, jobCompleteTime);

            return RigBoardProcess.UpdateRigJobStatusToComplete(id);
        }

        public bool ChangeRigJobStatusToCancel(int id, string notes)
        {
            return RigBoardProcess.UpdateRigJobStatusToCancel(id, notes);
        }

        /*
        public void CreateOrUpdateUnitSectionsByProductHaul(ProductHaul productHaul, int crewId)
        {
            RigBoardProcess.CreateOrUpdateUnitSectionsByProductHaul(productHaul, crewId);
        }
        */

        public BinInformation CreateBinInformation(BinInformation binAssignment)
        {
            return eServiceOnlineGateway.Instance.CreateBinInformation(binAssignment);
        }

        public void AssignBinToRig(int binId, int rigJobId,Collection<BinInformation> binInformationList)
        {
            Bin bin = this.GetBinById(binId);
            RigJob rigJob = this.GetRigJobById(rigJobId);
            Rig rig = eServiceOnlineGateway.Instance.GetRigById(rigJob.Rig.Id);
            ServicePoint workingServicePoint = eServiceOnlineGateway.Instance.GetServicePointById(rigJob.ServicePoint.Id);
            if (rig != null)
            {

//                BinInformation rigBinSection = new BinInformation
//                {
//                    Bin = bin,
//                    Rig = rig,
//                    BinStatus = BinStatus.Assigned,
//                    WorkingServicePoint = workingServicePoint
//                };
//                this.CreateBinInformation(rigBinSection);
               BinBoardProcess.UpdateBinInformationFormAssignBin(bin,rig,workingServicePoint, binInformationList);
                if (rigJob != null)
                {
                    if ((rigJob.JobLifeStatus==JobLifeStatus.Pending
                         || rigJob.JobLifeStatus==JobLifeStatus.Confirmed
                         || rigJob.JobLifeStatus==JobLifeStatus.Scheduled
                         || rigJob.JobLifeStatus==JobLifeStatus.Dispatched)
                        && !rigJob.CallSheetId.Equals(0))
                    {
                        BinSection binSection = new BinSection();
                        binSection.Bin = MddTypeToOldType.ConvertBin(bin);
                        binSection.RootId = rigJob.CallSheetId;
                        //04/09/2019 AW: Who said assign a bin to a job, it is on location right away?
                        //                        binSection.OnLocation = DateTime.Now;
                        this.CreateBinSection(binSection);
                    }
                }
            }
        }
        public List<BinInformation> GetBinInformationCollectionByRig(Rig rig)
        {
            return RigBoardProcess.GetRigBinSectionCollectionByRig(rig);
        }

        public bool UnassignBinToRig(int binId, int callSheetId, int rigJobId)
        {
            return RigBoardProcess.ReleaseBinFromRig(binId, callSheetId, rigJobId);
        }

        public User GetSecuredUserByApplicationAndUserName(string applicationName, string userName)
        {
            ISecurityDataMicroService securityService = ServiceFactory.Instance.GetService(typeof(ISecurityDataMicroService)) as ISecurityDataMicroService;
            if (securityService == null) throw new Exception("securityService can not be null.");
            User user = securityService.GetUserSecurityDataByUsernameAndApplication(applicationName, userName);

            return user;
        }

        public AccessRecord GetAccessRecordByUserId(int userId)
        {
            IAccessRecordMicroService accessRecordMicroService = ServiceFactory.Instance.GetService(typeof(IAccessRecordMicroService)) as IAccessRecordMicroService;
            if (accessRecordMicroService == null) throw new Exception("accessRecordMicroService can not be null.");
            AccessRecord accessRecord = accessRecordMicroService.GetAccessRecordByUserId(userId);

            return accessRecord;
        }

        public AccessRecord CreateAccessRecord(AccessRecord accessRecord)
        {
            IAccessRecordMicroService accessRecordMicroService = ServiceFactory.Instance.GetService(typeof(IAccessRecordMicroService)) as IAccessRecordMicroService;
            if (accessRecordMicroService == null) throw new Exception("accessRecordMicroService can not be null.");
            AccessRecord createaccessRecord = accessRecordMicroService.CreateAccessRecord(accessRecord);

            return createaccessRecord;
        }

        public AccessRecord UpdateAccessRecord(AccessRecord currentVersion, AccessRecord originalVersion)
        {
            IAccessRecordMicroService accessRecordMicroService = ServiceFactory.Instance.GetService(typeof(IAccessRecordMicroService)) as IAccessRecordMicroService;
            if (accessRecordMicroService == null) throw new Exception("accessRecordMicroService can not be null.");
            AccessRecord accessRecord = accessRecordMicroService.UpdateAccessRecord(currentVersion, originalVersion);

            return accessRecord;
        }

        public List<Rig> GetDeactivateRigByDrillingCompanyId(int drillingCompanyId)
        {
            return RigBoardProcess.GetDeactivateRigByDrillingCompanyId(drillingCompanyId);
        }

        public List<Rig> GetRigByDrillingCompanyId(int drillingCompanyId)
        {
            return RigBoardProcess.GetRigByDrillingCompanyId(drillingCompanyId);
        }

        public List<RigJob> GetRigJobCollectionForOperation()
        {
            return eServiceOnlineGateway.Instance.GetRigJobs().Where(p => p.CallSheetNumber > 0).ToList();
        }

        #endregion

        #region Retrieve RigJob Data

        public List<RigJob> GetAllRigJobInformation(int pageNumber, int pageSize, Collection<int> servicePointIds, Collection<int> rigTypes, Collection<int> jobLifeStatuses, bool isShowJobAlert, bool isShowFutureJobs, out int count)
        {
            return RigBoardProcess.GetAllRigJobInformation(pageNumber, pageSize, servicePointIds, rigTypes, jobLifeStatuses, this.GetBulkPlantRigJobIds(), isShowJobAlert, isShowFutureJobs, out count);
        }
        public List<RigJob> GetBulkPlantRigJobInformation(Collection<int> rigJobIds,Collection<int> servicePointIds)
        {
            return BulkPlantProcess.GetBulkPlantRigJobInformation(rigJobIds ,servicePointIds);
        }
        private JobLifeStatus GetRigJobStatus(CallSheet callSheet)
        {
            if (callSheet != null)
            {
                if (callSheet.Header.HeaderDetails.FirstCall.IsExpectedTimeOnLocation && (callSheet.Status == EServiceEntityStatus.InProgress))
                    return JobLifeStatus.Pending;
                if (!callSheet.Header.HeaderDetails.FirstCall.IsExpectedTimeOnLocation && (callSheet.Status == EServiceEntityStatus.InProgress))
                    return JobLifeStatus.Confirmed;
                if (!callSheet.Header.HeaderDetails.FirstCall.IsExpectedTimeOnLocation && (callSheet.Status == EServiceEntityStatus.Ready) && callSheet.Header.HeaderDetails.CallInformation.IsThisCallMade)
                    return JobLifeStatus.Dispatched;
                if (!callSheet.Header.HeaderDetails.FirstCall.IsExpectedTimeOnLocation && (callSheet.Status == EServiceEntityStatus.Ready))
                    return JobLifeStatus.Scheduled;
                Collection<JobPackage> jobPackages = this.GetJobPackageByCallSheetNumber(callSheet.CallSheetNumber);
                if ((callSheet.Status == EServiceEntityStatus.Locked) && (jobPackages != null) && (jobPackages.Count > 0))
                    return JobLifeStatus.Completed;
                if (callSheet.Status == EServiceEntityStatus.Locked)
                    return JobLifeStatus.InProgress;

                return JobLifeStatus.Alerted;
            }

            return JobLifeStatus.Alerted;
        }

        public Collection<JobPackage> GetJobPackageByCallSheetNumber(int callSheetNumber)
        {
            IJobService jobService = ServiceFactory.Instance.GetService(typeof(IJobService)) as IJobService;
            if (jobService == null) throw new Exception("jobService must be registered in service factory.");

            Collection<JobPackage> jobPackages = jobService.GetJobPackageByCallSheetNumber(callSheetNumber);

            return jobPackages;
        }

        public List<RigJob> GetUpComingJobsInformation(int pageNumber, int pageSize, Collection<int> servicePointIds, int windowStart, int windowEnd, out int count)
        {
            IUpcomingJobsProcess businessProcess = new UpcomingJobsProcess();

            return businessProcess.GetUpComingJobsInformation(pageNumber, pageSize, servicePointIds, windowStart, windowEnd, out count);
        }

        /*
        public void CreateOrUpdateThirdPartyUnitSections(ProductHaul producthaul,int crewId)
        {
            CrewProcess.CreateOrUpdateThirdPartyUnitSections(producthaul);
        }
        */

        public Rig GetRigByName(string rigName)
        {
            return eServiceOnlineGateway.Instance.GetRigByName(rigName);
        }

        /*
        public void UpdateProductHaulAndLoadsOnLocation(int productHaulId, DateTime onLocationTime, string loggedUser)
        {
            RigBoardProcess.SetProductHaulAndLoadsOnLocation(productHaulId, onLocationTime, loggedUser);
        }
        */
        public void UpdateProductHaulOnLocation(OnLocationProductHaulViewModel model)
        {
            ProductHaulProcess.SetProductHaulOnLocation(model.ProductHaulId, model.OnLocationTime, model.LoggedUser);
        }
        public List<ProductHaul> GetProductHaulCollectionBycallSheetNumber(int callSheetNumber)
        {
            return RigBoardProcess.GetProductHaulCollectionByCallSheetNumber(callSheetNumber);
        }
        public List<ProductHaulLoad> GetProductHaulLoadListByCallSheetNumber(int callSheetNumber)
        {
            return eServiceOnlineGateway.Instance.GetProductHaulLoadsByCallSheetNumber(callSheetNumber);
        }

        /*
        public int DeleteProductHaulLoadByModel(CancelBlendViewModel model)
        {
            var productHualLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(model.ProductHaulLoadId);
            productHualLoad.ModifiedUserName = model.LoggedUser;
            return ProductHaulProcess.DeleteProductHaulLoad(productHualLoad);
        }
        */
        /*
        public int DeleteProductHaulLoadById(ProductHaulLoadModel model)
        {
            var productHualLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(model.ProductHaulLoadId);
            productHualLoad.ModifiedUserName = model.LoggedUser;
            return ProductHaulProcess.DeleteProductHaulLoad(productHualLoad);
        }
        */
        public void DeleteProductHaulLoadById(int productHaulLoadId,string loggedUser)
        {
            var productHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productHaulLoadId);
            productHaulLoad.ModifiedUserName = loggedUser;
            List<int> shippingLoadSheetIds = new List<int>();
            shippingLoadSheetIds.Add(productHaulLoad.Id);
            var shippingLoadSheets = eServiceOnlineGateway.Instance.GetShippingLoadSheetByProductHaulLoads(shippingLoadSheetIds);
            foreach(var shippingLoadSheet in shippingLoadSheets)
            {
                var productHaul = eServiceOnlineGateway.Instance.GetProductHaulById(shippingLoadSheet.ProductHaul.Id);
                if(productHaul.ShippingLoadSheets.Count==1)
                {
                    if (productHaul.IsThirdParty)
                    {
                        RigJobThirdPartyBulkerCrewSection bulkerCrewSection = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionByProductHaul(productHaul.Id);
                        ThirdPartyBulkerCrewSchedule thirdPartyBulkerCrewSchedule = eServiceOnlineGateway.Instance.GetThirdPartyCrewScheduleByRigJobThirdPartyBulkerCrewSection(bulkerCrewSection);
                        eServiceOnlineGateway.Instance.DeletethirdPartyBulkerCrewSchedule(thirdPartyBulkerCrewSchedule.Id);
                        eServiceOnlineGateway.Instance.DeleteRigJobThirdPartyBulkerCrewSection(bulkerCrewSection.Id);
                    }
                    else
                    {
                        RigJobSanjelCrewSection sanjelCrewSection = eServiceOnlineGateway.Instance.GetRigJobCrewSectionsByProductHaul(productHaul.Id);
                        SanjelCrewSchedule sanjelCrewSchedule = eServiceOnlineGateway.Instance.GetSanjelCrewSchedulesByRigJobSanjelCrewSection(sanjelCrewSection);
                        eServiceOnlineGateway.Instance.DeleteCrewSchedule(sanjelCrewSchedule.Id, true);
                        eServiceOnlineGateway.Instance.DeleteRigJobCrewSection(sanjelCrewSection);
                    }
                    eServiceOnlineGateway.Instance.DeleteProductHaul(productHaul);
                }
                shippingLoadSheet.ModifiedUserName = loggedUser;
                eServiceOnlineGateway.Instance.DeleteShippingLoadSheet(shippingLoadSheet);
            }
            productHaulLoad.ModifiedUserName = loggedUser;
            eServiceOnlineGateway.Instance.DeleteProductHaulLoad(productHaulLoad);
        }
        public List<ProductHaulLoad> GetProductHaulLoadCollectionByProductHaulId(int productHaulId)
        {
            return eServiceOnlineGateway.Instance.GetProductHaulLoadsByProductHual(new ProductHaul {Id = productHaulId});
        }

        public List<ProductHaul> GetExistingProductHaulCollection()
        {
            return RigBoardProcess.GetExistingProductHaulCollection();
        }

        /*public void AddNewProductLoadToExistingProductHaul(ProductHaulModel productHaulModel)
        {
            ProductHaul productHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulModel.ProductHaulId);

            productHaul.ModifiedUserName = productHaulModel.LoggedUser;

            if (productHaul == null) throw new Exception("Existing Product Haul Id is not passed in, Error must be in UI");

            productHaul.ProductHaulLoads = eServiceOnlineGateway.Instance.GetProductHaulLoadsByProductHual(productHaul);

            var productHaulLoad = BuildProductHaulLoad(productHaulModel);

            //TODO: we may need to consider add seperate expected on location time for each product haul load
            productHaulLoad.ExpectedOnLocationTime = productHaul.ProductHaulLoads.FirstOrDefault().ExpectedOnLocationTime;
            productHaulLoad.IsGoWithCrew = productHaul.IsGoWithCrew;
            productHaulLoad.Description = ProductHaulProcess.BuildProductHaulLoadComments(productHaulLoad);
            productHaulLoad.ProductHaul = productHaul;

            RigBoardProcess.CreateProductHaulLoad(productHaulLoad);

            productHaul.ProductHaulLoads.Add(productHaulLoad);
            productHaul.Description = RigBoardProcess.BuildUnitSectionComments(productHaul);

            //If product haul load is for a job, add product haul information to call sheet unit section, but should not create new rig job crew assignment. The crew is assigned to product haul from this view.
            //If product haul load is for a haul only, no update is needed.
            //crew assignment and schedule description needs to be updated anyway


            if (productHaulLoad.CallSheetId > 0)
            {
                ProductHaulProcess.UpdateUnitSectionByProductHaul(productHaul);
            }

            if (productHaul.IsThirdParty)
            {
            
                RigJobThirdPartyBulkerCrewSection rigJobThirdPartyBulkerCrewSection = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionsByProductHual(productHaul.Id);
                if (rigJobThirdPartyBulkerCrewSection != null)
                {
                    rigJobThirdPartyBulkerCrewSection.ModifiedUserName= productHaulModel.LoggedUser;
                    rigJobThirdPartyBulkerCrewSection.ProductHaul = productHaul;
                    rigJobThirdPartyBulkerCrewSection.Description = productHaul.Description;
                    eServiceOnlineGateway.Instance.UpdateRigJobThirdPartyCrewSection(rigJobThirdPartyBulkerCrewSection);

                    ThirdPartyBulkerCrewSchedule thirdPartyBulkerCrewSchedule = 
                        eServiceOnlineGateway.Instance.GetThirdPartyBulkerCrewSchedule(rigJobThirdPartyBulkerCrewSection.Id);
                    if(thirdPartyBulkerCrewSchedule!=null)
                    {
                        thirdPartyBulkerCrewSchedule.ModifiedUserName = productHaulModel.LoggedUser;
                        thirdPartyBulkerCrewSchedule.Description = productHaul.Description;
                        eServiceOnlineGateway.Instance.UpdateThirdPartyCrewSchedule(thirdPartyBulkerCrewSchedule);
                    }
                }

            }
            else 
            {
                RigJobSanjelCrewSection rigJobSanjelCrewSection = eServiceOnlineGateway.Instance.GetRigJobCrewSectionsByProductHaul(productHaul.Id);
                if (rigJobSanjelCrewSection != null)
                {
                    rigJobSanjelCrewSection.ModifiedUserName = productHaulModel.LoggedUser;
                    rigJobSanjelCrewSection.ProductHaul = productHaul;
                    rigJobSanjelCrewSection.Description = productHaul.Description;
                    eServiceOnlineGateway.Instance.UpdateRigJobCrewSection(rigJobSanjelCrewSection);

                    SanjelCrewSchedule sanjelCrewSchedule = eServiceOnlineGateway.Instance.GetCrewScheduleByJobCrewSection(rigJobSanjelCrewSection.Id);
                    if (sanjelCrewSchedule != null)
                    {
                        sanjelCrewSchedule.ModifiedUserName = productHaulModel.LoggedUser;
                        sanjelCrewSchedule.Description = productHaul.Description;
                        eServiceOnlineGateway.Instance.UpdateSanjelCrewSchedule(sanjelCrewSchedule);
                    }

                }

            }
            

            eServiceOnlineGateway.Instance.UpdateProductHaul(productHaul);
        }*/
        

        /*
        private int GetCrewIdByProductHaul(ProductHaul productHaul)
        {
            int crewId = 0;
            if (productHaul.IsThirdParty)
            {
                ThirdPartyBulkerCrewSchedule schedule = eServiceOnlineGateway.Instance.GetThirdPartyCrewScheduleById(productHaul?.Schedule?.Id ?? 0);
                crewId = schedule.ThirdPartyBulkerCrew.Id;
            }
            else
            {
                SanjelCrewSchedule schedule = eServiceOnlineGateway.Instance.GetCrewScheduleById(productHaul?.Schedule?.Id ?? 0);
                crewId = schedule.SanjelCrew.Id;
            }
            return crewId;
        }
        */

        public ProductHaulLoad GetProductHaulLoadById(int productHaulLoadId)
        {
            ProductHaulLoad productHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productHaulLoadId, true);

            return productHaulLoad;
        }

        public void UpdateProductHaulLoadOnly(ProductHaulLoad productHaulLoad)
        {
            productHaulLoad = this.GetCalculatedProductHaulLoad(productHaulLoad);
            productHaulLoad.Description = ProductHaulProcess.BuildProductHaulLoadComments(productHaulLoad);
            eServiceOnlineGateway.Instance.UpdateProductHaulLoad(productHaulLoad, true);
        }
        /*public void UpdateProductHaulAndHaulLoad(ProductHaul productHaul, ProductHaulLoad productHaulLoad, bool isCreateProductHaul, int callSheetId,int rigJobId, int crewId = 0)
        {
            productHaulLoad = this.GetCalculatedProductHaulLoad(productHaulLoad);
            productHaul.ProductHaulLoads=new List<ProductHaulLoad>
            {
                productHaulLoad
            };
            if (isCreateProductHaul)
            {
                Schedule schedule = RigBoardProcess.CreateSchedule(crewId,productHaul.IsThirdParty,productHaul.EstimatedLoadTime, rigJobId, productHaulLoad.IsGoWithCrew, productHaul.EstimatedLoadTime.AddHours(productHaul.EstimatedTravelTime), productHaul);
                productHaul.Schedule = schedule;
                ProductHaul createProductHaul = RigBoardProcess.CreateProductHaulAndUpdateHaulLoad(productHaul, productHaulLoad, callSheetId, crewId);

                if (productHaul.IsThirdParty)
                {
                    RigJobThirdPartyBulkerCrewSection rigJobThirdPartyBulkerCrewSection = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSection(rigJobId, crewId);
                    if (rigJobThirdPartyBulkerCrewSection != null)
                    {
                        rigJobThirdPartyBulkerCrewSection.ProductHaul = createProductHaul;
                        eServiceOnlineGateway.Instance.UpdateRigJobThirdPartyCrewSection(rigJobThirdPartyBulkerCrewSection);
                    }
                    
                }
                else
                {
                    RigJobSanjelCrewSection rigJobSanjelCrewSection = eServiceOnlineGateway.Instance.GetRigJobCrewSection(rigJobId, crewId);
                    if (rigJobSanjelCrewSection != null)
                    {
                        rigJobSanjelCrewSection.ProductHaul = createProductHaul;
                        eServiceOnlineGateway.Instance.UpdateRigJobCrewSection(rigJobSanjelCrewSection);
                    }
                }
            }
            else
            {
                RigBoardProcess.UpdateHaulLoadAndUnitSections(productHaul, productHaulLoad, callSheetId, rigJobId);
            }
                
        }*/

        private ProductHaulLoad GetCalculatedProductHaulLoad(ProductHaulLoad productHaulLoad)
        {
            BlendChemical blendChemical = null;
            CallSheetHeader callSheetHeader = null;
            BlendSection blendSection = null;
            if (productHaulLoad.CallSheetNumber != 0)
            {
                callSheetHeader = this.GetCallSheetHeaderByCallSheetNumber(productHaulLoad.CallSheetNumber);
                CopyFromCallSheetHeader(productHaulLoad, callSheetHeader);
                blendSection = this.GetBlendSectionByBlendSectionId(productHaulLoad.BlendSectionId);
            }
            else
            {
                blendSection = this.GetProgramBlendSectionByBlendSectionId(productHaulLoad.BlendSectionId);
            }
            (blendChemical, productHaulLoad.BlendCategory) = this.GetBlendChemicalByBlendSection(blendSection);
            productHaulLoad = this.GetCalculatedProductHaulLoad(blendChemical, productHaulLoad, productHaulLoad.IsTotalBlendTonnage);
            return productHaulLoad;
        }

        /*
        public void UpdateShippingLoadSheetOnLocation(ShippingLoadSheet shippingLoadSheet, DateTime onLocationTime,
            string userName = null)
        {
            if(shippingLoadSheet.ShippingStatus != ShippingStatus.OnLocation)
	            ProductHaulProcess.UpdateShippingLoadSheetOnLocation(shippingLoadSheet, onLocationTime, userName);
            ProductHaulProcess.SetProductHaulOnLocationIfAllOnLocation(shippingLoadSheet.ProductHaul.Id, onLocationTime,
	            userName);
        }
        */


        /*
        //CJ (2018/07/18) Recalculate load and load sections 
        public void CreateProductLoadAndUpdateUnitSection(ProductHaulLoad productHaulLoad, int productHaulId, int callSheetId,int rigJobId)
        {
            this.GetCalculatedProductHaulLoad(productHaulLoad);
            RigBoardProcess.CreateProductLoadAndUpdateUnitSection(productHaulLoad, productHaulId, callSheetId, rigJobId);
        }
        */

        public void UpdateCompanyShortName(int rigJobId, int clientCompanyId, string companyShortName)
        {
            RigBoardProcess.UpdateCompanyShortName(rigJobId, clientCompanyId, companyShortName);
        }

        public List<BlendChemicalModel> GetBlendChemicalPageInfo(int i, int pageNumber, int pageSize, out int count)
        {
            IBlendChemicalProcess blendChemicalProcess = new BlendChemicalProcess();
            List<BlendChemical> blendChemicals = blendChemicalProcess.GetBlendChemicalCollectionByPaginated(0, pageNumber, pageSize, out count);
            List<BlendChemicalModel> blendChemicalModels = new List<BlendChemicalModel>();
            if (blendChemicals.Count > 0)
                foreach (BlendChemical blendChemical in blendChemicals)
                {
                    BlendChemicalModel blendChemicalModel = new BlendChemicalModel();
                    blendChemicalModel.Id = blendChemical.Id;
                    blendChemicalModel.Name = blendChemical.Name;
                    blendChemicalModel.Density = blendChemical.Density;
                    blendChemicalModel.BulkDensity = blendChemical.BulkDensity;
                    blendChemicalModel.Yield = blendChemical.Yield;
                    blendChemicalModel.SpecificGravity = blendChemical.SpecificGravity;
                    blendChemicalModel.MixWater = blendChemical.MixWater;
                    blendChemicalModel.IsBaseEligible = blendChemical.IsBaseEligible;
                    blendChemicalModel.IsAdditiveEligible = blendChemical.IsAdditiveEligible;
                    blendChemicalModel.PrimaryCategoryName = blendChemical.BlendPrimaryCategory.Name;
                    blendChemicalModel.PrimaryCategoryId = blendChemical.BlendPrimaryCategory.Id;
                    blendChemicalModel.StatusName = blendChemical.ProductStatus.ToString();
                    blendChemicalModel.StatusId = (int)blendChemical.ProductStatus;
                    blendChemicalModel.PriceCode = blendChemical.Product.PriceCode;
                    blendChemicalModel.InventoryNumber = blendChemical.Product.InventoryNumber;
                    blendChemicalModel.AERCode = blendChemical.AERCode;

                    
                    blendChemicalModels.Add(blendChemicalModel);
                }

            return blendChemicalModels;
        }

        /*private void CreateProductLoadAndProductHaul(ProductHaulModel model, string callSheetId, ProductHaulLoad productHaulLoad)
        {
          
            ProductHaul createProductHaul = new ProductHaul();
            model.PopulateTo(createProductHaul);
            if (!model.IsThirdParty)
            {
                List<Employee> employeeList = this.GetEmployeeList();
                Employee driver = employeeList.Find(employee => employee.Id == model.DriverId);
                Employee driver2 = employeeList.Find(employee => employee.Id == model.Driver2Id);
                if (driver != null)
                {
                    createProductHaul.Driver = driver;
                    createProductHaul.Driver.Id = driver.Id;
                    createProductHaul.Driver.FirstName = driver.FirstName;
                    createProductHaul.Driver.MiddleName = driver.MiddleName;
                    createProductHaul.Driver.LastName = driver.LastName;
                }
                if (driver2 != null)
                {
                    createProductHaul.Driver2 = driver2;
                    createProductHaul.Driver2.Id = driver2.Id;
                    createProductHaul.Driver2.FirstName = driver2.FirstName;
                    createProductHaul.Driver2.MiddleName = driver2.MiddleName;
                    createProductHaul.Driver2.LastName = driver2.LastName;
                }
            }
            createProductHaul.ProductHaulLifeStatus = ProductHaulStatus.Scheduled;
            if (!model.IsGoWithCrew)
                productHaulLoad.ExpectedOnLocationTime = model.ExpectedOlTime;


            if (model.IsGoWithCrew)
            {
                DateTime endTime = GetRigJobAssginCrewEndTime(model.RigJobId);
                if (endTime == DateTime.MinValue)
                {
                    createProductHaul.EstimatedTravelTime = 8;
                }
                else
                {
                    createProductHaul.EstimatedTravelTime = (endTime - createProductHaul.EstimatedLoadTime).TotalHours;
                }
            }

            this.UpdateProductHaulAndHaulLoad(createProductHaul, productHaulLoad, true, int.Parse(callSheetId), model.RigJobId, model.IsThirdParty ? model.ThirdPartyBulkerCrewId : model.CrewId);
        }*/

        /*
        //CJ (2018/07/30)  Don't use original haul
        public void NotUseOriginalHaul(ProductHaulModel model, string callSheetId)
        {
            ProductHaulLoad productHaulLoad = new ProductHaulLoad();
            model.PopulateToHaulLoad(productHaulLoad);
            //CJ (2018/07/18) delete the original UnitSections and update the UnitSections associated with the original productHaul 
            RigBoardProcess.DeleteHaulLoadAndUnitSections(model.ProductHaulLoadId, Convert.ToInt32(callSheetId), model.RigJobId);

            if (model.IsExistingHaul)
            {
                this.CreateProductLoadAndUpdateUnitSection(productHaulLoad, model.ProductHaulId, int.Parse(callSheetId),model.RigJobId);
            }           
            else
                this.CreateProductLoadAndProductHaul(model, callSheetId, productHaulLoad);
        }
        */

   

        public List<ProductLoadSection> GetProductLoadSectionCollectionByProductLoadId(int productHaulLoadId)
        {
            return eServiceOnlineGateway.Instance.GetProductLoadSectionsByProductLoadId(productHaulLoadId);
        }

        public int AssignACrew(int crewId, int rigJobId, DateTime callCrewTime, int duration)
        {
            int rtn = RigBoardProcess.AssignACrew(crewId, rigJobId, callCrewTime, duration);
            AlignPumperCrewAssignment(rigJobId);

            return rtn;
        }

        public List<RigJobSanjelCrewSection> GetRigJobCrewSectionByRigJob(int rigJobId)
        {
            return eServiceOnlineGateway.Instance.GetRigJobCrewSectionByRigJob(rigJobId);
        }

        public void CallAllCrews(int rigJobId)
        {
            RigBoardProcess.CallAllCrew(rigJobId);
        }

        public void WithdrawACrew(int rigJobId, int crewId, int jobCrewSectionStatusId)
        {
            RigBoardProcess.WithdrawACrew(rigJobId, crewId, jobCrewSectionStatusId);
            AlignPumperCrewAssignment(rigJobId);
        }

        public ServicePoint GetServicePointById(int id)
        {
            return eServiceOnlineGateway.Instance.GetServicePointById(id);
        }


        public int UpdateRigBinSection(BinInformation rigBinSection)
        {
            return eServiceOnlineGateway.Instance.UpdateBinInformation(rigBinSection);
        }

        public List<ProductHaulLoad> GetProductHaulLoadByBlendSectionId(int blendSectionId)
        {
            return eServiceOnlineGateway.Instance.GetProductHaulLoadByBlendSectionId(blendSectionId);
        }

        public Sesi.SanjelData.Entities.BusinessEntities.BaseEntites.BlendSection GetBlendSectionByBlendSectionId(int blendSectionId)
        {
            return eServiceOnlineGateway.Instance.GetBlendSectionByBlendSectionId(blendSectionId);
        }

        
        public Sesi.SanjelData.Entities.BusinessEntities.BaseEntites.BlendSection GetProgramBlendSectionByBlendSectionId(int blendSectionId)
        {
            return eServiceOnlineGateway.Instance.GetProgramBlendSectionByBlendSectionId(blendSectionId);
        }

        public Job GetJobByUniqueId(string jobUniqueId)
        {
            return eServiceOnlineGateway.Instance.GetJobByUniqueId(jobUniqueId);
        }

        public List<SanjelCrew> GetCrewsByRigJob(RigJob rigJob)
        {
            UpcomingJobsProcess upcomingJobsProcess = new UpcomingJobsProcess();

            return upcomingJobsProcess.GetCrewsByRigJob(rigJob);
        }

        public SanjelCrew GetCrewById(int id)
        {
            return CrewBoardProcess.GetCrewById(id);
        }

        public Employee GetEmployeeById(int id)
        {
            return CrewBoardProcess.GetEmployeeById(id);
        }

        public DateTime GetCallCrewTime(int rigJobId)
        {
            return RigBoardProcess.GetCallCrewTime(rigJobId);
        }

        public List<ProductHaulLoadModel> GetProductHaulPageInfo(int i, int servicePointId, int pageNumber, int pageSize, out int count)
        {
            List<ProductHaulLoad> productHaulLoads = ProductHaulProcess.GetProductHaulLoadCollectionByPaginated(0, servicePointId, pageNumber, pageSize, out count);
            List<ProductHaulLoadModel> productHaulLoadModels = new List<ProductHaulLoadModel>();
            if (productHaulLoads.Count > 0)
                foreach (ProductHaulLoad productHaulLoad in productHaulLoads)
                {
                    ProductHaulLoadModel productHaulLoadModel = new ProductHaulLoadModel();
                    productHaulLoadModel.PopulateFrom(productHaulLoad);
                    
                    if (!productHaulLoad.IsBlendTest)
                    {
                        ProductHaul productHaul =
                            eServiceOnlineGateway.Instance.GetProductHaulById(productHaulLoad.ProductHaul.Id);
                        if (productHaul != null)
                        {
                            productHaulLoadModel.ProductHaulId = productHaul.Id;
                            if (!productHaul.IsThirdParty)
                            {
                                productHaulLoadModel.BulkUnitName = productHaul.BulkUnit.UnitNumber;
                                productHaulLoadModel.TractorUnitName = productHaul.TractorUnit.UnitNumber;
                                productHaulLoadModel.Driver = productHaul.Driver.LastName + ", " +
                                                              (string.IsNullOrEmpty(productHaul.Driver.PreferedFirstName)
                                                                  ? productHaul.Driver.FirstName
                                                                  : productHaul.Driver.PreferedFirstName);
                            }
                            else
                            {
                                productHaulLoadModel.TractorUnitName = productHaul.ContractorCompany.Name;
                                productHaulLoadModel.BulkUnitName = productHaul.ThirdPartyUnitNumber;
                                productHaulLoadModel.Driver = productHaul.SupplierContactName;
                            }
                        }
                    }

                    if (productHaulLoad.CallSheetNumber != 0)
                    {
                        RigJob rigJob =
                            eServiceOnlineGateway.Instance.GetRigJobByCallSheetNumber(productHaulLoad.CallSheetNumber);
                        if (rigJob != null)
                        {
                            productHaulLoadModel.RigName = rigJob.Rig?.Name;
                            productHaulLoadModel.RigId = rigJob.Rig.Id;
                        }
                    }
                    else
                    {
                        productHaulLoadModel.RigName = productHaulLoad.Rig.Name;
                        productHaulLoadModel.RigId = productHaulLoad.Rig.Id;
                    }
                    int binId = productHaulLoad.Bin.Id;
                    var binInformation = this.GetBinInformationByBinId(binId, productHaulLoadModel.PodIndex);
                    if (binInformation != null)
                    {
                        productHaulLoadModel.BinInformationId = binInformation.Id;
                        productHaulLoadModel.BinInformationName = binInformation.Name;
                    }
                    productHaulLoadModels.Add(productHaulLoadModel);
                }

            return productHaulLoadModels;
        }

        public ProductHaulModel GetProductHaulModelByProductHaulLoadId(int productHaulLoadId)
        {
            ProductHaulModel model = new ProductHaulModel();
            ProductHaulLoad productHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productHaulLoadId, true);
            if (productHaulLoad != null)
            {
                /*
                if (productHaulLoad.ProductHaul != null && productHaulLoad.ProductHaul.Id != 0)
                {
                    ProductHaul productHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulLoad.ProductHaul.Id); //this.GetProductHaulInfoById(productHaulLoad.ProductHaul.Id);
                    if (productHaul != null)
                        model.PopulateFrom(productHaul);
                }
                */

                model.PopulateFromHaulLoad(productHaulLoad);

                var shippingLoadSheets =
                    eServiceOnlineGateway.Instance.GetShippingLoadSheetByProductHaulLoads(new List<int>()
                        { productHaulLoad.Id });
                if (shippingLoadSheets != null && shippingLoadSheets.Count>0)
                {
                    List<int> productHaulIdList = shippingLoadSheets.Select(p => p.ProductHaul?.Id ?? 0).Distinct().Except(new []{0}).ToList();
                    Collection<int> idCollection = new Collection<int>(productHaulIdList);

                    var productHauls =
                        eServiceOnlineGateway.Instance.GetProductHaulByQuery(p => idCollection.Contains(p.Id));
                    if (productHauls != null && productHauls.Count > 0)
                    {
                        model.PopulateFrom(productHauls.FirstOrDefault());
                    }

                    List<int> shippingLoadSheetIdList = shippingLoadSheets.Select(p => p.Id).Distinct().Except(new []{0}).ToList();
                    Collection<int> shippingLoadSheetIds = new Collection<int>(shippingLoadSheetIdList);
                    var blendUnloadSheets =
                        eServiceOnlineGateway.Instance.GetBlendUnloadSheetByQuery(p =>
                            shippingLoadSheetIds.Contains(p.ShippingLoadSheet.Id));
                    string destinationStorageName = string.Empty;

                    foreach (var blendUnloadSheet in blendUnloadSheets)
                    {
                        if(Math.Abs(blendUnloadSheet.UnloadAmount)>0.1)
                            destinationStorageName += ", " + blendUnloadSheet.DestinationStorage.Name;
                    }
                    if(!string.IsNullOrEmpty(destinationStorageName))
                        model.BinInformationName = destinationStorageName.Substring(2);
                }
                else
                {

                    if (model.BinId != 0 && model.PodIndex != 0)
                    {
                        var binInformation =
                            eServiceWebContext.Instance.GetBinInformationByBinId(model.BinId, model.PodIndex);
                        if (binInformation != null)
                        {
                            model.OrigBinInformationId = binInformation.Id;
                            model.BinInformationId = binInformation.Id;
                            model.BinInformationName = binInformation.Name;
                        }
                    }
                }
            }
            
            return model;
        }

        #endregion

        #region Crew Board Operation


        public List<ServicePoint> ssServicePoints()
        {
            return eServiceOnlineGateway.Instance.GetServicePoints();
        }

        public List<SanjelCrew> GetAllCrewsInfo(Collection<int> servicePointIds)
        {
            return CrewBoardProcess.GetAllCrewInfo(servicePointIds);
        }

        public List<SanjelCrew> GetSanjelCrews()
        {
            return RigBoardProcess.GetSanjelCrew();
        }

        public List<Employee> GetEmployeeList()
        {
            return eServiceOnlineGateway.Instance.GetEmployeeList();
        }

        public List<Employee> GetActivatedEmployees(int crewId)
        {
            return CrewBoardProcess.GetActivatedEmployees(crewId);
        }

        public List<TruckUnit> GetTruckUnitList()
        {
            return eServiceOnlineGateway.Instance.GetTruckUnitList();
        }

        public List<TruckUnit> GetActivatedTruckUnits(int crewId)
        {
            return CrewBoardProcess.GetActivatedTruckUnits(crewId);
        }

        public CrewType GetCrewTypeById(int id)
        {
            return eServiceOnlineGateway.Instance.GetCrewTypeById(id);
        }

        public int CreateCrew(SanjelCrew crew, int homeDistrictId, int primaryTruckUnitId = 0, int secondaryTruckUnitId = 0, int supervisorId = 0, int crewMemberId = 0)
        {
            return CrewBoardProcess.CreateCrew(crew, homeDistrictId, primaryTruckUnitId, secondaryTruckUnitId, supervisorId, crewMemberId);
        }

        public List<TruckUnit> GetTruckUnitsByCrew(int crewId)
        {
            return CrewBoardProcess.GetTruckUnitsByCrew(crewId);
        }

        public List<Employee> GetEmployeesByCrew(int crewId)
        {
            return CrewBoardProcess.GetWorkersByCrew(crewId);
        }

        public void AddUnitToCrew(int truckUnitId, int crewId)
        {
            CrewBoardProcess.AddUnitToCrew(truckUnitId, crewId, true);
        }

        public void AddWorkerToCrew(int workerId, int crewId, int crewPositionId)
        {
            CrewBoardProcess.AddWorkerToCrew(workerId, crewId, crewPositionId, true);
        }

        public void RemoveUnitFromCrew(int truckUnitId, int crewId)
        {
            CrewBoardProcess.RemoveUnitFromCrew(truckUnitId, crewId);
        }

        public void RemoveWorkerFromCrew(int workerId, int crewId)
        {
            CrewBoardProcess.RemoveWorkerFromCrew(workerId, crewId);
        }

        public List<CrewPosition> GetCrewPositions()
        {
            return eServiceOnlineGateway.Instance.GetCrewPositions();
        }

        public List<ServicePoint> GetServicePoints()
        {
            return eServiceOnlineGateway.Instance.GetServicePoints();
        }

        public List<CrewType> GetCrewTypesForSanjelCrew()
        {
            return CrewBoardProcess.GetCrewTypesForSanjelCrew();
        }

        public void LogOnDuty(int rigJobId)
        {
            RigBoardProcess.LogOnDuty(rigJobId);
        }

        public void LogOffDuty(int rigJobId)
        {
            RigBoardProcess.LogOffDuty(rigJobId);
        }

        public List<SanjelCrewSchedule> GetFutureCrewSchedules(int crewId, DateTime dateTime)
        {
            return CrewBoardProcess.GetFutureCrewSchedules(crewId, dateTime);
        }

        public List<SanjelCrew> GetEffectiveCrews(DateTime startTime, double duration, int workingDistrict, int rigJobId)
        {
            return CrewBoardProcess.GetEffectiveCrews(startTime, duration, workingDistrict, rigJobId);
        }

        public int UpdateCrew(SanjelCrew crew)
        {
            return eServiceOnlineGateway.Instance.UpdateCrew(crew);
        }

        public int UpdateWorker(Employee employee)
        {
            return CrewBoardProcess.UpdateWorker(employee);
        }

        public List<SanjelCrew> GetCrewsByServicePoint(Collection<int> servicePoints, out int count)
        {
            return CrewBoardProcess.GetCrewsByServicePoint(servicePoints, out count);
        }

        public List<TruckUnit> GetUnitsByServicePoint(int pageSize, int pageNumber, Collection<int> servicePoints, out int count)
        {
            return CrewBoardProcess.GetUnitsByServicePoint(pageSize, pageNumber, servicePoints, out count);
        }

        public List<Employee> GetWorkerListByServicePoints(int pageSize, int pageNumber, Collection<int> servicePoints, out int count)
        {
            return CrewBoardProcess.GetWorkerListByServicePoints(pageSize, pageNumber, servicePoints, out count);
        }



        public RigJobSanjelCrewSection GetRigJobCrewSectionById(int id)
        {
            return eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(id);
        }

        public List<TruckUnit> GetTruckUnitsByServicePoints(int pageSize, int pageNumber, Collection<int> servicePoints, out int count)
        {
            return CrewBoardProcess.GetTruckUnitsByServicePoints(pageSize, pageNumber, servicePoints, out count);
        }

        public void RemoveCrew(int crewId)
        {
            eServiceOnlineGateway.Instance.DeleteCrew(crewId);
        }

        public void AssignCrewToAnotherDistrict(int crewId, int workingDistrictId)
        {
            CrewBoardProcess.AssignCrewToAnotherDistrict(crewId, workingDistrictId);
        }

        public void RemoveAllWorker(int crewId)
        {
          CrewBoardProcess.RemoveAllWorker(crewId);
        }

        #endregion

        #region schedule

        public List<SanjelCrewSchedule> GetCrewSchedules()
        {
            return CalendarProcess.GetCrewSchedules();
        }
        public List<SanjelCrewSchedule> GetCrewSchedules(Collection<int> servicePoints, DateTime startTime, DateTime endTime)
        {
            return CalendarProcess.GetCrewSchedules(servicePoints, startTime,endTime);
        }


        public List<UnitSchedule> GetUnitSchedules()
        {
            return eServiceOnlineGateway.Instance.GetUnitSchedules();
        }

        public List<UnitSchedule> GetUnitSchedules(Collection<int> servicePoints, DateTime startTime, DateTime endTime)
        {
            return eServiceOnlineGateway.Instance.GetUnitSchedules(servicePoints, startTime,endTime);
        }

        public List<WorkerSchedule> GetWorkerSchedules()
        {
            return eServiceOnlineGateway.Instance.GetWorkerSchedules();
        }

        public List<WorkerSchedule> GetWorkerSchedules(Collection<int> servicePoints, DateTime startTime, DateTime endTime)
        {
            return eServiceOnlineGateway.Instance.GetWorkerSchedules(servicePoints, startTime,endTime);
        }

        public SanjelCrewSchedule GetCrewScheduleById(int id)
        {
            return CalendarProcess.GetCrewScheduleById(id);
        }

        public UnitSchedule GetUnitScheduleById(int id)
        {
            return CalendarProcess.GetUnitScheduleById(id);
        }

        public WorkerSchedule GetWorkerScheduleById(int id)
        {
            return CalendarProcess.GetWorkerScheduleById(id);
        }

        public int DeleteCrewSchedule(int id)
        {
            return CalendarProcess.DeleteCrewSchedule(id);
        }

        public int DeleteUnitSchedule(int id)
        {
            return CalendarProcess.DeleteUnitSchedule(id);
        }

        public int DeleteWorkSchedule(int id)
        {
            return CalendarProcess.DeleteWorkSchedule(id);
        }

        public int UpdateCrewSchedule(SanjelCrewSchedule crewSchedule)
        {
            return CalendarProcess.UpdateCrewSchedule(crewSchedule);
        }

        public int UpdateWorkerSchedule(WorkerSchedule workerSchedule)
        {
            return CalendarProcess.UpdateWorkerSchedule(workerSchedule);
        }

        public int UpdateUnitSchedule(UnitSchedule unitSchedule)
        {
            return CalendarProcess.UpdateUnitSchedule(unitSchedule);
        }

        public int InsertCrewSchedule(SanjelCrewSchedule crewSchedule)
        {
            return CalendarProcess.InsertCrewSchedule(crewSchedule);
        }

        public int InsertWorkerSchedule(WorkerSchedule workerSchedule)
        {
            return CalendarProcess.InsertWorkerSchedule(workerSchedule);
        }

        public int InsertUnitSchedule(UnitSchedule unitSchedule)
        {
            return CalendarProcess.InsertUnitSchedule(unitSchedule);
        }

        /*
        public List<UnitScheduleType> UnitScheduleTypes()
        {
            return CalendarProcess.GetListUnitScheduleTypes();
        }

        public List<WorkerScheduleType> WorkerScheduleTypes()
        {
            return CalendarProcess.GetListWorkerScheduleTypes();
        }
        */

        public SanjelCrewTruckUnitSection GetCrewTruckUnitSectionById(int id)
        {
            return eServiceOnlineGateway.Instance.GetCrewTruckUnitSectionById(id);
        }

        public SanjelCrewWorkerSection GetCrewWorkerSectionById(int id)
        {
            return eServiceOnlineGateway.Instance.GetCrewWorkerSectionById(id);
        }

        public string VerifyUnitSchedule(int truckUnitId, DateTime startTime, DateTime endTime)
        {
            List<UnitSchedule> unitSchedules = eServiceOnlineGateway.Instance.GetUnitSchedulesByTruckUnit(truckUnitId);
            string messageInfo = UnitScheduleValidation.ValidateUnitSchedule(startTime, endTime, unitSchedules);

            return messageInfo;
        }

        public string VerifyWorkerSchedule(int workerId, DateTime startTime, DateTime endTime)
        {
            List<WorkerSchedule> workerSchedules = eServiceOnlineGateway.Instance.GetWorkerSchedulesByWorker(workerId);
            string messageInfo = WorkerScheduleValidation.ValidateWorkerSchedule(startTime, endTime, workerSchedules);

            return messageInfo;
        }

        public string VerifyThirdPartyBulkerCrewSchedule(int thirdPartyBulkerCrewId, DateTime startTime, DateTime endTime)
        {
            List<ThirdPartyBulkerCrewSchedule> thirdPartyBulkerCrewSchedules =ThirdPartyCrewBoardProcess.GetThirdPartyCrewScheduleByThirdPartyCrewId(thirdPartyBulkerCrewId);
            string messageInfo = ThirdPartyBulkerCrewScheduleValidation.ValidateThirdPartyBulkerCrewSchedule(startTime, endTime, thirdPartyBulkerCrewSchedules);

            return messageInfo;
        }

        public string VerifySanjelCrewSchedule(int sanjelCrewId, DateTime startTime, DateTime endTime)
        {
            List<SanjelCrewSchedule> sanjelCrewSchedules = eServiceOnlineGateway.Instance.GetSanjelCrewSchedulesBySanjelCrew(sanjelCrewId);
            string messageInfo = SanjelCrewScheduleValidation.ValidateSanjelCrewSchedule(startTime, endTime, sanjelCrewSchedules, sanjelCrewId);

            return messageInfo;
        }
        
        #endregion

        #region Third Party Crew Board Operation
        public List<ThirdPartyBulkerCrew> GetThirdPartyBulkerCrewsByServicePoint(Collection<int> servicePoints, out int count)
        {
            return CrewBoardProcess.GetThirdPartyBulkerCrewsByServicePoint(servicePoints, out count);
        }

        public List<ThirdPartyBulkerCrew> GetThirdPartyCrews(int pageSize, int pageNumber, out int count)
        {
            return ThirdPartyCrewBoardProcess.GetThirdPartyBulkerCrewsByPage(pageSize, pageNumber, out count);
        }

        public List<ThirdPartyBulkerCrew> GetThirdPartyCrews()
        {
            return ThirdPartyCrewBoardProcess.GetAllThirdPartyBulkerCrews();
        }

        public ThirdPartyBulkerCrew GetThirdPartyBulkerCrewById(int id)
        {
            return eServiceOnlineGateway.Instance.GetThirdPartyBulkerCrewById(id);
        }

        public int UpdateThirdPartyBulkerCrew(ThirdPartyBulkerCrew thirdPartyBulkerCrew)
        {
            return ThirdPartyCrewBoardProcess.UpdateThirdPartyBulkerCrew(thirdPartyBulkerCrew);
        }

        public Collection<ContractorCompany> GetAllContractorCompanies()
        {
            return eServiceOnlineGateway.Instance.GetAllContractorCompanies();
        }

        public int CreateThirdPartyBulkerCrew(ThirdPartyBulkerCrew thirdPartyBulkerCrew)
        {
            return ThirdPartyCrewBoardProcess.CreateThirdPartyBulkerCrew(thirdPartyBulkerCrew);
        }

        public ContractorCompany GetContractorCompanyById(int id)
        {
            return eServiceOnlineGateway.Instance.GetContractorCompanyById(id);
        }


        public RigSizeType GetRigSizeTypeById(int id)
        {
            return eServiceOnlineGateway.Instance.GetRigSizeTypeById(id);
        }

        public RigSize GetRigSizeById(int id)
        {
            return eServiceOnlineGateway.Instance.GetRigSizeById(id);
        }

        public ThreadType GetThreadTypeById(int id)
        {
            return eServiceOnlineGateway.Instance.GetThreadTypeById(id);
        }

        /*
        public double GetDuration(int scheduleId, int rigJobId, bool isThridParty)
        {
            return RigBoardProcess.GetDuration(scheduleId, rigJobId, isThridParty);
        }
        */

        public TruckUnit GetTruckUnitById(int id)
        {
            return eServiceOnlineGateway.Instance.GetTruckUnitById(id);
        }

        public List<SanjelCrewSchedule> GetCrewScheduleByCrewId(int crewId)
        {
            return CalendarProcess.GetCrewScheduleByCrewId(crewId);
        }

        public ThirdPartyBulkerCrewSchedule GetThirdPartyCrewScheduleById(int id)
        {
            return eServiceOnlineGateway.Instance.GetThirdPartyCrewScheduleById(id);
        }

        public List<ProductHaul> GetProductHaulByCrewScheduleId(int crewScheduleId)
        {
            return eServiceOnlineGateway.Instance.GetProductHaulByCrewScheduleId(crewScheduleId);
        }

        public List<ThirdPartyBulkerCrewSchedule> GetThirdPartyCrewScheduleByThirdPartyCrewId(int thirdPartyCrewId)
        {
            return ThirdPartyCrewBoardProcess.GetThirdPartyCrewScheduleByThirdPartyCrewId(thirdPartyCrewId);
        }

        public int DeleteThirdPartyBulkerCrew(int crewId)
        {
            return eServiceOnlineGateway.Instance.DeleteThirdPartyBulkerCrew(crewId);
        }

        public int CreateSanjelCrewNote(SanjelCrewNote sanjelCrewNote)
        {
            return eServiceOnlineGateway.Instance.CreateSanjelCrewNote(sanjelCrewNote);
        }

        public SanjelCrewNote GetSanjelCrewNoteBySanjelCrewId(int sanjelCrewId)
        {
            return eServiceOnlineGateway.Instance.GetSanjelCrewNoteBySanjelCrew(sanjelCrewId);
        }

        public int UpdateSanjelCrewNote(int sanjelCrewId, string notes)
        {
            SanjelCrewNote crewNote = eServiceOnlineGateway.Instance.GetSanjelCrewNoteBySanjelCrew(sanjelCrewId);
            if (crewNote != null)
            {
                crewNote.Description = notes;
                crewNote.Name = notes;
                return eServiceOnlineGateway.Instance.UpdateSanjelCrewNote(crewNote);

            }
            else
            {
                if (!string.IsNullOrEmpty(notes))
                {
                    SanjelCrewNote sanjelCrewNote = new SanjelCrewNote();
                    SanjelCrew sanjelCrew = eServiceOnlineGateway.Instance.GetCrewById(sanjelCrewId);
                    sanjelCrewNote.SanjelCrew = sanjelCrew;
                    sanjelCrewNote.Description = notes;
                    sanjelCrewNote.Name = notes;
                    return eServiceOnlineGateway.Instance.CreateSanjelCrewNote(sanjelCrewNote);
                }

                return 0;
            }
        }

        public ThirdPartyBulkerCrewNote GeThirdPartyBulkerCrewNoteByThirdPartyBulkerCrew(int thirdPartyBulkerCrewId)
        {
            return eServiceOnlineGateway.Instance.GeThirdPartyBulkerCrewNoteByThirdPartyBulkerCrew(thirdPartyBulkerCrewId);
        }

        public int CreateThirdPartyBulkerCrewNote(ThirdPartyBulkerCrewNote thirdPartyBulkerCrewNote)
        {
            return eServiceOnlineGateway.Instance.CreateThirdPartyBulkerCrewNote(thirdPartyBulkerCrewNote);
        }

        public int UpdateThirdPartyBulkerCrewNote(int thirdPartyBulkerCrewId, string notes)
        {
            ThirdPartyBulkerCrewNote thirdPartyBulkerCrewNote = eServiceOnlineGateway.Instance.GeThirdPartyBulkerCrewNoteByThirdPartyBulkerCrew(thirdPartyBulkerCrewId);
            if (thirdPartyBulkerCrewNote != null)
            {
                thirdPartyBulkerCrewNote.Description = notes;
                thirdPartyBulkerCrewNote.Name = notes;
                return eServiceOnlineGateway.Instance.UpdateThirdPartyBulkerCrewNote(thirdPartyBulkerCrewNote);

            }
            else
            {
                if (!string.IsNullOrEmpty(notes))
                {
                    ThirdPartyBulkerCrewNote newthirdPartyBulkerCrewNote = new ThirdPartyBulkerCrewNote();
                    ThirdPartyBulkerCrew thirdPartyBulkerCrew = eServiceOnlineGateway.Instance.GetThirdPartyBulkerCrewById(thirdPartyBulkerCrewId);
                    newthirdPartyBulkerCrewNote.ThirdPartyBulkerCrew = thirdPartyBulkerCrew;
                    newthirdPartyBulkerCrewNote.Description = notes;
                    newthirdPartyBulkerCrewNote.Name = notes;
                    return eServiceOnlineGateway.Instance.CreateThirdPartyBulkerCrewNote(newthirdPartyBulkerCrewNote);
                }

                return 0;
            }
        }

        public EmployeeNote GetEmployeeNoteByEmployee(int employeeId)
        {
            return eServiceOnlineGateway.Instance.GetEmployeeNoteByEmployee(employeeId);
        }

        public int UpdateEmployeeNote(int employeeId, string notes)
        {
            EmployeeNote employeeNote = eServiceOnlineGateway.Instance.GetEmployeeNoteByEmployee(employeeId);
            if (employeeNote != null)
            {
                employeeNote.Description = notes;
                employeeNote.Name = notes;
                return eServiceOnlineGateway.Instance.UpdateEmployeeNote(employeeNote);

            }
            else
            {
                if (!string.IsNullOrEmpty(notes))
                {
                    EmployeeNote newemployeeNote = new EmployeeNote();
                    Employee employee = eServiceOnlineGateway.Instance.GetEmployeeById(employeeId);
                    newemployeeNote.Employee = employee;
                    newemployeeNote.Description = notes;
                    newemployeeNote.Name = notes;
                    return eServiceOnlineGateway.Instance.CreateEmployeeNote(newemployeeNote);
                }

                return 0;
            }
        }

        public int UpdateEmployeeProfile(int employeeId, string profile)
        {
            EmployeeProfile employeeProfile = eServiceOnlineGateway.Instance.GetEmployeeProfileByEmployee(employeeId);
            if (employeeProfile != null)
            {
                employeeProfile.Description = profile;
                employeeProfile.Name = profile;
                return eServiceOnlineGateway.Instance.UpdateEmployeeProfile(employeeProfile);

            }
            else
            {
                if (!string.IsNullOrEmpty(profile))
                {
                    EmployeeProfile newEmployeeProfile = new EmployeeProfile();
                    Employee employee = eServiceOnlineGateway.Instance.GetEmployeeById(employeeId);
                    newEmployeeProfile.Employee = employee;
                    newEmployeeProfile.Description = profile;
                    newEmployeeProfile.Name = profile;
                    return eServiceOnlineGateway.Instance.CreateEmployeeProfile(newEmployeeProfile);
                }

                return 0;
            }
        }

        public EmployeeProfile GetEmployeeProfileByEmployee(int employeeId)
        {
            return eServiceOnlineGateway.Instance.GetEmployeeProfileByEmployee(employeeId);
        }

        public TruckUnitNote GetTruckUnitNoteByTruckUnit(int truckUnitId)
        {
            return eServiceOnlineGateway.Instance.GetTruckUnitNoteByTruckUnit(truckUnitId);
        }

        public int UpdateTruckUnitNote(int truckUnitId, string notes)
        {
            TruckUnitNote truckUnitNote = eServiceOnlineGateway.Instance.GetTruckUnitNoteByTruckUnit(truckUnitId);
            if (truckUnitNote != null)
            {
                truckUnitNote.Description = notes;
                truckUnitNote.Name = notes;
                return eServiceOnlineGateway.Instance.UpdateTruckUnitNote(truckUnitNote);

            }
            else
            {
                if (!string.IsNullOrEmpty(notes))
                {
                    TruckUnitNote newTruckUnitNote = new TruckUnitNote();
                    TruckUnit truckUnit = eServiceOnlineGateway.Instance.GetTruckUnitById(truckUnitId);
                    newTruckUnitNote.TruckUnit = truckUnit;
                    newTruckUnitNote.Description = notes;
                    newTruckUnitNote.Name = notes;
                    return eServiceOnlineGateway.Instance.CreateTruckUnitNote(newTruckUnitNote);
                }

                return 0;
            }
        }


/*
        public int UpdateNotes(int id, string note, string type)
        {
            return NoteProcess.UpdateNote(id,note,type);
        }

        public string GetNotes(int id, string type)
        {
            return NoteProcess.GetNote(id, type);
        }
*/

        public List<Bin> GetBinCollectionByRig(Rig rig)
        {
            return RigBoardProcess.GetBinCollectionByRig(rig);
        }

        public List<BinInformation> GetBinInformationsByRigId(int rigId)
        {
            List<BinInformation> rigBinSections = rigId==0? new List<BinInformation>()
                :eServiceOnlineGateway.Instance.GetBinInformationsByRigId(rigId);

            foreach (var rigBinSection in rigBinSections)
            {
                rigBinSection.Bin = eServiceOnlineGateway.Instance.GetBinById(rigBinSection.Bin.Id);
            }

            return rigBinSections;
        }

        public List<ClientCompany> GetClientCompanyInfo()
        {
            return eServiceOnlineGateway.Instance.GetClientCompanyInfo();
        }

        public DrillingCompany GetDrillingCompanyById(int id)
        {
            return eServiceOnlineGateway.Instance.GetDrillingCompanyById(id);
        }

        public List<DrillingCompany> GetDrillingCompanyInfo()
        {
            return eServiceOnlineGateway.Instance.GetDrillingCompanyInfo();
        }

        public static void CopyFromCallSheetHeader(ProductHaulLoad productHaulLoad, CallSheetHeader callSheetHeader)
        {
            if (productHaulLoad == null) throw new ArgumentNullException("productHaulLoad");
            if (callSheetHeader != null)
            {
                productHaulLoad.CallSheetId = callSheetHeader.Id;
                productHaulLoad.CallSheetNumber = callSheetHeader.CallSheetNumber;
                productHaulLoad.JobType =  CacheData.JobTypes.FirstOrDefault(p=>p.Id==callSheetHeader.WellInformation.JobData.JobType.Id);
                productHaulLoad.JobDate = callSheetHeader.HeaderDetails.FirstCall.ExpectedTimeOnLocation;
                productHaulLoad.WellLocation = callSheetHeader.HeaderDetails.WellLocationInformation.WellLocation;
                productHaulLoad.Customer = CacheData.AllClientCompanies.FirstOrDefault(p=>p.Id==callSheetHeader.HeaderDetails
                    .CompanyInformation
                    .CustomerId);
                productHaulLoad.ClientRepresentative =callSheetHeader.HeaderDetails.FirstCall.AlertByConsultant1;
                productHaulLoad.ServicePoint = CacheData.ServicePointCollections.FirstOrDefault(p=>p.Id==callSheetHeader.WellInformation.JobData.ServicePoint.Id);
                if (productHaulLoad.IsGoWithCrew)
                    productHaulLoad.ExpectedOnLocationTime = productHaulLoad.JobDate;
            }
        }

        public static void CopyFromRigJob(ProductHaulLoad productHaulLoad, RigJob rigJob)
        {
            if (productHaulLoad == null) throw new ArgumentNullException("productHaulLoad");
            if (rigJob != null)
            {
                productHaulLoad.CallSheetId = rigJob.CallSheetId;
                productHaulLoad.CallSheetNumber = rigJob.CallSheetNumber;
                productHaulLoad.JobType = rigJob.JobType;
                productHaulLoad.JobDate = rigJob.JobDateTime;
                productHaulLoad.WellLocation = rigJob.WellLocation;
                productHaulLoad.Customer = rigJob.ClientCompany;
                productHaulLoad.ClientRepresentative =Utility.GetClientRepresentative(rigJob);
                productHaulLoad.ServicePoint = rigJob.ServicePoint;
                if (productHaulLoad.IsGoWithCrew)
                    productHaulLoad.ExpectedOnLocationTime = productHaulLoad.JobDate;
            }
        }

        public List<ProductHaulLoad> GetProductHaulLoadByBinId(int binId, int podIndex)
        {
            return eServiceOnlineGateway.Instance.GetProductHaulLoadByBinId(binId, podIndex);
        }
        public List<ProductHaulLoad> GetProductHaulLoadByBinId(int binId, Collection<int> productHaulLoadLifeStatuses)
        {
            return eServiceOnlineGateway.Instance.GetProductHaulLoadByBinId(binId, productHaulLoadLifeStatuses);
        }

        // Dec 27, 2023 zhangyuan 243_PR_AddBlendDropdown:Add blendId Parameter
        public void UpdateQuantity(int binId, double quantity, string notes, string username,int podIndex,int blendId)
        {
           BinBoardProcess.UpdateQuantity(binId,quantity, notes, username,podIndex, blendId);
        }

        public void UpdateCapacity(int binId, double capacity,int podIndex)
        {
           BinBoardProcess.UpdateCapacity(binId,capacity,podIndex);
        }

        public void UpdateBinformationBlend(int binId, int blendId, string username,int podIndex)
        {
           BinBoardProcess.UpdateBlend(binId,blendId,podIndex, username);
        }

        public DateTime GetRigJobAssginCrewEndTime(int rigJobId)
        {
            return RigBoardProcess.GetRigJobAssignCrewEndTime(rigJobId);

        }

        public int CreateServicePointNote(ServicePointNote servicePointNote)
        {
            return eServiceOnlineGateway.Instance.CreateServicePointNote(servicePointNote);
        }

        public ServicePointNote GetServicePointNoteByServicePointId(int servicePointId)
        {
            return eServiceOnlineGateway.Instance.GetServicePointNoteByServicePointId(servicePointId);
        }

        public int UpdateServicePointNote(int servicePointId, string notes)
        {
            ServicePointNote servicePointNote = eServiceOnlineGateway.Instance.GetServicePointNoteByServicePointId(servicePointId);
            if (servicePointNote != null)
            {
                servicePointNote.Description = notes;
                return eServiceOnlineGateway.Instance.UpdateServicePointNote(servicePointNote);

            }
            else
            {
                if (!string.IsNullOrEmpty(notes))
                {
                    ServicePointNote pointNote = new ServicePointNote();
                    ServicePoint servicePoint = eServiceOnlineGateway.Instance.GetServicePointById(servicePointId);
                    pointNote.ServicePoint = servicePoint;
                    pointNote.Description = notes;
                    pointNote.Name = servicePoint.Name + " Notes";
                    return eServiceOnlineGateway.Instance.CreateServicePointNote(pointNote);
                }
                return 0;
            }
        }

        public PlugLoadingHeadInformation GetPlugLoadingHeadInformationByPlugLoadingHeadId(int plugLoadingHeadId)
        {
            return eServiceOnlineGateway.Instance.GetPlugLoadingHeadInformationByPlugLoadingHeadId(plugLoadingHeadId);
        }

        public List<PlugLoadingHead> GetPlugLoadingHeadsByServicePoint(int pageSize, int pageNumber, Collection<int> servicePoints, out int count)
        {
            return PlugLoadingHeadBoardProcess.GetPlugLoadingHeadsByServicePoint(pageSize, pageNumber, servicePoints,out count);
        }

        public void UpdatePlugLoadingHeadNote(int plugLoadingHeadId, string notes)
        {
          PlugLoadingHeadBoardProcess.UpdateNotes(plugLoadingHeadId,notes);
        }

        public ManifoldInformation GetManifoldInformationByManifoldId(int manifoldId)
        {
            return eServiceOnlineGateway.Instance.GetManifoldInformationByManifoldId(manifoldId);
        }

        public List<Manifold> GetManifoldsByServicePoint(int pageSize, int pageNumber, Collection<int> servicePoints, out int count)
        {
            return ManifoldProcess.GetManifoldsByServicePoint(pageSize,pageNumber,servicePoints,out count);
        }

        public void UpdateManifoldNote(int manifoldId, string notes)
        {
           ManifoldProcess.UpdateNotes(manifoldId,notes);
        }

        public TopDrivceAdaptorInformation GetTopDrivceAdaptorInformationByTopDrivceAdaptorId(int topDrivceAdaptorId)
        {
            return eServiceOnlineGateway.Instance.GetTopDrivceAdaptorInformationByTopDrivceAdaptorId(topDrivceAdaptorId);
        }

        public List<TopDriveAdaptor> GeTopDriveAdaptorByServicePoint(int pageSize, int pageNumber, Collection<int> servicePoints, out int count)
        {
            return TopDrivceAdaptorProcess.GeTopDriveAdaptorByServicePoint(pageSize, pageNumber, servicePoints, out count);
        }

        public void UpdateTopDriveAdaptorNotes(int topDriveAdaptorId, string notes)
        {
           TopDrivceAdaptorProcess.UpdateNotes(topDriveAdaptorId,notes);
        }

        public PlugLoadingHeadSubInformation GetPlugLoadingHeadSubInformationByPlugLoadingHeadSubId(int plugLoadingHeadSubId)
        {
            return eServiceOnlineGateway.Instance.GetPlugLoadingHeadSubInformationByPlugLoadingHeadSubId(plugLoadingHeadSubId);
        }

        public List<PlugLoadingHeadSub> GetPlugLoadingHeadSubsByServicePoint(int pageSize, int pageNumber, Collection<int> servicePoints, out int count)
        {
            return PlugLoadingHeadSubBoardProcess.GetPlugLoadingHeadSubsByServicePoint(pageSize, pageNumber, servicePoints, out count);
        }

        public void UpdatePlugLoadingHeadSubNotes(int plugLoadingHeadSubId, string notes)
        {
            PlugLoadingHeadSubBoardProcess.UpdateNotes(plugLoadingHeadSubId, notes);
        }

        public List<PlugLoadingHead> GetPlugLoadingHeadsByServicePoint(int servicePointId)
        {
            return PlugLoadingHeadBoardProcess.GetPlugLoadingHeadsByServicePoint(servicePointId);
        }

        public List<PlugLoadingHeadSub> GetPlugLoadingHeadSubsByServicePoint(int servicePointId)
        {
            return PlugLoadingHeadSubBoardProcess.GetPlugLoadingHeadSubsByServicePoint(servicePointId);
        }

        public List<TopDriveAdaptor> GetTopDriveAdaptorsByServicePoint(int servicePointId)
        {
            return TopDrivceAdaptorProcess.GetTopDriveAdaptorsByServicePoint(servicePointId);
        }

        public List<Manifold> GetManifoldsByServicePoint(int servicePointId)
        {
            return ManifoldProcess.GetManifoldsByServicePoint(servicePointId);
        }

        public Manifold PlugLoadingHeadhasManifold(int plugLoadingHeadId)
        {
            return ManifoldProcess.PlugLoadingHeadhasManifold(plugLoadingHeadId);
        }

        public void AssignPlugLoadingHead(AssginPlugLoadingHeadModel model)
        {
            PlugLoadingHeadInformation plugLoadingHeadInformation = eServiceOnlineGateway.Instance.GetPlugLoadingHeadInformationByPlugLoadingHeadId(model.PlugLoadingHeadId);
            if (plugLoadingHeadInformation!=null)
            {
                StructPlugLoadingHeadInformation(model, plugLoadingHeadInformation);
                eServiceOnlineGateway.Instance.UpdatePlugLoadingHeadInformation(plugLoadingHeadInformation);
            }
            else
            {
                PlugLoadingHeadInformation newPlugLoadingHeadInformation=new PlugLoadingHeadInformation();
                StructPlugLoadingHeadInformation(model, newPlugLoadingHeadInformation);
                eServiceOnlineGateway.Instance.CreatePlugLoadingHeadInformation(newPlugLoadingHeadInformation);

            }
        }

        public List<PlugLoadingHeadInformation> GetPlugLoadingHeadInformationByRigJobId(int rigJobId)
        {
            List<PlugLoadingHeadInformation> plugLoadingHeadInformations= eServiceOnlineGateway.Instance.GetPlugLoadingHeadInformationByRigJobId(rigJobId);
            foreach (var plugLoadingHeadInformation in plugLoadingHeadInformations)
            {
                plugLoadingHeadInformation.PlugLoadingHead = eServiceOnlineGateway.Instance.GetPlugLoadingHeadById(plugLoadingHeadInformation.PlugLoadingHead.Id);
                plugLoadingHeadInformation.Manifold = eServiceOnlineGateway.Instance.GetManifoldById(plugLoadingHeadInformation.Manifold.Id);
                plugLoadingHeadInformation.TopDriveAdaptor = eServiceOnlineGateway.Instance.GetTopDriveAdaptorById(plugLoadingHeadInformation.TopDriveAdaptor.Id);
                plugLoadingHeadInformation.PlugLoadingHeadSub = eServiceOnlineGateway.Instance.GetPlugLoadingHeadSubById(plugLoadingHeadInformation.PlugLoadingHeadSub.Id);
            }

            return plugLoadingHeadInformations;
        }

        public void ReturnEquipments(List<ReturnEquipentsModel> models)
        {
            foreach (var model in models)
            {
                if (model.WhetherToReturn)
                {
                    if (model.EquipentType==EquipentType.PlugLoadingHead)
                    {
                        ReturnPlugLoadingHead(model);
                    }

                    if (model.EquipentType==EquipentType.PlugLoadingHeadSub)
                    {
                        ReturnPlugLoadingHeadSub(model);
                    }

                    if (model.EquipentType==EquipentType.TopDrivceAdaptor)
                    {
                        ReturnTopDriveAdaptor(model);
                    }

                    if (model.EquipentType==EquipentType.Nubbin)
                    {
                        ReturnNubbin(model);
                    }

                    if (model.EquipentType==EquipentType.Swedge)
                    {
                        ReturnSwedge(model);
                    }

                    if (model.EquipentType==EquipentType.WitsBox)
                    {
                        ReturnWitsBox(model);
                    }
                }
            }
        }

        public List<NubbinInformation> GetNubbinInformationByRigJobId(int rigJobId)
        {
            List<NubbinInformation> nubbinInformations= eServiceOnlineGateway.Instance.GetNubbinInformationByRigJobId(rigJobId);
            foreach (var nubbinInformation in nubbinInformations)
            {
                nubbinInformation.Nubbin = eServiceOnlineGateway.Instance.GetNubbinById(nubbinInformation.Nubbin.Id);
            }

            return nubbinInformations;
        }

        public List<SwedgeInformation> GetSwedgeInformationByRigJobId(int rigJobId)
        {
            List<SwedgeInformation> swedgeInformations= eServiceOnlineGateway.Instance.GetSwedgeInformationByRigJobId(rigJobId);
            foreach (var swedgeInformation in swedgeInformations)
            {
                swedgeInformation.Swedge = eServiceOnlineGateway.Instance.GetSwedgeById(swedgeInformation.Swedge.Id);
            }
            return swedgeInformations;
        }

        public List<WitsBoxInformation> GetWitsBoxInformationByRigJobId(int rigJobId)
        {
            List<WitsBoxInformation> witsBoxInformations= eServiceOnlineGateway.Instance.GetWitsBoxInformationByRigJobId(rigJobId);
            foreach (var witsBoxInformation in witsBoxInformations)
            {
                witsBoxInformation.WitsBox = eServiceOnlineGateway.Instance.GetWitsBoxById(witsBoxInformation.WitsBox.Id);
            }
            return witsBoxInformations;
        }

        private void ReturnNubbin(ReturnEquipentsModel model)
        {
            NubbinInformation nubbinInformation = eServiceOnlineGateway.Instance.GetNubbinInformationByNubbinId(model.Id);
            nubbinInformation.EquipmentStatus = EquipmentStatus.Yard;
            nubbinInformation.WorkingServicePoint = eServiceOnlineGateway.Instance.GetServicePointById(model.ServicePointId);
            nubbinInformation.RigJob = null;
            nubbinInformation.CallSheetNumber = 0;
            nubbinInformation.Location = null;
            eServiceOnlineGateway.Instance.UpdateNubbinInformation(nubbinInformation);
        }

        private void ReturnSwedge(ReturnEquipentsModel model)
        {
            SwedgeInformation swedgeInformation = eServiceOnlineGateway.Instance.GetSwedgeInformationBySwedgeId(model.Id);
            swedgeInformation.EquipmentStatus = EquipmentStatus.Yard;
            swedgeInformation.WorkingServicePoint = eServiceOnlineGateway.Instance.GetServicePointById(model.ServicePointId);
            swedgeInformation.RigJob = null;
            swedgeInformation.CallSheetNumber = 0;
            swedgeInformation.Location = null;
            eServiceOnlineGateway.Instance.UpdateSwedgeInformation(swedgeInformation);
        }

        private void ReturnWitsBox(ReturnEquipentsModel model)
        {
            WitsBoxInformation witsBoxInformation = eServiceOnlineGateway.Instance.GetWitsBoxInformationByWitsBoxId(model.Id);
            witsBoxInformation.EquipmentStatus = EquipmentStatus.Yard;
            witsBoxInformation.WorkingServicePoint = eServiceOnlineGateway.Instance.GetServicePointById(model.ServicePointId);
            witsBoxInformation.RigJob = null;
            witsBoxInformation.CallSheetNumber = 0;
            witsBoxInformation.Location = null;
            eServiceOnlineGateway.Instance.UpdateWitsBoxInformation(witsBoxInformation);
        }

        private void ReturnPlugLoadingHead(ReturnEquipentsModel model)
        {
            PlugLoadingHeadInformation plugLoadingHeadInformation = eServiceOnlineGateway.Instance.GetPlugLoadingHeadInformationByPlugLoadingHeadId(model.Id);
            plugLoadingHeadInformation.EquipmentStatus = EquipmentStatus.Yard;
            plugLoadingHeadInformation.WorkingServicePoint = eServiceOnlineGateway.Instance.GetServicePointById(model.ServicePointId);
            plugLoadingHeadInformation.PlugLoadingHeadSub = null;
            plugLoadingHeadInformation.TopDriveAdaptor = null;
            plugLoadingHeadInformation.RigJob = null;
            plugLoadingHeadInformation.CallsheetNumber = 0;
            plugLoadingHeadInformation.Location = null;
            eServiceOnlineGateway.Instance.UpdatePlugLoadingHeadInformation(plugLoadingHeadInformation);

            ReturnManifold(plugLoadingHeadInformation.Manifold?.Id ?? 0, model.ServicePointId);           
        }

        private void ReturnManifold(int manifoldId, int servicePointId)
        {
            ManifoldInformation manifoldInformation = eServiceOnlineGateway.Instance.GetManifoldInformationByManifoldId(manifoldId);
            manifoldInformation.WorkingServicePoint = eServiceOnlineGateway.Instance.GetServicePointById(servicePointId);
            manifoldInformation.CallsheetNumber = 0;
            manifoldInformation.Location = null;
            eServiceOnlineGateway.Instance.UpdateManifoldInformation(manifoldInformation);
        }

        private void ReturnPlugLoadingHeadSub(ReturnEquipentsModel model)
        {
            PlugLoadingHeadSubInformation plugLoadingHeadSubInformation = eServiceOnlineGateway.Instance.GetPlugLoadingHeadSubInformationByPlugLoadingHeadSubId(model.Id);
            plugLoadingHeadSubInformation.WorkingServicePoint = eServiceOnlineGateway.Instance.GetServicePointById(model.ServicePointId);
            plugLoadingHeadSubInformation.EquipmentStatus = EquipmentStatus.Yard;
            plugLoadingHeadSubInformation.CallsheetNumber = 0;
            plugLoadingHeadSubInformation.Location = null;
            eServiceOnlineGateway.Instance.UpdatePlugLoadingHeadSubInformation(plugLoadingHeadSubInformation);
        }
        private void ReturnTopDriveAdaptor(ReturnEquipentsModel model)
        {
            TopDrivceAdaptorInformation topDrivceAdaptorInformation = eServiceOnlineGateway.Instance.GetTopDrivceAdaptorInformationByTopDrivceAdaptorId(model.Id);
            topDrivceAdaptorInformation.WorkingServicePoint = eServiceOnlineGateway.Instance.GetServicePointById(model.ServicePointId);
            topDrivceAdaptorInformation.EquipmentStatus = EquipmentStatus.Yard;
            topDrivceAdaptorInformation.Location = null;
            topDrivceAdaptorInformation.CallsheetNumber = 0;
            eServiceOnlineGateway.Instance.UpdateTopDrivceAdaptorInformation(topDrivceAdaptorInformation);
        }

        private static void StructPlugLoadingHeadInformation(AssginPlugLoadingHeadModel model, PlugLoadingHeadInformation plugLoadingHeadInformation)
        {
            plugLoadingHeadInformation.RigJob = eServiceOnlineGateway.Instance.GetRigJobById(model.RigJobId);
            Rig rig = eServiceOnlineGateway.Instance.GetRigById(plugLoadingHeadInformation.RigJob?.Rig?.Id ?? 0);
            plugLoadingHeadInformation.Location = $"{rig.Name}/{plugLoadingHeadInformation?.RigJob?.JobType?.Name}";
            if (model.PlugLoadingHeadSubRequired)
            {
                plugLoadingHeadInformation.PlugLoadingHeadSub = eServiceOnlineGateway.Instance.GetPlugLoadingHeadSubById(model.PlugLoadingHeadSubId);
                PlugLoadingHeadSubBoardProcess.AssignPlugLoadingHeadUpdatePlugLoadingHeadSub(model.PlugLoadingHeadSubId,model.ServicePointId,model.CallSheetNumber,plugLoadingHeadInformation.Location);
            }

            if (model.TopDriveAdapterRequired)
            {
                plugLoadingHeadInformation.TopDriveAdaptor = eServiceOnlineGateway.Instance.GetTopDriveAdaptorById(model.TopDriveAdapterId);
                TopDrivceAdaptorProcess.AssignPlugLoadingHeadUpdateTopDriveAdaptor(model.TopDriveAdapterId, model.ServicePointId, model.CallSheetNumber, plugLoadingHeadInformation.Location);
            }

            plugLoadingHeadInformation.PlugLoadingHead = eServiceOnlineGateway.Instance.GetPlugLoadingHeadById(model.PlugLoadingHeadId);
            plugLoadingHeadInformation.Manifold = eServiceOnlineGateway.Instance.GetManifoldById(model.ManifoldId);
            ManifoldProcess.AssignPlugLoadingHeadUpdateManifold(model.ManifoldId,model.ServicePointId,model.CallSheetNumber,plugLoadingHeadInformation.Location);        

      
            plugLoadingHeadInformation.WorkingServicePoint = eServiceOnlineGateway.Instance.GetServicePointById(model.ServicePointId);
            plugLoadingHeadInformation.CallsheetNumber = model.CallSheetNumber;
            plugLoadingHeadInformation.EquipmentStatus = EquipmentStatus.Assigned;
        }

        #region Assign Wits Box

        public void UpdateWitsBoxNotes(int witsBoxId, string notes)
        {
           WitsBoxBoardProcess.UpdateNotes(witsBoxId,notes);
        }

        public void AssignWitsBoxToRigJob(WitsBoxModel witsBoxModel)
        {
            WitsBoxInformation witsBoxInformation = eServiceOnlineGateway.Instance.GetWitsBoxInformationByWitsBoxId(witsBoxModel.Id);
            if (witsBoxInformation == null)
            {
                witsBoxInformation = this.BuildWitsBoxInformation(witsBoxModel, null);
                eServiceOnlineGateway.Instance.CreateWitsBoxInformation(witsBoxInformation);
            }
            else
            {
                witsBoxInformation = this.BuildWitsBoxInformation(witsBoxModel, witsBoxInformation);
                eServiceOnlineGateway.Instance.UpdateWitsBoxInformation(witsBoxInformation);
            }
        }

        private WitsBoxInformation BuildWitsBoxInformation(WitsBoxModel model, WitsBoxInformation witsBoxInformation)
        {
            if(witsBoxInformation == null) witsBoxInformation = new WitsBoxInformation();
            witsBoxInformation.WitsBox = eServiceOnlineGateway.Instance.GetWitsBoxById(model.Id);
            RigJob rigJob = eServiceOnlineGateway.Instance.GetRigJobById(model.RigJobId);
            witsBoxInformation.RigJob = rigJob;
            witsBoxInformation.Location = $"{rigJob.Rig?.Name}/{rigJob.JobType?.Name}";
            witsBoxInformation.WorkingServicePoint = eServiceOnlineGateway.Instance.GetServicePointById(model.ServicePointId);
            witsBoxInformation.EquipmentStatus = EquipmentStatus.Assigned;
            witsBoxInformation.CallSheetNumber = model.CallSheetNumber;

            return witsBoxInformation;
        }

        public List<WitsBox> GetWitsBoxListByServicePoint(int servicePointId)
        {
            List<WitsBox> witsBoxList = new List<WitsBox>();
            List<WitsBox> witsBoxes = EServiceReferenceData.Data.WitsBoxCollection.Where(p => p.HomeServicePoint.Id == servicePointId).ToList();
            foreach (WitsBox witsBox in witsBoxes)
            {
                WitsBoxInformation witsBoxInformation = eServiceOnlineGateway.Instance.GetWitsBoxInformationByWitsBoxId(witsBox.Id);
                if(witsBoxInformation == null) witsBoxList.Add(witsBox);
            }

            List<WitsBoxInformation> witsBoxInformations = eServiceOnlineGateway.Instance.GetWitsBoxInformationByServicePointAndEquipmentStatus(servicePointId, EquipmentStatus.Yard);
            foreach (WitsBoxInformation witsBoxInformation in witsBoxInformations)
            {
                witsBoxList.Add(witsBoxInformation.WitsBox);
            }

            return witsBoxList;
        }

        #endregion

        #region Assign Nubbin

        public List<Nubbin> GetEffectiveNubbinsByServicePoint(int servicePointId)
        {
            List<Nubbin> nubbinList = new List<Nubbin>();
            List<Nubbin> nubbins = eServiceOnlineGateway.Instance.GetAllNubbins().Where(p => p.HomeServicePoint.Id == servicePointId).ToList();
            foreach (Nubbin nubbin in nubbins)
            {
                NubbinInformation nubbinInformation = eServiceOnlineGateway.Instance.GetNubbinInformationByNubbinId(nubbin.Id);
                if (nubbinInformation == null) nubbinList.Add(nubbin);
            }

            List<NubbinInformation> nubbinInformations = eServiceOnlineGateway.Instance.GetNubbinInformationByServicePointAndEquipmentStatus(servicePointId, EquipmentStatus.Yard);
            foreach (NubbinInformation nubbinInformation in nubbinInformations)
            {
                Nubbin nubbin = eServiceOnlineGateway.Instance.GetNubbinById(nubbinInformation.Nubbin.Id);
                nubbinList.Add(nubbin);
            }

            return nubbinList;
        }

        public void AssignNubbinToRigJob(NubbinModel model)
        {
            NubbinInformation nubbinInformation = eServiceOnlineGateway.Instance.GetNubbinInformationByNubbinId(model.Id);
            if (nubbinInformation == null)
            {
                nubbinInformation = this.BuildNubbinInformation(model, null);
                eServiceOnlineGateway.Instance.CreateNubbinInformation(nubbinInformation);
            }
            else
            {
                nubbinInformation = this.BuildNubbinInformation(model, nubbinInformation);
                eServiceOnlineGateway.Instance.UpdateNubbinInformation(nubbinInformation);
            }
        }

        private NubbinInformation BuildNubbinInformation(NubbinModel model, NubbinInformation nubbinInformation)
        {
            if (nubbinInformation == null) nubbinInformation = new NubbinInformation();
            nubbinInformation.Nubbin = eServiceOnlineGateway.Instance.GetNubbinById(model.Id);
            RigJob rigJob = eServiceOnlineGateway.Instance.GetRigJobById(model.RigJobId);
            nubbinInformation.RigJob = rigJob;
            nubbinInformation.Location = $"{rigJob.Rig?.Name}/{rigJob.JobType?.Name}";
            nubbinInformation.WorkingServicePoint = eServiceOnlineGateway.Instance.GetServicePointById(model.ServicePointId);
            nubbinInformation.EquipmentStatus = EquipmentStatus.Assigned;
            nubbinInformation.CallSheetNumber = model.CallSheetNumber;

            return nubbinInformation;
        }

        #endregion

        #region Assign Swedge

        public List<Swedge> GetEffectiveSwedgesByServicePoint(int servicePointId)
        {
            List<Swedge> swedgeList = new List<Swedge>();
            List<Swedge> swedges = eServiceOnlineGateway.Instance.GetAllSwedge().Where(p => p.HomeServicePoint.Id == servicePointId).ToList();
            foreach (Swedge swedge in swedges)
            {
                SwedgeInformation swedgeInformation = eServiceOnlineGateway.Instance.GetSwedgeInformationBySwedgeId(swedge.Id);
                if (swedgeInformation == null) swedgeList.Add(swedge);
            }

            List<SwedgeInformation> swedgeInformations = eServiceOnlineGateway.Instance.GetSwedgeInformationByServicePointAndEquipmentStatus(servicePointId, EquipmentStatus.Yard);
            foreach (SwedgeInformation swedgeInformation in swedgeInformations)
            {
                Swedge swedge = eServiceOnlineGateway.Instance.GetSwedgeById(swedgeInformation.Swedge.Id);
                swedgeList.Add(swedge);
            }

            return swedgeList;
        }

        public void AssignSwedgeToRigJob(SwedgeModel model)
        {
            SwedgeInformation swedgeInformation = eServiceOnlineGateway.Instance.GetSwedgeInformationBySwedgeId(model.Id);
            if (swedgeInformation == null)
            {
                swedgeInformation = this.BuildSwedgeInformation(model, null);
                eServiceOnlineGateway.Instance.CreateSwedgeInformation(swedgeInformation);
            }
            else
            {
                swedgeInformation = this.BuildSwedgeInformation(model, swedgeInformation);
                eServiceOnlineGateway.Instance.UpdateSwedgeInformation(swedgeInformation);
            }
        }

        public List<ProductHaulLoad> GetProductHaulLoadCollectionByServicePoint(int servicePointId)
        {
            return eServiceOnlineGateway.Instance.GetProductHaulLoadCollectionByServicePoint(servicePointId, ProductHaulLoadStatus.Scheduled);
        }

        public List<ProductHaulLoad> GetScheduledProductHaulLoadsByServicePoint(int servicePointId)
        {
            return eServiceOnlineGateway.Instance.GetProductHaulLoadCollectionByServicePoint(servicePointId, ProductHaulLoadStatus.Scheduled);
        }

        public List<ProductHaulLoad> GetBlendingProductHaulLoadsByServicePoint(int servicePointId)
        {
            return eServiceOnlineGateway.Instance.GetProductHaulLoadCollectionByServicePoint(servicePointId, ProductHaulLoadStatus.Blending);
        }

        public List<ProductHaulLoad> GetLoadedProductHaulLoadsByServicePoint(int servicePointId)
        {
            return eServiceOnlineGateway.Instance.GetProductHaulLoadCollectionByServicePoint(servicePointId, ProductHaulLoadStatus.Loaded);
        }
        public List<ProductHaulLoad> GetBlendCompletedProductHaulLoadsByServicePoint(int servicePointId)
        {
            return eServiceOnlineGateway.Instance.GetProductHaulLoadCollectionByServicePoint(servicePointId, ProductHaulLoadStatus.BlendCompleted);
        }

        public int SetProductHaulLoadBlending(int productLoadId)
        {
            ProductHaulLoad productHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productLoadId);
            if (productHaulLoad.ProductHaulLoadLifeStatus == ProductHaulLoadStatus.Scheduled)
                productHaulLoad.ProductHaulLoadLifeStatus = ProductHaulLoadStatus.Blending;
            else
                return 0;
            return eServiceOnlineGateway.Instance.UpdateProductHaulLoad(productHaulLoad);
        }

        public int SetProductHaulLoadLoaded(int productLoadId)
        {
            ProductHaulLoad productHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productLoadId);
            productHaulLoad.ProductHaulLoadLifeStatus = ProductHaulLoadStatus.Loaded;
            return eServiceOnlineGateway.Instance.UpdateProductHaulLoad(productHaulLoad);
        }

        public int SetProductHaulLoadBlendCompleted(int productLoadId)
        {
            ProductHaulLoad productHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productLoadId);
            productHaulLoad.ProductHaulLoadLifeStatus = ProductHaulLoadStatus.BlendCompleted;
            return eServiceOnlineGateway.Instance.UpdateProductHaulLoad(productHaulLoad);
        }

        public string SetBlendCompletedProductHaulLoadToScheduled(int productLoadId)
        {
            var productHaulload = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productLoadId);
            if (productHaulload.ProductHaulLoadLifeStatus != ProductHaulLoadStatus.BlendCompleted)
                return "The product haul load is not Blend Completed status";
            productHaulload.ProductHaulLoadLifeStatus = ProductHaulLoadStatus.Scheduled;
            var success = eServiceOnlineGateway.Instance.UpdateProductHaulLoad(productHaulload);
            return success == 1 ? "Success" : "Failed";
        }

        public string SetBlendingProductHaulLoadToScheduled(int productLoadId)
        {
            var productHaulload = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productLoadId);
            if (productHaulload.ProductHaulLoadLifeStatus != ProductHaulLoadStatus.Blending)
                return "The product haul load is not Blend Completed status";
            productHaulload.ProductHaulLoadLifeStatus = ProductHaulLoadStatus.Scheduled;
            var success = eServiceOnlineGateway.Instance.UpdateProductHaulLoad(productHaulload);
            return success == 1 ? "Success" : "Failed";
        }

        public string SetLoadedProductHaulLoadToScheduled(int productLoadId)
        {
            var productHaulload = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productLoadId);
            if (productHaulload.ProductHaulLoadLifeStatus != ProductHaulLoadStatus.Loaded)
                return "The product haul load is not Blend Completed status";
            productHaulload.ProductHaulLoadLifeStatus = ProductHaulLoadStatus.Scheduled;
            var success = eServiceOnlineGateway.Instance.UpdateProductHaulLoad(productHaulload);
            return success == 1 ? "Success" : "Failed";
        }

        public string SetProductHaulLoadScheduled(int productLoadId)
        {
            var productHaulload = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productLoadId);
            productHaulload.ProductHaulLoadLifeStatus = ProductHaulLoadStatus.Scheduled;
            var success = eServiceOnlineGateway.Instance.UpdateProductHaulLoad(productHaulload);
            return success == 1 ? "Success" : "Failed";
        }

        private SwedgeInformation BuildSwedgeInformation(SwedgeModel model, SwedgeInformation swedgeInformation)
        {
            if (swedgeInformation == null) swedgeInformation = new SwedgeInformation();
            swedgeInformation.Swedge = eServiceOnlineGateway.Instance.GetSwedgeById(model.Id);
            RigJob rigJob = eServiceOnlineGateway.Instance.GetRigJobById(model.RigJobId);
            swedgeInformation.RigJob = rigJob;
            swedgeInformation.Location = $"{rigJob.Rig?.Name}/{rigJob.JobType?.Name}";
            swedgeInformation.WorkingServicePoint = eServiceOnlineGateway.Instance.GetServicePointById(model.ServicePointId);
            swedgeInformation.EquipmentStatus = EquipmentStatus.Assigned;
            swedgeInformation.CallSheetNumber = model.CallSheetNumber;

            return swedgeInformation;
        }

        #endregion

        #endregion

        #region Bin Board

        public List<Bin> GetBinsByServicePoint(int pageSize, int pageNumber, Collection<int> servicePoints, out int count)
        {
            return BinBoardProcess.GetBinsByServicePoint(pageSize, pageNumber, servicePoints, out count);
        }
        public List<BinInformation> GetBinInformationsByServicePoint(int pageSize, int pageNumber, Collection<int> servicePoints, out int count)
        {
            return BinBoardProcess.GetBinInformationsByServicePoint(pageSize, pageNumber, servicePoints, out count);
        }
        public BinInformation GetBinInformationByBinId(int binId,int podIndex)
        {
            return eServiceOnlineGateway.Instance.GetBinInformationByBinIdAndPodIndex(binId,podIndex);
        }
        /*
        public BinInformation GetBinInformationByBinNumber(string binNumber)
        {
            return eServiceOnlineGateway.Instance.GetBinInformationByBinNumber(binNumber);
        }
        */

        public BinNote GetBinNoteByBinAndPodIndex(Bin bin,int podIndex)
        {
            return eServiceOnlineGateway.Instance.GetBinNoteByBinAndPodIndex(bin, podIndex);
        }

        public int UpdateBinNote(int binId, string notes,int podIndex)
        {
            BinNote binNote = eServiceOnlineGateway.Instance.GetBinNoteByBinAndPodIndex(new Bin{Id = binId }, podIndex);
            if (binNote != null)
            {
                binNote.Description = notes;
                binNote.Name = notes;

                return eServiceOnlineGateway.Instance.UpdateBinNote(binNote);
            }
            else
            {
                if (!string.IsNullOrEmpty(notes))
                {
                    BinNote newBinNote = new BinNote();
                    Bin bin = eServiceOnlineGateway.Instance.GetBinById(binId);
                    newBinNote.Bin = bin;
                    newBinNote.Description = notes;
                    newBinNote.Name = notes;
                    newBinNote.PodIndex = podIndex;
                    return eServiceOnlineGateway.Instance.CreateBinNote(newBinNote);
                }

                return 0;
            }
        }

        #endregion

        #region nubinsBoard

        public List<Nubbin> GetNubbinsByServicePoint(int pageSize, int pageNumber, Collection<int> servicePoints, out int count)
        {
            return NubbinBoardProcess.GetNubinsByServicePoint(pageSize, pageNumber, servicePoints, out count);
        }

        public NubbinInformation GetNubbinInformationByNubbinId(int nubbinId)
        {
            return eServiceOnlineGateway.Instance.GetNubbinInformationByNubbinId(nubbinId);
        }

        public void UpdateNubbinNotes(int nubbinId, string notes)
        {
            NubbinBoardProcess.UpdateNotes(nubbinId,notes);
        }

        public List<Swedge> GetSwedgeByServicePoint(int pageSize, int pageNumber, Collection<int> servicePoints, out int count)
        {
            return SwedgeBoardProcess.GetSwedgesByServicePoint(pageSize,pageNumber,servicePoints,out count);
        }

        public SwedgeInformation GetSwedgeInformationBySwedgeId(int swedgeId)
        {
            return eServiceOnlineGateway.Instance.GetSwedgeInformationBySwedgeId(swedgeId);
        }

        public void UpdateSwedgeNotes(int swedgeId, string notes)
        {
           SwedgeBoardProcess.UpdateNotes(swedgeId,notes);
        }

        public List<WitsBox> GetWitsBoxByServicePoint(int pageSize, int pageNumber, Collection<int> servicePoints, out int count)
        {
            return WitsBoxBoardProcess.GetWitsBoxsByServicePoint(pageSize, pageNumber, servicePoints, out count);
        }

        public WitsBoxInformation GetWitsBoxInformationByWitsBoxId(int witsBoxId)
        {
            return eServiceOnlineGateway.Instance.GetWitsBoxInformationByWitsBoxId(witsBoxId);
        }

        #endregion

        public BinInformation GetBinInformationById(int id)
        {
            return eServiceOnlineGateway.Instance.GetBinInformationById(id);
        }

        public void LoadBlendToBin(string loggedUser, int modelProductHaulLoadId)
        {
            var productHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(modelProductHaulLoadId);
            BinProcess.LoadBlendToBin(productHaulLoad.Bin, productHaulLoad.PodIndex, productHaulLoad.BlendChemical.Name,
                productHaulLoad.BlendChemical.Description, productHaulLoad.TotalBlendWeight, productHaulLoad, null, loggedUser);
            BinProcess.CreateShippingLoadSheetByLoadBlendToBin(loggedUser,productHaulLoad);

            productHaulLoad.ProductHaulLoadLifeStatus = ProductHaulLoadStatus.Stored;
            productHaulLoad.ModifiedUserName = loggedUser;
            eServiceOnlineGateway.Instance.UpdateProductHaulLoad(productHaulLoad);
        }

        public void UpdateCallSheetAndRigJob(int rigJobId)
        {
            RigJob oldRigJob = eServiceWebContext.Instance.GetRigJobById(rigJobId);
            if (oldRigJob != null)
            {
                var oldCallSheetStatus = EServiceEntityStatus.Empty;

                var oldRigJobStatus = oldRigJob.JobLifeStatus;
                CallSheet newcallSheet = eServiceOnlineGateway.Instance.GetCallSheetById(oldRigJob.CallSheetId);

                oldCallSheetStatus = newcallSheet.Status;

                CallSheetValidation.ValidateEntity(newcallSheet);

                if (oldCallSheetStatus != newcallSheet.Status)
                    eServiceWebContext.Instance.UpdateCallSheet(newcallSheet, newcallSheet);

                //TODO: The rig job status should be handled by one single method globally
                RigJob newRigJob = oldRigJob;
                newRigJob.IsCoDCleared = newcallSheet.Header.IsCODCleared;

                if (oldRigJobStatus == JobLifeStatus.Confirmed)
                {
                    if (newcallSheet.Status == EServiceEntityStatus.Ready)
                        newRigJob.JobLifeStatus = JobLifeStatus.Scheduled;
                }

                if (oldRigJobStatus == JobLifeStatus.Scheduled)
                {
                    if (newcallSheet.Status == EServiceEntityStatus.InProgress)
                        newRigJob.JobLifeStatus = JobLifeStatus.Confirmed;
                }

                if (newRigJob.JobLifeStatus != oldRigJobStatus)
                    RigBoardProcess.UpdateRigJob(newRigJob);
            }
        }

        public void AlignPumperCrewAssignment(int rigJobId)
        {
            RigJob rigJob = GetRigJobById(rigJobId);

            //Delete all existing pumper unit sections

            Collection<UnitSection> callUnitSections =
                eServiceOnlineGateway.Instance.GetUnitSectionsByCallSheetId(rigJob.CallSheetId);
            //delete call sheet unit section if RigJobCrewSection doesn't exist any more
            foreach (var unitSection in callUnitSections)
            {
                if (unitSection.CrewId != null && unitSection.CrewId != 0 &&
                    (unitSection.ProductHaulId == null || unitSection.ProductHaulId == 0))
                {
                        eServiceOnlineGateway.Instance.DeleteUnitSection(unitSection);
                }
            }

            //Insert all pumper assignments to call sheet

            List<RigJobSanjelCrewSection> crewAssignments = GetRigJobCrewSectionByRigJob(rigJobId);
    
            foreach (var rigJobSanjelCrewSection in crewAssignments)
            {
                if (rigJobSanjelCrewSection.ProductHaul == null || rigJobSanjelCrewSection.ProductHaul.Id == 0)
                {
                    CrewProcess.CreatePumperUnitSection(rigJobSanjelCrewSection,
                            rigJob.CallSheetId);
                }
            }


            UpdateCallSheetAndRigJob(rigJobId);
        }

        public void ScheduleProductHaul(ScheduleProductHaulFromRigJobBlendViewModel model)
        {
            model.PopulateToModel();
            CreateBlendRequest(model.ProductLoadInfoModel);
            CreateHaulBlend(model.RigJobId, model.LoggedUser, model.ProductLoadInfoModel, model.ProductHaulInfoModel, model.ShippingLoadSheetModel,true);
        }

        public void UpdateRigJobCrewSchedule(RigJob currentRigJob)
        {
            //update Sanjel Crew Schedule
            List<RigJobSanjelCrewSection> rigJobSanjelCrewSections =
                eServiceOnlineGateway.Instance.GetRigJobCrewSectionByRigJob(currentRigJob.Id);
            foreach (var rigJobSanjelCrewSection in rigJobSanjelCrewSections)
            {
                if (rigJobSanjelCrewSection.ProductHaul != null && rigJobSanjelCrewSection.ProductHaul.Id != 0)
                {
                    var productHaul =
                        eServiceOnlineGateway.Instance.GetProductHaulById(rigJobSanjelCrewSection.ProductHaul.Id);
                    if (productHaul.IsGoWithCrew)
                    {
                        //Update bulker crew schedule;
                        UpdateSanjelCrewSchedule(currentRigJob, rigJobSanjelCrewSection);
                    }
                }
                else
                {
                    //Update pumper crew schedule
                    UpdateSanjelCrewSchedule(currentRigJob, rigJobSanjelCrewSection);
                }
            }

            //Update Third Party Crew Schedule

            List<RigJobThirdPartyBulkerCrewSection> rigJobThirdPartyBulkerCrewSections =
                eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionByRigJob(currentRigJob.Id);
            foreach (var rigJobThirdPartyBulkerCrewSection in rigJobThirdPartyBulkerCrewSections)
            {
                if (rigJobThirdPartyBulkerCrewSection.ProductHaul != null &&
                    rigJobThirdPartyBulkerCrewSection.ProductHaul.Id != 0)
                {
                    var productHaul =
                        eServiceOnlineGateway.Instance.GetProductHaulById(rigJobThirdPartyBulkerCrewSection.ProductHaul
                            .Id);
                    if (productHaul.IsGoWithCrew)
                    {
                        UpdateThirdPartyScrewSchedule(currentRigJob, rigJobThirdPartyBulkerCrewSection);
                    }
                }
            }
        }

        private void UpdateThirdPartyScrewSchedule(RigJob currentRigJob, RigJobThirdPartyBulkerCrewSection rigJobThirdPartyBulkerCrewSection)
        {
            var crewSchedule =
                eServiceOnlineGateway.Instance.GetThirdPartyBulkerCrewSchedule(rigJobThirdPartyBulkerCrewSection.ThirdPartyBulkerCrew.Id);
            SetCrewScheduleByRigJob(currentRigJob, crewSchedule);

            eServiceOnlineGateway.Instance.UpdateThirdPartyCrewSchedule(crewSchedule);
        }

        private static void SetCrewScheduleByRigJob(RigJob currentRigJob, Schedule crewSchedule)
        {
            var startTime = currentRigJob.JobLifeStatus == JobLifeStatus.Pending?currentRigJob.JobDateTime : currentRigJob.CallCrewTime;
            var endTime =
                currentRigJob.JobDateTime.AddHours(currentRigJob.JobDuration == 0 ? 6 : currentRigJob.JobDuration / 60);
            crewSchedule.StartTime = startTime;
            crewSchedule.EndTime = endTime;
        }

        private void UpdateSanjelCrewSchedule(RigJob currentRigJob, RigJobSanjelCrewSection rigJobSanjelCrewSection)
        {
            var crewSchedule =
                eServiceOnlineGateway.Instance.GetSanjelCrewSchedulesByRigJobSanjelCrewSection(rigJobSanjelCrewSection);
            SetCrewScheduleByRigJob(currentRigJob, crewSchedule);

            foreach (var workerSchedule in crewSchedule.WorkerSchedule)
            {
                SetCrewScheduleByRigJob(currentRigJob, workerSchedule);
            }

            foreach (var unitSchedule in crewSchedule.UnitSchedule)
            {
                SetCrewScheduleByRigJob(currentRigJob, unitSchedule);
            }

            eServiceOnlineGateway.Instance.UpdateSanjelCrewSchedule(crewSchedule, true);
        }
        public string SetBulkerCrewStatusByCrewId(int crewId, BulkerCrewStatus status, bool isThirdParty, string username)
        {
            string result;
            if (status == BulkerCrewStatus.Down)
            {
                result = "Succeed";
                return result;
            }
            if (!isThirdParty)
            {
                // find current start
                var currentRigJobSanjelCrew = CrewProcess.GetCurrentBulkerCrewAssignment(crewId);
                // find current start end
                if (currentRigJobSanjelCrew != null)
                {
	                if (currentRigJobSanjelCrew.ProductHaul != null)
	                {
		                result = CrewProcess.ChangeSanjelCrewStatus1(currentRigJobSanjelCrew.SanjelCrew.Id,
			                currentRigJobSanjelCrew.ProductHaul.Id, status,
			                currentRigJobSanjelCrew.RigJobCrewSectionStatus,
			                currentRigJobSanjelCrew.ProductHaul.ProductHaulLifeStatus, isThirdParty, username);
	                }
	                else
	                {
		                result = "No Product Haul is found";
	                }
                }
                else
                {
	                if (status == BulkerCrewStatus.OffDuty)
	                {
		                result = CrewProcess.ChangeSanjelCrewStatus1(crewId,
			                0,  status,RigJobCrewSectionStatus.Returned, ProductHaulStatus.Empty, isThirdParty, username);
	                }
	                else
	                {
		                result = "No assignment is found";
	                }
                }
            }
            else
            {
                // find current start
                var currentThirdPartBulkerCrew = CrewProcess.GetCurrentThirdPartyBulkerCrewAssignment(crewId);
                // find current start end
                if (currentThirdPartBulkerCrew != null)
                {
	                if (currentThirdPartBulkerCrew.ProductHaul != null)
	                {
		                //                    result = CrewProcess.ChangeThirdPartyCrewStatus(crewId, status, currentThirdPartBulkerCrew, username);
		                result = CrewProcess.ChangeSanjelCrewStatus1(currentThirdPartBulkerCrew.ThirdPartyBulkerCrew.Id,
			                currentThirdPartBulkerCrew.ProductHaul.Id, status,
			                currentThirdPartBulkerCrew.RigJobCrewSectionStatus,
			                currentThirdPartBulkerCrew.ProductHaul.ProductHaulLifeStatus, isThirdParty, username);
	                }
	                else
	                {
		                result = "No Product Haul is found";
	                }
                }
                else
                {
	                if (status == BulkerCrewStatus.OffDuty)
	                {
		                result = CrewProcess.ChangeSanjelCrewStatus1(crewId,
			                0, status, RigJobCrewSectionStatus.Returned, ProductHaulStatus.Empty, isThirdParty, username);
	                }
	                else
	                {
		                result = "No assignment is found";
	                }
                }
            }
            return result;

        }

        public List<ShippingLoadSheet> GetScheduledShippingLoadSheetsByStorage(int binInformationId)
        {
	        return eServiceOnlineGateway.Instance.GetShippingLoadSheetsBySourceStorageIdAndStatus(binInformationId,
		        ShippingStatus.Scheduled);
        }
        // Nov 14, 2023 zhangyuan P63_Q4_174:Add Update Bininformation. create BinLoadHistory.Create ShippingLoadSheet
        public void TransferBlendToBin(string loggedUser,int pageType, int binInformationId, int toBinInformationId, double quantity)
        {
            var operationTime = DateTime.Now;
            var binInformation = eServiceOnlineGateway.Instance.GetBinInformationById(binInformationId);
            if (binInformation == null) throw new Exception("Origin Bin  doesn't exist.");
            BinProcess.TransferBlendToBin(binInformation, pageType, toBinInformationId, quantity, loggedUser,
                operationTime);
        }
        //Dec 11, 2023 zhangyuan 226_PR_CalcRemainsAmount: And  calculate remains amount
        public List<ShippingLoadSheet> GetShippingLoadSheetsByProductHaulLoadId(int productHaulLoadId)
        {
            return eServiceOnlineGateway.Instance.GetShippingLoadSheetsByProductHaulLoadId(productHaulLoadId,
                ShippingStatus.Scheduled);
        }

        // Dec 25, 2023 zhangyuan 195_PR_Haulback:  Add HaulBack Logic
        #region Haul Back
        public void HaulBackFromRigJobBin(int rigJobId, string loggedUser, ProductHaulInfoModel productHaulInfoModel, ShippingLoadSheetModel shippingLoadSheetModel, HaulBackFromBinModel haulBackFromBinModel, BulkPlantBinLoadModel bulkPlantBinLoadModel)
        {
            var binInformation = eServiceOnlineGateway.Instance.GetBinInformationById(haulBackFromBinModel.SourceBinInformationId);
            
            haulBackFromBinModel.DestinationBinInformationId = bulkPlantBinLoadModel.BinInformationId;

            BuildProductHaulDetails(loggedUser, new ProductLoadInfoModel(), productHaulInfoModel, shippingLoadSheetModel, null, rigJobId, haulBackFromBinModel);
        }
        #endregion

        public WorkerSchedule GetShiftScheduleEndDate(int employeeId)
        {
            DateTime today = DateTime.Now;
	        List<WorkerSchedule> workerSchedules = eServiceOnlineGateway.Instance.GetWorkerSchedulesByQuery(p=>p.Worker.Id == employeeId && p.EndTime>today && (p.Type == WorkerScheduleType.OffShift || p.Type == WorkerScheduleType.OnDuty));
	        return  (workerSchedules == null || workerSchedules.Count == 0)?null: workerSchedules.OrderByDescending(p => p.EndTime).First();
        }

        public int ExtendShiftScheduleEndDate(int employeeId, int rotationIndex, DateTime startDateTime, DateTime endDateTime)
        {
	        return eServiceOnlineGateway.Instance.UpdateWorkRotationSchedule(employeeId, rotationIndex, startDateTime, endDateTime);
        }
    }
}