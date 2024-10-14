using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eServiceOnline.BusinessProcess;
using eServiceOnline.Gateway;
using MetaShare.Common.Core.Daos.SqlServer;
using MetaShare.Common.Core.Proxies;
using MetaShare.Common.ServiceModel.Dao;
using MetaShare.Common.ServiceModel.Services;
using NUnit.Framework;
using Sanjel.Common.ApiServices;
using Sanjel.Common.Daos;
using Sanjel.Common.Security.Daos;
using Sanjel.Common.Security.Services;
using Sanjel.Common.Services;
using Sesi.SanjelData.Daos;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Crew;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch;
using Sesi.SanjelData.Entities.BusinessEntities.WellSite;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment;
using Sesi.SanjelData.Entities.Common.BusinessEntities.HumanResources;
using Sesi.SanjelData.Entities.Common.BusinessEntities.WellSite;
using Sesi.SanjelData.Services;
using Sesi.SanjelData.Services.Interfaces.BusinessEntities.Operation.Crew;
using Sesi.SanjelData.Services.Interfaces.BusinessEntities.Operation.Dispatch;
using Sesi.SanjelData.Services.Interfaces.BusinessEntities.Operation.Schedule.ProductSchedule;
using Sesi.SanjelData.Services.Interfaces.BusinessEntities.WellSite;
using Sesi.SanjelData.Services.Interfaces.Common.BusinessEntities.Equipment;
using Sesi.SanjelData.Services.Interfaces.Common.BusinessEntities.HumanResources;
using Sesi.SanjelData.Services.Interfaces.Common.BusinessEntities.WellSite;

namespace eServiceOnline.IntegrationTest
{
    public class ScheduleWithDRBForBinTestBase
    {
        protected ISanjelCrewService sanjelCrewService;
        private IEmployeeService employeeService;
        private ITruckUnitService truckUnitService;

        private IRigJobService rigJobService;
        private IRigService rigService;
        protected IThirdPartyBulkerCrewService thirdPartyBulkerCrewService;
        private IBinInformationService binInformationService;
        protected BinInformation binInformation = null;
        protected RigJob rigJob = null;
        protected Rig rig = null;
        protected Employee employee1;

        protected TruckUnit bulkerUnit1;
        protected TruckUnit tractorUnit1;
        protected SanjelCrew bulkerCrew1;
        protected SanjelCrew bulkerCrew2;
        protected ThirdPartyBulkerCrew thridPartyBulkerCrew1;
        protected ThirdPartyBulkerCrew thridPartyBulkerCrew2;
        protected BulkerCrewLog bulkerCrewLog;
        protected BulkerCrewLog bulkerCrew2Log;
        protected BulkerCrewLog thirdPartyCrewLog;
        protected BulkerCrewLog thirdPartyCrew2Log;

        [OneTimeSetUp]
        public void TestFixtureSetup()
        {
            DaoFactory.Instance.Clear();
            RegisterSecurityDaos.RegisterAll(DaoFactory.Instance);
            RegisterCommonDaos.RegisterAll(DaoFactory.Instance);
            Sanjel.EService.Daos.RegisterDaos.RegisterAll(DaoFactory.Instance);

            ServiceFactory.Instance.Clear();
            RegisterSecurityServices.RegisterAll(ServiceFactory.Instance);
            RegisterCommonServices.RegisterAll(ServiceFactory.Instance);
            Sanjel.Services.RegisterServices.RegisterAll(ServiceFactory.Instance);
            Sanjel.EService.MicroService.RegisterServices.RegisterAll(ServiceFactory.Instance);

            RegisterApiProxies.RegisterProxies(ProxyFactory.Instance);

            string connectionString = ConfigurationManager.ConnectionStrings["SanjelData"].ConnectionString;
            MetaShare.Common.Core.Daos.DaoFactory.Instance.ConnectionStringBuilder = new MetaShare.Common.Core.Daos.ConnectionStringBuilder(connectionString, typeof(MetaShare.Common.Core.Daos.SqlContext)) { SqlDialectType = typeof(SqlServerDialect), SqlDialectVersionType = typeof(SqlServerDialectVersion) };
            RegisterDaos.RegisterAll(MetaShare.Common.Core.Daos.DaoFactory.Instance.ConnectionStringBuilder.SqlDialectType, MetaShare.Common.Core.Daos.DaoFactory.Instance.ConnectionStringBuilder.SqlDialectVersionType);
            RegisterServices.RegisterAll();

            employeeService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IEmployeeService>();
            if (employeeService == null) throw new Exception("employeeService must be registered in service factory");
            truckUnitService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ITruckUnitService>();
            if (truckUnitService == null) throw new Exception("truckUnitService must be registered in service factory");
            sanjelCrewService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ISanjelCrewService>();
            if (sanjelCrewService == null) throw new Exception("crewService must be registered in service factory");

            rigJobService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigJobService>();
            if (rigJobService == null) throw new Exception("rigJobService must be registered in service factory");
            binInformationService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IBinInformationService>();
            if (binInformationService == null) throw new Exception("binInformationService must be registered in service factory");
            rigService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigService>();
            if (rigService == null) throw new Exception("rigService must be registered in service factory");
            thirdPartyBulkerCrewService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IThirdPartyBulkerCrewService>();
            if (thirdPartyBulkerCrewService == null) throw new Exception("thirdPartyBulkerCrewService must be registered in service factory");

            PrepareTruckUnitData();
            PrepareEmployeeData();
            EServiceReferenceData.SetReferenceData();
            rig = PrepareRig();
            binInformation = PrepareBinInfomation();
            rigJob = PrepareRigJob();
        }

        [OneTimeTearDown]
        public void TestFixtureTeardown()
        {
            truckUnitService.Delete(bulkerUnit1);
            truckUnitService.Delete(tractorUnit1);
            employeeService.Delete(employee1);
            rigService.Delete(rig);
            binInformationService.Delete(binInformation);
            rigJobService.Delete(rigJob);

        }

        private void PrepareEmployeeData()
        {
            employee1 = new Employee() { FirstName = "Employee1", LastName = "Test" };
            employeeService.Insert(employee1);
        }

        private void PrepareTruckUnitData()
        {
            bulkerUnit1 = new TruckUnit() { Name = "846106", UnitNumber = "846106", UnitSubType = new UnitSubType() { Id = 101 } };
            truckUnitService.Insert(bulkerUnit1);
            tractorUnit1 = new TruckUnit() { Name = "946106", UnitNumber = "946106", UnitSubType = new UnitSubType() { Id = 276 } };
            truckUnitService.Insert(tractorUnit1);
        }

        private BinInformation PrepareBinInfomation()
        {

            BinInformation binInformation = new BinInformation()
            {
                Rig = rig,
                BinStatus = BinStatus.Assigned,
                Bin = new Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment.Bin() { Id = 140, Name = "2023" },
            };
            binInformationService.Insert(binInformation);
            return binInformation;
        }
        private Rig PrepareRig()
        {
            var rig = new Rig() { Name = "Sanjel Rig", ModifiedUserName = "Test", Id = 2363 };
            rigService.Insert(rig);
            return rig;

        }

        private RigJob PrepareRigJob()
        {
            RigJob rigJob = new RigJob()
            {
                Rig = rig,
                RigStatus = Sesi.SanjelData.Entities.Common.BusinessEntities.WellSite.RigStatus.None,
                ClientCompanyShortName = "Test0621",
                JobLifeStatus = JobLifeStatus.Confirmed,
                IsListed = true,
                JobType = new Sesi.SanjelData.Entities.Common.BusinessEntities.Operation.JobType() { Id = 1503, Name = "Whipstock Plug" },
                CallCrewTime = DateTime.Now.AddHours(8),
                JobDateTime = DateTime.Now.AddHours(12)

            };
            rigJobService.Insert(rigJob);
            return rigJob;

        }
    }
}
