using System;
using System.Data;
using MetaShare.Common.Core.Daos;
using Sesi.LocalData.Daos.Interfaces.Legacy;
using Sesi.LocalData.Entities.Legacy;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Daos.Legacy
{
	public class UPLOAD_LOGDao : CommonObjectDao<UPLOAD_LOG>, IUPLOAD_LOGDao
	{
		public class UPLOAD_LOGSqlBuilder : ObjectSqlBuilder
		{
			public UPLOAD_LOGSqlBuilder(SqlDialect sqlDialect) : base(sqlDialect,"UPLOAD_LOG")
			{
				this.SqlInsert = "INSERT INTO UPLOAD_LOG (IS_CLEANED_UP,JOB_UNIQUE_ID,END_TIME,JOB_NUMBER,TIMEZONE,COMPUTER_NAME,VERSION,IS_RECEIVED_ON_SERVER,PACKING_TIME,PACK_SIZE,PACKING_DURATION,START_TIME," + this.SqlBaseFieldInsertFront + ") VALUES (@IS_CLEANED_UP,@JOB_UNIQUE_ID,@END_TIME,@JOB_NUMBER,@TIMEZONE,@COMPUTER_NAME,@VERSION,@IS_RECEIVED_ON_SERVER,@PACKING_TIME,@PACK_SIZE,@PACKING_DURATION,@START_TIME," + this.SqlBaseFieldInsertBack + ")";
				this.SqlUpdate = "UPDATE UPLOAD_LOG SET IS_CLEANED_UP=@IS_CLEANED_UP,JOB_UNIQUE_ID=@JOB_UNIQUE_ID,END_TIME=@END_TIME,JOB_NUMBER=@JOB_NUMBER,TIMEZONE=@TIMEZONE,COMPUTER_NAME=@COMPUTER_NAME,VERSION=@VERSION,IS_RECEIVED_ON_SERVER=@IS_RECEIVED_ON_SERVER,PACKING_TIME=@PACKING_TIME,PACK_SIZE=@PACK_SIZE,PACKING_DURATION=@PACKING_DURATION,START_TIME=@START_TIME," + this.SqlBaseFieldUpdate + " WHERE Id=@Id";
			}
		}

		public class UPLOAD_LOGResultHandler : CommonObjectResultHandler<UPLOAD_LOG>
		{
			public override void GetColumnValues(IDataReader reader, UPLOAD_LOG item)
			{
				base.GetColumnValues(reader, item);
				int ordinalJOB_NUMBER = reader.GetOrdinal("JOB_NUMBER");
				item.JOB_NUMBER = reader.IsDBNull(ordinalJOB_NUMBER) ? null : reader.GetString(ordinalJOB_NUMBER);
				int ordinalIS_CLEANED_UP = reader.GetOrdinal("IS_CLEANED_UP");
				item.IS_CLEANED_UP = !reader.IsDBNull(ordinalIS_CLEANED_UP) && reader.GetBoolean(ordinalIS_CLEANED_UP);
				int ordinalJOB_UNIQUE_ID = reader.GetOrdinal("JOB_UNIQUE_ID");
				item.JOB_UNIQUE_ID = reader.IsDBNull(ordinalJOB_UNIQUE_ID) ? null : reader.GetString(ordinalJOB_UNIQUE_ID);
				int ordinalTIMEZONE = reader.GetOrdinal("TIMEZONE");
				item.TIMEZONE = reader.IsDBNull(ordinalTIMEZONE) ? null : reader.GetString(ordinalTIMEZONE);
				int ordinalIS_RECEIVED_ON_SERVER = reader.GetOrdinal("IS_RECEIVED_ON_SERVER");
				item.IS_RECEIVED_ON_SERVER = !reader.IsDBNull(ordinalIS_RECEIVED_ON_SERVER) && reader.GetBoolean(ordinalIS_RECEIVED_ON_SERVER);
				int ordinalCOMPUTER_NAME = reader.GetOrdinal("COMPUTER_NAME");
				item.COMPUTER_NAME = reader.IsDBNull(ordinalCOMPUTER_NAME) ? null : reader.GetString(ordinalCOMPUTER_NAME);
				int ordinalVERSION = reader.GetOrdinal("VERSION");
				item.VERSION = reader.IsDBNull(ordinalVERSION) ? 0 : reader.GetInt32(ordinalVERSION);
				int ordinalEND_TIME = reader.GetOrdinal("END_TIME");
				item.END_TIME = reader.IsDBNull(ordinalEND_TIME) ? DateTime.MinValue : reader.GetDateTime(ordinalEND_TIME);
				int ordinalPACKING_TIME = reader.GetOrdinal("PACKING_TIME");
				item.PACKING_TIME = reader.IsDBNull(ordinalPACKING_TIME) ? DateTime.MinValue : reader.GetDateTime(ordinalPACKING_TIME);
				int ordinalPACKING_DURATION = reader.GetOrdinal("PACKING_DURATION");
				item.PACKING_DURATION = reader.IsDBNull(ordinalPACKING_DURATION) ? null : reader.GetString(ordinalPACKING_DURATION);
				int ordinalPACK_SIZE = reader.GetOrdinal("PACK_SIZE");
				item.PACK_SIZE = reader.IsDBNull(ordinalPACK_SIZE) ? 0 : reader.GetInt32(ordinalPACK_SIZE);
				int ordinalSTART_TIME = reader.GetOrdinal("START_TIME");
				item.START_TIME = reader.IsDBNull(ordinalSTART_TIME) ? DateTime.MinValue : reader.GetDateTime(ordinalSTART_TIME);
				/*add customized code between this region*/
				/*add customized code between this region*/
			}

			public override void AddInsertParameters(IContext context, IDbCommand command, UPLOAD_LOG item)
			{
				base.AddInsertParameters(context, command, item);
				context.AddParameter(command, "JOB_NUMBER", item.JOB_NUMBER ?? (object) DBNull.Value);
				context.AddParameter(command, "IS_CLEANED_UP", item.IS_CLEANED_UP);
				context.AddParameter(command, "JOB_UNIQUE_ID", item.JOB_UNIQUE_ID ?? (object) DBNull.Value);
				context.AddParameter(command, "TIMEZONE", item.TIMEZONE ?? (object) DBNull.Value);
				context.AddParameter(command, "IS_RECEIVED_ON_SERVER", item.IS_RECEIVED_ON_SERVER);
				context.AddParameter(command, "COMPUTER_NAME", item.COMPUTER_NAME ?? (object) DBNull.Value);
				context.AddParameter(command, "VERSION", item.VERSION);
				context.AddParameter(command, "END_TIME", item.END_TIME == DateTime.MinValue ? (object)DBNull.Value : item.END_TIME);
				context.AddParameter(command, "PACKING_TIME", item.PACKING_TIME == DateTime.MinValue ? (object)DBNull.Value : item.PACKING_TIME);
				context.AddParameter(command, "PACKING_DURATION", item.PACKING_DURATION ?? (object) DBNull.Value);
				context.AddParameter(command, "PACK_SIZE", item.PACK_SIZE);
				context.AddParameter(command, "START_TIME", item.START_TIME == DateTime.MinValue ? (object)DBNull.Value : item.START_TIME);
				/*add customized code between this region*/
				/*add customized code between this region*/
			}
		}

		public UPLOAD_LOGDao(SqlDialect sqlDialect) : base(new UPLOAD_LOGSqlBuilder(sqlDialect), new UPLOAD_LOGResultHandler())
		{
		}

		public UPLOAD_LOGDao(SqlDialect sqlDialect, string schemaConnectionString) : base(new UPLOAD_LOGSqlBuilder(sqlDialect), new UPLOAD_LOGResultHandler(), schemaConnectionString)
		{
		}
		/*add customized code between this region*/
		/*add customized code between this region*/
	}
}
