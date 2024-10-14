using eServiceOnline.Models.Commons;
using Sanjel.BusinessEntities.Sections.Header;
using Sanjel.Common.BusinessEntities.Reference;

namespace eServiceOnline.Models.RigBoard
{
    public class WellLocationInformationViewModel : ModelBase<WellLocationInformation>
    {
        public int CallSheetNumber { get; set; }
        public string DirectionToLocation { get; set; }
        public string DownHoleWellLocation { get; set; }
        public int DownHoleWellLocationTypeId { get; set; }
        public string DownHoleWellLocationTypeName { get; set; }
        public int WellLocationTypeId { get; set; }
        public string WellLocationTypeName { get; set; }
        public string WellLocation { get; set; }
        public int RigJobId { get; set; }

        public override void PopulateFrom(WellLocationInformation entity)
        {
            this.DirectionToLocation = entity.DirectionToLocation;
            this.WellLocation = entity.WellLocation;
            this.DownHoleWellLocation = entity.DownHoleWellLocation;
            if (entity.DownHoleWellLocationType != null)
            {
                this.DownHoleWellLocationTypeId = entity.DownHoleWellLocationType.Id;
                this.DownHoleWellLocationTypeName = entity.DownHoleWellLocationType.Description;
            }
            if (entity.WellLocationType != null)
            {
                this.WellLocationTypeId = entity.WellLocationType.Id;
                this.WellLocationTypeName = entity.WellLocationType.Description;
            }
        }

        public override void PopulateTo(WellLocationInformation entity)
        {
            entity.WellLocation = this.WellLocation;
            entity.DirectionToLocation = this.DirectionToLocation;
            entity.DownHoleWellLocation = this.DownHoleWellLocation;
            entity.DownHoleWellLocationType = new WellLocationType
            {
                Id = this.DownHoleWellLocationTypeId,
                Description = this.DownHoleWellLocationTypeName
            };
            entity.WellLocationType = new WellLocationType
            {
                Id = this.WellLocationTypeId,
                Description = this.WellLocationTypeName
            };
        }
    }
}