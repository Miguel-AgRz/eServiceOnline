using Sesi.SanjelData.Entities.BusinessEntities.BaseEntites;

namespace eServiceOnline.Models.ProductHaul
{
    public class BlendSectionModel
    {
        public int Id { get; set; }
        public int CallSheetNumber { get; set; }
        public string Blend { get; set; }
        public string Category { get; set; }
        public double Amount { get; set; }
        public double Sent { get; set; }
        public double Remains { get; set; }

        public double MixWater { get; set; }
        public double SackWeight { get; set; }
        public double Yield { get; set; }
        public double Density { get; set; }
        public bool IsBlendTest { get; set; }
        public void PopulateFrom(BlendSection blendSection)
        {
            if (blendSection != null)
            {
                this.Id = blendSection.Id;
                this.Blend = blendSection.BlendFluidType.Name;
                this.Amount = blendSection.Quantity??0;
                this.Category = blendSection.BlendCategory == null ? " " : (string.IsNullOrEmpty(blendSection.BlendCategory.Description) ? " ": blendSection.BlendCategory.Description);
                this.MixWater = blendSection.MixWaterRequirement;
                this.SackWeight = blendSection.SackWeight;
                this.Yield = blendSection.Yield;
                this.Density = blendSection.Density;
                this.IsBlendTest = blendSection.IsNeedFieldTesting;
            }
        }
    }
}
