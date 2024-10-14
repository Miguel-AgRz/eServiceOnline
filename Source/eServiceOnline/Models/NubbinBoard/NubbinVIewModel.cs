using System;
using System.Collections.Generic;
using eServiceOnline.Models.Commons;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment;

namespace eServiceOnline.Models.NubinsBoard
{
    public class NubbinVIewModel:ModelBase<Nubbin>
    {
        public const string MenuUpdateNotes = "Update Notes";

        public const string NubbinController = "NubbinBoard";


        public StyledCell Size { get; set; }
        public StyledCell ThreadType { get; set; }
        public StyledCell Location { get; set; }
        public StyledCell Notes { get; set; }
        public NubbinInformation NubbinInformation { get; set; }

        public override  void PopulateFrom(Nubbin nubbin)
        {
           
            this.Size = this.GetSizeStyledCell("Size", nubbin);
            this.ThreadType = this.GetThreadTypeStyledCell("ThreadType", nubbin);
            this.Location = this.GetLocationStyledCell("Location",nubbin);
            this.Notes = this.GetNotesStyledCell("Notes",nubbin);
        }
        private StyledCell GetSizeStyledCell(string propertyName, Nubbin nubbin)
        {
            StyledCell styledCell = new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue = nubbin?.NubbinSize?.Name };

            return styledCell;
        }
        private StyledCell GetThreadTypeStyledCell(string propertyName, Nubbin nubbin)
        {
            StyledCell styledCell = new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue =nubbin?.NubbinThreadType?.Name };

            return styledCell;
        }
        private StyledCell GetLocationStyledCell(string propertyName, Nubbin nubbin)
        {
            StyledCell styledCell = new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue = NubbinInformation == null ? nubbin?.HomeServicePoint?.Name : (NubbinInformation.EquipmentStatus == EquipmentStatus.Yard ? NubbinInformation?.WorkingServicePoint?.Name : NubbinInformation.Location) };

            return styledCell;
        }
        private StyledCell GetNotesStyledCell(string propertyName, Nubbin nubbin)
        {
            StyledCell styledCell = new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue = NubbinInformation?.Notes,ContextMenus = this.SetNoteContextMenus(nubbin) };

            return styledCell;
        }
        private List<ContextMenu> SetNoteContextMenus(Nubbin nubbin)
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
                            nubbin.Id.ToString(),
                        },
                    ControllerName =NubbinController,
                    ActionName = "UpdateNotes"
                }
            };

            return list;
        }
    }
}