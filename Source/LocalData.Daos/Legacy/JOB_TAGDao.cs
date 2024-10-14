using System;
using System.Data;
using MetaShare.Common.Core.Daos;
using Sesi.LocalData.Daos.Interfaces.Legacy;
using Sesi.LocalData.Entities.Legacy;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Daos.Legacy
{
	public class JOB_TAGDao : CommonObjectDao<JOB_TAG>, IJOB_TAGDao
	{
		public class JOB_TAGSqlBuilder : ObjectSqlBuilder
		{
			public JOB_TAGSqlBuilder(SqlDialect sqlDialect) : base(sqlDialect,"JOB_TAG")
			{
				this.SqlInsert = "INSERT INTO JOB_TAG (APPLICATION_VERSION,IS_CURRENT_JOB,TIMEZONE,CLIENT_COMPANY,VERSION,TIME_AREA,RIG_NAME,JOB_MONITOR_SETTING,JOB_UNIQUE_ID,WELL_NAME,SERVICE_POINT,JOB_DATE_TIME,SUPERVISOR,COMPUTER_NAME,IS_DATA_FROM_CSV,UNIT_SELECTION,JOB_PRINT_SETTING,HAS_DATA_FROM_CSV,SURFACE_LOCATION,JOB_NUMBER,WITS_SETTING,JOB_TYPE,DOWNHOLE_LOCATION,STATUS,CLIENT_REP,IS_DST_OFF,COMMENTS,JOB_START_TIME,JOB_END_TIME," + this.SqlBaseFieldInsertFront + ") VALUES (@APPLICATION_VERSION,@IS_CURRENT_JOB,@TIMEZONE,@CLIENT_COMPANY,@VERSION,@TIME_AREA,@RIG_NAME,@JOB_MONITOR_SETTING,@JOB_UNIQUE_ID,@WELL_NAME,@SERVICE_POINT,@JOB_DATE_TIME,@SUPERVISOR,@COMPUTER_NAME,@IS_DATA_FROM_CSV,@UNIT_SELECTION,@JOB_PRINT_SETTING,@HAS_DATA_FROM_CSV,@SURFACE_LOCATION,@JOB_NUMBER,@WITS_SETTING,@JOB_TYPE,@DOWNHOLE_LOCATION,@STATUS,@CLIENT_REP,@IS_DST_OFF,@COMMENTS,@JOB_START_TIME,@JOB_END_TIME," + this.SqlBaseFieldInsertBack + ")";
				this.SqlUpdate = "UPDATE JOB_TAG SET APPLICATION_VERSION=@APPLICATION_VERSION,IS_CURRENT_JOB=@IS_CURRENT_JOB,TIMEZONE=@TIMEZONE,CLIENT_COMPANY=@CLIENT_COMPANY,VERSION=@VERSION,TIME_AREA=@TIME_AREA,RIG_NAME=@RIG_NAME,JOB_MONITOR_SETTING=@JOB_MONITOR_SETTING,JOB_UNIQUE_ID=@JOB_UNIQUE_ID,WELL_NAME=@WELL_NAME,SERVICE_POINT=@SERVICE_POINT,JOB_DATE_TIME=@JOB_DATE_TIME,SUPERVISOR=@SUPERVISOR,COMPUTER_NAME=@COMPUTER_NAME,IS_DATA_FROM_CSV=@IS_DATA_FROM_CSV,UNIT_SELECTION=@UNIT_SELECTION,JOB_PRINT_SETTING=@JOB_PRINT_SETTING,HAS_DATA_FROM_CSV=@HAS_DATA_FROM_CSV,SURFACE_LOCATION=@SURFACE_LOCATION,JOB_NUMBER=@JOB_NUMBER,WITS_SETTING=@WITS_SETTING,JOB_TYPE=@JOB_TYPE,DOWNHOLE_LOCATION=@DOWNHOLE_LOCATION,STATUS=@STATUS,CLIENT_REP=@CLIENT_REP,IS_DST_OFF=@IS_DST_OFF,COMMENTS=@COMMENTS,JOB_START_TIME=@JOB_START_TIME,JOB_END_TIME=@JOB_END_TIME," + this.SqlBaseFieldUpdate + " WHERE Id=@Id";
			}
		}

		public class JOB_TAGResultHandler : CommonObjectResultHandler<JOB_TAG>
		{
			public override void GetColumnValues(IDataReader reader, JOB_TAG item)
			{
				base.GetColumnValues(reader, item);
				int ordinalJOB_END_TIME = reader.GetOrdinal("JOB_END_TIME");
				item.JOB_END_TIME = reader.IsDBNull(ordinalJOB_END_TIME) ? DateTime.MinValue : reader.GetDateTime(ordinalJOB_END_TIME);
				int ordinalVERSION = reader.GetOrdinal("VERSION");
				item.VERSION = reader.IsDBNull(ordinalVERSION) ? 0 : reader.GetInt32(ordinalVERSION);
				int ordinalRIG_NAME = reader.GetOrdinal("RIG_NAME");
				item.RIG_NAME = reader.IsDBNull(ordinalRIG_NAME) ? null : reader.GetString(ordinalRIG_NAME);
				int ordinalJOB_MONITOR_SETTING = reader.GetOrdinal("JOB_MONITOR_SETTING");
				item.JOB_MONITOR_SETTING = reader.IsDBNull(ordinalJOB_MONITOR_SETTING) ? null : reader.GetString(ordinalJOB_MONITOR_SETTING);
				int ordinalIS_DST_OFF = reader.GetOrdinal("IS_DST_OFF");
				item.IS_DST_OFF = !reader.IsDBNull(ordinalIS_DST_OFF) && reader.GetBoolean(ordinalIS_DST_OFF);
				int ordinalJOB_DATE_TIME = reader.GetOrdinal("JOB_DATE_TIME");
				item.JOB_DATE_TIME = reader.IsDBNull(ordinalJOB_DATE_TIME) ? DateTime.MinValue : reader.GetDateTime(ordinalJOB_DATE_TIME);
				int ordinalSUPERVISOR = reader.GetOrdinal("SUPERVISOR");
				item.SUPERVISOR = reader.IsDBNull(ordinalSUPERVISOR) ? null : reader.GetString(ordinalSUPERVISOR);
				int ordinalCLIENT_REP = reader.GetOrdinal("CLIENT_REP");
				item.CLIENT_REP = reader.IsDBNull(ordinalCLIENT_REP) ? null : reader.GetString(ordinalCLIENT_REP);
				int ordinalWITS_SETTING = reader.GetOrdinal("WITS_SETTING");
				item.WITS_SETTING = reader.IsDBNull(ordinalWITS_SETTING) ? null : reader.GetString(ordinalWITS_SETTING);
				int ordinalUNIT_SELECTION = reader.GetOrdinal("UNIT_SELECTION");
				item.UNIT_SELECTION = reader.IsDBNull(ordinalUNIT_SELECTION) ? null : reader.GetString(ordinalUNIT_SELECTION);
				int ordinalSERVICE_POINT = reader.GetOrdinal("SERVICE_POINT");
				item.SERVICE_POINT = reader.IsDBNull(ordinalSERVICE_POINT) ? null : reader.GetString(ordinalSERVICE_POINT);
				int ordinalTIMEZONE = reader.GetOrdinal("TIMEZONE");
				item.TIMEZONE = reader.IsDBNull(ordinalTIMEZONE) ? null : reader.GetString(ordinalTIMEZONE);
				int ordinalJOB_NUMBER = reader.GetOrdinal("JOB_NUMBER");
				item.JOB_NUMBER = reader.IsDBNull(ordinalJOB_NUMBER) ? null : reader.GetString(ordinalJOB_NUMBER);
				int ordinalSTATUS = reader.GetOrdinal("STATUS");
				item.STATUS = reader.IsDBNull(ordinalSTATUS) ? null : reader.GetString(ordinalSTATUS);
				int ordinalJOB_PRINT_SETTING = reader.GetOrdinal("JOB_PRINT_SETTING");
				item.JOB_PRINT_SETTING = reader.IsDBNull(ordinalJOB_PRINT_SETTING) ? null : reader.GetString(ordinalJOB_PRINT_SETTING);
				int ordinalWELL_NAME = reader.GetOrdinal("WELL_NAME");
				item.WELL_NAME = reader.IsDBNull(ordinalWELL_NAME) ? null : reader.GetString(ordinalWELL_NAME);
				int ordinalTIME_AREA = reader.GetOrdinal("TIME_AREA");
				item.TIME_AREA = reader.IsDBNull(ordinalTIME_AREA) ? null : reader.GetString(ordinalTIME_AREA);
				int ordinalJOB_START_TIME = reader.GetOrdinal("JOB_START_TIME");
				item.JOB_START_TIME = reader.IsDBNull(ordinalJOB_START_TIME) ? DateTime.MinValue : reader.GetDateTime(ordinalJOB_START_TIME);
				int ordinalCOMMENTS = reader.GetOrdinal("COMMENTS");
				item.COMMENTS = reader.IsDBNull(ordinalCOMMENTS) ? null : reader.GetString(ordinalCOMMENTS);
				int ordinalHAS_DATA_FROM_CSV = reader.GetOrdinal("HAS_DATA_FROM_CSV");
				item.HAS_DATA_FROM_CSV = !reader.IsDBNull(ordinalHAS_DATA_FROM_CSV) && reader.GetBoolean(ordinalHAS_DATA_FROM_CSV);
				int ordinalJOB_TYPE = reader.GetOrdinal("JOB_TYPE");
				item.JOB_TYPE = reader.IsDBNull(ordinalJOB_TYPE) ? null : reader.GetString(ordinalJOB_TYPE);
				int ordinalSURFACE_LOCATION = reader.GetOrdinal("SURFACE_LOCATION");
				item.SURFACE_LOCATION = reader.IsDBNull(ordinalSURFACE_LOCATION) ? null : reader.GetString(ordinalSURFACE_LOCATION);
				int ordinalCOMPUTER_NAME = reader.GetOrdinal("COMPUTER_NAME");
				item.COMPUTER_NAME = reader.IsDBNull(ordinalCOMPUTER_NAME) ? null : reader.GetString(ordinalCOMPUTER_NAME);
				int ordinalCLIENT_COMPANY = reader.GetOrdinal("CLIENT_COMPANY");
				item.CLIENT_COMPANY = reader.IsDBNull(ordinalCLIENT_COMPANY) ? null : reader.GetString(ordinalCLIENT_COMPANY);
				int ordinalDOWNHOLE_LOCATION = reader.GetOrdinal("DOWNHOLE_LOCATION");
				item.DOWNHOLE_LOCATION = reader.IsDBNull(ordinalDOWNHOLE_LOCATION) ? null : reader.GetString(ordinalDOWNHOLE_LOCATION);
				int ordinalAPPLICATION_VERSION = reader.GetOrdinal("APPLICATION_VERSION");
				item.APPLICATION_VERSION = reader.IsDBNull(ordinalAPPLICATION_VERSION) ? null : reader.GetString(ordinalAPPLICATION_VERSION);
				int ordinalIS_CURRENT_JOB = reader.GetOrdinal("IS_CURRENT_JOB");
				item.IS_CURRENT_JOB = !reader.IsDBNull(ordinalIS_CURRENT_JOB) && reader.GetBoolean(ordinalIS_CURRENT_JOB);
				int ordinalIS_DATA_FROM_CSV = reader.GetOrdinal("IS_DATA_FROM_CSV");
				item.IS_DATA_FROM_CSV = !reader.IsDBNull(ordinalIS_DATA_FROM_CSV) && reader.GetBoolean(ordinalIS_DATA_FROM_CSV);
				int ordinalJOB_UNIQUE_ID = reader.GetOrdinal("JOB_UNIQUE_ID");
				item.JOB_UNIQUE_ID = reader.IsDBNull(ordinalJOB_UNIQUE_ID) ? null : reader.GetString(ordinalJOB_UNIQUE_ID);
				/*add customized code between this region*/
				/*add customized code between this region*/
			}

			public override void AddInsertParameters(IContext context, IDbCommand command, JOB_TAG item)
			{
				base.AddInsertParameters(context, command, item);
				context.AddParameter(command, "JOB_END_TIME", item.JOB_END_TIME == DateTime.MinValue ? (object)DBNull.Value : item.JOB_END_TIME);
				context.AddParameter(command, "VERSION", item.VERSION);
				context.AddParameter(command, "RIG_NAME", item.RIG_NAME ?? (object) DBNull.Value);
				context.AddParameter(command, "JOB_MONITOR_SETTING", item.JOB_MONITOR_SETTING ?? (object) DBNull.Value);
				context.AddParameter(command, "IS_DST_OFF", item.IS_DST_OFF);
				context.AddParameter(command, "JOB_DATE_TIME", item.JOB_DATE_TIME == DateTime.MinValue ? (object)DBNull.Value : item.JOB_DATE_TIME);
				context.AddParameter(command, "SUPERVISOR", item.SUPERVISOR ?? (object) DBNull.Value);
				context.AddParameter(command, "CLIENT_REP", item.CLIENT_REP ?? (object) DBNull.Value);
				context.AddParameter(command, "WITS_SETTING", item.WITS_SETTING ?? (object) DBNull.Value);
				context.AddParameter(command, "UNIT_SELECTION", item.UNIT_SELECTION ?? (object) DBNull.Value);
				context.AddParameter(command, "SERVICE_POINT", item.SERVICE_POINT ?? (object) DBNull.Value);
				context.AddParameter(command, "TIMEZONE", item.TIMEZONE ?? (object) DBNull.Value);
				context.AddParameter(command, "JOB_NUMBER", item.JOB_NUMBER ?? (object) DBNull.Value);
				context.AddParameter(command, "STATUS", item.STATUS ?? (object) DBNull.Value);
				context.AddParameter(command, "JOB_PRINT_SETTING", item.JOB_PRINT_SETTING ?? (object) DBNull.Value);
				context.AddParameter(command, "WELL_NAME", item.WELL_NAME ?? (object) DBNull.Value);
				context.AddParameter(command, "TIME_AREA", item.TIME_AREA ?? (object) DBNull.Value);
				context.AddParameter(command, "JOB_START_TIME", item.JOB_START_TIME == DateTime.MinValue ? (object)DBNull.Value : item.JOB_START_TIME);
				context.AddParameter(command, "COMMENTS", item.COMMENTS ?? (object) DBNull.Value);
				context.AddParameter(command, "HAS_DATA_FROM_CSV", item.HAS_DATA_FROM_CSV);
				context.AddParameter(command, "JOB_TYPE", item.JOB_TYPE ?? (object) DBNull.Value);
				context.AddParameter(command, "SURFACE_LOCATION", item.SURFACE_LOCATION ?? (object) DBNull.Value);
				context.AddParameter(command, "COMPUTER_NAME", item.COMPUTER_NAME ?? (object) DBNull.Value);
				context.AddParameter(command, "CLIENT_COMPANY", item.CLIENT_COMPANY ?? (object) DBNull.Value);
				context.AddParameter(command, "DOWNHOLE_LOCATION", item.DOWNHOLE_LOCATION ?? (object) DBNull.Value);
				context.AddParameter(command, "APPLICATION_VERSION", item.APPLICATION_VERSION ?? (object) DBNull.Value);
				context.AddParameter(command, "IS_CURRENT_JOB", item.IS_CURRENT_JOB);
				context.AddParameter(command, "IS_DATA_FROM_CSV", item.IS_DATA_FROM_CSV);
				context.AddParameter(command, "JOB_UNIQUE_ID", item.JOB_UNIQUE_ID ?? (object) DBNull.Value);
				/*add customized code between this region*/
				/*add customized code between this region*/
			}
		}

		public JOB_TAGDao(SqlDialect sqlDialect) : base(new JOB_TAGSqlBuilder(sqlDialect), new JOB_TAGResultHandler())
		{
		}

		public JOB_TAGDao(SqlDialect sqlDialect, string schemaConnectionString) : base(new JOB_TAGSqlBuilder(sqlDialect), new JOB_TAGResultHandler(), schemaConnectionString)
		{
		}
		/*add customized code between this region*/
		/*add customized code between this region*/
	}
}
