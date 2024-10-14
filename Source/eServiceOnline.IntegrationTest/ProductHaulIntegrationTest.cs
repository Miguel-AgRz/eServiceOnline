using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eServiceOnline.BusinessProcess;
using eServiceOnline.Data;
using eServiceOnline.Gateway;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using Sanjel.Common.BusinessEntities.Standard.Products;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Crew;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ResourceSchedule;
using Sesi.SanjelData.Entities.Common.Model.Scheduling;
using Sesi.SanjelData.Services.Interfaces.BusinessEntities.Operation.Dispatch;

namespace eServiceOnline.IntegrationTest
{
    [TestFixture]
    public class ProductHaulIntegrationTest : ProductHaulTestBase
    {
        private static int globalProductHaulId = 0;
        private static int globalProductHaulId2 = 0;
        private static int globalProductHaulId3 = 0;
        private static int globalAssignmentId = 0;
        private static int globalAssignmentId2 = 0;
        private static int globalAssignmentId3 = 0;
        private static int globalScheduleId=0 ;
        private static int globalScheduleId2 =0;
        private static int globalScheduleId3 = 0;
        private static bool globalThirdParty = false;

        [TearDown]
        public static void Teardown()
        {
            if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
            {
                if (globalProductHaulId != 0)
                {
                    var updatedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(globalProductHaulId);
                    eServiceOnlineGateway.Instance.DeleteProductHaul(updatedProductHaul, true);
                }

                if (globalProductHaulId2 != 0)
                {
                    var updatedProductHaul =
                        eServiceOnlineGateway.Instance.GetProductHaulById(globalProductHaulId2);
                    eServiceOnlineGateway.Instance.DeleteProductHaul(updatedProductHaul, true);
                }

                if (globalProductHaulId3 != 0)
                {
                    var updatedProductHaul =
                        eServiceOnlineGateway.Instance.GetProductHaulById(globalProductHaulId3);
                    eServiceOnlineGateway.Instance.DeleteProductHaul(updatedProductHaul, true);
                }

                if (globalThirdParty)
                {
                    if (globalAssignmentId != 0)
                    {
                        var updatedAssignment =
                            eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionById(globalAssignmentId);
                        eServiceOnlineGateway.Instance.DeleteRigJobThirdPartyBulkerCrewSection(globalAssignmentId);
                    }

                    if (globalAssignmentId2 != 0)
                    {
                        var updatedAssignment =
                            eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionById(globalAssignmentId2);
                        eServiceOnlineGateway.Instance.DeleteRigJobThirdPartyBulkerCrewSection(globalAssignmentId2);
                    }

                    if (globalAssignmentId3 != 0)
                    {
                        var updatedAssignment =
                            eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionById(globalAssignmentId3);
                        eServiceOnlineGateway.Instance.DeleteRigJobThirdPartyBulkerCrewSection(globalAssignmentId3);
                    }

                    if (globalScheduleId != 0)
                    {
                        var updatedSanjelCrewSchedule =
                            eServiceOnlineGateway.Instance.GetThirdPartyCrewScheduleById(globalScheduleId);
                        eServiceOnlineGateway.Instance.DeletethirdPartyBulkerCrewSchedule(updatedSanjelCrewSchedule.Id);
                    }

                    if (globalScheduleId2 != 0)
                    {
                        var updatedSanjelCrewSchedule =
                            eServiceOnlineGateway.Instance.GetThirdPartyCrewScheduleById(globalScheduleId2);
                        eServiceOnlineGateway.Instance.DeletethirdPartyBulkerCrewSchedule(updatedSanjelCrewSchedule.Id);
                    }

                    if (globalScheduleId3 != 0)
                    {
                        var updatedSanjelCrewSchedule =
                            eServiceOnlineGateway.Instance.GetThirdPartyCrewScheduleById(globalScheduleId3);
                        eServiceOnlineGateway.Instance.DeletethirdPartyBulkerCrewSchedule(updatedSanjelCrewSchedule.Id);
                    }
                }
                else
                {
                    if (globalAssignmentId != 0)
                    {
                        var updatedAssignment =
                            eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(globalAssignmentId);
                        eServiceOnlineGateway.Instance.DeleteRigJobCrewSection(updatedAssignment);
                    }

                    if (globalAssignmentId2 != 0)
                    {
                        var updatedAssignment =
                            eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(globalAssignmentId2);
                        eServiceOnlineGateway.Instance.DeleteRigJobCrewSection(updatedAssignment);
                    }

                    if (globalAssignmentId3 != 0)
                    {
                        var updatedAssignment =
                            eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(globalAssignmentId3);
                        eServiceOnlineGateway.Instance.DeleteRigJobCrewSection(updatedAssignment);
                    }

                    if (globalScheduleId != 0)
                    {
                        var updatedSanjelCrewSchedule =
                            eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(globalScheduleId, true);
                        eServiceOnlineGateway.Instance.DeleteCrewSchedule(updatedSanjelCrewSchedule.Id, true);
                    }

                    if (globalScheduleId2 != 0)
                    {
                        var updatedSanjelCrewSchedule =
                            eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(globalScheduleId2, true);
                        eServiceOnlineGateway.Instance.DeleteCrewSchedule(updatedSanjelCrewSchedule.Id, true);
                    }

                    if (globalScheduleId3 != 0)
                    {
                        var updatedSanjelCrewSchedule =
                            eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(globalScheduleId3, true);
                        eServiceOnlineGateway.Instance.DeleteCrewSchedule(updatedSanjelCrewSchedule.Id, true);
                    }
                }
            }
        }


    #region Data Preparation
    private ProductHaul PrepareScheduledProductHaulForBulkerCrewStatusAPI(DateTime estimatedLoadTime,
	    bool isThirdParty = false)
        {
            ProductHaul productHaul = new ProductHaul()
            {
                Crew = bulkerCrew1,
                ProductHaulLifeStatus = ProductHaulStatus.Scheduled,
                EstimatedLoadTime = estimatedLoadTime,
                ExpectedOnLocationTime = estimatedLoadTime.AddHours(4),
                EstimatedTravelTime = 2,
                IsGoWithCrew = false,
                IsThirdParty = isThirdParty,
                ShippingLoadSheets = new List<ShippingLoadSheet>()
                {
                    new ShippingLoadSheet()
                    {
                        ShippingStatus = ShippingStatus.Scheduled
                    }
                }

            };

            eServiceOnlineGateway.Instance.CreateProductHaul(productHaul, true);

            return productHaul;
        }

        private ProductHaul PrepareSecondScheduledProductHaulForBulkerCrewStatusAPI(bool isThirdParty = false)
        {
            ProductHaul productHaul = new ProductHaul()
            {
                Crew = bulkerCrew1,
                ProductHaulLifeStatus = ProductHaulStatus.Scheduled,
                EstimatedLoadTime = DateTime.Now.AddHours(6),
                ExpectedOnLocationTime = DateTime.Now.AddHours(9),
                EstimatedTravelTime = 2,
                IsGoWithCrew = false,
                IsThirdParty = isThirdParty,
                ShippingLoadSheets = new List<ShippingLoadSheet>()
                {
                    new ShippingLoadSheet()
                    {
                        ShippingStatus = ShippingStatus.Scheduled
                    }
                }

            };

            eServiceOnlineGateway.Instance.CreateProductHaul(productHaul, true);

            return productHaul;
        }

        private ProductHaul PrepareThirdScheduledProductHaulForBulkerCrewStatusAPI(bool isThirdParty = false)
        {
            ProductHaul productHaul = new ProductHaul()
            {
                Crew = bulkerCrew1,
                ProductHaulLifeStatus = ProductHaulStatus.Scheduled,
                EstimatedLoadTime = DateTime.Now.AddHours(12),
                ExpectedOnLocationTime = DateTime.Now.AddHours(14),
                EstimatedTravelTime = 2,
                IsGoWithCrew = false,
                IsThirdParty = isThirdParty,
                ShippingLoadSheets = new List<ShippingLoadSheet>()
                {
                    new ShippingLoadSheet()
                    {
                        ShippingStatus = ShippingStatus.Scheduled
                    }
                }

            };

            eServiceOnlineGateway.Instance.CreateProductHaul(productHaul, true);

            return productHaul;
        }

        private (RigJobSanjelCrewSection, Schedule) AssignCrewToProductHaul(ProductHaul productHaul, SanjelCrew bulkerCrew, RigJob rigJob)
        {

            var rigJobSanjelCrewSection =
                ProductHaulProcess.CreateRigJobSanjelCrewSection(bulkerCrew, productHaul, rigJob, RigJobCrewSectionStatus.Scheduled,"IntegrationTest");
            var startTime = GetScheduleStartTime(productHaul, rigJob);
            var endTime = GetScheduleEndTime(productHaul, rigJob);
            Schedule schedule = RigBoardProcess.CreateSchedules(startTime, endTime,
                bulkerCrew, rigJob,
                productHaul, rigJobSanjelCrewSection, "IntegrationTest");

            return (rigJobSanjelCrewSection, schedule);
        }
        private RigJobSanjelCrewSection AssignCrewOtherStatus(RigJobSanjelCrewSection rigJobSanjelCrewSection, ProductHaulStatus productHaulStatus, RigJobCrewSectionStatus rigJobCrewSectionStatus)
        {
            rigJobSanjelCrewSection.ModifiedUserName = "TestAssignCrewOtherStatus";
            rigJobSanjelCrewSection.ProductHaul.ProductHaulLifeStatus = productHaulStatus;
            rigJobSanjelCrewSection.ProductHaul.ModifiedUserName = "TestAssignCrewOtherStatus";
            rigJobSanjelCrewSection.RigJobCrewSectionStatus = rigJobCrewSectionStatus;
            eServiceOnlineGateway.Instance.UpdateRigJobCrewSection(rigJobSanjelCrewSection);
            eServiceOnlineGateway.Instance.UpdateProductHaul(rigJobSanjelCrewSection.ProductHaul, false);
            return rigJobSanjelCrewSection;
        }

        private (RigJobThirdPartyBulkerCrewSection, Schedule) AssignThirdPartyCrewToProductHaul(ProductHaul productHaul, ThirdPartyBulkerCrew thirdPartyBulkerCrew, RigJob rigJob)
        {

            var rigJobThirdPartyBulkerCrewSection =
                RigBoardProcess.CreateRigJobThirdPartyBulkerCrewSection(thirdPartyBulkerCrew, productHaul, rigJob);
            var startTime = GetScheduleStartTime(productHaul, rigJob);
            var endTime = GetScheduleEndTime(productHaul, rigJob);
            Schedule schedule = RigBoardProcess.CreateThirdPartyBulkerCrewSchedule(startTime, endTime,
                thirdPartyBulkerCrew, rigJob,
                productHaul, rigJobThirdPartyBulkerCrewSection);

            return (rigJobThirdPartyBulkerCrewSection, schedule);
        }
        private RigJobThirdPartyBulkerCrewSection AssignThirdPartyCrewOtherStatus(RigJobThirdPartyBulkerCrewSection rigJobThirdPartyBulkerCrewSection, ProductHaulStatus productHaulStatus, RigJobCrewSectionStatus rigJobCrewSectionStatus)
        {
            rigJobThirdPartyBulkerCrewSection.ModifiedUserName = "TestAssignThirdPartyCrewOtherStatus";
            rigJobThirdPartyBulkerCrewSection.ProductHaul.ProductHaulLifeStatus = productHaulStatus;
            rigJobThirdPartyBulkerCrewSection.ProductHaul.ModifiedUserName = "TestAssignThirdPartyCrewOtherStatus";
            rigJobThirdPartyBulkerCrewSection.RigJobCrewSectionStatus = rigJobCrewSectionStatus;
            eServiceOnlineGateway.Instance.UpdateRigJobThirdPartyCrewSection(rigJobThirdPartyBulkerCrewSection);
            eServiceOnlineGateway.Instance.UpdateProductHaul(rigJobThirdPartyBulkerCrewSection.ProductHaul, false);
            return rigJobThirdPartyBulkerCrewSection;
        }

        private DateTime GetScheduleEndTime(ProductHaul productHaul, RigJob rigJob)
        {
            DateTime endTime;
            if (productHaul.IsGoWithCrew)
            {
                endTime = rigJob.JobDateTime.AddMinutes(rigJob.JobDuration == 0 ? 360 : rigJob.JobDuration);
            }
            else
            {
                endTime = productHaul.ExpectedOnLocationTime.AddHours(productHaul.EstimatedTravelTime);
            }

            return endTime;
        }

        private DateTime GetScheduleStartTime(ProductHaul productHaul, RigJob rigJob)
        {
            DateTime startTime;
            if (productHaul.IsGoWithCrew)
            {
                startTime = rigJob.CallCrewTime;
            }
            else
            {
                startTime = productHaul.ExpectedOnLocationTime.AddHours((-1) * productHaul.EstimatedTravelTime);
            }

            return startTime;
        }

        #endregion Data Preparation

        


        #region  Sanjel Crew Test
        [Test]
        public void TestSetBulkerCrewStatusOnLocationFromScheduled()
        {
            //1. Prepare test data
            //Product Haul is in progress
            ProductHaul productHaul = PrepareScheduledProductHaulForBulkerCrewStatusAPI(DateTime.Now.AddHours(1));
            int productHaulId = productHaul.Id;
            Assert.IsNotNull(productHaul);
            Assert.IsNotNull(productHaul.ShippingLoadSheets);
            Assert.Greater(productHaul.ShippingLoadSheets.Count, 0);
            RigJob rigJob = new RigJob();
            Assert.IsNotNull(rigJob);
            (RigJobSanjelCrewSection assignment, Schedule schedule) = AssignCrewToProductHaul(productHaul, bulkerCrew1, rigJob);
            int assignmentId = assignment.Id;
            Assert.IsNotNull(assignment);
            Assert.IsNotNull(schedule);
            SanjelCrewSchedule sanjelCrewSchedule = (SanjelCrewSchedule)schedule;
            Assert.IsNotNull(sanjelCrewSchedule);
            int sanjelCrewScheduleId = sanjelCrewSchedule.Id;
            Assert.IsNotNull(sanjelCrewSchedule.UnitSchedule);
            Assert.Greater(sanjelCrewSchedule.UnitSchedule.Count, 0);
            Assert.IsNotNull(sanjelCrewSchedule.WorkerSchedule);
            Assert.Greater(sanjelCrewSchedule.WorkerSchedule.Count, 0);

            //2.Check if test data meet preconditions
            Assert.AreEqual(ProductHaulStatus.Scheduled, productHaul.ProductHaulLifeStatus);
            Assert.AreEqual(assignment.RigJobCrewSectionStatus, RigJobCrewSectionStatus.Scheduled);
            foreach (var productHaulShippingLoadSheet in productHaul.ShippingLoadSheets)
            {
                Assert.AreEqual(ShippingStatus.Scheduled, productHaulShippingLoadSheet.ShippingStatus);
            }
            //3.Execute the method to test
            string result = eServiceWebContext.Instance.SetBulkerCrewStatusByCrewId(assignment.SanjelCrew.Id, BulkerCrewStatus.OnLocation, false, "TestSetBulkerCrewStatusOnLocationFromScheduled");
            //4. Verify the result meets post-conditions
            Assert.AreEqual(result, "Current assignment status is not LOADED or ENROUTE");
            var updatedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);
            Assert.IsNotNull(updatedProductHaul);
            Assert.IsNotNull(updatedProductHaul.ShippingLoadSheets);
            Assert.Greater(updatedProductHaul.ShippingLoadSheets.Count, 0);

            Assert.AreEqual(ProductHaulStatus.Scheduled, updatedProductHaul.ProductHaulLifeStatus);
            foreach (var productHaulShippingLoadSheet in updatedProductHaul.ShippingLoadSheets)
            {
                Assert.AreEqual(ShippingStatus.Scheduled, productHaulShippingLoadSheet.ShippingStatus);
            }


            var updatedAssignment = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(assignmentId);
            Assert.AreEqual(updatedAssignment.Id, assignment.Id);
            Assert.AreEqual(RigJobCrewSectionStatus.Scheduled, updatedAssignment.RigJobCrewSectionStatus);

            var updatedSanjelCrewSchedule =
                eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(sanjelCrewScheduleId, true);
            Assert.IsNotNull(updatedSanjelCrewSchedule);
            Assert.IsNotNull(updatedSanjelCrewSchedule.UnitSchedule);
            Assert.Greater(updatedSanjelCrewSchedule.UnitSchedule.Count, 0);
            Assert.IsNotNull(updatedSanjelCrewSchedule.WorkerSchedule);
            Assert.Greater(updatedSanjelCrewSchedule.WorkerSchedule.Count, 0);

            //5. Clean up test data
            eServiceOnlineGateway.Instance.DeleteCrewSchedule(updatedSanjelCrewSchedule.Id, true);
            eServiceOnlineGateway.Instance.DeleteRigJobCrewSection(updatedAssignment);
            eServiceOnlineGateway.Instance.DeleteProductHaul(updatedProductHaul, true);

            //6. Double check all data are cleaned
            var cleanedSanjelCrewSchedule =
                eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(sanjelCrewScheduleId, true);
            Assert.IsNull(cleanedSanjelCrewSchedule);
            var cleanedAssignment = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(assignmentId);
            Assert.IsNull(cleanedAssignment);
            var cleanedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);
            Assert.IsNull(cleanedProductHaul);
        }

        [Test]

        public void TestSetBulkerCrewStatusOnWayInFromScheduled()
        {
            //1. Prepare test data
            //Product Haul is in progress
            ProductHaul productHaul = PrepareScheduledProductHaulForBulkerCrewStatusAPI(DateTime.Now.AddHours(1));
            int productHaulId = productHaul.Id;
            Assert.IsNotNull(productHaul);
            Assert.IsNotNull(productHaul.ShippingLoadSheets);
            Assert.Greater(productHaul.ShippingLoadSheets.Count, 0);
            RigJob rigJob = new RigJob();
            Assert.IsNotNull(rigJob);
            (RigJobSanjelCrewSection assignment, Schedule schedule) = AssignCrewToProductHaul(productHaul, bulkerCrew1, rigJob);
            int assignmentId = assignment.Id;
            Assert.IsNotNull(assignment);
            Assert.IsNotNull(schedule);
            SanjelCrewSchedule sanjelCrewSchedule = (SanjelCrewSchedule)schedule;
            Assert.IsNotNull(sanjelCrewSchedule);
            int sanjelCrewScheduleId = sanjelCrewSchedule.Id;
            Assert.IsNotNull(sanjelCrewSchedule.UnitSchedule);
            Assert.Greater(sanjelCrewSchedule.UnitSchedule.Count, 0);
            Assert.IsNotNull(sanjelCrewSchedule.WorkerSchedule);
            Assert.Greater(sanjelCrewSchedule.WorkerSchedule.Count, 0);

            //2.Check if test data meet preconditions
            Assert.AreEqual(ProductHaulStatus.Scheduled, productHaul.ProductHaulLifeStatus);
            Assert.AreEqual(assignment.RigJobCrewSectionStatus, RigJobCrewSectionStatus.Scheduled);
            foreach (var productHaulShippingLoadSheet in productHaul.ShippingLoadSheets)
            {
                Assert.AreEqual(ShippingStatus.Scheduled, productHaulShippingLoadSheet.ShippingStatus);
            }
            //3.Execute the method to test
            string result = eServiceWebContext.Instance.SetBulkerCrewStatusByCrewId(assignment.SanjelCrew.Id, BulkerCrewStatus.OnWayIn, false, "TestSetBulkerCrewStatusOnWayInFromScheduled");
            //4. Verify the result meets post-conditions
            Assert.AreEqual("Current assignment status is not LOADED or ENROUTE or ON LOCATION", result);
            var updatedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);
            Assert.IsNotNull(updatedProductHaul);
            Assert.IsNotNull(updatedProductHaul.ShippingLoadSheets);
            Assert.Greater(updatedProductHaul.ShippingLoadSheets.Count, 0);

            Assert.AreEqual(ProductHaulStatus.Scheduled, updatedProductHaul.ProductHaulLifeStatus);
            foreach (var productHaulShippingLoadSheet in updatedProductHaul.ShippingLoadSheets)
            {
                Assert.AreEqual(ShippingStatus.Scheduled, productHaulShippingLoadSheet.ShippingStatus);
            }


            var updatedAssignment = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(assignmentId);
            Assert.AreEqual(updatedAssignment.Id, assignment.Id);
            Assert.AreEqual(RigJobCrewSectionStatus.Scheduled, updatedAssignment.RigJobCrewSectionStatus);

            var updatedSanjelCrewSchedule =
                eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(sanjelCrewScheduleId, true);
            Assert.IsNotNull(updatedSanjelCrewSchedule);
            Assert.IsNotNull(updatedSanjelCrewSchedule.UnitSchedule);
            Assert.Greater(updatedSanjelCrewSchedule.UnitSchedule.Count, 0);
            Assert.IsNotNull(updatedSanjelCrewSchedule.WorkerSchedule);
            Assert.Greater(updatedSanjelCrewSchedule.WorkerSchedule.Count, 0);

            //5. Clean up test data
            eServiceOnlineGateway.Instance.DeleteCrewSchedule(updatedSanjelCrewSchedule.Id, true);
            eServiceOnlineGateway.Instance.DeleteRigJobCrewSection(updatedAssignment);
            eServiceOnlineGateway.Instance.DeleteProductHaul(updatedProductHaul, true);

            //6. Double check all data are cleaned
            var cleanedSanjelCrewSchedule =
                eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(sanjelCrewScheduleId, true);
            Assert.IsNull(cleanedSanjelCrewSchedule);
            var cleanedAssignment = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(assignmentId);
            Assert.IsNull(cleanedAssignment);
            var cleanedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);
            Assert.IsNull(cleanedProductHaul);
        }

        [Test]
        public void TestSetBulkerCrewStatusCalled()
        {
            //1. Prepare test data
            //Product Haul is scheduled
            ProductHaul productHaul = PrepareScheduledProductHaulForBulkerCrewStatusAPI(DateTime.Now.AddHours(1));
            int productHaulId = productHaul.Id;
            Assert.IsNotNull(productHaul);
            RigJob rigJob = new RigJob();
            Assert.IsNotNull(rigJob);
            (RigJobSanjelCrewSection assignment, Schedule schedule) = AssignCrewToProductHaul(productHaul, bulkerCrew1, rigJob);
            int assignmentId = assignment.Id;
            Assert.IsNotNull(assignment);
            Assert.IsNotNull(schedule);
            SanjelCrewSchedule sanjelCrewSchedule = (SanjelCrewSchedule)schedule;
            Assert.IsNotNull(sanjelCrewSchedule);
            int sanjelCrewScheduleId = sanjelCrewSchedule.Id;
            Assert.IsNotNull(sanjelCrewSchedule.UnitSchedule);
            Assert.Greater(sanjelCrewSchedule.UnitSchedule.Count, 0);
            Assert.IsNotNull(sanjelCrewSchedule.WorkerSchedule);
            Assert.Greater(sanjelCrewSchedule.WorkerSchedule.Count, 0);

            //2.Check if test data meet preconditions
            Assert.AreEqual(ProductHaulStatus.Scheduled, productHaul.ProductHaulLifeStatus);
            Assert.AreEqual(RigJobCrewSectionStatus.Scheduled, assignment.RigJobCrewSectionStatus);

            //3.Execute the method to test
            string result = eServiceWebContext.Instance.SetBulkerCrewStatusByCrewId(assignment.SanjelCrew.Id, BulkerCrewStatus.Called, false, "TestSetBulkerCrewStatusCalled");

            //4. Verify the result meets post-conditions
            Assert.AreEqual(result, "Succeed");
            var updatedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);
            Assert.IsNotNull(updatedProductHaul);
            Assert.IsNotNull(updatedProductHaul.ShippingLoadSheets);
            Assert.Greater(updatedProductHaul.ShippingLoadSheets.Count, 0);

            Assert.AreEqual(ProductHaulStatus.Scheduled, updatedProductHaul.ProductHaulLifeStatus);


            var updatedAssignment = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(assignmentId);
            Assert.AreEqual(updatedAssignment.Id, assignment.Id);
           // Assert.AreEqual(RigJobCrewSectionStatus.Scheduled, updatedAssignment.RigJobCrewSectionStatus);
            Assert.AreEqual(RigJobCrewSectionStatus.Called, updatedAssignment.RigJobCrewSectionStatus);

            var updatedSanjelCrewSchedule =
                eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(sanjelCrewScheduleId, true);
            Assert.IsNotNull(updatedSanjelCrewSchedule);
            Assert.IsNotNull(updatedSanjelCrewSchedule.UnitSchedule);
            Assert.Greater(updatedSanjelCrewSchedule.UnitSchedule.Count, 0);
            Assert.IsNotNull(updatedSanjelCrewSchedule.WorkerSchedule);
            Assert.Greater(updatedSanjelCrewSchedule.WorkerSchedule.Count, 0);

            //5. Clean up test data
            eServiceOnlineGateway.Instance.DeleteCrewSchedule(updatedSanjelCrewSchedule.Id, true);
            eServiceOnlineGateway.Instance.DeleteRigJobCrewSection(updatedAssignment);
            eServiceOnlineGateway.Instance.DeleteProductHaul(updatedProductHaul, true);

            //6. Double check all data are cleaned
            var cleanedSanjelCrewSchedule =
                eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(sanjelCrewScheduleId, true);
            Assert.IsNull(cleanedSanjelCrewSchedule);
            var cleanedAssignment = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(assignmentId);
            Assert.IsNull(cleanedAssignment);
            var cleanedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);
            Assert.IsNull(cleanedProductHaul);

        }
        [Test]
        public void TestSetBulkerCrewStatusLoading()
        {
            //1. Prepare test data
            //Product Haul is scheduled
            ProductHaul productHaul = PrepareScheduledProductHaulForBulkerCrewStatusAPI(DateTime.Now.AddHours(1));
            int productHaulId = productHaul.Id;
            globalProductHaulId = productHaulId;
            Assert.IsNotNull(productHaul);
            RigJob rigJob = new RigJob();
            Assert.IsNotNull(rigJob);
            (RigJobSanjelCrewSection assignment, Schedule schedule) = AssignCrewToProductHaul(productHaul, bulkerCrew1, rigJob);
            int assignmentId = assignment.Id;
            globalAssignmentId = assignmentId;
            Assert.IsNotNull(assignment);
            Assert.IsNotNull(schedule);
            SanjelCrewSchedule sanjelCrewSchedule = (SanjelCrewSchedule)schedule;
            Assert.IsNotNull(sanjelCrewSchedule);

            int sanjelCrewScheduleId = sanjelCrewSchedule.Id;
            globalScheduleId = sanjelCrewScheduleId;
            Assert.IsNotNull(sanjelCrewSchedule.UnitSchedule);
            Assert.Greater(sanjelCrewSchedule.UnitSchedule.Count, 0);
            Assert.IsNotNull(sanjelCrewSchedule.WorkerSchedule);
            Assert.Greater(sanjelCrewSchedule.WorkerSchedule.Count, 0);
            //1.1 Set crew as called
            assignment = AssignCrewOtherStatus(assignment, ProductHaulStatus.Scheduled, RigJobCrewSectionStatus.Called);


            //2.Check if test data meet preconditions
            assignment = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(assignmentId);
            Assert.AreEqual(ProductHaulStatus.Scheduled, productHaul.ProductHaulLifeStatus);
            Assert.AreEqual(RigJobCrewSectionStatus.Called, assignment.RigJobCrewSectionStatus);

            //3.Execute the method to test
            string result = eServiceWebContext.Instance.SetBulkerCrewStatusByCrewId(assignment.SanjelCrew.Id, BulkerCrewStatus.Loading, false, "TestSetBulkerCrewStatusLoading");

            //4. Verify the result meets post-conditions
            Assert.AreEqual(result, "Succeed");
            var updatedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);
            Assert.IsNotNull(updatedProductHaul);
            Assert.IsNotNull(updatedProductHaul.ShippingLoadSheets);
            Assert.Greater(updatedProductHaul.ShippingLoadSheets.Count, 0);

            Assert.AreEqual(ProductHaulStatus.Scheduled, updatedProductHaul.ProductHaulLifeStatus);


            var updatedAssignment = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(assignmentId);
            Assert.AreEqual(updatedAssignment.Id, assignment.Id);
            Assert.AreEqual(RigJobCrewSectionStatus.Loading, updatedAssignment.RigJobCrewSectionStatus);

            var updatedSanjelCrewSchedule =
                eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(sanjelCrewScheduleId, true);
            Assert.IsNotNull(updatedSanjelCrewSchedule);
            Assert.IsNotNull(updatedSanjelCrewSchedule.UnitSchedule);
            Assert.Greater(updatedSanjelCrewSchedule.UnitSchedule.Count, 0);
            Assert.IsNotNull(updatedSanjelCrewSchedule.WorkerSchedule);
            Assert.Greater(updatedSanjelCrewSchedule.WorkerSchedule.Count, 0);

            //5. Clean up test data
            eServiceOnlineGateway.Instance.DeleteCrewSchedule(updatedSanjelCrewSchedule.Id, true);
            eServiceOnlineGateway.Instance.DeleteRigJobCrewSection(updatedAssignment);
            eServiceOnlineGateway.Instance.DeleteProductHaul(updatedProductHaul, true);

            //6. Double check all data are cleaned
            var cleanedSanjelCrewSchedule =
                eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(sanjelCrewScheduleId, true);
            Assert.IsNull(cleanedSanjelCrewSchedule);
            var cleanedAssignment = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(assignmentId);
            Assert.IsNull(cleanedAssignment);
            var cleanedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);
            Assert.IsNull(cleanedProductHaul);
        }
        [Test]
        public void TestSetBulkerCrewStatusLoaded()
        {
            TestSetBulkerCrewStatusLoaded(ProductHaulStatus.Scheduled, RigJobCrewSectionStatus.Called);
            TestSetBulkerCrewStatusLoaded(ProductHaulStatus.Scheduled, RigJobCrewSectionStatus.Loading);
        }

        private void TestSetBulkerCrewStatusLoaded(ProductHaulStatus productHaulStatus, RigJobCrewSectionStatus rigJobCrewSectionStatus)
        {
            //1. Prepare test data
            //Product Haul is in progress
            ProductHaul productHaul = PrepareScheduledProductHaulForBulkerCrewStatusAPI(DateTime.Now.AddHours(1));
            int productHaulId = productHaul.Id;
            globalProductHaulId = productHaulId;
            Assert.IsNotNull(productHaul);
            RigJob rigJob = new RigJob();
            Assert.IsNotNull(rigJob);
            (RigJobSanjelCrewSection assignment, Schedule schedule) = AssignCrewToProductHaul(productHaul, bulkerCrew1, rigJob);
            int assignmentId = assignment.Id;
            globalAssignmentId = assignmentId;
            Assert.IsNotNull(assignment);
            Assert.IsNotNull(schedule);
            SanjelCrewSchedule sanjelCrewSchedule = (SanjelCrewSchedule)schedule;
            Assert.IsNotNull(sanjelCrewSchedule);
            if (rigJobCrewSectionStatus != RigJobCrewSectionStatus.Scheduled || productHaulStatus != ProductHaulStatus.Scheduled)
            {
                assignment = AssignCrewOtherStatus(assignment, productHaulStatus, rigJobCrewSectionStatus);
            }
            int sanjelCrewScheduleId = sanjelCrewSchedule.Id;
            globalScheduleId = sanjelCrewScheduleId;
            Assert.IsNotNull(sanjelCrewSchedule.UnitSchedule);
            Assert.Greater(sanjelCrewSchedule.UnitSchedule.Count, 0);
            Assert.IsNotNull(sanjelCrewSchedule.WorkerSchedule);
            Assert.Greater(sanjelCrewSchedule.WorkerSchedule.Count, 0);

            //2.Check if test data meet preconditions
            Assert.AreEqual(productHaulStatus, productHaul.ProductHaulLifeStatus);
            Assert.AreEqual(rigJobCrewSectionStatus, assignment.RigJobCrewSectionStatus);

            //3.Execute the method to test
            string result = eServiceWebContext.Instance.SetBulkerCrewStatusByCrewId(assignment.SanjelCrew.Id, BulkerCrewStatus.Loaded, false, "TestSetBulkerCrewStatusLoaded");
            //4. Verify the result meets post-conditions
            Assert.AreEqual(result, "Succeed");
            var updatedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);
            Assert.IsNotNull(updatedProductHaul);
            Assert.IsNotNull(updatedProductHaul.ShippingLoadSheets);
            Assert.Greater(updatedProductHaul.ShippingLoadSheets.Count, 0);

            Assert.AreEqual(ProductHaulStatus.Scheduled, updatedProductHaul.ProductHaulLifeStatus);

            var updatedAssignment = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(assignmentId);
            Assert.AreEqual(updatedAssignment.Id, assignment.Id);
            Assert.AreEqual(RigJobCrewSectionStatus.Loaded, updatedAssignment.RigJobCrewSectionStatus);

            var updatedSanjelCrewSchedule =
                eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(sanjelCrewScheduleId, true);
            Assert.IsNotNull(updatedSanjelCrewSchedule);
            Assert.IsNotNull(updatedSanjelCrewSchedule.UnitSchedule);
            Assert.Greater(updatedSanjelCrewSchedule.UnitSchedule.Count, 0);
            Assert.IsNotNull(updatedSanjelCrewSchedule.WorkerSchedule);
            Assert.Greater(updatedSanjelCrewSchedule.WorkerSchedule.Count, 0);

            //5. Clean up test data
            eServiceOnlineGateway.Instance.DeleteCrewSchedule(updatedSanjelCrewSchedule.Id, true);
            eServiceOnlineGateway.Instance.DeleteRigJobCrewSection(updatedAssignment);
            eServiceOnlineGateway.Instance.DeleteProductHaul(updatedProductHaul, true);

            //6. Double check all data are cleaned
            var cleanedSanjelCrewSchedule =
                eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(sanjelCrewScheduleId, true);
            Assert.IsNull(cleanedSanjelCrewSchedule);
            var cleanedAssignment = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(assignmentId);
            Assert.IsNull(cleanedAssignment);
            var cleanedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);
            Assert.IsNull(cleanedProductHaul);

        }

        [Test]
        public void TestSetBulkerCrewStatusEnRoute()
        {
            TestSetBulkerCrewStatusEnRoute(ProductHaulStatus.Scheduled, RigJobCrewSectionStatus.Loading);
            TestSetBulkerCrewStatusEnRoute(ProductHaulStatus.Scheduled, RigJobCrewSectionStatus.Loaded);
        }
        private void TestSetBulkerCrewStatusEnRoute(ProductHaulStatus productHaulStatus, RigJobCrewSectionStatus rigJobCrewSectionStatus)
        {
            //1. Prepare test data
            //Product Haul is in progress
            ProductHaul productHaul = PrepareScheduledProductHaulForBulkerCrewStatusAPI(DateTime.Now.AddHours(1));
            int productHaulId = productHaul.Id;
            globalProductHaulId = productHaulId;
            Assert.IsNotNull(productHaul);
            RigJob rigJob = new RigJob();
            Assert.IsNotNull(rigJob);
            (RigJobSanjelCrewSection assignment, Schedule schedule) = AssignCrewToProductHaul(productHaul, bulkerCrew1, rigJob);
            int assignmentId = assignment.Id;
            globalAssignmentId = assignmentId;
            Assert.IsNotNull(assignment);
            Assert.IsNotNull(schedule);
            SanjelCrewSchedule sanjelCrewSchedule = (SanjelCrewSchedule)schedule;
            Assert.IsNotNull(sanjelCrewSchedule);
            if (rigJobCrewSectionStatus != RigJobCrewSectionStatus.Scheduled || productHaulStatus != ProductHaulStatus.Scheduled)
            {
                assignment = AssignCrewOtherStatus(assignment, productHaulStatus, rigJobCrewSectionStatus);
            }
            int sanjelCrewScheduleId = sanjelCrewSchedule.Id;
            globalScheduleId = sanjelCrewScheduleId;
            Assert.IsNotNull(sanjelCrewSchedule.UnitSchedule);
            Assert.Greater(sanjelCrewSchedule.UnitSchedule.Count, 0);
            Assert.IsNotNull(sanjelCrewSchedule.WorkerSchedule);
            Assert.Greater(sanjelCrewSchedule.WorkerSchedule.Count, 0);

            //2.Check if test data meet preconditions
            Assert.AreEqual(productHaulStatus, productHaul.ProductHaulLifeStatus);
            Assert.AreEqual(rigJobCrewSectionStatus, assignment.RigJobCrewSectionStatus);

            //3.Execute the method to test
            string result = eServiceWebContext.Instance.SetBulkerCrewStatusByCrewId(assignment.SanjelCrew.Id, BulkerCrewStatus.EnRoute, false, "TestSetBulkerCrewStatusEnRoute");
            //4. Verify the result meets post-conditions
            if(rigJobCrewSectionStatus == RigJobCrewSectionStatus.Loading)
	            Assert.AreEqual(result, "Current assignment status is not LOADED");
            if (rigJobCrewSectionStatus == RigJobCrewSectionStatus.Loaded)
                Assert.AreEqual(result, "Succeed");
            var updatedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);
            Assert.IsNotNull(updatedProductHaul);
            Assert.IsNotNull(updatedProductHaul.ShippingLoadSheets);
            Assert.Greater(updatedProductHaul.ShippingLoadSheets.Count, 0);

            if (rigJobCrewSectionStatus == RigJobCrewSectionStatus.Loading)
	            Assert.AreEqual(ProductHaulStatus.Scheduled, updatedProductHaul.ProductHaulLifeStatus);
            if (rigJobCrewSectionStatus == RigJobCrewSectionStatus.Loaded)
	            Assert.AreEqual(ProductHaulStatus.InProgress, updatedProductHaul.ProductHaulLifeStatus);


            var updatedAssignment = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(assignmentId);
            Assert.AreEqual(updatedAssignment.Id, assignment.Id);
            if (rigJobCrewSectionStatus == RigJobCrewSectionStatus.Loading)
	            Assert.AreEqual(RigJobCrewSectionStatus.Loading, updatedAssignment.RigJobCrewSectionStatus);
            if (rigJobCrewSectionStatus == RigJobCrewSectionStatus.Loaded)
	            Assert.AreEqual(RigJobCrewSectionStatus.EnRoute, updatedAssignment.RigJobCrewSectionStatus);

            var updatedSanjelCrewSchedule =
                eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(sanjelCrewScheduleId, true);
            Assert.IsNotNull(updatedSanjelCrewSchedule);
            Assert.IsNotNull(updatedSanjelCrewSchedule.UnitSchedule);
            Assert.Greater(updatedSanjelCrewSchedule.UnitSchedule.Count, 0);
            Assert.IsNotNull(updatedSanjelCrewSchedule.WorkerSchedule);
            Assert.Greater(updatedSanjelCrewSchedule.WorkerSchedule.Count, 0);

            //5. Clean up test data
            eServiceOnlineGateway.Instance.DeleteCrewSchedule(updatedSanjelCrewSchedule.Id, true);
            eServiceOnlineGateway.Instance.DeleteRigJobCrewSection(updatedAssignment);
            eServiceOnlineGateway.Instance.DeleteProductHaul(updatedProductHaul, true);

            //6. Double check all data are cleaned
            var cleanedSanjelCrewSchedule =
                eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(sanjelCrewScheduleId, true);
            Assert.IsNull(cleanedSanjelCrewSchedule);
            var cleanedAssignment = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(assignmentId);
            Assert.IsNull(cleanedAssignment);
            var cleanedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);
            Assert.IsNull(cleanedProductHaul);

        }
        [Test]
        public void TestSetBulkerCrewStatusOnLocation()
        {
            #region case 3    
            TestSetBulkerCrewStatusOnLocation(ProductHaulStatus.Scheduled, RigJobCrewSectionStatus.Loaded);
            #endregion
            #region case 4   
            TestSetBulkerCrewStatusOnLocation(ProductHaulStatus.InProgress, RigJobCrewSectionStatus.EnRoute);
            #endregion
        }

        private void TestSetBulkerCrewStatusOnLocation(ProductHaulStatus productHaulStatus, RigJobCrewSectionStatus rigJobCrewSectionStatus)
        {
            //1. Prepare test data
            //Product Haul is in progress
            ProductHaul productHaul = PrepareScheduledProductHaulForBulkerCrewStatusAPI(DateTime.Now.AddHours(1));
            int productHaulId = productHaul.Id;
            globalProductHaulId = productHaulId;
            Assert.IsNotNull(productHaul);
            Assert.IsNotNull(productHaul.ShippingLoadSheets);
            Assert.Greater(productHaul.ShippingLoadSheets.Count, 0);
            RigJob rigJob = new RigJob();
            Assert.IsNotNull(rigJob);
            (RigJobSanjelCrewSection assignment, Schedule schedule) = AssignCrewToProductHaul(productHaul, bulkerCrew1, rigJob);
            int assignmentId = assignment.Id;
            globalAssignmentId = assignmentId;
            Assert.IsNotNull(assignment);
            Assert.IsNotNull(schedule);
            SanjelCrewSchedule sanjelCrewSchedule = (SanjelCrewSchedule)schedule;
            Assert.IsNotNull(sanjelCrewSchedule);
            if (rigJobCrewSectionStatus != RigJobCrewSectionStatus.Scheduled || productHaulStatus != ProductHaulStatus.Scheduled)
            {
                assignment = AssignCrewOtherStatus(assignment, productHaulStatus, rigJobCrewSectionStatus);
            }
            int sanjelCrewScheduleId = sanjelCrewSchedule.Id;
            globalScheduleId = sanjelCrewScheduleId; 
            Assert.IsNotNull(sanjelCrewSchedule.UnitSchedule);
            Assert.Greater(sanjelCrewSchedule.UnitSchedule.Count, 0);
            Assert.IsNotNull(sanjelCrewSchedule.WorkerSchedule);
            Assert.Greater(sanjelCrewSchedule.WorkerSchedule.Count, 0);

            //2.Check if test data meet preconditions
            Assert.AreEqual(productHaulStatus, productHaul.ProductHaulLifeStatus);
            Assert.AreEqual(rigJobCrewSectionStatus, assignment.RigJobCrewSectionStatus);
            foreach (var productHaulShippingLoadSheet in productHaul.ShippingLoadSheets)
            {
                Assert.AreEqual(ShippingStatus.Scheduled, productHaulShippingLoadSheet.ShippingStatus);
            }
            //3.Execute the method to test
            string result = eServiceWebContext.Instance.SetBulkerCrewStatusByCrewId(assignment.SanjelCrew.Id, BulkerCrewStatus.OnLocation, false, "TestSetBulkerCrewStatusOnLocation");
            //4. Verify the result meets post-conditions
            Assert.AreEqual(result, "Succeed");
            var updatedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);
            Assert.IsNotNull(updatedProductHaul);
            Assert.IsNotNull(updatedProductHaul.ShippingLoadSheets);
            Assert.Greater(updatedProductHaul.ShippingLoadSheets.Count, 0);

            Assert.AreEqual(ProductHaulStatus.OnLocation, updatedProductHaul.ProductHaulLifeStatus);
            foreach (var productHaulShippingLoadSheet in updatedProductHaul.ShippingLoadSheets)
            {
                Assert.AreEqual(ShippingStatus.OnLocation, productHaulShippingLoadSheet.ShippingStatus);
            }


            var updatedAssignment = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(assignmentId);
            Assert.AreEqual(updatedAssignment.Id, assignment.Id);
            Assert.AreEqual(RigJobCrewSectionStatus.OnLocation, updatedAssignment.RigJobCrewSectionStatus);

            var updatedSanjelCrewSchedule =
                eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(sanjelCrewScheduleId, true);
            Assert.IsNotNull(updatedSanjelCrewSchedule);
            Assert.IsNotNull(updatedSanjelCrewSchedule.UnitSchedule);
            Assert.Greater(updatedSanjelCrewSchedule.UnitSchedule.Count, 0);
            Assert.IsNotNull(updatedSanjelCrewSchedule.WorkerSchedule);
            Assert.Greater(updatedSanjelCrewSchedule.WorkerSchedule.Count, 0);

            //5. Clean up test data
            eServiceOnlineGateway.Instance.DeleteCrewSchedule(updatedSanjelCrewSchedule.Id, true);
            eServiceOnlineGateway.Instance.DeleteRigJobCrewSection(updatedAssignment);
            eServiceOnlineGateway.Instance.DeleteProductHaul(updatedProductHaul, true);

            //6. Double check all data are cleaned
            var cleanedSanjelCrewSchedule =
                eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(sanjelCrewScheduleId, true);
            Assert.IsNull(cleanedSanjelCrewSchedule);
            var cleanedAssignment = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(assignmentId);
            Assert.IsNull(cleanedAssignment);
            var cleanedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);
            Assert.IsNull(cleanedProductHaul);
        }
        [Test]
        public void TestSetBulkerCrewStatusOnWayIn()
        {
            #region case 3    
            TestSetBulkerCrewStatusOnWayIn(ProductHaulStatus.Scheduled, RigJobCrewSectionStatus.Loaded);
            #endregion
            #region case 4   
            TestSetBulkerCrewStatusOnWayIn(ProductHaulStatus.InProgress, RigJobCrewSectionStatus.EnRoute);
            #endregion
            #region case 5    
            TestSetBulkerCrewStatusOnWayIn(ProductHaulStatus.OnLocation, RigJobCrewSectionStatus.OnLocation);
            #endregion

        }

        private void TestSetBulkerCrewStatusOnWayIn(ProductHaulStatus productHaulStatus, RigJobCrewSectionStatus rigJobCrewSectionStatus)
        {
            //1. Prepare test data
            //Product Haul is in progress
            ProductHaul productHaul = PrepareScheduledProductHaulForBulkerCrewStatusAPI(DateTime.Now.AddHours(1));
            int productHaulId = productHaul.Id;
            globalProductHaulId = productHaulId;
            Assert.IsNotNull(productHaul);
            Assert.IsNotNull(productHaul.ShippingLoadSheets);
            Assert.Greater(productHaul.ShippingLoadSheets.Count, 0);
            RigJob rigJob = new RigJob();
            Assert.IsNotNull(rigJob);
            (RigJobSanjelCrewSection assignment, Schedule schedule) = AssignCrewToProductHaul(productHaul, bulkerCrew1, rigJob);
            int assignmentId = assignment.Id;
            globalAssignmentId = assignmentId;
            Assert.IsNotNull(assignment);
            Assert.IsNotNull(schedule);
            SanjelCrewSchedule sanjelCrewSchedule = (SanjelCrewSchedule)schedule;
            Assert.IsNotNull(sanjelCrewSchedule);
            if (rigJobCrewSectionStatus != RigJobCrewSectionStatus.Scheduled || productHaulStatus != ProductHaulStatus.Scheduled)
            {
                assignment = AssignCrewOtherStatus(assignment, productHaulStatus, rigJobCrewSectionStatus);
            }
            int sanjelCrewScheduleId = sanjelCrewSchedule.Id;
            globalScheduleId = sanjelCrewScheduleId;
            Assert.IsNotNull(sanjelCrewSchedule.UnitSchedule);
            Assert.Greater(sanjelCrewSchedule.UnitSchedule.Count, 0);
            Assert.IsNotNull(sanjelCrewSchedule.WorkerSchedule);
            Assert.Greater(sanjelCrewSchedule.WorkerSchedule.Count, 0);

            //2.Check if test data meet preconditions
            Assert.AreEqual(productHaulStatus, productHaul.ProductHaulLifeStatus);
            Assert.AreEqual(rigJobCrewSectionStatus, assignment.RigJobCrewSectionStatus);
            foreach (var productHaulShippingLoadSheet in productHaul.ShippingLoadSheets)
            {
                Assert.AreEqual(ShippingStatus.Scheduled, productHaulShippingLoadSheet.ShippingStatus);
            }
            //3.Execute the method to test
            string result = eServiceWebContext.Instance.SetBulkerCrewStatusByCrewId(assignment.SanjelCrew.Id, BulkerCrewStatus.OnWayIn, false, "TestSetBulkerCrewStatusOnWayIn");
            //4. Verify the result meets post-conditions
            Assert.AreEqual(result, "Succeed");
            var updatedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);
            Assert.IsNotNull(updatedProductHaul);
            Assert.IsNotNull(updatedProductHaul.ShippingLoadSheets);
            Assert.Greater(updatedProductHaul.ShippingLoadSheets.Count, 0);

            Assert.AreEqual(ProductHaulStatus.OnLocation, updatedProductHaul.ProductHaulLifeStatus);
            if (productHaulStatus != ProductHaulStatus.OnLocation)
            {
                foreach (var productHaulShippingLoadSheet in updatedProductHaul.ShippingLoadSheets)
                {
                    Assert.AreEqual(ShippingStatus.OnLocation, productHaulShippingLoadSheet.ShippingStatus);
                }
            }

            var updatedAssignment = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(assignmentId);
            Assert.AreEqual(updatedAssignment.Id, assignment.Id);
            Assert.AreEqual(RigJobCrewSectionStatus.OnWayIn, updatedAssignment.RigJobCrewSectionStatus);

            var updatedSanjelCrewSchedule =
                eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(sanjelCrewScheduleId, true);
            Assert.IsNotNull(updatedSanjelCrewSchedule);
            Assert.IsNotNull(updatedSanjelCrewSchedule.UnitSchedule);
            Assert.Greater(updatedSanjelCrewSchedule.UnitSchedule.Count, 0);
            Assert.IsNotNull(updatedSanjelCrewSchedule.WorkerSchedule);
            Assert.Greater(updatedSanjelCrewSchedule.WorkerSchedule.Count, 0);

            //5. Clean up test data
            eServiceOnlineGateway.Instance.DeleteCrewSchedule(updatedSanjelCrewSchedule.Id, true);
            eServiceOnlineGateway.Instance.DeleteRigJobCrewSection(updatedAssignment);
            eServiceOnlineGateway.Instance.DeleteProductHaul(updatedProductHaul, true);

            //6. Double check all data are cleaned
            var cleanedSanjelCrewSchedule =
                eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(sanjelCrewScheduleId, true);
            Assert.IsNull(cleanedSanjelCrewSchedule);
            var cleanedAssignment = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(assignmentId);
            Assert.IsNull(cleanedAssignment);
            var cleanedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);
            Assert.IsNull(cleanedProductHaul);
        }
        [Test]
        public void TestSetBulkerCrewStatusReturned()
        {
            #region case 2    
            TestSetBulkerCrewStatusReturned(ProductHaulStatus.Scheduled, RigJobCrewSectionStatus.Loaded);
            #endregion
            #region case 3     
            TestSetBulkerCrewStatusReturned(ProductHaulStatus.InProgress, RigJobCrewSectionStatus.EnRoute);
            #endregion
            #region case 4      
            TestSetBulkerCrewStatusReturned(ProductHaulStatus.OnLocation, RigJobCrewSectionStatus.OnLocation);
            #endregion
            #region case 5  
            TestSetBulkerCrewStatusReturned(ProductHaulStatus.OnLocation, RigJobCrewSectionStatus.OnWayIn);
            #endregion
        }

        private void TestSetBulkerCrewStatusReturned(ProductHaulStatus productHaulStatus, RigJobCrewSectionStatus rigJobCrewSectionStatus)
        {
            //1. Prepare test data
            //Product Haul is in progress
            ProductHaul productHaul = PrepareScheduledProductHaulForBulkerCrewStatusAPI(DateTime.Now.AddHours(1));
            int productHaulId = productHaul.Id;
            globalProductHaulId = productHaulId;
            Assert.IsNotNull(productHaul);
            Assert.IsNotNull(productHaul.ShippingLoadSheets);
            Assert.Greater(productHaul.ShippingLoadSheets.Count, 0);
            RigJob rigJob = new RigJob();
            Assert.IsNotNull(rigJob);
            (RigJobSanjelCrewSection assignment, Schedule schedule) = AssignCrewToProductHaul(productHaul, bulkerCrew1, rigJob);
            int assignmentId = assignment.Id;
            globalAssignmentId = assignmentId;
            Assert.IsNotNull(assignment);
            Assert.IsNotNull(schedule);
            SanjelCrewSchedule sanjelCrewSchedule = (SanjelCrewSchedule)schedule;
            Assert.IsNotNull(sanjelCrewSchedule);
            if (rigJobCrewSectionStatus != RigJobCrewSectionStatus.Scheduled || productHaulStatus != ProductHaulStatus.Scheduled)
            {
                assignment = AssignCrewOtherStatus(assignment, productHaulStatus, rigJobCrewSectionStatus);
            }
            int sanjelCrewScheduleId = sanjelCrewSchedule.Id;
            globalScheduleId = sanjelCrewScheduleId;
            Assert.IsNotNull(sanjelCrewSchedule.UnitSchedule);
            Assert.Greater(sanjelCrewSchedule.UnitSchedule.Count, 0);
            Assert.IsNotNull(sanjelCrewSchedule.WorkerSchedule);
            Assert.Greater(sanjelCrewSchedule.WorkerSchedule.Count, 0);

            //2.Check if test data meet preconditions
            Assert.AreEqual(productHaulStatus, productHaul.ProductHaulLifeStatus);
            Assert.AreEqual(rigJobCrewSectionStatus, assignment.RigJobCrewSectionStatus);
            foreach (var productHaulShippingLoadSheet in productHaul.ShippingLoadSheets)
            {
                Assert.AreEqual(ShippingStatus.Scheduled, productHaulShippingLoadSheet.ShippingStatus);
            }
            //3.Execute the method to test
            string result = eServiceWebContext.Instance.SetBulkerCrewStatusByCrewId(assignment.SanjelCrew.Id, BulkerCrewStatus.Returned, false, "TestSetBulkerCrewStatusReturned");
            //4. Verify the result meets post-conditions
            Assert.AreEqual(result, "Succeed");
            ProductHaul updatedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);
            Assert.IsNotNull(updatedProductHaul);
            Assert.IsNotNull(updatedProductHaul.ShippingLoadSheets);
            Assert.Greater(updatedProductHaul.ShippingLoadSheets.Count, 0);

            Assert.AreEqual(ProductHaulStatus.Returned, updatedProductHaul.ProductHaulLifeStatus);
            foreach (var productHaulShippingLoadSheet in updatedProductHaul.ShippingLoadSheets)
            {
	            Assert.AreEqual(ShippingStatus.OnLocation, productHaulShippingLoadSheet.ShippingStatus);
            }

            var updatedAssignment = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(assignmentId);
            Assert.AreEqual(updatedAssignment.Id, assignment.Id);
            Assert.AreEqual(RigJobCrewSectionStatus.Returned, updatedAssignment.RigJobCrewSectionStatus);

            var updatedSanjelCrewSchedule =
                eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(sanjelCrewScheduleId, true);
            Assert.IsNotNull(updatedSanjelCrewSchedule);
            Assert.IsNotNull(updatedSanjelCrewSchedule.UnitSchedule);
            Assert.Greater(updatedSanjelCrewSchedule.UnitSchedule.Count, 0);
            Assert.IsNotNull(updatedSanjelCrewSchedule.WorkerSchedule);
            Assert.Greater(updatedSanjelCrewSchedule.WorkerSchedule.Count, 0);

            //5. Clean up test data
            eServiceOnlineGateway.Instance.DeleteCrewSchedule(updatedSanjelCrewSchedule.Id, true);
            eServiceOnlineGateway.Instance.DeleteRigJobCrewSection(updatedAssignment);
            eServiceOnlineGateway.Instance.DeleteProductHaul(updatedProductHaul, true);

            //6. Double check all data are cleaned
            var cleanedSanjelCrewSchedule =
                eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(sanjelCrewScheduleId, true);
            Assert.IsNull(cleanedSanjelCrewSchedule);
            var cleanedAssignment = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(assignmentId);
            Assert.IsNull(cleanedAssignment);
            var cleanedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);
            Assert.IsNull(cleanedProductHaul);
        }

        [Test]
        public void TestSetBulkerCrewStatusOffDuty()
        {
            //1. Prepare test data
            //Product Haul is in progress
            ProductHaul productHaul = PrepareScheduledProductHaulForBulkerCrewStatusAPI(DateTime.Now.AddHours(1));
            ProductHaul productHaul2 = PrepareScheduledProductHaulForBulkerCrewStatusAPI(DateTime.Now.AddHours(2));
            int productHaulId = productHaul.Id;
            globalProductHaulId = productHaulId;
            globalProductHaulId2 = productHaul2.Id;
            Assert.IsNotNull(productHaul);
            Assert.IsNotNull(productHaul.ShippingLoadSheets);
            Assert.Greater(productHaul.ShippingLoadSheets.Count, 0);
            RigJob rigJob = new RigJob();
            Assert.IsNotNull(rigJob);
            (RigJobSanjelCrewSection assignment, Schedule schedule) = AssignCrewToProductHaul(productHaul, bulkerCrew1, rigJob);
            (RigJobSanjelCrewSection assignment2, Schedule schedule2) = AssignCrewToProductHaul(productHaul2, bulkerCrew1, rigJob);
            int assignmentId = assignment.Id;
            globalAssignmentId = assignmentId;
            Assert.IsNotNull(assignment);
            Assert.IsNotNull(schedule);
            SanjelCrewSchedule sanjelCrewSchedule = (SanjelCrewSchedule)schedule;
            Assert.IsNotNull(sanjelCrewSchedule);
            int assignmentId2 = assignment.Id;
            globalAssignmentId2 = assignmentId2;
            Assert.IsNotNull(assignment2);
            Assert.IsNotNull(schedule2);
            SanjelCrewSchedule sanjelCrewSchedule2 = (SanjelCrewSchedule)schedule2;
            Assert.IsNotNull(sanjelCrewSchedule2);
            assignment = AssignCrewOtherStatus(assignment, ProductHaulStatus.OnLocation, RigJobCrewSectionStatus.Returned);
            int sanjelCrewScheduleId = sanjelCrewSchedule.Id;
            globalScheduleId = sanjelCrewScheduleId;
            globalScheduleId2 = schedule2.Id;
            Assert.IsNotNull(sanjelCrewSchedule.UnitSchedule);
            Assert.Greater(sanjelCrewSchedule.UnitSchedule.Count, 0);
            Assert.IsNotNull(sanjelCrewSchedule.WorkerSchedule);
            Assert.Greater(sanjelCrewSchedule.WorkerSchedule.Count, 0);

            //2.Check if test data meet preconditions
            Assert.AreEqual(ProductHaulStatus.OnLocation, productHaul.ProductHaulLifeStatus);
            Assert.AreEqual(RigJobCrewSectionStatus.Returned, assignment.RigJobCrewSectionStatus);
            Assert.AreEqual(ProductHaulStatus.Scheduled, productHaul2.ProductHaulLifeStatus);
            Assert.AreEqual(RigJobCrewSectionStatus.Scheduled, assignment2.RigJobCrewSectionStatus);
            //3.Execute the method to test
            string result = eServiceWebContext.Instance.SetBulkerCrewStatusByCrewId(assignment.SanjelCrew.Id, BulkerCrewStatus.OffDuty, false, "TestSetBulkerCrewStatusOffDuty");
            //4. Verify the result meets post-conditions
            Assert.AreEqual(result, "Succeed");
            var updatedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);
            Assert.IsNotNull(updatedProductHaul);
            Assert.IsNotNull(updatedProductHaul.ShippingLoadSheets);
            Assert.Greater(updatedProductHaul.ShippingLoadSheets.Count, 0);

            Assert.AreEqual(ProductHaulStatus.OnLocation, updatedProductHaul.ProductHaulLifeStatus);

            var updatedAssignment = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(assignmentId);
            Assert.AreEqual(updatedAssignment.Id, assignment.Id);

            var rigJobCrewOffDutySections = eServiceOnlineGateway.Instance.GetRigJobCrewSectionsByCrew(updatedAssignment.SanjelCrew.Id).Where(p => p.RigJobCrewSectionStatus == RigJobCrewSectionStatus.LogOffDuty);
            Assert.GreaterOrEqual(rigJobCrewOffDutySections.Count(), 1);

            var activedRigJobCrewSections = eServiceOnlineGateway.Instance.GetRigJobCrewSectionsByCrew(assignment.SanjelCrew.Id).Where(p => p.RigJobCrewSectionStatus  == RigJobCrewSectionStatus.Scheduled);
            Assert.GreaterOrEqual(activedRigJobCrewSections.Count(), 1);

            var bulkerCrewLog = eServiceOnlineGateway.Instance.GetBulkerCrewLog(assignment.SanjelCrew.Id);   
             Assert.AreEqual(BulkerCrewStatus.LoadRequested, bulkerCrewLog.CrewStatus);

            var updatedSanjelCrewSchedule =
                eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(sanjelCrewScheduleId, true);
            Assert.IsNotNull(updatedSanjelCrewSchedule);
            Assert.IsNotNull(updatedSanjelCrewSchedule.UnitSchedule);
            Assert.Greater(updatedSanjelCrewSchedule.UnitSchedule.Count, 0);
            Assert.IsNotNull(updatedSanjelCrewSchedule.WorkerSchedule);
            Assert.Greater(updatedSanjelCrewSchedule.WorkerSchedule.Count, 0);

            //5. Clean up test data
            eServiceOnlineGateway.Instance.DeleteCrewSchedule(updatedSanjelCrewSchedule.Id, true);
            eServiceOnlineGateway.Instance.DeleteRigJobCrewSection(updatedAssignment);
            eServiceOnlineGateway.Instance.DeleteProductHaul(updatedProductHaul, true);
            eServiceOnlineGateway.Instance.DeleteCrewSchedule(schedule2.Id, true);
            eServiceOnlineGateway.Instance.DeleteRigJobCrewSection(assignment2);
            eServiceOnlineGateway.Instance.DeleteProductHaul(productHaul2, true);

            //6. Double check all data are cleaned
            var cleanedSanjelCrewSchedule =
                eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(sanjelCrewScheduleId, true);
            Assert.IsNull(cleanedSanjelCrewSchedule);
            var cleanedAssignment = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(assignmentId);
            Assert.IsNull(cleanedAssignment);
            var cleanedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);
            Assert.IsNull(cleanedProductHaul);

            var cleanedSanjelCrewSchedule2 =
           eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(schedule2.Id, true);
            Assert.IsNull(cleanedSanjelCrewSchedule2);
            var cleanedAssignment2 = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(assignment2.Id);
            Assert.IsNull(cleanedAssignment2);
            var cleanedProductHaul2 = eServiceOnlineGateway.Instance.GetProductHaulById(productHaul2.Id);
            Assert.IsNull(cleanedProductHaul2);
        }
        [Test]
        public void TestSetBulkerCrewStatusOffDutyNoFutureAssignment()
        {
            //1. Prepare test data
            //Product Haul is in progress
            ProductHaul productHaul = PrepareScheduledProductHaulForBulkerCrewStatusAPI(DateTime.Now.AddHours(1));
            int productHaulId = productHaul.Id;
            Assert.IsNotNull(productHaul);
            Assert.IsNotNull(productHaul.ShippingLoadSheets);
            Assert.Greater(productHaul.ShippingLoadSheets.Count, 0);
            RigJob rigJob = new RigJob();
            Assert.IsNotNull(rigJob);
            (RigJobSanjelCrewSection assignment, Schedule schedule) = AssignCrewToProductHaul(productHaul, bulkerCrew1, rigJob);
            int assignmentId = assignment.Id;
            Assert.IsNotNull(assignment);
            Assert.IsNotNull(schedule);
            SanjelCrewSchedule sanjelCrewSchedule = (SanjelCrewSchedule)schedule;
            Assert.IsNotNull(sanjelCrewSchedule);
            assignment = AssignCrewOtherStatus(assignment, ProductHaulStatus.OnLocation, RigJobCrewSectionStatus.Returned);
            int sanjelCrewScheduleId = sanjelCrewSchedule.Id;
            Assert.IsNotNull(sanjelCrewSchedule.UnitSchedule);
            Assert.Greater(sanjelCrewSchedule.UnitSchedule.Count, 0);
            Assert.IsNotNull(sanjelCrewSchedule.WorkerSchedule);
            Assert.Greater(sanjelCrewSchedule.WorkerSchedule.Count, 0);

            //2.Check if test data meet preconditions
            Assert.AreEqual(ProductHaulStatus.OnLocation, productHaul.ProductHaulLifeStatus);
            Assert.AreEqual(assignment.RigJobCrewSectionStatus, RigJobCrewSectionStatus.Returned);
            //3.Execute the method to test
            string result = eServiceWebContext.Instance.SetBulkerCrewStatusByCrewId(assignment.SanjelCrew.Id, BulkerCrewStatus.OffDuty, false, "TestSetBulkerCrewStatusOffDuty");
            //4. Verify the result meets post-conditions
            Assert.AreEqual(result, "Succeed");
            var updatedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);
            Assert.IsNotNull(updatedProductHaul);
            Assert.IsNotNull(updatedProductHaul.ShippingLoadSheets);
            Assert.Greater(updatedProductHaul.ShippingLoadSheets.Count, 0);

            Assert.AreEqual(ProductHaulStatus.OnLocation, updatedProductHaul.ProductHaulLifeStatus);

            var updatedAssignment = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(assignmentId);
            Assert.AreEqual(updatedAssignment.Id, assignment.Id);

            var rigJobCrewOffDutySections = eServiceOnlineGateway.Instance.GetRigJobCrewSectionsByCrew(updatedAssignment.SanjelCrew.Id).Where(p => p.RigJobCrewSectionStatus == RigJobCrewSectionStatus.LogOffDuty);
            Assert.GreaterOrEqual(rigJobCrewOffDutySections.Count(), 1);

            var activedRigJobCrewSections = eServiceOnlineGateway.Instance.GetRigJobCrewSectionsByCrew(assignment.SanjelCrew.Id).Where(p => p.RigJobCrewSectionStatus != RigJobCrewSectionStatus.Removed && p.RigJobCrewSectionStatus != RigJobCrewSectionStatus.LogOffDuty);
            Assert.GreaterOrEqual(activedRigJobCrewSections.Count(), 0);

            var bulkerCrewLog = eServiceOnlineGateway.Instance.GetBulkerCrewLog(assignment.SanjelCrew.Id);
            Assert.AreEqual(BulkerCrewStatus.OffDuty, bulkerCrewLog.CrewStatus);

            var updatedSanjelCrewSchedule =
                eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(sanjelCrewScheduleId, true);
            Assert.IsNotNull(updatedSanjelCrewSchedule);
            Assert.IsNotNull(updatedSanjelCrewSchedule.UnitSchedule);
            Assert.Greater(updatedSanjelCrewSchedule.UnitSchedule.Count, 0);
            Assert.IsNotNull(updatedSanjelCrewSchedule.WorkerSchedule);
            Assert.Greater(updatedSanjelCrewSchedule.WorkerSchedule.Count, 0);

            //5. Clean up test data
            eServiceOnlineGateway.Instance.DeleteCrewSchedule(updatedSanjelCrewSchedule.Id, true);
            eServiceOnlineGateway.Instance.DeleteRigJobCrewSection(updatedAssignment);
            eServiceOnlineGateway.Instance.DeleteProductHaul(updatedProductHaul, true);

            //6. Double check all data are cleaned
            var cleanedSanjelCrewSchedule =
                eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(sanjelCrewScheduleId, true);
            Assert.IsNull(cleanedSanjelCrewSchedule);
            var cleanedAssignment = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(assignmentId);
            Assert.IsNull(cleanedAssignment);
            var cleanedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);
            Assert.IsNull(cleanedProductHaul);
        }
        [Test]
        public void TestSetBulkerCrewStatusDown()
        {
            //1. Prepare test data
            //Product Haul is in progress
            ProductHaul productHaul = PrepareScheduledProductHaulForBulkerCrewStatusAPI(DateTime.Now.AddHours(1));
            int productHaulId = productHaul.Id;
            globalProductHaulId = productHaulId;
            Assert.IsNotNull(productHaul);
            Assert.IsNotNull(productHaul.ShippingLoadSheets);
            Assert.Greater(productHaul.ShippingLoadSheets.Count, 0);
            RigJob rigJob = new RigJob();
            Assert.IsNotNull(rigJob);
            (RigJobSanjelCrewSection assignment, Schedule schedule) = AssignCrewToProductHaul(productHaul, bulkerCrew1, rigJob);
            int assignmentId = assignment.Id;
            globalAssignmentId = assignmentId;
            Assert.IsNotNull(assignment);
            Assert.IsNotNull(schedule);
            SanjelCrewSchedule sanjelCrewSchedule = (SanjelCrewSchedule)schedule;
            Assert.IsNotNull(sanjelCrewSchedule);
            assignment = AssignCrewOtherStatus(assignment, ProductHaulStatus.OnLocation, RigJobCrewSectionStatus.Returned);
            int sanjelCrewScheduleId = sanjelCrewSchedule.Id;
            globalScheduleId = sanjelCrewScheduleId;
            Assert.IsNotNull(sanjelCrewSchedule.UnitSchedule);
            Assert.Greater(sanjelCrewSchedule.UnitSchedule.Count, 0);
            Assert.IsNotNull(sanjelCrewSchedule.WorkerSchedule);
            Assert.Greater(sanjelCrewSchedule.WorkerSchedule.Count, 0);

            //2.Check if test data meet preconditions
            Assert.AreEqual(ProductHaulStatus.OnLocation, productHaul.ProductHaulLifeStatus);
            Assert.AreEqual(RigJobCrewSectionStatus.Returned, assignment.RigJobCrewSectionStatus);
            //3.Execute the method to test
            string result = eServiceWebContext.Instance.SetBulkerCrewStatusByCrewId(assignment.SanjelCrew.Id, BulkerCrewStatus.Down, false, "TestSetBulkerCrewStatusDown");
            //4. Verify the result meets post-conditions
            Assert.AreEqual(result, "Succeed");
            var updatedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);
            Assert.IsNotNull(updatedProductHaul);
            Assert.IsNotNull(updatedProductHaul.ShippingLoadSheets);
            Assert.Greater(updatedProductHaul.ShippingLoadSheets.Count, 0);

            Assert.AreEqual(ProductHaulStatus.OnLocation, updatedProductHaul.ProductHaulLifeStatus);

            var updatedAssignment = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(assignmentId);
            Assert.AreEqual(updatedAssignment.Id, assignment.Id);
            Assert.AreEqual(RigJobCrewSectionStatus.Returned, updatedAssignment.RigJobCrewSectionStatus);

            var updatedSanjelCrewSchedule =
                eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(sanjelCrewScheduleId, true);
            Assert.IsNotNull(updatedSanjelCrewSchedule);
            Assert.IsNotNull(updatedSanjelCrewSchedule.UnitSchedule);
            Assert.Greater(updatedSanjelCrewSchedule.UnitSchedule.Count, 0);
            Assert.IsNotNull(updatedSanjelCrewSchedule.WorkerSchedule);
            Assert.Greater(updatedSanjelCrewSchedule.WorkerSchedule.Count, 0);

            //5. Clean up test data
            eServiceOnlineGateway.Instance.DeleteCrewSchedule(updatedSanjelCrewSchedule.Id, true);
            eServiceOnlineGateway.Instance.DeleteRigJobCrewSection(updatedAssignment);
            eServiceOnlineGateway.Instance.DeleteProductHaul(updatedProductHaul, true);

            //6. Double check all data are cleaned
            var cleanedSanjelCrewSchedule =
                eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(sanjelCrewScheduleId, true);
            Assert.IsNull(cleanedSanjelCrewSchedule);
            var cleanedAssignment = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(assignmentId);
            Assert.IsNull(cleanedAssignment);
            var cleanedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);
            Assert.IsNull(cleanedProductHaul);
        }
        [Test]
        public void TestSetBulkerCrewMultipleAssignmentsStatusLoaded()
        {
            //1. Prepare test data
            //Product Haul is in progress
            ProductHaul productHaul = PrepareScheduledProductHaulForBulkerCrewStatusAPI(DateTime.Now.AddHours(1));
            ProductHaul secondProductHaul = PrepareSecondScheduledProductHaulForBulkerCrewStatusAPI();
            ProductHaul thirdProductHaul = PrepareThirdScheduledProductHaulForBulkerCrewStatusAPI();
            int productHaulId = productHaul.Id;
            globalProductHaulId = productHaulId;
            Assert.IsNotNull(productHaul);

            int secondProductHaulId = secondProductHaul.Id;
            globalProductHaulId2 = secondProductHaulId;
            Assert.IsNotNull(secondProductHaulId);
            int thirdProductHaulId = thirdProductHaul.Id;
            globalProductHaulId3 = thirdProductHaulId;
            Assert.IsNotNull(thirdProductHaulId);

            RigJob rigJob = new RigJob();
            Assert.IsNotNull(rigJob);
            (RigJobSanjelCrewSection assignment, Schedule schedule) = AssignCrewToProductHaul(productHaul, bulkerCrew1, rigJob);
            (RigJobSanjelCrewSection secondAssignment, Schedule secondSchedule) = AssignCrewToProductHaul(secondProductHaul, bulkerCrew1, rigJob);
            (RigJobSanjelCrewSection thirdAssignment, Schedule thirdSchedule) = AssignCrewToProductHaul(thirdProductHaul, bulkerCrew1, rigJob);
            int assignmentId = assignment.Id;
            Assert.IsNotNull(assignment);
            Assert.IsNotNull(schedule);
            SanjelCrewSchedule sanjelCrewSchedule = (SanjelCrewSchedule)schedule;
            Assert.IsNotNull(sanjelCrewSchedule);

            int secondAssignmentId = secondAssignment.Id;
            globalAssignmentId2 = secondAssignmentId;
            Assert.IsNotNull(secondAssignment);
            Assert.IsNotNull(secondSchedule);
            SanjelCrewSchedule secondSanjelCrewSchedule = (SanjelCrewSchedule)secondSchedule;
            Assert.IsNotNull(secondSanjelCrewSchedule);

            int thirdAssignmentId = thirdAssignment.Id;
            globalAssignmentId3 = thirdAssignmentId;
            Assert.IsNotNull(thirdAssignment);
            Assert.IsNotNull(thirdSchedule);
            SanjelCrewSchedule thirdSanjelCrewSchedule = (SanjelCrewSchedule)thirdSchedule;
            Assert.IsNotNull(thirdSanjelCrewSchedule);

            int sanjelCrewScheduleId = sanjelCrewSchedule.Id;
            globalScheduleId = sanjelCrewScheduleId;
            int secondSanjelCrewScheduleId = secondSanjelCrewSchedule.Id;
            globalScheduleId2 = secondSanjelCrewScheduleId;
            int thirdSanjelCrewScheduleId = thirdSanjelCrewSchedule.Id;
            globalScheduleId3 = thirdSanjelCrewScheduleId;

            //2.Check if test data meet preconditions
            Assert.AreEqual(ProductHaulStatus.Scheduled, productHaul.ProductHaulLifeStatus);
            Assert.AreEqual(RigJobCrewSectionStatus.Scheduled, assignment.RigJobCrewSectionStatus);

            Assert.AreEqual(ProductHaulStatus.Scheduled, secondProductHaul.ProductHaulLifeStatus);
            Assert.AreEqual(RigJobCrewSectionStatus.Scheduled, secondAssignment.RigJobCrewSectionStatus);

            Assert.AreEqual(ProductHaulStatus.Scheduled, thirdProductHaul.ProductHaulLifeStatus);
            Assert.AreEqual(RigJobCrewSectionStatus.Scheduled, thirdAssignment.RigJobCrewSectionStatus);

            //3.Execute the method to test
            string result = eServiceWebContext.Instance.SetBulkerCrewStatusByCrewId(bulkerCrew1.Id, BulkerCrewStatus.Loaded, false, "TestSetMultipleBulkerCrewStatusLoaded");
            //4. Verify the result meets post-conditions
            Assert.AreEqual(result, "Succeed");
            var updatedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);
            Assert.IsNotNull(updatedProductHaul);
            var updatedSecondProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(secondProductHaulId);
            Assert.IsNotNull(updatedSecondProductHaul);
            var updatedThirdProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(thirdProductHaulId);
            Assert.IsNotNull(updatedThirdProductHaul);

            Assert.AreEqual(ProductHaulStatus.Scheduled, updatedProductHaul.ProductHaulLifeStatus);
            Assert.AreEqual(ProductHaulStatus.Scheduled, updatedSecondProductHaul.ProductHaulLifeStatus);
            Assert.AreEqual(ProductHaulStatus.Scheduled, updatedThirdProductHaul.ProductHaulLifeStatus);


            var updatedAssignment = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(assignmentId);
            Assert.AreEqual(updatedAssignment.Id, assignment.Id);
            Assert.AreEqual(RigJobCrewSectionStatus.Loaded, updatedAssignment.RigJobCrewSectionStatus);

            var updatedSecondAssignment = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(secondAssignmentId);
            Assert.AreEqual(updatedSecondAssignment.Id, secondAssignment.Id);
            Assert.AreEqual(RigJobCrewSectionStatus.Scheduled, updatedSecondAssignment.RigJobCrewSectionStatus);

            var updatedThirdAssignment = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(thirdAssignmentId);
            Assert.AreEqual(updatedThirdAssignment.Id, thirdAssignment.Id);
            Assert.AreEqual(RigJobCrewSectionStatus.Scheduled, updatedThirdAssignment.RigJobCrewSectionStatus);

            var updatedSanjelCrewSchedule = eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(sanjelCrewScheduleId, true);
            Assert.IsNotNull(updatedSanjelCrewSchedule);
            var updatedSecondSanjelCrewSchedule = eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(secondSanjelCrewScheduleId, true);
            Assert.IsNotNull(updatedSecondSanjelCrewSchedule);
            var updatedThirdSanjelCrewSchedule = eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(thirdSanjelCrewScheduleId, true);
            Assert.IsNotNull(updatedThirdSanjelCrewSchedule);


            //5. Clean up test data
            eServiceOnlineGateway.Instance.DeleteCrewSchedule(updatedSanjelCrewSchedule.Id, true);
            eServiceOnlineGateway.Instance.DeleteRigJobCrewSection(updatedAssignment);
            eServiceOnlineGateway.Instance.DeleteProductHaul(updatedProductHaul, true);

            eServiceOnlineGateway.Instance.DeleteCrewSchedule(updatedSecondSanjelCrewSchedule.Id, true);
            eServiceOnlineGateway.Instance.DeleteRigJobCrewSection(updatedSecondAssignment);
            eServiceOnlineGateway.Instance.DeleteProductHaul(updatedSecondProductHaul, true);

            eServiceOnlineGateway.Instance.DeleteCrewSchedule(updatedThirdSanjelCrewSchedule.Id, true);
            eServiceOnlineGateway.Instance.DeleteRigJobCrewSection(updatedThirdAssignment);
            eServiceOnlineGateway.Instance.DeleteProductHaul(updatedThirdProductHaul, true);

            //6. Double check all data are cleaned
            var cleanedSanjelCrewSchedule = eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(sanjelCrewScheduleId, true);
            Assert.IsNull(cleanedSanjelCrewSchedule);
            var cleanedAssignment = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(assignmentId);
            Assert.IsNull(cleanedAssignment);
            var cleanedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);
            Assert.IsNull(cleanedProductHaul);

            var cleanedSecondSanjelCrewSchedule = eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(secondSanjelCrewScheduleId, true);
            Assert.IsNull(cleanedSecondSanjelCrewSchedule);
            var cleanedSecondAssignment = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(secondAssignmentId);
            Assert.IsNull(cleanedSecondAssignment);
            var cleanedSecondProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(secondProductHaulId);
            Assert.IsNull(cleanedSecondProductHaul);

            var cleanedThirdSanjelCrewSchedule = eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(thirdSanjelCrewScheduleId, true);
            Assert.IsNull(cleanedThirdSanjelCrewSchedule);
            var cleanedThirdAssignment = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(thirdAssignmentId);
            Assert.IsNull(cleanedThirdAssignment);
            var cleanedThirdProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(thirdProductHaulId);
            Assert.IsNull(cleanedThirdProductHaul);

        }
        #endregion

        #region  ThirdParty Crew Test
        [Test]
        public void TestSetThirdPartyBulkerCrewStatusCalled()
        {
            //1. Prepare test data
            //Product Haul is in progress
            ProductHaul productHaul = PrepareScheduledProductHaulForBulkerCrewStatusAPI(DateTime.Now.AddHours(1), true);
            int productHaulId = productHaul.Id;
            globalProductHaulId = productHaulId;
            Assert.IsNotNull(productHaul);
            RigJob rigJob = new RigJob();
            Assert.IsNotNull(rigJob);
            (RigJobThirdPartyBulkerCrewSection assignment, Schedule schedule) = AssignThirdPartyCrewToProductHaul(productHaul, thridPartyBulkerCrew1, rigJob);
            int assignmentId = assignment.Id;
            globalAssignmentId = assignmentId;
            Assert.IsNotNull(assignment);
            Assert.IsNotNull(schedule);
            ThirdPartyBulkerCrewSchedule thirdPartyCrewSchedule = (ThirdPartyBulkerCrewSchedule)schedule;
            Assert.IsNotNull(thirdPartyCrewSchedule);
            int thirdPartyCrewScheduleId = thirdPartyCrewSchedule.Id;
            globalScheduleId = thirdPartyCrewScheduleId;
            //2.Check if test data meet preconditions
            Assert.AreEqual(ProductHaulStatus.Scheduled, productHaul.ProductHaulLifeStatus);
            Assert.AreEqual(RigJobCrewSectionStatus.Scheduled, assignment.RigJobCrewSectionStatus);

            //3.Execute the method to test
            string result = eServiceWebContext.Instance.SetBulkerCrewStatusByCrewId(assignment.ThirdPartyBulkerCrew.Id, BulkerCrewStatus.Called, true, "TestSetThirdPartyBulkerCrewStatusCalled");
            //4. Verify the result meets post-conditions
            Assert.AreEqual(result, "Succeed");
            var updatedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);
            Assert.IsNotNull(updatedProductHaul);
            Assert.IsNotNull(updatedProductHaul.ShippingLoadSheets);
            Assert.Greater(updatedProductHaul.ShippingLoadSheets.Count, 0);

            Assert.AreEqual(ProductHaulStatus.Scheduled, updatedProductHaul.ProductHaulLifeStatus);


            var updatedAssignment = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionById(assignmentId);
            Assert.AreEqual(updatedAssignment.Id, assignment.Id);
            Assert.AreEqual(RigJobCrewSectionStatus.Called, updatedAssignment.RigJobCrewSectionStatus);

            var updatedThirdPartyCrewSchedule =
                eServiceOnlineGateway.Instance.GetThirdPartyCrewScheduleById(thirdPartyCrewScheduleId);
            Assert.IsNotNull(updatedThirdPartyCrewSchedule);

            //5. Clean up test data
            eServiceOnlineGateway.Instance.DeletethirdPartyBulkerCrewSchedule(updatedThirdPartyCrewSchedule.Id);
            eServiceOnlineGateway.Instance.DeleteRigJobThirdPartyBulkerCrewSection(updatedAssignment.Id);
            eServiceOnlineGateway.Instance.DeleteProductHaul(updatedProductHaul, true);

            //6. Double check all data are cleaned
            var cleanedThirdPartyCrewSchedule =
                eServiceOnlineGateway.Instance.GetThirdPartyCrewScheduleById(thirdPartyCrewScheduleId);
            Assert.IsNull(cleanedThirdPartyCrewSchedule);
            var cleanedAssignment = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionById(assignmentId);
            Assert.IsNull(cleanedAssignment);
            var cleanedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);
            Assert.IsNull(cleanedProductHaul);

        }
        [Test]
        public void TestSetThirdPartyBulkerCrewStatusLoading()
        {
            //1. Prepare test data
            //Product Haul is in progress
            ProductHaul productHaul = PrepareScheduledProductHaulForBulkerCrewStatusAPI(DateTime.Now.AddHours(1), true);
            int productHaulId = productHaul.Id;
            globalProductHaulId = productHaulId;
            Assert.IsNotNull(productHaul);
            RigJob rigJob = new RigJob();
            Assert.IsNotNull(rigJob);
            (RigJobThirdPartyBulkerCrewSection assignment, Schedule schedule) = AssignThirdPartyCrewToProductHaul(productHaul, thridPartyBulkerCrew1, rigJob);
            int assignmentId = assignment.Id;
            globalAssignmentId = assignmentId;
            Assert.IsNotNull(assignment);
            Assert.IsNotNull(schedule);
            ThirdPartyBulkerCrewSchedule thirdPartyCrewSchedule = (ThirdPartyBulkerCrewSchedule)schedule;
            Assert.IsNotNull(thirdPartyCrewSchedule);
            int thirdPartyCrewScheduleId = thirdPartyCrewSchedule.Id;
            globalScheduleId = thirdPartyCrewScheduleId;
            //2.Check if test data meet preconditions
            Assert.AreEqual(ProductHaulStatus.Scheduled, productHaul.ProductHaulLifeStatus);
            Assert.AreEqual(RigJobCrewSectionStatus.Scheduled, assignment.RigJobCrewSectionStatus);

            //3.Execute the method to test
            string result = eServiceWebContext.Instance.SetBulkerCrewStatusByCrewId(assignment.ThirdPartyBulkerCrew.Id, BulkerCrewStatus.Loading, true, "TestSetThirdPartyBulkerCrewStatusCalled");
            //4. Verify the result meets post-conditions
            Assert.AreEqual(result, "Succeed");
            var updatedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);
            Assert.IsNotNull(updatedProductHaul);
            Assert.IsNotNull(updatedProductHaul.ShippingLoadSheets);
            Assert.Greater(updatedProductHaul.ShippingLoadSheets.Count, 0);
            Assert.AreEqual(ProductHaulStatus.Scheduled, updatedProductHaul.ProductHaulLifeStatus);
            var updatedAssignment = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionById(assignmentId);
            Assert.AreEqual(updatedAssignment.Id, assignment.Id);
            Assert.AreEqual(RigJobCrewSectionStatus.Loading, updatedAssignment.RigJobCrewSectionStatus);

            var updatedThirdPartyCrewSchedule =
                eServiceOnlineGateway.Instance.GetThirdPartyCrewScheduleById(thirdPartyCrewScheduleId);
            Assert.IsNotNull(updatedThirdPartyCrewSchedule);

            //5. Clean up test data
            eServiceOnlineGateway.Instance.DeletethirdPartyBulkerCrewSchedule(updatedThirdPartyCrewSchedule.Id);
            eServiceOnlineGateway.Instance.DeleteRigJobThirdPartyBulkerCrewSection(updatedAssignment.Id);
            eServiceOnlineGateway.Instance.DeleteProductHaul(updatedProductHaul, true);

            //6. Double check all data are cleaned
            var cleanedThirdPartyCrewSchedule =
                eServiceOnlineGateway.Instance.GetThirdPartyCrewScheduleById(thirdPartyCrewScheduleId);
            Assert.IsNull(cleanedThirdPartyCrewSchedule);
            var cleanedAssignment = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionById(assignmentId);
            Assert.IsNull(cleanedAssignment);
            var cleanedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);
            Assert.IsNull(cleanedProductHaul);

        }
        [Test]
        public void TestSetThirdPartyBulkerCrewStatusLoaded()
        {
            TestSetThirdPartyBulkerCrewStatusLoaded(ProductHaulStatus.Scheduled, RigJobCrewSectionStatus.Scheduled);
            TestSetThirdPartyBulkerCrewStatusLoaded(ProductHaulStatus.Scheduled, RigJobCrewSectionStatus.Loading);
        }

        private void TestSetThirdPartyBulkerCrewStatusLoaded(ProductHaulStatus productHaulStatus, RigJobCrewSectionStatus rigJobCrewSectionStatus)
        {
            //1. Prepare test data
            //Product Haul is in progress
            ProductHaul productHaul = PrepareScheduledProductHaulForBulkerCrewStatusAPI(DateTime.Now.AddHours(1), true);
            int productHaulId = productHaul.Id;
            globalProductHaulId = productHaulId;
            Assert.IsNotNull(productHaul);
            RigJob rigJob = new RigJob();
            Assert.IsNotNull(rigJob);
            (RigJobThirdPartyBulkerCrewSection assignment, Schedule schedule) = AssignThirdPartyCrewToProductHaul(productHaul, thridPartyBulkerCrew1, rigJob);
            int assignmentId = assignment.Id;
            globalAssignmentId = assignmentId;
            Assert.IsNotNull(assignment);
            Assert.IsNotNull(schedule);
            ThirdPartyBulkerCrewSchedule thirdPartyCrewSchedule = (ThirdPartyBulkerCrewSchedule)schedule;
            Assert.IsNotNull(thirdPartyCrewSchedule);
            if (rigJobCrewSectionStatus != RigJobCrewSectionStatus.Scheduled || productHaulStatus != ProductHaulStatus.Scheduled)
            {
                assignment = AssignThirdPartyCrewOtherStatus(assignment, productHaulStatus, rigJobCrewSectionStatus);
            }
            int thirdPartyCrewScheduleId = thirdPartyCrewSchedule.Id;
            globalScheduleId = thirdPartyCrewScheduleId;
            //2.Check if test data meet preconditions
            Assert.AreEqual(productHaulStatus, productHaul.ProductHaulLifeStatus);
            Assert.AreEqual(rigJobCrewSectionStatus, assignment.RigJobCrewSectionStatus);

            //3.Execute the method to test
            string result = eServiceWebContext.Instance.SetBulkerCrewStatusByCrewId(assignment.ThirdPartyBulkerCrew.Id, BulkerCrewStatus.Loaded, true, "TestSetThirdPartyBulkerCrewStatusLoaded");
            
            //4. Verify the result meets post-conditions
            Assert.AreEqual(result, "Succeed");
            var updatedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);
            Assert.IsNotNull(updatedProductHaul);
            Assert.IsNotNull(updatedProductHaul.ShippingLoadSheets);
            Assert.Greater(updatedProductHaul.ShippingLoadSheets.Count, 0);

            Assert.AreEqual(ProductHaulStatus.Scheduled, updatedProductHaul.ProductHaulLifeStatus);

            var updatedAssignment = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionById(assignmentId);
            Assert.AreEqual(updatedAssignment.Id, assignment.Id);
            Assert.AreEqual(RigJobCrewSectionStatus.Loaded, updatedAssignment.RigJobCrewSectionStatus);

            var updatedThirdPartyCrewSchedule =
                eServiceOnlineGateway.Instance.GetThirdPartyCrewScheduleById(thirdPartyCrewScheduleId);
            Assert.IsNotNull(updatedThirdPartyCrewSchedule);

            //5. Clean up test data
            eServiceOnlineGateway.Instance.DeletethirdPartyBulkerCrewSchedule(updatedThirdPartyCrewSchedule.Id);
            eServiceOnlineGateway.Instance.DeleteRigJobThirdPartyBulkerCrewSection(updatedAssignment.Id);
            eServiceOnlineGateway.Instance.DeleteProductHaul(updatedProductHaul, true);

            //6. Double check all data are cleaned
            var cleanedThirdPartyCrewSchedule =
                eServiceOnlineGateway.Instance.GetThirdPartyCrewScheduleById(thirdPartyCrewScheduleId);
            Assert.IsNull(cleanedThirdPartyCrewSchedule);
            var cleanedAssignment = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionById(assignmentId);
            Assert.IsNull(cleanedAssignment);
            var cleanedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);
            Assert.IsNull(cleanedProductHaul);

        }
        [Test]
        public void TestSetThirdPartyBulkerCrewStatusEnRoute()
        {
            TestSetThirdPartyBulkerCrewStatusEnRoute(ProductHaulStatus.Scheduled, RigJobCrewSectionStatus.Loading);
            TestSetThirdPartyBulkerCrewStatusEnRoute(ProductHaulStatus.Scheduled, RigJobCrewSectionStatus.Loaded);
        }
        private void TestSetThirdPartyBulkerCrewStatusEnRoute(ProductHaulStatus productHaulStatus, RigJobCrewSectionStatus rigJobCrewSectionStatus)
        {
            //1. Prepare test data
            //Product Haul is in progress
            ProductHaul productHaul = PrepareScheduledProductHaulForBulkerCrewStatusAPI(DateTime.Now.AddHours(1), true);
            int productHaulId = productHaul.Id;
            globalProductHaulId = productHaulId;
            Assert.IsNotNull(productHaul);
            RigJob rigJob = new RigJob();
            Assert.IsNotNull(rigJob);
            (RigJobThirdPartyBulkerCrewSection assignment, Schedule schedule) = AssignThirdPartyCrewToProductHaul(productHaul, thridPartyBulkerCrew1, rigJob);
            int assignmentId = assignment.Id;
            globalAssignmentId = assignmentId;
            Assert.IsNotNull(assignment);
            Assert.IsNotNull(schedule);
            ThirdPartyBulkerCrewSchedule thirdPartyCrewSchedule = (ThirdPartyBulkerCrewSchedule)schedule;
            Assert.IsNotNull(thirdPartyCrewSchedule);
            if (rigJobCrewSectionStatus != RigJobCrewSectionStatus.Scheduled || productHaulStatus != ProductHaulStatus.Scheduled)
            {
                assignment = AssignThirdPartyCrewOtherStatus(assignment, productHaulStatus, rigJobCrewSectionStatus);
            }
            int thirdPartyCrewScheduleId = thirdPartyCrewSchedule.Id;
            globalScheduleId = thirdPartyCrewScheduleId;
            //2.Check if test data meet preconditions
            Assert.AreEqual(productHaulStatus, productHaul.ProductHaulLifeStatus);
            Assert.AreEqual(rigJobCrewSectionStatus, assignment.RigJobCrewSectionStatus);

            //3.Execute the method to test
            string result = eServiceWebContext.Instance.SetBulkerCrewStatusByCrewId(assignment.ThirdPartyBulkerCrew.Id, BulkerCrewStatus.EnRoute, true, "TestSetThirdPartyBulkerCrewStatusEnRoute");
            //4. Verify the result meets post-conditions
            if (rigJobCrewSectionStatus == RigJobCrewSectionStatus.Loading)
	            Assert.AreEqual(result, "Current assignment status is not LOADED");
            if (rigJobCrewSectionStatus == RigJobCrewSectionStatus.Loaded)
	            Assert.AreEqual(result, "Succeed");
            var updatedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);
            Assert.IsNotNull(updatedProductHaul);
            Assert.IsNotNull(updatedProductHaul.ShippingLoadSheets);
            Assert.Greater(updatedProductHaul.ShippingLoadSheets.Count, 0);

            if (rigJobCrewSectionStatus == RigJobCrewSectionStatus.Loading)
	            Assert.AreEqual(ProductHaulStatus.Scheduled, updatedProductHaul.ProductHaulLifeStatus);
            if (rigJobCrewSectionStatus == RigJobCrewSectionStatus.Loaded)
	            Assert.AreEqual(ProductHaulStatus.InProgress, updatedProductHaul.ProductHaulLifeStatus);

            var updatedAssignment = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionById(assignmentId);
            Assert.AreEqual(updatedAssignment.Id, assignment.Id);
            if (rigJobCrewSectionStatus == RigJobCrewSectionStatus.Loading)
	            Assert.AreEqual(RigJobCrewSectionStatus.Loading, updatedAssignment.RigJobCrewSectionStatus);
            if (rigJobCrewSectionStatus == RigJobCrewSectionStatus.Loaded)
	            Assert.AreEqual(RigJobCrewSectionStatus.EnRoute, updatedAssignment.RigJobCrewSectionStatus);

            var updatedThirdPartyCrewSchedule =
                eServiceOnlineGateway.Instance.GetThirdPartyCrewScheduleById(thirdPartyCrewScheduleId);
            Assert.IsNotNull(updatedThirdPartyCrewSchedule);

            //5. Clean up test data
            eServiceOnlineGateway.Instance.DeletethirdPartyBulkerCrewSchedule(updatedThirdPartyCrewSchedule.Id);
            eServiceOnlineGateway.Instance.DeleteRigJobThirdPartyBulkerCrewSection(updatedAssignment.Id);
            eServiceOnlineGateway.Instance.DeleteProductHaul(updatedProductHaul, true);

            //6. Double check all data are cleaned
            var cleanedThirdPartyCrewSchedule =
                eServiceOnlineGateway.Instance.GetThirdPartyCrewScheduleById(thirdPartyCrewScheduleId);
            Assert.IsNull(cleanedThirdPartyCrewSchedule);
            var cleanedAssignment = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionById(assignmentId);
            Assert.IsNull(cleanedAssignment);
            var cleanedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);
            Assert.IsNull(cleanedProductHaul);

        }

        [Test]
        public void TestSetThirdPartyBulkerCrewStatusOnLocation()
        {

	        #region case 3    
            TestSetThirdPartyBulkerCrewStatusOnLocation(ProductHaulStatus.Scheduled, RigJobCrewSectionStatus.Loaded);
            #endregion
            #region case 4   
            TestSetThirdPartyBulkerCrewStatusOnLocation(ProductHaulStatus.Scheduled, RigJobCrewSectionStatus.EnRoute);
            #endregion
        }

        private void TestSetThirdPartyBulkerCrewStatusOnLocation(ProductHaulStatus productHaulStatus, RigJobCrewSectionStatus rigJobCrewSectionStatus)
        {
            //1. Prepare test data
            //Product Haul is in progress
            ProductHaul productHaul = PrepareScheduledProductHaulForBulkerCrewStatusAPI(DateTime.Now.AddHours(1), true);
            int productHaulId = productHaul.Id;
            globalProductHaulId = productHaulId;
            Assert.IsNotNull(productHaul);
            Assert.IsNotNull(productHaul.ShippingLoadSheets);
            Assert.Greater(productHaul.ShippingLoadSheets.Count, 0);
            RigJob rigJob = new RigJob();
            Assert.IsNotNull(rigJob);
            (RigJobThirdPartyBulkerCrewSection assignment, Schedule schedule) = AssignThirdPartyCrewToProductHaul(productHaul, thridPartyBulkerCrew1, rigJob);
            int assignmentId = assignment.Id;
            globalAssignmentId=assignment.Id;
            Assert.IsNotNull(assignment);
            Assert.IsNotNull(schedule);
            ThirdPartyBulkerCrewSchedule thirdPartyCrewSchedule = (ThirdPartyBulkerCrewSchedule)schedule;
            Assert.IsNotNull(thirdPartyCrewSchedule);
            if (rigJobCrewSectionStatus != RigJobCrewSectionStatus.Scheduled || productHaulStatus != ProductHaulStatus.Scheduled)
            {
                assignment = AssignThirdPartyCrewOtherStatus(assignment, productHaulStatus, rigJobCrewSectionStatus);
            }
            int thirdPartyCrewScheduleId = thirdPartyCrewSchedule.Id;
            globalScheduleId=thirdPartyCrewScheduleId;
            //2.Check if test data meet preconditions
            Assert.AreEqual(productHaulStatus, productHaul.ProductHaulLifeStatus);
            Assert.AreEqual(rigJobCrewSectionStatus, assignment.RigJobCrewSectionStatus);
            foreach (var productHaulShippingLoadSheet in productHaul.ShippingLoadSheets)
            {
                Assert.AreEqual(ShippingStatus.Scheduled, productHaulShippingLoadSheet.ShippingStatus);
            }
            //3.Execute the method to test
            string result = eServiceWebContext.Instance.SetBulkerCrewStatusByCrewId(assignment.ThirdPartyBulkerCrew.Id, BulkerCrewStatus.OnLocation, true, "TestSetThirdPartyBulkerCrewStatusOnLocation");
            //4. Verify the result meets post-conditions
            Assert.AreEqual(result, "Succeed");
            var updatedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);
            Assert.IsNotNull(updatedProductHaul);
            Assert.IsNotNull(updatedProductHaul.ShippingLoadSheets);
            Assert.Greater(updatedProductHaul.ShippingLoadSheets.Count, 0);

            Assert.AreEqual(ProductHaulStatus.OnLocation, updatedProductHaul.ProductHaulLifeStatus);
            foreach (var productHaulShippingLoadSheet in updatedProductHaul.ShippingLoadSheets)
            {
                Assert.AreEqual(ShippingStatus.OnLocation, productHaulShippingLoadSheet.ShippingStatus);
            }

            var updatedAssignment = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionById(assignmentId);
            Assert.AreEqual(updatedAssignment.Id, assignment.Id);
            Assert.AreEqual(RigJobCrewSectionStatus.OnLocation, updatedAssignment.RigJobCrewSectionStatus);

            var updatedThirdPartyCrewSchedule =
                eServiceOnlineGateway.Instance.GetThirdPartyCrewScheduleById(thirdPartyCrewScheduleId);
            Assert.IsNotNull(updatedThirdPartyCrewSchedule);

            //5. Clean up test data
            eServiceOnlineGateway.Instance.DeletethirdPartyBulkerCrewSchedule(updatedThirdPartyCrewSchedule.Id);
            eServiceOnlineGateway.Instance.DeleteRigJobThirdPartyBulkerCrewSection(updatedAssignment.Id);
            eServiceOnlineGateway.Instance.DeleteProductHaul(updatedProductHaul, true);

            //6. Double check all data are cleaned
            var cleanedThirdPartyCrewSchedule =
                eServiceOnlineGateway.Instance.GetThirdPartyCrewScheduleById(thirdPartyCrewScheduleId);
            Assert.IsNull(cleanedThirdPartyCrewSchedule);
            var cleanedAssignment = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionById(assignmentId);
            Assert.IsNull(cleanedAssignment);
            var cleanedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);
            Assert.IsNull(cleanedProductHaul);
        }
        [Test]
        public void TestSetThirdPartyBulkerCrewStatusOnWayIn()
        {
            #region case 3    
            TestSetThirdPartyBulkerCrewStatusOnWayIn(ProductHaulStatus.InProgress, RigJobCrewSectionStatus.Loaded);
            #endregion
            #region case 4   
            TestSetThirdPartyBulkerCrewStatusOnWayIn(ProductHaulStatus.InProgress, RigJobCrewSectionStatus.EnRoute);
            #endregion
            #region case 5    
            TestSetThirdPartyBulkerCrewStatusOnWayIn(ProductHaulStatus.OnLocation, RigJobCrewSectionStatus.OnLocation);
            #endregion

        }

        private void TestSetThirdPartyBulkerCrewStatusOnWayIn(ProductHaulStatus productHaulStatus, RigJobCrewSectionStatus rigJobCrewSectionStatus)
        {
            //1. Prepare test data
            //Product Haul is in progress
            ProductHaul productHaul = PrepareScheduledProductHaulForBulkerCrewStatusAPI(DateTime.Now.AddHours(1), true);
            int productHaulId = productHaul.Id;
            globalProductHaulId = productHaulId;
            Assert.IsNotNull(productHaul);
            Assert.IsNotNull(productHaul.ShippingLoadSheets);
            Assert.Greater(productHaul.ShippingLoadSheets.Count, 0);
            RigJob rigJob = new RigJob();
            Assert.IsNotNull(rigJob);
            (RigJobThirdPartyBulkerCrewSection assignment, Schedule schedule) = AssignThirdPartyCrewToProductHaul(productHaul, thridPartyBulkerCrew1, rigJob);
            int assignmentId = assignment.Id;
            globalAssignmentId = assignmentId;
            Assert.IsNotNull(assignment);
            Assert.IsNotNull(schedule);
            ThirdPartyBulkerCrewSchedule thirdPartyCrewSchedule = (ThirdPartyBulkerCrewSchedule)schedule;
            Assert.IsNotNull(thirdPartyCrewSchedule);
            if (rigJobCrewSectionStatus != RigJobCrewSectionStatus.Scheduled || productHaulStatus != ProductHaulStatus.Scheduled)
            {
                assignment = AssignThirdPartyCrewOtherStatus(assignment, productHaulStatus, rigJobCrewSectionStatus);
            }
            int thirdPartyCrewScheduleId = thirdPartyCrewSchedule.Id;
            globalScheduleId = thirdPartyCrewScheduleId;
            //2.Check if test data meet preconditions
            Assert.AreEqual(productHaulStatus, productHaul.ProductHaulLifeStatus);
            Assert.AreEqual(rigJobCrewSectionStatus, assignment.RigJobCrewSectionStatus);
            foreach (var productHaulShippingLoadSheet in productHaul.ShippingLoadSheets)
            {
                Assert.AreEqual(ShippingStatus.Scheduled, productHaulShippingLoadSheet.ShippingStatus);
            }
            //3.Execute the method to test
            string result = eServiceWebContext.Instance.SetBulkerCrewStatusByCrewId(assignment.ThirdPartyBulkerCrew.Id, BulkerCrewStatus.OnWayIn, true, "TestSetBulkerCrewStatusOnWayIn");
            //4. Verify the result meets post-conditions
            Assert.AreEqual(result, "Succeed");
            var updatedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);
            Assert.IsNotNull(updatedProductHaul);
            Assert.IsNotNull(updatedProductHaul.ShippingLoadSheets);
            Assert.Greater(updatedProductHaul.ShippingLoadSheets.Count, 0);

            Assert.AreEqual(ProductHaulStatus.OnLocation, updatedProductHaul.ProductHaulLifeStatus);
            if (productHaulStatus != ProductHaulStatus.OnLocation)
            {
                foreach (var productHaulShippingLoadSheet in updatedProductHaul.ShippingLoadSheets)
                {
                    Assert.AreEqual(ShippingStatus.OnLocation, productHaulShippingLoadSheet.ShippingStatus);
                }
            }

            var updatedAssignment = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionById(assignmentId);
            Assert.AreEqual(updatedAssignment.Id, assignment.Id);
            Assert.AreEqual(RigJobCrewSectionStatus.OnWayIn, updatedAssignment.RigJobCrewSectionStatus);

            var updatedThirdPartyCrewSchedule =
                eServiceOnlineGateway.Instance.GetThirdPartyCrewScheduleById(thirdPartyCrewScheduleId);
            Assert.IsNotNull(updatedThirdPartyCrewSchedule);

            //5. Clean up test data
            eServiceOnlineGateway.Instance.DeletethirdPartyBulkerCrewSchedule(updatedThirdPartyCrewSchedule.Id);
            eServiceOnlineGateway.Instance.DeleteRigJobThirdPartyBulkerCrewSection(updatedAssignment.Id);
            eServiceOnlineGateway.Instance.DeleteProductHaul(updatedProductHaul, true);

            //6. Double check all data are cleaned
            var cleanedThirdPartyCrewSchedule =
                eServiceOnlineGateway.Instance.GetThirdPartyCrewScheduleById(thirdPartyCrewScheduleId);
            Assert.IsNull(cleanedThirdPartyCrewSchedule);
            var cleanedAssignment = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionById(assignmentId);
            Assert.IsNull(cleanedAssignment);
            var cleanedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);
            Assert.IsNull(cleanedProductHaul);
        }
        [Test]
        public void TestSetThirdPartyBulkerCrewMultipleAssignmentsStatusLoaded()
        {
            //1. Prepare test data
            //Product Haul is in progress
            ProductHaul productHaul = PrepareScheduledProductHaulForBulkerCrewStatusAPI(DateTime.Now.AddHours(1));
            ProductHaul secondProductHaul = PrepareSecondScheduledProductHaulForBulkerCrewStatusAPI();
            ProductHaul thirdProductHaul = PrepareThirdScheduledProductHaulForBulkerCrewStatusAPI();
            int productHaulId = productHaul.Id;
            globalProductHaulId = productHaulId;
            Assert.IsNotNull(productHaul);

            int secondProductHaulId = secondProductHaul.Id;
            globalProductHaulId2 = secondProductHaulId;
            Assert.IsNotNull(secondProductHaulId);
            int thirdProductHaulId = thirdProductHaul.Id;
            globalProductHaulId3 = thirdProductHaulId;
            Assert.IsNotNull(thirdProductHaulId);

            RigJob rigJob = new RigJob();
            Assert.IsNotNull(rigJob);
            (RigJobThirdPartyBulkerCrewSection assignment, Schedule schedule) = AssignThirdPartyCrewToProductHaul(productHaul, thridPartyBulkerCrew1, rigJob);
            (RigJobThirdPartyBulkerCrewSection secondAssignment, Schedule secondSchedule) = AssignThirdPartyCrewToProductHaul(secondProductHaul, thridPartyBulkerCrew1, rigJob);
            (RigJobThirdPartyBulkerCrewSection thirdAssignment, Schedule thirdSchedule) = AssignThirdPartyCrewToProductHaul(thirdProductHaul, thridPartyBulkerCrew1, rigJob);
            int assignmentId = assignment.Id;
            globalAssignmentId = assignmentId;
            Assert.IsNotNull(assignment);
            Assert.IsNotNull(schedule);
            ThirdPartyBulkerCrewSchedule thirdPartyBulkerSchedule = (ThirdPartyBulkerCrewSchedule)schedule;
            Assert.IsNotNull(thirdPartyBulkerSchedule);

            int secondAssignmentId = secondAssignment.Id;
            globalAssignmentId2 = secondAssignmentId;
            Assert.IsNotNull(secondAssignment);
            Assert.IsNotNull(secondSchedule);
            ThirdPartyBulkerCrewSchedule secondThirdPartyBulkerCrewSchedule = (ThirdPartyBulkerCrewSchedule)secondSchedule;
            Assert.IsNotNull(secondThirdPartyBulkerCrewSchedule);

            int thirdAssignmentId = thirdAssignment.Id;
            globalAssignmentId3 = thirdAssignmentId;
            Assert.IsNotNull(thirdAssignment);
            Assert.IsNotNull(thirdSchedule);
            ThirdPartyBulkerCrewSchedule thirdThirdPartyBulkerCrewSchedule = (ThirdPartyBulkerCrewSchedule)thirdSchedule;
            Assert.IsNotNull(thirdThirdPartyBulkerCrewSchedule);

            int thirdPartyBulkerCrewScheduleId = thirdPartyBulkerSchedule.Id;
            globalScheduleId = thirdPartyBulkerCrewScheduleId;
            int secondThirdPartyBulkerCrewScheduleId = secondThirdPartyBulkerCrewSchedule.Id;
            globalScheduleId2 = secondThirdPartyBulkerCrewScheduleId;
            int thirdThirdPartyBulkerCrewScheduleId = thirdThirdPartyBulkerCrewSchedule.Id;
            globalScheduleId3 = thirdThirdPartyBulkerCrewScheduleId;

            //2.Check if test data meet preconditions
            Assert.AreEqual(ProductHaulStatus.Scheduled, productHaul.ProductHaulLifeStatus);
            Assert.AreEqual(RigJobCrewSectionStatus.Scheduled, assignment.RigJobCrewSectionStatus);

            Assert.AreEqual(ProductHaulStatus.Scheduled, secondProductHaul.ProductHaulLifeStatus);
            Assert.AreEqual(RigJobCrewSectionStatus.Scheduled, secondAssignment.RigJobCrewSectionStatus);

            Assert.AreEqual(ProductHaulStatus.Scheduled, thirdProductHaul.ProductHaulLifeStatus);
            Assert.AreEqual(RigJobCrewSectionStatus.Scheduled, thirdAssignment.RigJobCrewSectionStatus);

            //3.Execute the method to test
            string result = eServiceWebContext.Instance.SetBulkerCrewStatusByCrewId(thridPartyBulkerCrew1.Id, BulkerCrewStatus.Loaded, true, "TestSetMultiThirdPartyBulkerCrewStatusLoaded");
            //4. Verify the result meets post-conditions
            Assert.AreEqual(result, "Succeed");
            var updatedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);
            Assert.IsNotNull(updatedProductHaul);
            var updatedSecondProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(secondProductHaulId);
            Assert.IsNotNull(updatedSecondProductHaul);
            var updatedThirdProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(thirdProductHaulId);
            Assert.IsNotNull(updatedThirdProductHaul);

            Assert.AreEqual(ProductHaulStatus.Scheduled, updatedProductHaul.ProductHaulLifeStatus);
            Assert.AreEqual(ProductHaulStatus.Scheduled, updatedSecondProductHaul.ProductHaulLifeStatus);
            Assert.AreEqual(ProductHaulStatus.Scheduled, updatedThirdProductHaul.ProductHaulLifeStatus);


            var updatedAssignment = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionById(assignmentId);
            Assert.AreEqual(updatedAssignment.Id, assignment.Id);
            Assert.AreEqual(RigJobCrewSectionStatus.Loaded, updatedAssignment.RigJobCrewSectionStatus);

            var updatedSecondAssignment = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionById(secondAssignmentId);
            Assert.AreEqual(updatedSecondAssignment.Id, secondAssignment.Id);
            Assert.AreEqual(RigJobCrewSectionStatus.Scheduled, updatedSecondAssignment.RigJobCrewSectionStatus);

            var updatedThirdAssignment = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionById(thirdAssignmentId);
            Assert.AreEqual(updatedThirdAssignment.Id, thirdAssignment.Id);
            Assert.AreEqual(RigJobCrewSectionStatus.Scheduled, updatedThirdAssignment.RigJobCrewSectionStatus);

            var updatedThirdPartyCrewSchedule = eServiceOnlineGateway.Instance.GetThirdPartyCrewScheduleById(thirdPartyBulkerCrewScheduleId);
            Assert.IsNotNull(updatedThirdPartyCrewSchedule);
            var updatedSecondThirdPartyCrewSchedule = eServiceOnlineGateway.Instance.GetThirdPartyCrewScheduleById(secondThirdPartyBulkerCrewScheduleId);
            Assert.IsNotNull(updatedSecondThirdPartyCrewSchedule);
            var updatedThirdThirdPartyCrewSchedule = eServiceOnlineGateway.Instance.GetThirdPartyCrewScheduleById(thirdThirdPartyBulkerCrewScheduleId);
            Assert.IsNotNull(updatedThirdThirdPartyCrewSchedule);


            //5. Clean up test data
            eServiceOnlineGateway.Instance.DeletethirdPartyBulkerCrewSchedule(updatedThirdPartyCrewSchedule.Id);
            eServiceOnlineGateway.Instance.DeleteRigJobThirdPartyBulkerCrewSection(updatedAssignment.Id);
            eServiceOnlineGateway.Instance.DeleteProductHaul(updatedProductHaul, true);

            eServiceOnlineGateway.Instance.DeletethirdPartyBulkerCrewSchedule(updatedSecondThirdPartyCrewSchedule.Id);
            eServiceOnlineGateway.Instance.DeleteRigJobThirdPartyBulkerCrewSection(updatedSecondAssignment.Id);
            eServiceOnlineGateway.Instance.DeleteProductHaul(updatedSecondProductHaul, true);

            eServiceOnlineGateway.Instance.DeletethirdPartyBulkerCrewSchedule(updatedThirdThirdPartyCrewSchedule.Id);
            eServiceOnlineGateway.Instance.DeleteRigJobThirdPartyBulkerCrewSection(updatedThirdAssignment.Id);
            eServiceOnlineGateway.Instance.DeleteProductHaul(updatedThirdProductHaul, true);

            //6. Double check all data are cleaned
            var cleanedThirdPartyCrewSchedule = eServiceOnlineGateway.Instance.GetThirdPartyCrewScheduleById(thirdPartyBulkerCrewScheduleId);
            Assert.IsNull(cleanedThirdPartyCrewSchedule);
            var cleanedAssignment = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionById(assignmentId);
            Assert.IsNull(cleanedAssignment);
            var cleanedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);
            Assert.IsNull(cleanedProductHaul);

            var cleanedSecondThirdPartySanjelCrewSchedule = eServiceOnlineGateway.Instance.GetThirdPartyCrewScheduleById(secondThirdPartyBulkerCrewScheduleId);
            Assert.IsNull(cleanedSecondThirdPartySanjelCrewSchedule);
            var cleanedSecondAssignment = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionById(secondAssignmentId);
            Assert.IsNull(cleanedSecondAssignment);
            var cleanedSecondProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(secondProductHaulId);
            Assert.IsNull(cleanedSecondProductHaul);

            var cleanedThirdThirdPartySanjelCrewSchedule = eServiceOnlineGateway.Instance.GetThirdPartyCrewScheduleById(thirdThirdPartyBulkerCrewScheduleId);
            Assert.IsNull(cleanedThirdThirdPartySanjelCrewSchedule);
            var cleanedThirdAssignment = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionById(thirdAssignmentId);
            Assert.IsNull(cleanedThirdAssignment);
            var cleanedThirdProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(thirdProductHaulId);
            Assert.IsNull(cleanedThirdProductHaul);

        }

        [Test]
        public void TestSetThirdPartyBulkerCrewStatusReturned()
        {

            #region case 2    
            TestSetThirdPartyBulkerCrewStatusReturned(ProductHaulStatus.InProgress, RigJobCrewSectionStatus.Loaded);
            #endregion
            #region case 3     
            TestSetThirdPartyBulkerCrewStatusReturned(ProductHaulStatus.InProgress, RigJobCrewSectionStatus.EnRoute);
            #endregion
            #region case 4      
            TestSetThirdPartyBulkerCrewStatusReturned(ProductHaulStatus.OnLocation, RigJobCrewSectionStatus.OnLocation);
            #endregion
            #region case 5  
            TestSetThirdPartyBulkerCrewStatusReturned(ProductHaulStatus.OnLocation, RigJobCrewSectionStatus.OnWayIn);
            #endregion
        }

        private void TestSetThirdPartyBulkerCrewStatusReturned(ProductHaulStatus productHaulStatus, RigJobCrewSectionStatus rigJobCrewSectionStatus)
        {
            //1. Prepare test data
            //Product Haul is in progress
            ProductHaul productHaul = PrepareScheduledProductHaulForBulkerCrewStatusAPI(DateTime.Now.AddHours(1), true);
            int productHaulId = productHaul.Id;
            globalProductHaulId = productHaulId;
            Assert.IsNotNull(productHaul);
            Assert.IsNotNull(productHaul.ShippingLoadSheets);
            Assert.Greater(productHaul.ShippingLoadSheets.Count, 0);
            RigJob rigJob = new RigJob();
            Assert.IsNotNull(rigJob);
            (RigJobThirdPartyBulkerCrewSection assignment, Schedule schedule) = AssignThirdPartyCrewToProductHaul(productHaul, thridPartyBulkerCrew1, rigJob);
            int assignmentId = assignment.Id;
            globalAssignmentId=assignmentId;
            Assert.IsNotNull(assignment);
            Assert.IsNotNull(schedule);
            ThirdPartyBulkerCrewSchedule thirdPartyCrewSchedule = (ThirdPartyBulkerCrewSchedule)schedule;
            Assert.IsNotNull(thirdPartyCrewSchedule);
            if (rigJobCrewSectionStatus != RigJobCrewSectionStatus.Scheduled || productHaulStatus != ProductHaulStatus.Scheduled)
            {
                assignment = AssignThirdPartyCrewOtherStatus(assignment, productHaulStatus, rigJobCrewSectionStatus);
            }
            int thirdPartyCrewScheduleId = thirdPartyCrewSchedule.Id;
            globalScheduleId = thirdPartyCrewScheduleId;
            //2.Check if test data meet preconditions
            Assert.AreEqual(productHaulStatus, productHaul.ProductHaulLifeStatus);
            Assert.AreEqual(rigJobCrewSectionStatus, assignment.RigJobCrewSectionStatus);
            foreach (var productHaulShippingLoadSheet in productHaul.ShippingLoadSheets)
            {
                Assert.AreEqual(ShippingStatus.Scheduled, productHaulShippingLoadSheet.ShippingStatus);
            }
            //3.Execute the method to test
            string result = eServiceWebContext.Instance.SetBulkerCrewStatusByCrewId(assignment.ThirdPartyBulkerCrew.Id, BulkerCrewStatus.Returned, true, "TestSetThirdPartyBulkerCrewStatusReturned");
            //4. Verify the result meets post-conditions
            Assert.AreEqual(result, "Succeed");
            var updatedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);
            Assert.IsNotNull(updatedProductHaul);
            Assert.IsNotNull(updatedProductHaul.ShippingLoadSheets);
            Assert.Greater(updatedProductHaul.ShippingLoadSheets.Count, 0);

            Assert.AreEqual(ProductHaulStatus.Returned, updatedProductHaul.ProductHaulLifeStatus);
            if (productHaulStatus == ProductHaulStatus.Returned)
            {
                foreach (var productHaulShippingLoadSheet in updatedProductHaul.ShippingLoadSheets)
                {
                    Assert.AreEqual(ShippingStatus.OnLocation, productHaulShippingLoadSheet.ShippingStatus);
                }
            }

            var updatedAssignment = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionById(assignmentId);
            Assert.AreEqual(updatedAssignment.Id, assignment.Id);
            Assert.AreEqual(RigJobCrewSectionStatus.Returned, updatedAssignment.RigJobCrewSectionStatus);

            var updatedThirdPartyCrewSchedule =
                    eServiceOnlineGateway.Instance.GetThirdPartyCrewScheduleById(thirdPartyCrewScheduleId);
            Assert.IsNotNull(updatedThirdPartyCrewSchedule);

            //5. Clean up test data
            eServiceOnlineGateway.Instance.DeletethirdPartyBulkerCrewSchedule(updatedThirdPartyCrewSchedule.Id);
            eServiceOnlineGateway.Instance.DeleteRigJobThirdPartyBulkerCrewSection(updatedAssignment.Id);
            eServiceOnlineGateway.Instance.DeleteProductHaul(updatedProductHaul, true);

            //6. Double check all data are cleaned
            var cleanedThirdPartyCrewSchedule =
                eServiceOnlineGateway.Instance.GetThirdPartyCrewScheduleById(thirdPartyCrewScheduleId);
            Assert.IsNull(cleanedThirdPartyCrewSchedule);
            var cleanedAssignment = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionById(assignmentId);
            Assert.IsNull(cleanedAssignment);
            var cleanedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);
            Assert.IsNull(cleanedProductHaul);
        }
        [Test]
        public void TestSetThirdPartyBulkerCrewStatusOffDuty()
        {
            //1. Prepare test data
            //Product Haul is in progress
            ProductHaul productHaul = PrepareScheduledProductHaulForBulkerCrewStatusAPI(DateTime.Now.AddHours(1), true);
            ProductHaul productHaul2 = PrepareScheduledProductHaulForBulkerCrewStatusAPI(DateTime.Now.AddHours(2), true);
            int productHaulId = productHaul.Id;
            globalProductHaulId = productHaulId;
            globalProductHaulId2 = productHaul2.Id;
            Assert.IsNotNull(productHaul);
            Assert.IsNotNull(productHaul.ShippingLoadSheets);
            Assert.Greater(productHaul.ShippingLoadSheets.Count, 0);
            RigJob rigJob = new RigJob();
            Assert.IsNotNull(rigJob);
            (RigJobThirdPartyBulkerCrewSection assignment, Schedule schedule) = AssignThirdPartyCrewToProductHaul(productHaul, thridPartyBulkerCrew1, rigJob);
            (RigJobThirdPartyBulkerCrewSection assignment2, Schedule schedule2) = AssignThirdPartyCrewToProductHaul(productHaul2, thridPartyBulkerCrew1, rigJob);
            int assignmentId = assignment.Id;
            globalAssignmentId=assignmentId;
            Assert.IsNotNull(assignment);
            Assert.IsNotNull(schedule);
            int assignment2Id = assignment2.Id;
            globalAssignmentId2 = assignment2Id;
            Assert.IsNotNull(assignment2Id);
            Assert.IsNotNull(schedule2);
            ThirdPartyBulkerCrewSchedule thirdPartyCrewSchedule = (ThirdPartyBulkerCrewSchedule)schedule;
            Assert.IsNotNull(thirdPartyCrewSchedule);
            assignment = AssignThirdPartyCrewOtherStatus(assignment, ProductHaulStatus.OnLocation, RigJobCrewSectionStatus.Returned);
            int thirdPartyCrewScheduleId = thirdPartyCrewSchedule.Id;
            globalScheduleId = thirdPartyCrewScheduleId;
            globalScheduleId2 = schedule2.Id;

            //2.Check if test data meet preconditions
            Assert.AreEqual(ProductHaulStatus.OnLocation, productHaul.ProductHaulLifeStatus);
            Assert.AreEqual(RigJobCrewSectionStatus.Returned, assignment.RigJobCrewSectionStatus);
            Assert.AreEqual(ProductHaulStatus.Scheduled, productHaul2.ProductHaulLifeStatus);
            Assert.AreEqual(RigJobCrewSectionStatus.Scheduled, assignment2.RigJobCrewSectionStatus);
            //3.Execute the method to test
            string result = eServiceWebContext.Instance.SetBulkerCrewStatusByCrewId(assignment.ThirdPartyBulkerCrew.Id, BulkerCrewStatus.OffDuty, true, "TestSetThirdPartyBulkerCrewStatusOffDuty");
            //4. Verify the result meets post-conditions
            Assert.AreEqual(result, "Succeed");
            var updatedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);
            Assert.IsNotNull(updatedProductHaul);
            Assert.IsNotNull(updatedProductHaul.ShippingLoadSheets);
            Assert.Greater(updatedProductHaul.ShippingLoadSheets.Count, 0);

            Assert.AreEqual(ProductHaulStatus.OnLocation, updatedProductHaul.ProductHaulLifeStatus);

            var updatedAssignment = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionById(assignmentId);
            Assert.AreEqual(updatedAssignment.Id, assignment.Id);
            rigJobThirdPartyBulkerCrewSectionService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigJobThirdPartyBulkerCrewSectionService>();
            if (rigJobThirdPartyBulkerCrewSectionService == null) throw new Exception("rigJobThirdPartyBulkerCrewSectionService must be registered in service factory");

            var rigJobCrewOffDutySections = rigJobThirdPartyBulkerCrewSectionService.SelectByThirdPartyBulkerCrew(updatedAssignment.ThirdPartyBulkerCrew.Id).Where(p => p.RigJobCrewSectionStatus == RigJobCrewSectionStatus.LogOffDuty);
            Assert.GreaterOrEqual(rigJobCrewOffDutySections.Count(), 1);

            var activedRigJobCrewSections = rigJobThirdPartyBulkerCrewSectionService.SelectByThirdPartyBulkerCrew(assignment.ThirdPartyBulkerCrew.Id).Where(p => p.RigJobCrewSectionStatus == RigJobCrewSectionStatus.Scheduled);
            Assert.GreaterOrEqual(activedRigJobCrewSections.Count(), 1);

            var bulkerCrewLog = eServiceOnlineGateway.Instance.GetThirdPartyBulkerCrewLog(assignment.ThirdPartyBulkerCrew.Id);
            Assert.AreEqual(BulkerCrewStatus.LoadRequested, bulkerCrewLog.CrewStatus);

            var updatedThirdPartyCrewSchedule =
                eServiceOnlineGateway.Instance.GetThirdPartyCrewScheduleById(thirdPartyCrewScheduleId);
            Assert.IsNotNull(updatedThirdPartyCrewSchedule);

            //5. Clean up test data
            eServiceOnlineGateway.Instance.DeletethirdPartyBulkerCrewSchedule(updatedThirdPartyCrewSchedule.Id);
            eServiceOnlineGateway.Instance.DeleteRigJobThirdPartyBulkerCrewSection(updatedAssignment.Id);
            eServiceOnlineGateway.Instance.DeleteProductHaul(updatedProductHaul, true);

            eServiceOnlineGateway.Instance.DeletethirdPartyBulkerCrewSchedule(schedule2.Id);
            eServiceOnlineGateway.Instance.DeleteRigJobThirdPartyBulkerCrewSection(assignment2.Id);
            eServiceOnlineGateway.Instance.DeleteProductHaul(productHaul2, true);

            //6. Double check all data are cleaned
            var cleanedSanjelCrewSchedule =
                eServiceOnlineGateway.Instance.GetThirdPartyCrewScheduleById(thirdPartyCrewScheduleId);
            Assert.IsNull(cleanedSanjelCrewSchedule);
            var cleanedAssignment = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionById(assignmentId);
            Assert.IsNull(cleanedAssignment);
            var cleanedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);
            Assert.IsNull(cleanedProductHaul);

            var cleanedSanjelCrewSchedule2 =
           eServiceOnlineGateway.Instance.GetThirdPartyCrewScheduleById(schedule2.Id);
            Assert.IsNull(cleanedSanjelCrewSchedule2);
            var cleanedAssignment2 = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionById(assignment2Id);
            Assert.IsNull(cleanedAssignment2);
            var cleanedProductHaul2 = eServiceOnlineGateway.Instance.GetProductHaulById(productHaul2.Id);
            Assert.IsNull(cleanedProductHaul2);
        }
        [Test]
        public void TestSetThirdPartyBulkerCrewStatusOffDutyNoFutureAssignment()
        {
            //1. Prepare test data
            //Product Haul is in progress
            ProductHaul productHaul = PrepareScheduledProductHaulForBulkerCrewStatusAPI(DateTime.Now.AddHours(1));
            int productHaulId = productHaul.Id;
            globalProductHaulId = productHaul.Id;
            Assert.IsNotNull(productHaul);
            Assert.IsNotNull(productHaul.ShippingLoadSheets);
            Assert.Greater(productHaul.ShippingLoadSheets.Count, 0);
            RigJob rigJob = new RigJob();
            Assert.IsNotNull(rigJob);
            (RigJobThirdPartyBulkerCrewSection assignment, Schedule schedule) = AssignThirdPartyCrewToProductHaul(productHaul, thridPartyBulkerCrew1, rigJob);
            int assignmentId = assignment.Id;
            globalAssignmentId = assignmentId;
            Assert.IsNotNull(assignment);
            Assert.IsNotNull(schedule);
            ThirdPartyBulkerCrewSchedule thirdPartyCrewSchedule = (ThirdPartyBulkerCrewSchedule)schedule;
            Assert.IsNotNull(thirdPartyCrewSchedule);
            assignment = AssignThirdPartyCrewOtherStatus(assignment, ProductHaulStatus.OnLocation, RigJobCrewSectionStatus.Returned);
            int thirdPartyCrewScheduleId = thirdPartyCrewSchedule.Id;
            globalScheduleId = thirdPartyCrewScheduleId;
            //2.Check if test data meet preconditions
            Assert.AreEqual(ProductHaulStatus.OnLocation, productHaul.ProductHaulLifeStatus);
            Assert.AreEqual(assignment.RigJobCrewSectionStatus, RigJobCrewSectionStatus.Returned);
            //3.Execute the method to test
            string result = eServiceWebContext.Instance.SetBulkerCrewStatusByCrewId(assignment.ThirdPartyBulkerCrew.Id, BulkerCrewStatus.OffDuty, true, "TestSetThirdPartyOffDutyNoFutureAssignment");
            //4. Verify the result meets post-conditions
            Assert.AreEqual(result, "Succeed");
            var updatedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);
            Assert.IsNotNull(updatedProductHaul);
            Assert.IsNotNull(updatedProductHaul.ShippingLoadSheets);
            Assert.Greater(updatedProductHaul.ShippingLoadSheets.Count, 0);

            Assert.AreEqual(ProductHaulStatus.OnLocation, updatedProductHaul.ProductHaulLifeStatus);

            var updatedAssignment = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionById(assignmentId);
            Assert.AreEqual(updatedAssignment.Id, assignment.Id);
            rigJobThirdPartyBulkerCrewSectionService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigJobThirdPartyBulkerCrewSectionService>();
            if (rigJobThirdPartyBulkerCrewSectionService == null) throw new Exception("rigJobThirdPartyBulkerCrewSectionService must be registered in service factory");

            var rigJobCrewOffDutySections = rigJobThirdPartyBulkerCrewSectionService.SelectByThirdPartyBulkerCrew(updatedAssignment.ThirdPartyBulkerCrew.Id).Where(p => p.RigJobCrewSectionStatus == RigJobCrewSectionStatus.LogOffDuty);
            Assert.GreaterOrEqual(rigJobCrewOffDutySections.Count(), 1);

            var activedRigJobCrewSections = rigJobThirdPartyBulkerCrewSectionService.SelectByThirdPartyBulkerCrew(assignment.ThirdPartyBulkerCrew.Id).Where(p => p.RigJobCrewSectionStatus != RigJobCrewSectionStatus.Removed && p.RigJobCrewSectionStatus != RigJobCrewSectionStatus.LogOffDuty);
            Assert.GreaterOrEqual(activedRigJobCrewSections.Count(), 0);

            var bulkerCrewLog = eServiceOnlineGateway.Instance.GetThirdPartyBulkerCrewLog(assignment.ThirdPartyBulkerCrew.Id);
            Assert.AreEqual(BulkerCrewStatus.OffDuty, bulkerCrewLog.CrewStatus);

            var updatedSanjelCrewSchedule =
                eServiceOnlineGateway.Instance.GetThirdPartyCrewScheduleById(thirdPartyCrewScheduleId);
            Assert.IsNotNull(updatedSanjelCrewSchedule);

            //5. Clean up test data
            eServiceOnlineGateway.Instance.DeletethirdPartyBulkerCrewSchedule(updatedSanjelCrewSchedule.Id);
            eServiceOnlineGateway.Instance.DeleteRigJobThirdPartyBulkerCrewSection(updatedAssignment.Id);
            eServiceOnlineGateway.Instance.DeleteProductHaul(updatedProductHaul, true);

            //6. Double check all data are cleaned
            var cleanedSanjelCrewSchedule =
                eServiceOnlineGateway.Instance.GetThirdPartyCrewScheduleById(thirdPartyCrewScheduleId);
            Assert.IsNull(cleanedSanjelCrewSchedule);
            var cleanedAssignment = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionById(assignmentId);
            Assert.IsNull(cleanedAssignment);
            var cleanedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);
            Assert.IsNull(cleanedProductHaul);
        }
        [Test]
        public void TestSetThirdPartyBulkerCrewStatusDown()
        {
            //1. Prepare test data
            //Product Haul is in progress
            ProductHaul productHaul = PrepareScheduledProductHaulForBulkerCrewStatusAPI(DateTime.Now.AddHours(1));
            int productHaulId = productHaul.Id;
            globalProductHaulId = productHaulId;
            Assert.IsNotNull(productHaul);
            Assert.IsNotNull(productHaul.ShippingLoadSheets);
            Assert.Greater(productHaul.ShippingLoadSheets.Count, 0);
            RigJob rigJob = new RigJob();
            Assert.IsNotNull(rigJob);
            (RigJobThirdPartyBulkerCrewSection assignment, Schedule schedule) = AssignThirdPartyCrewToProductHaul(productHaul, thridPartyBulkerCrew1, rigJob);
            int assignmentId = assignment.Id;
            globalAssignmentId = assignmentId;
            Assert.IsNotNull(assignment);
            Assert.IsNotNull(schedule);
            ThirdPartyBulkerCrewSchedule thirdPartyCrewSchedule = (ThirdPartyBulkerCrewSchedule)schedule;
            Assert.IsNotNull(thirdPartyCrewSchedule);
            assignment = AssignThirdPartyCrewOtherStatus(assignment, ProductHaulStatus.OnLocation, RigJobCrewSectionStatus.Returned);
            int thirdPartyCrewScheduleId = thirdPartyCrewSchedule.Id;
            globalScheduleId=thirdPartyCrewScheduleId;

            //2.Check if test data meet preconditions
            Assert.AreEqual(ProductHaulStatus.OnLocation, productHaul.ProductHaulLifeStatus);
            Assert.AreEqual(RigJobCrewSectionStatus.Returned, assignment.RigJobCrewSectionStatus);
            //3.Execute the method to test
            string result = eServiceWebContext.Instance.SetBulkerCrewStatusByCrewId(assignment.ThirdPartyBulkerCrew.Id, BulkerCrewStatus.Down, true, "TestSetThirdPartyBulkerCrewStatusDown");
            //4. Verify the result meets post-conditions
            Assert.AreEqual(result, "Succeed");
            var updatedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);
            Assert.IsNotNull(updatedProductHaul);
            Assert.IsNotNull(updatedProductHaul.ShippingLoadSheets);
            Assert.Greater(updatedProductHaul.ShippingLoadSheets.Count, 0);

            Assert.AreEqual(ProductHaulStatus.OnLocation, updatedProductHaul.ProductHaulLifeStatus);

            var updatedAssignment = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionById(assignmentId);
            Assert.AreEqual(updatedAssignment.Id, assignment.Id);
            Assert.AreEqual(RigJobCrewSectionStatus.Returned, updatedAssignment.RigJobCrewSectionStatus);

            var updatedSanjelCrewSchedule =
               eServiceOnlineGateway.Instance.GetThirdPartyCrewScheduleById(thirdPartyCrewScheduleId);
            Assert.IsNotNull(updatedSanjelCrewSchedule);

            //5. Clean up test data
            eServiceOnlineGateway.Instance.DeletethirdPartyBulkerCrewSchedule(updatedSanjelCrewSchedule.Id);
            eServiceOnlineGateway.Instance.DeleteRigJobThirdPartyBulkerCrewSection(updatedAssignment.Id);
            eServiceOnlineGateway.Instance.DeleteProductHaul(updatedProductHaul, true);

            //6. Double check all data are cleaned
            var cleanedSanjelCrewSchedule =
                eServiceOnlineGateway.Instance.GetThirdPartyCrewScheduleById(thirdPartyCrewScheduleId);
            Assert.IsNull(cleanedSanjelCrewSchedule);
            var cleanedAssignment = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionById(assignmentId);
            Assert.IsNull(cleanedAssignment);
            var cleanedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);
            Assert.IsNull(cleanedProductHaul);
        }
        #endregion

    }
}
