using System.Collections.Generic;
using MetaShare.Common.Core.Entities;
using MetaShare.Common.Core.Services;
using MetaShare.Common.Core.Services.Version;
using Sesi.LocalData.Entities.PostJobReport;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Services.Interfaces.PostJobReport
{
	public interface IPostJobReportService : IObjectVersionService<Entities.PostJobReport.PostJobReport>
	{
	/*add customized code between this region*/
    int CreatePostJobReport(Entities.PostJobReport.PostJobReport postJobReport);
    int UpdatePostJobReport(Entities.PostJobReport.PostJobReport postJobReport);
    /*add customized code between this region*/
    }
}
