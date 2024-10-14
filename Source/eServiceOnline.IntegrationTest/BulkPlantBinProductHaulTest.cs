using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using eServiceOnline.Controllers;
using eServiceOnline.Data;
using eServiceOnline.Gateway;
using eServiceOnline.Models.ProductHaul;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Sesi.SanjelData.Entities.BusinessEntities.BaseEntites;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ResourceSchedule;
using Sesi.SanjelData.Entities.BusinessEntities.WellSite;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Products;
using Sesi.SanjelData.Services.Interfaces.BusinessEntities.Operation.Dispatch;

namespace eServiceOnline.IntegrationTest
{
    [TestFixture]
    public class BulkPlantBinProductHaulTest : IntegrationTestBase
    {
        protected int callSheetNumber = 0;
        protected int callSheetId = 0;
        protected int callSheetBlendSectionId = 0;
        protected int productHaulId = 0;
        protected int productHaulLoadId = 0;
        protected RigJob rigJob = null;
        protected CallSheetBlendSection blendSection = null;
        protected int crewId = 3005;
        private int rigJobId = 67461;
        private int binId = 442;
        private string programId = "PRG2200386";
        private string programRev = "00";
        private int rigId = 2349;
        private string loggedUser = "awang";
        private int binInformationId = 2967;



        [SetUp]
        public void Setup()
        {
            #region Sustainment Data

            var pendingJobs =
                eServiceOnlineGateway.Instance.GetRigJobsByQuery(null, p => p.JobLifeStatus == JobLifeStatus.Pending)
                    .Where(i => i.Id == rigJobId).FirstOrDefault();

            bool foundIt = false;


            if (pendingJobs != null)
            {

                var availableBlends = eServiceOnlineGateway.Instance
                    .GetBlendSectionsByCallSheetAndBlend(pendingJobs.CallSheetNumber).ToList();


                if (availableBlends != null)
                {


                    foreach (var callSheetBlendSection in availableBlends)
                    {
                        if (callSheetBlendSection.BlendCategory.Name == "Lead 1")
                        {
                            callSheetBlendSectionId = callSheetBlendSection.Id;
                            callSheetNumber = pendingJobs.CallSheetNumber;
                            callSheetId = pendingJobs.CallSheetId;
                            blendSection = callSheetBlendSection;
                            foundIt = true;
                            break;
                        }
                    }
                }
            }

            if (foundIt)
            {
                rigJob = pendingJobs;
            }

            #endregion

        }

        public void CreateScheduleBlendRequest(double amount)
        {

            var binInformation = eServiceWebContext.Instance.GetBinInformationById(binInformationId);

            ProductLoadInfoModel model = new ProductLoadInfoModel();

            model.BulkPlantId = rigId;
            model.BinId = binInformation.Bin.Id;
            model.PodIndex = binInformation.PodIndex;

            model.BulkPlantId = rigId;
            model.BulkPlantName = "EST Bulk Plant";
            model.RigId = rigId;
            model.BinNumber = "1652";
            model.BinInformationId = binInformationId;
            model.BinId = 0;
            model.BinInformationName = "Silo 3";
            model.ProgramNumber = programId + "." + programRev;
            model.CustomerId = 1513;
            model.JobTypeId = 1504;
            model.BaseBlendSectionId = 101435;
            model.MixWater = 0.99;
            model.BulkPlantId = rigId;
            model.PodIndex = 2;
            model.Amount = amount;
            model.LoggedUser = loggedUser;
            model.ClientName = "Candian Natural Resources Ltd.";

            model.Comments = "Schedule Blend Request";
            model.ClientRepresentative = "";
            model.ServicePointName = "";


            eServiceWebContext _context = new eServiceWebContext();

            _context.CreateBlendRequest(model);

            var productHaulLoad =
                eServiceOnlineGateway.Instance.GetProductHaulLoadByQuery(p => p.ProgramId == programId)
                    .OrderByDescending(p => p.Id).First();

            if (productHaulLoad != null)
            {
                productHaulLoadId = productHaulLoad.Id;
            }

        }

        public void CancelScheduleBlendRequest(int productHaulLoadId)
        {
            eServiceWebContext _context = new eServiceWebContext();

            _context.DeleteProductHaulLoadById(productHaulLoadId, loggedUser);

        }


        public void CreateHualBlendFromRigBulkPlantBin(double amount)
        {
            ProductHaulLoad productHaulLoad =
                eServiceOnlineGateway.Instance.GetProductHaulLoadById(productHaulLoadId, true);

            BinInformation binInformation = eServiceOnlineGateway.Instance.GetBinInformationById(binInformationId);

            ProductLoadInfoModel productLoadInfoModel = new ProductLoadInfoModel();

            productLoadInfoModel.ProductHaulLoadId = productHaulLoad.Id;
            productLoadInfoModel.CallSheetNumber = productHaulLoad.CallSheetNumber;
            productLoadInfoModel.BaseBlendSectionId = productHaulLoad.BlendSectionId;
            BlendChemical blendChemical = productHaulLoad.BlendChemical;
            productLoadInfoModel.BaseBlend = blendChemical == null
                ? ""
                : (((string.IsNullOrWhiteSpace(blendChemical.Description)
                    ? blendChemical.Name
                    : blendChemical.Description)).Split('+').FirstOrDefault());
            productLoadInfoModel.Category = productHaulLoad.BlendCategory == null
                ? ""
                : (string.IsNullOrEmpty(productHaulLoad.BlendCategory.Description)
                    ? productHaulLoad.BlendCategory.Name
                    : productHaulLoad.BlendCategory.Description);
            productLoadInfoModel.CategoryId =
                productHaulLoad.BlendCategory == null ? 0 : productHaulLoad.BlendCategory.Id;
            productLoadInfoModel.MixWater = productHaulLoad.MixWater;
            productLoadInfoModel.Amount = productHaulLoad.TotalBlendWeight / 1000;
            productLoadInfoModel.TotalBlendWeight = productHaulLoad.TotalBlendWeight / 1000;
            productLoadInfoModel.BaseBlendWeight = productHaulLoad.BaseBlendWeight / 1000;
            productLoadInfoModel.BlendChemicalDescription = productHaulLoad.BlendChemical?.Description;
            productLoadInfoModel.IsTotalBlendTonnage = productHaulLoad.IsTotalBlendTonnage;
            productLoadInfoModel.OnLocationTime = productHaulLoad.OnLocationTime;


            productLoadInfoModel.ProgramNumber = productHaulLoad.ProgramId;

            productLoadInfoModel.BaseBlendUnit =
                (productHaulLoad.Unit == null || string.IsNullOrEmpty(productHaulLoad.Unit.Description))
                    ? "t"
                    : productHaulLoad.Unit.Description;
            productLoadInfoModel.IsBlendTest = productHaulLoad.IsBlendTest;
            productLoadInfoModel.RigName = productHaulLoad.Rig?.Name;
            productLoadInfoModel.RigId = productHaulLoad.Rig == null ? 0 : productHaulLoad.Rig.Id;
            productLoadInfoModel.ClientName = productHaulLoad.Customer?.Name;
            productLoadInfoModel.RemainsAmount = Math.Round(productHaulLoad.RemainsAmount / 1000, 2);
            productLoadInfoModel.PodIndex = productHaulLoad.PodIndex;
            productLoadInfoModel.ClientRepresentative = productHaulLoad.ClientRepresentative;
            if (productHaulLoad.Bin != null)
            {
                productLoadInfoModel.BinId = productHaulLoad.Bin.Id;
                productLoadInfoModel.BinNumber = productHaulLoad.Bin.Name;
            }

            if (productHaulLoad.BulkPlant != null && productHaulLoad.BulkPlant.Id > 0)
            {
                productLoadInfoModel.BulkPlantId = productHaulLoad.BulkPlant.Id;
                productLoadInfoModel.BulkPlantName = productHaulLoad.BulkPlant.Name;
            }

            if (productHaulLoad.JobType != null && productHaulLoad.JobType.Id > 0)
            {
                productLoadInfoModel.JobTypeId = productHaulLoad.JobType.Id;
                productLoadInfoModel.JobTypeName = productHaulLoad.JobType.Name;
            }

            if (productHaulLoad.ServicePoint != null && productHaulLoad.ServicePoint.Id > 0)
            {
                productLoadInfoModel.ServicePointId = productHaulLoad.ServicePoint.Id;
                productLoadInfoModel.ServicePointName = productHaulLoad.ServicePoint.Name;
            }

            productLoadInfoModel.Amount = productLoadInfoModel.TotalBlendWeight;

            if (binInformation != null)
            {
                productLoadInfoModel.BinInformationName = binInformation.Name;
            }

            ProductHaulInfoModel productHaulInfoModel = new ProductHaulInfoModel();

            productHaulInfoModel.InitializeProductHaulSchedule();

            var binInformations = eServiceOnlineGateway.Instance.GetBinInformationsByRigId(rigJob.Id);

            ShippingLoadSheetModel shippingLoadSheet = new ShippingLoadSheetModel();

            shippingLoadSheet.InitializeShippingLoadSheets(productHaulLoad.Rig, binInformations);

            List<PodLoad> podLoads = new List<PodLoad>();
            podLoads.Add(new PodLoad() { LoadAmount = 1, PodIndex = 0 });
            podLoads.Add(new PodLoad() { LoadAmount = 1, PodIndex = 1 });
            podLoads.Add(new PodLoad() { LoadAmount = 0, PodIndex = 2 });
            podLoads.Add(new PodLoad() { LoadAmount = 0, PodIndex = 3 });

            productHaulInfoModel.PodLoadModels = podLoads;

            List<BlendUnloadSheet> blendUnloadSheets = new List<BlendUnloadSheet>();

            var bins = eServiceOnlineGateway.Instance.GetBinInformationsByRigId(rigJob.Rig.Id);

            double binAmount = 0;

            if (amount > 0)
            {
                binAmount = 2;
            }

            foreach (var binInfo in bins)
            {
                blendUnloadSheets.Add(new BlendUnloadSheet()
                    { DestinationStorage = binInfo, UnloadAmount = binAmount == 0 ? 0 : binAmount });

                if (binAmount > 0)
                {
                    binAmount--;
                }
            }


            shippingLoadSheet.BlendUnloadSheetModels = blendUnloadSheets;

            double loadAmount = 4;

            productHaulInfoModel.CrewId = 3055;
            productHaulInfoModel.EstimatedLoadTime = DateTime.Now.AddDays(2);
            productHaulInfoModel.EstimatedTravelTime = 48;
            productHaulInfoModel.ExpectedOnLocationTime = DateTime.Now.AddDays(2);
            shippingLoadSheet.LoadAmount = loadAmount;
            productLoadInfoModel.Amount = loadAmount;


            eServiceWebContext.Instance.CreateHaulBlend(rigJobId, "awang", productLoadInfoModel, productHaulInfoModel,
                shippingLoadSheet);



            if (productHaulLoad != null)
            {
                productHaulLoadId = productHaulLoad.Id;

                var vsshippingLoadSheet =
                    eServiceOnlineGateway.Instance
                        .GetShippingLoadSheetByProductHaulLoads(new List<int>() { productHaulLoad.Id }).First();

                if (vsshippingLoadSheet != null)
                {
                    productHaulId = vsshippingLoadSheet.ProductHaul.Id;
                }
            }

        }


        public void CancelProductHaulFromRigBulkPlantBin()
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
                    IsChecked = false,
                    IsReadOnly = shippingLoadSheets.Count > 1 ? true : false,
                    ShippingLoadSheetModel = shippingLoadSheet,
                    ProductHaulLoadModel = productHualLoad
                });
            }
            model.CheckShipingLoadSheetModels = CheckShipingLoadSheetModels;

            model.LoggedUser = loggedUser;

            eServiceWebContext _context = new eServiceWebContext();

            if (model.ProductHaulId > 0)
            {
                _context.DeleteProductHaul(model);
            }

        }

        [Test]
        public void TestScheduleBlendRequest()
        {
            double amount = 2;


            CreateScheduleBlendRequest(amount);


            Assert.AreNotEqual(productHaulLoadId, 0);

            var productHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productHaulLoadId);

            Assert.NotNull(productHaulLoad);
            Assert.AreEqual(productHaulLoad.BaseBlendWeight / 1000, amount);
            Assert.AreEqual(productHaulLoad.BulkPlant.Id, rigId);
            Assert.AreEqual(productHaulLoad.BulkPlant.Id, rigId);

            Assert.AreEqual(productHaulLoad.IsTotalBlendTonnage, false);
            //Assert.AreEqual(productHaulLoad.ClientRepresentative, Utility.GetClientRepresentative(rigJob));
            Assert.AreEqual(productHaulLoad.ProductHaulLoadLifeStatus, ProductHaulLoadStatus.Scheduled);


            CancelScheduleBlendRequest(productHaulLoadId);
        }


        [Test]
        public void TestRescheduleBlendFromBulkPlantBin()
        {

            double amount = 2;


            CreateScheduleBlendRequest(amount);


            var productHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productHaulLoadId);


            ProductLoadInfoModel productLoadInfoModel = new ProductLoadInfoModel();

            if (productHaulLoad != null && productHaulLoad.Id > 0)
            {
                productLoadInfoModel.ProductHaulLoadId = productHaulLoad.Id;
                productLoadInfoModel.CallSheetNumber = productHaulLoad.CallSheetNumber;
                productLoadInfoModel.BaseBlendSectionId = productHaulLoad.BlendSectionId;
                BlendChemical blendChemical = productHaulLoad.BlendChemical;
                productLoadInfoModel.BaseBlend = blendChemical == null
                    ? ""
                    : (((string.IsNullOrWhiteSpace(blendChemical.Description)
                        ? blendChemical.Name
                        : blendChemical.Description)).Split('+').FirstOrDefault());
                productLoadInfoModel.Category = productHaulLoad.BlendCategory == null
                    ? ""
                    : (string.IsNullOrEmpty(productHaulLoad.BlendCategory.Description)
                        ? productHaulLoad.BlendCategory.Name
                        : productHaulLoad.BlendCategory.Description);
                productLoadInfoModel.CategoryId =
                    productHaulLoad.BlendCategory == null ? 0 : productHaulLoad.BlendCategory.Id;
                productLoadInfoModel.MixWater = productHaulLoad.MixWater;
                productLoadInfoModel.Amount = productHaulLoad.TotalBlendWeight / 1000;
                productLoadInfoModel.TotalBlendWeight = productHaulLoad.TotalBlendWeight / 1000;
                productLoadInfoModel.BaseBlendWeight = productHaulLoad.BaseBlendWeight / 1000;
                productLoadInfoModel.BlendChemicalDescription = productHaulLoad.BlendChemical?.Description;
                productLoadInfoModel.IsTotalBlendTonnage = productHaulLoad.IsTotalBlendTonnage;
                productLoadInfoModel.OnLocationTime = productHaulLoad.OnLocationTime;
                productLoadInfoModel.ExpectedOnLocationTime = productHaulLoad.ExpectedOnLocationTime;
                productLoadInfoModel.EstimatedLoadTime = productHaulLoad.EstmatedLoadTime;

                productLoadInfoModel.ProgramNumber = productHaulLoad.ProgramId;

                productLoadInfoModel.BaseBlendUnit =
                    (productHaulLoad.Unit == null || string.IsNullOrEmpty(productHaulLoad.Unit.Description))
                        ? "t"
                        : productHaulLoad.Unit.Description;
                productLoadInfoModel.IsBlendTest = productHaulLoad.IsBlendTest;
                productLoadInfoModel.RigName = productHaulLoad.Rig?.Name;
                productLoadInfoModel.RigId = productHaulLoad.Rig == null ? 0 : productHaulLoad.Rig.Id;
                productLoadInfoModel.ClientName = productHaulLoad.Customer?.Name;
                productLoadInfoModel.RemainsAmount = Math.Round(productHaulLoad.RemainsAmount / 1000, 2);
                productLoadInfoModel.PodIndex = productHaulLoad.PodIndex;
                productLoadInfoModel.ClientRepresentative = productHaulLoad.ClientRepresentative;
                if (productHaulLoad.Bin != null)
                {
                    productLoadInfoModel.BinId = productHaulLoad.Bin.Id;
                    productLoadInfoModel.BinNumber = productHaulLoad.Bin.Name;
                }

                if (productHaulLoad.BulkPlant != null && productHaulLoad.BulkPlant.Id > 0)
                {
                    productLoadInfoModel.BulkPlantId = productHaulLoad.BulkPlant.Id;
                    productLoadInfoModel.BulkPlantName = productHaulLoad.BulkPlant.Name;
                }

                if (productHaulLoad.JobType != null && productHaulLoad.JobType.Id > 0)
                {
                    productLoadInfoModel.JobTypeId = productHaulLoad.JobType.Id;
                    productLoadInfoModel.JobTypeName = productHaulLoad.JobType.Name;
                }

                if (productHaulLoad.ServicePoint != null && productHaulLoad.ServicePoint.Id > 0)
                {
                    productLoadInfoModel.ServicePointId = productHaulLoad.ServicePoint.Id;
                    productLoadInfoModel.ServicePointName = productHaulLoad.ServicePoint.Name;
                }
            }


            eServiceWebContext.Instance.RescheduleBlendRequest(productLoadInfoModel);



            CancelScheduleBlendRequest(productHaulLoadId);
        }


        [Test]
        public void TestCancelBlendRequestFromBulkPlantBin()
        {
            double amount = 2;

            CreateScheduleBlendRequest(amount);

            var productHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productHaulLoadId);

            CancelScheduleBlendRequest(productHaulLoadId);

            ////Verify
            Assert.AreNotEqual(productHaulLoadId, 0);
            Assert.NotNull(productHaulLoad);
            Assert.NotNull(productHaulLoadId);

            var newProductHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productHaulLoadId);
            Assert.Null(newProductHaulLoad);

        }

        [Test]
        public void TestHualBlendFromRigBulkPlantBin()
        {
            double amount = 2;

            CreateScheduleBlendRequest(amount);

            CreateHualBlendFromRigBulkPlantBin(amount);

            Assert.AreNotEqual(productHaulId, 0);

            var asProductHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productHaulLoadId);

            Assert.NotNull(asProductHaulLoad);
            Assert.AreEqual(asProductHaulLoad.BlendSectionId, callSheetBlendSectionId);
            Assert.AreEqual(asProductHaulLoad.BaseBlendWeight / 1000, amount);
            Assert.AreEqual(asProductHaulLoad.IsTotalBlendTonnage, false);
            Assert.AreEqual(asProductHaulLoad.ClientRepresentative, Utility.GetClientRepresentative(rigJob));
            Assert.AreEqual(asProductHaulLoad.ProductHaulLoadLifeStatus, ProductHaulLoadStatus.Scheduled);

            //Verify shipping load sheet
            var shippingLoadSheets =
                eServiceOnlineGateway.Instance.GetShippingLoadSheetByProductHaulLoads(new List<int>()
                    { productHaulLoadId });
            Assert.Greater(shippingLoadSheets.Count, 0);
            var asShippingLoadSheet = shippingLoadSheets.OrderByDescending(p => p.Id).First();
            Assert.NotNull(asShippingLoadSheet);
            asShippingLoadSheet = eServiceOnlineGateway.Instance.GetShippingLoadSheetById(asShippingLoadSheet.Id, true);
            Assert.NotNull(asShippingLoadSheet);
            Assert.Greater(asShippingLoadSheet.BlendUnloadSheets.Count, 0);
            Assert.AreEqual(asShippingLoadSheet.LoadAmount / 1000, amount);
            Assert.NotNull(asShippingLoadSheet.ProductHaul);
            Assert.AreNotEqual(asShippingLoadSheet.ProductHaul.Id, 0);

            //verify product haul sheet
            var productHaul = eServiceOnlineGateway.Instance.GetProductHaulById(asShippingLoadSheet.ProductHaul.Id);
            Assert.NotNull(productHaul);
            Assert.Greater(productHaul.PodLoad.Count, 0);

            //verify crew assignment
            RigJobSanjelCrewSection rigJobSanjelCrewSection =
                eServiceOnlineGateway.Instance.GetRigJobCrewSectionsByProductHaul(asShippingLoadSheet.ProductHaul.Id);
            Assert.NotNull(rigJobSanjelCrewSection);
            Assert.AreNotEqual(rigJobSanjelCrewSection.ProductHaul.Id, 0);

            SanjelCrewSchedule crewSchedule =
                eServiceOnlineGateway.Instance.GetCrewScheduleByJobCrewSection(rigJobSanjelCrewSection.Id);
            Assert.NotNull(crewSchedule);

            //Verify call sheet unit section
            var unitSections =
                eServiceOnlineGateway.Instance.GetUnitSectionsByCallSheetId(asShippingLoadSheet.CallSheetId);
            Assert.NotNull(unitSections);
            Assert.Greater(unitSections.Count, 0);

            var unitSectionsByProductHaul = unitSections.Where(p => p.ProductHaulId == productHaul.Id);
            Assert.NotNull(unitSectionsByProductHaul);
            Assert.AreEqual(unitSectionsByProductHaul.Count(), 1);

            var unitSection = unitSectionsByProductHaul.First();
            Assert.NotNull(unitSection);
            Assert.AreEqual(unitSection.CrewId, crewId);
            Assert.AreEqual(productHaul.Description, unitSection.HaulDescription);
        }


        [Test]
        public void TestRescheduleProductHaul()
        {

            double amount = 2;

            CreateScheduleBlendRequest(amount);

            CreateHualBlendFromRigBulkPlantBin(amount);

            var productHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productHaulId);

            var productHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);

            ProductHaulInfoModel productHaulInfoModel = new ProductHaulInfoModel();


            productHaulInfoModel.IsGoWithCrew = productHaul.IsGoWithCrew;
            productHaulInfoModel.IsThirdParty = productHaul.IsThirdParty;
            productHaulInfoModel.CrewId = productHaul.Crew.Id;
            productHaulInfoModel.IsExistingHaul = true;
            productHaulInfoModel.BulkPlantId = productHaul.BulkPlant.Id;
            productHaulInfoModel.EstimatedTravelTime = productHaul.EstimatedTravelTime;
            productHaulInfoModel.EstimatedLoadTime = productHaul.EstimatedLoadTime;
            productHaulInfoModel.ExpectedOnLocationTime = productHaul.ExpectedOnLocationTime;
            productHaulInfoModel.ProductHaulId = productHaul.Id;


            List<PodLoadAndBendUnLoadModel> podLoadAndBendUnLoadModels = new List<PodLoadAndBendUnLoadModel>();
            var podLoads = eServiceOnlineGateway.Instance.GetPodLoadsByProductHaul(productHaulId);

            int nAmout = 3;

            //PodLoad LoadAmount
            foreach (var PodLoad in podLoads)
            {
                PodLoad.LoadAmount = nAmout;
                nAmout--;

            }

            podLoads.ForEach(p => p.LoadAmount = p.LoadAmount * 1000);
            foreach (var shippingLoadSheetItem in productHaul.ShippingLoadSheets)
            {
                var shippingLoadSheet =
                    eServiceOnlineGateway.Instance.GetShippingLoadSheetById(shippingLoadSheetItem.Id, true);

                PodLoadAndBendUnLoadModel podLoadAndBendUnLoadModel = new PodLoadAndBendUnLoadModel();
                podLoadAndBendUnLoadModel.IsCheckShippingLoadSheet = true;
                var unloadSheet = shippingLoadSheet.BlendUnloadSheets.FirstOrDefault();
                podLoadAndBendUnLoadModel.RigId = unloadSheet == null
                    ? 0
                    : (unloadSheet.DestinationStorage == null ? 0 : unloadSheet.DestinationStorage.Rig?.Id ?? 0);
                podLoadAndBendUnLoadModel.RigName = shippingLoadSheet.Rig?.Name;
                podLoadAndBendUnLoadModel.LoadAmount = shippingLoadSheet.LoadAmount / 1000;
                podLoadAndBendUnLoadModel.ProgramId = productHaulLoad.ProgramId;
                podLoadAndBendUnLoadModel.CallSheetNumber = shippingLoadSheet.CallSheetNumber;
                podLoadAndBendUnLoadModel.ShippingLoadSheetId = shippingLoadSheet.Id;
                podLoadAndBendUnLoadModel.Blend = shippingLoadSheet.BlendDescription;
                podLoadAndBendUnLoadModel.IsGoWithCrew = shippingLoadSheet.IsGoWithCrew;

                //BlendUnloadSheets LoadAmount
                if (shippingLoadSheet.BlendUnloadSheets.Count >= 2)
                {
                    shippingLoadSheet.BlendUnloadSheets[0].UnloadAmount = 5;
                    shippingLoadSheet.BlendUnloadSheets[1].UnloadAmount = 1;
                }

                if (shippingLoadSheet.BlendUnloadSheets != null && shippingLoadSheet.BlendUnloadSheets.Count > 0)
                {
                    podLoadAndBendUnLoadModel.BlendUnloadSheetModels = shippingLoadSheet.BlendUnloadSheets;
                }
                else
                {
                    podLoadAndBendUnLoadModel.BlendUnloadSheetModels = new List<BlendUnloadSheet>();
                    if (productHaulLoad.Rig != null && productHaulLoad.Rig.Id != 0)
                    {
                        var binInformations =
                            eServiceOnlineGateway.Instance.GetBinInformationsByRigId(productHaulLoad.Rig.Id);
                        foreach (var item in binInformations)
                        {
                            podLoadAndBendUnLoadModel.BlendUnloadSheetModels.Add(new BlendUnloadSheet()
                            {
                                UnloadAmount = 0,
                                DestinationStorage = item
                            });
                        }
                    }
                }

                podLoadAndBendUnLoadModel.PodLoadModels = podLoads;

                podLoadAndBendUnLoadModels.Add(podLoadAndBendUnLoadModel);


            }

            productHaulInfoModel.CrewId = 3055;
            productHaulInfoModel.EstimatedTravelTime = 72;
            productHaulInfoModel.EstimatedLoadTime = DateTime.Now.AddDays(5);
            productHaulInfoModel.ExpectedOlTime = DateTime.Now.AddDays(3);

            RescheduleProductHaulViewModel rescheduleProductHaulViewModel = new RescheduleProductHaulViewModel();
            rescheduleProductHaulViewModel.PodLoadAndBendUnLoadModels = podLoadAndBendUnLoadModels;
            rescheduleProductHaulViewModel.ProductHaulInfoModel = productHaulInfoModel;
//            eServiceWebContext.Instance.ReschedulePodLoadAndBlendUnLoad(podLoadAndBendUnLoadModels,
//                productHaulInfoModel.ProductHaulId, "awang");
            eServiceWebContext.Instance.RescheduleProductHaul1(rescheduleProductHaulViewModel, rigJob.Id, "awang");


            //Verify 
            Assert.AreNotEqual(productHaulId, 0);

            Assert.NotNull(podLoadAndBendUnLoadModels);
            Assert.NotNull(productHaulInfoModel);


            ProductHaul product = eServiceWebContext.Instance.GetProductHaulById(productHaulId);

            Assert.NotNull(product);

            Assert.AreEqual(product.Crew.Id, productHaulInfoModel.CrewId);
        }



        [Test]
        public void TestCancelProductHaul()
        {
            var productHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);

            var productHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productHaulLoadId);

            CancelProductHaulFromRigBulkPlantBin();


            ////Verify
            Assert.AreNotEqual(productHaulId, 0);
            Assert.AreNotEqual(productHaulLoadId, 0);
            Assert.NotNull(productHaulLoad);
            Assert.NotNull(productHaulId);

            var newProductHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productHaulLoadId);

            var newProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);

            Assert.Null(newProductHaulLoad);
            Assert.Null(newProductHaul);

        }

        [Test]
        public void OnLocationProductHaul()
        {

            double amount = 2;

            CreateScheduleBlendRequest(amount);

            CreateHualBlendFromRigBulkPlantBin(amount);


            OnLocationProductHaulViewModel model = new OnLocationProductHaulViewModel()
                { ProductHaulId = Convert.ToInt32(productHaulId) };
            model.ShippingLoadSheetModels =
                eServiceOnlineGateway.Instance.GetShippingLoadSheetByProductHaulId(model.ProductHaulId);
            if (model.ShippingLoadSheetModels != null && model.ShippingLoadSheetModels.Count > 0)
            {
                bool sameLocation = true;
                string firstLocation = string.Empty;
                foreach (ShippingLoadSheet item in model.ShippingLoadSheetModels)
                {
                    if (item != null)
                    {
                        if (string.IsNullOrEmpty(firstLocation))
                        {
                            firstLocation = item.Destination;
                        }
                        else
                        {
                            if (!string.Equals(firstLocation, item.Destination))
                                sameLocation = false;
                        }
                    }
                }

                model.IsSameLocation = sameLocation;
                model.OnLocationTime = DateTime.Now;
            }

            model.LoggedUser = loggedUser;


            RigJobSanjelCrewSection rigJobSanjelCrewSection =
                eServiceOnlineGateway.Instance.GetRigJobCrewSectionsByProductHaul(productHaulId);

            int rigJobCrewSectionId = rigJobSanjelCrewSection.Id;


            SanjelCrewSchedule crewSchedule =
                eServiceOnlineGateway.Instance.GetCrewScheduleByJobCrewSection(rigJobCrewSectionId);


            eServiceWebContext.Instance.UpdateProductHaulOnLocation(model);

            ////Verify
            ProductHaul productHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);

            List<ShippingLoadSheet> shippingLoadSheets =
                eServiceOnlineGateway.Instance.GetShippingLoadSheetByProductHaulId(productHaul.Id);

            if (shippingLoadSheets.Count > 0)
            {
                foreach (var haulLoad in shippingLoadSheets)
                {
                    Assert.AreEqual(haulLoad.ShippingStatus, ShippingStatus.OnLocation);
                    Assert.AreEqual(haulLoad.ModifiedUserName, loggedUser);
                    //Assert.AreEqual(haulLoad.OnLocationTime, locationDateTime);
                    Assert.NotNull(haulLoad.OnLocationTime);

                }
            }

            RigJobSanjelCrewSection newRigJobSanjelCrewSection =
                eServiceOnlineGateway.Instance.GetRigJobCrewSectionsByProductHaul(productHaul.Id);

            SanjelCrewSchedule newCrewSchedule =
                eServiceOnlineGateway.Instance.GetCrewScheduleByJobCrewSection(rigJobCrewSectionId);

            Assert.NotNull(rigJobSanjelCrewSection);
            Assert.NotNull(crewSchedule);
            Assert.Null(newRigJobSanjelCrewSection);
            Assert.Null(newCrewSchedule);


            Assert.AreEqual(productHaul.ProductHaulLifeStatus, ShippingStatus.OnLocation);
            Assert.AreEqual(productHaul.ModifiedUserName, loggedUser);


        }
    }
}