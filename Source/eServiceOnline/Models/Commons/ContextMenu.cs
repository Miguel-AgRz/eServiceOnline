using System.Collections.Generic;

namespace eServiceOnline.Models.Commons
{
    public class ContextMenu
    {
        public string MenuName { get; set; }
        public ProcessingMode ProcessingMode { get; set; }
        public List<string> Parms { get; set; }
        //maybe this
//        public List<object> Parms { get; set; }
        public string DialogName { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public bool IsHaveSplitLine { get; set; }
        public bool IsDisabled { get; set; }
        public  List<ContextMenu> MenuList { get; set; }
        public string MenuStyle { get; set; }
        public string MenuTips { get; set; }
        public ContextMenu()
        {
            this.IsHaveSplitLine = false;
            this.IsDisabled = false;
            this.MenuStyle = "";
        }
    }


    public enum ProcessingMode
    {
        NoAction = 0,
        NoPopsUpWindow = 1,
        PopsUpWindow = 2,
        HaveNextMenu=3,
        OpenInNewTab=4
    }
}