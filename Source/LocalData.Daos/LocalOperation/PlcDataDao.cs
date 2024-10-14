using System;
using System.Data;
using MetaShare.Common.Core.Daos;
using Sesi.LocalData.Daos.Interfaces.LocalOperation;
using Sesi.LocalData.Entities.LocalOperation;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Daos.LocalOperation
{
	public class PlcDataDao : CommonObjectDao<PlcData>, IPlcDataDao
	{
		public class PlcDataSqlBuilder : ObjectSqlBuilder
		{
			public PlcDataSqlBuilder(SqlDialect sqlDialect) : base(sqlDialect,"PlcData")
			{
				this.SqlInsert = "INSERT INTO PlcData (JSON,TimeStamp,UnitID," + this.SqlBaseFieldInsertFront + ") VALUES (@JSON,@TimeStamp,@UnitID," + this.SqlBaseFieldInsertBack + ")";
				this.SqlUpdate = "UPDATE PlcData SET JSON=@JSON,TimeStamp=@TimeStamp,UnitID=@UnitID," + this.SqlBaseFieldUpdate + " WHERE Id=@Id";
			}
		}

		public class PlcDataResultHandler : CommonObjectResultHandler<PlcData>
		{
			public override void GetColumnValues(IDataReader reader, PlcData item)
			{
				base.GetColumnValues(reader, item);
				int ordinalTimeStamp = reader.GetOrdinal("TimeStamp");
				item.TimeStamp = reader.IsDBNull(ordinalTimeStamp) ? DateTime.MinValue : reader.GetDateTime(ordinalTimeStamp);
				int ordinalJSON = reader.GetOrdinal("JSON");
				item.JSON = reader.IsDBNull(ordinalJSON) ? null : reader.GetString(ordinalJSON);
				int ordinalUnitID = reader.GetOrdinal("UnitID");
				item.UnitID = reader.IsDBNull(ordinalUnitID) ? null : reader.GetString(ordinalUnitID);
				/*add customized code between this region*/
				/*add customized code between this region*/
			}

			public override void AddInsertParameters(IContext context, IDbCommand command, PlcData item)
			{
				base.AddInsertParameters(context, command, item);
				context.AddParameter(command, "TimeStamp", item.TimeStamp == DateTime.MinValue ? (object)DBNull.Value : item.TimeStamp);
				context.AddParameter(command, "JSON", item.JSON ?? (object) DBNull.Value);
				context.AddParameter(command, "UnitID", item.UnitID ?? (object) DBNull.Value);
				/*add customized code between this region*/
				/*add customized code between this region*/
			}
		}

		public PlcDataDao(SqlDialect sqlDialect) : base(new PlcDataSqlBuilder(sqlDialect), new PlcDataResultHandler())
		{
		}

		public PlcDataDao(SqlDialect sqlDialect, string schemaConnectionString) : base(new PlcDataSqlBuilder(sqlDialect), new PlcDataResultHandler(), schemaConnectionString)
		{
		}
		/*add customized code between this region*/
		/*add customized code between this region*/
	}
}
