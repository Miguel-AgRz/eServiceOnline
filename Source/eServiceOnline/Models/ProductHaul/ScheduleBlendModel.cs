namespace eServiceOnline.Models.ProductHaul
{
    public class ScheduleBlendModel
    {
        public ScheduleBlendModel()
        {
            ProductHaulInfoModel = new ProductHaulInfoModel();
            ProductLoadInfoModel = new ProductLoadInfoModel();
        }
        public int RigJobId { get; set; }

        public int ProductHaulLoadId { get; set; }

        public int OrigBulkPlantId { get; set; }
        public int OrigBinInformationId { set; get; }
        public ProductHaulInfoModel ProductHaulInfoModel { set; get; }
        public ProductLoadInfoModel ProductLoadInfoModel { set; get; }

    }
}
