using MetaShare.Common.Core.DataSchema;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Daos.DataSchema.LocalOperation
{
	public class JobTagDdlBuilder : DdlBuilder
	{
		public override string GetSqlCreateTable()
		{
			return @"CREATE TABLE JobTag(Id int IDENTITY(1,1) PRIMARY KEY NOT NULL,HasDataFromCsv bit,PrintingSetting nvarchar(255),SurfaceLocation nvarchar(255),Timezone nvarchar(255),ClientCompany nvarchar(255),ComputerName nvarchar(255),IsDataFromCsv bit,TruckUnitSelection nvarchar(255),IsCurrentJob bit,Name nvarchar(255),ClientRep nvarchar(255),DownHoleLocation nvarchar(255),AppicationVersion nvarchar(255),JobEndTime datetime2,TimeArea nvarchar(255),Comments nvarchar(255),Version int,JobNumber nvarchar(255),JobDateTime datetime2,Description nvarchar(255),ServicePoint nvarchar(255),WellName nvarchar(255),Status nvarchar(255),JobStartTime datetime2,RigName nvarchar(255),JobUniqueId nvarchar(255),IsDstOff bit,JobType nvarchar(255),Supervisor nvarchar(255),WITSSetting nvarchar(255),JobMonitorSetting nvarchar(255),Description nvarchar(255),Owner_Id int,Entity_Status int)";
		}

		public override string GetSqlDropTable()
		{
			return @"DROP TABLE JobTag";
		}

		public override string GetSqlExistTable()
		{
			return @"SELECT COUNT(*) FROM Information_Schema.COLUMNS WHERE TABLE_NAME = 'JobTag'";
		}
		/*add customized code between this region*/
		/*add customized code between this region*/
	}
}
