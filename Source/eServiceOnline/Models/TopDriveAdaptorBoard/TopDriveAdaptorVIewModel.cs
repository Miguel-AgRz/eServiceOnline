using System;
using System.Collections.Generic;
using eServiceOnline.Models.Commons;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment;

namespace eServiceOnline.Models.TopDriveAdaptorBoard
{
    public class TopDriveAdaptorVIewModel:ModelBase<TopDriveAdaptor>
    {

        public const string MenuUpdateNotes = "Update Notes";

        public const string TopDriveAdaptorBoardController = "TopDriveAdaptorBoard";
        public StyledCell Size { get; set; }
        public StyledCell ThreadType { get; set; }
        public StyledCell Location { get; set; }
        public StyledCell Notes { get; set; }
        public TopDrivceAdaptorInformation TopDrivceAdaptorInformation { get; set; }

        public override void PopulateFrom(TopDriveAdaptor topDriveAdaptor)
        {
            if (topDriveAdaptor == null) throw new Exception("entity must be instance of class topDriveAdaptor.");
            this.Size = this.GetSizeStyledCell("Size", topDriveAdaptor);
            this.ThreadType = this.GetThreadTypeStyledCell("ThreadType", topDriveAdaptor);
            this.Location = this.GetLocationStyledCell("Location", topDriveAdaptor);
            this.Notes = this.GetNotesStyledCell("Notes", topDriveAdaptor);
        }

        private StyledCell GetSizeStyledCell(string propertyName, TopDriveAdaptor topDriveAdaptor)
        {
            StyledCell styledCell = new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue = topDriveAdaptor?.TopDrivceAdaptorSize?.Name };

            return styledCell;
        }
        private StyledCell GetThreadTypeStyledCell(string propertyName, TopDriveAdaptor topDriveAdaptor)
        {
            StyledCell styledCell = new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue = topDriveAdaptor?.TopDrviceAdaptorThreadType?.Name };

            return styledCell;
        }
        private StyledCell GetLocationStyledCell(string propertyName, TopDriveAdaptor topDriveAdaptor)
        {
            StyledCell styledCell = new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue = TopDrivceAdaptorInformation == null ? topDriveAdaptor?.HomeServicePoint?.Name : (TopDrivceAdaptorInformation.EquipmentStatus == EquipmentStatus.Yard ? TopDrivceAdaptorInformation?.WorkingServicePoint?.Name : TopDrivceAdaptorInformation.Location) };

            return styledCell;
        }

        private StyledCell GetNotesStyledCell(string propertyName, TopDriveAdaptor topDriveAdaptor)
        {
            StyledCell styledCell = new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue = TopDrivceAdaptorInformation?.Notes,ContextMenus = this.SetNoteContextMenus(topDriveAdaptor) };

            return styledCell;
        }

        private List<ContextMenu> SetNoteContextMenus(TopDriveAdaptor topDriveAdaptor)
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
                            topDriveAdaptor.Id.ToString(),
                        },
                    ControllerName =TopDriveAdaptorBoardController,
                    ActionName = "UpdateNotes"
                }
            };

            return list;
        }
    }
}