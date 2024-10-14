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
	public class PlcDataCalculationFormulaService : Service<PlcDataCalculationFormula>, IPlcDataCalculationFormulaService
	{
		public PlcDataCalculationFormulaService() : base(typeof (IPlcDataCalculationFormulaDao))
		{
		}

		public List<PlcDataCalculationFormula> SelectPlcDataCalculationFormulaByUnitCalculations(int[] unitCalculationIds, bool isAggregatedChildren = false)
        {
            List<PlcDataCalculationFormula> items = this.SelectByColumnIds("UnitCalculationId",unitCalculationIds,isAggregatedChildren);
            return items;
        }
		public List<PlcDataCalculationFormula> SelectPlcDataCalculationFormulaByUnitCalculations(Pager pager, int[] unitCalculationIds, bool isAggregatedChildren = false)
        {
            List<PlcDataCalculationFormula> items = this.SelectByColumnIds(pager,"UnitCalculationId",unitCalculationIds,isAggregatedChildren);
            return items;
        }
		public List<PlcDataCalculationFormula> SelectByUnitCalculation(int pageIndex,int pageSize,int unitCalculationId)
        {
            Pager pager = new Pager { PageIndex = pageIndex, PageSize = pageSize };
            List<PlcDataCalculationFormula> items = this.SelectBy(pager,new PlcDataCalculationFormula { UnitCalculation = new Sesi.LocalData.Entities.LocalOperation.UnitCalculationFormula{ Id = unitCalculationId } },new List<string> { "UnitCalculationId" });
            return items;
        }
		public List<PlcDataCalculationFormula> SelectByUnitCalculation(int unitCalculationId)
        {
            List<PlcDataCalculationFormula> items = this.SelectBy(new PlcDataCalculationFormula { UnitCalculation = new Sesi.LocalData.Entities.LocalOperation.UnitCalculationFormula{ Id = unitCalculationId } },new List<string> { "UnitCalculationId" });
            return items;
        }/*add customized code between this region*/
		/*add customized code between this region*/

	}
}
