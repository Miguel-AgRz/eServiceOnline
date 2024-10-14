using MetaShare.Common.Core.DataSchema;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Daos.DataSchema.LocalOperation
{
	public class PlcDataCalculationFormulaDdlBuilder : DdlBuilder
	{
		public override string GetSqlCreateTable()
		{
			return @"CREATE TABLE PlcDataCalculationFormula(Id int IDENTITY(1,1) PRIMARY KEY NOT NULL,TruckUnitNumber nvarchar(255),UnitCalculationId nvarchar(255),,IsTemplate bit,Expression nvarchar(255),Name nvarchar(255),Description nvarchar(255),Title nvarchar(255),IsEnabled bit,Description nvarchar(255),Owner_Id int,Entity_Status int)";
		}

		public override string GetSqlDropTable()
		{
			return @"DROP TABLE PlcDataCalculationFormula";
		}

		public override string GetSqlExistTable()
		{
			return @"SELECT COUNT(*) FROM Information_Schema.COLUMNS WHERE TABLE_NAME = 'PlcDataCalculationFormula'";
		}
		/*add customized code between this region*/
		/*add customized code between this region*/
	}
}
