using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Timers;
using eServiceOnline.Data;
using eServiceOnline.Gateway;
using eServiceOnline.SecurityControl;
using MetaShare.Common.Core.Daos;
using MetaShare.Common.Core.Daos.SqlServer;
using MetaShare.Common.Core.Proxies;
using Microsoft.AspNetCore.Hosting;
using Sanjel.Common.Daos;
using Sanjel.Common.Security.Daos;
using Sanjel.Common.Security.MockServices;
using Sanjel.Common.Security.Services;
using MetaShare.Common.ServiceModel.Services;
using Sanjel.Common.ApiServices;
using Sanjel.Common.Services;
using Sanjel.EService.Daos;
using Sanjel.Services;
using ConnectionStringBuilder = MetaShare.Common.ServiceModel.Dao.ConnectionStringBuilder;
using DaoFactory = MetaShare.Common.ServiceModel.Dao.DaoFactory;
using RegisterDaos = Sesi.SanjelData.Daos.RegisterDaos;
using RegisterServices = Sesi.SanjelData.Services.RegisterServices;

namespace eServiceOnline
{
    public class Program
    {
        private static bool isUseMocked = false;

        public static void Main(string[] args)
        {
//            isUseMocked = true;

            if (!isUseMocked)
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
            else
            {
                //LocalDataManager.Instance = new MockLocalDataManager();
                //DeviceManager.Instance = new MockDeviceManager();

                DaoFactory.Instance.Clear();

                RegisterSecurityDaos.RegisterAll(DaoFactory.Instance);

                ServiceFactory.Instance.Clear();
                MockSecurityServiceFactory mockSecurityServiceFactory = new MockSecurityServiceFactory();

                mockSecurityServiceFactory.RegisterAll(ServiceFactory.Instance);
            }

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();
            /*
                        EServiceReferenceData.SetReferenceData();

                        string time = ConfigurationManager.AppSettings["timeInterval"];
                        Timer timer = new Timer
                        {
                            Enabled = true,
                            Interval = Convert.ToInt32(time),
                        };
                        timer.Start();
                        timer.Elapsed += SetRefereceData;
            */
            host.Run();
        }

        private static void SetRefereceData(object source, ElapsedEventArgs e)
        {
            EServiceReferenceData.SetReferenceData();
            Debug.Write("update reference");
        }
    }
}