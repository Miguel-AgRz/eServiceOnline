using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using eServiceOnline.Controllers;
using eServiceOnline.Data;
using eServiceOnline.Gateway;
using eServiceOnline.Models.ProductHaul;
using MetaShare.Common.Core.Daos.SqlServer;
using MetaShare.Common.Core.Proxies;
using MetaShare.Common.Foundation.Versioning;
using MetaShare.Common.ServiceModel.Dao;
using MetaShare.Common.ServiceModel.Services;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Sanjel.Common.ApiServices;
using Sanjel.Common.BusinessEntities.Lookup;
using Sanjel.Common.Daos;
using Sanjel.Common.Security.Daos;
using Sanjel.Common.Security.Services;
using Sanjel.Common.Services;
using Sesi.SanjelData.Daos;
using Sesi.SanjelData.Entities.BusinessEntities.BaseEntites;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Crew;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ResourceSchedule;
using Sesi.SanjelData.Entities.BusinessEntities.WellSite;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Organization;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Products;
using Sesi.SanjelData.Services;
using Sesi.SanjelData.Services.Interfaces.BusinessEntities.Operation.Crew;
using Sesi.SanjelData.Services.Interfaces.BusinessEntities.Operation.Dispatch;
using Sesi.SanjelData.Services.Interfaces.BusinessEntities.WellSite;
using Sesi.SanjelData.Services.Interfaces.Common.BusinessEntities.Equipment;
using Sesi.SanjelData.Services.Interfaces.Common.BusinessEntities.HumanResources;
using Sesi.SanjelData.Services.Interfaces.Common.BusinessEntities.WellSite;
using Rig = Sesi.SanjelData.Entities.Common.BusinessEntities.WellSite.Rig;

namespace eServiceOnline.IntegrationTest
{
    public class OnlocationProductHaulIntegrationWithDRBTestForBlend : IntegrationTestBase
    {

        private string loggedUser = "awang";

        private int randomNumber = 0;

        private DateTime onDateTimeTime;

        private List<SanjelCrew> listCrews = null;
        private List<ThirdPartyBulkerCrew> listThirdPartyBulkerCrew = null;

        private IRigService rigService;
        private IRigJobService bigJobService;
        private IBinService binService;
        private IBinInformationService binInformationService;

        private RigJob rigJob = null;
        private Rig rig = null;
        private List<BinInformation> binInformations = null;
        private CallSheet callSheet = null;
        private BlendSection blendSection = null;
        [SetUp]
        public void Setup()
        {
            #region Sustainment Data
            ISanjelCrewService sanjelCrewService  = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ISanjelCrewService>();
            if (sanjelCrewService == null) throw new Exception("sanjelCrewService must be registered in service factory");
            
            rigService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigService>();
            if (rigService == null) throw new Exception("rigService must be registered in service factory");

            binService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IBinService>();
            if (binService == null) throw new Exception("binService must be registered in service factory");
            
            binInformationService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IBinInformationService>();
            if (binInformationService == null) throw new Exception("binInformationService must be registered in service factory");

            bigJobService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigJobService>();
            if (bigJobService == null) throw new Exception("bigJobService must be registered in service factory");

            //Prepare the common Blend Section data for this test case
            PrepareBlendSectionData();

            //Prepare the common CallSheet data for this test case
            PrepareCallSheetData();

            //Prepare the common RigJob data for this test case
            PrepareRigJobData();

            #endregion
        }

        //Prepare the common RigJob data for this test case
        private void PrepareRigJobData()
        {
            Random random = new Random();

            //Get Rig information from cache for creating RigJob array
            List<Rig> listRigs = CacheData.Rigs.ToList();
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

        //Create SanjelCrew information and BulkerCrewLog information
        private int PrepareSanjelCrewData(string name)
        {
            listCrews = new List<SanjelCrew>();

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

        //Create ThirdPartyBulkerCrew information and BulkerCrewLog information
        private int PrepareThirdPartyBulkerCrewData(string name)
        {
            ThirdPartyBulkerCrew thirdPartyBulkerCrew = new ThirdPartyBulkerCrew();

            CrewType crewType = new CrewType();

            crewType.Id = 2;

            thirdPartyBulkerCrew.Name = name;
            thirdPartyBulkerCrew.Type = crewType;

            BulkerCrewLog bulkerCrewLog = new BulkerCrewLog();

            bulkerCrewLog.ThirdPartyBulkerCrew= thirdPartyBulkerCrew;

            bulkerCrewLog.CrewStatus = BulkerCrewStatus.None;

            eServiceOnlineGateway.Instance.CreateThirdPartyBulkerCrew(thirdPartyBulkerCrew);

            eServiceOnlineGateway.Instance.CreateBulkerCrewLog(bulkerCrewLog);

            listThirdPartyBulkerCrew.Add(thirdPartyBulkerCrew);

            return thirdPartyBulkerCrew.Id;
        }

        //Prepare the common CallSheet data for this test case
        private void PrepareCallSheetData()
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

        //Prepare the common Blend Section data for this test case
        private void PrepareBlendSectionData()
        {

            Random random = new Random();

            List<CallSheetBlendSection> listBlendSections = eServiceOnlineGateway.Instance.SelectAllBlendSection();

            listBlendSections= this.GetBlendSectionsGroupByFilter(listBlendSections, new string[] { "Lead 1", "Lead 2", "Lead 3", "Lead 4", "Tail", "Plug" });

            randomNumber = random.Next(0, listBlendSections.Count);

            blendSection = listBlendSections[randomNumber];


        }

        private List<CallSheetBlendSection> GetBlendSectionsGroupByFilter(List<CallSheetBlendSection> data, string[] filters)
        {
            List<CallSheetBlendSection> result = new List<CallSheetBlendSection>();
            if (data?.FirstOrDefault(t => filters.Contains(t.BlendCategory.Name)) != null)
                result.AddRange(data.Where(t => filters.Contains(t.BlendCategory.Name)));
            return result.OrderBy(person => filters.ToList().IndexOf(person.BlendCategory.Name)).ToList();
        }

        #region Create a Schedule based on different preconditions
        private ScheduleProductHaulFromRigJobBlendViewModel PreparingBasicData(bool IsGowithCrew, bool IsThird, int crewId)
        {
            Random random = new Random();

            onDateTimeTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy/MM/dd"));

            ScheduleProductHaulFromRigJobBlendViewModel model = new ScheduleProductHaulFromRigJobBlendViewModel();

            model.ProductLoadInfoModel.CallSheetNumber = callSheet.CallSheetNumber;
            model.RigJobId =rigJob.Id;
            model.ProductLoadInfoModel.RigId = rig.Id;
            model.ProductLoadInfoModel.MixWater =0.02;

            model.ShippingLoadSheetModel.RigName = rig.Name;
            model.ShippingLoadSheetModel.RigId = rig.Id;

            List<PodLoad> podLoadModels = new List<PodLoad>();
            for (var i = 0; i < 4; i++)
            {
                podLoadModels.Add(new PodLoad()
                {
                    PodIndex = i,
                    LoadAmount = 0
                }); ;
            }

            model.PodLoadModels = podLoadModels;

            List<Rig> listRigs = eServiceOnlineGateway.Instance.GetBulkPlants();

            randomNumber = random.Next(0, listRigs.Count);


            var BulkPlantId = listRigs[randomNumber].Id;
            var BulkPlantName = listRigs[randomNumber].Name;

            model.ProductLoadInfoModel.BulkPlantId = BulkPlantId;
            model.ProductLoadInfoModel.BulkPlantName = BulkPlantName;

            model.ProductLoadInfoModel.RigId = rigJob.Rig.Id;
            model.ProductLoadInfoModel.RigName = rigJob.Rig.Name;

            model.ProductLoadInfoModel.Amount = 1;

            model.ProductLoadInfoModel.BaseBlendSectionId = blendSection.Id;

            model.ProductHaulInfoModel.IsGoWithCrew = IsGowithCrew;

            model.ProductHaulInfoModel.BulkPlantId = BulkPlantId;

            if (IsThird)
            {
                model.ProductHaulInfoModel.ThirdPartyBulkerCrewId = crewId;

                model.ProductHaulInfoModel.IsThirdParty = true;
            }
            else
            {
                model.ProductHaulInfoModel.CrewId = crewId;
            }

            model.PodLoadModels[0].LoadAmount = 1;

            model.ProductHaulInfoModel.IsGoWithCrew = IsGowithCrew;

            model.ProductHaulInfoModel.BulkPlantId = BulkPlantId;

            if (!IsGowithCrew)
            {
                model.ProductHaulInfoModel.EstimatedTravelTime = 4;
                model.ProductHaulInfoModel.ExpectedOnLocationTime = onDateTimeTime.AddHours(6);

                List<BlendUnloadSheet> blendUnloadSheetModels = new List<BlendUnloadSheet>();

                foreach (var item in binInformations)
                {
                    blendUnloadSheetModels.Add(new BlendUnloadSheet()
                    {
                        UnloadAmount = 0,
                        DestinationStorage = item
                    });
                }

                model.BlendUnloadSheetModels = blendUnloadSheetModels;

                if (model.BlendUnloadSheetModels.Count > 0)
                {
                    model.BlendUnloadSheetModels[0].UnloadAmount = 1;
                }
            }
            else
            {
                model.ProductHaulInfoModel.ExpectedOnLocationTime = onDateTimeTime;
                model.ProductHaulInfoModel.EstimatedLoadTime = onDateTimeTime.AddHours(6);
                model.ProductHaulInfoModel.EstimatedTravelTime = 4;

            }

            return model;
        }

        public void ScheduleRigJobBlendProductHaul(bool IsGowithCrew, bool IsThird, int crewId)
        {
            eServiceWebContext _context = new eServiceWebContext();

            ScheduleProductHaulFromRigJobBlendViewModel model = PreparingBasicData(IsGowithCrew, IsThird, crewId);

            if (model == null) throw new Exception("There is no data available for testing in the current database");

            model.LoggedUser = loggedUser;

            _context.ScheduleProductHaul(model);
        }


        private ScheduleProductHaulFromRigJobBlendViewModel PreparingBasicDataByExistingHaul(bool IsThird, int crewId,int existingHaulId)
        {
            Random random = new Random();

            onDateTimeTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy/MM/dd"));

            ScheduleProductHaulFromRigJobBlendViewModel model = new ScheduleProductHaulFromRigJobBlendViewModel();

            model.ProductLoadInfoModel.CallSheetNumber = callSheet.CallSheetNumber;
            model.RigJobId = rigJob.Id;
            model.ProductLoadInfoModel.RigId = rig.Id;
            model.ProductLoadInfoModel.MixWater = 0.02;

            model.ShippingLoadSheetModel.RigName = rig.Name;
            model.ShippingLoadSheetModel.RigId = rig.Id;

            List<PodLoad> podLoadModels = new List<PodLoad>();
            for (var i = 0; i < 4; i++)
            {
                podLoadModels.Add(new PodLoad()
                {
                    PodIndex = i,
                    LoadAmount = 0
                }); ;
            }

            model.PodLoadModels = podLoadModels;

            List<Rig> listRigs = eServiceOnlineGateway.Instance.GetBulkPlants();

            randomNumber = random.Next(0, listRigs.Count);


            var BulkPlantId = listRigs[randomNumber].Id;
            var BulkPlantName = listRigs[randomNumber].Name;

            model.ProductLoadInfoModel.BulkPlantId = BulkPlantId;
            model.ProductLoadInfoModel.BulkPlantName = BulkPlantName;

            model.ProductLoadInfoModel.RigId = rigJob.Rig.Id;
            model.ProductLoadInfoModel.RigName = rigJob.Rig.Name;

            model.ProductLoadInfoModel.Amount = 1;

            model.ProductLoadInfoModel.BaseBlendSectionId = blendSection.Id;

            model.ProductHaulInfoModel.IsGoWithCrew = false;

            model.ProductHaulInfoModel.BulkPlantId = BulkPlantId;

            if (IsThird)
            {
                model.ProductHaulInfoModel.ThirdPartyBulkerCrewId = crewId;

                model.ProductHaulInfoModel.IsThirdParty = true;
            }
            else
            {
                model.ProductHaulInfoModel.CrewId = crewId;
            }

            model.PodLoadModels[0].LoadAmount = 1;

            model.ProductHaulInfoModel.IsGoWithCrew = false;

            model.ProductHaulInfoModel.BulkPlantId = BulkPlantId;

            ProductHaul existingHaul = eServiceOnlineGateway.Instance.GetProductHaulById(existingHaulId);

            model.ProductHaulInfoModel.IsExistingHaul = true;
            model.ProductHaulInfoModel.ProductHaulId = existingHaul.Id;
            model.ProductHaulInfoModel.BulkPlantId = existingHaul.BulkPlant.Id;
            model.ProductHaulInfoModel.ExpectedOnLocationTime = existingHaul.ExpectedOnLocationTime;
            model.ProductHaulInfoModel.EstimatedTravelTime = existingHaul.EstimatedTravelTime;
            model.ProductHaulInfoModel.EstimatedLoadTime = existingHaul.EstimatedLoadTime;
            model.ProductHaulInfoModel.CrewId = 0;

            int existproductHaulId = existingHaul.Id;

            List<PodLoad> listPodLoads = eServiceOnlineGateway.Instance.GetPodLoadsByProductHaul(existingHaul.Id);


            if (listPodLoads == null || listPodLoads.Count == 0)
            {
                model.PodLoadModels[0].LoadAmount = 1;
            }
            else
            {
                for (int n = 0; n < 4; n++)
                {
                    if (listPodLoads[n].LoadAmount == 0)
                    {
                        model.PodLoadModels[n].LoadAmount = 1;
                    }
                    else
                    {
                        model.PodLoadModels[n].LoadAmount = listPodLoads[n].LoadAmount;
                    }
                }
            }


            return model;
        }

        public void ScheduleRigJobBlendProductHaulByExist(bool IsThird, int crewId, int existingHaulId)
        {
            eServiceWebContext _context = new eServiceWebContext();

            ScheduleProductHaulFromRigJobBlendViewModel model = PreparingBasicDataByExistingHaul(IsThird, crewId, existingHaulId);

            if (model == null) throw new Exception("There is no data available for testing in the current database");

            model.LoggedUser = loggedUser;

            _context.ScheduleProductHaul(model);
        }
        #endregion

        public void OnlocationScheduleRigJobBlendProductHaul(int productHaulId)
        {
            eServiceWebContext _context = new eServiceWebContext();

            OnLocationProductHaulViewModel model = new OnLocationProductHaulViewModel();

            model.ProductHaulId = productHaulId;

            model.OnLocationTime = DateTime.Now;
            model.LoggedUser = loggedUser;


            _context.UpdateProductHaulOnLocation(model);
        }

        [Test]
        public void TestOnlocationProductHaulbyScheduledLoadRequested()
        {
            //Create separate Crew information for this test
            int crewId = PrepareSanjelCrewData("OnlocationSL");

            //Check whether the pre-data meets the conditions
            Assert.NotNull(callSheet);
            Assert.NotNull(blendSection);
            Assert.NotNull(rig);
            Assert.NotNull(rigJob);
            Assert.NotNull(binInformations);
            Assert.AreNotEqual(binInformations, 0);
            Assert.AreNotEqual(crewId, 0);

            eServiceWebContext _context = new eServiceWebContext();

            //Update the status of BulkerCrewLog
            BulkerCrewLog bulkerCrewLog = eServiceOnlineGateway.Instance.GetBulkerCrewLog(crewId);
            bulkerCrewLog.CrewStatus = BulkerCrewStatus.OffDuty;
            eServiceOnlineGateway.Instance.UpdateBulkerCrewLog(bulkerCrewLog);

            //Scheduled a ProductHaul and check if the creation was successful
            ScheduleRigJobBlendProductHaul(false, false, crewId);
            ProductHaul productHaul = eServiceOnlineGateway.Instance.GetProductHaulByCrew(crewId).FirstOrDefault();
            Assert.NotNull(productHaul);
            Assert.AreNotEqual(productHaul.Id, 0);

            //Get the scheduled data to prepare for later comparison
            bulkerCrewLog = eServiceOnlineGateway.Instance.GetBulkerCrewLog(crewId);
            RigJobSanjelCrewSection sanjelCrewSection = eServiceOnlineGateway.Instance.GetRigJobCrewSectionsByProductHaul(productHaul.Id);
            SanjelCrewSchedule sanjelCrewSchedule = eServiceOnlineGateway.Instance.GetSanjelCrewSchedulesByRigJobSanjelCrewSection(sanjelCrewSection);


            OnlocationScheduleRigJobBlendProductHaul(productHaul.Id);

            //After  onlocation a ProductHaul, check whether the associated table object is created successfully
            List<ShippingLoadSheet> shippingLoadSheets = eServiceOnlineGateway.Instance.GetShippingLoadSheetByProductHaulId(productHaul.Id);


            foreach (var shippingLoadSheet in shippingLoadSheets)
            {
                Assert.AreEqual(shippingLoadSheet.ShippingStatus, ShippingStatus.OnLocation);

                ProductHaulLoad productHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(shippingLoadSheet.ProductHaulLoad.Id);
                Assert.NotNull(productHaulLoad);
                Assert.AreEqual(productHaulLoad.ProductHaulLoadLifeStatus, ProductHaulLoadStatus.OnLocation);

                List<BlendUnloadSheet> blendUnloadSheets = eServiceOnlineGateway.Instance.GetBlendUnloadSheetByshippingLoadSheetId(shippingLoadSheet.Id);
                Assert.AreNotEqual(blendUnloadSheets.Count, 0);

            }

            List<PodLoad> podLoads = eServiceOnlineGateway.Instance.GetPodLoadsByProductHaul(productHaul.Id);
            Assert.AreNotEqual(podLoads.Count, 0);


            //After onlocation a ProductHaul, check whether the object RigJobSanjelCrewSection is created and whether the state change meets the requirements
            RigJobSanjelCrewSection currentSanjelCrewSection = eServiceOnlineGateway.Instance.GetRigJobCrewSectionsByProductHaul(productHaul.Id);
            Assert.NotNull(currentSanjelCrewSection);
            Assert.AreEqual(currentSanjelCrewSection.RigJobCrewSectionStatus, RigJobCrewSectionStatus.OnLocation);

            //After onlocation a ProductHaul, check whether the object BulkerCrewLog whether the state change meets the requirements
            BulkerCrewLog currentBulkerCrewLog = eServiceOnlineGateway.Instance.GetBulkerCrewLog(crewId);
            Assert.NotNull(bulkerCrewLog);
            Assert.NotNull(currentBulkerCrewLog);
            Assert.AreNotEqual(bulkerCrewLog.CrewStatus, currentBulkerCrewLog.CrewStatus);
            Assert.AreNotEqual(currentBulkerCrewLog.CrewStatus, BulkerCrewStatus.LoadRequested);
            Assert.AreEqual(currentBulkerCrewLog.CrewStatus, BulkerCrewStatus.OnLocation);
        }

        [Test]
        public void TestOnlocationProductHaulbyScheduledCalled()
        {
            //Create separate Crew information for this test
            int crewId = PrepareSanjelCrewData("OnlocationSC");

            //Check whether the pre-data meets the conditions
            Assert.NotNull(callSheet);
            Assert.NotNull(blendSection);
            Assert.NotNull(rig);
            Assert.NotNull(rigJob);
            Assert.NotNull(binInformations);
            Assert.AreNotEqual(binInformations, 0);
            Assert.AreNotEqual(crewId, 0);

            eServiceWebContext _context = new eServiceWebContext();

            //Update the status of BulkerCrewLog
            BulkerCrewLog bulkerCrewLog = eServiceOnlineGateway.Instance.GetBulkerCrewLog(crewId);
            bulkerCrewLog.CrewStatus = BulkerCrewStatus.OffDuty;
            eServiceOnlineGateway.Instance.UpdateBulkerCrewLog(bulkerCrewLog);

            //Scheduled a ProductHaul and check if the creation was successful
            ScheduleRigJobBlendProductHaul(false, false, crewId);
            ProductHaul productHaul = eServiceOnlineGateway.Instance.GetProductHaulByCrew(crewId).FirstOrDefault();
            Assert.NotNull(productHaul);
            Assert.AreNotEqual(productHaul.Id, 0);

            //Get the scheduled data to prepare for later comparison
            bulkerCrewLog = eServiceOnlineGateway.Instance.GetBulkerCrewLog(crewId);
            RigJobSanjelCrewSection sanjelCrewSection = eServiceOnlineGateway.Instance.GetRigJobCrewSectionsByProductHaul(productHaul.Id);
            SanjelCrewSchedule sanjelCrewSchedule = eServiceOnlineGateway.Instance.GetSanjelCrewSchedulesByRigJobSanjelCrewSection(sanjelCrewSection);

            bulkerCrewLog.CrewStatus = BulkerCrewStatus.Called;
            eServiceOnlineGateway.Instance.UpdateBulkerCrewLog(bulkerCrewLog);


            OnlocationScheduleRigJobBlendProductHaul(productHaul.Id);

            //After  onlocation a ProductHaul, check whether the associated table object is created successfully
            List<ShippingLoadSheet> shippingLoadSheets = eServiceOnlineGateway.Instance.GetShippingLoadSheetByProductHaulId(productHaul.Id);


            foreach (var shippingLoadSheet in shippingLoadSheets)
            {
                Assert.AreEqual(shippingLoadSheet.ShippingStatus, ShippingStatus.OnLocation);

                ProductHaulLoad productHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(shippingLoadSheet.ProductHaulLoad.Id);
                Assert.NotNull(productHaulLoad);
                Assert.AreEqual(productHaulLoad.ProductHaulLoadLifeStatus, ProductHaulLoadStatus.OnLocation);

                List<BlendUnloadSheet> blendUnloadSheets = eServiceOnlineGateway.Instance.GetBlendUnloadSheetByshippingLoadSheetId(shippingLoadSheet.Id);
                Assert.AreNotEqual(blendUnloadSheets.Count, 0);

            }

            List<PodLoad> podLoads = eServiceOnlineGateway.Instance.GetPodLoadsByProductHaul(productHaul.Id);
            Assert.AreNotEqual(podLoads.Count, 0);


            //After onlocation a ProductHaul, check whether the object RigJobSanjelCrewSection is created and whether the state change meets the requirements
            RigJobSanjelCrewSection currentSanjelCrewSection = eServiceOnlineGateway.Instance.GetRigJobCrewSectionsByProductHaul(productHaul.Id);
            Assert.NotNull(currentSanjelCrewSection);
            Assert.AreEqual(currentSanjelCrewSection.RigJobCrewSectionStatus, RigJobCrewSectionStatus.OnLocation);

            //After onlocation a ProductHaul, check whether the object BulkerCrewLog whether the state change meets the requirements
            BulkerCrewLog currentBulkerCrewLog = eServiceOnlineGateway.Instance.GetBulkerCrewLog(crewId);
            Assert.NotNull(bulkerCrewLog);
            Assert.NotNull(currentBulkerCrewLog);

            Assert.AreNotEqual(bulkerCrewLog.CrewStatus, currentBulkerCrewLog.CrewStatus);
            Assert.AreNotEqual(currentBulkerCrewLog.CrewStatus, BulkerCrewStatus.Called);
            Assert.AreEqual(currentBulkerCrewLog.CrewStatus, BulkerCrewStatus.OnLocation);
        }
    }
}