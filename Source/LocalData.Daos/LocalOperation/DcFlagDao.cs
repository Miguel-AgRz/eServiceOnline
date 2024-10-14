using System;
using System.Data;
using MetaShare.Common.Core.Daos;
using Sesi.LocalData.Daos.Interfaces.LocalOperation;
using Sesi.LocalData.Entities.LocalOperation;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Daos.LocalOperation
{
	public class DcFlagDao : CommonObjectDao<DcFlag>, IDcFlagDao
	{
		public class DcFlagSqlBuilder : ObjectSqlBuilder
		{
			public DcFlagSqlBuilder(SqlDialect sqlDialect) : base(sqlDialect,"DcFlag")
			{
				this.SqlInsert = "INSERT INTO DcFlag (TimeStamp,Value,Version," + this.SqlBaseFieldInsertFront + ") VALUES (@TimeStamp,@Value,@Version," + this.SqlBaseFieldInsertBack + ")";
				this.SqlUpdate = "UPDATE DcFlag SET TimeStamp=@TimeStamp,Value=@Value,Version=@Version," + this.SqlBaseFieldUpdate + " WHERE Id=@Id";
			}
		}

		public class DcFlagResultHandler : CommonObjectResultHandler<DcFlag>
		{
			public override void GetColumnValues(IDataReader reader, DcFlag item)
			{
				base.GetColumnValues(reader, item);
				int ordinalTimeStamp = reader.GetOrdinal("TimeStamp");
				item.TimeStamp = reader.IsDBNull(ordinalTimeStamp) ? DateTime.MinValue : reader.GetDateTime(ordinalTimeStamp);
				int ordinalValue = reader.GetOrdinal("Value");
				item.Value = !reader.IsDBNull(ordinalValue) && reader.GetBoolean(ordinalValue);
				int ordinalVersion = reader.GetOrdinal("Version");
				item.Version = reader.IsDBNull(ordinalVersion) ? 0 : reader.GetInt32(ordinalVersion);
				/*add customized code between this region*/
				/*add customized code between this region*/
			}

			public override void AddInsertParameters(IContext context, IDbCommand command, DcFlag item)
			{
				base.AddInsertParameters(context, command, item);
				context.AddParameter(command, "TimeStamp", item.TimeStamp == DateTime.MinValue ? (object)DBNull.Value : item.TimeStamp);
				context.AddParameter(command, "Value", item.Value);
				context.AddParameter(command, "Version", item.Version);
				/*add customized code between this region*/
				/*add customized code between this region*/
			}
		}

		public DcFlagDao(SqlDialect sqlDialect) : base(new DcFlagSqlBuilder(sqlDialect), new DcFlagResultHandler())
		{
		}

		public DcFlagDao(SqlDialect sqlDialect, string schemaConnectionString) : base(new DcFlagSqlBuilder(sqlDialect), new DcFlagResultHandler(), schemaConnectionString)
		{
		}
		/*add customized code between this region*/
		/*add customized code between this region*/
	}
}
