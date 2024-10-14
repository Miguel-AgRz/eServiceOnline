using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using eServiceOnline.BusinessProcess;
using eServiceOnline.Data;
using eServiceOnline.Gateway;
using MetaShare.Common.Core.CommonService;
using MetaShare.Common.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Remotion.Linq.Utilities;
using Sesi.SanjelData.Entities.BusinessEntities.BulkPlant;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Crew;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ResourceSchedule;
using Sesi.SanjelData.Entities.BusinessEntities.WellSite;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Operation;
using Sesi.SanjelData.Services.BusinessEntities.HumanResources.WorkforceReadiness;
using Sesi.SanjelData.Services.Interfaces.BusinessEntities.BulkPlant;
using Sesi.SanjelData.Services.Interfaces.BusinessEntities.Operation.Crew;
using Sesi.SanjelData.Services.Interfaces.BusinessEntities.Operation.Dispatch;
using Sesi.SanjelData.Services.Interfaces.BusinessEntities.Operation.Schedule.ProductSchedule;
using Sesi.SanjelData.Services.Interfaces.BusinessEntities.Operation.Schedule.ResourceSchedule;
using Sesi.SanjelData.Services.Interfaces.Common.BusinessEntities.Equipment;
using Syncfusion.JavaScript.Models;

namespace eServiceOnline.Controllers
{
    public class DataCleanUpController : Controller
    {
        public IActionResult Index()
        {
            return new JsonResult("Success");
        }

        public IActionResult BackfillBPO()
        {
            Dictionary<int, string> list = new Dictionary<int, string>()
            {
                { 9996, "Mking" },
                { 9957, "MKing" },
                { 9836, "Relliott" },
                { 9828, "Relliott" },
                { 9804, "Relliott" },
                { 9797, "Relliott" },
                { 9793, "Relliott" },
                { 9784, "Relliott" },
                { 9754, "Relliott" },
                { 9744, "Relliott" },
                { 9738, "Relliott" },
                { 9736, "Relliott" },
                { 9711, "Relliott" },
                { 9709, "Relliott" },
                { 9703, "Relliott" },
                { 9697, "Relliott" },
                { 9677, "Relliott" },
                { 9670, "Relliott" },
                { 9662, "Relliott" },
                { 9658, "Relliott" },
                { 9629, "Relliott" },
                { 9625, "Relliott" },
                { 9618, "Relliott" },
                { 9616, "Relliott" },
                { 9574, "Relliott" },
                { 9573, "Relliott" },
                { 9571, "MKing" },
                { 9566, "Relliott" },
                { 9562, "Relliott" },
                { 9540, "Tfrasch" },
                { 9539, "Tfrasch" },
                { 9538, "Relliott" },
                { 9535, "Relliott" },
                { 9531, "Relliott" },
                { 9530, "MKing" },
                { 9525, "Relliott" },
                { 9497, "Relliott" },
                { 9492, "Relliott" },
                { 9489, "Relliott" },
                { 9487, "Relliott" },
                { 9457, "Relliott" },
                { 9427, "Relliott" },
                { 9421, "Relliott" },
                { 9418, "Relliott" },
                { 9415, "Relliott" },
                { 9382, "Relliott" },
                { 9379, "Relliott" },
                { 9376, "Relliott" },
                { 9372, "Relliott" },
                { 9349, "Relliott" },
                { 9345, "Relliott" },
                { 9343, "MKing" },
                { 9341, "Relliott" },
                { 9338, "MKing" },
                { 9333, "Relliott" },
                { 9203, "Hgoodyear" },
                { 9202, "MKing" },
                { 9109, "MKing" },
                { 9101, "Tnoad" },
                { 8909, "MKing" },
                { 8536, "Sfarthing" },
                { 8116, "MKing" },
                { 8109, "MKing" }
            };
            IBlendLogService logService =
                MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IBlendLogService>();
            if (logService == null)
                throw new Exception("clientConsultantService must be registered in service factory");

            foreach (var i in list)
            {
                var blendLog = logService.SelectById(new BlendLog() { Id = i.Key });
                if (blendLog == null) continue;

                blendLog.BulkPlantOperator = i.Value;
                logService.Update(blendLog);
            }

            return new JsonResult("Success");

        }


        //Sample: http://localhost:44703/DataCleanUp/CleanUpBinInformationMissingRig
        public IActionResult CleanUpBinInformationMissingRig()
        {

            IBinService binService =
                MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IBinService>();
            if (binService == null) throw new Exception("binService must be registered in service factory");
            var allBins = binService.SelectAll();
            string errorBinNumber = string.Empty;
            foreach (var bin in allBins)
            {
                for (int podIndex = 0; podIndex < bin.PodCount; podIndex++)
                {

                    var binInformations =
                        eServiceOnlineGateway.Instance.GetBinInformationsByBinIdAndPodIndex(bin.Id, podIndex + 1);
                    if (binInformations != null && binInformations.Count > 0)
                    {
                        if (binInformations.Count / bin.PodCount == 1)
                        {
                            if (binInformations[0].BinStatus == BinStatus.Assigned &&
                                (binInformations[0].Rig == null || binInformations[0].Rig.Id == 0))
                            {
                                errorBinNumber += bin.Id + "|";
                                break;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else if (binInformations.Count == 1)
                        {
                            continue;
                        }
                        else if (binInformations.Count == 2)
                        {
                            var record1 = binInformations[0];
                            var record2 = binInformations[1];
                            errorBinNumber += "Bin Id: " + bin.Id + " |Record1 Id: " + record1.Id + " - " +
                                              record1.BlendChemical.Name + " - " + record1.BinStatus.ToString()
                                              + "|Record2 Id: " + record2.Id + " - " + record2.BlendChemical.Name +
                                              " - " + record2.BinStatus.ToString() + "<br>";
                        }
                        else
                        {
                            errorBinNumber += bin.Id + "|";
                            break;
                        }
                    }
                }
            }



            return new JsonResult(errorBinNumber);
        }



        //Left Over Blend Request clean up

        //First step: set all shipping load sheet on location if job is completed
        //Sample: http://localhost:44703/DataCleanUp/SetScheduledShippingLoadSheetOnLocationForCompletedJob
        public IActionResult SetScheduledShippingLoadSheetOnLocationForCompletedJob()
        {
	        var scheduledShippingLoadSheet =
		        eServiceOnlineGateway.Instance.GetShippingLoadSheetsByStatus(ShippingStatus.Scheduled);
	        int count = 0;
	        foreach (var shippingLoadSheet in scheduledShippingLoadSheet)
	        {
		        if (shippingLoadSheet.CallSheetNumber == 0) continue;

		        var rigJob =
			        eServiceOnlineGateway.Instance.GetRigJobByCallSheetNumber(shippingLoadSheet.CallSheetNumber);
		        if (rigJob != null && rigJob.JobLifeStatus == JobLifeStatus.Completed)
		        {
			        shippingLoadSheet.ShippingStatus = ShippingStatus.OnLocation;
			        shippingLoadSheet.OnLocationTime = rigJob.JobDateTime;
			        shippingLoadSheet.ModifiedUserName = "DataCleanup";
			        eServiceOnlineGateway.Instance.UpdateShippingLoadSheet(shippingLoadSheet);
			        count++;
		        }
	        }

	        return new JsonResult("Set On Location count:" + count + " of " + scheduledShippingLoadSheet.Count);
        }

        //Second step: set all blend request on location if no shipping load sheet
        //Sample: http://localhost:44703/DataCleanUp/SetCompletedBlendRequestOnLocationForCompletedJob
        public IActionResult SetCompletedBlendRequestOnLocationForCompletedJob()
        {
            var scheduledProductHaulLoads = eServiceOnlineGateway.Instance.GetBlendCompletedProductHaulLoads();
            int count = 0;
            foreach (var productHaulLoad in scheduledProductHaulLoads)
            {
                var rigJob =
                    eServiceOnlineGateway.Instance.GetRigJobByCallSheetNumber(productHaulLoad.CallSheetNumber);
                if (rigJob != null && (rigJob.JobLifeStatus == JobLifeStatus.Completed || rigJob.JobLifeStatus == JobLifeStatus.Canceled))
                {
                    var shippingLoadSheet =
                        eServiceOnlineGateway.Instance.GetShippingLoadSheetByProductHaulLoadId(productHaulLoad.Id);
                    if (shippingLoadSheet == null || shippingLoadSheet.Count == 0)
                    {
                        productHaulLoad.ProductHaulLoadLifeStatus = ProductHaulLoadStatus.OnLocation;
                        productHaulLoad.OnLocationTime = rigJob.JobDateTime;
                        productHaulLoad.ModifiedUserName = "DataCleanUp";
                        eServiceOnlineGateway.Instance.UpdateProductHaulLoad(productHaulLoad);
                        count++;
                    }
                }

            }

            return new JsonResult("Set On Location count:" + count + " of " + scheduledProductHaulLoads.Count);
        }

        //Second step: set all product haul load by shipping load sheet on location status
        //Sample: http://localhost:44703/DataCleanUp/SetScheduledProductHaulLoadOnLocationByShippingLoadSheetStatus
        public IActionResult SetScheduledProductHaulLoadOnLocationByShippingLoadSheetStatus()
        {
            var notOnLocationProductHaulLoads =
	            eServiceOnlineGateway.Instance.GetProductHaulLoadByQuery(p =>
		            p.ProductHaulLoadLifeStatus != ProductHaulLoadStatus.OnLocation);

            int count = 0;

            foreach (var notOnLocationProductHaulLoad in notOnLocationProductHaulLoads)
            {
	            var shippingLoadSheets =
		            eServiceOnlineGateway.Instance.GetShippingLoadSheetByProductHaulLoadId(notOnLocationProductHaulLoad.Id);

	            bool allOnlocation = true;
                DateTime onLocationTime = DateTime.MinValue;
                if(shippingLoadSheets==null || shippingLoadSheets.Count == 0) continue;

                foreach (var shippingLoadSheet in shippingLoadSheets)
                {
	                allOnlocation &= shippingLoadSheet.ShippingStatus == ShippingStatus.OnLocation;
	                onLocationTime = shippingLoadSheet.OnLocationTime > onLocationTime
		                ? shippingLoadSheet.OnLocationTime
		                : onLocationTime;
                }

                if (allOnlocation)
                {
	                ProductHaulProcess.SetProductHaulLoadOnLocation(onLocationTime, "DataCleanup",
		                notOnLocationProductHaulLoad);
                }

                count++;
            }

            return new JsonResult("Set On Location count:" + count + " of " + notOnLocationProductHaulLoads.Count);
        }



        //Fourth step: from scheduled shipping load sheet find on location product hauls and set all related children on location. Ideally this is nothing to change.
        //Sample: http://localhost:44703/DataCleanUp/SetOnLocationProductHaulsChildrenOnLocation
        public IActionResult SetOnLocationProductHaulsChildrenOnLocation()
        {
            var shippingLoadSheets =
                eServiceOnlineGateway.Instance.GetShippingLoadSheetsByStatus(ShippingStatus.Scheduled);
            int count = 0;
            foreach (var shippingLoadSheet in shippingLoadSheets)
            {
                if (shippingLoadSheet.ProductHaul != null && shippingLoadSheet.ProductHaul.Id != 0)
                {
                    var productHaul =
                        eServiceOnlineGateway.Instance.GetProductHaulById(shippingLoadSheet.ProductHaul.Id);

                    if (productHaul.ProductHaulLifeStatus == ProductHaulStatus.OnLocation)
                    {
                        if (shippingLoadSheet.ProductHaulLoad.Id != 0)
                        {
                            var productHaulLoad =
                                eServiceOnlineGateway.Instance.GetProductHaulLoadById(shippingLoadSheet.ProductHaulLoad
                                    .Id);
                            if (productHaulLoad.ProductHaulLoadLifeStatus != ProductHaulLoadStatus.OnLocation)
                            {
                                productHaulLoad.ProductHaulLoadLifeStatus = ProductHaulLoadStatus.OnLocation;
                                productHaulLoad.OnLocationTime = productHaul.ExpectedOnLocationTime;
                                productHaulLoad.ModifiedUserName = "DataCleanup";
                                eServiceOnlineGateway.Instance.UpdateProductHaulLoad(productHaulLoad);
                            }
                        }

                        shippingLoadSheet.ShippingStatus = ShippingStatus.OnLocation;
                        shippingLoadSheet.OnLocationTime = productHaul.ExpectedOnLocationTime;
                        shippingLoadSheet.ModifiedUserName = "DataCleanup";
                        eServiceOnlineGateway.Instance.UpdateShippingLoadSheet(shippingLoadSheet);
                        count++;
                    }
                }
            }

            return new JsonResult("Set On Location count:" + count + " of " + shippingLoadSheets.Count);
        }

        //Fifth step: If a product haul is scheduled without call sheet, set on location after 7 days.
        //Sample: http://localhost:44703/DataCleanUp/SetProductHaulWithoutCallSheetOnLocation
        public IActionResult SetProductHaulWithoutCallSheetOnLocation()
        {
            DateTime weekago = DateTime.Now.AddDays(-7);
            var productHaulLoads = eServiceOnlineGateway.Instance.GetProductHaulLoadByQuery(p =>
                p.ProductHaulLoadLifeStatus == ProductHaulLoadStatus.Scheduled && p.CallSheetNumber == 0 &&
                p.ExpectedOnLocationTime < weekago);
            int count = 0;
            foreach (var productHaulLoad in productHaulLoads)
            {
                var shippingLoadSheets =
                    eServiceOnlineGateway.Instance.GetShippingLoadSheetByProductHaulLoadId(productHaulLoad.Id);

                foreach (var shippingLoadSheet in shippingLoadSheets)
                {
                    if (shippingLoadSheet.ProductHaul != null && shippingLoadSheet.ProductHaul.Id != 0)
                    {
                        var productHaul =
                            eServiceOnlineGateway.Instance.GetProductHaulById(shippingLoadSheet.ProductHaul.Id);

                        productHaul.ProductHaulLifeStatus = ProductHaulStatus.OnLocation;
                        productHaul.ModifiedUserName = "DataCleanup";
                        eServiceOnlineGateway.Instance.UpdateProductHaul(productHaul);


                    }

                    shippingLoadSheet.ShippingStatus = ShippingStatus.OnLocation;
                    shippingLoadSheet.OnLocationTime = productHaulLoad.ExpectedOnLocationTime;
                    shippingLoadSheet.ModifiedUserName = "DataCleanup";
                    eServiceOnlineGateway.Instance.UpdateShippingLoadSheet(shippingLoadSheet);
                }

                ProductHaulProcess.SetProductHaulLoadOnLocation(productHaulLoad.ExpectedOnLocationTime,
                    "DataCleanup", productHaulLoad);
                count++;

            }

            return new JsonResult("Set On Location count:" + count + " of " + productHaulLoads.Count);
        }

        //Sample: http://localhost:44703/DataCleanUp/SetBlendCompletedProductHaulLoadOnLocationForCompletedJob
        // Sixth Step: If the blend is completed or blending, but the job was canceled after, we consider it is on location due the current process doesn't cover it well.
        public IActionResult SetBlendCompletedProductHaulLoadOnLocationForCompletedJob()
        {
            var blendCompletedProductHaulLoads = eServiceOnlineGateway.Instance.GetBlendCompletedProductHaulLoads();
            var blendingProductHaulLoads = eServiceOnlineGateway.Instance.GetBlendingProductHaulLoads();
            blendCompletedProductHaulLoads.AddRange(blendingProductHaulLoads);
            var loadedProductHaulLoads = eServiceOnlineGateway.Instance.GetLoadedProductHaulLoads();
            blendCompletedProductHaulLoads.AddRange(loadedProductHaulLoads);
            int count = 0;
            foreach (var productHaulLoad in blendCompletedProductHaulLoads)
            {
                if (productHaulLoad.CallSheetNumber == 0)
                {
                    productHaulLoad.ProductHaulLoadLifeStatus = ProductHaulLoadStatus.OnLocation;
                    productHaulLoad.OnLocationTime = productHaulLoad.ExpectedOnLocationTime;
                    productHaulLoad.ModifiedUserName = "DataCleanup";
                    eServiceOnlineGateway.Instance.UpdateProductHaulLoad(productHaulLoad);
                    count++;
                }
                else
                {
                    var rigJob =
                        eServiceOnlineGateway.Instance.GetRigJobByCallSheetNumber(productHaulLoad.CallSheetNumber);
                    if (rigJob != null && (rigJob.JobLifeStatus == JobLifeStatus.Completed ||
                                           rigJob.JobLifeStatus == JobLifeStatus.Canceled))
                    {
                        productHaulLoad.ProductHaulLoadLifeStatus = ProductHaulLoadStatus.OnLocation;
                        productHaulLoad.OnLocationTime = rigJob.JobDateTime;
                        productHaulLoad.ModifiedUserName = "DataCleanup";
                        eServiceOnlineGateway.Instance.UpdateProductHaulLoad(productHaulLoad);
                        var shippingLoadSheets =
                            eServiceOnlineGateway.Instance.GetShippingLoadSheetByProductHaulLoadId(productHaulLoad.Id);

                        if (shippingLoadSheets != null)
                        {
                            foreach (var shippingLoadSheet in shippingLoadSheets)
                            {
                                shippingLoadSheet.ShippingStatus = ShippingStatus.OnLocation;
                                shippingLoadSheet.ModifiedUserName = "DataCleanup";
                                eServiceOnlineGateway.Instance.UpdateShippingLoadSheet(shippingLoadSheet, false);
                            }
                        }

                        count++;
                    }
                }

            }

            return new JsonResult("Set On Location count:" + count + " of " + blendCompletedProductHaulLoads.Count);
        }

        //Sample: http://localhost:44703/DataCleanUp/SetScheduledProductHaulLoadOnLocationForCompletedJob
        // Seventh Step: If a product haul is in Scheduled status, the job is complete, set is as on location
        public IActionResult SetScheduledProductHaulLoadOnLocationForCompletedJob()
        {
            var scheduledProductHaulLoads = eServiceOnlineGateway.Instance.GetScheduledProductHaulLoads();
            int count = 0;
            foreach (var productHaulLoad in scheduledProductHaulLoads)
            {
                if(productHaulLoad.CallSheetNumber == 0) continue;

                var rigJob =
                    eServiceOnlineGateway.Instance.GetRigJobByCallSheetNumber(productHaulLoad.CallSheetNumber);
                if (rigJob != null && (rigJob.JobLifeStatus == JobLifeStatus.Completed ||
                                       rigJob.JobLifeStatus == JobLifeStatus.Canceled))
                {
	                productHaulLoad.ProductHaulLoadLifeStatus = ProductHaulLoadStatus.OnLocation;
	                productHaulLoad.ModifiedUserName = "DataCleanup";
	                productHaulLoad.OnLocationTime = rigJob.JobDateTime;
	                eServiceOnlineGateway.Instance.UpdateProductHaulLoad(productHaulLoad);
	                count++;
                }
            }

            return new JsonResult("Set On Location count:" + count + " of " + scheduledProductHaulLoads.Count);
        }


        //Sample: http://localhost:44703/DataCleanUp/SetScheduledProductHaulOnLocationForOnLocationBlendRequest
        // eighth Step: If a product haul load is on location status, it is related product haul should be on location
        public IActionResult SetScheduledProductHaulOnLocationForOnLocationBlendRequest()
        {
            var productHauls = eServiceOnlineGateway.Instance.GetScheduledProductHaulsWithShippingLoadSheets();

            int count = 0;
            foreach (var productHaul in productHauls)
            {
	            bool changed = false;
	            var productHaulStatus = ProductHaulStatus.OnLocation;
	            foreach (var productHaulShippingLoadSheet in productHaul.ShippingLoadSheets)
	            {
		            var shippingLoadSheet =
			            eServiceOnlineGateway.Instance.GetShippingLoadSheetById(productHaulShippingLoadSheet.Id);
		            if (shippingLoadSheet.ProductHaulLoad.Id != 0)
		            {
			            var productHaulLoad =
				            eServiceOnlineGateway.Instance.GetProductHaulLoadById(productHaulShippingLoadSheet
					            .ProductHaulLoad.Id);
			            if (shippingLoadSheet.ShippingStatus != ShippingStatus.OnLocation && productHaulLoad.ProductHaulLoadLifeStatus == ProductHaulLoadStatus.OnLocation)
			            {
				            shippingLoadSheet.ShippingStatus = ShippingStatus.OnLocation;
				            shippingLoadSheet.ModifiedUserName = "DataCleanup";
				            eServiceOnlineGateway.Instance.UpdateShippingLoadSheet(shippingLoadSheet, false);
				            changed = true;
			            }
                    }

		            if (shippingLoadSheet.ShippingStatus != ShippingStatus.OnLocation)
		            {
			            productHaulStatus = ProductHaulStatus.Scheduled;
		            }
	            }

	            if (changed && productHaulStatus == ProductHaulStatus.OnLocation)
	            {
		            var productHaul1 =
			            eServiceOnlineGateway.Instance.GetProductHaulById(productHaul.Id);

		            productHaul1.ProductHaulLifeStatus = ProductHaulStatus.OnLocation;
		            productHaul1.ModifiedUserName = "DataCleanup";
		            eServiceOnlineGateway.Instance.UpdateProductHaul(productHaul1);
		            count++;
	            }
            }

            return new JsonResult("Set On Location count:" + count + " of " + productHauls.Count);
        }


        //Sample: http://localhost:44703/DataCleanUp/CleanupProductHaulForADeletedRigRjob
        // Nineth Step: Clean up shipping load sheet and product haul junk data for deleted jobs
        public IActionResult CleanupProductHaulForADeletedRigRjob()
        {
            var shippingLoadSheets = eServiceOnlineGateway.Instance.GetShippingLoadSheetsByStatus(ShippingStatus.Scheduled);

            int count = 0;
            foreach (var shippingLoadSheet in shippingLoadSheets)
            {
                bool changed = true;

                if (shippingLoadSheet.CallSheetNumber > 0)
                {
	                var rigjob =
		                eServiceOnlineGateway.Instance.GetRigJobByCallSheetNumber(shippingLoadSheet.CallSheetNumber);
	                if (rigjob == null || rigjob.JobLifeStatus != JobLifeStatus.Deleted)
	                {
		                changed = false;
	                }
                }

                if (changed)
                {
	                shippingLoadSheet.ModifiedUserName = "DataCleanup";
	                eServiceOnlineGateway.Instance.DeleteShippingLoadSheet(shippingLoadSheet);

	                if (shippingLoadSheet.ProductHaul.Id != 0)
	                {
		                var productHaul =
			                eServiceOnlineGateway.Instance.GetProductHaulById(shippingLoadSheet.ProductHaul.Id);

		                if (productHaul != null && productHaul.ShippingLoadSheets.Count == 0)
		                {
			                productHaul.ModifiedUserName = "DataCleanup";
			                eServiceOnlineGateway.Instance.DeleteProductHaul(productHaul);
		                }
	                }

	                count++;
                }
            }

            return new JsonResult("Set On Location count:" + count + " of " + shippingLoadSheets.Count);
        }


        //Sample: http://localhost:44703/DataCleanUp/DeleteScheduledProductHaulsWithoutShippingLoadSheet
        // tenth Step: If a scheduled product haul doesn't have any shipping load sheet, delete it.
        public IActionResult DeleteScheduledProductHaulsWithoutShippingLoadSheet()
        {
            var productHauls = eServiceOnlineGateway.Instance.GetScheduledProductHaulsWithShippingLoadSheets();
            int maxProductHaulId = 0;
            int count = 0;

            foreach (var productHaul in productHauls)
            {
                bool changed = false;

                if (productHaul.ShippingLoadSheets.Count == 0)
                {
	                productHaul.ModifiedUserName = "DataCleanup";
	                eServiceOnlineGateway.Instance.DeleteProductHaul(productHaul);

	                if (productHaul.Id > maxProductHaulId) maxProductHaulId = productHaul.Id;
	                count++;
                }
                else
                {
	                bool allOnLocation = true;

	                foreach (var shippingLoadSheet in productHaul.ShippingLoadSheets)
	                {
		                if (shippingLoadSheet.ShippingStatus != ShippingStatus.OnLocation) allOnLocation = false;
	                }

	                if (allOnLocation)
	                {
		                if (!productHaul.IsThirdParty)
		                {
			                RigJobSanjelCrewSection rigJobSanjelCrewSection = eServiceOnlineGateway.Instance.GetRigJobCrewSectionsByProductHaul(productHaul.Id);
			                if (rigJobSanjelCrewSection != null)
			                {
				                SanjelCrewSchedule crewSchedule =
					                eServiceOnlineGateway.Instance.GetCrewScheduleByJobCrewSection(rigJobSanjelCrewSection.Id);

								if (crewSchedule?.SanjelCrew != null)
								{
									crewSchedule.ModifiedUserName = "DataCleanUp";
					                ScheduleProcess.DeleteSanjelCrewSchedule(crewSchedule);
				                }

								rigJobSanjelCrewSection.ModifiedUserName = "DataCleanUp";
				                eServiceOnlineGateway.Instance.DeleteRigJobCrewSection(rigJobSanjelCrewSection);
                            }

                        }
                        else
		                {
			                RigJobThirdPartyBulkerCrewSection rigJobThirdPartyBulkerCrewSection = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionsByProductHual(productHaul.Id);
			                if (rigJobThirdPartyBulkerCrewSection != null)
			                {
				                ThirdPartyBulkerCrewSchedule crewSchedule =
					                eServiceOnlineGateway.Instance.GetThirdPartyBulkerCrewSchedule(rigJobThirdPartyBulkerCrewSection.Id);
				                if (crewSchedule != null && crewSchedule.ThirdPartyBulkerCrew != null)
				                {
					                crewSchedule.ModifiedUserName = "DataCleanUp";
					                ScheduleProcess.DeleteThirdPartyCrewSchedule(crewSchedule);
				                }

				                rigJobThirdPartyBulkerCrewSection.ModifiedUserName = "DataCleanUp";
				                eServiceOnlineGateway.Instance.DeleteRigJobThirdPartyBulkerCrewSection(rigJobThirdPartyBulkerCrewSection.Id);
			                }
                        }
		                productHaul.ModifiedUserName = "DataCleanUp";
		                eServiceOnlineGateway.Instance.DeleteProductHaul(productHaul);
                        count++;
	                }
                }

            }

            return new JsonResult("Set On Location count:" + count + " of " + productHauls.Count + "  " + maxProductHaulId);
        }




        //Sample: http://localhost:44703/DataCleanUp/CleanupOrphanSchedules
        public IActionResult CleanupOrphanSchedules()
        {
            ISanjelCrewScheduleService sanjelCrewScheduleService = MetaShare.Common.Core.CommonService.ServiceFactory
                .Instance.GetService<ISanjelCrewScheduleService>();
            if (sanjelCrewScheduleService == null)
                throw new Exception("ISanjelCrewScheduleService must be registered in service factory");
            List<SanjelCrewSchedule> assignedSchedules =
                sanjelCrewScheduleService.SelectBy(new SanjelCrewSchedule(), p => p.Type == CrewScheduleType.Assigned);
            int totalSchedules = assignedSchedules.Count;
            int deletedSchedules = 0;
            foreach (var sanjelCrewSchedule in assignedSchedules)
            {
                if (sanjelCrewSchedule.RigJobSanjelCrewSection != null && sanjelCrewSchedule.RigJobSanjelCrewSection.Id != 0)
                {
                    var rigJobSanjelCrewSection =
                        eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(sanjelCrewSchedule
                            .RigJobSanjelCrewSection.Id);
                    if (rigJobSanjelCrewSection == null)
                    {
                        sanjelCrewScheduleService.Delete(sanjelCrewSchedule);
                        deletedSchedules++;
                    }
                }
            }

            return new JsonResult("Delete " + deletedSchedules + " of " + totalSchedules);

        }



        //Redo data clean up
        //Action One: Safe clean from shipping load sheet for completed jobs.
        // 1.1 If rig job is completed, all shippingloadsheets related are considered on location.
        // 1.2 If shipping load sheet is loaded, but associated job is canceled or deleted, set shipping load sheet on location
        // 1.3 Delete scheduled shipping load sheet if associated job is canceled or deleted
        // 1.4 If a product hauls all related shippingloadsheets are on location, product haul is considered on location.
        // 1.5 If on location shipping load sheet amount is same as associated blend request amount, blend request is considered on location.
        // 1.6 If a product haul is scheduled but doesn't have shipping load sheet, it could be caused by exception. Find clue from assignment associated rig job.


        // 1.1 If rig job is completed, all shippingloadsheets related are considered on location.
        // 1.2 If shipping load sheet is loaded, but associated job is canceled or deleted, set shipping load sheet on location
        //Sample: http://localhost:44703/DataCleanUp/SetNotOnLocationShippingLoadSheetOnLocationForCompletedJob
        public IActionResult SetNotOnLocationShippingLoadSheetOnLocationForCompletedJob()
        {
	        var scheduledShippingLoadSheet =
		        eServiceOnlineGateway.Instance.GetShippingLoadSheetsByStatus(ShippingStatus.Scheduled);

	        int countScheduled = 0;
	        int countNoCallSheetScheduled = 0;
	        foreach (var shippingLoadSheet in scheduledShippingLoadSheet)
	        {
		        if (shippingLoadSheet.CallSheetNumber == 0)
		        {
			        if (shippingLoadSheet.ExpectedOnLocationTime < DateTime.Now.AddDays(-7))
			        {
				        shippingLoadSheet.ShippingStatus = ShippingStatus.OnLocation;
				        shippingLoadSheet.OnLocationTime = shippingLoadSheet.ExpectedOnLocationTime;
				        shippingLoadSheet.ModifiedUserName = "DataCleanup";
				        eServiceOnlineGateway.Instance.UpdateShippingLoadSheet(shippingLoadSheet);
				        countNoCallSheetScheduled++;

			        }
		        }
		        else
		        {

			        var rigJob =
				        eServiceOnlineGateway.Instance.GetRigJobByCallSheetNumber(shippingLoadSheet.CallSheetNumber);
			        if (rigJob != null && rigJob.JobLifeStatus == JobLifeStatus.Completed)
			        {
				        shippingLoadSheet.ShippingStatus = ShippingStatus.OnLocation;
				        shippingLoadSheet.OnLocationTime = rigJob.JobDateTime;
				        shippingLoadSheet.ModifiedUserName = "DataCleanup";
				        eServiceOnlineGateway.Instance.UpdateShippingLoadSheet(shippingLoadSheet);
				        countScheduled++;
			        }
		        }
	        }


	        var loadedShippingLoadSheet =
		        eServiceOnlineGateway.Instance.GetShippingLoadSheetsByStatus(ShippingStatus.Loaded);
	        int countLoaded = 0;
	        int countNoCallSheetLoaded = 0;
            foreach (var shippingLoadSheet in loadedShippingLoadSheet)
	        {
		        if (shippingLoadSheet.CallSheetNumber == 0)
		        {
			        if (shippingLoadSheet.ExpectedOnLocationTime < DateTime.Now.AddDays(-7))
			        {
				        shippingLoadSheet.ShippingStatus = ShippingStatus.OnLocation;
				        shippingLoadSheet.OnLocationTime = shippingLoadSheet.ExpectedOnLocationTime;
				        shippingLoadSheet.ModifiedUserName = "DataCleanup";
				        eServiceOnlineGateway.Instance.UpdateShippingLoadSheet(shippingLoadSheet);
				        countNoCallSheetLoaded++;

                    }
                }
		        else
		        {
			        var rigJob =
				        eServiceOnlineGateway.Instance.GetRigJobByCallSheetNumber(shippingLoadSheet.CallSheetNumber);
			        if (rigJob != null && (rigJob.JobLifeStatus == JobLifeStatus.Completed ||
			                               rigJob.JobLifeStatus == JobLifeStatus.Canceled ||
			                               rigJob.JobLifeStatus == JobLifeStatus.Deleted))
			        {
				        shippingLoadSheet.ShippingStatus = ShippingStatus.OnLocation;
				        shippingLoadSheet.OnLocationTime = rigJob.JobDateTime;
				        shippingLoadSheet.ModifiedUserName = "DataCleanup";
				        eServiceOnlineGateway.Instance.UpdateShippingLoadSheet(shippingLoadSheet);
				        countLoaded++;
			        }
		        }
	        }


            return new JsonResult("Set On Location (Loaded)count:" + countScheduled + " countLoaded:" + countLoaded + " countNoCallSheetLoaded:" + countNoCallSheetLoaded + " countNoCallSheetScheduled:" + countNoCallSheetScheduled + " of " + scheduledShippingLoadSheet.Count);
        }

        // 1.3 Delete scheduled shipping load sheet if associated job is canceled or deleted
        //Sample: http://localhost:44703/DataCleanUp/DeleteScheduledShippingLoadSheetForCanceledOrDeletedJob
        public IActionResult DeleteScheduledShippingLoadSheetForCanceledOrDeletedJob()
        {
	        var shippingLoadSheets = eServiceOnlineGateway.Instance.GetShippingLoadSheetsByStatus(ShippingStatus.Scheduled);

	        int count = 0;
	        foreach (var shippingLoadSheet in shippingLoadSheets)
	        {
		        bool shouldDelete = false;

		        if (shippingLoadSheet.CallSheetNumber > 0)
		        {
			        var rigjob =
				        eServiceOnlineGateway.Instance.GetRigJobByCallSheetNumber(shippingLoadSheet.CallSheetNumber);
			        if (rigjob == null || rigjob.JobLifeStatus == JobLifeStatus.Deleted || rigjob.JobLifeStatus == JobLifeStatus.Canceled)
			        {
				        shouldDelete = true;
			        }
		        }

		        if (shouldDelete)
		        {
			        shippingLoadSheet.ModifiedUserName = "DataCleanup";
			        eServiceOnlineGateway.Instance.DeleteShippingLoadSheet(shippingLoadSheet);

			        if (shippingLoadSheet.ProductHaul.Id != 0)
			        {
				        var productHaul =
					        eServiceOnlineGateway.Instance.GetProductHaulById(shippingLoadSheet.ProductHaul.Id);

				        if (productHaul != null && productHaul.ShippingLoadSheets.Count == 0)
				        {
					        productHaul.ModifiedUserName = "DataCleanup";
					        eServiceOnlineGateway.Instance.DeleteProductHaul(productHaul);
				        }
			        }

			        count++;
		        }
	        }

	        return new JsonResult("Set On Location count:" + count + " of " + shippingLoadSheets.Count);
        }


        // 1.4 If a product hauls all related shippingloadsheets are on location, product haul is considered on location.
        //Sample: http://localhost:44703/DataCleanUp/SetNotOnLocationProductHaulOnLocationByShippingLoadSheetStatus
        public IActionResult SetNotOnLocationProductHaulOnLocationByShippingLoadSheetStatus()
        {
	        var notOnLocationProductHauls =
		        eServiceOnlineGateway.Instance.GetProductHaulByQuery(p =>
			        p.ProductHaulLifeStatus != ProductHaulStatus.OnLocation && p.ProductHaulLifeStatus != ProductHaulStatus.Returned);

	        int count = 0;

	        foreach (var notOnLocationProductHaul in notOnLocationProductHauls)
	        {
		        var shippingLoadSheets =
			        eServiceOnlineGateway.Instance.GetShippingLoadSheetByProductHaulId(notOnLocationProductHaul.Id);

		        bool allOnlocation = true;
		        DateTime onLocationTime = DateTime.MinValue;
		        if (shippingLoadSheets == null || shippingLoadSheets.Count == 0) continue;

		        foreach (var shippingLoadSheet in shippingLoadSheets)
		        {
			        allOnlocation &= shippingLoadSheet.ShippingStatus == ShippingStatus.OnLocation;
			        onLocationTime = shippingLoadSheet.OnLocationTime > onLocationTime
				        ? shippingLoadSheet.OnLocationTime
				        : onLocationTime;
		        }

		        if (allOnlocation)
		        {
			        notOnLocationProductHaul.ProductHaulLifeStatus = ProductHaulStatus.OnLocation;
			        notOnLocationProductHaul.ModifiedUserName = "DataCleanUp";
			        eServiceOnlineGateway.Instance.UpdateProductHaul(notOnLocationProductHaul, false);

			        count++;
		        }
            }

	        return new JsonResult("Set On Location count:" + count + " of " + notOnLocationProductHauls.Count);
        }

        /*// 1.5 If on location shipping load sheet amount is same as associated blend request amount, blend request is considered on location.
        //Sample: http://localhost:44703/DataCleanUp/SetNotOnLocationProductHaulLoadOnLocationForOnLocationShippingLoadSheet
        public IActionResult SetNotOnLocationProductHaulLoadOnLocationForOnLocationShippingLoadSheet()
        {
            var notOnLocationProductHaulLoads = eServiceOnlineGateway.Instance.GetProductHaulLoadByQuery(p=>p.ProductHaulLoadLifeStatus!= ProductHaulLoadStatus.OnLocation);
            int count = 0;
            foreach (var productHaulLoad in notOnLocationProductHaulLoads)
            {
	            double onLocationAmount = 0.0;
	            bool allOnLocation = true;
					var shippingLoadSheets =
                        eServiceOnlineGateway.Instance.GetShippingLoadSheetByProductHaulLoadId(productHaulLoad.Id);

                    if (shippingLoadSheets != null)
                    {
                        foreach (var shippingLoadSheet in shippingLoadSheets)
                        {
	                        if (shippingLoadSheet.ShippingStatus == ShippingStatus.OnLocation)
		                        onLocationAmount += shippingLoadSheet.LoadAmount;
	                        else
	                        {
		                        allOnLocation = false;
		                        break;
	                        }
                        }
                    }

                    if (allOnLocation && (onLocationAmount > productHaulLoad.TotalBlendWeight ||
                                          Math.Abs(productHaulLoad.TotalBlendWeight - onLocationAmount) < 1))
                    {
	                    productHaulLoad.ProductHaulLoadLifeStatus = ProductHaulLoadStatus.OnLocation;
	                    productHaulLoad.ModifiedUserName = "DataCleanup";
	                    eServiceOnlineGateway.Instance.UpdateProductHaulLoad(productHaulLoad, false);
	                    count++;
	                }

            }

            return new JsonResult("Set On Location count:" + count + " of " + notOnLocationProductHaulLoads.Count);
        }*/

        // 1.6 If a product haul is scheduled but doesn't have shipping load sheet, it could be caused by exception. Find clue from assignment associated rig job.
        //Sample: http://localhost:44703/DataCleanUp/FixScheduledProductHaulsWithoutShippingLoadSheet

        public IActionResult FixScheduledProductHaulsWithoutShippingLoadSheet()
        {
            var productHauls = eServiceOnlineGateway.Instance.GetScheduledProductHaulsWithShippingLoadSheets();
            int maxProductHaulId = 0;
            int count = 0;
            int countJobCompleted = 0;
            int countJobDeleted = 0;
            int countJobNotExist = 0;


            foreach (var productHaul in productHauls)
            {
                bool changed = false;

                if (productHaul.ShippingLoadSheets.Count == 0)
                {
                    if (!productHaul.IsThirdParty)
                    {
                        RigJobSanjelCrewSection rigJobSanjelCrewSection = eServiceOnlineGateway.Instance.GetRigJobCrewSectionsByProductHaul(productHaul.Id);
                        if (rigJobSanjelCrewSection != null)
                        {
	                        if (rigJobSanjelCrewSection.RigJob.Id != 0)
	                        {
		                        var rigjob =
			                        eServiceOnlineGateway.Instance.GetRigJobById(rigJobSanjelCrewSection.RigJob.Id);
                                if(rigjob != null){
	                                if (rigjob.JobLifeStatus == JobLifeStatus.Completed)
	                                {
		                                SanjelCrewSchedule crewSchedule =
			                                eServiceOnlineGateway.Instance.GetCrewScheduleByJobCrewSection(rigJobSanjelCrewSection.Id);

		                                if (crewSchedule != null)
		                                {
			                                crewSchedule.ModifiedUserName = "DataCleanUp";
			                                ScheduleProcess.DeleteSanjelCrewSchedule(crewSchedule);
		                                }

		                                rigJobSanjelCrewSection.ModifiedUserName = "DataCleanUp";
		                                rigJobSanjelCrewSection.RigJobCrewSectionStatus =
			                                RigJobCrewSectionStatus.LogOffDuty;
		                                eServiceOnlineGateway.Instance.UpdateRigJobSanjelCrewSection(rigJobSanjelCrewSection);

		                                productHaul.ProductHaulLifeStatus = ProductHaulStatus.OnLocation;
		                                productHaul.ModifiedUserName = "DataCleanup";
		                                eServiceOnlineGateway.Instance.UpdateProductHaul(productHaul);
		                                countJobCompleted++;
	                                }
                                    else if (rigjob.JobLifeStatus== JobLifeStatus.Canceled || rigjob.JobLifeStatus == JobLifeStatus.Deleted)
	                                {
		                                SanjelCrewSchedule crewSchedule =
			                                eServiceOnlineGateway.Instance.GetCrewScheduleByJobCrewSection(rigJobSanjelCrewSection.Id);

		                                if (crewSchedule != null)
		                                {
			                                crewSchedule.ModifiedUserName = "DataCleanUp";
			                                ScheduleProcess.DeleteSanjelCrewSchedule(crewSchedule);
		                                }

		                                rigJobSanjelCrewSection.ModifiedUserName = "DataCleanUp";
		                                eServiceOnlineGateway.Instance.DeleteRigJobCrewSection(rigJobSanjelCrewSection);

		                                productHaul.ModifiedUserName = "DataCleanup";
		                                eServiceOnlineGateway.Instance.DeleteProductHaul(productHaul);
		                                countJobDeleted++;
	                                }
                                }
                                else
                                {
	                                SanjelCrewSchedule crewSchedule =
		                                eServiceOnlineGateway.Instance.GetCrewScheduleByJobCrewSection(rigJobSanjelCrewSection.Id);

	                                if (crewSchedule != null)
	                                {
		                                crewSchedule.ModifiedUserName = "DataCleanUp";
		                                ScheduleProcess.DeleteSanjelCrewSchedule(crewSchedule);
	                                }

	                                rigJobSanjelCrewSection.ModifiedUserName = "DataCleanUp";
	                                eServiceOnlineGateway.Instance.DeleteRigJobCrewSection(rigJobSanjelCrewSection);

	                                productHaul.ModifiedUserName = "DataCleanup";
	                                eServiceOnlineGateway.Instance.DeleteProductHaul(productHaul);
	                                countJobNotExist++;
                                }
                            }
                        }
                        else
                        {

                        }

                    }
                    else
                    {
                        RigJobThirdPartyBulkerCrewSection rigJobThirdPartyBulkerCrewSection = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionsByProductHual(productHaul.Id);
                        if (rigJobThirdPartyBulkerCrewSection != null)
                        {
                            ThirdPartyBulkerCrewSchedule crewSchedule =
                                eServiceOnlineGateway.Instance.GetThirdPartyBulkerCrewSchedule(rigJobThirdPartyBulkerCrewSection.Id);
                            if (crewSchedule != null && crewSchedule.ThirdPartyBulkerCrew != null)
                            {
                                crewSchedule.ModifiedUserName = "DataCleanUp";
                                ScheduleProcess.DeleteThirdPartyCrewSchedule(crewSchedule);
                            }

                            rigJobThirdPartyBulkerCrewSection.ModifiedUserName = "DataCleanUp";
                            eServiceOnlineGateway.Instance.DeleteRigJobThirdPartyBulkerCrewSection(rigJobThirdPartyBulkerCrewSection.Id);
                        }
                        productHaul.ModifiedUserName = "DataCleanup";
                        productHaul.ProductHaulLifeStatus = ProductHaulStatus.OnLocation;
                        eServiceOnlineGateway.Instance.UpdateProductHaul(productHaul);
                        count++;
                    }
                }

            }

            return new JsonResult("Set On Location count:" + count + ", countJobCompleted: " + countJobCompleted + ", countJobDeleted: " + countJobDeleted + ", countJobNotExist: " + countJobNotExist + ", total " + productHauls.Count);
        }

        // Action 2: Assignment Alignment
        // 2.1 If product haul is on location or returned, set its assigned SanjelCrew assignments offduty.


        //Sample: http://localhost:44703/DataCleanUp/SetAssignedSanjelCrewAssignmentOffDutyIfProductHaulIsOnLocation
        public IActionResult SetAssignedSanjelCrewAssignmentOffDutyIfProductHaulIsOnLocation()
        {
	        ISanjelCrewScheduleService sanjelCrewScheduleService = MetaShare.Common.Core.CommonService.ServiceFactory
		        .Instance.GetService<ISanjelCrewScheduleService>();
	        if (sanjelCrewScheduleService == null)
		        throw new Exception("ISanjelCrewScheduleService must be registered in service factory");

	        var assingedAssignments = eServiceOnlineGateway.Instance.GetRigJobSanjelCrewSectionsByQuery(p => p.RigJobCrewSectionStatus == RigJobCrewSectionStatus.Assigned && p.ProductHaul.Id !=0);
            int count = 0;
            
            foreach (var assignment in assingedAssignments)
            {
                double onLocationAmount = 0.0;
                bool allOnLocation = true;
                var productHaul =
                    eServiceOnlineGateway.Instance.GetProductHaulById(assignment.ProductHaul.Id);

                if (productHaul != null)
                {
	                if (productHaul.ProductHaulLifeStatus == ProductHaulStatus.OnLocation|| productHaul.ProductHaulLifeStatus == ProductHaulStatus.Returned)
	                {
		                var schedule = eServiceOnlineGateway.Instance.GetSanjelCrewSchedulesByRigJobSanjelCrewSection(assignment);
                        if(schedule != null)
			                sanjelCrewScheduleService.Delete(schedule);
		                var onLocationAssignment =
			                eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(assignment.Id);
		                onLocationAssignment.RigJobCrewSectionStatus = RigJobCrewSectionStatus.LogOffDuty;
                        productHaul.ModifiedUserName = "DataCleanup";
                        eServiceOnlineGateway.Instance.UpdateRigJobSanjelCrewSection(onLocationAssignment);
		                count++;
	                }
                }
            }
            
            return new JsonResult("Set On Location count:" + count + " of " + assingedAssignments.Count);
        }

        // 2.2 If product haul is on location or returned, set its assigned scheduled ThirdPartyCrew assignments offduty.

        //Sample: http://localhost:44703/DataCleanUp/SetAssignedThirdPartyAssignmentOffDutyIfProductHaulIsOnLocation
        public IActionResult SetAssignedThirdPartyAssignmentOffDutyIfProductHaulIsOnLocation()
        {

	        var assingedAssignments = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionByQuery(p => p.RigJobCrewSectionStatus == RigJobCrewSectionStatus.Assigned && p.ProductHaul.Id != 0);
	        int count = 0;

	        foreach (var assignment in assingedAssignments)
	        {
		        double onLocationAmount = 0.0;
		        bool allOnLocation = true;
		        var productHaul =
			        eServiceOnlineGateway.Instance.GetProductHaulById(assignment.ProductHaul.Id);

		        if (productHaul != null)
		        {
			        if (productHaul.ProductHaulLifeStatus == ProductHaulStatus.OnLocation || productHaul.ProductHaulLifeStatus == ProductHaulStatus.Returned)
			        {
				        var onLocationAssignment =
					        eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionById(assignment.Id);
				        onLocationAssignment.RigJobCrewSectionStatus = RigJobCrewSectionStatus.LogOffDuty;
				        productHaul.ModifiedUserName = "DataCleanup";
				        eServiceOnlineGateway.Instance.UpdateRigJobThirdPartyCrewSection(onLocationAssignment);
				        count++;
			        }
		        }
	        }

	        return new JsonResult("Set On Location count:" + count + " of " + assingedAssignments.Count);
        }

        // 2.3 If product haul is on location or returned, set its scheduled SanjelCrew assignments offduty.


        //Sample: http://localhost:44703/DataCleanUp/SetScheduledSanjelCrewAssignmentOffDutyIfProductHaulIsOnLocation
        public IActionResult SetScheduledSanjelCrewAssignmentOffDutyIfProductHaulIsOnLocation()
        {
            ISanjelCrewScheduleService sanjelCrewScheduleService = MetaShare.Common.Core.CommonService.ServiceFactory
                .Instance.GetService<ISanjelCrewScheduleService>();
            if (sanjelCrewScheduleService == null)
                throw new Exception("ISanjelCrewScheduleService must be registered in service factory");

            var assingedAssignments = eServiceOnlineGateway.Instance.GetRigJobSanjelCrewSectionsByQuery(p => p.RigJobCrewSectionStatus == RigJobCrewSectionStatus.Scheduled && p.ProductHaul.Id != 0);
            int count = 0;

            foreach (var assignment in assingedAssignments)
            {
                double onLocationAmount = 0.0;
                bool allOnLocation = true;
                var productHaul =
                    eServiceOnlineGateway.Instance.GetProductHaulById(assignment.ProductHaul.Id);

                if (productHaul != null)
                {
                    if (productHaul.ProductHaulLifeStatus == ProductHaulStatus.OnLocation || productHaul.ProductHaulLifeStatus == ProductHaulStatus.Returned)
                    {
                        var schedule = eServiceOnlineGateway.Instance.GetSanjelCrewSchedulesByRigJobSanjelCrewSection(assignment);
                        if (schedule != null)
                            sanjelCrewScheduleService.Delete(schedule);
                        var onLocationAssignment =
                            eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(assignment.Id);
                        onLocationAssignment.RigJobCrewSectionStatus = RigJobCrewSectionStatus.LogOffDuty;
                        productHaul.ModifiedUserName = "DataCleanup";
                        eServiceOnlineGateway.Instance.UpdateRigJobSanjelCrewSection(onLocationAssignment);
                        count++;
                    }
                }
            }

            return new JsonResult("Set On Location count:" + count + " of " + assingedAssignments.Count);
        }

        //2.4 Set scheduled Third Party Assignment On Location if product haul is on location

        //Sample: http://localhost:44703/DataCleanUp/SetScheduledThirdPartyAssignmentOnLocationIfProductHaulIsOnLocation
        public IActionResult SetScheduledThirdPartyAssignmentOnLocationIfProductHaulIsOnLocation()
        {
	        int[] bulkPlantIds = new[] { 79176, 77678, 73242, 66945, 77585, 73638, 73107, 72807, 73637, 73108, 71213, 71210, 73106, 73029, 72772, 71211, 71212, 66863 };
	        ISanjelCrewScheduleService sanjelCrewScheduleService = MetaShare.Common.Core.CommonService.ServiceFactory
		        .Instance.GetService<ISanjelCrewScheduleService>();
	        if (sanjelCrewScheduleService == null)
		        throw new Exception("ISanjelCrewScheduleService must be registered in service factory");

	        var assingedAssignments = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionByQuery(p => p.RigJobCrewSectionStatus == RigJobCrewSectionStatus.Scheduled && p.ProductHaul.Id != 0);
	        int count = 0;
	        bool isCanceled = false;
	        bool isCompleted = false;

            foreach (var assignment in assingedAssignments)
	        {
		        double onLocationAmount = 0.0;
		        bool allOnLocation = true;

		        if (assignment.RigJob.Id != 0 && !bulkPlantIds.Contains(assignment.RigJob.Id))
		        {
			        var rigJob = eServiceOnlineGateway.Instance.GetRigJobById(assignment.RigJob.Id);
			        if (rigJob.JobLifeStatus == JobLifeStatus.Canceled || rigJob.JobLifeStatus == JobLifeStatus.Deleted)
			        {
				        var productHaul =
					        eServiceOnlineGateway.Instance.GetProductHaulById(assignment.ProductHaul.Id);

				        if (productHaul != null)
				        {
					        if (productHaul.ProductHaulLifeStatus == ProductHaulStatus.OnLocation)
					        {
						        var onLocationAssignment =
							        eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionById(assignment.Id);
						        onLocationAssignment.RigJobCrewSectionStatus = RigJobCrewSectionStatus.LogOffDuty;
						        onLocationAssignment.ModifiedUserName = "DataCleanup";
						        eServiceOnlineGateway.Instance.UpdateRigJobThirdPartyCrewSection(onLocationAssignment);
						        count++;
					        }
					        else
					        {
						        productHaul.ModifiedUserName = "DataCleanup";
						        eServiceOnlineGateway.Instance.DeleteProductHaul(productHaul);
						        eServiceOnlineGateway.Instance.DeleteRigJobThirdPartyBulkerCrewSection(assignment.Id);
					        }
                        }
			        }
                    else if (rigJob.JobLifeStatus == JobLifeStatus.Completed)
			        {
				        var productHaul =
					        eServiceOnlineGateway.Instance.GetProductHaulById(assignment.ProductHaul.Id);

				        if (productHaul != null)
				        {
					        if (productHaul.ProductHaulLifeStatus != ProductHaulStatus.OnLocation)
					        {
						        productHaul.ProductHaulLifeStatus = ProductHaulStatus.OnLocation;
						        productHaul.ModifiedUserName = "DataCleanup";
						        eServiceOnlineGateway.Instance.UpdateProductHaul(productHaul);
					        }
					        var onLocationAssignment =
						        eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionById(assignment.Id);
					        onLocationAssignment.ModifiedUserName = "DataCleanup";
					        onLocationAssignment.RigJobCrewSectionStatus = RigJobCrewSectionStatus.LogOffDuty;
					        eServiceOnlineGateway.Instance.UpdateRigJobThirdPartyCrewSection(onLocationAssignment);
					        count++;
				        }
                    }
                }
		        else
		        {
			        var productHaul =
				        eServiceOnlineGateway.Instance.GetProductHaulById(assignment.ProductHaul.Id);

			        if (productHaul != null)
			        {
				        if (productHaul.ProductHaulLifeStatus == ProductHaulStatus.OnLocation || productHaul.ProductHaulLifeStatus == ProductHaulStatus.Returned)
				        {
					        var onLocationAssignment =
						        eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionById(assignment.Id);
					        onLocationAssignment.RigJobCrewSectionStatus = RigJobCrewSectionStatus.LogOffDuty;
					        onLocationAssignment.ModifiedUserName = "DataCleanup";
					        eServiceOnlineGateway.Instance.UpdateRigJobThirdPartyCrewSection(onLocationAssignment);
					        count++;
				        }
				        else
				        {
					        if (productHaul.ExpectedOnLocationTime > DateTime.Now.AddDays(-7))
					        {
						        productHaul.ProductHaulLifeStatus = ProductHaulStatus.OnLocation;
						        productHaul.ModifiedUserName = "DataCleanup";
						        eServiceOnlineGateway.Instance.UpdateProductHaul(productHaul);
						        var onLocationAssignment =
							        eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionById(assignment.Id);
						        onLocationAssignment.RigJobCrewSectionStatus = RigJobCrewSectionStatus.LogOffDuty;
						        onLocationAssignment.ModifiedUserName = "DataCleanup";
						        eServiceOnlineGateway.Instance.UpdateRigJobThirdPartyCrewSection(onLocationAssignment);
						        count++;
					        }
                        }
			        }

                }

            }

	        return new JsonResult("Set On Location count:" + count + " of " + assingedAssignments.Count);
        }

        //2.5 Set scheduled Third Party Assignment On Location if product haul is on location

        //Sample: http://localhost:44703/DataCleanUp/SetAssignedThirdPartyAssignmentOnLocationIfProductHaulIsOnLocation
        public IActionResult SetAssignedThirdPartyAssignmentOnLocationIfProductHaulIsOnLocation()
        {
	        int[] bulkPlantIds = new[] { 79176, 77678, 73242, 66945, 77585, 73638, 73107, 72807, 73637, 73108, 71213, 71210, 73106, 73029, 72772, 71211, 71212, 66863 };
            ISanjelCrewScheduleService sanjelCrewScheduleService = MetaShare.Common.Core.CommonService.ServiceFactory
                .Instance.GetService<ISanjelCrewScheduleService>();
            if (sanjelCrewScheduleService == null)
                throw new Exception("ISanjelCrewScheduleService must be registered in service factory");

            var assingedAssignments = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionByQuery(p => p.RigJobCrewSectionStatus == RigJobCrewSectionStatus.Assigned && p.ProductHaul.Id != 0);
            int count = 0;
            bool isCanceled = false;
            bool isCompleted = false;

            foreach (var assignment in assingedAssignments)
            {
                double onLocationAmount = 0.0;
                bool allOnLocation = true;

                if (assignment.RigJob.Id != 0 && !bulkPlantIds.Contains(assignment.RigJob.Id))
                {
                    var rigJob = eServiceOnlineGateway.Instance.GetRigJobById(assignment.RigJob.Id);
                    if (rigJob.JobLifeStatus == JobLifeStatus.Canceled || rigJob.JobLifeStatus == JobLifeStatus.Deleted)
                    {
                        var productHaul =
                            eServiceOnlineGateway.Instance.GetProductHaulById(assignment.ProductHaul.Id);

                        if (productHaul != null)
                        {
                            if (productHaul.ProductHaulLifeStatus == ProductHaulStatus.OnLocation)
                            {
                                var onLocationAssignment =
                                    eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionById(assignment.Id);
                                onLocationAssignment.RigJobCrewSectionStatus = RigJobCrewSectionStatus.LogOffDuty;
                                onLocationAssignment.ModifiedUserName = "DataCleanup";
                                eServiceOnlineGateway.Instance.UpdateRigJobThirdPartyCrewSection(onLocationAssignment);
                                count++;
                            }
                            else
                            {
                                eServiceOnlineGateway.Instance.DeleteProductHaul(productHaul);
                                eServiceOnlineGateway.Instance.DeleteRigJobThirdPartyBulkerCrewSection(assignment.Id);
                            }
                        }
                    }
                    else if (rigJob.JobLifeStatus == JobLifeStatus.Completed)
                    {
                        var productHaul =
                            eServiceOnlineGateway.Instance.GetProductHaulById(assignment.ProductHaul.Id);

                        if (productHaul != null)
                        {
                            if (productHaul.ProductHaulLifeStatus != ProductHaulStatus.OnLocation)
                            {
                                productHaul.ProductHaulLifeStatus = ProductHaulStatus.OnLocation;
                                productHaul.ModifiedUserName = "DataCleanup";
                                eServiceOnlineGateway.Instance.UpdateProductHaul(productHaul);
                            }
                            var onLocationAssignment =
                                eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionById(assignment.Id);
                            onLocationAssignment.RigJobCrewSectionStatus = RigJobCrewSectionStatus.LogOffDuty;
                            onLocationAssignment.ModifiedUserName = "DataCleanup";
                            eServiceOnlineGateway.Instance.UpdateRigJobThirdPartyCrewSection(onLocationAssignment);
                            count++;
                        }
                    }
                }
                else
                {
                    var productHaul =
                        eServiceOnlineGateway.Instance.GetProductHaulById(assignment.ProductHaul.Id);

                    if (productHaul != null)
                    {
                        if (productHaul.ProductHaulLifeStatus == ProductHaulStatus.OnLocation || productHaul.ProductHaulLifeStatus == ProductHaulStatus.Returned)
                        {
                            var onLocationAssignment =
                                eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionById(assignment.Id);
                            onLocationAssignment.RigJobCrewSectionStatus = RigJobCrewSectionStatus.LogOffDuty;
                            onLocationAssignment.ModifiedUserName = "DataCleanup";
                            eServiceOnlineGateway.Instance.UpdateRigJobThirdPartyCrewSection(onLocationAssignment);
                            count++;
                        }
                        else
                        {
                            if (productHaul.ExpectedOnLocationTime > DateTime.Now.AddDays(-7))
                            {
                                productHaul.ProductHaulLifeStatus = ProductHaulStatus.OnLocation;
                                productHaul.ModifiedUserName = "DataCleanup";
                                eServiceOnlineGateway.Instance.UpdateProductHaul(productHaul);
                                var onLocationAssignment =
                                    eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionById(assignment.Id);
                                onLocationAssignment.RigJobCrewSectionStatus = RigJobCrewSectionStatus.LogOffDuty;
                                onLocationAssignment.ModifiedUserName = "DataCleanup";
                                eServiceOnlineGateway.Instance.UpdateRigJobThirdPartyCrewSection(onLocationAssignment);
                                count++;
                            }
                        }
                    }

                }

            }

            return new JsonResult("Set On Location count:" + count + " of " + assingedAssignments.Count);
        }


        //Action 3: Clean up orphan data

        //3.1: If a scheduled product haul doesn't have any shipping load sheet, delete it.
        //Sample: http://localhost:44703/DataCleanUp/DeleteScheduledProductHaulsWithoutShippingLoadSheet1
        public IActionResult DeleteScheduledProductHaulsWithoutShippingLoadSheet1()
        {
            var productHauls = eServiceOnlineGateway.Instance.GetProductHaulByQuery(p=>p.ProductHaulLifeStatus == ProductHaulStatus.Scheduled);
            int maxProductHaulId = 0;
            int count = 0;

            foreach (var productHaul in productHauls)
            {
                bool changed = false;
                productHaul.ShippingLoadSheets =
	                eServiceOnlineGateway.Instance.GetShippingLoadSheetByProductHaulId(productHaul.Id);

                if (productHaul.ShippingLoadSheets.Count == 0)
                {
	                var assignments =
		                eServiceOnlineGateway.Instance.GetRigJobSanjelCrewSectionsByQuery(p =>
			                p.ProductHaul.Id == productHaul.Id);
	                foreach (var rigJobSanjelCrewSection in assignments)
	                {
		                rigJobSanjelCrewSection.ModifiedUserName = "DataCleanup";
		                eServiceOnlineGateway.Instance.DeleteRigJobCrewSection(rigJobSanjelCrewSection);
	                }

	                var thirdPartyAssignments =
		                eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionByQuery(p =>
			                p.ProductHaul.Id == productHaul.Id);
	                foreach (var thirdPartyBulkerCrewSection in thirdPartyAssignments)
	                {
		                thirdPartyBulkerCrewSection.ModifiedUserName = "DataCleanup";
		                eServiceOnlineGateway.Instance.DeleteRigJobThirdPartyBulkerCrewSection(thirdPartyBulkerCrewSection.Id);
	                }

	                productHaul.ModifiedUserName = "DataCleanup";
                    eServiceOnlineGateway.Instance.DeleteProductHaul(productHaul);

                    if (productHaul.Id > maxProductHaulId) maxProductHaulId = productHaul.Id;
                    count++;
                }


            }

            return new JsonResult("Set On Location count:" + count + " of " + productHauls.Count + "  " + maxProductHaulId);
        }

        //3.2: DeleteAssignedAssignmentWithCanceledOrDeletedRigJob
        //Sample: http://localhost:44703/DataCleanUp/DeleteAssignedAssignmentWithCanceledOrDeletedRigJob
        public IActionResult DeleteAssignedAssignmentWithCanceledOrDeletedRigJob()
        {
	        var rigJobSanjelCrewSections = eServiceOnlineGateway.Instance.GetRigJobSanjelCrewSectionsByQuery(p=>(p.RigJobCrewSectionStatus == RigJobCrewSectionStatus.Assigned || p.RigJobCrewSectionStatus == RigJobCrewSectionStatus.Scheduled) && p.ProductHaul.Id !=0);
	        int count = 0;

	        foreach (var crewSection in rigJobSanjelCrewSections)
	        {
		        var rigJob =
			        eServiceOnlineGateway.Instance.GetRigJobById(crewSection.RigJob.Id);
		        if (rigJob != null && (rigJob.JobLifeStatus == JobLifeStatus.Canceled ||
		                               rigJob.JobLifeStatus == JobLifeStatus.Deleted))
		        {
			        var productHaul = eServiceOnlineGateway.Instance.GetProductHaulById(crewSection.ProductHaul.Id);
			        if (productHaul != null)
			        {
                        productHaul.ModifiedUserName = "DataCleanUp";
                        eServiceOnlineGateway.Instance.DeleteProductHaul(productHaul, true);
			        }

			        crewSection.ModifiedUserName = "DataCleanUp";
			        eServiceOnlineGateway.Instance.DeleteRigJobCrewSection(crewSection);
			        count++;
		        }
	        }

	        return new JsonResult("Delete count:" + count + " of " + rigJobSanjelCrewSections.Count);
        }

        //3.3: DeleteScheduledThirdPartyAssignmentWithCanceledOrDeletedRigJob
        //Sample: http://localhost:44703/DataCleanUp/DeleteAssignedThirdPartyAssignmentWithCanceledOrDeletedRigJob
        public IActionResult DeleteAssignedThirdPartyAssignmentWithCanceledOrDeletedRigJob()
        {
	        var rigJobSanjelCrewSections = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionByQuery(p => p.RigJobCrewSectionStatus == RigJobCrewSectionStatus.Assigned || p.RigJobCrewSectionStatus == RigJobCrewSectionStatus.Scheduled);
	        int count = 0;

	        foreach (var crewSection in rigJobSanjelCrewSections)
	        {
		        if (crewSection.ProductHaul != null && crewSection.ProductHaul.Id != 0)
		        {
			        var rigJob =
				        eServiceOnlineGateway.Instance.GetRigJobById(crewSection.RigJob.Id);
			        if (rigJob != null && (rigJob.JobLifeStatus == JobLifeStatus.Canceled ||
			                               rigJob.JobLifeStatus == JobLifeStatus.Deleted))
			        {
				        var productHaul = eServiceOnlineGateway.Instance.GetProductHaulById(crewSection.ProductHaul.Id);
				        if (productHaul != null)
				        {
					        productHaul.ModifiedUserName = "DataCleanUp";
					        eServiceOnlineGateway.Instance.DeleteProductHaul(productHaul, true);
				        }

				        crewSection.ModifiedUserName = "DataCleanUp";
				        eServiceOnlineGateway.Instance.DeleteRigJobThirdPartyBulkerCrewSection(crewSection.Id);
				        count++;
			        }
		        }
	        }

	        return new JsonResult("Delete count:" + count + " of " + rigJobSanjelCrewSections.Count);
        }
        //3.4 Clean up completed assignment with wrong status
        //Sample: http://localhost:44703/DataCleanUp/CleanUpCompletedAssignmentWithWrongStatus
        public IActionResult CleanUpCompletedAssignmentWithWrongStatus()
        {
	        var rigJobSanjelCrewSections = eServiceOnlineGateway.Instance.GetRigJobSanjelCrewSectionsByQuery(p => (p.RigJobCrewSectionStatus == RigJobCrewSectionStatus.Assigned || p.RigJobCrewSectionStatus == RigJobCrewSectionStatus.Scheduled 
		        || p.RigJobCrewSectionStatus == RigJobCrewSectionStatus.EnRoute || p.RigJobCrewSectionStatus == RigJobCrewSectionStatus.Loaded || p.RigJobCrewSectionStatus == RigJobCrewSectionStatus.Loading || p.RigJobCrewSectionStatus == RigJobCrewSectionStatus.OnWayIn) && p.ProductHaul.Id != 0);
	        int count = 0;
	        int countBulkPlant = 0;
	        int[] bulkPlantIds = new[] { 79176, 77678, 73242, 66945, 77585, 73638, 73107, 72807, 73637, 73108, 71213, 71210, 73106, 73029, 72772, 71211, 71212, 66863 };

            foreach (var crewSection in rigJobSanjelCrewSections)
	        {
                if(crewSection.RigJob.Id == 0) continue;
                if (bulkPlantIds.Contains(crewSection.RigJob.Id))
                {
	                var productHaul = eServiceOnlineGateway.Instance.GetProductHaulById(crewSection.ProductHaul.Id);
	                if (productHaul != null && productHaul.ProductHaulLifeStatus == ProductHaulStatus.OnLocation)
	                {
		                productHaul.ModifiedUserName = "DataCleanUp";
		                productHaul.ProductHaulLifeStatus = ProductHaulStatus.Returned;
		                eServiceOnlineGateway.Instance.UpdateProductHaul(productHaul);
		                crewSection.ModifiedUserName = "DataCleanUp";
		                crewSection.RigJobCrewSectionStatus = RigJobCrewSectionStatus.LogOffDuty;
		                eServiceOnlineGateway.Instance.UpdateRigJobSanjelCrewSection(crewSection);
		                count++;
	                }
                    countBulkPlant++;
                }
                else
                {
	                var rigJob =
		                eServiceOnlineGateway.Instance.GetRigJobById(crewSection.RigJob.Id);
	                if (rigJob != null && rigJob.JobLifeStatus == JobLifeStatus.Completed)
	                {
		                var productHaul = eServiceOnlineGateway.Instance.GetProductHaulById(crewSection.ProductHaul.Id);
		                if (productHaul != null)
		                {
			                productHaul.ModifiedUserName = "DataCleanUp";
			                productHaul.ProductHaulLifeStatus = ProductHaulStatus.Returned;
			                eServiceOnlineGateway.Instance.UpdateProductHaul(productHaul);
		                }

		                crewSection.ModifiedUserName = "DataCleanUp";
		                crewSection.RigJobCrewSectionStatus = RigJobCrewSectionStatus.LogOffDuty;
		                eServiceOnlineGateway.Instance.UpdateRigJobSanjelCrewSection(crewSection);
		                count++;
	                }
                }
	        }

	        return new JsonResult("count:" + count + " countBulkPlant:" + countBulkPlant + " of " + rigJobSanjelCrewSections.Count);
        }

        //3.5 Align bulker crew status
        //Sample: http://localhost:44703/DataCleanUp/AlignBulkerCrewStatus
        public IActionResult AlignBulkerCrewStatus()
        {
	        var bulkerCrews = RigBoardProcess.GetSanjeBulkerCrew();
            var deletedCrews = bulkerCrews.FindAll(p => p.IsDeleted);

            int deletedCount = 0;

            foreach (var deletedCrew in deletedCrews)
            {
	            var crewbulkerlog = eServiceOnlineGateway.Instance.GetBulkerCrewLog(deletedCrew.Id);
                if(crewbulkerlog != null)
                {
	                if (crewbulkerlog.CrewStatus != BulkerCrewStatus.OffDuty)
	                {
		                crewbulkerlog.CrewStatus = BulkerCrewStatus.OffDuty;
		                crewbulkerlog.ModifiedUserName = "DataCleanUp";
//		                eServiceOnlineGateway.Instance.UpdateBulkerCrewLog(crewbulkerlog);
		                deletedCount++;
	                }
                }
            }


            var activeCrews = bulkerCrews.FindAll(p => !p.IsDeleted);
            int countNoAssignmentMisalignment = 0;
            int countOffDutyMisalignment = 0;
            int countLoadRequestedMisalignment = 0;
            int countEnRouteMisalignment = 0;
            int countCalledMisalignment = 0;
            int countLoadingMisalignment = 0;
            int countLoadedMisalignment = 0;
            int countOnLocationMisalignment = 0;
            int countOnWayInMisalignment = 0;
            int countReturnedMisalignment = 0;
            int countOtherMisalignment = 0;

            foreach (var activeCrew in activeCrews)
            {
	            var crewbulkerlog = eServiceOnlineGateway.Instance.GetBulkerCrewLog(activeCrew.Id);
                var crewAssignments = eServiceOnlineGateway.Instance.GetRigJobCrewSectionsByCrew(activeCrew.Id);

                if (crewAssignments.Count == 0)
                {
	                if (crewbulkerlog != null && crewbulkerlog.CrewStatus != BulkerCrewStatus.OffDuty)
	                {
		                crewbulkerlog.CrewStatus = BulkerCrewStatus.OffDuty;
		                crewbulkerlog.ModifiedUserName = "DataCleanUp";
                        //		                eServiceOnlineGateway.Instance.UpdateBulkerCrewLog(crewbulkerlog);
                        countNoAssignmentMisalignment++;
	                }
                }
                else
                {
	                var crewLastAssignment = crewAssignments.OrderByDescending(p => p.Id).First();
                    //check last assignment
                    if (crewbulkerlog.CrewStatus == BulkerCrewStatus.OffDuty &&
                        crewLastAssignment.RigJobCrewSectionStatus != RigJobCrewSectionStatus.LogOffDuty)
                    {
	                    countOffDutyMisalignment++;
                    }

                    if (crewbulkerlog.CrewStatus == BulkerCrewStatus.LoadRequested &&
                        crewLastAssignment.RigJobCrewSectionStatus != RigJobCrewSectionStatus.Scheduled)
                    {
	                    countLoadRequestedMisalignment++;
                    }

                    if (crewbulkerlog.CrewStatus == BulkerCrewStatus.Called &&
                        crewLastAssignment.RigJobCrewSectionStatus != RigJobCrewSectionStatus.Called)
                    {
	                    countCalledMisalignment++;
                    }
                    
                    if (crewbulkerlog.CrewStatus == BulkerCrewStatus.Loading &&
                        crewLastAssignment.RigJobCrewSectionStatus != RigJobCrewSectionStatus.Loading)
                    {
	                    countLoadingMisalignment++;
                    }

                    if (crewbulkerlog.CrewStatus == BulkerCrewStatus.Loaded &&
                        crewLastAssignment.RigJobCrewSectionStatus != RigJobCrewSectionStatus.Loaded)
                    {
	                    countLoadedMisalignment++;
                    }
                    if (crewbulkerlog.CrewStatus == BulkerCrewStatus.EnRoute &&
                        crewLastAssignment.RigJobCrewSectionStatus != RigJobCrewSectionStatus.EnRoute)
                    {
	                    countEnRouteMisalignment++;
                    }

                    if (crewbulkerlog.CrewStatus == BulkerCrewStatus.OnLocation &&
                        crewLastAssignment.RigJobCrewSectionStatus != RigJobCrewSectionStatus.OnLocation)
                    {
	                    countOnLocationMisalignment++;
                    }

                    if (crewbulkerlog.CrewStatus == BulkerCrewStatus.OnWayIn &&
                        crewLastAssignment.RigJobCrewSectionStatus != RigJobCrewSectionStatus.OnWayIn)
                    {
	                    countOnWayInMisalignment++;
                    }

                    if (crewbulkerlog.CrewStatus == BulkerCrewStatus.Returned &&
                        crewLastAssignment.RigJobCrewSectionStatus != RigJobCrewSectionStatus.Returned)
                    {
	                    countReturnedMisalignment++;
                    }

                    if ((crewbulkerlog.CrewStatus == BulkerCrewStatus.Down || crewbulkerlog.CrewStatus == BulkerCrewStatus.GoodToGo) &&
                        crewLastAssignment.RigJobCrewSectionStatus != RigJobCrewSectionStatus.Returned)
                    {
	                    countOtherMisalignment++;
                    }

                }

                if (crewbulkerlog != null)
	            {
		            if (crewbulkerlog.CrewStatus != BulkerCrewStatus.OffDuty)
		            {
			            crewbulkerlog.CrewStatus = BulkerCrewStatus.OffDuty;
			            crewbulkerlog.ModifiedUserName = "DataCleanUp";
			            //		                eServiceOnlineGateway.Instance.UpdateBulkerCrewLog(crewbulkerlog);
			            deletedCount++;
		            }
	            }
            }


            return new JsonResult("updated:" + deletedCount + ", of deleted crews:" + deletedCrews.Count 
                                  + ", countNoAssignmentMisalignment:" + countNoAssignmentMisalignment
                                  + ", countOffDutyMisalignment:" + countOffDutyMisalignment
                                  + ", countLoadRequestedMisalignment:" + countLoadRequestedMisalignment
                                  + ", countCalledMisalignment:" + countCalledMisalignment
                                  + ", countEnRouteMisalignment:" + countEnRouteMisalignment
                                  + ", countLoadingMisalignment:" + countLoadingMisalignment
                                  + ", countLoadedMisalignment:" + countLoadedMisalignment
                                  + ", countOnLocationMisalignment:" + countOnLocationMisalignment
                                  + ", countOnWayInMisalignment:" + countOnWayInMisalignment
                                  + ", countReturnedMisalignment:" + countReturnedMisalignment
                                  + ", countOtherMisalignment:" + countOtherMisalignment
                                  + " of " + activeCrews.Count);
        }


        //Action 4.1: SetCompletedBlendRequestOnLocationForCanceledOrDeletedRigJob
        //Sample: http://localhost:44703/DataCleanUp/SetCompletedBlendRequestOnLocationForCanceledOrDeletedRigJob
        public IActionResult SetCompletedBlendRequestOnLocationForCanceledOrDeletedRigJob()
        {
	        var scheduledProductHaulLoads = eServiceOnlineGateway.Instance.GetBlendCompletedProductHaulLoads();
	        int countProgramLoaded = 0;
	        int countJobLoaded = 0;
	        int countProgramTestStored = 0;
	        int countJobTestStored = 0;
	        foreach (var productHaulLoad in scheduledProductHaulLoads)
	        {
		        if (productHaulLoad.Bin == null || productHaulLoad.Bin.Id == 0)
		        {
			        if (productHaulLoad.CallSheetNumber == 0)
			        {
				        if (productHaulLoad.EstmatedLoadTime < DateTime.Now.AddDays(-7))
				        {
					        productHaulLoad.ProductHaulLoadLifeStatus = ProductHaulLoadStatus.Loaded;
					        productHaulLoad.ModifiedUserName = "DataCleanUp";
					        eServiceOnlineGateway.Instance.UpdateProductHaulLoad(productHaulLoad);
					        countProgramLoaded ++;

				        }
			        }
                    else
			        {
				        var rigJob =
					        eServiceOnlineGateway.Instance.GetRigJobByCallSheetNumber(productHaulLoad.CallSheetNumber);
				        if (rigJob != null && (rigJob.JobLifeStatus == JobLifeStatus.Deleted || rigJob.JobLifeStatus == JobLifeStatus.Canceled || rigJob.JobLifeStatus == JobLifeStatus.Completed || rigJob.JobLifeStatus == JobLifeStatus.None))
				        {
					        productHaulLoad.ProductHaulLoadLifeStatus = ProductHaulLoadStatus.Loaded;
					        productHaulLoad.ModifiedUserName = "DataCleanUp";
					        eServiceOnlineGateway.Instance.UpdateProductHaulLoad(productHaulLoad);
					        countJobLoaded ++;
				        }

                    }

                }
		        else
		        {
			        if (productHaulLoad.CallSheetNumber == 0)
			        {
				        if (productHaulLoad.EstmatedLoadTime < DateTime.Now.AddDays(-7))
				        {
					        productHaulLoad.ProductHaulLoadLifeStatus = ProductHaulLoadStatus.Stored;
					        productHaulLoad.ModifiedUserName = "DataCleanUp";
					        eServiceOnlineGateway.Instance.UpdateProductHaulLoad(productHaulLoad);
					        countProgramTestStored++;

				        }
			        }
			        else
			        {
				        var rigJob =
					        eServiceOnlineGateway.Instance.GetRigJobByCallSheetNumber(productHaulLoad.CallSheetNumber);
				        if (rigJob != null && (rigJob.JobLifeStatus == JobLifeStatus.Deleted || rigJob.JobLifeStatus == JobLifeStatus.Canceled || rigJob.JobLifeStatus == JobLifeStatus.Completed || rigJob.JobLifeStatus == JobLifeStatus.None))
				        {
					        productHaulLoad.ProductHaulLoadLifeStatus = ProductHaulLoadStatus.Stored;
					        productHaulLoad.ModifiedUserName = "DataCleanUp";
					        eServiceOnlineGateway.Instance.UpdateProductHaulLoad(productHaulLoad);
					        countJobTestStored++;
				        }

			        }
		        }


            }

	        return new JsonResult("countProgramLoaded:" + countProgramLoaded + ", countJobLoaded:" + countJobLoaded + ", countProgramTestStored:" + countProgramTestStored + ", countJobTestStored:" + countJobTestStored + " of " + scheduledProductHaulLoads.Count);
        }

        //Action 4.2: SetScheduledProductHaulLoadDeletedForCancelledJob
        //Sample: http://localhost:44703/DataCleanUp/SetScheduledProductHaulLoadDeletedForCancelledJob
        public IActionResult SetScheduledProductHaulLoadDeletedForCancelledJob()
        {
	        var allScheduledProductLoads = eServiceOnlineGateway.Instance.GetScheduledProductHaulLoads();
	        int count = 0;
	        foreach (var scheduledProductLoad in allScheduledProductLoads)
	        {
		        var productHaulId = scheduledProductLoad.ProductHaul.Id;
		        var rigJob =
			        eServiceOnlineGateway.Instance.GetRigJobByCallSheetNumber(scheduledProductLoad.CallSheetNumber);
		        if (rigJob != null && (rigJob.JobLifeStatus == JobLifeStatus.Canceled ||
		                               rigJob.JobLifeStatus == JobLifeStatus.Deleted))
		        {
			        scheduledProductLoad.ModifiedUserName = "DataCleanup";
			        eServiceOnlineGateway.Instance.DeleteProductHaulLoad(scheduledProductLoad, true);

			        count++;
		        }

	        }

	        return new JsonResult("Delete count:" + count + " of " + allScheduledProductLoads.Count);
        }

        //Utility
        //Sample: http://localhost:44703/DataCleanUp/LoadBlendToBin
        public IActionResult LoadBlendToBin(string storageName, int productHaulLoadId, double quantity, string bulkPlantName)
        {
	        var productHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productHaulLoadId);
            if(productHaulLoad==null)
	            return new JsonResult("ProductHaulLoad not found");
	        var binInformation = eServiceOnlineGateway.Instance.GetBinInformationByQuery(p => p.Name == storageName && p.Rig.Name==bulkPlantName).FirstOrDefault();
	        if (binInformation == null)
		        return new JsonResult("BinInformation not found");
	        if (binInformation.BlendChemical != null &&  binInformation.BlendChemical.Id !=0)
		        return new JsonResult("Bin is not empty");

            binInformation.BlendChemical = productHaulLoad.BlendChemical;
            if (Math.Abs(quantity) < 0.001)
	            binInformation.Quantity = productHaulLoad.TotalBlendWeight / 1000;
            else
		        binInformation.Quantity = quantity;
	        binInformation.LastProductHaulLoadId = productHaulLoad.Id;
	        binInformation.ModifiedUserName ="Data Import";
	        eServiceOnlineGateway.Instance.UpdateBinInformation(binInformation);
	        return new JsonResult("Success");

        }
    }
}