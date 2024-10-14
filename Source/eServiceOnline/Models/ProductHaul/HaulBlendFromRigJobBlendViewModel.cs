using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule;
using Sesi.SanjelData.Entities.BusinessEntities.WellSite;
using Sesi.SanjelData.Entities.Common.BusinessEntities.WellSite;
using System;
using System.Collections.Generic;

namespace eServiceOnline.Models.ProductHaul
{
    public class HaulBlendFromRigJobBlendViewModel
    {
        public HaulBlendFromRigJobBlendViewModel()
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

        public int RigJobId { set; get; }
        public string LoggedUser { set; get; }
        public void PopulateToModel()
        {
            this.ShippingLoadSheetModel.BlendUnloadSheetModels = this.BlendUnloadSheetModels;
            this.ProductHaulInfoModel.PodLoadModels = this.PodLoadModels;
        }

    }
}
