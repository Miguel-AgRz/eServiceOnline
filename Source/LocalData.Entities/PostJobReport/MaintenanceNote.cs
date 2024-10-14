using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MetaShare.Common.Core.Entities;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Entities.PostJobReport
{
	public class MaintenanceNote : ObjectVersion
	{
		[DataMember]
		public string UnitNumber {get; set;}

		[DataMember]
		public string JobUniqueId {get; set;}

		[DataMember]
		public string JobNumber {get; set;}

		[DataMember]
		public string UnitType {get; set;}

		[DataMember]
		public string Notes {get; set;}

	/*add customized code between this region*/
	/*add customized code between this region*/
	}
}
