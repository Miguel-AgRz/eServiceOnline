using System.Collections.Generic;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Crew;
using Sesi.SanjelData.Entities.Common.BusinessEntities.HumanResources;

namespace eServiceOnline.Models.UpcomingJobs
{
    public class UnitAndWorkerModel
    {
        public string DriverOneName { get; set; }
        public string DriverTwoName { get; set; }
        public string TractorUnitNumber { get; set; }
        public string TruckUnitNumber { get; set; }

        public void PopulateFrom(List<SanjelCrewWorkerSection> crewWorkerSections)
        {
            if (crewWorkerSections != null && crewWorkerSections.Count > 0)
                this.DriverOneName = this.GetEmployeeShortName(crewWorkerSections[0].Worker);
            if (crewWorkerSections != null && crewWorkerSections.Count > 1)
                this.DriverTwoName = this.GetEmployeeShortName(crewWorkerSections[1].Worker);
        }

        public void PopulateFrom(List<SanjelCrewTruckUnitSection> crewTruckUnitSections)
        {
            if (crewTruckUnitSections != null && crewTruckUnitSections.Count > 0)
                this.TruckUnitNumber = crewTruckUnitSections[0].TruckUnit.UnitNumber;
            if (crewTruckUnitSections != null && crewTruckUnitSections.Count > 1)
                this.TractorUnitNumber = crewTruckUnitSections[1].TruckUnit.UnitNumber;
        }

        private string GetEmployeeShortName(Employee employee)
        {
            string shortName;
            if (string.IsNullOrEmpty(employee.FirstName))
                shortName = employee.Name;
            else
                shortName = employee.FirstName + " " + employee.LastName.Substring(0, 1);

            return shortName;
        }

    }
}