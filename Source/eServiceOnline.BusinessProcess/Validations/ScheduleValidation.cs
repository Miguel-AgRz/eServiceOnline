using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eServiceOnline.Gateway;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Crew;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ResourceSchedule;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment;
using Sesi.SanjelData.Entities.Common.BusinessEntities.HumanResources;
using Sesi.SanjelData.Entities.Common.Model.Scheduling;

namespace eServiceOnline.BusinessProcess
{
    public class ScheduleValidation
    {
        public static Schedule ValidateSchedule(DateTime startTime, DateTime endTime, IEnumerable<Schedule> schedules)
        {
            Schedule conflictSchedule = null;
            List<Schedule> conflictSchedules = schedules.ToList().FindAll(p => (p.StartTime >= startTime && p.StartTime < endTime) || (p.EndTime > startTime && p.EndTime <= endTime) || (p.StartTime <= startTime && p.EndTime >= endTime));
            if (conflictSchedules.Count > 0)
            {
                conflictSchedules = conflictSchedules.OrderBy(p => p.StartTime).ToList();
                conflictSchedule = conflictSchedules.First();
            }

            return conflictSchedule;
        }
    }

    public class UnitScheduleValidation : ScheduleValidation
    {
        public static string ValidateUnitSchedule(DateTime startTime, DateTime endTime, List<UnitSchedule> unitSchedules)
        {
            string messageInfo = string.Empty;
            UnitSchedule unitSchedule = ValidateSchedule(startTime, endTime, unitSchedules) as UnitSchedule;
            if (unitSchedule != null)
            {
                TruckUnit truckUnit = eServiceOnlineGateway.Instance.GetTruckUnitById(unitSchedule.TruckUnit.Id);
                messageInfo = $"{truckUnit?.UnitNumber} has been scheduled to another task from {unitSchedule.StartTime} and {unitSchedule.EndTime}"+ $".Your new schedule is from {startTime} to {endTime}.Do you want to continue?"; 
            }

            return messageInfo;
        }
    }

    public class WorkerScheduleValidation : ScheduleValidation
    {
        public static string ValidateWorkerSchedule(DateTime startTime, DateTime endTime, List<WorkerSchedule> workerSchedules)
        {
            string messageInfo = string.Empty;
            WorkerSchedule workerSchedule = ValidateSchedule(startTime, endTime, workerSchedules) as WorkerSchedule;
            if (workerSchedule != null)
            {
                Employee worker = eServiceOnlineGateway.Instance.GetEmployeeById(workerSchedule.Worker.Id);
                messageInfo = $"{worker.LastName}, {worker.FirstName} has been scheduled to another task from {workerSchedule.StartTime} and {workerSchedule.EndTime}" + $".Your new schedule is from {startTime} to {endTime}.Do you want to continue?";
            }

            return messageInfo;
        }
    }

    public class ThirdPartyBulkerCrewScheduleValidation : ScheduleValidation
    {
        public static string ValidateThirdPartyBulkerCrewSchedule(DateTime startTime, DateTime endTime, List<ThirdPartyBulkerCrewSchedule> thirdPartyBulkerCrewSchedules)
        {
            string messageInfo = string.Empty;
            ThirdPartyBulkerCrewSchedule thirdPartyBulkerCrewSchedule = ValidateSchedule(startTime, endTime, thirdPartyBulkerCrewSchedules) as ThirdPartyBulkerCrewSchedule;
            if (thirdPartyBulkerCrewSchedule != null)
            {
                ThirdPartyBulkerCrew thirdPartyBulkerCrew = eServiceOnlineGateway.Instance.GetThirdPartyBulkerCrewById(thirdPartyBulkerCrewSchedule.ThirdPartyBulkerCrew.Id);
                messageInfo = $"{thirdPartyBulkerCrew.Description} has been scheduled to another task from {thirdPartyBulkerCrewSchedule.StartTime} and {thirdPartyBulkerCrewSchedule.EndTime}" + $".Your new schedule is from {startTime} to {endTime}.Do you want to continue?";
            }

            return messageInfo;
        }
    }

    public class SanjelCrewScheduleValidation : ScheduleValidation
    {
        public static string ValidateSanjelCrewSchedule(DateTime startTime, DateTime endTime, List<SanjelCrewSchedule> sanjelCrewSchedules, int sanjelCrewId)
        {
            string messageInfo = string.Empty;
            SanjelCrewSchedule sanjelCrewSchedule = ValidateSchedule(startTime, endTime, sanjelCrewSchedules) as SanjelCrewSchedule;
            if (sanjelCrewSchedule != null)
            {
                SanjelCrew sanjelCrew = eServiceOnlineGateway.Instance.GetCrewById(sanjelCrewSchedule.SanjelCrew.Id);
                messageInfo = $"{sanjelCrew.Description} has been scheduled to another task from {sanjelCrewSchedule.StartTime} and {sanjelCrewSchedule.EndTime}" + $".Your new schedule is from {startTime} to {endTime}.Do you want to continue?";

                return messageInfo;
            }
            else
            {
                List<SanjelCrewWorkerSection> sanjelCrewWorkerSections = eServiceOnlineGateway.Instance.GetWorkerSectionsByCrew(sanjelCrewId);
                foreach (SanjelCrewWorkerSection sanjelCrewWorkerSection in sanjelCrewWorkerSections)
                {
                    List<WorkerSchedule> workerSchedules = eServiceOnlineGateway.Instance.GetWorkerSchedulesByWorker(sanjelCrewWorkerSection.Worker.Id);
                    WorkerSchedule workerSchedule = ValidateSchedule(startTime, endTime, workerSchedules) as WorkerSchedule;
                    if (workerSchedule != null)
                    {
                        Employee worker = eServiceOnlineGateway.Instance.GetEmployeeById(workerSchedule.Worker.Id);
                        messageInfo = $"{worker.LastName}, {worker.FirstName} has been scheduled to another task from {workerSchedule.StartTime} and {workerSchedule.EndTime}" + $".Your new schedule is from {startTime} to {endTime}.Do you want to continue?";

                        return messageInfo;
                    }
                }

                List<SanjelCrewTruckUnitSection> sanjelCrewTruckUnitSections = eServiceOnlineGateway.Instance.GetTruckUnitSectionsByCrew(sanjelCrewId);
                foreach (SanjelCrewTruckUnitSection sanjelCrewTruckUnitSection in sanjelCrewTruckUnitSections)
                {
                    List<UnitSchedule> unitSchedules = eServiceOnlineGateway.Instance.GetUnitSchedulesByTruckUnit(sanjelCrewTruckUnitSection.TruckUnit.Id);
                    UnitSchedule unitSchedule = ValidateSchedule(startTime, endTime, unitSchedules) as UnitSchedule;
                    if (unitSchedule != null)
                    {
                        TruckUnit truckUnit = eServiceOnlineGateway.Instance.GetTruckUnitById(unitSchedule.TruckUnit.Id);
                        messageInfo = $"{truckUnit?.UnitNumber} has been scheduled to another task from {unitSchedule.StartTime} and {unitSchedule.EndTime}" + $".Your new schedule is from {startTime} to {endTime}.Do you want to continue?";

                        return messageInfo;
                    }
                }
            }

            return messageInfo;
        }
    }

}
