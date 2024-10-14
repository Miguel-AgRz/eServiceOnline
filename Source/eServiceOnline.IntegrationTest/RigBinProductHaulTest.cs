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
    public class RigBinProductHaulTest : IntegrationTestBase
    {

        protected int productHaulId = 0;
        protected int productHaulLoadId = 0;
        protected RigJob rigJob = null;
        protected CallSheetBlendSection blendSection = null;
        protected int crewId = 3005;
        private int rigJobId = 67461;
        private string programId = "PRG2200386";

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




        public void CreateScheduleProductHaulToRigBin(double loadAmount)
        {
            ScheduleProductHaulToRigBinViewModel model = new ScheduleProductHaulToRigBinViewModel();

            model.RigJobId = 0;

            model.OrigBinId = 320;

            var binInformations = eServiceOnlineGateway.Instance.GetBinInformationsByRigId(rigJob.Rig.Id);
            foreach (var binInformation in binInformations)
            {
                if (binInformation.Bin.Id == model.OrigBinId)
                {
                    model.OrigBinInformationId = binInformation.Id;
                }
            }

            model.ShippingLoadSheetModel.RigName = rigJob.Rig.Name;
            model.ShippingLoadSheetModel.RigId = rigJob.Rig.Id;
            model.ShippingLoadSheetModel.BlendUnloadSheetModels = new List<BlendUnloadSheet>();
            foreach (var item in binInformations)
            {
                model.ShippingLoadSheetModel.BlendUnloadSheetModels.Add(new BlendUnloadSheet()
                {
                    UnloadAmount = 0,
                    DestinationStorage = item
                });
            }

            model.ShippingLoadSheetModel.BulkPlantId = 2348;
            model.ShippingLoadSheetModel.BulkPlantName = "EDM Bulk Plant";
            model.ShippingLoadSheetModel.ClientRepresentative = "Akita 25 Rig Phone";
            model.ShippingLoadSheetModel.ClientName = "Canadian Natural Resources Ltd";


            model.ProductLoadInfoModel.Amount = loadAmount;
            model.ProductLoadInfoModel.JobTypeId = 1504;
            model.ProductLoadInfoModel.BaseBlendSectionId = 101435;
            model.ProductLoadInfoModel.BulkPlantId = 2348;
            model.ProductLoadInfoModel.BulkPlantName = "EDM Bulk Plant";
            model.ProductLoadInfoModel.ClientRepresentative = "Akita 25 Rig Phone";
            model.ProductLoadInfoModel.ClientName = "Canadian Natural Resources Ltd";
            model.ProductLoadInfoModel.ProgramNumber = "PRG2200386";
            model.ProductLoadInfoModel.RigId = rigJob.Rig.Id;
            model.ProductLoadInfoModel.RigName = rigJob.Rig.Name;
            model.ProductLoadInfoModel.EstimatedLoadTime = DateTime.Now.AddDays(1);
            model.ProductLoadInfoModel.ExpectedOnLocationTime = DateTime.Now.AddDays(1);
            model.ProductLoadInfoModel.ClientRepresentative = "Akita 25 Rig Phone";
            model.ProductLoadInfoModel.MixWater = 0.99;
            model.ProductLoadInfoModel.CustomerId = 1513;
            model.ProductLoadInfoModel.IsTotalBlendTonnage = true;
            model.ProductLoadInfoModel.IsBlendTest = false;
            model.ProductLoadInfoModel.Comments = "Schedule Product Haul To RigBin";
            model.ProductLoadInfoModel.RemainsAmount = loadAmount;

            model.ProductHaulInfoModel.EstimatedTravelTime = 24;
            model.ProductHaulInfoModel.EstimatedLoadTime = DateTime.Now.AddDays(1);
            model.ProductHaulInfoModel.ExpectedOnLocationTime = DateTime.Now.AddDays(1);
            model.ProductHaulInfoModel.BulkPlantId = 2348;
            model.ProductHaulInfoModel.CrewId = 3131;


            model.BlendUnloadSheetModels = new List<BlendUnloadSheet>();

            foreach (var binInformation in binInformations)
            {
                if (binInformation.Id == model.OrigBinInformationId)
                {
                    model.BlendUnloadSheetModels.Add(new BlendUnloadSheet()
                    { DestinationStorage = binInformation, UnloadAmount = loadAmount });
                }
                else
                {
                    model.BlendUnloadSheetModels.Add(new BlendUnloadSheet()
                    { DestinationStorage = binInformation, UnloadAmount = 0 });

                }

            }

            model.PodLoadModels = new List<PodLoad>();
            model.PodLoadModels.Add(new PodLoad() { LoadAmount = loadAmount, PodIndex = 0 });
            model.PodLoadModels.Add(new PodLoad() { LoadAmount = 0, PodIndex = 1 });
            model.PodLoadModels.Add(new PodLoad() { LoadAmount = 0, PodIndex = 2 });
            model.PodLoadModels.Add(new PodLoad() { LoadAmount = 0, PodIndex = 3 });


            model.LoggedUser = "awang";

            eServiceWebContext _context = new eServiceWebContext();

            _context.ScheduleProductHaul(model);

            var productHaulLoad =
                eServiceOnlineGateway.Instance.GetProductHaulLoadByQuery(p => p.ProgramId == programId).OrderByDescending(p=>p.Id).First();

            if (productHaulLoad != null)
            {
                productHaulLoadId = productHaulLoad.Id;

                var shippingLoadSheet =
                    eServiceOnlineGateway.Instance.GetShippingLoadSheetByProductHaulLoads(new List<int>() { productHaulLoad.Id }).First();

                if (shippingLoadSheet != null)
                {
                    productHaulId = shippingLoadSheet.ProductHaul.Id;
                }
            }
        }

        public void CancelScheduleProductHaulToRigBin(ProductHaulLoad productHaulLoad, ProductHaul productHaul, bool isCheckShipingLoadSheet)
        {

            CancelProductViewModel model = new CancelProductViewModel();


            model.ProductHaulId = productHaulId;
            List<CheckShipingLoadSheetModel> CheckShipingLoadSheetModels = new List<CheckShipingLoadSheetModel>();
            foreach (ShippingLoadSheet shippingLoadSheet in productHaul.ShippingLoadSheets)
            {
                var productHualLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(shippingLoadSheet.ProductHaulLoad.Id);
                List<int> ids = new List<int>();
                ids.Add(productHualLoad.Id);
                var shippingLoadSheets = eServiceOnlineGateway.Instance.GetShippingLoadSheetByProductHaulLoads(ids);
                CheckShipingLoadSheetModels.Add(new CheckShipingLoadSheetModel()
                {
                    IsChecked = isCheckShipingLoadSheet,
                    IsReadOnly = shippingLoadSheets.Count > 1 ? true : false,
                    ShippingLoadSheetModel = shippingLoadSheet,
                    ProductHaulLoadModel = productHualLoad
                });
            }
            model.CheckShipingLoadSheetModels = CheckShipingLoadSheetModels;

            eServiceWebContext _context = new eServiceWebContext();

            _context.DeleteProductHaul(model);
        }

        [Test]
        public void TestScheduleProductHaulToRigBin()
        {
            double loadAmount = 2;

            CreateScheduleProductHaulToRigBin(loadAmount);

            Assert.AreNotEqual(productHaulId, 0);

            var productHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productHaulLoadId);

            Assert.NotNull(productHaulLoad);

            Assert.AreEqual(productHaulLoad.BaseBlendWeight / 1000, loadAmount);
            Assert.AreEqual(productHaulLoad.IsTotalBlendTonnage, false);
            Assert.AreEqual(productHaulLoad.ClientRepresentative, Utility.GetClientRepresentative(rigJob));
            Assert.AreEqual(productHaulLoad.ProductHaulLoadLifeStatus, ProductHaulLoadStatus.Scheduled);

            //Verify shipping load sheet
            var shippingLoadSheets =
                eServiceOnlineGateway.Instance.GetShippingLoadSheetByProductHaulLoads(new List<int>() { productHaulLoad.Id });
            Assert.Greater(shippingLoadSheets.Count, 0);
            var shippingLoadSheet = shippingLoadSheets.OrderByDescending(p => p.Id).First();
            Assert.NotNull(shippingLoadSheet);
            shippingLoadSheet = eServiceOnlineGateway.Instance.GetShippingLoadSheetById(shippingLoadSheet.Id, true);
            Assert.NotNull(shippingLoadSheet);
            Assert.Greater(shippingLoadSheet.BlendUnloadSheets.Count, 0);
            Assert.AreEqual(shippingLoadSheet.LoadAmount / 1000, loadAmount);
            Assert.NotNull(shippingLoadSheet.ProductHaul);
            Assert.AreNotEqual(shippingLoadSheet.ProductHaul.Id, 0);

            //verify product haul sheet
            var productHaul = eServiceOnlineGateway.Instance.GetProductHaulById(shippingLoadSheet.ProductHaul.Id);
            Assert.NotNull(productHaul);
            Assert.Greater(productHaul.PodLoad.Count, 0);

            //verify crew assignment
            RigJobSanjelCrewSection rigJobSanjelCrewSection = eServiceOnlineGateway.Instance.GetRigJobCrewSectionsByProductHaul(shippingLoadSheet.ProductHaul.Id);
            Assert.NotNull(rigJobSanjelCrewSection);
            Assert.AreNotEqual(rigJobSanjelCrewSection.ProductHaul.Id, 0);

            SanjelCrewSchedule crewSchedule =
                eServiceOnlineGateway.Instance.GetCrewScheduleByJobCrewSection(rigJobSanjelCrewSection.Id);
            Assert.NotNull(crewSchedule);

            //Verify call sheet unit section
            var unitSections =
                eServiceOnlineGateway.Instance.GetUnitSectionsByCallSheetId(shippingLoadSheet.CallSheetId);
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
        public void TestCancelProductHaulWithCheckTrue()
        {
            double loadAmount = 2;

            CreateScheduleProductHaulToRigBin(loadAmount);

            var productHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productHaulLoadId);

            var productHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);

            CancelScheduleProductHaulToRigBin(productHaulLoad, productHaul, true);

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
        public void TestCancelProductHaulWithCheckFalse()
        {

            double loadAmount = 2;

            CreateScheduleProductHaulToRigBin(loadAmount);

            var productHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productHaulLoadId);

            var productHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);


            CancelScheduleProductHaulToRigBin(productHaulLoad, productHaul, false);

            ////Verify
            Assert.AreNotEqual(productHaulId, 0);
            Assert.AreNotEqual(productHaulLoadId, 0);
            Assert.NotNull(productHaulLoad);
            Assert.NotNull(productHaulId);

            var newProductHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productHaulLoadId);

            var newProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);


            Assert.NotNull(newProductHaulLoad);
            Assert.Null(newProductHaul);
        }

        [Test]
        public void TestOnLocationProductHaul()
        {

            double loadAmount = 2;
            DateTime locationDateTime = DateTime.Now;
            string userName = "awang";
            CreateScheduleProductHaulToRigBin(loadAmount);


            OnLocationProductHaulViewModel model = new OnLocationProductHaulViewModel()
            { ProductHaulId = Convert.ToInt32(productHaulId) };

            model.ShippingLoadSheetModels =
                eServiceOnlineGateway.Instance.GetShippingLoadSheetByProductHaulId(productHaulId);

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
                model.OnLocationTime = locationDateTime;
            }

            model.LoggedUser = userName;



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
                    Assert.AreEqual(haulLoad.ModifiedUserName, userName);
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
            Assert.AreEqual(productHaul.ModifiedUserName, userName);

        }

        [Test]
        public void TestRescheduleBlendFromRigBin()
        {
            
            double loadAmount = 2;

            CreateScheduleProductHaulToRigBin(loadAmount);

            ProductLoadInfoModel model = new ProductLoadInfoModel();

            var productHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productHaulLoadId);

            if (productHaulLoad != null && productHaulLoad.Id > 0)
            {
                model.ProductHaulLoadId = productHaulLoad.Id;
                model.CallSheetNumber = productHaulLoad.CallSheetNumber;
                model.BaseBlendSectionId = productHaulLoad.BlendSectionId;
                BlendChemical blendChemical = productHaulLoad.BlendChemical;
                model.BaseBlend = blendChemical == null ? "" : (((string.IsNullOrWhiteSpace(blendChemical.Description) ? blendChemical.Name : blendChemical.Description)).Split('+').FirstOrDefault());
                model.Category = productHaulLoad.BlendCategory == null ? "" : (string.IsNullOrEmpty(productHaulLoad.BlendCategory.Description) ? productHaulLoad.BlendCategory.Name : productHaulLoad.BlendCategory.Description);
                model.CategoryId = productHaulLoad.BlendCategory == null ? 0 : productHaulLoad.BlendCategory.Id;
       
                model.TotalBlendWeight = productHaulLoad.TotalBlendWeight / 1000;
                model.BaseBlendWeight = productHaulLoad.BaseBlendWeight / 1000;
                model.BlendChemicalDescription = productHaulLoad.BlendChemical?.Description;
                model.IsTotalBlendTonnage = productHaulLoad.IsTotalBlendTonnage;
                model.OnLocationTime = productHaulLoad.OnLocationTime;
                model.ExpectedOnLocationTime = productHaulLoad.ExpectedOnLocationTime;
                model.EstimatedLoadTime = productHaulLoad.EstmatedLoadTime;

                model.ProgramNumber = productHaulLoad.ProgramId;

                model.BaseBlendUnit = (productHaulLoad.Unit == null || string.IsNullOrEmpty(productHaulLoad.Unit.Description)) ? "t" : productHaulLoad.Unit.Description;
                model.IsBlendTest = productHaulLoad.IsBlendTest;
                model.RigName = productHaulLoad.Rig?.Name;
                model.RigId = productHaulLoad.Rig == null ? 0 : productHaulLoad.Rig.Id;
                model.ClientName = productHaulLoad.Customer?.Name;
                model.RemainsAmount = Math.Round(productHaulLoad.RemainsAmount / 1000, 2);
                model.PodIndex = productHaulLoad.PodIndex;
                model.ClientRepresentative = productHaulLoad.ClientRepresentative;

                if (productHaulLoad.Bin != null)
                {
                    model.BinId = productHaulLoad.Bin.Id;
                    model.BinNumber = productHaulLoad.Bin.Name;
                }

                if (productHaulLoad.BulkPlant != null && productHaulLoad.BulkPlant.Id > 0)
                {
                    model.BulkPlantId = productHaulLoad.BulkPlant.Id;
                    model.BulkPlantName = productHaulLoad.BulkPlant.Name;
                }
                if (productHaulLoad.JobType != null && productHaulLoad.JobType.Id > 0)
                {
                    model.JobTypeId = productHaulLoad.JobType.Id;
                    model.JobTypeName = productHaulLoad.JobType.Name;
                }

                if (productHaulLoad.ServicePoint != null && productHaulLoad.ServicePoint.Id > 0)
                {
                    model.ServicePointId = productHaulLoad.ServicePoint.Id;
                    model.ServicePointName = productHaulLoad.ServicePoint.Name;
                }

                model.MixWater = 0.33;
                model.Amount = 3;
                model.Comments = "Reschedule Blend From RigBin";
                model.IsTotalBlendTonnage = true;
            }


            eServiceWebContext.Instance.RescheduleBlendRequest(model, true);


            ////Verify
            var newProductHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productHaulLoadId);


            Assert.NotNull(newProductHaulLoad);
            Assert.AreEqual(newProductHaulLoad.MixWater, model.MixWater);
            //Assert.AreEqual(newProductHaulLoad.TotalBlendWeight / 1000, model.Amount);
            //Assert.AreEqual(newProductHaulLoad.Comments, model.Comments);
            Assert.AreEqual(newProductHaulLoad.IsTotalBlendTonnage, model.IsTotalBlendTonnage);


            var productHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);

            CancelScheduleProductHaulToRigBin(newProductHaulLoad, productHaul, true);

        }

        [Test]
        public void TestRescheduleProductHaul()
        {

            double loadAmount = 2;

            CreateScheduleProductHaulToRigBin(loadAmount);

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

            double nAmout = loadAmount+1;
            podLoads[0].LoadAmount = podLoads[0].LoadAmount + 1;



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

                ////BlendUnloadSheets LoadAmount
                //if (shippingLoadSheet.BlendUnloadSheets.Count >= 2)
                //{
                //    shippingLoadSheet.BlendUnloadSheets[0].UnloadAmount = 5;
                //    shippingLoadSheet.BlendUnloadSheets[1].UnloadAmount = 1;
                //}

                if (shippingLoadSheet.BlendUnloadSheets != null && shippingLoadSheet.BlendUnloadSheets.Count > 0)
                {
                    podLoadAndBendUnLoadModel.BlendUnloadSheetModels = shippingLoadSheet.BlendUnloadSheets;

                    podLoadAndBendUnLoadModel.BlendUnloadSheetModels[0].UnloadAmount = podLoadAndBendUnLoadModel.BlendUnloadSheetModels[0].UnloadAmount  + 1*1000;

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


            //Verify 
            Assert.AreNotEqual(productHaulId, 0);

            Assert.NotNull(podLoadAndBendUnLoadModels);
            Assert.NotNull(productHaulInfoModel);

            RescheduleProductHaulViewModel model = new RescheduleProductHaulViewModel()
	            { ProductHaulInfoModel = productHaulInfoModel, PodLoadAndBendUnLoadModels = podLoadAndBendUnLoadModels };

//            eServiceWebContext.Instance.ReschedulePodLoadAndBlendUnLoad(podLoadAndBendUnLoadModels, productHaulInfoModel.ProductHaulId, "awang");
            eServiceWebContext.Instance.RescheduleProductHaul1(model, rigJob.Id, "awang");


            ProductHaul product = eServiceWebContext.Instance.GetProductHaulById(productHaulId);

            Assert.NotNull(product);

            Assert.AreEqual(product.Crew.Id, productHaulInfoModel.CrewId);
        }

    }
}