using System;
using System.Data;
using MetaShare.Common.Core.Daos;
using Sesi.LocalData.Daos.Interfaces.Legacy;
using Sesi.LocalData.Entities.Legacy;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Daos.Legacy
{
	public class WITS_SETTINGDao : CommonObjectDao<WITS_SETTING>, IWITS_SETTINGDao
	{
		public class WITS_SETTINGSqlBuilder : ObjectSqlBuilder
		{
			public WITS_SETTINGSqlBuilder(SqlDialect sqlDialect) : base(sqlDialect,"WITS_SETTING")
			{
				this.SqlInsert = "INSERT INTO WITS_SETTING (TimeStamp,VERSION,JSON," + this.SqlBaseFieldInsertFront + ") VALUES (@TimeStamp,@VERSION,@JSON," + this.SqlBaseFieldInsertBack + ")";
				this.SqlUpdate = "UPDATE WITS_SETTING SET TimeStamp=@TimeStamp,VERSION=@VERSION,JSON=@JSON," + this.SqlBaseFieldUpdate + " WHERE Id=@Id";
			}
		}

		public class WITS_SETTINGResultHandler : CommonObjectResultHandler<WITS_SETTING>
		{
			public override void GetColumnValues(IDataReader reader, WITS_SETTING item)
			{
				base.GetColumnValues(reader, item);
				int ordinalJSON = reader.GetOrdinal("JSON");
				item.JSON = reader.IsDBNull(ordinalJSON) ? null : reader.GetString(ordinalJSON);
				int ordinalVERSION = reader.GetOrdinal("VERSION");
				item.VERSION = reader.IsDBNull(ordinalVERSION) ? 0 : reader.GetInt32(ordinalVERSION);
				int ordinalTimeStamp = reader.GetOrdinal("TimeStamp");
				item.TimeStamp = reader.IsDBNull(ordinalTimeStamp) ? DateTime.MinValue : reader.GetDateTime(ordinalTimeStamp);
				/*add customized code between this region*/
				/*add customized code between this region*/
			}

			public override void AddInsertParameters(IContext context, IDbCommand command, WITS_SETTING item)
			{
				base.AddInsertParameters(context, command, item);
				context.AddParameter(command, "JSON", item.JSON ?? (object) DBNull.Value);
				context.AddParameter(command, "VERSION", item.VERSION);
				context.AddParameter(command, "TimeStamp", item.TimeStamp == DateTime.MinValue ? (object)DBNull.Value : item.TimeStamp);
				/*add customized code between this region*/
				/*add customized code between this region*/
			}
		}

		public WITS_SETTINGDao(SqlDialect sqlDialect) : base(new WITS_SETTINGSqlBuilder(sqlDialect), new WITS_SETTINGResultHandler())
		{
		}

		public WITS_SETTINGDao(SqlDialect sqlDialect, string schemaConnectionString) : base(new WITS_SETTINGSqlBuilder(sqlDialect), new WITS_SETTINGResultHandler(), schemaConnectionString)
		{
		}
		/*add customized code between this region*/
		/*add customized code between this region*/
	}
}
