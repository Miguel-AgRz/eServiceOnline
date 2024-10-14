using MetaShare.Common.Core.DataSchema;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Daos.DataSchema.LocalOperation
{
	public class DcFlagDdlBuilder : DdlBuilder
	{
		public override string GetSqlCreateTable()
		{
			return @"CREATE TABLE DcFlag(Id int IDENTITY(1,1) PRIMARY KEY NOT NULL,TimeStamp datetime2,Description nvarchar(255),Value bit,Name nvarchar(255),Version int,Description nvarchar(255),Owner_Id int,Entity_Status int)";
		}

		public override string GetSqlDropTable()
		{
			return @"DROP TABLE DcFlag";
		}

		public override string GetSqlExistTable()
		{
			return @"SELECT COUNT(*) FROM Information_Schema.COLUMNS WHERE TABLE_NAME = 'DcFlag'";
		}
		/*add customized code between this region*/
		/*add customized code between this region*/
	}
}
