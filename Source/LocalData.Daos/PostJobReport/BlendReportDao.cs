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
	public class BlendReportDao : ObjectVersionDao<BlendReport>, IBlendReportDao
	{
		public class BlendReportSqlBuilder : ObjectVersionSqlBuilder
		{
			public BlendReportSqlBuilder(SqlDialect sqlDialect) : base(sqlDialect,"BlendReport")
			{
				this.SqlInsertFront = "TotalPumpedVolume,ExpectedCementTop,JobUniqueId,";
				this.SqlInsertBack = "@TotalPumpedVolume,@ExpectedCementTop,@JobUniqueId,";
			}
		}

		public class BlendReportResultHandler : ObjectVersionResultHandler<BlendReport>
		{
			public override void GetColumnValues(IDataReader reader, BlendReport item)
			{
				base.GetColumnValues(reader, item);
				int ordinalJobUniqueId = reader.GetOrdinal("JobUniqueId");
				item.JobUniqueId = reader.IsDBNull(ordinalJobUniqueId) ? null : reader.GetString(ordinalJobUniqueId);
				int ordinalTotalPumpedVolume = reader.GetOrdinal("TotalPumpedVolume");
				item.TotalPumpedVolume =  reader.IsDBNull(ordinalTotalPumpedVolume) ? 0 : reader.GetDouble(ordinalTotalPumpedVolume);
				int ordinalExpectedCementTop = reader.GetOrdinal("ExpectedCementTop");
				item.ExpectedCementTop =  reader.IsDBNull(ordinalExpectedCementTop) ? 0 : reader.GetDouble(ordinalExpectedCementTop);
				/*add customized code between this region*/
				/*add customized code between this region*/
			}

			public override void AddInsertParameters(IContext context, IDbCommand command, BlendReport item)
			{
				base.AddInsertParameters(context, command, item);
				context.AddParameter(command, "JobUniqueId", item.JobUniqueId ?? (object) DBNull.Value);
				context.AddParameter(command, "TotalPumpedVolume", item.TotalPumpedVolume);
				context.AddParameter(command, "ExpectedCementTop", item.ExpectedCementTop);
				/*add customized code between this region*/
				/*add customized code between this region*/
			}
		}

		public BlendReportDao(SqlDialect sqlDialect) : base(new BlendReportSqlBuilder(sqlDialect), new BlendReportResultHandler())
		{
		}

		public BlendReportDao(SqlDialect sqlDialect, string schemaConnectionString) : base(new BlendReportSqlBuilder(sqlDialect), new BlendReportResultHandler(), schemaConnectionString)
		{
		}
		/*add customized code between this region*/
		/*add customized code between this region*/
	}
}
