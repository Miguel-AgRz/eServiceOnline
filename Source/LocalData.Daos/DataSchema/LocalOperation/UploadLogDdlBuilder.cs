using MetaShare.Common.Core.DataSchema;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Daos.DataSchema.LocalOperation
{
	public class UploadLogDdlBuilder : DdlBuilder
	{
		public override string GetSqlCreateTable()
		{
			return @"CREATE TABLE UploadLog(Id int IDENTITY(1,1) PRIMARY KEY NOT NULL,Version int,Name nvarchar(255),StartTime datetime2,JobUniqueId nvarchar(255),JobNumber nvarchar(255),PackingEndDateTime datetime2,IsRecievedOnServer bit,Description nvarchar(255),IsPlcDataDeleted bit,TimeZone nvarchar(255),ComputerName nvarchar(255),EndTime datetime2,PackingDuration nvarchar(255),PackSize int,Description nvarchar(255),Owner_Id int,Entity_Status int)";
		}

		public override string GetSqlDropTable()
		{
			return @"DROP TABLE UploadLog";
		}

		public override string GetSqlExistTable()
		{
			return @"SELECT COUNT(*) FROM Information_Schema.COLUMNS WHERE TABLE_NAME = 'UploadLog'";
		}
		/*add customized code between this region*/
		/*add customized code between this region*/
	}
}
