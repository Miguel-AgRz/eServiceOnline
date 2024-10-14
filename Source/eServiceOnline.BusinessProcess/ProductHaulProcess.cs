using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Net.Http.Headers;
using eServiceOnline.BusinessProcess.Interface;
using eServiceOnline.Gateway;
using MetaShare.Common.Core.Entities;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Crew;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule;
using Sesi.SanjelData.Entities.Common.BusinessEntities.HumanResources;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Operation;
using sesi.SanjelLibrary.NotficationLibrary;
using UnitSection = Sanjel.BusinessEntities.Sections.Common.UnitSection;
using ThirdPartyUnitSection = Sanjel.Common.EService.Sections.Common.ThirdPartyUnitSection;
namespace eServiceOnline.BusinessProcess
{
    public class ProductHaulProcess
    {
        private static readonly bool SendCrewNotification = ConfigurationManager.AppSettings["SendCrewNotification"] == "true";
        #region DeleteProductHaul

        public static void DeleteProductHaul(ProductHaul productHaul, bool deleteLoads)
        {
            //1.Delete relevant schedule
            ReleaseProductHaulCrew(productHaul);

            //2.Delete relevant record in eService

            Collection<UnitSection> unitSections = eServiceOnlineGateway.Instance.GetUnitSectionsByProductHaul(productHaul);
            if (unitSections != null && unitSections.Count > 0)
            {
                foreach (UnitSection unitSection in unitSections)
                {
                    eServiceOnlineGateway.Instance.DeleteUnitSection(unitSection);
                }
            }

            Collection<ThirdPartyUnitSection> thirdPartyUnitSections = eServiceOnlineGateway.Instance.GetThirdPartyUnitSectionsByProductHaul(productHaul);
            if (thirdPartyUnitSections != null && thirdPartyUnitSections.Count > 0)
            {
                foreach (ThirdPartyUnitSection thirdPartyUnitSection in thirdPartyUnitSections)
                {
                    eServiceOnlineGateway.Instance.DeleteThirdPartyUnitSection(thirdPartyUnitSection);
                }
            }

            //4.Delete product haul itself
            eServiceOnlineGateway.Instance.DeleteProductHaul(productHaul, true);
        }

        public static void DeleteProductHaulLoadScheduledTogether(ShippingLoadSheet shippingLoadSheet)
        {
            if (shippingLoadSheet.ProductHaulLoad != null && shippingLoadSheet.ProductHaulLoad.Id != 0)
            {
                var productHaulLoad =
                    eServiceOnlineGateway.Instance.GetProductHaulLoadByQuery(p =>
                        p.Id == shippingLoadSheet.ProductHaulLoad.Id).OrderByDescending(p=>p.TotalBlendWeight).FirstOrDefault();
                if (productHaulLoad != null && productHaulLoad.ProductHaulLoadLifeStatus == ProductHaulLoadStatus.Scheduled)
                {
                    //Only for bulk plant schedule product haul with blend request at same time
                    if(shippingLoadSheet.BulkPlant.Id == productHaulLoad.BulkPlant.Id && shippingLoadSheet.SourceStorage.Id == 0)
                    {
                        eServiceOnlineGateway.Instance.DeleteProductHaulLoad(productHaulLoad, true);
                    }
                }
            }
        }


        public static List<ProductHaulLoad> GetProductHaulLoadCollectionByPaginated(int callSheetNumber, int servicePointId, int pageNumber, int pageSize, out int totalRecordsCount)
        {
            Pager pager = new Pager(){PageIndex = pageNumber, PageSize = pageSize, PageTotal = 10, TotalCounts = 180 };

            List<ProductHaulLoad> productHaulLoads = eServiceOnlineGateway.Instance.GetProductHaulLoads(pager);

            if (callSheetNumber != 0)
            {
                productHaulLoads = productHaulLoads.FindAll(p => p.CallSheetNumber == callSheetNumber);
            }

            if (servicePointId != 0)
            {
                productHaulLoads = productHaulLoads.FindAll(p => p.ServicePoint.Id == servicePointId);
            }
            List<ProductHaulLoad> pagedProductHaulLoads = productHaulLoads.OrderByDescending(p => p.Id).ToList();
            totalRecordsCount = 180;
            return pagedProductHaulLoads;
        }

        #endregion

        #region Build Comments

        public static string BuildProductHaulLoadComments(ProductHaulLoad productHaulLoad)
        {
            string comments = String.Empty;
            var storageName = eServiceOnlineGateway.Instance
                .GetBinInformationByBinIdAndPodIndex(productHaulLoad.Bin.Id, productHaulLoad.PodIndex)?.Name;
            if (productHaulLoad.ProductHaul == null || productHaulLoad.ProductHaul.Id == 0)
            {
                if (!String.IsNullOrEmpty(storageName))
                    //Nov 3, 2023 Tongtao P45_Q4_105: Modify numerical precision to 3 decimal places
                    //Nov 22, 2023 Tongtao P45_Q4_175: Adjusting precision (integer formatting to one decimal place, keeping one to three decimal places unchanged, rounding to three decimal places, formatting to three decimal places)
                    comments += $"Blend " + (productHaulLoad.BaseBlendWeight / 1000).ToString("#.##") +
                                $"t {productHaulLoad.BlendChemical?.Name} to {storageName}";
                else
                    //Nov 3, 2023 Tongtao P45_Q4_105: Modify numerical precision to 3 decimal places
                    //Nov 22, 2023 Tongtao P45_Q4_175: Adjusting precision (integer formatting to one decimal place, keeping one to three decimal places unchanged, rounding to three decimal places, formatting to three decimal places)
                    comments += $"Blend " + (productHaulLoad.BaseBlendWeight / 1000).ToString("#.##") +
                                $"t {productHaulLoad.BlendChemical?.Name}";
                if (productHaulLoad.IsBlendTest==true)
                    comments +=" for testing";
            }
            else
            {
                //Nov 3, 2023 Tongtao P45_Q4_105: Modify numerical precision to 3 decimal places
                //Nov 22, 2023 Tongtao P45_Q4_175: Adjusting precision (integer formatting to one decimal place, keeping one to three decimal places unchanged, rounding to three decimal places, formatting to three decimal places)
                comments += $"Haul " + (productHaulLoad.BaseBlendWeight / 1000).ToString("#.##") +
                            $"t {productHaulLoad.BlendChemical?.Name}";
                if (!String.IsNullOrEmpty(storageName))
                {
                    comments += $" to Bin {storageName}";
                }

                if (productHaulLoad.IsGoWithCrew)
                {
                    comments += " Go With Crew";
                }
                else
                {
                    comments += " on " + productHaulLoad.ExpectedOnLocationTime.ToString("MMM dd,yyyy");
                }

                if (!String.IsNullOrEmpty(productHaulLoad.Comments))
                {
                    comments += ", " + productHaulLoad.Comments;
                }
                else
                {
                    comments += ".";
                }
            }

            return comments;
        }

        public static string BuildBlendUnloadSheetComments(BlendUnloadSheet blendUnloadSheet, string blendDescription, bool isGoWithCrew, DateTime expectedOnLocationTime)
        {
            string comments = String.Empty;
            var storageName =blendUnloadSheet.DestinationStorage.Name;

            //Nov 3, 2023 Tongtao P45_Q4_105: Modify numerical precision to 3 decimal places
            //Nov 22, 2023 Tongtao P45_Q4_175: Adjusting precision (integer formatting to one decimal place, keeping one to three decimal places unchanged, rounding to three decimal places, formatting to three decimal places)
            comments += $"Haul " + (blendUnloadSheet.UnloadAmount / 1000).ToString("#.##") +
                        $"t {blendDescription}";
            if (!String.IsNullOrEmpty(storageName))
            {
                comments += $" to {blendUnloadSheet.ShippingLoadSheet.Destination} Bin {storageName}";
            }

            if (isGoWithCrew)
            {
                comments += " Go With Crew";
            }
            else
            {
                comments += " on " + expectedOnLocationTime.ToString("MMM dd,yyyy");
            }

            return comments;
        }

        public static string BuildSingleBlendUnloadSheetComment(BlendUnloadSheet blendUnloadSheet)
        {
            string comments = String.Empty;
            var storageName =blendUnloadSheet.DestinationStorage.Name;

            //Nov 3, 2023 Tongtao P45_Q4_105: Modify numerical precision to 3 decimal places
            //Nov 22, 2023 Tongtao P45_Q4_175: Adjusting precision (integer formatting to one decimal place, keeping one to three decimal places unchanged, rounding to three decimal places, formatting to three decimal places)
            comments += $"Offload " + (blendUnloadSheet.UnloadAmount / 1000).ToString("#.##") +
                        $"t to Bin {storageName}";

            return comments;
        }

        public static string BuildProductHaulName(ProductHaul productHaul)
        {
                return productHaul.Crew.Description + " " + productHaul.EstimatedLoadTime.ToString("MMM d H:mm");
        }
        #endregion

        public static DateTime UpdateShippingLoadSheetExpectedOnLocationTime(ProductHaul originalProductHaul, DateTime updatedExpectedOnLocationTime, DateTime updatedEstimateLoadTime)
        {
            DateTime earlistExpectedOnLocationTime= updatedExpectedOnLocationTime;
            foreach (var shippingLoadSheet in originalProductHaul.ShippingLoadSheets)
            {
                if (shippingLoadSheet.IsGoWithCrew)
                {
                    if (shippingLoadSheet.CallSheetNumber != 0)
                    {
                        var rigJob =
                            eServiceOnlineGateway.Instance.GetRigJobByCallSheetNumber(shippingLoadSheet.CallSheetNumber);
                        shippingLoadSheet.ExpectedOnLocationTime = rigJob.JobDateTime;
                    }
                    else
                        shippingLoadSheet.ExpectedOnLocationTime = originalProductHaul.ExpectedOnLocationTime;
                }
                else
                {
                    shippingLoadSheet.ExpectedOnLocationTime = updatedExpectedOnLocationTime;
                }

                if (earlistExpectedOnLocationTime > shippingLoadSheet.ExpectedOnLocationTime)
	                earlistExpectedOnLocationTime = shippingLoadSheet.ExpectedOnLocationTime;

                shippingLoadSheet.EstimatedLoadTime = updatedEstimateLoadTime;
            }

            return earlistExpectedOnLocationTime;
        }

        public static string BuildProductHaulDescription(ProductHaul productHaul)
        {
	        return string.Join(";", productHaul.ShippingLoadSheets.Select(p => p.Description));
        }

        public static string BuildShippingLoadSheetComments(ShippingLoadSheet shippingLoadSheet)
        {

            string comments = String.Empty;

            if (shippingLoadSheet == null) return comments;

            var blendDescription = string.IsNullOrEmpty(shippingLoadSheet.BlendDescription) == true
	            ? string.Empty
	            : shippingLoadSheet.BlendDescription.Split('+')[0].Trim();

            //Nov 3, 2023 Tongtao P45_Q4_105: Modify numerical precision to 3 decimal places
            //Nov 22, 2023 Tongtao P45_Q4_175: Adjusting precision (integer formatting to one decimal place, keeping one to three decimal places unchanged, rounding to three decimal places, formatting to three decimal places)

            comments += $"Haul " + (shippingLoadSheet.LoadAmount / 1000).ToString("#.##") +
                        $"t {blendDescription}";
            comments += " from " + shippingLoadSheet.BulkPlant.Name;
           if(shippingLoadSheet.SourceStorage != null && !string.IsNullOrEmpty(shippingLoadSheet.SourceStorage.Name))
               comments += " Bin " + shippingLoadSheet.SourceStorage.Name + " ";
           comments += " to " + shippingLoadSheet.Rig.Name;

            if (shippingLoadSheet.IsGoWithCrew)
            {
                comments += " Go With Crew.";
            }
            else
            {
                comments += " on " + shippingLoadSheet.ExpectedOnLocationTime.ToString("MMM dd,yyyy")+";";
            }


            if (shippingLoadSheet.BlendUnloadSheets!=null && shippingLoadSheet.BlendUnloadSheets.Count > 0)
            {
                foreach (var blendUnloadSheet in shippingLoadSheet.BlendUnloadSheets)
                {
                    if (blendUnloadSheet.UnloadAmount > 0.01)
                    {
                        comments = comments + BuildSingleBlendUnloadSheetComment(blendUnloadSheet) + ";";
                    }
                }
            }

            if(comments.Length>2)
            {
                comments = comments.Remove(comments.Length - 1, 1);
                comments += ".";
            }

            return comments;
        }
        //Nov 21, 2023 zhangyuan P63_Q4_174: add transfer shippingLoadSheetCount comments
        public static string BuildTransferBlendComment(ShippingLoadSheet shippingLoadSheet, BlendUnloadSheet blendUnloadSheet)
        {
            string comments = String.Empty;
            var sourceStorageName = shippingLoadSheet.SourceStorage.Name;
            var unloadStorageName = blendUnloadSheet.DestinationStorage.Name;
            var blendDescription = string.IsNullOrEmpty(shippingLoadSheet.BlendDescription) == true
                ? string.Empty
                : shippingLoadSheet.BlendDescription.Split('+')[0].Trim();

            //Nov 24,2013 AW develop: Roll back TongTao's change on Nov 3 and after.
            comments += $"Transfer " + (shippingLoadSheet.LoadAmount / 1000).ToString("#.###") +
                        $"t {blendDescription} from {sourceStorageName} to {unloadStorageName}";


            comments += " on " + shippingLoadSheet.OnLocationTime.ToString("MMM dd,yyyy");
            // Transfer xx t[Blend Name] from[Bin 1] to[Bin 2] on[Date]
            return comments;
        }


        //Dec 06, 2023 Tongtao P45_Q4_212: Add loadBlendToBin shippingLoadSheetCount comments
        public static string BuildLoadBlendToBinComments(ShippingLoadSheet shippingLoadSheet,DateTime expectedOnLocationTime)
        {

            string comments = String.Empty;

            if (shippingLoadSheet == null) return comments;

            var blendDescription = string.IsNullOrEmpty(shippingLoadSheet.BlendDescription) == true
                ? string.Empty
                : shippingLoadSheet.BlendDescription.Split('+')[0].Trim();

            comments += $"Load Blend " + (shippingLoadSheet.LoadAmount / 1000).ToString("#.##");
            comments += $"t {blendDescription} ";
            comments += $"to Bin {shippingLoadSheet.Destination}";
            comments += " on " + expectedOnLocationTime.ToString("MMM dd,yyyy") + ";";

            if (comments.Length > 2)
            {
                comments = comments.Remove(comments.Length - 1, 1);
                comments += ".";
            }

            return comments;
        }


        public static void SetThirdPartyDriver(ProductHaul productHaul, ThirdPartyBulkerCrew thirdPartyBulkerCrew)
        {
            productHaul.TractorUnit = null;
            productHaul.BulkUnit = null;
            productHaul.Driver = null;
            productHaul.Driver2 = null;

            productHaul.ThirdPartyUnitNumber = thirdPartyBulkerCrew.ThirdPartyUnitNumber;
            productHaul.ContractorCompany = thirdPartyBulkerCrew.ContractorCompany;
            productHaul.SupplierContactName = thirdPartyBulkerCrew.SupplierContactName;
            productHaul.SupplierContactNumber = thirdPartyBulkerCrew.SupplierContactNumber;

        }
        public static void SetDriverAndBulk(ProductHaul productHaul, SanjelCrew sanjelCrew)
        {
            productHaul.ThirdPartyUnitNumber = null;
            productHaul.ContractorCompany = null;
            productHaul.SupplierContactName = null;
            productHaul.SupplierContactNumber = null;

            if (sanjelCrew!=null && sanjelCrew.SanjelCrewTruckUnitSection != null)
            {
                foreach (var sanjelCrewTruckUnitSection in sanjelCrew.SanjelCrewTruckUnitSection)
                {
                    if (sanjelCrewTruckUnitSection.TruckUnit!=null &&sanjelCrewTruckUnitSection.TruckUnit.UnitMainType!=null&&sanjelCrewTruckUnitSection.TruckUnit.UnitSubType.Id == 276)
                    {
                        productHaul.TractorUnit = eServiceOnlineGateway.Instance.GetTruckUnitById(sanjelCrewTruckUnitSection.TruckUnit.Id);
                    }
                    else
                    {
                        productHaul.BulkUnit = eServiceOnlineGateway.Instance.GetTruckUnitById(sanjelCrewTruckUnitSection.TruckUnit.Id);
                    }
                }
            }
            var employeeCount = 0;
            if (sanjelCrew != null && sanjelCrew.SanjelCrewWorkerSection != null)
            {
                foreach (var sanjelCrewWorkerSection in sanjelCrew.SanjelCrewWorkerSection)
                {
                    if (sanjelCrewWorkerSection.Worker != null)
                    {
                        if (employeeCount == 0)
                        {
                            productHaul.Driver = eServiceOnlineGateway.Instance.GetEmployeeById(sanjelCrewWorkerSection.Worker.Id);
                        }
                        else
                        {
                            productHaul.Driver2 = eServiceOnlineGateway.Instance.GetEmployeeById(sanjelCrewWorkerSection.Worker.Id);
                        }
                        employeeCount++;
                    }
                }
            }

            if (employeeCount == 1) productHaul.Driver2 = null;

        }

        public static void SetProductHaulOnLocation(int productHaulId, DateTime onLocationTime, string userName = null)
        {
	        throw new Exception("Set Product Haul Location is not available from Rig Board any more");
        }
        public static string SetProductHaulLoadedAndAssignmentStatus(int productHaulId, DateTime onLocationTime, RigJobCrewSectionStatus assignmentStatus, string userName = null)
        {
	        //Dec 11, 2023: AW Update the logic for crew status integration. 
	        string result = string.Empty;

	        ProductHaul productHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);

	        if (productHaul == null)
		        result = $"Product Haul {productHaulId} is not found";
	        else
	        {
                //Update all shipping load sheet as Loaded
                foreach (var productHaulShippingLoadSheet in productHaul.ShippingLoadSheets)
                {
	                if (productHaulShippingLoadSheet.ShippingStatus == ShippingStatus.Scheduled && productHaulShippingLoadSheet.BulkPlant.Id == productHaul.BulkPlant.Id)
	                {
                        //Dec 13, 2023 AW: Add logic to load from source storage to bulker

                        string rtn = BinProcess.LoadBlendToBulker(productHaulShippingLoadSheet.Id,  userName,productHaulShippingLoadSheet.SourceStorage?.Id??0);

                        if (!rtn.Equals("Succeed"))
                        {
	                        result += rtn;
                        }
	                }
                }

                if (string.IsNullOrEmpty(result))
                {
	                result = "Succeed";

	                productHaul.ProductHaulLifeStatus = ProductHaulStatus.Loaded;
	                productHaul.ModifiedUserName = userName;
	                eServiceOnlineGateway.Instance.UpdateProductHaul(productHaul, false);

	                CrewProcess.SetBulkerCrewAssignmentStatus(productHaulId, assignmentStatus, productHaul.IsThirdParty,
		                userName);
                }
	        }

	        return result;
        }

        public static string SetProductHaulInProgressAndAssignmentStatus(int productHaulId, DateTime onLocationTime, RigJobCrewSectionStatus assignmentStatus, string userName = null)
        {
            //Dec 7, 2023: AW Update the logic for crew status integration. TODO: following logic should be merged with standard On Location logic
	        string result = "Succeed";
            ProductHaul productHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);

            if (productHaul == null) 
	            result = $"Product Haul {productHaulId} is not found";
            else
            {
                
	            productHaul.ProductHaulLifeStatus = ProductHaulStatus.InProgress;
	            productHaul.ModifiedUserName = userName;
	            eServiceOnlineGateway.Instance.UpdateProductHaul(productHaul, false);

	            CrewProcess.SetBulkerCrewAssignmentStatus(productHaulId, assignmentStatus, productHaul.IsThirdParty, userName);
            }

            return result;
        }
        public static string SetProductHaulOnLocationAndAssignmentStatus(int productHaulId, DateTime onLocationTime, RigJobCrewSectionStatus assignmentStatus, string userName = null)
        {
	        //Dec 7, 2023: AW Update the logic for crew status integration. TODO: following logic should be merged with standard On Location logic
	        string result = string.Empty;
	        ProductHaul productHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);
            bool isRigToRig = false;
	        if (productHaul == null)
		        result = $"Product Haul {productHaulId} is not found";
	        else
	        {
                // Feb 5, 2024 zhangyuan 195_PR_Haulback: Modify Set different states for ProductHaul in different destinations
                List<ShippingLoadSheet> shippingLoadSheets =
			        eServiceOnlineGateway.Instance.GetShippingLoadSheetByProductHaulId(productHaul.Id);

                List<int> bulkPlantIds = EServiceReferenceData.Data.BulkPlantCollection.Select(p => p.Id).ToList();
                List<int> rigIds = EServiceReferenceData.Data.RigCollection.Where(p=>p.OperationSiteType!= OperationSiteType.BulkPlant).Select(p => p.Id).ToList();

                if (shippingLoadSheets.Count > 0)
                {
	                foreach (var shippingLoadSheet in shippingLoadSheets)
	                {
		                if (shippingLoadSheet.ShippingStatus != ShippingStatus.OnLocation)
                        {
                            //Feb 22, 2024 zhangyuan 295_PR_BulkerCrewstatus: Add Rig To Rig Onlocation condition
                            isRigToRig = rigIds.Contains(shippingLoadSheet.BulkPlant.Id) &&
                                         rigIds.Contains(shippingLoadSheet.Rig.Id);
                            //If destination is a Bulk Plant OR Rig, load rig bin blend to bulker
                            if (bulkPlantIds.Contains(shippingLoadSheet.Rig.Id)|| isRigToRig)
			                {
				                if (shippingLoadSheet.ShippingStatus != ShippingStatus.Loaded)
				                {
					                string rtn = BinProcess.LoadBlendToBulker(shippingLoadSheet.Id, userName,
						                shippingLoadSheet.SourceStorage.Id);
					                if (assignmentStatus == RigJobCrewSectionStatus.Returned)
					                {
						                string rtn1 =
							                UpdateShippingLoadSheetOnLocation(shippingLoadSheet, onLocationTime, userName);
						                if (!rtn1.Equals("Succeed"))
						                {
							                result += rtn1;
						                }
                                    }
                                    if (!rtn.Equals("Succeed"))
					                {
						                result += rtn;
					                }
				                }
                                else
				                {
					                string rtn1 =
						                UpdateShippingLoadSheetOnLocation(shippingLoadSheet, onLocationTime, userName);
					                if (!rtn1.Equals("Succeed"))
					                {
						                result += rtn1;
					                }
				                }
			                }
			                //If destination is a Rig, set it on location
			                else
			                {
				                string rtn =
					                UpdateShippingLoadSheetOnLocation(shippingLoadSheet, onLocationTime, userName);
				                if (!rtn.Equals("Succeed"))
				                {
					                result += rtn;
				                }
			                }
		                }
                    }
                }

                if (string.IsNullOrEmpty(result))
		        {
			        result = "Succeed";

			        productHaul.ProductHaulLifeStatus = assignmentStatus == RigJobCrewSectionStatus.Returned
				        ? ProductHaulStatus.Returned
				        : ProductHaulStatus.OnLocation;
			        productHaul.ModifiedUserName = userName;
			        eServiceOnlineGateway.Instance.UpdateProductHaul(productHaul, false);

			        CrewProcess.SetBulkerCrewAssignmentStatus(productHaulId, assignmentStatus, productHaul.IsThirdParty,
				        userName);
		        }
	        }

	        return result;
        }

        /*
        public static ProductHaul SetProductHaulOnLocationIfAllOnLocation(int productHaulId, DateTime onLocationTime, string userName = null)
        {
	        ProductHaul productHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);
	        if (productHaul == null) return null;
	        List<ShippingLoadSheet> shippingLoadSheets = eServiceOnlineGateway.Instance.GetShippingLoadSheetByProductHaulId(productHaul.Id).FindAll(p=>p.ShippingStatus != ShippingStatus.OnLocation);
	        if (shippingLoadSheets.Count == 0)
	        {
		        productHaul.ProductHaulLifeStatus = ProductHaulStatus.OnLocation;
		        productHaul.ModifiedUserName = userName;
		        eServiceOnlineGateway.Instance.UpdateProductHaul(productHaul, false);

		        CrewProcess.SetBulkerCrewAssignmentStatus(productHaulId, RigJobCrewSectionStatus.OnLocation, productHaul.IsThirdParty, userName);
	        }

            return productHaul;
        }
        */

        public static void AssignProductHaulCrew(ProductHaul productHaul, SanjelCrew crew, RigJob rigJob,
	        string username = null)
        {
	        /*
	        RigBoardProcess.CreateSchedule(model.IsThirdParty ? model.ThirdPartyBulkerCrewId : model.CrewId,
		        model.IsThirdParty, model.EstimatedLoadTime, rigJobId, model.IsGoWithCrew,
		        GetScheduleEndTime(model, rigJobId), originalProductHaul);
	        ProductHaulProcess.SendBulkerCrewAssignmentNotification((SanjelCrew)originalProductHaul.Crew, rigJob,
		        originalProductHaul, userName);
		        */

        }
        public static RigJobCrewSectionStatus ReleaseProductHaulCrew(ProductHaul originalProductHaul, string username=null)
        {
	        RigJobCrewSectionStatus assignmentStatus = RigJobCrewSectionStatus.Scheduled;

            if (!originalProductHaul.IsThirdParty)
            {
                RigJobSanjelCrewSection rigJobSanjelCrewSection = eServiceOnlineGateway.Instance.GetRigJobCrewSectionsByProductHaul(originalProductHaul.Id);
                if (rigJobSanjelCrewSection != null)
                {
	                assignmentStatus = rigJobSanjelCrewSection.RigJobCrewSectionStatus;

                    var crew = rigJobSanjelCrewSection.SanjelCrew;
                    CrewProcess.ReleaseSanjelCrew(rigJobSanjelCrewSection);

                    if(SendCrewNotification)
						SendBulkerCrewUnAssignmentNotification(crew, originalProductHaul, username??originalProductHaul.ModifiedUserName);
                }
            }
            else
            {
                RigJobThirdPartyBulkerCrewSection rigJobThirdPartyBulkerCrewSection = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionsByProductHual(originalProductHaul.Id);
                if (rigJobThirdPartyBulkerCrewSection != null)
                {
	                assignmentStatus = rigJobThirdPartyBulkerCrewSection.RigJobCrewSectionStatus;
                    CrewProcess.ReleaseThirdPartyCrew(rigJobThirdPartyBulkerCrewSection);
                }
            }
            //Feb 08, 2024 AW: Remove duplicate status update
            return assignmentStatus;
        }

        public static string UpdateShippingLoadSheetOnLocation(ShippingLoadSheet shippingLoadSheet, DateTime onLocationTime,
            string userName = null)
        {
	        string result = string.Empty;
            if (!shippingLoadSheet.IsGoWithCrew && shippingLoadSheet.BlendUnloadSheets != null &&
	            shippingLoadSheet.BlendUnloadSheets.Count != 0)
            {
                foreach (var blendUnloadSheet in shippingLoadSheet.BlendUnloadSheets)
                {
                    //In case invalid blend unload sheet is saved in database
	                if (Math.Abs(blendUnloadSheet.UnloadAmount) < 0.01) continue;
                    var destinationStorage =
                        eServiceOnlineGateway.Instance.GetBinInformationById(blendUnloadSheet.DestinationStorage.Id);
                    if (destinationStorage != null)
                    {
	                   var rtn = BinProcess.LoadBlendToBin(destinationStorage.Bin,
                            destinationStorage.PodIndex, shippingLoadSheet.Name,
                            shippingLoadSheet.BlendDescription,
	                        blendUnloadSheet.UnloadAmount,
                            shippingLoadSheet.ProductHaulLoad, shippingLoadSheet, userName);
	                   if (!rtn.Equals("Succeed"))
	                   {
		                   result += rtn;
	                   }
                    }
                }
            }


            if (string.IsNullOrEmpty(result))
            {
	            result = "Succeed";
	            shippingLoadSheet.ShippingStatus = ShippingStatus.OnLocation;
	            shippingLoadSheet.OnLocationTime = onLocationTime;
	            shippingLoadSheet.ModifiedUserName = userName;
	            eServiceOnlineGateway.Instance.UpdateShippingLoadSheet(shippingLoadSheet,false);

	            if (shippingLoadSheet.ProductHaulLoad != null && shippingLoadSheet.ProductHaulLoad.Id != 0)
	            {
		            var productHaulLoad =
			            eServiceOnlineGateway.Instance.GetProductHaulLoadById(shippingLoadSheet.ProductHaulLoad.Id);

		            var shippingLoadSheets =
			            eServiceOnlineGateway.Instance.GetShippingLoadSheetByProductHaulLoadId(productHaulLoad.Id);
		            if (shippingLoadSheets.Count == 1)
		            {
			            SetProductHaulLoadOnLocation(onLocationTime, userName, productHaulLoad);
		            }
		            else
		            {
			            if (productHaulLoad.RemainsAmount < 0.0 || Math.Abs(productHaulLoad.RemainsAmount) < 0.1)
			            {
				            bool allOnLocation = true;
				            foreach (var loadSheet in shippingLoadSheets)
				            {
					            if (loadSheet.ShippingStatus != ShippingStatus.OnLocation)
					            {
						            allOnLocation = false;
						            break;
					            }
				            }

				            if (allOnLocation)
				            {
					            SetProductHaulLoadOnLocation(onLocationTime, userName, productHaulLoad);
				            }
			            }
		            }

	            }
            }

            return result;
        }

        public static void SetProductHaulLoadOnLocation(DateTime onLocationTime, string userName,
            ProductHaulLoad productHaulLoad)
        {
            productHaulLoad.ProductHaulLoadLifeStatus = ProductHaulLoadStatus.OnLocation;
            productHaulLoad.OnLocationTime = onLocationTime;
            productHaulLoad.ModifiedUserName = userName;
            eServiceOnlineGateway.Instance.UpdateProductHaulLoad(productHaulLoad);
        }
        public static void SetProductHaulStatus(int productHaulId, ProductHaulStatus productHaulStatus, string userName)
        {
            ProductHaul productHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);
            if (productHaul == null) return;
            productHaul.ModifiedUserName = userName;
            productHaul.ProductHaulLifeStatus = productHaulStatus;
            eServiceOnlineGateway.Instance.UpdateProductHaul(productHaul, false);

        }

        // Dec 25, 2023 zhangyuan 195_PR_Haulback: Add HaulBack Comments
        #region  Haulback Comments

        public static string BuildHaulBackBlendUnloadSheetComments(BlendUnloadSheet blendUnloadSheet, string blendDescription, bool isGoWithCrew, DateTime expectedOnLocationTime)
        {
            string comments = String.Empty;
            var storageName = blendUnloadSheet.DestinationStorage?.Name;

            //Nov 3, 2023 Tongtao P45_Q4_105: Modify numerical precision to 3 decimal places
            //Nov 22, 2023 Tongtao P45_Q4_175: Adjusting precision (integer formatting to one decimal place, keeping one to three decimal places unchanged, rounding to three decimal places, formatting to three decimal places)
            comments += $"Back Haul " + (blendUnloadSheet.UnloadAmount / 1000).ToString("#.##") +
                        $"t {blendDescription}";
            if (!String.IsNullOrEmpty(storageName))
            {
                comments += $" to {blendUnloadSheet.ShippingLoadSheet.Destination} Bin {storageName}";
            }

            if (isGoWithCrew)
            {
                comments += " Go With Crew";
            }
            else
            {
                comments += " on " + expectedOnLocationTime.ToString("MMM dd,yyyy");
            }

            return comments;
        }
        public static string BuildHaulBackShippingLoadSheetComments(ShippingLoadSheet shippingLoadSheet)
        {

            string comments = String.Empty;

            if (shippingLoadSheet == null) return comments;

            var blendDescription = string.IsNullOrEmpty(shippingLoadSheet.BlendDescription) == true
                ? string.Empty
                : shippingLoadSheet.BlendDescription.Split('+')[0].Trim();

            //Nov 3, 2023 Tongtao P45_Q4_105: Modify numerical precision to 3 decimal places
            //Nov 22, 2023 Tongtao P45_Q4_175: Adjusting precision (integer formatting to one decimal place, keeping one to three decimal places unchanged, rounding to three decimal places, formatting to three decimal places)

            comments += $"Haul " + (shippingLoadSheet.LoadAmount / 1000).ToString("#.##") +
                        $"t {blendDescription}";

            comments += " from " + shippingLoadSheet.BulkPlant.Name + " Bin " + shippingLoadSheet.SourceStorage.Name + " ";

            comments += " to " + shippingLoadSheet.Rig.Name;

            if (shippingLoadSheet.IsGoWithCrew)
            {
                comments += " Go With Crew.";
            }
            else
            {
                comments += " on " + shippingLoadSheet.ExpectedOnLocationTime.ToString("MMM dd,yyyy") + ";";
            }


            if (shippingLoadSheet.BlendUnloadSheets != null && shippingLoadSheet.BlendUnloadSheets.Count > 0)
            {
                foreach (var blendUnloadSheet in shippingLoadSheet.BlendUnloadSheets)
                {
                    if (blendUnloadSheet.UnloadAmount > 0.01)
                    {
                        comments = comments + BuildHaulBackSingleBlendUnloadSheetComment(blendUnloadSheet, shippingLoadSheet.Rig.Name) + ";";
                    }
                }
            }

            if (comments.Length > 2)
            {
                comments = comments.Remove(comments.Length - 1, 1);
                comments += ".";
            }

            return comments;
        }
        public static string BuildHaulBackSingleBlendUnloadSheetComment(BlendUnloadSheet blendUnloadSheet, string bulkerPlantName)
        {
            string comments = String.Empty;
            var storageName = blendUnloadSheet.DestinationStorage?.Name;

            //Nov 3, 2023 Tongtao P45_Q4_105: Modify numerical precision to 3 decimal places
            //Nov 22, 2023 Tongtao P45_Q4_175: Adjusting precision (integer formatting to one decimal place, keeping one to three decimal places unchanged, rounding to three decimal places, formatting to three decimal places)
            comments += $"Offload " + (blendUnloadSheet.UnloadAmount / 1000).ToString("#.##") +
                        $"t to {bulkerPlantName}";
            if (!string.IsNullOrEmpty(storageName))
            {
	            comments += $" Bin {storageName}";
            }

            return comments;
        }
        //Feb 6, 2024 zhangyuan 195_PR_Haulback: delete back Haul ProductHaul comments
        /* 
        public static string BuildHaulBackProductHaulDescription(ProductHaul productHaul)
        {
            string comments = String.Empty;
            List<ShippingLoadSheet> shippingLoadSheets = productHaul.ShippingLoadSheets.OrderBy(p => p.ExpectedOnLocationTime).ToList();

            if (!String.IsNullOrEmpty(productHaul.Comments)) comments = productHaul.Comments + " ";

            foreach (var shippingLoadSheet in productHaul.ShippingLoadSheets)
            {
                var shippingLoadSheet1 = shippingLoadSheet;
                if (shippingLoadSheet.Id > 0)
                {
                    shippingLoadSheet1 = eServiceOnlineGateway.Instance.GetShippingLoadSheetById(shippingLoadSheet.Id, true);
                }
                comments += ProductHaulProcess.BuildHaulBackShippingLoadSheetComments(shippingLoadSheet1);
            }
            return comments;
        }
        */

        #endregion

        public static RigJobSanjelCrewSection CreateRigJobSanjelCrewSection(SanjelCrew crew, ProductHaul productHaul, RigJob rigJob, RigJobCrewSectionStatus rigJobCrewSectionStatus, string username = null)
        {
	        RigJobSanjelCrewSection section = new RigJobSanjelCrewSection
	        {
		        RigJob = rigJob,
		        SanjelCrew = crew,
		        RigJobCrewSectionStatus = rigJobCrewSectionStatus,
		        ProductHaul = productHaul,
                ModifiedUserName = username
	        };
	        int rtn = eServiceOnlineGateway.Instance.CreateRigJobCrewSection(section);

	        if (SendCrewNotification && rtn == 1 && !string.IsNullOrEmpty(username))
	        {
                SendBulkerCrewAssignmentNotification(crew, rigJob, productHaul, username);
	        }
	        return section;
        }

        public static void SendBulkerCrewAssignmentNotification(SanjelCrew crew, RigJob rigJob,
	        ProductHaul productHaul, string username)
        {
	        List<SanjelCrewWorkerSection> sanjelCrewWorkerSections = RigBoardProcess.GetCrewWorkerSections(crew.Id);
	        List<Employee> workers = new List<Employee>();
	        foreach (var crewWorkerSection in sanjelCrewWorkerSections)
	        {
		        var worker =
			        EServiceReferenceData.Data.EmployeeCollection.FirstOrDefault(p =>
				        p.Id == crewWorkerSection.Worker.Id);
		        if (worker != null)
			        workers.Add(worker);
	        }

	        string clientName = string.Empty;
	        string destination = string.Empty;

	        foreach (var shippingLoadSheet in productHaul.ShippingLoadSheets)
	        {
		        var clientShortName = string.IsNullOrEmpty(shippingLoadSheet.ClientName)?"Sanjel": shippingLoadSheet.ClientName.Split(new char[] { ' ' })?.First();

                if (clientName != clientShortName)
			        clientName = clientName + clientShortName + "/";
                if(destination != shippingLoadSheet.Destination)
                    destination = destination + shippingLoadSheet.Destination + "/";
	        }

	        clientName = clientName.TrimEnd('/');
	        destination = destination.TrimEnd('/');

	        string bulkPlant = productHaul.BulkPlant.Name?.Replace(" Bulk Plant","");
            string url = ConfigurationManager.AppSettings["AppServiceAddress"] + "bulker-assignment-mobile";

            var recipients = workers.Where(e => !string.IsNullOrEmpty(e.WorkEmail)).Select(e => e.WorkEmail);
	        INotificationsService notificationService =
		        MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<INotificationsService>();
	        string jobType = rigJob == null ? "Product Haul" :
		        rigJob.JobType == null ? "Product Haul" : rigJob.JobType.Name;
            //Apr 10, 2024 zhangyuan 317_Fix_Crew_Assigned_Notification_missing: Add exception handling with jobTypeName being empty
            if (jobType == null)
            {
                throw new Exception("RigJob "+rigJob.Id.ToString() +" jobTypeName is Null");
            }

            notificationService.SendNotificationAsync(action: "assign", actionUser: username + "@sanjel.com", recipients: recipients,
		        company: clientName, rig: destination, district:"",jobType: jobType, crewType: "bulker", bulkPlant: bulkPlant,
		        jobStatus: productHaul.IsGoWithCrew?"Go With Crew" : "Haul",  url: url);
        }

        public static void SendBulkerCrewUnAssignmentNotification(SanjelCrew crew, ProductHaul productHaul, string username)
        {
            List<SanjelCrewWorkerSection> sanjelCrewWorkerSections = RigBoardProcess.GetCrewWorkerSections(crew.Id);
            List<Employee> workers = new List<Employee>();
            foreach (var crewWorkerSection in sanjelCrewWorkerSections)
            {
                var worker =
                    EServiceReferenceData.Data.EmployeeCollection.FirstOrDefault(p =>
                        p.Id == crewWorkerSection.Worker.Id);
                if (worker != null)
                    workers.Add(worker);
            }

            string clientName = string.Empty;
            string destination = string.Empty;

            foreach (var shippingLoadSheet in productHaul.ShippingLoadSheets)
            {
                var clientShortName = string.IsNullOrEmpty(shippingLoadSheet.ClientName) ? "Sanjel":shippingLoadSheet.ClientName.Split(new char[] { ' ' })?.First();

                if (clientName != clientShortName)
                    clientName = clientName + clientShortName + "/";
                if (destination != shippingLoadSheet.Destination)
                    destination = destination + shippingLoadSheet.Destination + "/";
            }

            clientName = clientName.TrimEnd('/');
            destination = destination.TrimEnd('/');
            // Feb 23, 2024 zhangyuan 295_PR_BulkerCrewstatus: Modify if HaulBack productHaul BulkPlant is null
            string bulkPlant = productHaul.BulkPlant.Name?.Replace(" Bulk Plant", "")??"";
            string url = ConfigurationManager.AppSettings["AppServiceAddress"] + "bulker-assignment";

            var recipients = workers.Where(e => !string.IsNullOrEmpty(e.WorkEmail)).Select(e => e.WorkEmail);
            INotificationsService notificationService =
                MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<INotificationsService>();
            notificationService.SendNotificationAsync(action: "unassign", actionUser: username + "@sanjel.com", recipients: recipients,
                company: clientName, rig: destination, district: "", jobType: "", crewType: "bulker", bulkPlant: bulkPlant,
                jobStatus: productHaul.IsGoWithCrew ? "Go With Crew" : "Haul", url: url);
        }

        public static DateTime GetScheduleEndTime(ProductHaul productHaul, RigJob rigJob)
        {
	        DateTime endTime;
	        if (productHaul.IsGoWithCrew && rigJob != null)
	        {
		        endTime = rigJob.JobDateTime.AddMinutes(rigJob.JobDuration == 0 ? 360 : rigJob.JobDuration);
	        }
	        else
	        {
		        endTime = productHaul.ExpectedOnLocationTime.AddHours(productHaul.EstimatedTravelTime);
	        }

	        return endTime;
        }

        public static DateTime GetScheduleStartTime(ProductHaul productHaul, RigJob rigJob)
        {
	        DateTime startTime;
	        if (productHaul.IsGoWithCrew && rigJob != null)
	        {
		        startTime = rigJob.CallCrewTime == DateTime.MinValue ? rigJob.JobDateTime : rigJob.CallCrewTime;
	        }
	        else
	        {
		        startTime = productHaul.ExpectedOnLocationTime.AddHours((-1) * productHaul.EstimatedTravelTime);
	        }

	        return startTime;
        }

    }
}