using System.Collections.Generic;
using MetaShare.Common.Core.Entities;
using MetaShare.Common.Core.Services;
using Sesi.LocalData.Entities.LocalOperation;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Services.Interfaces.LocalOperation
{
	public interface IPlcParameterService : IPagingService<PlcParameter>
	{
	        List<PlcParameter> SelectPlcParameterByPlcCalculations(int[] plcCalculationIds, bool isAggregatedChildren = false);
	        List<PlcParameter> SelectPlcParameterByPlcCalculations(Pager pager,int[] plcCalculationIds, bool isAggregatedChildren = false);
	        List<PlcParameter> SelectByPlcCalculation(int pageIndex,int pageSize,int plcCalculationId);
	        List<PlcParameter> SelectByPlcCalculation(int plcCalculationId);
	/*add customized code between this region*/
	/*add customized code between this region*/
	}
}
