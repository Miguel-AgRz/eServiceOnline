using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MetaShare.Common.Core.Entities;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Entities.PostJobReport
{
	public class StorageInfo : ObjectVersion
	{
		[DataMember]
		public double Remains {get; set;}

		[DataMember]
		public string JobNumber {get; set;}

		[DataMember]
		public double ScaleReading {get; set;}

		[DataMember]
		public double PumpedWithAdds {get; set;}

		[DataMember]
		public string JobUniqueId {get; set;}

		[DataMember]
		public bool IsReadyForNext {get; set;}

		[DataMember]
		public double PumpedWoAdds {get; set;}

		[DataMember]
		public double InitialTonnage {get; set;}

		[DataMember]
		public string StorageType {get; set;}

		[DataMember]
		public string BlendName {get; set;}

	/*add customized code between this region*/
	/*add customized code between this region*/
	}
}
