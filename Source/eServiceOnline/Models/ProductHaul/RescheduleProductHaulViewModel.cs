using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule;
using Sesi.SanjelData.Entities.Common.BusinessEntities.WellSite;
using System;
using System.Collections.Generic;
namespace eServiceOnline.Models.ProductHaul
{
    public class RescheduleProductHaulViewModel
    {
        public RescheduleProductHaulViewModel()
        {
            ProductHaulInfoModel = new ProductHaulInfoModel();
            PodLoadAndBendUnLoadModels = new List<PodLoadAndBendUnLoadModel>();
        }
        public ProductHaulInfoModel ProductHaulInfoModel { set; get; }


        public List<PodLoadAndBendUnLoadModel> PodLoadAndBendUnLoadModels { set; get; }
        public int RigJobId { set; get; }

        public int OriginalProductHaulId { set; get; }
        public string LoggedUser { set; get; }
       

        public void PopulateFromHaul(Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul productHaul)
        {
            this.ProductHaulInfoModel.IsGoWithCrew = productHaul.IsGoWithCrew;
            this.ProductHaulInfoModel.IsThirdParty = productHaul.IsThirdParty;
            this.ProductHaulInfoModel.CrewId = productHaul.Crew.Id;
            this.ProductHaulInfoModel.IsExistingHaul = true;
            this.ProductHaulInfoModel.BulkPlantId = productHaul.BulkPlant.Id;
            this.ProductHaulInfoModel.EstimatedTravelTime = productHaul.EstimatedTravelTime;
            this.ProductHaulInfoModel.EstimatedLoadTime = productHaul.EstimatedLoadTime;
            this.ProductHaulInfoModel.ExpectedOnLocationTime = productHaul.ExpectedOnLocationTime;
            this.ProductHaulInfoModel.ProductHaulId = productHaul.Id;

        }

    }
}
