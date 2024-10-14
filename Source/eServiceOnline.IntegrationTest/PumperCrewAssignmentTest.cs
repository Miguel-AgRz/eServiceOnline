using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eServiceOnline.BusinessProcess;
using eServiceOnline.Data;
using eServiceOnline.Gateway;
using eServiceOnline.Models.CrewBoard;
using NUnit.Framework;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Crew;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ResourceSchedule;

namespace eServiceOnline.IntegrationTest
{
    public class PumperCrewAssignmentTest: IntegrationTestBase
    {
        private int crewId=0;
        private int callSheetId=0;
        private DateTime callCrewTime = DateTime.Now;
        private DateTime jobDateTime = DateTime.Now;
        private int rigJobId = 0;
        private int estJobDuration = 360;


        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestAssignPumperCrew()
        {
            var workingServicePointId = 0;
            var jobServicePointId = 0;

            #region Test Data Preparation
            
            //Get proper rig job -- Confirmed job for assigning a crew
            var latestConfirmedJob =
                eServiceOnlineGateway.Instance.GetRigJobsByQuery(null, p => p.JobLifeStatus == JobLifeStatus.Confirmed || p.JobLifeStatus == JobLifeStatus.Pending).OrderByDescending(p => p.Id).FirstOrDefault();
            Assert.IsNotNull(latestConfirmedJob);
            rigJobId = latestConfirmedJob.Id;
            callSheetId = latestConfirmedJob.CallSheetId;
            callCrewTime = latestConfirmedJob.CallCrewTime;
            estJobDuration = 4;
            jobDateTime = latestConfirmedJob.JobDateTime;
            jobServicePointId = latestConfirmedJob.ServicePoint.Id;

            var pumperCrewList = eServiceOnlineGateway.Instance.GetCrewList().FindAll(p => p.Type.Id == 1 && p.WorkingServicePoint.Id!=latestConfirmedJob.ServicePoint.Id && p.IsPrimaryCrew == true).OrderBy(p=>p.Description);
            Assert.Less(0, pumperCrewList.Count());

            var selectedCrew = pumperCrewList.FirstOrDefault();
            crewId = selectedCrew.Id;
            workingServicePointId = selectedCrew.WorkingServicePoint.Id;

            #endregion Test Data Preparation

            #region Set up model data
            
            CrewModel model = new CrewModel() {Id=crewId, RigJobId = rigJobId, CallCrewTime =latestConfirmedJob.JobLifeStatus == JobLifeStatus.Pending?DateTime.MinValue : callCrewTime, EstJobDuration = estJobDuration };

            #endregion Set up model data

            #region Run target method

            var jobDuration = eServiceWebContext.Instance.AssignACrew(model.Id, model.RigJobId, model.CallCrewTime, model.EstJobDuration);
            eServiceWebContext.Instance.UpdateCallSheetAndRigJob(model.RigJobId);

            #endregion  Run target method

            #region Get Actaul result

            RigJobSanjelCrewSection rigJobSanjelCrewSection = eServiceOnlineGateway.Instance.GetRigJobCrewSection(rigJobId, crewId);
            Assert.NotNull(rigJobSanjelCrewSection);
            Assert.AreEqual(0, rigJobSanjelCrewSection.ProductHaul.Id);

            var rigJob = eServiceOnlineGateway.Instance.GetRigJobById(rigJobId);


            var startTime = rigJob.JobLifeStatus == JobLifeStatus.Pending ? rigJob.JobDateTime : rigJob.CallCrewTime;
            var endTime = rigJob.JobDateTime.AddMinutes(rigJob.JobDuration);

            SanjelCrewSchedule crewSchedule =
                eServiceOnlineGateway.Instance.GetCrewScheduleByJobCrewSection(rigJobSanjelCrewSection.Id);
            Assert.NotNull(crewSchedule);
            Assert.Greater(1, Math.Abs((startTime-crewSchedule.StartTime).TotalSeconds));
            Assert.Greater(1, Math.Abs((endTime-crewSchedule.EndTime).TotalSeconds));
            Assert.AreEqual(selectedCrew.Id, crewSchedule.SanjelCrew.Id);
            Assert.AreEqual(jobServicePointId, crewSchedule.WorkingServicePoint.Id);

            //Make sure the working service point is not changed by job assignment
            SanjelCrew sanjelCrew = eServiceOnlineGateway.Instance.GetCrewById(crewId);
            Assert.AreEqual(workingServicePointId, sanjelCrew.WorkingServicePoint.Id);

            #endregion Get Actaul result
        }

        [Test]
        public void TestReleasePumperCrew()
        {
            List<int> assignmentIds = new List<int>();
            List<int> crewScheduleIds = new List<int>();
            List<int> unitScheduleIds = new List<int>();;
            List<int> workerScheduleIds = new List<int>();

            #region Test Data Preparation
            
            //Get proper rig job -- Confirmed job for assigning a crew
            var latestScheduledJobs =
                eServiceOnlineGateway.Instance.GetRigJobsByQuery(null, p => p.JobLifeStatus == JobLifeStatus.Scheduled).OrderByDescending(p => p.Id);
            Assert.IsNotNull(latestScheduledJobs);
            Assert.Less(0, latestScheduledJobs.Count());

            bool isFound = false;
            RigJob rigJob = null;


            foreach (var latestScheduledJob in latestScheduledJobs)
            {
                List<RigJobSanjelCrewSection> rigJobSanjelCrewSections = eServiceOnlineGateway.Instance.GetRigJobCrewSectionByRigJob(latestScheduledJob.Id);
                foreach (var rigJobSanjelCrewSection in rigJobSanjelCrewSections)
                {
                    if (rigJobSanjelCrewSection.ProductHaul == null || rigJobSanjelCrewSection.ProductHaul.Id == 0)
                    {
                        isFound = true;
                        FindScheduleIds(rigJobSanjelCrewSection,assignmentIds ,crewScheduleIds, unitScheduleIds, workerScheduleIds);
                    }
                    else
                    {
                        //Need to confirm if product haul needs to be canceled automatically
                        /*
                        var productHaul =
                            eServiceOnlineGateway.Instance.GetProductHaulById(rigJobSanjelCrewSection.ProductHaul.Id);
                        Assert.IsNotNull(productHaul);
                        if (!productHaul.IsGoWithCrew) continue;

                        isFound = true;
                        FindScheduleIds(rigJobSanjelCrewSection, assignmentIds, crewScheduleIds, unitScheduleIds, workerScheduleIds);
                    */
                    }

                }

                if (isFound)
                {
                    rigJob = latestScheduledJob;
                    break;
                }
            }

            rigJobId = rigJob.Id;
            callSheetId = rigJob.CallSheetId;

            #endregion Test Data Preparation

            #region Run target method

            eServiceWebContext.Instance.ReleaseCrew(rigJobId, false, DateTime.Now);

            #endregion Run target method

            #region Get Actaul result

            foreach (var assignmentId in assignmentIds)
            {
                var assignment = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(assignmentId);
                Assert.IsNull(assignment);
            }

            foreach (var assignmentId in crewScheduleIds)
            {
                var assignment = eServiceOnlineGateway.Instance.GetCrewScheduleById(assignmentId);
                Assert.IsNull(assignment);
            }
            foreach (var assignmentId in unitScheduleIds)
            {
                var assignment = eServiceOnlineGateway.Instance.GetUnitScheduleById(assignmentId);
                Assert.IsNull(assignment);
            }
            foreach (var assignmentId in workerScheduleIds)
            {
                var assignment = eServiceOnlineGateway.Instance.GetWorkerScheduleById(assignmentId);
                Assert.IsNull(assignment);
            }
            #endregion Get Actaul result

        }
        [Test]
        public void TestRescheduleJob()
        {
            List<int> assignmentIds = new List<int>();
            List<int> crewScheduleIds = new List<int>();
            List<int> unitScheduleIds = new List<int>();;
            List<int> workerScheduleIds = new List<int>();
            DateTime origJobDateTime = DateTime.Now;
            DateTime origCallCrewTime = DateTime.Now;
            DateTime rescheduledJobDateTime = DateTime.Now;
            DateTime rescheduledCallCrewTime = DateTime.Now;

            #region Test Data Preparation
            
            //Get proper rig job -- Confirmed job for assigning a crew
            var latestScheduledJobs =
                eServiceOnlineGateway.Instance.GetRigJobsByQuery(null, p => p.JobLifeStatus == JobLifeStatus.Scheduled).OrderByDescending(p => p.Id);
            Assert.IsNotNull(latestScheduledJobs);
            Assert.Less(0, latestScheduledJobs.Count());

            bool isFound = false;
            RigJob rigJob = null;


            foreach (var latestScheduledJob in latestScheduledJobs)
            {
                List<RigJobSanjelCrewSection> rigJobSanjelCrewSections = eServiceOnlineGateway.Instance.GetRigJobCrewSectionByRigJob(latestScheduledJob.Id);
                foreach (var rigJobSanjelCrewSection in rigJobSanjelCrewSections)
                {
                    if (rigJobSanjelCrewSection.ProductHaul == null || rigJobSanjelCrewSection.ProductHaul.Id == 0)
                    {
                        isFound = true;
                        FindScheduleIds(rigJobSanjelCrewSection,assignmentIds ,crewScheduleIds, unitScheduleIds, workerScheduleIds);
                    }
                    else
                    {
                        //Need to confirm if product haul needs to be canceled automatically
                        /*
                        var productHaul =
                            eServiceOnlineGateway.Instance.GetProductHaulById(rigJobSanjelCrewSection.ProductHaul.Id);
                        Assert.IsNotNull(productHaul);
                        if (!productHaul.IsGoWithCrew) continue;

                        isFound = true;
                        FindScheduleIds(rigJobSanjelCrewSection, assignmentIds, crewScheduleIds, unitScheduleIds, workerScheduleIds);
                    */
                    }

                }

                if (isFound)
                {
                    rigJob = latestScheduledJob;
                    break;
                }
            }

            rigJobId = rigJob.Id;
            callSheetId = rigJob.CallSheetId;
            origCallCrewTime = rigJob.CallCrewTime;
            origJobDateTime = rigJob.JobDateTime;
            rescheduledCallCrewTime = origCallCrewTime.AddHours(3);
            rescheduledJobDateTime = origJobDateTime.AddHours(4);

            #endregion Test Data Preparation

            #region Run target method

            rigJob.CallCrewTime = rescheduledCallCrewTime;
            rigJob.JobDateTime = rescheduledJobDateTime;

            eServiceWebContext.Instance.UpdateRigJob(rigJob, rigJob);
            eServiceWebContext.Instance.UpdateRigJobCrewSchedule(rigJob);

            #endregion Run target method

            #region Get Actaul result

            DateTime endTime = rescheduledJobDateTime.AddHours(rigJob.JobDuration == 0 ? 6 : rigJob.JobDuration / 60);

            foreach (var assignmentId in assignmentIds)
            {
                var assignment = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(assignmentId);
                Assert.IsNotNull(assignment);
            }

            foreach (var crewScheduleId in crewScheduleIds)
            {
                var sanjelCrewSchedule = eServiceOnlineGateway.Instance.GetCrewScheduleById(crewScheduleId);
                Assert.IsNotNull(sanjelCrewSchedule);
                Assert.Greater(1, Math.Abs((rescheduledCallCrewTime-sanjelCrewSchedule.StartTime).TotalSeconds));
                Assert.Greater(1, Math.Abs((endTime-sanjelCrewSchedule.EndTime).TotalSeconds));
            }
            foreach (var unitScheduleId in unitScheduleIds)
            {
                var unitSchedule = eServiceOnlineGateway.Instance.GetUnitScheduleById(unitScheduleId);
                Assert.IsNotNull(unitSchedule);
                Assert.Greater(1, Math.Abs((rescheduledCallCrewTime-unitSchedule.StartTime).TotalSeconds));
                Assert.Greater(1, Math.Abs((endTime-unitSchedule.EndTime).TotalSeconds));
            }
            foreach (var workerScheduleId in workerScheduleIds)
            {
                var workerSchedule = eServiceOnlineGateway.Instance.GetWorkerScheduleById(workerScheduleId);
                Assert.IsNotNull(workerSchedule);
                Assert.Greater(1, Math.Abs((rescheduledCallCrewTime-workerSchedule.StartTime).TotalSeconds));
                Assert.Greater(1, Math.Abs((endTime-workerSchedule.EndTime).TotalSeconds));
            }
            #endregion Get Actaul result

        }

        private static void FindScheduleIds(RigJobSanjelCrewSection rigJobSanjelCrewSection, List<int> assignmentIds, List<int> crewScheduleIds,
            List<int> unitScheduleIds, List<int> workerScheduleIds)
        {
            assignmentIds.Add(rigJobSanjelCrewSection.Id);
            var crewSchedule = eServiceOnlineGateway.Instance.GetCrewScheduleByJobCrewSection(rigJobSanjelCrewSection.Id, true);
            Assert.IsNotNull(crewSchedule);
            crewScheduleIds.Add(crewSchedule.Id);

            foreach (var unitSchedule in crewSchedule.UnitSchedule)
            {
                unitScheduleIds.Add(unitSchedule.Id);
            }

            foreach (var workerSchedule in crewSchedule.WorkerSchedule)
            {
                workerScheduleIds.Add(workerSchedule.Id);
            }
        }
    }
}
