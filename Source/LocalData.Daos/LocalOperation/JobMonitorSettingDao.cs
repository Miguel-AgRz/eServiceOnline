using System;
using System.Data;
using MetaShare.Common.Core.Daos;
using Sesi.LocalData.Daos.Interfaces.LocalOperation;
using Sesi.LocalData.Entities.LocalOperation;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Daos.LocalOperation
{
	public class JobMonitorSettingDao : CommonObjectDao<JobMonitorSetting>, IJobMonitorSettingDao
	{
		public class JobMonitorSettingSqlBuilder : ObjectSqlBuilder
		{
			public JobMonitorSettingSqlBuilder(SqlDialect sqlDialect) : base(sqlDialect,"JobMonitorSetting")
			{
				this.SqlInsert = "INSERT INTO JobMonitorSetting (Frequency,Duration,JobNumber,JobUniqueId,Interval," + this.SqlBaseFieldInsertFront + ") VALUES (@Frequency,@Duration,@JobNumber,@JobUniqueId,@Interval," + this.SqlBaseFieldInsertBack + ")";
				this.SqlUpdate = "UPDATE JobMonitorSetting SET Frequency=@Frequency,Duration=@Duration,JobNumber=@JobNumber,JobUniqueId=@JobUniqueId,Interval=@Interval," + this.SqlBaseFieldUpdate + " WHERE Id=@Id";
			}
		}

		public class JobMonitorSettingResultHandler : CommonObjectResultHandler<JobMonitorSetting>
		{
			public override void GetColumnValues(IDataReader reader, JobMonitorSetting item)
			{
				base.GetColumnValues(reader, item);
				int ordinalFrequency = reader.GetOrdinal("Frequency");
				item.Frequency = reader.IsDBNull(ordinalFrequency) ? 0 : reader.GetInt32(ordinalFrequency);
				int ordinalJobUniqueId = reader.GetOrdinal("JobUniqueId");
				item.JobUniqueId = reader.IsDBNull(ordinalJobUniqueId) ? null : reader.GetString(ordinalJobUniqueId);
				int ordinalDuration = reader.GetOrdinal("Duration");
				item.Duration = reader.IsDBNull(ordinalDuration) ? 0 : reader.GetInt32(ordinalDuration);
				int ordinalJobNumber = reader.GetOrdinal("JobNumber");
				item.JobNumber = reader.IsDBNull(ordinalJobNumber) ? null : reader.GetString(ordinalJobNumber);
				int ordinalInterval = reader.GetOrdinal("Interval");
				item.Interval = reader.IsDBNull(ordinalInterval) ? 0 : reader.GetInt32(ordinalInterval);
				/*add customized code between this region*/
				/*add customized code between this region*/
			}

			public override void AddInsertParameters(IContext context, IDbCommand command, JobMonitorSetting item)
			{
				base.AddInsertParameters(context, command, item);
				context.AddParameter(command, "Frequency", item.Frequency);
				context.AddParameter(command, "JobUniqueId", item.JobUniqueId ?? (object) DBNull.Value);
				context.AddParameter(command, "Duration", item.Duration);
				context.AddParameter(command, "JobNumber", item.JobNumber ?? (object) DBNull.Value);
				context.AddParameter(command, "Interval", item.Interval);
				/*add customized code between this region*/
				/*add customized code between this region*/
			}
		}

		public JobMonitorSettingDao(SqlDialect sqlDialect) : base(new JobMonitorSettingSqlBuilder(sqlDialect), new JobMonitorSettingResultHandler())
		{
		}

		public JobMonitorSettingDao(SqlDialect sqlDialect, string schemaConnectionString) : base(new JobMonitorSettingSqlBuilder(sqlDialect), new JobMonitorSettingResultHandler(), schemaConnectionString)
		{
		}
		/*add customized code between this region*/
		/*add customized code between this region*/
	}
}
