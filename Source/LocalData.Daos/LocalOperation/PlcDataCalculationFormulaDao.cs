using System;
using System.Data;
using MetaShare.Common.Core.Daos;
using Sesi.LocalData.Daos.Interfaces.LocalOperation;
using Sesi.LocalData.Entities.LocalOperation;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Daos.LocalOperation
{
	public class PlcDataCalculationFormulaDao : CommonObjectDao<PlcDataCalculationFormula>, IPlcDataCalculationFormulaDao
	{
		public class PlcDataCalculationFormulaSqlBuilder : ObjectSqlBuilder
		{
			public PlcDataCalculationFormulaSqlBuilder(SqlDialect sqlDialect) : base(sqlDialect,"PlcDataCalculationFormula")
			{
				this.SqlInsert = "INSERT INTO PlcDataCalculationFormula (IsEnabled,IsTemplate,Title,TruckUnitNumber,UnitCalculationid,Expression," + this.SqlBaseFieldInsertFront + ") VALUES (@IsEnabled,@IsTemplate,@Title,@TruckUnitNumber,@UnitCalculationid,@Expression," + this.SqlBaseFieldInsertBack + ")";
				this.SqlUpdate = "UPDATE PlcDataCalculationFormula SET IsEnabled=@IsEnabled,IsTemplate=@IsTemplate,Title=@Title,TruckUnitNumber=@TruckUnitNumber,UnitCalculationid=@UnitCalculationid,Expression=@Expression," + this.SqlBaseFieldUpdate + " WHERE Id=@Id";
			}
		}

		public class PlcDataCalculationFormulaResultHandler : CommonObjectResultHandler<PlcDataCalculationFormula>
		{
			public override void GetColumnValues(IDataReader reader, PlcDataCalculationFormula item)
			{
				base.GetColumnValues(reader, item);
				int ordinalTruckUnitNumber = reader.GetOrdinal("TruckUnitNumber");
				item.TruckUnitNumber = reader.IsDBNull(ordinalTruckUnitNumber) ? null : reader.GetString(ordinalTruckUnitNumber);
				int ordinalUnitCalculationId = reader.GetOrdinal("UnitCalculationId");
				item.UnitCalculation = reader.IsDBNull(ordinalUnitCalculationId) ? null :reader.GetInt32(ordinalUnitCalculationId)==0?null:new UnitCalculationFormula { Id=reader.GetInt32(ordinalUnitCalculationId)};
				int ordinalIsTemplate = reader.GetOrdinal("IsTemplate");
				item.IsTemplate = !reader.IsDBNull(ordinalIsTemplate) && reader.GetBoolean(ordinalIsTemplate);
				int ordinalExpression = reader.GetOrdinal("Expression");
				item.Expression = reader.IsDBNull(ordinalExpression) ? null : reader.GetString(ordinalExpression);
				int ordinalTitle = reader.GetOrdinal("Title");
				item.Title = reader.IsDBNull(ordinalTitle) ? null : reader.GetString(ordinalTitle);
				int ordinalIsEnabled = reader.GetOrdinal("IsEnabled");
				item.IsEnabled = !reader.IsDBNull(ordinalIsEnabled) && reader.GetBoolean(ordinalIsEnabled);
				/*add customized code between this region*/
				/*add customized code between this region*/
			}

			public override void AddInsertParameters(IContext context, IDbCommand command, PlcDataCalculationFormula item)
			{
				base.AddInsertParameters(context, command, item);
				context.AddParameter(command, "TruckUnitNumber", item.TruckUnitNumber ?? (object) DBNull.Value);
				context.AddParameter(command, "UnitCalculationId", item.UnitCalculation ==null? 0:item.UnitCalculation.Id);

				context.AddParameter(command, "IsTemplate", item.IsTemplate);
				context.AddParameter(command, "Expression", item.Expression ?? (object) DBNull.Value);
				context.AddParameter(command, "Title", item.Title ?? (object) DBNull.Value);
				context.AddParameter(command, "IsEnabled", item.IsEnabled);
				/*add customized code between this region*/
				/*add customized code between this region*/
			}
		}

		public PlcDataCalculationFormulaDao(SqlDialect sqlDialect) : base(new PlcDataCalculationFormulaSqlBuilder(sqlDialect), new PlcDataCalculationFormulaResultHandler())
		{
		}

		public PlcDataCalculationFormulaDao(SqlDialect sqlDialect, string schemaConnectionString) : base(new PlcDataCalculationFormulaSqlBuilder(sqlDialect), new PlcDataCalculationFormulaResultHandler(), schemaConnectionString)
		{
		}
		/*add customized code between this region*/
		/*add customized code between this region*/
	}
}
