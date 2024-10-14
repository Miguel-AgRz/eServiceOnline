using MetaShare.Common.Core.DataSchema;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Daos.DataSchema.LocalOperation
{
	public class WitsDataDdlBuilder : DdlBuilder
	{
		public override string GetSqlCreateTable()
		{
			return @"CREATE TABLE WitsData(Id int IDENTITY(1,1) PRIMARY KEY NOT NULL,VERSION int,JSON ntext,Name nvarchar(255),Description nvarchar(255),TimeStamp nvarchar(255),Description nvarchar(255),Owner_Id int,Entity_Status int)";
		}

		public override string GetSqlDropTable()
		{
			return @"DROP TABLE WitsData";
		}

		public override string GetSqlExistTable()
		{
			return @"SELECT COUNT(*) FROM Information_Schema.COLUMNS WHERE TABLE_NAME = 'WitsData'";
		}
		/*add customized code between this region*/
		/*add customized code between this region*/
	}
}
