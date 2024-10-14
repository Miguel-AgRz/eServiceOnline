using System;
using eServiceOnline.Models.Commons;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule;
namespace eServiceOnline.Models.ProductHaul
{
    public class ProductLoadSectionModel: ModelBase<ProductLoadSection>
    {
        public ProductLoadSectionModel()
        {
            this.BlendChemicalModel = new BlendChemicalModel();
        }

        public int ProductHaulId { get; set; }
        public BlendChemicalModel BlendChemicalModel { get; set; }
        public double RequiredAmount { get; set; }
        public double LoadedAmount { get; set; }
        public double BagSize { get; set; }
        public double NumberOfBags { get; set; }
        public string Unit { get; set; }
        public string AdditionMethod { get; set; }
        public override void PopulateFrom(ProductLoadSection productLoadSection)
        {
            this.BlendChemicalModel.Id = productLoadSection.BlendChemical.Id;
            this.BlendChemicalModel.Name = productLoadSection.BlendChemical.Name;
            this.RequiredAmount = Math.Round(productLoadSection.RequiredAmount , 1, MidpointRounding.AwayFromZero);
            this.LoadedAmount = productLoadSection.LoadedAmount;
            this.BagSize = productLoadSection.BagSize;
            this.NumberOfBags = productLoadSection.NumberOfBags;
            this.Unit = productLoadSection.BlendAdditiveMeasureUnit.Description;
            this.AdditionMethod = productLoadSection.AdditiveBlendMethod.Name;
        }
    }
}
