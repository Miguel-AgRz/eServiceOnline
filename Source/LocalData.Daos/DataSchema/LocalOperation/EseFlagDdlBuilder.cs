using MetaShare.Common.Core.DataSchema;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Daos.DataSchema.LocalOperation
{
	public class EseFlagDdlBuilder : DdlBuilder
	{
		public override string GetSqlCreateTable()
		{
			return @"CREATE TABLE EseFlag(Id int IDENTITY(1,1) PRIMARY KEY NOT NULL,Value bit,Version int,TimeStamp datetime2,Name nvarchar(255),Description nvarchar(255),Description nvarchar(255),Owner_Id int,Entity_Status int)";
		}

		public override string GetSqlDropTable()
		{
			return @"DROP TABLE EseFlag";
		}

		public override string GetSqlExistTable()
		{
			return @"SELECT COUNT(*) FROM Information_Schema.COLUMNS WHERE TABLE_NAME = 'EseFlag'";
		}
		/*add customized code between this region*/
		/*add customized code between this region*/
	}
}
