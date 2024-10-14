using Sesi.SanjelData.Entities.BusinessEntities.Operation.Crew;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment;
using Sesi.SanjelData.Entities.Common.BusinessEntities.HumanResources;

namespace eServiceOnline.Models.Calendar
{
    public class ScheduleStyleModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Color { get; set; }
        public string FontColor { get; set; }
        public void PopulateFrom(SanjelCrew crew)
        {
            this.Id = crew.Id;
            this.Text = crew.Description;
            this.Color = "#207722";
            this.FontColor = "#207722";
        }
        public void PopulateFrom(Employee employee)
        {
            this.Id = employee.Id;
            this.Text = $"{employee.LastName},{employee.PreferedFirstName}";
            this.Color = "#207722";
            this.FontColor = "#207722";
        }
        public void PopulateFrom(TruckUnit truckUnit)
        {
            this.Id = truckUnit.Id;
            this.Text = truckUnit.UnitNumber;
            this.Color = "#207722";
            this.FontColor = "#207722";
        }

    }
}