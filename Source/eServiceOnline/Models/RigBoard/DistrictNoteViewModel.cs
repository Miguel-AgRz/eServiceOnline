using System.Collections.Generic;
using eServiceOnline.Models.Commons;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Crew;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Organization;

namespace eServiceOnline.Models.RigBoard
{
    public class DistrictNoteViewModel:ModelBase<ServicePointNote>
    {
        public const string UpdateNotes = "Update Notes";
        public StyledCell DistrictNotes { get; set; }
        public NoteModel NoteModel { get; set; }
      

        public override void PopulateFrom(ServicePointNote note)
        {
            this.NoteModel = GetDistrictNoteModel(note);
            this.DistrictNotes = this.GetDistrictNoteStyledCell("DistrictNotes", note);
        }
        private StyledCell GetDistrictNoteStyledCell(string propertyName, ServicePointNote note)
        {
            StyledCell styledCell = new StyledCell(propertyName, null, this.LoggedUser,null) { PropertyValue = NoteModel?.Notes, ContextMenus = this.SetNotesContextMenus(note)};         
            return styledCell;
        }
        private List<ContextMenu> SetNotesContextMenus(ServicePointNote note)
        {
            List<ContextMenu> list = new List<ContextMenu>();
            list.Add(new ContextMenu
            {
                MenuName = UpdateNotes,
                ProcessingMode = ProcessingMode.PopsUpWindow,
                Parms = new List<string> { NoteModel?.Id.ToString(), NoteModel?.ReturnActionName, NoteModel?.ReturnControllerName, NoteModel?.PostControllerName, NoteModel?.PostMethodName, note?.Description },
                ControllerName = NoteModel?.CallingControllerName,
                ActionName = NoteModel?.CallingMethodName
            });

            return list;
        }

        private NoteModel GetDistrictNoteModel(ServicePointNote note)
        {
            NoteModel noteModel = new NoteModel();

            noteModel.Id = note.ServicePoint?.Id ?? 0;
            noteModel.Notes = string.IsNullOrEmpty(note?.Description)?"Please enter NOTES for this district": note?.Description;
            noteModel.Notes = noteModel.Notes.Replace("\r\n", " || ");
            noteModel.ReturnActionName = "Index";
            noteModel.ReturnControllerName = "RigBoard";
            noteModel.CallingControllerName = "RigBoard";
            noteModel.CallingMethodName = "UpdateDistrictNotes";
            noteModel.PostControllerName = "RigBoard";
            noteModel.PostMethodName = "UpdateDistrictNotes";
            return noteModel;
        }

    }

}