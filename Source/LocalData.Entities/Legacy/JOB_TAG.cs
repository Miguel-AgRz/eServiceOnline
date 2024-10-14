using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MetaShare.Common.Core.Entities;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Entities.Legacy
{
	public class JOB_TAG : MetaShare.Common.Core.Entities.Common
	{
		[DataMember]
		public DateTime JOB_END_TIME {get; set;}

		[DataMember]
		public int VERSION {get; set;}

		[DataMember]
		public string RIG_NAME {get; set;}

		[DataMember]
		public string JOB_MONITOR_SETTING {get; set;}

		[DataMember]
		public bool IS_DST_OFF {get; set;}

		[DataMember]
		public DateTime JOB_DATE_TIME {get; set;}

		[DataMember]
		public string SUPERVISOR {get; set;}

		[DataMember]
		public string CLIENT_REP {get; set;}

		[DataMember]
		public string WITS_SETTING {get; set;}

		[DataMember]
		public string UNIT_SELECTION {get; set;}

		[DataMember]
		public string SERVICE_POINT {get; set;}

		[DataMember]
		public string TIMEZONE {get; set;}

		[DataMember]
		public string JOB_NUMBER {get; set;}

		[DataMember]
		public string STATUS {get; set;}

		[DataMember]
		public string JOB_PRINT_SETTING {get; set;}

		[DataMember]
		public string WELL_NAME {get; set;}

		[DataMember]
		public string TIME_AREA {get; set;}

		[DataMember]
		public DateTime JOB_START_TIME {get; set;}

		[DataMember]
		public string COMMENTS {get; set;}

		[DataMember]
		public bool HAS_DATA_FROM_CSV {get; set;}

		[DataMember]
		public string JOB_TYPE {get; set;}

		[DataMember]
		public string SURFACE_LOCATION {get; set;}

		[DataMember]
		public string COMPUTER_NAME {get; set;}

		[DataMember]
		public string CLIENT_COMPANY {get; set;}

		[DataMember]
		public string DOWNHOLE_LOCATION {get; set;}

		[DataMember]
		public string APPLICATION_VERSION {get; set;}

		[DataMember]
		public bool IS_CURRENT_JOB {get; set;}

		[DataMember]
		public bool IS_DATA_FROM_CSV {get; set;}

		[DataMember]
		public string JOB_UNIQUE_ID {get; set;}

	/*add customized code between this region*/
	/*add customized code between this region*/
	}
}
