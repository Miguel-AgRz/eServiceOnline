namespace eServiceOnline.Models.Commons
{
    public class ProfileModel
    {
        public int Id { get; set; }
        public string Profile { get; set; }
        public string ReturnActionName { get; set; }
        public string ReturnControllerName { get; set; }
        public string CallingControllerName { get; set; }
        public string CallingMethodName { get; set; }

        public string PostControllerName { get; set; }
        public string PostMethodName { get; set; }
    }
}