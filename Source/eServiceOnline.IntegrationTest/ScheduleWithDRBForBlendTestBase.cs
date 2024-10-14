using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eServiceOnline.BusinessProcess;
using eServiceOnline.Data;
using eServiceOnline.Gateway;
using eServiceOnline.Models.ProductHaul;
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
using Sesi.SanjelData.Entities.BusinessEntities.BaseEntites;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Crew;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule;
using Sesi.SanjelData.Entities.BusinessEntities.WellSite;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment;
using Sesi.SanjelData.Entities.Common.BusinessEntities.HumanResources;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Operation;
using Sesi.SanjelData.Entities.Common.BusinessEntities.WellSite;
using Sesi.SanjelData.Services;
using Sesi.SanjelData.Services.Interfaces.BusinessEntities.Operation.Crew;
using Sesi.SanjelData.Services.Interfaces.BusinessEntities.Operation.Dispatch;
using Sesi.SanjelData.Services.Interfaces.BusinessEntities.Operation.Schedule.ProductSchedule;
using Sesi.SanjelData.Services.Interfaces.Common.BusinessEntities.Equipment;
using Sesi.SanjelData.Services.Interfaces.Common.BusinessEntities.HumanResources;

namespace eServiceOnline.IntegrationTest
{
    public class ScheduleWithDRBForBlendTestBase
    {
        private ISanjelCrewService sanjelCrewService;
        private IRigJobService bigJobService;

        public List<SanjelCrew> listCrews = new List<SanjelCrew>();
        public List<ThirdPartyBulkerCrew> listThirdPartyBulkerCrew = new List<ThirdPartyBulkerCrew>();
        public List<int> listProductHaulId = new List<int>();

        public RigJob rigJob = null;
        public Rig rig = null;
        public List<BinInformation> binInformations = null;
        public CallSheet callSheet = null;
        public BlendSection blendSection = null;

        private int randomNumber = 0;

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


            ISanjelCrewService sanjelCrewService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ISanjelCrewService>();
            if (sanjelCrewService == null) throw new Exception("sanjelCrewService must be registered in service factory");

            bigJobService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigJobService>();
            if (bigJobService == null) throw new Exception("bigJobService must be registered in service factory");

            PrepareBlendSectionData();

            PrepareCallSheetData();

            PrepareRigJobData();

        }

        //Prepare the common Blend Section data for this test case
        public void PrepareBlendSectionData()
        {

            Random random = new Random();

            List<CallSheetBlendSection> listBlendSections = eServiceOnlineGateway.Instance.SelectAllBlendSection();

            listBlendSections = this.GetBlendSectionsGroupByFilter(listBlendSections, new string[] { "Lead 1", "Lead 2", "Lead 3", "Lead 4", "Tail", "Plug" });

            randomNumber = random.Next(0, listBlendSections.Count);

            blendSection = listBlendSections[randomNumber];

        }

        //Prepare the common CallSheet data for this test case
        public void PrepareCallSheetData()
        {
            Random random = new Random();

            int callSheetId = blendSection.OwnerId;

            List<CallSheet> listCallSheets = eServiceOnlineGateway.Instance.SelecCallSheetById(callSheetId);

            if (listCallSheets == null || listCallSheets.Count == 0)
            {
                callSheet = null;
            }
            else
            {
                randomNumber = random.Next(0, listCallSheets.Count);

                callSheet = listCallSheets[randomNumber];
            }
        }


        //Prepare the common RigJob data for this test case
        private void PrepareRigJobData()
        {
            Random random = new Random();

            //Get Rig information from cache for creating RigJob array
            List<Rig> listRigs = CacheData.Rigs.Where(p => p.OperationSiteType == OperationSiteType.Rig).ToList();
            randomNumber = random.Next(0, listRigs.Count);
            rig = listRigs[randomNumber];


            List<BinInformation> listBins = CacheData.BinInformations.ToList();
            binInformations = new List<BinInformation>();
            binInformations.Add(listBins[0]);
            binInformations.Add(listBins[1]);

            //Create the RigJob information used in this test case
            rigJob = new RigJob();
            rigJob.Rig = rig;
            rigJob.CallCrewTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy/MM/dd")).AddHours(8);
            rigJob.JobDateTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy/MM/dd")).AddHours(12);
            bigJobService.Insert(rigJob);
        }

        //Prepare the SanjelCrew data for this test case
        public int PrepareSanjelCrewData(string name)
        {
            SanjelCrew sanjelCrew = new SanjelCrew();

            CrewType crewType = new CrewType();

            crewType.Id = 2;

            sanjelCrew.Name = name;

            sanjelCrew.Type = crewType;

            BulkerCrewLog bulkerCrewLog = new BulkerCrewLog();

            bulkerCrewLog.SanjelCrew = sanjelCrew;

            bulkerCrewLog.CrewStatus = BulkerCrewStatus.None;

            eServiceOnlineGateway.Instance.CreateCrew(sanjelCrew);

            eServiceOnlineGateway.Instance.CreateBulkerCrewLog(bulkerCrewLog);

            listCrews.Add(sanjelCrew);

            return sanjelCrew.Id;
        }


        //Prepare the SanjelCrew data for this test case
        public int PrepareSanjelCrewDataWithOutBulkerCrewLog(string name)
        {
            SanjelCrew sanjelCrew = new SanjelCrew();

            CrewType crewType = new CrewType();

            crewType.Id = 2;

            sanjelCrew.Name = name;

            sanjelCrew.Type = crewType;

            eServiceOnlineGateway.Instance.CreateCrew(sanjelCrew);

            listCrews.Add(sanjelCrew);

            return sanjelCrew.Id;
        }


        //Prepare the SanjelCrew data for this test case
        public int PrepareThirdPartyBulkerCrewData(string name)
        {
            ThirdPartyBulkerCrew thirdPartyBulkerCrew = new ThirdPartyBulkerCrew();

            CrewType crewType = new CrewType();

            crewType.Id = 2;

            thirdPartyBulkerCrew.Name = name;

            thirdPartyBulkerCrew.Type = crewType;

            BulkerCrewLog bulkerCrewLog = new BulkerCrewLog();

            bulkerCrewLog.ThirdPartyBulkerCrew = thirdPartyBulkerCrew;

            bulkerCrewLog.CrewStatus = BulkerCrewStatus.None;

            eServiceOnlineGateway.Instance.CreateThirdPartyBulkerCrew(thirdPartyBulkerCrew);

            eServiceOnlineGateway.Instance.CreateBulkerCrewLog(bulkerCrewLog);

            listThirdPartyBulkerCrew.Add(thirdPartyBulkerCrew);

            return thirdPartyBulkerCrew.Id;
        }


        public List<CallSheetBlendSection> GetBlendSectionsGroupByFilter(List<CallSheetBlendSection> data, string[] filters)
        {
            List<CallSheetBlendSection> result = new List<CallSheetBlendSection>();
            if (data?.FirstOrDefault(t => filters.Contains(t.BlendCategory.Name)) != null)
                result.AddRange(data.Where(t => filters.Contains(t.BlendCategory.Name)));
            return result.OrderBy(person => filters.ToList().IndexOf(person.BlendCategory.Name)).ToList();
        }

        //Prepare the SanjelCrew data for this test case
        public void DeleteProductHaul(int productHaulId, bool isDeleteShipingLoadSheet)
        {
            CancelProductViewModel model = new CancelProductViewModel();

            model.ProductHaulId = productHaulId;

            var productHaul = eServiceOnlineGateway.Instance.GetProductHaulById(model.ProductHaulId);

            List<CheckShipingLoadSheetModel> CheckShipingLoadSheetModels = new List<CheckShipingLoadSheetModel>();

            foreach (ShippingLoadSheet shippingLoadSheet in productHaul.ShippingLoadSheets)
            {
                var productHualLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(shippingLoadSheet.ProductHaulLoad.Id);
                List<int> ids = new List<int>();
                ids.Add(productHualLoad.Id);
                var shippingLoadSheets = eServiceOnlineGateway.Instance.GetShippingLoadSheetByProductHaulLoads(ids);
                CheckShipingLoadSheetModels.Add(new CheckShipingLoadSheetModel()
                {
                    IsChecked = isDeleteShipingLoadSheet,
                    IsReadOnly = shippingLoadSheets.Count > 1 ? true : false,
                    ShippingLoadSheetModel = shippingLoadSheet,
                    ProductHaulLoadModel = productHualLoad
                });
            }
            model.CheckShipingLoadSheetModels = CheckShipingLoadSheetModels;

            eServiceWebContext _context = new eServiceWebContext();

            _context.DeleteProductHaul(model);
        }


        [OneTimeTearDown]
        public void TestFixtureTeardown()
        {
            //Clean up the productHaul data created during the test to complete the test
            //(the productHaul data that remains when the test process is interrupted and the cleanup is not performed correctly)
            foreach (var productHaulId in listProductHaulId)
            {
                var ProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);

                if (ProductHaul != null)
                {
                    DeleteProductHaul(ProductHaul.Id, true);
                }
            }

            //Clean up the sanjel crew data created during the test to complete the test
            foreach (SanjelCrew crew in listCrews)
            {
                if (crew != null)
                {
                    BulkerCrewLog bulkerCrewLog = eServiceOnlineGateway.Instance.GetBulkerCrewLog(crew.Id);

                    if (bulkerCrewLog != null)
                    {
                        eServiceOnlineGateway.Instance.DeleteBulkerCrewLog(bulkerCrewLog);
                    }

                    eServiceOnlineGateway.Instance.DeleteCrew(crew.Id);
                }
            }

            //Clean up the thirdParty bulkercrew  data created during the test to complete the test
            foreach (ThirdPartyBulkerCrew crew in listThirdPartyBulkerCrew)
            {
                if (crew != null)
                {
                    BulkerCrewLog bulkerCrewLog = eServiceOnlineGateway.Instance.GetThirdPartyBulkerCrewLog(crew.Id);

                    if (bulkerCrewLog != null)
                    {
                        eServiceOnlineGateway.Instance.DeleteBulkerCrewLog(bulkerCrewLog);
                    }

                    eServiceOnlineGateway.Instance.DeleteThirdPartyBulkerCrew(crew.Id);
                }
            }

            //Clean up the rigJob  data created during the test to complete the test
            if (rigJob != null)
            {
                eServiceOnlineGateway.Instance.DeleteRigJob(rigJob);
            }


        }
    }
}
