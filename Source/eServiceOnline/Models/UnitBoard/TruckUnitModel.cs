using Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment;

namespace eServiceOnline.Models.UnitBoard
{
    public class TruckUnitModel
    {
        public int Id { get; set; }
        public string UnitNumber { get; set; }

        public void PopulateFrom(TruckUnit truckUnit)
        {
            if (truckUnit != null)
            {
                this.Id = truckUnit.Id;
                this.UnitNumber = truckUnit.UnitNumber;
            }
        }
    }
}