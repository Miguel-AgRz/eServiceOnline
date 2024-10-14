using System.Collections.Generic;
using MetaShare.Common.Core.Entities;
using MetaShare.Common.Core.Services;
using Sesi.LocalData.Entities.LocalOperation;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Services.Interfaces.LocalOperation
{
	public interface ISeriesDefinitionService : IPagingService<SeriesDefinition>
	{
	        List<SeriesDefinition> SelectAllWithReferenceData(List<SeriesDefinition> items);
	        List<SeriesDefinition> SelectSeriesDefinitionByPlcDataCalculationFormulas(int[] plcDataCalculationFormulaIds, bool isAggregatedChildren = false);
	        List<SeriesDefinition> SelectSeriesDefinitionByPlcDataCalculationFormulas(Pager pager,int[] plcDataCalculationFormulaIds, bool isAggregatedChildren = false);
	        List<SeriesDefinition> SelectSeriesDefinitionByParameters(int[] parameterIds, bool isAggregatedChildren = false);
	        List<SeriesDefinition> SelectSeriesDefinitionByParameters(Pager pager,int[] parameterIds, bool isAggregatedChildren = false);
	        List<SeriesDefinition> SelectSeriesDefinitionByUnitCalculationFormulas(int[] unitCalculationFormulaIds, bool isAggregatedChildren = false);
	        List<SeriesDefinition> SelectSeriesDefinitionByUnitCalculationFormulas(Pager pager,int[] unitCalculationFormulaIds, bool isAggregatedChildren = false);
	        List<SeriesDefinition> SelectSeriesDefinitionByCharts(int[] chartIds, bool isAggregatedChildren = false);
	        List<SeriesDefinition> SelectSeriesDefinitionByCharts(Pager pager,int[] chartIds, bool isAggregatedChildren = false);
	        List<SeriesDefinition> SelectByChart(int pageIndex,int pageSize,int chartId);
	        List<SeriesDefinition> SelectByChart(int chartId);
	        List<SeriesDefinition> SelectByChartParameter(int pageIndex,int pageSize,int chartId,int parameterId);
	        List<SeriesDefinition> SelectByChartParameter(int chartId,int parameterId);
	        List<SeriesDefinition> SelectByChartPlcDataCalculationFormula(int pageIndex,int pageSize,int chartId,int plcDataCalculationFormulaId);
	        List<SeriesDefinition> SelectByChartPlcDataCalculationFormula(int chartId,int plcDataCalculationFormulaId);
	        List<SeriesDefinition> SelectByChartUnitCalculationFormula(int pageIndex,int pageSize,int chartId,int unitCalculationFormulaId);
	        List<SeriesDefinition> SelectByChartUnitCalculationFormula(int chartId,int unitCalculationFormulaId);
	        List<SeriesDefinition> SelectByParameter(int pageIndex,int pageSize,int parameterId);
	        List<SeriesDefinition> SelectByParameter(int parameterId);
	        List<SeriesDefinition> SelectByParameterPlcDataCalculationFormula(int pageIndex,int pageSize,int parameterId,int plcDataCalculationFormulaId);
	        List<SeriesDefinition> SelectByParameterPlcDataCalculationFormula(int parameterId,int plcDataCalculationFormulaId);
	        List<SeriesDefinition> SelectByParameterUnitCalculationFormula(int pageIndex,int pageSize,int parameterId,int unitCalculationFormulaId);
	        List<SeriesDefinition> SelectByParameterUnitCalculationFormula(int parameterId,int unitCalculationFormulaId);
	        List<SeriesDefinition> SelectByPlcDataCalculationFormula(int pageIndex,int pageSize,int plcDataCalculationFormulaId);
	        List<SeriesDefinition> SelectByPlcDataCalculationFormula(int plcDataCalculationFormulaId);
	        List<SeriesDefinition> SelectByPlcDataCalculationFormulaUnitCalculationFormula(int pageIndex,int pageSize,int plcDataCalculationFormulaId,int unitCalculationFormulaId);
	        List<SeriesDefinition> SelectByPlcDataCalculationFormulaUnitCalculationFormula(int plcDataCalculationFormulaId,int unitCalculationFormulaId);
	        List<SeriesDefinition> SelectByUnitCalculationFormula(int pageIndex,int pageSize,int unitCalculationFormulaId);
	        List<SeriesDefinition> SelectByUnitCalculationFormula(int unitCalculationFormulaId);
	/*add customized code between this region*/
	/*add customized code between this region*/
	}
}
