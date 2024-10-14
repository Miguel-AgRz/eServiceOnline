using eServiceOnline.Models.Commons;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Products;

namespace eServiceOnline.Models.ProductHaul
{
    public class ProductHaulLoadModel : ModelBase<ProductHaulLoad>
    {
        public int ProductHaulLoadId { get; set; }
        public int ProductHaulId { get; set; }
        public int CallSheetNumber { get; set; }
        public string RigName { get; set; }
        public int RigId { get; set; }
        public string Category { get; set; }
        public string BaseBlend { get; set; }
        public string BaseBlendName { get; set; }
        public double Amount { get; set; }
        public string BulkUnitName { get; set; }
        public string TractorUnitName { get; set; }
        public string Driver { get; set; }
        public string BinNumber { get; set; }
        public int BinId { get; set; }
        public int PodIndex { set; get; }

        public double MixWater { get; set; }
        public double Yield { get; set; }
        public double Density { get; set; }
        public double BulkVolume { get; set; }
        public double SackWeight { get; set; }
        public string ServicePointName { get; set; }
        public string BulkPlantName { get; set; }
        public int BulkPlantId { get; set; }
        public ProductHaulLoadStatus Status { get; set; }
        public BlendShippingStatus BlendShippingStatus { get; set; }

        public ShippingStatus ShippingStatus { get; set; }

        public BlendTestingStatus BlendTestingStatus { get; set; }

        public double HaulAmount { set; get; }

        public int BinInformationId { set; get; }
        public string BinInformationName { set; get; }
        public bool IsTotalBlendTonnage { get; set; }
        public double TotalBlendWeight { get; set; }
        public double BaseBlendWeight { get; set; }
        public int BlendSectionId { get; set; }
        public bool IsBlendTest { get; set; }
        public string StatusString { get; set; }

        public override void PopulateFrom(ProductHaulLoad productHaulLoad)
        {
            if (productHaulLoad != null)
            {
                this.ProductHaulLoadId = productHaulLoad.Id;
                this.CallSheetNumber = productHaulLoad.CallSheetNumber;
                BlendChemical baseBlend = productHaulLoad.BlendChemical;
                this.BaseBlend = baseBlend == null ? "" : (string.IsNullOrWhiteSpace(baseBlend.Description) ? baseBlend.Name : baseBlend.Description);
                this.BaseBlendName = baseBlend == null ? "" : (string.IsNullOrWhiteSpace(baseBlend.Name) ? string.Empty : baseBlend.Name);
                this.Category = productHaulLoad.BlendCategory == null ? "" : (string.IsNullOrEmpty(productHaulLoad.BlendCategory.Description) ? productHaulLoad.BlendCategory.Name : productHaulLoad.BlendCategory.Description);
                this.IsTotalBlendTonnage = productHaulLoad.IsTotalBlendTonnage;
                this.TotalBlendWeight = productHaulLoad.TotalBlendWeight;
                this.BaseBlendWeight = productHaulLoad.BaseBlendWeight;
                this.IsBlendTest = productHaulLoad.IsBlendTest;

                double enteredBlendWeight = productHaulLoad.IsTotalBlendTonnage ? productHaulLoad.TotalBlendWeight : productHaulLoad.BaseBlendWeight;
                this.Amount = enteredBlendWeight / 1000;
               
                this.Yield = productHaulLoad.Yield;
                this.Density = productHaulLoad.Density;
                this.MixWater = productHaulLoad.MixWater;
                this.BulkVolume = productHaulLoad.BulkVolume;
                this.SackWeight = productHaulLoad.SackWeight;
                this.PodIndex = productHaulLoad.PodIndex;
                if (productHaulLoad.Bin != null)
                {
                    this.BinId = productHaulLoad.Bin.Id;
                    this.BinNumber = productHaulLoad.Bin.Name;
                }

                this.BulkPlantName = productHaulLoad.BulkPlant.Name;
                this.BulkPlantId = productHaulLoad.BulkPlant.Id;
                this.ServicePointName = productHaulLoad.ServicePoint.Name;
                this.Status = productHaulLoad.ProductHaulLoadLifeStatus;
                this.BlendShippingStatus = productHaulLoad.BlendShippingStatus;
                this.BlendTestingStatus = productHaulLoad.BlendTestingStatus;
                this.BlendSectionId = productHaulLoad.BlendSectionId;
                this.StatusString = this.Status.ToString();
            }
        }
    }
}