using eServiceOnline.Models.Commons;
using Sesi.SanjelData.Entities.Common.BusinessEntities.WellSite;
using Rig = Sesi.SanjelData.Entities.Common.BusinessEntities.WellSite.Rig;
using RigStatus = Sesi.SanjelData.Entities.Common.BusinessEntities.WellSite.RigStatus;
using RigSizeType = Sesi.SanjelData.Entities.Common.BusinessEntities.WellSite.RigSizeType;
using ValueUnitType=Sesi.SanjelData.Entities.Common.Entities.General.ValueUnitType;
using ThreadType = Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment.ThreadType;
namespace eServiceOnline.Models.RigBoard
{
    public class RigViewModel : ModelBase<Rig>
    {
        public int RigId { get; set; }
        public string Name { get; set; }
        public string RigNumber { get; set; }
        public int DrillingCompanyId { get; set; }
        public string DrillingCompanyName { get; set; }
        public RigStatus Status { get; set; }
        public bool IsServiceRig { get; set; }
        public bool IsProjectRig { get; set; }
        public bool IsDelete { get; set; }
        public bool IsTopDrive { get; set; }
//        public double SizeTypeValue { get; set; }
//        public string SizeTypeUnit { get; set; }
        public int SizeId { get; set; }
        public double SizeValue { get; set; }
        public int RigSizeTypeId { get; set; }
        public string RigSizeTypeName { get; set; }
        public int ThreadTypeId { get; set; }
        public string ThreadTypeName { get; set; }
        public int RigJobId { get; set; }

        public override void PopulateTo(Rig rig)
        {
            if (rig != null)
            {
                rig.Id = this.RigId;
                rig.Name = this.Name;
                rig.RigNumber = this.RigNumber;
                rig.Status = this.Status;
                rig.IsServiceRig = this.IsServiceRig;
                rig.IsProjectRig = this.IsProjectRig;
                rig.IsDeleted = this.IsDelete;
                rig.IsTopDrive = this.IsTopDrive;

                if (rig.DrillingCompany != null)
                {
                    rig.DrillingCompany.Id = this.DrillingCompanyId;
                    rig.DrillingCompany.Name = this.DrillingCompanyName;
                }
                else
                {
                    rig.DrillingCompany = new DrillingCompany() {Id = this.DrillingCompanyId, Name = this.DrillingCompanyName};
                }

                if (rig.Size != null)
                {
                    rig.Size.Value = this.SizeValue;
                    rig.Size.Id = this.SizeId;
                }
                else
                {
//                    rig.Size = new ValueUnitType() {Value = this.SizeTypeValue, Id = this.ValueUnitTypeId };
                    rig.Size = new RigSize {Value = this.SizeValue, Id = this.SizeId };
                }

                if (rig.RigSize != null)
                {
                    rig.RigSize.Id = this.RigSizeTypeId;
                    rig.RigSize.Name = this.RigSizeTypeName;
                }
                else
                {
                    rig.RigSize = new RigSizeType() {Id = this.RigSizeTypeId, Name = this.RigSizeTypeName};
                }

                if (rig.ThreadType != null)
                {
                    rig.ThreadType.Id = this.ThreadTypeId;
                    rig.ThreadType.Name = this.ThreadTypeName;
                }
                else
                {
                    rig.ThreadType = new ThreadType() {Id = this.ThreadTypeId, Name = this.ThreadTypeName};
                }
            }
        }

        public override void PopulateFrom(Rig rig)
        {
            if (rig != null)
            {
                this.RigId = rig.Id;
                this.Name = rig.Name;
                this.RigNumber = rig.RigNumber;
                this.Status = rig.Status;
                this.IsProjectRig = rig.IsProjectRig;
                this.IsServiceRig = rig.IsServiceRig;
                this.IsDelete = rig.IsDeleted;
                this.IsTopDrive = rig.IsTopDrive;

                if (rig.DrillingCompany != null)
                {
                    this.DrillingCompanyId = rig.DrillingCompany.Id;
                    this.DrillingCompanyName = rig.DrillingCompany.Name;
                }

                if (rig.Size != null)
                {
                    this.SizeValue = rig.Size.Value;
                    this.SizeId = rig.Size.Id;
                }

                if (rig.RigSize != null)
                {
                    this.RigSizeTypeId = rig.RigSize.Id;
                    this.RigSizeTypeName = rig.RigSize.Name;
                }

                if (rig.ThreadType != null)
                {
                    this.ThreadTypeId = rig.ThreadType.Id;
                    this.ThreadTypeName = rig.ThreadType.Name;
                }
            }
        }
    }
}