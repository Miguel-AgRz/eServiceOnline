using eServiceOnline.BusinessProcess;
using eServiceOnline.Gateway;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Crew;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ResourceSchedule;
using Sesi.SanjelData.Entities.BusinessEntities.Sales;
using Sesi.SanjelData.Entities.Common.Model.Scheduling;
using System;
using System.Collections.Generic;

namespace eServiceOnline.IntegrationTest
{
    public class ScheduleWithDRBForBinTest : ScheduleWithDRBForBinTestBase
    {
        private string loggedUser = "awang";

        private ProductHaulLoad productHaulLoad;
        private ShippingLoadSheet shippingLoadSheet;
        private ProductHaul productHaul;
        private ProductHaul secondProductHaul;
        private ProductHaul thirdProductHaul;
        private RigJobSanjelCrewSection rigJobSanjelCrewSection;
        private RigJobSanjelCrewSection secondRigJobSanjelCrewSection;
        private RigJobSanjelCrewSection thirdRigJobSanjelCrewSection;
        private RigJobThirdPartyBulkerCrewSection rigJobThirdPartyBulkerCrewSection;
        private Schedule schedule;
        private Schedule secondSchedule;
        private Schedule thirdSchedule;
        private static int globalSanjelScheduleId = 0;
        private static int globalSecondSanjelScheduleId = 0;
        private static int globalThirdSanjelScheduleId = 0;
        private static int globalThirdPartyScheduleId = 0;
        private static int globalThirdPartyScheduleId2 = 0;

        # region Data preparation methods
        private ProductHaulLoad PrepareProductHaulLoad()
        {
            ProductHaulLoad productHaulLoad = new ProductHaulLoad()
            {
                //BlendSectionId = rigJob.ble //12214,
                IsTotalBlendTonnage = false,
                BaseBlendWeight = 103 * 1000,
                MixWater = 2.468,
                OnLocationTime = DateTime.Now.AddHours(4),
                EstmatedLoadTime = DateTime.Now.AddHours(4),
                ExpectedOnLocationTime = DateTime.Now.AddHours(8),
                IsBlendTest = false,
                Bin = new Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment.Bin() { Id = 140, Name = "2023" },
                JobType = new Sesi.SanjelData.Entities.Common.BusinessEntities.Operation.JobType() { Id = rigJob.JobType.Id },
                ProductHaulLoadLifeStatus = ProductHaulLoadStatus.Scheduled,
                Rig = rig,
                BulkPlant =rig,

            };
            RigBoardProcess.CreateProductHaulLoad(productHaulLoad);
            return productHaulLoad;
        }
        private ShippingLoadSheet PrepareShippingLoadSheet()
        {
            var shippingLoadSheet = new ShippingLoadSheet()
            {
                ModifiedUserName = "awang",
                ProductHaulLoad = productHaulLoad,
                ProductHaul = productHaul,
                ShippingStatus = ShippingStatus.Scheduled,
                IsGoWithCrew = false,
                SourceStorage = binInformation,
                BlendUnloadSheets = new List<BlendUnloadSheet> {
                 new BlendUnloadSheet(){ DestinationStorage = binInformation}
                }
            };
            shippingLoadSheet.OwnerId = productHaul.Id;
            eServiceOnlineGateway.Instance.CreateShippingLoadSheet(shippingLoadSheet);
            return shippingLoadSheet;
        }

        private ProductHaul PrepareScheduledProductHaul()
        {
            ProductHaul productHaul = new ProductHaul()
            {
                Crew = bulkerCrew1,
                ProductHaulLifeStatus = ProductHaulStatus.Scheduled,
                EstimatedLoadTime = DateTime.Now.AddHours(4),
                ExpectedOnLocationTime = DateTime.Now.AddHours(8),
                EstimatedTravelTime = 2,
                IsGoWithCrew = false,
                IsThirdParty = false,
            };
            eServiceOnlineGateway.Instance.CreateProductHaul(productHaul, true);
            return productHaul;
        }
        private ProductHaul PrepareSecondScheduledProductHaul()
        {
            ProductHaul productHaul = new ProductHaul()
            {
                Crew = bulkerCrew1,
                ProductHaulLifeStatus = ProductHaulStatus.Scheduled,
                EstimatedLoadTime = DateTime.Now.AddHours(10),
                ExpectedOnLocationTime = DateTime.Now.AddHours(12),
                EstimatedTravelTime = 4,
                IsGoWithCrew = false,
                IsThirdParty = false,
            };
            eServiceOnlineGateway.Instance.CreateProductHaul(productHaul, true);
            return productHaul;
        }
        private ProductHaul PrepareThirdScheduledProductHaul()
        {
            ProductHaul productHaul = new ProductHaul()
            {
                Crew = bulkerCrew1,
                ProductHaulLifeStatus = ProductHaulStatus.Scheduled,
                EstimatedLoadTime = DateTime.Now.AddHours(11),
                ExpectedOnLocationTime = DateTime.Now.AddHours(13),
                EstimatedTravelTime = 4,
                IsGoWithCrew = false,
                IsThirdParty = false,
            };
            eServiceOnlineGateway.Instance.CreateProductHaul(productHaul, true);
            return productHaul;
        }
        private ProductHaul PrepareScheduledProductHaulFormThirdParty()
        {
            ProductHaul productHaul = new ProductHaul()
            {
                Crew =  thridPartyBulkerCrew1,
                ProductHaulLifeStatus = ProductHaulStatus.Scheduled,
                EstimatedLoadTime = DateTime.Now.AddHours(4),
                ExpectedOnLocationTime = DateTime.Now.AddHours(8),
                EstimatedTravelTime = 2,
                IsGoWithCrew = false,
                IsThirdParty = true,
            };
            eServiceOnlineGateway.Instance.CreateProductHaul(productHaul, true);
            return productHaul;
        }
        private ShippingLoadSheet PrepareShippingLoadSheetGoWithCrew()
        {
            var shippingLoadSheet = new ShippingLoadSheet()
            {
                ModifiedUserName = "awang",
                ProductHaulLoad = productHaulLoad,
                ProductHaul = productHaul,
                ShippingStatus = ShippingStatus.Scheduled,
                IsGoWithCrew = true,
                SourceStorage = binInformation,
                BlendUnloadSheets = new List<BlendUnloadSheet> {
                 new BlendUnloadSheet(){ DestinationStorage = binInformation}
                }
            };
            shippingLoadSheet.OwnerId = productHaul.Id;
            eServiceOnlineGateway.Instance.CreateShippingLoadSheet(shippingLoadSheet);
            return shippingLoadSheet;
        }

        private ProductHaul PrepareScheduledProductHaulGoWithCrew()
        {
            ProductHaul productHaul = new ProductHaul()
            {
                Crew = bulkerCrew1,
                ProductHaulLifeStatus = ProductHaulStatus.Scheduled,
                EstimatedLoadTime = DateTime.Now,
                ExpectedOnLocationTime = DateTime.Now,
                EstimatedTravelTime = 4,
                IsGoWithCrew = true,
                IsThirdParty = false,
            };
            eServiceOnlineGateway.Instance.CreateProductHaul(productHaul, true);
            return productHaul;
        }
        private ProductHaul PrepareScheduledProductHaulFormThirdPartyGoWithCrew()
        {
            ProductHaul productHaul = new ProductHaul()
            {
                Crew = thridPartyBulkerCrew1,
                ProductHaulLifeStatus = ProductHaulStatus.Scheduled,
                EstimatedLoadTime = DateTime.Now,
                ExpectedOnLocationTime = DateTime.Now,
                EstimatedTravelTime = 4,
                IsGoWithCrew = true,
                IsThirdParty = true,
            };
            eServiceOnlineGateway.Instance.CreateProductHaul(productHaul, true);
            return productHaul;
        }
        private RigJobSanjelCrewSection PrepareRigJobSanjelCrewSection()
        {
            var rigJobSanjelCrewSection = ProductHaulProcess.CreateRigJobSanjelCrewSection(bulkerCrew1, productHaul, rigJob, RigJobCrewSectionStatus.Scheduled, "IntegrationTest");
            return rigJobSanjelCrewSection;
        }
        private RigJobSanjelCrewSection PrepareSecondRigJobSanjelCrewSection()
        {
            var rigJobSanjelCrewSection = ProductHaulProcess.CreateRigJobSanjelCrewSection(bulkerCrew1, secondProductHaul, rigJob, RigJobCrewSectionStatus.Scheduled, "IntegrationTest");
            return rigJobSanjelCrewSection;
        }
        private RigJobSanjelCrewSection PrepareThirdRigJobSanjelCrewSection()
        {
            var rigJobSanjelCrewSection = ProductHaulProcess.CreateRigJobSanjelCrewSection(bulkerCrew1, thirdProductHaul, rigJob, RigJobCrewSectionStatus.Scheduled, "IntegrationTest");
            return rigJobSanjelCrewSection;
        }
        private RigJobThirdPartyBulkerCrewSection PrepareRigJobThirdPartyBulkerCrewSection()
        {
            var rigJobThirdPartyBulkerCrewSection = RigBoardProcess.CreateRigJobThirdPartyBulkerCrewSection(thridPartyBulkerCrew1, productHaul, rigJob);
            return rigJobThirdPartyBulkerCrewSection;
        }

        private Schedule PrepareSchedule()
        {

            var startTime = productHaul.ExpectedOnLocationTime.AddHours((-1) * productHaul.EstimatedTravelTime);
            var endTime = productHaul.ExpectedOnLocationTime.AddHours(productHaul.EstimatedTravelTime);
            Schedule schedule = RigBoardProcess.CreateSchedules(startTime, endTime, bulkerCrew1, rigJob, productHaul, rigJobSanjelCrewSection, "IntegrationTest");
            return schedule;
        }
        private Schedule PrepareSecondSchedule()
        {

            var startTime = secondProductHaul.ExpectedOnLocationTime.AddHours((-1) * secondProductHaul.EstimatedTravelTime);
            var endTime = secondProductHaul.ExpectedOnLocationTime.AddHours(secondProductHaul.EstimatedTravelTime);
            Schedule schedule = RigBoardProcess.CreateSchedules(startTime, endTime, bulkerCrew1, rigJob, secondProductHaul, secondRigJobSanjelCrewSection, "IntegrationTest");
            return schedule;
        }
        private Schedule PrepareThirdSchedule()
        {

            var startTime = thirdProductHaul.ExpectedOnLocationTime.AddHours((-1) * thirdProductHaul.EstimatedTravelTime);
            var endTime = thirdProductHaul.ExpectedOnLocationTime.AddHours(thirdProductHaul.EstimatedTravelTime);
            Schedule schedule = RigBoardProcess.CreateSchedules(startTime, endTime, bulkerCrew1, rigJob, thirdProductHaul, thirdRigJobSanjelCrewSection, "IntegrationTest");
            return schedule;
        }
        private Schedule PrepareThirdPartySchedule()
        {

            var startTime = productHaul.ExpectedOnLocationTime.AddHours((-1) * productHaul.EstimatedTravelTime);
            var endTime = productHaul.ExpectedOnLocationTime.AddHours(productHaul.EstimatedTravelTime);
            Schedule schedule = RigBoardProcess.CreateThirdPartyBulkerCrewSchedule(startTime, endTime, thridPartyBulkerCrew1, rigJob, productHaul, rigJobThirdPartyBulkerCrewSection);
            return schedule;
        }
        private Schedule PrepareScheduleGoWithCrew()
        {

            var startTime = rigJob.CallCrewTime;
            var endTime = rigJob.JobDateTime.AddMinutes(360);
            Schedule schedule = RigBoardProcess.CreateSchedules(startTime, endTime, bulkerCrew1, rigJob, productHaul, rigJobSanjelCrewSection, "IntegrationTest");
            return schedule;
        }
        private Schedule PrepareThirdPartyScheduleGoWithCrew()
        {

            var startTime = rigJob.CallCrewTime;
            var endTime = rigJob.JobDateTime.AddMinutes(360);
            Schedule schedule = RigBoardProcess.CreateThirdPartyBulkerCrewSchedule(startTime, endTime, thridPartyBulkerCrew1, rigJob, productHaul, rigJobThirdPartyBulkerCrewSection);
            return schedule;
        }
        private void PrepareBulkerCrew()
        {
            bulkerCrew1 = new SanjelCrew() { Type = new CrewType() { Id = 2, Name = "Bulker Crew" } };
            bulkerCrew1.SanjelCrewWorkerSection.Add(new SanjelCrewWorkerSection() { Worker = employee1 });
            bulkerCrew1.SanjelCrewTruckUnitSection.Add(new SanjelCrewTruckUnitSection() { TruckUnit = bulkerUnit1 });
            bulkerCrew1.SanjelCrewTruckUnitSection.Add(new SanjelCrewTruckUnitSection() { TruckUnit = tractorUnit1 });
            bulkerCrew1.Name = CrewBoardProcess.BuildCrewName(bulkerCrew1);
            bulkerCrew1.Description = CrewBoardProcess.BuildCrewDescription(bulkerCrew1);
            sanjelCrewService.Insert(bulkerCrew1, true);
        }
        private void PrepareBulkerCrew2()
        {
            bulkerCrew2 = new SanjelCrew() { Type = new CrewType() { Id = 2, Name = "Bulker Crew2" } };
            bulkerCrew2.SanjelCrewWorkerSection.Add(new SanjelCrewWorkerSection() { Worker = employee1 });
            bulkerCrew2.SanjelCrewTruckUnitSection.Add(new SanjelCrewTruckUnitSection() { TruckUnit = bulkerUnit1 });
            bulkerCrew2.SanjelCrewTruckUnitSection.Add(new SanjelCrewTruckUnitSection() { TruckUnit = tractorUnit1 });
            bulkerCrew2.Name = CrewBoardProcess.BuildCrewName(bulkerCrew2);
            bulkerCrew2.Description = CrewBoardProcess.BuildCrewDescription(bulkerCrew2);
            sanjelCrewService.Insert(bulkerCrew2, true);
        }
        private void PrepareBulkerCrewLog()
        {
            bulkerCrewLog = new BulkerCrewLog();
            bulkerCrewLog.SanjelCrew = bulkerCrew1;
            bulkerCrewLog.CrewStatus = BulkerCrewStatus.None;
            eServiceOnlineGateway.Instance.CreateBulkerCrewLog(bulkerCrewLog);
        }
        private void PrepareBulkerCrew2Log()
        {
            bulkerCrew2Log = new BulkerCrewLog();
            bulkerCrew2Log.SanjelCrew = bulkerCrew2;
            bulkerCrew2Log.CrewStatus = BulkerCrewStatus.None;
            eServiceOnlineGateway.Instance.CreateBulkerCrewLog(bulkerCrew2Log);
        }

        private void PrepareThirdPartyCrewLog()
        {
            thirdPartyCrewLog = new BulkerCrewLog();
            thirdPartyCrewLog.ThirdPartyBulkerCrew = thridPartyBulkerCrew1;
            thirdPartyCrewLog.CrewStatus = BulkerCrewStatus.None;
            eServiceOnlineGateway.Instance.CreateBulkerCrewLog(thirdPartyCrewLog);
        }
        private void PrepareThirdPartyCrew2Log()
        {
            thirdPartyCrew2Log = new BulkerCrewLog();
            thirdPartyCrew2Log.ThirdPartyBulkerCrew = thridPartyBulkerCrew2;
            thirdPartyCrew2Log.CrewStatus = BulkerCrewStatus.None;
            eServiceOnlineGateway.Instance.CreateBulkerCrewLog(thirdPartyCrew2Log);
        }
        private void PrepareThirdPartyBulkerCrew()
        {
            thridPartyBulkerCrew1 = new ThirdPartyBulkerCrew() { Type = new CrewType() { Id = 3, Name = "Third Party Bulker Crew" } };
            thridPartyBulkerCrew1.Name = "Brett Third Party";
            thridPartyBulkerCrew1.Description = "TEST|Evolve Logistics Inc | Brett";
            thirdPartyBulkerCrewService.Insert(thridPartyBulkerCrew1, true);
        }
        private void PrepareThirdPartyBulkerCrew2()
        {
            thridPartyBulkerCrew2 = new ThirdPartyBulkerCrew() { Type = new CrewType() { Id = 3, Name = "Third Party Bulker Crew2" } };
            thridPartyBulkerCrew2.Name = "Brett Third Party";
            thridPartyBulkerCrew2.Description = "TEST|Evolve Logistics Inc | Brett";
            thirdPartyBulkerCrewService.Insert(thridPartyBulkerCrew2, true);
        }
        private RigJobSanjelCrewSection TestAssignCrewOtherStatus(RigJobSanjelCrewSection rigJobSanjelCrewSection, ProductHaulStatus productHaulStatus, RigJobCrewSectionStatus rigJobCrewSectionStatus)
        {
            rigJobSanjelCrewSection.ModifiedUserName = "TestAssignCrewOtherStatus";
            rigJobSanjelCrewSection.ProductHaul.ProductHaulLifeStatus = productHaulStatus;
            rigJobSanjelCrewSection.ProductHaul.ModifiedUserName = "TestAssignCrewOtherStatus";
            rigJobSanjelCrewSection.RigJobCrewSectionStatus = rigJobCrewSectionStatus;
            eServiceOnlineGateway.Instance.UpdateRigJobCrewSection(rigJobSanjelCrewSection);
            eServiceOnlineGateway.Instance.UpdateProductHaul(rigJobSanjelCrewSection.ProductHaul, false);
            return rigJobSanjelCrewSection;
        }
        #endregion

        [SetUp]
        public void Setup()
        {
            PrepareBulkerCrew();
            PrepareBulkerCrewLog();
            PrepareThirdPartyBulkerCrew();
            PrepareThirdPartyCrewLog();
        }
        [TearDown]
        public void TearDown()
        {
            eServiceOnlineGateway.Instance.DeleteCrew(bulkerCrew1.Id);
            eServiceOnlineGateway.Instance.DeleteBulkerCrewLog(bulkerCrewLog);
            eServiceOnlineGateway.Instance.DeleteThirdPartyBulkerCrew(thridPartyBulkerCrew1.Id);
            eServiceOnlineGateway.Instance.DeleteBulkerCrewLog(thirdPartyCrewLog);

            // delete bussiness prepare data
            if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
            {
                if (productHaulLoad != null && productHaulLoad.Id > 0)
                {
                    eServiceOnlineGateway.Instance.DeleteProductHaulLoad(productHaulLoad);
                }
                if (shippingLoadSheet != null && shippingLoadSheet.Id > 0)
                {
                    eServiceOnlineGateway.Instance.DeleteShippingLoadSheet(shippingLoadSheet);
                }
                if (productHaul != null && productHaul.Id > 0)
                {
                    eServiceOnlineGateway.Instance.DeleteProductHaul(productHaul);
                }
                if (secondProductHaul != null && secondProductHaul.Id > 0)
                {
                    eServiceOnlineGateway.Instance.DeleteProductHaul(secondProductHaul);
                }
                if (thirdProductHaul != null && thirdProductHaul.Id > 0)
                {
                    eServiceOnlineGateway.Instance.DeleteProductHaul(thirdProductHaul);
                }
                if (rigJobSanjelCrewSection != null && rigJobSanjelCrewSection.Id > 0)
                {
                    eServiceOnlineGateway.Instance.DeleteRigJobCrewSection(rigJobSanjelCrewSection);
                }
                if (secondRigJobSanjelCrewSection != null && secondRigJobSanjelCrewSection.Id > 0)
                {
                    eServiceOnlineGateway.Instance.DeleteRigJobCrewSection(secondRigJobSanjelCrewSection);
                }
                if (thirdRigJobSanjelCrewSection != null && thirdRigJobSanjelCrewSection.Id > 0)
                {
                    eServiceOnlineGateway.Instance.DeleteRigJobCrewSection(thirdRigJobSanjelCrewSection);
                }
                if (rigJobThirdPartyBulkerCrewSection != null && rigJobThirdPartyBulkerCrewSection.Id > 0)
                {
                    eServiceOnlineGateway.Instance.DeleteRigJobThirdPartyBulkerCrewSection(rigJobThirdPartyBulkerCrewSection.Id);
                }
                if (globalSanjelScheduleId > 0)
                {

                    eServiceOnlineGateway.Instance.DeleteCrewSchedule(globalSanjelScheduleId);
                }
                if (globalSecondSanjelScheduleId > 0)
                {

                    eServiceOnlineGateway.Instance.DeleteCrewSchedule(globalSecondSanjelScheduleId);
                }
                if (globalThirdSanjelScheduleId > 0)
                {

                    eServiceOnlineGateway.Instance.DeleteCrewSchedule(globalThirdSanjelScheduleId);
                }
                if (globalThirdPartyScheduleId > 0)
                {

                    eServiceOnlineGateway.Instance.DeletethirdPartyBulkerCrewSchedule(globalThirdPartyScheduleId);
                }
                if (globalThirdPartyScheduleId2 > 0)
                {

                    eServiceOnlineGateway.Instance.DeletethirdPartyBulkerCrewSchedule(globalThirdPartyScheduleId2);
                }
            }
        }

        #region ScheduleProductHaul
        [Test]
        public void TestScheduleProductHaulbyRigJobBin()
        {
            int crewId = bulkerCrew1.Id;

            //1. Prepare test data
            productHaulLoad = PrepareProductHaulLoad();
            productHaul = PrepareScheduledProductHaul();
            shippingLoadSheet = PrepareShippingLoadSheet();
            rigJobSanjelCrewSection = PrepareRigJobSanjelCrewSection();
            schedule = PrepareSchedule();
            bulkerCrewLog.CrewStatus = BulkerCrewStatus.OffDuty;
            eServiceOnlineGateway.Instance.UpdateBulkerCrewLog(bulkerCrewLog);

            //2.Check if test data meet preconditions
            Assert.AreNotEqual(crewId, 0);
            // The bulker crew is currently is in OffDuty
            Assert.NotNull(productHaulLoad);
            BulkerCrewLog currentbulkerCrewLog = eServiceOnlineGateway.Instance.GetBulkerCrewLog(crewId);
            Assert.AreEqual(BulkerCrewStatus.OffDuty, currentbulkerCrewLog.CrewStatus);
            // bin request

            //3.Execute the method to test
            CrewProcess.UpdateBulkerCrewStatus(bulkerCrew1.Id, false, loggedUser);

            //4. Verify the result meets post-conditions
            var resultProductHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productHaulLoad.Id);
            //A Blend Request (ProductHaulLoad) is scheduled
            Assert.Greater(resultProductHaulLoad.Id, 0);
            Assert.AreEqual(resultProductHaulLoad.ProductHaulLoadLifeStatus, ProductHaulLoadStatus.Scheduled);
            //A Product Haul is scheduled with Pod allocations specified
            var resultProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaul.Id);
            Assert.Greater(resultProductHaul.Id, 0);
            Assert.AreEqual(resultProductHaul.ProductHaulLifeStatus, ProductHaulStatus.Scheduled);
            //A Shipping Load Sheet is scheduled with Blend Unload Sheet(s) specified
            var resultShippingLoadSheet = eServiceOnlineGateway.Instance.GetShippingLoadSheetById(shippingLoadSheet.Id);
            Assert.Greater(resultShippingLoadSheet.Id, 0);
            Assert.AreEqual(resultShippingLoadSheet.ShippingStatus, ShippingStatus.Scheduled);
            //A crew assignment is scheduled (RigJobSanjelCrewSection)
            var resultRigJobSanjelCrewSection = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(rigJobSanjelCrewSection.Id);
            Assert.Greater(resultRigJobSanjelCrewSection.Id, 0);
            Assert.AreEqual(resultRigJobSanjelCrewSection.RigJobCrewSectionStatus, RigJobCrewSectionStatus.Scheduled);
            //Bulker crew schedule is created for the round trip. Start Time = Expected On Location Time - Estimated Travel Time, End Time = Expected On Location Time + Estimated Travel Time.
            var resultSanjelCrewSchedule = eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(schedule.Id);
            Assert.Greater(resultSanjelCrewSchedule.Id, 0);
            globalSanjelScheduleId = resultSanjelCrewSchedule.Id;
            Assert.AreEqual(resultSanjelCrewSchedule.StartTime, resultProductHaul.ExpectedOnLocationTime.AddHours((-1) * resultProductHaul.EstimatedTravelTime));
            Assert.AreEqual(resultSanjelCrewSchedule.EndTime, resultProductHaul.ExpectedOnLocationTime.AddHours(resultProductHaul.EstimatedTravelTime));
            // The bulker crew status is in LoadRequested status
            BulkerCrewLog resultBulkerCrewLog = eServiceOnlineGateway.Instance.GetBulkerCrewLog(crewId);
            Assert.AreEqual(BulkerCrewStatus.LoadRequested, resultBulkerCrewLog.CrewStatus);

            //5. Clean up test data
            eServiceOnlineGateway.Instance.DeleteProductHaul(resultProductHaul);
            eServiceOnlineGateway.Instance.DeleteProductHaulLoad(resultProductHaulLoad);
            eServiceOnlineGateway.Instance.DeleteShippingLoadSheet(resultShippingLoadSheet);
            eServiceOnlineGateway.Instance.DeleteRigJobCrewSection(resultRigJobSanjelCrewSection);
            eServiceOnlineGateway.Instance.DeleteCrewSchedule(resultSanjelCrewSchedule.Id);

            //6. Double check all data are cleaned
            var cleanedProductHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(resultProductHaulLoad.Id);
            Assert.IsNull(cleanedProductHaulLoad);
            var cleanedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(resultProductHaul.Id);
            Assert.IsNull(cleanedProductHaul);
            var cleanedShippingLoadSheet = eServiceOnlineGateway.Instance.GetShippingLoadSheetById(resultShippingLoadSheet.Id);
            Assert.IsNull(cleanedShippingLoadSheet);
            var cleanedRigJobSanjelCrewSection = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(resultRigJobSanjelCrewSection.Id);
            Assert.IsNull(cleanedRigJobSanjelCrewSection);
            var cleanedSanjelCrewSchedule = eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(resultSanjelCrewSchedule.Id);
            Assert.IsNull(cleanedSanjelCrewSchedule);
            globalSanjelScheduleId = 0;

        }

        [Test]
        public void TestScheduleProductHaulbyRigJobBinToThirdParty()
        {
            int thridPartyBulkerCrewId = thridPartyBulkerCrew1.Id;

            //1. Prepare test data
            productHaulLoad = PrepareProductHaulLoad();
            productHaul = PrepareScheduledProductHaulFormThirdParty();
            shippingLoadSheet = PrepareShippingLoadSheet();
            rigJobThirdPartyBulkerCrewSection = PrepareRigJobThirdPartyBulkerCrewSection();
            schedule = PrepareThirdPartySchedule();
            thirdPartyCrewLog.CrewStatus = BulkerCrewStatus.OffDuty;
            eServiceOnlineGateway.Instance.UpdateBulkerCrewLog(thirdPartyCrewLog);

            //2.Check if test data meet preconditions
            Assert.AreNotEqual(thridPartyBulkerCrewId, 0);
            // The bulker crew is currently is in OffDuty
            Assert.NotNull(productHaulLoad);
            BulkerCrewLog currentThridPartyCrewLog = eServiceOnlineGateway.Instance.GetThirdPartyBulkerCrewLog(thridPartyBulkerCrewId);
            Assert.AreEqual(BulkerCrewStatus.OffDuty, currentThridPartyCrewLog.CrewStatus);

            //3.Execute the method to test
            CrewProcess.UpdateBulkerCrewStatus(thridPartyBulkerCrew1.Id, true, loggedUser);

            //4. Verify the result meets post-conditions
            var resultProductHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productHaulLoad.Id);
            //A Blend Request (ProductHaulLoad) is scheduled
            Assert.Greater(resultProductHaulLoad.Id, 0);
            Assert.AreEqual(resultProductHaulLoad.ProductHaulLoadLifeStatus, ProductHaulLoadStatus.Scheduled);
            //A Product Haul is scheduled with Pod allocations specified
            var resultProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaul.Id);
            Assert.Greater(resultProductHaul.Id, 0);
            Assert.AreEqual(resultProductHaul.ProductHaulLifeStatus, ProductHaulStatus.Scheduled);
            //A Shipping Load Sheet is scheduled with Blend Unload Sheet(s) specified
            var resultShippingLoadSheet = eServiceOnlineGateway.Instance.GetShippingLoadSheetById(shippingLoadSheet.Id);
            Assert.Greater(resultShippingLoadSheet.Id, 0);
            Assert.AreEqual(resultShippingLoadSheet.ShippingStatus, ShippingStatus.Scheduled);
            //A crew assignment is scheduled (RigJobSanjelCrewSection)
            var resultRigJobThirdPartyBulkerCrewSection = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionById(rigJobThirdPartyBulkerCrewSection.Id);
            Assert.Greater(resultRigJobThirdPartyBulkerCrewSection.Id, 0);
            Assert.AreEqual(resultRigJobThirdPartyBulkerCrewSection.RigJobCrewSectionStatus, RigJobCrewSectionStatus.Scheduled);
            //Bulker crew schedule is created for the round trip. Start Time = Expected On Location Time - Estimated Travel Time, End Time = Expected On Location Time + Estimated Travel Time.
            var resultThirdPartySchedule = eServiceOnlineGateway.Instance.GetThirdPartyCrewScheduleById(schedule.Id);
            Assert.Greater(resultThirdPartySchedule.Id, 0);
            globalThirdPartyScheduleId = resultThirdPartySchedule.Id;
            Assert.AreEqual(resultThirdPartySchedule.StartTime, resultProductHaul.ExpectedOnLocationTime.AddHours((-1) * resultProductHaul.EstimatedTravelTime));
            Assert.AreEqual(resultThirdPartySchedule.EndTime, resultProductHaul.ExpectedOnLocationTime.AddHours(resultProductHaul.EstimatedTravelTime));
            // The bulker crew status is in LoadRequested status
            BulkerCrewLog resultBulkerCrewLog = eServiceOnlineGateway.Instance.GetThirdPartyBulkerCrewLog(thridPartyBulkerCrewId);
            Assert.AreEqual(BulkerCrewStatus.LoadRequested, resultBulkerCrewLog.CrewStatus);

            //5. Clean up test data
            eServiceOnlineGateway.Instance.DeleteProductHaul(resultProductHaul);
            eServiceOnlineGateway.Instance.DeleteProductHaulLoad(resultProductHaulLoad);
            eServiceOnlineGateway.Instance.DeleteShippingLoadSheet(resultShippingLoadSheet);
            eServiceOnlineGateway.Instance.DeleteRigJobThirdPartyBulkerCrewSection(resultRigJobThirdPartyBulkerCrewSection.Id);
            eServiceOnlineGateway.Instance.DeletethirdPartyBulkerCrewSchedule(resultThirdPartySchedule.Id);

            //6. Double check all data are cleaned
            var cleanedProductHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(resultProductHaulLoad.Id);
            Assert.IsNull(cleanedProductHaulLoad);
            var cleanedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(resultProductHaul.Id);
            Assert.IsNull(cleanedProductHaul);
            var cleanedShippingLoadSheet = eServiceOnlineGateway.Instance.GetShippingLoadSheetById(resultShippingLoadSheet.Id);
            Assert.IsNull(cleanedShippingLoadSheet);
            var cleanedRigJobSanjelCrewSection = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionById(resultRigJobThirdPartyBulkerCrewSection.Id);
            Assert.IsNull(cleanedRigJobSanjelCrewSection);
            var cleanedThirdPartyCrewSchedule = eServiceOnlineGateway.Instance.GetThirdPartyCrewScheduleById(resultThirdPartySchedule.Id);
            Assert.IsNull(cleanedThirdPartyCrewSchedule);
            globalThirdPartyScheduleId = 0;

        }

        [Test]
        public void TestScheduleProductHaulbyRigJobBinGoWithCrew()
        {
            int crewId = bulkerCrew1.Id;

            //1. Prepare test data
            productHaulLoad = PrepareProductHaulLoad();
            productHaul = PrepareScheduledProductHaulGoWithCrew();
            shippingLoadSheet = PrepareShippingLoadSheetGoWithCrew();
            rigJobSanjelCrewSection = PrepareRigJobSanjelCrewSection();
            schedule = PrepareScheduleGoWithCrew();
            bulkerCrewLog.CrewStatus = BulkerCrewStatus.OffDuty;
            eServiceOnlineGateway.Instance.UpdateBulkerCrewLog(bulkerCrewLog);

            //2.Check if test data meet preconditions
            Assert.AreNotEqual(crewId, 0);
            // The bulker crew is currently is in OffDuty
            Assert.NotNull(productHaulLoad);
            BulkerCrewLog currentbulkerCrewLog = eServiceOnlineGateway.Instance.GetBulkerCrewLog(crewId);
            Assert.AreEqual(BulkerCrewStatus.OffDuty, currentbulkerCrewLog.CrewStatus);
            // bin request

            //3.Execute the method to test
            CrewProcess.UpdateBulkerCrewStatus(bulkerCrew1.Id, false, loggedUser);

            //4. Verify the result meets post-conditions
            var resultProductHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productHaulLoad.Id);
            //A Blend Request (ProductHaulLoad) is scheduled
            Assert.Greater(resultProductHaulLoad.Id, 0);
            Assert.AreEqual(resultProductHaulLoad.ProductHaulLoadLifeStatus, ProductHaulLoadStatus.Scheduled);
            //A Product Haul is scheduled with Pod allocations specified
            var resultProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaul.Id);
            Assert.Greater(resultProductHaul.Id, 0);
            Assert.AreEqual(resultProductHaul.ProductHaulLifeStatus, ProductHaulStatus.Scheduled);
            //A Shipping Load Sheet is scheduled with Blend Unload Sheet(s) specified
            var resultShippingLoadSheet = eServiceOnlineGateway.Instance.GetShippingLoadSheetById(shippingLoadSheet.Id);
            Assert.Greater(resultShippingLoadSheet.Id, 0);
            Assert.AreEqual(resultShippingLoadSheet.ShippingStatus, ShippingStatus.Scheduled);
            //A crew assignment is scheduled (RigJobSanjelCrewSection)
            var resultRigJobSanjelCrewSection = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(rigJobSanjelCrewSection.Id);
            Assert.Greater(resultRigJobSanjelCrewSection.Id, 0);
            Assert.AreEqual(resultRigJobSanjelCrewSection.RigJobCrewSectionStatus, RigJobCrewSectionStatus.Scheduled);
            //Product Haul Expected On Location Time is same as job's Expected On Location Time
            var resultSanjelCrewSchedule = eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(schedule.Id);
            Assert.Greater(resultSanjelCrewSchedule.Id, 0);
            globalSanjelScheduleId = resultSanjelCrewSchedule.Id;
            Assert.AreEqual(resultSanjelCrewSchedule.StartTime.ToString("yyyyMMdd hh:MM:ss"), rigJob.CallCrewTime.ToString("yyyyMMdd hh:MM:ss"));
            Assert.AreEqual(resultSanjelCrewSchedule.EndTime.ToString("yyyyMMdd hh:MM:ss"), rigJob.JobDateTime.AddMinutes(360).ToString("yyyyMMdd hh:MM:ss"));
            // The bulker crew status is in LoadRequested status
            BulkerCrewLog resultBulkerCrewLog = eServiceOnlineGateway.Instance.GetBulkerCrewLog(crewId);
            Assert.AreEqual(BulkerCrewStatus.LoadRequested, resultBulkerCrewLog.CrewStatus);

            //5. Clean up test data
            eServiceOnlineGateway.Instance.DeleteProductHaul(resultProductHaul);
            eServiceOnlineGateway.Instance.DeleteProductHaulLoad(resultProductHaulLoad);
            eServiceOnlineGateway.Instance.DeleteShippingLoadSheet(resultShippingLoadSheet);
            eServiceOnlineGateway.Instance.DeleteRigJobCrewSection(resultRigJobSanjelCrewSection);
            eServiceOnlineGateway.Instance.DeleteCrewSchedule(resultSanjelCrewSchedule.Id);

            //6. Double check all data are cleaned
            var cleanedProductHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(resultProductHaulLoad.Id);
            Assert.IsNull(cleanedProductHaulLoad);
            var cleanedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(resultProductHaul.Id);
            Assert.IsNull(cleanedProductHaul);
            var cleanedShippingLoadSheet = eServiceOnlineGateway.Instance.GetShippingLoadSheetById(resultShippingLoadSheet.Id);
            Assert.IsNull(cleanedShippingLoadSheet);
            var cleanedRigJobSanjelCrewSection = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(resultRigJobSanjelCrewSection.Id);
            Assert.IsNull(cleanedRigJobSanjelCrewSection);
            var cleanedSanjelCrewSchedule = eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(resultSanjelCrewSchedule.Id);
            Assert.IsNull(cleanedSanjelCrewSchedule);
            globalSanjelScheduleId = 0;

        }

        [Test]
        public void TestScheduleProductHaulbyRigJobBinToThirdPartyGoWithCrew()
        {
            int thridPartyBulkerCrewId = thridPartyBulkerCrew1.Id;

            //1. Prepare test data
            productHaulLoad = PrepareProductHaulLoad();
            productHaul = PrepareScheduledProductHaulFormThirdPartyGoWithCrew();
            shippingLoadSheet = PrepareShippingLoadSheetGoWithCrew();
            rigJobThirdPartyBulkerCrewSection = PrepareRigJobThirdPartyBulkerCrewSection();
            schedule = PrepareThirdPartyScheduleGoWithCrew();
            thirdPartyCrewLog.CrewStatus = BulkerCrewStatus.OffDuty;
            eServiceOnlineGateway.Instance.UpdateBulkerCrewLog(thirdPartyCrewLog);

            //2.Check if test data meet preconditions
            Assert.AreNotEqual(thridPartyBulkerCrewId, 0);
            // The bulker crew is currently is in OffDuty
            Assert.NotNull(productHaulLoad);
            BulkerCrewLog currentThridPartyCrewLog = eServiceOnlineGateway.Instance.GetThirdPartyBulkerCrewLog(thridPartyBulkerCrewId);
            Assert.AreEqual(BulkerCrewStatus.OffDuty, currentThridPartyCrewLog.CrewStatus);
            // bin request

            //3.Execute the method to test
            CrewProcess.UpdateBulkerCrewStatus(thridPartyBulkerCrew1.Id, true, loggedUser);

            //4. Verify the result meets post-conditions
            var resultProductHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productHaulLoad.Id);
            //A Blend Request (ProductHaulLoad) is scheduled
            Assert.Greater(resultProductHaulLoad.Id, 0);
            Assert.AreEqual(resultProductHaulLoad.ProductHaulLoadLifeStatus, ProductHaulLoadStatus.Scheduled);
            //A Product Haul is scheduled with Pod allocations specified
            var resultProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaul.Id);
            Assert.Greater(resultProductHaul.Id, 0);
            Assert.AreEqual(resultProductHaul.ProductHaulLifeStatus, ProductHaulStatus.Scheduled);
            //A Shipping Load Sheet is scheduled with Blend Unload Sheet(s) specified
            var resultShippingLoadSheet = eServiceOnlineGateway.Instance.GetShippingLoadSheetById(shippingLoadSheet.Id);
            Assert.Greater(resultShippingLoadSheet.Id, 0);
            Assert.AreEqual(resultShippingLoadSheet.ShippingStatus, ShippingStatus.Scheduled);
            //A crew assignment is scheduled (RigJobSanjelCrewSection)
            var resultRigJobThirdPartyBulkerCrewSection = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionById(rigJobThirdPartyBulkerCrewSection.Id);
            Assert.Greater(resultRigJobThirdPartyBulkerCrewSection.Id, 0);
            Assert.AreEqual(resultRigJobThirdPartyBulkerCrewSection.RigJobCrewSectionStatus, RigJobCrewSectionStatus.Scheduled);
            //Product Haul Expected On Location Time is same as job's Expected On Location Time
            var resultThirdPartySchedule = eServiceOnlineGateway.Instance.GetThirdPartyCrewScheduleById(schedule.Id);
            Assert.Greater(resultThirdPartySchedule.Id, 0);
            globalThirdPartyScheduleId = resultThirdPartySchedule.Id;
            Assert.AreEqual(resultThirdPartySchedule.StartTime.ToString("yyyyMMdd hh:MM:ss"), rigJob.CallCrewTime.ToString("yyyyMMdd hh:MM:ss"));
            Assert.AreEqual(resultThirdPartySchedule.EndTime.ToString("yyyyMMdd hh:MM:ss"), rigJob.JobDateTime.AddMinutes(360).ToString("yyyyMMdd hh:MM:ss"));
            // The bulker crew status is in LoadRequested status
            BulkerCrewLog resultBulkerCrewLog = eServiceOnlineGateway.Instance.GetThirdPartyBulkerCrewLog(thridPartyBulkerCrewId);
            Assert.AreEqual(BulkerCrewStatus.LoadRequested, resultBulkerCrewLog.CrewStatus);

            //5. Clean up test data
            eServiceOnlineGateway.Instance.DeleteProductHaul(resultProductHaul);
            eServiceOnlineGateway.Instance.DeleteProductHaulLoad(resultProductHaulLoad);
            eServiceOnlineGateway.Instance.DeleteShippingLoadSheet(resultShippingLoadSheet);
            eServiceOnlineGateway.Instance.DeleteRigJobThirdPartyBulkerCrewSection(resultRigJobThirdPartyBulkerCrewSection.Id);
            eServiceOnlineGateway.Instance.DeletethirdPartyBulkerCrewSchedule(resultThirdPartySchedule.Id);

            //6. Double check all data are cleaned
            var cleanedProductHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(resultProductHaulLoad.Id);
            Assert.IsNull(cleanedProductHaulLoad);
            var cleanedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(resultProductHaul.Id);
            Assert.IsNull(cleanedProductHaul);
            var cleanedShippingLoadSheet = eServiceOnlineGateway.Instance.GetShippingLoadSheetById(resultShippingLoadSheet.Id);
            Assert.IsNull(cleanedShippingLoadSheet);
            var cleanedRigJobSanjelCrewSection = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionById(resultRigJobThirdPartyBulkerCrewSection.Id);
            Assert.IsNull(cleanedRigJobSanjelCrewSection);
            var cleaneThirdPartyCrewSchedule = eServiceOnlineGateway.Instance.GetThirdPartyCrewScheduleById(resultThirdPartySchedule.Id);
            Assert.IsNull(cleaneThirdPartyCrewSchedule);
            globalThirdPartyScheduleId = 0;

        }
        #endregion

        #region Cancel ProductHaul
        [Test]
        public void TestCancelProductHaulBulkerCrewOffDuty()
        {
            int crewId = bulkerCrew1.Id;

            //1. Prepare test data
            productHaulLoad = PrepareProductHaulLoad();
            productHaul = PrepareScheduledProductHaul();
            shippingLoadSheet = PrepareShippingLoadSheet();
            rigJobSanjelCrewSection = PrepareRigJobSanjelCrewSection();
            schedule = PrepareSchedule();
            bulkerCrewLog.CrewStatus = BulkerCrewStatus.LoadRequested;
            eServiceOnlineGateway.Instance.UpdateBulkerCrewLog(bulkerCrewLog);

            //2.Check if test data meet preconditions
            Assert.AreNotEqual(crewId, 0);
            // The bulker crew is currently is in OffDuty
            Assert.NotNull(productHaulLoad);
            BulkerCrewLog currentbulkerCrewLog = eServiceOnlineGateway.Instance.GetBulkerCrewLog(crewId);
            Assert.AreEqual(BulkerCrewStatus.LoadRequested, currentbulkerCrewLog.CrewStatus);

            //3.Execute the method to test
            ProductHaulProcess.DeleteProductHaul(productHaul, true);

            //4. Verify the result meets post-conditions
            var resultProductHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productHaulLoad.Id);
            Assert.Greater(resultProductHaulLoad.Id, 0);
            Assert.AreEqual(resultProductHaulLoad.ProductHaulLoadLifeStatus, ProductHaulLoadStatus.Scheduled);
            var resultProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaul.Id);
            Assert.IsNull(resultProductHaul);
            var resultShippingLoadSheet = eServiceOnlineGateway.Instance.GetShippingLoadSheetById(shippingLoadSheet.Id);
            Assert.IsNull(resultShippingLoadSheet);
            var resultRigJobSanjelCrewSection = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(rigJobSanjelCrewSection.Id);
            Assert.IsNull(resultRigJobSanjelCrewSection);
            var resultSanjelCrewSchedule = eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(schedule.Id);
            Assert.IsNull(resultSanjelCrewSchedule);
            BulkerCrewLog resultBulkerCrewLog = eServiceOnlineGateway.Instance.GetBulkerCrewLog(crewId);
            Assert.AreEqual(BulkerCrewStatus.OffDuty, resultBulkerCrewLog.CrewStatus);

            //5. Clean up test data
            eServiceOnlineGateway.Instance.DeleteProductHaulLoad(resultProductHaulLoad);

            //6. Double check all data are cleaned
            var cleanedProductHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(resultProductHaulLoad.Id);
            Assert.IsNull(cleanedProductHaulLoad);
            var cleanedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaul.Id);
            Assert.IsNull(cleanedProductHaul);
            var cleanedShippingLoadSheet = eServiceOnlineGateway.Instance.GetShippingLoadSheetById(shippingLoadSheet.Id);
            Assert.IsNull(cleanedShippingLoadSheet);
            var cleanedRigJobSanjelCrewSection = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(rigJobSanjelCrewSection.Id);
            Assert.IsNull(cleanedRigJobSanjelCrewSection);
            var cleanedSanjelCrewSchedule = eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(schedule.Id);
            Assert.IsNull(cleanedSanjelCrewSchedule);

        }

        [Test]
        public void TestCancelProductHaulBulkerCrewLoadRequested()
        {
            int crewId = bulkerCrew1.Id;

            //1. Prepare test data
            productHaulLoad = PrepareProductHaulLoad();
            productHaul = PrepareScheduledProductHaul();
            secondProductHaul = PrepareSecondScheduledProductHaul();
            shippingLoadSheet = PrepareShippingLoadSheet();
            rigJobSanjelCrewSection = PrepareRigJobSanjelCrewSection();
            secondRigJobSanjelCrewSection = PrepareSecondRigJobSanjelCrewSection();
            schedule = PrepareSchedule();
            secondSchedule = PrepareSecondSchedule();
            bulkerCrewLog.CrewStatus = BulkerCrewStatus.LoadRequested;
            eServiceOnlineGateway.Instance.UpdateBulkerCrewLog(bulkerCrewLog);

            //2.Check if test data meet preconditions
            Assert.AreNotEqual(crewId, 0);
            // The bulker crew is currently is in OffDuty
            Assert.NotNull(productHaulLoad);
            Assert.NotNull(secondProductHaul);
            Assert.NotNull(secondRigJobSanjelCrewSection);
            Assert.Greater(schedule.Id, 0);
            globalSanjelScheduleId = schedule.Id;
            Assert.NotNull(secondSchedule);
            Assert.Greater(secondProductHaul.Id, 0);
            Assert.Greater(secondSchedule.Id, 0);
            globalSecondSanjelScheduleId = secondSchedule.Id;
            BulkerCrewLog currentbulkerCrewLog = eServiceOnlineGateway.Instance.GetBulkerCrewLog(crewId);
            Assert.AreEqual(BulkerCrewStatus.LoadRequested, currentbulkerCrewLog.CrewStatus);

            //3.Execute the method to test
            ProductHaulProcess.DeleteProductHaul(productHaul, true);

            //4. Verify the result meets post-conditions
            var resultProductHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productHaulLoad.Id);
            Assert.Greater(resultProductHaulLoad.Id, 0);
            Assert.AreEqual(resultProductHaulLoad.ProductHaulLoadLifeStatus, ProductHaulLoadStatus.Scheduled);
            var resultProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaul.Id);
            Assert.IsNull(resultProductHaul);
            var resultShippingLoadSheet = eServiceOnlineGateway.Instance.GetShippingLoadSheetById(shippingLoadSheet.Id);
            Assert.IsNull(resultShippingLoadSheet);
            var resultRigJobSanjelCrewSection = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(rigJobSanjelCrewSection.Id);
            Assert.IsNull(resultRigJobSanjelCrewSection);
            var resultSanjelCrewSchedule = eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(schedule.Id);
            Assert.IsNull(resultSanjelCrewSchedule);
            BulkerCrewLog resultBulkerCrewLog = eServiceOnlineGateway.Instance.GetBulkerCrewLog(crewId);
            Assert.AreEqual(BulkerCrewStatus.LoadRequested, resultBulkerCrewLog.CrewStatus);

            //5. Clean up test data
            eServiceOnlineGateway.Instance.DeleteProductHaulLoad(resultProductHaulLoad);
            eServiceOnlineGateway.Instance.DeleteProductHaul(secondProductHaul);
            eServiceOnlineGateway.Instance.DeleteRigJobCrewSection(secondRigJobSanjelCrewSection);
            eServiceOnlineGateway.Instance.DeleteCrewSchedule(secondSchedule.Id);

            //6. Double check all data are cleaned
            var cleanedProductHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(resultProductHaulLoad.Id);
            Assert.IsNull(cleanedProductHaulLoad);
            var cleanedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaul.Id);
            Assert.IsNull(cleanedProductHaul);
            var cleanedSecondProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(secondProductHaul.Id);
            Assert.IsNull(cleanedSecondProductHaul);
            var cleanedShippingLoadSheet = eServiceOnlineGateway.Instance.GetShippingLoadSheetById(shippingLoadSheet.Id);
            Assert.IsNull(cleanedShippingLoadSheet);
            var cleanedRigJobSanjelCrewSection = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(rigJobSanjelCrewSection.Id);
            Assert.IsNull(cleanedRigJobSanjelCrewSection);
            var cleanedRigJobSanjelCrewSection2 = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(secondRigJobSanjelCrewSection.Id);
            Assert.IsNull(cleanedRigJobSanjelCrewSection2);
            var cleanedSanjelCrewSchedule = eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(schedule.Id);
            Assert.IsNull(cleanedSanjelCrewSchedule);
            globalSanjelScheduleId = 0;
            var cleanedSanjelCrewSchedule2 = eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(secondSchedule.Id);
            Assert.IsNull(cleanedSanjelCrewSchedule2);
            globalSecondSanjelScheduleId = 0;

        }
        [Test]
        public void TestCancelProductHaulBulkerCrewCalled()
        {
            int crewId = bulkerCrew1.Id;

            //1. Prepare test data
            productHaulLoad = PrepareProductHaulLoad();
            productHaul = PrepareScheduledProductHaul();
            secondProductHaul = PrepareSecondScheduledProductHaul();
            shippingLoadSheet = PrepareShippingLoadSheet();
            rigJobSanjelCrewSection = PrepareRigJobSanjelCrewSection();
            secondRigJobSanjelCrewSection = PrepareSecondRigJobSanjelCrewSection();
            schedule = PrepareSchedule();
            secondSchedule = PrepareSecondSchedule();
            bulkerCrewLog.CrewStatus = BulkerCrewStatus.Called;
            eServiceOnlineGateway.Instance.UpdateBulkerCrewLog(bulkerCrewLog);

            //2.Check if test data meet preconditions
            Assert.AreNotEqual(crewId, 0);
            Assert.NotNull(productHaulLoad);
            Assert.Greater(schedule.Id, 0);
            globalSanjelScheduleId = schedule.Id;
            Assert.NotNull(secondProductHaul);
            Assert.NotNull(secondRigJobSanjelCrewSection);
            Assert.NotNull(secondSchedule);
            Assert.Greater(secondProductHaul.Id, 0);
            Assert.Greater(secondSchedule.Id, 0);
            globalSecondSanjelScheduleId = secondSchedule.Id;
            BulkerCrewLog currentbulkerCrewLog = eServiceOnlineGateway.Instance.GetBulkerCrewLog(crewId);
            Assert.AreEqual(BulkerCrewStatus.Called, currentbulkerCrewLog.CrewStatus);

            //3.Execute the method to test
            ProductHaulProcess.DeleteProductHaul(productHaul, true);

            //4. Verify the result meets post-conditions
            var resultProductHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productHaulLoad.Id);
            Assert.Greater(resultProductHaulLoad.Id, 0);
            Assert.AreEqual(resultProductHaulLoad.ProductHaulLoadLifeStatus, ProductHaulLoadStatus.Scheduled);
            var resultProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaul.Id);
            Assert.IsNull(resultProductHaul);
            var resultShippingLoadSheet = eServiceOnlineGateway.Instance.GetShippingLoadSheetById(shippingLoadSheet.Id);
            Assert.IsNull(resultShippingLoadSheet);
            var resultRigJobSanjelCrewSection = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(rigJobSanjelCrewSection.Id);
            Assert.IsNull(resultRigJobSanjelCrewSection);
            var resultSanjelCrewSchedule = eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(schedule.Id);
            Assert.IsNull(resultSanjelCrewSchedule);
            BulkerCrewLog resultBulkerCrewLog = eServiceOnlineGateway.Instance.GetBulkerCrewLog(crewId);
            Assert.AreEqual(BulkerCrewStatus.Called, resultBulkerCrewLog.CrewStatus);

            //5. Clean up test data
            eServiceOnlineGateway.Instance.DeleteProductHaulLoad(resultProductHaulLoad);
            eServiceOnlineGateway.Instance.DeleteProductHaul(secondProductHaul);
            eServiceOnlineGateway.Instance.DeleteRigJobCrewSection(secondRigJobSanjelCrewSection);
            eServiceOnlineGateway.Instance.DeleteCrewSchedule(secondSchedule.Id);

            //6. Double check all data are cleaned
            var cleanedProductHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(resultProductHaulLoad.Id);
            Assert.IsNull(cleanedProductHaulLoad);
            var cleanedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaul.Id);
            Assert.IsNull(cleanedProductHaul);
            var cleanedSecondProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(secondProductHaul.Id);
            Assert.IsNull(cleanedSecondProductHaul);
            var cleanedShippingLoadSheet = eServiceOnlineGateway.Instance.GetShippingLoadSheetById(shippingLoadSheet.Id);
            Assert.IsNull(cleanedShippingLoadSheet);
            var cleanedRigJobSanjelCrewSection = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(rigJobSanjelCrewSection.Id);
            Assert.IsNull(cleanedRigJobSanjelCrewSection);
            var cleanedRigJobSanjelCrewSection2 = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(secondRigJobSanjelCrewSection.Id);
            Assert.IsNull(cleanedRigJobSanjelCrewSection2);
            var cleanedSanjelCrewSchedule = eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(schedule.Id);
            Assert.IsNull(cleanedSanjelCrewSchedule);
            globalSanjelScheduleId = 0;
            var cleanedSanjelCrewSchedule2 = eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(secondSchedule.Id);
            Assert.IsNull(cleanedSanjelCrewSchedule2);
            globalSecondSanjelScheduleId = 0;

        }

        [Test]
        public void TestCancelProductHaulBulkerCrewReturned()
        {
            int crewId = bulkerCrew1.Id;

            //1. Prepare test data
            productHaulLoad = PrepareProductHaulLoad();
            productHaul = PrepareScheduledProductHaul();
            secondProductHaul = PrepareSecondScheduledProductHaul();
            shippingLoadSheet = PrepareShippingLoadSheet();
            rigJobSanjelCrewSection = PrepareRigJobSanjelCrewSection();
            secondRigJobSanjelCrewSection = PrepareSecondRigJobSanjelCrewSection();
            schedule = PrepareSchedule();
            secondSchedule = PrepareSecondSchedule();
            TestAssignCrewOtherStatus(rigJobSanjelCrewSection, ProductHaulStatus.OnLocation, RigJobCrewSectionStatus.Returned);
            bulkerCrewLog.CrewStatus = BulkerCrewStatus.LoadRequested;
            eServiceOnlineGateway.Instance.UpdateBulkerCrewLog(bulkerCrewLog);

            //2.Check if test data meet preconditions
            Assert.AreNotEqual(crewId, 0);
            Assert.NotNull(productHaulLoad);
            Assert.Greater(schedule.Id, 0);
            globalSanjelScheduleId = schedule.Id;
            Assert.NotNull(secondProductHaul);
            Assert.NotNull(secondRigJobSanjelCrewSection);
            Assert.NotNull(secondSchedule);
            Assert.Greater(secondProductHaul.Id, 0);
            Assert.Greater(secondSchedule.Id, 0);
            globalSecondSanjelScheduleId = secondSchedule.Id;
            BulkerCrewLog currentbulkerCrewLog = eServiceOnlineGateway.Instance.GetBulkerCrewLog(crewId);
            Assert.AreEqual(BulkerCrewStatus.LoadRequested, currentbulkerCrewLog.CrewStatus);

            //3.Execute the method to test
            ProductHaulProcess.DeleteProductHaul(secondProductHaul, true);

            //4. Verify the result meets post-conditions
            var resultProductHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productHaulLoad.Id);
            Assert.Greater(resultProductHaulLoad.Id, 0);
            Assert.AreEqual(resultProductHaulLoad.ProductHaulLoadLifeStatus, ProductHaulLoadStatus.Scheduled);
            var resultProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(secondProductHaul.Id);
            Assert.IsNull(resultProductHaul);
            var resultShippingLoadSheet = eServiceOnlineGateway.Instance.GetShippingLoadSheetById(shippingLoadSheet.Id);
            Assert.IsNotNull(resultShippingLoadSheet);
            var resultRigJobSanjelCrewSection = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(secondRigJobSanjelCrewSection.Id);
            Assert.IsNull(resultRigJobSanjelCrewSection);
            var resultSanjelCrewSchedule = eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(secondSchedule.Id);
            Assert.IsNull(resultSanjelCrewSchedule);
            BulkerCrewLog resultBulkerCrewLog = eServiceOnlineGateway.Instance.GetBulkerCrewLog(crewId);
            Assert.AreEqual(BulkerCrewStatus.Returned, resultBulkerCrewLog.CrewStatus);

            //5. Clean up test data
            eServiceOnlineGateway.Instance.DeleteProductHaulLoad(resultProductHaulLoad);
            eServiceOnlineGateway.Instance.DeleteProductHaul(productHaul);
            eServiceOnlineGateway.Instance.DeleteRigJobCrewSection(rigJobSanjelCrewSection);
            eServiceOnlineGateway.Instance.DeleteCrewSchedule(schedule.Id);
            eServiceOnlineGateway.Instance.DeleteShippingLoadSheet(resultShippingLoadSheet);

            //6. Double check all data are cleaned
            var cleanedProductHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(resultProductHaulLoad.Id);
            Assert.IsNull(cleanedProductHaulLoad);
            var cleanedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaul.Id);
            Assert.IsNull(cleanedProductHaul);
            var cleanedSecondProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(secondProductHaul.Id);
            Assert.IsNull(cleanedSecondProductHaul);
            var cleanedShippingLoadSheet = eServiceOnlineGateway.Instance.GetShippingLoadSheetById(shippingLoadSheet.Id);
            Assert.IsNull(cleanedShippingLoadSheet);
            var cleanedRigJobSanjelCrewSection = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(rigJobSanjelCrewSection.Id);
            Assert.IsNull(cleanedRigJobSanjelCrewSection);
            var cleanedRigJobSanjelCrewSection2 = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(secondRigJobSanjelCrewSection.Id);
            Assert.IsNull(cleanedRigJobSanjelCrewSection2);
            var cleanedSanjelCrewSchedule = eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(schedule.Id);
            Assert.IsNull(cleanedSanjelCrewSchedule);
            globalSanjelScheduleId = 0;
            var cleanedSanjelCrewSchedule2 = eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(secondSchedule.Id);
            Assert.IsNull(cleanedSanjelCrewSchedule2);
            globalSecondSanjelScheduleId = 0;

        }

        [Test]
        public void TestCancelProductHaulBulkerCrewOtherStatuses()
        {
            int crewId = bulkerCrew1.Id;

            //1. Prepare test data
            productHaulLoad = PrepareProductHaulLoad();
            productHaul = PrepareScheduledProductHaul();
            secondProductHaul = PrepareSecondScheduledProductHaul();
            thirdProductHaul = PrepareThirdScheduledProductHaul();
            shippingLoadSheet = PrepareShippingLoadSheet();
            rigJobSanjelCrewSection = PrepareRigJobSanjelCrewSection();
            secondRigJobSanjelCrewSection = PrepareSecondRigJobSanjelCrewSection();
            thirdRigJobSanjelCrewSection = PrepareThirdRigJobSanjelCrewSection();
            schedule = PrepareSchedule();
            secondSchedule = PrepareSecondSchedule();
            thirdSchedule = PrepareThirdSchedule();
            TestAssignCrewOtherStatus(rigJobSanjelCrewSection, ProductHaulStatus.OnLocation, RigJobCrewSectionStatus.Returned);
            bulkerCrewLog.CrewStatus = BulkerCrewStatus.LoadRequested;
            eServiceOnlineGateway.Instance.UpdateBulkerCrewLog(bulkerCrewLog);

            //2.Check if test data meet preconditions
            Assert.AreNotEqual(crewId, 0);
            Assert.NotNull(productHaulLoad);
            Assert.Greater(schedule.Id, 0);
            globalSanjelScheduleId = schedule.Id;
            Assert.NotNull(secondProductHaul);
            Assert.NotNull(secondRigJobSanjelCrewSection);
            Assert.NotNull(secondSchedule);
            Assert.Greater(secondProductHaul.Id, 0);
            Assert.Greater(secondSchedule.Id, 0);
            globalSecondSanjelScheduleId = secondSchedule.Id;
            Assert.NotNull(thirdProductHaul);
            Assert.NotNull(thirdRigJobSanjelCrewSection);
            Assert.NotNull(thirdSchedule);
            Assert.Greater(thirdProductHaul.Id, 0);
            Assert.Greater(thirdSchedule.Id, 0);
            globalThirdSanjelScheduleId = thirdSchedule.Id;
            BulkerCrewLog currentbulkerCrewLog = eServiceOnlineGateway.Instance.GetBulkerCrewLog(crewId);
            Assert.AreEqual(BulkerCrewStatus.LoadRequested, currentbulkerCrewLog.CrewStatus);

            //3.Execute the method to test
            ProductHaulProcess.DeleteProductHaul(secondProductHaul, true);

            //4. Verify the result meets post-conditions
            var resultProductHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productHaulLoad.Id);
            Assert.Greater(resultProductHaulLoad.Id, 0);
            Assert.AreEqual(resultProductHaulLoad.ProductHaulLoadLifeStatus, ProductHaulLoadStatus.Scheduled);
            var resultProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(secondProductHaul.Id);
            Assert.IsNull(resultProductHaul);
            var resultShippingLoadSheet = eServiceOnlineGateway.Instance.GetShippingLoadSheetById(shippingLoadSheet.Id);
            Assert.IsNotNull(resultShippingLoadSheet);
            var resultRigJobSanjelCrewSection = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(secondRigJobSanjelCrewSection.Id);
            Assert.IsNull(resultRigJobSanjelCrewSection);
            var resultSanjelCrewSchedule = eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(secondSchedule.Id);
            Assert.IsNull(resultSanjelCrewSchedule);
            BulkerCrewLog resultBulkerCrewLog = eServiceOnlineGateway.Instance.GetBulkerCrewLog(crewId);
            Assert.AreEqual(BulkerCrewStatus.LoadRequested, resultBulkerCrewLog.CrewStatus);

            //5. Clean up test data
            eServiceOnlineGateway.Instance.DeleteProductHaulLoad(resultProductHaulLoad);
            eServiceOnlineGateway.Instance.DeleteProductHaul(productHaul);
            eServiceOnlineGateway.Instance.DeleteRigJobCrewSection(rigJobSanjelCrewSection);
            eServiceOnlineGateway.Instance.DeleteCrewSchedule(schedule.Id);
            eServiceOnlineGateway.Instance.DeleteProductHaul(thirdProductHaul);
            eServiceOnlineGateway.Instance.DeleteRigJobCrewSection(thirdRigJobSanjelCrewSection);
            eServiceOnlineGateway.Instance.DeleteCrewSchedule(thirdSchedule.Id);
            eServiceOnlineGateway.Instance.DeleteShippingLoadSheet(resultShippingLoadSheet);

            //6. Double check all data are cleaned
            var cleanedProductHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(resultProductHaulLoad.Id);
            Assert.IsNull(cleanedProductHaulLoad);
            var cleanedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaul.Id);
            Assert.IsNull(cleanedProductHaul);
            var cleanedProductHaul2 = eServiceOnlineGateway.Instance.GetProductHaulById(secondProductHaul.Id);
            Assert.IsNull(cleanedProductHaul2);
            var cleanedProductHaul3 = eServiceOnlineGateway.Instance.GetProductHaulById(thirdProductHaul.Id);
            Assert.IsNull(cleanedProductHaul3);
            var cleanedShippingLoadSheet = eServiceOnlineGateway.Instance.GetShippingLoadSheetById(shippingLoadSheet.Id);
            Assert.IsNull(cleanedShippingLoadSheet);
            var cleanedRigJobSanjelCrewSection = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(rigJobSanjelCrewSection.Id);
            Assert.IsNull(cleanedRigJobSanjelCrewSection);
            var cleanedRigJobSanjelCrewSection2 = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(secondRigJobSanjelCrewSection.Id);
            Assert.IsNull(cleanedRigJobSanjelCrewSection2);
            var cleanedRigJobSanjelCrewSection3 = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(thirdRigJobSanjelCrewSection.Id);
            Assert.IsNull(cleanedRigJobSanjelCrewSection3);
            var cleanedSanjelCrewSchedule = eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(schedule.Id);
            Assert.IsNull(cleanedSanjelCrewSchedule);
            globalSanjelScheduleId = 0;
            var cleanedSanjelCrewSchedule2 = eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(secondSchedule.Id);
            Assert.IsNull(cleanedSanjelCrewSchedule2);
            globalSecondSanjelScheduleId = 0;
            var cleanedSanjelCrewSchedule3 = eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(thirdSchedule.Id);
            Assert.IsNull(cleanedSanjelCrewSchedule3);
            globalThirdSanjelScheduleId = 0;
        }
        #endregion

        #region Reschedule ProductHaul
        [Test]
        public void TestRescheduleProductHaulSanjelCrewToSanjelCrew()
        {
            int crewId = bulkerCrew1.Id;
            //1. Prepare test data
            PrepareBulkerCrew2();
            PrepareBulkerCrew2Log();
            Assert.NotNull(bulkerCrew2);
            Assert.NotNull(bulkerCrew2Log);
            int crew2Id = bulkerCrew2.Id;

            productHaulLoad = PrepareProductHaulLoad();
            productHaul = PrepareScheduledProductHaul();
            shippingLoadSheet = PrepareShippingLoadSheet();
            rigJobSanjelCrewSection = PrepareRigJobSanjelCrewSection();
            schedule = PrepareSchedule();
            // Crew1 LoadRequested
            bulkerCrewLog.CrewStatus = BulkerCrewStatus.LoadRequested;
            eServiceOnlineGateway.Instance.UpdateBulkerCrewLog(bulkerCrewLog);
            //Change Crew2 OffDuty
            bulkerCrew2Log.CrewStatus = BulkerCrewStatus.OffDuty;
            eServiceOnlineGateway.Instance.UpdateBulkerCrewLog(bulkerCrew2Log);

            //2.Check if test data meet preconditions
            Assert.AreNotEqual(crewId, 0);
            Assert.NotNull(productHaulLoad);
            Assert.NotNull(productHaul);
            Assert.NotNull(shippingLoadSheet);
            Assert.NotNull(rigJobSanjelCrewSection);
            Assert.NotNull(schedule);
            Assert.Greater(schedule.Id, 0);
            globalSanjelScheduleId = schedule.Id;
            BulkerCrewLog currentbulkerCrewLog = eServiceOnlineGateway.Instance.GetBulkerCrewLog(crewId);
            Assert.AreEqual(BulkerCrewStatus.LoadRequested, currentbulkerCrewLog.CrewStatus);
            BulkerCrewLog currentbulkerCrew2Log = eServiceOnlineGateway.Instance.GetBulkerCrewLog(crew2Id);
            Assert.AreEqual(BulkerCrewStatus.OffDuty, currentbulkerCrew2Log.CrewStatus);

            //3.Execute the method to test
            ProductHaulProcess.ReleaseProductHaulCrew(productHaul);
            SanjelCrewSchedule newScheduled = (SanjelCrewSchedule)RigBoardProcess.CreateSchedule(crew2Id, false, productHaul.EstimatedLoadTime, rigJob.Id, false, DateTime.Now.AddHours(3), productHaul, "IntegrationTest");
            CrewProcess.UpdateBulkerCrewStatus(crew2Id, false, loggedUser);

            //4. Verify the result meets post-conditions
            var resultProductHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productHaulLoad.Id);
            Assert.Greater(resultProductHaulLoad.Id, 0);
            Assert.AreEqual(resultProductHaulLoad.ProductHaulLoadLifeStatus, ProductHaulLoadStatus.Scheduled);
            //var resultProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaul.Id);
            //Assert.IsNull(resultProductHaul);
            var resultShippingLoadSheet = eServiceOnlineGateway.Instance.GetShippingLoadSheetById(shippingLoadSheet.Id);
            Assert.IsNotNull(resultShippingLoadSheet);
            var resultRigJobSanjelCrewSection = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(rigJobSanjelCrewSection.Id);
            Assert.IsNull(resultRigJobSanjelCrewSection);
            var resultSanjelCrewSchedule = eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(schedule.Id);
            Assert.IsNull(resultSanjelCrewSchedule);
            globalSanjelScheduleId = 0;
            var resultNewCrewSchedule = eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(newScheduled.Id);
            Assert.IsNotNull(resultNewCrewSchedule);
            globalSecondSanjelScheduleId = resultNewCrewSchedule.Id;
            BulkerCrewLog resultBulkerCrewLog = eServiceOnlineGateway.Instance.GetBulkerCrewLog(crewId);
            Assert.AreEqual(BulkerCrewStatus.OffDuty, resultBulkerCrewLog.CrewStatus);
            BulkerCrewLog resultBulkerCrew2Log = eServiceOnlineGateway.Instance.GetBulkerCrewLog(crew2Id);
            Assert.AreEqual(BulkerCrewStatus.LoadRequested, resultBulkerCrew2Log.CrewStatus);

            //5. Clean up test data
            eServiceOnlineGateway.Instance.DeleteProductHaulLoad(resultProductHaulLoad);
            eServiceOnlineGateway.Instance.DeleteRigJobCrewSection(newScheduled.RigJobSanjelCrewSection);
            eServiceOnlineGateway.Instance.DeleteCrewSchedule(newScheduled.Id);
            eServiceOnlineGateway.Instance.DeleteProductHaul(newScheduled.RigJobSanjelCrewSection.ProductHaul);
            eServiceOnlineGateway.Instance.DeleteShippingLoadSheet(resultShippingLoadSheet);

            //6. Double check all data are cleaned
            var cleanedProductHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(resultProductHaulLoad.Id);
            Assert.IsNull(cleanedProductHaulLoad);
            var cleanedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaul.Id);
            Assert.IsNull(cleanedProductHaul);
            var cleanedShippingLoadSheet = eServiceOnlineGateway.Instance.GetShippingLoadSheetById(shippingLoadSheet.Id);
            Assert.IsNull(cleanedShippingLoadSheet);
            var cleanedRigJobSanjelCrewSection = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(rigJobSanjelCrewSection.Id);
            Assert.IsNull(cleanedRigJobSanjelCrewSection);
            var cleanedSanjelCrewSchedule = eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(newScheduled.Id);
            Assert.IsNull(cleanedSanjelCrewSchedule);
            globalSecondSanjelScheduleId = 0;

        }
        [Test]
        public void TestRescheduleProductHaulSanjelCrewToThirdPartyCrew()
        {
            //1. Prepare test data
            int crewId = bulkerCrew1.Id;
            int thridPartyBulkerCrew1Id = thridPartyBulkerCrew1.Id;
            Assert.Greater(thridPartyBulkerCrew1Id,0); 
            Assert.NotNull(thirdPartyCrewLog);

            productHaulLoad = PrepareProductHaulLoad();
            productHaul = PrepareScheduledProductHaul();
            shippingLoadSheet = PrepareShippingLoadSheet();
            rigJobSanjelCrewSection = PrepareRigJobSanjelCrewSection();
            schedule = PrepareSchedule();
            // Crew1 LoadRequested
            bulkerCrewLog.CrewStatus = BulkerCrewStatus.LoadRequested;
            eServiceOnlineGateway.Instance.UpdateBulkerCrewLog(bulkerCrewLog);
            //Change Crew2 OffDuty
            thirdPartyCrewLog.CrewStatus = BulkerCrewStatus.OffDuty;
            eServiceOnlineGateway.Instance.UpdateBulkerCrewLog(thirdPartyCrewLog);

            //2.Check if test data meet preconditions
            Assert.AreNotEqual(crewId, 0);
            Assert.NotNull(productHaulLoad);
            Assert.NotNull(productHaul);
            Assert.NotNull(shippingLoadSheet);
            Assert.NotNull(rigJobSanjelCrewSection);
            Assert.NotNull(schedule);
            Assert.Greater(schedule.Id, 0);
            globalSanjelScheduleId = schedule.Id;
            BulkerCrewLog currentbulkerCrewLog = eServiceOnlineGateway.Instance.GetBulkerCrewLog(crewId);
            Assert.AreEqual(BulkerCrewStatus.LoadRequested, currentbulkerCrewLog.CrewStatus);
            BulkerCrewLog currentbulkerCrew2Log = eServiceOnlineGateway.Instance.GetThirdPartyBulkerCrewLog(thridPartyBulkerCrew1Id);
            Assert.AreEqual(BulkerCrewStatus.OffDuty, currentbulkerCrew2Log.CrewStatus);

            //3.Execute the method to test
            ProductHaulProcess.ReleaseProductHaulCrew(productHaul);
            ThirdPartyBulkerCrewSchedule newScheduled = (ThirdPartyBulkerCrewSchedule)RigBoardProcess.CreateSchedule(thridPartyBulkerCrew1Id, true, productHaul.EstimatedLoadTime, rigJob.Id, false, DateTime.Now.AddHours(3), productHaul,  "IntegrationTest");
            CrewProcess.UpdateBulkerCrewStatus(thridPartyBulkerCrew1Id, true, loggedUser);
            int resultRigJobThirdPartyBulkerCrewSectionId = newScheduled.RigJobThirdPartyBulkerCrewSection.Id;
            int resultNewScheduledId = newScheduled.Id;

            //4. Verify the result meets post-conditions
            var resultProductHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productHaulLoad.Id);
            Assert.Greater(resultProductHaulLoad.Id, 0);
            Assert.AreEqual(resultProductHaulLoad.ProductHaulLoadLifeStatus, ProductHaulLoadStatus.Scheduled);
            //var resultProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaul.Id);
            //Assert.IsNull(resultProductHaul);
            var resultShippingLoadSheet = eServiceOnlineGateway.Instance.GetShippingLoadSheetById(shippingLoadSheet.Id);
            Assert.IsNotNull(resultShippingLoadSheet);
            var resultRigJobSanjelCrewSection = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(rigJobSanjelCrewSection.Id);
            Assert.IsNull(resultRigJobSanjelCrewSection);
            var resultSanjelCrewSchedule = eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(schedule.Id);
            Assert.IsNull(resultSanjelCrewSchedule);
            globalSanjelScheduleId = 0;
            var resultNewCrewSchedule = eServiceOnlineGateway.Instance.GetThirdPartyCrewScheduleById(newScheduled.Id);
            Assert.IsNotNull(resultNewCrewSchedule);
            globalThirdPartyScheduleId = resultNewCrewSchedule.Id;
            BulkerCrewLog resultBulkerCrewLog = eServiceOnlineGateway.Instance.GetBulkerCrewLog(crewId);
            Assert.AreEqual(BulkerCrewStatus.OffDuty, resultBulkerCrewLog.CrewStatus);
            BulkerCrewLog resultBulkerCrew2Log = eServiceOnlineGateway.Instance.GetThirdPartyBulkerCrewLog(thridPartyBulkerCrew1Id);
            Assert.AreEqual(BulkerCrewStatus.LoadRequested, resultBulkerCrew2Log.CrewStatus);

            //5. Clean up test data
            eServiceOnlineGateway.Instance.DeleteProductHaulLoad(resultProductHaulLoad);
            eServiceOnlineGateway.Instance.DeletethirdPartyBulkerCrewSchedule(newScheduled.Id);
            eServiceOnlineGateway.Instance.DeleteRigJobThirdPartyBulkerCrewSection(newScheduled.RigJobThirdPartyBulkerCrewSection.Id);
            eServiceOnlineGateway.Instance.DeleteProductHaul(newScheduled.RigJobThirdPartyBulkerCrewSection.ProductHaul);
            eServiceOnlineGateway.Instance.DeleteShippingLoadSheet(resultShippingLoadSheet);

            //6. Double check all data are cleaned
            var cleanedProductHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(resultProductHaulLoad.Id);
            Assert.IsNull(cleanedProductHaulLoad);
            var cleanedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaul.Id);
            Assert.IsNull(cleanedProductHaul);
            var cleanedShippingLoadSheet = eServiceOnlineGateway.Instance.GetShippingLoadSheetById(shippingLoadSheet.Id);
            Assert.IsNull(cleanedShippingLoadSheet);
            var cleanedRigJobThirdPartyCrewSection = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionById(resultRigJobThirdPartyBulkerCrewSectionId);
            Assert.IsNull(cleanedRigJobThirdPartyCrewSection);
            var cleanedThirdPartyCrewSchedule = eServiceOnlineGateway.Instance.GetThirdPartyCrewScheduleById(resultNewScheduledId);
            Assert.IsNull(cleanedThirdPartyCrewSchedule);
            globalThirdPartyScheduleId = 0;

        }
        [Test]
        public void TestRescheduleProductHaulThirdPartyCrewToSanjelCrew()
        {
            //1. Prepare test data
            int thridPartyBulkerCrew1Id = thridPartyBulkerCrew1.Id;
            int crew2Id = bulkerCrew1.Id;
            Assert.AreNotEqual(thridPartyBulkerCrew1Id, 0);
            Assert.AreNotEqual(crew2Id, 0);

            productHaulLoad = PrepareProductHaulLoad();
            productHaul = PrepareScheduledProductHaulFormThirdParty();
            shippingLoadSheet = PrepareShippingLoadSheet();
            rigJobThirdPartyBulkerCrewSection = PrepareRigJobThirdPartyBulkerCrewSection();
            schedule = (ThirdPartyBulkerCrewSchedule)PrepareThirdPartySchedule();
            // Crew1 LoadRequested
            thirdPartyCrewLog.CrewStatus = BulkerCrewStatus.LoadRequested;
            eServiceOnlineGateway.Instance.UpdateBulkerCrewLog(thirdPartyCrewLog);
            //Change Crew2 OffDuty
            bulkerCrewLog.CrewStatus = BulkerCrewStatus.OffDuty;
            eServiceOnlineGateway.Instance.UpdateBulkerCrewLog(bulkerCrewLog);

            //2.Check if test data meet preconditions
            Assert.NotNull(productHaulLoad);
            Assert.NotNull(productHaul);
            Assert.NotNull(shippingLoadSheet);
            Assert.NotNull(rigJobThirdPartyBulkerCrewSection);
            Assert.NotNull(schedule);
            Assert.Greater(schedule.Id, 0);
            globalThirdPartyScheduleId = schedule.Id;
            BulkerCrewLog currentbulkerCrewLog = eServiceOnlineGateway.Instance.GetThirdPartyBulkerCrewLog(thridPartyBulkerCrew1Id);
            Assert.AreEqual(BulkerCrewStatus.LoadRequested, currentbulkerCrewLog.CrewStatus);
            BulkerCrewLog currentbulkerCrew2Log = eServiceOnlineGateway.Instance.GetBulkerCrewLog(crew2Id);
            Assert.AreEqual(BulkerCrewStatus.OffDuty, currentbulkerCrew2Log.CrewStatus);

            //3.Execute the method to test
            ProductHaulProcess.ReleaseProductHaulCrew(productHaul);
            SanjelCrewSchedule newScheduled = (SanjelCrewSchedule)RigBoardProcess.CreateSchedule(crew2Id, false, productHaul.EstimatedLoadTime, rigJob.Id, false, DateTime.Now.AddHours(3), productHaul, "IntegrationTest");
            CrewProcess.UpdateBulkerCrewStatus(crew2Id, false, loggedUser);
            int newRigJobSanjelCrewSectionId = newScheduled.RigJobSanjelCrewSection.Id;
            int newScheduledId = newScheduled.Id;

            //4. Verify the result meets post-conditions
            var resultProductHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productHaulLoad.Id);
            Assert.Greater(resultProductHaulLoad.Id, 0);
            Assert.AreEqual(resultProductHaulLoad.ProductHaulLoadLifeStatus, ProductHaulLoadStatus.Scheduled);
            //var resultProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaul.Id);
            //Assert.IsNull(resultProductHaul);
            var resultShippingLoadSheet = eServiceOnlineGateway.Instance.GetShippingLoadSheetById(shippingLoadSheet.Id);
            Assert.IsNotNull(resultShippingLoadSheet);
            var resultRigJobThirdPartyCrewSection = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionById(rigJobThirdPartyBulkerCrewSection.Id);
            Assert.IsNull(resultRigJobThirdPartyCrewSection);
            var resultThirdPartyCrewSchedule = eServiceOnlineGateway.Instance.GetThirdPartyCrewScheduleById(schedule.Id);
            Assert.IsNull(resultThirdPartyCrewSchedule);
            globalThirdPartyScheduleId = 0;
            var resultNewCrewSchedule = eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(newScheduled.Id);
            Assert.IsNotNull(resultNewCrewSchedule);
            globalSanjelScheduleId = resultNewCrewSchedule.Id;
            BulkerCrewLog resultBulkerCrewLog = eServiceOnlineGateway.Instance.GetThirdPartyBulkerCrewLog(thridPartyBulkerCrew1Id);
            Assert.AreEqual(BulkerCrewStatus.OffDuty, resultBulkerCrewLog.CrewStatus);
            BulkerCrewLog resultBulkerCrew2Log = eServiceOnlineGateway.Instance.GetBulkerCrewLog(crew2Id);
            Assert.AreEqual(BulkerCrewStatus.LoadRequested, resultBulkerCrew2Log.CrewStatus);

            //5. Clean up test data
            eServiceOnlineGateway.Instance.DeleteProductHaulLoad(resultProductHaulLoad);
            eServiceOnlineGateway.Instance.DeleteCrewSchedule(newScheduled.Id);
            eServiceOnlineGateway.Instance.DeleteRigJobCrewSection(newScheduled.RigJobSanjelCrewSection);
            eServiceOnlineGateway.Instance.DeleteProductHaul(newScheduled.RigJobSanjelCrewSection.ProductHaul);
            eServiceOnlineGateway.Instance.DeleteShippingLoadSheet(resultShippingLoadSheet);

            //6. Double check all data are cleaned
            var cleanedProductHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(resultProductHaulLoad.Id);
            Assert.IsNull(cleanedProductHaulLoad);
            var cleanedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaul.Id);
            Assert.IsNull(cleanedProductHaul);
            var cleanedShippingLoadSheet = eServiceOnlineGateway.Instance.GetShippingLoadSheetById(shippingLoadSheet.Id);
            Assert.IsNull(cleanedShippingLoadSheet);
            var cleanedRigJobCrewSection = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(newRigJobSanjelCrewSectionId);
            Assert.IsNull(cleanedRigJobCrewSection);
            var cleanedSanjelCrewSchedule = eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(newScheduledId);
            Assert.IsNull(cleanedSanjelCrewSchedule);
            globalSanjelScheduleId = 0;

        }
        [Test]
        public void TestRescheduleProductHaulThirdPartyCrewToThirdPartyCrew()
        {
            int thridPartyBulkerCrew1Id = thridPartyBulkerCrew1.Id;
            //1. Prepare test data
            PrepareThirdPartyBulkerCrew2();
            PrepareThirdPartyCrew2Log();
            Assert.NotNull(thridPartyBulkerCrew2);
            Assert.NotNull(thirdPartyCrew2Log);
            int thridPartyBulkerCrew2Id = thridPartyBulkerCrew2.Id;

            productHaulLoad = PrepareProductHaulLoad();
            productHaul = PrepareScheduledProductHaulFormThirdParty();
            shippingLoadSheet = PrepareShippingLoadSheet();
            rigJobThirdPartyBulkerCrewSection = PrepareRigJobThirdPartyBulkerCrewSection();
            schedule = (ThirdPartyBulkerCrewSchedule)PrepareThirdPartySchedule();
            // Crew1 LoadRequested
            thirdPartyCrewLog.CrewStatus = BulkerCrewStatus.LoadRequested;
            eServiceOnlineGateway.Instance.UpdateBulkerCrewLog(thirdPartyCrewLog);
            //Change Crew2 OffDuty
            thirdPartyCrew2Log.CrewStatus = BulkerCrewStatus.OffDuty;
            eServiceOnlineGateway.Instance.UpdateBulkerCrewLog(thirdPartyCrew2Log);

            //2.Check if test data meet preconditions
            Assert.AreNotEqual(thridPartyBulkerCrew1Id, 0);
            Assert.NotNull(productHaulLoad);
            Assert.NotNull(productHaul);
            Assert.NotNull(shippingLoadSheet);
            Assert.NotNull(rigJobThirdPartyBulkerCrewSection);
            Assert.NotNull(schedule);
            Assert.Greater(schedule.Id, 0);
            globalThirdSanjelScheduleId = schedule.Id;
            BulkerCrewLog currentbulkerCrewLog = eServiceOnlineGateway.Instance.GetThirdPartyBulkerCrewLog(thridPartyBulkerCrew1Id);
            Assert.AreEqual(BulkerCrewStatus.LoadRequested, currentbulkerCrewLog.CrewStatus);
            BulkerCrewLog currentbulkerCrew2Log = eServiceOnlineGateway.Instance.GetThirdPartyBulkerCrewLog(thridPartyBulkerCrew2Id);
            Assert.AreEqual(BulkerCrewStatus.OffDuty, currentbulkerCrew2Log.CrewStatus);

            //3.Execute the method to test
            ProductHaulProcess.ReleaseProductHaulCrew(productHaul);
            ThirdPartyBulkerCrewSchedule newScheduled = (ThirdPartyBulkerCrewSchedule)RigBoardProcess.CreateSchedule(thridPartyBulkerCrew2Id, true, productHaul.EstimatedLoadTime, rigJob.Id, false, DateTime.Now.AddHours(3), productHaul, "IntegrationTest");
            CrewProcess.UpdateBulkerCrewStatus(thridPartyBulkerCrew2Id, true, loggedUser);
            int resultRigJobThirdPartyBulkerCrewSectionId = newScheduled.RigJobThirdPartyBulkerCrewSection.Id;
            int resultNewScheduledId = newScheduled.Id;

            //4. Verify the result meets post-conditions
            var resultProductHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productHaulLoad.Id);
            Assert.Greater(resultProductHaulLoad.Id, 0);
            Assert.AreEqual(resultProductHaulLoad.ProductHaulLoadLifeStatus, ProductHaulLoadStatus.Scheduled);
            //var resultProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaul.Id);
            //Assert.IsNull(resultProductHaul);
            var resultShippingLoadSheet = eServiceOnlineGateway.Instance.GetShippingLoadSheetById(shippingLoadSheet.Id);
            Assert.IsNotNull(resultShippingLoadSheet);
            var resultRigJobThirdPartyCrewSection = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionById(rigJobThirdPartyBulkerCrewSection.Id);
            Assert.IsNull(resultRigJobThirdPartyCrewSection);
            var resultThirdPartyCrewSchedule = eServiceOnlineGateway.Instance.GetThirdPartyCrewScheduleById(schedule.Id);
            Assert.IsNull(resultThirdPartyCrewSchedule);
            globalThirdSanjelScheduleId = 0;
            var resultNewCrewSchedule = eServiceOnlineGateway.Instance.GetThirdPartyCrewScheduleById(newScheduled.Id);
            Assert.IsNotNull(resultNewCrewSchedule);
            globalThirdPartyScheduleId2 = resultNewScheduledId;
            BulkerCrewLog resultBulkerCrewLog = eServiceOnlineGateway.Instance.GetThirdPartyBulkerCrewLog(thridPartyBulkerCrew1Id);
            Assert.AreEqual(BulkerCrewStatus.OffDuty, resultBulkerCrewLog.CrewStatus);
            BulkerCrewLog resultBulkerCrew2Log = eServiceOnlineGateway.Instance.GetThirdPartyBulkerCrewLog(thridPartyBulkerCrew2Id);
            Assert.AreEqual(BulkerCrewStatus.LoadRequested, resultBulkerCrew2Log.CrewStatus);

            //5. Clean up test data
            eServiceOnlineGateway.Instance.DeleteProductHaulLoad(resultProductHaulLoad);
            eServiceOnlineGateway.Instance.DeletethirdPartyBulkerCrewSchedule(newScheduled.Id);
            eServiceOnlineGateway.Instance.DeleteRigJobThirdPartyBulkerCrewSection(newScheduled.RigJobThirdPartyBulkerCrewSection.Id);
            eServiceOnlineGateway.Instance.DeleteProductHaul(newScheduled.RigJobThirdPartyBulkerCrewSection.ProductHaul);
            eServiceOnlineGateway.Instance.DeleteShippingLoadSheet(resultShippingLoadSheet);
            eServiceOnlineGateway.Instance.DeleteThirdPartyBulkerCrew(thridPartyBulkerCrew2Id);

            //6. Double check all data are cleaned
            var cleanedProductHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(resultProductHaulLoad.Id);
            Assert.IsNull(cleanedProductHaulLoad);
            var cleanedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaul.Id);
            Assert.IsNull(cleanedProductHaul);
            var cleanedShippingLoadSheet = eServiceOnlineGateway.Instance.GetShippingLoadSheetById(shippingLoadSheet.Id);
            Assert.IsNull(cleanedShippingLoadSheet);
            var cleanedRigJobThirdPartyCrewSection = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionById(resultRigJobThirdPartyBulkerCrewSectionId);
            Assert.IsNull(cleanedRigJobThirdPartyCrewSection);
            var cleanedThirdPartyCrewSchedule = eServiceOnlineGateway.Instance.GetThirdPartyCrewScheduleById(resultNewScheduledId);
            Assert.IsNull(cleanedThirdPartyCrewSchedule);
            globalThirdPartyScheduleId2 = 0;
            var cleanedThirdPartyCrew = eServiceOnlineGateway.Instance.GetThirdPartyBulkerCrewById(thridPartyBulkerCrew2Id);
            Assert.IsNull(cleanedThirdPartyCrew);

        }
        #endregion

        #region  ProductHaul OnLoaction
        [Test]
        public void TestProductHaulOnLoaction()
        {
            int crewId = bulkerCrew1.Id;
            //1. Prepare test data
            productHaulLoad = PrepareProductHaulLoad();
            productHaul = PrepareScheduledProductHaul();
            shippingLoadSheet = PrepareShippingLoadSheet();
            rigJobSanjelCrewSection = PrepareRigJobSanjelCrewSection();
            schedule = PrepareSchedule();
            // Crew1 Loaded
            bulkerCrewLog.CrewStatus = BulkerCrewStatus.Loaded;
            eServiceOnlineGateway.Instance.UpdateBulkerCrewLog(bulkerCrewLog);

            //2.Check if test data meet preconditions
            Assert.AreNotEqual(crewId, 0);
            Assert.NotNull(productHaulLoad);
            Assert.NotNull(productHaul);
            Assert.NotNull(shippingLoadSheet);
            Assert.NotNull(rigJobSanjelCrewSection);
            Assert.NotNull(schedule);
            BulkerCrewLog currentbulkerCrewLog = eServiceOnlineGateway.Instance.GetBulkerCrewLog(crewId);
            Assert.AreEqual(BulkerCrewStatus.Loaded, currentbulkerCrewLog.CrewStatus);

            //3.Execute the method to test
            ProductHaulProcess.SetProductHaulOnLocation(productHaul.Id, DateTime.Now, loggedUser);

            //4. Verify the result meets post-conditions
            var resultProductHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productHaulLoad.Id);
            Assert.Greater(resultProductHaulLoad.Id, 0);
            Assert.AreEqual(resultProductHaulLoad.ProductHaulLoadLifeStatus, ProductHaulLoadStatus.OnLocation);
            var resultProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaul.Id);
            Assert.AreEqual(ProductHaulStatus.OnLocation, resultProductHaul.ProductHaulLifeStatus);
            Assert.IsNotNull(resultProductHaul);
            var resultShippingLoadSheet = eServiceOnlineGateway.Instance.GetShippingLoadSheetById(shippingLoadSheet.Id);
            Assert.IsNotNull(resultShippingLoadSheet);
            var resultRigJobSanjelCrewSection = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(rigJobSanjelCrewSection.Id);
            Assert.IsNotNull(resultRigJobSanjelCrewSection);
            Assert.AreEqual(RigJobCrewSectionStatus.OnLocation, resultRigJobSanjelCrewSection.RigJobCrewSectionStatus);
            var resultSanjelCrewSchedule = eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(schedule.Id);
            Assert.IsNotNull(resultSanjelCrewSchedule);
            globalSanjelScheduleId = schedule.Id;
            BulkerCrewLog resultBulkerCrewLog = eServiceOnlineGateway.Instance.GetBulkerCrewLog(crewId);
            Assert.AreEqual(BulkerCrewStatus.OnLocation, resultBulkerCrewLog.CrewStatus);

            //5. Clean up test data
            eServiceOnlineGateway.Instance.DeleteProductHaulLoad(resultProductHaulLoad);
            eServiceOnlineGateway.Instance.DeleteProductHaul(resultProductHaul);
            eServiceOnlineGateway.Instance.DeleteShippingLoadSheet(resultShippingLoadSheet);
            eServiceOnlineGateway.Instance.DeleteRigJobCrewSection(resultRigJobSanjelCrewSection);
            eServiceOnlineGateway.Instance.DeleteCrewSchedule(resultSanjelCrewSchedule.Id);

            //6. Double check all data are cleaned
            var cleanedProductHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(resultProductHaulLoad.Id);
            Assert.IsNull(cleanedProductHaulLoad);
            var cleanedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaul.Id);
            Assert.IsNull(cleanedProductHaul);
            var cleanedShippingLoadSheet = eServiceOnlineGateway.Instance.GetShippingLoadSheetById(shippingLoadSheet.Id);
            Assert.IsNull(cleanedShippingLoadSheet);
            var cleanedRigJobSanjelCrewSection = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(rigJobSanjelCrewSection.Id);
            Assert.IsNull(cleanedRigJobSanjelCrewSection);
            var cleanedSanjelCrewSchedule = eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(schedule.Id);
            Assert.IsNull(cleanedSanjelCrewSchedule);
            globalSanjelScheduleId = 0;

        }

        [Test]
        public void TestThirdPartyProductHaulOnLoaction()
        {
            int crewId = thridPartyBulkerCrew1.Id;
            //1. Prepare test data
            productHaulLoad = PrepareProductHaulLoad();
            productHaul = PrepareScheduledProductHaulFormThirdParty();
            shippingLoadSheet = PrepareShippingLoadSheet();
            rigJobThirdPartyBulkerCrewSection = PrepareRigJobThirdPartyBulkerCrewSection();
            schedule = (ThirdPartyBulkerCrewSchedule)PrepareThirdPartySchedule();
            // Crew1 Loaded
            thirdPartyCrewLog.CrewStatus = BulkerCrewStatus.EnRoute;
            eServiceOnlineGateway.Instance.UpdateBulkerCrewLog(thirdPartyCrewLog);

            //2.Check if test data meet preconditions
            Assert.AreNotEqual(crewId, 0);
            Assert.NotNull(productHaulLoad);
            Assert.NotNull(productHaul);
            Assert.NotNull(shippingLoadSheet);
            Assert.NotNull(rigJobThirdPartyBulkerCrewSection);
            Assert.NotNull(schedule);
            BulkerCrewLog currentbulkerCrewLog = eServiceOnlineGateway.Instance.GetThirdPartyBulkerCrewLog(crewId);
            Assert.AreEqual(BulkerCrewStatus.EnRoute, currentbulkerCrewLog.CrewStatus);

            //3.Execute the method to test
            ProductHaulProcess.SetProductHaulOnLocation(productHaul.Id, DateTime.Now, loggedUser);

            //4. Verify the result meets post-conditions
            var resultProductHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productHaulLoad.Id);
            Assert.Greater(resultProductHaulLoad.Id, 0);
            Assert.AreEqual(resultProductHaulLoad.ProductHaulLoadLifeStatus, ProductHaulLoadStatus.OnLocation);
            var resultProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaul.Id);
            Assert.AreEqual(ProductHaulStatus.OnLocation, resultProductHaul.ProductHaulLifeStatus);
            Assert.IsNotNull(resultProductHaul);
            var resultShippingLoadSheet = eServiceOnlineGateway.Instance.GetShippingLoadSheetById(shippingLoadSheet.Id);
            Assert.IsNotNull(resultShippingLoadSheet);
            var resultRigJobThirdPartyCrewSection = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionById(rigJobThirdPartyBulkerCrewSection.Id);
            Assert.IsNotNull(resultRigJobThirdPartyCrewSection);
            Assert.AreEqual(RigJobCrewSectionStatus.OnLocation, resultRigJobThirdPartyCrewSection.RigJobCrewSectionStatus);
            var resultThirdPartyCrewSchedule = eServiceOnlineGateway.Instance.GetThirdPartyCrewScheduleById(schedule.Id);
            Assert.IsNotNull(resultThirdPartyCrewSchedule);
            globalThirdPartyScheduleId = schedule.Id;
            BulkerCrewLog resultBulkerCrewLog = eServiceOnlineGateway.Instance.GetThirdPartyBulkerCrewLog(crewId);
            Assert.AreEqual(BulkerCrewStatus.OnLocation, resultBulkerCrewLog.CrewStatus);

            //5. Clean up test data
            eServiceOnlineGateway.Instance.DeleteProductHaulLoad(resultProductHaulLoad);
            eServiceOnlineGateway.Instance.DeleteProductHaul(resultProductHaul);
            eServiceOnlineGateway.Instance.DeleteShippingLoadSheet(resultShippingLoadSheet);
            eServiceOnlineGateway.Instance.DeleteRigJobThirdPartyBulkerCrewSection(resultRigJobThirdPartyCrewSection.Id);
            eServiceOnlineGateway.Instance.DeletethirdPartyBulkerCrewSchedule(resultThirdPartyCrewSchedule.Id);

            //6. Double check all data are cleaned
            var cleanedProductHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(resultProductHaulLoad.Id);
            Assert.IsNull(cleanedProductHaulLoad);
            var cleanedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaul.Id);
            Assert.IsNull(cleanedProductHaul);
            var cleanedShippingLoadSheet = eServiceOnlineGateway.Instance.GetShippingLoadSheetById(shippingLoadSheet.Id);
            Assert.IsNull(cleanedShippingLoadSheet);
            var cleanedRigJobThirdPartyCrewSection = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionById(resultRigJobThirdPartyCrewSection.Id);
            Assert.IsNull(cleanedRigJobThirdPartyCrewSection);
            var cleanedThirdPartyCrewSchedule = eServiceOnlineGateway.Instance.GetThirdPartyCrewScheduleById(resultThirdPartyCrewSchedule.Id);
            Assert.IsNull(cleanedThirdPartyCrewSchedule);
            globalThirdPartyScheduleId = 0;
        }
        #endregion 

    }
}