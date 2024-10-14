using System;
using System.Collections.Generic;
using eServiceOnline.Models.Commons;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment;

namespace eServiceOnline.Models.PlugLoadingHeadSubBoard
{
    public class PlugLoadingHeadSubViewModel:ModelBase<PlugLoadingHeadSub>
    {

        public const string MenuUpdateNotes = "Update Notes";

        public const string PlugLoadingHeadSubBoardController = "PlugLoadingHeadSubBoard";
        public StyledCell Size { get; set; }
        public StyledCell ThreadType { get; set; }
        public StyledCell Location { get; set; }
        public StyledCell Notes { get; set; }
        public PlugLoadingHeadSubInformation PlugLoadingHeadSubInformation { get; set; }

        public override void PopulateFrom(PlugLoadingHeadSub plugLoadingHeadSub)
        {
            if (plugLoadingHeadSub == null) throw new Exception("entity must be instance of class plugLoadingHeadSub.");
            this.Size = this.GetSizeStyledCell("Size", plugLoadingHeadSub);
            this.ThreadType = this.GetThreadTypeStyledCell("ThreadType", plugLoadingHeadSub);
            this.Location = this.GetLocationStyledCell("Location", plugLoadingHeadSub);
            this.Notes = this.GetNotesStyledCell("Notes", plugLoadingHeadSub);
        }

        private StyledCell GetSizeStyledCell(string propertyName, PlugLoadingHeadSub plugLoadingHeadSub)
        {
            StyledCell styledCell = new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue = plugLoadingHeadSub?.PlugLoadingHeadSubSize?.Name };

            return styledCell;
        }
        private StyledCell GetThreadTypeStyledCell(string propertyName, PlugLoadingHeadSub plugLoadingHeadSub)
        {
            StyledCell styledCell = new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue = plugLoadingHeadSub?.PlugLoadingHeadSubThreadType?.Name };

            return styledCell;
        }
        private StyledCell GetLocationStyledCell(string propertyName, PlugLoadingHeadSub plugLoadingHeadSub)
        {
            StyledCell styledCell = new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue = PlugLoadingHeadSubInformation == null ? plugLoadingHeadSub?.HomeServicePoint?.Name : (PlugLoadingHeadSubInformation.EquipmentStatus == EquipmentStatus.Yard ? PlugLoadingHeadSubInformation?.WorkingServicePoint?.Name : PlugLoadingHeadSubInformation.Location) };

            return styledCell;
        }

        private StyledCell GetNotesStyledCell(string propertyName, PlugLoadingHeadSub plugLoadingHeadSub)
        {
            StyledCell styledCell = new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue = PlugLoadingHeadSubInformation?.Notes,ContextMenus = this.SetNoteContextMenus(plugLoadingHeadSub) };

            return styledCell;
        }

        private List<ContextMenu> SetNoteContextMenus(PlugLoadingHeadSub plugLoadingHeadSub)
        {
            List<ContextMenu> list = new List<ContextMenu>
            {
                new ContextMenu
                {
                    MenuName = MenuUpdateNotes,
                    ProcessingMode = ProcessingMode.PopsUpWindow,
                    Parms =
                        new List<string>
                        {
                            plugLoadingHeadSub.Id.ToString(),
                        },
                    ControllerName =PlugLoadingHeadSubBoardController,
                    ActionName = "UpdateNotes"
                }
            };

            return list;
        }
    }
}