using System.Collections.Generic;
using System.Linq;
using MetaShare.Common.Core.Entities;
using Sesi.LocalData.Entities.PostJobReport;
using MetaShare.Common.Core.Services;
using MetaShare.Common.Core.Services.Version;
using Sesi.LocalData.Daos.Interfaces.PostJobReport;
using Sesi.LocalData.Services.Interfaces.PostJobReport;

/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Services.PostJobReport
{
	public class MaintenanceNoteService : ObjectVersionService<MaintenanceNote>, IMaintenanceNoteService
	{
		public MaintenanceNoteService() : base(typeof (IMaintenanceNoteDao))
		{
		}
		/*add customized code between this region*/
        public int CreateMaintenanceNote(MaintenanceNote maintenanceNote)
        {
            maintenanceNote.Id = 0;
            maintenanceNote.SystemId = 0;

            var storageInfos = this.SelectBy(maintenanceNote, new List<string> {"JobUniqueId","UnitNumber"});
            if (storageInfos != null)
            {
                foreach (var info in storageInfos)
                {
                    this.Delete(info);
                }
            }

            return this.Insert(maintenanceNote);
        }

        public int UpdateMaintenanceNote(MaintenanceNote maintenanceNote)
        {
            maintenanceNote.Id = 0;
            maintenanceNote.SystemId = 0;

            var maintenanceNotes = this.SelectBy(maintenanceNote, new List<string> {"JobUniqueId", "UnitNumber"});
            if (maintenanceNotes == null || maintenanceNotes.Count == 0)
            {
                return this.Insert(maintenanceNote);
            }
            else if (maintenanceNotes.Count > 1)
            {
                foreach (var info in maintenanceNotes)
                {
                    this.Delete(info);
                }
                return this.Insert(maintenanceNote);
            }
            else
            {
                var existingMaintenanceNote = maintenanceNotes.FirstOrDefault();
                existingMaintenanceNote.JobNumber = maintenanceNote.JobNumber;
                existingMaintenanceNote.UnitType = maintenanceNote.UnitType;
                existingMaintenanceNote.Notes = maintenanceNote.Notes;

                return this.Update(existingMaintenanceNote);
            }
        }

        public int DeleteMaintenanceNote(MaintenanceNote maintenanceNote)
        {
            var maintenanceNotes = this.SelectBy(maintenanceNote, new List<string> {"JobUniqueId", "UnitNumber"});
            if (maintenanceNotes != null || maintenanceNotes.Count > 0)
            {
                return this.Delete(maintenanceNotes.FirstOrDefault());
            }

            return 0;
        }
        /*add customized code between this region*/

    }
}
