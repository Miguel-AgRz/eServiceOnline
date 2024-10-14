using System;
using System.Data;
using MetaShare.Common.Core.Daos;
using Sesi.LocalData.Daos.Interfaces.LocalOperation;
using Sesi.LocalData.Entities.LocalOperation;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Daos.LocalOperation
{
	public class EseFlagDao : CommonObjectDao<EseFlag>, IEseFlagDao
	{
		public class EseFlagSqlBuilder : ObjectSqlBuilder
		{
			public EseFlagSqlBuilder(SqlDialect sqlDialect) : base(sqlDialect,"EseFlag")
			{
				this.SqlInsert = "INSERT INTO EseFlag (Version,Value,TimeStamp," + this.SqlBaseFieldInsertFront + ") VALUES (@Version,@Value,@TimeStamp," + this.SqlBaseFieldInsertBack + ")";
				this.SqlUpdate = "UPDATE EseFlag SET Version=@Version,Value=@Value,TimeStamp=@TimeStamp," + this.SqlBaseFieldUpdate + " WHERE Id=@Id";
			}
		}

		public class EseFlagResultHandler : CommonObjectResultHandler<EseFlag>
		{
			public override void GetColumnValues(IDataReader reader, EseFlag item)
			{
				base.GetColumnValues(reader, item);
				int ordinalValue = reader.GetOrdinal("Value");
				item.Value = !reader.IsDBNull(ordinalValue) && reader.GetBoolean(ordinalValue);
				int ordinalVersion = reader.GetOrdinal("Version");
				item.Version = reader.IsDBNull(ordinalVersion) ? 0 : reader.GetInt32(ordinalVersion);
				int ordinalTimeStamp = reader.GetOrdinal("TimeStamp");
				item.TimeStamp = reader.IsDBNull(ordinalTimeStamp) ? DateTime.MinValue : reader.GetDateTime(ordinalTimeStamp);
				/*add customized code between this region*/
				/*add customized code between this region*/
			}

			public override void AddInsertParameters(IContext context, IDbCommand command, EseFlag item)
			{
				base.AddInsertParameters(context, command, item);
				context.AddParameter(command, "Value", item.Value);
				context.AddParameter(command, "Version", item.Version);
				context.AddParameter(command, "TimeStamp", item.TimeStamp == DateTime.MinValue ? (object)DBNull.Value : item.TimeStamp);
				/*add customized code between this region*/
				/*add customized code between this region*/
			}
		}

		public EseFlagDao(SqlDialect sqlDialect) : base(new EseFlagSqlBuilder(sqlDialect), new EseFlagResultHandler())
		{
		}

		public EseFlagDao(SqlDialect sqlDialect, string schemaConnectionString) : base(new EseFlagSqlBuilder(sqlDialect), new EseFlagResultHandler(), schemaConnectionString)
		{
		}
		/*add customized code between this region*/
		/*add customized code between this region*/
	}
}
