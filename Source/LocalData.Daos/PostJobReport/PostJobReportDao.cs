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
	public class PostJobReportDao : ObjectVersionDao<Sesi.LocalData.Entities.PostJobReport.PostJobReport>, IPostJobReportDao
	{
		public class PostJobReportSqlBuilder : ObjectVersionSqlBuilder
		{
			public PostJobReportSqlBuilder(SqlDialect sqlDialect) : base(sqlDialect,"PostJobReport")
			{
				this.SqlInsertFront = "JobUniqueId,CallSheetNumber,DownHoleLocation,RevisedDirection,JobDate,ClientName,JobType,IsDirectionRevised,RigName,SurfaceLocation,JobNumber,AdditionalInformation,";
				this.SqlInsertBack = "@JobUniqueId,@CallSheetNumber,@DownHoleLocation,@RevisedDirection,@JobDate,@ClientName,@JobType,@IsDirectionRevised,@RigName,@SurfaceLocation,@JobNumber,@AdditionalInformation,";
			}
		}

		public class PostJobReportResultHandler : ObjectVersionResultHandler<Sesi.LocalData.Entities.PostJobReport.PostJobReport>
		{
			public override void GetColumnValues(IDataReader reader, Sesi.LocalData.Entities.PostJobReport.PostJobReport item)
			{
				base.GetColumnValues(reader, item);
				int ordinalSurfaceLocation = reader.GetOrdinal("SurfaceLocation");
				item.SurfaceLocation = reader.IsDBNull(ordinalSurfaceLocation) ? null : reader.GetString(ordinalSurfaceLocation);
				int ordinalCallSheetNumber = reader.GetOrdinal("CallSheetNumber");
				item.CallSheetNumber = reader.IsDBNull(ordinalCallSheetNumber) ? null : reader.GetString(ordinalCallSheetNumber);
				int ordinalRevisedDirection = reader.GetOrdinal("RevisedDirection");
				item.RevisedDirection = reader.IsDBNull(ordinalRevisedDirection) ? null : reader.GetString(ordinalRevisedDirection);
				int ordinalJobUniqueId = reader.GetOrdinal("JobUniqueId");
				item.JobUniqueId = reader.IsDBNull(ordinalJobUniqueId) ? null : reader.GetString(ordinalJobUniqueId);
				int ordinalAdditionalInformation = reader.GetOrdinal("AdditionalInformation");
				item.AdditionalInformation = reader.IsDBNull(ordinalAdditionalInformation) ? null : reader.GetString(ordinalAdditionalInformation);
				int ordinalClientName = reader.GetOrdinal("ClientName");
				item.ClientName = reader.IsDBNull(ordinalClientName) ? null : reader.GetString(ordinalClientName);
				int ordinalJobNumber = reader.GetOrdinal("JobNumber");
				item.JobNumber = reader.IsDBNull(ordinalJobNumber) ? null : reader.GetString(ordinalJobNumber);
				int ordinalIsDirectionRevised = reader.GetOrdinal("IsDirectionRevised");
				item.IsDirectionRevised = !reader.IsDBNull(ordinalIsDirectionRevised) && reader.GetBoolean(ordinalIsDirectionRevised);
				int ordinalRigName = reader.GetOrdinal("RigName");
				item.RigName = reader.IsDBNull(ordinalRigName) ? null : reader.GetString(ordinalRigName);
				int ordinalJobType = reader.GetOrdinal("JobType");
				item.JobType = reader.IsDBNull(ordinalJobType) ? null : reader.GetString(ordinalJobType);
				int ordinalDownHoleLocation = reader.GetOrdinal("DownHoleLocation");
				item.DownHoleLocation = reader.IsDBNull(ordinalDownHoleLocation) ? null : reader.GetString(ordinalDownHoleLocation);
				int ordinalJobDate = reader.GetOrdinal("JobDate");
				item.JobDate = reader.IsDBNull(ordinalJobDate) ? DateTime.MinValue : reader.GetDateTime(ordinalJobDate);
				/*add customized code between this region*/
				/*add customized code between this region*/
			}

			public override void AddInsertParameters(IContext context, IDbCommand command, Sesi.LocalData.Entities.PostJobReport.PostJobReport item)
			{
				base.AddInsertParameters(context, command, item);
				context.AddParameter(command, "SurfaceLocation", item.SurfaceLocation ?? (object) DBNull.Value);
				context.AddParameter(command, "CallSheetNumber", item.CallSheetNumber ?? (object) DBNull.Value);
				context.AddParameter(command, "RevisedDirection", item.RevisedDirection ?? (object) DBNull.Value);
				context.AddParameter(command, "JobUniqueId", item.JobUniqueId ?? (object) DBNull.Value);
				context.AddParameter(command, "AdditionalInformation", item.AdditionalInformation ?? (object) DBNull.Value);
				context.AddParameter(command, "ClientName", item.ClientName ?? (object) DBNull.Value);
				context.AddParameter(command, "JobNumber", item.JobNumber ?? (object) DBNull.Value);
				context.AddParameter(command, "IsDirectionRevised", item.IsDirectionRevised);
				context.AddParameter(command, "RigName", item.RigName ?? (object) DBNull.Value);
				context.AddParameter(command, "JobType", item.JobType ?? (object) DBNull.Value);
				context.AddParameter(command, "DownHoleLocation", item.DownHoleLocation ?? (object) DBNull.Value);
				context.AddParameter(command, "JobDate", item.JobDate == DateTime.MinValue ? (object)DBNull.Value : item.JobDate);
				/*add customized code between this region*/
				/*add customized code between this region*/
			}
		}

		public PostJobReportDao(SqlDialect sqlDialect) : base(new PostJobReportSqlBuilder(sqlDialect), new PostJobReportResultHandler())
		{
		}

		public PostJobReportDao(SqlDialect sqlDialect, string schemaConnectionString) : base(new PostJobReportSqlBuilder(sqlDialect), new PostJobReportResultHandler(), schemaConnectionString)
		{
		}
		/*add customized code between this region*/
		/*add customized code between this region*/
	}
}
