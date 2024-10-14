using MetaShare.Common.Core.DataSchema;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Daos.DataSchema.PostJobReport
{
	public class StorageInfoDdlBuilder : DdlBuilder
	{
		public override string GetSqlCreateTable()
		{
			return @"CREATE TABLE StorageInfo(Id int IDENTITY(1,1) PRIMARY KEY NOT NULL,Remains decimal(18,2),Name nvarchar(255),Description nvarchar(255),JobNumber nvarchar(255),ScaleReading decimal(18,2),PumpedWithAdds decimal(18,2),JobUniqueId nvarchar(255),IsReadyForNext bit,PumpedWoAdds decimal(18,2),InitialTonnage decimal(18,2),StorageType nvarchar(255),BlendName nvarchar(255),Description nvarchar(255),Owner_Id int,Entity_Status int)";
		}

		public override string GetSqlDropTable()
		{
			return @"DROP TABLE StorageInfo";
		}

		public override string GetSqlExistTable()
		{
			return @"SELECT COUNT(*) FROM Information_Schema.COLUMNS WHERE TABLE_NAME = 'StorageInfo'";
		}
		/*add customized code between this region*/
		/*add customized code between this region*/
	}
}
