namespace eServiceOnline.Models.RigBoard
{
    public class AssginPlugLoadingHeadModel
    {
        public int RigJobId { get; set; }
        public int CallSheetNumber { get; set; }
        public int PlugLoadingHeadId { get; set; }
        public int ManifoldId { get; set; }
        public int TopDriveAdapterId { get; set; }
        public int PlugLoadingHeadSubId { get; set; }

        public bool TopDriveAdapterRequired { get; set; }
        public bool ManifoldRequired { get; set; }
        public bool PlugLoadingHeadSubRequired { get; set; }

        public int ServicePointId { get; set; }
    }
}