using System;
using System.Data;
using MetaShare.Common.Core.Daos;
using MetaShare.Common.Core.Daos.Version;
using Sesi.LocalData.Daos.Interfaces.PostJobReport;
using Sesi.LocalData.Entities.PostJobReport;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Daos.PostJobReport
{
	public class BlendConsumptionDao : ObjectVersionDao<BlendConsumption>, IBlendConsumptionDao
	{
		public class BlendConsumptionSqlBuilder : ObjectVersionSqlBuilder
		{
			public BlendConsumptionSqlBuilder(SqlDialect sqlDialect) : base(sqlDialect,"BlendConsumption")
			{
				this.SqlInsertFront = "BlendDescription,JobIntervalId,SlurryTemperature,BlendCategory,BlendReportid,BlendReportSystemId,BlendReportName,BlendReportDescription,PumpedVolume,BulkTemperature,WaterTemperature,BlendName,JobIntervalTypeName,JobEventNumber,";
				this.SqlInsertBack = "@BlendDescription,@JobIntervalId,@SlurryTemperature,@BlendCategory,@BlendReportid,@BlendReportSystemId,@BlendReportName,@BlendReportDescription,@PumpedVolume,@BulkTemperature,@WaterTemperature,@BlendName,@JobIntervalTypeName,@JobEventNumber,";
			}
		}

		public class BlendConsumptionResultHandler : ObjectVersionResultHandler<BlendConsumption>
		{
			public override void GetColumnValues(IDataReader reader, BlendConsumption item)
			{
				base.GetColumnValues(reader, item);
				int ordinalBlendCategory = reader.GetOrdinal("BlendCategory");
				item.BlendCategory = reader.IsDBNull(ordinalBlendCategory) ? null : reader.GetString(ordinalBlendCategory);
				int ordinalBlendReportId = reader.GetOrdinal("BlendReportId");
				int blendReportId= reader.IsDBNull(ordinalBlendReportId) ? 0 :reader.GetInt32(ordinalBlendReportId);
				int ordinalBlendReportSystemId = reader.GetOrdinal("BlendReportSystemId");
				int blendReportSystemId= reader.IsDBNull(ordinalBlendReportSystemId) ? 0 :reader.GetInt32(ordinalBlendReportSystemId);
				int ordinalBlendReportName = reader.GetOrdinal("BlendReportName");
				string blendReportName= reader.IsDBNull(ordinalBlendReportName) ? null :reader.GetString(ordinalBlendReportName);
				int ordinalBlendReportDescription = reader.GetOrdinal("BlendReportDescription");
				string blendReportDescription= reader.IsDBNull(ordinalBlendReportDescription) ? null:reader.GetString(ordinalBlendReportDescription);
				item.BlendReport = new BlendReport { Id=blendReportId,SystemId = blendReportSystemId,Name = blendReportName,Description = blendReportDescription};
				int ordinalSlurryTemperature = reader.GetOrdinal("SlurryTemperature");
				item.SlurryTemperature =  reader.IsDBNull(ordinalSlurryTemperature) ? 0 : reader.GetDouble(ordinalSlurryTemperature);
				int ordinalBulkTemperature = reader.GetOrdinal("BulkTemperature");
				item.BulkTemperature =  reader.IsDBNull(ordinalBulkTemperature) ? 0 : reader.GetDouble(ordinalBulkTemperature);
				int ordinalJobIntervalTypeName = reader.GetOrdinal("JobIntervalTypeName");
				item.JobIntervalTypeName = reader.IsDBNull(ordinalJobIntervalTypeName) ? null : reader.GetString(ordinalJobIntervalTypeName);
				int ordinalJobIntervalId = reader.GetOrdinal("JobIntervalId");
				item.JobIntervalId = reader.IsDBNull(ordinalJobIntervalId) ? 0 : reader.GetInt32(ordinalJobIntervalId);
				int ordinalWaterTemperature = reader.GetOrdinal("WaterTemperature");
				item.WaterTemperature =  reader.IsDBNull(ordinalWaterTemperature) ? 0 : reader.GetDouble(ordinalWaterTemperature);
				int ordinalBlendName = reader.GetOrdinal("BlendName");
				item.BlendName = reader.IsDBNull(ordinalBlendName) ? null : reader.GetString(ordinalBlendName);
				int ordinalJobEventNumber = reader.GetOrdinal("JobEventNumber");
				item.JobEventNumber = reader.IsDBNull(ordinalJobEventNumber) ? 0 : reader.GetInt32(ordinalJobEventNumber);
				int ordinalBlendDescription = reader.GetOrdinal("BlendDescription");
				item.BlendDescription = reader.IsDBNull(ordinalBlendDescription) ? null : reader.GetString(ordinalBlendDescription);
				int ordinalPumpedVolume = reader.GetOrdinal("PumpedVolume");
				item.PumpedVolume =  reader.IsDBNull(ordinalPumpedVolume) ? 0 : reader.GetDouble(ordinalPumpedVolume);
				/*add customized code between this region*/
				/*add customized code between this region*/
			}

			public override void AddInsertParameters(IContext context, IDbCommand command, BlendConsumption item)
			{
				base.AddInsertParameters(context, command, item);
				context.AddParameter(command, "BlendCategory", item.BlendCategory ?? (object) DBNull.Value);
				context.AddParameter(command, "BlendReportId", item.BlendReport ==null? 0:item.BlendReport.Id);

                context.AddParameter(command, "BlendReportSystemId", item.BlendReport ==null? 0:item.BlendReport.SystemId);
                context.AddParameter(command, "BlendReportName", item.BlendReport ==null?(object)DBNull.Value : string.IsNullOrEmpty(item.BlendReport.Name) ? (object)DBNull.Value : item.BlendReport.Name);
                context.AddParameter(command, "BlendReportDescription", item.BlendReport ==null? (object)DBNull.Value : string.IsNullOrEmpty(item.BlendReport.Description) ? (object)DBNull.Value : item.BlendReport.Description);

				context.AddParameter(command, "SlurryTemperature", item.SlurryTemperature);
				context.AddParameter(command, "BulkTemperature", item.BulkTemperature);
				context.AddParameter(command, "JobIntervalTypeName", item.JobIntervalTypeName ?? (object) DBNull.Value);
				context.AddParameter(command, "JobIntervalId", item.JobIntervalId);
				context.AddParameter(command, "WaterTemperature", item.WaterTemperature);
				context.AddParameter(command, "BlendName", item.BlendName ?? (object) DBNull.Value);
				context.AddParameter(command, "JobEventNumber", item.JobEventNumber);
				context.AddParameter(command, "BlendDescription", item.BlendDescription ?? (object) DBNull.Value);
				context.AddParameter(command, "PumpedVolume", item.PumpedVolume);
				/*add customized code between this region*/
				/*add customized code between this region*/
			}
		}

		public BlendConsumptionDao(SqlDialect sqlDialect) : base(new BlendConsumptionSqlBuilder(sqlDialect), new BlendConsumptionResultHandler())
		{
		}

		public BlendConsumptionDao(SqlDialect sqlDialect, string schemaConnectionString) : base(new BlendConsumptionSqlBuilder(sqlDialect), new BlendConsumptionResultHandler(), schemaConnectionString)
		{
		}
		/*add customized code between this region*/
		/*add customized code between this region*/
	}
}
