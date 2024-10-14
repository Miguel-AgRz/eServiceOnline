using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eServiceOnline.Gateway;
using Sanjel.BusinessEntities.Sections.Common;
using Sanjel.Common.BusinessEntities.Lookup;
using Sanjel.Common.BusinessEntities.Reference;
using Sanjel.Common.EService.Sections.Common;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Crew;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ResourceSchedule;
using Sesi.SanjelData.Entities.Common.Model.Scheduling;

namespace eServiceOnline.BusinessProcess
{
    public class CrewProcess
    {
        public static void ReleaseSanjelCrew(RigJobSanjelCrewSection rigJobCrewSection)
        {
            var sanjelCrewId = rigJobCrewSection.SanjelCrew.Id;
            SanjelCrewSchedule crewSchedule =
                eServiceOnlineGateway.Instance.GetCrewScheduleByJobCrewSection(rigJobCrewSection.Id);

            //crewSchedule = eServiceOnlineGateway.Instance.GetCrewScheduleById(crewSchedule.Id);

            //if (crewSchedule != null && crewSchedule.SanjelCrew != null)
            //{
            //    ScheduleProcess.DeleteSanjelCrewSchedule(crewSchedule);
            //}

            //Jan 12, 2024 tongtao 257_PR_EstimatedLoadTimeExpectedOnLocationTimeUpdateBug: if crewSchedule is null, then not check SanjelCrew
            if (crewSchedule?.SanjelCrew != null)
            {
                ScheduleProcess.DeleteSanjelCrewSchedule(crewSchedule);
            }

            eServiceOnlineGateway.Instance.DeleteRigJobCrewSection(rigJobCrewSection);
            CrewProcess.UpdateBulkerCrewStatus(sanjelCrewId, false, "");
        }

        public static void ReleaseThirdPartyCrew(RigJobThirdPartyBulkerCrewSection rigJobThirdPartyBulkerCrewSection)
        {
            var crewId = rigJobThirdPartyBulkerCrewSection.ThirdPartyBulkerCrew.Id;
            ThirdPartyBulkerCrewSchedule crewSchedule =
                eServiceOnlineGateway.Instance.GetThirdPartyBulkerCrewSchedule(rigJobThirdPartyBulkerCrewSection.Id);
            if (crewSchedule != null && crewSchedule.ThirdPartyBulkerCrew != null)
            {
                ScheduleProcess.DeleteThirdPartyCrewSchedule(crewSchedule);
            }

            eServiceOnlineGateway.Instance.DeleteRigJobThirdPartyBulkerCrewSection(rigJobThirdPartyBulkerCrewSection.Id);
            CrewProcess.UpdateBulkerCrewStatus(crewId, true, "");
        }

        public static void LogoffSanjelCrew(int rigJobSanjelCrewSectionId, DateTime jobCompleteTime)
        {
            SanjelCrewSchedule crewSchedule = eServiceOnlineGateway.Instance.GetCrewScheduleByJobCrewSection(rigJobSanjelCrewSectionId);
            crewSchedule.EndTime = jobCompleteTime;
            //AW 07/13/2023
            //When a crew finishes a job duty, it should kept as assigned to past job but a different status, maybe "Completed". OffDuty means the crew is in reset due to DRB definition.
            //Keep the schedule type as Assigned for now, clarify and update in future. This may slow down the performance.
//            crewSchedule.Type = CrewScheduleType.NotApplicable;

            foreach (var workerSchedule in crewSchedule.WorkerSchedule)
            {
                workerSchedule.EndTime = jobCompleteTime;
            }
            foreach (var unitSchedule in crewSchedule.UnitSchedule)
            {
                unitSchedule.EndTime = jobCompleteTime;
            }

            eServiceOnlineGateway.Instance.UpdateSanjelCrewSchedule(crewSchedule, true);
        }

        public static void SetBulkerCrewAssignmentStatus(int productHaulId, RigJobCrewSectionStatus status,
            bool isThirdParty, string userName)
        {
            if (!isThirdParty)
            {
                RigJobSanjelCrewSection rigJobSanjelCrewSection =
                    eServiceOnlineGateway.Instance.GetRigJobCrewSectionsByProductHaul(productHaulId);

                if (rigJobSanjelCrewSection != null)
                {
                    rigJobSanjelCrewSection.RigJobCrewSectionStatus = status;
                    rigJobSanjelCrewSection.ModifiedUserName = userName;
                    eServiceOnlineGateway.Instance.UpdateRigJobCrewSection(rigJobSanjelCrewSection);

                }
            }
            else
            {
                RigJobThirdPartyBulkerCrewSection rigJobThirdPartyBulkerCrewSection =
                    eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionsByProductHual(productHaulId);

                if (rigJobThirdPartyBulkerCrewSection != null)
                {
                    rigJobThirdPartyBulkerCrewSection.RigJobCrewSectionStatus = status;
                    rigJobThirdPartyBulkerCrewSection.ModifiedUserName = userName;
                    eServiceOnlineGateway.Instance.UpdateRigJobThirdPartyCrewSection(rigJobThirdPartyBulkerCrewSection);

                }
            }

        }
        public static void SetBulkerCrewLog(int crewId, BulkerCrewStatus status, bool isThirdParty, string userName)
        {

            BulkerCrewLog currentBulkerCrewLog = isThirdParty ? eServiceOnlineGateway.Instance.GetThirdPartyBulkerCrewLog(crewId) : eServiceOnlineGateway.Instance.GetBulkerCrewLog(crewId);
            if (currentBulkerCrewLog == null)
            {
                BulkerCrewLog bulkerCrewLog = new BulkerCrewLog();
                bulkerCrewLog.CrewStatus = status;
                bulkerCrewLog.ModifiedUserName = userName;
                bulkerCrewLog.LastUpdatedTime = DateTime.Now;
                bulkerCrewLog.EnrouteTime = DateTime.MinValue;
                if (isThirdParty)
                    bulkerCrewLog.ThirdPartyBulkerCrew = new ThirdPartyBulkerCrew() { Id = crewId };
                else
                    bulkerCrewLog.SanjelCrew = new SanjelCrew() { Id = crewId };
                eServiceOnlineGateway.Instance.CreateBulkerCrewLog(bulkerCrewLog);
            }
            else
            {
	            if (currentBulkerCrewLog.CrewStatus != status)
	            {
		            currentBulkerCrewLog.CrewStatus = status;
		            currentBulkerCrewLog.ModifiedUserName = userName;
		            switch (status)
		            {
			            case BulkerCrewStatus.OffDuty:
				            currentBulkerCrewLog.EnrouteTime = DateTime.MinValue;
				            currentBulkerCrewLog.LastUpdatedTime = DateTime.Now;
				            break;
			            case BulkerCrewStatus.EnRoute:
				            currentBulkerCrewLog.EnrouteTime = DateTime.Now;
				            break;
			            case BulkerCrewStatus.LoadRequested:
			            case BulkerCrewStatus.Called:
			            case BulkerCrewStatus.Loading:
			            case BulkerCrewStatus.Loaded:
				            break;
			            default:
				            currentBulkerCrewLog.LastUpdatedTime = DateTime.Now;
				            break;
		            }
		            eServiceOnlineGateway.Instance.UpdateBulkerCrewLog(currentBulkerCrewLog);
	            }
            }
        }
        public static string ChangeSanjelCrewStatus1(int crewId, int productHaulid, BulkerCrewStatus status, RigJobCrewSectionStatus assignmentStatus, ProductHaulStatus productHaulStatus, bool isThirdParty, string userName)
        {
            string result = "Succeed";
            //assignmentStatusUpdate is only for updating crew status without other business entity changed
            bool assignmentStatusUpdate = false;
            bool bulkerCrewStatusUpdate = true;

            switch (status)
            {
                case BulkerCrewStatus.Called:
                    //crew is called is a shift action, it doesn't affect any assignment status. Just do a status check to assure data integrity
                    if (assignmentStatus != RigJobCrewSectionStatus.Scheduled || productHaulStatus != ProductHaulStatus.Scheduled)
                    {
                        result = "Current assignment or Product Haul status is not SCHEDULED";
                    }
                    else
                    {
                        assignmentStatus = RigJobCrewSectionStatus.Called;
                        assignmentStatusUpdate = true;
                    }
                    break;
                case BulkerCrewStatus.Loading:
                    if (assignmentStatus == RigJobCrewSectionStatus.Called || assignmentStatus == RigJobCrewSectionStatus.Scheduled)
                    {
                        assignmentStatus = RigJobCrewSectionStatus.Loading;
                        assignmentStatusUpdate = true;
                    }
                    else
                    {
                        result = "Current assignment status is not SCHEDULED or CALLED";
                    }
                    
				    if (productHaulStatus != ProductHaulStatus.Scheduled)
				        result = "Product Haul status is not SCHEDULED";

                    break;
                case BulkerCrewStatus.Loaded:

                    if (assignmentStatus == RigJobCrewSectionStatus.Called || assignmentStatus == RigJobCrewSectionStatus.Loading || assignmentStatus == RigJobCrewSectionStatus.Scheduled)
                    {
	                    // Dec 21, 2023 AW: Disable blend complete validation until better BPAVS integration is built.
	                    /*
	                    string rtn = CheckBlendCompleted(productHaulid);

                        if (rtn.Equals("Succeed"))
	                    {
		                    */
		                    result = ProductHaulProcess.SetProductHaulLoadedAndAssignmentStatus(productHaulid,
			                    DateTime.Now, RigJobCrewSectionStatus.Loaded);
	                    /*
	                    }
	                    else
                        {
	                        result = rtn; ;
	                    }
                    */
                    }
                    else
                    {
                        result = "Current assignment status is not SCHEDULED or CALLED or LOADING";
                    }

                    if (productHaulStatus != ProductHaulStatus.Scheduled)
	                    result = "Product Haul status is not SCHEDULED";
                    break;
                case BulkerCrewStatus.EnRoute:

                    if (assignmentStatus == RigJobCrewSectionStatus.Loaded)
                    {
	                    result = ProductHaulProcess.SetProductHaulInProgressAndAssignmentStatus(productHaulid, DateTime.Now, RigJobCrewSectionStatus.EnRoute);
                    }
                    else
                    {
                        result = "Current assignment status is not LOADED";
                    }
                    break;
                case BulkerCrewStatus.OnLocation:
                    if (assignmentStatus == RigJobCrewSectionStatus.Loaded ||
                        assignmentStatus == RigJobCrewSectionStatus.EnRoute)
                    {
                        // Nov 24, 2023 zhangyuan P63_Q4_127:Add Blend checking alert in product haul #127
                        bool isCheckBackHaul = false;
                        string resDifferent = CheckOnlocationIsDifferentBlend(productHaulid, isCheckBackHaul);
                        if (!resDifferent.Equals("Succeed"))
                        {
                            result = resDifferent;
                        }
                        else
                        {
	                        result = ProductHaulProcess.SetProductHaulOnLocationAndAssignmentStatus(productHaulid, DateTime.Now, RigJobCrewSectionStatus.OnLocation);
                        }
                    }
                    else
                    {
                        result = "Current assignment status is not LOADED or ENROUTE";
                    }
                    break;
                case BulkerCrewStatus.OnWayIn:
                    if (assignmentStatus == RigJobCrewSectionStatus.Loaded ||
                        assignmentStatus == RigJobCrewSectionStatus.EnRoute ||
                        assignmentStatus == RigJobCrewSectionStatus.OnLocation)
                    {
                        bool isCheckBackHaul = false;
                        string resDifferent = CheckOnlocationIsDifferentBlend(productHaulid, isCheckBackHaul);
	                    if (!resDifferent.Equals("Succeed"))
	                    {
		                    result = resDifferent;
	                    }
	                    else
	                    {
		                    result = ProductHaulProcess.SetProductHaulOnLocationAndAssignmentStatus(productHaulid,
				                    DateTime.Now, RigJobCrewSectionStatus.OnWayIn);
                        }
                    }
                    else
                    {
                            result = "Current assignment status is not LOADED or ENROUTE or ON LOCATION";
                    }

                    break;
                case BulkerCrewStatus.Returned:

                    if (assignmentStatus == RigJobCrewSectionStatus.Loaded ||
                        assignmentStatus == RigJobCrewSectionStatus.EnRoute || assignmentStatus == RigJobCrewSectionStatus.OnLocation || assignmentStatus == RigJobCrewSectionStatus.OnWayIn)
                    {
                        bool isCheckBackHaul = true;
                        string resDifferent = CheckOnlocationIsDifferentBlend(productHaulid, isCheckBackHaul);
	                    if (!resDifferent.Equals("Succeed"))
	                    {
		                    result = resDifferent;
	                    }
	                    else
	                    {
		                    result = ProductHaulProcess.SetProductHaulOnLocationAndAssignmentStatus(productHaulid, DateTime.Now, RigJobCrewSectionStatus.Returned);
	                    }
                    }
                    else
                    {
                            result = "Current assignment status is LOADED or ENROUTE or ON LOCATION OR ON WAY IN";
                    }
                    break;
                case BulkerCrewStatus.OffDuty:
	                if (assignmentStatus == RigJobCrewSectionStatus.Returned || assignmentStatus == RigJobCrewSectionStatus.Scheduled)
	                {
		                result = SetCrewOffDuty(crewId, isThirdParty, userName);
		                bulkerCrewStatusUpdate = false;
	                }
                    else if (assignmentStatus != RigJobCrewSectionStatus.LogOffDuty)
		                result = "Current assignment status is not RETURNED OR SCHEDULED";
                    break;
                case BulkerCrewStatus.Down:
                    break;
                default:
	                bulkerCrewStatusUpdate = false;
	                break;
            }

            if (result.Equals("Succeed"))
            {
                if(bulkerCrewStatusUpdate)
		            SetBulkerCrewLog(crewId, status, isThirdParty, userName);

	            if (assignmentStatusUpdate)
	            {
		            SetBulkerCrewAssignmentStatus(productHaulid, assignmentStatus, isThirdParty, userName);
	            }
            }

            return result;
        }

        private static string CheckBlendCompleted(int productHaulid)
        {
	        string result = string.Empty;
	        ProductHaul productHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulid);
	        if (productHaul == null) return "Product Haul is Null";
	        List<ShippingLoadSheet> shippingLoadSheets = productHaul.ShippingLoadSheets
		        ?.Where(p => p.ShippingStatus != ShippingStatus.OnLocation).ToList();
	        if (shippingLoadSheets == null) return "ShippingLoadSheet is Null";

	        foreach (var shippingLoadSheet in shippingLoadSheets)
	        {
		        if (shippingLoadSheet.ProductHaulLoad.Id != 0)
		        {
			        var productHaulLoad =
				        eServiceOnlineGateway.Instance.GetProductHaulLoadById(shippingLoadSheet.ProductHaulLoad.Id);
			        if (productHaulLoad.ProductHaulLoadLifeStatus == ProductHaulLoadStatus.Blending)
			        {
				        result += "Blend Request " + shippingLoadSheet.ProductHaulLoad.Id + " is Blending. ";
			        }
			        else if (productHaulLoad.ProductHaulLoadLifeStatus == ProductHaulLoadStatus.Scheduled)
			        {
				        result += "Blend Request " + shippingLoadSheet.ProductHaulLoad.Id + " is not blended yet. ";
			        }
		        }
            }

	        if (!string.IsNullOrEmpty(result))
	        {
		        return result;
	        }
	        else
	        {
		        return "Succeed";
	        }
        }

        private static BulkerCrewStatus RollbackBulkerCrewStatus(RigJobCrewSectionStatus assignmentStatus, int crewId, string username = null)
        {
	        BulkerCrewStatus status = BulkerCrewStatus.OffDuty;
	        switch (assignmentStatus)
	        {
		        case RigJobCrewSectionStatus.Scheduled:
			        status = BulkerCrewStatus.LoadRequested;
			        break;
                case RigJobCrewSectionStatus.Called:
	                status = BulkerCrewStatus.Called;
	                break;
                case RigJobCrewSectionStatus.Loading:
	                status = BulkerCrewStatus.Loading;
	                break;
                case RigJobCrewSectionStatus.Loaded:
	                status = BulkerCrewStatus.Loaded;
	                break;
                case RigJobCrewSectionStatus.EnRoute:
	                status = BulkerCrewStatus.EnRoute;
	                break;
                case RigJobCrewSectionStatus.OnLocation:
	                status = BulkerCrewStatus.OnLocation;
	                break;
                case RigJobCrewSectionStatus.OnWayIn:
	                status = BulkerCrewStatus.OnWayIn;
	                break;
                case RigJobCrewSectionStatus.Returned:
	                status = BulkerCrewStatus.Returned;
	                break;
                default:
	                status = BulkerCrewStatus.OffDuty;
	                break;
            }

	        return status;
        }

        // Nov 24, 2023 zhangyuan P63_Q4_127:Add Blend checking alert in product haul #127
        private static string CheckOnlocationIsDifferentBlend(int productHaulid,bool isCheckBackHaul)
        {
            string result = string.Empty;
            ProductHaul productHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulid);
            if (productHaul == null) return "Product Haul is Null";
            List<ShippingLoadSheet> shippingLoadSheets = productHaul.ShippingLoadSheets
                ?.Where(p => p.ShippingStatus != ShippingStatus.OnLocation).ToList();
            if (shippingLoadSheets == null) return "ShippingLoadSheet is Null";

            foreach (var shippingLoadSheet in shippingLoadSheets)
            {
                bool isRegularHaul = shippingLoadSheet.BulkPlant.Id == productHaul.BulkPlant.Id;
                if(!isCheckBackHaul&& !isRegularHaul) continue;
                var blendUnloadSheets =
                    eServiceOnlineGateway.Instance.GetBlendUnloadSheetByshippingLoadSheetId(shippingLoadSheet.Id);
                foreach (var blendUnloadSheet in blendUnloadSheets)
	            {
		            var binInformation =
			            eServiceOnlineGateway.Instance.GetBinInformationById(blendUnloadSheet.DestinationStorage.Id);
                    if (binInformation != null && binInformation.BlendChemical != null &&  Math.Abs(binInformation.Quantity) > 0.001  )
		            {
			            if (binInformation.BlendChemical.Description != shippingLoadSheet.BlendDescription)
			            {
				            result =
					            $"Bin {binInformation.Name} Has {binInformation.Quantity} T {binInformation.BlendChemical.Name}. ";
                        }
                    }
	            }
            }

            if (!string.IsNullOrEmpty(result))
            {
	            result +=
		            $"Empty the bin(s) prior to setting status to OnLocation";
	            return result;
            }
            else
            {
	            return "Succeed";
            }
        }

        private static string SetCrewOffDuty(int crewId, bool isThirdParty, string userName)
        {
            string result = "Succeed";
            var crewStatus = BulkerCrewStatus.OffDuty;
            if (!isThirdParty)
            {
                var rigJobSanjelCrewSections =
                eServiceOnlineGateway.Instance.GetActiveBulkerCrewAssignmentByCrewId(crewId);
                if (rigJobSanjelCrewSections.Count > 0)
                {

	                var returnedAssignments = rigJobSanjelCrewSections
		                .Where(p => p.RigJobCrewSectionStatus == RigJobCrewSectionStatus.Returned).ToList();
	                List<RigJobCrewSectionStatus> futureAssignmentStatuses = new List<RigJobCrewSectionStatus>()
	                {
		                RigJobCrewSectionStatus.Scheduled, RigJobCrewSectionStatus.Loading,
		                RigJobCrewSectionStatus.Loaded
	                };
	                var futureAssignments = rigJobSanjelCrewSections
		                .Where(p => futureAssignmentStatuses.Contains(p.RigJobCrewSectionStatus)).ToList();
	                if (returnedAssignments.Count == 0)
	                {
		                if (futureAssignments.Count == 0)
			                result = "No returned bulker crew assignments found.";
	                }
	                else
	                {
		                foreach (var returnedAssignment in returnedAssignments)
		                {
			                returnedAssignment.RigJobCrewSectionStatus = RigJobCrewSectionStatus.LogOffDuty;
			                returnedAssignment.ModifiedUserName = userName;
			                eServiceOnlineGateway.Instance.UpdateRigJobSanjelCrewSection(returnedAssignment);
		                }
	                }

	                crewStatus = futureAssignments.Count != 0
		                ? BulkerCrewStatus.LoadRequested
		                : BulkerCrewStatus.OffDuty;
                }
            }
            else
            {
                var thirdPartyCrewSections = eServiceOnlineGateway.Instance.GetActiveThirdPartyBulkerCrewAssignmentByCrewId(crewId);
                if (thirdPartyCrewSections.Count > 0)
                {

	                var returnedAssignments = thirdPartyCrewSections
		                .Where(p => p.RigJobCrewSectionStatus == RigJobCrewSectionStatus.Returned).ToList();
	                List<RigJobCrewSectionStatus> futureAssignmentStatuses = new List<RigJobCrewSectionStatus>()
	                {
		                RigJobCrewSectionStatus.Scheduled, RigJobCrewSectionStatus.Loading,
		                RigJobCrewSectionStatus.Loaded
	                };
	                var futureAssignments = thirdPartyCrewSections
		                .Where(p => futureAssignmentStatuses.Contains(p.RigJobCrewSectionStatus)).ToList();

	                if (returnedAssignments.Count == 0)
	                {
		                if (futureAssignments.Count == 0)
			                result = "No returned bulker crew assignments found.";
	                }
	                else
	                {
		                foreach (var returnedAssignment in returnedAssignments)
		                {
			                returnedAssignment.RigJobCrewSectionStatus = RigJobCrewSectionStatus.LogOffDuty;
			                returnedAssignment.ModifiedUserName = userName;
			                eServiceOnlineGateway.Instance.UpdateRigJobThirdPartyCrewSection(returnedAssignment);
		                }
	                }

	                crewStatus = futureAssignments.Count != 0
		                ? BulkerCrewStatus.LoadRequested
		                : BulkerCrewStatus.OffDuty;
                }
            }
            SetBulkerCrewLog(crewId, crewStatus, isThirdParty, userName);
            return result;
        }

        public static void UpdateBulkerCrewStatus(int crewId, bool isThirdParty, string userName)
        {
            BulkerCrewLog bulkerCrewLog = null;
            BulkerCrewStatus newStatus = BulkerCrewStatus.None;
            var currentAssignmentStatus = RigJobCrewSectionStatus.Removed;

            if (isThirdParty)
            {
                var currentAssignment = GetCurrentThirdPartyBulkerCrewAssignment(crewId);
                if (currentAssignment == null)
                {
                    newStatus = BulkerCrewStatus.OffDuty;
                }
                else
                {
                    currentAssignmentStatus = currentAssignment.RigJobCrewSectionStatus;
                }
                bulkerCrewLog = eServiceOnlineGateway.Instance.GetThirdPartyBulkerCrewLog(crewId);
            }
            else
            {
                var currentAssignment = GetCurrentBulkerCrewAssignment(crewId);
                if (currentAssignment == null)
                {
                    newStatus = BulkerCrewStatus.OffDuty;
                }
                else
                {
                    currentAssignmentStatus = currentAssignment.RigJobCrewSectionStatus;
                }
                bulkerCrewLog = eServiceOnlineGateway.Instance.GetBulkerCrewLog(crewId);
            }

            if (bulkerCrewLog == null)
            {
	            bulkerCrewLog = new BulkerCrewLog
	            {
		            CrewStatus = BulkerCrewStatus.OffDuty
	            };
            }

            
            switch (currentAssignmentStatus)
            {
                case RigJobCrewSectionStatus.Called:
	                if (bulkerCrewLog.CrewStatus != BulkerCrewStatus.Called)
	                {
		                newStatus = BulkerCrewStatus.Called;
	                }
	                break;
                case RigJobCrewSectionStatus.Scheduled:
                    if (bulkerCrewLog.CrewStatus == BulkerCrewStatus.OffDuty)
                    {
                        newStatus = BulkerCrewStatus.LoadRequested;
                    }
                    break;
                case RigJobCrewSectionStatus.Loading:
                    if (bulkerCrewLog.CrewStatus != BulkerCrewStatus.Loading)
                    {
                        newStatus = BulkerCrewStatus.Loading;
                    }

                    break;
                case RigJobCrewSectionStatus.Loaded:
                    if (bulkerCrewLog.CrewStatus != BulkerCrewStatus.Loaded)
                    {
                        newStatus = BulkerCrewStatus.Loaded;
                    }

                    break;
                case RigJobCrewSectionStatus.EnRoute:
                    if (bulkerCrewLog.CrewStatus != BulkerCrewStatus.EnRoute)
                    {
                        newStatus = BulkerCrewStatus.EnRoute;
                    }

                    break;
                case RigJobCrewSectionStatus.OnLocation:
                    if (bulkerCrewLog.CrewStatus != BulkerCrewStatus.OnLocation)
                    {
                        newStatus = BulkerCrewStatus.OnLocation;
                    }

                    break;
                case RigJobCrewSectionStatus.OnWayIn:
                    if (bulkerCrewLog.CrewStatus != BulkerCrewStatus.OnWayIn)
                    {
                        newStatus = BulkerCrewStatus.OnWayIn;
                    }

                    break;
                case RigJobCrewSectionStatus.Returned:
                    if (bulkerCrewLog.CrewStatus != BulkerCrewStatus.Returned)
                    {
                        newStatus = BulkerCrewStatus.Returned;
                    }

                    break;
            }
        

            if (newStatus != BulkerCrewStatus.None)
            {
                CrewProcess.SetBulkerCrewLog(crewId, newStatus, isThirdParty, userName);
            }

        }

        public static RigJobSanjelCrewSection GetCurrentBulkerCrewAssignment(int crewId)
        {
            var rigJobSanjelCrewSections = eServiceOnlineGateway.Instance.GetActiveBulkerCrewAssignmentByCrewId(crewId);

            var sortedList1 = rigJobSanjelCrewSections.Where(p=>p.RigJobCrewSectionStatus != RigJobCrewSectionStatus.Returned).OrderBy(assignment=>assignment.ProductHaul.ExpectedOnLocationTime).ToList();
            RigJobSanjelCrewSection currentSanjelCrewSection = sortedList1.FirstOrDefault();
            return currentSanjelCrewSection;
        }
        public static RigJobThirdPartyBulkerCrewSection GetCurrentThirdPartyBulkerCrewAssignment(int crewId)
        {
            var rigJobThirdPartyBulkerCrewSections = eServiceOnlineGateway.Instance.GetActiveThirdPartyBulkerCrewAssignmentByCrewId(crewId);

            RigJobThirdPartyBulkerCrewSection currentSanjelCrewSection = rigJobThirdPartyBulkerCrewSections.Where(p => p.RigJobCrewSectionStatus != RigJobCrewSectionStatus.Returned).OrderBy(assignment => assignment.ProductHaul.ExpectedOnLocationTime).ToList()
                .FirstOrDefault();
            return currentSanjelCrewSection;
        }

        public static void AssignBulkerCrewToProductHaul(RigJob rigJob, ProductHaul productHaul, RigJobCrewSectionStatus rigJobCrewSectionStatus, string userName)
        {
            if (!productHaul.IsThirdParty)
	        {
		        SanjelCrew crew = eServiceOnlineGateway.Instance.GetCrewById(productHaul.Crew.Id, true);
		        productHaul.Crew = crew;
		        ProductHaulProcess.SetDriverAndBulk(productHaul, crew);
		        productHaul.Name = ProductHaulProcess.BuildProductHaulName(productHaul);
		        AssignBulkerCrewToProductHaul(rigJob, productHaul, crew, null, userName, rigJobCrewSectionStatus);

	        }
	        else
	        {
		        ThirdPartyBulkerCrew thirdPartyBulkerCrew = eServiceOnlineGateway.Instance.GetThirdPartyBulkerCrewById(productHaul.Crew.Id);
		        productHaul.Crew = thirdPartyBulkerCrew;
		        ProductHaulProcess.SetThirdPartyDriver(productHaul, thirdPartyBulkerCrew);
		        productHaul.Name = ProductHaulProcess.BuildProductHaulName(productHaul);
		        AssignBulkerCrewToProductHaul(rigJob, productHaul, null, thirdPartyBulkerCrew, userName, rigJobCrewSectionStatus);
	        }
        }

        public static void AssignBulkerCrewToProductHaul(RigJob rigJob, ProductHaul productHaul, SanjelCrew sanjelCrew,
	        ThirdPartyBulkerCrew thirdPartyBulkerCrew, string loggedUser, RigJobCrewSectionStatus rigJobCrewSectionStatus)
        {
	        var startTime = ProductHaulProcess.GetScheduleStartTime(productHaul, rigJob);
	        var endTime = ProductHaulProcess.GetScheduleEndTime(productHaul, rigJob);
	        if (!productHaul.IsThirdParty)
	        {
		        RigBoardProcess.CreateCrewSectionAndSchedules(startTime, endTime, sanjelCrew, rigJob, rigJobCrewSectionStatus, productHaul, loggedUser);
		        CrewProcess.UpdateBulkerCrewStatus(sanjelCrew.Id, false, loggedUser);
	        }
            else
	        {
		        RigBoardProcess.CreateThridPartyCrewScheduleAndSection(startTime, endTime, thirdPartyBulkerCrew, rigJob, rigJobCrewSectionStatus, loggedUser, productHaul, productHaul.IsGoWithCrew);
		        CrewProcess.UpdateBulkerCrewStatus(thirdPartyBulkerCrew.Id, true, loggedUser);
	        }


        }


        public static void CreateOrUpdateThirdPartyUnitSections(ProductHaul productHaul)
        {
            Collection<ThirdPartyUnitSection> thirdPartyUnitSections = eServiceOnlineGateway.Instance.GetThirdPartyUnitSectionsByProductHaul(productHaul);
            Collection<UnitSection> unitSections = eServiceOnlineGateway.Instance.GetUnitSectionsByProductHaul(productHaul);
            //Delete all unit section/third party unit section assigned to this product haul
            foreach (UnitSection unitSection in unitSections)
            {
                eServiceOnlineGateway.Instance.DeleteUnitSection(unitSection);
            }
            foreach (ThirdPartyUnitSection thirdPartyUnitSection in thirdPartyUnitSections)
            {
                eServiceOnlineGateway.Instance.DeleteThirdPartyUnitSection(thirdPartyUnitSection);
            }

            var groupedResult = productHaul.ShippingLoadSheets.FindAll(p => p.CallSheetId != 0)
	            .GroupBy(p => p.CallSheetId)
	            .Select(g => new { CallSheetId = g.Key, Description = String.Join("; ", g.Select(p => p.Description)) });

            if (!productHaul.IsThirdParty)
            {
	            var rigJobSanjelCrewSection = eServiceOnlineGateway.Instance
		            .GetRigJobSanjelCrewSectionsByQuery(p => p.ProductHaul.Id == productHaul.Id).FirstOrDefault();
                //Create unit section/third party unit section again with new assignment

                foreach (var productHaulShippingLoadSheet in groupedResult)
                {
	                CreateBulkerUnitSection(rigJobSanjelCrewSection, productHaulShippingLoadSheet.CallSheetId,
		                    !productHaul.IsGoWithCrew, productHaulShippingLoadSheet.Description, productHaul.Id);
                }
            }
            else
            {
	            foreach (var productHaulShippingLoadSheet in groupedResult)
	            {
		            CreateUnitSectionFromThirdPartyCrew(productHaul.Id, productHaulShippingLoadSheet.CallSheetId, productHaul.Crew.Id, productHaulShippingLoadSheet.Description);
	            }
            }
        }

        private static void CreateUnitSectionFromThirdPartyCrew(int productHaulId, int callsheetId, int crewId, string description)
        {

	        ThirdPartyBulkerCrew thirdPartyBulkerCrew = eServiceOnlineGateway.Instance.GetThirdPartyBulkerCrewById(crewId);

	        ThirdPartyUnitSection thirdPartyUnitSection = new ThirdPartyUnitSection();
	        thirdPartyUnitSection.ProductHaulId = productHaulId;
	        thirdPartyUnitSection.RootId = callsheetId;
	        thirdPartyUnitSection.CrewId = crewId;
	        thirdPartyUnitSection.SupplierCompanyName = thirdPartyBulkerCrew?.ContractorCompany?.Name;
	        thirdPartyUnitSection.ThirdPartyUnitNumber = thirdPartyBulkerCrew?.ThirdPartyUnitNumber;
	        thirdPartyUnitSection.SupplierContactName = thirdPartyBulkerCrew?.SupplierContactName;
	        thirdPartyUnitSection.SupplierContactNumber = thirdPartyBulkerCrew?.SupplierContactNumber;
	        thirdPartyUnitSection.Description = description;

		    eServiceOnlineGateway.Instance.CreateThirdPartyUnitSection(thirdPartyUnitSection);
        }


        public static bool CreatePumperUnitSection(RigJobSanjelCrewSection rigJobSanjelCrewSection, int callsheetId)
        {
	        bool isProductHaul = false;
	        string description = string.Empty;
	        int productHaulId = 0;

            SanjelCrew crew = eServiceOnlineGateway.Instance.GetSanjelCrewById(rigJobSanjelCrewSection.SanjelCrew.Id);
            if (crew?.Type != null && (crew.Type.Name == "Pumper Crew" || crew.Type.Name == "Spare Crew"))
            {

                List<int> pumperIds = new List<int>() { 205, 288, 238, 71, 62, 291, 276 };
                List<int> tractorIds = new List<int>() { 276 };
                List<int> pickupIds = new List<int>() { 93, 94, 95 };

                List<int> unitTypeOrder = new List<int>() { 205, 288, 238, 71, 62, 291, 276, 93, 94, 95 };

                var loadedTruckUnitSections = crew.SanjelCrewTruckUnitSection.Join(CacheData.TruckUnits.ToList(),
                    section => section.TruckUnit.Id, unit => unit.Id, (section, unit) =>
                    {
                        section.TruckUnit = unit;
                        return section;
                    }).OrderBy(p => unitTypeOrder.IndexOf(p.TruckUnit.UnitSubType.Id)).ToList();

                var pumperSection = loadedTruckUnitSections.FindAll(p => pumperIds.Contains(p.TruckUnit.UnitSubType.Id)).FirstOrDefault();
                var tractorSection = loadedTruckUnitSections.FindAll(p => tractorIds.Contains(p.TruckUnit.UnitSubType.Id)).FirstOrDefault();

                var pickupSections = loadedTruckUnitSections.FindAll(p => pickupIds.Contains(p.TruckUnit.UnitSubType.Id));

                var loadedWorkerSections = crew.SanjelCrewWorkerSection.Join(CacheData.Employees.ToList(),
                    section => section.Worker.Id, worker => worker.Id, (section, worker) =>
                    {
                        section.Worker = worker;
                        return section;
                    }).ToList();

                List<int> positionOrder = new List<int>() { 408, 359, 358, 357, 409, 361, 360, 412, 420, 422 };
                List<SanjelCrewWorkerSection> workerSections = loadedWorkerSections.OrderBy(p => positionOrder.IndexOf(p.Worker.BonusPosition.Id)).ToList();


                if (pumperSection != null)
                {
                    if (pickupSections.Any())
                    {
                        if (loadedWorkerSections.Count > 3)
                        {
                            CreateCallSheetUnitSection(callsheetId, pumperSection, tractorSection, loadedWorkerSections[0], loadedWorkerSections[1], crew.Id, isProductHaul, description, productHaulId);

                            int i = 2;
                            bool isContinue = true;
                            while (isContinue)
                            {
                                foreach (var pickupSection in pickupSections)
                                {
                                    SanjelCrewWorkerSection worker1 = loadedWorkerSections[i];
                                    SanjelCrewWorkerSection worker2 = null;
                                    i++;
                                    if (i > loadedWorkerSections.Count)
                                    {
                                        worker2 = loadedWorkerSections[i];
                                        i++;
                                    }
                                    CreateCallSheetUnitSection(callsheetId, pickupSection, null, worker1, worker2, crew.Id, isProductHaul, description, productHaulId);
                                    if (i > loadedWorkerSections.Count)
                                    {
                                        isContinue = false;
                                    }
                                }
                            }
                        }
                        else if (loadedWorkerSections.Count == 3)
                        {
                            CreateCallSheetUnitSection(callsheetId, pumperSection, tractorSection,
                                loadedWorkerSections[0], loadedWorkerSections[1], crew.Id, isProductHaul, description, productHaulId);
                            CreateCallSheetUnitSection(callsheetId, pickupSections[0], null,
                                loadedWorkerSections[2], null, crew.Id, isProductHaul, description, productHaulId);
                        }
                        else if (loadedWorkerSections.Count == 2)
                        {
                            CreateCallSheetUnitSection(callsheetId, pumperSection, tractorSection,
                                loadedWorkerSections[0], null, crew.Id, isProductHaul, description, productHaulId);
                            CreateCallSheetUnitSection(callsheetId, pickupSections[0], null,
                                loadedWorkerSections[1], null, crew.Id, isProductHaul, description, productHaulId);
                        }
                        else
                        {
                            CreateCallSheetUnitSection(callsheetId, pumperSection, tractorSection,
                                loadedWorkerSections[0], null, crew.Id, isProductHaul, description, productHaulId);
                            CreateCallSheetUnitSection(callsheetId, pickupSections[0], null,
                                null, null, crew.Id, isProductHaul, description, productHaulId);
                        }
                    }
                    else
                    {
                        int i = 0;
                        bool isContinue = true;
                        while (isContinue)
                        {
                            SanjelCrewWorkerSection worker1 = loadedWorkerSections[i];
                            SanjelCrewWorkerSection worker2 = null;
                            i++;
                            if (i < loadedWorkerSections.Count)
                            {
                                worker2 = loadedWorkerSections[i];
                                i++;
                            }
                            CreateCallSheetUnitSection(callsheetId, pumperSection, tractorSection, worker1, worker2, crew.Id, isProductHaul, description, productHaulId);
                            if (i >= loadedWorkerSections.Count)
                            {
                                isContinue = false;
                            }
                        }
                    }
                }
                else
                {
                    if (pickupSections.Any())
                    {
                        int i = 0;
                        bool isContinue = true;
                        while (isContinue)
                        {
                            foreach (var pickupSection in pickupSections)
                            {
                                SanjelCrewWorkerSection worker1 = loadedWorkerSections[i];
                                SanjelCrewWorkerSection worker2 = null;
                                i++;
                                if (i > loadedWorkerSections.Count)
                                {
                                    worker2 = loadedWorkerSections[i];
                                    i++;
                                }
                                CreateCallSheetUnitSection(callsheetId, pickupSection, null, worker1, worker2, crew.Id, isProductHaul, description, productHaulId);
                                if (i > loadedWorkerSections.Count)
                                {
                                    isContinue = false;
                                }
                            }
                        }

                    }
                }

                return true;
            }
            return false;
        }

        public static bool CreateBulkerUnitSection(RigJobSanjelCrewSection rigJobSanjelCrewSection, int callsheetId, bool isProductHaul, string description, int productHaulId)
        {
            SanjelCrew crew = eServiceOnlineGateway.Instance.GetSanjelCrewById(rigJobSanjelCrewSection.SanjelCrew.Id);
            if (crew?.Type != null && (crew.Type.Name == "Bulker Crew" || crew.Type.Name == "Spare Crew"))
            {

                List<int> bulkerIds = new List<int>() { 15, 280, 101, 34, 27, 61 };
                List<int> tractorIds = new List<int>() { 276 };
                List<int> pickupIds = new List<int>() { 93, 94, 95 };

                List<int> unitTypeOrder = new List<int>() { 205, 288, 238, 71, 62, 291, 276, 93, 94, 95 };

                var loadedTruckUnitSections = crew.SanjelCrewTruckUnitSection.Join(CacheData.TruckUnits.ToList(),
                    section => section.TruckUnit.Id, unit => unit.Id, (section, unit) =>
                    {
                        section.TruckUnit = unit;
                        return section;
                    }).OrderBy(p => unitTypeOrder.IndexOf(p.TruckUnit.UnitSubType.Id)).ToList();

                var bulkerSection = loadedTruckUnitSections.FindAll(p => bulkerIds.Contains(p.TruckUnit.UnitSubType.Id)).FirstOrDefault();
                var tractorSection = loadedTruckUnitSections.FindAll(p => tractorIds.Contains(p.TruckUnit.UnitSubType.Id)).FirstOrDefault();
                //If bulker crew is only built with tractor. Use tractor as primary unit
                if (bulkerSection == null && tractorSection != null)
                {
	                bulkerSection = tractorSection;
	                tractorSection = null;
                }

                var pickupSections = loadedTruckUnitSections.FindAll(p => pickupIds.Contains(p.TruckUnit.UnitSubType.Id));

                var loadedWorkerSections = crew.SanjelCrewWorkerSection.Join(CacheData.Employees.ToList(),
                    section => section.Worker.Id, worker => worker.Id, (section, worker) =>
                    {
                        section.Worker = worker;
                        return section;
                    }).ToList();


                if (bulkerSection != null)
                {
                    if (pickupSections.Any())
                    {
                        if (loadedWorkerSections.Count > 3)
                        {
                            CreateCallSheetUnitSection(callsheetId, bulkerSection, tractorSection, loadedWorkerSections[0], loadedWorkerSections[1], crew.Id, isProductHaul, description, productHaulId);

                            int i = 2;
                            bool isContinue = true;
                            while (isContinue)
                            {
                                foreach (var pickupSection in pickupSections)
                                {
                                    SanjelCrewWorkerSection worker1 = loadedWorkerSections[i];
                                    SanjelCrewWorkerSection worker2 = null;
                                    i++;
                                    if (i > loadedWorkerSections.Count)
                                    {
                                        worker2 = loadedWorkerSections[i];
                                        i++;
                                    }
                                    CreateCallSheetUnitSection(callsheetId, pickupSection, null, worker1, worker2, crew.Id, isProductHaul, description, productHaulId);
                                    if (i > loadedWorkerSections.Count)
                                    {
                                        isContinue = false;
                                    }
                                }
                            }
                        }
                        else if (loadedWorkerSections.Count == 3)
                        {
                            CreateCallSheetUnitSection(callsheetId, bulkerSection, tractorSection,
                                loadedWorkerSections[0], loadedWorkerSections[1], crew.Id, isProductHaul, description, productHaulId);
                            CreateCallSheetUnitSection(callsheetId, pickupSections[0], null,
                                loadedWorkerSections[2], null, crew.Id, isProductHaul, description, productHaulId);
                        }
                        else if (loadedWorkerSections.Count == 2)
                        {
                            CreateCallSheetUnitSection(callsheetId, bulkerSection, tractorSection,
                                loadedWorkerSections[0], null, crew.Id, isProductHaul, description, productHaulId);
                            CreateCallSheetUnitSection(callsheetId, pickupSections[0], null,
                                loadedWorkerSections[1], null, crew.Id, isProductHaul, description, productHaulId);
                        }
                        else
                        {
                            CreateCallSheetUnitSection(callsheetId, bulkerSection, tractorSection,
                                loadedWorkerSections[0], null, crew.Id, isProductHaul, description, productHaulId);
                            CreateCallSheetUnitSection(callsheetId, pickupSections[0], null,
                                null, null, crew.Id, isProductHaul, description, productHaulId);
                        }
                    }
                    else
                    {
                        int i = 0;
                        bool isContinue = true;
                        while (isContinue)
                        {
                            SanjelCrewWorkerSection worker1 = loadedWorkerSections[i];
                            SanjelCrewWorkerSection worker2 = null;
                            i++;
                            if (i < loadedWorkerSections.Count)
                            {
                                worker2 = loadedWorkerSections[i];
                                i++;
                            }
                            CreateCallSheetUnitSection(callsheetId, bulkerSection, tractorSection, worker1, worker2, crew.Id, isProductHaul, description, productHaulId);
                            if (i >= loadedWorkerSections.Count)
                            {
                                isContinue = false;
                            }
                        }
                    }
                }
                else
                {
                    if (pickupSections.Any())
                    {
                        int i = 0;
                        bool isContinue = true;
                        while (isContinue)
                        {
                            foreach (var pickupSection in pickupSections)
                            {
                                SanjelCrewWorkerSection worker1 = loadedWorkerSections[i];
                                SanjelCrewWorkerSection worker2 = null;
                                i++;
                                if (i > loadedWorkerSections.Count)
                                {
                                    worker2 = loadedWorkerSections[i];
                                    i++;
                                }
                                CreateCallSheetUnitSection(callsheetId, pickupSection, null, worker1, worker2, crew.Id, isProductHaul, description, productHaulId);
                                if (i > loadedWorkerSections.Count)
                                {
                                    isContinue = false;
                                }
                            }
                        }

                    }
                }

                return true;
            }
            return false;
        }

        private static void CreateCallSheetUnitSection(int callsheetId, SanjelCrewTruckUnitSection primaryUnitSection,
	        SanjelCrewTruckUnitSection tractorSection, SanjelCrewWorkerSection workerSection,
	        SanjelCrewWorkerSection workerSection1, int crewId, bool isProductHaul, string description,
	        int productHaulId)
        {
            UnitSection unitSection = new UnitSection();
            unitSection.RootId = callsheetId;
            unitSection.CrewId = crewId;
            if (tractorSection != null)
            {
                unitSection.TractorUnit = new TruckUnit();
                unitSection.TractorUnit.Id = tractorSection.TruckUnit.Id;
                unitSection.TractorUnit.UnitNumber = tractorSection.TruckUnit.UnitNumber;
                unitSection.TractorMainType = new UnitMainType();
                unitSection.TractorMainType.Id = tractorSection.TruckUnit.UnitMainType.Id;
                unitSection.TractorMainType.Name = tractorSection.TruckUnit.UnitMainType.Name;
                if (tractorSection.TruckUnit.UnitSubType != null)
                {
                    unitSection.TractorSubtype = new UnitSubtype();
                    unitSection.TractorSubtype.Id = tractorSection.TruckUnit.UnitSubType.Id;
                    unitSection.TractorSubtype.Name = tractorSection.TruckUnit.UnitSubType.Name;
                }
            }
            unitSection.TruckUnit = new TruckUnit();
            unitSection.TruckUnit.Id = primaryUnitSection.TruckUnit.Id;
            unitSection.TruckUnit.UnitNumber = primaryUnitSection.TruckUnit.UnitNumber;
            unitSection.UnitMainType = new UnitMainType();
            unitSection.UnitMainType.Id = primaryUnitSection.TruckUnit.UnitMainType.Id;
            unitSection.UnitMainType.Name = primaryUnitSection.TruckUnit.UnitMainType.Name;
            if (primaryUnitSection.TruckUnit.UnitSubType != null)
            {
                unitSection.UnitSubtype = new UnitSubtype();
                unitSection.UnitSubtype.Id = primaryUnitSection.TruckUnit.UnitSubType.Id;
                unitSection.UnitSubtype.Name = primaryUnitSection.TruckUnit.UnitSubType.Name;
            }

            if (workerSection != null)
            {
                unitSection.Operator1 = new Employee();
                unitSection.Operator1.Id = workerSection.Worker.Id;
                unitSection.Operator1.LastName = workerSection.Worker.LastName;
                unitSection.Operator1.MiddleName = workerSection.Worker.MiddleName;
                unitSection.Operator1.FirstName = workerSection.Worker.FirstName;
            }

            if (workerSection1 != null)
            {
                unitSection.Operator2 = new Employee();
                unitSection.Operator2.Id = workerSection1.Worker.Id;
                unitSection.Operator2.LastName = workerSection1.Worker.LastName;
                unitSection.Operator2.MiddleName = workerSection1.Worker.MiddleName;
                unitSection.Operator2.FirstName = workerSection1.Worker.FirstName;
            }

            unitSection.IsProductHaul = isProductHaul;
            unitSection.HaulDescription = description;
            unitSection.ProductHaulId = productHaulId;

            eServiceOnlineGateway.Instance.CreateUnitSection(unitSection);


        }

        public static void DeleteUnitSectionByProductHaul(ProductHaul productHaul)
        {
	        Collection<UnitSection> unitSections = eServiceOnlineGateway.Instance.GetUnitSectionsByProductHaul(productHaul);
	        if (unitSections != null && unitSections.Count > 0)
	        {
		        foreach (var unitSection in unitSections)
		        {
			        eServiceOnlineGateway.Instance.DeleteUnitSection(unitSection);
		        }
	        }

        }
    }
}
