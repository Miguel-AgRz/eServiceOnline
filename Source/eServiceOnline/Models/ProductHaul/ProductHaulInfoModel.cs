using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using eServiceOnline.Models.Commons;
using eServiceOnline.Models.ModelBinder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule;

namespace eServiceOnline.Models.ProductHaul
{
    [ModelBinder(BinderType = typeof(CommonModelBinder<ProductHaulInfoModel>))]
    public class ProductHaulInfoModel
    {
        public bool IsExistingHaul { get; set; }
        public int ProductHaulId { get; set; }

        public bool IsGoWithCrew { get; set; }
        public DateTime EstimatedLoadTime {get ; set; }
        public DateTime ExpectedOnLocationTime { get; set; }
        public DateTime ExpectedOlTime { get; set; }
        public double EstimatedTravelTime { get; set; } = 4;
        public bool IsThirdParty{ get; set; }
        public int CrewId { get; set; }
        public int BulkPlantId { get; set; }
        public int ThirdPartyBulkerCrewId { get; set; }

        public List<PodLoad> PodLoadModels { get; set; }
        public int OrigCrewId { set; get; }
        public int OrigThirdPartyBulkerCrewId { set; get; }


        public void PopulateToProductHaul(Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul productHaul)
        {
            if (productHaul != null)
            {
//                productHaul.PodLoad = this.PodLoadModels;
                productHaul.BulkPlant = new Sesi.SanjelData.Entities.Common.BusinessEntities.WellSite.Rig() { Id = this.BulkPlantId };
                productHaul.Crew = new Sesi.SanjelData.Entities.Common.BusinessEntities.Operation.Crew { Id = this.CrewId };
                productHaul.IsGoWithCrew = this.IsGoWithCrew;
                productHaul.IsThirdParty = this.IsThirdParty;
                productHaul.EstimatedLoadTime = this.EstimatedLoadTime;
                productHaul.EstimatedTravelTime = this.EstimatedTravelTime;
                productHaul.ExpectedOnLocationTime = this.ExpectedOnLocationTime;
            }
        }

        public void InitializeProductHaulSchedule()
        {
            this.EstimatedTravelTime = 4;
            this.EstimatedLoadTime = DateTime.Now;
            this.ExpectedOnLocationTime = DateTime.Now;
            this.PodLoadModels = new List<PodLoad>();
            for (var i = 0; i < 4; i++)
            {
                this.PodLoadModels.Add(new PodLoad()
                {
                    PodIndex = i,
                    LoadAmount = 0
                }); ;
            }
        }

        public DateTime GetScheduleEndTime(RigJob rigJob)
        {
	        DateTime endTime;
	        if (this.IsGoWithCrew)
	        {    
		        endTime = rigJob.JobDateTime.AddMinutes(rigJob.JobDuration == 0 ? 360 : rigJob.JobDuration);
	        }
	        else
	        {
		        endTime = this.ExpectedOnLocationTime.AddHours(this.EstimatedTravelTime);
	        }

	        return endTime;
        }
    }
}
