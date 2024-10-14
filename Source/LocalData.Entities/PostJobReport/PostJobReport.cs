using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MetaShare.Common.Core.Entities;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Entities.PostJobReport
{
	public class PostJobReport : ObjectVersion
	{
		[DataMember]
		public string SurfaceLocation {get; set;}

		[DataMember]
		public string CallSheetNumber {get; set;}

		[DataMember]
		public string RevisedDirection {get; set;}

		[DataMember]
		public string JobUniqueId {get; set;}

		[DataMember]
		public string AdditionalInformation {get; set;}

		[DataMember]
		public string ClientName {get; set;}

		[DataMember]
		public string JobNumber {get; set;}

		[DataMember]
		public bool IsDirectionRevised {get; set;}

		[DataMember]
		public string RigName {get; set;}

		[DataMember]
		public string JobType {get; set;}

		[DataMember]
		public string DownHoleLocation {get; set;}

		[DataMember]
		public DateTime JobDate {get; set;}

	/*add customized code between this region*/
	/*add customized code between this region*/
	}
}
