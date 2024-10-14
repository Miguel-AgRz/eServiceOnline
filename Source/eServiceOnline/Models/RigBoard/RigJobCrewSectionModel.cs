using Sesi.SanjelData.Entities.BusinessEntities.Operation.Crew;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch;

namespace eServiceOnline.Models.RigBoard
{
    public class RigJobCrewSectionModel
    {
        public int Id { get; set; }
        public int CrewId { get; set; }
        public string CrewName { get; set; }
        public string CrewDescription { get; set; }
        public int JobCrewSectionStatusId { get; set; }
        public string JobCrewSectionStatusName { get; set; }
        public int RigJobId { get; set; }
        public string HomeDistrictName { get; set; }

        public void PopulateFrom(RigJobSanjelCrewSection entity)
        {
            this.Id = entity.Id;
            if (entity.SanjelCrew != null)
            {
                this.CrewId = entity.SanjelCrew.Id;
                this.CrewName = entity.SanjelCrew.Name;
                this.CrewDescription = entity.SanjelCrew.Description;
                if (entity.SanjelCrew.HomeServicePoint != null)
                {
                    this.HomeDistrictName = entity.SanjelCrew.HomeServicePoint.Name;
                }
            }

            this.JobCrewSectionStatusId = (int) entity.RigJobCrewSectionStatus;
//            this.JobCrewSectionStatusName = entity.RigJobCrewSectionStatus.Name;
        }

        public void PopulateTo(RigJobSanjelCrewSection entity)
        {
            entity.Id = this.Id;
            entity.SanjelCrew = new SanjelCrew
            {
                Id = this.CrewId,
                Name = this.CrewName,
                Description = this.CrewDescription
            };
            entity.RigJobCrewSectionStatus = (RigJobCrewSectionStatus) JobCrewSectionStatusId;

        }
    }
}