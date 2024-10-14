using eServiceOnline.Models.Commons;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch;
using Sesi.SanjelData.Entities.Common.BusinessEntities.HumanResources;
using UnitSection = Sanjel.BusinessEntities.Sections.Common.UnitSection;
using Employee= Sanjel.Common.BusinessEntities.Reference.Employee;
namespace eServiceOnline.Models.UpcomingJobs
{
    public class UnitSectionModel : ModelBase<UnitSection>
    {
        public string DriverOneName { get; set; }
        public string DriverTwoName { get; set; }
        public string TractorUnitNumber { get; set; }
        public string TruckUnitNumber { get; set; }

        public override void PopulateFrom(UnitSection unitSection)
        {
            if (unitSection != null && unitSection.TruckUnit != null)
            {
                this.TruckUnitNumber = unitSection.TruckUnit.UnitNumber;
            }
            if (unitSection != null && unitSection.TractorUnit != null)
            {
                this.TractorUnitNumber = unitSection.TractorUnit.UnitNumber;
            }
            if (unitSection != null && unitSection.Operator1 != null)
            {
                this.DriverOneName = this.GetEmployeeShortName(unitSection.Operator1);
            }
            if (unitSection != null && unitSection.Operator2 != null)
            {
                this.DriverTwoName = this.GetEmployeeShortName(unitSection.Operator2);
            }
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