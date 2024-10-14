using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule;

namespace eServiceOnline.Models.ProductHaul
{
    public class CheckShipingLoadSheetModel
    {
        public bool IsChecked { get; set; }

        public bool IsReadOnly { set; get; }
        public ShippingLoadSheet ShippingLoadSheetModel { set; get; }
        public ProductHaulLoad ProductHaulLoadModel { set; get; }
    }
}
