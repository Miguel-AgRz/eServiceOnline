using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule;
using Sesi.SanjelData.Entities.BusinessEntities.WellSite;
using Sesi.SanjelData.Entities.Common.BusinessEntities.WellSite;
using System;
using System.Collections.Generic;
using eServiceOnline.Models.RigBoard;

namespace eServiceOnline.Models.ProductHaul
{
    public class HaulBackFromRigJobBinViewModel
    {
        public HaulBackFromRigJobBinViewModel()
        {
            ProductHaulInfoModel = new ProductHaulInfoModel();
            ShippingLoadSheetModel = new ShippingLoadSheetModel();
        }
        public ProductHaulInfoModel ProductHaulInfoModel { set; get; }
        public ShippingLoadSheetModel ShippingLoadSheetModel { set; get; }

        public List<BlendUnloadSheet> BlendUnloadSheetModels { set; get; }

        public HaulBackFromBinModel HaulBackFromBinModel { set; get; } = new HaulBackFromBinModel();

        public List<PodLoad> PodLoadModels { get; set; }
        public BulkPlantBinLoadModel BulkPlantBinLoadModel { get; set; } = new BulkPlantBinLoadModel();

        public int RigJobId { set; get; }
        public string LoggedUser { set; get; }
        public void PopulateToModel()
        {
            this.ShippingLoadSheetModel.BlendUnloadSheetModels = this.BlendUnloadSheetModels;
            this.ProductHaulInfoModel.PodLoadModels = this.PodLoadModels;
        }

        public void PopulateFromHaul()
        {
            this.ProductHaulInfoModel.EstimatedTravelTime = 4;
            this.ProductHaulInfoModel.EstimatedLoadTime = DateTime.Now;
            this.ProductHaulInfoModel.ExpectedOnLocationTime = DateTime.Now;
            this.ProductHaulInfoModel.PodLoadModels = new List<PodLoad>();
            for (var i = 0; i < 4; i++)
            {
                this.ProductHaulInfoModel.PodLoadModels.Add(new PodLoad()
                {
                    PodIndex = i,
                    LoadAmount = 0
                }); ;
            }
        }

        public void PopulateFromBinSection(BinInformation binInformation)
        {
            this.HaulBackFromBinModel.SourceBinInformationId = binInformation.Id;
            this.HaulBackFromBinModel.SourceBinInformationName = binInformation.Name;
            this.HaulBackFromBinModel.SourceAmount = binInformation.Quantity;
            this.HaulBackFromBinModel.SourceBaseBlend = binInformation.BlendChemical?.Name;
            this.HaulBackFromBinModel.SourceBlendChemicalDescription = binInformation.BlendChemical?.Description;
            this.HaulBackFromBinModel.sourceRigId = binInformation.Rig.Id;
            this.HaulBackFromBinModel.sourceRigName = binInformation.Rig.Name;
        }
    }
}
