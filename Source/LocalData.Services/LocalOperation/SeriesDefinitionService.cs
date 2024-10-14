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
	public class SeriesDefinitionService : Service<SeriesDefinition>, ISeriesDefinitionService
	{
		public SeriesDefinitionService() : base(typeof (ISeriesDefinitionDao))
		{
		}

		public List<SeriesDefinition> SelectAllWithReferenceData(List<SeriesDefinition> items)
        {
            if (items != null && items.Count > 0)
            {
                return this.SelectBy(items, this.CreateReferenceInfoAggregation());
            }
            return items;
        }

        private ServiceAggregationInfo CreateReferenceInfoAggregation()
        {
            ServiceAggregationInfo aggregation = ServiceAggregationInfo.CreateRoot(typeof(SeriesDefinition), typeof(ISeriesDefinitionDao));
		    aggregation.AddReferenceChild("PlcDataCalculationFormula", typeof(Sesi.LocalData.Entities.LocalOperation.PlcDataCalculationFormula), typeof(Sesi.LocalData.Daos.Interfaces.LocalOperation.IPlcDataCalculationFormulaDao));
		    aggregation.AddReferenceChild("Parameter", typeof(Sesi.LocalData.Entities.LocalOperation.PlcParameter), typeof(Sesi.LocalData.Daos.Interfaces.LocalOperation.IPlcParameterDao));
		    aggregation.AddReferenceChild("UnitCalculationFormula", typeof(Sesi.LocalData.Entities.LocalOperation.UnitCalculationFormula), typeof(Sesi.LocalData.Daos.Interfaces.LocalOperation.IUnitCalculationFormulaDao));

		    return aggregation;
        }

		public List<SeriesDefinition> SelectSeriesDefinitionByPlcDataCalculationFormulas(int[] plcDataCalculationFormulaIds, bool isAggregatedChildren = false)
        {
            List<SeriesDefinition> items = this.SelectByColumnIds("PlcDataCalculationFormulaId",plcDataCalculationFormulaIds,isAggregatedChildren);
            return items;
        }
		public List<SeriesDefinition> SelectSeriesDefinitionByPlcDataCalculationFormulas(Pager pager, int[] plcDataCalculationFormulaIds, bool isAggregatedChildren = false)
        {
            List<SeriesDefinition> items = this.SelectByColumnIds(pager,"PlcDataCalculationFormulaId",plcDataCalculationFormulaIds,isAggregatedChildren);
            return items;
        }
		public List<SeriesDefinition> SelectSeriesDefinitionByParameters(int[] parameterIds, bool isAggregatedChildren = false)
        {
            List<SeriesDefinition> items = this.SelectByColumnIds("ParameterId",parameterIds,isAggregatedChildren);
            return items;
        }
		public List<SeriesDefinition> SelectSeriesDefinitionByParameters(Pager pager, int[] parameterIds, bool isAggregatedChildren = false)
        {
            List<SeriesDefinition> items = this.SelectByColumnIds(pager,"ParameterId",parameterIds,isAggregatedChildren);
            return items;
        }
		public List<SeriesDefinition> SelectSeriesDefinitionByUnitCalculationFormulas(int[] unitCalculationFormulaIds, bool isAggregatedChildren = false)
        {
            List<SeriesDefinition> items = this.SelectByColumnIds("UnitCalculationFormulaId",unitCalculationFormulaIds,isAggregatedChildren);
            return items;
        }
		public List<SeriesDefinition> SelectSeriesDefinitionByUnitCalculationFormulas(Pager pager, int[] unitCalculationFormulaIds, bool isAggregatedChildren = false)
        {
            List<SeriesDefinition> items = this.SelectByColumnIds(pager,"UnitCalculationFormulaId",unitCalculationFormulaIds,isAggregatedChildren);
            return items;
        }
		public List<SeriesDefinition> SelectSeriesDefinitionByCharts(int[] chartIds, bool isAggregatedChildren = false)
        {
            List<SeriesDefinition> items = this.SelectByColumnIds("ChartId",chartIds,isAggregatedChildren);
            return items;
        }
		public List<SeriesDefinition> SelectSeriesDefinitionByCharts(Pager pager, int[] chartIds, bool isAggregatedChildren = false)
        {
            List<SeriesDefinition> items = this.SelectByColumnIds(pager,"ChartId",chartIds,isAggregatedChildren);
            return items;
        }
		public List<SeriesDefinition> SelectByChart(int pageIndex,int pageSize,int chartId)
        {
            Pager pager = new Pager { PageIndex = pageIndex, PageSize = pageSize };
            List<SeriesDefinition> items = this.SelectBy(pager,new SeriesDefinition { Chart = new Sesi.LocalData.Entities.LocalOperation.ChartDefinition{ Id = chartId } },new List<string> { "ChartId" });
            return items;
        }
		public List<SeriesDefinition> SelectByChart(int chartId)
        {
            List<SeriesDefinition> items = this.SelectBy(new SeriesDefinition { Chart = new Sesi.LocalData.Entities.LocalOperation.ChartDefinition{ Id = chartId } },new List<string> { "ChartId" });
            return items;
        }
		public List<SeriesDefinition> SelectByChartParameter(int pageIndex,int pageSize,int chartId,int parameterId)
        {
            Pager pager = new Pager { PageIndex = pageIndex, PageSize = pageSize };
            List<SeriesDefinition> items = this.SelectBy(pager,new SeriesDefinition { Chart = new Sesi.LocalData.Entities.LocalOperation.ChartDefinition{ Id = chartId },Parameter = new Sesi.LocalData.Entities.LocalOperation.PlcParameter{ Id = parameterId } },new List<string> { "ChartId","ParameterId" });
            return items;
        }
		public List<SeriesDefinition> SelectByChartParameter(int chartId,int parameterId)
        {
            List<SeriesDefinition> items = this.SelectBy(new SeriesDefinition { Chart = new Sesi.LocalData.Entities.LocalOperation.ChartDefinition{ Id = chartId },Parameter = new Sesi.LocalData.Entities.LocalOperation.PlcParameter{ Id = parameterId } },new List<string> { "ChartId","ParameterId" });
            return items;
        }
		public List<SeriesDefinition> SelectByChartPlcDataCalculationFormula(int pageIndex,int pageSize,int chartId,int plcDataCalculationFormulaId)
        {
            Pager pager = new Pager { PageIndex = pageIndex, PageSize = pageSize };
            List<SeriesDefinition> items = this.SelectBy(pager,new SeriesDefinition { Chart = new Sesi.LocalData.Entities.LocalOperation.ChartDefinition{ Id = chartId },PlcDataCalculationFormula = new Sesi.LocalData.Entities.LocalOperation.PlcDataCalculationFormula{ Id = plcDataCalculationFormulaId } },new List<string> { "ChartId","PlcDataCalculationFormulaId" });
            return items;
        }
		public List<SeriesDefinition> SelectByChartPlcDataCalculationFormula(int chartId,int plcDataCalculationFormulaId)
        {
            List<SeriesDefinition> items = this.SelectBy(new SeriesDefinition { Chart = new Sesi.LocalData.Entities.LocalOperation.ChartDefinition{ Id = chartId },PlcDataCalculationFormula = new Sesi.LocalData.Entities.LocalOperation.PlcDataCalculationFormula{ Id = plcDataCalculationFormulaId } },new List<string> { "ChartId","PlcDataCalculationFormulaId" });
            return items;
        }
		public List<SeriesDefinition> SelectByChartUnitCalculationFormula(int pageIndex,int pageSize,int chartId,int unitCalculationFormulaId)
        {
            Pager pager = new Pager { PageIndex = pageIndex, PageSize = pageSize };
            List<SeriesDefinition> items = this.SelectBy(pager,new SeriesDefinition { Chart = new Sesi.LocalData.Entities.LocalOperation.ChartDefinition{ Id = chartId },UnitCalculationFormula = new Sesi.LocalData.Entities.LocalOperation.UnitCalculationFormula{ Id = unitCalculationFormulaId } },new List<string> { "ChartId","UnitCalculationFormulaId" });
            return items;
        }
		public List<SeriesDefinition> SelectByChartUnitCalculationFormula(int chartId,int unitCalculationFormulaId)
        {
            List<SeriesDefinition> items = this.SelectBy(new SeriesDefinition { Chart = new Sesi.LocalData.Entities.LocalOperation.ChartDefinition{ Id = chartId },UnitCalculationFormula = new Sesi.LocalData.Entities.LocalOperation.UnitCalculationFormula{ Id = unitCalculationFormulaId } },new List<string> { "ChartId","UnitCalculationFormulaId" });
            return items;
        }
		public List<SeriesDefinition> SelectByParameter(int pageIndex,int pageSize,int parameterId)
        {
            Pager pager = new Pager { PageIndex = pageIndex, PageSize = pageSize };
            List<SeriesDefinition> items = this.SelectBy(pager,new SeriesDefinition { Parameter = new Sesi.LocalData.Entities.LocalOperation.PlcParameter{ Id = parameterId } },new List<string> { "ParameterId" });
            return items;
        }
		public List<SeriesDefinition> SelectByParameter(int parameterId)
        {
            List<SeriesDefinition> items = this.SelectBy(new SeriesDefinition { Parameter = new Sesi.LocalData.Entities.LocalOperation.PlcParameter{ Id = parameterId } },new List<string> { "ParameterId" });
            return items;
        }
		public List<SeriesDefinition> SelectByParameterPlcDataCalculationFormula(int pageIndex,int pageSize,int parameterId,int plcDataCalculationFormulaId)
        {
            Pager pager = new Pager { PageIndex = pageIndex, PageSize = pageSize };
            List<SeriesDefinition> items = this.SelectBy(pager,new SeriesDefinition { Parameter = new Sesi.LocalData.Entities.LocalOperation.PlcParameter{ Id = parameterId },PlcDataCalculationFormula = new Sesi.LocalData.Entities.LocalOperation.PlcDataCalculationFormula{ Id = plcDataCalculationFormulaId } },new List<string> { "ParameterId","PlcDataCalculationFormulaId" });
            return items;
        }
		public List<SeriesDefinition> SelectByParameterPlcDataCalculationFormula(int parameterId,int plcDataCalculationFormulaId)
        {
            List<SeriesDefinition> items = this.SelectBy(new SeriesDefinition { Parameter = new Sesi.LocalData.Entities.LocalOperation.PlcParameter{ Id = parameterId },PlcDataCalculationFormula = new Sesi.LocalData.Entities.LocalOperation.PlcDataCalculationFormula{ Id = plcDataCalculationFormulaId } },new List<string> { "ParameterId","PlcDataCalculationFormulaId" });
            return items;
        }
		public List<SeriesDefinition> SelectByParameterUnitCalculationFormula(int pageIndex,int pageSize,int parameterId,int unitCalculationFormulaId)
        {
            Pager pager = new Pager { PageIndex = pageIndex, PageSize = pageSize };
            List<SeriesDefinition> items = this.SelectBy(pager,new SeriesDefinition { Parameter = new Sesi.LocalData.Entities.LocalOperation.PlcParameter{ Id = parameterId },UnitCalculationFormula = new Sesi.LocalData.Entities.LocalOperation.UnitCalculationFormula{ Id = unitCalculationFormulaId } },new List<string> { "ParameterId","UnitCalculationFormulaId" });
            return items;
        }
		public List<SeriesDefinition> SelectByParameterUnitCalculationFormula(int parameterId,int unitCalculationFormulaId)
        {
            List<SeriesDefinition> items = this.SelectBy(new SeriesDefinition { Parameter = new Sesi.LocalData.Entities.LocalOperation.PlcParameter{ Id = parameterId },UnitCalculationFormula = new Sesi.LocalData.Entities.LocalOperation.UnitCalculationFormula{ Id = unitCalculationFormulaId } },new List<string> { "ParameterId","UnitCalculationFormulaId" });
            return items;
        }
		public List<SeriesDefinition> SelectByPlcDataCalculationFormula(int pageIndex,int pageSize,int plcDataCalculationFormulaId)
        {
            Pager pager = new Pager { PageIndex = pageIndex, PageSize = pageSize };
            List<SeriesDefinition> items = this.SelectBy(pager,new SeriesDefinition { PlcDataCalculationFormula = new Sesi.LocalData.Entities.LocalOperation.PlcDataCalculationFormula{ Id = plcDataCalculationFormulaId } },new List<string> { "PlcDataCalculationFormulaId" });
            return items;
        }
		public List<SeriesDefinition> SelectByPlcDataCalculationFormula(int plcDataCalculationFormulaId)
        {
            List<SeriesDefinition> items = this.SelectBy(new SeriesDefinition { PlcDataCalculationFormula = new Sesi.LocalData.Entities.LocalOperation.PlcDataCalculationFormula{ Id = plcDataCalculationFormulaId } },new List<string> { "PlcDataCalculationFormulaId" });
            return items;
        }
		public List<SeriesDefinition> SelectByPlcDataCalculationFormulaUnitCalculationFormula(int pageIndex,int pageSize,int plcDataCalculationFormulaId,int unitCalculationFormulaId)
        {
            Pager pager = new Pager { PageIndex = pageIndex, PageSize = pageSize };
            List<SeriesDefinition> items = this.SelectBy(pager,new SeriesDefinition { PlcDataCalculationFormula = new Sesi.LocalData.Entities.LocalOperation.PlcDataCalculationFormula{ Id = plcDataCalculationFormulaId },UnitCalculationFormula = new Sesi.LocalData.Entities.LocalOperation.UnitCalculationFormula{ Id = unitCalculationFormulaId } },new List<string> { "PlcDataCalculationFormulaId","UnitCalculationFormulaId" });
            return items;
        }
		public List<SeriesDefinition> SelectByPlcDataCalculationFormulaUnitCalculationFormula(int plcDataCalculationFormulaId,int unitCalculationFormulaId)
        {
            List<SeriesDefinition> items = this.SelectBy(new SeriesDefinition { PlcDataCalculationFormula = new Sesi.LocalData.Entities.LocalOperation.PlcDataCalculationFormula{ Id = plcDataCalculationFormulaId },UnitCalculationFormula = new Sesi.LocalData.Entities.LocalOperation.UnitCalculationFormula{ Id = unitCalculationFormulaId } },new List<string> { "PlcDataCalculationFormulaId","UnitCalculationFormulaId" });
            return items;
        }
		public List<SeriesDefinition> SelectByUnitCalculationFormula(int pageIndex,int pageSize,int unitCalculationFormulaId)
        {
            Pager pager = new Pager { PageIndex = pageIndex, PageSize = pageSize };
            List<SeriesDefinition> items = this.SelectBy(pager,new SeriesDefinition { UnitCalculationFormula = new Sesi.LocalData.Entities.LocalOperation.UnitCalculationFormula{ Id = unitCalculationFormulaId } },new List<string> { "UnitCalculationFormulaId" });
            return items;
        }
		public List<SeriesDefinition> SelectByUnitCalculationFormula(int unitCalculationFormulaId)
        {
            List<SeriesDefinition> items = this.SelectBy(new SeriesDefinition { UnitCalculationFormula = new Sesi.LocalData.Entities.LocalOperation.UnitCalculationFormula{ Id = unitCalculationFormulaId } },new List<string> { "UnitCalculationFormulaId" });
            return items;
        }/*add customized code between this region*/
		/*add customized code between this region*/

	}
}
