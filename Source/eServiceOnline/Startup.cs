using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using MetaShare.Common.Core.Daos;
using MetaShare.Common.Core.Daos.SqlServer;
using MetaShare.Logging.Diagnostics;
using MetaShare.Logging.MicrosoftExtensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.DotNet.PlatformAbstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.DependencyModel.Resolution;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using sesi.SanjelLibrary.NotficationLibrary;
using sesi.SanjelLibrary.NotficationLibrary.Config;
using SanjelLogger= MetaShare.Logging.ILogger;
using Microsoft.Extensions.Options;
using sesi.SanjelLibrary.NotficationLibrary.Extensions;

namespace eServiceOnline
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();

            LoggingFactory.RegisterAll(new LoggingFactoryOptions
            {
                Configuration = this.Configuration,
                SectionName = "CustomLogging"
            });
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            // Add framework services.
            ApplicationPartManager applicationPartManager = new ApplicationPartManager();
            applicationPartManager.ApplicationParts.Add(new AssemblyPart(typeof(Startup).Assembly));
            services.AddSingleton(applicationPartManager);

            services.AddMvc().AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

            //            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddMemoryCache();
            // Add framework services.
            services.AddSession();
/*
            var mvcBuilder = services.AddMvc();
            new MvcConfiguration().ConfigureMvc(mvcBuilder);
*/
            services.AddLogging();
            services.AddTransient<ILogger>(s => s.GetService<ILogger<Program>>());
            //Dec 19, 2023 zhangyuan 239_PR_Notification: Register NotificationsService
            RegisterNotificationsService.RegisterAll();
            MetaShare.Common.Core.CommonService.ServiceFactory.Instance.AddNotificationsService(Configuration.GetSection(NotificationsOptions.Section));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // Register Syncfusion License
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NTAzMDkyQDMxMzgyZTMzMmUzMG1OSFhBcEV5UFJtNndXRjJWc3VRNlgrZnc5UmZXVE5ubWR1VGxEVXBHS009;NTAzMDkzQDMxMzgyZTMzMmUzMFJlTllEUFU1d000YTArS0tKOHpxaVNlNW1oNWNPYXorcnE2VjAwNncwcE09;NTAzMDk0QDMxMzgyZTMzMmUzMEl5aU9oaTNzTUF2YS81ZWYyZEVqVDBIZ1FsQmFiaTNpU1E1dGFacUpnRkE9;NTAzMDk1QDMxMzgyZTMzMmUzMEhkazZ5RXg0K3lwdXJVdGIwZXdtNlZlQ1VDUi9SWFk0MlBXMnA0N1BqaW89;NTAzMDk2QDMxMzgyZTMzMmUzMEMzSkJpRHFqcE9MSmJ2dUNZOFlPbEh1YkNua3lMU2xvLzRIVlNnWnh1Yk09;NTAzMDk3QDMxMzgyZTMzMmUzMGFUUTd1SHNoRjluUUR2UXRXTjJJU2FvRnJGcVgwWStXQnZDS2xQNjBJVkE9");

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            SanjelLogger logger = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<MetaShare.Logging.ILogger>();
            loggerFactory.AddMetaShareLogger(logger);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=RigBoard}/{action=Index}/{id?}");
            });
        }
    }


    public class MvcConfiguration : IDesignTimeMvcBuilderConfiguration
    {
        public void ConfigureMvc(IMvcBuilder builder)
        {
            // .NET Core SDK v1 does not pick up reference assemblies so
            // they have to be added for Razor manually. Resolved for
            // SDK v2 by https://github.com/dotnet/sdk/pull/876  OR SO WE THOUGHT
            /*builder.AddRazorOptions(razor =>
            {
            razor.AdditionalCompilationReferences.Add(
            MetadataReference.CreateFromFile(
            typeof(PdfHttpHandler).Assembly.Location));
            });*/

            // .NET Core SDK v2 does not resolve reference assemblies‘ paths
            // at all, so we have to hack around with reflection
            typeof(CompilationLibrary)
                .GetTypeInfo()
                .GetDeclaredField("<DefaultResolver>k__BackingField")
                .SetValue(null, new CompositeCompilationAssemblyResolver(new ICompilationAssemblyResolver[]
                {
                    new DirectReferenceAssemblyResolver(),
                    new AppBaseCompilationAssemblyResolver(),
                    new ReferenceAssemblyPathResolver(),
                    new PackageCompilationAssemblyResolver()
                }));
        }

        private class DirectReferenceAssemblyResolver : ICompilationAssemblyResolver
        {
            public bool TryResolveAssemblyPaths(CompilationLibrary library, List<string> assemblies)
            {
                if (!string.Equals(library.Type, "reference", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                var paths = new List<string>();

                foreach (var assembly in library.Assemblies)
                {
                    var path = Path.Combine(ApplicationEnvironment.ApplicationBasePath, assembly);

                    if (!File.Exists(path))
                    {
                        return false;
                    }

                    paths.Add(path);
                }

                assemblies.AddRange(paths);

                return true;
            }
        }
    }
}
