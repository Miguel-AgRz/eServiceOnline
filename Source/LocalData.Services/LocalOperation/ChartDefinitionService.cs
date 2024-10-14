using System.Collections.Generic;
using MetaShare.Common.Core.Entities;
using Sesi.LocalData.Entities.LocalOperation;
using MetaShare.Common.Core.Services;
using Sesi.LocalData.Daos.Interfaces.LocalOperation;
using Sesi.LocalData.Services.Interfaces.LocalOperation;

/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Services.LocalOperation
{
	public class ChartDefinitionService : Service<ChartDefinition>, IChartDefinitionService
	{
		public ChartDefinitionService() : base(typeof (IChartDefinitionDao))
		{
			ServiceAggregationInfo seriesDefinitionsServiceAggregation = this.ServiceAggregationInfo.AddCompositeCollectionChild("SeriesDefinitions", typeof(Sesi.LocalData.Entities.LocalOperation.SeriesDefinition), typeof(Sesi.LocalData.Daos.Interfaces.LocalOperation.ISeriesDefinitionDao), "Chart");
			seriesDefinitionsServiceAggregation.AddReferenceChild("PlcDataCalculationFormula", typeof(Sesi.LocalData.Entities.LocalOperation.PlcDataCalculationFormula), typeof(Sesi.LocalData.Daos.Interfaces.LocalOperation.IPlcDataCalculationFormulaDao));
			seriesDefinitionsServiceAggregation.AddReferenceChild("Parameter", typeof(Sesi.LocalData.Entities.LocalOperation.PlcParameter), typeof(Sesi.LocalData.Daos.Interfaces.LocalOperation.IPlcParameterDao));
			seriesDefinitionsServiceAggregation.AddReferenceChild("UnitCalculationFormula", typeof(Sesi.LocalData.Entities.LocalOperation.UnitCalculationFormula), typeof(Sesi.LocalData.Daos.Interfaces.LocalOperation.IUnitCalculationFormulaDao));
		}

		public List<ChartDefinition> SelectChartDefinitionByJobMonSettings(int[] jobMonSettingIds, bool isAggregatedChildren = false)
        {
            List<ChartDefinition> items = this.SelectByColumnIds("JobMonSettingId",jobMonSettingIds,isAggregatedChildren);
            return items;
        }
		public List<ChartDefinition> SelectChartDefinitionByJobMonSettings(Pager pager, int[] jobMonSettingIds, bool isAggregatedChildren = false)
        {
            List<ChartDefinition> items = this.SelectByColumnIds(pager,"JobMonSettingId",jobMonSettingIds,isAggregatedChildren);
            return items;
        }
		public List<ChartDefinition> SelectByJobMonSetting(int pageIndex,int pageSize,int jobMonSettingId)
        {
            Pager pager = new Pager { PageIndex = pageIndex, PageSize = pageSize };
            List<ChartDefinition> items = this.SelectBy(pager,new ChartDefinition { JobMonSetting = new Sesi.LocalData.Entities.LocalOperation.JobMonitorSetting{ Id = jobMonSettingId } },new List<string> { "JobMonSettingId" });
            return items;
        }
		public List<ChartDefinition> SelectByJobMonSetting(int jobMonSettingId)
        {
            List<ChartDefinition> items = this.SelectBy(new ChartDefinition { JobMonSetting = new Sesi.LocalData.Entities.LocalOperation.JobMonitorSetting{ Id = jobMonSettingId } },new List<string> { "JobMonSettingId" });
            return items;
        }/*add customized code between this region*/
		/*add customized code between this region*/

	}
}
