using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MetaShare.Common.Core.Entities;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Entities.LocalOperation
{
	public class ChartDefinition : MetaShare.Common.Core.Entities.Common
	{
		[DataMember]
		public bool IsPrintTogether {get; set;}

		[DataMember]
		public bool IsEnabled {get; set;}

		[DataMember]
		public double SecondaryYAxisInterval {get; set;}

		[DataMember]
		public double YAxisInterval {get; set;}

		[DataMember]
		public List<SeriesDefinition> SeriesDefinitions {get; set;}

		[DataMember]
		public JobMonitorSetting JobMonSetting {get; set;}

		[DataMember]
		public double YAxisMin {get; set;}

		[DataMember]
		public string YAxisUom {get; set;}

		[DataMember]
		public string SecondaryYAxisUom {get; set;}

		[DataMember]
		public double SecondaryYAxisMax {get; set;}

		[DataMember]
		public double SecondaryYAxisMin {get; set;}

		[DataMember]
		public double YAxisMax {get; set;}

		[DataMember]
		public string Title {get; set;}

		[DataMember]
		public bool ExistSecondAxis {get; set;}

		[DataMember]
		public string LabelFormat {get; set;}

	/*add customized code between this region*/
	/*add customized code between this region*/
	}
}
