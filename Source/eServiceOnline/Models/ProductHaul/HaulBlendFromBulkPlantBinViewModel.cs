using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule;
using Sesi.SanjelData.Entities.BusinessEntities.WellSite;
using Sesi.SanjelData.Entities.Common.BusinessEntities.WellSite;
using System;
using System.Collections.Generic;

namespace eServiceOnline.Models.ProductHaul
{
    public class HaulBlendFromBulkPlantBinViewModel
    {
        public HaulBlendFromBulkPlantBinViewModel()
        {
            ProductHaulInfoModel = new ProductHaulInfoModel();
            ProductLoadInfoModel = new ProductLoadInfoModel();
            ShippingLoadSheetModel = new ShippingLoadSheetModel();
        }
        public ProductHaulInfoModel ProductHaulInfoModel { set; get; }
        public ProductLoadInfoModel ProductLoadInfoModel { set; get; }
        public ShippingLoadSheetModel ShippingLoadSheetModel { set; get; }

        public List<BlendUnloadSheet> BlendUnloadSheetModels { set; get; }

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


        public void PopulateFromShippingLoadSheet(Rig rig, List<BinInformation> binInformations)
        {

            this.ShippingLoadSheetModel.RigName = rig.Name;
            this.ShippingLoadSheetModel.RigId = rig.Id;
            this.ShippingLoadSheetModel.BlendUnloadSheetModels = new List<BlendUnloadSheet>();
            foreach (var item in binInformations)
            {
                this.ShippingLoadSheetModel.BlendUnloadSheetModels.Add(new BlendUnloadSheet()
                {
                    UnloadAmount = 0,
                    DestinationStorage = item
                });
            }

        }

    }
}
