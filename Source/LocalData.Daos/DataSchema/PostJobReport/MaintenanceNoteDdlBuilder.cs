using MetaShare.Common.Core.DataSchema;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Daos.DataSchema.PostJobReport
{
	public class MaintenanceNoteDdlBuilder : DdlBuilder
	{
		public override string GetSqlCreateTable()
		{
			return @"CREATE TABLE MaintenanceNote(Id int IDENTITY(1,1) PRIMARY KEY NOT NULL,UnitNumber nvarchar(255),JobUniqueId nvarchar(255),JobNumber nvarchar(255),Description nvarchar(255),UnitType nvarchar(255),Notes ntext,Name nvarchar(255),Description nvarchar(255),Owner_Id int,Entity_Status int)";
		}

		public override string GetSqlDropTable()
		{
			return @"DROP TABLE MaintenanceNote";
		}

		public override string GetSqlExistTable()
		{
			return @"SELECT COUNT(*) FROM Information_Schema.COLUMNS WHERE TABLE_NAME = 'MaintenanceNote'";
		}
		/*add customized code between this region*/
		/*add customized code between this region*/
	}
}
