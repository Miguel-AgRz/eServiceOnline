using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using eServiceOnline.BusinessProcess;
using eServiceOnline.Data;
using eServiceOnline.Gateway;
using eServiceOnline.Models.ProductHaul;
using MetaShare.Common.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NUnit.Framework;
using Sesi.SanjelData.Entities.BusinessEntities.BaseEntites;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Crew;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ResourceSchedule;
using Sesi.SanjelData.Entities.BusinessEntities.WellSite;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Products;
using Sesi.SanjelData.Services.Interfaces.BusinessEntities.Operation.Dispatch;
using Utility = eServiceOnline.Controllers.Utility;

namespace eServiceOnline.IntegrationTest
{
    [TestFixture]
    public class RigJobBlendProductHaulTest: IntegrationTestBase
    {
        protected int callSheetNumber = 0;
        protected int callSheetId = 0;
        protected int callSheetBlendSectionId = 0;
        protected int productHaulId = 0;
        protected int productHaulLoadId = 0;
        protected RigJob rigJob = null;
        protected CallSheetBlendSection blendSection = null;
        protected int crewId = 0;
        protected int reCrewId = 0;
        private int rigJobId = 0;
        private string loggedUser = "awang";
        private int bulkPlantId = 0;
        private string bulkPlantName = string.Empty;

        [SetUp]
        public void Setup()
        {
            //Find the last usable blend for test

            var latestPendingJobs =
                eServiceOnlineGateway.Instance.GetRigJobsByQuery(null, p => p.JobLifeStatus == JobLifeStatus.Pending).OrderByDescending(p => p.Id);
            foreach (var latestPendingJob in latestPendingJobs)
            {
                var availableBlends = eServiceOnlineGateway.Instance.GetBlendSectionsByCallSheetAndBlend(latestPendingJob.CallSheetNumber);
                bool foundIt = false;
                if (availableBlends != null && availableBlends.Count > 0)
                {
                    foreach (var callSheetBlendSection in availableBlends)
                    {
                        if (callSheetBlendSection.BlendCategory.Name == "Lead 1")
                        {
                            callSheetBlendSectionId = callSheetBlendSection.Id;
                            callSheetNumber = latestPendingJob.CallSheetNumber;
                            callSheetId = latestPendingJob.CallSheetId;
                            blendSection = callSheetBlendSection;
                            foundIt = true;
                            break;
                        }
                    }
                }

                if (foundIt)
                {
                    rigJob = latestPendingJob;
                    break;
                }

            }



            //Get Crew Info
            var itemCrews = RigBoardProcess.GetSanjeBulkerCrew().Where(s => s.Name != null && s.Name.Contains("|"))
                .OrderBy(s => s.Name).ToList();

            if (itemCrews != null)
            {
                crewId = itemCrews[0].Id;

                reCrewId = itemCrews[1].Id;
            }


            var itemBulkPlants = eServiceOnlineGateway.Instance.GetBulkPlants().OrderBy(s => s.Name).FirstOrDefault();

            if (itemCrews != null)
            {
                bulkPlantId = itemBulkPlants.Id;

                bulkPlantName = itemBulkPlants.Name;

            }

        }


        #region Data Preparation Methods

        public void CreateScheduleProductHaulToRigBin(double firstLoad, double secondLoad, double thirdLoad, double fourthLoad)
        {
            //Set up model
            double loadAmount = firstLoad + secondLoad + thirdLoad + fourthLoad;


            ScheduleProductHaulToRigBinViewModel model = new ScheduleProductHaulToRigBinViewModel();
            model.RigJobId = rigJob.Id;
            model.LoggedUser = loggedUser;
            model.ProductHaulInfoModel.CrewId = crewId;
            model.ProductHaulInfoModel.BulkPlantId = bulkPlantId;
            model.ProductHaulInfoModel.EstimatedLoadTime = DateTime.Now;
            model.ProductHaulInfoModel.EstimatedTravelTime = 4;
            model.ProductHaulInfoModel.IsExistingHaul = false;
            model.ProductHaulInfoModel.IsGoWithCrew = false;
            model.ProductHaulInfoModel.IsThirdParty = false;
            model.PodLoadModels = new List<PodLoad>();
            model.PodLoadModels.Add(new PodLoad() { LoadAmount = firstLoad, PodIndex = 0 });
            model.PodLoadModels.Add(new PodLoad() { LoadAmount = secondLoad, PodIndex = 1 });
            model.PodLoadModels.Add(new PodLoad() { LoadAmount = thirdLoad, PodIndex = 2 });
            model.PodLoadModels.Add(new PodLoad() { LoadAmount = fourthLoad, PodIndex = 3 });

            var bins = eServiceOnlineGateway.Instance.GetBinInformationsByRigId(rigJob.Rig.Id);

            double binAmount = 0;

            if (loadAmount > 0)
            {
                binAmount=1;
            }

            model.BlendUnloadSheetModels = new List<BlendUnloadSheet>();
            foreach (var binInformation in bins)
            {
                model.BlendUnloadSheetModels.Add(new BlendUnloadSheet()
                { DestinationStorage = binInformation, UnloadAmount = binAmount==0?0: binAmount });

                if (binAmount > 0)
                {
                    binAmount--;
                }
            }

            model.ProductLoadInfoModel.Amount = loadAmount;
            model.ProductLoadInfoModel.BaseBlendSectionId = callSheetBlendSectionId;
            model.ProductLoadInfoModel.BulkPlantId = bulkPlantId;
            model.ProductLoadInfoModel.BulkPlantName = bulkPlantName;
            model.ProductLoadInfoModel.CallSheetId = callSheetId;
            model.ProductLoadInfoModel.CallSheetNumber = callSheetNumber;
            model.ProductLoadInfoModel.EstimatedLoadTime = DateTime.Now.AddDays(1);
            model.ProductHaulInfoModel.ExpectedOnLocationTime = DateTime.Now.AddDays(1);
            model.ProductLoadInfoModel.IsBlendTest = false;
            model.ProductLoadInfoModel.IsTotalBlendTonnage = false;
            model.ProductLoadInfoModel.MixWater = blendSection.MixWaterRequirement;
            model.ProductLoadInfoModel.RemainsAmount = loadAmount;
            model.ProductLoadInfoModel.RigId = rigJob.Rig.Id;
            model.ProductLoadInfoModel.RigName = rigJob.Rig.Name;

            model.ShippingLoadSheetModel.BulkPlantId = bulkPlantId;
            model.ShippingLoadSheetModel.BulkPlantName = bulkPlantName;
            model.ShippingLoadSheetModel.CallSheetId = callSheetId;
            model.ShippingLoadSheetModel.CallSheetNumber = callSheetNumber;
            model.ShippingLoadSheetModel.IsGoWithCrew = false;
            model.ShippingLoadSheetModel.RigId = rigJob.Rig.Id;
            model.ShippingLoadSheetModel.RigName = rigJob.Rig.Name;

            eServiceWebContext.Instance.ScheduleProductHaul(model);

            var productHaulLoad =
                eServiceOnlineGateway.Instance.GetProductHaulLoadByQuery(p => p.CallSheetId == callSheetId).First();

            if (productHaulLoad != null)
            {
                productHaulLoadId = productHaulLoad.Id;

                var shippingLoadSheet =
                    eServiceOnlineGateway.Instance.GetShippingLoadSheetByProductHaulLoads(new List<int>() { productHaulLoad.Id }).First();

                if (shippingLoadSheet != null)
                {
                    productHaulId = shippingLoadSheet.ProductHaul.Id;
                }
            }
        }

        public void CancelScheduleProductHaulToRigBin(ProductHaulLoad productHaulLoad, ProductHaul productHaul, bool isCheckShipingLoadSheet)
        {

            CancelProductViewModel model = new CancelProductViewModel();

            
            model.ProductHaulId = productHaulId;
            List<CheckShipingLoadSheetModel> CheckShipingLoadSheetModels = new List<CheckShipingLoadSheetModel>();
            foreach (ShippingLoadSheet shippingLoadSheet in productHaul.ShippingLoadSheets)
            {
                var productHualLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(shippingLoadSheet.ProductHaulLoad.Id);
                List<int> ids = new List<int>();
                ids.Add(productHualLoad.Id);
                var shippingLoadSheets = eServiceOnlineGateway.Instance.GetShippingLoadSheetByProductHaulLoads(ids);
                CheckShipingLoadSheetModels.Add(new CheckShipingLoadSheetModel()
                {
                    IsChecked = isCheckShipingLoadSheet,
                    IsReadOnly = shippingLoadSheets.Count > 1 ? true : false,
                    ShippingLoadSheetModel = shippingLoadSheet,
                    ProductHaulLoadModel = productHualLoad
                });
            }
            model.CheckShipingLoadSheetModels = CheckShipingLoadSheetModels;

            eServiceWebContext _context = new eServiceWebContext();

            _context.DeleteProductHaul(model);
        }

        public void CreateScheduleBlendRequest(double amount)
        {
            ProductLoadInfoModel model = new ProductLoadInfoModel();

            model.Amount = amount;

            model.BaseBlendSectionId = blendSection.Id;
            model.BinInformationId = 3058;
            model.BulkPlantId = bulkPlantId;

            model.BulkPlantName = bulkPlantName;
            model.CallSheetId = callSheetId;
            model.CallSheetNumber = callSheetNumber;
            model.Comments = "Schedule Blend Request";
            model.MixWater = blendSection.MixWaterRequirement;
            model.IsBlendTest = blendSection.IsNeedFieldTesting;
            model.EstimatedLoadTime = DateTime.Now.AddDays(1);
            model.ExpectedOnLocationTime = DateTime.Now.AddDays(1);
            model.RigId = rigJob.Rig.Id;

            model.LoggedUser = loggedUser;

            eServiceWebContext.Instance.CreateBlendRequest(model);


            var productHaulLoad =
                eServiceOnlineGateway.Instance.GetProductHaulLoadByQuery(p => p.CallSheetId == callSheetId).First();

            if (productHaulLoad != null)
            {
                productHaulLoadId = productHaulLoad.Id;
            }
        }

        public void CancelScheduleBlendRequest(int productHaulLoadId)
        {
            eServiceWebContext _context = new eServiceWebContext();

            _context.DeleteProductHaulLoadById(productHaulLoadId, loggedUser);

        }

        #endregion Data Preparation Methods

        [Test]
        public void TestRigJobSetUp()
        {
            Assert.AreNotEqual(callSheetNumber, 0);
            Assert.AreNotEqual(callSheetBlendSectionId, 0);
        }

        [Test]
        public void TestScheduleProductHaulToRigBin()
        {
            #region Test Data Preparation

            //Get proper rig job -- Pending/has bins attached to the rig/has a Lead blend for haul
            var properJobs =
                eServiceOnlineGateway.Instance.GetRigJobsByQuery(null, p => p.JobLifeStatus == JobLifeStatus.Pending || p.JobLifeStatus == JobLifeStatus.Confirmed).OrderByDescending(p => p.Id);
            List<BinInformation> attachedBins = new List<BinInformation>();

            foreach (var properJob in properJobs)
            {
                attachedBins  = eServiceOnlineGateway.Instance.GetBinInformationsByRigId(rigJob.Rig.Id);
                if (attachedBins.Count == 0) continue;
                var availableBlends = eServiceOnlineGateway.Instance.GetBlendSectionsByCallSheetAndBlend(properJob.CallSheetNumber);
                bool foundIt = false;
                if (availableBlends != null && availableBlends.Count > 0)
                {
                    foreach (var callSheetBlendSection in availableBlends)
                    {
                        if (callSheetBlendSection.BlendCategory.Name == "Lead 1")
                        {
                            callSheetBlendSectionId = callSheetBlendSection.Id;
                            callSheetNumber = properJob.CallSheetNumber;
                            callSheetId = properJob.CallSheetId;
                            blendSection = callSheetBlendSection;
                            foundIt = true;
                            break;
                        }
                    }
                }

                if (foundIt)
                {
                    rigJob = properJob;
                    break;
                }

            }

            //Prepare input data for this test case

            //Proper bulk Plant match rigjob

            var bulkPlants = eServiceOnlineGateway.Instance.GetBulkPlants();
            Collection<int> bulkPlantIds = new Collection<int>(bulkPlants.Select(p => p.Id).Distinct().Except(new []{0}).ToList());
            Pager pager = new Pager(){PageIndex = 1, PageSize = 18, PageTotal = 10, TotalCounts = 180, OrderBy = "RigStatus, ClientCompanyShortName"};
            var bulkPlantList =
                eServiceOnlineGateway.Instance.GetRigJobsByQuery(pager, p => bulkPlantIds.Contains(p.Rig.Id));

            var bulkPlant = bulkPlantList.FirstOrDefault(q => q.ServicePoint.Id == rigJob.ServicePoint.Id)?.Rig;
            
            Assert.IsNotNull(bulkPlant);

            //Proper bulker crew list

            var bulkerCrewList = eServiceOnlineGateway.Instance.GetCrewList().FindAll(p => p.Type.Id == 2 && p.HomeServicePoint.Id==rigJob.ServicePoint.Id).OrderBy(p=>p.Description);

            Assert.IsNotNull(bulkerCrewList);
            Assert.Greater(bulkerCrewList.Count(), 1);

            SanjelCrew selectedCrew = bulkerCrewList.First();
            crewId = selectedCrew.Id;

            double firstLoad=1, secondLoad=2, thirdLoad=3, fourthLoad = 0;

            double totalAmount = 6;

            DateTime estimateLoadTime = DateTime.Now.AddHours(8);
            DateTime expectedOnLocationTime = DateTime.Now.AddHours(12);
            double estimateTravelTime = 4;
            DateTime expectedScheduleStartTime = expectedOnLocationTime.AddHours((-1) * estimateTravelTime);
            DateTime expectedScheduleEndTime = expectedOnLocationTime.AddHours(estimateTravelTime);
            #endregion Test Data Preparation

            #region Set up model data

            ScheduleProductHaulToRigBinViewModel model = new ScheduleProductHaulToRigBinViewModel();
            //hidden values with form inititalization
            model.RigJobId = rigJob.Id;
            model.LoggedUser = loggedUser;
            model.ProductLoadInfoModel.CallSheetId = callSheetId;

            //Form display and data entries
            model.ProductLoadInfoModel.CallSheetNumber = callSheetNumber;
            model.ProductLoadInfoModel.BaseBlendSectionId = callSheetBlendSectionId;
            model.ProductLoadInfoModel.IsTotalBlendTonnage = false;
            model.ProductLoadInfoModel.Amount = totalAmount;
            model.ProductLoadInfoModel.MixWater = blendSection.MixWaterRequirement;
            model.ProductLoadInfoModel.IsBlendTest = false;
            model.ProductLoadInfoModel.BulkPlantId = bulkPlant.Id;
            //            model.ProductLoadInfoModel.BulkPlantName = bulkPlant.Name;
            model.ShippingLoadSheetModel.IsGoWithCrew = false;
            model.ProductLoadInfoModel.RigId = rigJob.Rig.Id;
            //            model.ProductLoadInfoModel.RigName = rigJob.Rig.Name;

            //Load all blend to one bin
            model.BlendUnloadSheetModels = new List<BlendUnloadSheet>();
            for (int i = 0; i< attachedBins.Count; i++)
            {
                model.BlendUnloadSheetModels.Add(new BlendUnloadSheet()
                    { DestinationStorage = attachedBins[0], UnloadAmount = i==0?totalAmount:0 });
                
            }

            model.ProductHaulInfoModel.IsExistingHaul = false;
            model.ProductHaulInfoModel.EstimatedLoadTime = estimateLoadTime;
            model.ProductHaulInfoModel.ExpectedOnLocationTime = expectedOnLocationTime;
            model.ProductHaulInfoModel.EstimatedTravelTime = 4;
            model.ProductHaulInfoModel.IsThirdParty = false;
            model.ProductHaulInfoModel.CrewId = crewId;
            model.PodLoadModels = new List<PodLoad>();
            model.PodLoadModels.Add(new PodLoad() { LoadAmount = firstLoad, PodIndex = 0 });
            model.PodLoadModels.Add(new PodLoad() { LoadAmount = secondLoad, PodIndex = 1 });
            model.PodLoadModels.Add(new PodLoad() { LoadAmount = thirdLoad, PodIndex = 2 });
            model.PodLoadModels.Add(new PodLoad() { LoadAmount = fourthLoad, PodIndex = 3 });

            #endregion Set up model data 
            
            #region Run target method

            eServiceWebContext.Instance.ScheduleProductHaul(model);

            #endregion  Run target method

            #region Get Actaul result
            //The last records in related entities should be the result insert by above method
            var actualProductHaul = eServiceOnlineGateway.Instance.GetProductHaulByQuery(p => p.ProductHaulLifeStatus == ProductHaulStatus.Scheduled).OrderByDescending(p => p.Id).First();
            Assert.IsNotNull(actualProductHaul);
            Assert.AreNotEqual(0, actualProductHaul.Id);
            productHaulId = actualProductHaul.Id;
            //Assert product haul name and description

            var actualProductHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadByQuery(p => p.ProductHaulLoadLifeStatus == ProductHaulLoadStatus.Scheduled).OrderByDescending(p => p.Id).First();;
            Assert.IsNotNull(actualProductHaulLoad);
            Assert.AreNotEqual(0, actualProductHaulLoad.Id);
            Assert.AreEqual(actualProductHaulLoad.BlendSectionId, callSheetBlendSectionId);
            Assert.AreEqual(actualProductHaulLoad.BaseBlendWeight/1000, totalAmount);
            Assert.AreEqual(actualProductHaulLoad.IsTotalBlendTonnage, false);
            Assert.AreEqual(actualProductHaulLoad.ClientRepresentative, Utility.GetClientRepresentative(rigJob));
            Assert.AreEqual(actualProductHaulLoad.ProductHaulLoadLifeStatus, ProductHaulLoadStatus.Scheduled);
            //Assert product haul Load name and description

            
            productHaulLoadId = actualProductHaulLoad.Id;


            var actualShippingLoadSheets =
                eServiceOnlineGateway.Instance.GetShippingLoadSheetByProductHaulId(productHaulId);
            Assert.IsNotNull(actualShippingLoadSheets);
            Assert.AreEqual(1, actualShippingLoadSheets.Count);

            var actualShippingLoadSheet = actualShippingLoadSheets.FirstOrDefault();
            actualShippingLoadSheet =
                eServiceOnlineGateway.Instance.GetShippingLoadSheetById(actualShippingLoadSheet.Id, true);

            Assert.NotNull(actualShippingLoadSheet);
            Assert.AreEqual(1, actualShippingLoadSheet.BlendUnloadSheets.Count);
            Assert.AreEqual(totalAmount, actualShippingLoadSheet.BlendUnloadSheets[0].UnloadAmount/1000);
            Assert.AreEqual(totalAmount, actualShippingLoadSheet.LoadAmount/1000);
            Assert.NotNull(actualShippingLoadSheet.ProductHaul);
            Assert.NotNull(actualShippingLoadSheet.ProductHaulLoad);
            Assert.AreEqual(productHaulLoadId, actualShippingLoadSheet.ProductHaulLoad.Id);
            //Assert shipping load sheet name and description


            //verify crew assignment
            RigJobSanjelCrewSection rigJobSanjelCrewSection = eServiceOnlineGateway.Instance.GetRigJobCrewSectionsByProductHaul(productHaulId);
            Assert.NotNull(rigJobSanjelCrewSection);
            Assert.AreNotEqual(0, rigJobSanjelCrewSection.ProductHaul.Id);
            
            SanjelCrewSchedule crewSchedule =
                eServiceOnlineGateway.Instance.GetCrewScheduleByJobCrewSection(rigJobSanjelCrewSection.Id);
            Assert.NotNull(crewSchedule);
            Assert.Greater(1, Math.Abs((expectedScheduleStartTime-crewSchedule.StartTime).TotalSeconds));
            Assert.Greater(1, Math.Abs((expectedScheduleEndTime-crewSchedule.EndTime).TotalSeconds));
            Assert.AreEqual(selectedCrew.Id, crewSchedule.SanjelCrew.Id);
            
            List<WorkerSchedule> workerSchedules =
                eServiceOnlineGateway.Instance.GetWorkerScheduleByCrewScheduleId(crewSchedule.Id);
            foreach (var workerSchedule in workerSchedules)
            {
                Assert.Greater(1, Math.Abs((expectedScheduleStartTime-workerSchedule.StartTime).TotalSeconds));
                Assert.Greater(1, Math.Abs((expectedScheduleEndTime-workerSchedule.EndTime).TotalSeconds));
            }

            List<UnitSchedule> unitSchedules =
                eServiceOnlineGateway.Instance.GetUnitScheduleByCrewScheduleId(crewSchedule.Id);
            foreach (var unitSchedule in unitSchedules)
            {
                Assert.Greater(1, Math.Abs((expectedScheduleStartTime-unitSchedule.StartTime).TotalSeconds));
                Assert.Greater(1, Math.Abs((expectedScheduleEndTime-unitSchedule.EndTime).TotalSeconds));
            }

            
            //Verify call sheet unit section
            var unitSections =
                eServiceOnlineGateway.Instance.GetUnitSectionsByCallSheetId(callSheetId);
            Assert.NotNull(unitSections);
            Assert.Less(0, unitSections.Count);

            var unitSectionsByProductHaul = unitSections.Where(p => p.ProductHaulId == productHaulId);
            Assert.NotNull(unitSectionsByProductHaul);
            Assert.AreEqual(1, unitSectionsByProductHaul.Count());

            var unitSection = unitSectionsByProductHaul.First();
            Assert.NotNull(unitSection);
            Assert.AreEqual(unitSection.CrewId, crewId);
            Assert.AreEqual(actualProductHaul.Description, unitSection.HaulDescription);

            #endregion Get Actaul result

            #region Test Data Clean Up


            #endregion Test Data Clean Up
        }

        [Test]
        public void TestRescheduleProductHaul()
        {

            double firstLoad = 1, secondLoad = 2, thirdLoad = 3, fourthLoad = 0;

            CreateScheduleProductHaulToRigBin(firstLoad, secondLoad, thirdLoad, fourthLoad);

            var productHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productHaulId);

            var productHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);

            ProductHaulInfoModel productHaulInfoModel = new ProductHaulInfoModel();


            productHaulInfoModel.IsGoWithCrew = productHaul.IsGoWithCrew;
            productHaulInfoModel.IsThirdParty = productHaul.IsThirdParty;
            productHaulInfoModel.CrewId = productHaul.Crew.Id;
            productHaulInfoModel.IsExistingHaul = true;
            productHaulInfoModel.BulkPlantId = productHaul.BulkPlant.Id;
            productHaulInfoModel.EstimatedTravelTime = productHaul.EstimatedTravelTime;
            productHaulInfoModel.EstimatedLoadTime = productHaul.EstimatedLoadTime;
            productHaulInfoModel.ExpectedOnLocationTime = productHaul.ExpectedOnLocationTime;
            productHaulInfoModel.ProductHaulId = productHaul.Id;


            List<PodLoadAndBendUnLoadModel> podLoadAndBendUnLoadModels = new List<PodLoadAndBendUnLoadModel>();
            var podLoads = eServiceOnlineGateway.Instance.GetPodLoadsByProductHaul(productHaulId);

            int nAmout = 3;

            //PodLoad LoadAmount
            foreach (var PodLoad in podLoads)
            {
                if (nAmout >= 0)
                {
                    PodLoad.LoadAmount = nAmout;
                    nAmout--;
                }

            }

            foreach (var shippingLoadSheetItem in productHaul.ShippingLoadSheets)
            {
                var shippingLoadSheet =
                    eServiceOnlineGateway.Instance.GetShippingLoadSheetById(shippingLoadSheetItem.Id, true);

                PodLoadAndBendUnLoadModel podLoadAndBendUnLoadModel = new PodLoadAndBendUnLoadModel();
                podLoadAndBendUnLoadModel.IsCheckShippingLoadSheet = true;
                var unloadSheet = shippingLoadSheet.BlendUnloadSheets.FirstOrDefault();
                podLoadAndBendUnLoadModel.RigId = unloadSheet == null
                    ? 0
                    : (unloadSheet.DestinationStorage == null ? 0 : unloadSheet.DestinationStorage.Rig?.Id ?? 0);
                podLoadAndBendUnLoadModel.RigName = shippingLoadSheet.Rig?.Name;
                podLoadAndBendUnLoadModel.LoadAmount = nAmout;
                podLoadAndBendUnLoadModel.ProgramId = productHaulLoad.ProgramId;
                podLoadAndBendUnLoadModel.CallSheetNumber = shippingLoadSheet.CallSheetNumber;
                podLoadAndBendUnLoadModel.ShippingLoadSheetId = shippingLoadSheet.Id;
                podLoadAndBendUnLoadModel.Blend = shippingLoadSheet.BlendDescription;
                podLoadAndBendUnLoadModel.IsGoWithCrew = shippingLoadSheet.IsGoWithCrew;

                //BlendUnloadSheets LoadAmount
                if (shippingLoadSheet.BlendUnloadSheets.Count >= 2)
                {
                    shippingLoadSheet.BlendUnloadSheets[0].UnloadAmount = 2;
                    shippingLoadSheet.BlendUnloadSheets[1].UnloadAmount = 1;
                }

                if (shippingLoadSheet.BlendUnloadSheets != null && shippingLoadSheet.BlendUnloadSheets.Count > 0)
                {
                    podLoadAndBendUnLoadModel.BlendUnloadSheetModels = shippingLoadSheet.BlendUnloadSheets;
                }
                else
                {
                    podLoadAndBendUnLoadModel.BlendUnloadSheetModels = new List<BlendUnloadSheet>();
                    if (productHaulLoad.Rig != null && productHaulLoad.Rig.Id != 0)
                    {
                        var binInformations =
                            eServiceOnlineGateway.Instance.GetBinInformationsByRigId(productHaulLoad.Rig.Id);
                        foreach (var item in binInformations)
                        {
                            podLoadAndBendUnLoadModel.BlendUnloadSheetModels.Add(new BlendUnloadSheet()
                            {
                                UnloadAmount = 0,
                                DestinationStorage = item
                            });
                        }
                    }
                }

                podLoadAndBendUnLoadModel.PodLoadModels = podLoads;

                podLoadAndBendUnLoadModels.Add(podLoadAndBendUnLoadModel);


            }

            productHaulInfoModel.CrewId = reCrewId;
            productHaulInfoModel.EstimatedTravelTime = 72;
            productHaulInfoModel.EstimatedLoadTime = DateTime.Now.AddDays(2);
            productHaulInfoModel.ExpectedOlTime = DateTime.Now.AddDays(2);

            RescheduleProductHaulViewModel model = new RescheduleProductHaulViewModel()
	            { ProductHaulInfoModel = productHaulInfoModel, PodLoadAndBendUnLoadModels = podLoadAndBendUnLoadModels };

//            eServiceWebContext.Instance.ReschedulePodLoadAndBlendUnLoad(podLoadAndBendUnLoadModels,
//                productHaulInfoModel.ProductHaulId, loggedUser);
            eServiceWebContext.Instance.RescheduleProductHaul1(model, rigJob.Id, loggedUser);


            //Verify 
            Assert.AreNotEqual(productHaulId, 0);

            Assert.NotNull(podLoadAndBendUnLoadModels);
            Assert.NotNull(productHaulInfoModel);


            ProductHaul product = eServiceWebContext.Instance.GetProductHaulById(productHaulId);

            Assert.NotNull(product);

            Assert.AreEqual(product.Crew.Id, productHaulInfoModel.CrewId);
        }

        [Test]
        public void TestCancelProductHaulWithCheckTrue()
        {

            double firstLoad = 1, secondLoad = 2, thirdLoad = 3, fourthLoad = 0;

            CreateScheduleProductHaulToRigBin(firstLoad, secondLoad, thirdLoad, fourthLoad);


            var productHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productHaulLoadId);

            var productHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);


            CancelScheduleProductHaulToRigBin(productHaulLoad, productHaul,true);

            ////Verify
            Assert.AreNotEqual(productHaulId, 0);
            Assert.AreNotEqual(productHaulLoadId, 0);
            Assert.NotNull(productHaulLoad);
            Assert.NotNull(productHaulId);

            var newProductHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productHaulLoadId);

            var newProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);

            Assert.Null(newProductHaulLoad);
            Assert.Null(newProductHaul);
        }

        [Test]
        public void TestCancelProductHaulWithCheckFalse()
        {

            double firstLoad = 1, secondLoad = 2, thirdLoad = 3, fourthLoad = 0;

            CreateScheduleProductHaulToRigBin(firstLoad, secondLoad, thirdLoad, fourthLoad);


            var productHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productHaulLoadId);

            var productHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);


            CancelScheduleProductHaulToRigBin(productHaulLoad, productHaul,false);

            ////Verify
            Assert.AreNotEqual(productHaulId, 0);
            Assert.AreNotEqual(productHaulLoadId, 0);
            Assert.NotNull(productHaulLoad);
            Assert.NotNull(productHaulId);

            var newProductHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productHaulLoadId);

            var newProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);


            Assert.NotNull(newProductHaulLoad);
            Assert.Null(newProductHaul);
        }

        [Test]
        public void TestOnLocationProductHaul()
        {
            double firstLoad = 1, secondLoad = 2, thirdLoad = 3, fourthLoad = 0;

            CreateScheduleProductHaulToRigBin(firstLoad, secondLoad, thirdLoad, fourthLoad);

            OnLocationProductHaulViewModel model = new OnLocationProductHaulViewModel()
            { ProductHaulId = Convert.ToInt32(productHaulId) };

            model.ShippingLoadSheetModels =
                eServiceOnlineGateway.Instance.GetShippingLoadSheetByProductHaulId(productHaulId);

            if (model.ShippingLoadSheetModels != null && model.ShippingLoadSheetModels.Count > 0)
            {
                bool sameLocation = true;
                string firstLocation = string.Empty;
                foreach (ShippingLoadSheet item in model.ShippingLoadSheetModels)
                {
                    if (item != null)
                    {
                        if (string.IsNullOrEmpty(firstLocation))
                        {
                            firstLocation = item.Destination;
                        }
                        else
                        {
                            if (!string.Equals(firstLocation, item.Destination))
                                sameLocation = false;
                        }
                    }
                }

                model.IsSameLocation = sameLocation;
                model.OnLocationTime = DateTime.Now;
            }

            model.LoggedUser = loggedUser;

            RigJobSanjelCrewSection rigJobSanjelCrewSection =
                eServiceOnlineGateway.Instance.GetRigJobCrewSectionsByProductHaul(productHaulId);

            int rigJobCrewSectionId = rigJobSanjelCrewSection.Id;


            SanjelCrewSchedule crewSchedule =
                eServiceOnlineGateway.Instance.GetCrewScheduleByJobCrewSection(rigJobCrewSectionId);



            eServiceWebContext.Instance.UpdateProductHaulOnLocation(model);



            ////Verify
            ProductHaul productHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);

            List<ShippingLoadSheet> shippingLoadSheets =
                eServiceOnlineGateway.Instance.GetShippingLoadSheetByProductHaulId(productHaul.Id);

            if (shippingLoadSheets.Count > 0)
            {
                foreach (var haulLoad in shippingLoadSheets)
                {
                    Assert.AreEqual(haulLoad.ShippingStatus, ShippingStatus.OnLocation);
                    Assert.AreEqual(haulLoad.ModifiedUserName, loggedUser);
                    //Assert.AreEqual(haulLoad.OnLocationTime, locationDateTime);
                    Assert.NotNull(haulLoad.OnLocationTime);

                }
            }

            RigJobSanjelCrewSection newRigJobSanjelCrewSection =
                eServiceOnlineGateway.Instance.GetRigJobCrewSectionsByProductHaul(productHaul.Id);

            SanjelCrewSchedule newCrewSchedule =
                eServiceOnlineGateway.Instance.GetCrewScheduleByJobCrewSection(rigJobCrewSectionId);

            Assert.NotNull(rigJobSanjelCrewSection);
            Assert.NotNull(crewSchedule);
            Assert.Null(newRigJobSanjelCrewSection);
            Assert.Null(newCrewSchedule);


            Assert.AreEqual(productHaul.ProductHaulLifeStatus, ShippingStatus.OnLocation);
            Assert.AreEqual(productHaul.ModifiedUserName, loggedUser);

        }

        [Test]
        public void TestUpdateTheBlend()
        {
            Sanjel.BusinessEntities.Sections.Common.BlendSection blendSection = eServiceWebContext.Instance.GetBlendSectionById(callSheetBlendSectionId);
            Sanjel.BusinessEntities.Sections.Common.BlendSection newBlendSection = blendSection;
            newBlendSection.Quantity =130;

            eServiceWebContext.Instance.UpdateBlendSection(newBlendSection, blendSection);

            Assert.NotNull(blendSection);

            var changeBlendSection = eServiceWebContext.Instance.GetBlendSectionByBlendSectionId(callSheetBlendSectionId);

            Assert.NotNull(blendSection);
            Assert.NotNull(changeBlendSection);
            Assert.AreNotEqual(changeBlendSection.Quantity, blendSection.Quantity);
        }


        [Test]
        public void TestScheduleBlendRequest()
        {

            double amount = 4;

            CreateScheduleBlendRequest(amount);

            Assert.AreNotEqual(productHaulLoadId, 0);

            var productHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productHaulLoadId);

            Assert.NotNull(productHaulLoad);
            Assert.AreEqual(productHaulLoad.BlendSectionId, callSheetBlendSectionId);
            Assert.AreEqual(productHaulLoad.BaseBlendWeight / 1000, amount);
            Assert.AreEqual(productHaulLoad.IsTotalBlendTonnage, false);
            Assert.AreEqual(productHaulLoad.ClientRepresentative, Utility.GetClientRepresentative(rigJob));
            Assert.AreEqual(productHaulLoad.ProductHaulLoadLifeStatus, ProductHaulLoadStatus.Scheduled);
        }

        [Test]
        public void TestCancelScheduleBlendRequest()
        {
            double amount = 4;

            CreateScheduleBlendRequest(amount);

            CancelScheduleBlendRequest(productHaulLoadId);

            Assert.AreNotEqual(productHaulLoadId, 0);

            var productHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productHaulLoadId);

            Assert.IsNull(productHaulLoad);
        }


        [Test]
        public void TestRescheduleBlendRequest()
        {
            double amount = 4;

            CreateScheduleBlendRequest(amount);

            ProductHaulLoad productHaulLoad = null;

            if (productHaulLoadId != 0)
            { 
                productHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productHaulLoadId);

                if (productHaulLoad != null)
                {
                    ProductLoadInfoModel model = new ProductLoadInfoModel();

                    model.LoggedUser = "awang";

                    model.ProductHaulLoadId = productHaulLoad.Id;

                    model.CallSheetNumber = productHaulLoad.CallSheetNumber;

                    model.BaseBlendSectionId = productHaulLoad.BlendSectionId;

                    BlendChemical blendChemical = productHaulLoad.BlendChemical;

                    model.BaseBlend = blendChemical == null ? "" : (((string.IsNullOrWhiteSpace(blendChemical.Description) ? blendChemical.Name : blendChemical.Description)).Split('+').FirstOrDefault());

                    model.Category = productHaulLoad.BlendCategory == null ? "" : (string.IsNullOrEmpty(productHaulLoad.BlendCategory.Description) ? productHaulLoad.BlendCategory.Name : productHaulLoad.BlendCategory.Description);
                    model.CategoryId = productHaulLoad.BlendCategory == null ? 0 : productHaulLoad.BlendCategory.Id;


                    model.BlendChemicalDescription = productHaulLoad.BlendChemical?.Description;

                    model.OnLocationTime = productHaulLoad.OnLocationTime;
                    model.ExpectedOnLocationTime = productHaulLoad.ExpectedOnLocationTime;
                    model.EstimatedLoadTime = productHaulLoad.EstmatedLoadTime;

                    model.ProgramNumber = productHaulLoad.ProgramId;

                    model.BaseBlendUnit = (productHaulLoad.Unit == null || string.IsNullOrEmpty(productHaulLoad.Unit.Description)) ? "t" : productHaulLoad.Unit.Description;

                    model.RigName = productHaulLoad.Rig?.Name;
                    model.RigId = productHaulLoad.Rig == null ? 0 : productHaulLoad.Rig.Id;
                    model.ClientName = productHaulLoad.Customer?.Name;
                    model.RemainsAmount = Math.Round(productHaulLoad.RemainsAmount / 1000, 2);
                    model.PodIndex = productHaulLoad.PodIndex;
                    model.ClientRepresentative = productHaulLoad.ClientRepresentative;


                    if (productHaulLoad.Bin != null)
                    {
                        model.BinId = productHaulLoad.Bin.Id;
                        model.BinNumber = productHaulLoad.Bin.Name;
                    }

                    if (productHaulLoad.BulkPlant != null && productHaulLoad.BulkPlant.Id > 0)
                    {
                        model.BulkPlantId = productHaulLoad.BulkPlant.Id;
                        model.BulkPlantName = productHaulLoad.BulkPlant.Name;
                    }
                    if (productHaulLoad.JobType != null && productHaulLoad.JobType.Id > 0)
                    {
                        model.JobTypeId = productHaulLoad.JobType.Id;
                        model.JobTypeName = productHaulLoad.JobType.Name;
                    }

                    if (productHaulLoad.ServicePoint != null && productHaulLoad.ServicePoint.Id > 0)
                    {
                        model.ServicePointId = productHaulLoad.ServicePoint.Id;
                        model.ServicePointName = productHaulLoad.ServicePoint.Name;
                    }

                    model.BinInformationId = 2971;
                    model.BulkPlantId = bulkPlantId;
                    model.BulkPlantName = bulkPlantName;
                    model.IsBlendTest = true;
                    model.IsTotalBlendTonnage = true;
                    model.Amount = 5;
                    model.MixWater = blendSection.MixWaterRequirement;

                    model.TotalBlendWeight = 0;
                    model.BaseBlendWeight = 0;
                    model.Comments = "Reschedule Blend Request";

                    model.EstimatedLoadTime = DateTime.Now.AddDays(5);

                    eServiceWebContext.Instance.RescheduleBlendRequest(model);
                }
            }  
            
            Assert.AreNotEqual(productHaulLoadId, 0);

            ProductHaulLoad newProductHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productHaulLoadId);

            Assert.NotNull(productHaulLoad);

            Assert.NotNull(newProductHaulLoad);

        }


        [Test]
        public void TestHaulBlendReuqest()
        {
            double amount = 5;

            CreateScheduleBlendRequest(amount);

            HaulBlendFromRigJobBlendViewModel model = new HaulBlendFromRigJobBlendViewModel();

            ProductHaulLoad productHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productHaulLoadId, true);

            BinInformation binInformation =
                eServiceOnlineGateway.Instance.GetBinInformationByBinIdAndPodIndex(productHaulLoad.Bin.Id, productHaulLoad.PodIndex);

            model.RigJobId = rigJob.Id;


            ProductLoadInfoModel productLoadInfoModel = new ProductLoadInfoModel();

            productLoadInfoModel.ProductHaulLoadId = productHaulLoad.Id;
            productLoadInfoModel.CallSheetNumber = productHaulLoad.CallSheetNumber;
            productLoadInfoModel.BaseBlendSectionId = productHaulLoad.BlendSectionId;
            BlendChemical blendChemical = productHaulLoad.BlendChemical;
            productLoadInfoModel.BaseBlend = blendChemical == null ? "" : (((string.IsNullOrWhiteSpace(blendChemical.Description) ? blendChemical.Name : blendChemical.Description)).Split('+').FirstOrDefault());
            productLoadInfoModel.Category = productHaulLoad.BlendCategory == null ? "" : (string.IsNullOrEmpty(productHaulLoad.BlendCategory.Description) ? productHaulLoad.BlendCategory.Name : productHaulLoad.BlendCategory.Description);
            productLoadInfoModel.CategoryId = productHaulLoad.BlendCategory == null ? 0 : productHaulLoad.BlendCategory.Id;
            productLoadInfoModel.MixWater = productHaulLoad.MixWater;
            productLoadInfoModel.Amount = productHaulLoad.TotalBlendWeight / 1000;
            productLoadInfoModel.TotalBlendWeight = productHaulLoad.TotalBlendWeight / 1000;
            productLoadInfoModel.BaseBlendWeight = productHaulLoad.BaseBlendWeight / 1000;
            productLoadInfoModel.BlendChemicalDescription = productHaulLoad.BlendChemical?.Description;
            productLoadInfoModel.IsTotalBlendTonnage = productHaulLoad.IsTotalBlendTonnage;
            productLoadInfoModel.OnLocationTime = productHaulLoad.OnLocationTime;


            productLoadInfoModel.ProgramNumber = productHaulLoad.ProgramId;

            productLoadInfoModel.BaseBlendUnit = (productHaulLoad.Unit == null || string.IsNullOrEmpty(productHaulLoad.Unit.Description)) ? "t" : productHaulLoad.Unit.Description;
            productLoadInfoModel.IsBlendTest = productHaulLoad.IsBlendTest;
            productLoadInfoModel.RigName = productHaulLoad.Rig?.Name;
            productLoadInfoModel.RigId = productHaulLoad.Rig == null ? 0 : productHaulLoad.Rig.Id;
            productLoadInfoModel.ClientName = productHaulLoad.Customer?.Name;
            productLoadInfoModel.RemainsAmount = Math.Round(productHaulLoad.RemainsAmount / 1000, 2);
            productLoadInfoModel.PodIndex = productHaulLoad.PodIndex;
            productLoadInfoModel.ClientRepresentative = productHaulLoad.ClientRepresentative;
            if (productHaulLoad.Bin != null)
            {
                productLoadInfoModel.BinId = productHaulLoad.Bin.Id;
                productLoadInfoModel.BinNumber = productHaulLoad.Bin.Name;
            }

            if (productHaulLoad.BulkPlant != null && productHaulLoad.BulkPlant.Id > 0)
            {
                productLoadInfoModel.BulkPlantId = productHaulLoad.BulkPlant.Id;
                productLoadInfoModel.BulkPlantName = productHaulLoad.BulkPlant.Name;
            }
            if (productHaulLoad.JobType != null && productHaulLoad.JobType.Id > 0)
            {
                productLoadInfoModel.JobTypeId = productHaulLoad.JobType.Id;
                productLoadInfoModel.JobTypeName = productHaulLoad.JobType.Name;
            }

            if (productHaulLoad.ServicePoint != null && productHaulLoad.ServicePoint.Id > 0)
            {
                productLoadInfoModel.ServicePointId = productHaulLoad.ServicePoint.Id;
                productLoadInfoModel.ServicePointName = productHaulLoad.ServicePoint.Name;
            }

            model.ProductLoadInfoModel.Amount = model.ProductLoadInfoModel.TotalBlendWeight;

            if (binInformation != null)
            {
                model.ProductLoadInfoModel.BinInformationName = binInformation.Name;
            }

            ProductHaulInfoModel productHaulInfoModel = new ProductHaulInfoModel();

            productHaulInfoModel.InitializeProductHaulSchedule();

            var binInformations = eServiceOnlineGateway.Instance.GetBinInformationsByRigId(rigJob.Id);

            ShippingLoadSheetModel shippingLoadSheet = new ShippingLoadSheetModel();

            shippingLoadSheet.InitializeShippingLoadSheets(productHaulLoad.Rig, binInformations);

            List <PodLoad> podLoads = new List<PodLoad>();
            podLoads.Add(new PodLoad() { LoadAmount = 2, PodIndex = 0 });
            podLoads.Add(new PodLoad() { LoadAmount = 2, PodIndex = 1 });
            podLoads.Add(new PodLoad() { LoadAmount = 0, PodIndex = 2 });
            podLoads.Add(new PodLoad() { LoadAmount = 0, PodIndex = 3 });

            productHaulInfoModel.PodLoadModels = podLoads;

            List<BlendUnloadSheet> blendUnloadSheets = new List<BlendUnloadSheet>();

            var bins = eServiceOnlineGateway.Instance.GetBinInformationsByRigId(rigJob.Rig.Id);

            double binAmount = 0;

            if (amount > 0)
            {
                binAmount = 2;
            }

            foreach (var binInfo in bins)
            {
                blendUnloadSheets.Add(new BlendUnloadSheet()
                { DestinationStorage = binInfo, UnloadAmount = binAmount == 0 ? 0 : binAmount });

                if (binAmount > 0)
                {
                    binAmount--;
                }
            }


            shippingLoadSheet.BlendUnloadSheetModels = blendUnloadSheets;

            double loadAmount = 4;

            productHaulInfoModel.CrewId = 3055;
            productHaulInfoModel.EstimatedLoadTime = DateTime.Now.AddDays(2);
            productHaulInfoModel.EstimatedTravelTime =48;
            productHaulInfoModel.ExpectedOnLocationTime = DateTime.Now.AddDays(2);
            shippingLoadSheet.LoadAmount = loadAmount;
            productLoadInfoModel.Amount = loadAmount;


            eServiceWebContext.Instance.CreateHaulBlend(rigJob.Id, loggedUser, productLoadInfoModel, productHaulInfoModel, shippingLoadSheet);


            Assert.AreNotEqual(productHaulId, 0);

            var asProductHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productHaulLoadId);

            Assert.NotNull(asProductHaulLoad);
            Assert.AreEqual(asProductHaulLoad.BlendSectionId, callSheetBlendSectionId);
            Assert.AreEqual(asProductHaulLoad.BaseBlendWeight / 1000, loadAmount);
            Assert.AreEqual(asProductHaulLoad.IsTotalBlendTonnage, false);
            Assert.AreEqual(asProductHaulLoad.ClientRepresentative, Utility.GetClientRepresentative(rigJob));
            Assert.AreEqual(asProductHaulLoad.ProductHaulLoadLifeStatus, ProductHaulLoadStatus.Scheduled);

            //Verify shipping load sheet
            var shippingLoadSheets =
                eServiceOnlineGateway.Instance.GetShippingLoadSheetByProductHaulLoads(new List<int>() { productHaulLoad.Id });
            Assert.Greater(shippingLoadSheets.Count, 0);
            var asShippingLoadSheet = shippingLoadSheets.OrderByDescending(p => p.Id).First();
            Assert.NotNull(asShippingLoadSheet);
            asShippingLoadSheet = eServiceOnlineGateway.Instance.GetShippingLoadSheetById(asShippingLoadSheet.Id, true);
            Assert.NotNull(asShippingLoadSheet);
            Assert.Greater(asShippingLoadSheet.BlendUnloadSheets.Count, 0);
            Assert.AreEqual(asShippingLoadSheet.LoadAmount / 1000, loadAmount);
            Assert.NotNull(asShippingLoadSheet.ProductHaul);
            Assert.AreNotEqual(asShippingLoadSheet.ProductHaul.Id, 0);

            //verify product haul sheet
            var productHaul = eServiceOnlineGateway.Instance.GetProductHaulById(asShippingLoadSheet.ProductHaul.Id);
            Assert.NotNull(productHaul);
            Assert.Greater(productHaul.PodLoad.Count, 0);

            //verify crew assignment
            RigJobSanjelCrewSection rigJobSanjelCrewSection = eServiceOnlineGateway.Instance.GetRigJobCrewSectionsByProductHaul(asShippingLoadSheet.ProductHaul.Id);
            Assert.NotNull(rigJobSanjelCrewSection);
            Assert.AreNotEqual(rigJobSanjelCrewSection.ProductHaul.Id, 0);

            SanjelCrewSchedule crewSchedule =
                eServiceOnlineGateway.Instance.GetCrewScheduleByJobCrewSection(rigJobSanjelCrewSection.Id);
            Assert.NotNull(crewSchedule);

            //Verify call sheet unit section
            var unitSections =
                eServiceOnlineGateway.Instance.GetUnitSectionsByCallSheetId(shippingLoadSheet.CallSheetId);
            Assert.NotNull(unitSections);
            Assert.Greater(unitSections.Count, 0);

            var unitSectionsByProductHaul = unitSections.Where(p => p.ProductHaulId == productHaul.Id);
            Assert.NotNull(unitSectionsByProductHaul);
            Assert.AreEqual(unitSectionsByProductHaul.Count(), 1);

            var unitSection = unitSectionsByProductHaul.First();
            Assert.NotNull(unitSection);
            Assert.AreEqual(unitSection.CrewId, crewId);
            Assert.AreEqual(productHaul.Description, unitSection.HaulDescription);
        }
    }
}