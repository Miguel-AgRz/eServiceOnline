using System;
using System.Data;
using MetaShare.Common.Core.Daos;
using Sesi.LocalData.Daos.Interfaces.LocalOperation;
using Sesi.LocalData.Entities.LocalOperation;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Daos.LocalOperation
{
	public class JobTagDao : CommonObjectDao<JobTag>, IJobTagDao
	{
		public class JobTagSqlBuilder : ObjectSqlBuilder
		{
			public JobTagSqlBuilder(SqlDialect sqlDialect) : base(sqlDialect,"JobTag")
			{
				this.SqlInsert = "INSERT INTO JobTag (JobMonitorSetting,JobEndTime,PrintingSetting,JobType,JobStartTime,ClientCompany,JobNumber,HasDataFromCsv,IsDataFromCsv,RigName,ComputerName,ServicePoint,TimeArea,TruckUnitSelection,SurfaceLocation,DownHoleLocation,WITSSetting,Status,IsCurrentJob,Supervisor,WellName,Version,AppicationVersion,Timezone,JobUniqueId,IsDstOff,Comments,JobDateTime,ClientRep," + this.SqlBaseFieldInsertFront + ") VALUES (@JobMonitorSetting,@JobEndTime,@PrintingSetting,@JobType,@JobStartTime,@ClientCompany,@JobNumber,@HasDataFromCsv,@IsDataFromCsv,@RigName,@ComputerName,@ServicePoint,@TimeArea,@TruckUnitSelection,@SurfaceLocation,@DownHoleLocation,@WITSSetting,@Status,@IsCurrentJob,@Supervisor,@WellName,@Version,@AppicationVersion,@Timezone,@JobUniqueId,@IsDstOff,@Comments,@JobDateTime,@ClientRep," + this.SqlBaseFieldInsertBack + ")";
				this.SqlUpdate = "UPDATE JobTag SET JobMonitorSetting=@JobMonitorSetting,JobEndTime=@JobEndTime,PrintingSetting=@PrintingSetting,JobType=@JobType,JobStartTime=@JobStartTime,ClientCompany=@ClientCompany,JobNumber=@JobNumber,HasDataFromCsv=@HasDataFromCsv,IsDataFromCsv=@IsDataFromCsv,RigName=@RigName,ComputerName=@ComputerName,ServicePoint=@ServicePoint,TimeArea=@TimeArea,TruckUnitSelection=@TruckUnitSelection,SurfaceLocation=@SurfaceLocation,DownHoleLocation=@DownHoleLocation,WITSSetting=@WITSSetting,Status=@Status,IsCurrentJob=@IsCurrentJob,Supervisor=@Supervisor,WellName=@WellName,Version=@Version,AppicationVersion=@AppicationVersion,Timezone=@Timezone,JobUniqueId=@JobUniqueId,IsDstOff=@IsDstOff,Comments=@Comments,JobDateTime=@JobDateTime,ClientRep=@ClientRep," + this.SqlBaseFieldUpdate + " WHERE Id=@Id";
			}
		}

		public class JobTagResultHandler : CommonObjectResultHandler<JobTag>
		{
			public override void GetColumnValues(IDataReader reader, JobTag item)
			{
				base.GetColumnValues(reader, item);
				int ordinalHasDataFromCsv = reader.GetOrdinal("HasDataFromCsv");
				item.HasDataFromCsv = !reader.IsDBNull(ordinalHasDataFromCsv) && reader.GetBoolean(ordinalHasDataFromCsv);
				int ordinalPrintingSetting = reader.GetOrdinal("PrintingSetting");
				item.PrintingSetting = reader.IsDBNull(ordinalPrintingSetting) ? null : reader.GetString(ordinalPrintingSetting);
				int ordinalSurfaceLocation = reader.GetOrdinal("SurfaceLocation");
				item.SurfaceLocation = reader.IsDBNull(ordinalSurfaceLocation) ? null : reader.GetString(ordinalSurfaceLocation);
				int ordinalTimezone = reader.GetOrdinal("Timezone");
				item.Timezone = reader.IsDBNull(ordinalTimezone) ? null : reader.GetString(ordinalTimezone);
				int ordinalClientCompany = reader.GetOrdinal("ClientCompany");
				item.ClientCompany = reader.IsDBNull(ordinalClientCompany) ? null : reader.GetString(ordinalClientCompany);
				int ordinalComputerName = reader.GetOrdinal("ComputerName");
				item.ComputerName = reader.IsDBNull(ordinalComputerName) ? null : reader.GetString(ordinalComputerName);
				int ordinalIsDataFromCsv = reader.GetOrdinal("IsDataFromCsv");
				item.IsDataFromCsv = !reader.IsDBNull(ordinalIsDataFromCsv) && reader.GetBoolean(ordinalIsDataFromCsv);
				int ordinalTruckUnitSelection = reader.GetOrdinal("TruckUnitSelection");
				item.TruckUnitSelection = reader.IsDBNull(ordinalTruckUnitSelection) ? null : reader.GetString(ordinalTruckUnitSelection);
				int ordinalIsCurrentJob = reader.GetOrdinal("IsCurrentJob");
				item.IsCurrentJob = !reader.IsDBNull(ordinalIsCurrentJob) && reader.GetBoolean(ordinalIsCurrentJob);
				int ordinalClientRep = reader.GetOrdinal("ClientRep");
				item.ClientRep = reader.IsDBNull(ordinalClientRep) ? null : reader.GetString(ordinalClientRep);
				int ordinalDownHoleLocation = reader.GetOrdinal("DownHoleLocation");
				item.DownHoleLocation = reader.IsDBNull(ordinalDownHoleLocation) ? null : reader.GetString(ordinalDownHoleLocation);
				int ordinalAppicationVersion = reader.GetOrdinal("AppicationVersion");
				item.AppicationVersion = reader.IsDBNull(ordinalAppicationVersion) ? null : reader.GetString(ordinalAppicationVersion);
				int ordinalJobEndTime = reader.GetOrdinal("JobEndTime");
				item.JobEndTime = reader.IsDBNull(ordinalJobEndTime) ? DateTime.MinValue : reader.GetDateTime(ordinalJobEndTime);
				int ordinalTimeArea = reader.GetOrdinal("TimeArea");
				item.TimeArea = reader.IsDBNull(ordinalTimeArea) ? null : reader.GetString(ordinalTimeArea);
				int ordinalComments = reader.GetOrdinal("Comments");
				item.Comments = reader.IsDBNull(ordinalComments) ? null : reader.GetString(ordinalComments);
				int ordinalVersion = reader.GetOrdinal("Version");
				item.Version = reader.IsDBNull(ordinalVersion) ? 0 : reader.GetInt32(ordinalVersion);
				int ordinalJobNumber = reader.GetOrdinal("JobNumber");
				item.JobNumber = reader.IsDBNull(ordinalJobNumber) ? null : reader.GetString(ordinalJobNumber);
				int ordinalJobDateTime = reader.GetOrdinal("JobDateTime");
				item.JobDateTime = reader.IsDBNull(ordinalJobDateTime) ? DateTime.MinValue : reader.GetDateTime(ordinalJobDateTime);
				int ordinalServicePoint = reader.GetOrdinal("ServicePoint");
				item.ServicePoint = reader.IsDBNull(ordinalServicePoint) ? null : reader.GetString(ordinalServicePoint);
				int ordinalWellName = reader.GetOrdinal("WellName");
				item.WellName = reader.IsDBNull(ordinalWellName) ? null : reader.GetString(ordinalWellName);
				int ordinalStatus = reader.GetOrdinal("Status");
				item.Status = reader.IsDBNull(ordinalStatus) ? null : reader.GetString(ordinalStatus);
				int ordinalJobStartTime = reader.GetOrdinal("JobStartTime");
				item.JobStartTime = reader.IsDBNull(ordinalJobStartTime) ? DateTime.MinValue : reader.GetDateTime(ordinalJobStartTime);
				int ordinalRigName = reader.GetOrdinal("RigName");
				item.RigName = reader.IsDBNull(ordinalRigName) ? null : reader.GetString(ordinalRigName);
				int ordinalJobUniqueId = reader.GetOrdinal("JobUniqueId");
				item.JobUniqueId = reader.IsDBNull(ordinalJobUniqueId) ? null : reader.GetString(ordinalJobUniqueId);
				int ordinalIsDstOff = reader.GetOrdinal("IsDstOff");
				item.IsDstOff = !reader.IsDBNull(ordinalIsDstOff) && reader.GetBoolean(ordinalIsDstOff);
				int ordinalJobType = reader.GetOrdinal("JobType");
				item.JobType = reader.IsDBNull(ordinalJobType) ? null : reader.GetString(ordinalJobType);
				int ordinalSupervisor = reader.GetOrdinal("Supervisor");
				item.Supervisor = reader.IsDBNull(ordinalSupervisor) ? null : reader.GetString(ordinalSupervisor);
				int ordinalWITSSetting = reader.GetOrdinal("WITSSetting");
				item.WITSSetting = reader.IsDBNull(ordinalWITSSetting) ? null : reader.GetString(ordinalWITSSetting);
				int ordinalJobMonitorSetting = reader.GetOrdinal("JobMonitorSetting");
				item.JobMonitorSetting = reader.IsDBNull(ordinalJobMonitorSetting) ? null : reader.GetString(ordinalJobMonitorSetting);
				/*add customized code between this region*/
				/*add customized code between this region*/
			}

			public override void AddInsertParameters(IContext context, IDbCommand command, JobTag item)
			{
				base.AddInsertParameters(context, command, item);
				context.AddParameter(command, "HasDataFromCsv", item.HasDataFromCsv);
				context.AddParameter(command, "PrintingSetting", item.PrintingSetting ?? (object) DBNull.Value);
				context.AddParameter(command, "SurfaceLocation", item.SurfaceLocation ?? (object) DBNull.Value);
				context.AddParameter(command, "Timezone", item.Timezone ?? (object) DBNull.Value);
				context.AddParameter(command, "ClientCompany", item.ClientCompany ?? (object) DBNull.Value);
				context.AddParameter(command, "ComputerName", item.ComputerName ?? (object) DBNull.Value);
				context.AddParameter(command, "IsDataFromCsv", item.IsDataFromCsv);
				context.AddParameter(command, "TruckUnitSelection", item.TruckUnitSelection ?? (object) DBNull.Value);
				context.AddParameter(command, "IsCurrentJob", item.IsCurrentJob);
				context.AddParameter(command, "ClientRep", item.ClientRep ?? (object) DBNull.Value);
				context.AddParameter(command, "DownHoleLocation", item.DownHoleLocation ?? (object) DBNull.Value);
				context.AddParameter(command, "AppicationVersion", item.AppicationVersion ?? (object) DBNull.Value);
				context.AddParameter(command, "JobEndTime", item.JobEndTime == DateTime.MinValue ? (object)DBNull.Value : item.JobEndTime);
				context.AddParameter(command, "TimeArea", item.TimeArea ?? (object) DBNull.Value);
				context.AddParameter(command, "Comments", item.Comments ?? (object) DBNull.Value);
				context.AddParameter(command, "Version", item.Version);
				context.AddParameter(command, "JobNumber", item.JobNumber ?? (object) DBNull.Value);
				context.AddParameter(command, "JobDateTime", item.JobDateTime == DateTime.MinValue ? (object)DBNull.Value : item.JobDateTime);
				context.AddParameter(command, "ServicePoint", item.ServicePoint ?? (object) DBNull.Value);
				context.AddParameter(command, "WellName", item.WellName ?? (object) DBNull.Value);
				context.AddParameter(command, "Status", item.Status ?? (object) DBNull.Value);
				context.AddParameter(command, "JobStartTime", item.JobStartTime == DateTime.MinValue ? (object)DBNull.Value : item.JobStartTime);
				context.AddParameter(command, "RigName", item.RigName ?? (object) DBNull.Value);
				context.AddParameter(command, "JobUniqueId", item.JobUniqueId ?? (object) DBNull.Value);
				context.AddParameter(command, "IsDstOff", item.IsDstOff);
				context.AddParameter(command, "JobType", item.JobType ?? (object) DBNull.Value);
				context.AddParameter(command, "Supervisor", item.Supervisor ?? (object) DBNull.Value);
				context.AddParameter(command, "WITSSetting", item.WITSSetting ?? (object) DBNull.Value);
				context.AddParameter(command, "JobMonitorSetting", item.JobMonitorSetting ?? (object) DBNull.Value);
				/*add customized code between this region*/
				/*add customized code between this region*/
			}
		}

		public JobTagDao(SqlDialect sqlDialect) : base(new JobTagSqlBuilder(sqlDialect), new JobTagResultHandler())
		{
		}

		public JobTagDao(SqlDialect sqlDialect, string schemaConnectionString) : base(new JobTagSqlBuilder(sqlDialect), new JobTagResultHandler(), schemaConnectionString)
		{
		}
		/*add customized code between this region*/
		/*add customized code between this region*/
	}
}
