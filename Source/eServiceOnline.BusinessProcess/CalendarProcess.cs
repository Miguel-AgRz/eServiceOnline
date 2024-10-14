using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using eServiceOnline.Gateway;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ResourceSchedule;

namespace eServiceOnline.BusinessProcess
{
    public class CalendarProcess
    {
        public static List<SanjelCrewSchedule> GetCrewSchedules()
        {
            List<SanjelCrewSchedule> crewSchedules = eServiceOnlineGateway.Instance.GetCrewSchedules();
            foreach (SanjelCrewSchedule crewSchedule in crewSchedules)
            {
                crewSchedule.RigJobSanjelCrewSection = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(crewSchedule.RigJobSanjelCrewSection.Id);
            }
            return crewSchedules;
        }

        public static List<SanjelCrewSchedule> GetCrewSchedules(Collection<int> servicePoints, DateTime startTime, DateTime endTime)
        {
            List<SanjelCrewSchedule> crewSchedules = eServiceOnlineGateway.Instance.GetCrewSchedules(servicePoints, startTime,endTime);
            foreach (SanjelCrewSchedule crewSchedule in crewSchedules)
            {
                crewSchedule.RigJobSanjelCrewSection = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(crewSchedule.RigJobSanjelCrewSection.Id);
            }
            return crewSchedules;
        }

        public static SanjelCrewSchedule GetCrewScheduleById(int id)
        {
            SanjelCrewSchedule crewSchedule = eServiceOnlineGateway.Instance.GetCrewScheduleById(id);
            return crewSchedule;
        }

        public static UnitSchedule GetUnitScheduleById(int id)
        {
            UnitSchedule unitSchedule = eServiceOnlineGateway.Instance.GetUnitScheduleById(id);
            return unitSchedule;
        }

        public static WorkerSchedule GetWorkerScheduleById(int id)
        {
            WorkerSchedule workerSchedule = eServiceOnlineGateway.Instance.GetWorkerScheduleById(id);
            return workerSchedule;
        }

        public static int DeleteCrewSchedule(int id)
        {
            return eServiceOnlineGateway.Instance.DeleteCrewSchedule(id);
        }

        public static int DeleteUnitSchedule(int id)
        {
            return eServiceOnlineGateway.Instance.DeleteUnitSchedule(id);
        }

        public static int DeleteWorkSchedule(int id)
        {
            return eServiceOnlineGateway.Instance.DeleteWorkSchedule(id);
        }

        public static int UpdateCrewSchedule(SanjelCrewSchedule crewSchedule)
        {
            return eServiceOnlineGateway.Instance.UpdateSanjelCrewSchedule(crewSchedule);
        }

        public static int UpdateWorkerSchedule(WorkerSchedule workerSchedule)
        {
            return eServiceOnlineGateway.Instance.UpdateWorkerSchedule(workerSchedule);
        }

        public static int UpdateUnitSchedule(UnitSchedule unitSchedule)
        {
            return eServiceOnlineGateway.Instance.UpdateUnitSchedule(unitSchedule);
        }

        public static int InsertCrewSchedule(SanjelCrewSchedule crewSchedule)
        {
            return eServiceOnlineGateway.Instance.InsertCrewSchedule(crewSchedule);
        }

        public static int InsertWorkerSchedule(WorkerSchedule workerSchedule)
        {
            return eServiceOnlineGateway.Instance.InsertWorkerSchedule(workerSchedule);
        }

        public static int InsertUnitSchedule(UnitSchedule unitSchedule)
        {
            return eServiceOnlineGateway.Instance.InsertUnitSchedule(unitSchedule);
        }

        /*
        public static List<UnitScheduleType> GetListUnitScheduleTypes()
        {
            return eServiceOnlineGateway.Instance.UnitScheduleTypes();
        }
        */

        /*
        public static List<WorkerScheduleType> GetListWorkerScheduleTypes()
        {
            return eServiceOnlineGateway.Instance.WorkerScheduleTypes();
        }
        */

        public static List<SanjelCrewSchedule> GetCrewScheduleByCrewId(int crewId)
        {
            List<SanjelCrewSchedule> crewSchedules = eServiceOnlineGateway.Instance.GetCrewScheduleByCrewId(crewId);
          crewSchedules = crewSchedules.FindAll(s => s.EndTime > DateTime.Now).ToList();
            if (crewSchedules.Count != 0)
            {
                foreach (var crewSchedule in crewSchedules)
                {
                    crewSchedule.RigJobSanjelCrewSection = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(crewSchedule?.RigJobSanjelCrewSection?.Id ?? 0);
                    if (crewSchedule.RigJobSanjelCrewSection != null)
                    {
                        crewSchedule.RigJobSanjelCrewSection.RigJob = eServiceOnlineGateway.Instance.GetRigJobById(crewSchedule.RigJobSanjelCrewSection?.RigJob?.Id ?? 0);
                    }
                }
            }
            return crewSchedules.OrderBy(p => p.EndTime).ToList();
        }

    }
}