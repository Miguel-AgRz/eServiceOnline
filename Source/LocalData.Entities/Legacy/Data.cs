using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MetaShare.Common.Core.Entities;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Entities.Legacy
{
	public class Data : MetaShare.Common.Core.Entities.Common
	{
		[DataMember]
		public string JSON {get; set;}

		[DataMember]
		public string UnitID {get; set;}

		[DataMember]
		public DateTime TimeStamp {get; set;}

	/*add customized code between this region*/
	/*add customized code between this region*/
	}
}
