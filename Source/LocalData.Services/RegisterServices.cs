using MetaShare.Common.Core.CommonService;
using Sesi.LocalData.Services.Interfaces.Legacy;
using Sesi.LocalData.Services.Legacy;
using Sesi.LocalData.Services.Interfaces.LocalOperation;
using Sesi.LocalData.Services.LocalOperation;
using Sesi.LocalData.Services.Interfaces.PostJobReport;
using Sesi.LocalData.Services.PostJobReport;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Services
{
	public class RegisterServices
	{
		public static void RegisterAll()
		{
			Register(ServiceFactory.Instance, true);
		}

		public static void UnRegisterAll()
		{
			Register(ServiceFactory.Instance, false);
		}

		public static void Register(ServiceFactory factory, bool isRegister)
		{
			/*
			factory.Register(typeof(IDataService), new DataService(), isRegister);
			factory.Register(typeof(IUPLOAD_LOGService), new UPLOAD_LOGService(), isRegister);
			factory.Register(typeof(IJOB_TAGService), new JOB_TAGService(), isRegister);
			factory.Register(typeof(IDC_FLAGSService), new DC_FLAGSService(), isRegister);
			factory.Register(typeof(IWITS_SETTINGService), new WITS_SETTINGService(), isRegister);
			factory.Register(typeof(IESE_FLAGSService), new ESE_FLAGSService(), isRegister);
			factory.Register(typeof(IUploadLogService), new UploadLogService(), isRegister);
			factory.Register(typeof(ISeriesDefinitionService), new SeriesDefinitionService(), isRegister);
			factory.Register(typeof(IJobTagService), new JobTagService(), isRegister);
			factory.Register(typeof(IJobMonitorSettingService), new JobMonitorSettingService(), isRegister);
			factory.Register(typeof(IPrintingSettingService), new PrintingSettingService(), isRegister);
			factory.Register(typeof(IPlcParameterService), new PlcParameterService(), isRegister);
			factory.Register(typeof(IWitsDataService), new WitsDataService(), isRegister);
			factory.Register(typeof(IChartDefinitionService), new ChartDefinitionService(), isRegister);
			factory.Register(typeof(IPlcDataService), new PlcDataService(), isRegister);
			factory.Register(typeof(IDcFlagService), new DcFlagService(), isRegister);
			factory.Register(typeof(IUnitCalculationFormulaService), new UnitCalculationFormulaService(), isRegister);
			factory.Register(typeof(IEseFlagService), new EseFlagService(), isRegister);
			factory.Register(typeof(IPlcDataCalculationFormulaService), new PlcDataCalculationFormulaService(), isRegister);
			*/
			factory.Register(typeof(IBlendReportService), new BlendReportService(), isRegister);
			factory.Register(typeof(IPostJobReportService), new PostJobReportService(), isRegister);
			factory.Register(typeof(IStorageInfoService), new StorageInfoService(), isRegister);
			factory.Register(typeof(IBlendConsumptionService), new BlendConsumptionService(), isRegister);
			factory.Register(typeof(IMaintenanceNoteService), new MaintenanceNoteService(), isRegister);
			/*add customized code between this region*/
			/*add customized code between this region*/
		}
	}
}
