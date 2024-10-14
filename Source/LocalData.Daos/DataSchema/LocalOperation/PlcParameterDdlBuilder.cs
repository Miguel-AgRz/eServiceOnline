using MetaShare.Common.Core.DataSchema;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Daos.DataSchema.LocalOperation
{
	public class PlcParameterDdlBuilder : DdlBuilder
	{
		public override string GetSqlCreateTable()
		{
			return @"CREATE TABLE PlcParameter(Id int IDENTITY(1,1) PRIMARY KEY NOT NULL,PlcCalculationId nvarchar(255),Description nvarchar(255),Comments nvarchar(255),Name nvarchar(255),Uom nvarchar(255),DataIndex int,DataType nvarchar(255),Description nvarchar(255),Owner_Id int,Entity_Status int)";
		}

		public override string GetSqlDropTable()
		{
			return @"DROP TABLE PlcParameter";
		}

		public override string GetSqlExistTable()
		{
			return @"SELECT COUNT(*) FROM Information_Schema.COLUMNS WHERE TABLE_NAME = 'PlcParameter'";
		}
		/*add customized code between this region*/
		/*add customized code between this region*/
	}
}
