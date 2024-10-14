using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MetaShare.Common.Core.Entities;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Entities.Legacy
{
	public class UPLOAD_LOG : MetaShare.Common.Core.Entities.Common
	{
		[DataMember]
		public string JOB_NUMBER {get; set;}

		[DataMember]
		public bool IS_CLEANED_UP {get; set;}

		[DataMember]
		public string JOB_UNIQUE_ID {get; set;}

		[DataMember]
		public string TIMEZONE {get; set;}

		[DataMember]
		public bool IS_RECEIVED_ON_SERVER {get; set;}

		[DataMember]
		public string COMPUTER_NAME {get; set;}

		[DataMember]
		public int VERSION {get; set;}

		[DataMember]
		public DateTime END_TIME {get; set;}

		[DataMember]
		public DateTime PACKING_TIME {get; set;}

		[DataMember]
		public string PACKING_DURATION {get; set;}

		[DataMember]
		public int PACK_SIZE {get; set;}

		[DataMember]
		public DateTime START_TIME {get; set;}

	/*add customized code between this region*/
	/*add customized code between this region*/
	}
}
