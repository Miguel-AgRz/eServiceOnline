using MetaShare.Common.Core.DataSchema;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Daos.DataSchema.LocalOperation
{
	public class UnitCalculationFormulaDdlBuilder : DdlBuilder
	{
		public override string GetSqlCreateTable()
		{
			return @"CREATE TABLE UnitCalculationFormula(Id int IDENTITY(1,1) PRIMARY KEY NOT NULL,Description nvarchar(255),Name nvarchar(255),,Expression nvarchar(255),Description nvarchar(255),Owner_Id int,Entity_Status int)";
		}

		public override string GetSqlDropTable()
		{
			return @"DROP TABLE UnitCalculationFormula";
		}

		public override string GetSqlExistTable()
		{
			return @"SELECT COUNT(*) FROM Information_Schema.COLUMNS WHERE TABLE_NAME = 'UnitCalculationFormula'";
		}
		/*add customized code between this region*/
		/*add customized code between this region*/
	}
}
