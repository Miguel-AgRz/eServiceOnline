using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MetaShare.Common.Core.Entities;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Entities.LocalOperation
{
	public class SeriesDefinition : MetaShare.Common.Core.Entities.Common
	{
		[DataMember]
		public PlcDataCalculationFormula PlcDataCalculationFormula {get; set;}

		[DataMember]
		public string Title {get; set;}

		[DataMember]
		public string YAxisName {get; set;}

		[DataMember]
		public PlcParameter Parameter {get; set;}

		[DataMember]
		public UnitCalculationFormula UnitCalculationFormula {get; set;}

		[DataMember]
		public ChartDefinition Chart {get; set;}

		[DataMember]
		public string ActionName {get; set;}

		[DataMember]
		public string TruckUnitNumber {get; set;}

		[DataMember]
		public string Color {get; set;}

		[DataMember]
		public string ControllerName {get; set;}

		[DataMember]
		public bool IsEnabled {get; set;}

		[DataMember]
		public bool IsTemplate {get; set;}

		[DataMember]
		public bool IsSecondaryYAxis {get; set;}

		[DataMember]
		public double MaxValue {get; set;}

	/*add customized code between this region*/
	/*add customized code between this region*/
	}
}
