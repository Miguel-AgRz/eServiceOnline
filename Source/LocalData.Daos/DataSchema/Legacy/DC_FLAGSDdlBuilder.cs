using MetaShare.Common.Core.DataSchema;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Daos.DataSchema.Legacy
{
	public class DC_FLAGSDdlBuilder : DdlBuilder
	{
		public override string GetSqlCreateTable()
		{
			return @"CREATE TABLE DC_FLAGS(Id int IDENTITY(1,1) PRIMARY KEY NOT NULL,VERSION int,Name nvarchar(255),Value bit,Description nvarchar(255),TimeStamp datetime2,Description nvarchar(255),Owner_Id int,Entity_Status int)";
		}

		public override string GetSqlDropTable()
		{
			return @"DROP TABLE DC_FLAGS";
		}

		public override string GetSqlExistTable()
		{
			return @"SELECT COUNT(*) FROM Information_Schema.COLUMNS WHERE TABLE_NAME = 'DC_FLAGS'";
		}
		/*add customized code between this region*/
		/*add customized code between this region*/
	}
}
