namespace eServiceOnline.Models.Commons
{
    public class NoteModel
    {
        public int Id { get; set; }
        public string Notes { get; set; }
        public int PodIndex { get; set; }
        public string ReturnActionName { get; set; }
        public string ReturnControllerName { get; set; }
        public string CallingControllerName { get; set; }
        public string CallingMethodName { get; set; }

        public string PostControllerName { get; set; }
        public string PostMethodName { get; set; }


    }
}