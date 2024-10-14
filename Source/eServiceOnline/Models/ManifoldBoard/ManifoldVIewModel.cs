using System;
using System.Collections.Generic;
using eServiceOnline.Models.Commons;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment;

namespace eServiceOnline.Models.ManifoldBoard
{
    public class ManifoldVIewModel:ModelBase<Manifold>
    {

        public const string MenuUpdateNotes = "Update Notes";

        public const string ManifoldController = "ManifoldBoard";
        public StyledCell ManifoldType { get; set; }
        public StyledCell Location { get; set; }
        public StyledCell Notes { get; set; }

        public ManifoldInformation ManifoldInformation { get; set; }

        public override void PopulateFrom(Manifold manifold)
        {
            if (manifold == null) throw new Exception("entity must be instance of class plugLoadingHead.");
            this.ManifoldType = this.GetManifoldTypeStyledCell("ManifoldType", manifold);
            this.Location = this.GetLocationStyledCell("Location", manifold);
            this.Notes = this.GetNotesStyledCell("Notes", manifold);
        }
        private StyledCell GetManifoldTypeStyledCell(string propertyName, Manifold manifold)
        {
            StyledCell styledCell = new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue = manifold?.ManifoldType?.Name };

            return styledCell;
        }
        private StyledCell GetLocationStyledCell(string propertyName, Manifold manifold)
        {
            StyledCell styledCell = new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue = ManifoldInformation == null ? manifold?.HomeServicePoint?.Name : (ManifoldInformation.EquipmentStatus == EquipmentStatus.Yard ? ManifoldInformation?.WorkingServicePoint?.Name : ManifoldInformation.Location) };

            return styledCell;
        }
        private StyledCell GetNotesStyledCell(string propertyName, Manifold manifold)
        {
            StyledCell styledCell = new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue = ManifoldInformation?.Notes,ContextMenus = this.SetNoteContextMenus(manifold) };

            return styledCell;
        }

        private List<ContextMenu> SetNoteContextMenus(Manifold manifold)
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
                            manifold.Id.ToString(),

                        },
                    ControllerName =ManifoldController,
                    ActionName = "UpdateNotes"
                }
            };

            return list;
        }
    }
}