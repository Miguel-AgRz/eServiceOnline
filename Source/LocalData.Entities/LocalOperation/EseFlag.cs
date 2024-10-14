using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MetaShare.Common.Core.Entities;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Entities.LocalOperation
{
	public class EseFlag : MetaShare.Common.Core.Entities.Common
	{
		[DataMember]
		public bool Value {get; set;}

		[DataMember]
		public int Version {get; set;}

		[DataMember]
		public DateTime TimeStamp {get; set;}

	/*add customized code between this region*/
	/*add customized code between this region*/
	}
}
