using System;
using System.Data;
using MetaShare.Common.Core.Daos;
using Sesi.LocalData.Daos.Interfaces.LocalOperation;
using Sesi.LocalData.Entities.LocalOperation;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Daos.LocalOperation
{
	public class PrintingSettingDao : CommonObjectDao<PrintingSetting>, IPrintingSettingDao
	{
		public class PrintingSettingSqlBuilder : ObjectSqlBuilder
		{
			public PrintingSettingSqlBuilder(SqlDialect sqlDialect) : base(sqlDialect,"PrintingSetting")
			{
				this.SqlInsert = "INSERT INTO PrintingSetting (EndTime,StartTime,IsDataFromCsv," + this.SqlBaseFieldInsertFront + ") VALUES (@EndTime,@StartTime,@IsDataFromCsv," + this.SqlBaseFieldInsertBack + ")";
				this.SqlUpdate = "UPDATE PrintingSetting SET EndTime=@EndTime,StartTime=@StartTime,IsDataFromCsv=@IsDataFromCsv," + this.SqlBaseFieldUpdate + " WHERE Id=@Id";
			}
		}

		public class PrintingSettingResultHandler : CommonObjectResultHandler<PrintingSetting>
		{
			public override void GetColumnValues(IDataReader reader, PrintingSetting item)
			{
				base.GetColumnValues(reader, item);
				int ordinalIsDataFromCsv = reader.GetOrdinal("IsDataFromCsv");
				item.IsDataFromCsv = !reader.IsDBNull(ordinalIsDataFromCsv) && reader.GetBoolean(ordinalIsDataFromCsv);
				int ordinalEndTime = reader.GetOrdinal("EndTime");
				item.EndTime = reader.IsDBNull(ordinalEndTime) ? DateTime.MinValue : reader.GetDateTime(ordinalEndTime);
				int ordinalStartTime = reader.GetOrdinal("StartTime");
				item.StartTime = reader.IsDBNull(ordinalStartTime) ? DateTime.MinValue : reader.GetDateTime(ordinalStartTime);
				/*add customized code between this region*/
				/*add customized code between this region*/
			}

			public override void AddInsertParameters(IContext context, IDbCommand command, PrintingSetting item)
			{
				base.AddInsertParameters(context, command, item);
				context.AddParameter(command, "IsDataFromCsv", item.IsDataFromCsv);
				context.AddParameter(command, "EndTime", item.EndTime == DateTime.MinValue ? (object)DBNull.Value : item.EndTime);
				context.AddParameter(command, "StartTime", item.StartTime == DateTime.MinValue ? (object)DBNull.Value : item.StartTime);
				/*add customized code between this region*/
				/*add customized code between this region*/
			}
		}

		public PrintingSettingDao(SqlDialect sqlDialect) : base(new PrintingSettingSqlBuilder(sqlDialect), new PrintingSettingResultHandler())
		{
		}

		public PrintingSettingDao(SqlDialect sqlDialect, string schemaConnectionString) : base(new PrintingSettingSqlBuilder(sqlDialect), new PrintingSettingResultHandler(), schemaConnectionString)
		{
		}
		/*add customized code between this region*/
		/*add customized code between this region*/
	}
}
