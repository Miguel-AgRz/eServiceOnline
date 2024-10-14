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
using Sesi.SanjelData.Services.BusinessEntities.Operation.Crew;
using Sesi.SanjelData.Services.Interfaces.BusinessEntities.Operation.Crew;
using Sesi.SanjelData.Services.Interfaces.BusinessEntities.Operation.Dispatch;
using Sesi.SanjelData.Services.Interfaces.BusinessEntities.WellSite;
using Sesi.SanjelData.Services.Interfaces.Common.BusinessEntities.Equipment;
using Sesi.SanjelData.Services.Interfaces.Common.BusinessEntities.HumanResources;
using Sesi.SanjelData.Services.Interfaces.Common.BusinessEntities.WellSite;
using Rig = Sesi.SanjelData.Entities.Common.BusinessEntities.WellSite.Rig;

namespace eServiceOnline.IntegrationTest
{

    public class ScheduleWithDRBForBlend : ScheduleWithDRBForBlendTestBase
    {
        private string loggedUser = "awang";

        private int randomNumber = 0;

        private DateTime onDateTimeTime;

        private ProductLoadInfoModel PreparingBlendData(bool isTotalBlendTonnage)
        {
            Random random = new Random();

            ProductLoadInfoModel model = new ProductLoadInfoModel();

            List<Rig> listRigs = eServiceOnlineGateway.Instance.GetBulkPlants();

            randomNumber = random.Next(0, listRigs.Count);

            int bulkPlantId = listRigs[randomNumber].Id;
            string bulkPlantName = listRigs[randomNumber].Name;

            model.IsBlendTest = blendSection.IsNeedFieldTesting;
            model.CallSheetNumber = callSheet.CallSheetNumber;
            model.RigId = rig.Id;

            model.MixWater = blendSection.MixWaterRequirement;
            model.ExpectedOnLocationTime = DateTime.Now;
            model.EstimatedLoadTime = DateTime.Now;

            model.RigId = rigJob.Rig.Id;
            model.RigName = rigJob.Rig.Name;


            model.BaseBlendSectionId = blendSection.Id;

            model.RigId = rig.Id;
            model.MixWater = blendSection.MixWaterRequirement;

            model.BulkPlantId = bulkPlantId;
            model.BulkPlantName = bulkPlantName;

            model.LoggedUser = loggedUser;

            model.Amount = 1;
            model.IsTotalBlendTonnage = isTotalBlendTonnage;


            return model;
        }

        private HaulBlendFromRigJobBlendViewModel PreparingHaulBlendData(int productHaulLoadId, int crewId, int existingHaulId,bool isGowithCrew,bool isThird)
        {
            onDateTimeTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy/MM/dd"));

            HaulBlendFromRigJobBlendViewModel model = new HaulBlendFromRigJobBlendViewModel();

            ProductHaulLoad productHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productHaulLoadId, true);

            model.RigJobId = rigJob.Id;

            model.ProductLoadInfoModel.ProductHaulLoadId = productHaulLoad.Id;
            model.ProductLoadInfoModel.CallSheetNumber = productHaulLoad.CallSheetNumber;
            model.ProductLoadInfoModel.BaseBlendSectionId = productHaulLoad.BlendSectionId;
            //BlendChemical blendChemical = productHaulLoad.BlendChemical;

            model.ProductLoadInfoModel.Category = productHaulLoad.BlendCategory == null ? "" : (string.IsNullOrEmpty(productHaulLoad.BlendCategory.Description) ? productHaulLoad.BlendCategory.Name : productHaulLoad.BlendCategory.Description);
            model.ProductLoadInfoModel.CategoryId = productHaulLoad.BlendCategory == null ? 0 : productHaulLoad.BlendCategory.Id;

            model.ProductLoadInfoModel.MixWater = productHaulLoad.MixWater;
            model.ProductLoadInfoModel.Amount = productHaulLoad.TotalBlendWeight / 1000;
            model.ProductLoadInfoModel.TotalBlendWeight = productHaulLoad.TotalBlendWeight / 1000;
            model.ProductLoadInfoModel.BaseBlendWeight = productHaulLoad.BaseBlendWeight / 1000;
            model.ProductLoadInfoModel.BlendChemicalDescription = productHaulLoad.BlendChemical?.Description;
            model.ProductLoadInfoModel.IsTotalBlendTonnage = productHaulLoad.IsTotalBlendTonnage;

            model.ProductLoadInfoModel.BaseBlendUnit = (productHaulLoad.Unit == null || string.IsNullOrEmpty(productHaulLoad.Unit.Description)) ? "t" : productHaulLoad.Unit.Description;

            model.ProductLoadInfoModel.RigName = productHaulLoad.Rig?.Name;
            model.ProductLoadInfoModel.RigId = productHaulLoad.Rig == null ? 0 : productHaulLoad.Rig.Id;
            model.ProductLoadInfoModel.ClientName = productHaulLoad.Customer?.Name;
            model.ProductLoadInfoModel.RemainsAmount = Math.Round(productHaulLoad.RemainsAmount / 1000, 2);

            model.ProductLoadInfoModel.PodIndex = productHaulLoad.PodIndex;


            model.ProductLoadInfoModel.BulkPlantId = productHaulLoad.BulkPlant.Id;
            model.ProductLoadInfoModel.BulkPlantName = productHaulLoad.BulkPlant.Name;

            model.ProductHaulInfoModel.IsGoWithCrew = isGowithCrew;


            if (existingHaulId != 0)
            {
                ProductHaul existingHaul = eServiceOnlineGateway.Instance.GetProductHaulById(existingHaulId);

                model.ProductHaulInfoModel.IsExistingHaul = true;
                model.ProductHaulInfoModel.ProductHaulId = existingHaul.Id;
                model.ProductHaulInfoModel.BulkPlantId = existingHaul.BulkPlant.Id;
                model.ProductHaulInfoModel.ExpectedOnLocationTime = existingHaul.ExpectedOnLocationTime;
                model.ProductHaulInfoModel.EstimatedTravelTime = existingHaul.EstimatedTravelTime;
                model.ProductHaulInfoModel.EstimatedLoadTime = existingHaul.EstimatedLoadTime;

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

                List<PodLoad> listExistPodLoads = eServiceOnlineGateway.Instance.GetPodLoadsByProductHaul(existingHaul.Id);

                if (listExistPodLoads == null || listExistPodLoads.Count == 0)
                {

                    List<PodLoad> podLoadModels = new List<PodLoad>();

                    for (var i = 0; i < 4; i++)
                    {
                        podLoadModels.Add(new PodLoad()
                        {
                            PodIndex = i,
                            LoadAmount = 0
                        });
                    }

                    model.PodLoadModels = podLoadModels;

                    model.PodLoadModels[0].LoadAmount = 1;
                }
                else
                {
                    model.PodLoadModels = listExistPodLoads;


                    model.PodLoadModels[0].LoadAmount = listExistPodLoads[0].LoadAmount/1000;

                    model.PodLoadModels[1].LoadAmount = 1;
                }
            }
            else
            {
                if (!isGowithCrew)
                {
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
                }

                model.ProductHaulInfoModel.EstimatedTravelTime = 4;
                model.ProductHaulInfoModel.ExpectedOnLocationTime = onDateTimeTime.AddHours(6);

                List<PodLoad> podLoadModels = new List<PodLoad>();

                for (var i = 0; i < 4; i++)
                {
                    podLoadModels.Add(new PodLoad()
                    {
                        PodIndex = i,
                        LoadAmount = 0
                    });
                    ;
                }

                model.PodLoadModels = podLoadModels;

                model.PodLoadModels[0].LoadAmount = 1;
            }


            model.ProductHaulInfoModel.PodLoadModels = model.PodLoadModels;

            model.ShippingLoadSheetModel.BulkPlantId = productHaulLoad.BulkPlant.Id;
            model.ShippingLoadSheetModel.BulkPlantName = productHaulLoad.BulkPlant.Name;

            if (isThird)
            {
                model.ProductHaulInfoModel.ThirdPartyBulkerCrewId = crewId;

                model.ProductHaulInfoModel.IsThirdParty = true;
            }
            else
            {
                model.ProductHaulInfoModel.CrewId = crewId;
            }

            return model;
        }


        private ScheduleProductHaulFromRigJobBlendViewModel PreparingBasicData(bool IsGowithCrew, bool IsThird,int existingHaulId, int crewId)
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



            if (existingHaulId != 0)
            {

                ProductHaul existingHaul = eServiceOnlineGateway.Instance.GetProductHaulById(existingHaulId);

                model.ProductHaulInfoModel.IsExistingHaul = true;
                model.ProductHaulInfoModel.ProductHaulId = existingHaul.Id;
                model.ProductHaulInfoModel.BulkPlantId = existingHaul.BulkPlant.Id;
                model.ProductHaulInfoModel.ExpectedOnLocationTime = existingHaul.ExpectedOnLocationTime;
                model.ProductHaulInfoModel.EstimatedTravelTime = existingHaul.EstimatedTravelTime;
                model.ProductHaulInfoModel.EstimatedLoadTime = existingHaul.EstimatedLoadTime;
                model.ProductHaulInfoModel.CrewId = 0;

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


                List<PodLoad> listPodLoads = eServiceOnlineGateway.Instance.GetPodLoadsByProductHaul(existingHaul.Id);

                if (listPodLoads == null || listPodLoads.Count == 0)
                {
                    model.PodLoadModels[0].LoadAmount = 1;
                }
                else
                {
                    model.PodLoadModels[0].LoadAmount = listPodLoads[0].LoadAmount;
                    model.PodLoadModels[1].LoadAmount = 1;
                }
            }
            else
            {
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

            }

            model.ProductHaulInfoModel.PodLoadModels = model.PodLoadModels;

            return model;
        }


        [Test]
        public void TestScheduleProductHaulbyBlend()
        {
            //1. Prepare test data
            int crewId = PrepareSanjelCrewData("HaulBlend");
            int productLoadInfoId = 0;

            Assert.NotNull(callSheet);
            Assert.NotNull(blendSection);
            Assert.NotNull(rig);
            Assert.NotNull(rigJob);
            Assert.NotNull(binInformations);
            Assert.AreNotEqual(binInformations, 0);
            Assert.AreNotEqual(crewId, 0);

            ProductLoadInfoModel productLoadInfoModel = PreparingBlendData(true);
            //create blend request
            eServiceWebContext.Instance.CreateBlendRequest(productLoadInfoModel);
            productLoadInfoId = productLoadInfoModel.ProductHaulLoadId;

            //Update the status of BulkerCrewLog
            BulkerCrewLog bulkerCrewLog = eServiceOnlineGateway.Instance.GetBulkerCrewLog(crewId);
            bulkerCrewLog.CrewStatus = BulkerCrewStatus.OffDuty;
            eServiceOnlineGateway.Instance.UpdateBulkerCrewLog(bulkerCrewLog);
            bulkerCrewLog = eServiceOnlineGateway.Instance.GetBulkerCrewLog(crewId);

            //2.Check if test data meet preconditions
            Assert.AreEqual(BulkerCrewStatus.OffDuty, bulkerCrewLog.CrewStatus);
            Assert.AreNotEqual(productLoadInfoId, 0);
            ProductHaulLoad currentProductHaulLoad =
                eServiceOnlineGateway.Instance.GetProductHaulLoadById(productLoadInfoId);
            Assert.NotNull(currentProductHaulLoad);

            //Prepare Haul blend data
            HaulBlendFromRigJobBlendViewModel haulBlendFromRigJobBlendViewModel =
                PreparingHaulBlendData(productLoadInfoId, crewId,0,false,false);

            //3.Execute the method to test
            eServiceWebContext.Instance.CreateHaulBlend(haulBlendFromRigJobBlendViewModel.RigJobId,
                haulBlendFromRigJobBlendViewModel.LoggedUser,
                haulBlendFromRigJobBlendViewModel.ProductLoadInfoModel,
                haulBlendFromRigJobBlendViewModel.ProductHaulInfoModel,
                haulBlendFromRigJobBlendViewModel.ShippingLoadSheetModel);

            //4. Verify the result meets post-conditions
            //A Blend Request (ProductHaulLoad) is scheduled
            ProductHaul productHaul = eServiceOnlineGateway.Instance.GetProductHaulByCrew(crewId).FirstOrDefault();

            Assert.NotNull(productHaul);
            Assert.AreNotEqual(productHaul.Id, 0);
            Assert.AreEqual(productHaul.ProductHaulLifeStatus, ProductHaulStatus.Scheduled);

            listProductHaulId.Add(productHaul.Id);

            //A Product Haul is scheduled with Pod allocations specified
            List<PodLoad> podLoads = eServiceOnlineGateway.Instance.GetPodLoadsByProductHaul(productHaul.Id);
            Assert.NotNull(podLoads);

            //A Shipping Load Sheet is scheduled with Blend Unload Sheet(s) specified.
            List<ShippingLoadSheet> shippingLoadSheets =
                eServiceOnlineGateway.Instance.GetShippingLoadSheetByProductHaulId(productHaul.Id);
            Assert.NotNull(shippingLoadSheets);
            Assert.AreNotEqual(shippingLoadSheets.Count, 0);
            foreach (var shippingLoadSheet in shippingLoadSheets)
            {
                ProductHaulLoad productHaulLoad =
                    eServiceOnlineGateway.Instance.GetProductHaulLoadById(shippingLoadSheet.ProductHaulLoad.Id);
                Assert.NotNull(productHaulLoad);

                List<BlendUnloadSheet> blendUnloadSheets =
                    eServiceOnlineGateway.Instance.GetBlendUnloadSheetByshippingLoadSheetId(shippingLoadSheet.Id);
                Assert.NotNull(blendUnloadSheets);
            }

            //A crew assignment is scheduled (RigJobSanjelCrewSection)
            RigJobSanjelCrewSection sanjelCrewSection =
                eServiceOnlineGateway.Instance.GetRigJobCrewSectionsByProductHaul(productHaul.Id);
            Assert.NotNull(sanjelCrewSection);
            Assert.AreEqual(sanjelCrewSection.RigJobCrewSectionStatus, RigJobCrewSectionStatus.Scheduled);

            //Bulker crew schedule is created for the round trip. Start Time = Expected On Location Time - Estimated Travel Time, End Time = Expected On Location Time + Estimated Travel Time.
            double nEstimatedTravelTime = productHaul.EstimatedTravelTime;
            SanjelCrewSchedule currentsanjelCrewSchedule =
                eServiceOnlineGateway.Instance.GetSanjelCrewSchedulesByRigJobSanjelCrewSection(sanjelCrewSection);
            Assert.NotNull(currentsanjelCrewSchedule);
            Assert.AreEqual(currentsanjelCrewSchedule.StartTime,
                productHaul.ExpectedOnLocationTime.AddHours(-nEstimatedTravelTime));
            Assert.AreEqual(currentsanjelCrewSchedule.EndTime,
                productHaul.ExpectedOnLocationTime.AddHours(productHaul.EstimatedTravelTime));

            //AThe bulker crew status is in LoadRequested status
            BulkerCrewLog currentBulkerCrewLog = eServiceOnlineGateway.Instance.GetBulkerCrewLog(crewId);
            Assert.NotNull(bulkerCrewLog);
            Assert.NotNull(currentBulkerCrewLog);
            Assert.AreNotEqual(bulkerCrewLog.CrewStatus, currentBulkerCrewLog.CrewStatus);
            Assert.AreEqual(currentBulkerCrewLog.CrewStatus, BulkerCrewStatus.LoadRequested);

            //5. Clean up test data
            DeleteProductHaul(productHaul.Id, true);

            //6. Double check all data are cleaned
            var cleanedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaul.Id);
            Assert.IsNull(cleanedProductHaul);

            //After delete a ProductHaul, check whether the associated table object is created successfully
            List<ShippingLoadSheet> cleanedShippingLoadSheets =
                eServiceOnlineGateway.Instance.GetShippingLoadSheetByProductHaulId(productHaul.Id);
            Assert.AreEqual(cleanedShippingLoadSheets.Count, 0);
            foreach (var shippingLoadSheet in shippingLoadSheets)
            {
                ProductHaulLoad productHaulLoad =
                    eServiceOnlineGateway.Instance.GetProductHaulLoadById(shippingLoadSheet.ProductHaulLoad.Id);
                Assert.Null(productHaulLoad);

                List<BlendUnloadSheet> blendUnloadSheets =
                    eServiceOnlineGateway.Instance.GetBlendUnloadSheetByshippingLoadSheetId(shippingLoadSheet.Id);
                Assert.AreEqual(blendUnloadSheets.Count, 0);
            }

            List<PodLoad> cleanedpodLoads = eServiceOnlineGateway.Instance.GetPodLoadsByProductHaul(productHaul.Id);
            Assert.AreEqual(cleanedpodLoads.Count, 0);
        }

        [Test]
        public void TestScheduleProductHaulbyBlendGoWithCrew()
        {
            //1. Prepare test data
            int crewId = PrepareSanjelCrewData("BlendGoWithCrew");
            int productLoadInfoId = 0;

            Assert.NotNull(callSheet);
            Assert.NotNull(blendSection);
            Assert.NotNull(rig);
            Assert.NotNull(rigJob);
            Assert.NotNull(binInformations);
            Assert.AreNotEqual(binInformations, 0);
            Assert.AreNotEqual(crewId, 0);

            ProductLoadInfoModel productLoadInfoModel = PreparingBlendData(true);
            //create blend request
            eServiceWebContext.Instance.CreateBlendRequest(productLoadInfoModel);
            productLoadInfoId = productLoadInfoModel.ProductHaulLoadId;

            //Update the status of BulkerCrewLog
            BulkerCrewLog bulkerCrewLog = eServiceOnlineGateway.Instance.GetBulkerCrewLog(crewId);
            bulkerCrewLog.CrewStatus = BulkerCrewStatus.OffDuty;
            eServiceOnlineGateway.Instance.UpdateBulkerCrewLog(bulkerCrewLog);
            BulkerCrewLog UpdatebulkerCrewLog = eServiceOnlineGateway.Instance.GetBulkerCrewLog(crewId);

            //2.Check if test data meet preconditions
            Assert.AreEqual(BulkerCrewStatus.OffDuty, UpdatebulkerCrewLog.CrewStatus);
            Assert.AreNotEqual(productLoadInfoId, 0);
            ProductHaulLoad currentProductHaulLoad =
                eServiceOnlineGateway.Instance.GetProductHaulLoadById(productLoadInfoId);
            Assert.NotNull(currentProductHaulLoad);

            //Prepare Haul blend data
            HaulBlendFromRigJobBlendViewModel haulBlendFromRigJobBlendViewModel =
                PreparingHaulBlendData(productLoadInfoId, crewId,0,false,false);

            //3.Execute the method to test
            eServiceWebContext.Instance.CreateHaulBlend(haulBlendFromRigJobBlendViewModel.RigJobId,
                haulBlendFromRigJobBlendViewModel.LoggedUser,
                haulBlendFromRigJobBlendViewModel.ProductLoadInfoModel,
                haulBlendFromRigJobBlendViewModel.ProductHaulInfoModel,
                haulBlendFromRigJobBlendViewModel.ShippingLoadSheetModel);

            //4. Verify the result meets post-conditions
            //A Blend Request (ProductHaulLoad) is scheduled
            ProductHaul productHaul = eServiceOnlineGateway.Instance.GetProductHaulByCrew(crewId).FirstOrDefault();

            Assert.NotNull(productHaul);
            Assert.AreNotEqual(productHaul.Id, 0);
            Assert.AreEqual(productHaul.ProductHaulLifeStatus, ProductHaulStatus.Scheduled);
            Assert.AreEqual(productHaul.IsGoWithCrew, true);

            listProductHaulId.Add(productHaul.Id);

            //A Product Haul is scheduled with Pod allocations specified
            List<PodLoad> podLoads = eServiceOnlineGateway.Instance.GetPodLoadsByProductHaul(productHaul.Id);
            Assert.NotNull(podLoads);

            //A Shipping Load Sheet is scheduled with Blend Unload Sheet(s) specified.
            List<ShippingLoadSheet> shippingLoadSheets =
                eServiceOnlineGateway.Instance.GetShippingLoadSheetByProductHaulId(productHaul.Id);
            Assert.NotNull(shippingLoadSheets);
            Assert.AreNotEqual(shippingLoadSheets.Count, 0);
            foreach (var shippingLoadSheet in shippingLoadSheets)
            {
                ProductHaulLoad productHaulLoad =
                    eServiceOnlineGateway.Instance.GetProductHaulLoadById(shippingLoadSheet.ProductHaulLoad.Id);
                Assert.NotNull(productHaulLoad);

                List<BlendUnloadSheet> blendUnloadSheets =
                    eServiceOnlineGateway.Instance.GetBlendUnloadSheetByshippingLoadSheetId(shippingLoadSheet.Id);
                Assert.AreEqual(blendUnloadSheets.Count,0);
            }

            //A crew assignment is scheduled (RigJobSanjelCrewSection)
            RigJobSanjelCrewSection sanjelCrewSection =
                eServiceOnlineGateway.Instance.GetRigJobCrewSectionsByProductHaul(productHaul.Id);
            Assert.NotNull(sanjelCrewSection);
            Assert.AreEqual(sanjelCrewSection.RigJobCrewSectionStatus, RigJobCrewSectionStatus.Scheduled);

            //Bulker crew schedule is created for the round trip. Start Time = Expected On Location Time - Estimated Travel Time, End Time = Expected On Location Time + Estimated Travel Time.
            double nEstimatedTravelTime = productHaul.EstimatedTravelTime;
            SanjelCrewSchedule currentsanjelCrewSchedule =
                eServiceOnlineGateway.Instance.GetSanjelCrewSchedulesByRigJobSanjelCrewSection(sanjelCrewSection);
            Assert.NotNull(currentsanjelCrewSchedule);
            Assert.AreEqual(currentsanjelCrewSchedule.StartTime, rigJob.CallCrewTime);
            Assert.AreEqual(currentsanjelCrewSchedule.EndTime, rigJob.JobDateTime.AddMinutes(rigJob.JobDuration == 0 ? 360 : rigJob.JobDuration));

            //AThe bulker crew status is in LoadRequested status
            BulkerCrewLog currentBulkerCrewLog = eServiceOnlineGateway.Instance.GetBulkerCrewLog(crewId);
            Assert.NotNull(UpdatebulkerCrewLog);
            Assert.NotNull(currentBulkerCrewLog);
            Assert.AreNotEqual(UpdatebulkerCrewLog.CrewStatus, currentBulkerCrewLog.CrewStatus);
            Assert.AreEqual(currentBulkerCrewLog.CrewStatus, BulkerCrewStatus.LoadRequested);

            //5. Clean up test data
            DeleteProductHaul(productHaul.Id, true);

            //6. Double check all data are cleaned
            var cleanedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaul.Id);
            Assert.IsNull(cleanedProductHaul);

            //After delete a ProductHaul, check whether the associated table object is created successfully
            List<ShippingLoadSheet> cleanedShippingLoadSheets =
                eServiceOnlineGateway.Instance.GetShippingLoadSheetByProductHaulId(productHaul.Id);
            Assert.AreEqual(cleanedShippingLoadSheets.Count, 0);
            foreach (var shippingLoadSheet in shippingLoadSheets)
            {
                ProductHaulLoad productHaulLoad =
                    eServiceOnlineGateway.Instance.GetProductHaulLoadById(shippingLoadSheet.ProductHaulLoad.Id);
                Assert.Null(productHaulLoad);

                List<BlendUnloadSheet> blendUnloadSheets =
                    eServiceOnlineGateway.Instance.GetBlendUnloadSheetByshippingLoadSheetId(shippingLoadSheet.Id);
                Assert.AreEqual(blendUnloadSheets.Count, 0);
            }

            List<PodLoad> cleanedpodLoads = eServiceOnlineGateway.Instance.GetPodLoadsByProductHaul(productHaul.Id);
            Assert.AreEqual(cleanedpodLoads.Count, 0);
        }

        [Test]
        public void TestScheduleProductHaulbyExistingHaul()
        {
            //1. Prepare test data
            int crewId = PrepareSanjelCrewData("ExistingHaul");
            int productLoadInfoId = 0;

            Assert.NotNull(callSheet);
            Assert.NotNull(blendSection);
            Assert.NotNull(rig);
            Assert.NotNull(rigJob);
            Assert.NotNull(binInformations);
            Assert.AreNotEqual(binInformations, 0);
            Assert.AreNotEqual(crewId, 0);


            //Update the status of BulkerCrewLog
            BulkerCrewLog bulkerCrewLog = eServiceOnlineGateway.Instance.GetBulkerCrewLog(crewId);
            bulkerCrewLog.CrewStatus = BulkerCrewStatus.OffDuty;
            eServiceOnlineGateway.Instance.UpdateBulkerCrewLog(bulkerCrewLog);
            BulkerCrewLog UpdatebulkerCrewLog = eServiceOnlineGateway.Instance.GetBulkerCrewLog(crewId);


            ScheduleProductHaulFromRigJobBlendViewModel scheduleProductHaulFromRigJobBlendViewModel
                = PreparingBasicData(false, false, 0, crewId);

            //create existingHaul
            eServiceWebContext.Instance.ScheduleProductHaul(scheduleProductHaulFromRigJobBlendViewModel);

            //Check if existingHaul has been scheduled
            ProductHaul existingHaul = eServiceOnlineGateway.Instance.GetProductHaulByCrew(crewId).FirstOrDefault();

            Assert.NotNull(existingHaul);
            Assert.AreNotEqual(existingHaul.Id, 0);
            Assert.AreEqual(existingHaul.ProductHaulLifeStatus, ProductHaulStatus.Scheduled);


            ProductLoadInfoModel productLoadInfoModel = PreparingBlendData(true);
            //create blend request
            eServiceWebContext.Instance.CreateBlendRequest(productLoadInfoModel);
            productLoadInfoId = productLoadInfoModel.ProductHaulLoadId;

            //2.Check if test data meet preconditions
            Assert.AreEqual(BulkerCrewStatus.OffDuty, UpdatebulkerCrewLog.CrewStatus);
            Assert.AreNotEqual(productLoadInfoId, 0);
            ProductHaulLoad currentProductHaulLoad =
                eServiceOnlineGateway.Instance.GetProductHaulLoadById(productLoadInfoId);
            Assert.NotNull(currentProductHaulLoad);

            //Prepare Haul blend data
            HaulBlendFromRigJobBlendViewModel haulBlendFromRigJobBlendViewModel =
                PreparingHaulBlendData(productLoadInfoId, crewId, existingHaul.Id, false,false);

            //3.Execute the method to test
            eServiceWebContext.Instance.CreateHaulBlend(haulBlendFromRigJobBlendViewModel.RigJobId,
                haulBlendFromRigJobBlendViewModel.LoggedUser,
                haulBlendFromRigJobBlendViewModel.ProductLoadInfoModel,
                haulBlendFromRigJobBlendViewModel.ProductHaulInfoModel,
                haulBlendFromRigJobBlendViewModel.ShippingLoadSheetModel);

            //4. Verify the result meets post-conditions
            //A Blend Request (ProductHaulLoad) is scheduled
            ProductHaul productHaul = eServiceOnlineGateway.Instance.GetProductHaulByCrew(crewId).ToList().FirstOrDefault();

            Assert.NotNull(productHaul);
            Assert.AreNotEqual(productHaul.Id, 0);
            Assert.AreEqual(productHaul.ProductHaulLifeStatus, ProductHaulStatus.Scheduled);
            Assert.AreEqual(productHaul.Id, existingHaul.Id);


            //A Product Haul is scheduled with Pod allocations specified
            List<PodLoad> podLoads = eServiceOnlineGateway.Instance.GetPodLoadsByProductHaul(productHaul.Id);
            Assert.NotNull(podLoads);
            Assert.AreEqual(podLoads[0].LoadAmount/1000, 1);
            Assert.AreEqual(podLoads[1].LoadAmount/1000, 1);

            //A Shipping Load Sheet is scheduled with Blend Unload Sheet(s) specified.
            List<ShippingLoadSheet> shippingLoadSheets =
                eServiceOnlineGateway.Instance.GetShippingLoadSheetByProductHaulId(productHaul.Id);
            Assert.NotNull(shippingLoadSheets);
            Assert.AreNotEqual(shippingLoadSheets.Count, 0);
            foreach (var shippingLoadSheet in shippingLoadSheets)
            {
                ProductHaulLoad productHaulLoad =
                    eServiceOnlineGateway.Instance.GetProductHaulLoadById(shippingLoadSheet.ProductHaulLoad.Id);
                Assert.NotNull(productHaulLoad);

                List<BlendUnloadSheet> blendUnloadSheets =
                    eServiceOnlineGateway.Instance.GetBlendUnloadSheetByshippingLoadSheetId(shippingLoadSheet.Id);
                Assert.NotNull(blendUnloadSheets);
            }


            //AThe bulker crew status is in LoadRequested status
            BulkerCrewLog currentBulkerCrewLog = eServiceOnlineGateway.Instance.GetBulkerCrewLog(crewId);
            Assert.NotNull(UpdatebulkerCrewLog);
            Assert.NotNull(currentBulkerCrewLog);
            Assert.AreEqual(UpdatebulkerCrewLog.CrewStatus, currentBulkerCrewLog.CrewStatus);
            Assert.AreEqual(currentBulkerCrewLog.CrewStatus, BulkerCrewStatus.LoadRequested);

            //5. Clean up test data
            DeleteProductHaul(productHaul.Id, true);

            //6. Double check all data are cleaned
            var cleanedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaul.Id);
            Assert.IsNull(cleanedProductHaul);

            //After delete a ProductHaul, check whether the associated table object is created successfully
            List<ShippingLoadSheet> cleanedShippingLoadSheets =
                eServiceOnlineGateway.Instance.GetShippingLoadSheetByProductHaulId(productHaul.Id);
            Assert.AreEqual(cleanedShippingLoadSheets.Count, 0);
            foreach (var shippingLoadSheet in shippingLoadSheets)
            {
                ProductHaulLoad productHaulLoad =
                    eServiceOnlineGateway.Instance.GetProductHaulLoadById(shippingLoadSheet.ProductHaulLoad.Id);
                Assert.Null(productHaulLoad);

                List<BlendUnloadSheet> blendUnloadSheets =
                    eServiceOnlineGateway.Instance.GetBlendUnloadSheetByshippingLoadSheetId(shippingLoadSheet.Id);
                Assert.AreEqual(blendUnloadSheets.Count, 0);
            }

            List<PodLoad> cleanedpodLoads = eServiceOnlineGateway.Instance.GetPodLoadsByProductHaul(productHaul.Id);
            Assert.AreEqual(cleanedpodLoads.Count, 0);
        }

        [Test]
        public void TestScheduleProductHaulbyBlendThirdParty()
        {
            //1. Prepare test data
            int crewId = PrepareThirdPartyBulkerCrewData("BlendThirdParty");
            int productLoadInfoId = 0;

            Assert.NotNull(callSheet);
            Assert.NotNull(blendSection);
            Assert.NotNull(rig);
            Assert.NotNull(rigJob);
            Assert.NotNull(binInformations);
            Assert.AreNotEqual(binInformations, 0);
            Assert.AreNotEqual(crewId, 0);

            ProductLoadInfoModel productLoadInfoModel = PreparingBlendData(true);
            //create blend request
            eServiceWebContext.Instance.CreateBlendRequest(productLoadInfoModel);
            productLoadInfoId = productLoadInfoModel.ProductHaulLoadId;

            //Update the status of BulkerCrewLog
            BulkerCrewLog bulkerCrewLog = eServiceOnlineGateway.Instance.GetThirdPartyBulkerCrewLog(crewId);
            bulkerCrewLog.CrewStatus = BulkerCrewStatus.OffDuty;
            eServiceOnlineGateway.Instance.UpdateBulkerCrewLog(bulkerCrewLog);
            BulkerCrewLog UpdatebulkerCrewLog = eServiceOnlineGateway.Instance.GetThirdPartyBulkerCrewLog(crewId);

            //2.Check if test data meet preconditions
            Assert.AreEqual(BulkerCrewStatus.OffDuty, UpdatebulkerCrewLog.CrewStatus);
            Assert.AreNotEqual(productLoadInfoId, 0);
            ProductHaulLoad currentProductHaulLoad =
                eServiceOnlineGateway.Instance.GetProductHaulLoadById(productLoadInfoId);
            Assert.NotNull(currentProductHaulLoad);

            //Prepare Haul blend data
            HaulBlendFromRigJobBlendViewModel haulBlendFromRigJobBlendViewModel =
                PreparingHaulBlendData(productLoadInfoId, crewId, 0, false,true);

            //3.Execute the method to test
            eServiceWebContext.Instance.CreateHaulBlend(haulBlendFromRigJobBlendViewModel.RigJobId,
                haulBlendFromRigJobBlendViewModel.LoggedUser,
                haulBlendFromRigJobBlendViewModel.ProductLoadInfoModel,
                haulBlendFromRigJobBlendViewModel.ProductHaulInfoModel,
                haulBlendFromRigJobBlendViewModel.ShippingLoadSheetModel);

            //4. Verify the result meets post-conditions
            //A Blend Request (ProductHaulLoad) is scheduled
            ProductHaul productHaul = eServiceOnlineGateway.Instance.GetProductHaulByCrew(crewId).FirstOrDefault();

            Assert.NotNull(productHaul);
            Assert.AreNotEqual(productHaul.Id, 0);
            Assert.AreEqual(productHaul.ProductHaulLifeStatus, ProductHaulStatus.Scheduled);

            listProductHaulId.Add(productHaul.Id);

            //A Product Haul is scheduled with Pod allocations specified
            List<PodLoad> podLoads = eServiceOnlineGateway.Instance.GetPodLoadsByProductHaul(productHaul.Id);
            Assert.NotNull(podLoads);

            //A Shipping Load Sheet is scheduled with Blend Unload Sheet(s) specified.
            List<ShippingLoadSheet> shippingLoadSheets =
                eServiceOnlineGateway.Instance.GetShippingLoadSheetByProductHaulId(productHaul.Id);
            Assert.NotNull(shippingLoadSheets);
            Assert.AreNotEqual(shippingLoadSheets.Count, 0);
            foreach (var shippingLoadSheet in shippingLoadSheets)
            {
                ProductHaulLoad productHaulLoad =
                    eServiceOnlineGateway.Instance.GetProductHaulLoadById(shippingLoadSheet.ProductHaulLoad.Id);
                Assert.NotNull(productHaulLoad);

                List<BlendUnloadSheet> blendUnloadSheets =
                    eServiceOnlineGateway.Instance.GetBlendUnloadSheetByshippingLoadSheetId(shippingLoadSheet.Id);
                Assert.NotNull(blendUnloadSheets);
            }

            //A crew assignment is scheduled (RigJobThirdPartyBulkerCrewSection)
            RigJobThirdPartyBulkerCrewSection thirdPartyBulkerSection =
                eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionsByProductHual(productHaul.Id);
            Assert.NotNull(thirdPartyBulkerSection);
            Assert.AreEqual(thirdPartyBulkerSection.RigJobCrewSectionStatus, RigJobCrewSectionStatus.Scheduled);

            //Bulker crew schedule is created for the round trip. Start Time = Expected On Location Time - Estimated Travel Time, End Time = Expected On Location Time + Estimated Travel Time.
            double nEstimatedTravelTime = productHaul.EstimatedTravelTime;

            ThirdPartyBulkerCrewSchedule currentsanjelCrewSchedule =
                eServiceOnlineGateway.Instance.GetThirdPartyCrewScheduleByRigJobThirdPartyBulkerCrewSection(thirdPartyBulkerSection);

            Assert.NotNull(currentsanjelCrewSchedule);
            Assert.AreEqual(currentsanjelCrewSchedule.StartTime,
                productHaul.ExpectedOnLocationTime.AddHours(-nEstimatedTravelTime));
            Assert.AreEqual(currentsanjelCrewSchedule.EndTime,
                productHaul.ExpectedOnLocationTime.AddHours(productHaul.EstimatedTravelTime));

            //AThe bulker crew status is in LoadRequested status
            BulkerCrewLog currentBulkerCrewLog = eServiceOnlineGateway.Instance.GetThirdPartyBulkerCrewLog(crewId);
            Assert.NotNull(UpdatebulkerCrewLog);
            Assert.NotNull(currentBulkerCrewLog);
            Assert.AreNotEqual(UpdatebulkerCrewLog.CrewStatus, currentBulkerCrewLog.CrewStatus);
            Assert.AreEqual(currentBulkerCrewLog.CrewStatus, BulkerCrewStatus.LoadRequested);

            //5. Clean up test data
            DeleteProductHaul(productHaul.Id, true);

            //6. Double check all data are cleaned
            var cleanedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaul.Id);
            Assert.IsNull(cleanedProductHaul);

            //After delete a ProductHaul, check whether the associated table object is created successfully
            List<ShippingLoadSheet> cleanedShippingLoadSheets =
                eServiceOnlineGateway.Instance.GetShippingLoadSheetByProductHaulId(productHaul.Id);
            Assert.AreEqual(cleanedShippingLoadSheets.Count, 0);
            foreach (var shippingLoadSheet in shippingLoadSheets)
            {
                ProductHaulLoad productHaulLoad =
                    eServiceOnlineGateway.Instance.GetProductHaulLoadById(shippingLoadSheet.ProductHaulLoad.Id);
                Assert.Null(productHaulLoad);

                List<BlendUnloadSheet> blendUnloadSheets =
                    eServiceOnlineGateway.Instance.GetBlendUnloadSheetByshippingLoadSheetId(shippingLoadSheet.Id);
                Assert.AreEqual(blendUnloadSheets.Count, 0);
            }

            List<PodLoad> cleanedpodLoads = eServiceOnlineGateway.Instance.GetPodLoadsByProductHaul(productHaul.Id);
            Assert.AreEqual(cleanedpodLoads.Count, 0);
        }


        [Test]
        public void TestScheduleProductHaulbyGoWithCrew()
        {
            //1. Prepare test data
            int crewId = PrepareThirdPartyBulkerCrewData("GoWithCrew");
            int productLoadInfoId = 0;

            Assert.NotNull(callSheet);
            Assert.NotNull(blendSection);
            Assert.NotNull(rig);
            Assert.NotNull(rigJob);
            Assert.NotNull(binInformations);
            Assert.AreNotEqual(binInformations, 0);
            Assert.AreNotEqual(crewId, 0);


            //Update the status of BulkerCrewLog
            BulkerCrewLog bulkerCrewLog = eServiceOnlineGateway.Instance.GetThirdPartyBulkerCrewLog(crewId);
            bulkerCrewLog.CrewStatus = BulkerCrewStatus.OffDuty;
            eServiceOnlineGateway.Instance.UpdateBulkerCrewLog(bulkerCrewLog);
            BulkerCrewLog UpdatebulkerCrewLog = eServiceOnlineGateway.Instance.GetThirdPartyBulkerCrewLog(crewId);


            //2.Execute the method to test
            //Prepare Haul blend data
            ScheduleProductHaulFromRigJobBlendViewModel scheduleProductHaulFromRigJobBlendViewModel
                = PreparingBasicData(true, false, 0, crewId);

            //create existingHaul
            eServiceWebContext.Instance.ScheduleProductHaul(scheduleProductHaulFromRigJobBlendViewModel);

            //3. Verify the result meets post-conditions
            //A Blend Request (ProductHaulLoad) is scheduled
            ProductHaul productHaul = eServiceOnlineGateway.Instance.GetProductHaulByCrew(crewId).FirstOrDefault();

            Assert.NotNull(productHaul);
            Assert.AreNotEqual(productHaul.Id, 0);
            Assert.AreEqual(productHaul.ProductHaulLifeStatus, ProductHaulStatus.Scheduled);
            Assert.AreEqual(productHaul.IsGoWithCrew, true);

            listProductHaulId.Add(productHaul.Id);

            //A Product Haul is scheduled with Pod allocations specified
            List<PodLoad> podLoads = eServiceOnlineGateway.Instance.GetPodLoadsByProductHaul(productHaul.Id);
            Assert.NotNull(podLoads);

            //A Shipping Load Sheet is scheduled with Blend Unload Sheet(s) specified.
            List<ShippingLoadSheet> shippingLoadSheets =
                eServiceOnlineGateway.Instance.GetShippingLoadSheetByProductHaulId(productHaul.Id);
            Assert.NotNull(shippingLoadSheets);
            Assert.AreNotEqual(shippingLoadSheets.Count, 0);
            foreach (var shippingLoadSheet in shippingLoadSheets)
            {
                ProductHaulLoad productHaulLoad =
                    eServiceOnlineGateway.Instance.GetProductHaulLoadById(shippingLoadSheet.ProductHaulLoad.Id);
                Assert.NotNull(productHaulLoad);

                List<BlendUnloadSheet> blendUnloadSheets =
                    eServiceOnlineGateway.Instance.GetBlendUnloadSheetByshippingLoadSheetId(shippingLoadSheet.Id);
                Assert.AreEqual(blendUnloadSheets.Count, 0);
            }

            //A crew assignment is scheduled (RigJobSanjelCrewSection)
            RigJobSanjelCrewSection sanjelCrewSection =
                eServiceOnlineGateway.Instance.GetRigJobCrewSectionsByProductHaul(productHaul.Id);
            Assert.NotNull(sanjelCrewSection);
            Assert.AreEqual(sanjelCrewSection.RigJobCrewSectionStatus, RigJobCrewSectionStatus.Scheduled);

            //Bulker crew schedule is created for the round trip. Start Time = Expected On Location Time - Estimated Travel Time, End Time = Expected On Location Time + Estimated Travel Time.
            double nEstimatedTravelTime = productHaul.EstimatedTravelTime;
            SanjelCrewSchedule currentsanjelCrewSchedule =
                eServiceOnlineGateway.Instance.GetSanjelCrewSchedulesByRigJobSanjelCrewSection(sanjelCrewSection);
            Assert.NotNull(currentsanjelCrewSchedule);
            Assert.AreEqual(currentsanjelCrewSchedule.StartTime, rigJob.CallCrewTime);
            Assert.AreEqual(currentsanjelCrewSchedule.EndTime, rigJob.JobDateTime.AddMinutes(rigJob.JobDuration == 0 ? 360 : rigJob.JobDuration));

            //AThe bulker crew status is in LoadRequested status
            BulkerCrewLog currentBulkerCrewLog = eServiceOnlineGateway.Instance.GetBulkerCrewLog(crewId);
            Assert.NotNull(UpdatebulkerCrewLog);
            Assert.NotNull(currentBulkerCrewLog);
            Assert.AreNotEqual(UpdatebulkerCrewLog.CrewStatus, currentBulkerCrewLog.CrewStatus);
            Assert.AreEqual(currentBulkerCrewLog.CrewStatus, BulkerCrewStatus.LoadRequested);

            //4. Clean up test data
            DeleteProductHaul(productHaul.Id, true);

            //5. Double check all data are cleaned
            var cleanedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaul.Id);
            Assert.IsNull(cleanedProductHaul);

            //After delete a ProductHaul, check whether the associated table object is created successfully
            List<ShippingLoadSheet> cleanedShippingLoadSheets =
                eServiceOnlineGateway.Instance.GetShippingLoadSheetByProductHaulId(productHaul.Id);
            Assert.AreEqual(cleanedShippingLoadSheets.Count, 0);
            foreach (var shippingLoadSheet in shippingLoadSheets)
            {
                ProductHaulLoad productHaulLoad =
                    eServiceOnlineGateway.Instance.GetProductHaulLoadById(shippingLoadSheet.ProductHaulLoad.Id);
                Assert.Null(productHaulLoad);

                List<BlendUnloadSheet> blendUnloadSheets =
                    eServiceOnlineGateway.Instance.GetBlendUnloadSheetByshippingLoadSheetId(shippingLoadSheet.Id);
                Assert.AreEqual(blendUnloadSheets.Count, 0);
            }

            List<PodLoad> cleanedpodLoads = eServiceOnlineGateway.Instance.GetPodLoadsByProductHaul(productHaul.Id);
            Assert.AreEqual(cleanedpodLoads.Count, 0);
        }

        [Test]
        public void TestScheduleProductHaulbyThirdParty()
        {
            //1. Prepare test data
            int crewId = PrepareThirdPartyBulkerCrewData("ThirdParty");
            int productLoadInfoId = 0;

            Assert.NotNull(callSheet);
            Assert.NotNull(blendSection);
            Assert.NotNull(rig);
            Assert.NotNull(rigJob);
            Assert.NotNull(binInformations);
            Assert.AreNotEqual(binInformations, 0);
            Assert.AreNotEqual(crewId, 0);


            //Update the status of BulkerCrewLog
            BulkerCrewLog bulkerCrewLog = eServiceOnlineGateway.Instance.GetThirdPartyBulkerCrewLog(crewId);
            bulkerCrewLog.CrewStatus = BulkerCrewStatus.OffDuty;
            eServiceOnlineGateway.Instance.UpdateBulkerCrewLog(bulkerCrewLog);
            BulkerCrewLog UpdatebulkerCrewLog = eServiceOnlineGateway.Instance.GetThirdPartyBulkerCrewLog(crewId);


            //2.Execute the method to test
            //Prepare Haul blend data
            ScheduleProductHaulFromRigJobBlendViewModel scheduleProductHaulFromRigJobBlendViewModel
                = PreparingBasicData(false, true, 0, crewId);

            //create existingHaul
            eServiceWebContext.Instance.ScheduleProductHaul(scheduleProductHaulFromRigJobBlendViewModel);

            //3. Verify the result meets post-conditions
            //A Product Haul is scheduled
            ProductHaul productHaul = eServiceOnlineGateway.Instance.GetProductHaulByCrew(crewId).FirstOrDefault();

            Assert.NotNull(productHaul);
            Assert.AreNotEqual(productHaul.Id, 0);
            Assert.AreEqual(productHaul.ProductHaulLifeStatus, ProductHaulStatus.Scheduled);

            listProductHaulId.Add(productHaul.Id);

            //A Product Haul is scheduled with Pod allocations specified
            List<PodLoad> podLoads = eServiceOnlineGateway.Instance.GetPodLoadsByProductHaul(productHaul.Id);
            Assert.NotNull(podLoads);

            //A Shipping Load Sheet is scheduled with Blend Unload Sheet(s) specified.
            List<ShippingLoadSheet> shippingLoadSheets =
                eServiceOnlineGateway.Instance.GetShippingLoadSheetByProductHaulId(productHaul.Id);
            Assert.NotNull(shippingLoadSheets);
            Assert.AreNotEqual(shippingLoadSheets.Count, 0);
            foreach (var shippingLoadSheet in shippingLoadSheets)
            {
                ProductHaulLoad productHaulLoad =
                    eServiceOnlineGateway.Instance.GetProductHaulLoadById(shippingLoadSheet.ProductHaulLoad.Id);
                Assert.NotNull(productHaulLoad);

                List<BlendUnloadSheet> blendUnloadSheets =
                    eServiceOnlineGateway.Instance.GetBlendUnloadSheetByshippingLoadSheetId(shippingLoadSheet.Id);
                Assert.NotNull(blendUnloadSheets);
            }

            //A crew assignment is scheduled (RigJobThirdPartyBulkerCrewSection)
            RigJobThirdPartyBulkerCrewSection thirdPartyBulkerSection =
                eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionsByProductHual(productHaul.Id);
            Assert.NotNull(thirdPartyBulkerSection);
            Assert.AreEqual(thirdPartyBulkerSection.RigJobCrewSectionStatus, RigJobCrewSectionStatus.Scheduled);

            //Bulker crew schedule is created for the round trip. Start Time = Expected On Location Time - Estimated Travel Time, End Time = Expected On Location Time + Estimated Travel Time.
            double nEstimatedTravelTime = productHaul.EstimatedTravelTime;

            ThirdPartyBulkerCrewSchedule currentsanjelCrewSchedule =
                eServiceOnlineGateway.Instance.GetThirdPartyCrewScheduleByRigJobThirdPartyBulkerCrewSection(thirdPartyBulkerSection);

            Assert.NotNull(currentsanjelCrewSchedule);
            Assert.AreEqual(currentsanjelCrewSchedule.StartTime,
                productHaul.ExpectedOnLocationTime.AddHours(-nEstimatedTravelTime));
            Assert.AreEqual(currentsanjelCrewSchedule.EndTime,
                productHaul.ExpectedOnLocationTime.AddHours(productHaul.EstimatedTravelTime));

            //AThe bulker crew status is in LoadRequested status
            BulkerCrewLog currentBulkerCrewLog = eServiceOnlineGateway.Instance.GetThirdPartyBulkerCrewLog(crewId);
            Assert.NotNull(UpdatebulkerCrewLog);
            Assert.NotNull(currentBulkerCrewLog);
            Assert.AreNotEqual(UpdatebulkerCrewLog.CrewStatus, currentBulkerCrewLog.CrewStatus);
            Assert.AreEqual(currentBulkerCrewLog.CrewStatus, BulkerCrewStatus.LoadRequested);

            //5. Clean up test data
            DeleteProductHaul(productHaul.Id, true);

            //6. Double check all data are cleaned
            var cleanedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaul.Id);
            Assert.IsNull(cleanedProductHaul);

            //After delete a ProductHaul, check whether the associated table object is created successfully
            List<ShippingLoadSheet> cleanedShippingLoadSheets =
                eServiceOnlineGateway.Instance.GetShippingLoadSheetByProductHaulId(productHaul.Id);
            Assert.AreEqual(cleanedShippingLoadSheets.Count, 0);
            foreach (var shippingLoadSheet in shippingLoadSheets)
            {
                ProductHaulLoad productHaulLoad =
                    eServiceOnlineGateway.Instance.GetProductHaulLoadById(shippingLoadSheet.ProductHaulLoad.Id);
                Assert.Null(productHaulLoad);

                List<BlendUnloadSheet> blendUnloadSheets =
                    eServiceOnlineGateway.Instance.GetBlendUnloadSheetByshippingLoadSheetId(shippingLoadSheet.Id);
                Assert.AreEqual(blendUnloadSheets.Count, 0);
            }

            List<PodLoad> cleanedpodLoads = eServiceOnlineGateway.Instance.GetPodLoadsByProductHaul(productHaul.Id);
            Assert.AreEqual(cleanedpodLoads.Count, 0);
        }

        [Test]
        public void TestScheduleProductHaulWithOutBulkerCrewLog()
        {
            //1. Prepare test data
            int crewId =  PrepareSanjelCrewDataWithOutBulkerCrewLog("WithOutBulkerCrewLog");

            Assert.NotNull(callSheet);
            Assert.NotNull(blendSection);
            Assert.NotNull(rig);
            Assert.NotNull(rigJob);
            Assert.NotNull(binInformations);
            Assert.AreNotEqual(binInformations, 0);
            Assert.AreNotEqual(crewId, 0);


            //2.Execute the method to test
            //Prepare Haul blend data
            ScheduleProductHaulFromRigJobBlendViewModel scheduleProductHaulFromRigJobBlendViewModel
                = PreparingBasicData(false, false, 0, crewId);

            //create existingHaul
            eServiceWebContext.Instance.ScheduleProductHaul(scheduleProductHaulFromRigJobBlendViewModel);

            //3. Verify the result meets post-conditions
            //A Product Haul is scheduled
            ProductHaul productHaul = eServiceOnlineGateway.Instance.GetProductHaulByCrew(crewId).FirstOrDefault();

            Assert.NotNull(productHaul);
            Assert.AreNotEqual(productHaul.Id, 0);
            Assert.AreEqual(productHaul.ProductHaulLifeStatus, ProductHaulStatus.Scheduled);

            listProductHaulId.Add(productHaul.Id);

            //A Product Haul is scheduled with Pod allocations specified
            List<PodLoad> podLoads = eServiceOnlineGateway.Instance.GetPodLoadsByProductHaul(productHaul.Id);
            Assert.NotNull(podLoads);

            //A Shipping Load Sheet is scheduled with Blend Unload Sheet(s) specified.
            List<ShippingLoadSheet> shippingLoadSheets =
                eServiceOnlineGateway.Instance.GetShippingLoadSheetByProductHaulId(productHaul.Id);
            Assert.NotNull(shippingLoadSheets);
            Assert.AreNotEqual(shippingLoadSheets.Count, 0);
            foreach (var shippingLoadSheet in shippingLoadSheets)
            {
                ProductHaulLoad productHaulLoad =
                    eServiceOnlineGateway.Instance.GetProductHaulLoadById(shippingLoadSheet.ProductHaulLoad.Id);
                Assert.NotNull(productHaulLoad);

                List<BlendUnloadSheet> blendUnloadSheets =
                    eServiceOnlineGateway.Instance.GetBlendUnloadSheetByshippingLoadSheetId(shippingLoadSheet.Id);
                Assert.NotNull(blendUnloadSheets);
            }

            //A crew assignment is scheduled (RigJobThirdPartyBulkerCrewSection)
            RigJobThirdPartyBulkerCrewSection thirdPartyBulkerSection =
                eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionsByProductHual(productHaul.Id);
            Assert.NotNull(thirdPartyBulkerSection);
            Assert.AreEqual(thirdPartyBulkerSection.RigJobCrewSectionStatus, RigJobCrewSectionStatus.Scheduled);

            //Bulker crew schedule is created for the round trip. Start Time = Expected On Location Time - Estimated Travel Time, End Time = Expected On Location Time + Estimated Travel Time.
            double nEstimatedTravelTime = productHaul.EstimatedTravelTime;

            ThirdPartyBulkerCrewSchedule currentsanjelCrewSchedule =
                eServiceOnlineGateway.Instance.GetThirdPartyCrewScheduleByRigJobThirdPartyBulkerCrewSection(thirdPartyBulkerSection);

            Assert.NotNull(currentsanjelCrewSchedule);
            Assert.AreEqual(currentsanjelCrewSchedule.StartTime,
                productHaul.ExpectedOnLocationTime.AddHours(-nEstimatedTravelTime));
            Assert.AreEqual(currentsanjelCrewSchedule.EndTime,
                productHaul.ExpectedOnLocationTime.AddHours(productHaul.EstimatedTravelTime));

            //AThe bulker crew status is in LoadRequested status
            BulkerCrewLog currentBulkerCrewLog = eServiceOnlineGateway.Instance.GetBulkerCrewLog(crewId);
            Assert.Null(currentBulkerCrewLog);

            //5. Clean up test data
            DeleteProductHaul(productHaul.Id, true);

            //6. Double check all data are cleaned
            var cleanedProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaul.Id);
            Assert.IsNull(cleanedProductHaul);

            //After delete a ProductHaul, check whether the associated table object is created successfully
            List<ShippingLoadSheet> cleanedShippingLoadSheets =
                eServiceOnlineGateway.Instance.GetShippingLoadSheetByProductHaulId(productHaul.Id);
            Assert.AreEqual(cleanedShippingLoadSheets.Count, 0);
            foreach (var shippingLoadSheet in shippingLoadSheets)
            {
                ProductHaulLoad productHaulLoad =
                    eServiceOnlineGateway.Instance.GetProductHaulLoadById(shippingLoadSheet.ProductHaulLoad.Id);
                Assert.Null(productHaulLoad);

                List<BlendUnloadSheet> blendUnloadSheets =
                    eServiceOnlineGateway.Instance.GetBlendUnloadSheetByshippingLoadSheetId(shippingLoadSheet.Id);
                Assert.AreEqual(blendUnloadSheets.Count, 0);
            }

            List<PodLoad> cleanedpodLoads = eServiceOnlineGateway.Instance.GetPodLoadsByProductHaul(productHaul.Id);
            Assert.AreEqual(cleanedpodLoads.Count, 0);
        }
    }

}