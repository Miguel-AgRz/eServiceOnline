using eServiceOnline.Gateway;
using MetaShare.Common.Foundation.EntityBases;
using MetaShare.Common.ServiceModel.Services;
using Sanjel.BusinessEntities;
using Sanjel.BusinessEntities.Sections.Common;
using Sanjel.Common.EService.Sections.Common;
using Sanjel.EService.MicroService.Interfaces;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Crew;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ResourceSchedule;
using Sesi.SanjelData.Entities.BusinessEntities.WellSite;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment;
using Sesi.SanjelData.Entities.Common.BusinessEntities.HumanResources;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Operation;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Organization;
using Sesi.SanjelData.Entities.Common.BusinessEntities.WellSite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using eServiceOnline.BusinessProcess;
using MetaShare.Common.Core.Entities;
using Sanjel.BusinessEntities.CallSheets;
using Sesi.SanjelData.Entities.Common.Model.Scheduling;
using CallSheet = Sanjel.BusinessEntities.CallSheets.CallSheet;
using JobLifeStatus = Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch.JobLifeStatus;
using RigJob = Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch.RigJob;
using ThirdPartyUnitSection = Sanjel.Common.EService.Sections.Common.ThirdPartyUnitSection;
using UnitSection = Sanjel.BusinessEntities.Sections.Common.UnitSection;
using Sanjel.Services.Interfaces;
using sesi.SanjelLibrary.NotficationLibrary;
using BlendSection = Sanjel.BusinessEntities.Sections.Common.BlendSection;

namespace eServiceOnline.BusinessProcess
{
    public class RigBoardProcess 
    {
        #region ChangeRigJobStatus

        public static bool UpdateRigJobStatusToComplete(int id)
        {
            bool succeed = false;
            try
            {
                RigJob rigJob = eServiceOnlineGateway.Instance.GetRigJobById(id);
                if (rigJob != null)
                {
                    rigJob.JobLifeStatus =JobLifeStatus.Completed;
                    rigJob = UpdateRigJob(rigJob);

                if(rigJob != null)
                    succeed = true;
                }
            }
            catch
            {
                succeed = false;
            }

            return succeed;
        }

        public static RigJob UpdateRigJob(RigJob currentRigJob)
        {
            eServiceOnlineGateway.Instance.UpdateRigJob(currentRigJob);
            RigJob rigJob = eServiceOnlineGateway.Instance.GetRigJobById(currentRigJob.Id);
          
            if (!rigJob.JobLifeStatus.Equals(JobLifeStatus.Alerted)) rigJob = RigJobProcess.UpdateListedRigJob(rigJob.Rig.Id, rigJob.Id);

            return rigJob;
        }

        public static int DeleteRigJobAndUpdateListedRigJob(RigJob rigJob)
        {
            int result = eServiceOnlineGateway.Instance.DeleteRigJob(rigJob);
            if (rigJob != null && rigJob.JobLifeStatus != JobLifeStatus.Alerted) RigJobProcess.UpdateListedRigJob(rigJob.Rig.Id, 0);

            return result;
        }

        public static bool UpdateRigJobStatusToCancel(int id,string notes)
        {
            bool succeed = false;
            try
            {
                RigJob rigJob = eServiceOnlineGateway.Instance.GetRigJobById(id);
                if (rigJob != null)
                {
                    if (rigJob.JobLifeStatus == JobLifeStatus.InProgress ||
                        rigJob.JobLifeStatus == JobLifeStatus.Completed) return false;
                    rigJob.JobLifeStatus = JobLifeStatus.Canceled;
                    rigJob.Notes = notes;
                    UpdateRigJob(rigJob);

                    ICallSheetMicroService callSheetMicroService = ServiceFactory.Instance.GetService(typeof(ICallSheetMicroService)) as ICallSheetMicroService;
                    if (callSheetMicroService == null) throw new Exception("ICallSheetMicroService can not be null.");
                    CallSheet originalCallSheet = callSheetMicroService.GetCallSheetByCallSheetNumber(rigJob.CallSheetNumber);
                    if (originalCallSheet != null)
                    {
                        CallSheet currentCallSheet = originalCallSheet;
                        currentCallSheet.Status = EServiceEntityStatus.Canceled;
                        callSheetMicroService.UpdateCallSheet(currentCallSheet, originalCallSheet);
                    }

                    succeed = true;
                }
            }
            catch
            {
                succeed = false;
            }

            return succeed;
        }

        #endregion ChangeRigJobStatusToComplete

        #region CreateOrUpdateUnitSectionByProductHaul

        /*
        public static void CreateOrUpdateUnitSectionsByProductHaul(ProductHaul productHaul,int crewId)
        {
            Collection<ThirdPartyUnitSection> thirdPartyUnitSections = eServiceOnlineGateway.Instance.GetThirdPartyUnitSectionsByProductHaul(productHaul);
            Collection<UnitSection> unitSections = eServiceOnlineGateway.Instance.GetUnitSectionsByProductHaul(productHaul);
            var callsheetId = 0;
            if (thirdPartyUnitSections.Count > 0 && unitSections.Count < 1)
            {
                foreach (ThirdPartyUnitSection thirdPartyUnitSection in thirdPartyUnitSections)
                {
                    CreateUnitSection(crewId, callsheetId, productHaul.IsThirdParty, productHaul.IsGoWithCrew, productHaul.Id);
                    eServiceOnlineGateway.Instance.DeleteThirdPartyUnitSection(thirdPartyUnitSection);
                }              
            }
            else
            {
                foreach (UnitSection unitSection in unitSections)
                {
                    SetUnitSectionInfo(productHaul, unitSection);
                    eServiceOnlineGateway.Instance.UpdateUnitSection(unitSection);
                }
            }

        }
        */

        /*private static void SetUnitSectionInfo(ProductHaul productHaul, UnitSection unitSection)
        {
            if (productHaul.BulkUnit != null && productHaul.BulkUnit.Id != 0)
            {
                TruckUnit truckUnit = eServiceOnlineGateway.Instance.GetTruckUnitById(productHaul.BulkUnit.Id);
                if (truckUnit != null) unitSection.TruckUnit =new Sanjel.Common.BusinessEntities.Lookup.TruckUnit()
                {
                    Id = truckUnit.Id,
                    UnitNumber = truckUnit.UnitNumber
                };
            }

            if (productHaul.TractorUnit != null && productHaul.TractorUnit.Id != 0)
            {
                TruckUnit tractorUnit = eServiceOnlineGateway.Instance.GetTruckUnitById(productHaul.TractorUnit.Id);
                if (tractorUnit != null) unitSection.TractorUnit = new Sanjel.Common.BusinessEntities.Lookup.TruckUnit()
                {
                    Id = tractorUnit.Id,
                    UnitNumber = tractorUnit.UnitNumber
                };
            }
            else
            {
                unitSection.TractorUnit = new Sanjel.Common.BusinessEntities.Lookup.TruckUnit();
            }

            if (productHaul.Driver != null && productHaul.Driver.Id != 0)
            {
                Employee employee = eServiceOnlineGateway.Instance.GetEmployeeById(productHaul.Driver.Id);
                unitSection.Operator1 = new Sanjel.Common.BusinessEntities.Reference.Employee()
                {
                    Id = employee.Id,
                    MiddleName = employee.MiddleName,
                    LastName = employee.LastName,
                    FirstName = employee.FirstName
                };
            }

            if (productHaul.Driver2 != null && productHaul.Driver2.Id != 0)
            {
                Employee employee = eServiceOnlineGateway.Instance.GetEmployeeById(productHaul.Driver2.Id);
                unitSection.Operator2 = new Sanjel.Common.BusinessEntities.Reference.Employee()
                {
                    Id = employee.Id,
                    MiddleName = employee.MiddleName,
                    LastName = employee.LastName,
                    FirstName = employee.FirstName
                };
            }
            else
            {
                unitSection.Operator2 = new Sanjel.Common.BusinessEntities.Reference.Employee();
            }

            unitSection.HaulDescription = BuildUnitSectionComments(productHaul);
            unitSection.IsProductHaul = !productHaul.IsGoWithCrew;
        }*/
        /*
        private static string UpdateCrewScheduleBuildJobInfo(ProductHaul productHaul)
        {
            string comments = string.Empty;
            if(productHaul.ProductHaulLoads==null)
            {
                return comments;
            }
            List<ProductHaulLoad> productHaulLoads = productHaul.ProductHaulLoads.OrderBy(p => p.ExpectedOnLocationTime).ToList();
            foreach (var productHaulLoad in productHaulLoads)
            {
                RigJob rigJob = eServiceOnlineGateway.Instance.GetRigJobByCallSheetNumber(productHaulLoad.CallSheetNumber);
                if (productHaulLoads[0].IsGoWithCrew)
                {
                    comments = comments + BuildRigJobInfo(rigJob);
                }
                else
                {
                    comments = comments + BuildProductHaulInfo(productHaulLoad, rigJob);
                }
            }
            if (comments.Length > 0)
            {
                comments = comments?.Substring(0, comments.Length - 1);
            }
            return comments;
        }
        */


/*        private static string BuildJobInfo(ProductHaulLoad productHaulLoad,RigJob rigJob,string jobInfo)
        {
            if (!string.IsNullOrEmpty(jobInfo))
            {
                jobInfo = jobInfo + ";";
            }
            if (productHaulLoad != null)
            {
                if (productHaulLoad.IsGoWithCrew)
                {
                    jobInfo = jobInfo + BuildRigJobInfo(rigJob);
                }
                else
                {
                    double enteredBlendWeight = productHaulLoad.IsTotalBlendTonnage ? productHaulLoad.TotalBlendWeight : productHaulLoad.BaseBlendWeight;
                    jobInfo = jobInfo + $"Haul {enteredBlendWeight / 1000} t {productHaulLoad.BlendChemical?.Name} to {rigJob?.Rig?.Name};";
                }
            }
            else
            {
                jobInfo = jobInfo + BuildRigJobInfo(rigJob);
            }

            jobInfo = jobInfo?.Substring(0, jobInfo.Length - 1);                       
            return jobInfo;
        }*/

        public static string BuildRigJobInfo(RigJob rigJob)
        {
             return $"{rigJob?.Rig?.Name}/{rigJob?.JobType?.Name}" + ";";
        }
        public static string BuildProductHaulInfo(ProductHaulLoad productHaulLoad, RigJob rigJob)
        {
            double enteredBlendWeight = productHaulLoad.IsTotalBlendTonnage
                ? productHaulLoad.TotalBlendWeight
                : productHaulLoad.BaseBlendWeight;
            return $"Haul {enteredBlendWeight / 1000} t {productHaulLoad.BlendChemical?.Name} to {rigJob?.Rig?.Name};";
        }
        public static string BuildProductHaulInfo(ProductHaulLoad productHaulLoad)
        {
            double enteredBlendWeight = productHaulLoad.IsTotalBlendTonnage
                ? productHaulLoad.TotalBlendWeight
                : productHaulLoad.BaseBlendWeight;
            var rtn = "";
            return  ($"Haul {enteredBlendWeight / 1000} t {productHaulLoad.BlendChemical?.Name} to {productHaulLoad?.Rig?.Name}") + (productHaulLoad.IsGoWithCrew?" Go with crew.":$" Bin {productHaulLoad.Bin.Name} Pod {productHaulLoad.PodIndex}");
        }

        /*
        public static string BuildUnitSectionComments(ProductHaul productHaul)
        {
            return ProductHaulProcess.BuildProductHaulDescription(productHaul);
        }
        */


        #endregion CreateOrUpdateUnitSectionByProductHaul

        #region Update RigStatus

        public static bool UpdateRigStatus(int rigId, RigStatus newStatus)
        {
            //Get rig by id,
            //Get current listed rig job by rig id
            //remove old rig listed flag and update
            //Create new rig job with new status and set listed flag, copy over service point and set time as now
            //move bins assigned to the rig along to new rig job
            //update rig to new status

            bool succeed = false;
            Rig rig = eServiceOnlineGateway.Instance.GetRigById(rigId);
            if (rig != null && rig.Status != newStatus)
            {
                rig.Status = newStatus;
                rig.ModifiedDateTime = DateTime.Now;
                rig.ModifiedUserName = "RigBoard";
                rig.ModifiedUserId = 0;
                rig.OperationType = MetaShare.Common.Core.Entities.DataOperationType.Update;

                eServiceOnlineGateway.Instance.UpdateRig(rig);
                Rig updatedRig = eServiceOnlineGateway.Instance.GetRigById(rig.Id);
                if (updatedRig != null && updatedRig.Status == newStatus)
                {
                    succeed = true;
                }

                List<RigJob> rigJobs = eServiceOnlineGateway.Instance.GetListedRigJobByRigId(rigId).Where(p => p.JobLifeStatus!=JobLifeStatus.Alerted).ToList();
                RigJob currentListedJob = rigJobs.OrderByDescending(p => p.JobDateTime).FirstOrDefault();
                if (currentListedJob != null)
                {
                    if (currentListedJob.JobLifeStatus != JobLifeStatus.Completed &&
                        currentListedJob.JobLifeStatus != JobLifeStatus.Canceled && currentListedJob.JobLifeStatus != JobLifeStatus.None && currentListedJob.JobLifeStatus != JobLifeStatus.Deleted)
                    {
                        //update current listed job
                        currentListedJob.RigStatus = newStatus;
                        UpdateRigJob(currentListedJob);
                    }
                    else
                    {
                        RigJob updatedRigJob = currentListedJob.DeepClone() as RigJob;
                        if (updatedRigJob == null) throw new Exception("Internal error");

                        RigJob newRigJob = new RigJob
                        {
                            Rig = rig,
                            RigStatus = newStatus
                        };

                        if (newStatus != RigStatus.Deactivated)
                        {
                            newRigJob.ClientCompany = updatedRigJob.ClientCompany;
                            newRigJob.ClientCompanyShortName = updatedRigJob.ClientCompany.ShortName;
                            newRigJob.WellLocationType = updatedRigJob.WellLocationType;
                            newRigJob.WellLocation = updatedRigJob.WellLocation;
                            newRigJob.SurfaceLocation = updatedRigJob.SurfaceLocation;
                            newRigJob.JobType = updatedRigJob.JobType;
                            newRigJob.Notes = updatedRigJob.Notes;
                            newRigJob.ServicePoint = updatedRigJob.ServicePoint;
                            newRigJob.JobDateTime = updatedRigJob.RigStatus != RigStatus.Deactivated
                                ? updatedRigJob.JobDateTime
                                : DateTime.Now;
                            newRigJob.JobLifeStatus = JobLifeStatus.None;
                            newRigJob.IsNeedBins = updatedRigJob.IsNeedBins;
                            newRigJob.ClientConsultant1 = updatedRigJob.ClientConsultant1;
                            newRigJob.ClientConsultant2 = updatedRigJob.ClientConsultant2;
                        }
                        else
                        {
                            newRigJob.JobDateTime = DateTime.Now;
                            newRigJob.IsNeedBins = false;
                        }

                        CreateRigJob(newRigJob);
                    }
                }

            }
            return succeed;
        }

        public static RigJob CreateRigJob(RigJob rigJob)
        {
            eServiceOnlineGateway.Instance.CreateRigJob(rigJob);
            RigJob createRigJob = eServiceOnlineGateway.Instance.GetRigJobById(rigJob.Id);
            createRigJob.Rig = eServiceOnlineGateway.Instance.GetRigById(createRigJob.Rig?.Id ?? 0);
//            createRigJob.JobLifeStatus = eServiceOnlineGateway.Instance.GetJobLifeStatusById(createRigJob.JobLifeStatus?.Id ?? 0);
            if (createRigJob.JobLifeStatus != JobLifeStatus.Alerted) createRigJob = RigJobProcess.UpdateListedRigJob(createRigJob.Rig.Id, createRigJob.Id);

            return createRigJob;
        }



        #endregion

        #region GetAllRigJobInformation
        public static List<RigJob> GetAllRigJobInformation(int pageNumber, int pageSize,
            Collection<int> servicePointIds, Collection<int> rigTypes, Collection<int> jobLifeStatuses,
            Collection<int> bulkPlantRigJobIds, bool isShowJobAlert, bool isShowFutureJobs, out int count)
        {
            /*

            //Following seems added by mistake
            IBlendSectionMicroService blendSectionMicroService = ServiceFactory.Instance.GetService(typeof(IBlendSectionMicroService)) as IBlendSectionMicroService;
            if (blendSectionMicroService == null) throw new Exception("IBlendSectionMicroService must be registered in service factory.");
            */
            List<RigJob> rigJobCollection = GetRigJobCollectionByRequestedConditionAndPaginated(pageNumber > 0 ? pageNumber : 1, pageSize, servicePointIds, rigTypes, jobLifeStatuses, bulkPlantRigJobIds,isShowJobAlert, isShowFutureJobs, out count);

            return rigJobCollection;
        }

        private static List<RigJob> GetRigJobCollectionByRequestedConditionAndPaginated(int pageNumber, int pageSize,
            Collection<int> servicePointIds, Collection<int> rigTypes, Collection<int> jobLifeStatuses,
            Collection<int> bulkPlantRigJobIds, bool isShowJobAlert, bool isShowFutureJobs, out int count)
        {
//            Pager pager = new Pager(){PageIndex = pageNumber, PageSize = 18, PageTotal = 10, TotalCounts = 180, OrderBy = "RigStatus, ClientCompanyShortName"};
            Pager pager = null;

            List<RigJob> jobAlerts = new List<RigJob>();

            List<RigJob> allRigJobs = null;


            if (!isShowFutureJobs)
            {
                if (servicePointIds.Count > 0)
                {
                    if (jobLifeStatuses.Count > 0)
                    {
                        if (rigTypes[0].Equals(1))
                        {
                            if (rigTypes[1].Equals(1))//is Service Rig
                            {
                                allRigJobs = eServiceOnlineGateway.Instance.GetRigJobsByQuery(pager,
                                    rigJob => rigJob.RigStatus != RigStatus.Deactivated &&  rigJob.IsListed == true &&
                                              jobLifeStatuses.Contains((int) rigJob.JobLifeStatus) &&
                                              rigJob.IsProjectRig == true && rigJob.IsServiceRig == true &&
                                              servicePointIds.Contains(rigJob.ServicePoint.Id));
                            }
                            else if (rigTypes[1].Equals(2))//is drilling Rig
                            {
	                            allRigJobs = eServiceOnlineGateway.Instance.GetRigJobsByQuery(pager,
		                            rigJob => rigJob.RigStatus != RigStatus.Deactivated && rigJob.IsListed == true &&
		                                      jobLifeStatuses.Contains((int)rigJob.JobLifeStatus) &&
		                                      rigJob.IsProjectRig == true && rigJob.IsServiceRig == false &&
		                                      servicePointIds.Contains(rigJob.ServicePoint.Id));
                            }
                            else 
                            {
                                allRigJobs = eServiceOnlineGateway.Instance.GetRigJobsByQuery(pager,
                                    rigJob => rigJob.RigStatus != RigStatus.Deactivated &&  rigJob.IsListed == true &&
                                              jobLifeStatuses.Contains((int) rigJob.JobLifeStatus) &&
                                              rigJob.IsProjectRig == true && servicePointIds.Contains(rigJob.ServicePoint.Id));
                            }

                        }
                        else
                        {
                            if (rigTypes[1].Equals(1))
                            {
                                allRigJobs = eServiceOnlineGateway.Instance.GetRigJobsByQuery(pager,
                                    rigJob => rigJob.RigStatus != RigStatus.Deactivated &&  rigJob.IsListed == true &&
                                              jobLifeStatuses.Contains((int) rigJob.JobLifeStatus) &&
                                              rigJob.IsServiceRig == true && servicePointIds.Contains(rigJob.ServicePoint.Id));
                            }
                            else if (rigTypes[1].Equals(2))
                            {
	                            allRigJobs = eServiceOnlineGateway.Instance.GetRigJobsByQuery(pager,
		                            rigJob => rigJob.RigStatus != RigStatus.Deactivated && rigJob.IsListed == true &&
		                                      jobLifeStatuses.Contains((int)rigJob.JobLifeStatus) &&
		                                      rigJob.IsServiceRig == false && servicePointIds.Contains(rigJob.ServicePoint.Id));
                            }
                            else
                            {
                                allRigJobs = eServiceOnlineGateway.Instance.GetRigJobsByQuery(pager,
                                    rigJob => rigJob.RigStatus != RigStatus.Deactivated &&  rigJob.IsListed == true &&
                                              jobLifeStatuses.Contains((int) rigJob.JobLifeStatus) && servicePointIds.Contains(rigJob.ServicePoint.Id));
                            }
                        }
                    }
                    else
                    {
                        if (rigTypes[0].Equals(1))
                        {
                            if (rigTypes[1].Equals(1))
                            {
                                allRigJobs = eServiceOnlineGateway.Instance.GetRigJobsByQuery(pager,
                                    rigJob => rigJob.RigStatus != RigStatus.Deactivated &&  rigJob.IsListed == true  &&
                                              rigJob.IsProjectRig == true && rigJob.IsServiceRig == true &&
                                              servicePointIds.Contains(rigJob.ServicePoint.Id));
                            }
                            else if (rigTypes[1].Equals(2))
                            {
	                            allRigJobs = eServiceOnlineGateway.Instance.GetRigJobsByQuery(pager,
		                            rigJob => rigJob.RigStatus != RigStatus.Deactivated && rigJob.IsListed == true &&
		                                      rigJob.IsProjectRig == true && rigJob.IsServiceRig == false &&
		                                      servicePointIds.Contains(rigJob.ServicePoint.Id));
                            }
                            else
                            {
                                allRigJobs = eServiceOnlineGateway.Instance.GetRigJobsByQuery(pager,
                                    rigJob => rigJob.RigStatus != RigStatus.Deactivated &&  rigJob.IsListed == true  &&
                                              rigJob.IsProjectRig == true && servicePointIds.Contains(rigJob.ServicePoint.Id));
                            }

                        }
                        else
                        {
                            if (rigTypes[1].Equals(1))
                            {
                                allRigJobs = eServiceOnlineGateway.Instance.GetRigJobsByQuery(pager,
                                    rigJob => rigJob.RigStatus != RigStatus.Deactivated &&  rigJob.IsListed == true && rigJob.IsServiceRig == true &&
                                              servicePointIds.Contains(rigJob.ServicePoint.Id));
                            }
                            else if (rigTypes[1].Equals(2))
                            {
	                            allRigJobs = eServiceOnlineGateway.Instance.GetRigJobsByQuery(pager,
		                            rigJob => rigJob.RigStatus != RigStatus.Deactivated && rigJob.IsListed == false && rigJob.IsServiceRig == false &&
		                                      servicePointIds.Contains(rigJob.ServicePoint.Id));
                            }
                            else
                            {
                                allRigJobs = eServiceOnlineGateway.Instance.GetRigJobsByQuery(pager,
                                    rigJob => rigJob.RigStatus != RigStatus.Deactivated &&  rigJob.IsListed == true && servicePointIds.Contains(rigJob.ServicePoint.Id));
                            }
                        }
                    }
                }
                else
                {
                    if (jobLifeStatuses.Count > 0)
                    {
                        if (rigTypes[0].Equals(1))
                        {
                            if (rigTypes[1].Equals(1))
                            {
                                allRigJobs = eServiceOnlineGateway.Instance.GetRigJobsByQuery(pager,
                                    rigJob => rigJob.RigStatus != RigStatus.Deactivated &&  rigJob.IsListed == true &&
                                              jobLifeStatuses.Contains((int) rigJob.JobLifeStatus) &&
                                              rigJob.IsProjectRig == true && rigJob.IsServiceRig == true);
                            }
                            else if (rigTypes[1].Equals(2))
                            {
	                            allRigJobs = eServiceOnlineGateway.Instance.GetRigJobsByQuery(pager,
		                            rigJob => rigJob.RigStatus != RigStatus.Deactivated && rigJob.IsListed == true &&
		                                      jobLifeStatuses.Contains((int)rigJob.JobLifeStatus) &&
		                                      rigJob.IsProjectRig == true && rigJob.IsServiceRig == false);
                            }
                            else
                            {
                                allRigJobs = eServiceOnlineGateway.Instance.GetRigJobsByQuery(pager,
                                    rigJob => rigJob.RigStatus != RigStatus.Deactivated &&  rigJob.IsListed == true &&
                                              jobLifeStatuses.Contains((int) rigJob.JobLifeStatus) &&
                                              rigJob.IsProjectRig == true);
                            }

                        }
                        else
                        {
                            if (rigTypes[1].Equals(1))
                            {
                                allRigJobs = eServiceOnlineGateway.Instance.GetRigJobsByQuery(pager,
                                    rigJob => rigJob.RigStatus != RigStatus.Deactivated &&  rigJob.IsListed == true &&
                                              jobLifeStatuses.Contains((int) rigJob.JobLifeStatus) &&
                                              rigJob.IsServiceRig == true);
                            }
                            else if (rigTypes[1].Equals(2))
                            {
	                            allRigJobs = eServiceOnlineGateway.Instance.GetRigJobsByQuery(pager,
		                            rigJob => rigJob.RigStatus != RigStatus.Deactivated && rigJob.IsListed == true &&
		                                      jobLifeStatuses.Contains((int)rigJob.JobLifeStatus) &&
		                                      rigJob.IsServiceRig == false);
                            }
                            else
                            {
                                allRigJobs = eServiceOnlineGateway.Instance.GetRigJobsByQuery(pager,
                                    rigJob => rigJob.RigStatus != RigStatus.Deactivated &&  rigJob.IsListed == true && jobLifeStatuses.Contains((int) rigJob.JobLifeStatus));
                            }
                        }
                    }
                    else
                    {
                        if (rigTypes[0].Equals(1))
                        {
                            if (rigTypes[1].Equals(1))
                            {
                                allRigJobs = eServiceOnlineGateway.Instance.GetRigJobsByQuery(pager,
                                    rigJob => rigJob.RigStatus != RigStatus.Deactivated &&  rigJob.IsListed == true  &&
                                              rigJob.IsProjectRig == true && rigJob.IsServiceRig == true);
                            }
                            else if (rigTypes[1].Equals(2))
                            {
	                            allRigJobs = eServiceOnlineGateway.Instance.GetRigJobsByQuery(pager,
		                            rigJob => rigJob.RigStatus != RigStatus.Deactivated && rigJob.IsListed == true &&
		                                      rigJob.IsProjectRig == true && rigJob.IsServiceRig == false);
                            }
                            else
                            {
                                allRigJobs = eServiceOnlineGateway.Instance.GetRigJobsByQuery(pager,
                                    rigJob => rigJob.RigStatus != RigStatus.Deactivated &&  rigJob.IsListed == true  && rigJob.IsProjectRig == true);
                            }

                        }
                        else
                        {
                            if (rigTypes[1].Equals(1))
                            {
                                allRigJobs = eServiceOnlineGateway.Instance.GetRigJobsByQuery(pager,
                                    rigJob => rigJob.RigStatus != RigStatus.Deactivated &&  rigJob.IsListed == true && rigJob.IsServiceRig == true);
                            }
                            else if (rigTypes[1].Equals(2))
                            {
	                            allRigJobs = eServiceOnlineGateway.Instance.GetRigJobsByQuery(pager,
		                            rigJob => rigJob.RigStatus != RigStatus.Deactivated && rigJob.IsListed == true && rigJob.IsServiceRig == false);
                            }
                            else
                            {
                                allRigJobs = eServiceOnlineGateway.Instance.GetRigJobsByQuery(pager,
                                    rigJob => rigJob.RigStatus != RigStatus.Deactivated &&  rigJob.IsListed == true);
                            }
                        }
                    }
                }
            }
            else
            {
                var currentTime = DateTime.Now.Date;
                if (servicePointIds.Count > 0)
                {
                    if (jobLifeStatuses.Count > 0)
                    {
                        if (rigTypes[0].Equals(1))
                        {
                            if (rigTypes[1].Equals(1))
                            {
                                allRigJobs = eServiceOnlineGateway.Instance.GetRigJobsByQuery(pager,
                                    rigJob => rigJob.RigStatus != RigStatus.Deactivated &&  jobLifeStatuses.Contains((int) rigJob.JobLifeStatus) &&
                                              rigJob.IsProjectRig == true && rigJob.IsServiceRig == true &&
                                              servicePointIds.Contains(rigJob.ServicePoint.Id) && (rigJob.JobDateTime > currentTime || rigJob.IsListed==true));
                            }
                            else if (rigTypes[1].Equals(2))
                            {
	                            allRigJobs = eServiceOnlineGateway.Instance.GetRigJobsByQuery(pager,
		                            rigJob => rigJob.RigStatus != RigStatus.Deactivated && jobLifeStatuses.Contains((int)rigJob.JobLifeStatus) &&
		                                      rigJob.IsProjectRig == true && rigJob.IsServiceRig == false &&
		                                      servicePointIds.Contains(rigJob.ServicePoint.Id) && (rigJob.JobDateTime > currentTime || rigJob.IsListed == true));
                            }
                            else
                            {
                                allRigJobs = eServiceOnlineGateway.Instance.GetRigJobsByQuery(pager,
                                    rigJob => rigJob.RigStatus != RigStatus.Deactivated &&  jobLifeStatuses.Contains((int) rigJob.JobLifeStatus) &&
                                              rigJob.IsProjectRig == true && servicePointIds.Contains(rigJob.ServicePoint.Id) && (rigJob.JobDateTime > currentTime || rigJob.IsListed==true));
                            }

                        }
                        else
                        {
                            if (rigTypes[1].Equals(1))
                            {
                                allRigJobs = eServiceOnlineGateway.Instance.GetRigJobsByQuery(pager,
                                    rigJob => rigJob.RigStatus != RigStatus.Deactivated &&  jobLifeStatuses.Contains((int) rigJob.JobLifeStatus) &&
                                              rigJob.IsServiceRig == true && servicePointIds.Contains(rigJob.ServicePoint.Id) && (rigJob.JobDateTime > currentTime || rigJob.IsListed==true));
                            }
                            else if (rigTypes[1].Equals(2))
                            {
	                            allRigJobs = eServiceOnlineGateway.Instance.GetRigJobsByQuery(pager,
		                            rigJob => rigJob.RigStatus != RigStatus.Deactivated && jobLifeStatuses.Contains((int)rigJob.JobLifeStatus) &&
		                                      rigJob.IsServiceRig == false && servicePointIds.Contains(rigJob.ServicePoint.Id) && (rigJob.JobDateTime > currentTime || rigJob.IsListed == true));
                            }
                            else
                            {
                                allRigJobs = eServiceOnlineGateway.Instance.GetRigJobsByQuery(pager,
                                    rigJob => rigJob.RigStatus != RigStatus.Deactivated &&  jobLifeStatuses.Contains((int) rigJob.JobLifeStatus) && servicePointIds.Contains(rigJob.ServicePoint.Id) && (rigJob.JobDateTime > currentTime || rigJob.IsListed==true));
                            }
                        }
                    }
                    else
                    {
                        if (rigTypes[0].Equals(1))
                        {
                            if (rigTypes[1].Equals(1))
                            {
                                allRigJobs = eServiceOnlineGateway.Instance.GetRigJobsByQuery(pager,
                                    rigJob => rigJob.RigStatus != RigStatus.Deactivated &&  rigJob.IsProjectRig == true && rigJob.IsServiceRig == true &&
                                              servicePointIds.Contains(rigJob.ServicePoint.Id) && (rigJob.JobDateTime > currentTime || rigJob.IsListed==true));
                            }
                            else if (rigTypes[1].Equals(2))
                            {
	                            allRigJobs = eServiceOnlineGateway.Instance.GetRigJobsByQuery(pager,
		                            rigJob => rigJob.RigStatus != RigStatus.Deactivated && rigJob.IsProjectRig == true && rigJob.IsServiceRig == false &&
		                                      servicePointIds.Contains(rigJob.ServicePoint.Id) && (rigJob.JobDateTime > currentTime || rigJob.IsListed == true));
                            }
                            else
                            {
                                allRigJobs = eServiceOnlineGateway.Instance.GetRigJobsByQuery(pager,
                                    rigJob => rigJob.RigStatus != RigStatus.Deactivated &&  rigJob.IsProjectRig == true && servicePointIds.Contains(rigJob.ServicePoint.Id) && (rigJob.JobDateTime > currentTime || rigJob.IsListed==true));
                            }

                        }
                        else
                        {
                            if (rigTypes[1].Equals(1))
                            {
                                allRigJobs = eServiceOnlineGateway.Instance.GetRigJobsByQuery(pager,
                                    rigJob => rigJob.RigStatus != RigStatus.Deactivated &&   rigJob.IsServiceRig == true &&
                                              servicePointIds.Contains(rigJob.ServicePoint.Id) && (rigJob.JobDateTime > currentTime || rigJob.IsListed==true));
                            }
                            else if (rigTypes[1].Equals(2))
                            {
	                            allRigJobs = eServiceOnlineGateway.Instance.GetRigJobsByQuery(pager,
		                            rigJob => rigJob.RigStatus != RigStatus.Deactivated && rigJob.IsServiceRig == false &&
		                                      servicePointIds.Contains(rigJob.ServicePoint.Id) && (rigJob.JobDateTime > currentTime || rigJob.IsListed == true));
                            }
                            else
                            {
                                allRigJobs = eServiceOnlineGateway.Instance.GetRigJobsByQuery(pager,
                                    rigJob => (rigJob.JobDateTime > currentTime || rigJob.IsListed==true) && rigJob.RigStatus != RigStatus.Deactivated &&  servicePointIds.Contains(rigJob.ServicePoint.Id));
                            }
                        }
                    }
                }
                else
                {

                    if (jobLifeStatuses.Count > 0)
                    {
                        if (rigTypes[0].Equals(1))
                        {
                            if (rigTypes[1].Equals(1))
                            {
                                allRigJobs = eServiceOnlineGateway.Instance.GetRigJobsByQuery(pager,
                                    rigJob => rigJob.RigStatus != RigStatus.Deactivated &&  jobLifeStatuses.Contains((int) rigJob.JobLifeStatus) &&
                                              rigJob.IsProjectRig == true && rigJob.IsServiceRig == true && rigJob.JobDateTime > currentTime);
                            }
                            else if (rigTypes[1].Equals(2))
                            {
	                            allRigJobs = eServiceOnlineGateway.Instance.GetRigJobsByQuery(pager,
		                            rigJob => rigJob.RigStatus != RigStatus.Deactivated && jobLifeStatuses.Contains((int)rigJob.JobLifeStatus) &&
		                                      rigJob.IsProjectRig == true && rigJob.IsServiceRig == false && rigJob.JobDateTime > currentTime);
                            }
                            else
                            {
                                allRigJobs = eServiceOnlineGateway.Instance.GetRigJobsByQuery(pager,
                                    rigJob => rigJob.RigStatus != RigStatus.Deactivated &&  jobLifeStatuses.Contains((int) rigJob.JobLifeStatus) &&
                                              rigJob.IsProjectRig == true && (rigJob.JobDateTime > currentTime || rigJob.IsListed==true));
                            }

                        }
                        else
                        {
                            if (rigTypes[1].Equals(1))
                            {
                                allRigJobs = eServiceOnlineGateway.Instance.GetRigJobsByQuery(pager,
                                    rigJob => rigJob.RigStatus != RigStatus.Deactivated &&  jobLifeStatuses.Contains((int) rigJob.JobLifeStatus) &&
                                              rigJob.IsServiceRig == true && (rigJob.JobDateTime > currentTime || rigJob.IsListed==true));
                            }
                            else if (rigTypes[1].Equals(2))
                            {
	                            allRigJobs = eServiceOnlineGateway.Instance.GetRigJobsByQuery(pager,
		                            rigJob => rigJob.RigStatus != RigStatus.Deactivated && jobLifeStatuses.Contains((int)rigJob.JobLifeStatus) &&
		                                      rigJob.IsServiceRig == false && (rigJob.JobDateTime > currentTime || rigJob.IsListed == true));
                            }
                            else
                            {
                                allRigJobs = eServiceOnlineGateway.Instance.GetRigJobsByQuery(pager,
                                    rigJob => rigJob.RigStatus != RigStatus.Deactivated &&  jobLifeStatuses.Contains((int) rigJob.JobLifeStatus) && (rigJob.JobDateTime > currentTime || rigJob.IsListed==true));
                            }
                        }
                    }
                    else
                    {
                        if (rigTypes[0].Equals(1))
                        {
                            if (rigTypes[1].Equals(1))
                            {
                                allRigJobs = eServiceOnlineGateway.Instance.GetRigJobsByQuery(pager,
                                    rigJob => rigJob.RigStatus != RigStatus.Deactivated &&  rigJob.IsProjectRig == true && rigJob.IsServiceRig == true && (rigJob.JobDateTime > currentTime || rigJob.IsListed==true));
                            }
                            else if (rigTypes[1].Equals(2))
                            {
	                            allRigJobs = eServiceOnlineGateway.Instance.GetRigJobsByQuery(pager,
		                            rigJob => rigJob.RigStatus != RigStatus.Deactivated && rigJob.IsProjectRig == true && rigJob.IsServiceRig == false && (rigJob.JobDateTime > currentTime || rigJob.IsListed == true));
                            }
                            else
                            {
                                allRigJobs = eServiceOnlineGateway.Instance.GetRigJobsByQuery(pager,
                                    rigJob => rigJob.RigStatus != RigStatus.Deactivated &&  rigJob.IsProjectRig == true && (rigJob.JobDateTime > currentTime || rigJob.IsListed==true));
                            }

                        }
                        else
                        {
                            if (rigTypes[1].Equals(1))
                            {
                                allRigJobs = eServiceOnlineGateway.Instance.GetRigJobsByQuery(pager,
                                    rigJob => rigJob.RigStatus != RigStatus.Deactivated &&  rigJob.IsServiceRig == true && (rigJob.JobDateTime > currentTime || rigJob.IsListed==true));
                            }
                            else if (rigTypes[1].Equals(2))
                            {
	                            allRigJobs = eServiceOnlineGateway.Instance.GetRigJobsByQuery(pager,
		                            rigJob => rigJob.RigStatus != RigStatus.Deactivated && rigJob.IsServiceRig == false && (rigJob.JobDateTime > currentTime || rigJob.IsListed == true));
                            }
                            else
                            {
                                allRigJobs = eServiceOnlineGateway.Instance.GetRigJobsByQuery(pager, rigJob=>(rigJob.JobDateTime > currentTime || rigJob.IsListed==true));
                            }
                        }
                    }
                }

            }

            //count = allRigJobs.Count;

            List<RigJob> rigJobs = allRigJobs.Where(p => p.JobLifeStatus != JobLifeStatus.Alerted).Where(p=>p.RigStatus != RigStatus.Deactivated).Where(p => !bulkPlantRigJobIds.Contains(p.Id)).ToList();

            if (isShowFutureJobs && servicePointIds.Count > 0)
                rigJobs = rigJobs.Where(p => servicePointIds.Contains(p.ServicePoint.Id)).ToList();

            jobAlerts = eServiceOnlineGateway.Instance.GetRigJobsByQuery(pager, rigJob=>rigJob.JobLifeStatus == JobLifeStatus.Alerted); 
//			jobAlerts = allRigJobs.Where(p => p.JobLifeStatus == JobLifeStatus.Alerted).OrderBy(p => p.ClientCompanyShortName).ThenBy(p => p.Rig.Name).ToList();            
            if (servicePointIds.Count > 0)
                jobAlerts = jobAlerts.Where(p => servicePointIds.Contains(p.ServicePoint.Id)).ToList();
          

	        rigJobs = rigJobs.OrderBy(p => p.RigStatus).ThenBy(p => p.ClientCompanyShortName).ThenByDescending(p => p.Rig?.Name).ThenBy(p => p.JobDateTime).GroupBy(s => s.Id).Select(c => c.First()).ToList();
            
             jobAlerts.AddRange(rigJobs);

            if (rigTypes[2] == 1)
            {
                HashSet<int> cidsInB = new HashSet<int>(eServiceOnlineGateway.Instance.GetTestingCallSheetBlendSection().Select(b => b.CallSheet.Id));

                jobAlerts = jobAlerts.Where(a => cidsInB.Contains(a.CallSheetId)).ToList();


            }

            //May 06, 2024 Tongtao 186_PR_RigBoardPageSelection: The number of data items should be obtained after the final data filtering is completed, otherwise it will result in pages with no data.
            count = jobAlerts.Count;

            var rigJobList = jobAlerts.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();
          
             return rigJobList;
        }


        /*private static List<RigJob> GetRigJobCollectionByRequestedConditionAndPaginated_Bak(int pageNumber, int pageSize, Collection<int> servicePointIds, Collection<int> rigTypes, Collection<int> jobLifeStatuses, bool isShowJobAlert, bool isShowFutureJobs, out int count)
        {
            List<RigJob> jobAlerts = new List<RigJob>();

            List<RigJob> allRigJobs = servicePointIds.Count > 0 ? eServiceOnlineGateway.Instance.GetRigJobsByServicePoints(servicePointIds) : eServiceOnlineGateway.Instance.GetRigJobs();
            List<RigJob> rigJobs = allRigJobs;
            rigJobs = rigJobs.Where(p => p.IsProjectRig == rigTypes[0].Equals(1)).ToList();
            if (!rigTypes[1].Equals(2))
            {
                rigJobs = rigJobs.Where(p => p.IsServiceRig == rigTypes[1].Equals(1)).ToList();
            }

            List<RigJob> rigJobList = jobLifeStatuses.Count > 0 ? rigJobs.FindAll(p => jobLifeStatuses.Contains((int) p.JobLifeStatus)).ToList() : rigJobs;
            rigJobList = rigJobList.Where(p => p.IsListed && p.RigStatus != RigStatus.Deactivated).ToList();

            if (isShowFutureJobs)
            {
                List<RigJob> upcomingJobs = new List<RigJob>();

                foreach (RigJob rigJob in rigJobList)
                {
                    if (rigJob.JobLifeStatus == JobLifeStatus.Completed || rigJob.JobLifeStatus==JobLifeStatus.Canceled || rigJob.JobLifeStatus == JobLifeStatus.None) continue;
                    upcomingJobs.AddRange(allRigJobs.FindAll(p => p.Rig.Id == rigJob.Rig.Id && p.Id != rigJob.Id && (p.JobLifeStatus == JobLifeStatus.InProgress || p.JobLifeStatus == JobLifeStatus.Pending || p.JobLifeStatus == JobLifeStatus.Confirmed || p.JobLifeStatus == JobLifeStatus.Scheduled || p.JobLifeStatus == JobLifeStatus.Dispatched)));
                }
                rigJobList.AddRange(upcomingJobs);
            }

            if (isShowJobAlert)
            {
                jobAlerts = rigJobs.FindAll(p => p.JobLifeStatus == JobLifeStatus.Alerted && !string.IsNullOrEmpty(p.ClientCompanyShortName)).ToList();
            }

            rigJobList.AddRange(jobAlerts);
            rigJobList = rigJobList.OrderBy(p => p.RigStatus).ThenBy(p => p.ClientCompanyShortName).ThenByDescending(p => p.Rig?.Name).ThenBy(p => p.JobDateTime).ToList();

            if (jobLifeStatuses.Count.Equals(0) || jobLifeStatuses.Contains(1))
            {
                rigJobList.AddRange(rigJobs.FindAll(p => p.JobLifeStatus == JobLifeStatus.Alerted && string.IsNullOrEmpty(p.ClientCompanyShortName)));
            }
            rigJobList = rigJobList.GroupBy(s => s.Id).Select(c => c.First()).ToList();
            count = rigJobList.Count;
            rigJobList = eServiceOnlineGateway.Instance.GetRigJobsWithChildren(rigJobList).Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();

            return rigJobList;
        }*/
        public static List<ProductHaul> GetProductHaulCollectionByCallSheetNumber(
            int callSheetNumber)
        {
            List<ProductHaul> productHauls = new List<ProductHaul>();
            List<ShippingLoadSheet> loads = eServiceOnlineGateway.Instance.GetShippingLoadSheetsByCallSheetNumber(callSheetNumber).ToList();
           
            if (loads.Count > 0)
            {
                List<int> productHaulIds = loads.Select(p => p.ProductHaul.Id).Distinct().ToList();

                if (productHaulIds.Count > 0)
                {
                    productHauls = eServiceOnlineGateway.Instance.GetProductHaulByIds(productHaulIds);
                }
            }

            return productHauls;
        }

        public static List<Bin> GetBinCollectionByRig(Rig rig)
        {
            List<Bin> bins = new List<Bin>();
            if (rig != null)
            {
                List<BinInformation> rigBinSections = eServiceOnlineGateway.Instance.GetBinInformationsByRigId(rig.Id);
                bins = GetBinCollectionByRigBinSections(rigBinSections);
            }

            return bins;
        }
        public static List<Bin> GetBinCollectionByRigBinSections(List<BinInformation> rigBinSections)
        {
            List<Bin> bins = new List<Bin>();
            foreach (var rigBinSection in rigBinSections)
            {
                bins.Add(rigBinSection.Bin);
            }
            return bins;
        }
        public static List<BinInformation> GetRigBinSectionCollectionByRig(Rig rig)
        {
            return  eServiceOnlineGateway.Instance.GetBinInformationsByRigId(rig.Id);
        }

        #endregion

        public static bool ReleaseBinFromRig(int binId, int callSheetId, int rigJobId)
        {
            RigJob rigJob = eServiceOnlineGateway.Instance.GetRigJobById(rigJobId);
            List<BinInformation> rigBinSections = eServiceOnlineGateway.Instance.GetBinInformationsByRigId(rigJob.Rig.Id);
            List<BinInformation> binInformationList = rigBinSections.Where(p => p.Bin.Id == binId).ToList();
            // 触发remove bin时，修改bininformation信息
            int result = 0;
            foreach(BinInformation binInformation in binInformationList)
            {
                binInformation.Rig = null;
                binInformation.BinStatus = BinStatus.Yard;
                binInformation.WorkingServicePoint = null;
                result += eServiceOnlineGateway.Instance.UpdateBinInformation(binInformation);
//Revert the logic to fix the bug:Assign Bin created new BinInformation Record, but not disabled previous record.
//                result += eServiceOnlineGateway.Instance.DeleteBinInformation(binInformation);
            }
       
            if (result>0)
            {
                if ((rigJob.JobLifeStatus == JobLifeStatus.Pending || rigJob.JobLifeStatus == JobLifeStatus.Confirmed || rigJob.JobLifeStatus == JobLifeStatus.Scheduled))
                {
                    IBinSectionMicroService binSectionMicroService = ServiceFactory.Instance.GetService(typeof(IBinSectionMicroService)) as IBinSectionMicroService;
                    if (binSectionMicroService == null) throw new Exception("binSectionMicroService can not be null.");
                    Collection<BinSection> binSections = binSectionMicroService.GetBinSectionCollctionByRootId(callSheetId);

                    foreach (var binSection in binSections)
                    {
                        if (binSection.Bin.Id == binId)
                        {
                            binSectionMicroService.DeleteBinSection(binSection);
                            break;
                        }
                    }

                    return true;
                }

                return true;
            }

            return false;
        }

        private static RigJob GetEarliestAndNotCompletedRigJobByRigId(int rigId, List<string> jobLifeStatusList)
        {
            List<RigJob> rigJobs = eServiceOnlineGateway.Instance.GetUnListedRigJobByRigId(rigId);
            RigJob rigJob = rigJobs.Where(p => !jobLifeStatusList.Contains(p.JobLifeStatus.ToString())).OrderBy(p => p.JobDateTime).FirstOrDefault();

            return rigJob;
        }

        private static RigJob GetPreviousRigJobByRigId(int rigId, List<string> jobLifeStatusList)
        {
            List<RigJob> rigJobs = eServiceOnlineGateway.Instance.GetUnListedRigJobByRigId(rigId);
            RigJob rigJob = rigJobs.Where(p => jobLifeStatusList.Contains(p.JobLifeStatus.ToString())).OrderBy(p => p.JobDateTime).FirstOrDefault();

            return rigJob;
        }

        public static int DeleteRigJob(RigJob rigJob)
        {
            if (rigJob == null) return 0;
//            rigJob.JobLifeStatus = eServiceOnlineGateway.Instance.GetJobLifeStatusById(rigJob.JobLifeStatus?.Id ?? 0);
            if (rigJob.JobLifeStatus == JobLifeStatus.Alerted) return DeleteRigJobAndUpdateListedRigJob(rigJob);
            
            RigJob newRigJob = rigJob;
            RigJob oldRigJob = rigJob;
            List<string> statusNames = new List<string>
            {
                "None", "Alerted", "Canceled", "Completed", "Deleted"
            };
            
            RigJob earliestRigJob = GetEarliestAndNotCompletedRigJobByRigId(rigJob.Rig?.Id ?? 0, statusNames);

            if (earliestRigJob != null)
            {
                newRigJob = earliestRigJob;
                newRigJob.IsListed = true;
                UpdateRigJob(newRigJob);
            }
            else
            {
                List<string> statusNameList = new List<string>
                {
                    "None", "Alerted", "Canceled", "Completed"
                };
                RigJob previousRigJob = GetPreviousRigJobByRigId(rigJob.Rig?.Id ?? 0, statusNameList);

                if (previousRigJob != null)
                {
                    newRigJob = previousRigJob;
                    newRigJob.IsListed = true;
                    UpdateRigJob(newRigJob);
                }
                else
                {
                    //If there is no previous rig job exists, create an empty rig job to keep the active rig stay on the board.
                    newRigJob = new RigJob(){Rig = rigJob.Rig,IsListed = true,JobDateTime = DateTime.Now, JobLifeStatus=JobLifeStatus.None };
                    CreateRigJob(newRigJob);
                }
            }

            oldRigJob.IsListed = false;
            JobLifeStatus jobLifeStatus = JobLifeStatus.Deleted;
            oldRigJob.JobLifeStatus = jobLifeStatus;
            UpdateRigJob(oldRigJob);

            return 1;
        }

        public static bool ActivateARig(int rigId)
        {
            //Get rig by id, update rig status

            bool succeed = false;
            Rig rig = eServiceOnlineGateway.Instance.GetRigById(rigId);

            if (rig == null) throw new Exception("The rig status doesn't exist for a Rig Activation");
            if (rig.Status != RigStatus.Deactivated) throw new Exception("The rig status is not Deactivated for a Rig Activation");


            rig.Status = RigStatus.Active;
            rig.ModifiedUserName = "RigBoard";

            eServiceOnlineGateway.Instance.UpdateRig(rig);

            //Verify activation is successful

            Rig updatedRig = eServiceOnlineGateway.Instance.GetRigById(rig.Id);
            if (updatedRig != null && updatedRig.Status == RigStatus.Active)
            {
	            succeed = true;
            }

            //Get last rig deactivation job
            List<RigJob> deactivatedRigJobs = eServiceOnlineGateway.Instance.GetRigJobsByQuery(null, job => job.RigStatus == RigStatus.Deactivated && job.Rig.Id == rigId);

            //Get last completed job by rig id
            List<RigJob> completedRigJobs = eServiceOnlineGateway.Instance.GetRigJobsByQuery(null, job => job.JobLifeStatus == JobLifeStatus.Completed && job.Rig.Id == rigId);

            RigJob rigJob = new RigJob()  { JobDateTime=DateTime.Now, ModifiedUserName = "RigBoard" };
            bool update = false;
            //Find last rig deactivation job
            if (deactivatedRigJobs != null && deactivatedRigJobs.Count > 1)
            {
	            rigJob = deactivatedRigJobs.OrderByDescending(p => p.Id).FirstOrDefault();
	            update = true;
            }

            if (completedRigJobs != null && completedRigJobs.Count > 0)
            {
                //Carry over last completed job information
	            var lastCompletedJob = completedRigJobs.OrderByDescending(p => p.JobDateTime).FirstOrDefault();

	            rigJob.ClientCompany = lastCompletedJob.ClientCompany;
	            rigJob.ClientCompanyShortName = lastCompletedJob.ClientCompany.ShortName;
	            rigJob.WellLocationType = lastCompletedJob.WellLocationType;
	            rigJob.WellLocation = lastCompletedJob.WellLocation;
	            rigJob.SurfaceLocation = lastCompletedJob.SurfaceLocation;
	            rigJob.JobType = lastCompletedJob.JobType;
	            rigJob.Notes = lastCompletedJob.Notes;
	            rigJob.ServicePoint = lastCompletedJob.ServicePoint;
	            rigJob.JobDateTime = lastCompletedJob.JobDateTime;
	            rigJob.JobLifeStatus = JobLifeStatus.None;
	            rigJob.IsNeedBins = lastCompletedJob.IsNeedBins;
	            rigJob.ClientConsultant1 = lastCompletedJob.ClientConsultant1;
	            rigJob.ClientConsultant2 = lastCompletedJob.ClientConsultant2;
            }

            rigJob.RigStatus = RigStatus.Active;
            rigJob.IsListed = true;


            if (update)
	            eServiceOnlineGateway.Instance.UpdateRigJob(rigJob);
            else
	            eServiceOnlineGateway.Instance.CreateRigJob(rigJob);


            return succeed;
        }

        public static bool DeactivateRig(int rigId)
        {
            bool succeed = false;
            int updateRigReturn = 0;
            Rig rig = eServiceOnlineGateway.Instance.GetRigById(rigId);
            if (rig != null)
            {
                rig.Status = RigStatus.Deactivated;
                rig.ModifiedDateTime = DateTime.Now;
                rig.ModifiedUserName = "RigBoard";
                rig.ModifiedUserId = 0;
                rig.OperationType = MetaShare.Common.Core.Entities.DataOperationType.Update;

                updateRigReturn = eServiceOnlineGateway.Instance.UpdateRig(rig);

                List<RigJob> rigJobs = eServiceOnlineGateway.Instance.GetListedRigJobByRigId(rigId).Where(p => p.JobLifeStatus!=JobLifeStatus.Alerted).ToList();
                RigJob currentListedJob = rigJobs.OrderByDescending(p => p.JobDateTime).FirstOrDefault();
                if (currentListedJob != null)
                {
                    if (currentListedJob.JobLifeStatus != JobLifeStatus.Completed &&
                        currentListedJob.JobLifeStatus != JobLifeStatus.Canceled && currentListedJob.JobLifeStatus != JobLifeStatus.None && currentListedJob.JobLifeStatus != JobLifeStatus.Deleted)
                    {
//The rig should not be deactivated if a job is still in process
                        return false;
                    }
                    else
                    {
                        currentListedJob.IsListed = false;

                        eServiceOnlineGateway.Instance.UpdateRigJob(currentListedJob);


                        RigJob newRigJob = new RigJob
                        {
                            Rig = rig,
                            RigStatus = RigStatus.Deactivated
                        };

                        newRigJob.ClientCompany = currentListedJob.ClientCompany;
                        newRigJob.ClientCompanyShortName = currentListedJob.ClientCompany.ShortName;
                        newRigJob.WellLocationType = currentListedJob.WellLocationType;
                        newRigJob.WellLocation = currentListedJob.WellLocation;
                        newRigJob.SurfaceLocation = currentListedJob.SurfaceLocation;
                        newRigJob.JobType = currentListedJob.JobType;
                        newRigJob.Notes = currentListedJob.Notes;
                        newRigJob.ServicePoint = currentListedJob.ServicePoint;
                        newRigJob.JobDateTime = currentListedJob.JobDateTime;
                        newRigJob.JobLifeStatus = JobLifeStatus.None;
                        newRigJob.IsNeedBins = currentListedJob.IsNeedBins;
                        newRigJob.ClientConsultant1 = currentListedJob.ClientConsultant1;
                        newRigJob.ClientConsultant2 = currentListedJob.ClientConsultant2;

                        eServiceOnlineGateway.Instance.CreateRigJob(newRigJob);
                    }
                }

            }
            return updateRigReturn==1;

        }

        #region CreateOrUpdateThirdPartyUnitSectionByProductHaul


        #endregion

        #region #68 Update ClientConsultant And  Related to it Rigjob.

        public static void UpdateClientConsultant(bool isUpdateRigJob, ClientConsultant clientConsultant)
        {
            eServiceOnlineGateway.Instance.UpdateClientConsultant(clientConsultant);

            if (isUpdateRigJob)
            {
                List<RigJob> collection = eServiceOnlineGateway.Instance.GetRigJobsByQuery(null, p=>(p.ClientConsultant1.Id == clientConsultant.Id || p.ClientConsultant2.Id == clientConsultant.Id)  && (p.JobLifeStatus == JobLifeStatus.Pending || p.JobLifeStatus == JobLifeStatus.Confirmed || p.JobLifeStatus == JobLifeStatus.Scheduled || p.JobLifeStatus == JobLifeStatus.Dispatched || p.IsListed == true));
                if (collection.Count >0 )
                {
                    foreach (RigJob rigJob in collection)
                    {
                        isUpdateRigJob = false;
                        RigJob entity = rigJob;
                        if (rigJob.ClientConsultant1 != null && rigJob.ClientConsultant1.Id == clientConsultant.Id)
                        {
                            isUpdateRigJob = true;
                            entity.ClientConsultant1 = clientConsultant;
                        }

                        if (rigJob.ClientConsultant2 != null && rigJob.ClientConsultant2.Id == clientConsultant.Id)
                        {
                            isUpdateRigJob = true;
                            entity.ClientConsultant2 = clientConsultant;
                        }

                        if (isUpdateRigJob)
                        {
                            UpdateRigJob(entity);
                        }
                    }
                }
            }
        }

        #endregion

        #region #71 Modifying the Logical Reconstruction of Update Rig Info

        public static void UpdateRigInfo(Rig rig)
        {
            Rig originalRig = eServiceOnlineGateway.Instance.GetRigById(rig.Id);
            rig.Status = originalRig.Status;
            var updatedRig = eServiceOnlineGateway.Instance.UpdateRig(rig);

            if (updatedRig != 0)
            {
                List<RigJob> rigJobs = eServiceOnlineGateway.Instance.GetRigJobsByRigId(rig.Id);
                RigJob listedRigJob = rigJobs.Find(p => p.IsListed == true) as RigJob;
                {
                    //create fake job to log the rig status change
                    RigJob newRigJob = new RigJob();
                    newRigJob.Rig = rig;
                    newRigJob.RigStatus = rig.Status;
                    newRigJob.JobDateTime = DateTime.MinValue;
                    newRigJob.JobLifeStatus = JobLifeStatus.None;
                    newRigJob.Notes = "Rig Information is changed";

                    CreateRigJob(newRigJob);
                }
                //Apply rig changes to  upcoming jobs 
                foreach (RigJob rigJob in rigJobs)
                {
                    var newVersionRigJob = rigJob;
                    if (rigJob.JobDateTime > listedRigJob.JobDateTime && listedRigJob.JobLifeStatus != JobLifeStatus.Canceled && listedRigJob.JobLifeStatus != JobLifeStatus.Completed)
                    {
                        //update rig jobs later than listed rig job
                        newVersionRigJob.Rig = rig;
                        UpdateRigJob(newVersionRigJob);
                    }
                    else
                    {
                        if (rig.IsTopDrive != originalRig.IsTopDrive)
                        {
                            newVersionRigJob.Rig.IsTopDrive = rig.IsTopDrive;
                            UpdateRigJob(newVersionRigJob);
                        }
                    }
                }
            }
        }

        #endregion

        #region Update ProductHaul And ProductHaulLoads OnLocation Status

        /*
        public static void SetProductHaulAndLoadsOnLocation(int productHaulId, DateTime onLocationTime, string userName = null)
        {
            List<ProductHaulLoad> productHaulLoads = eServiceOnlineGateway.Instance.GetProductHaulLoadsByProductHual(new ProductHaul{Id = productHaulId });
            foreach (var productHaulLoad in productHaulLoads)
            {
                SetProductHaulLoadOnLocation(productHaulLoad, onLocationTime, userName);
            }

            ProductHaulProcess.SetProductHaulOnLocation(productHaulId, onLocationTime, userName);

            //计算bininfomation的容量值
//            BinBoardProcess.CalculateBinformation(productHaulLoads);

        }
        */
        /*
        public static void UpdateProductHaulLoadOnLocation(ProductHaulLoad productHaulLoad, DateTime onLocationTime, string userName = null)
        {
            SetProductHaulLoadOnLocation(productHaulLoad, onLocationTime, userName);
            ProductHaulProcess.SetProductHaulOnLocation(productHaulLoad.ProductHaul.Id, onLocationTime, userName);

        }
        */

        /*
        public static void SetProductHaulLoadOnLocation(ProductHaulLoad productHaulLoad, DateTime onLocationTime,
            string userName = null)
        {
            if(!productHaulLoad.IsGoWithCrew && productHaulLoad.Bin!=null && productHaulLoad.Bin.Id !=0)
                BinProcess.LoadBlendToBin(productHaulLoad.Bin,productHaulLoad.PodIndex, productHaulLoad.BlendChemical.Name,
                productHaulLoad.BlendChemical.Description, productHaulLoad.TotalBlendWeight,
                productHaulLoad.ProductHaul, userName);
            productHaulLoad.ProductHaulLoadLifeStatus = ProductHaulLoadStatus.OnLocation;
            productHaulLoad.OnLocationTime = onLocationTime;
            productHaulLoad.ModifiedUserName = userName;
            eServiceOnlineGateway.Instance.UpdateProductHaulLoad(productHaulLoad);
        }
        */
        public static List<ProductHaul> GetExistingProductHaulCollection()
        {
            List<ProductHaul> productHaulCollection = new List<ProductHaul>();
            List<ProductHaul> scheduledProductHauls = eServiceOnlineGateway.Instance.GetScheduledProductHauls();
            productHaulCollection.AddRange(scheduledProductHauls);

            return productHaulCollection;
        }

        public static void CreateProductHaulLoad(ProductHaulLoad productHaulLoad)
        {
//            List<ProductLoadSection> productLoadSections = productHaulLoad.AllProductLoadList;

            productHaulLoad.ProductHaulLoadLifeStatus = ProductHaulLoadStatus.Scheduled;
            eServiceOnlineGateway.Instance.CreateProductHaulLoad(productHaulLoad, true);

          
            /*
            if (productLoadSections != null && productLoadSections.Count > 0)
            {
                foreach (ProductLoadSection productLoadSection in productLoadSections)
                {
                    productLoadSection.OwnerId = productHaulLoad.Id;
                    productLoadSection.ProductHaulLoad = productHaulLoad;
                    eServiceOnlineGateway.Instance.CreateProductLoadSection(productLoadSection);
                }
            }
            */
            //if (productHaulLoad.Bin != null && productHaulLoad.Bin.Id > 0)
            //{
            //    BinInformation binInformation = eServiceOnlineGateway.Instance.GetBinInformationByBinIdAndPodIndex(productHaulLoad.Bin.Id, productHaulLoad.PodIndex);
            //    binInformation.LastProductHaulLoadId = productHaulLoad.Id;
            //    eServiceOnlineGateway.Instance.UpdateBinInformation(binInformation);
            //}
        }

        /*public static void UpdateProductHaulAndCreateLoad(ProductHaulLoad productHaulLoad, int productHaulId, int callSheetId,int crewId,int rigJobId)
        {
            ProductHaul productHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);
            RigJob rigJob = eServiceOnlineGateway.Instance.GetRigJobById(rigJobId);
            
            List<ProductLoadSection> productLoadSections = productHaulLoad.AllProductLoadList;
            productHaulLoad.ProductHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);
            ProductHaulLoad createProductHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productHaulLoad.Id);


            if (productHaul != null)
            {             
                if (productHaul.IsThirdParty)
                {
                    ThirdPartyBulkerCrewSchedule thirdPartyBulkerCrewSchedule = eServiceOnlineGateway.Instance.GetThirdPartyCrewScheduleById(productHaul.Schedule?.Id ?? 0);
                  
                    UpdateThirdPartyCrewScheduleJobInfo(productHaul);

                    ThirdPartyUnitSection thirdPartyUnitSection = eServiceOnlineGateway.Instance.GetThirdPartyUnitSectionByCallSheetAndProductHaul(new Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch.CallSheet{Id = callSheetId}, new ProductHaul{Id = productHaulId});
                
                    if (thirdPartyUnitSection != null)
                    {
                        thirdPartyUnitSection.Description = BuildUnitSectionComments(productHaul);
                        eServiceOnlineGateway.Instance.UpdateThirdPartyUnitSection(thirdPartyUnitSection);
                    }
                    else
                    {
                        CreateUnitSection(thirdPartyBulkerCrewSchedule?.ThirdPartyBulkerCrew?.Id??0, callSheetId, productHaul.IsThirdParty, productHaul.IsGoWithCrew, productHaulId);
                    }       
                }
                else
                {
                    SanjelCrewSchedule crewSchedule = eServiceOnlineGateway.Instance.GetCrewScheduleById(productHaul.Schedule?.Id ?? 0);

                    UpdateCrewScheduleJobInfo(productHaul);

                    UnitSection unitSection = eServiceOnlineGateway.Instance.GetUnitSectionByCallSheetAndProductHaul(new Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch.CallSheet { Id = callSheetId }, new ProductHaul { Id = productHaulId });
                    if (unitSection != null)
                    {
                        unitSection.HaulDescription = BuildUnitSectionComments(productHaul);
                        eServiceOnlineGateway.Instance.UpdateUnitSection(unitSection);
                    }
                    else
                    {
                        CreateUnitSection(crewSchedule?.SanjelCrew?.Id??0, callSheetId, productHaul.IsThirdParty, productHaul.IsGoWithCrew, productHaulId);
                    }
                    UpdateUnitSections(productHaul);
                }
            }

        }*/

        /*
        private static void UpdateUnitSections(ProductHaul productHaul)
        {
            Collection<UnitSection> unitSections = eServiceOnlineGateway.Instance.GetUnitSectionsByProductHaul(productHaul);
            foreach (UnitSection unitSection in unitSections)
            {
                unitSection.HaulDescription = productHaul.Description;
                eServiceOnlineGateway.Instance.UpdateUnitSection(unitSection);
            }
        }
        */

        private static void UpdateThirdPartyUnitSections(ProductHaul productHaul)
        {
            Collection<ThirdPartyUnitSection> thirdPartyUnitSections = eServiceOnlineGateway.Instance.GetThirdPartyUnitSectionsByProductHaul(productHaul);
            foreach (ThirdPartyUnitSection thirdPartyUnitSection in thirdPartyUnitSections)
            {
                thirdPartyUnitSection.Description = productHaul.Description;
                eServiceOnlineGateway.Instance.UpdateThirdPartyUnitSection(thirdPartyUnitSection);
            }
        }

        /*
        public static void CreateUnitSectionOrThirdPartyUnitSection(ProductHaul productHaul, int callSheetId,int crewId)
        {
            CreateUnitSection(crewId, callSheetId, productHaul.IsThirdParty, productHaul.IsGoWithCrew, productHaul.Id);
        }
        */


        public static void DeleteThirdPartyUnitSectionByProductHaul(ProductHaul productHaul)
        {
            Collection<ThirdPartyUnitSection> thirdPartyUnitSections = eServiceOnlineGateway.Instance.GetThirdPartyUnitSectionsByProductHaul(productHaul);
            if (thirdPartyUnitSections != null && thirdPartyUnitSections.Count > 0)
            {
                foreach (var thirdPartyUnitSection in thirdPartyUnitSections)
                {
                    eServiceOnlineGateway.Instance.DeleteThirdPartyUnitSection(thirdPartyUnitSection);
                }
            }

        }
        /*
        public static void UpdateProductHaulAndHaulLoads(ProductHaul productHaul, bool isGoWithCrew, DateTime expectedOnLocationTime,int rigJobId,int crewId, bool originalHaulIsThirdParty)
        {
            ProductHaul originalVersion = eServiceOnlineGateway.Instance.GetProductHaulById(productHaul.Id);
            List<ProductHaulLoad> productHaulLoads = eServiceOnlineGateway.Instance.GetProductHaulLoadsByProductHual(originalVersion);

            originalVersion.IsGoWithCrew = isGoWithCrew;
           
            originalVersion.IsThirdParty = productHaul.IsThirdParty;
            originalVersion.EstimatedLoadTime = productHaul.EstimatedLoadTime;
            originalVersion.EstimatedTravelTime = productHaul.EstimatedTravelTime;

            var rigJob = eServiceOnlineGateway.Instance.GetRigJobById(rigJobId);

            if (productHaulLoads.Count > 0)
            {
                foreach (ProductHaulLoad productHaulLoad in productHaulLoads)
                {
                    if (isGoWithCrew)
                    {
                        productHaulLoad.ExpectedOnLocationTime = rigJob==null? DateTime.MinValue: rigJob.JobDateTime;
                        productHaulLoad.EstmatedLoadTime = productHaul.EstimatedLoadTime;
                    }
                    else
                    {
                        productHaulLoad.ExpectedOnLocationTime = expectedOnLocationTime;
                        productHaulLoad.EstmatedLoadTime = productHaul.EstimatedLoadTime;
                    }
                    productHaulLoad.IsGoWithCrew = isGoWithCrew;
                    eServiceOnlineGateway.Instance.UpdateProductHaulLoad(productHaulLoad);
                }
            }

            originalVersion.ProductHaulLoads = productHaulLoads;
            //更新schedule
            if (isGoWithCrew)
            {
                DateTime endTime = GetRigJobAssignCrewEndTime(rigJobId);
                if (endTime == DateTime.MinValue)
                {
                    originalVersion.EstimatedTravelTime = 8;
                }
                else
                {
                    originalVersion.EstimatedTravelTime = (endTime - originalVersion.EstimatedLoadTime).TotalHours;
                }

            }

            UpdateSchedule(originalVersion,crewId, originalHaulIsThirdParty);
  
            eServiceOnlineGateway.Instance.UpdateProductHaul(originalVersion);
        }
        */
        public static DateTime GetRigJobAssignCrewEndTime(int rigJobId)
        {
            RigJobSanjelCrewSection rigJobSanjelCrewSection = eServiceOnlineGateway.Instance.GetRigJobCrewSectionByRigJob(rigJobId).FindAll(s => s.ProductHaul.Id == 0).OrderByDescending(s => s.Id).FirstOrDefault();
            if (rigJobSanjelCrewSection != null)
            {
                SanjelCrewSchedule schedule = eServiceOnlineGateway.Instance.GetCrewScheduleByJobCrewSection(rigJobSanjelCrewSection.Id);
                if (schedule==null || schedule.EndTime < DateTime.Now)
                {
                    return DateTime.MinValue;
                }
                return schedule.EndTime;
            }
            return DateTime.MinValue;
        }
        private static void UpdateSanjelCrewSchedule(DateTime startTime, DateTime endTime, SanjelCrewSchedule sanjelCrewSchedule)
        {
            if (sanjelCrewSchedule != null)
            {
                var originalSanjelCrewSchedule =
                    eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(sanjelCrewSchedule.Id, true);
                
                originalSanjelCrewSchedule.StartTime = startTime;
                originalSanjelCrewSchedule.EndTime = endTime;
                
                foreach (var workerSchedule in originalSanjelCrewSchedule.WorkerSchedule)
                {
                    workerSchedule.StartTime = startTime;
                    workerSchedule.EndTime = endTime;
                }
                
                foreach (var unitSchedule in originalSanjelCrewSchedule.UnitSchedule)
                {
                    unitSchedule.StartTime = startTime;
                    unitSchedule.EndTime = endTime;
                }
                
                eServiceOnlineGateway.Instance.UpdateSanjelCrewSchedule(originalSanjelCrewSchedule, true);

            }
            
        }

        private static void UpdateThirdPartyBulkerCrewSchedule(DateTime startTime, DateTime endTime, ThirdPartyBulkerCrewSchedule thirdPartyBulkerCrewSchedule)
        {
            if (thirdPartyBulkerCrewSchedule != null)
            {
                thirdPartyBulkerCrewSchedule.StartTime = startTime;
                thirdPartyBulkerCrewSchedule.EndTime = endTime;
                eServiceOnlineGateway.Instance.UpdateThirdPartyCrewSchedule(thirdPartyBulkerCrewSchedule);
            }
        }

        /*private  static void UpdateRigJobCrewSection(ProductHaul productHaul,int crewId, bool originalHaulIsThirdParty)
        {      
            int oldCrewId = 0;
            ThirdPartyBulkerCrewSchedule thirdPartyBulkerCrewSchedule = eServiceOnlineGateway.Instance.GetThirdPartyCrewScheduleById(productHaul?.Schedule?.Id ?? 0);

            SanjelCrewSchedule sanjelCrewSchedule = eServiceOnlineGateway.Instance.GetCrewScheduleById(productHaul?.Schedule?.Id ?? 0);



            if (originalHaulIsThirdParty == productHaul.IsThirdParty)
            {
                if (productHaul.IsThirdParty)
                {
                    if (thirdPartyBulkerCrewSchedule != null)
                    {
                        oldCrewId = thirdPartyBulkerCrewSchedule.ThirdPartyBulkerCrew.Id;
                        if (crewId == oldCrewId) UpdateThirdPartyBulkerCrewSchedule(productHaul.EstimatedLoadTime, productHaul.EstimatedLoadTime.AddHours(productHaul.EstimatedTravelTime), thirdPartyBulkerCrewSchedule);
                        else
                        {
                            thirdPartyBulkerCrewSchedule.ThirdPartyBulkerCrew = eServiceOnlineGateway.Instance.GetThirdPartyBulkerCrewById(crewId);
                            eServiceOnlineGateway.Instance.UpdateThirdPartyCrewSchedule(thirdPartyBulkerCrewSchedule);
                            CreateOrUpdateCrewSections(productHaul.IsThirdParty, productHaul, true, crewId);
                        }
                        UpdateThirdPartyCrewScheduleJobInfo(productHaul);
                    }
                }
                else
                {
                    if (sanjelCrewSchedule != null)
                    {
                        oldCrewId = sanjelCrewSchedule.SanjelCrew.Id;
                        if (crewId == oldCrewId) UpdateSanjelCrewSchedule(productHaul.EstimatedLoadTime, productHaul.EstimatedLoadTime.AddHours(productHaul.EstimatedTravelTime), sanjelCrewSchedule);
                        else
                        {
                            var sanjelCrew = eServiceOnlineGateway.Instance.GetCrewById(crewId, true);
                            sanjelCrewSchedule.SanjelCrew = sanjelCrew;
                            SetDriverAndBulk(productHaul, sanjelCrew);

                            eServiceOnlineGateway.Instance.UpdateSanjelCrewSchedule(sanjelCrewSchedule);
                            CreateOrUpdateCrewSections(productHaul.IsThirdParty, productHaul, true, crewId);
                        }
                        UpdateCrewScheduleJobInfo(productHaul);
                    }
                }
            }
            else
            {
                if (originalHaulIsThirdParty)
                {
                    if (thirdPartyBulkerCrewSchedule != null)
                    {
                        RigJobThirdPartyBulkerCrewSection rigJobThirdPartyBulkerCrewSection = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionById(thirdPartyBulkerCrewSchedule.RigJobThirdPartyBulkerCrewSection?.Id??0);
                        RigJob rigJob = eServiceOnlineGateway.Instance.GetRigJobById(rigJobThirdPartyBulkerCrewSection.RigJob.Id);
                        SanjelCrew sanjelCrew = eServiceOnlineGateway.Instance.GetCrewById(crewId);
                        CreateOrUpdateCrewSections(productHaul.IsThirdParty, productHaul, false, crewId);
                        RigJobSanjelCrewSection rigJobSanjelCrewSection = eServiceOnlineGateway.Instance.GetRigJobCrewSectionsByProductHaul(productHaul.Id);
//                        List<RigJobSanjelCrewSection> rigJobSanjelCrewSections = eServiceOnlineGateway.Instance.GetRigJobCrewSectionsByProductHaul(productHaul.Id);
//                        RigJobSanjelCrewSection rigJobSanjelCrewSection = rigJobSanjelCrewSections.Find(p => p.RigJob.Id == rigJob.Id);
                        productHaul.Schedule = CreateSchedules(productHaul.EstimatedLoadTime, productHaul.EstimatedLoadTime.AddHours(productHaul.EstimatedTravelTime) , sanjelCrew,rigJob, null, rigJobSanjelCrewSection);

                        eServiceOnlineGateway.Instance.DeletethirdPartyBulkerCrewSchedule(thirdPartyBulkerCrewSchedule.Id);

                    }
                }
                else
                {
                    if (sanjelCrewSchedule != null)
                    {
                        RigJobSanjelCrewSection rigJobSanjelCrewSection = eServiceOnlineGateway.Instance.GetRigJobCrewSectionById(sanjelCrewSchedule.RigJobSanjelCrewSection?.Id ??0);
                        if (rigJobSanjelCrewSection != null)
                        {
                            RigJob rigJob = eServiceOnlineGateway.Instance.GetRigJobById(rigJobSanjelCrewSection.RigJob.Id);
                            ThirdPartyBulkerCrew thirdPartyBulkerCrew = eServiceOnlineGateway.Instance.GetThirdPartyBulkerCrewById(crewId);

                            CreateOrUpdateCrewSections(productHaul.IsThirdParty, productHaul, false, crewId);
                            RigJobThirdPartyBulkerCrewSection rigJobThirdPartyBulkerCrewSection = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionsByProductHual(productHaul.Id);
                            //                        List<RigJobThirdPartyBulkerCrewSection> rigJobThirdPartyBulkerCrewSections = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionsByProductHual(productHaul.Id);
                            //                        RigJobThirdPartyBulkerCrewSection rigJobThirdPartyBulkerCrewSection = rigJobThirdPartyBulkerCrewSections.Find(p => p.RigJob.Id == rigJob.Id);

                            productHaul.Schedule = CreateThirdPartyBulkerCrewSchedule(productHaul.EstimatedLoadTime, productHaul.EstimatedLoadTime.AddHours(productHaul.EstimatedTravelTime), thirdPartyBulkerCrew, rigJob, productHaul, rigJobThirdPartyBulkerCrewSection);
                            ScheduleProcess.DeleteSanjelCrewSchedule(sanjelCrewSchedule);
                        }
                    }
                }
            }
            
        }*/


        /*
        private static void CreateOrUpdateCrewSections(bool isThirdParty, ProductHaul productHaul, bool isUpdate, int crewId)
        {
            if (isThirdParty)
            {
                ThirdPartyBulkerCrew thirdPartyBulkerCrew = eServiceOnlineGateway.Instance.GetThirdPartyBulkerCrewById(crewId);
                if (isUpdate)
                {

                    RigJobThirdPartyBulkerCrewSection rigJobThirdPartyBulkerCrewSection = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionsByProductHual(productHaul.Id);
                    rigJobThirdPartyBulkerCrewSection.ThirdPartyBulkerCrew = thirdPartyBulkerCrew;
                    eServiceOnlineGateway.Instance.UpdateRigJobThirdPartyCrewSection(rigJobThirdPartyBulkerCrewSection);

/*
                    List<RigJobThirdPartyBulkerCrewSection> rigJobThirdPartyBulkerCrewSections = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionsByProductHual(productHaul.Id);
                    foreach (RigJobThirdPartyBulkerCrewSection rigJobThirdPartyBulkerCrewSection in rigJobThirdPartyBulkerCrewSections)
                    {
                        rigJobThirdPartyBulkerCrewSection.ThirdPartyBulkerCrew = thirdPartyBulkerCrew;
                        eServiceOnlineGateway.Instance.UpdateRigJobThirdPartyCrewSection(rigJobThirdPartyBulkerCrewSection);
                    }
#1#
                }
                else
                {
                    RigJobSanjelCrewSection rigJobSanjelCrewSection = eServiceOnlineGateway.Instance.GetRigJobCrewSectionsByProductHaul(productHaul.Id);
                    RigJobThirdPartyBulkerCrewSection rigJobThirdPartyBulkerCrewSection = new RigJobThirdPartyBulkerCrewSection
                    {
                        RigJob = rigJobSanjelCrewSection.RigJob,
                        ThirdPartyBulkerCrew = thirdPartyBulkerCrew,
                        RigJobCrewSectionStatus = rigJobSanjelCrewSection.RigJobCrewSectionStatus,
                        ProductHaul = rigJobSanjelCrewSection.ProductHaul
                    };
                    eServiceOnlineGateway.Instance.CreateRigJobThirdPartyBulkerCrewSection(rigJobThirdPartyBulkerCrewSection);
                    eServiceOnlineGateway.Instance.DeleteRigJobCrewSection(rigJobSanjelCrewSection);
                }
            }
            else
            {
                SanjelCrew sanjelCrew = eServiceOnlineGateway.Instance.GetCrewById(crewId);
                if (isUpdate)
                {
                    RigJobSanjelCrewSection rigJobSanjelCrewSection = eServiceOnlineGateway.Instance.GetRigJobCrewSectionsByProductHaul(productHaul.Id);
                    rigJobSanjelCrewSection.SanjelCrew = sanjelCrew;
                    eServiceOnlineGateway.Instance.UpdateRigJobCrewSection(rigJobSanjelCrewSection);
                }
                else
                {
                    RigJobThirdPartyBulkerCrewSection rigJobThirdPartyBulkerCrewSection = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionsByProductHual(productHaul.Id);
                    RigJobSanjelCrewSection rigJobSanjelCrewSection = new RigJobSanjelCrewSection
                    {
                        RigJob = rigJobThirdPartyBulkerCrewSection.RigJob,
                        SanjelCrew = sanjelCrew,
                        RigJobCrewSectionStatus = rigJobThirdPartyBulkerCrewSection.RigJobCrewSectionStatus,
                        ProductHaul = rigJobThirdPartyBulkerCrewSection.ProductHaul
                    };
                    eServiceOnlineGateway.Instance.CreateRigJobCrewSection(rigJobSanjelCrewSection);
                    eServiceOnlineGateway.Instance.DeleteRigJobThirdPartyBulkerCrewSection(rigJobThirdPartyBulkerCrewSection.Id);
                }
            }
        }*/

        /*
        private static void UpdateSchedule(ProductHaul originalProductHaul,int crewId, bool originalHaulIsThirdParty)
        {
            UpdateRigJobCrewSection(originalProductHaul, crewId, originalHaulIsThirdParty);
        }
        */

        public static void UpdateCrewScheduleByProductHaul(ProductHaul productHaul)
        {
            if (!productHaul.IsThirdParty)
            {
                RigJobSanjelCrewSection rigJobSanjelCrewSection =
                    eServiceOnlineGateway.Instance.GetRigJobCrewSectionsByProductHaul(productHaul.Id);
                if (rigJobSanjelCrewSection != null)
                {
                    SanjelCrewSchedule originalCrewSchedule =
                        eServiceOnlineGateway.Instance.GetSanjelCrewSchedulesByRigJobSanjelCrewSection(
                            rigJobSanjelCrewSection);
                    if (originalCrewSchedule != null)
                    {
                        originalCrewSchedule =
                            eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(originalCrewSchedule.Id, true);
                        originalCrewSchedule.StartTime = productHaul.EstimatedLoadTime;
                        originalCrewSchedule.EndTime =
                            GetScheduleEndTime(productHaul, rigJobSanjelCrewSection.RigJob.Id);
                        ScheduleProcess.UpdateSanjelCrewSchedule(originalCrewSchedule);
                    }
                }
            }
            else
            {
                RigJobThirdPartyBulkerCrewSection rigJobThirdPartyBulkerCrewSection =
                    eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionsByProductHual(
                        productHaul.Id);
                if (rigJobThirdPartyBulkerCrewSection != null)
                {
                    ThirdPartyBulkerCrewSchedule originalThirdPartyBulkerCrewSchedule =
                        eServiceOnlineGateway.Instance.GetThirdPartyCrewScheduleByRigJobThirdPartyBulkerCrewSection(
                            rigJobThirdPartyBulkerCrewSection);
                    if (originalThirdPartyBulkerCrewSchedule != null)
                    {
                        originalThirdPartyBulkerCrewSchedule.StartTime = productHaul.EstimatedLoadTime;
                        originalThirdPartyBulkerCrewSchedule.EndTime = GetScheduleEndTime(productHaul,
                            rigJobThirdPartyBulkerCrewSection.RigJob.Id);
                        eServiceOnlineGateway.Instance.UpdateThirdPartyCrewSchedule(
                            originalThirdPartyBulkerCrewSchedule);
                    }
                }
            }
        }        

        private static DateTime GetScheduleEndTime(ProductHaul productHaul,int rigJobId)
        {
            DateTime endTime;
            if (productHaul.IsGoWithCrew)
            {
                endTime = GetRigJobAssignCrewEndTime(rigJobId);
                if (endTime == DateTime.MinValue)
                {
                    productHaul.EstimatedTravelTime = 8;
                    endTime = productHaul.EstimatedLoadTime.AddHours(8);
                }
                else
                {
                    productHaul.EstimatedTravelTime = (endTime - productHaul.EstimatedLoadTime).TotalHours;
                }
            }
            else
            {
                endTime = productHaul.EstimatedLoadTime.AddHours(productHaul.EstimatedTravelTime);
            }

            return endTime;
        }

        /*
        public static void UpdateHaulLoadAndUnitSections(ProductHaul productHaul, ProductHaulLoad productHaulLoad, int callSheetId,int rigJobId)
        {
            eServiceOnlineGateway.Instance.UpdateProductHaul(productHaul);
            eServiceOnlineGateway.Instance.UpdateProductHaulLoad(productHaulLoad);
    
            UpdateProductLoadSection(productHaul, productHaulLoad);

            if (productHaul.IsThirdParty)
            {
                UpdateThirdPartyUnitSections(productHaul);
                UpdateThirdPartyCrewScheduleJobInfo(productHaul);
            }
            else
            {
                UpdateUnitSections(productHaul);
                UpdateCrewScheduleJobInfo(productHaul);
            }
        }
        */

        /*public static void UpdateCrewScheduleJobInfo(ProductHaul productHaul)
        {

            SanjelCrewSchedule crewSchedule = eServiceOnlineGateway.Instance.GetCrewScheduleById(productHaul?.Schedule?.Id ?? 0);
            if (crewSchedule!=null)
            {
                if (productHaul != null)
                {
                    productHaul.ProductHaulLoads = eServiceOnlineGateway.Instance.GetProductHaulLoadsByProductHual(productHaul);
                   
                    crewSchedule.Description = UpdateCrewScheduleBuildJobInfo(productHaul);
                    eServiceOnlineGateway.Instance.UpdateSanjelCrewSchedule(crewSchedule);
                }
            }
        }

        public static void UpdateThirdPartyCrewScheduleJobInfo(ProductHaul productHaul)
        {
            ThirdPartyBulkerCrewSchedule thirdPartyBulkerCrewSchedule = eServiceOnlineGateway.Instance.GetThirdPartyCrewScheduleById(productHaul?.Schedule?.Id ?? 0);
            if (thirdPartyBulkerCrewSchedule!=null)
            {
                if (productHaul!=null)
                {
                    productHaul.ProductHaulLoads = eServiceOnlineGateway.Instance.GetProductHaulLoadsByProductHual(productHaul);
                    thirdPartyBulkerCrewSchedule.Description = UpdateCrewScheduleBuildJobInfo(productHaul);
                    eServiceOnlineGateway.Instance.UpdateThirdPartyCrewSchedule(thirdPartyBulkerCrewSchedule);
                }
            }
        }

        public static ProductHaul CreateProductHaulAndUpdateHaulLoad(ProductHaul productHaul, ProductHaulLoad productHaulLoad, int callSheetId,int crewId)
        {
            eServiceOnlineGateway.Instance.CreateProductHaul(productHaul);
            productHaulLoad.ProductHaul = productHaul;
            productHaulLoad.ProductHaulLoadLifeStatus =ProductHaulLoadStatus.Scheduled;
            eServiceOnlineGateway.Instance.CreateProductHaulLoad(productHaulLoad);
            UpdateProductLoadSection(productHaul, productHaulLoad);
            CreateUnitSectionOrThirdPartyUnitSection(productHaul, callSheetId, crewId);

            return productHaul;
        }*/

        private static void UpdateProductLoadSection(ProductHaul productHaul, ProductHaulLoad productHaulLoad)
        {
            List<ProductLoadSection> productLoadSections = eServiceOnlineGateway.Instance.GetProductLoadSectionsByProductLoadId(productHaulLoad.Id);

            foreach (ProductLoadSection productLoadSection in productLoadSections)
            {
                eServiceOnlineGateway.Instance.DeleteProductLoadSection(productLoadSection);
            }

            foreach (ProductLoadSection productLoadSection in productHaulLoad.AllProductLoadList)
            {
                productLoadSection.OwnerId = productHaulLoad.Id;
                productLoadSection.ProductHaulLoad = productHaulLoad;
                eServiceOnlineGateway.Instance.CreateProductLoadSection(productLoadSection);
            }
        }


        /*
        public static void DeleteHaulLoadAndUnitSections(int productHaulLoadId, int callSheetId,int rigJobId)
        {
            ProductHaulLoad productHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productHaulLoadId);

            if (productHaulLoad!=null)
            {              
                ProductHaul productHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulLoad.ProductHaul.Id);
//                DeleteAllProductLoadSectionsByLoadId(productHaulLoad.Id);
                eServiceOnlineGateway.Instance.DeleteProductHaulLoad(productHaulLoad, true);
                if (productHaul != null)
                {
                    UpdateThridUnitSectionsOrUnitSections(callSheetId, productHaul, productHaulLoad);

                    //原来的productHaul对应的crewSchedule更新jobInfo
                    productHaul.ProductHaulLoads = eServiceOnlineGateway.Instance.GetProductHaulLoadsByProductHual(productHaul);
                    RigJob rigJob = eServiceOnlineGateway.Instance.GetRigJobById(rigJobId);
                    if (productHaul.IsThirdParty)
                    {
                        UpdateThirdPartyCrewScheduleJobInfo(productHaul);
                    }
                    else
                    {
                        UpdateCrewScheduleJobInfo(productHaul);
                    }
                }

            }
        }
        */

        /*private static void UpdateThridUnitSectionsOrUnitSections(int callSheetId, ProductHaul productHaul, ProductHaulLoad productHaulLoad)
        {
            List<ProductHaulLoad> productHaulLoads = eServiceOnlineGateway.Instance.GetProductHaulLoadsByProductHual(productHaul).FindAll(s => s.CallSheetNumber == productHaulLoad.CallSheetNumber).ToList();
            if (productHaulLoads.Count < 1)
            {
                if (productHaul.IsThirdParty)
                {
                    ThirdPartyUnitSection thirdPartyUnitSection = eServiceOnlineGateway.Instance.GetThirdPartyUnitSectionByCallSheetAndProductHaul(new Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch.CallSheet{Id = callSheetId}, new ProductHaul{Id = productHaul.Id});
                    eServiceOnlineGateway.Instance.DeleteThirdPartyUnitSection(thirdPartyUnitSection);
                    UpdateThirdPartyUnitSections(productHaul);
                }
                else
                {
                    UnitSection unitSection = eServiceOnlineGateway.Instance.GetUnitSectionByCallSheetAndProductHaul(new Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch.CallSheet { Id = callSheetId }, new ProductHaul { Id = productHaul.Id });
                    eServiceOnlineGateway.Instance.DeleteUnitSection(unitSection);
                    UpdateUnitSections(productHaul);
                }
            }
            else
            {
                if (productHaul.IsThirdParty)
                {
                    UpdateThirdPartyUnitSections(productHaul);
                }
                else
                {
                    UpdateUnitSections(productHaul);
                }
            }            
        }*/

        /*
        public static void DeleteAllProductLoadSectionsByLoadId(int productHaulLoadId)
        {
            List<ProductLoadSection> productLoadSections = eServiceOnlineGateway.Instance.GetProductLoadSectionsByProductLoadId(productHaulLoadId);
            foreach (ProductLoadSection productLoadSection in productLoadSections)
            {
                eServiceOnlineGateway.Instance.DeleteProductLoadSection(productLoadSection);
            }
        }
        */

        #endregion

        #region Company Operation

        public static void UpdateCompanyShortName(int rigJobId, int clientCompanyId, string companyShortName)
        {
            RigJob rigJob = eServiceOnlineGateway.Instance.GetRigJobById(rigJobId);
            if (rigJob != null)
            {
                rigJob.ClientCompanyShortName = companyShortName;
                eServiceOnlineGateway.Instance.UpdateRigJob(rigJob);
            }

            ClientCompany company = eServiceOnlineGateway.Instance.GetClientCompanyById(clientCompanyId);
            if (company != null)
            {
                company.ShortName = companyShortName;
                eServiceOnlineGateway.Instance.UpdateClientCompany(company);
            }

        }

        #endregion

        /*public static RigJobSanjelCrewSection CreateRigJobSanjelCrewSection(int crewId, int rigJobId, ProductHaul productHaul, ProductHaulLoad productHaulLoad = null)
        {
            RigJobSanjelCrewSection rigJobSanjelCrewSection = eServiceOnlineGateway.Instance.GetRigJobCrewSection(rigJobId, crewId);
            if (rigJobSanjelCrewSection == null)
            {
                RigJobSanjelCrewSection section = new RigJobSanjelCrewSection
                {
                    RigJob = eServiceOnlineGateway.Instance.GetRigJobById(rigJobId),
                    SanjelCrew = eServiceOnlineGateway.Instance.GetCrewById(crewId),
                    RigJobCrewSectionStatus = RigJobCrewSectionStatus.Scheduled,
                    ProductHaul = productHaul,
                    Description = productHaulLoad==null? productHaul.Description:productHaulLoad.Description
                };
                eServiceOnlineGateway.Instance.CreateRigJobCrewSection(section);

                return section;
            }

            return null;
        }

        public static RigJobThirdPartyBulkerCrewSection CreateRigJobThirdPartyCrewSection(int crewId, int rigJobId, ProductHaul productHaul, ProductHaulLoad productHaulLoad = null)
        {
            RigJobThirdPartyBulkerCrewSection rigJobThirdPartyBulkerCrewSection = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionsByProductHual(productHaul.Id);
            if (rigJobThirdPartyBulkerCrewSection == null)
            {
                RigJobThirdPartyBulkerCrewSection section = new RigJobThirdPartyBulkerCrewSection
                {
                    RigJob = eServiceOnlineGateway.Instance.GetRigJobById(rigJobId),
                    ThirdPartyBulkerCrew = eServiceOnlineGateway.Instance.GetThirdPartyBulkerCrewById(crewId),
                    RigJobCrewSectionStatus = RigJobCrewSectionStatus.Scheduled,
                    ProductHaul = productHaul,
                    Description = productHaulLoad == null ? productHaul.Description : productHaulLoad.Description
                };
                eServiceOnlineGateway.Instance.CreateRigJobThirdPartyBulkerCrewSection(section);

                return section;
            }

            return null;
        }

        public static void CreateRigJobCrewSection(bool isThirdParty, int crewId, int rigJobId, ProductHaul productHaul)
        {
            if (isThirdParty)
            {
                RigJobThirdPartyBulkerCrewSection rigJobThirdPartyBulkerCrewSection = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSection(rigJobId, crewId);
                if (rigJobThirdPartyBulkerCrewSection == null)
                {
                    RigJobThirdPartyBulkerCrewSection section = new RigJobThirdPartyBulkerCrewSection
                    {
                        RigJob = eServiceOnlineGateway.Instance.GetRigJobById(rigJobId),
                        ThirdPartyBulkerCrew = eServiceOnlineGateway.Instance.GetThirdPartyBulkerCrewById(crewId),
                        RigJobCrewSectionStatus = RigJobCrewSectionStatus.Scheduled,
                        ProductHaul = productHaul
                    };
                    eServiceOnlineGateway.Instance.CreateRigJobThirdPartyBulkerCrewSection(section);
                }
            }
            else
            {
                RigJobSanjelCrewSection rigJobSanjelCrewSection = eServiceOnlineGateway.Instance.GetRigJobCrewSection(rigJobId, crewId);
                if (rigJobSanjelCrewSection == null)
                {
                    RigJobSanjelCrewSection section = new RigJobSanjelCrewSection
                    {
                        RigJob = eServiceOnlineGateway.Instance.GetRigJobById(rigJobId),
                        SanjelCrew = eServiceOnlineGateway.Instance.GetCrewById(crewId),
                        RigJobCrewSectionStatus = RigJobCrewSectionStatus.Scheduled,
                        ProductHaul = productHaul
                    };
                    eServiceOnlineGateway.Instance.CreateRigJobCrewSection(section);
                }
            }
        }
        */


        public static RigJobThirdPartyBulkerCrewSection CreateRigJobThirdPartyBulkerCrewSection(ThirdPartyBulkerCrew crew, ProductHaul productHaul, RigJob rigJob)
        {
            RigJobThirdPartyBulkerCrewSection section = new RigJobThirdPartyBulkerCrewSection
            {
                RigJob = rigJob,
                ThirdPartyBulkerCrew = crew,
                RigJobCrewSectionStatus = RigJobCrewSectionStatus.Scheduled,
                ProductHaul = productHaul
            };
            eServiceOnlineGateway.Instance.CreateRigJobThirdPartyBulkerCrewSection(section);
            return section;
        }

        /*//CJ (2018/07/25) Create producthaulload based on producthaulId and update unit section or third party unit section
        public static void CreateProductLoadAndUpdateUnitSection(ProductHaulLoad productHaulLoad,int productHaulId, int callSheetId,int rigJobId)
        {
            ProductHaul productHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);

            productHaulLoad.ProductHaul = productHaul;
            productHaulLoad.ProductHaulLoadLifeStatus =ProductHaulLoadStatus.Scheduled;
            eServiceOnlineGateway.Instance.CreateProductHaulLoad(productHaulLoad);
            UpdateProductLoadSection(productHaul, productHaulLoad);            
            if (productHaul!=null)
            {
                if (productHaul.IsThirdParty)
                {
                    ThirdPartyUnitSection thirdPartyUnitSection = eServiceOnlineGateway.Instance.GetThirdPartyUnitSectionByCallSheetAndProductHaul(new Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch.CallSheet { Id = callSheetId }, new ProductHaul { Id = productHaulId });
                    if (thirdPartyUnitSection==null)
                    {
                        ThirdPartyBulkerCrewSchedule thirdPartyBulkerCrewSchedule = eServiceOnlineGateway.Instance.GetThirdPartyCrewScheduleById(productHaul.Schedule?.Id ?? 0);
                        CreateRigJobCrewSection(productHaul.IsThirdParty, thirdPartyBulkerCrewSchedule.ThirdPartyBulkerCrew.Id, rigJobId, productHaul);
                        CreateUnitSectionOrThirdPartyUnitSection(productHaul, callSheetId, thirdPartyBulkerCrewSchedule.ThirdPartyBulkerCrew.Id);
                    }

                    UpdateThirdPartyCrewScheduleJobInfo(productHaul);
                    UpdateThirdPartyUnitSections(productHaul);
                }
                else
                {
                    UnitSection unitSection = eServiceOnlineGateway.Instance.GetUnitSectionByCallSheetAndProductHaul(new Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch.CallSheet { Id = callSheetId }, new ProductHaul { Id = productHaulId });
                    if (unitSection==null)
                    {
                        SanjelCrewSchedule schedule = eServiceOnlineGateway.Instance.GetCrewScheduleById(productHaul.Schedule?.Id ?? 0);
                        CreateRigJobCrewSection(productHaul.IsThirdParty, schedule.SanjelCrew.Id, rigJobId, productHaul);
                        CreateUnitSectionOrThirdPartyUnitSection(productHaul, callSheetId, schedule.SanjelCrew.Id);
                    }
                    
                    UpdateCrewScheduleJobInfo(productHaul);
                    UpdateUnitSections(productHaul);
                }
            }

        }*/

        public static DateTime GetCallCrewTime(int rigJobId)
        {
            DateTime callCrewTime = DateTime.MinValue;
            RigJob rigJob = eServiceOnlineGateway.Instance.GetRigJobById(rigJobId);
            if (rigJob != null)
            {
                ICallSheetMicroService callSheetMicroService = ServiceFactory.Instance.GetService(typeof(ICallSheetMicroService)) as ICallSheetMicroService;
                if (callSheetMicroService == null) throw new Exception("callSheetMicroService can not be null.");
                CallSheet callSheet = callSheetMicroService.GetCallSheetById(rigJob.CallSheetId);
                callCrewTime = callSheet.Header.HeaderDetails.CallInformation.CallCrewDateTime;
            }

            return callCrewTime;
        }

        public static int AssignACrew(int crewId, int rigJobId, DateTime callCrewTime, int duration)
        {
            int jobDuration = 0;
            SanjelCrew crew = eServiceOnlineGateway.Instance.GetCrewById(crewId);
            RigJob rigJob = eServiceOnlineGateway.Instance.GetRigJobById(rigJobId);
            //check  productHaul schedule
            //This is not for product haul
            //            InspectProductHaulScheduleByAssignCrew(rigJobId, callCrewTime.AddHours(duration));

            //create rigJobCrewSection
            int callSheetId = 0;
            if (rigJob != null)
            {
                jobDuration = duration == 0 ? (rigJob.JobDuration == 0 ? 360 : rigJob.JobDuration) : duration * 60;
                var endTime = rigJob.JobDateTime.AddMinutes(jobDuration);
                var startTime = rigJob.JobLifeStatus == JobLifeStatus.Pending
                    ? rigJob.JobDateTime
                    : rigJob.CallCrewTime;
                CreateCrewSectionAndSchedules(startTime, endTime, crew, rigJob, RigJobCrewSectionStatus.Scheduled, null, null);

                if (rigJob.JobDuration != jobDuration)
                {
                    rigJob.JobDuration = jobDuration;
                    eServiceOnlineGateway.Instance.UpdateRigJob(rigJob);
                }


                //Update crew working district
//                AssignCrewToAnotherDistrict(crew, rigJob.ServicePoint.Id);

//                     callSheetId = rigJob.CallSheetId;
//                CreatePumperUnitSection(crew.Id, callSheetId);
            }
            //CreateUnitSection(crew.Id, callSheetId, false);

            return jobDuration;
        }

        public static void InspectProductHaulScheduleByAssignCrew(int rigJobId, DateTime startTime, DateTime endTime)
        {
            List<RigJobSanjelCrewSection> rigJobSanjelCrewSections = eServiceOnlineGateway.Instance.GetRigJobCrewSectionByRigJob(rigJobId).FindAll(s=>s.ProductHaul.Id!=0);

            foreach (var rigJobSanjelCrewSection in rigJobSanjelCrewSections)
            {
                SanjelCrewSchedule schedule = eServiceOnlineGateway.Instance.GetCrewScheduleByJobCrewSection(rigJobSanjelCrewSection.Id);
                if (schedule==null) continue;
                ProductHaul productHaul = eServiceOnlineGateway.Instance.GetProductHaulByCrewScheduleId(schedule.Id).FirstOrDefault();
                if (productHaul==null) continue;
                if (!productHaul.IsGoWithCrew) continue;

                productHaul.EstimatedTravelTime = (endTime-productHaul.EstimatedLoadTime).TotalHours;
                eServiceOnlineGateway.Instance.UpdateProductHaul(productHaul);
                UpdateSanjelCrewSchedule(startTime, endTime, schedule);
            }
            List<RigJobThirdPartyBulkerCrewSection> rigJobThirdPartyBulkerCrewSections = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionByRigJob(rigJobId);
            foreach (var rigJobThirdPartyBulkerCrewSection in rigJobThirdPartyBulkerCrewSections)
            {
                ThirdPartyBulkerCrewSchedule schedule = eServiceOnlineGateway.Instance.GetThirdPartyBulkerCrewSchedule(rigJobThirdPartyBulkerCrewSection.Id);
                if (schedule == null) continue;
                ProductHaul productHaul = eServiceOnlineGateway.Instance.GetProductHaulByCrewScheduleId(schedule.Id).FirstOrDefault();
                if (productHaul == null) continue;
                if (!productHaul.IsGoWithCrew) continue;
                productHaul.EstimatedTravelTime = (endTime - productHaul.EstimatedLoadTime).TotalHours;
                eServiceOnlineGateway.Instance.UpdateProductHaul(productHaul);
                schedule.EndTime = startTime;
                schedule.EndTime = endTime;
                eServiceOnlineGateway.Instance.UpdateThirdPartyCrewSchedule(schedule);    
            }
        }

        public static double GetDuration(int scheduleId, int rigJobId, bool isThridParty)
        {
            if (isThridParty)
            {               
                ThirdPartyBulkerCrewSchedule thirdPartyBulkerCrewSchedule = eServiceOnlineGateway.Instance.GetThirdPartyCrewScheduleById(scheduleId);
                if (thirdPartyBulkerCrewSchedule!=null)
                {
                    return (thirdPartyBulkerCrewSchedule.EndTime - thirdPartyBulkerCrewSchedule.StartTime).TotalHours;
                }
            }
            else
            {

                SanjelCrewSchedule crewSchedule = eServiceOnlineGateway.Instance.GetCrewScheduleByJobCrewSection(scheduleId);
                if (crewSchedule != null)
                {
                    return (crewSchedule.EndTime - crewSchedule.StartTime).TotalHours;
                }
            }
            
            return 0;
        }

        public static Schedule CreateCrewSectionAndSchedules(DateTime callCrewTime, DateTime scheduleEndTime,
	        SanjelCrew crew, RigJob rigJob, RigJobCrewSectionStatus rigJobCrewSectionStatus,
	        ProductHaul productHaul = null, string loggedUser=null)
        {
	        var rigJobSanjelCrewSection = ProductHaulProcess.CreateRigJobSanjelCrewSection(crew, productHaul, rigJob, rigJobCrewSectionStatus,loggedUser);

            Schedule crewSchedule = CreateSchedules(callCrewTime, scheduleEndTime, crew, rigJob, productHaul, rigJobSanjelCrewSection, loggedUser);

            return crewSchedule;
        }

        public static Schedule CreateSchedules(DateTime startDateTime, DateTime scheduleEndTime, SanjelCrew crew,
	        RigJob rigJob, ProductHaul productHaul, RigJobSanjelCrewSection rigJobCrewSection, string loggedUser)
        {
            //create crew schedule
            SanjelCrewSchedule crewSchedule = new SanjelCrewSchedule();
            crewSchedule.Name = productHaul!=null? "Product Haul": rigJob.Rig?.Name +"/" + rigJob.JobType?.Name;
            crewSchedule.StartTime = startDateTime;
            crewSchedule.EndTime = scheduleEndTime;
            crewSchedule.RigJobSanjelCrewSection = rigJobCrewSection;
            //Nov 6, 2023 AW P45_Q4_105: Prevent null rigjob caused exception
            crewSchedule.WorkingServicePoint = rigJob?.ServicePoint;
            crewSchedule.SanjelCrew = crew;
            crewSchedule.Description = productHaul!=null?productHaul.Description:BuildRigJobInfo(rigJob);
            crewSchedule.Type = CrewScheduleType.Assigned;
            crewSchedule.ModifiedUserName = loggedUser;

            ScheduleProcess.CreateCrewSchedule(crewSchedule);

            return crewSchedule;
        }



        public static void AssignCrewToAnotherDistrict(SanjelCrew crew, int workingDistrictId)
        {
            if (crew != null)
            {
                if (crew.WorkingServicePoint == null || crew.WorkingServicePoint.Id != workingDistrictId)
                {
                    ServicePoint workingServicePoint =
                        eServiceOnlineGateway.Instance.GetServicePointById(workingDistrictId);
                    crew.WorkingServicePoint = workingServicePoint;
                    eServiceOnlineGateway.Instance.UpdateCrew(crew);
                }
            }
        }

        public static Schedule CreateThridPartyCrewScheduleAndSection(DateTime callCrewTime, DateTime scheduleEndTime, ThirdPartyBulkerCrew crew, RigJob rigJob, RigJobCrewSectionStatus rigJobCrewSectionStatus, string loggedUser, ProductHaul productHaul =null,bool isGoWithCrew =true)
        {
            RigJobThirdPartyBulkerCrewSection rigJobThirdPartyCrewSection = new RigJobThirdPartyBulkerCrewSection
            {
                RigJob = rigJob,
                ThirdPartyBulkerCrew = crew,
                RigJobCrewSectionStatus = rigJobCrewSectionStatus,
                ProductHaul = productHaul,
                ModifiedUserName = loggedUser
            };
            eServiceOnlineGateway.Instance.CreateRigJobThirdPartyBulkerCrewSection(rigJobThirdPartyCrewSection);
            ThirdPartyBulkerCrewSchedule crewSchedule = CreateThirdPartyBulkerCrewSchedule(callCrewTime, scheduleEndTime, crew, rigJob, productHaul, rigJobThirdPartyCrewSection);

            return crewSchedule;
        }

        public static ThirdPartyBulkerCrewSchedule CreateThirdPartyBulkerCrewSchedule(DateTime callCrewTime, DateTime scheduleEndTime,
            ThirdPartyBulkerCrew crew, RigJob rigJob, ProductHaul productHaul,
            RigJobThirdPartyBulkerCrewSection rigJobThirdPartyBulkerCrewSection)
        {
            ThirdPartyBulkerCrewSchedule crewSchedule = new ThirdPartyBulkerCrewSchedule();
            //Jan 23, 2024 zhangyuan 275_PR_ThirdPartyError: Modify rigJob is null Exception
            crewSchedule.Name = (rigJob==null||rigJob.Id==0)?"Product Haul": rigJob.Rig?.Name +"/" + rigJob.JobType?.Name;
            crewSchedule.StartTime = callCrewTime;
            crewSchedule.EndTime = scheduleEndTime;
            crewSchedule.RigJobThirdPartyBulkerCrewSection = rigJobThirdPartyBulkerCrewSection;
            crewSchedule.ThirdPartyBulkerCrew = crew;
            crewSchedule.Type = CrewScheduleType.Assigned;
            crewSchedule.Description = productHaul!=null?productHaul.Description:BuildRigJobInfo(rigJob);

            eServiceOnlineGateway.Instance.InsertThirdPartyBulkerCrewSchedule(crewSchedule);
            return crewSchedule;
        }

        public static Schedule CreateSchedule(int crewId, bool isThirdParty, DateTime estimatedLoadTime, int rigJobId,
	        bool isGoWithCrew, DateTime scheduleEndTime, ProductHaul productHaul, string loggedUser)
        {
            DateTime startTime = DateTime.MinValue;
            RigJob rigJob = eServiceOnlineGateway.Instance.GetRigJobById(rigJobId);
            startTime = estimatedLoadTime;

            if (!isThirdParty)
            {
                SanjelCrew crew = eServiceOnlineGateway.Instance.GetCrewById(crewId, true);
                productHaul.Crew = crew;
                ProductHaulProcess.SetDriverAndBulk(productHaul, crew);
                return RigBoardProcess.CreateCrewSectionAndSchedules(startTime, scheduleEndTime, crew, rigJob, RigJobCrewSectionStatus.Scheduled, productHaul, loggedUser);
            }
            else
            {
                ThirdPartyBulkerCrew thirdPartyBulkerCrew = eServiceOnlineGateway.Instance.GetThirdPartyBulkerCrewById(crewId);
                productHaul.Crew = thirdPartyBulkerCrew;
                ProductHaulProcess.SetThirdPartyDriver(productHaul, thirdPartyBulkerCrew);
                return RigBoardProcess.CreateThridPartyCrewScheduleAndSection(startTime, scheduleEndTime, thirdPartyBulkerCrew, rigJob, RigJobCrewSectionStatus.Scheduled, loggedUser, productHaul, isGoWithCrew);
            }
        }

        private static List<SanjelCrewTruckUnitSection> GetCrewTruckUnitSection(int id)
        {
            List<SanjelCrewTruckUnitSection> crewTruckUnitSections = eServiceOnlineGateway.Instance.GetTruckUnitSectionsByCrew(id);
            foreach (var crewTruckUnitSection in crewTruckUnitSections)
            {
                crewTruckUnitSection.TruckUnit = eServiceOnlineGateway.Instance.GetTruckUnitById(crewTruckUnitSection.TruckUnit.Id);
            }
            return crewTruckUnitSections;
        }

        public static List<SanjelCrewWorkerSection> GetCrewWorkerSections(int id)
        {
            List<SanjelCrewWorkerSection> crewWorkerSections = eServiceOnlineGateway.Instance.GetWorkerSectionsByCrew(id);
            foreach (var crewWorkerSection in crewWorkerSections)
            {
                crewWorkerSection.Worker = eServiceOnlineGateway.Instance.GetEmployeeById(crewWorkerSection.Worker.Id);
            }
            return crewWorkerSections;
        }
        public static void CallAllCrew(int rigJobId)
        {
            List<RigJobSanjelCrewSection> rigJobCrewSections = eServiceOnlineGateway.Instance.GetRigJobCrewSectionByRigJob(rigJobId);
            UpdateRigJobSanjelCrewSectionsStatus(rigJobCrewSections, RigJobCrewSectionStatus.Called);
            List<RigJobThirdPartyBulkerCrewSection> rigJobThirdPartyBulkerCrewSections = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionByRigJob(rigJobId);
            UpdateRigJobThirdPartyBulkerCrewSectionsStatus(rigJobThirdPartyBulkerCrewSections, RigJobCrewSectionStatus.Called);
        }

        /*
        public static bool CreateUnitSection(int crewId, int callsheetId, bool isThirdParty, bool isGoWithCrew = true,
            int productHaulId = 0)
        {
            if (isThirdParty)
            {
                 CreateUnitSectionFromThirdPartyCrew(callsheetId, crewId, productHaulId);
                 return true;
            }
            else
            {
                 List<SanjelCrewTruckUnitSection> crewTruckUnitSections = GetCrewTruckUnitSection(crewId);
                 List<SanjelCrewWorkerSection> crewWorkerSections = GetCrewWorkerSections(crewId);
                     CreateUnitSectionFromOtherCrew(callsheetId, productHaulId, isGoWithCrew, crewTruckUnitSections, crewWorkerSections, crewId);
                     return true;
            }           
            return false;
        }

        */
        /*
        private static UnitSection InitUnitSection(int productHaulId,int callsheetId,bool isGoWithCrew)
        {
            UnitSection unitSection = new UnitSection();
            unitSection.RootId =callsheetId;
            if (productHaulId != 0)
            {
                unitSection.ProductHaulId = productHaulId;
                unitSection.HaulDescription = BuildUnitSectionComments(eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId));
                unitSection.IsProductHaul = !isGoWithCrew;
            }             
            return unitSection;
        }
        */

        /*
        private static void CreateUnitSectionFromOtherCrew(int callsheetId, int productHaulId, bool isGoWithCrew,
            List<SanjelCrewTruckUnitSection> crewTruckUnitSections, List<SanjelCrewWorkerSection> crewWorkerSections,
            int crewId)
        {
            //A crew is limited to one unit set (may have tractor and trailer), or one pickup, or one single pumper


            UnitSection unitSection = InitUnitSection(productHaulId, callsheetId, isGoWithCrew);
            unitSection.CrewId = crewId;

            if (crewTruckUnitSections.Count == 1)
            {
                SanjelCrewTruckUnitSection sanjelCrewTruckUnitSection = crewTruckUnitSections.ElementAtOrDefault(0);
                if (sanjelCrewTruckUnitSection != null && sanjelCrewTruckUnitSection.TruckUnit != null)
                {
                    unitSection.TruckUnit = new Sanjel.Common.BusinessEntities.Lookup.TruckUnit();
                    unitSection.TruckUnit.Id = sanjelCrewTruckUnitSection.TruckUnit.Id;
                    unitSection.TruckUnit.UnitNumber = sanjelCrewTruckUnitSection.TruckUnit.UnitNumber;
                    unitSection.UnitMainType = new Sanjel.Common.BusinessEntities.Lookup.UnitMainType();
                    unitSection.UnitMainType.Id = sanjelCrewTruckUnitSection.TruckUnit.UnitMainType.Id;
                    unitSection.UnitMainType.Name = sanjelCrewTruckUnitSection.TruckUnit.UnitMainType.Name;
                    unitSection.UnitSubtype = new Sanjel.Common.BusinessEntities.Lookup.UnitSubtype();
                    unitSection.UnitSubtype.Id = sanjelCrewTruckUnitSection.TruckUnit.UnitSubType.Id;
                    unitSection.UnitSubtype.Name = sanjelCrewTruckUnitSection.TruckUnit.UnitSubType.Name;
                }

            }
            else if (crewTruckUnitSections.Count >1)
            {

                foreach (var sanjelCrewTruckUnitSection in crewTruckUnitSections)
                {
                    if (sanjelCrewTruckUnitSection.TruckUnit != null &&
                        sanjelCrewTruckUnitSection.TruckUnit.UnitMainType != null)
                    {
                        if (sanjelCrewTruckUnitSection.TruckUnit.UnitMainType.Id == 2 ||
                            sanjelCrewTruckUnitSection.TruckUnit.UnitMainType.Id == 5)
                        {
                            unitSection.TruckUnit = new Sanjel.Common.BusinessEntities.Lookup.TruckUnit();
                            unitSection.TruckUnit.Id = sanjelCrewTruckUnitSection.TruckUnit.Id;
                            unitSection.TruckUnit.UnitNumber = sanjelCrewTruckUnitSection.TruckUnit.UnitNumber;
                            unitSection.UnitMainType = new Sanjel.Common.BusinessEntities.Lookup.UnitMainType();
                            unitSection.UnitMainType.Id = sanjelCrewTruckUnitSection.TruckUnit.UnitMainType.Id;
                            unitSection.UnitMainType.Name = sanjelCrewTruckUnitSection.TruckUnit.UnitMainType.Name;
                            if (sanjelCrewTruckUnitSection.TruckUnit.UnitSubType != null)
                            {
                                unitSection.UnitSubtype = new Sanjel.Common.BusinessEntities.Lookup.UnitSubtype();
                                unitSection.UnitSubtype.Id = sanjelCrewTruckUnitSection.TruckUnit.UnitSubType.Id;
                                unitSection.UnitSubtype.Name = sanjelCrewTruckUnitSection.TruckUnit.UnitSubType.Name;
                            }
                        }
                        else
                        {
                            unitSection.TractorUnit = new Sanjel.Common.BusinessEntities.Lookup.TruckUnit();
                            unitSection.TractorUnit.Id = sanjelCrewTruckUnitSection.TruckUnit.Id;
                            unitSection.TractorUnit.UnitNumber = sanjelCrewTruckUnitSection.TruckUnit.UnitNumber;
                            unitSection.TractorMainType = new Sanjel.Common.BusinessEntities.Lookup.UnitMainType();
                            unitSection.TractorMainType.Id = sanjelCrewTruckUnitSection.TruckUnit.UnitMainType.Id;
                            unitSection.TractorMainType.Name = sanjelCrewTruckUnitSection.TruckUnit.UnitMainType.Name;
                            if (sanjelCrewTruckUnitSection.TruckUnit.UnitSubType != null)
                            {
                            unitSection.TractorSubtype = new Sanjel.Common.BusinessEntities.Lookup.UnitSubtype();
                            unitSection.TractorSubtype.Id = sanjelCrewTruckUnitSection.TruckUnit.UnitSubType.Id;
                            unitSection.TractorSubtype.Name = sanjelCrewTruckUnitSection.TruckUnit.UnitSubType.Name;
                                }
                        }
                    }
                }
            }

            if (crewWorkerSections.Count > 0)
            {

                SanjelCrewWorkerSection operator1CrewWorkerSection = crewWorkerSections.ElementAtOrDefault(0);
                if (operator1CrewWorkerSection != null && operator1CrewWorkerSection.Worker != null)
                {
                    unitSection.Operator1 = new Sanjel.Common.BusinessEntities.Reference.Employee();
                    unitSection.Operator1.Id = operator1CrewWorkerSection.Worker.Id;
                    unitSection.Operator1.LastName = operator1CrewWorkerSection.Worker.LastName;
                    unitSection.Operator1.MiddleName = operator1CrewWorkerSection.Worker.MiddleName;
                    unitSection.Operator1.FirstName = operator1CrewWorkerSection.Worker.FirstName;
                }

                if (crewWorkerSections.Count > 1)
                {
                    SanjelCrewWorkerSection operator2CrewWorkerSection = crewWorkerSections.ElementAtOrDefault(1);
                    if (operator2CrewWorkerSection != null && operator2CrewWorkerSection.Worker != null)
                    {
                        unitSection.Operator2 = new Sanjel.Common.BusinessEntities.Reference.Employee();
                        unitSection.Operator2.Id = operator2CrewWorkerSection.Worker.Id;
                        unitSection.Operator2.LastName = operator2CrewWorkerSection.Worker.LastName;
                        unitSection.Operator2.MiddleName = operator2CrewWorkerSection.Worker.MiddleName;
                        unitSection.Operator2.FirstName = operator2CrewWorkerSection.Worker.FirstName;
                    }
                }
            }

            eServiceOnlineGateway.Instance.CreateUnitSection(unitSection);

            /*
                        if (crewTruckUnitSections.Count % 2 == 0)
                        {
                            for (int i = 0; i < crewTruckUnitSections.Count; i += 2)
                            {
                                CreateUnitSectionFromBulkerCrew(callsheetId,productHaulId,isGoWithCrew, crewTruckUnitSections, crewWorkerSections, i);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < crewTruckUnitSections.Count - 1; i += 2)
                            {
                                CreateUnitSectionFromBulkerCrew(callsheetId,productHaulId, isGoWithCrew,crewTruckUnitSections, crewWorkerSections, i);
                            }
                            CreateLastUnitSection(callsheetId, productHaulId, isGoWithCrew, crewTruckUnitSections, crewWorkerSections);      
                        }
            #1#
        }
        */


        /*
        private static void CreateUnitSectionFromThirdPartyCrew(int callsheetId,int crewId,int productHaulId)
        {
            
            ProductHaul productHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);
            ThirdPartyBulkerCrew thirdPartyBulkerCrew = eServiceOnlineGateway.Instance.GetThirdPartyBulkerCrewById(crewId);

            ThirdPartyUnitSection thirdPartyUnitSection = new ThirdPartyUnitSection();
            thirdPartyUnitSection.ProductHaulId = productHaulId;
            thirdPartyUnitSection.RootId = callsheetId;
            thirdPartyUnitSection.CrewId = crewId;
            thirdPartyUnitSection.SupplierCompanyName = thirdPartyBulkerCrew?.ContractorCompany?.Name;
            thirdPartyUnitSection.ThirdPartyUnitNumber = thirdPartyBulkerCrew?.ThirdPartyUnitNumber;
            thirdPartyUnitSection.SupplierContactName = thirdPartyBulkerCrew?.SupplierContactName;
            thirdPartyUnitSection.SupplierContactNumber = thirdPartyBulkerCrew?.SupplierContactNumber;
            if (productHaul!=null)
            {
                thirdPartyUnitSection.Description = BuildUnitSectionComments(productHaul);
            }
            eServiceOnlineGateway.Instance.CreateThirdPartyUnitSection(thirdPartyUnitSection);
        }
        */


        public static void WithdrawACrew(int rigJobId, int crewId, int jobCrewSectionStatusId)
        {
            RigJobSanjelCrewSection rigJobCrewSection = eServiceOnlineGateway.Instance.GetRigJobCrewSection(rigJobId, crewId, jobCrewSectionStatusId);
            RigJob rigJob = eServiceOnlineGateway.Instance.GetRigJobById(rigJobId);
            SanjelCrewSchedule sanjelCrewSchedule = eServiceOnlineGateway.Instance.GetCrewScheduleByJobCrewSection(rigJobCrewSection.Id);
            ScheduleProcess.DeleteSanjelCrewSchedule(sanjelCrewSchedule);
            eServiceOnlineGateway.Instance.DeleteRigJobCrewSection(rigJobCrewSection);
            Collection<UnitSection> unitSections = eServiceOnlineGateway.Instance.GetUnitSectionsByCrewId(crewId);
            if (unitSections != null && unitSections.Count > 0)
            {
                foreach (var unitSection in unitSections)
                {
                    if(unitSection.RootId == rigJob.CallSheetId)
                        eServiceOnlineGateway.Instance.DeleteUnitSection(unitSection);
                }
            }
        }

        public static void LogOnDuty(int rigJobId)
        {
            List<RigJobSanjelCrewSection> rigJobCrewSections = eServiceOnlineGateway.Instance.GetRigJobCrewSectionByRigJob(rigJobId);
            UpdateRigJobSanjelCrewSectionsStatus(rigJobCrewSections, RigJobCrewSectionStatus.LogOnDuty);
            List<RigJobThirdPartyBulkerCrewSection> rigJobThirdPartyBulkerCrewSections = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionByRigJob(rigJobId);
            UpdateRigJobThirdPartyBulkerCrewSectionsStatus(rigJobThirdPartyBulkerCrewSections, RigJobCrewSectionStatus.LogOnDuty);
        }

        public static void UpdateRigJobSanjelCrewSectionsStatus(List<RigJobSanjelCrewSection> rigJobSanjelCrewSections, RigJobCrewSectionStatus rigJobCrewSectionStatus)
        {
            if (rigJobSanjelCrewSections != null && rigJobSanjelCrewSections.Count > 0)
            {
                foreach (RigJobSanjelCrewSection rigJobCrewSection in rigJobSanjelCrewSections)
                {
                    rigJobCrewSection.RigJobCrewSectionStatus = rigJobCrewSectionStatus;
                    eServiceOnlineGateway.Instance.UpdateRigJobCrewSection(rigJobCrewSection);
                }
            }
        }

        public static void UpdateRigJobThirdPartyBulkerCrewSectionsStatus(List<RigJobThirdPartyBulkerCrewSection> rigJobThirdPartyBulkerCrewSections, RigJobCrewSectionStatus rigJobCrewSectionStatus)
        {
            if (rigJobThirdPartyBulkerCrewSections != null && rigJobThirdPartyBulkerCrewSections.Count > 0)
            {
                foreach (RigJobThirdPartyBulkerCrewSection rigJobThirdPartyBulkerCrewSection in rigJobThirdPartyBulkerCrewSections)
                {
                    rigJobThirdPartyBulkerCrewSection.RigJobCrewSectionStatus = rigJobCrewSectionStatus;
                    eServiceOnlineGateway.Instance.UpdateRigJobThirdPartyCrewSection(rigJobThirdPartyBulkerCrewSection);
                }
            }
        }

        public static void LogOffDuty(int rigJobId)
        {
            List<RigJobSanjelCrewSection> rigJobCrewSections = eServiceOnlineGateway.Instance.GetRigJobCrewSectionByRigJob(rigJobId);
            UpdateRigJobSanjelCrewSectionsStatus(rigJobCrewSections, RigJobCrewSectionStatus.LogOffDuty);
            List<RigJobThirdPartyBulkerCrewSection> rigJobThirdPartyBulkerCrewSections = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionByRigJob(rigJobId);
            UpdateRigJobThirdPartyBulkerCrewSectionsStatus(rigJobThirdPartyBulkerCrewSections, RigJobCrewSectionStatus.LogOffDuty);
        }

        public static SanjelCrewSchedule GetCrewScheduleByJobCrewSection(int jobCrewSetionId)
        {
            return eServiceOnlineGateway.Instance.GetCrewScheduleByJobCrewSection(jobCrewSetionId);
        }

        /*
        public static bool AdjustJobDuration(int rigJobId,int estJobDuration)
        {
            RigJob rigJob = eServiceOnlineGateway.Instance.GetRigJobById(rigJobId);
            rigJob.JobDuration = estJobDuration;
            List<RigJobSanjelCrewSection> rigJobCrewSections = eServiceOnlineGateway.Instance.GetRigJobCrewSectionByRigJob(rigJobId).FindAll(s=>s.ProductHaul.Id==0);
            DateTime startTime=GetCallCrewTime(rigJobId);

            foreach (var rigJobCrewSection in rigJobCrewSections)
            {
                if (rigJobCrewSection.SanjelCrew.Type.Id == 1 || rigJobCrewSection.SanjelCrew.Type.Id == 4)
                {
                    SanjelCrewSchedule crewSchedule =
                        eServiceOnlineGateway.Instance.GetCrewScheduleByJobCrewSection(rigJobCrewSection.Id);
                    if (crewSchedule == null) continue;
                    UpdateSanjelCrewSchedule(crewSchedule.StartTime, crewSchedule.StartTime.AddMinutes(estJobDuration),
                        crewSchedule);
                }
            }
        
            InspectProductHaulScheduleByAssignCrew(rigJobId, startTime, startTime.AddMinutes(estJobDuration));

            return true;
        }
        */

        public static void ReleaseCrew(int rigJobId, bool isCompleteJob, DateTime jobCompleteTime)
        {
            //If isCompleteJob, set crew off duty, update schedules use job complete time. Delete Assignments.
            //If not isCompleteJob, delete all assignments and schedules. ProductHaul assignment needs to be released manually

            List<RigJobSanjelCrewSection> rigJobCrewSections =
                eServiceOnlineGateway.Instance.GetRigJobCrewSectionByRigJob(rigJobId);
            foreach (var rigJobSanjelCrewSection in rigJobCrewSections)
            {
                SanjelCrew sanjelCrew =
                    eServiceOnlineGateway.Instance.GetCrewById(rigJobSanjelCrewSection.SanjelCrew.Id, true);
                if (sanjelCrew != null && sanjelCrew.Type != null && (sanjelCrew.Type.Name == "Pumper Crew" ||
                                                                      sanjelCrew.Type.Name == "Spare Crew"))
                {
                    if (isCompleteJob)
                    {
                        rigJobSanjelCrewSection.RigJobCrewSectionStatus = RigJobCrewSectionStatus.LogOffDuty;
                        eServiceOnlineGateway.Instance.UpdateRigJobCrewSection(rigJobSanjelCrewSection);
                        CrewProcess.LogoffSanjelCrew(rigJobSanjelCrewSection.Id, jobCompleteTime);
                    }
                    else
                    {
                        CrewProcess.ReleaseSanjelCrew(rigJobSanjelCrewSection);
                    }
                }
            }
        }


        /*
        public static void UpdateCrewScheduleByRigJob(RigJob rigJob)
        {
            List<RigJobSanjelCrewSection> rigJobCrewSections = eServiceOnlineGateway.Instance.GetRigJobCrewSectionByRigJob(rigJob.Id);
            DateTime startTime = rigJob.CallCrewTime;
            foreach (var rigJobCrewSection in rigJobCrewSections)
            {
                SanjelCrewSchedule crewSchedule = eServiceOnlineGateway.Instance.GetCrewScheduleByJobCrewSection(rigJobCrewSection.Id);
                if (crewSchedule != null && crewSchedule.SanjelCrew != null)
                {
                    SanjelCrew crew = eServiceOnlineGateway.Instance.GetCrewById(crewSchedule.SanjelCrew.Id, true);
                    if (crew != null && crew.Type != null && !crew.Type.Name.Equals("Bulker Crew"))
                    {
                        UpdateSanjelCrewSchedule(startTime, startTime.AddHours(8), crewSchedule);
                    }
                }
            }
            //reschedule rigJob 更新pumper crew时，在检查一次productHaul的schedule
            InspectProductHaulScheduleByAssignCrew(rigJob.Id, startTime, startTime.AddHours(8));
        }
        */

        public static List<SanjelCrew> GetSanjelCrew()
        {
            return eServiceOnlineGateway.Instance.GetCrewList();
        }

        public static List<SanjelCrew> GetSanjeBulkerCrew()
        {
            return eServiceOnlineGateway.Instance.GetCrewList().FindAll(p => p.Type.Id == 2);
        }
        public static int GetMaxProgramRevisionById(string programId)
        {
            var maxVersion = 0;
            try
            {
                //ProgramMicroService
                if (!string.IsNullOrEmpty(programId))
                {
                    IProgramMicroService programService = ServiceFactory.Instance.GetService(typeof(IProgramMicroService)) as IProgramMicroService;
                    if (programService == null) throw new Exception("IProgramService must be registered in service factory.");
                    maxVersion = programService.GetMaxProgramRevisionByID(programId)??0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return maxVersion;
        }

        /*
        public static Collection<BlendSection> GetBlendSectionCollectionByRootIdIsCallSheetId(int callSheetId)
        {
            Sanjel.Services.Interfaces.ICallSheetService callSheetService =
            ServiceFactory.Instance.GetService(typeof(Sanjel.Services.Interfaces.ICallSheetService)) as Sanjel.Services.Interfaces.ICallSheetService;
            
            if (callSheetService == null) throw new Exception("ICallSheetService must be registered in service factory.");
        
            /*
            Collection<BlendSection> temp = new Collection<BlendSection>();
            Collection<BlendSection> blendSections = callSheetService.GetBlendSectionsByCallSheetId(callSheetId);
            foreach (var blendSection in blendSections)
            {
                temp.Add(callSheetService.GetBlendSectionByBlendSectionId(blendSection.Id));
            }
                
            return temp;
        #1#

            Collection<BlendSection> blendSections = callSheetService.GetBlendSectionsByCallSheetId(callSheetId);
            return blendSections;
        }
        */

        public static List<Rig> GetRigByDrillingCompanyId(int drillingCompanyId)
        {
            return eServiceOnlineGateway.Instance.GetRigByDrillingCompanyId(drillingCompanyId);
        }

        public static List<Rig> GetDeactivateRigByDrillingCompanyId(int drillingCompanyId)
        {
            List<Rig> rigs =eServiceOnlineGateway.Instance.GetRigByDrillingCompanyId(drillingCompanyId);
            return rigs.Where(s => s.Status == RigStatus.Deactivated).ToList();
        }
    }
}