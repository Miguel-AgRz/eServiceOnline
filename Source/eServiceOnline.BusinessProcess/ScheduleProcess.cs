using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eServiceOnline.Gateway;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Crew;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ResourceSchedule;

namespace eServiceOnline.BusinessProcess
{
    public class ScheduleProcess
    {
        public static void DeleteSanjelCrewSchedule(SanjelCrewSchedule sanjelCrewSchedule)
        {
            if (sanjelCrewSchedule != null)
            {
                eServiceOnlineGateway.Instance.DeleteCrewSchedule(sanjelCrewSchedule.Id, true);
            }
        }

        public static void DeleteThirdPartyCrewSchedule(ThirdPartyBulkerCrewSchedule thirdPartyCrewSchedule)
        {
            if (thirdPartyCrewSchedule != null)
            {
                eServiceOnlineGateway.Instance.DeletethirdPartyBulkerCrewSchedule(thirdPartyCrewSchedule.Id);
            }
        }

        public static void UpdateSanjelCrewSchedule(SanjelCrewSchedule originalCrewSchedule)
        {
            if (originalCrewSchedule != null)
            {
                UpdateWorkerScheduleByCrewSchedule(originalCrewSchedule);
                UpdateUnitScheduleByCrewSchedule(originalCrewSchedule);
                eServiceOnlineGateway.Instance.UpdateSanjelCrewSchedule(originalCrewSchedule,true);
            }
        }

        private static void UpdateUnitScheduleByCrewSchedule(SanjelCrewSchedule sanjelCrewSchedule)
        {
            foreach (var unitSchedule in sanjelCrewSchedule.UnitSchedule)
            {
                unitSchedule.Description = sanjelCrewSchedule.Description;
                unitSchedule.StartTime = sanjelCrewSchedule.StartTime;
                unitSchedule.EndTime = sanjelCrewSchedule.EndTime;
            }
        }

        private static void UpdateWorkerScheduleByCrewSchedule(SanjelCrewSchedule sanjelCrewSchedule)
        {
            foreach (var workerSchedule in sanjelCrewSchedule.WorkerSchedule)
            {
                workerSchedule.Description = sanjelCrewSchedule.Description;
                workerSchedule.StartTime = sanjelCrewSchedule.StartTime;
                workerSchedule.EndTime = sanjelCrewSchedule.EndTime;
            }
        }

        public static void CreateCrewSchedule(SanjelCrewSchedule crewSchedule)
        {
            //create unit schedule
            List<SanjelCrewTruckUnitSection> truckUnitSections =
                eServiceOnlineGateway.Instance.GetTruckUnitSectionsByCrew(crewSchedule.SanjelCrew.Id);
            if(crewSchedule.UnitSchedule == null) crewSchedule.UnitSchedule = new List<UnitSchedule>();
            foreach (SanjelCrewTruckUnitSection truckUnitSection in truckUnitSections)
            {
                UnitSchedule unitSchedule = new UnitSchedule
                {
                    Name = crewSchedule.Name,
                    Description = crewSchedule.Description,
                    StartTime = crewSchedule.StartTime,
                    EndTime = crewSchedule.EndTime,
                    TruckUnit = truckUnitSection.TruckUnit,
                    WorkingServicePoint = crewSchedule.WorkingServicePoint,
                    Type = UnitScheduleType.Assigned
                };
                crewSchedule.UnitSchedule.Add(unitSchedule);
            }

            //create worker schedule
            List<SanjelCrewWorkerSection> workerSections =
                eServiceOnlineGateway.Instance.GetWorkerSectionsByCrew(crewSchedule.SanjelCrew.Id);
            if(crewSchedule.WorkerSchedule == null) crewSchedule.WorkerSchedule = new List<WorkerSchedule>();
            foreach (SanjelCrewWorkerSection workerSection in workerSections)
            {
                WorkerSchedule workerSchedule = new WorkerSchedule
                {
                    Name = crewSchedule.Name,
                    Description = crewSchedule.Description,
                    StartTime = crewSchedule.StartTime,
                    EndTime = crewSchedule.EndTime,
                    Worker = eServiceOnlineGateway.Instance.GetEmployeeById(workerSection.Worker.Id),
                    WorkingServicePoint = crewSchedule.WorkingServicePoint,
                    Type = WorkerScheduleType.Assigned
                };
                crewSchedule.WorkerSchedule.Add(workerSchedule);
            }

            eServiceOnlineGateway.Instance.InsertCrewSchedule(crewSchedule, true);
        }
    }
}
