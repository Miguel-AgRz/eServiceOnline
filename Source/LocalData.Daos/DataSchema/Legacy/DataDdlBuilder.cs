using MetaShare.Common.Core.DataSchema;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Daos.DataSchema.Legacy
{
	public class DataDdlBuilder : DdlBuilder
	{
		public override string GetSqlCreateTable()
		{
			return @"CREATE TABLE Data(Id int IDENTITY(1,1) PRIMARY KEY NOT NULL,Description nvarchar(255),Name nvarchar(255),JSON ntext,UnitID nvarchar(255),TimeStamp datetime2,Description nvarchar(255),Owner_Id int,Entity_Status int)";
		}

		public override string GetSqlDropTable()
		{
			return @"DROP TABLE Data";
		}

		public override string GetSqlExistTable()
		{
			return @"SELECT COUNT(*) FROM Information_Schema.COLUMNS WHERE TABLE_NAME = 'Data'";
		}
		/*add customized code between this region*/
		/*add customized code between this region*/
	}
}
