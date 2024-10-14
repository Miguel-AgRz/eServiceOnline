namespace eServiceOnline.Models.ProductHaul
{
    public class ScheduleProductHaulToRigBinViewModel:ScheduleProductHaulFromRigJobBlendViewModel
    {
        public ScheduleProductHaulToRigBinViewModel() : base()
        {

        }

        public int OrigBinInformationId { set; get; }
        public int OrigBinId { set; get; }
    }
}
