using System;
using System.Collections.Generic;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Crew;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ResourceSchedule;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment;
using Sesi.SanjelData.Entities.Common.BusinessEntities.HumanResources;

namespace eServiceOnline.Models.Calendar
{
    public class ScheduleModel
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Location { get; set; }
        public string Comment { get; set; }
        public int OwnerId { get; set; }

        public int StatusId { get; set; }
        public string ScheduleTypeName { get; set; }
        public string StatusName { get; set; }
        public string ClientCompanyName { get; set; }
        public string RigName { get; set; }

        public string JobType { get; set; }

        public void PopulateFrom(SanjelCrewSchedule crewSchedule)
        {
            this.Id = crewSchedule.Id;
            this.Subject = crewSchedule.Name;
            this.StartTime = crewSchedule.StartTime;
            this.EndTime = crewSchedule.EndTime;
            this.Comment = crewSchedule.Description;
            if (crewSchedule?.RigJobSanjelCrewSection?.SanjelCrew != null)
            {
                this.OwnerId = crewSchedule.RigJobSanjelCrewSection?.SanjelCrew?.Id??0;
            }

        }
        public void PopulateTo(SanjelCrewSchedule crewSchedule)
        {
            crewSchedule.Id = this.Id;
            crewSchedule.Name = this.Subject;
            crewSchedule.StartTime = this.StartTime;
            crewSchedule.EndTime = this.EndTime;
            crewSchedule.Description = this.Comment;
//            crewSchedule.SanjelCrew=new SanjelCrew
//            {
//                Id = this.OwnerId
//            };
            
        }
        public void PopulateFrom(WorkerSchedule workerSchedule)
        {
            this.Id = workerSchedule.Id;
            this.Subject = workerSchedule.Name;
            this.StartTime = workerSchedule.StartTime;
            this.EndTime = workerSchedule.EndTime;
            this.Comment = workerSchedule.Description??workerSchedule.Name;
            this.OwnerId = workerSchedule.Worker.Id;
            this.StatusId = (int)workerSchedule.Type;
           
        }

        public void PopulateTo(WorkerSchedule workerSchedule)
        {
            workerSchedule.Id = this.Id;
            workerSchedule.Name = this.Subject;
            workerSchedule.StartTime = this.StartTime;
            workerSchedule.EndTime = this.EndTime;
            workerSchedule.Description = this.Comment;
            workerSchedule.Worker=new Employee
            {
                Id = this.OwnerId
            };
            workerSchedule.Type=(WorkerScheduleType)this.StatusId;
        }

        public void PopulateFrom(UnitSchedule unitSchedule)
        {
            this.Id = unitSchedule.Id;
            this.Subject = unitSchedule.Name;
            this.StartTime = unitSchedule.StartTime;
            this.EndTime = unitSchedule.EndTime;
            this.Comment = unitSchedule.Description??unitSchedule.Name;
            this.OwnerId = unitSchedule.TruckUnit.Id;
            this.StatusId = (int)unitSchedule.Type;
        }

        public void PopulateTo(UnitSchedule unitSchedule)
        {
            unitSchedule.Id = this.Id;
            unitSchedule.Name = this.Subject;
            unitSchedule.StartTime = this.StartTime;
            unitSchedule.EndTime = this.EndTime;
            unitSchedule.Description = this.Comment;
            unitSchedule.TruckUnit=new TruckUnit
            {
                Id = this.OwnerId
            };
            unitSchedule.Type = (UnitScheduleType)this.StatusId;
        }
    }

}