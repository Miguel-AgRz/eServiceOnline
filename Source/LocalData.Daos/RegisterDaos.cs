using System;
using MetaShare.Common.Core.Daos;
using Sesi.LocalData.Daos.Interfaces.Legacy;
using Sesi.LocalData.Daos.Legacy;
using Sesi.LocalData.Daos.Interfaces.LocalOperation;
using Sesi.LocalData.Daos.LocalOperation;
using Sesi.LocalData.Daos.Interfaces.PostJobReport;
using Sesi.LocalData.Daos.PostJobReport;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Daos
{
	public class RegisterDaos
	{
		public static void RegisterAll(Type sqlDialect, Type sqlDialectVersion)
		{
			Register(DaoFactory.Instance, true,sqlDialect,sqlDialectVersion);
		}

		public static void UnRegisterAll(Type sqlDialect, Type sqlDialectVersion)
		{
			Register(DaoFactory.Instance, false,sqlDialect,sqlDialectVersion);
		}

		public static void Register(DaoFactory factory, bool isRegister, Type sqlDialect, Type sqlDialectVersion)
		{
			factory.Register(typeof(IDataDao), new DataDao(Activator.CreateInstance(sqlDialect) as SqlDialect), isRegister);
			factory.Register(typeof(IUPLOAD_LOGDao), new UPLOAD_LOGDao(Activator.CreateInstance(sqlDialect) as SqlDialect), isRegister);
			factory.Register(typeof(IJOB_TAGDao), new JOB_TAGDao(Activator.CreateInstance(sqlDialect) as SqlDialect), isRegister);
			factory.Register(typeof(IDC_FLAGSDao), new DC_FLAGSDao(Activator.CreateInstance(sqlDialect) as SqlDialect), isRegister);
			factory.Register(typeof(IWITS_SETTINGDao), new WITS_SETTINGDao(Activator.CreateInstance(sqlDialect) as SqlDialect), isRegister);
			factory.Register(typeof(IESE_FLAGSDao), new ESE_FLAGSDao(Activator.CreateInstance(sqlDialect) as SqlDialect), isRegister);
			factory.Register(typeof(IUploadLogDao), new UploadLogDao(Activator.CreateInstance(sqlDialect) as SqlDialect), isRegister);
			factory.Register(typeof(ISeriesDefinitionDao), new SeriesDefinitionDao(Activator.CreateInstance(sqlDialect) as SqlDialect), isRegister);
			factory.Register(typeof(IJobTagDao), new JobTagDao(Activator.CreateInstance(sqlDialect) as SqlDialect), isRegister);
			factory.Register(typeof(IJobMonitorSettingDao), new JobMonitorSettingDao(Activator.CreateInstance(sqlDialect) as SqlDialect), isRegister);
			factory.Register(typeof(IPrintingSettingDao), new PrintingSettingDao(Activator.CreateInstance(sqlDialect) as SqlDialect), isRegister);
			factory.Register(typeof(IPlcParameterDao), new PlcParameterDao(Activator.CreateInstance(sqlDialect) as SqlDialect), isRegister);
			factory.Register(typeof(IWitsDataDao), new WitsDataDao(Activator.CreateInstance(sqlDialect) as SqlDialect), isRegister);
			factory.Register(typeof(IChartDefinitionDao), new ChartDefinitionDao(Activator.CreateInstance(sqlDialect) as SqlDialect), isRegister);
			factory.Register(typeof(IPlcDataDao), new PlcDataDao(Activator.CreateInstance(sqlDialect) as SqlDialect), isRegister);
			factory.Register(typeof(IDcFlagDao), new DcFlagDao(Activator.CreateInstance(sqlDialect) as SqlDialect), isRegister);
			factory.Register(typeof(IUnitCalculationFormulaDao), new UnitCalculationFormulaDao(Activator.CreateInstance(sqlDialect) as SqlDialect), isRegister);
			factory.Register(typeof(IEseFlagDao), new EseFlagDao(Activator.CreateInstance(sqlDialect) as SqlDialect), isRegister);
			factory.Register(typeof(IPlcDataCalculationFormulaDao), new PlcDataCalculationFormulaDao(Activator.CreateInstance(sqlDialect) as SqlDialect), isRegister);
			factory.Register(typeof(IBlendReportDao), new BlendReportDao(Activator.CreateInstance(sqlDialectVersion) as SqlDialect), isRegister);
			factory.Register(typeof(IPostJobReportDao), new PostJobReportDao(Activator.CreateInstance(sqlDialectVersion) as SqlDialect), isRegister);
			factory.Register(typeof(IStorageInfoDao), new StorageInfoDao(Activator.CreateInstance(sqlDialectVersion) as SqlDialect), isRegister);
			factory.Register(typeof(IBlendConsumptionDao), new BlendConsumptionDao(Activator.CreateInstance(sqlDialectVersion) as SqlDialect), isRegister);
			factory.Register(typeof(IMaintenanceNoteDao), new MaintenanceNoteDao(Activator.CreateInstance(sqlDialectVersion) as SqlDialect), isRegister);
			/*add customized code between this region*/
			/*add customized code between this region*/
		}
	}
}
