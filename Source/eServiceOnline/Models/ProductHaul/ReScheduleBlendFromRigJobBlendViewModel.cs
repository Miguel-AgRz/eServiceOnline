namespace eServiceOnline.Models.ProductHaul
{
    public class ReScheduleBlendFromRigJobBlendViewModel
    {
        public ReScheduleBlendFromRigJobBlendViewModel()
        {
            ProductLoadInfoModel = new ProductLoadInfoModel();
            ProductHaulInfoModel = new ProductHaulInfoModel();
        }
        public ProductLoadInfoModel ProductLoadInfoModel { set; get; }
        public ProductHaulInfoModel ProductHaulInfoModel { set; get; }


        public int RigJobId { set; get; }
        public int ProductHaulLoadId { set; get; }
        public int OrigBulkPlantId { set; get; }
        public int OrigBinInformationId { set; get; }   
        public string LoggedUser { set; get; }
    }
}
