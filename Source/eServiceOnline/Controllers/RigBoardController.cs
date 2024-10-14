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
using eServiceOnline.Models.Job;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using BlendSection = Sesi.SanjelData.Entities.BusinessEntities.BaseEntites.BlendSection;

namespace eServiceOnline.Controllers
{
    public class RigBoardController : eServicePageController
    {
        private readonly IeServiceWebContext _context;
        private IMemoryCache _memoryCache;

        public RigBoardController(IMemoryCache memoryCache)
        {
            this._context = eServiceWebContext.Instance;
            _memoryCache = memoryCache;

            this.Logger?.Trace("RigBoardController called");//can use a filter to log all controllers
        }
        public ActionResult Index(string selectedDistricts = null)
        {
            SecurityUtility.SetSecurityData(LoggedUser, _memoryCache);
            this.ViewBag.HighLight = "RigBoard";
            ViewBag.VersionNumber = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            SetDistrictSelection(selectedDistricts);

            return this.View();
        }


        public ActionResult GetPagedRigJobViewModels([FromBody] DataManager dataManager)
        {
            int pageSize = dataManager.Take;
            int pageNumber = dataManager.Skip/pageSize + 1;
            int count;
            int sequenceNumber = (pageNumber - 1)*pageSize + 1;

            if (this.HttpContext.Session.GetString("ServicePoint") == null)
            {
                string retrievalstr = JsonConvert.SerializeObject(new RetrievalCondition());
                this.HttpContext.Session.SetString("ServicePoint", retrievalstr);
            }
            RetrievalCondition retrieval =
                JsonConvert.DeserializeObject<RetrievalCondition>(this.HttpContext.Session.GetString("ServicePoint"));
            if (retrieval.IsChange)
            {
                retrieval.IsChange = false;
                retrieval.PageNumber = 1;
                sequenceNumber = 1;
            }
            else
            {
                //Apr 30, 2024 Tongtao 186_PR_RigBoardPageSelection: when goto the first page,could not get current page.
                if (pageNumber != 1) 
                 retrieval.PageNumber = pageNumber;
                else
                 retrieval.PageNumber = 1;
            }

            this.HttpContext.Session.SetString("ServicePoint", JsonConvert.SerializeObject(retrieval));
            pageNumber = retrieval.PageNumber;
            List<Collection<int>> resuhSet = retrieval.ResuhSet(retrieval);
            Collection<int> servicePoints = Utility.GetSearchCollections(resuhSet[0]);
            Collection<int> rigTypes = new Collection<int>
            {
                resuhSet[1][0],
                resuhSet[1][1].Equals(resuhSet[1][2]) ? 0 : (resuhSet[1][1].Equals(1)?1:2),
                resuhSet[1][3]
            };
            Collection<int> jobLifeStatuses = Utility.GetSearchCollections(resuhSet[2]);
            bool isShowJobAlert = (jobLifeStatuses == null || jobLifeStatuses.Count < 1 || jobLifeStatuses.Contains(1)) && servicePoints.Count > 0;
            Collection<int> futureJob = Utility.GetSearchCollections(resuhSet[3]);
            bool isShowFutureJobs = futureJob.Count > 0 && futureJob.Contains(1);
            DateTime startDateTime = DateTime.Now;
            List<RigJob> rigJobList = new List<RigJob>();
            rigJobList = this._context.GetAllRigJobInformation(pageNumber, pageSize, servicePoints, rigTypes, jobLifeStatuses, isShowJobAlert, isShowFutureJobs, out count);
            Debug.WriteLine("取数据时间差1:- {0,21} S", DateTime.Now.Subtract(startDateTime));
            DateTime startDateTime1 = DateTime.Now;
            List<RigJobViewModel> data  = BuildRigJobViewModels(rigJobList, sequenceNumber, resuhSet[1][3]);


            Debug.WriteLine("取数据时间差2:- {0,21} S", DateTime.Now.Subtract(startDateTime1));
            DateTime startDateTime2 = DateTime.Now;

            if (dataManager.Sorted != null && dataManager.Sorted.Count.Equals(1))
            {
                Sort sort = dataManager.Sorted.First();
                if (sort.Name.Equals("Company"))
                {
                    data = sort.Direction.Equals("ascending")
                        ? data.OrderBy(a => a.Company.PropertyValue).ToList()
                        : data.OrderByDescending(a => a.Company.PropertyValue).ToList();
                }
                else if (sort.Name.Equals("Rig"))
                {
                    data = sort.Direction.Equals("ascending")
                        ? data.OrderBy(a => a.Rig.PropertyValue).ToList()
                        : data.OrderByDescending(a => a.Rig.PropertyValue).ToList();
                }
                else if (sort.Name.Equals("Date"))
                {
                    data = sort.Direction.Equals("ascending")
                        ? data.OrderBy(a => a.Date.PropertyValue.Length==0?DateTime.MinValue:a.Date.PropertyValue.Length<7?DateTime.ParseExact(a.Date.PropertyValue, "MMM d", CultureInfo.InvariantCulture):DateTime.ParseExact(a.Date.PropertyValue, "MMM d H:mm",
                            CultureInfo.InvariantCulture)).ToList()
                        : data.OrderByDescending(a => a.Date.PropertyValue).ToList();
                }
                else
                {
                    return this.Json(new {result = data, count = count});
                }
            }
            Debug.WriteLine("取数据时间差3:- {0,21} S", DateTime.Now.Subtract(startDateTime2));
             
            return this.Json(new {result = data, count = count});
        }

        private List<RigJobViewModel> BuildRigJobViewModels(List<RigJob> rigJobList, int sequenceNumber,int isProTesting)
        {
            DateTime startDateTime2 = DateTime.Now;
            List<RigJobViewModel> data = new List<RigJobViewModel>();

            if (rigJobList != null && rigJobList.Count > 0)
            {
                int[] rigIds =  rigJobList.Select(p => p.Rig?.Id ?? 0).Distinct().Except(new []{0}).ToArray();

                List<int> rigJobIdList = rigJobList.Select(p => p.Id).Distinct().Except(new[] { 0 }).ToList();

                List<RigJobSanjelCrewSection> crewAssignments =
                    eServiceOnlineGateway.Instance.GetRigJobSanjelCrewSectionsByQuery(p =>
                        rigJobIdList.Contains(p.RigJob.Id));

                List<BinInformation> assignedBinSections = eServiceOnlineGateway.Instance.GetBinInformationByRigIds(rigIds);

                List<int> binInformationIds =assignedBinSections.Select(p=>p.Id).Distinct().Except(new []{0}).ToList();

                List<int> callSheetIdsArray =  rigJobList.FindAll(p=>p.RigStatus==RigStatus.Active && p.JobLifeStatus != JobLifeStatus.Completed).Select(p => p.CallSheetId).Distinct().Except(new []{0}).ToList();

                List<ProductHaulLoad> productHaulLoadsWithCallSheetByQuery = callSheetIdsArray.Count>0? eServiceOnlineGateway.Instance.GetProductHaulLoadByQuery(productHaulLoad=>callSheetIdsArray.Contains(productHaulLoad.CallSheetId)) : new List<ProductHaulLoad>();


                //Collect all related shipping load sheet
                //1. All source storage id equals rig bin section id
                List<ShippingLoadSheet> shippingLoadSheetsFromRigBin =
	                eServiceOnlineGateway.Instance.GetShippingLoadSheetsBySourceStorageIds(binInformationIds);
                //2. All destination is the same as the rig
                List<ShippingLoadSheet> shippingLoadSheetsToRig =
	                eServiceOnlineGateway.Instance.GetShippingLoadSheetByRigIds(rigIds.ToList());

                List<ShippingLoadSheet> shippingLoadSheetsAll = shippingLoadSheetsFromRigBin.Union(shippingLoadSheetsToRig, new ShippingLoadSheetComparer()).ToList();


                /*List<int> shippingLoadSheetIds1 = blendUnloadSheets.Select(p => p.ShippingLoadSheet.Id).Distinct()
                    .Except(new[] {0}).ToList();

                List<ShippingLoadSheet> shippingLoadSheets1 =
                    eServiceOnlineGateway.Instance.GetShippingLoadSheetsByIds(shippingLoadSheetIds1);*/

                List<int> shippingLoadSheetProductLoadIds = shippingLoadSheetsAll.Select(p => p.ProductHaulLoad.Id).Distinct().Except(new []{0}).ToList();

                Collection<int> idCollection = new Collection<int>(shippingLoadSheetProductLoadIds);

                List<ProductHaulLoad> productHaulLoadsWithBins = shippingLoadSheetProductLoadIds.Count>0?
                    eServiceOnlineGateway.Instance.GetProductHaulLoadByQuery(productHaulLoad =>
                        idCollection.Contains(productHaulLoad.Id)): new List<ProductHaulLoad>();
                //Get ShippingLoad Sheets associated to Rig Job Blend

                /*
                List<int> productHaulLoadsWithCallSheetByQueryIds = productHaulLoadsWithCallSheetByQuery
                    .Select(p => p.Id).Distinct().Except(new[] {0}).ToList();
                    */

                /*
                List<ShippingLoadSheet> shippingLoadSheets2 =
                    eServiceOnlineGateway.Instance.GetShippingLoadSheetByProductHaulLoads(productHaulLoadsWithCallSheetByQueryIds);
                    */

//                List<ShippingLoadSheet> shippingLoadSheetsAll = shippingLoadSheets1.Union(shippingLoadSheets2, new ShippingLoadSheetComparer()).ToList();

                // Filter out 1. not completed jobs; 2. find by no call sheet id but  on location

                List<ShippingLoadSheet> shippingLoadSheetsClean = shippingLoadSheetsAll.FindAll(p =>
	                callSheetIdsArray.Contains(p.CallSheetId) ||
	                (p.CallSheetId == 0 && p.ShippingStatus != ShippingStatus.OnLocation));


                List<ProductHaulLoad> productHaulLoadsAll = productHaulLoadsWithCallSheetByQuery.Union(productHaulLoadsWithBins, new ProductHaulLoadComparer()).OrderByDescending(p => p.Id).ToList();

                List<int> productHaulIds = shippingLoadSheetsClean
                    .Select(p => p.ProductHaul.Id).Distinct().Except(new[] {0}).ToList();
                List<int> cleanShippingLoadSheetIds = shippingLoadSheetsClean
	                .Select(p => p.Id).Distinct().Except(new[] { 0 }).ToList();

                List<ProductHaul> productHaulAll = productHaulIds.Count>0? eServiceOnlineGateway.Instance.GetProductHaulByQuery(productHaul => productHaulIds.Contains(productHaul.Id)).OrderByDescending(p=>p.Id).ToList():new List<ProductHaul>();
                foreach (var productHaul in productHaulAll)
                {
                    productHaul.ShippingLoadSheets =
	                    shippingLoadSheetsClean.FindAll(p => p.ProductHaul.Id == productHaul.Id);
                }
                List<BlendUnloadSheet> blendUnloadSheets =
	                eServiceOnlineGateway.Instance.GetBlendUnloadSheetByQuery(blendUnloadSheet =>
		                cleanShippingLoadSheetIds.Contains(blendUnloadSheet.ShippingLoadSheet.Id));

                foreach (RigJob rigJob in rigJobList)
                {
                    Collection<BlendSection> blendSections = new Collection<BlendSection>();;
                    if (rigJob.JobLifeStatus != JobLifeStatus.Alerted)
                    {
                        if (rigJob.JobLifeStatus != JobLifeStatus.Canceled && rigJob.JobLifeStatus != JobLifeStatus.Completed
                                                                           && rigJob.JobLifeStatus != JobLifeStatus.None &&
                                                                           rigJob.JobLifeStatus != JobLifeStatus.Deleted)
                        {
                            
                            blendSections = eServiceWebContext.Instance.GetBlendSectionCollectionByRootIdIsCallSheetId(rigJob.CallSheetId);
                        }
                    }

                    List<ClientConsultant> consultantContacts = GetConsultantContacts(rigJob);

                    //End:Prepare rigJob related data
                    List<RigJobViewModel> list = BuildRigJobViewModelsFromSingleRigJob(sequenceNumber, rigJob, productHaulLoadsAll, assignedBinSections, consultantContacts, blendSections, productHaulAll, shippingLoadSheetsAll, blendUnloadSheets, crewAssignments);


                    if (isProTesting == 1)
                    {
                        bool hasValidTestInfo = FindTestInfo(list).Any();

                        if (hasValidTestInfo)
                        {
                            data.AddRange(list);

                            sequenceNumber++;
                        }
                    }
                    else
                    {
                        data.AddRange(list);

                        sequenceNumber++;
                    }

                    Debug.WriteLine("Job Processing Log:- call sheet id :{0}  processing time: {1} S", rigJob.Id, DateTime.Now.Subtract(startDateTime2));

                }
            }

            return data;
        }


        private List<RigJobViewModel> FindTestInfo(List<RigJobViewModel> list)
        {
            return list.Where(item =>
                (item.Bl1.PropertyValue != null && item.Bl1.PropertyValue.IndexOf("\U0001f9ea") >= 0) ||
                (item.Bl2.PropertyValue != null && item.Bl2.PropertyValue.IndexOf("\U0001f9ea") >= 0) ||
                (item.Bl3.PropertyValue != null && item.Bl3.PropertyValue.IndexOf("\U0001f9ea") >= 0)
            ).ToList();
        }

        private List<BlendSection> GetBlendSectionsGroupByFilter(Collection<BlendSection> data, string[] filters)
        {
            List<BlendSection> result = new List<BlendSection>();
            if (data?.FirstOrDefault(t => filters.Contains(t.BlendCategory.Name)) != null)
                result.AddRange(data.Where(t => filters.Contains(t.BlendCategory.Name)));
            return result.OrderBy(person => filters.ToList().IndexOf(person.BlendCategory.Name)).ToList();

        }
        private List<RigJobViewModel> BuildRigJobViewModelsFromSingleRigJob(int sequenceNumber, RigJob rigJob, List<ProductHaulLoad> productHaulLoadsAll, List<BinInformation> binInformations, List<ClientConsultant> consultantContacts, Collection<BlendSection> allBlendSections, List<ProductHaul> productHaulAll, List<ShippingLoadSheet> shippingLoadSheetAll, List<BlendUnloadSheet> blendUnloadSheetAll, List<RigJobSanjelCrewSection> crewAssignments)
        {
            List<RigJobViewModel> rigJobViewModels = new List<RigJobViewModel>();

 /*
 37	- BL1 rename to PREFL, display products under category "Preflush", "Scavenger" and "Spacer"
 38	- BL2 rename to BLEND, display products under category "Lead 1 -4", "Tail" and "Plug".
 39	- BL3 rename to DISPL, display products under category "Displacement", "Web Shoe"
 */
            List<BlendSection> preflushSections = this.GetBlendSectionsGroupByFilter(allBlendSections, new string[] { "Preflush", "Scavenger", "Spacer" });
            List<BlendSection> blendSections = this.GetBlendSectionsGroupByFilter(allBlendSections, new string[] {"Cap Cement", "Lead 1", "Lead 2", "Lead 3", "Lead 4", "Tail", "Plug" });
            List<BlendSection> displacementSections = this.GetBlendSectionsGroupByFilter(allBlendSections, new string[] { "Displacement", "Wet Shoe" });

            List<BinInformation> assignedBinSections =  binInformations.FindAll(p => p.Rig.Id == rigJob.Rig?.Id);
            //jan 10, 2024 zhangyuan 248_PR_RepeatMenu: Modify cancel filtering conditions
            //List<ProductHaulLoad> productHaulLoads3 = productHaulLoadsAll.FindAll(c=>c.CallSheetId == rigJob.CallSheetId);
            List<ProductHaulLoad> productHaulLoads3 = productHaulLoadsAll;
            List<ShippingLoadSheet> shippingLoadSheet2 = shippingLoadSheetAll.FindAll(s => s.CallSheetId != 0 && s.CallSheetId == rigJob.CallSheetId);
            List<int> productHaulIds = shippingLoadSheet2
                .Select(p => p.ProductHaul.Id).Distinct().Except(new[] {0}).ToList();
            var productHauls2 = productHaulAll.FindAll(p => productHaulIds.Contains(p.Id));

            int maxCount = new int[] { assignedBinSections?.Count ?? 0, consultantContacts?.Count ?? 0 , blendSections?.Count ?? 0, preflushSections?.Count ?? 0, displacementSections?.Count ?? 0, 1 }.Max();

            int maxRevision = this.GetMaxProgramRevisionById(rigJob);

            //For each rigJob, it will show one row on the RigBoard. Other than Bin and OSR, other columns are all same. So a base view model can be created without bin and OSR
//            var productHauls = GetProductHaulsByProductLoads(productHaulLoads3);



            for (int i = 0; i < maxCount; i++)
            {
                var baseRigJobView = BuildBaseRigJobViewModel(rigJob, i==0?maxCount:0, sequenceNumber, productHauls2, crewAssignments);

                baseRigJobView.BinSectionModel.Count = maxCount;
                if (i == 0) baseRigJobView.BinSectionModel.IsFirst = true;
                baseRigJobView.BinSectionModel.PopulateFrom(i >= assignedBinSections.Count ? null : assignedBinSections[i], rigJob);

                baseRigJobView.ConsultantViewModel.Count = maxCount;
                if (i == 0) baseRigJobView.ConsultantViewModel.IsFirst = true;

                if (preflushSections != null && preflushSections.Count > i)
                {
                    baseRigJobView.BlendProductHaulModel1.PopulateFrom(preflushSections[i], productHaulAll, productHaulLoads3, shippingLoadSheetAll);
                }

                if (blendSections != null && blendSections.Count > i)
                {
                    baseRigJobView.BlendProductHaulModel2.PopulateFrom(blendSections[i], productHaulAll, productHaulLoads3, shippingLoadSheetAll);
                }

                if (displacementSections != null && displacementSections.Count > i)
                {
                    baseRigJobView.BlendProductHaulModel3.PopulateFrom(displacementSections[i], productHaulAll, productHaulLoads3, shippingLoadSheetAll);
                }

                baseRigJobView.ConsultantViewModel.PopulateFromRigJob(i >= consultantContacts.Count ? null : consultantContacts[i], rigJob);
                //Nov 30, 2023 zhangyuan 203_PR_UpdateBinNotes: Add GetBinNote To Model
                int[] binIds = assignedBinSections.Select(p => p.Bin.Id)?.Distinct().ToArray();
                if (binIds != null && binIds.Length > 0)
                {
                    baseRigJobView.BinSectionModel.BinNotes = eServiceOnlineGateway.Instance.GetBinNoteByBins(binIds);
                }

                baseRigJobView.PopulateFromBlendAndConsultantContacts(rigJob, assignedBinSections, productHauls2, shippingLoadSheetAll);

                //                foreach (var assignedBinSection in assignedBinSections)
                if (assignedBinSections.Count == 0 && i==0)
                {
                    
                    baseRigJobView.PopulateFromBin(rigJob, null, null,null);
                }
                else
                {
                    if (i < assignedBinSections.Count)
                    {
                        /*
                        List<ProductHaulLoad> productHaulLoads = new List<ProductHaulLoad>();
                        List<ProductHaul> productHaulsToBin = new List<ProductHaul>();
                        productHaulLoads =
                            eServiceOnlineGateway.Instance.GetNotOnLocationProductHaulLoadByBinId(
                                assignedBinSections[i].Bin.Id,
                                assignedBinSections[i].PodIndex);

                        foreach (var productHaulLoad in productHaulLoads)
                        {
                            if (productHaulLoad.ProductHaul != null && productHaulLoad.ProductHaul.Id != 0)
                            {
                                var productHaul =
                                    eServiceOnlineGateway.Instance.GetProductHaulById(productHaulLoad.ProductHaul.Id);
                                productHaul.ProductHaulLoads =
                                    eServiceOnlineGateway.Instance.GetProductHaulLoadsByProductHual(productHaul);
                                productHaulsToBin.Add(productHaul);
                            }
                        }
                        */
                        List<BlendUnloadSheet> blendUnloadSheetsByBin =
                            blendUnloadSheetAll.FindAll(p => p.DestinationStorage.Id == assignedBinSections[i].Id);
                        List<int> shippingLoadSheetIdsByBin =  blendUnloadSheetsByBin.Select(p => p.ShippingLoadSheet.Id).Distinct().Except(new []{0}).ToList();
                        List<int> shippingLoadSheetIdsFromBin = shippingLoadSheetAll.Where(p=>p.SourceStorage.Id == assignedBinSections[i].Id).Select(p=>p.Id).Distinct().Except(new[] { 0 }).ToList();
                        
                        List<ShippingLoadSheet> loadSheets = shippingLoadSheetAll.FindAll(p => shippingLoadSheetIdsByBin.Contains(p.Id)|| shippingLoadSheetIdsFromBin.Contains(p.Id));
                        // List<ShippingLoadSheet> loadSheets = eServiceOnlineGateway.Instance.GetShippingLoadSheetsByDestinationStorage(assignedBinSections[i]);
                        List<ProductHaul> productHauls1 = new List<ProductHaul>();
                        if (loadSheets != null && loadSheets.Count > 0)
                        {
                            foreach (var shippingLoadSheet in loadSheets)
                            {
                                ProductHaul productHaul =
                                    productHaulAll.Find(p => p.Id == shippingLoadSheet.ProductHaul.Id);

                                    if(productHaul!=null)
                                    {
                                        productHauls1.Add(productHaul);
                                    }
                            }
                        }
                        //Jan 5, 2024 zhangyuan 248_PR_RepeatMenu: Remove duplicate productHauls1
                        var distinctProductHauls1 =  productHauls1.Distinct().ToList();
                        baseRigJobView.PopulateFromBin(rigJob, assignedBinSections[i], distinctProductHauls1,
                            productHaulLoads3);
                    }
                }

                if(rigJob.JobLifeStatus!=JobLifeStatus.Completed && rigJob.JobLifeStatus!=JobLifeStatus.Canceled && rigJob.JobLifeStatus!=JobLifeStatus.None && maxRevision > rigJob.MaxProgramRevision)
                    baseRigJobView.Company.Style = RigJobViewModel.StyleJobAlertAmber;
                    rigJobViewModels.Add(baseRigJobView);
                }


            return rigJobViewModels;
        }

        private List<ProductHaul> GetProductHaulsByProductLoads(List<ProductHaulLoad> productHaulLoads)
        {
            List<ProductHaul> productHaulList = new List<ProductHaul>();

            if (productHaulLoads != null)
            {
                List<int> productHaulLoadIds = productHaulLoads.Select(p => p.Id).Distinct().ToList();
                if (productHaulLoadIds.Count > 0)
                {
                    List<ShippingLoadSheet> shippingLoadSheets =
                        eServiceOnlineGateway.Instance.GetShippingLoadSheetByProductHaulLoads(productHaulLoadIds);
                    List<int> productHaulIds = shippingLoadSheets.Select(p => p.ProductHaul.Id).Distinct().ToList();
                    if (productHaulIds.Count > 0)
                    {
                        foreach (var productHaulId in productHaulIds)
                        {
                            var productHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);
                            if(productHaul != null)
                                productHaulList.Add(productHaul);
                        }
                    }
                    foreach(var productHaulLoad in productHaulLoads)
                    {
                        if(shippingLoadSheets.FirstOrDefault(p=>p.ProductHaulLoad.Id== productHaulLoad.Id)==null)
                        {
                            ProductHaul productHaul = new ProductHaul();
                            ShippingLoadSheet shippingLoadSheet= new ShippingLoadSheet();
                            shippingLoadSheet.ProductHaul = productHaul;
                            shippingLoadSheet.ProductHaulLoad = productHaulLoad;
                            List<ShippingLoadSheet> shippingLoadSheets1 = new List<ShippingLoadSheet>();
                            shippingLoadSheets1.Add(shippingLoadSheet);
                            productHaul.ShippingLoadSheets = shippingLoadSheets1;
                            productHaulList.Add(productHaul);
                        }
                    }
                }
            }

            return productHaulList;
        }

        private int GetMaxProgramRevisionById(RigJob rigJob)
        {
            return RigBoardProcess.GetMaxProgramRevisionById(rigJob.ProgramId);
        }

        private void GetRigJobInfo(RigJob rigJob)
        {
            Job job = this._context.GetJobByUniqueId(string.IsNullOrEmpty(rigJob.JobUniqueId) ? "" : rigJob.JobUniqueId);
            if (job != null)
            {
                rigJob.JobNumber = job.JobNumber;
            }
        }

        public JsonResult GetSwitchNotes(int id)
        {
            int count;
            List<RigJob> data1 = this._context.GetAllRigJobInformation(0, 999, new Collection<int>() { id }, new Collection<int>() { 0, 2 }, new Collection<int>() { 2, 3, 4, 5 }, false, false, out count);
            List<RigJob> data2 = this._context.GetAllRigJobInformation(0, 999, new Collection<int>() { id }, new Collection<int>() { 1, 2 }, new Collection<int>() { 2, 3, 4, 5 }, false, false, out count);

            var dataAll = new List<RigJob>();
            dataAll.AddRange(data1.ToList());
            dataAll.AddRange(data2.ToList());

            List<SwitchNoteView> switchNoteViews = new List<SwitchNoteView>();

            foreach (var rigJob in dataAll)
            {
                SwitchNoteView switchNoteView = new SwitchNoteView();
                switchNoteView.JobInformation = rigJob.ClientCompany.ShortName + "\r\n" + rigJob.Rig.Name + "\r\n" + rigJob.JobType.Name;
                switchNoteView.JobDateTime = rigJob.JobDateTime==null?string.Empty: Utility.GetDateTimeValue(rigJob.JobDateTime, "MMM dd HH:mm");
                switchNoteView.Notes = rigJob.Notes;
                switchNoteViews.Add(switchNoteView);
            }

            return this.Json(new { result = switchNoteViews, count = count });

        }

    

        private RigJobViewModel BuildBaseRigJobViewModel(RigJob rigJob, int count, int sequence, List<ProductHaul> productHauls, List<RigJobSanjelCrewSection> crewAssignments)
        {
            RigJobViewModel jobViewModel = this.PopulateRigJobViewModel(rigJob, productHauls, crewAssignments);
//            RigJobViewModel jobViewModel = new RigJobViewModel(_memoryCache, LoggedUser);
            jobViewModel.RowMergeNumber = count;
            jobViewModel.BinSectionModel.Count = count;
            jobViewModel.ConsultantViewModel.Count = count;
            jobViewModel.Sequence = sequence;
            jobViewModel.PopulateFrom(rigJob);

            return jobViewModel;
        }

        private RigJobViewModel PopulateRigJobViewModel(RigJob rigJob, List<ProductHaul> productHauls, List<RigJobSanjelCrewSection> crewAssignments)
        {
            RigJobViewModel jobViewModel = new RigJobViewModel(_memoryCache, LoggedUser);
            /*
                        Collection<BlendSection> blendSections = null;
                        if (rigJob.JobLifeStatus!=JobLifeStatus.Alerted)
                        {
                            if (rigJob.JobLifeStatus!=JobLifeStatus.Canceled && rigJob.JobLifeStatus!=JobLifeStatus.Completed &&
                                rigJob.JobLifeStatus!=JobLifeStatus.None && rigJob.JobLifeStatus!=JobLifeStatus.Deleted)
                            {
                                blendSections = this._context.GetBlendSectionCollectionByRootIdIsCallSheetId(rigJob.CallSheetId);
                            }
                        }
                        if (blendSections != null && blendSections.Count > 0)
                        {
                            List<ProductHaul> productHauls1 = this.GetProductHaulsByBlendSectionId(blendSections[0].Id, productHauls);

                            jobViewModel.BlendProductHaulModel1.PopulateFrom(blendSections[0], GetProductHaulAndCrewNameViewModels(productHauls1));

                            if (blendSections.Count > 1)
                            {
            //                    List<ProductHaul> productHauls2 = this.GetProductHaulsByBlendSectionId(blendSections[1].Id,                        productHauls);
            //                    jobViewModel.BlendProductHaulModel2.PopulateFrom(blendSections[1], GetProductHaulAndCrewNameViewModels(productHauls2));
                                if (blendSections.Count > 2)
                                {
                                    jobViewModel.BlendProductHaulModel3 = this.GetBlendProductHaulModel3(blendSections,
                                        productHauls);
                                }
                            }
                        }
            */
            this.GetCrewDataByRigJob(rigJob, productHauls, crewAssignments, jobViewModel);

            return jobViewModel;
        }

        /*
        private List<ProductHaulAndCrewNameViewModel> GetProductHaulAndCrewNameViewModels(List<ProductHaul> productHauls)
        {
            List<ProductHaulAndCrewNameViewModel> productHaulAndCrewNameViewModels=new List<ProductHaulAndCrewNameViewModel>();
            if (productHauls.Count!=0)
            {
                foreach (var productHaul in productHauls)
                {
                    ProductHaulAndCrewNameViewModel model = new ProductHaulAndCrewNameViewModel
                    {
                        ProductHaul = productHaul, CrewName = productHaul.Name
                    };
                    productHaulAndCrewNameViewModels.Add(model);
                }
            }

            return productHaulAndCrewNameViewModels;
        }
        */

        public void GetCrewDataByRigJob(RigJob rigJob, List<ProductHaul> productHauls, List<RigJobSanjelCrewSection> crewAssignments, RigJobViewModel rigJobViewModel)
        {
            List<Crew> crews = new List<Crew>();
            List<SanjelCrew> sanjelCrews = new List<SanjelCrew>();
            List<CrewModel> crewModels = new List<CrewModel>();
            List<ThirdPartyCrewModel> thirdPartyCrewModels = new List<ThirdPartyCrewModel>();
            List<ThirdPartyBulkerCrew> thirdPartyBulkerCrews = new List<ThirdPartyBulkerCrew>();
//            List<RigJobSanjelCrewSection> rigJobCrewSections = this._context.GetRigJobCrewSectionByRigJob(rigJob.Id);

            List<RigJobSanjelCrewSection> rigJobCrewSections = crewAssignments.FindAll(p=>p.RigJob.Id == rigJob.Id && p.ProductHaul.Id == 0);

            /*
            foreach (RigJobSanjelCrewSection rigJobCrewSection in rigJobCrewSections)
            {
                rigJobCrewSection.SanjelCrew = this._context.GetCrewById(rigJobCrewSection.SanjelCrew.Id);
                if (rigJobCrewSection.SanjelCrew != null)
                {
                    rigJobCrewSection.SanjelCrew.HomeServicePoint = this._context.GetServicePointById(rigJobCrewSection.SanjelCrew.HomeServicePoint.Id);
                }
//                rigJobCrewSection.RigJobCrewSectionStatus = this._context.GetJobCrewSectionStatusById(rigJobCrewSection.RigJobCrewSectionStatus.Id);
            }

            if (rigJobCrewSections.Count > 0)
            {
                List<RigJobSanjelCrewSection> assignedCrewSections = rigJobCrewSections.FindAll(p => p.SanjelCrew?.Type?.Id == 1 || p.SanjelCrew?.Type?.Id == 4);
                List<RigJobCrewSectionModel> rigJobCrewSectionModels = new List<RigJobCrewSectionModel>();

                foreach (RigJobSanjelCrewSection assignedCrewSection in assignedCrewSections)
                {
                    RigJobCrewSectionModel model = new RigJobCrewSectionModel();
                    model.PopulateFrom(assignedCrewSection);
                    rigJobCrewSectionModels.Add(model);
                }
                rigJobViewModel.RigJobCrewSectionModels = rigJobCrewSectionModels;
            }
            */

            if (rigJobCrewSections.Count > 0)
            {
                List<RigJobCrewSectionModel> rigJobCrewSectionModels = new List<RigJobCrewSectionModel>();

                foreach (RigJobSanjelCrewSection assignedCrewSection in rigJobCrewSections)
                {
                    RigJobCrewSectionModel model = new RigJobCrewSectionModel();
                    model.PopulateFrom(assignedCrewSection);
                    rigJobCrewSectionModels.Add(model);
                }
                rigJobViewModel.RigJobCrewSectionModels = rigJobCrewSectionModels;
            }


            if (productHauls != null && productHauls.Count > 0)
            {
                List<ProductHaul> onGoingProductHauls = productHauls.FindAll(p => p.ProductHaulLifeStatus!=ProductHaulStatus.OnLocation);
                foreach (ProductHaul onGoingProductHaul in onGoingProductHauls)
                {
                    if (onGoingProductHaul.Crew == null) continue;

                    if (!onGoingProductHaul.IsThirdParty)
                    {
                        crewModels.Add(new CrewModel() { Id = onGoingProductHaul.Crew.Id, Name = onGoingProductHaul.Crew.Name??onGoingProductHaul.Crew.Description, Description = $"{{{onGoingProductHaul.Crew.Description}}} {onGoingProductHaul.Description}"});
//                        crews.Add(onGoingProductHaul.Crew);
                    }
                    else
                    {
                        thirdPartyCrewModels.Add(new ThirdPartyCrewModel() { Id = onGoingProductHaul.Crew.Id, Name = onGoingProductHaul.Crew.Name, Description = onGoingProductHaul.Description });
//                        thirdPartyBulkerCrews.Add(new ThirdPartyBulkerCrew(){Id = onGoingProductHaul.Id, Name = onGoingProductHaul.Crew.Name, Description = onGoingProductHaul.Description, ThirdPartyUnitNumber = onGoingProductHaul.ThirdPartyUnitNumber, ContractorCompany = onGoingProductHaul.ContractorCompany, SupplierContactName = onGoingProductHaul.SupplierContactName, SupplierContactNumber = onGoingProductHaul.SupplierContactNumber });
                    }

                }
            }

            /*
            foreach (Crew sanjelCrew in crews)
            {
                CrewModel crewModel = new CrewModel();
                crewModel.PopulateFrom(sanjelCrew);
                crewModels.Add(crewModel);
            }

            foreach (ThirdPartyBulkerCrew thirdPartyBulkerCrew in thirdPartyBulkerCrews)
            {
                ThirdPartyCrewModel thirdPartyCrewModel = new ThirdPartyCrewModel();
                thirdPartyCrewModel.PopulateFrom(thirdPartyBulkerCrew);
                thirdPartyCrewModels.Add(thirdPartyCrewModel);
            }
            */
            
            rigJobViewModel.CrewModels = crewModels;
            rigJobViewModel.ThirdPartyCrewModels = thirdPartyCrewModels;
        }

        /*
        private List<ProductHaul> GetProductHaulsByBlendSectionId(int blendSectionId, List<ProductHaul> productHauls)
        {
            List<ProductHaul> productHaulList = new List<ProductHaul>();
            foreach (ProductHaul productHaul in productHauls)
            {
                if (productHaul.ProductHaulLoads.Count > 0)
                {
//                    if (productHaul.IsThirdParty)
//                    {
//                        productHaul.Crew = this._context.GetThirdPartyBulkerCrewById(productHaul.Crew?.Id ?? 0);
//                    }
//                    else
//                    {
//                        productHaul.Crew = this._context.GetCrewById(productHaul.Crew?.Id ?? 0);
//                    }
                    List<ProductHaulLoad> productHaulLoads = productHaul.ProductHaulLoads.Where(p => p.BlendSectionId == blendSectionId).ToList();

                    if (productHaulLoads.Count > 0)
                    {
                        productHaulList.Add(productHaul);
                    }
                }
            }

            return productHaulList;
        }
        */

        public ActionResult GetLsdInfoDirectionByNumber(List<string> parms)
        {
            WellLocationInformationViewModel model = new WellLocationInformationViewModel
            {
                CallSheetNumber = Convert.ToInt32(parms[0]),
                RigJobId = Convert.ToInt32(parms[1])
            };
            WellLocationInformation well = this._context.GetWellLocationInfoByCallSheetNumber(Convert.ToInt32(parms[0]));
            if (well != null)
            {
                model.PopulateFrom(well);
                return this.PartialView("_UpdateLsdInfoDirection", model);
            }

            return this.PartialView("_UpdateLsdInfoDirection", model);
        }

        public ActionResult GetLsdInfoWellLocationByNumber(List<string> parms)
        {
            WellLocationInformationViewModel model = new WellLocationInformationViewModel
            {
                CallSheetNumber = Convert.ToInt32(parms[0]),
                RigJobId = Convert.ToInt32(parms[1])
            };
            WellLocationInformation well = this._context.GetWellLocationInfoByCallSheetNumber(Convert.ToInt32(parms[0]));
            if (well != null)
            {
                model.PopulateFrom(well);
                return this.PartialView("_UpdateLsdInfoWellLocation", model);
            }

            return this.PartialView("_UpdateLsdInfoWellLocation", model);
        }

        [HttpPost]
        public ActionResult UpdateLsdInfoDirection(WellLocationInformationViewModel model)
        {
            WellLocationInformation wellLocationInformation = this._context.GetWellLocationInfoByCallSheetNumber(model.CallSheetNumber);
            if (
                !(string.IsNullOrWhiteSpace(wellLocationInformation.DirectionToLocation) &&
                  model.DirectionToLocation == null) ||
                !wellLocationInformation.DirectionToLocation.Equals(model.DirectionToLocation))
            {
                wellLocationInformation.DirectionToLocation = model.DirectionToLocation;
                this._context.UpdateWellLocationInfoByCallSheet(model.RigJobId, model.CallSheetNumber, wellLocationInformation);
            }

            return this.RedirectToAction("Index", "RigBoard");
        }

        public ActionResult VerifyLsdIsChange(WellLocationInformationViewModel model)
        {
            WellLocationInformation well = this._context.GetWellLocationInfoByCallSheetNumber(model.CallSheetNumber);
            RigJob rigJob = this._context.GetRigJobByCallsheetNumber(model.CallSheetNumber);
            if (well != null && rigJob.JobLifeStatus!=JobLifeStatus.None)
            {
                if ((string.IsNullOrWhiteSpace(well.DirectionToLocation) && model.DirectionToLocation == null) ||
                    well.DirectionToLocation.Equals(model.DirectionToLocation))
                {
                    return this.Json(false);
                }
            }

            return this.Json(true);
        }

        public ActionResult UpdateLsdInfoWellLocation(WellLocationInformationViewModel model)
        {
            WellLocationInformation wellLocationInformation =
                this._context.GetWellLocationInfoByCallSheetNumber(model.CallSheetNumber);
            if (wellLocationInformation != null)
            {
                model.PopulateTo(wellLocationInformation);
                wellLocationInformation.HasDownHoleWellLocation = !string.IsNullOrWhiteSpace(model.DownHoleWellLocation);
                this._context.UpdateWellLocationInfoByCallSheet(model.RigJobId, model.CallSheetNumber,
                    wellLocationInformation);
            }

            return this.RedirectToAction("Index", "RigBoard");
        }

        public ActionResult CreateRig()
        {
            this.GetDrillingCompaniesInfo();
            this.GetRigSizeInfo();
            this.GetSizeInfo();
            this.GetThreadInfo();

            return this.PartialView("_CreateRig");
        }

        [HttpPost]
        public bool VerifyRigNumber(int drillingCompanyId, string rigNumber)
        {
            bool isExist = false;
            List<Rig> rigs = eServiceWebContext.Instance.GetRigByDrillingCompanyId(drillingCompanyId);
            Rig rig = rigs.Find(a => a.RigNumber == rigNumber);
            if (rig != null)
            {
                isExist = true;
            }
            return isExist;
        }

        [HttpPost]
        public bool VerifyRigName(string rigName, int rigId)
        {
            bool isExist = false;
            Rig rig = eServiceWebContext.Instance.GetRigByName(rigName);
            if (rig != null && rig.Id != rigId)
            {
                isExist = true;
            }
            return isExist;
        }

        public void GetDrillingCompaniesInfo()
        {
            List<DrillingCompany> companies = this._context.GetDrillingCompanyInfo().OrderBy(a => a.Name).ToList();
            List<SelectListItem> company = companies.Select(p => new SelectListItem {Text = p.Name, Value = p.Id.ToString()}).ToList();
            this.ViewData["drillingCompany"] = company;
        }

        public void GetClientCompaniesInfo()
        {
            List<ClientCompany> companies = this._context.GetClientCompanyInfo().OrderBy(a => a.Name).ToList();
            List<SelectListItem> company = companies.Select(p => new SelectListItem { Text = p.Name, Value = p.Id.ToString() }).ToList();
            this.ViewData["clientCompany"] = company;
        }

        public void GetSizeInfo()
        {
            List<RigSize> valueUnitTypes = new List<RigSize>(CacheData.StandardSizeTypes);
            List<SelectListItem> valueUnit = valueUnitTypes.Select(p => new SelectListItem {Text = p.Value.ToString(CultureInfo.InvariantCulture), Value = p.Id.ToString()}).ToList();
            this.ViewData["valueUnit"] = valueUnit;
        }

        public void GetRigSizeInfo()
        {
            List<RigSizeType> rigSize = new List<RigSizeType>(CacheData.RigSizeTypes);
            List<SelectListItem> rigSizeList = rigSize.Select(p => new SelectListItem {Text = p.Description, Value = p.Id.ToString()}).ToList();
            this.ViewData["rigSizeList"] = rigSizeList;
        }

        public void GetThreadInfo()
        {
            List<ThreadType> threadType = new List<ThreadType>(CacheData.ThreadTypes);
            List<SelectListItem> threadList = threadType.Select(p => new SelectListItem {Text = p.Description, Value = p.Id.ToString()}).ToList();
            this.ViewData["threadList"] = threadList;
        }

        [HttpPost]
        public ActionResult CreateRig(RigViewModel rigModel)
        {
            try
            {
                DrillingCompany company = this._context.GetDrillingCompanyById(rigModel.DrillingCompanyId);
                Rig rig = new Rig();
                rigModel.PopulateTo(rig);
                rig.DrillingCompany = company;
                rig.Status =RigStatus.Active;
                rig.RigSize = this._context.GetRigSizeTypeById(rig.RigSize?.Id ?? 0);
                rig.Size = this._context.GetRigSizeById(rig.Size?.Id ?? 0);
                rig.ThreadType = this._context.GetThreadTypeById(rig.ThreadType?.Id ?? 0);
                rig.OperationSiteType = OperationSiteType.Rig;
                this._context.CreateRig(rig);

                return this.RedirectToAction("Index", "RigBoard");
            }
            catch (Exception)
            {
                return this.RedirectToAction("Index", "RigBoard");
            }
        }

        public ActionResult GetRigById(List<string> parms)
        {
            this.GetRigSizeInfo();
            this.GetSizeInfo();
            this.GetThreadInfo();
            Rig rig = this._context.GetRigInfoByRigId(Convert.ToInt32(parms[0]));
            RigViewModel model = new RigViewModel();
            model.PopulateFrom(rig);
            model.RigJobId = Convert.ToInt32(parms[1]);

            return this.PartialView("_UpdateRig", model);
        }

        [HttpPost]
        public ActionResult UpdateRigInfo(RigViewModel model)
        {
            Rig rig = new Rig();
            model.PopulateTo(rig);
            rig.RigSize = this._context.GetRigSizeTypeById(rig.RigSize?.Id ?? 0);
            rig.Size = this._context.GetRigSizeById(rig.Size?.Id ?? 0);
            rig.ThreadType = this._context.GetThreadTypeById(rig.ThreadType?.Id ?? 0);
            this._context.UpdateRigInfo(rig);

            return this.RedirectToAction("Index", "RigBoard");
        }

        public ActionResult UpdateRigStatus(List<string> parms)
        {
            List<BinInformation> binsAssignments = eServiceWebContext.Instance.GetBinInformationCollectionByRig(new Rig{Id = Convert.ToInt32(parms[0]) });
            this.ViewBag.HaveBinAssigned = binsAssignments.Count > 0 ? "true" : "false";
            this.ViewBag.RigId = parms[0];

            return this.PartialView("_DeactivateARig");
        }

        [HttpPost]
        public ActionResult UpdateRigStatus()
        {
            string rigId = this.Request.Form["RigId"];
            eServiceWebContext.Instance.DeactivateRig(Convert.ToInt32(rigId));

            return this.RedirectToAction("Index", "RigBoard");
        }

        public ActionResult EnableRig(List<string> parms)
        {
            this.ViewBag.RigId = parms[0];
            this.ViewBag.RigStatus = parms[1];

            return this.PartialView("_EnableRig");
        }

        [HttpPost]
        public ActionResult EnableRig()
        {
            string rigId = this.Request.Form["RigId"];
            this._context.UpdateRigStatus(Convert.ToInt32(rigId), RigStatus.Active);

            return this.RedirectToAction("Index", "RigBoard");
        }

        public ActionResult UpdateRigStatusToDown(List<string> parms)
        {
            this.ViewBag.RigId = parms[0];
            this.ViewBag.DownStatus = parms[1];

            return this.PartialView("_UpdateRigStatusToDown");
        }

        [HttpPost]
        public ActionResult UpdateRigStatusToDown()
        {
            string rigId = this.Request.Form["RigId"];
            string downStatus = this.Request.Form["DownStatus"];
            var rigStatus = (RigStatus)Enum.Parse(typeof(RigStatus), downStatus);

            //var username = this.LoggedUser;
            //This is the way to know who is operating
            this._context.UpdateRigStatus(Convert.ToInt32(rigId), rigStatus);

            return this.RedirectToAction("Index", "RigBoard");
        }

        public ActionResult ModifyCompanyCodClearedFlag(List<string> parms)
        {
            int callSheetNumber = Int32.Parse(parms[0]);
            bool isIsCoDCleared = bool.Parse(parms[1]);
            CallSheet callSheet = this._context.GetCallSheetByNumberForRigJob(callSheetNumber);
            if (callSheet?.Header != null)
            {
                CallSheet newCallSheet = callSheet;
                newCallSheet.Header.IsCODCleared = isIsCoDCleared;
                this._context.UpdateCallSheet(newCallSheet, callSheet);
                //Removed, update COD flag won't change call sheet status

                return this.Json(true);
            }

            return this.Json(false);
        }
        //This integration was disabled somehow, leave it for now
        public ActionResult UpdateIsNeedBin(NeedBinsViewModel model)
        {
            CallSheet callSheet = this._context.GetCallSheetByIdForBin(model.CallSheetId);
            if (callSheet?.CommonSection?.UnitPersonnel != null)
            {
                CallSheet newCallSheet = callSheet;
                newCallSheet.CommonSection.UnitPersonnel.IsNeedBin = model.IsNeedBin;
                newCallSheet.CommonSection.UnitPersonnel.NumberOfBin = model.NumberOfBin;
//                this.UpdateCallSheetAndRigJob(model.RigJobId, newCallSheet, callSheet);
            }
            return this.RedirectToAction("Index", "RigBoard");
        }

        public ActionResult VerifyNumberOfBin(NeedBinsViewModel binSectionModel)
        {
            int binSectionCount = this._context.GetBinSectionCollctionByRootId(binSectionModel.CallSheetId) != null
                ? this._context.GetBinSectionCollctionByRootId(binSectionModel.CallSheetId).Count
                : 0;
            if (binSectionCount < binSectionModel.NumberOfBin)
            {
                return this.Json(true);
            }

            return this.Json(false);
        }

        #region bl column operation

        public ActionResult NeedBin(List<string> parms)
        {
            RigJob originalVersion = this._context.GetRigJobById(Convert.ToInt32(parms[0]));
            if (originalVersion != null)
            {
                RigJob currentVersion = originalVersion;
                currentVersion.IsNeedBins = true;
                this._context.UpdateRigJob(currentVersion, originalVersion);
                return this.Json(true);
            }

            return this.Json(false);
        }

        public ActionResult NotNeedBin(List<string> parms)
        {
            RigJob originalVersion = this._context.GetRigJobById(Convert.ToInt32(parms[0]));
            if (originalVersion?.Rig != null)
            {
                if (this._context.GetBinInformationCollectionByRig(originalVersion.Rig)?.Count < 1)
                {
                    RigJob currentVersion = originalVersion;
                    currentVersion.IsNeedBins = false;
                    this._context.UpdateRigJob(currentVersion, originalVersion);
                    return this.Json(true);
                }
            }

            return this.Json(false);
        }
        public JsonResult VerifyBinInformation(int binTypeId, int rigJobId, int binId)
        {
            var rigJob = eServiceWebContext.Instance.GetRigJobById(rigJobId);
            var bin = eServiceWebContext.Instance.GetBinById(binId);

            string result = "Assign " + bin.Name + " to Rig " + rigJob.Rig.Name + "?"; 
            return new JsonResult(result);
        }
        public ActionResult GetBinInfoByBinTypeId(int binTypeId, int rigJobId)
        {
            if (binTypeId.Equals(0) || rigJobId.Equals(0)) return this.Json(new List<SelectListItem>());

            List<Bin> availablBinList = GetUnassignedBin(binTypeId);
            /*
            if (availablBinList.Count.Equals(0)) return this.Json(new List<SelectListItem>());

            List<SelectListItem> binList = availablBinList.Count.Equals(0)
                ? new List<SelectListItem>()
                : availablBinList.Select(p => new SelectListItem { Text = p.Name, Value = p.Id.ToString() }).ToList();
            binList = binList.OrderBy(p => p.Text).ToList();
            */

            return this.Json(availablBinList);
        }

        private List<Bin> GetUnassignedBin(int binTypeId)
        {
            List<BinInformation> binInformations = eServiceOnlineGateway.Instance.GetBinInformationByQuery(binInformation => binInformation.BinStatus == BinStatus.Assigned);
            List<int> assignedBinIds =
                binInformations.Select(c=>c.Bin.Id).Distinct().ToList();
            List<Bin> binCollection =EServiceReferenceData.Data.BinCollection.Where(p => p.BinType.Id == binTypeId && !assignedBinIds.Contains(p.Id)).ToList();

            return binCollection;
        }

        public ActionResult AssignABin(List<string> parms)
        {
            this.GetBinTypeInfo();
            this.ViewData["binList"] = new List<SelectListItem>();
            RigBinModel model = new RigBinModel
            {
                RigJobId = Convert.ToInt32(parms[0])
            };

            return this.PartialView("_AssignABin", model);
        }

        private void GetBinTypeInfo()
        {
            List<BinType> binTypes = CacheData.BinTypes.OrderBy(a => a.Id).ToList();
            List<SelectListItem> binTypeList = binTypes.Select(p => new SelectListItem { Text = p.Description, Value = p.Id.ToString() }).ToList();
            this.ViewData["binTypeList"] = binTypeList;
        }

        [HttpPost]
        public ActionResult AssignABin(RigBinModel rigBinModel)
        {
            this._context.AssignBinToRig(rigBinModel.BinId, rigBinModel.RigJobId,rigBinModel.BinInformationList);
            string url = Request.Headers["Referer"].ToString();
            return Redirect(url);
        }

        /*
        public ActionResult ScheduleBlend(List<string> parms)
        {
            this.ViewData["binList"] = new List<SelectListItem>();
            ProductHaulModel model = new ProductHaulModel(){BinId = Convert.ToInt32(parms[0])};
            return this.PartialView("_ScheduleBlend", model);
        }

        public ActionResult TransferBlend(List<string> parms)
        {
            this.ViewData["binList"] = new List<SelectListItem>();
            ProductHaulModel model = new ProductHaulModel(){BinId = Convert.ToInt32(parms[0])};
            return this.PartialView("_TransferBlend", model);
        }

        public ActionResult HaulBlend(List<string> parms)
        {
            this.ViewData["binList"] = new List<SelectListItem>();
            ProductHaulModel model = new ProductHaulModel(){BinId = Convert.ToInt32(parms[0])};
            return this.PartialView("_HaulBlend", model);
        }
        */

        public ActionResult RemoveABin(List<string> parms)
        {
            this.GetBinTypeInfo();
            int binId = Convert.ToInt32(parms[0]);
            int callSheetId = Convert.ToInt32(parms[1]);
            int rigJobId = Convert.ToInt32(parms[2]);
            this.ViewData["binId"] = parms[0];
            this.ViewData["callSheetId"] = parms[1];
            this.ViewData["rigJobId"] = parms[2];
            RigJob rigJob = eServiceOnlineGateway.Instance.GetRigJobById(rigJobId);
            List<BinInformation> rigBinSections = eServiceOnlineGateway.Instance.GetBinInformationsByRigId(rigJob.Rig.Id);
            List<BinInformation> binInformationList = rigBinSections.Where(p => p.Bin.Id == binId).ToList();
            this.ViewData["binInformationList"] = binInformationList;
            return this.PartialView("_RemoveABin");
        }
        [HttpPost]
        public ActionResult RemoveABin(int binId,int callSheetId,int rigJobId)
        {
            try
            {
                bool result = this._context.UnassignBinToRig(binId, callSheetId, rigJobId);
            }
            catch
            { }
            string url = Request.Headers["Referer"].ToString();
            return Redirect(url);
        }


        #endregion

        public void GetWorkShiftInfo()
        {
            List<ShiftType> shiftType = this._context.GetWorkShiftInfo();
            List<SelectListItem> shiftList = shiftType.Select(p => new SelectListItem {Text = p.Description, Value = p.Id.ToString()}).ToList();
            this.ViewData["shiftList"] = shiftList;
        }

        public ActionResult CreateConsultant()
        {
            this.GetClientCompaniesInfo();
            this.GetWorkShiftInfo();
            return this.PartialView("_CreateConsultant");
        }

        [HttpPost]
        public ActionResult CreateConsultant(ConsultantViewModel model)
        {
            ClientConsultant clientConsultant = new ClientConsultant();
            model.PopulateTo(clientConsultant);
            clientConsultant.Client = this._context.GetClientCompanyById(model.ClientId);
            clientConsultant.WorkShift = this._context.GetWorkShiftById(model.WorkShiftTypeId);
            this._context.CreateConsultant(clientConsultant);

            return this.RedirectToAction("Index", "RigBoard");
        }

        public ActionResult AddAConsultant(List<string> parms)
        {
            ConsultantViewModel model = new ConsultantViewModel {CallSheetId = Convert.ToInt32(parms[0])};
            List<ClientConsultant> clientConsultants = this._context.GetClientConsultantCollection().OrderBy(a => a.Name).ToList();
            List<SelectListItem> clientConsultantList = clientConsultants.Select(p => new SelectListItem {Text = p.Name, Value = p.Id.ToString()}).ToList();
            this.ViewBag.ClientConsultantList = clientConsultantList;
            this.ViewBag.RigJobId = Convert.ToInt32(parms[1]);

            return this.PartialView("_AddAConsultant", model);
        }

        public JsonResult GetConsultantInfo(int consultantId)
        {
            ClientConsultant clientConsultant = this._context.GetConsultantById(consultantId);
            ConsultantViewModel model = new ConsultantViewModel();
            model.PopulateFrom(clientConsultant);

            return Json(model);
        }

        [HttpPost]
        public ActionResult AddAConsultant(ConsultantViewModel model)
        {
            string rigJobId = this.Request.Form["rigJobId"];
            ClientConsultant clientConsultant = this._context.GetConsultantById(model.ConsultantId);

            CallSheet callSheet = this._context.GetCallSheetByIdForBin(model.CallSheetId);
            CallSheet currentVersion = callSheet;

            RigJob rigJob = this._context.GetRigJobById(Int32.Parse(rigJobId));
            RigJob currentRigJob = rigJob;
            bool isUpdateRigJob = false;
            bool isUpdateCallSheet = false;

            //Update RigJob
            if (currentRigJob.ClientConsultant1 == null)
            {
                currentRigJob.ClientConsultant1 = clientConsultant;
                isUpdateRigJob = true;
            }
            else if (currentRigJob.ClientConsultant2?.Id == 0 && !currentRigJob.ClientConsultant1.Name.Equals(clientConsultant.Name))
            {
                currentRigJob.ClientConsultant2 = clientConsultant;
                isUpdateRigJob = true;
            }

            if (callSheet?.Header?.HeaderDetails?.FirstCall != null)
            {
                FirstCall consultant = callSheet.Header.HeaderDetails.FirstCall;

                if (consultant.SelectedClientConsultant1 == null)
                {
                    consultant.AlertByConsultant1 = clientConsultant.Name;
                    consultant.AlertByConsultantCellNumber1 = clientConsultant.Cell;
                    consultant.AlertByConsultantEmail1 = model.Email;
                    consultant.SelectedClientConsultant1 = new Sanjel.Common.BusinessEntities.Lookup.ClientConsultant()
                    {
                        Id = clientConsultant.Id,
                        Name = clientConsultant.Name,
                        Cell = clientConsultant.Cell,
                        Phone2 = clientConsultant.Phone2,
                        Client = new Company
                        {
                            Id = clientConsultant.Client.Id,
                        },
                        WorkShift = new Sanjel.Common.BusinessEntities.Reference.ShiftType()
                        {
                            Id = clientConsultant.WorkShift.Id,
                            Name = clientConsultant.WorkShift.Name,
                        }
                    };
                    isUpdateCallSheet = true;

                }
                else if (consultant.SelectedClientConsultant2?.Id == 0 &&
                         !consultant.AlertByConsultant1.Equals(model.Name))
                {
                    consultant.AlertByConsultant2 = clientConsultant.Name;
                    consultant.AlertByConsultantCellNumber2 = clientConsultant.Cell;
                    consultant.AlertByConsultantEmail2 = clientConsultant.Email;
                    consultant.SelectedClientConsultant2 = new Sanjel.Common.BusinessEntities.Lookup.ClientConsultant()
                    {
                        Id = clientConsultant.Id,
                        Name = clientConsultant.Name,
                        Cell = clientConsultant.Cell,
                        Phone2 = clientConsultant.Phone2,
                        Client = new Company
                        {
                            Id = clientConsultant.Client.Id,
                        },
                        WorkShift = new Sanjel.Common.BusinessEntities.Reference.ShiftType()
                        {
                            Id = clientConsultant.WorkShift.Id,
                            Name = clientConsultant.WorkShift.Name,
                        }
                    };
                    isUpdateCallSheet = true;
                }
                if(isUpdateCallSheet)
                    currentVersion.Header.HeaderDetails.FirstCall = consultant;
            }

            if (isUpdateRigJob && isUpdateCallSheet)
            {
                this._context.UpdateCallSheet(currentVersion, callSheet);
                this._context.UpdateRigJob(currentRigJob, rigJob);
            }
            

            return this.RedirectToAction("Index", "RigBoard");
        }

        public ActionResult GetConsultantContactsByCallSheetNumber(List<string> parms)
        {
            FirstCallViewModel model = new FirstCallViewModel {CallSheetNumber = Convert.ToInt32(parms[0])};
            FirstCall consultant = this._context.GetConsultantContactsByCallSheetNumber(Convert.ToInt32(parms[0]));
            model.PopulateFrom(consultant);

            return this.PartialView("_UpdateConsultantInfo", model);
        }

        public ActionResult GetClientConsultantById(List<string> parms)
        {
            this.GetClientCompaniesInfo();
            this.GetWorkShiftInfo();
            ConsultantViewModel model = new ConsultantViewModel();
            ClientConsultant clientConsultant = this._context.GetConsultantById(Convert.ToInt32(parms[0]));
            model.PopulateFrom(clientConsultant);

            return this.PartialView("_UpdateClientConsultantInfo", model);
        }

        public ActionResult UpdateClientConsultant(ConsultantViewModel model)
        {
            ClientConsultant clientConsultant = new ClientConsultant();
            model.PopulateTo(clientConsultant);
            clientConsultant.Client = this._context.GetClientCompanyById(model.ClientId);
            clientConsultant.WorkShift = this._context.GetWorkShiftById(model.WorkShiftTypeId);
            this._context.UpdateClientConsultant(true, clientConsultant);

            return this.RedirectToAction("Index", "RigBoard");
        }

        public ActionResult UpdateConsultantInfo(FirstCallViewModel model)
        {
            FirstCall consultant = new FirstCall();
            model.PopulateTo(consultant);
            this._context.UpdateConsultantInfo(model.CallSheetNumber, consultant);

            return this.RedirectToAction("Index", "RigBoard");
        }

        public ActionResult RemoveConsultant(List<string> parms)
        {
            try
            {
                CallSheet callSheet = this._context.GetCallSheetByNumberForRigJob(Convert.ToInt32(parms[0]));
                RigJob rigJob = this._context.GetRigJobById(Convert.ToInt32(parms[2]));

                bool isFirst = Convert.ToBoolean(parms[1]);
                RigJob currentRigJob = rigJob;
                CallSheet currentCallSheet = callSheet;
                FirstCall firstCall = callSheet.Header.HeaderDetails.FirstCall;

                if (isFirst)
                {
                    //Delete First and promote second to first.

                    currentRigJob.ClientConsultant1 = currentRigJob.ClientConsultant2;


                    firstCall.SelectedClientConsultant1 = firstCall.SelectedClientConsultant2;
                    firstCall.AlertByConsultant1 = firstCall.SelectedClientConsultant1.Name;
                    firstCall.AlertByConsultantCellNumber1 = firstCall.SelectedClientConsultant1.Cell;
                    firstCall.AlertByConsultantEmail1 = firstCall.SelectedClientConsultant1.Email;

                }

                currentRigJob.ClientConsultant2 = null;
                firstCall.SelectedClientConsultant2 = null;
                firstCall.AlertByConsultant2 = string.Empty;
                firstCall.AlertByConsultantCellNumber2 = string.Empty;
                firstCall.AlertByConsultantEmail2 = string.Empty;

                this._context.UpdateRigJob(currentRigJob, rigJob);
                this._context.UpdateCallSheet(currentCallSheet, callSheet);

                return this.Json(true);
            }
            catch
            {
                return this.Json(false);
            }
        }

        public ActionResult DeleteConsultant(List<string> parms)
        {
            try
            {
                ClientConsultant clientConsultant = this._context.GetConsultantById(Convert.ToInt32(parms[0]));
                this._context.DeleteClientConsultant(clientConsultant);

                return this.Json(true);
            }
            catch
            {
                return this.Json(false);
            }
        }

        public ActionResult UpdateWorkShift(List<string> parms)
        {
            try
            {
                ClientConsultant clientConsultant = this._context.GetConsultantById(Convert.ToInt32(parms[0]));
                clientConsultant.WorkShift = this._context.GetWorkShiftById(Convert.ToInt32(parms[1]));
                this._context.UpdateClientConsultant(false, clientConsultant);

                return this.Json(true);
            }
            catch
            {
                return this.Json(false);
            }
        }

        public ActionResult GetRigJobById(List<string> parms)
        {
            int rigJobId = Int32.Parse(parms[0]);
            RigJob rigJob = null;
            rigJob = rigJobId.Equals(0)
                ? this._context.CreateRigJobByCallSheet(Int32.Parse(parms[1]))
                : this._context.GetRigJobById(Int32.Parse(parms[0]));
            if (rigJob != null)
            {
                this.ViewBag.rigJobId = rigJob.Id;
                this.ViewBag.notes = rigJob.Notes;
            }

            return this.PartialView("_UpdateNotes");
        }

        public ActionResult CancelRigJob(List<string> parms)
        {
            int rigJobId = Int32.Parse(parms[0]);
            RigJob rigJob = null;
            rigJob = rigJobId.Equals(0)
                ? this._context.CreateRigJobByCallSheet(Int32.Parse(parms[1]))
                : this._context.GetRigJobById(Int32.Parse(parms[0]));
            if (rigJob != null)
            {
                if(rigJob.JobLifeStatus == JobLifeStatus.Canceled || rigJob.JobLifeStatus == JobLifeStatus.InProgress ||rigJob.JobLifeStatus == JobLifeStatus.Completed)
                {
                    this.ViewBag.ErrorMessage =
                        $"This job cannot be canceled, it is {rigJob.JobLifeStatus.ToString()}. \n\nClose this message to allow Rig Board refreshed.";

                }
                else
                {
                    this.ViewBag.rigJobId = rigJob.Id;
                    this.ViewBag.notes = rigJob.Notes;
                    return this.PartialView("_UpdateRigJobStatusToCancel");
                }
            }
            else
            {
                this.ViewBag.ErrorMessage =
                    $"This job cannot be found anymore. \n\nClose this message to allow Rig Board refreshed.";
            }
            return this.PartialView("_JobStatusError");
        }
        public ActionResult DeleteRigJobById(List<string> parms)
        {
            this.ViewBag.RigJobId = parms[0];
            this.ViewBag.CallSheetNumber = parms[1];

            return this.PartialView("_IsDeleteRigJob");
        }

        [HttpPost]
        public ActionResult DeleteRigJobById()
        {
            string rigJobId = this.Request.Form["RigJobId"];
            string callSheetNumber = this.Request.Form["CallSheetNumber"];
            int result = this._context.DeleteRigJob(this._context.GetRigJobById(Int32.Parse(rigJobId != null ? rigJobId : "0")));
            if (!result.Equals(1)) return this.Json(false);
            CallSheet oldcallSheet = this._context.GetCallSheetByNumber(Convert.ToInt32(callSheetNumber != null ? callSheetNumber : "0"));
            if (oldcallSheet != null)
            {
                CallSheet newCallSheet = oldcallSheet;
                newCallSheet.Header.Status = EServiceEntityStatus.Deleted;
                newCallSheet.Status = EServiceEntityStatus.Deleted;
                this._context.UpdateCallSheet(newCallSheet, oldcallSheet);
            }

            //Release Crew
            RigBoardProcess.ReleaseCrew(Int32.Parse(rigJobId ?? "0"), false, DateTime.Now);
            return this.RedirectToAction("Index", "RigBoard");
        }

        [HttpPost]
        public ActionResult UpdateRigJobNotes()
        {
            string id = this.Request.Form["id"];
            string notes = this.Request.Form["notes"];

            RigJob originalRigJob = this._context.GetRigJobById(Int32.Parse(id));
            RigJob currentRigJob = originalRigJob;
            if (originalRigJob != null)
            {
                currentRigJob.Notes = notes;
            }
            this._context.UpdateRigJob(currentRigJob, originalRigJob);

            return this.RedirectToAction("Index", "RigBoard");
        }

        [HttpPost]
        public ActionResult UpdateRigJobStatusToCancel()
        {
            string id = this.Request.Form["id"];
            string notes = this.Request.Form["notes"];
            if (this._context.ChangeRigJobStatusToCancel(Int32.Parse(id), notes))
            {
                eServiceWebContext.Instance.ReleaseCrew(Int32.Parse(id), true, DateTime.Now);

                return this.RedirectToAction("Index", "RigBoard");
            }
            else
            {
                this.ViewBag.ErrorMessage =
                    $"This job cannot be canceled, it might be In Progress or Completed. \n\nClose this message to allow Rig Board refreshed.";
                return this.PartialView("_JobStatusError");

            }
        }

        public List<Employee> GetEmployees()
        {
            List<Employee> employees = this._context.GetEmployeeList().OrderBy(a => a.FirstName).ToList();
            List<SelectListItem> employeeItems = employees.Select(p => new SelectListItem {Text = Utility.PreferedName(p), Value = p.Id.ToString()}).ToList();
            this.ViewData["employeeItems"] = employeeItems;

            return employees;
        }
  
        public ActionResult UpdateJobDate(List<string> parms)
        {
            CallInformationViewModel model = new CallInformationViewModel()
            {
                CallSheetNumber = Convert.ToInt32(parms[0])
            };
            CallSheet callSheet = this._context.GetCallSheetByNumberForRigJob(Convert.ToInt32(parms[0]));
            List<ClientConsultant> calloutConsultantses = new List<ClientConsultant>();
            if (callSheet != null)
            {
                model.CallCrewDateTime = callSheet.Header.HeaderDetails.CallInformation.CallCrewDateTime;
                if (callSheet.Header.HeaderDetails.FirstCall.IsExpectedTimeOnLocation)
                    model.RequestedDateTime = callSheet.Header.HeaderDetails.FirstCall.ExpectedTimeOnLocation;
                else
                    model.RequestedDateTime = callSheet.Header.HeaderDetails.CallInformation.RequestedDateTime;

                if (callSheet.Header?.HeaderDetails?.FirstCall != null)
                {
                    ClientConsultant alertByConsultant1 = new ClientConsultant
                    {
                        Id = 1,
                        Name = callSheet.Header.HeaderDetails.FirstCall.AlertByConsultant1,
                    };
                    calloutConsultantses.Add(alertByConsultant1);
                    if (!string.IsNullOrEmpty(callSheet.Header.HeaderDetails.FirstCall.AlertByConsultant2))
                    {
                        ClientConsultant alertByConsultant2 = new ClientConsultant
                        {
                            Id = 2,
                            Name = callSheet.Header.HeaderDetails.FirstCall.AlertByConsultant2
                        };
                        calloutConsultantses.Add(alertByConsultant2);
                    }
                }
            }

            this.GetEmployees();
            List<SelectListItem> calloutConsultantsList = calloutConsultantses.Select(p => new SelectListItem {Text = p.Name, Value = p.Id.ToString()}).ToList();
            this.ViewData["calloutConsultantsList"] = calloutConsultantsList;
            RigJob rigJob = this._context.GetRigJobById(Convert.ToInt32(parms[2]));
            if (rigJob == null) throw new Exception("Rig Job doesn't exist, this is an unexpected error.");
            this.ViewBag.JobDateTime = rigJob.JobDateTime;
            this.ViewBag.RigJobId = parms[2];
            if (parms[1].Equals("Confirm"))
            {
                if (rigJob.JobLifeStatus != JobLifeStatus.Pending)
                {
                    this.ViewBag.ErrorMessage =
                        $"This job has been confirmed, its current status is {rigJob.JobLifeStatus.ToString()}.\n\nClose this message to allow Rig Board refreshed.";
                    return this.PartialView("_JobStatusError");
                }

                model.Operation = "Confirm";
                model.CallDateTime = DateTime.Now;
                this.ViewBag.CallDateTime = model.CallDateTime;
                this.ViewBag.RequestedDateTime = model.RequestedDateTime;
                model.CallCrewDateTime = DateTime.Now;
                this.ViewBag.CallCrewDateTime = model.CallCrewDateTime;

                return this.PartialView("_UpdateJobDateConfirm", model);
            }
            if (parms[1].Equals("Reschedule"))
            {
                model.Operation = "Reschedule";
                model.CallDateTime = DateTime.Now;
                this.ViewBag.CallDateTime = model.CallDateTime;
                this.ViewBag.RequestedDateTime = model.RequestedDateTime;
                this.ViewBag.CallCrewDateTime = model.CallCrewDateTime;
                return this.PartialView("_UpdateJobDateReschedule", model);
            }
            if (parms[1].Equals("CallOut"))
            {
                model.Operation = "CallOut";
                return this.PartialView("_UpdateJobDateDispatched", model);
            }
            if (parms[1].Equals("OnHold"))
            {
                model.Operation = "OnHold";
                model.CallDateTime = DateTime.Now;
                this.ViewBag.RequestedDateTime = model.RequestedDateTime;
                return this.PartialView("_UpdateJobDatePending", model);
            }

            return this.RedirectToAction("Index", "RigBoard");
        }

        [HttpPost]
        public ActionResult UpdateJobDate(CallInformationViewModel model)
        {

            CallSheet callSheet = this._context.GetCallSheetByNumberForRigJob(model.CallSheetNumber);
            CallSheet currentcallSheet = callSheet;
            string rigJobId = this.Request.Form["rigJobId"];
            if (callSheet?.Header?.HeaderDetails?.FirstCall != null &&
                callSheet?.Header?.HeaderDetails?.CallInformation != null)
            {
                if (model.Operation.Equals("Reschedule"))
                {
                    //Precondition: Job is confirmed or scheduled
                    // 1. Update CallInformation.RequestedDateTime; 2. Update CallInformation.CallCrewDateTime 
                    currentcallSheet.Header.HeaderDetails.CallInformation.CallDateTime = model.CallDateTime;
                    currentcallSheet.Header.HeaderDetails.CallInformation.CallCrewDateTime = model.CallCrewDateTime;
                    currentcallSheet.Header.HeaderDetails.CallInformation.RequestedDateTime = model.RequestedDateTime;
                }
                else if (model.Operation.Equals("Confirm"))
                {
                    CallSheet newCallSheet = eServiceWebContext.Instance.GetCallSheetById(currentcallSheet.Id);
                    CallSheetValidation.ValidateEntityForScheduled(newCallSheet);
                    currentcallSheet.Header.HeaderDetails.FirstCall.IsExpectedTimeOnLocation = false;
                    currentcallSheet.Header.HeaderDetails.CallInformation.CallDateTime = model.CallDateTime;
                    currentcallSheet.Header.HeaderDetails.CallInformation.CallCrewDateTime = model.CallCrewDateTime;
                    currentcallSheet.Header.HeaderDetails.CallInformation.RequestedDateTime = model.RequestedDateTime;
                    currentcallSheet.Status = newCallSheet.Status;

                }
                else if (model.Operation.Equals("CallOut"))
                {
                    currentcallSheet.Header.HeaderDetails.CallInformation.IsThisCallMade = true;
                    currentcallSheet.Header.HeaderDetails.CallInformation.RequestedDateTime = model.RequestedDateTime;
                    currentcallSheet.Header.HeaderDetails.CallInformation.CallCrewDateTime = DateTime.Now;
                }
                else if (model.Operation.Equals("OnHold"))
                {
                    currentcallSheet.Header.HeaderDetails.FirstCall.IsExpectedTimeOnLocation = true;
                    currentcallSheet.Header.HeaderDetails.FirstCall.AlertDateTime = DateTime.Now;
                    currentcallSheet.Header.HeaderDetails.FirstCall.ExpectedTimeOnLocation = model.RequestedDateTime;
                    currentcallSheet.Header.HeaderDetails.CallInformation.CallDateTime = DateTime.MinValue;
                    currentcallSheet.Header.HeaderDetails.CallInformation.CalloutConsultant = string.Empty;
                    currentcallSheet.Header.HeaderDetails.CallInformation.CalloutConsultantCellNumber = String.Empty;
                    currentcallSheet.Header.HeaderDetails.CallInformation.CalloutConsultantEmail = string.Empty;
                    currentcallSheet.Header.HeaderDetails.CallInformation.RequestedDateTime = DateTime.MinValue;
                    currentcallSheet.Header.HeaderDetails.CallInformation.CallCrewDateTime = DateTime.MinValue;
                    currentcallSheet.Header.HeaderDetails.CallInformation.IsLoadAndGo = false;
                    currentcallSheet.Header.HeaderDetails.CallInformation.IsThisCallMade = false;
                    currentcallSheet.Status = EServiceEntityStatus.InProgress;
                }

                if (model.SelectedCalloutConsultantId == 1)
                {
                    currentcallSheet.Header.HeaderDetails.CallInformation.SelectedCalloutConsultant.Id =
                        currentcallSheet.Header.HeaderDetails.FirstCall.SelectedClientConsultant1.Id;
                    currentcallSheet.Header.HeaderDetails.CallInformation.CalloutConsultant =
                        model.CalloutConsultantName;
                    currentcallSheet.Header.HeaderDetails.CallInformation.CalloutConsultantCellNumber =
                        currentcallSheet.Header.HeaderDetails.FirstCall.AlertByConsultantCellNumber1;
                    currentcallSheet.Header.HeaderDetails.CallInformation.CalloutConsultantEmail =
                        currentcallSheet.Header.HeaderDetails.FirstCall.AlertByConsultantEmail1;
                }
                else if (model.SelectedCalloutConsultantId == 2)
                {
                    currentcallSheet.Header.HeaderDetails.CallInformation.SelectedCalloutConsultant.Id =
                        currentcallSheet.Header.HeaderDetails.FirstCall.SelectedClientConsultant2.Id;
                    currentcallSheet.Header.HeaderDetails.CallInformation.CalloutConsultant =
                        model.CalloutConsultantName;
                    currentcallSheet.Header.HeaderDetails.CallInformation.CalloutConsultantCellNumber =
                        currentcallSheet.Header.HeaderDetails.FirstCall.AlertByConsultantCellNumber2;
                    currentcallSheet.Header.HeaderDetails.CallInformation.CalloutConsultantEmail =
                        currentcallSheet.Header.HeaderDetails.FirstCall.AlertByConsultantEmail2;
                }
                else
                {
                    List<Employee> employeeList = this.GetEmployees();
                    Employee dispatcher = employeeList.Find(employee => employee.Id == model.CalloutDispatcherId);
                    currentcallSheet.Header.HeaderDetails.CallInformation.CalloutDispatcher.Id =
                        model.CalloutDispatcherId;
                    currentcallSheet.Header.HeaderDetails.CallInformation.CalloutDispatcher.FirstName =
                        dispatcher.FirstName;
                    currentcallSheet.Header.HeaderDetails.CallInformation.CalloutDispatcher.MiddleName =
                        dispatcher.MiddleName;
                    currentcallSheet.Header.HeaderDetails.CallInformation.CalloutDispatcher.LastName =
                        dispatcher.LastName;
                }
                this._context.UpdateCallSheet(currentcallSheet, callSheet);
            }
            RigJob rigJob = this._context.GetRigJobById(Int32.Parse(rigJobId));
            if (rigJob != null)
            {
                RigJob currentRigJob = rigJob;
                currentRigJob.JobDateTime = model.RequestedDateTime;
                if (model.Operation.Equals("Confirm"))
                {
                    if (currentcallSheet.Status == EServiceEntityStatus.Ready)
                    {
                        currentRigJob.JobLifeStatus = JobLifeStatus.Scheduled;
                    }
                    else
                    {
                        currentRigJob.JobLifeStatus = JobLifeStatus.Confirmed;
                    }
                    currentRigJob.CallCrewTime = model.CallCrewDateTime;
                    this._context.UpdateRigJob(currentRigJob, rigJob);
                    eServiceWebContext.Instance.UpdateRigJobCrewSchedule(currentRigJob);
                }
                else if (model.Operation.Equals("Reschedule"))
                {
                    currentRigJob.CallCrewTime = model.CallCrewDateTime;
                    this._context.UpdateRigJob(currentRigJob, rigJob);
                    if (model.IsReleaseCrew)
                    {
                        eServiceWebContext.Instance.ReleaseCrew(currentRigJob.Id, false, DateTime.Now);
                    }
                    else
                    {
                        eServiceWebContext.Instance.UpdateRigJobCrewSchedule(currentRigJob);
                    }
                }
                else if (model.Operation.Equals("CallOut"))
                {
                    currentRigJob.JobLifeStatus = JobLifeStatus.Dispatched;
                    this._context.UpdateRigJob(currentRigJob, rigJob);
                    eServiceWebContext.Instance.UpdateRigJobCrewSchedule(currentRigJob);
                }
                else if (model.Operation.Equals("OnHold"))
                {
                    currentRigJob.JobLifeStatus = JobLifeStatus.Pending;
                    currentRigJob.CallCrewTime = DateTime.MinValue;
                    this._context.UpdateRigJob(currentRigJob, rigJob);
                    if (model.IsReleaseCrew)
                    {
                        eServiceWebContext.Instance.ReleaseCrew(currentRigJob.Id, false, DateTime.Now);
                    }
                    else
                    {
                        eServiceWebContext.Instance.UpdateRigJobCrewSchedule(currentRigJob);
                    }
                }
            }
            else
            {
                this._context.CreateRigJobByCallSheet(model.CallSheetNumber);
            }

            //Removed. Call sheet status and rig job status are handled by above logic.

            return this.RedirectToAction("Index", "RigBoard");
        }

        public ActionResult ChangeRigJobToComplete(List<string> parms)
        {
            try
            {
                int id = Convert.ToInt32(parms[0]);
                bool succeed = eServiceWebContext.Instance.ChangeRigJobStatusToComplete(id, DateTime.Now);
//                eServiceWebContext.Instance.ReleaseCrew(id, false);

                return this.Json(succeed);
            }
            catch (Exception)
            {
                return this.Json(false);
            }
        }

        public void UpdateAccessRecord(string userName)
        {
//            string userId = this.HttpContext.Session.GetString("UserId");
//            LoggingUtility.LogoutAccess(Convert.ToInt32(userId), userName);
        }

        public ActionResult ActivateARig()
        {
            this.GetDrillingCompaniesInfo();

            return this.PartialView("_ActivateARig", new RigViewModel());
        }

        public JsonResult GetDeactivateRigByDrillingCompanyId(int drillingCompanyId)
        {
            List<Rig> rigs = eServiceWebContext.Instance.GetDeactivateRigByDrillingCompanyId(drillingCompanyId).ToList();

            return new JsonResult(rigs);
        }

        [HttpPost]
        public ActionResult ActivateARig(RigViewModel model)
        {
            this._context.ActivateARig(model.RigId);

            return this.RedirectToAction("Index", "RigBoard");
        }

        #region Get ReferenceData when create Job Alert 

        public void GetCompanyClientConsultant()
        {
            List<ClientConsultant> consultants = CacheData.ClientConsultants.OrderBy(a => a.Name).ToList();
            List<SelectListItem> consultantItems = consultants.Select(p => new SelectListItem {Text = p.Name, Value = p.Id.ToString()}).ToList();

            this.ViewData["jobAlert_Consultant"] = consultantItems; 
        }

        public void GetCompanyRig()
        {
            List<Rig> rigs = CacheData.Rigs.OrderBy(a => a.Name).ToList();
            List<SelectListItem> rigItems = rigs.Select(p => new SelectListItem {Text = p.Name, Value = p.Id.ToString()}).ToList();

            this.ViewData["jobAlert_Rig"] = rigItems;
        }

        public void GetServicePointCollection()
        {
            List<ServicePoint> servicePoints = CacheData.ServicePointCollections.OrderBy(a => a.Name).ToList();
            List<SelectListItem> servicePointItems = servicePoints.Select(p => new SelectListItem {Text = p.Name, Value = p.Id.ToString()}).ToList();
            this.ViewData["jobAlert_ServicePoint"] = servicePointItems;
        }

        #endregion

        #region Create or update or Delete a jobAlert

        public ActionResult CreateJobAlert()
        {
            this.ViewBag.Date = DateTime.Now;
            this.GetClientCompaniesInfo();
            this.GetServicePointCollection();
            this.GetCompanyRig();
            this.GetCompanyClientConsultant();

            return this.PartialView("_CreateJobAlert");
        }

        [HttpPost]
        public ActionResult CreateJobAlert(RigJobModel model)
        {
            RigJob rigJob = new RigJob();
            model.PopulateToJobAlert(rigJob);
            rigJob.JobLifeStatus = JobLifeStatus.Alerted;
            rigJob.IsListed = true;
            if (!model.ClientCompanyId.Equals(0))
            {
                rigJob.ClientCompany = this._context.GetClientCompanyById(model.ClientCompanyId);
                    rigJob.ClientCompanyShortName = rigJob.ClientCompany.ShortName;
            }
            if (!model.ConsultantId.Equals(0))
                rigJob.ClientConsultant1 = CacheData.ClientConsultants.FirstOrDefault(a => a.Id == model.ConsultantId);
            if (!model.ServicePointId.Equals(0))
                rigJob.ServicePoint = CacheData.ServicePointCollections.FirstOrDefault(a => a.Id == model.ServicePointId);
            if (!model.RigId.Equals(0))
            {
                rigJob.Rig = this._context.GetRigInfoByRigId(model.RigId);
                rigJob.RigStatus = rigJob.Rig.Status;
                rigJob.IsServiceRig = model.IsServiceRig;
                rigJob.IsProjectRig = model.IsProjectRig;
            }

            this._context.CreateMicroRigJob(rigJob);

            return this.RedirectToAction("Index", "RigBoard");
        }

        public ActionResult UpdateJobAlert(List<string> parms)
        {
            this.ViewBag.CreateOrUpdate = "Update";
            RigJob rigJob = this._context.GetRigJobById(Convert.ToInt32(parms[0]));
            rigJob.ClientCompany = this._context.GetClientCompanyById(rigJob.ClientCompany?.Id ?? 0);
            RigJobModel model = new RigJobModel();
            model.PopulateFromJobAlert(rigJob);
            this.ViewBag.Date = model.Date;
            this.GetServicePointCollection();
            this.GetCompanyRig();
            this.GetCompanyClientConsultant();

            return this.PartialView("_UpdateJobAlert", model);
        }

        [HttpPost]
        public ActionResult UpdateJobAlert(RigJobModel model)
        {
            RigJob rigJob = this._context.GetRigJobById(model.Id);
            if (rigJob != null)
            {
                RigJob newrRigJob = rigJob;
                model.PopulateToJobAlert(rigJob);
                if (!model.ClientCompanyId.Equals(0))
                    newrRigJob.ClientCompany = this._context.GetClientCompanyById(model.ClientCompanyId);
                if (!model.ServicePointId.Equals(0))
                    newrRigJob.ServicePoint = CacheData.ServicePointCollections.FirstOrDefault(a => a.Id == model.ServicePointId);
                if (!model.RigId.Equals(0))
                {
                    newrRigJob.Rig = this._context.GetRigInfoByRigId(model.RigId);
                    rigJob.RigStatus = rigJob.Rig.Status;
                    newrRigJob.IsProjectRig = model.IsProjectRig;
                    newrRigJob.IsServiceRig = model.IsServiceRig;
                }
                if (!model.ConsultantId.Equals(0) && CacheData.ClientConsultants.FirstOrDefault(a => a.Id == model.ConsultantId && a.Name == model.ConsultantName) != null)
                {
                    newrRigJob.ClientConsultant1 = CacheData.ClientConsultants.FirstOrDefault(a => a.Id == model.ConsultantId && a.Name == model.ConsultantName);
                }
                else
                {
                    newrRigJob.ClientConsultant1 = rigJob.ClientConsultant1;
                }

                this._context.UpdateRigJob(newrRigJob, rigJob);
            }

            return this.RedirectToAction("Index", "RigBoard");
        }

        public ActionResult DeleteJobAlert(List<string> parms)
        {
            this.ViewBag.RigJobId = parms[0];

            return this.PartialView("_IsDeleteJobAlert");
        }

        [HttpPost]
        public ActionResult DeleteJobAlert(RigJobModel model)
        {
            string rigJobId = this.Request.Form["RigJobId"];
            RigJob rigJob = this._context.GetRigJobById(Convert.ToInt32(rigJobId));
            this._context.DeleteRigJob(rigJob);

            return this.RedirectToAction("Index", "RigBoard");
        }
        #endregion

        #region Company Operation

        public ActionResult UpdateCompanyShortName(List<string> parms)
        {
            RigJob rigJob = eServiceWebContext.Instance.GetRigJobById(Convert.ToInt32(parms[0]));
            RigJobModel model = new RigJobModel();
            if (rigJob != null)
            {
                model.Id = rigJob.Id;
                model.ClientCompanyId = rigJob.ClientCompany.Id;
                model.Company = rigJob.ClientCompanyShortName;
            }

            return this.PartialView("_UpdateCompanyShortName", model);
        }

        [HttpPost]
        public ActionResult UpdateCompanyShortName(RigJobModel model)
        {
            eServiceWebContext.Instance.UpdateCompanyShortName(model.Id,model.ClientCompanyId,model.Company);

            return this.RedirectToAction("Index", "RigBoard");
        }
        #endregion

        #region RigJob Operation

        public ActionResult PostponeJob(List<string> parms) 
        {
            RigJob rigJob = eServiceWebContext.Instance.GetRigJobById(Convert.ToInt32(parms[0]));
            RigJobModel model = new RigJobModel();
            if (rigJob != null)
            {
                if (rigJob.JobLifeStatus == JobLifeStatus.Canceled || rigJob.JobLifeStatus == JobLifeStatus.Completed)
                {
                    this.ViewBag.ErrorMessage =
                        $"This job has been completed or canceled. \n\nClose this message to allow Rig Board refreshed.";
                    return this.PartialView("_JobStatusError");
                }
                else
                {
                    model.Id = rigJob.Id;
                    ClientCompany clientCompany =
                        eServiceWebContext.Instance.GetClientCompanyById(rigJob.ClientCompany?.Id ?? 0);
                    model.Company = clientCompany.ShortName;
                    model.LSD = !string.IsNullOrEmpty(rigJob.SurfaceLocation) ? rigJob.SurfaceLocation : rigJob.WellLocation;
                    model.RigName = rigJob.Rig?.Name;
                    model.JobType = rigJob.JobType?.Name;
                    this.ViewBag.JobDateTime = rigJob.JobDateTime;
                    return this.PartialView("_PostponeJob", model);
                }
            }

            this.ViewBag.ErrorMessage =
                $"This job cannot be found anymore. \n\nClose this message to allow Rig Board refreshed.";
            return this.PartialView("_JobStatusError");
        }

        [HttpPost]
        public ActionResult PostponeJob(RigJobModel model)
        {
            RigJob rigJob = this._context.GetRigJobById(model.Id);
            if (rigJob != null)
            {
                rigJob.JobDateTime = model.Date;
                this._context.UpdateRigJob(rigJob, rigJob);
                if (model.IsReleaseCrew)
                {
                    eServiceWebContext.Instance.ReleaseCrew(rigJob.Id, false, DateTime.Now);
                }
                else
                {
                    eServiceWebContext.Instance.UpdateRigJobCrewSchedule(rigJob);
                }
            }

            return this.RedirectToAction("Index", "RigBoard");
        }

        #endregion

        #region Crew Operation

        public JsonResult GetEffectiveCrews(DateTime startTime, double duration, int workingDistrict, int rigJobId)
        {
            List<SanjelCrew> crews = this._context.GetEffectiveCrews(startTime, duration, workingDistrict, rigJobId).OrderBy(p => p.Name).ToList();

            return new JsonResult(crews);
        }

        public ActionResult AssignACrew(List<string> parms)
        {
            this.GetServicePoints();
            DateTime callCrewTime = this._context.GetCallCrewTime(Int32.Parse(parms[0]));
            CrewModel model = new CrewModel {CallCrewTime = callCrewTime, EstJobDuration = 8, RigJobId = Int32.Parse(parms[0])};
            RigJob rigJob = this._context.GetRigJobById(Int32.Parse(parms[0]));
            model.WorkingDistrictId = rigJob.ServicePoint.Id;

            return this.PartialView("_AssignACrew", model);
        }

        public JsonResult VerifySanjelCrewSchedule(int sanjelCrewId, DateTime startTime, double duration)
        {
            string messageInfo = eServiceWebContext.Instance.VerifySanjelCrewSchedule(sanjelCrewId, startTime, startTime.AddHours(duration));

            return this.Json(messageInfo);
        }

        private void GetServicePoints()
        {
            List<ServicePoint> servicePoints = this._context.GetServicePoints().OrderBy(p => p.Name).ToList();
            List<SelectListItem> servicePointItems = servicePoints.Select(p => new SelectListItem { Text = p.Name, Value = p.Id.ToString()}).ToList();
            this.ViewData["servicePoints"] = servicePointItems;
        }

        [HttpPost]
        public ActionResult AssignACrew(CrewModel model)
        {
            this._context.AssignACrew(model.Id, model.RigJobId, model.CallCrewTime, model.EstJobDuration);
            eServiceWebContext.Instance.UpdateCallSheetAndRigJob(model.RigJobId);
            return this.RedirectToAction("Index", "RigBoard");
        }

        public ActionResult WithdrawACrew(List<string> parms)
        {
            RigJobCrewSectionModel model = new RigJobCrewSectionModel();
            model.RigJobId = Int32.Parse(parms[0]);
            model.CrewId = Int32.Parse(parms[1]);
            model.JobCrewSectionStatusId = Int32.Parse(parms[2]);

            return this.PartialView("_WithdrawACrew", model);
        }

        [HttpPost]
        public ActionResult WithdrawACrew(RigJobCrewSectionModel model)
        {
            this._context.WithdrawACrew(model.RigJobId, model.CrewId, model.JobCrewSectionStatusId);
            eServiceWebContext.Instance.UpdateCallSheetAndRigJob(model.RigJobId);
            return this.RedirectToAction("Index", "RigBoard");
        }

        public ActionResult CallAllCrews(List<string> parms)
        {
            CrewModel model = new CrewModel();
            model.RigJobId = Int32.Parse(parms[0]);

            return this.PartialView("_CallCrew", model);
        }

        [HttpPost]
        public ActionResult CallAllCrews(CrewModel model)
        {
            this._context.CallAllCrews(model.RigJobId);
            return this.RedirectToAction("Index", "RigBoard");
        }

        public ActionResult LogOnDuty(List<string> parms)
        {
            CrewModel model = new CrewModel();
            model.RigJobId = Int32.Parse(parms[0]);

            return this.PartialView("_LogOnDuty", model);
        }

        [HttpPost]
        public ActionResult LogOnDuty(CrewModel model)
        {
            this._context.LogOnDuty(model.RigJobId);
            return this.RedirectToAction("Index", "RigBoard");
        }

        public ActionResult LogOffDuty(List<string> parms)
        {
            CrewModel model = new CrewModel();
            model.RigJobId = Int32.Parse(parms[0]);

            return this.PartialView("_LogOffDuty", model);
        }

        [HttpPost]
        public ActionResult LogOffDuty(CrewModel model)
        {
            this._context.LogOffDuty(model.RigJobId);
            return this.RedirectToAction("Index", "RigBoard");
        }

        public ActionResult AdjustJobDuration(List<string> parms)
        {
            //On UI, job duration is by hour
            CrewModel crewModel=new CrewModel();
            crewModel.RigJobId = Int32.Parse(parms[0]);
            var rigJob = eServiceWebContext.Instance.GetRigJobById(crewModel.RigJobId);
            if (rigJob.JobDuration == 0) crewModel.EstJobDuration = 6;
            else crewModel.EstJobDuration = rigJob.JobDuration / 60;

            return this.PartialView("_AdjustJobDuration", crewModel);
        }

        [HttpPost]
        public ActionResult AdjustJobDuration(CrewModel crewModel)
        {
            //On UI, job duration is by hour
            this._context.AdjustJobDuration(crewModel.RigJobId, crewModel.EstJobDuration);
            return this.RedirectToAction("Index", "RigBoard");
        }
        public static List<ClientConsultant> GetConsultantContacts(RigJob rigJob)
        {
            List<ClientConsultant> list = new List<ClientConsultant>();
            if (rigJob.ClientConsultant1 != null && (!string.IsNullOrEmpty(rigJob.ClientConsultant1.Name)))
            {
                var clientConsultant1 = CacheData.ClientConsultants.FirstOrDefault(p => p.Id == rigJob.ClientConsultant1.Id);
                if (clientConsultant1 != null) 
                    list.Add(clientConsultant1);
                else
                    list.Add(rigJob.ClientConsultant1);            }
            if (rigJob.ClientConsultant2 != null && (!string.IsNullOrEmpty(rigJob.ClientConsultant2.Name)))
            {
                var clientConsultant2 = CacheData.ClientConsultants.FirstOrDefault(p => p.Id == rigJob.ClientConsultant2.Id);
                if (clientConsultant2 != null) 
                    list.Add(clientConsultant2);
                else
                    list.Add(rigJob.ClientConsultant2);            }
            return list;
        }
        #endregion

 
        #region District Notes

        public ActionResult GetDistrictRigJobViewModels([FromBody] DataManager dataManager)
        {
            List<DistrictNoteViewModel> data=new List<DistrictNoteViewModel>();
            DistrictNoteViewModel districtNoteViewModel=new DistrictNoteViewModel();
            if (this.HttpContext.Session.GetString("ServicePoint") == null)
            {
                string retrievalstr = JsonConvert.SerializeObject(new RetrievalCondition());
                this.HttpContext.Session.SetString("ServicePoint", retrievalstr);
            }
            RetrievalCondition retrieval =
                JsonConvert.DeserializeObject<RetrievalCondition>(this.HttpContext.Session.GetString("ServicePoint"));
            List<Collection<int>> resuhSet = retrieval.ResuhSet(retrieval);
            Collection<int> servicePoints = Utility.GetSearchCollections(resuhSet[0]);

            if (servicePoints.Count==1)
            {
                districtNoteViewModel.PopulateFrom(this._context.GetServicePointNoteByServicePointId(servicePoints[0]));            
            }
            else
            {
                districtNoteViewModel.DistrictNotes = new StyledCell("DistrictNotes", null, LoggedUser, null) { PropertyValue = "District notes is displayed only when single district is selected." };
            }
   
            
            data.Add(districtNoteViewModel);
            return this.Json(new {result = data});
        }


        public IActionResult UpdateDistrictNotes(List<string> parms)
        {
            NoteModel noteModel = new NoteModel();
            noteModel.Id = int.Parse(parms[0]);
            noteModel.ReturnActionName = parms[1];
            noteModel.ReturnControllerName = parms[2];
            noteModel.PostControllerName = parms[3];
            noteModel.PostMethodName = parms[4];
            noteModel.Notes = parms.Count > 5 ? parms[5] : string.Empty;
            return PartialView("~/Views/Shared/_UpdateNotes.cshtml", noteModel);

        }
        [HttpPost]
        public IActionResult UpdateDistrictNotes(NoteModel model)
        {
            eServiceWebContext.Instance.UpdateServicePointNote(model.Id, model.Notes);
            return this.RedirectToAction(model.ReturnActionName, model.ReturnControllerName);
        }
        #endregion

        #region assgin plug loading head 

        public IActionResult AssignPlugLoadingHead(List<string> parms)
        {
            AssginPlugLoadingHeadModel model=new AssginPlugLoadingHeadModel();
            RigJob rigJob = this._context.GetRigJobById(int.Parse(parms[0]));
            Rig rig = this._context.GetRigInfoByRigId(rigJob?.Rig?.Id ?? 0);

            int servicePointId = rigJob?.ServicePoint?.Id ?? 0;
            model.TopDriveAdapterRequired = rig.IsTopDrive;
            model.ServicePointId = servicePointId;
            model.RigJobId = int.Parse(parms[0]);
            model.CallSheetNumber=Int32.Parse(parms[1]);

            ViewBag.plugLoadingList = this.GetPlugLoadingHeadsByServicePoint(servicePointId);
            ViewBag.ManifoldList = this.GetManifoldsByServicePoint(servicePointId);
            ViewBag.plugLoadingHeadSubList = this.GetPlugLoadingHeadSubsByServicePoint(servicePointId);
            ViewBag.topDriveAdaptorList = this.GetTopDriveAdaptorsByServicePoint(servicePointId);
            return PartialView("_AssginPlugLoadingHead", model);

        }

        private List<SelectListItem> GetPlugLoadingHeadsByServicePoint(int servicePointId)
        {
          
            List<PlugLoadingHead> plugLoadingHeads = this._context.GetPlugLoadingHeadsByServicePoint(servicePointId);
            List<SelectListItem> plugLoadingHeadItems = plugLoadingHeads.Select(p => new SelectListItem {Text =$"{p.PlugLoadingHeadSize.Name} {p.PlugLoadingHeadThreadType.Name} {p.PlugLoadingHeadSpecialty.Name} {p.PlugLoadingHeadType.Name} {p.Id}",Value = p.Id.ToString()}).ToList();
            return plugLoadingHeadItems;
        }

        private List<SelectListItem> GetPlugLoadingHeadSubsByServicePoint(int servicePointId)
        {
            List<PlugLoadingHeadSub> plugLoadingHeadSubs = this._context.GetPlugLoadingHeadSubsByServicePoint(servicePointId);
            List<SelectListItem> plugLoadingHeadSubitems= plugLoadingHeadSubs.Select(p => new SelectListItem { Text = $"{p.PlugLoadingHeadSubSize.Name} {p.PlugLoadingHeadSubThreadType.Name} {p.Id}", Value = p.Id.ToString() }).ToList();
            return plugLoadingHeadSubitems;
        }

        private List<SelectListItem> GetTopDriveAdaptorsByServicePoint(int servicePointId)
        {
            List<TopDriveAdaptor> topDriveAdaptors = this._context.GetTopDriveAdaptorsByServicePoint(servicePointId);
            List<SelectListItem> plugLoadingHeadSubitems= topDriveAdaptors.Select(p => new SelectListItem { Text = $"{p.TopDrivceAdaptorSize.Name} {p.TopDrviceAdaptorThreadType.Name} {p.Id}", Value = p.Id.ToString() }).ToList();
            return plugLoadingHeadSubitems;
        }
        public List<SelectListItem> GetManifoldsByServicePoint(int servicePointId)
        {
            List<Manifold> manifolds = this._context.GetManifoldsByServicePoint(servicePointId);
            List<SelectListItem> manifolditems = manifolds.Select(p => new SelectListItem { Text = $"{p.ManifoldType.Name} {p.Id}", Value = p.Id.ToString()}).ToList();
            return manifolditems;
                                     
        }

        public JsonResult PlugLoadingHeadhasManifold(int plugLoadingHeadId)
        {
            Manifold manifold= this._context.PlugLoadingHeadhasManifold(plugLoadingHeadId);
            if (manifold!=null&&manifold.Id!=0)
            {
                List<SelectListItem> item = new List<SelectListItem>() { new SelectListItem(){ Text = $"{manifold.ManifoldType.Name} {manifold.Id}", Value = manifold.Id.ToString() }  };
                return new JsonResult(new {flag=true,data= item });
            }
            return new JsonResult(new {flag=false,Data=""});
        }

        [HttpPost]
        public IActionResult AssignPlugLoadingHead(AssginPlugLoadingHeadModel model)
        {
            this._context.AssignPlugLoadingHead(model);
            return this.RedirectToAction("Index", "RigBoard");
        }

        #endregion

        
        #region return Equipments

        public IActionResult ReturnEquipments(List<string> parms)
        {
           List<ReturnEquipentsModel> models=new List<ReturnEquipentsModel>();
           List<PlugLoadingHeadInformation> plugLoadingHeadInformations = this._context.GetPlugLoadingHeadInformationByRigJobId(int.Parse(parms[0])).OrderBy(s => s.Id).ToList();


           foreach (var plugLoadingHeadInformation in plugLoadingHeadInformations)
           {
               List<ReturnEquipentsModel> equipentsModel;
               equipentsModel = this.ConvertEquipent(plugLoadingHeadInformation);
               models.AddRange(equipentsModel);
           }

           this.GetNubbinEquipments(models,int.Parse(parms[0]));
           this.GetSwedgeEquipments(models,int.Parse(parms[0]));
           this.GetWitsBoxEquipments(models,int.Parse(parms[0]));
            this.GetServicePoints();
          ViewData["RigJobId"] = parms[0];
          return PartialView("_ReturnEquipments",models);
        }

        private void GetNubbinEquipments(List<ReturnEquipentsModel> models,int rigJobId)
        {
            List<NubbinInformation> nubbinInformations = this._context.GetNubbinInformationByRigJobId(rigJobId).OrderBy(s => s.Id).ToList();
            foreach (var nubbinInformation in nubbinInformations)
            {
                ReturnEquipentsModel model=new ReturnEquipentsModel();
                model.EquipentName = $"{nubbinInformation.Nubbin.NubbinSize.Name} {nubbinInformation.Nubbin.NubbinThreadType.Name} {nubbinInformation.Nubbin.Id}";
                model.ServicePointId = nubbinInformation.WorkingServicePoint.Id;
                model.Id = nubbinInformation.Nubbin?.Id ?? 0;
                model.EquipentType = EquipentType.Nubbin;
                model.WhetherToReturn = true;
                models.Add(model);
            }
        }
        private void GetSwedgeEquipments(List<ReturnEquipentsModel> models, int rigJobId)
        {
            List<SwedgeInformation> swedgeInformations = this._context.GetSwedgeInformationByRigJobId(rigJobId).OrderBy(s => s.Id).ToList();
            foreach (var swedgeInformation in swedgeInformations)
            {
                ReturnEquipentsModel model = new ReturnEquipentsModel();
                model.EquipentName = $"{swedgeInformation.Swedge.SwedgeSize.Name} {swedgeInformation.Swedge.SwedgeThreadType.Name} {swedgeInformation.Swedge.Id}";
                model.ServicePointId = swedgeInformation.WorkingServicePoint.Id;
                model.Id = swedgeInformation.Swedge?.Id ?? 0;
                model.EquipentType = EquipentType.Swedge;
                model.WhetherToReturn = true;
                models.Add(model);
            }
        }
        private void GetWitsBoxEquipments(List<ReturnEquipentsModel> models, int rigJobId)
        {
            List<WitsBoxInformation> witsBoxInformations = this._context.GetWitsBoxInformationByRigJobId(rigJobId).OrderBy(s => s.Id).ToList();
            foreach (var witsBoxInformation in witsBoxInformations)
            {
                ReturnEquipentsModel model = new ReturnEquipentsModel();
                model.EquipentName = $"{witsBoxInformation.WitsBox.Name} {witsBoxInformation.WitsBox.Id}";
                model.ServicePointId = witsBoxInformation.WorkingServicePoint.Id;
                model.Id = witsBoxInformation.WitsBox?.Id ?? 0;
                model.EquipentType = EquipentType.WitsBox;
                model.WhetherToReturn = true;
                models.Add(model);
            }
        }
        [HttpPost]
        public IActionResult ReturnEquipments()
        {
            var rigJobId = Request.Form["RigJobId"];
            List<ReturnEquipentsModel> models = this.GetRecyclingEquipment();
            this._context.ReturnEquipments(models);
//            this._context.ChangeRigJobStatusToComplete(int.Parse(rigJobId));
            return RedirectToAction("Index","RigBoard");
        }

        public IActionResult CompleteJob(List<string> parms)
        {
            JobCompletionModel model = new JobCompletionModel();
            model.RigJobId = Convert.ToInt32(parms[0]);
            return PartialView("_CompleteJob", model);
        }
        [HttpPost]
        public IActionResult CompleteJob(JobCompletionModel model)
        {
            if (model.IsClearAll)
            {
                var rigJob = eServiceOnlineGateway.Instance.GetRigJobById(model.RigJobId);
                if (rigJob != null && rigJob.Rig.Id > 0)
                {
                    //Set all related product haul on location
                    //Dec 14, 2023 AW: Disable auto product haul on location feature, rely on Job Board operation
                    /*
	                List<ShippingLoadSheet> shippingLoadSheets = eServiceOnlineGateway.Instance.GetShippingLoadSheetsByCallSheetNumber(rigJob.CallSheetNumber);
	                foreach (var shippingLoadSheet in shippingLoadSheets)
	                {
		                eServiceWebContext.Instance.UpdateShippingLoadSheetOnLocation(shippingLoadSheet, DateTime.Now, LoggedUser);
	                }

	                //Empty all bins
                    List<BinInformation> list = eServiceOnlineGateway.Instance.GetBinInformationsByRigId(rigJob.Rig.Id);
                    foreach (var item in list)
                    {
                        BinProcess.EmptyBin(item.Id, "Empty Bin when job is completed from Rig Board", LoggedUser);
                    }
	                */
                }
            }
            //Use Now as job complete time until we add a DateTime control to UI
            this._context.ChangeRigJobStatusToComplete(model.RigJobId, DateTime.Now);

            return RedirectToAction("Index", "RigBoard");
        }
        private List<ReturnEquipentsModel> GetRecyclingEquipment()
        {
            var ids = Request.Form["item.Id"];
            var check = Request.Form["WhetherToReturn"];
            var equipentTypes = Request.Form["item.EquipentType"];
            var servicePoints = Request.Form["item.ServicePointId"];

            var idStr = Utility.SplitString(ids);
            var checkStr = Utility.SplitString(check);
            var equipentTypesStr = Utility.SplitString(equipentTypes);
            var servicePointsStr = Utility.SplitString(servicePoints);

            List<ReturnEquipentsModel> returnEquipentsModels=new List<ReturnEquipentsModel>();
            if (idStr!=null)
            {
                for (int i = 0; i < idStr.Length; i++)
                {
                    ReturnEquipentsModel model=new ReturnEquipentsModel();
                    model.Id =int.Parse(idStr[i]);
                    model.WhetherToReturn = bool.Parse(checkStr[i]);
                    model.EquipentType = (EquipentType)Enum.Parse(typeof(EquipentType),equipentTypesStr[i],false) ;
                    model.ServicePointId = int.Parse(servicePointsStr[i]);
                    returnEquipentsModels.Add(model);
                }
            }
            return returnEquipentsModels;
        }

        private List<ReturnEquipentsModel> ConvertEquipent(PlugLoadingHeadInformation plugLoadingHeadInformation)
        {
            List<ReturnEquipentsModel> models = new List<ReturnEquipentsModel>();

            if (plugLoadingHeadInformation.PlugLoadingHead!=null) models.Add(this.GetPlugLoadingHeadModel(plugLoadingHeadInformation));
            if (plugLoadingHeadInformation.Manifold!=null) models.Add(this.GetManifoldModel(plugLoadingHeadInformation));
            if (plugLoadingHeadInformation.TopDriveAdaptor != null) models.Add(this.GetTopDriveAdapterModel(plugLoadingHeadInformation));
            if (plugLoadingHeadInformation.PlugLoadingHeadSub != null) models.Add(this.GetplugLoadingHeadSubModel(plugLoadingHeadInformation));
            return models;
        }

        private ReturnEquipentsModel GetPlugLoadingHeadModel(PlugLoadingHeadInformation plugLoadingHeadInformation)
        {
            ReturnEquipentsModel plugLoadingHeadModel = new ReturnEquipentsModel();
            plugLoadingHeadModel.EquipentName = $"{plugLoadingHeadInformation.PlugLoadingHead?.PlugLoadingHeadSize?.Name} {plugLoadingHeadInformation.PlugLoadingHead?.PlugLoadingHeadThreadType?.Name} {plugLoadingHeadInformation.PlugLoadingHead?.PlugLoadingHeadSpecialty?.Name} {plugLoadingHeadInformation.PlugLoadingHead?.PlugLoadingHeadType?.Name} {plugLoadingHeadInformation.PlugLoadingHead?.Id}";
            plugLoadingHeadModel.ServicePointId = plugLoadingHeadInformation.WorkingServicePoint.Id;
            plugLoadingHeadModel.EquipentType = EquipentType.PlugLoadingHead;
            plugLoadingHeadModel.Id = plugLoadingHeadInformation.PlugLoadingHead?.Id??0;
            plugLoadingHeadModel.WhetherToReturn = true;
            return plugLoadingHeadModel;
        }
        private ReturnEquipentsModel GetManifoldModel(PlugLoadingHeadInformation plugLoadingHeadInformation)
        {
            ReturnEquipentsModel manifoldModel = new ReturnEquipentsModel();
            manifoldModel.EquipentName = $"{plugLoadingHeadInformation.Manifold?.ManifoldType?.Name} {plugLoadingHeadInformation.Manifold?.Id}";
            manifoldModel.ServicePointId = plugLoadingHeadInformation.WorkingServicePoint.Id;
            manifoldModel.EquipentType = EquipentType.Manifold;
            manifoldModel.Id = plugLoadingHeadInformation.Manifold?.Id??0;
            manifoldModel.WhetherToReturn = true;
            return manifoldModel;
        }
        private ReturnEquipentsModel GetTopDriveAdapterModel(PlugLoadingHeadInformation plugLoadingHeadInformation)
        {
            ReturnEquipentsModel topDriveAdapterModel = new ReturnEquipentsModel();
            topDriveAdapterModel.EquipentName = $"{plugLoadingHeadInformation.TopDriveAdaptor?.TopDrivceAdaptorSize?.Name} {plugLoadingHeadInformation.TopDriveAdaptor?.TopDrviceAdaptorThreadType?.Name} {plugLoadingHeadInformation?.TopDriveAdaptor?.Id}";
            topDriveAdapterModel.ServicePointId = plugLoadingHeadInformation.WorkingServicePoint.Id;
            topDriveAdapterModel.EquipentType = EquipentType.TopDrivceAdaptor;
            topDriveAdapterModel.Id = plugLoadingHeadInformation.TopDriveAdaptor?.Id??0;
            topDriveAdapterModel.WhetherToReturn = true;
            return topDriveAdapterModel;
        }
        private ReturnEquipentsModel GetplugLoadingHeadSubModel(PlugLoadingHeadInformation plugLoadingHeadInformation)
        {
            ReturnEquipentsModel plugLoadingHeadSubModel = new ReturnEquipentsModel();
            plugLoadingHeadSubModel.EquipentName = $"{plugLoadingHeadInformation.PlugLoadingHeadSub?.PlugLoadingHeadSubSize?.Name} {plugLoadingHeadInformation.PlugLoadingHeadSub?.PlugLoadingHeadSubThreadType?.Name} {plugLoadingHeadInformation.PlugLoadingHeadSub?.Id}";
            plugLoadingHeadSubModel.ServicePointId = plugLoadingHeadInformation.WorkingServicePoint.Id;
            plugLoadingHeadSubModel.EquipentType = EquipentType.PlugLoadingHeadSub;
            plugLoadingHeadSubModel.Id = plugLoadingHeadInformation.PlugLoadingHeadSub?.Id??0;
            plugLoadingHeadSubModel.WhetherToReturn = true;
            return plugLoadingHeadSubModel;
        }
        #endregion

        public ActionResult GetPrograms(int customerId)
        {
             List<SelectListItem> binList =  new List<SelectListItem>()
            {
                new SelectListItem() { Value = "22255", Text = "PRG2101899"},
                new SelectListItem() { Value = "22254", Text = "PRG2101898"},
                new SelectListItem() { Value = "22253", Text = "PRG2101897"},
                new SelectListItem() { Value = "22252", Text = "PRG2101896"},
            };
            return this.Json(binList);
        }

        public ActionResult GetProgramInfo(string programId)
        {
            ProgramInfo programinfo=new ProgramInfo();

            //Nov 7, 2023 tongtao P45_Q4_167: add ProgramId format validation,When programId does not match the specified format or the characters after the point are not numeric, return an empty data set.
            if (string.IsNullOrEmpty(programId) || string.IsNullOrEmpty(programId.Trim()) || (!programId.Contains('.'))) return  this.Json(programinfo);

            string programNumber = programId.Split('.')[0];

            int revision = 0;

            try
            {
                revision = Convert.ToInt32(programId.Split('.')[1]);
            }
            catch
            {
                return this.Json(programinfo);
            }

            var jobDesign = eServiceOnlineGateway.Instance.GetJobDesignByProgramIdAndRevision(programNumber, revision);


            if (jobDesign != null)
            {
                    programinfo.ClientName = jobDesign.ClientCompany.Name;
                    programinfo.CustomerId = jobDesign.ClientCompany.Id;
                    programinfo.ServicePointId = jobDesign.ServicePoint.Id;
                    programinfo.ServicePointName = jobDesign.ServicePoint.Name;

                if (jobDesign.JobDesignPumpingJobSection != null && jobDesign.JobDesignPumpingJobSection.Count > 0)
                {
                    programinfo.JobTypes = new Dictionary<int, JobInfoSection>(){{0, new JobInfoSection(){ JobType = "None"} }};
                    foreach (var programPumpingJobSection in jobDesign.JobDesignPumpingJobSection)
                    {
                        var jobType = new JobInfoSection();
                        jobType.JobType = programPumpingJobSection.JobType.Name;
                        jobType.BlendSectionList = new Dictionary<int, BlendInfoSection>(){{0,new BlendInfoSection(){Description = "None", MixWater = 0}}};
                        var blendSections =
                            eServiceOnlineGateway.Instance.GetBlendSectionByJobDesignPumpingSectionId(
                                programPumpingJobSection.Id);
                        foreach (var blendSection in blendSections)
                        {
                            jobType.BlendSectionList.Add(blendSection.Id, new BlendInfoSection(){Description = blendSection.BlendCategory.Name + " - " + blendSection.BlendFluidType.Name,MixWater = blendSection.MixWaterRequirement, IsBlendTest = blendSection.IsNeedFieldTesting });
                        }
                        programinfo.JobTypes.Add(programPumpingJobSection.JobType.Id, jobType);
                    }

                }
            }

            return this.Json(programinfo);
        }
        public ActionResult GetBlends(string programId, int jobSectionId)
        {
            List<SelectListItem> blendList = new List<SelectListItem>();

            var program = eServiceOnlineGateway.Instance.GetProgramByProgramId(programId);
            ProgramInfo programinfo=new ProgramInfo();
            if (program != null)
            {
                if (program.PumpingJobSections != null && program.PumpingJobSections.Count > 0)
                {
                    foreach (var programPumpingJobSection in program.PumpingJobSections)
                    {
                        if (programPumpingJobSection.Id != jobSectionId) continue;
                        if (programPumpingJobSection.ProductSection?.BlendSections?.Count > 0)
                        {
                            foreach (var productSectionBlendSection in programPumpingJobSection.ProductSection.BlendSections)
                            {
                                blendList.Add(new SelectListItem() { Value = productSectionBlendSection.Id.ToString(), Text = productSectionBlendSection.BlendCategory + " - " + productSectionBlendSection.BlendFluidType.Name });
                            }
                        }
                    }
                }
            }

            return this.Json(blendList);
        }
        //Nov 29, 2023 zhangyuan 203_PR_UpdateBinNotes: Add UpdateBinNotes
        public ActionResult UpdateBinNotes(List<string> parms)
        {
            int BinId = Convert.ToInt32(parms[0]);
            int PodIndex = Convert.ToInt32(parms[1]);
            Bin bin = BinId > 0 ? this._context.GetBinById(BinId) : null;
            if (bin == null) return null;
            NoteModel noteModel = GetNoteModel(bin, PodIndex);

            return PartialView("~/Views/Shared/_UpdateNotes.cshtml", noteModel);
        }
        //Nov 29, 2023 zhangyuan 203_PR_UpdateBinNotes: Set Model Attribution
        private NoteModel GetNoteModel(Bin bin,int PodIndex)
        {
            NoteModel noteModel = new NoteModel();
            BinNote note = this._context.GetBinNoteByBinAndPodIndex(bin, PodIndex);
            noteModel.Id = bin.Id;
            noteModel.PodIndex = PodIndex;
            noteModel.Notes = note?.Description ?? string.Empty;
            noteModel.ReturnActionName = "Index";
            noteModel.ReturnControllerName = "RigBoard";
            noteModel.PostControllerName = "BinBoard";
            noteModel.PostMethodName = "UpdateNotes";

            return noteModel;
        }
    }
}

public class SwitchNoteView
{
    public string JobInformation { get; set; }
    public string JobDateTime { get; set; }
    public string Notes { get; set; }
}

public class ProgramInfo
{
    public string ClientName { get; set; }
    public int CustomerId { get; set; }
    public string ServicePointName { get; set; }
    public int ServicePointId { get; set; }
    public Dictionary<int, JobInfoSection> JobTypes { get; set; } 
}

public class JobInfoSection
{
    public string JobType { get; set; }
    public Dictionary<int, BlendInfoSection> BlendSectionList { get; set; }
}

public class BlendInfoSection
{
    public string Description { get; set; }
    public double MixWater { get; set; }
    public bool IsBlendTest { get; set; }
}

