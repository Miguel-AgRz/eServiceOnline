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
	public class MaintenanceNoteDao : ObjectVersionDao<MaintenanceNote>, IMaintenanceNoteDao
	{
		public class MaintenanceNoteSqlBuilder : ObjectVersionSqlBuilder
		{
			public MaintenanceNoteSqlBuilder(SqlDialect sqlDialect) : base(sqlDialect,"MaintenanceNote")
			{
				this.SqlInsertFront = "UnitNumber,JobNumber,UnitType,JobUniqueId,Notes,";
				this.SqlInsertBack = "@UnitNumber,@JobNumber,@UnitType,@JobUniqueId,@Notes,";
			}
		}

		public class MaintenanceNoteResultHandler : ObjectVersionResultHandler<MaintenanceNote>
		{
			public override void GetColumnValues(IDataReader reader, MaintenanceNote item)
			{
				base.GetColumnValues(reader, item);
				int ordinalUnitNumber = reader.GetOrdinal("UnitNumber");
				item.UnitNumber = reader.IsDBNull(ordinalUnitNumber) ? null : reader.GetString(ordinalUnitNumber);
				int ordinalJobUniqueId = reader.GetOrdinal("JobUniqueId");
				item.JobUniqueId = reader.IsDBNull(ordinalJobUniqueId) ? null : reader.GetString(ordinalJobUniqueId);
				int ordinalJobNumber = reader.GetOrdinal("JobNumber");
				item.JobNumber = reader.IsDBNull(ordinalJobNumber) ? null : reader.GetString(ordinalJobNumber);
				int ordinalUnitType = reader.GetOrdinal("UnitType");
				item.UnitType = reader.IsDBNull(ordinalUnitType) ? null : reader.GetString(ordinalUnitType);
				int ordinalNotes = reader.GetOrdinal("Notes");
				item.Notes = reader.IsDBNull(ordinalNotes) ? null : reader.GetString(ordinalNotes);
				/*add customized code between this region*/
				/*add customized code between this region*/
			}

			public override void AddInsertParameters(IContext context, IDbCommand command, MaintenanceNote item)
			{
				base.AddInsertParameters(context, command, item);
				context.AddParameter(command, "UnitNumber", item.UnitNumber ?? (object) DBNull.Value);
				context.AddParameter(command, "JobUniqueId", item.JobUniqueId ?? (object) DBNull.Value);
				context.AddParameter(command, "JobNumber", item.JobNumber ?? (object) DBNull.Value);
				context.AddParameter(command, "UnitType", item.UnitType ?? (object) DBNull.Value);
				context.AddParameter(command, "Notes", item.Notes ?? (object) DBNull.Value);
				/*add customized code between this region*/
				/*add customized code between this region*/
			}
		}

		public MaintenanceNoteDao(SqlDialect sqlDialect) : base(new MaintenanceNoteSqlBuilder(sqlDialect), new MaintenanceNoteResultHandler())
		{
		}

		public MaintenanceNoteDao(SqlDialect sqlDialect, string schemaConnectionString) : base(new MaintenanceNoteSqlBuilder(sqlDialect), new MaintenanceNoteResultHandler(), schemaConnectionString)
		{
		}
		/*add customized code between this region*/
		/*add customized code between this region*/
	}
}
