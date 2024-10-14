using MetaShare.Common.Core.DataSchema;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Daos.DataSchema.Legacy
{
	public class WITS_SETTINGDdlBuilder : DdlBuilder
	{
		public override string GetSqlCreateTable()
		{
			return @"CREATE TABLE WITS_SETTING(Id int IDENTITY(1,1) PRIMARY KEY NOT NULL,JSON ntext,VERSION int,Description nvarchar(255),TimeStamp datetime2,Name nvarchar(255),Description nvarchar(255),Owner_Id int,Entity_Status int)";
		}

		public override string GetSqlDropTable()
		{
			return @"DROP TABLE WITS_SETTING";
		}

		public override string GetSqlExistTable()
		{
			return @"SELECT COUNT(*) FROM Information_Schema.COLUMNS WHERE TABLE_NAME = 'WITS_SETTING'";
		}
		/*add customized code between this region*/
		/*add customized code between this region*/
	}
}
