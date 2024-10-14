using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule;
using Sesi.SanjelData.Entities.Common.BusinessEntities.WellSite;
using System;
using System.Collections.Generic;

namespace eServiceOnline.Models.ProductHaul
{
    public class RescheduleProductHaulLoadViewModel
    {
        public RescheduleProductHaulLoadViewModel()
        {

            ProductLoadInfoModel = new ProductLoadInfoModel();
        }

        public ProductLoadInfoModel ProductLoadInfoModel { set; get; }
        

        public List<PodLoad> PodLoadModels { get; set; }

        public int RigJobId { set; get; }
        public int ProductHaulLoadId { set; get; }

        public int OrigBulkPlantId { set; get; }
        public int OrigBinInformationId { set; get; }


        public string LoggedUser { set; get; }
       

    }
}
