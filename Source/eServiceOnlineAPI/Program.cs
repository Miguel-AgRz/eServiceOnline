using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using MetaShare.Common.Core.Daos.SqlServer;
using MetaShare.Common.Core.Proxies;
using MetaShare.Common.ServiceModel.Dao;
using MetaShare.Common.ServiceModel.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sanjel.Common.ApiServices;
//using Sanjel.Common.Daos;
//using Sanjel.Common.Security.Daos;
//using Sanjel.Common.Security.MockServices;
//using Sanjel.Common.Security.Services;
//using Sanjel.Common.Services;
using Sesi.SanjelData.Daos;
using Sesi.SanjelData.Services;

namespace eServiceOnlineAPI
{
    public class Program
    {
        private static bool isUseMocked = false;
        public static void Main(string[] args)
        {
            
            if (!isUseMocked)
            {
//                DaoFactory.Instance.Clear();
//                RegisterCommonDaos.RegisterAll(DaoFactory.Instance);
//                Sanjel.EService.Daos.RegisterDaos.RegisterAll(DaoFactory.Instance);

 //               ServiceFactory.Instance.Clear();
//                RegisterCommonServices.RegisterAll(ServiceFactory.Instance);
//                Sanjel.Services.RegisterServices.RegisterAll(ServiceFactory.Instance);
//                Sanjel.EService.MicroService.RegisterServices.RegisterAll(ServiceFactory.Instance);

//                RegisterApiProxies.RegisterProxies(ProxyFactory.Instance);

                string connectionString = ConfigurationManager.ConnectionStrings["SanjelData"].ConnectionString;
                MetaShare.Common.Core.Daos.DaoFactory.Instance.ConnectionStringBuilder = new MetaShare.Common.Core.Daos.ConnectionStringBuilder(connectionString, typeof(MetaShare.Common.Core.Daos.SqlContext)) { SqlDialectType = typeof(SqlServerDialect), SqlDialectVersionType = typeof(SqlServerDialectVersion) };
                RegisterDaos.RegisterAll(MetaShare.Common.Core.Daos.DaoFactory.Instance.ConnectionStringBuilder.SqlDialectType, MetaShare.Common.Core.Daos.DaoFactory.Instance.ConnectionStringBuilder.SqlDialectVersionType);
                RegisterServices.RegisterAll();

            }
            else
            {
                //LocalDataManager.Instance = new MockLocalDataManager();
                //DeviceManager.Instance = new MockDeviceManager();

//                DaoFactory.Instance.Clear();

//                RegisterSecurityDaos.RegisterAll(DaoFactory.Instance);

//                ServiceFactory.Instance.Clear();
//                MockSecurityServiceFactory mockSecurityServiceFactory = new MockSecurityServiceFactory();

//                mockSecurityServiceFactory.RegisterAll(ServiceFactory.Instance);
            }
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
