using MetaShare.Common.Core.DataSchema;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Daos.DataSchema.LocalOperation
{
	public class SeriesDefinitionDdlBuilder : DdlBuilder
	{
		public override string GetSqlCreateTable()
		{
			return @"CREATE TABLE SeriesDefinition(Id int IDENTITY(1,1) PRIMARY KEY NOT NULL,PlcDataCalculationFormulaId nvarchar(255),Title nvarchar(255),YAxisName nvarchar(255),ParameterId nvarchar(255),UnitCalculationFormulaId nvarchar(255),ChartId nvarchar(255),ActionName nvarchar(255),TruckUnitNumber nvarchar(255),Color nvarchar(255),ControllerName nvarchar(255),IsEnabled bit,IsTemplate bit,Name nvarchar(255),IsSecondaryYAxis bit,Description nvarchar(255),MaxValue decimal(18,2),Description nvarchar(255),Owner_Id int,Entity_Status int)";
		}

		public override string GetSqlDropTable()
		{
			return @"DROP TABLE SeriesDefinition";
		}

		public override string GetSqlExistTable()
		{
			return @"SELECT COUNT(*) FROM Information_Schema.COLUMNS WHERE TABLE_NAME = 'SeriesDefinition'";
		}
		/*add customized code between this region*/
		/*add customized code between this region*/
	}
}
