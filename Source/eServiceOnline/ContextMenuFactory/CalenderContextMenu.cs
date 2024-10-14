using System;
using System.Collections.Generic;
using eServiceOnline.Data;
using eServiceOnline.Models.Calendar;

namespace eServiceOnline.ContextMenuFactory
{
    public class CalenderContextMenu:IContextMenuCommon
    {

        private const string Crew = "Crews";
        private const string Unit = "Units";
        private const string Worker = "Workers";
        private const string Add = "Add";
        private const string UpdateOrDelete = "UpdateOrDelete";
        private readonly bool _isEnabled=false;
        public List<string> Param;
        public CalenderContextMenu(List<string> param, bool isEnabled)
        {
            if (param[1].Equals(UpdateOrDelete))
            {
                _isEnabled = this.VerificationOwner(param[2], param[3]);
            }       
            Param = param;

        }


        private bool VerificationOwner(string type,string id)
        {
            if (type.Equals(Unit))
            {
              return  eServiceWebContext.Instance.GetUnitScheduleById(Int32.Parse(id))?.OwnerId != 0;
            }

            if (type.Equals(Worker))
            {
                return eServiceWebContext.Instance.GetWorkerScheduleById(Int32.Parse(id))?.OwnerId != 0;
            }
            else
            {
                return false;
            }
        }

        public  List<Models.Commons.ContextMenu> GetContextMenu()
        {

            ScheduleContextMenuModel menuModel = new ScheduleContextMenuModel();
            List<Models.Commons.ContextMenu> menus = new List<Models.Commons.ContextMenu>();
            if (Param[1].Equals(Add))
            {

                if (Param[2].Equals(Unit))
                {
                    menus.Add(menuModel.UnitAddContextMenu(Param[3], Param[4]));
                    return menus;
                }
                if (Param[2].Equals(Worker))
                {
                    menus.Add(menuModel.WorkAddContextMenu(Param[3], Param[4]));
                    return menus;
                }
            }

            if (Param[1].Equals(UpdateOrDelete))
            {
                if (Param[2].Equals(Crew))
                {
                    menus.Add(menuModel.CrewDetailContextMenu(Param[3]));
                    return menus;
                }
                if (Param[2].Equals(Unit))
                {
                    menus.Add(menuModel.UnitDetailContextMenu(Param[3]));
                    menus.Add(menuModel.UnitUpdateContextMenu(Param[3], _isEnabled));
                    menus.Add(menuModel.UnitDeleteContextMenu(Param[3], _isEnabled));
                    return menus;
                }
                if (Param[2].Equals(Worker))
                {
                    menus.Add(menuModel.WorkDetailContextMenu(Param[3]));
                    menus.Add(menuModel.WorkUpdateContextMenu(Param[3], _isEnabled));
                    menus.Add(menuModel.WorkDeleteContextMenu(Param[3], _isEnabled));
                    return menus;
                }
            }
            return menus;
        }
    }
}
