using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MetaShare.Common.Core.Entities;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Entities.LocalOperation
{
	public class PrintingSetting : MetaShare.Common.Core.Entities.Common
	{
		[DataMember]
		public bool IsDataFromCsv {get; set;}

		[DataMember]
		public DateTime EndTime {get; set;}

		[DataMember]
		public DateTime StartTime {get; set;}

	/*add customized code between this region*/
	/*add customized code between this region*/
	}
}
