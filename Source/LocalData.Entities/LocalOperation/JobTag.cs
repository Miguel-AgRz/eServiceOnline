using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MetaShare.Common.Core.Entities;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Entities.LocalOperation
{
	public class JobTag : MetaShare.Common.Core.Entities.Common
	{
		[DataMember]
		public bool HasDataFromCsv {get; set;}

		[DataMember]
		public string PrintingSetting {get; set;}

		[DataMember]
		public string SurfaceLocation {get; set;}

		[DataMember]
		public string Timezone {get; set;}

		[DataMember]
		public string ClientCompany {get; set;}

		[DataMember]
		public string ComputerName {get; set;}

		[DataMember]
		public bool IsDataFromCsv {get; set;}

		[DataMember]
		public string TruckUnitSelection {get; set;}

		[DataMember]
		public bool IsCurrentJob {get; set;}

		[DataMember]
		public string ClientRep {get; set;}

		[DataMember]
		public string DownHoleLocation {get; set;}

		[DataMember]
		public string AppicationVersion {get; set;}

		[DataMember]
		public DateTime JobEndTime {get; set;}

		[DataMember]
		public string TimeArea {get; set;}

		[DataMember]
		public string Comments {get; set;}

		[DataMember]
		public int Version {get; set;}

		[DataMember]
		public string JobNumber {get; set;}

		[DataMember]
		public DateTime JobDateTime {get; set;}

		[DataMember]
		public string ServicePoint {get; set;}

		[DataMember]
		public string WellName {get; set;}

		[DataMember]
		public string Status {get; set;}

		[DataMember]
		public DateTime JobStartTime {get; set;}

		[DataMember]
		public string RigName {get; set;}

		[DataMember]
		public string JobUniqueId {get; set;}

		[DataMember]
		public bool IsDstOff {get; set;}

		[DataMember]
		public string JobType {get; set;}

		[DataMember]
		public string Supervisor {get; set;}

		[DataMember]
		public string WITSSetting {get; set;}

		[DataMember]
		public string JobMonitorSetting {get; set;}

	/*add customized code between this region*/
	/*add customized code between this region*/
	}
}
