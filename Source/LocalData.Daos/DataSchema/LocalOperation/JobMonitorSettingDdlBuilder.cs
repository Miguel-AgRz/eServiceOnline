using MetaShare.Common.Core.DataSchema;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Daos.DataSchema.LocalOperation
{
	public class JobMonitorSettingDdlBuilder : DdlBuilder
	{
		public override string GetSqlCreateTable()
		{
			return @"CREATE TABLE JobMonitorSetting(Id int IDENTITY(1,1) PRIMARY KEY NOT NULL,Frequency int,JobUniqueId nvarchar(255),Description nvarchar(255),Name nvarchar(255),Duration int,JobNumber nvarchar(255),,Interval int,Description nvarchar(255),Owner_Id int,Entity_Status int)";
		}

		public override string GetSqlDropTable()
		{
			return @"DROP TABLE JobMonitorSetting";
		}

		public override string GetSqlExistTable()
		{
			return @"SELECT COUNT(*) FROM Information_Schema.COLUMNS WHERE TABLE_NAME = 'JobMonitorSetting'";
		}
		/*add customized code between this region*/
		/*add customized code between this region*/
	}
}
