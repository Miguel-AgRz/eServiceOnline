using System;
using System.Collections.Generic;

namespace eServiceOnline.ContextMenuFactory
{
    public class ContextMenuFactory
    {

        public static List<Models.Commons.ContextMenu> GetContextMenus(List<string> param,bool isEnabled=false)
        {
            if (string.IsNullOrEmpty(param[0]))
            {
                throw new  Exception("No assignment to the control type");
            }

            var b = Control.Calendar.ToString();
            if (param[0].Equals(Control.Calendar.ToString()))
            {
                return new CalenderContextMenu(param,isEnabled).GetContextMenu();
            }

            if (param[0].Equals(Control.Gird.ToString()))
            {
                return new DataGridContextMenu(param).GetContextMenu();
            }
            else
            {
                throw new Exception("This control is not supported");
            }
        }
        public enum Control
        {
            Calendar,
            Gird
        }
    }
}
