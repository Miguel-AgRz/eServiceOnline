using System;
using System.Data;
using MetaShare.Common.Core.Daos;
using Sesi.LocalData.Daos.Interfaces.LocalOperation;
using Sesi.LocalData.Entities.LocalOperation;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Daos.LocalOperation
{
	public class ChartDefinitionDao : CommonObjectDao<ChartDefinition>, IChartDefinitionDao
	{
		public class ChartDefinitionSqlBuilder : ObjectSqlBuilder
		{
			public ChartDefinitionSqlBuilder(SqlDialect sqlDialect) : base(sqlDialect,"ChartDefinition")
			{
				this.SqlInsert = "INSERT INTO ChartDefinition (YAxisMax,LabelFormat,SecondaryYAxisUom,SecondaryYAxisMin,YAxisUom,JobMonSettingid,YAxisInterval,IsPrintTogether,SecondaryYAxisInterval,YAxisMin,ExistSecondAxis,Title,SecondaryYAxisMax,IsEnabled," + this.SqlBaseFieldInsertFront + ") VALUES (@YAxisMax,@LabelFormat,@SecondaryYAxisUom,@SecondaryYAxisMin,@YAxisUom,@JobMonSettingid,@YAxisInterval,@IsPrintTogether,@SecondaryYAxisInterval,@YAxisMin,@ExistSecondAxis,@Title,@SecondaryYAxisMax,@IsEnabled," + this.SqlBaseFieldInsertBack + ")";
				this.SqlUpdate = "UPDATE ChartDefinition SET YAxisMax=@YAxisMax,LabelFormat=@LabelFormat,SecondaryYAxisUom=@SecondaryYAxisUom,SecondaryYAxisMin=@SecondaryYAxisMin,YAxisUom=@YAxisUom,JobMonSettingid=@JobMonSettingid,YAxisInterval=@YAxisInterval,IsPrintTogether=@IsPrintTogether,SecondaryYAxisInterval=@SecondaryYAxisInterval,YAxisMin=@YAxisMin,ExistSecondAxis=@ExistSecondAxis,Title=@Title,SecondaryYAxisMax=@SecondaryYAxisMax,IsEnabled=@IsEnabled," + this.SqlBaseFieldUpdate + " WHERE Id=@Id";
			}
		}

		public class ChartDefinitionResultHandler : CommonObjectResultHandler<ChartDefinition>
		{
			public override void GetColumnValues(IDataReader reader, ChartDefinition item)
			{
				base.GetColumnValues(reader, item);
				int ordinalIsPrintTogether = reader.GetOrdinal("IsPrintTogether");
				item.IsPrintTogether = !reader.IsDBNull(ordinalIsPrintTogether) && reader.GetBoolean(ordinalIsPrintTogether);
				int ordinalIsEnabled = reader.GetOrdinal("IsEnabled");
				item.IsEnabled = !reader.IsDBNull(ordinalIsEnabled) && reader.GetBoolean(ordinalIsEnabled);
				int ordinalSecondaryYAxisInterval = reader.GetOrdinal("SecondaryYAxisInterval");
				item.SecondaryYAxisInterval =  reader.IsDBNull(ordinalSecondaryYAxisInterval) ? 0 : reader.GetDouble(ordinalSecondaryYAxisInterval);
				int ordinalYAxisInterval = reader.GetOrdinal("YAxisInterval");
				item.YAxisInterval =  reader.IsDBNull(ordinalYAxisInterval) ? 0 : reader.GetDouble(ordinalYAxisInterval);
				int ordinalJobMonSettingId = reader.GetOrdinal("JobMonSettingId");
				item.JobMonSetting = reader.IsDBNull(ordinalJobMonSettingId) ? null :reader.GetInt32(ordinalJobMonSettingId)==0?null:new JobMonitorSetting { Id=reader.GetInt32(ordinalJobMonSettingId)};
				int ordinalYAxisMin = reader.GetOrdinal("YAxisMin");
				item.YAxisMin =  reader.IsDBNull(ordinalYAxisMin) ? 0 : reader.GetDouble(ordinalYAxisMin);
				int ordinalYAxisUom = reader.GetOrdinal("YAxisUom");
				item.YAxisUom = reader.IsDBNull(ordinalYAxisUom) ? null : reader.GetString(ordinalYAxisUom);
				int ordinalSecondaryYAxisUom = reader.GetOrdinal("SecondaryYAxisUom");
				item.SecondaryYAxisUom = reader.IsDBNull(ordinalSecondaryYAxisUom) ? null : reader.GetString(ordinalSecondaryYAxisUom);
				int ordinalSecondaryYAxisMax = reader.GetOrdinal("SecondaryYAxisMax");
				item.SecondaryYAxisMax =  reader.IsDBNull(ordinalSecondaryYAxisMax) ? 0 : reader.GetDouble(ordinalSecondaryYAxisMax);
				int ordinalSecondaryYAxisMin = reader.GetOrdinal("SecondaryYAxisMin");
				item.SecondaryYAxisMin =  reader.IsDBNull(ordinalSecondaryYAxisMin) ? 0 : reader.GetDouble(ordinalSecondaryYAxisMin);
				int ordinalYAxisMax = reader.GetOrdinal("YAxisMax");
				item.YAxisMax =  reader.IsDBNull(ordinalYAxisMax) ? 0 : reader.GetDouble(ordinalYAxisMax);
				int ordinalTitle = reader.GetOrdinal("Title");
				item.Title = reader.IsDBNull(ordinalTitle) ? null : reader.GetString(ordinalTitle);
				int ordinalExistSecondAxis = reader.GetOrdinal("ExistSecondAxis");
				item.ExistSecondAxis = !reader.IsDBNull(ordinalExistSecondAxis) && reader.GetBoolean(ordinalExistSecondAxis);
				int ordinalLabelFormat = reader.GetOrdinal("LabelFormat");
				item.LabelFormat = reader.IsDBNull(ordinalLabelFormat) ? null : reader.GetString(ordinalLabelFormat);
				/*add customized code between this region*/
				/*add customized code between this region*/
			}

			public override void AddInsertParameters(IContext context, IDbCommand command, ChartDefinition item)
			{
				base.AddInsertParameters(context, command, item);
				context.AddParameter(command, "IsPrintTogether", item.IsPrintTogether);
				context.AddParameter(command, "IsEnabled", item.IsEnabled);
				context.AddParameter(command, "SecondaryYAxisInterval", item.SecondaryYAxisInterval);
				context.AddParameter(command, "YAxisInterval", item.YAxisInterval);
				context.AddParameter(command, "JobMonSettingId", item.JobMonSetting ==null? 0:item.JobMonSetting.Id);

				context.AddParameter(command, "YAxisMin", item.YAxisMin);
				context.AddParameter(command, "YAxisUom", item.YAxisUom ?? (object) DBNull.Value);
				context.AddParameter(command, "SecondaryYAxisUom", item.SecondaryYAxisUom ?? (object) DBNull.Value);
				context.AddParameter(command, "SecondaryYAxisMax", item.SecondaryYAxisMax);
				context.AddParameter(command, "SecondaryYAxisMin", item.SecondaryYAxisMin);
				context.AddParameter(command, "YAxisMax", item.YAxisMax);
				context.AddParameter(command, "Title", item.Title ?? (object) DBNull.Value);
				context.AddParameter(command, "ExistSecondAxis", item.ExistSecondAxis);
				context.AddParameter(command, "LabelFormat", item.LabelFormat ?? (object) DBNull.Value);
				/*add customized code between this region*/
				/*add customized code between this region*/
			}
		}

		public ChartDefinitionDao(SqlDialect sqlDialect) : base(new ChartDefinitionSqlBuilder(sqlDialect), new ChartDefinitionResultHandler())
		{
		}

		public ChartDefinitionDao(SqlDialect sqlDialect, string schemaConnectionString) : base(new ChartDefinitionSqlBuilder(sqlDialect), new ChartDefinitionResultHandler(), schemaConnectionString)
		{
		}
		/*add customized code between this region*/
		/*add customized code between this region*/
	}
}
