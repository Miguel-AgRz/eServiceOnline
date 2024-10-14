using System;
using RigJob = Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch.RigJob;

namespace eServiceOnline.Models.OperationBoard
{
    public class ScheduleBoardData
    {
        public int ProgramId { get; set; }
        public string ProgramName { get; set; }
        public string Comments { get; set; }
        public DateTime ProgramStartTime { get; set; }
        public DateTime ProgramEndTime { get; set; }
        public bool IsAllDay { get; set; }
        public bool IsRecurrence { get; set; }
        public string RecurrenceRule { get; set; }
        public string DownholeLocation { get; set; }
        public string Customer { get; set; }
        public string ScheduleBoardStyleId { get; set; }
        public string Categorize { get; set; }
        // RigJob transforms display.
        public void GetRigJobTransformDisplay(RigJob rigJob)
        {
            this.ProgramId = rigJob.Id;
            
            if (!string.IsNullOrWhiteSpace(rigJob.ClientCompany?.ShortName))
            {
                this.ProgramName = rigJob.ClientCompany?.ShortName;
            }
            if (!string.IsNullOrWhiteSpace(rigJob.WellLocation))
            {
                this.ProgramName += " " + rigJob.WellLocation;
            }
            this.ProgramName += " " + rigJob.JobType + " " + rigJob.JobLifeStatus;
            if (rigJob.JobDateTime != null) this.ProgramStartTime = (DateTime) rigJob.JobDateTime;
            if (this.ProgramStartTime != null) this.ProgramEndTime = this.ProgramStartTime.AddHours(1);
            this.Comments = rigJob.Notes;
            this.ScheduleBoardStyleId = ((int)rigJob.JobLifeStatus).ToString();
            this.Categorize = ((int)rigJob.JobLifeStatus).ToString();
            this.RecurrenceRule = "";
        }
    }
}
