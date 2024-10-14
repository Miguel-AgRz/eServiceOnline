using System;
using System.Data;
using MetaShare.Common.Core.Daos;
using Sesi.LocalData.Daos.Interfaces.LocalOperation;
using Sesi.LocalData.Entities.LocalOperation;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Daos.LocalOperation
{
	public class SeriesDefinitionDao : CommonObjectDao<SeriesDefinition>, ISeriesDefinitionDao
	{
		public class SeriesDefinitionSqlBuilder : ObjectSqlBuilder
		{
			public SeriesDefinitionSqlBuilder(SqlDialect sqlDialect) : base(sqlDialect,"SeriesDefinition")
			{
				this.SqlInsert = "INSERT INTO SeriesDefinition (IsEnabled,PlcDataCalculationFormulaName,Color,ParameterName,Title,ActionName,PlcDataCalculationFormulaid,ParameterDescription,IsSecondaryYAxis,UnitCalculationFormulaid,UnitCalculationFormulaName,ControllerName,Parameterid,PlcDataCalculationFormulaDescription,TruckUnitNumber,UnitCalculationFormulaDescription,IsTemplate,Chartid,YAxisName,MaxValue," + this.SqlBaseFieldInsertFront + ") VALUES (@IsEnabled,@PlcDataCalculationFormulaName,@Color,@ParameterName,@Title,@ActionName,@PlcDataCalculationFormulaid,@ParameterDescription,@IsSecondaryYAxis,@UnitCalculationFormulaid,@UnitCalculationFormulaName,@ControllerName,@Parameterid,@PlcDataCalculationFormulaDescription,@TruckUnitNumber,@UnitCalculationFormulaDescription,@IsTemplate,@Chartid,@YAxisName,@MaxValue," + this.SqlBaseFieldInsertBack + ")";
				this.SqlUpdate = "UPDATE SeriesDefinition SET IsEnabled=@IsEnabled,PlcDataCalculationFormulaName=@PlcDataCalculationFormulaName,Color=@Color,ParameterName=@ParameterName,Title=@Title,ActionName=@ActionName,PlcDataCalculationFormulaid=@PlcDataCalculationFormulaid,ParameterDescription=@ParameterDescription,IsSecondaryYAxis=@IsSecondaryYAxis,UnitCalculationFormulaid=@UnitCalculationFormulaid,UnitCalculationFormulaName=@UnitCalculationFormulaName,ControllerName=@ControllerName,Parameterid=@Parameterid,PlcDataCalculationFormulaDescription=@PlcDataCalculationFormulaDescription,TruckUnitNumber=@TruckUnitNumber,UnitCalculationFormulaDescription=@UnitCalculationFormulaDescription,IsTemplate=@IsTemplate,Chartid=@Chartid,YAxisName=@YAxisName,MaxValue=@MaxValue," + this.SqlBaseFieldUpdate + " WHERE Id=@Id";
			}
		}

		public class SeriesDefinitionResultHandler : CommonObjectResultHandler<SeriesDefinition>
		{
			public override void GetColumnValues(IDataReader reader, SeriesDefinition item)
			{
				base.GetColumnValues(reader, item);
				int ordinalPlcDataCalculationFormulaId = reader.GetOrdinal("PlcDataCalculationFormulaId");
				int ordinalPlcDataCalculationFormulaName = reader.GetOrdinal("PlcDataCalculationFormulaName");
				string plcDataCalculationFormulaName= reader.IsDBNull(ordinalPlcDataCalculationFormulaName) ? null :reader.GetString(ordinalPlcDataCalculationFormulaName);
				int ordinalPlcDataCalculationFormulaDescription = reader.GetOrdinal("PlcDataCalculationFormulaDescription");
				string plcDataCalculationFormulaDescription= reader.IsDBNull(ordinalPlcDataCalculationFormulaDescription) ? null:reader.GetString(ordinalPlcDataCalculationFormulaDescription);
				item.PlcDataCalculationFormula = reader.IsDBNull(ordinalPlcDataCalculationFormulaId) ? null :reader.GetInt32(ordinalPlcDataCalculationFormulaId)==0?null:new PlcDataCalculationFormula { Id=reader.GetInt32(ordinalPlcDataCalculationFormulaId), Name = plcDataCalculationFormulaName, Description = plcDataCalculationFormulaDescription};
				int ordinalTitle = reader.GetOrdinal("Title");
				item.Title = reader.IsDBNull(ordinalTitle) ? null : reader.GetString(ordinalTitle);
				int ordinalYAxisName = reader.GetOrdinal("YAxisName");
				item.YAxisName = reader.IsDBNull(ordinalYAxisName) ? null : reader.GetString(ordinalYAxisName);
				int ordinalParameterId = reader.GetOrdinal("ParameterId");
				int ordinalParameterName = reader.GetOrdinal("ParameterName");
				string parameterName= reader.IsDBNull(ordinalParameterName) ? null :reader.GetString(ordinalParameterName);
				int ordinalParameterDescription = reader.GetOrdinal("ParameterDescription");
				string parameterDescription= reader.IsDBNull(ordinalParameterDescription) ? null:reader.GetString(ordinalParameterDescription);
				item.Parameter = reader.IsDBNull(ordinalParameterId) ? null :reader.GetInt32(ordinalParameterId)==0?null:new PlcParameter { Id=reader.GetInt32(ordinalParameterId), Name = parameterName, Description = parameterDescription};
				int ordinalUnitCalculationFormulaId = reader.GetOrdinal("UnitCalculationFormulaId");
				int ordinalUnitCalculationFormulaName = reader.GetOrdinal("UnitCalculationFormulaName");
				string unitCalculationFormulaName= reader.IsDBNull(ordinalUnitCalculationFormulaName) ? null :reader.GetString(ordinalUnitCalculationFormulaName);
				int ordinalUnitCalculationFormulaDescription = reader.GetOrdinal("UnitCalculationFormulaDescription");
				string unitCalculationFormulaDescription= reader.IsDBNull(ordinalUnitCalculationFormulaDescription) ? null:reader.GetString(ordinalUnitCalculationFormulaDescription);
				item.UnitCalculationFormula = reader.IsDBNull(ordinalUnitCalculationFormulaId) ? null :reader.GetInt32(ordinalUnitCalculationFormulaId)==0?null:new UnitCalculationFormula { Id=reader.GetInt32(ordinalUnitCalculationFormulaId), Name = unitCalculationFormulaName, Description = unitCalculationFormulaDescription};
				int ordinalChartId = reader.GetOrdinal("ChartId");
				item.Chart = reader.IsDBNull(ordinalChartId) ? null :reader.GetInt32(ordinalChartId)==0?null:new ChartDefinition { Id=reader.GetInt32(ordinalChartId)};
				int ordinalActionName = reader.GetOrdinal("ActionName");
				item.ActionName = reader.IsDBNull(ordinalActionName) ? null : reader.GetString(ordinalActionName);
				int ordinalTruckUnitNumber = reader.GetOrdinal("TruckUnitNumber");
				item.TruckUnitNumber = reader.IsDBNull(ordinalTruckUnitNumber) ? null : reader.GetString(ordinalTruckUnitNumber);
				int ordinalColor = reader.GetOrdinal("Color");
				item.Color = reader.IsDBNull(ordinalColor) ? null : reader.GetString(ordinalColor);
				int ordinalControllerName = reader.GetOrdinal("ControllerName");
				item.ControllerName = reader.IsDBNull(ordinalControllerName) ? null : reader.GetString(ordinalControllerName);
				int ordinalIsEnabled = reader.GetOrdinal("IsEnabled");
				item.IsEnabled = !reader.IsDBNull(ordinalIsEnabled) && reader.GetBoolean(ordinalIsEnabled);
				int ordinalIsTemplate = reader.GetOrdinal("IsTemplate");
				item.IsTemplate = !reader.IsDBNull(ordinalIsTemplate) && reader.GetBoolean(ordinalIsTemplate);
				int ordinalIsSecondaryYAxis = reader.GetOrdinal("IsSecondaryYAxis");
				item.IsSecondaryYAxis = !reader.IsDBNull(ordinalIsSecondaryYAxis) && reader.GetBoolean(ordinalIsSecondaryYAxis);
				int ordinalMaxValue = reader.GetOrdinal("MaxValue");
				item.MaxValue =  reader.IsDBNull(ordinalMaxValue) ? 0 : reader.GetDouble(ordinalMaxValue);
				/*add customized code between this region*/
				/*add customized code between this region*/
			}

			public override void AddInsertParameters(IContext context, IDbCommand command, SeriesDefinition item)
			{
				base.AddInsertParameters(context, command, item);
				context.AddParameter(command, "PlcDataCalculationFormulaId", item.PlcDataCalculationFormula ==null? 0:item.PlcDataCalculationFormula.Id);
                context.AddParameter(command, "PlcDataCalculationFormulaName", item.PlcDataCalculationFormula ==null?(object)DBNull.Value : string.IsNullOrEmpty(item.PlcDataCalculationFormula.Name) ? (object)DBNull.Value : item.PlcDataCalculationFormula.Name);
                context.AddParameter(command, "PlcDataCalculationFormulaDescription", item.PlcDataCalculationFormula ==null? (object)DBNull.Value : string.IsNullOrEmpty(item.PlcDataCalculationFormula.Description) ? (object)DBNull.Value : item.PlcDataCalculationFormula.Description);

				context.AddParameter(command, "Title", item.Title ?? (object) DBNull.Value);
				context.AddParameter(command, "YAxisName", item.YAxisName ?? (object) DBNull.Value);
				context.AddParameter(command, "ParameterId", item.Parameter ==null? 0:item.Parameter.Id);
                context.AddParameter(command, "ParameterName", item.Parameter ==null?(object)DBNull.Value : string.IsNullOrEmpty(item.Parameter.Name) ? (object)DBNull.Value : item.Parameter.Name);
                context.AddParameter(command, "ParameterDescription", item.Parameter ==null? (object)DBNull.Value : string.IsNullOrEmpty(item.Parameter.Description) ? (object)DBNull.Value : item.Parameter.Description);

				context.AddParameter(command, "UnitCalculationFormulaId", item.UnitCalculationFormula ==null? 0:item.UnitCalculationFormula.Id);
                context.AddParameter(command, "UnitCalculationFormulaName", item.UnitCalculationFormula ==null?(object)DBNull.Value : string.IsNullOrEmpty(item.UnitCalculationFormula.Name) ? (object)DBNull.Value : item.UnitCalculationFormula.Name);
                context.AddParameter(command, "UnitCalculationFormulaDescription", item.UnitCalculationFormula ==null? (object)DBNull.Value : string.IsNullOrEmpty(item.UnitCalculationFormula.Description) ? (object)DBNull.Value : item.UnitCalculationFormula.Description);

				context.AddParameter(command, "ChartId", item.Chart ==null? 0:item.Chart.Id);

				context.AddParameter(command, "ActionName", item.ActionName ?? (object) DBNull.Value);
				context.AddParameter(command, "TruckUnitNumber", item.TruckUnitNumber ?? (object) DBNull.Value);
				context.AddParameter(command, "Color", item.Color ?? (object) DBNull.Value);
				context.AddParameter(command, "ControllerName", item.ControllerName ?? (object) DBNull.Value);
				context.AddParameter(command, "IsEnabled", item.IsEnabled);
				context.AddParameter(command, "IsTemplate", item.IsTemplate);
				context.AddParameter(command, "IsSecondaryYAxis", item.IsSecondaryYAxis);
				context.AddParameter(command, "MaxValue", item.MaxValue);
				/*add customized code between this region*/
				/*add customized code between this region*/
			}
		}

		public SeriesDefinitionDao(SqlDialect sqlDialect) : base(new SeriesDefinitionSqlBuilder(sqlDialect), new SeriesDefinitionResultHandler())
		{
		}

		public SeriesDefinitionDao(SqlDialect sqlDialect, string schemaConnectionString) : base(new SeriesDefinitionSqlBuilder(sqlDialect), new SeriesDefinitionResultHandler(), schemaConnectionString)
		{
		}
		/*add customized code between this region*/
		/*add customized code between this region*/
	}
}
