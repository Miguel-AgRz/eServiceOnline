using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MetaShare.Common.Core.Entities;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Entities.LocalOperation
{
	public class UploadLog : MetaShare.Common.Core.Entities.Common
	{
		[DataMember]
		public int Version {get; set;}

		[DataMember]
		public DateTime StartTime {get; set;}

		[DataMember]
		public string JobUniqueId {get; set;}

		[DataMember]
		public string JobNumber {get; set;}

		[DataMember]
		public DateTime PackingEndDateTime {get; set;}

		[DataMember]
		public bool IsRecievedOnServer {get; set;}

		[DataMember]
		public bool IsPlcDataDeleted {get; set;}

		[DataMember]
		public string TimeZone {get; set;}

		[DataMember]
		public string ComputerName {get; set;}

		[DataMember]
		public DateTime EndTime {get; set;}

		[DataMember]
		public string PackingDuration {get; set;}

		[DataMember]
		public int PackSize {get; set;}

	/*add customized code between this region*/
	/*add customized code between this region*/
	}
}
