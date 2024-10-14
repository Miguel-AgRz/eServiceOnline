using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule;
using System;
using System.Collections.Generic;

namespace eServiceOnline.Models.ProductHaul
{
    public class OnLocationProductHaulViewModel
    {
        public int ProductHaulId { set; get; }
        public int ProductHaulLoadId { set; get; }
        public string LoggedUser { set; get; }
        public DateTime OnLocationTime { set; get; }
        public bool IsSameLocation{set;get;}
        public List<ShippingLoadSheet> ShippingLoadSheetModels { set; get; }
    }
}
