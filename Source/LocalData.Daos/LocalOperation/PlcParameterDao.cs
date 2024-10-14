using System;
using System.Data;
using MetaShare.Common.Core.Daos;
using Sesi.LocalData.Daos.Interfaces.LocalOperation;
using Sesi.LocalData.Entities.LocalOperation;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Daos.LocalOperation
{
	public class PlcParameterDao : CommonObjectDao<PlcParameter>, IPlcParameterDao
	{
		public class PlcParameterSqlBuilder : ObjectSqlBuilder
		{
			public PlcParameterSqlBuilder(SqlDialect sqlDialect) : base(sqlDialect,"PlcParameter")
			{
				this.SqlInsert = "INSERT INTO PlcParameter (Comments,PlcCalculationid,DataType,Uom,DataIndex," + this.SqlBaseFieldInsertFront + ") VALUES (@Comments,@PlcCalculationid,@DataType,@Uom,@DataIndex," + this.SqlBaseFieldInsertBack + ")";
				this.SqlUpdate = "UPDATE PlcParameter SET Comments=@Comments,PlcCalculationid=@PlcCalculationid,DataType=@DataType,Uom=@Uom,DataIndex=@DataIndex," + this.SqlBaseFieldUpdate + " WHERE Id=@Id";
			}
		}

		public class PlcParameterResultHandler : CommonObjectResultHandler<PlcParameter>
		{
			public override void GetColumnValues(IDataReader reader, PlcParameter item)
			{
				base.GetColumnValues(reader, item);
				int ordinalPlcCalculationId = reader.GetOrdinal("PlcCalculationId");
				item.PlcCalculation = reader.IsDBNull(ordinalPlcCalculationId) ? null :reader.GetInt32(ordinalPlcCalculationId)==0?null:new PlcDataCalculationFormula { Id=reader.GetInt32(ordinalPlcCalculationId)};
				int ordinalComments = reader.GetOrdinal("Comments");
				item.Comments = reader.IsDBNull(ordinalComments) ? null : reader.GetString(ordinalComments);
				int ordinalUom = reader.GetOrdinal("Uom");
				item.Uom = reader.IsDBNull(ordinalUom) ? null : reader.GetString(ordinalUom);
				int ordinalDataIndex = reader.GetOrdinal("DataIndex");
				item.DataIndex = reader.IsDBNull(ordinalDataIndex) ? 0 : reader.GetInt32(ordinalDataIndex);
				int ordinalDataType = reader.GetOrdinal("DataType");
				item.DataType = reader.IsDBNull(ordinalDataType) ? null : reader.GetString(ordinalDataType);
				/*add customized code between this region*/
				/*add customized code between this region*/
			}

			public override void AddInsertParameters(IContext context, IDbCommand command, PlcParameter item)
			{
				base.AddInsertParameters(context, command, item);
				context.AddParameter(command, "PlcCalculationId", item.PlcCalculation ==null? 0:item.PlcCalculation.Id);

				context.AddParameter(command, "Comments", item.Comments ?? (object) DBNull.Value);
				context.AddParameter(command, "Uom", item.Uom ?? (object) DBNull.Value);
				context.AddParameter(command, "DataIndex", item.DataIndex);
				context.AddParameter(command, "DataType", item.DataType ?? (object) DBNull.Value);
				/*add customized code between this region*/
				/*add customized code between this region*/
			}
		}

		public PlcParameterDao(SqlDialect sqlDialect) : base(new PlcParameterSqlBuilder(sqlDialect), new PlcParameterResultHandler())
		{
		}

		public PlcParameterDao(SqlDialect sqlDialect, string schemaConnectionString) : base(new PlcParameterSqlBuilder(sqlDialect), new PlcParameterResultHandler(), schemaConnectionString)
		{
		}
		/*add customized code between this region*/
		/*add customized code between this region*/
	}
}
