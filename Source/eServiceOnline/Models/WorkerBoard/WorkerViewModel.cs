using System;
using System.Collections.Generic;
using eServiceOnline.Models.Commons;
using Sesi.SanjelData.Entities.Common.BusinessEntities.HumanResources;

namespace eServiceOnline.Models.WorkerBoard
{
    public class WorkerViewModel: ModelBase<Employee>
    {
        public const string CrewBoardController = "CrewBoard";
        public const string UpdateWorkerProfile = "Update Worker Profile";
        public const string MenuUpdateNotes = "Update Notes";
        public StyledCell WorkerName { get; set; }
        public StyledCell Position { get; set; }
        public StyledCell Notes { get; set; }
        public StyledCell Profile { get; set; }
        public ProfileModel ProfileModel { get; set; }

        public override void PopulateFrom(Employee employee)
        {
            if (employee == null) throw new Exception("entity must be instance of class employee.");
            this.WorkerName = this.GetWorkerNameStyledCell("WorkerName", employee);
            this.Position = this.GetPositionStyleCell("Position", employee);
            this.Notes = this.GetNotesStyledCell("Notes", employee);
            this.Profile = this.GetProfileStyledCell("Notes", employee);
        }

        private StyledCell GetWorkerNameStyledCell(string propertyName, Employee employee)
        {
            StyledCell styledCell = new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue = $"{employee.LastName}, {employee.FirstName}"};
//            styledCell.ContextMenus =SetWorkerProfileContextMenus(employee) ;
            return styledCell;
        }

        private StyledCell GetPositionStyleCell(string propertyName, Employee employee)
        {
            StyledCell styledCell = new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue =employee.BonusPosition?.Name };
            return styledCell;
        }

        private StyledCell GetNotesStyledCell(string propertyName, Employee employee)
        {
            StyledCell styledCell = new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue = this.NoteModel.Notes,ContextMenus = SetNoteContextMenus(employee.Id.ToString()) };
            return styledCell;
        }
        private StyledCell GetProfileStyledCell(string propertyName, Employee employee)
        {
            StyledCell styledCell = new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue = this.ProfileModel.Profile, ContextMenus = SetProfileContextMenus(employee) };
            return styledCell;
        }
        //        private List<ContextMenu> SetWorkerProfileContextMenus(Employee employee)
        //        {
        //            List<ContextMenu> list = new List<ContextMenu>();
        //            list.Add(new ContextMenu
        //            {
        //                MenuName = UpdateWorkerProfile,
        //                ProcessingMode = ProcessingMode.PopsUpWindow,
        //                Parms = new List<string>() { employee.Id.ToString() },
        //                DialogName= UpdateWorkerProfile,
        //                ControllerName = CrewBoardController,
        //                ActionName = "UpdateWorkerProfile"
        //            });
        //            return list;
        //        }

        private List<ContextMenu> SetProfileContextMenus(Employee employee)
        {
            List<ContextMenu> list = new List<ContextMenu>();
            list.Add(new ContextMenu
            {
                MenuName = UpdateWorkerProfile,
                ProcessingMode = ProcessingMode.PopsUpWindow,
                Parms = new List<string>() { employee.Id.ToString(), ProfileModel.ReturnActionName, ProfileModel.ReturnControllerName, ProfileModel.PostControllerName, ProfileModel.PostMethodName, ProfileModel.Profile },
                ControllerName = ProfileModel.CallingControllerName,
                ActionName = ProfileModel.CallingMethodName
            });
            return list;
        }
    }
}
