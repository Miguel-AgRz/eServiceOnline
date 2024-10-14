using MetaShare.Common.Core.DataSchema;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Daos.DataSchema.LocalOperation
{
	public class ChartDefinitionDdlBuilder : DdlBuilder
	{
		public override string GetSqlCreateTable()
		{
			return @"CREATE TABLE ChartDefinition(Id int IDENTITY(1,1) PRIMARY KEY NOT NULL,IsPrintTogether bit,IsEnabled bit,SecondaryYAxisInterval decimal(18,2),YAxisInterval decimal(18,2),,JobMonSettingId nvarchar(255),YAxisMin decimal(18,2),YAxisUom nvarchar(255),Name nvarchar(255),SecondaryYAxisUom nvarchar(255),SecondaryYAxisMax decimal(18,2),SecondaryYAxisMin decimal(18,2),YAxisMax decimal(18,2),Title nvarchar(255),ExistSecondAxis bit,Description nvarchar(255),LabelFormat nvarchar(255),Description nvarchar(255),Owner_Id int,Entity_Status int)";
		}

		public override string GetSqlDropTable()
		{
			return @"DROP TABLE ChartDefinition";
		}

		public override string GetSqlExistTable()
		{
			return @"SELECT COUNT(*) FROM Information_Schema.COLUMNS WHERE TABLE_NAME = 'ChartDefinition'";
		}
		/*add customized code between this region*/
		/*add customized code between this region*/
	}
}
