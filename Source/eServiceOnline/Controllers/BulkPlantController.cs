using eServiceOnline.Data;
using eServiceOnline.Models.Commons;
using eServiceOnline.Models.CrewBoard;
using eServiceOnline.Models.RigBoard;
using eServiceOnline.SecurityControl;
using eServiceOnline.BusinessProcess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Sanjel.BusinessEntities;
using Sanjel.BusinessEntities.Jobs;
using Sanjel.BusinessEntities.Sections.Common;
using Sanjel.BusinessEntities.Sections.Header;
using Sanjel.Common.BusinessEntities.Reference;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Crew;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ResourceSchedule;
using Sesi.SanjelData.Entities.BusinessEntities.WellSite;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Organization;
using Sesi.SanjelData.Entities.Common.BusinessEntities.WellSite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using eServiceOnline.Gateway;
using eServiceOnline.Models.BulkPlantBoard;
using eServiceOnline.Models.ProductHaul;
using eServiceOnline.Models.ThirdPartyCrewBoard;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Operation;
using Bin = Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment.Bin;
using BinType = Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment.BinType;
using CallSheet = Sanjel.BusinessEntities.CallSheets.CallSheet;
using ClientConsultant = Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch.ClientConsultant;
using DataManager = Syncfusion.JavaScript.DataManager;
using Employee = Sesi.SanjelData.Entities.Common.BusinessEntities.HumanResources.Employee;
using ProductHaul = Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul;
using Rig = Sesi.SanjelData.Entities.Common.BusinessEntities.WellSite.Rig;
using RigJob = Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch.RigJob;
using RigSizeType = Sesi.SanjelData.Entities.Common.BusinessEntities.WellSite.RigSizeType;
using ServicePoint = Sesi.SanjelData.Entities.Common.BusinessEntities.Organization.ServicePoint;
using ShiftType = Sesi.SanjelData.Entities.Common.BusinessEntities.HumanResources.ShiftType;
using Sort = Syncfusion.JavaScript.Sort;
using ThreadType = Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment.ThreadType;
using DrillingCompany = Sesi.SanjelData.Entities.Common.BusinessEntities.WellSite.DrillingCompany;

namespace eServiceOnline.Controllers
{
    public class BulkPlantController : eServicePageController
    {
        private readonly IeServiceWebContext _context;
        private IMemoryCache _memoryCache;
        public BulkPlantController(IMemoryCache memoryCache)
        {
            this._context = eServiceWebContext.Instance;
            _memoryCache = memoryCache;
        }

        #region Index Page
        public ActionResult Index(string selectedDistricts = null)
        {
            SecurityUtility.SetSecurityData(LoggedUser, _memoryCache);
            this.ViewBag.HighLight = "BulkPlant";
            ViewBag.VersionNumber = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            SetDistrictSelection(selectedDistricts);
            return this.View();
        }

        public ActionResult GetPagedRigJobViewModels([FromBody] DataManager dataManager)
        {
            int sequenceNumber = 1;

            if (this.HttpContext.Session.GetString("ServicePoint") == null)
            {
                string retrievalstr = JsonConvert.SerializeObject(new RetrievalCondition());
                this.HttpContext.Session.SetString("ServicePoint", retrievalstr);
            }
            RetrievalCondition retrieval =
                JsonConvert.DeserializeObject<RetrievalCondition>(this.HttpContext.Session.GetString("ServicePoint"));

            this.HttpContext.Session.SetString("ServicePoint", JsonConvert.SerializeObject(retrieval));
            List<Collection<int>> resuhSet = retrieval.ResuhSet(retrieval);
            Collection<int> servicePoints = Utility.GetSearchCollections(resuhSet[0]);
            DateTime startDateTime = DateTime.Now;
            List<RigJob> rigJobList = new List<RigJob>();
            Collection<int> bulkplantIds = this._context.GetBulkPlantRigJobIds();
            bulkplantIds.Add(66945);
            rigJobList = this._context.GetBulkPlantRigJobInformation(bulkplantIds, servicePoints);
            
            //count = rigJobList.Count;
            Debug.WriteLine("取数据时间差1:- {0,21} S", DateTime.Now.Subtract(startDateTime));
            DateTime startDateTime1 = DateTime.Now;
            List<BulkPlantJobViewModel> data = BuildBulkPlantViewModels(rigJobList, sequenceNumber);
            SetRigMergeNumber(data);
            Debug.WriteLine("取数据时间差2:- {0,21} S", DateTime.Now.Subtract(startDateTime1));

            return this.Json(new { result = data });
        }


        private void SetRigMergeNumber(List<BulkPlantJobViewModel> bulkPlantJobViewModels)
        {
            BulkPlantJobViewModel bulkPlantJobViewModel=null;
            int count = bulkPlantJobViewModels.Count;
            int sequence = 1;
            int rowCounts = 0;
            for(int i=0;i<count;i++)
            {
                if (i == 0) { 
                    bulkPlantJobViewModel = bulkPlantJobViewModels[i];
                    rowCounts = 0;
                }
                if(sequence!=bulkPlantJobViewModels[i].Sequence)
                {
                    bulkPlantJobViewModel.Rig.RowMergeNumber = rowCounts;
                    bulkPlantJobViewModel.SequenceNumber.RowMergeNumber = rowCounts;
                    bulkPlantJobViewModel = bulkPlantJobViewModels[i];
                    sequence = bulkPlantJobViewModels[i].Sequence;
                    rowCounts = 1;
                }
                else
                {
                    rowCounts++;
                }
                if(i==(count-1))
                {
                    bulkPlantJobViewModel.Rig.RowMergeNumber = rowCounts;
                    bulkPlantJobViewModel.SequenceNumber.RowMergeNumber = rowCounts;
                }
            }
        }

        private List<BulkPlantJobViewModel> BuildBulkPlantViewModels(List<RigJob> rigJobList, int sequenceNumber)
        {
            List<BulkPlantJobViewModel> data = new List<BulkPlantJobViewModel>();

            if (rigJobList != null && rigJobList.Count > 0)
            {
                List<int> rigIdList = rigJobList.Select(p => p.Rig?.Id ?? 0).Distinct().Except(new []{0}).ToList();
                int[] rigIds =  rigIdList.ToArray();
                Collection<int> rigIdCollection = new Collection<int>(rigIdList);

                List<BinInformation> assignedBinSections = eServiceOnlineGateway.Instance.GetBinInformationByRigIds(rigIds);

                List<int> binInformationIds =assignedBinSections.Select(p=>p.Id).Distinct().Except(new []{0}).ToList();

                List<ProductHaulLoad> productHaulLoads = eServiceOnlineGateway.Instance.GetProductHaulLoadByQuery(p => rigIdCollection.Contains(p.BulkPlant.Id) &&
                    p.ProductHaulLoadLifeStatus != ProductHaulLoadStatus.OnLocation);

                List<ShippingLoadSheet> shippingLoadSheetList =
                    eServiceOnlineGateway.Instance.GetNotOnLocationShippingLoadSheetsBySourceStorageIds(binInformationIds).Where(p => p.ProductHaul.Id != 0).ToList();
                List<int> productHaulIds = shippingLoadSheetList.Where(p=>p.ShippingStatus != ShippingStatus.OnLocation).Select(p=>p.ProductHaul.Id).Distinct().Except(new[] {0}).ToList();
                List<ProductHaul> productHauls = productHaulIds.Count==0?new List<ProductHaul>(): eServiceOnlineGateway.Instance.GetProductHaulByIds(productHaulIds);

                //Jan 2, 2024 zhangyuan 244_PR_AddBulkers: Add Get shippingLoadSheet And ProductHaul
                List<ShippingLoadSheet> shippingLoadSheetFormRigJobList =
                    eServiceOnlineGateway.Instance.GetShippingLoadSheetsBySourceStorageId(0).FindAll(p => p.ProductHaul.ProductHaulLifeStatus == ProductHaulStatus.Scheduled || p.ProductHaul.ProductHaulLifeStatus == ProductHaulStatus.Loading || p.ProductHaul.ProductHaulLifeStatus == ProductHaulStatus.Loaded); ;
                List<int> productHaulFormRigJobIds = shippingLoadSheetFormRigJobList.Where(p => p.ShippingStatus != ShippingStatus.OnLocation).Select(p => p.ProductHaul.Id).Distinct().Except(new[] { 0 }).ToList();
                List<ProductHaul> productHaulFormRigJobs = productHaulFormRigJobIds.Count == 0 ? new List<ProductHaul>() : eServiceOnlineGateway.Instance.GetProductHaulByIds(productHaulFormRigJobIds) ;

                //Mar 7,2024 zhangyaun 308_PR_BulkerShownLoaded : Add has source bin And Loaded status 's  shippingLoadSheet 
                var loadedShippingLoadSheets =
                    shippingLoadSheetList.Where(p => p.ShippingStatus == ShippingStatus.Loaded)?.ToList(); ;
                var loadedShippingLoadSheetProductHaulIds = loadedShippingLoadSheets.Select(p => p.ProductHaul.Id).Distinct().Except(new[] { 0 })?.ToList();
                List<ProductHaul> loadedShippingLoadSheetProductHauls = loadedShippingLoadSheetProductHaulIds.Count == 0 ? new List<ProductHaul>() : eServiceOnlineGateway.Instance.GetProductHaulByIds(loadedShippingLoadSheetProductHaulIds).ToList();
                shippingLoadSheetFormRigJobList = shippingLoadSheetFormRigJobList.Union(loadedShippingLoadSheets, new ShippingLoadSheetComparer()).ToList(); ;
                productHaulFormRigJobs = productHaulFormRigJobs.Union(loadedShippingLoadSheetProductHauls).ToList(); ;

                foreach (RigJob rigJob in rigJobList)
                {
                    //Prepare rigJob related data
   

                    //End:Prepare rigJob related data

                    data.AddRange(BuildRigJobViewModelsFromSingleBulkPlant(sequenceNumber, rigJob, assignedBinSections, productHaulLoads, shippingLoadSheetList, productHauls));
                    //Jan 2, 2024 zhangyuan 244_PR_AddBulkers: Add BulkPlantJobViewModel Data source collection
                    data.AddRange(BuildRigJobViewModelsFromScheduledProductHaul(sequenceNumber, rigJob, productHaulLoads, shippingLoadSheetFormRigJobList, productHaulFormRigJobs));
                    sequenceNumber++;
                }
            }

            return data;
        }

        /*private List<BulkPlantJobViewModel> BuildRigJobViewModelsFromSingleBulkPlant1(int sequenceNumber, RigJob rigJob, List<BinInformation> assignedBinSections)
        {
            List<BulkPlantJobViewModel> rigJobViewModels = new List<BulkPlantJobViewModel>();

            int maxCount = new int[] { assignedBinSections?.Count ?? 0, 1 }.Max();
            //For each rigJob, it will show one row on the RigBoard. Other than Bin and OSR, other columns are all same. So a base view model can be created without bin and OSR
            DateTime dtCompare = DateTime.Now.AddHours(-48);
            for (int i = 0; i < maxCount; i++)
            {
                var binInformation = (assignedBinSections==null||i >= assignedBinSections.Count) ? null : assignedBinSections[i];
                var productHaulLoadList = assignedBinSections?.Count == 0 ? null : this._context.GetProductHaulLoadByBinId(binInformation.Bin.Id, binInformation.PodIndex).FindAll(p => p.ProductHaulLoadLifeStatus != ProductHaulLoadStatus.OnLocation && p.ProductHaulLoadLifeStatus != ProductHaulLoadStatus.Stored);

                productHaulLoadList = assignedBinSections?.Count == 0
                    ? null
                    : eServiceOnlineGateway.Instance.GetProductHaulLoadByQuery(p =>
                        p.Bin.Id == binInformation.Bin.Id && p.PodIndex == binInformation.PodIndex &&
                        p.ProductHaulLoadLifeStatus != ProductHaulLoadStatus.OnLocation &&
                        p.ProductHaulLoadLifeStatus != ProductHaulLoadStatus.Stored);
                List<ProductHaul> productHaulList = null;
                if (productHaulLoadList != null)
                {
                    
                    List<int> productHualLoadIds = productHaulLoadList.Where(p => string.IsNullOrEmpty(p.ProgramId) == false).Select(p => p.Id).ToList();

                    if (productHualLoadIds.Count > 0)
                    {
                        var shippingLoadSheets = eServiceOnlineGateway.Instance.GetShippingLoadSheetByProductHaulLoads(productHualLoadIds).OrderByDescending(p=>p.Id).ToList();
                        //Collection<int> productHaulIds = new Collection<int>(eServiceOnlineGateway.Instance.GetShippingLoadSheetByProductHaulLoads(productHualLoadIds).Select(p => p.ProductHaul.Id).ToList());
                        List<int> productHaulIds = new List<int>();
                        foreach (var shippingLoadSheet in shippingLoadSheets)
                        {
                            productHaulIds.Add(shippingLoadSheet.ProductHaul.Id);
                        }
                        //Collection<int> productHaulIds = new Collection<int>(productHaulLoadList.Select(p => p.ProductHaul.Id).ToList());
                        productHaulList = this._context.GetProductHaulCollectionByProductHaulIds(new Collection<int>(productHaulIds));
                        if (productHaulList != null)
                        {
                            productHaulList=productHaulList.FindAll(p=>p.ProductHaulLifeStatus!=ProductHaulStatus.OnLocation).OrderByDescending(p=>p.Id).ToList();
                            foreach (var productHual in productHaulList)
                            {
                                productHual.ShippingLoadSheets = eServiceOnlineGateway.Instance.GetShippingLoadSheetByProductHaulId(productHual.Id);
                                productHual.ProductHaulLoads = new List<ProductHaulLoad>();
                                foreach (var shippingLoadSheet in shippingLoadSheets)
                                {
                                    if (shippingLoadSheet.ProductHaul.Id == productHual.Id)
                                    {
                                        productHual.ProductHaulLoads.Add(productHaulLoadList.FirstOrDefault(p => p.Id == shippingLoadSheet.ProductHaulLoad.Id)) ;
                                    }
                                }
                            }
                        }
                    }

                }

                if (assignedBinSections.Count > i)
                {
                    rigJobViewModels.AddRange(BuildRigJobViewModelsFromSingleBin(sequenceNumber, rigJob, assignedBinSections[i], binInformation, productHaulList, productHaulLoadList,null));
                }

            }

            return rigJobViewModels;
        }*/
        private List<BulkPlantJobViewModel> BuildRigJobViewModelsFromSingleBulkPlant(int sequenceNumber, RigJob rigJob, List<BinInformation> allAssignedBinSections, List<ProductHaulLoad> allProductHaulLoads, List<ShippingLoadSheet> allShippingLoadSheets, List<ProductHaul> productHauls)
        {
            List<BulkPlantJobViewModel> rigJobViewModels = new List<BulkPlantJobViewModel>();
            List<BinInformation> assignedBinSections = allAssignedBinSections.FindAll(p => p.Rig.Id == rigJob.Rig.Id);
            List<ProductHaulLoad> productHaulLoads = allProductHaulLoads.FindAll(p => p.BulkPlant.Id == rigJob.Rig.Id && p.ProductHaulLoadLifeStatus != ProductHaulLoadStatus.Stored);
            int maxCount = new int[] { assignedBinSections?.Count ?? 0, 1 }.Max();
            //For each rigJob, it will show one row on the RigBoard. Other than Bin and OSR, other columns are all same. So a base view model can be created without bin and OSR

            for (int i = 0; i < maxCount; i++)
            {
                var productHaulLoadListPerBin = productHaulLoads.FindAll(p =>
                    p.Bin.Id == assignedBinSections[i].Bin.Id && p.PodIndex == assignedBinSections[i].PodIndex);
                var shippingLoadSheetListPerBin =
                    allShippingLoadSheets.FindAll(p => p.SourceStorage.Id == assignedBinSections[i].Id);
                var productHaulIdsPerBin =  shippingLoadSheetListPerBin.Select(p => p.ProductHaul.Id).Distinct().Except(new[] {0}).ToList();

                List<ProductHaul> productHaulList = productHauls.FindAll(p => productHaulIdsPerBin.Contains(p.Id));
                foreach (var shippingLoadSheet in shippingLoadSheetListPerBin)
                {
                    var productHaul = productHaulList.Find(p => p.Id == shippingLoadSheet.ProductHaul.Id);
                    if (productHaul != null)
                    {
                        if (productHaul.ShippingLoadSheets == null)
                            productHaul.ShippingLoadSheets = new List<ShippingLoadSheet>();
                        productHaul.ShippingLoadSheets.Add(shippingLoadSheet);
                        /*
                        if (shippingLoadSheet.ProductHaulLoad != null)
                        {
                            var productHaulLoad =
                                eServiceOnlineGateway.Instance.GetProductHaulLoadById(shippingLoadSheet.ProductHaulLoad.Id);
                        }
                    */
                    }
                }

                if (assignedBinSections.Count > i)
                {
	                ProductHaulLoad blendOrigin = null;
	                int lastId = assignedBinSections[i].LastProductHaulLoadId;

                    if (assignedBinSections[i].LastProductHaulLoadId != 0)
	                {
		                var lastProductHaulLoad = allProductHaulLoads
			                .FindAll(p => p.Id == assignedBinSections[i].LastProductHaulLoadId).FirstOrDefault();
		                if (lastProductHaulLoad != null)
		                {
//			                blendOrigin = lastProductHaulLoad.CallSheetNumber != 0 ? ($"CS:{lastProductHaulLoad.CallSheetNumber.ToString()},"): (!string.IsNullOrEmpty(lastProductHaulLoad.ProgramId) ? ($"PRG:{lastProductHaulLoad.ProgramId.ToString()}.{lastProductHaulLoad.ProgramVersion.ToString("D2")},") : "");
			                blendOrigin = lastProductHaulLoad;
		                }
                    }
                    rigJobViewModels.AddRange(BuildRigJobViewModelsFromSingleBin(sequenceNumber, rigJob, assignedBinSections[i], productHaulList, productHaulLoadListPerBin, blendOrigin));
      
                }

            }

            rigJobViewModels = rigJobViewModels.OrderBy(item => item.Bin.PropertyValue, new AlphanumericComparer()).ToList();


            return rigJobViewModels;
        }

        //Jan 2, 2024 zhangyuan 244_PR_AddBulkers: Add filter Data source collection
        private List<BulkPlantJobViewModel> BuildRigJobViewModelsFromScheduledProductHaul(int sequenceNumber, RigJob rigJob, List<ProductHaulLoad> allProductHaulLoads, List<ShippingLoadSheet> allShippingLoadSheets, List<ProductHaul> productHauls)
        {
            List<BulkPlantJobViewModel> rigJobViewModels = new List<BulkPlantJobViewModel>();
            List<ProductHaulLoad> productHaulLoads = allProductHaulLoads.FindAll(p => p.BulkPlant.Id == rigJob.Rig.Id &&(p.ProductHaulLoadLifeStatus == ProductHaulLoadStatus.Scheduled ||
                p.ProductHaulLoadLifeStatus == ProductHaulLoadStatus.BlendCompleted|| 
                p.ProductHaulLoadLifeStatus == ProductHaulLoadStatus.Loaded || 
                p.ProductHaulLoadLifeStatus == ProductHaulLoadStatus.Stored||
                p.ProductHaulLoadLifeStatus == ProductHaulLoadStatus.Blending)).OrderByDescending(p => p.Id).ToList();

            List <ShippingLoadSheet> assignedShip = allShippingLoadSheets.FindAll(p => p.BulkPlant.Id == rigJob.Rig.Id&& (p.ShippingStatus == ShippingStatus.Scheduled|| p.ShippingStatus == ShippingStatus.Loaded)).OrderByDescending(p => p.Id).ToList();

            int maxCount = new int[] { assignedShip?.Count ?? 0 }.Max();
            //For each rigJob, it will show one row on the RigBoard. Other than Bin and OSR, other columns are all same. So a base view model can be created without bin and OSR


            for (int i = 0; i < maxCount; i++)
            {
                var productHaulLoadListPerBin =  productHaulLoads.FindAll(p => assignedShip[i].ProductHaulLoad.Id==p.Id);
                 var productHaul = productHauls.FirstOrDefault(p =>p.Id == assignedShip[i].ProductHaul.Id);

                if (productHaul != null)
                {
                    // productHaul.Crew = eServiceOnlineGateway.Instance.GetCrewById(productHaul.Crew.Id);
                    if (assignedShip.Count > i)
                    {

                        rigJobViewModels.AddRange(BuildRigJobViewModelsFromScheduledProductHaul(sequenceNumber, rigJob, productHaul, assignedShip[i], productHaulLoadListPerBin));

                    }
                }
            }

            rigJobViewModels = rigJobViewModels.OrderBy(item => item.Bin.PropertyValue, new AlphanumericComparer()).ToList();


            return rigJobViewModels;
        }

        public class AlphanumericComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                string[] xParts = System.Text.RegularExpressions.Regex.Split(x, "([0-9]+)");
                string[] yParts = System.Text.RegularExpressions.Regex.Split(y, "([0-9]+)");

                for (int i = 0; i < xParts.Length && i < yParts.Length; i++)
                {
                    if (i % 2 != 0)
                    {
 
                        int xNum = int.Parse(xParts[i]);
                        int yNum = int.Parse(yParts[i]);
                        int numComparison = xNum.CompareTo(yNum);
                        if (numComparison != 0)
                        {
                            return numComparison;
                        }
                    }
                    else
                    {

                        int alphaComparison = xParts[i].CompareTo(yParts[i]);
                        if (alphaComparison != 0)
                        {
                            return alphaComparison;
                        }
                    }
                }

                return x.Length.CompareTo(y.Length);
            }
        }

        private List<BulkPlantJobViewModel> BuildRigJobViewModelsFromSingleBin(int sequenceNumber, RigJob rigJob,  BinInformation binInformation,List<ProductHaul> productHaulList,List<ProductHaulLoad> productHaulLoadList, ProductHaulLoad blendOrigin)
        {
            List<BulkPlantJobViewModel> rigJobViewModels = new List<BulkPlantJobViewModel>();
            int maxCount = new int[] { productHaulLoadList?.Count ?? 0, 1 }.Max();
            for (int i = 0; i < maxCount; i++)
            {
                var baseRigJobView = BuildBaseBulkPlantViewModel(rigJob, i == 0 ? maxCount : 0, sequenceNumber);
                baseRigJobView.IsBulkPlant = true;

                baseRigJobView.BinSectionModel.Count = maxCount;
                if (i == 0) baseRigJobView.BinSectionModel.IsFirst = true;
                baseRigJobView.BinSectionModel.PopulateFrom(binInformation, rigJob);
                //TODO: Different populate function is needed
                baseRigJobView.PopulateFromBin(rigJob, binInformation,
                    productHaulList, productHaulLoadList);
                var productHaulLoadItem = (productHaulLoadList == null || i >= productHaulLoadList.Count) ? null : productHaulLoadList[i];
               baseRigJobView.PopulateFromBinInformationAndProductHaulLoads(rigJob, binInformation,productHaulLoadItem, blendOrigin);
                baseRigJobView.Bin.IsNeedRowMerge = true;
                if(i==0)
                {
                    baseRigJobView.Bin.RowMergeNumber = maxCount;
                }
                rigJobViewModels.Add(baseRigJobView);
            }
            return rigJobViewModels;
        }

        //Jan 2, 2024 zhangyuan 244_PR_AddBulkers: Add Assigning Values to Model Items
        private List<BulkPlantJobViewModel> BuildRigJobViewModelsFromScheduledProductHaul(int sequenceNumber, RigJob rigJob, ProductHaul productHaul,ShippingLoadSheet shippingLoadSheet, List<ProductHaulLoad> productHaulLoadList)
        {
            List<BulkPlantJobViewModel> rigJobViewModels = new List<BulkPlantJobViewModel>();
            int maxCount = new int[] { productHaulLoadList?.Count ?? 0, 1 }.Max();
            for (int i = 0; i < maxCount; i++)
            {
                var baseRigJobView = BuildBaseBulkPlantViewModel(rigJob, i == 0 ? maxCount : 0, sequenceNumber);
                baseRigJobView.IsBulkPlant = true;

                baseRigJobView.BinSectionModel.Count = maxCount;
                if (i == 0) baseRigJobView.BinSectionModel.IsFirst = true;
                baseRigJobView.BinSectionModel.PopulateProductHaulFrom(productHaul);
                //TODO: Different populate function is needed
                baseRigJobView.PopulateFromProductHaulBin(rigJob, null,
                    productHaul, shippingLoadSheet, productHaulLoadList);
                var productHaulLoadItem = (productHaulLoadList == null || i >= productHaulLoadList.Count) ? null : productHaulLoadList[i];
                if (productHaulLoadItem != null)
                {
	                if (productHaulLoadItem.ProductHaulLoadLifeStatus == ProductHaulLoadStatus.Loaded|| productHaulLoadItem.ProductHaulLoadLifeStatus == ProductHaulLoadStatus.Stored)
	                {
		                baseRigJobView.PopulateFromProductHaulLoads(rigJob, null, productHaulLoadItem, productHaul,
			                shippingLoadSheet);
	                }
	                else
	                {
		                baseRigJobView.PopulateFromProductHaulLoads(rigJob, productHaulLoadItem, null, productHaul,
			                shippingLoadSheet);
	                }
                }

                baseRigJobView.Bin.IsNeedRowMerge = true;
                if (i == 0)
                {
                    baseRigJobView.Bin.RowMergeNumber = maxCount;
                }
                rigJobViewModels.Add(baseRigJobView);
            }
            return rigJobViewModels;
        }

        private BulkPlantJobViewModel BuildBaseBulkPlantViewModel(RigJob rigJob, int count, int sequence)
        {
            BulkPlantJobViewModel jobViewModel = new BulkPlantJobViewModel(_memoryCache, LoggedUser); 
            jobViewModel.RowMergeNumber = count;
            jobViewModel.BinSectionModel.Count = count;
            jobViewModel.Sequence = sequence;
            jobViewModel.PopulateFrom(rigJob);
            return jobViewModel;
        }


        #endregion
        #region Empty Bin

        public ActionResult EmptyBin(List<string> parms)
        {
            
            var binInformationId = Convert.ToInt32(parms[0]);
            BinInformation binInformation = this._context.GetBinInformationById(binInformationId);
            return this.PartialView("_EmptyBin",binInformation);
        }

        [HttpPost]
        public ActionResult EmptyBin(int id)
        {
            BinProcess.EmptyBin(id, "Empty Bin from Bulk Plant Board");
            string url = Request.Headers["Referer"].ToString();
            return Redirect(url);
        }
        #endregion


    }
}


