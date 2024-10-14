using MetaShare.Common.Core.DataSchema;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Daos.DataSchema.Legacy
{
	public class UPLOAD_LOGDdlBuilder : DdlBuilder
	{
		public override string GetSqlCreateTable()
		{
			return @"CREATE TABLE UPLOAD_LOG(Id int IDENTITY(1,1) PRIMARY KEY NOT NULL,JOB_NUMBER nvarchar(255),IS_CLEANED_UP bit,JOB_UNIQUE_ID nvarchar(255),TIMEZONE nvarchar(255),IS_RECEIVED_ON_SERVER bit,COMPUTER_NAME nvarchar(255),Description nvarchar(255),VERSION int,END_TIME datetime2,PACKING_TIME datetime2,PACKING_DURATION nvarchar(255),Name nvarchar(255),PACK_SIZE int,START_TIME datetime2,Description nvarchar(255),Owner_Id int,Entity_Status int)";
		}

		public override string GetSqlDropTable()
		{
			return @"DROP TABLE UPLOAD_LOG";
		}

		public override string GetSqlExistTable()
		{
			return @"SELECT COUNT(*) FROM Information_Schema.COLUMNS WHERE TABLE_NAME = 'UPLOAD_LOG'";
		}
		/*add customized code between this region*/
		/*add customized code between this region*/
	}
}
