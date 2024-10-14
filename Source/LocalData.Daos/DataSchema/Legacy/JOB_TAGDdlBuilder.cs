using MetaShare.Common.Core.DataSchema;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Daos.DataSchema.Legacy
{
	public class JOB_TAGDdlBuilder : DdlBuilder
	{
		public override string GetSqlCreateTable()
		{
			return @"CREATE TABLE JOB_TAG(Id int IDENTITY(1,1) PRIMARY KEY NOT NULL,JOB_END_TIME datetime2,VERSION int,RIG_NAME nvarchar(255),JOB_MONITOR_SETTING ntext,IS_DST_OFF bit,JOB_DATE_TIME datetime2,SUPERVISOR nvarchar(255),CLIENT_REP nvarchar(255),WITS_SETTING ntext,UNIT_SELECTION ntext,SERVICE_POINT nvarchar(255),TIMEZONE nvarchar(255),JOB_NUMBER nvarchar(255),STATUS nvarchar(255),Name nvarchar(255),Description nvarchar(255),JOB_PRINT_SETTING ntext,WELL_NAME nvarchar(255),TIME_AREA nvarchar(255),JOB_START_TIME datetime2,COMMENTS ntext,HAS_DATA_FROM_CSV bit,JOB_TYPE nvarchar(255),SURFACE_LOCATION nvarchar(255),COMPUTER_NAME nvarchar(255),CLIENT_COMPANY nvarchar(255),DOWNHOLE_LOCATION nvarchar(255),APPLICATION_VERSION nvarchar(255),IS_CURRENT_JOB bit,IS_DATA_FROM_CSV bit,JOB_UNIQUE_ID nvarchar(255),Description nvarchar(255),Owner_Id int,Entity_Status int)";
		}

		public override string GetSqlDropTable()
		{
			return @"DROP TABLE JOB_TAG";
		}

		public override string GetSqlExistTable()
		{
			return @"SELECT COUNT(*) FROM Information_Schema.COLUMNS WHERE TABLE_NAME = 'JOB_TAG'";
		}
		/*add customized code between this region*/
		/*add customized code between this region*/
	}
}
