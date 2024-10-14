using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule;
using System;

namespace eServiceOnline.Models.ProductHaul
{
    public class OnLocationProductHaulLoadViewModel
    {
        public int ProductHaulLoadId { set; get; }

        public int ShippingLoadSheetId { set; get; }
        public DateTime OnLocationTime { set; get; }
        public string LoggedUser { set; get; }
        public ProductHaulLoad ProductHaulLoadModel { set; get; }

        public ShippingLoadSheet ShippingLoadSheetModel { set; get; }
    }
}
