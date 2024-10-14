using System;
using System.Collections.Generic;
using eServiceOnline.Models.Commons;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment;

namespace eServiceOnline.Models.NubinsBoard
{
    public class SwedgeVIewModel:ModelBase<Swedge>
    {
        public const string MenuUpdateNotes = "Update Notes";

        public const string SwedgeController = "SwedgeBoard";
        public StyledCell Size { get; set; }
        public StyledCell ThreadType { get; set; }
        public StyledCell Location { get; set; }
        public StyledCell Notes { get; set; }
        public SwedgeInformation SwedgeInformation { get; set; }

        public override void PopulateFrom(Swedge swedge)
        {

            this.Size = this.GetSizeStyledCell("Size", swedge);
            this.ThreadType = this.GetThreadTypeStyledCell("ThreadType", swedge);
            this.Location = this.GetLocationStyledCell("Location", swedge);
            this.Notes = this.GetNotesStyledCell("Notes", swedge);
        }
        private StyledCell GetSizeStyledCell(string propertyName, Swedge swedge)
        {
            StyledCell styledCell = new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue = swedge?.SwedgeSize?.Name };

            return styledCell;
        }
        private StyledCell GetThreadTypeStyledCell(string propertyName, Swedge swedge)
        {
            StyledCell styledCell = new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue = swedge?.SwedgeThreadType?.Name };

            return styledCell;
        }
        private StyledCell GetLocationStyledCell(string propertyName, Swedge swedge)
        {
            StyledCell styledCell = new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue = SwedgeInformation == null ? swedge?.HomeServicePoint?.Name : (SwedgeInformation.EquipmentStatus == EquipmentStatus.Yard ? SwedgeInformation?.WorkingServicePoint?.Name : SwedgeInformation.Location) };

            return styledCell;
        }
        private StyledCell GetNotesStyledCell(string propertyName, Swedge swedge)
        {
            StyledCell styledCell = new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue = SwedgeInformation?.Notes,ContextMenus = this.SetNoteContextMenus(swedge) };

            return styledCell;
        }
        private List<ContextMenu> SetNoteContextMenus(Swedge swedge)
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
                            swedge.Id.ToString(),

                        },
                    ControllerName =SwedgeController,
                    ActionName = "UpdateNotes"
                }
            };

            return list;
        }
    }
}