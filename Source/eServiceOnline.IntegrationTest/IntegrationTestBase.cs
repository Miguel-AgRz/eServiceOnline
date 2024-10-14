using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetaShare.Common.Core.Daos.SqlServer;
using MetaShare.Common.ServiceModel.Dao;
using MetaShare.Common.ServiceModel.Services;
using Microsoft.Extensions.DiagnosticAdapter.Internal;
using NUnit.Framework;
using Sanjel.Common.ApiServices;
using Sanjel.Common.Daos;
using Sanjel.Common.Security.Daos;
using Sanjel.Common.Security.Services;
using Sanjel.Common.Services;
using Sanjel.EService.Daos;
using Sesi.SanjelData.Services;
using DaoFactory = MetaShare.Common.ServiceModel.Dao.DaoFactory;
using ProxyFactory =MetaShare.Common.Core.Proxies.ProxyFactory;
using RegisterDaos = Sesi.SanjelData.Daos.RegisterDaos;
using RegisterServices = Sesi.SanjelData.Services.RegisterServices;

namespace eServiceOnline.IntegrationTest
{
    public class IntegrationTestBase
    {
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

        }

        [OneTimeTearDown]
        public void TestFixtureTeardown()
        {

        }
    }
}
