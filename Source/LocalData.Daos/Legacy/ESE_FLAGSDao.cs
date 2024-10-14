using System;
using System.Data;
using MetaShare.Common.Core.Daos;
using Sesi.LocalData.Daos.Interfaces.Legacy;
using Sesi.LocalData.Entities.Legacy;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Daos.Legacy
{
	public class ESE_FLAGSDao : CommonObjectDao<ESE_FLAGS>, IESE_FLAGSDao
	{
		public class ESE_FLAGSSqlBuilder : ObjectSqlBuilder
		{
			public ESE_FLAGSSqlBuilder(SqlDialect sqlDialect) : base(sqlDialect,"ESE_FLAGS")
			{
				this.SqlInsert = "INSERT INTO ESE_FLAGS (TIMESTAMP,VERSION,VALUE," + this.SqlBaseFieldInsertFront + ") VALUES (@TIMESTAMP,@VERSION,@VALUE," + this.SqlBaseFieldInsertBack + ")";
				this.SqlUpdate = "UPDATE ESE_FLAGS SET TIMESTAMP=@TIMESTAMP,VERSION=@VERSION,VALUE=@VALUE," + this.SqlBaseFieldUpdate + " WHERE Id=@Id";
			}
		}

		public class ESE_FLAGSResultHandler : CommonObjectResultHandler<ESE_FLAGS>
		{
			public override void GetColumnValues(IDataReader reader, ESE_FLAGS item)
			{
				base.GetColumnValues(reader, item);
				int ordinalVALUE = reader.GetOrdinal("VALUE");
				item.VALUE = !reader.IsDBNull(ordinalVALUE) && reader.GetBoolean(ordinalVALUE);
				int ordinalVERSION = reader.GetOrdinal("VERSION");
				item.VERSION = reader.IsDBNull(ordinalVERSION) ? 0 : reader.GetInt32(ordinalVERSION);
				int ordinalTIMESTAMP = reader.GetOrdinal("TIMESTAMP");
				item.TIMESTAMP = reader.IsDBNull(ordinalTIMESTAMP) ? DateTime.MinValue : reader.GetDateTime(ordinalTIMESTAMP);
				/*add customized code between this region*/
				/*add customized code between this region*/
			}

			public override void AddInsertParameters(IContext context, IDbCommand command, ESE_FLAGS item)
			{
				base.AddInsertParameters(context, command, item);
				context.AddParameter(command, "VALUE", item.VALUE);
				context.AddParameter(command, "VERSION", item.VERSION);
				context.AddParameter(command, "TIMESTAMP", item.TIMESTAMP == DateTime.MinValue ? (object)DBNull.Value : item.TIMESTAMP);
				/*add customized code between this region*/
				/*add customized code between this region*/
			}
		}

		public ESE_FLAGSDao(SqlDialect sqlDialect) : base(new ESE_FLAGSSqlBuilder(sqlDialect), new ESE_FLAGSResultHandler())
		{
		}

		public ESE_FLAGSDao(SqlDialect sqlDialect, string schemaConnectionString) : base(new ESE_FLAGSSqlBuilder(sqlDialect), new ESE_FLAGSResultHandler(), schemaConnectionString)
		{
		}
		/*add customized code between this region*/
		/*add customized code between this region*/
	}
}
