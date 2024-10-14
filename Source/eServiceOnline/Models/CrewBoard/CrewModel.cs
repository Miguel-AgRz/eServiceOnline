using System;
using System.Collections.Generic;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Crew;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment;
using Sesi.SanjelData.Entities.Common.BusinessEntities.HumanResources;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Operation;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Organization;

namespace eServiceOnline.Models.CrewBoard
{
    public class CrewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int TruckUnitId { get; set; }
        public int PrimaryTruckUnitId { get; set; }
        public int SecondaryTruckUnitId { get; set; }
        public string TruckUnitNumber { get; set; }
        public int WorkerId { get; set; }
        public int SupervisorId { get; set; }
        public int CrewMemberId { get; set; }
        public int JobTypeId { get; set; }
        public string JobTypeName { get; set; }
        public string Notes { get; set; }
        public int CrewTypeId { get; set; }
        public string CrewTypeName { get; set; }
        public string CrewTypeDescription { get; set; }
        public int CrewPositionId { get; set; }
        public int HomeDistrictId { get; set; }
        public int WorkingDistrictId { get; set; }
        public DateTime CallCrewTime { get; set; }
        public int EstJobDuration { get; set; }
        public int RigJobId { get; set; }
        public string HomeDistrictName { get; set; }
        public string Rotation { get; set; }

        public void PopulateFrom(SanjelCrew entity)
        {
            this.Id = entity.Id;
            this.Name = entity.Name;
            this.Description = entity.Description;
            this.Rotation = entity.Rotation;
            if (entity.Type != null)
            {
                this.CrewTypeId = entity.Type.Id;
                this.CrewTypeName = entity.Type.Name;
                this.CrewTypeDescription = entity.Type.Description;
            }
            this.Notes = entity.Notes;
            if (entity.HomeServicePoint != null)
            {
                this.HomeDistrictName = entity.HomeServicePoint.Name;
            }
        }

        public void PopulateTo(SanjelCrew entity)
        {
            entity.Name = this.Name;
            entity.Description = this.Description;
            entity.Rotation = this.Rotation;
            entity.Type = new CrewType
            {
                Id = this.CrewTypeId,
                Name = this.CrewTypeName,
                Description = this.CrewTypeDescription
            };
            entity.Notes = this.Notes;
        }


        public void PopulateBulkerTo(SanjelCrew entity, string crewName)
        {
            entity.Name = crewName;

            entity.Description = this.Description;
            entity.Rotation = this.Rotation;
            entity.Type = new CrewType
            {
                Id = this.CrewTypeId,
                Name = this.CrewTypeName,
                Description = this.CrewTypeDescription
            };
            entity.Notes = this.Notes;
        }

        public void PopulateFrom(Crew entity)
        {
            this.Id = entity.Id;
            this.Name = entity.Name;
            this.Description = entity.Description;
            if (entity.Type != null)
            {
                this.CrewTypeId = entity.Type.Id;
                this.CrewTypeName = entity.Type.Name;
                this.CrewTypeDescription = entity.Type.Description;
            }
            this.Notes = entity.Notes;
        }


        public void PopulateFormarFrom(SanjelCrew entity, List<TruckUnit> unit, List<Employee> listEmployee)
        {
            this.Id = entity.Id;

            if (listEmployee != null)
            {
                int n = listEmployee.Count;

                if (n > 1)
                {
                    this.Name = listEmployee[0].FirstName + "/" + listEmployee[1].FirstName + "/" + unit[0].UnitNumber + "/" + unit[1].UnitNumber;
                }
                else if (n == 1)
                {
                    this.Name = listEmployee[0].FirstName + "/" + unit[0].UnitNumber + "/" + unit[1].UnitNumber;
                }
            }

            this.Description = entity.Description;
            if (entity.Type != null)
            {
                this.CrewTypeId = entity.Type.Id;
                this.CrewTypeName = entity.Type.Name;
                this.CrewTypeDescription = entity.Type.Description;
            }
            this.Notes = entity.Notes;
        }

    }
}