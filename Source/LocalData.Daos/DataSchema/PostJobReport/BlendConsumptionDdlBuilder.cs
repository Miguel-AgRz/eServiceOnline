using MetaShare.Common.Core.DataSchema;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Daos.DataSchema.PostJobReport
{
	public class BlendConsumptionDdlBuilder : DdlBuilder
	{
		public override string GetSqlCreateTable()
		{
			return @"CREATE TABLE BlendConsumption(Id int IDENTITY(1,1) PRIMARY KEY NOT NULL,BlendCategory nvarchar(255),BlendReportId nvarchar(255),SlurryTemperature decimal(18,2),BulkTemperature decimal(18,2),JobIntervalTypeName nvarchar(255),Name nvarchar(255),JobIntervalId int,WaterTemperature decimal(18,2),BlendName nvarchar(255),Description nvarchar(255),JobEventNumber int,BlendDescription nvarchar(255),PumpedVolume decimal(18,2),Description nvarchar(255),Owner_Id int,Entity_Status int)";
		}

		public override string GetSqlDropTable()
		{
			return @"DROP TABLE BlendConsumption";
		}

		public override string GetSqlExistTable()
		{
			return @"SELECT COUNT(*) FROM Information_Schema.COLUMNS WHERE TABLE_NAME = 'BlendConsumption'";
		}
		/*add customized code between this region*/
		/*add customized code between this region*/
	}
}
