using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MetaShare.Common.Core.Entities;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Entities.LocalOperation
{
	public class UnitCalculationFormula : MetaShare.Common.Core.Entities.Common
	{
		[DataMember]
		public List<PlcDataCalculationFormula> PlcDataCalculationFormulas {get; set;}

		[DataMember]
		public string Expression {get; set;}

	/*add customized code between this region*/
	/*add customized code between this region*/
	}
}
