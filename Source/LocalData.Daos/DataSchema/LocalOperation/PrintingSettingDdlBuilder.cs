using MetaShare.Common.Core.DataSchema;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Daos.DataSchema.LocalOperation
{
	public class PrintingSettingDdlBuilder : DdlBuilder
	{
		public override string GetSqlCreateTable()
		{
			return @"CREATE TABLE PrintingSetting(Id int IDENTITY(1,1) PRIMARY KEY NOT NULL,IsDataFromCsv bit,Name nvarchar(255),EndTime datetime2,Description nvarchar(255),StartTime datetime2,Description nvarchar(255),Owner_Id int,Entity_Status int)";
		}

		public override string GetSqlDropTable()
		{
			return @"DROP TABLE PrintingSetting";
		}

		public override string GetSqlExistTable()
		{
			return @"SELECT COUNT(*) FROM Information_Schema.COLUMNS WHERE TABLE_NAME = 'PrintingSetting'";
		}
		/*add customized code between this region*/
		/*add customized code between this region*/
	}
}
