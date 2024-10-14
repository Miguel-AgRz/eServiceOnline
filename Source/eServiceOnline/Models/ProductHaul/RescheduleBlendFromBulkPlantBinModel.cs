namespace eServiceOnline.Models.ProductHaul
{
    public class RescheduleBlendFromBulkPlantBinModel
    {
        public RescheduleBlendFromBulkPlantBinModel()
        {
            this.ProductLoadInfoModel = new ProductLoadInfoModel();
        }
        public int ProductHaulLoadId { set; get; }

        public ProductLoadInfoModel ProductLoadInfoModel { set; get; }
    }
}
