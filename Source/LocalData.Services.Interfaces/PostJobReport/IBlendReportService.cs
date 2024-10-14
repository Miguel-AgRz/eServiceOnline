using System.Collections.Generic;
using MetaShare.Common.Core.Entities;
using MetaShare.Common.Core.Services;
using MetaShare.Common.Core.Services.Version;
using Sesi.LocalData.Entities.PostJobReport;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Services.Interfaces.PostJobReport
{
	public interface IBlendReportService : IObjectVersionService<BlendReport>
	{
	/*add customized code between this region*/
    int CreateBlendReport(BlendReport blendReport);
    int UpdateBlendReport(BlendReport blendReport);
    /*add customized code between this region*/
    }
}
