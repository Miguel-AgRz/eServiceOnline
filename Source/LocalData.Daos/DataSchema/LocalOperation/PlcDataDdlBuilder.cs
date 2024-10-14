using MetaShare.Common.Core.DataSchema;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Daos.DataSchema.LocalOperation
{
	public class PlcDataDdlBuilder : DdlBuilder
	{
		public override string GetSqlCreateTable()
		{
			return @"CREATE TABLE PlcData(Id int IDENTITY(1,1) PRIMARY KEY NOT NULL,Description nvarchar(255),Name nvarchar(255),TimeStamp datetime2,JSON nvarchar(255),UnitID nvarchar(255),Description nvarchar(255),Owner_Id int,Entity_Status int)";
		}

		public override string GetSqlDropTable()
		{
			return @"DROP TABLE PlcData";
		}

		public override string GetSqlExistTable()
		{
			return @"SELECT COUNT(*) FROM Information_Schema.COLUMNS WHERE TABLE_NAME = 'PlcData'";
		}
		/*add customized code between this region*/
		/*add customized code between this region*/
	}
}
