using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MetaShare.Common.Core.Entities;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Entities.LocalOperation
{
	public class PlcDataCalculationFormula : MetaShare.Common.Core.Entities.Common
	{
		[DataMember]
		public string TruckUnitNumber {get; set;}

		[DataMember]
		public UnitCalculationFormula UnitCalculation {get; set;}

		[DataMember]
		public List<PlcParameter> Parameters {get; set;}

		[DataMember]
		public bool IsTemplate {get; set;}

		[DataMember]
		public string Expression {get; set;}

		[DataMember]
		public string Title {get; set;}

		[DataMember]
		public bool IsEnabled {get; set;}

	/*add customized code between this region*/
	/*add customized code between this region*/
	}
}
