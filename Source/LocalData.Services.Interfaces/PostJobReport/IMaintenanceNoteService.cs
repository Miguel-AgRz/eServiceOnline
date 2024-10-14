using System.Collections.Generic;
using MetaShare.Common.Core.Entities;
using MetaShare.Common.Core.Services;
using MetaShare.Common.Core.Services.Version;
using Sesi.LocalData.Entities.PostJobReport;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Services.Interfaces.PostJobReport
{
	public interface IMaintenanceNoteService : IObjectVersionService<MaintenanceNote>
	{
	/*add customized code between this region*/
    int CreateMaintenanceNote(MaintenanceNote maintenanceNote);
    int UpdateMaintenanceNote(MaintenanceNote maintenanceNote);
    int DeleteMaintenanceNote(MaintenanceNote existingMaintenanceNote);
    /*add customized code between this region*/
    }
}
