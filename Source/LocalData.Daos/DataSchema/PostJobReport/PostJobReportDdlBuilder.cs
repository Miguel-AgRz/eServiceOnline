using MetaShare.Common.Core.DataSchema;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Daos.DataSchema.PostJobReport
{
	public class PostJobReportDdlBuilder : DdlBuilder
	{
		public override string GetSqlCreateTable()
		{
			return @"CREATE TABLE PostJobReport(Id int IDENTITY(1,1) PRIMARY KEY NOT NULL,SurfaceLocation nvarchar(255),CallSheetNumber nvarchar(255),RevisedDirection nvarchar(255),JobUniqueId nvarchar(255),AdditionalInformation nvarchar(255),ClientName nvarchar(255),JobNumber nvarchar(255),Name nvarchar(255),IsDirectionRevised bit,RigName nvarchar(255),JobType nvarchar(255),DownHoleLocation nvarchar(255),JobDate datetime2,Description nvarchar(255),Description nvarchar(255),Owner_Id int,Entity_Status int)";
		}

		public override string GetSqlDropTable()
		{
			return @"DROP TABLE PostJobReport";
		}

		public override string GetSqlExistTable()
		{
			return @"SELECT COUNT(*) FROM Information_Schema.COLUMNS WHERE TABLE_NAME = 'PostJobReport'";
		}
		/*add customized code between this region*/
		/*add customized code between this region*/
	}
}
