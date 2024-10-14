using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MetaShare.Common.Core.Entities;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Entities.LocalOperation
{
	public class JobMonitorSetting : MetaShare.Common.Core.Entities.Common
	{
		[DataMember]
		public int Frequency {get; set;}

		[DataMember]
		public string JobUniqueId {get; set;}

		[DataMember]
		public int Duration {get; set;}

		[DataMember]
		public string JobNumber {get; set;}

		[DataMember]
		public List<ChartDefinition> ChartDefinitions {get; set;}

		[DataMember]
		public int Interval {get; set;}

	/*add customized code between this region*/
	/*add customized code between this region*/
	}
}
