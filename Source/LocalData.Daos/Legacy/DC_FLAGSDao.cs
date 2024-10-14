using System;
using System.Data;
using MetaShare.Common.Core.Daos;
using Sesi.LocalData.Daos.Interfaces.Legacy;
using Sesi.LocalData.Entities.Legacy;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Daos.Legacy
{
	public class DC_FLAGSDao : CommonObjectDao<DC_FLAGS>, IDC_FLAGSDao
	{
		public class DC_FLAGSSqlBuilder : ObjectSqlBuilder
		{
			public DC_FLAGSSqlBuilder(SqlDialect sqlDialect) : base(sqlDialect,"DC_FLAGS")
			{
				this.SqlInsert = "INSERT INTO DC_FLAGS (TimeStamp,VERSION,Value," + this.SqlBaseFieldInsertFront + ") VALUES (@TimeStamp,@VERSION,@Value," + this.SqlBaseFieldInsertBack + ")";
				this.SqlUpdate = "UPDATE DC_FLAGS SET TimeStamp=@TimeStamp,VERSION=@VERSION,Value=@Value," + this.SqlBaseFieldUpdate + " WHERE Id=@Id";
			}
		}

		public class DC_FLAGSResultHandler : CommonObjectResultHandler<DC_FLAGS>
		{
			public override void GetColumnValues(IDataReader reader, DC_FLAGS item)
			{
				base.GetColumnValues(reader, item);
				int ordinalVERSION = reader.GetOrdinal("VERSION");
				item.VERSION = reader.IsDBNull(ordinalVERSION) ? 0 : reader.GetInt32(ordinalVERSION);
				int ordinalValue = reader.GetOrdinal("Value");
				item.Value = !reader.IsDBNull(ordinalValue) && reader.GetBoolean(ordinalValue);
				int ordinalTimeStamp = reader.GetOrdinal("TimeStamp");
				item.TimeStamp = reader.IsDBNull(ordinalTimeStamp) ? DateTime.MinValue : reader.GetDateTime(ordinalTimeStamp);
				/*add customized code between this region*/
				/*add customized code between this region*/
			}

			public override void AddInsertParameters(IContext context, IDbCommand command, DC_FLAGS item)
			{
				base.AddInsertParameters(context, command, item);
				context.AddParameter(command, "VERSION", item.VERSION);
				context.AddParameter(command, "Value", item.Value);
				context.AddParameter(command, "TimeStamp", item.TimeStamp == DateTime.MinValue ? (object)DBNull.Value : item.TimeStamp);
				/*add customized code between this region*/
				/*add customized code between this region*/
			}
		}

		public DC_FLAGSDao(SqlDialect sqlDialect) : base(new DC_FLAGSSqlBuilder(sqlDialect), new DC_FLAGSResultHandler())
		{
		}

		public DC_FLAGSDao(SqlDialect sqlDialect, string schemaConnectionString) : base(new DC_FLAGSSqlBuilder(sqlDialect), new DC_FLAGSResultHandler(), schemaConnectionString)
		{
		}
		/*add customized code between this region*/
		/*add customized code between this region*/
	}
}
