using System.Collections.Generic;
using MetaShare.Common.Core.Entities;
using MetaShare.Common.Core.Services;
using Sesi.LocalData.Entities.LocalOperation;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Services.Interfaces.LocalOperation
{
	public interface IChartDefinitionService : IPagingService<ChartDefinition>
	{
	        List<ChartDefinition> SelectChartDefinitionByJobMonSettings(int[] jobMonSettingIds, bool isAggregatedChildren = false);
	        List<ChartDefinition> SelectChartDefinitionByJobMonSettings(Pager pager,int[] jobMonSettingIds, bool isAggregatedChildren = false);
	        List<ChartDefinition> SelectByJobMonSetting(int pageIndex,int pageSize,int jobMonSettingId);
	        List<ChartDefinition> SelectByJobMonSetting(int jobMonSettingId);
	/*add customized code between this region*/
	/*add customized code between this region*/
	}
}
