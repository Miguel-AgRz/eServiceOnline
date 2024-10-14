using MetaShare.Common.Core.DataSchema;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Daos.DataSchema.PostJobReport
{
	public class BlendReportDdlBuilder : DdlBuilder
	{
		public override string GetSqlCreateTable()
		{
			return @"CREATE TABLE BlendReport(Id int IDENTITY(1,1) PRIMARY KEY NOT NULL,JobUniqueId nvarchar(255),Description nvarchar(255),TotalPumpedVolume decimal(18,2),Name nvarchar(255),,ExpectedCementTop decimal(18,2),Description nvarchar(255),Owner_Id int,Entity_Status int)";
		}

		public override string GetSqlDropTable()
		{
			return @"DROP TABLE BlendReport";
		}

		public override string GetSqlExistTable()
		{
			return @"SELECT COUNT(*) FROM Information_Schema.COLUMNS WHERE TABLE_NAME = 'BlendReport'";
		}
		/*add customized code between this region*/
		/*add customized code between this region*/
	}
}
