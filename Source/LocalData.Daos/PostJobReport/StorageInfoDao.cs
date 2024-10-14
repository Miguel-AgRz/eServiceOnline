using System;
using System.Data;
using MetaShare.Common.Core.Daos;
using MetaShare.Common.Core.Daos.Version;
using Sesi.LocalData.Daos.Interfaces.PostJobReport;
using Sesi.LocalData.Entities.PostJobReport;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Daos.PostJobReport
{
	public class StorageInfoDao : ObjectVersionDao<StorageInfo>, IStorageInfoDao
	{
		public class StorageInfoSqlBuilder : ObjectVersionSqlBuilder
		{
			public StorageInfoSqlBuilder(SqlDialect sqlDialect) : base(sqlDialect,"StorageInfo")
			{
				this.SqlInsertFront = "IsReadyForNext,BlendName,ScaleReading,JobUniqueId,JobNumber,PumpedWithAdds,PumpedWoAdds,StorageType,Remains,InitialTonnage,";
				this.SqlInsertBack = "@IsReadyForNext,@BlendName,@ScaleReading,@JobUniqueId,@JobNumber,@PumpedWithAdds,@PumpedWoAdds,@StorageType,@Remains,@InitialTonnage,";
			}
		}

		public class StorageInfoResultHandler : ObjectVersionResultHandler<StorageInfo>
		{
			public override void GetColumnValues(IDataReader reader, StorageInfo item)
			{
				base.GetColumnValues(reader, item);
				int ordinalRemains = reader.GetOrdinal("Remains");
				item.Remains =  reader.IsDBNull(ordinalRemains) ? 0 : reader.GetDouble(ordinalRemains);
				int ordinalJobNumber = reader.GetOrdinal("JobNumber");
				item.JobNumber = reader.IsDBNull(ordinalJobNumber) ? null : reader.GetString(ordinalJobNumber);
				int ordinalScaleReading = reader.GetOrdinal("ScaleReading");
				item.ScaleReading =  reader.IsDBNull(ordinalScaleReading) ? 0 : reader.GetDouble(ordinalScaleReading);
				int ordinalPumpedWithAdds = reader.GetOrdinal("PumpedWithAdds");
				item.PumpedWithAdds =  reader.IsDBNull(ordinalPumpedWithAdds) ? 0 : reader.GetDouble(ordinalPumpedWithAdds);
				int ordinalJobUniqueId = reader.GetOrdinal("JobUniqueId");
				item.JobUniqueId = reader.IsDBNull(ordinalJobUniqueId) ? null : reader.GetString(ordinalJobUniqueId);
				int ordinalIsReadyForNext = reader.GetOrdinal("IsReadyForNext");
				item.IsReadyForNext = !reader.IsDBNull(ordinalIsReadyForNext) && reader.GetBoolean(ordinalIsReadyForNext);
				int ordinalPumpedWoAdds = reader.GetOrdinal("PumpedWoAdds");
				item.PumpedWoAdds =  reader.IsDBNull(ordinalPumpedWoAdds) ? 0 : reader.GetDouble(ordinalPumpedWoAdds);
				int ordinalInitialTonnage = reader.GetOrdinal("InitialTonnage");
				item.InitialTonnage =  reader.IsDBNull(ordinalInitialTonnage) ? 0 : reader.GetDouble(ordinalInitialTonnage);
				int ordinalStorageType = reader.GetOrdinal("StorageType");
				item.StorageType = reader.IsDBNull(ordinalStorageType) ? null : reader.GetString(ordinalStorageType);
				int ordinalBlendName = reader.GetOrdinal("BlendName");
				item.BlendName = reader.IsDBNull(ordinalBlendName) ? null : reader.GetString(ordinalBlendName);
				/*add customized code between this region*/
				/*add customized code between this region*/
			}

			public override void AddInsertParameters(IContext context, IDbCommand command, StorageInfo item)
			{
				base.AddInsertParameters(context, command, item);
				context.AddParameter(command, "Remains", item.Remains);
				context.AddParameter(command, "JobNumber", item.JobNumber ?? (object) DBNull.Value);
				context.AddParameter(command, "ScaleReading", item.ScaleReading);
				context.AddParameter(command, "PumpedWithAdds", item.PumpedWithAdds);
				context.AddParameter(command, "JobUniqueId", item.JobUniqueId ?? (object) DBNull.Value);
				context.AddParameter(command, "IsReadyForNext", item.IsReadyForNext);
				context.AddParameter(command, "PumpedWoAdds", item.PumpedWoAdds);
				context.AddParameter(command, "InitialTonnage", item.InitialTonnage);
				context.AddParameter(command, "StorageType", item.StorageType ?? (object) DBNull.Value);
				context.AddParameter(command, "BlendName", item.BlendName ?? (object) DBNull.Value);
				/*add customized code between this region*/
				/*add customized code between this region*/
			}
		}

		public StorageInfoDao(SqlDialect sqlDialect) : base(new StorageInfoSqlBuilder(sqlDialect), new StorageInfoResultHandler())
		{
		}

		public StorageInfoDao(SqlDialect sqlDialect, string schemaConnectionString) : base(new StorageInfoSqlBuilder(sqlDialect), new StorageInfoResultHandler(), schemaConnectionString)
		{
		}
		/*add customized code between this region*/
		/*add customized code between this region*/
	}
}
