using System.Collections.Generic;

namespace eServiceOnline.ContextMenuFactory
{
    public class DataGridContextMenu:IContextMenuCommon
    {
        public List<string> Param;
        public DataGridContextMenu(List<string> param)
        {
            this.Param = param;
        }

        public  List<Models.Commons.ContextMenu> GetContextMenu()
        {
            return null;
        }
    }
}
