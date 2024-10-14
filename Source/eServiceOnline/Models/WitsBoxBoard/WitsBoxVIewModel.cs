using System;
using System.Collections.Generic;
using eServiceOnline.Models.Commons;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment;

namespace eServiceOnline.Models.NubinsBoard
{
    public class WitsBoxVIewModel:ModelBase<WitsBox>
    {
        public const string MenuUpdateNotes = "Update Notes";

        public const string WitsBoxController = "WitsBoxBoard";
        public StyledCell Name { get; set; }
        public StyledCell Location { get; set; }
        public StyledCell Notes { get; set; }
        public WitsBoxInformation WitsBoxInformation { get; set; }

        public override void PopulateFrom(WitsBox witsBox)
        {

            this.Name = this.GetNameStyledCell("Name", witsBox);
          
            this.Location = this.GetLocationStyledCell("Location", witsBox);
            this.Notes = this.GetNotesStyledCell("Notes", witsBox);
        }
        private StyledCell GetNameStyledCell(string propertyName, WitsBox witsBox)
        {
            StyledCell styledCell = new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue =$"{witsBox?.Name} {witsBox?.Id}"  };

            return styledCell;
        }
        private StyledCell GetLocationStyledCell(string propertyName, WitsBox witsBox)
        {
            StyledCell styledCell = new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue = WitsBoxInformation == null ? witsBox?.HomeServicePoint?.Name : (WitsBoxInformation.EquipmentStatus == EquipmentStatus.Yard ? WitsBoxInformation?.WorkingServicePoint?.Name : WitsBoxInformation.Location) };

            return styledCell;
        }
        private StyledCell GetNotesStyledCell(string propertyName, WitsBox witsBox)
        {
            StyledCell styledCell = new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue = WitsBoxInformation?.Notes,ContextMenus = this.SetNoteContextMenus(witsBox)};

            return styledCell;
        }
        private List<ContextMenu> SetNoteContextMenus(WitsBox witsBox)
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
                            witsBox.Id.ToString(),
                        },
                    ControllerName =WitsBoxController,
                    ActionName = "UpdateNotes"
                }
            };

            return list;
        }
    }
}