using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MetaShare.Common.Core.Entities;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Entities.PostJobReport
{
	public class BlendConsumption : ObjectVersion
	{
		[DataMember]
		public string BlendCategory {get; set;}

		[DataMember]
		public BlendReport BlendReport {get; set;}

		[DataMember]
		public double SlurryTemperature {get; set;}

		[DataMember]
		public double BulkTemperature {get; set;}

		[DataMember]
		public string JobIntervalTypeName {get; set;}

		[DataMember]
		public int JobIntervalId {get; set;}

		[DataMember]
		public double WaterTemperature {get; set;}

		[DataMember]
		public string BlendName {get; set;}

		[DataMember]
		public int JobEventNumber {get; set;}

		[DataMember]
		public string BlendDescription {get; set;}

		[DataMember]
		public double PumpedVolume {get; set;}

	/*add customized code between this region*/
	/*add customized code between this region*/
	}
}
