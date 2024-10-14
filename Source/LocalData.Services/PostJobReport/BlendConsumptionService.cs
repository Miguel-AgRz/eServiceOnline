using System.Collections.Generic;
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
	public class BlendConsumptionService : ObjectVersionService<BlendConsumption>, IBlendConsumptionService
	{
		public BlendConsumptionService() : base(typeof (IBlendConsumptionDao))
		{
		}

		public List<BlendConsumption> SelectBlendConsumptionByBlendReports(int[] blendReportIds, bool isAggregatedChildren = false)
        {
            List<BlendConsumption> items = this.SelectByColumnIds("BlendReportId",blendReportIds,isAggregatedChildren);
            return items;
        }
		public List<BlendConsumption> SelectBlendConsumptionByBlendReports(Pager pager, int[] blendReportIds, bool isAggregatedChildren = false)
        {
            List<BlendConsumption> items = this.SelectByColumnIds(pager,"BlendReportId",blendReportIds,isAggregatedChildren);
            return items;
        }
		public List<BlendConsumption> SelectByBlendReport(int pageIndex,int pageSize,int blendReportId)
        {
            Pager pager = new Pager { PageIndex = pageIndex, PageSize = pageSize };
            List<BlendConsumption> items = this.SelectBy(pager,new BlendConsumption { BlendReport = new Sesi.LocalData.Entities.PostJobReport.BlendReport{ Id = blendReportId } },new List<string> { "BlendReportId" });
            return items;
        }
		public List<BlendConsumption> SelectByBlendReport(int blendReportId)
        {
            List<BlendConsumption> items = this.SelectBy(new BlendConsumption { BlendReport = new Sesi.LocalData.Entities.PostJobReport.BlendReport{ Id = blendReportId } },new List<string> { "BlendReportId" });
            return items;
        }/*add customized code between this region*/
		/*add customized code between this region*/

	}
}
