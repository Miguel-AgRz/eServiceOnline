using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MetaShare.Common.Core.Entities;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Entities.LocalOperation
{
	public class PlcParameter : MetaShare.Common.Core.Entities.Common
	{
		[DataMember]
		public PlcDataCalculationFormula PlcCalculation {get; set;}

		[DataMember]
		public string Comments {get; set;}

		[DataMember]
		public string Uom {get; set;}

		[DataMember]
		public int DataIndex {get; set;}

		[DataMember]
		public string DataType {get; set;}

	/*add customized code between this region*/
	/*add customized code between this region*/
	}
}
