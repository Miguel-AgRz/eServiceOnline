using System;
using System.Collections.Generic;
using eServiceOnline.Models.Commons;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment;

namespace eServiceOnline.Models.PlugLoadingHeadBoard
{
    public class PlugLoadingHeadViewModel:ModelBase<PlugLoadingHead>
    {


        public const string MenuUpdateNotes = "Update Notes";

        public const string PlugLoadingHeadController = "PlugLoadingHeadBoard";
        public StyledCell Size { get; set; }
        public StyledCell ThreadType { get; set; }
        public StyledCell Type { get; set; }
        public StyledCell Location { get; set; }
        public StyledCell Manifold { get; set; }
        public StyledCell TDCap { get; set; }
        public StyledCell PLHSub { get; set; }
        public StyledCell Notes { get; set; }

        public PlugLoadingHeadInformation plugLoadingHeadInformation { get; set; }

        public override void PopulateFrom(PlugLoadingHead plugLoadingHead)
        {
            if (plugLoadingHead == null) throw new Exception("entity must be instance of class plugLoadingHead.");
            this.Size = this.GetSizeStyledCell("Size", plugLoadingHead);
            this.ThreadType = this.GetThreadTypeStyledCell("ThreadType", plugLoadingHead);
            this.Type = this.GetTypeStyledCell("Type", plugLoadingHead);
            this.Location = this.GetLocationStyledCell("Location", plugLoadingHead);
            this.Manifold = this.GetManifoldStyledCell("Manifold", plugLoadingHead);
            this.TDCap = this.GetTDCapStyledCell("TDCap", plugLoadingHead);
            this.PLHSub = this.GetPLHSubStyledCell("PLHSub", plugLoadingHead);
            this.Notes = this.GetNotesStyledCell("Notes", plugLoadingHead);
        }

        private StyledCell GetSizeStyledCell(string propertyName, PlugLoadingHead plugLoadingHead)
        {
            StyledCell styledCell = new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue = plugLoadingHead?.PlugLoadingHeadSize?.Name };

            return styledCell;
        }
        private StyledCell GetThreadTypeStyledCell(string propertyName, PlugLoadingHead plugLoadingHead)
        {
            StyledCell styledCell = new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue = plugLoadingHead?.PlugLoadingHeadThreadType?.Name };

            return styledCell;
        }
        private StyledCell GetTypeStyledCell(string propertyName, PlugLoadingHead plugLoadingHead)
        {
            StyledCell styledCell = new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue = plugLoadingHead?.PlugLoadingHeadType?.Name };

            return styledCell;
        }
        private StyledCell GetLocationStyledCell(string propertyName, PlugLoadingHead plugLoadingHead)
        {
            StyledCell styledCell = new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue = plugLoadingHeadInformation==null?plugLoadingHead?.HomeServicePoint?.Name:(plugLoadingHeadInformation.EquipmentStatus==EquipmentStatus.Yard?plugLoadingHeadInformation?.WorkingServicePoint?.Name:plugLoadingHeadInformation.Location) };

            return styledCell;
        }
        private StyledCell GetManifoldStyledCell(string propertyName, PlugLoadingHead plugLoadingHead)
        {
            StyledCell styledCell = new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue = plugLoadingHeadInformation?.Manifold?.Id.ToString() };

            return styledCell;
        }
        private StyledCell GetTDCapStyledCell(string propertyName, PlugLoadingHead plugLoadingHead)
        {
            StyledCell styledCell = new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue = plugLoadingHeadInformation?.TopDriveAdaptor?.Id.ToString() };

            return styledCell;
        }
        private StyledCell GetPLHSubStyledCell(string propertyName, PlugLoadingHead plugLoadingHead)
        {
            StyledCell styledCell = new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue = plugLoadingHeadInformation?.PlugLoadingHeadSub?.Id.ToString() };

            return styledCell;
        }
        private StyledCell GetNotesStyledCell(string propertyName, PlugLoadingHead plugLoadingHead)
        {
            StyledCell styledCell = new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue = plugLoadingHeadInformation?.Notes,ContextMenus = this.SetNoteContextMenus(plugLoadingHead)};

            return styledCell;
        }
        private List<ContextMenu> SetNoteContextMenus(PlugLoadingHead plugLoadingHead)
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
                            plugLoadingHead.Id.ToString(),

                        },
                    ControllerName =PlugLoadingHeadController,
                    ActionName = "UpdateNotes"
                }
            };

            return list;
        }
    }
}