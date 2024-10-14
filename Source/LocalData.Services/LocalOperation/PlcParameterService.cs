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
	public class PlcParameterService : Service<PlcParameter>, IPlcParameterService
	{
		public PlcParameterService() : base(typeof (IPlcParameterDao))
		{
		}

		public List<PlcParameter> SelectPlcParameterByPlcCalculations(int[] plcCalculationIds, bool isAggregatedChildren = false)
        {
            List<PlcParameter> items = this.SelectByColumnIds("PlcCalculationId",plcCalculationIds,isAggregatedChildren);
            return items;
        }
		public List<PlcParameter> SelectPlcParameterByPlcCalculations(Pager pager, int[] plcCalculationIds, bool isAggregatedChildren = false)
        {
            List<PlcParameter> items = this.SelectByColumnIds(pager,"PlcCalculationId",plcCalculationIds,isAggregatedChildren);
            return items;
        }
		public List<PlcParameter> SelectByPlcCalculation(int pageIndex,int pageSize,int plcCalculationId)
        {
            Pager pager = new Pager { PageIndex = pageIndex, PageSize = pageSize };
            List<PlcParameter> items = this.SelectBy(pager,new PlcParameter { PlcCalculation = new Sesi.LocalData.Entities.LocalOperation.PlcDataCalculationFormula{ Id = plcCalculationId } },new List<string> { "PlcCalculationId" });
            return items;
        }
		public List<PlcParameter> SelectByPlcCalculation(int plcCalculationId)
        {
            List<PlcParameter> items = this.SelectBy(new PlcParameter { PlcCalculation = new Sesi.LocalData.Entities.LocalOperation.PlcDataCalculationFormula{ Id = plcCalculationId } },new List<string> { "PlcCalculationId" });
            return items;
        }/*add customized code between this region*/
		/*add customized code between this region*/

	}
}
