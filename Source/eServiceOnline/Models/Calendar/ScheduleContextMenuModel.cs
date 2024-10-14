using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eServiceOnline.Models.Commons;

namespace eServiceOnline.Models.Calendar
{
    public class ScheduleContextMenuModel
    {

        public const string AddWorkEvent = "Add Worker Schedule";
        public const string UpdateWorkEvent = "Update Worker Schedule";
        public const string DeleteWorkEvent = "Cancel Worker Schedule";
        public const string DetailWorkEvent = "View Worker Schedule";

        public const string AddCrewEvent = "Add Crew Schedule";
        public const string UpdateCrewEvent = "Update Crew Schedule";
        public const string DeleteCrewEvent = "Cancel Crew Schedule";
        public const string DetailCrewEvent = "View Crew Schedule";

        public const string AddUnitEvent = "Add Unit Schedule";
        public const string UpdateUnitEvent = "Update Unit Schedule";
        public const string DeleteUnitEvent = "Cancel Unit Schedule";
        public const string DetailUnitEvent = "Detail Unit Schedule";


        public const string Controller = "Calendar";

        public ContextMenu WorkAddContextMenu(string date, string content)
        {
            return new ContextMenu()
            {
                MenuName = AddWorkEvent,
                ProcessingMode = ProcessingMode.PopsUpWindow,
                ControllerName = Controller,
                Parms = new List<string>() { date,content },
                ActionName = "AddWorkSchedule"
            };
        }
        public ContextMenu WorkUpdateContextMenu( string param=null, bool isDisabled = false)
        {
            return new ContextMenu()
            {
                MenuName = UpdateWorkEvent,
                ProcessingMode = ProcessingMode.PopsUpWindow,
                ControllerName = Controller,
                Parms = new List<string>(){param},
                ActionName = "UpdateWorkSchedule",
                IsDisabled = isDisabled

            };
        }
        public ContextMenu WorkDeleteContextMenu(string param=null, bool isDisabled= false)
        {
            return new ContextMenu()
            {
                MenuName = DeleteWorkEvent,
                ProcessingMode = ProcessingMode.NoPopsUpWindow,
                ControllerName = Controller,
                Parms = new List<string>() { param },
                ActionName = "CancelWorkSchedule",
                IsDisabled = isDisabled
                
            };
        }

        public ContextMenu WorkDetailContextMenu(string param = null)
        {
            return new ContextMenu()
            {
                MenuName = DetailWorkEvent,
                ProcessingMode = ProcessingMode.PopsUpWindow,
                ControllerName = Controller,
                Parms = new List<string>() { param },
                ActionName = "DetailWorkerSchedule",
               

            };
        }

        public ContextMenu CrewAddContextMenu()
        {
            return new ContextMenu()
            {
                MenuName = AddCrewEvent,
                ProcessingMode = ProcessingMode.PopsUpWindow,
                ControllerName = Controller,
                ActionName = "AddCrewSchedule"
            };
        }
        public ContextMenu CrewUpdateContextMenu(string param=null)
        {
            return new ContextMenu()
            {
                MenuName = UpdateCrewEvent,
                ProcessingMode = ProcessingMode.PopsUpWindow,
                ControllerName = Controller,
                Parms = new List<string>() { param },
                ActionName = "UpdateCrewSchedule"
            };
        }
        public ContextMenu CrewDeleteContextMenu(string param=null)
        {
            return new ContextMenu()
            {
                MenuName = DeleteCrewEvent,
                ProcessingMode = ProcessingMode.NoPopsUpWindow,
                ControllerName = Controller,
                Parms = new List<string>() { param },
                ActionName = "CancelCrewSchedule"
            };
        }

        public ContextMenu CrewDetailContextMenu(string param = null)
        {
            return new ContextMenu()
            {
                MenuName = DetailCrewEvent,
                ProcessingMode = ProcessingMode.PopsUpWindow,
                ControllerName = Controller,
                Parms = new List<string>() { param },
                ActionName = "DetailCrewSchedule"
            };
        }

        public ContextMenu UnitAddContextMenu(string date,string content)
        {
            return new ContextMenu()
            {
                MenuName = AddUnitEvent,
                ProcessingMode = ProcessingMode.PopsUpWindow,
                ControllerName = Controller,
                Parms = new List<string>() { date,content },
                ActionName = "AddUnitSchedule"
            };
        }
        public ContextMenu UnitUpdateContextMenu(string param=null, bool isDisabled = false)
        {
            return new ContextMenu()
            {
                MenuName = UpdateUnitEvent,
                ProcessingMode = ProcessingMode.PopsUpWindow,
                ControllerName = Controller,
                Parms = new List<string>() { param },
                ActionName = "UpdateUnitSchedule",
                IsDisabled = isDisabled
            };
        }
        public ContextMenu UnitDeleteContextMenu(string param=null,bool isDisabled= false)
        {
            return new ContextMenu()
            {
                MenuName = DeleteUnitEvent,
                ProcessingMode = ProcessingMode.NoPopsUpWindow,
                ControllerName = Controller,
                Parms = new List<string>() { param },
                ActionName = "CancelUnitSchedule",
                IsDisabled = isDisabled
            };
        }
        public ContextMenu UnitDetailContextMenu(string param=null)
        {
            return new ContextMenu()
            {
                MenuName = DetailUnitEvent,
                ProcessingMode = ProcessingMode.PopsUpWindow,
                ControllerName = Controller,
                Parms = new List<string>() { param },
                ActionName = "DetailUnitSchedule"
            };
        }
    }
}
