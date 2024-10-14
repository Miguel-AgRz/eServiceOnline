using System.Collections.Generic;
using MetaShare.Common.Core.Entities;
using MetaShare.Common.Core.Services;
using Sesi.LocalData.Entities.LocalOperation;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Services.Interfaces.LocalOperation
{
	public interface IPlcDataCalculationFormulaService : IPagingService<PlcDataCalculationFormula>
	{
	        List<PlcDataCalculationFormula> SelectPlcDataCalculationFormulaByUnitCalculations(int[] unitCalculationIds, bool isAggregatedChildren = false);
	        List<PlcDataCalculationFormula> SelectPlcDataCalculationFormulaByUnitCalculations(Pager pager,int[] unitCalculationIds, bool isAggregatedChildren = false);
	        List<PlcDataCalculationFormula> SelectByUnitCalculation(int pageIndex,int pageSize,int unitCalculationId);
	        List<PlcDataCalculationFormula> SelectByUnitCalculation(int unitCalculationId);
	/*add customized code between this region*/
	/*add customized code between this region*/
	}
}
