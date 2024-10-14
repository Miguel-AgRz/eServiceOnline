using System;
using System.Data;
using MetaShare.Common.Core.Daos;
using Sesi.LocalData.Daos.Interfaces.LocalOperation;
using Sesi.LocalData.Entities.LocalOperation;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Daos.LocalOperation
{
	public class WitsDataDao : CommonObjectDao<WitsData>, IWitsDataDao
	{
		public class WitsDataSqlBuilder : ObjectSqlBuilder
		{
			public WitsDataSqlBuilder(SqlDialect sqlDialect) : base(sqlDialect,"WitsData")
			{
				this.SqlInsert = "INSERT INTO WitsData (VERSION,JSON,TimeStamp," + this.SqlBaseFieldInsertFront + ") VALUES (@VERSION,@JSON,@TimeStamp," + this.SqlBaseFieldInsertBack + ")";
				this.SqlUpdate = "UPDATE WitsData SET VERSION=@VERSION,JSON=@JSON,TimeStamp=@TimeStamp," + this.SqlBaseFieldUpdate + " WHERE Id=@Id";
			}
		}

		public class WitsDataResultHandler : CommonObjectResultHandler<WitsData>
		{
			public override void GetColumnValues(IDataReader reader, WitsData item)
			{
				base.GetColumnValues(reader, item);
				int ordinalVERSION = reader.GetOrdinal("VERSION");
				item.VERSION = reader.IsDBNull(ordinalVERSION) ? 0 : reader.GetInt32(ordinalVERSION);
				int ordinalJSON = reader.GetOrdinal("JSON");
				item.JSON = reader.IsDBNull(ordinalJSON) ? null : reader.GetString(ordinalJSON);
				int ordinalTimeStamp = reader.GetOrdinal("TimeStamp");
				item.TimeStamp = reader.IsDBNull(ordinalTimeStamp) ? null : reader.GetString(ordinalTimeStamp);
				/*add customized code between this region*/
				/*add customized code between this region*/
			}

			public override void AddInsertParameters(IContext context, IDbCommand command, WitsData item)
			{
				base.AddInsertParameters(context, command, item);
				context.AddParameter(command, "VERSION", item.VERSION);
				context.AddParameter(command, "JSON", item.JSON ?? (object) DBNull.Value);
				context.AddParameter(command, "TimeStamp", item.TimeStamp ?? (object) DBNull.Value);
				/*add customized code between this region*/
				/*add customized code between this region*/
			}
		}

		public WitsDataDao(SqlDialect sqlDialect) : base(new WitsDataSqlBuilder(sqlDialect), new WitsDataResultHandler())
		{
		}

		public WitsDataDao(SqlDialect sqlDialect, string schemaConnectionString) : base(new WitsDataSqlBuilder(sqlDialect), new WitsDataResultHandler(), schemaConnectionString)
		{
		}
		/*add customized code between this region*/
		/*add customized code between this region*/
	}
}
