using MetaShare.Common.Core.DataSchema;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Daos.DataSchema.Legacy
{
	public class ESE_FLAGSDdlBuilder : DdlBuilder
	{
		public override string GetSqlCreateTable()
		{
			return @"CREATE TABLE ESE_FLAGS(Id int IDENTITY(1,1) PRIMARY KEY NOT NULL,VALUE bit,VERSION int,TIMESTAMP datetime2,Description nvarchar(255),Name nvarchar(255),Description nvarchar(255),Owner_Id int,Entity_Status int)";
		}

		public override string GetSqlDropTable()
		{
			return @"DROP TABLE ESE_FLAGS";
		}

		public override string GetSqlExistTable()
		{
			return @"SELECT COUNT(*) FROM Information_Schema.COLUMNS WHERE TABLE_NAME = 'ESE_FLAGS'";
		}
		/*add customized code between this region*/
		/*add customized code between this region*/
	}
}
