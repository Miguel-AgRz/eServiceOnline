using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MetaShare.Common.Core.Entities;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Entities.Legacy
{
	public class ESE_FLAGS : MetaShare.Common.Core.Entities.Common
	{
		[DataMember]
		public bool VALUE {get; set;}

		[DataMember]
		public int VERSION {get; set;}

		[DataMember]
		public DateTime TIMESTAMP {get; set;}

	/*add customized code between this region*/
	/*add customized code between this region*/
	}
}
