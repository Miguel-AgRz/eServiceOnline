using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MetaShare.Common.Core.Entities;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Entities.PostJobReport
{
	public class BlendReport : ObjectVersion
	{
		[DataMember]
		public string JobUniqueId {get; set;}

		[DataMember]
		public double TotalPumpedVolume {get; set;}

		[DataMember]
		public List<BlendConsumption> BlendConsumptions {get; set;}

		[DataMember]
		public double ExpectedCementTop {get; set;}

	/*add customized code between this region*/
	/*add customized code between this region*/
	}
}
