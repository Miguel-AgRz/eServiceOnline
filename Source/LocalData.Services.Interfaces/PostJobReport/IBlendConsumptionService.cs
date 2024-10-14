using System.Collections.Generic;
using MetaShare.Common.Core.Entities;
using MetaShare.Common.Core.Services;
using MetaShare.Common.Core.Services.Version;
using Sesi.LocalData.Entities.PostJobReport;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Services.Interfaces.PostJobReport
{
	public interface IBlendConsumptionService : IObjectVersionService<BlendConsumption>
	{
	        List<BlendConsumption> SelectBlendConsumptionByBlendReports(int[] blendReportIds, bool isAggregatedChildren = false);
	        List<BlendConsumption> SelectBlendConsumptionByBlendReports(Pager pager,int[] blendReportIds, bool isAggregatedChildren = false);
	        List<BlendConsumption> SelectByBlendReport(int pageIndex,int pageSize,int blendReportId);
	        List<BlendConsumption> SelectByBlendReport(int blendReportId);
	/*add customized code between this region*/
	/*add customized code between this region*/
	}
}
