using Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment;

namespace eServiceOnline.Models.NubbinBoard
{
    public class NubbinModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int RigJobId { get; set; }
        public int CallSheetNumber { get; set; }
        public int ServicePointId { get; set; }
        public string Notes { get; set; }

        public void PopulateFrom(Nubbin entity)
        {
            this.Id = entity.Id;
            this.Name = entity.Name;
            this.Description = entity.Description;
        }

        public void PopulateTo(Nubbin entity)
        {
            entity.Id = this.Id;
            entity.Name = this.Name;
            entity.Description = this.Description;
        }
    }
}