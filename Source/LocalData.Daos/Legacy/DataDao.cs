using System;
using System.Data;
using MetaShare.Common.Core.Daos;
using Sesi.LocalData.Daos.Interfaces.Legacy;
using Sesi.LocalData.Entities.Legacy;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Daos.Legacy
{
	public class DataDao : CommonObjectDao<Data>, IDataDao
	{
		public class DataSqlBuilder : ObjectSqlBuilder
		{
			public DataSqlBuilder(SqlDialect sqlDialect) : base(sqlDialect,"Data")
			{
				this.SqlInsert = "INSERT INTO Data (JSON,TimeStamp,UnitID," + this.SqlBaseFieldInsertFront + ") VALUES (@JSON,@TimeStamp,@UnitID," + this.SqlBaseFieldInsertBack + ")";
				this.SqlUpdate = "UPDATE Data SET JSON=@JSON,TimeStamp=@TimeStamp,UnitID=@UnitID," + this.SqlBaseFieldUpdate + " WHERE Id=@Id";
			}
		}

		public class DataResultHandler : CommonObjectResultHandler<Data>
		{
			public override void GetColumnValues(IDataReader reader, Data item)
			{
				base.GetColumnValues(reader, item);
				int ordinalJSON = reader.GetOrdinal("JSON");
				item.JSON = reader.IsDBNull(ordinalJSON) ? null : reader.GetString(ordinalJSON);
				int ordinalUnitID = reader.GetOrdinal("UnitID");
				item.UnitID = reader.IsDBNull(ordinalUnitID) ? null : reader.GetString(ordinalUnitID);
				int ordinalTimeStamp = reader.GetOrdinal("TimeStamp");
				item.TimeStamp = reader.IsDBNull(ordinalTimeStamp) ? DateTime.MinValue : reader.GetDateTime(ordinalTimeStamp);
				/*add customized code between this region*/
				/*add customized code between this region*/
			}

			public override void AddInsertParameters(IContext context, IDbCommand command, Data item)
			{
				base.AddInsertParameters(context, command, item);
				context.AddParameter(command, "JSON", item.JSON ?? (object) DBNull.Value);
				context.AddParameter(command, "UnitID", item.UnitID ?? (object) DBNull.Value);
				context.AddParameter(command, "TimeStamp", item.TimeStamp == DateTime.MinValue ? (object)DBNull.Value : item.TimeStamp);
				/*add customized code between this region*/
				/*add customized code between this region*/
			}
		}

		public DataDao(SqlDialect sqlDialect) : base(new DataSqlBuilder(sqlDialect), new DataResultHandler())
		{
		}

		public DataDao(SqlDialect sqlDialect, string schemaConnectionString) : base(new DataSqlBuilder(sqlDialect), new DataResultHandler(), schemaConnectionString)
		{
		}
		/*add customized code between this region*/
		/*add customized code between this region*/
	}
}
