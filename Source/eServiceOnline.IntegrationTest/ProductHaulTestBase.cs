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
using Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment;
using Sesi.SanjelData.Entities.Common.BusinessEntities.HumanResources;
using Sesi.SanjelData.Services;
using Sesi.SanjelData.Services.Interfaces.BusinessEntities.Operation.Crew;
using Sesi.SanjelData.Services.Interfaces.BusinessEntities.Operation.Dispatch;
using Sesi.SanjelData.Services.Interfaces.BusinessEntities.Operation.Schedule.ProductSchedule;
using Sesi.SanjelData.Services.Interfaces.Common.BusinessEntities.Equipment;
using Sesi.SanjelData.Services.Interfaces.Common.BusinessEntities.HumanResources;

namespace eServiceOnline.IntegrationTest
{
    public class ProductHaulTestBase
    {
        private RigJobSanjelCrewSection rigJobSanjelCrewSectionTest;
        protected IRigJobSanjelCrewSectionService rigJobSanjelCrewSectionService;
        private IProductHaulService productHaulService;
        private ISanjelCrewService sanjelCrewService;
        private IEmployeeService employeeService;
        private ITruckUnitService truckUnitService;

        private IShippingLoadSheetService shippingLoadSheetService;
        private IBlendUnloadSheetService blendUnloadSheetService;
        private IThirdPartyBulkerCrewService thirdPartyBulkerCrewService;
        protected IRigJobThirdPartyBulkerCrewSectionService rigJobThirdPartyBulkerCrewSectionService;

        protected Employee employee1;
        protected Employee employee2;
        protected Employee employee3;
        protected Employee employee4;
        protected TruckUnit bulkerUnit1;
        protected TruckUnit tractorUnit1;
        protected SanjelCrew bulkerCrew1;
        protected SanjelCrew bulkerCrew2;
        protected ThirdPartyBulkerCrew thridPartyBulkerCrew1;

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
            thirdPartyBulkerCrewService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IThirdPartyBulkerCrewService>();
            if (thirdPartyBulkerCrewService == null) throw new Exception("thirdPartyBulkerCrewService must be registered in service factory");
            PrepareEmployeeData();
            PrepareTruckUnitData();
            EServiceReferenceData.SetReferenceData();
            PrepareBulkerCrew();
            PrepareThirdPartyBulkerCrew();
        }

        [OneTimeTearDown]
        public void TestFixtureTeardown()
        {
            eServiceOnlineGateway.Instance.DeleteCrew(bulkerCrew1.Id);
            truckUnitService.Delete(bulkerUnit1);
            truckUnitService.Delete(tractorUnit1);
            employeeService.Delete(employee1);
            employeeService.Delete(employee2);
            employeeService.Delete(employee3);
            employeeService.Delete(employee4);
            eServiceOnlineGateway.Instance.DeleteThirdPartyBulkerCrew(thridPartyBulkerCrew1.Id);
        }



        private void PrepareEmployeeData()
        {
            employee1 = new Employee() { FirstName = "Employee1", LastName = "Test" };
            employeeService.Insert(employee1);
            employee2 = new Employee() { FirstName = "Employee2", LastName = "Test" };
            employeeService.Insert(employee2);
            employee3 = new Employee() { FirstName = "Employee3", LastName = "Test" };
            employeeService.Insert(employee3);
            employee4 = new Employee() { FirstName = "Employee4", LastName = "Test" };
            employeeService.Insert(employee4);
        }

        private void PrepareTruckUnitData()
        {
            bulkerUnit1 = new TruckUnit() { Name = "846106", UnitNumber = "846106", UnitSubType = new UnitSubType() { Id = 101 } };
            truckUnitService.Insert(bulkerUnit1);
            tractorUnit1 = new TruckUnit() { Name = "946106", UnitNumber = "946106", UnitSubType = new UnitSubType() { Id = 276 } };
            truckUnitService.Insert(tractorUnit1);
        }

        private void PrepareBulkerCrew()
        {
            bulkerCrew1 = new SanjelCrew() { Type = new CrewType() { Id = 2, Name = "Bulker Crew" } };
            bulkerCrew1.SanjelCrewWorkerSection.Add(new SanjelCrewWorkerSection() { Worker = employee1 });
            bulkerCrew1.SanjelCrewTruckUnitSection.Add(new SanjelCrewTruckUnitSection() { TruckUnit = bulkerUnit1 });
            bulkerCrew1.SanjelCrewTruckUnitSection.Add(new SanjelCrewTruckUnitSection() { TruckUnit = tractorUnit1 });
            bulkerCrew1.Name = CrewBoardProcess.BuildCrewName(bulkerCrew1);
            bulkerCrew1.Description = CrewBoardProcess.BuildCrewDescription(bulkerCrew1);
            sanjelCrewService.Insert(bulkerCrew1, true);
        }

        private void PrepareThirdPartyBulkerCrew()
        {
            thridPartyBulkerCrew1 = new ThirdPartyBulkerCrew() { Type = new CrewType() { Id = 3, Name = "Third Party Bulker Crew" } };
            thridPartyBulkerCrew1.Name = "Brett Third Party";
            thridPartyBulkerCrew1.Description = "TEST|Evolve Logistics Inc | Brett";
            thirdPartyBulkerCrewService.Insert(thridPartyBulkerCrew1, true);
        }


    }
}
