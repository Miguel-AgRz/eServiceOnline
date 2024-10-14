using System;
using System.Data;
using MetaShare.Common.Core.Daos;
using Sesi.LocalData.Daos.Interfaces.LocalOperation;
using Sesi.LocalData.Entities.LocalOperation;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Daos.LocalOperation
{
	public class UnitCalculationFormulaDao : CommonObjectDao<UnitCalculationFormula>, IUnitCalculationFormulaDao
	{
		public class UnitCalculationFormulaSqlBuilder : ObjectSqlBuilder
		{
			public UnitCalculationFormulaSqlBuilder(SqlDialect sqlDialect) : base(sqlDialect,"UnitCalculationFormula")
			{
				this.SqlInsert = "INSERT INTO UnitCalculationFormula (Expression," + this.SqlBaseFieldInsertFront + ") VALUES (@Expression," + this.SqlBaseFieldInsertBack + ")";
				this.SqlUpdate = "UPDATE UnitCalculationFormula SET Expression=@Expression," + this.SqlBaseFieldUpdate + " WHERE Id=@Id";
			}
		}

		public class UnitCalculationFormulaResultHandler : CommonObjectResultHandler<UnitCalculationFormula>
		{
			public override void GetColumnValues(IDataReader reader, UnitCalculationFormula item)
			{
				base.GetColumnValues(reader, item);
				int ordinalExpression = reader.GetOrdinal("Expression");
				item.Expression = reader.IsDBNull(ordinalExpression) ? null : reader.GetString(ordinalExpression);
				/*add customized code between this region*/
				/*add customized code between this region*/
			}

			public override void AddInsertParameters(IContext context, IDbCommand command, UnitCalculationFormula item)
			{
				base.AddInsertParameters(context, command, item);
				context.AddParameter(command, "Expression", item.Expression ?? (object) DBNull.Value);
				/*add customized code between this region*/
				/*add customized code between this region*/
			}
		}

		public UnitCalculationFormulaDao(SqlDialect sqlDialect) : base(new UnitCalculationFormulaSqlBuilder(sqlDialect), new UnitCalculationFormulaResultHandler())
		{
		}

		public UnitCalculationFormulaDao(SqlDialect sqlDialect, string schemaConnectionString) : base(new UnitCalculationFormulaSqlBuilder(sqlDialect), new UnitCalculationFormulaResultHandler(), schemaConnectionString)
		{
		}
		/*add customized code between this region*/
		/*add customized code between this region*/
	}
}
