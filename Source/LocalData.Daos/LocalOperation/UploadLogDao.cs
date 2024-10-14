using System;
using System.Data;
using MetaShare.Common.Core.Daos;
using Sesi.LocalData.Daos.Interfaces.LocalOperation;
using Sesi.LocalData.Entities.LocalOperation;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Daos.LocalOperation
{
	public class UploadLogDao : CommonObjectDao<UploadLog>, IUploadLogDao
	{
		public class UploadLogSqlBuilder : ObjectSqlBuilder
		{
			public UploadLogSqlBuilder(SqlDialect sqlDialect) : base(sqlDialect,"UploadLog")
			{
				this.SqlInsert = "INSERT INTO UploadLog (JobUniqueId,IsRecievedOnServer,IsPlcDataDeleted,Version,PackingEndDateTime,PackSize,TimeZone,ComputerName,EndTime,JobNumber,StartTime,PackingDuration," + this.SqlBaseFieldInsertFront + ") VALUES (@JobUniqueId,@IsRecievedOnServer,@IsPlcDataDeleted,@Version,@PackingEndDateTime,@PackSize,@TimeZone,@ComputerName,@EndTime,@JobNumber,@StartTime,@PackingDuration," + this.SqlBaseFieldInsertBack + ")";
				this.SqlUpdate = "UPDATE UploadLog SET JobUniqueId=@JobUniqueId,IsRecievedOnServer=@IsRecievedOnServer,IsPlcDataDeleted=@IsPlcDataDeleted,Version=@Version,PackingEndDateTime=@PackingEndDateTime,PackSize=@PackSize,TimeZone=@TimeZone,ComputerName=@ComputerName,EndTime=@EndTime,JobNumber=@JobNumber,StartTime=@StartTime,PackingDuration=@PackingDuration," + this.SqlBaseFieldUpdate + " WHERE Id=@Id";
			}
		}

		public class UploadLogResultHandler : CommonObjectResultHandler<UploadLog>
		{
			public override void GetColumnValues(IDataReader reader, UploadLog item)
			{
				base.GetColumnValues(reader, item);
				int ordinalVersion = reader.GetOrdinal("Version");
				item.Version = reader.IsDBNull(ordinalVersion) ? 0 : reader.GetInt32(ordinalVersion);
				int ordinalStartTime = reader.GetOrdinal("StartTime");
				item.StartTime = reader.IsDBNull(ordinalStartTime) ? DateTime.MinValue : reader.GetDateTime(ordinalStartTime);
				int ordinalJobUniqueId = reader.GetOrdinal("JobUniqueId");
				item.JobUniqueId = reader.IsDBNull(ordinalJobUniqueId) ? null : reader.GetString(ordinalJobUniqueId);
				int ordinalJobNumber = reader.GetOrdinal("JobNumber");
				item.JobNumber = reader.IsDBNull(ordinalJobNumber) ? null : reader.GetString(ordinalJobNumber);
				int ordinalPackingEndDateTime = reader.GetOrdinal("PackingEndDateTime");
				item.PackingEndDateTime = reader.IsDBNull(ordinalPackingEndDateTime) ? DateTime.MinValue : reader.GetDateTime(ordinalPackingEndDateTime);
				int ordinalIsRecievedOnServer = reader.GetOrdinal("IsRecievedOnServer");
				item.IsRecievedOnServer = !reader.IsDBNull(ordinalIsRecievedOnServer) && reader.GetBoolean(ordinalIsRecievedOnServer);
				int ordinalIsPlcDataDeleted = reader.GetOrdinal("IsPlcDataDeleted");
				item.IsPlcDataDeleted = !reader.IsDBNull(ordinalIsPlcDataDeleted) && reader.GetBoolean(ordinalIsPlcDataDeleted);
				int ordinalTimeZone = reader.GetOrdinal("TimeZone");
				item.TimeZone = reader.IsDBNull(ordinalTimeZone) ? null : reader.GetString(ordinalTimeZone);
				int ordinalComputerName = reader.GetOrdinal("ComputerName");
				item.ComputerName = reader.IsDBNull(ordinalComputerName) ? null : reader.GetString(ordinalComputerName);
				int ordinalEndTime = reader.GetOrdinal("EndTime");
				item.EndTime = reader.IsDBNull(ordinalEndTime) ? DateTime.MinValue : reader.GetDateTime(ordinalEndTime);
				int ordinalPackingDuration = reader.GetOrdinal("PackingDuration");
				item.PackingDuration = reader.IsDBNull(ordinalPackingDuration) ? null : reader.GetString(ordinalPackingDuration);
				int ordinalPackSize = reader.GetOrdinal("PackSize");
				item.PackSize = reader.IsDBNull(ordinalPackSize) ? 0 : reader.GetInt32(ordinalPackSize);
				/*add customized code between this region*/
				/*add customized code between this region*/
			}

			public override void AddInsertParameters(IContext context, IDbCommand command, UploadLog item)
			{
				base.AddInsertParameters(context, command, item);
				context.AddParameter(command, "Version", item.Version);
				context.AddParameter(command, "StartTime", item.StartTime == DateTime.MinValue ? (object)DBNull.Value : item.StartTime);
				context.AddParameter(command, "JobUniqueId", item.JobUniqueId ?? (object) DBNull.Value);
				context.AddParameter(command, "JobNumber", item.JobNumber ?? (object) DBNull.Value);
				context.AddParameter(command, "PackingEndDateTime", item.PackingEndDateTime == DateTime.MinValue ? (object)DBNull.Value : item.PackingEndDateTime);
				context.AddParameter(command, "IsRecievedOnServer", item.IsRecievedOnServer);
				context.AddParameter(command, "IsPlcDataDeleted", item.IsPlcDataDeleted);
				context.AddParameter(command, "TimeZone", item.TimeZone ?? (object) DBNull.Value);
				context.AddParameter(command, "ComputerName", item.ComputerName ?? (object) DBNull.Value);
				context.AddParameter(command, "EndTime", item.EndTime == DateTime.MinValue ? (object)DBNull.Value : item.EndTime);
				context.AddParameter(command, "PackingDuration", item.PackingDuration ?? (object) DBNull.Value);
				context.AddParameter(command, "PackSize", item.PackSize);
				/*add customized code between this region*/
				/*add customized code between this region*/
			}
		}

		public UploadLogDao(SqlDialect sqlDialect) : base(new UploadLogSqlBuilder(sqlDialect), new UploadLogResultHandler())
		{
		}

		public UploadLogDao(SqlDialect sqlDialect, string schemaConnectionString) : base(new UploadLogSqlBuilder(sqlDialect), new UploadLogResultHandler(), schemaConnectionString)
		{
		}
		/*add customized code between this region*/
		/*add customized code between this region*/
	}
}
