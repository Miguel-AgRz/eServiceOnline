using Sesi.SanjelData.Entities.Common.BusinessEntities.HumanResources;

namespace eServiceOnline.Models.WorkerBoard
{
    public class EmployeeModel
    {
        public int Id { get; set; }
        public string PreferedName { get; set; }
        public string Notes { get; set; }

        public void PopulateFrom(Employee employee)
        {
            if (employee != null)
            {
                this.Id = employee.Id;
                this.PreferedName = $"{employee.Name}";
                this.Notes = employee.Description;
            }
        }
    }
}
