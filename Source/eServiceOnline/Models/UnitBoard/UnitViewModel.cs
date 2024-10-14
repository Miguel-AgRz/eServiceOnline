using System;
using System.Collections.Generic;
using eServiceOnline.Models.Commons;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment;

namespace eServiceOnline.Models.UnitBoard
{
    public class UnitViewModel: ModelBase<TruckUnit>
    {
        public const string MenuUpdateNotes = "Update Notes";
        public StyledCell UnitNumber { get; set; }
        public StyledCell Notes { get; set; }


        public override void PopulateFrom(TruckUnit truckUnit)
        {
            if (truckUnit == null) throw new Exception("entity must be instance of class truckUnit.");
            this.UnitNumber = this.GetUnitNumberStyledCell("UnitNumber", truckUnit);
            this.Notes = this.GetNotesStyledCell("Notes", truckUnit);
        }

        private StyledCell GetUnitNumberStyledCell(string propertyName, TruckUnit truckUnit)
        {
            StyledCell styledCell = new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue = truckUnit.UnitNumber.ToString() };

            return styledCell;
        }

        private StyledCell GetNotesStyledCell(string propertyName, TruckUnit truckUnit)
        {
            StyledCell styledCell = new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue =this.NoteModel.Notes,ContextMenus = SetNoteContextMenus(truckUnit.Id.ToString()) };

            return styledCell;
        }

    }

   
}
