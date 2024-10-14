using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eServiceOnline.Controllers;
using eServiceOnline.Gateway;
using eServiceOnline.Models.Commons;
using eServiceOnline.Models.CrewBoard;
using eServiceOnline.Models.ProductHaul;
using eServiceOnline.Models.Shared;
using eServiceOnline.Models.ThirdPartyCrewBoard;
using Microsoft.Extensions.Caching.Memory;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule;
using Sesi.SanjelData.Entities.BusinessEntities.WellSite;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Organization;
using Sesi.SanjelData.Entities.Common.BusinessEntities.WellSite;
using JobType = Sesi.SanjelData.Entities.Common.BusinessEntities.Operation.JobType;
using RigJob = Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch.RigJob;
using Rig = Sesi.SanjelData.Entities.Common.BusinessEntities.WellSite.Rig;

namespace eServiceOnline.Models.RigBoard
{
    public class RigJobViewModel :RigJobViewModelBase
    {
        #region const

        public const string More = "More";
        public const string LongDateTimeFormat = "MMM d H:mm";
        public const string ShortDateTimeFormat = "MMM d";
        public const string ShortTimeFormat = "H:mm";
        public const string NoticeLongDateTimeFormat = "MMM d,H:mm";
        public const string NoticeShortDateTimeFormat = "MMM d,yyyy";
        public const string StyleJobAlertJobOrDate = "jobalert-job-date";
        public const string StyleJobRiskMatrixRange = "job-RiskMatrix-";
        public const string StyleJobAlertAmber = "jobalert-amberstate";
        #region Company

        public const string HoverCod = "COD";
        public const string CodCleared = "COD cleared";
        public const string StyleCod = "cod";
        public const string StyleCodCleared = "codcleared";
        public const string MenuRemoveCodCleared = "Remove COD cleared";
        public const string MenuRemoveJobAlert = "Remove Job Alert";
        public const string MenuUpdateJobAlert = "Update Job Alert";
        public const string MenuCreateJobAlert = "Create Job Alert";
        public const string MenuUpdateCompanyShortName = "Update Company Short Name";

        #endregion

        #region  Lsd

        public const string HoverDirectionRequired = "Directions required";
        public const string StyleDirectionNotEntered = "notentered";
        public const string StyleDirectionEntered = "entered";
        public const string MenuUpdateWellLocation = "Update Well Location";
        public const string MenuUpdateUpdateDirection = "Update Direction";
        public const string DialogUpdateWellLocation = "updatewelllocation";
        public const string DialogUpdateUpdateDirection = "UpdateDirection";

        #endregion

        #region  Rig

        public const string HoverIsTopDrive = "TOP DRIVE RIG";
        public const string StyleRigIsTopDrive = "topdrive";
        public const string MenuCreate = "Add New Rig";
        public const string MenuUpdate = "Update the Rig";
        public const string MenuEnable = "Enable the Rig";
        public const string MenuDownMaintenance = "Down for maintenance";
        public const string MenuDownForHoldingEquipment = "Down for holding equipment";
        public const string MenuDownForWeather = "Down for weather";
        public const string MenuDownForNewLeaseLicenses = "Down for new lease/licenses";
        public const string MenuDeactivate = "Deactivate";
        public const string MenuActivate = "Activate a Rig";
        public const string DialogAddNewRig = "addnewrig";
        public const string DialogUpdateTheRig = "updatetherig";

        #endregion

        #region Job

        public const string MenuAssignPlugLoadingHead = "Assign Plug Loading Head";
        public const string MenuAssignWitsBox = "Assign Wits Box";
        public const string MenuAssignNubbin = "Assign Nubbin";
        public const string MenuAssignSwedge = "Assign Swedge";
        #endregion

        #region  Date

        public const string MenuJobdateConfirmationCall = "Confirmation Call";
        public const string MenuJobdateFirstCall = "FirstCall";
        public const string MenuJobdateBidLost = "Bid Lost";
        public const string MenuJobdateCancel = "Cancel";
        public const string MenuJobdateDelete = "Delete";
        public const string MenuJobdateReschedule = "Reschedule";
        public const string MenuJobdateOnHold = "On Hold";
        public const string MenuJobdateCallOut = "Call Out";
        public const string MenuJobdateComplete = "Complete";
        public const string MenuJobdatePostpone = "Postpone";
        public const string StyleJobdateAlerted = "alerted";
        public const string StyleJobdatePending = "pending";
        public const string StyleJobdateConfirmed = "confirmed";
        public const string StyleJobdateScheduled = "scheduled";
        public const string StyleJobdateDispatched = "dispatched";
        public const string StyleJobdateInprogress = "inprogress";
        public const string StyleJobdateCompleted = "completed";
        public const string StyleJobdateCanceled = "canceled";
        public const string NoticeJobdatePending = "Job is pending, Expected time on location is ";
        public const string NoticeJobdateConfirmed = "Job is Confirmed, expected time on location is ";
        public const string NoticeJobdateScheduled = "Resources are scheduled, expected time on location is ";
        public const string NoticeJobdateDispatched = "Crew is called out, expected time on location is ";
        public const string NoticeJobdateInprogress = "Job is in progress, expected time on location is ";

        #endregion

        #region  Bl

        public const string StyleBlNeedahaul = "bl-needahaul";
        public const string StyleBlPartialScheduled = "bl-partialscheduled";
        public const string StyleBlHaulScheduled = "bl-haulscheduled";
        public const string StyleBlPartialOnLocation = "bl-partialonlocation";
        public const string StyleBlOnLocation = "bl-onlocation";
        public const string StyleBlGowithcrew = "bl-gowithcrew";
        public const string MenuNeedAHaul = "Need a haul";
        public const string MenuGowithCrew = "Go with Crew";
        public const string MenuDoNotNeedAHaul = "Don’t need a haul";
        public const string MenuScheduleProductHaul = "Schedule Product Haul";
        public const string MenuReScheduleProductHaul = "Re-schedule Product Haul";
        public const string MenuCancelProductHaul = "Cancel Product Haul";
        public const string MenuOnLocation = "On Location";
        public const string MenuUpdateBlend = "Update the Blend";
        public const string DialogScheduleProductHaul = "scheduleproducthaul";


        #endregion

        #region Crew

        public const string MenuAssignACrew = "Assign A Crew";
        public const string MenuWithdrawCrew = "Withdraw A Crew";
        public const string MenuCallCrew = "Call All Crew";
        public const string MenuLogOnDuty = "Log On Duty";
        public const string MenuLogOffDuty = "Log Off Duty";

        public const string StyleJobJobTypeState = "job-jobtypestate";
        public const string MenAdjustJobDuration = "Adjust Job Duration";

        #endregion

        #region  Notes

        public const string MenuUpdateNotes = "Update Notes";
        public const string DialogUpdateNotes = "updatenotes";

        #endregion


        #region Consultant Contacts

        public const string MenuAddAConsultant = "Add A Consultant";
        public const string MenuUpdateTheConsultant = "Update The Consultant";
        public const string MenuRemoveConsultant = "Remove Consultant";
        public const string MenuCreateNewConsultant = "Create New Consultant";
        public const string MenuUpdateConsultant = "Update Consultant";
        public const string MenuDeleteConsultant = "Delete Consultant";
        public const string MenuAssignToDayShift = "Assign to Day Shift";
        public const string MenuAssignToNightShift = "Assign to Night Shift";
        public const string MenuAssignTo24HourShift = "Assign to 24 Hour Shift";
        public const string DialogUpdateTheConsultant = "updatetheconsultant";
        public const string DialogAddAConsultant = "addaconsultant";
        public const string DialogAddNewConsultant = "addnewconsultant";
        public const string DialogUpdateConsultant = "updateconsultant";
        public const string NoticeConsultantDay = "Day Shift ";
        public const string NoticeConsultantNight = "Night Shift ";
        public const string NoticeConsultant24 = "24 Hour ";

        #endregion

        #endregion const

        #region Constructor

        public RigJobViewModel(IMemoryCache memoryCache, string loggedUser)
        {
            this._memoryCache = memoryCache;
            this.LoggedUser = loggedUser;
            this.BlendProductHaulModel1 = new BlendProductHaulModel();
            this.BlendProductHaulModel2 = new BlendProductHaulModel();
            this.BlendProductHaulModel3 = new BlendProductHaulModel();
            this.BinSectionModel = new BinSectionModel();
            this.ConsultantViewModel = new ConsultantViewModel();
            this.RigJobCrewSectionModels = new List<RigJobCrewSectionModel>();
            this.CrewModels = new List<CrewModel>();
            this.ThirdPartyCrewModels = new List<ThirdPartyCrewModel>();
        }

        #endregion Constructor

        #region Properties

        public BlendProductHaulModel BlendProductHaulModel1 { get; set; }
        public BlendProductHaulModel BlendProductHaulModel2 { get; set; }
        public BlendProductHaulModel BlendProductHaulModel3 { get; set; }
        public List<RigJobCrewSectionModel> RigJobCrewSectionModels { get; set; }
        public ConsultantViewModel ConsultantViewModel { get; set; }
        public List<CrewModel> CrewModels { get; set; }
        public List<ThirdPartyCrewModel> ThirdPartyCrewModels { get; set; }
        public List<ProductHaulLoad> ProductHaulLoadList { set; get; }


        public StyledCell Lsd { get; set; }
        public StyledCell Company { get; set; }
        public StyledCell Job { get; set; }
        public StyledCell Date { get; set; }
        public StyledCell Bl1 { get; set; }
        public StyledCell Bl2 { get; set; }
        public StyledCell Bl3 { get; set; }
        public StyledCell Crew { get; set; }
        public StyledCell Notes { get; set; }
        public StyledCell ConsultantContacts { get; set; }

        #endregion Properties

        #region  Methods



        public override void PopulateFrom(RigJob rigJob)
        {
            if (rigJob == null) throw new Exception("entity must be instance of class RigJob.");

            this.SequenceNumber = this.GetSequenceNumberStyledCell("SequenceNumber");
            this.Company = this.GetCompanyStyledCell("Company", rigJob);
            this.Lsd = this.GetLsdStyledCell("Lsd", rigJob);
            this.Rig = this.GetRigStyledCell("Rig", rigJob);
            this.Job = this.GetJobStyledCell("Job", rigJob);
            this.Date = this.GetDateStyledCell("Date", rigJob);
            this.Crew = this.GetCrewStyledCell("Crew", rigJob);
//            this.Notes = this.GetNotesStyledCell("Notes", rigJob);
        }

        public void PopulateFromBlendAndConsultantContacts(RigJob rigJob, List<BinInformation> rigBinSections,
            List<Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul> productHauls,
            List<ShippingLoadSheet> shippingLoadSheets)
        {
            this.ConsultantContacts = this.GetConsultantContactsStyledCell("ConsultantContacts", rigJob);
            this.Bl1 = this.GetPreflushStyledCell("BL1", rigJob, productHauls);
            this.Bl2 = this.GetBlendStyledCell("BL2", rigJob, productHauls);
            this.Bl3 = this.GetDisplacementStyledCell("BL3", rigJob, productHauls);
            this.Notes = this.GetNotesStyledCell("Notes", rigJob, rigBinSections);
        }


        #region Compute company information from entity

        protected StyledCell GetCompanyStyledCell(string propertyName, RigJob rigJob)
        {
            string statusName;
            ClientCompany clientCompany = rigJob.ClientCompany;

            StyledCell styledCell = new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache) {PropertyValue = string.IsNullOrEmpty(rigJob.ClientCompanyShortName) ? clientCompany?.Name: rigJob.ClientCompanyShortName };
            styledCell = this.SetCompanyStyledCellByStatus(rigJob, styledCell, propertyName, out statusName);
            styledCell.Style = styledCell.ComputeStyle(propertyName, statusName, this.GetDownRigSuffix(rigJob));

            styledCell = this.SetCellMerge(styledCell);

            return styledCell;
        }

        private StyledCell SetCompanyStyledCellByStatus(RigJob rigJob, StyledCell styledCell, string propertyName, out string statusName)
        {
            statusName = string.Empty;
            ContextMenu contextMenu;
            if (rigJob.JobLifeStatus!=null)
            {
                if (rigJob.JobLifeStatus==JobLifeStatus.Alerted)
                {
                    contextMenu = new ContextMenu() { MenuName = MenuCreateJobAlert, ProcessingMode = ProcessingMode.PopsUpWindow, DialogName = MenuCreateJobAlert, ControllerName = RigBoardController, ActionName = "CreateJobAlert" };
                    if (styledCell.IsDisplayMenu) styledCell.ContextMenus.Add(contextMenu);
                    contextMenu = new ContextMenu() { MenuName = MenuRemoveJobAlert, ProcessingMode = ProcessingMode.PopsUpWindow, DialogName = MenuRemoveJobAlert, ControllerName = RigBoardController, ActionName = "DeleteJobAlert", Parms = new List<string>() { rigJob.Id.ToString() } };
                    if (styledCell.IsDisplayMenu) styledCell.ContextMenus.Add(contextMenu);
                    contextMenu = new ContextMenu() { MenuName = MenuUpdateJobAlert, ProcessingMode = ProcessingMode.PopsUpWindow, DialogName = MenuUpdateJobAlert, ControllerName = RigBoardController, ActionName = "UpdateJobAlert", Parms = new List<string>() { rigJob.Id.ToString() } };
                    if (styledCell.IsDisplayMenu) styledCell.ContextMenus.Add(contextMenu);
                }
                else
                {
                    if (rigJob.ClientCompany !=null && rigJob.ClientCompany.IsCODCustomer)
                    {
                        contextMenu = new ContextMenu() { ProcessingMode = ProcessingMode.NoPopsUpWindow, ControllerName = RigBoardController, ActionName = "ModifyCompanyCodClearedFlag", Parms = new List<string>() { rigJob.CallSheetNumber.ToString(), (!rigJob.IsCoDCleared).ToString(), rigJob.Id.ToString() } };
                        if (rigJob.CallSheetId > 0)
                        {
                            if (!rigJob.IsCoDCleared)
                            {
                                styledCell.Notice = HoverCod;
                                contextMenu.MenuName = CodCleared;
                                if (styledCell.IsDisplayMenu)
                                {
                                    styledCell.ContextMenus.Add(contextMenu);
                                }
                                statusName = StyleCod;
                            }
                            else
                            {
                                styledCell.Notice = CodCleared;
                                contextMenu.MenuName = MenuRemoveCodCleared;
                                if (styledCell.IsDisplayMenu)
                                {
                                    styledCell.ContextMenus.Add(contextMenu);
                                }
                                statusName = StyleCodCleared;
                            }
                        }
                    }
                    contextMenu = new ContextMenu() { MenuName = MenuCreateJobAlert, ProcessingMode = ProcessingMode.PopsUpWindow, DialogName = MenuCreateJobAlert, ControllerName = RigBoardController, ActionName = "CreateJobAlert" };
                    if (styledCell.IsDisplayMenu) styledCell.ContextMenus.Add(contextMenu);
                    contextMenu = new ContextMenu() { MenuName = MenuUpdateCompanyShortName, ProcessingMode = ProcessingMode.PopsUpWindow, ControllerName = RigBoardController, ActionName = "UpdateCompanyShortName", Parms = new List<string> { rigJob.Id.ToString() } };
                    if (styledCell.IsDisplayMenu) styledCell.ContextMenus.Add(contextMenu);
                }
            }

            return styledCell;
        }

        #endregion

        #region Compute lsd information from entity

        private StyledCell GetLsdStyledCell(string propertyName, RigJob rigJob)
        {
            string statusName = null;
            bool isChange = rigJob.RigStatus==RigStatus.Active || rigJob.RigStatus==RigStatus.Deactivated;
            StyledCell styledCell = new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache) { PropertyValue = !string.IsNullOrEmpty(rigJob.WellLocation) ? rigJob.WellLocation : rigJob.SurfaceLocation };
            if (rigJob.JobLifeStatus!=JobLifeStatus.None && rigJob.JobLifeStatus!=JobLifeStatus.Alerted)
            {
                if (string.IsNullOrEmpty(rigJob.Directions))
                {
                    styledCell.Notice = HoverDirectionRequired;
                    statusName = StyleDirectionNotEntered;
                }
                else
                {
                    styledCell.Notice = rigJob.Directions;
                    statusName = StyleDirectionEntered;
                }
            }
            styledCell.Style = styledCell.ComputeStyle(propertyName, isChange ? statusName : null, isChange ? null : this.GetDownRigSuffix(rigJob));
            styledCell.ContextMenus = styledCell.IsDisplayMenu ? ((rigJob.JobLifeStatus==JobLifeStatus.Completed || rigJob.JobLifeStatus==JobLifeStatus.Canceled || rigJob.JobLifeStatus==JobLifeStatus.Alerted) ? null : this.SetLsdContextMenus(rigJob)) : null;

            styledCell = this.SetCellMerge(styledCell);

            return styledCell;
        }

        private List<ContextMenu> SetLsdContextMenus(RigJob rigJob)
        {
            List<ContextMenu> list = new List<ContextMenu>();

            //Update Well Location or Update Direction by different job status,update it in callsheet or jobAlert or job?or other?
            if (rigJob.CallSheetNumber != 0)
            {
                ContextMenu menu1 = new ContextMenu() {MenuName = MenuUpdateWellLocation, ProcessingMode = ProcessingMode.PopsUpWindow, DialogName = DialogUpdateWellLocation, ControllerName = RigBoardController, ActionName = "GetLsdInfoWellLocationByNumber"};
                ContextMenu menu2 = new ContextMenu() {MenuName = MenuUpdateUpdateDirection, ProcessingMode = ProcessingMode.PopsUpWindow, DialogName = DialogUpdateUpdateDirection, ControllerName = RigBoardController, ActionName = "GetLsdInfoDirectionByNumber"};
                menu1.Parms = new List<string>() { rigJob.CallSheetNumber.ToString(), rigJob.Id.ToString() };
                menu2.Parms = new List<string>() { rigJob.CallSheetNumber.ToString(), rigJob.Id.ToString() };
                list.Add(menu1);
                list.Add(menu2);
            }

            return list;
        }

        #endregion

        #region Compute Rig information from entity

        protected override StyledCell GetRigStyledCell(string propertyName, RigJob rigJob)
        {
            Rig rig = rigJob.Rig;
            if (rig == null)
            {
                StyledCell styledCellRig = this.SetCellMerge(new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache));
                styledCellRig.ContextMenus = styledCellRig.IsDisplayMenu && rigJob.JobLifeStatus!=JobLifeStatus.Alerted ? this.SetRigContextMenus(rigJob) : null;
                return styledCellRig;
            }

            string statusName = string.Empty;
            StyledCell styledCell = new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache) {PropertyValue = rig.Name};
            var tooltip = string.Empty;
            if (rig.IsTopDrive)
            {
                tooltip = HoverIsTopDrive;
                statusName = StyleRigIsTopDrive;
            }
            styledCell.Style = styledCell.ComputeStyle(propertyName, statusName, this.GetDownRigSuffix(rigJob));
            styledCell.ContextMenus = (styledCell.IsDisplayMenu && rigJob.JobLifeStatus != JobLifeStatus.Alerted) ? this.SetRigContextMenus(rigJob) : null;

            if (rig.Status!=RigStatus.None)
            {
                if(string.IsNullOrEmpty(tooltip))
                    tooltip = rig.Status.ToString();
                else
                    tooltip = tooltip + " - " + rig.Status.ToString();
            }

            styledCell.Notice = rig.Status.Equals(JobLifeStatus.Alerted) ? null : tooltip;
            styledCell = this.SetCellMerge(styledCell);

            return styledCell;
        }

        private List<ContextMenu> SetRigContextMenus(RigJob rigJob)
        {
            List<ContextMenu> list = new List<ContextMenu>();
            Rig rigJobRig = rigJob.Rig ?? new Rig();


            list.Add(new ContextMenu()
            {
                MenuName = MenuUpdate,
                ProcessingMode = ProcessingMode.PopsUpWindow,
                Parms = new List<string>() {rigJobRig.Id.ToString(), rigJob.Id.ToString()},
                DialogName = DialogUpdateTheRig,
                ControllerName = RigBoardController,
                ActionName = "GetRigById",
                IsDisabled = rigJobRig.Id.Equals(0),
            });
            list.Add(new ContextMenu()
            {
                MenuName = MenuEnable,
                ProcessingMode = ProcessingMode.PopsUpWindow,
                Parms = new List<string>() { rigJobRig.Id.ToString(), rigJobRig.Status.ToString() },
                DialogName = DialogUpdateTheRig,
                ControllerName = RigBoardController,
                ActionName = "EnableRig",
                IsDisabled =  rigJobRig.Status==RigStatus.Active
            });
            list.Add(new ContextMenu()
            {
                MenuName = MenuDownMaintenance,
                ProcessingMode = ProcessingMode.PopsUpWindow,
                Parms = new List<string>() {rigJobRig.Id.ToString(), "DownForMaintenance"},
                ControllerName = RigBoardController,
                ActionName = "UpdateRigStatusToDown",
                IsDisabled = IsDisabledForDownOrDeactivate(rigJob),
            });
            list.Add(new ContextMenu()
            {
                MenuName = MenuDownForHoldingEquipment,
                ProcessingMode = ProcessingMode.PopsUpWindow,
                Parms = new List<string>() {rigJobRig.Id.ToString(), "DownForHoldingEquipment"},
                ControllerName = RigBoardController,
                ActionName = "UpdateRigStatusToDown",
                IsDisabled = IsDisabledForDownOrDeactivate(rigJob),
            });
            list.Add(new ContextMenu()
            {
                MenuName = MenuDownForWeather,
                ProcessingMode = ProcessingMode.PopsUpWindow,
                Parms = new List<string>() {rigJobRig.Id.ToString(), "DownForWeather"},
                ControllerName = RigBoardController,
                ActionName = "UpdateRigStatusToDown",
                IsDisabled = IsDisabledForDownOrDeactivate(rigJob),
            });
            list.Add(new ContextMenu()
            {
                MenuName = MenuDownForNewLeaseLicenses,
                ProcessingMode = ProcessingMode.PopsUpWindow,
                Parms = new List<string>() {rigJobRig.Id.ToString(), "DownForNewLeaseOrLicenses"},
                ControllerName = RigBoardController,
                ActionName = "UpdateRigStatusToDown",
                IsDisabled = IsDisabledForDownOrDeactivate(rigJob),
            });
            list.Add(new ContextMenu()
            {
                MenuName = MenuDeactivate,
                ProcessingMode = ProcessingMode.PopsUpWindow,
                Parms = new List<string>() {rigJobRig.Id.ToString()},
                ControllerName = RigBoardController,
                ActionName = "UpdateRigStatus",
                IsDisabled = IsDisabledForDownOrDeactivate(rigJob),
            });

            list.Add(new ContextMenu()
            {
                MenuName = MenuCreate,
                ProcessingMode = ProcessingMode.PopsUpWindow,
                DialogName = DialogAddNewRig,
                ControllerName = RigBoardController,
                IsHaveSplitLine = true,
                ActionName = "CreateRig"
            });

            list.Add(new ContextMenu()
            {
                MenuName = MenuActivate,
                ProcessingMode = ProcessingMode.PopsUpWindow,
                ControllerName = RigBoardController,
                ActionName = "ActivateARig"
            });

            return list;
        }

        private static bool IsDisabledForDownOrDeactivate(RigJob rigJob)
        {
            Rig rigJobRig = rigJob.Rig ?? new Rig();
            return rigJobRig.Id.Equals(0) || (rigJob.JobLifeStatus!=JobLifeStatus.Completed && rigJob.JobLifeStatus!=JobLifeStatus.None && rigJob.JobLifeStatus!=JobLifeStatus.Canceled);
        }

        #endregion

        #region Compute JobType information from entity

        private StyledCell GetJobStyledCell(string propertyName, RigJob rigJob)
        {
            JobType rigJobJob = rigJob.JobType;
            if (rigJobJob == null)
                return this.SetCellMerge(new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache));

            StyledCell styledCell = new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache) {PropertyValue = string.IsNullOrEmpty(rigJobJob.Name) ? "" : rigJobJob.Name };
            styledCell = this.SetJobStyledCellByStatusOrRiskMatrix(styledCell, rigJob);
            styledCell = this.SetCellMerge(styledCell);
            styledCell.ContextMenus = this.SetJobContextMenus(rigJob);
            return styledCell;
        }

        private StyledCell SetJobStyledCellByStatusOrRiskMatrix(StyledCell styledCell, RigJob rigJob)
        {
            styledCell.Style = styledCell.ComputeStyle(styledCell.PropertyName, null, this.GetDownRigSuffix(rigJob));

            if (rigJob.JobLifeStatus == JobLifeStatus.Alerted)
            {
                styledCell.Style = StyleJobAlertJobOrDate;
            }
            else if (rigJob.JobLifeStatus == JobLifeStatus.Pending 
                     || rigJob.JobLifeStatus == JobLifeStatus.Confirmed
                     || rigJob.JobLifeStatus == JobLifeStatus.Scheduled 
                     || rigJob.JobLifeStatus == JobLifeStatus.Dispatched 
                     || rigJob.JobLifeStatus == JobLifeStatus.InProgress)
            {
                styledCell.Style = GetJobStyledCellByRiskMatrix(styledCell, rigJob);
            }

            return styledCell;
        }

        private string GetJobStyledCellByRiskMatrix(StyledCell styledCell, RigJob rigJob)
        {
            var riskMatrixRangeSuffix = "";

            switch (rigJob.RiskMatrix)
            {
                case var rm when (rm >= 0 && rm < 60):
                    riskMatrixRangeSuffix = "059";
                    break;

                case var rm when (rm >= 60 && rm < 80):
                    riskMatrixRangeSuffix = "6079";
                    break;

                case var rm when (rm >= 80 && rm < 120):
                    riskMatrixRangeSuffix = "80119";
                    break;

                case var rm when (rm >= 120):
                    riskMatrixRangeSuffix = "120";
                    break;

                default://in case under 0
                    return styledCell.ComputeStyle(styledCell.PropertyName, null, this.GetDownRigSuffix(rigJob));
            }

            return $"{StyleJobRiskMatrixRange}{riskMatrixRangeSuffix}";
        }
        #endregion

        #region Compute date information from entity

        private StyledCell GetDateStyledCell(string propertyName, RigJob rigJob)
        {
            DateTime? rigJobDate = rigJob.JobDateTime;
            if (rigJobDate == null)
                return this.SetCellMerge(new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache));

            string statusName;
            StyledCell styledCell = new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache) {PropertyValue = Utility.GetDateTimeValue(rigJobDate.Value, LongDateTimeFormat)};

            if (rigJob.JobLifeStatus == JobLifeStatus.Completed || rigJob.JobLifeStatus == JobLifeStatus.None || rigJob.JobLifeStatus == JobLifeStatus.Canceled || rigJob.JobLifeStatus== JobLifeStatus.Alerted)
            {
                styledCell.PropertyValue = Utility.GetDateTimeValue(rigJobDate.Value, ShortDateTimeFormat);
            }
            styledCell = this.SetDateStyledCellByStatus(rigJob, styledCell, propertyName, out statusName);
            styledCell.Style = rigJob.JobLifeStatus==JobLifeStatus.Alerted ? StyleJobAlertJobOrDate : styledCell.ComputeStyle(propertyName, statusName, this.GetDownRigSuffix(rigJob));
            styledCell = this.SetCellMerge(styledCell);

            return styledCell;
        }

        private StyledCell SetDateStyledCellByStatus(RigJob rigJob, StyledCell styledCell, string propertyName, out string statusName)
        {
            statusName = string.Empty;

            switch (rigJob.JobLifeStatus)
            {
                case JobLifeStatus.Alerted:
                    statusName = StyleJobdateAlerted;
                    break;
                case JobLifeStatus.Pending:
                    statusName = StyleJobdatePending;
                    if (rigJob.JobDateTime != null) styledCell.Notice = string.Format(NoticeJobdatePending + "{0}{1}{2}", "{", Utility.GetDateTimeValue(rigJob.JobDateTime, NoticeLongDateTimeFormat), "}");
                    break;
                case JobLifeStatus.Confirmed:
                    statusName = StyleJobdateConfirmed;
                    if (rigJob.JobDateTime != null) styledCell.Notice = string.Format(NoticeJobdateConfirmed + "{0}{1}{2}", "{", Utility.GetDateTimeValue(rigJob.JobDateTime, NoticeLongDateTimeFormat), "}");

                    break;
                case JobLifeStatus.Scheduled:
                    statusName = StyleJobdateScheduled;
                    if (rigJob.JobDateTime != null) styledCell.Notice = string.Format(NoticeJobdateScheduled + "{0}{1}{2}", "{", Utility.GetDateTimeValue(rigJob.JobDateTime, NoticeLongDateTimeFormat), "}");
                    break;
                case JobLifeStatus.Dispatched:
                    statusName = StyleJobdateDispatched;
                    if (rigJob.JobDateTime != null) styledCell.Notice = string.Format(NoticeJobdateDispatched + "{0}{1}{2}", "{", Utility.GetDateTimeValue(rigJob.JobDateTime, NoticeLongDateTimeFormat), "}");
                    break;
                case JobLifeStatus.InProgress:
                    statusName = StyleJobdateInprogress;
                    if (rigJob.JobDateTime != null) styledCell.Notice = string.Format(NoticeJobdateInprogress + "{0}{1}{2}", "{", Utility.GetDateTimeValue(rigJob.JobDateTime, NoticeLongDateTimeFormat), "}");
                    break;
                case JobLifeStatus.Completed:
                    statusName = StyleJobdateCompleted;
                    if (rigJob.JobDateTime != null) styledCell.Notice = string.Format("Job {0}{3}{2} has been completed on {0}{1}{2}", "{", Utility.GetDateTimeValue(rigJob.JobDateTime, NoticeShortDateTimeFormat), "}", rigJob.CallSheetNumber.ToString());
                    break;
                case JobLifeStatus.Canceled:
                    statusName = StyleJobdateCanceled;
                    if (rigJob.JobDateTime != null) styledCell.Notice = string.Format("Job {0}{3}{2} has been canceled on {0}{1}{2}", "{", Utility.GetDateTimeValue(rigJob.JobDateTime, NoticeShortDateTimeFormat), "}", rigJob.CallSheetNumber.ToString());
                    break;
            }
            styledCell.ContextMenus = styledCell.IsDisplayMenu ? this.SetDateContextMenus(rigJob, styledCell) : null;
            if (styledCell.Notice != null) styledCell.Notice = this.SetDateNotice(rigJob, styledCell.Notice);

            return styledCell;
        }

        private string SetDateNotice(RigJob rigJob,string notice)
        {
            if (rigJob != null)
            {
                if (rigJob.CallSheetNumber != 0) notice = notice + $"\nCallSheet Number {{{rigJob.CallSheetNumber}}}";
                if (rigJob.ProgramId != null) notice = notice + $"\nProgram Number {{{rigJob.ProgramId}}}";
                if (rigJob.JobNumber != 0 && rigJob.JobLifeStatus == JobLifeStatus.Completed) notice = notice + $"\nTicket Number {{{rigJob.JobNumber}}}";
            }

            return notice;
        }

        private List<ContextMenu> SetDateContextMenus(RigJob rigJob, StyledCell styledCell)
        {
            List<ContextMenu> list = new List<ContextMenu>();
            switch (rigJob.JobLifeStatus)
            {
                case JobLifeStatus.Alerted: //2018/02/13 AW:we don't have this status yet
/*                    if (styledCell.PropertyValue != string.Empty)
                    {
                        list.Add(new ContextMenu()
                        {
                            MenuName = MenuJobdateBidLost,
                            ProcessingMode = ProcessingMode.PopsUpWindow,
                            //                            Parms = new List<string>() { },
                            //                            DialogName = "",
                            //                            ControllerName = RigBoardController,
                            //                            ActionName = ""
                        });
                        list.Add(new ContextMenu()
                        {
                            MenuName = MenuJobdateConfirmationCall,
                            ProcessingMode = ProcessingMode.PopsUpWindow,
                            Parms = new List<string>() { rigJob.CallSheetNumber.ToString(), "Confirm", rigJob.Id.ToString() },
                            DialogName = "",
                            ControllerName = RigBoardController,
                            ActionName = "UpdateJobDate"
                        });
                        list.Add(new ContextMenu()
                        {
                            MenuName = MenuJobdateFirstCall,
                            ProcessingMode = ProcessingMode.PopsUpWindow,
                            Parms = new List<string>() { rigJob.CallSheetNumber.ToString(), "Pending", rigJob.Id.ToString() },
                            DialogName = "",
                            ControllerName = RigBoardController,
                            ActionName = "UpdateJobDate"
                        });
                    }*/
                    break;
                case JobLifeStatus.Pending:
                    if (styledCell.PropertyValue != string.Empty)
                    {
                        list.Add(new ContextMenu()
                        {
                            MenuName = MenuJobdateOnHold,
                            ProcessingMode = ProcessingMode.PopsUpWindow,
                            Parms = new List<string>() { rigJob.CallSheetNumber.ToString(), "OnHold", rigJob.Id.ToString() },
                            DialogName = "",
                            ControllerName = RigBoardController,
                            ActionName = "UpdateJobDate"
                        });
                        list.Add(new ContextMenu()
                        {
                            MenuName = MenuJobdateConfirmationCall,
                            ProcessingMode = ProcessingMode.PopsUpWindow,
                            Parms = new List<string>() { rigJob.CallSheetNumber.ToString(), "Confirm", rigJob.Id.ToString() },
                            DialogName = "",
                            ControllerName = RigBoardController,
                            ActionName = "UpdateJobDate"
                        });
                        list.Add(new ContextMenu()
                        {
                            MenuName = MenuJobdateDelete,
                            ProcessingMode = ProcessingMode.PopsUpWindow,
                            Parms = new List<string>() { rigJob.Id.ToString(), rigJob.CallSheetNumber.ToString() },
                            DialogName = "",
                            ControllerName = RigBoardController,
                            ActionName = "DeleteRigJobById"
                        });
                        list.Add(new ContextMenu()
                        {
                            MenuName = MenuJobdateCancel,
                            ProcessingMode = ProcessingMode.PopsUpWindow,
                            Parms = new List<string>() { rigJob.Id.ToString(), rigJob.CallSheetNumber.ToString(), MenuJobdateCancel },
                            DialogName = "",
                            ControllerName = RigBoardController,
                            ActionName = "CancelRigJob"
                        });
                    }
                    break;
                case JobLifeStatus.Confirmed:
                    if (styledCell.PropertyValue != string.Empty)
                    {
                        list.Add(new ContextMenu()
                        {
                            MenuName = MenuJobdateReschedule,
                            ProcessingMode = ProcessingMode.PopsUpWindow,
                            Parms = new List<string>() { rigJob.CallSheetNumber.ToString(), "Reschedule", rigJob.Id.ToString() },
                            DialogName = "",
                            ControllerName = RigBoardController,
                            ActionName = "UpdateJobDate"
                        });
                        list.Add(new ContextMenu()
                        {
                            MenuName = MenuJobdateOnHold,
                            ProcessingMode = ProcessingMode.PopsUpWindow,
                            Parms = new List<string>() { rigJob.CallSheetNumber.ToString(), "OnHold", rigJob.Id.ToString() },
                            DialogName = "",
                            ControllerName = RigBoardController,
                            ActionName = "UpdateJobDate"
                        });
                        list.Add(new ContextMenu()
                        {
                            MenuName = MenuJobdateCancel,
                            ProcessingMode = ProcessingMode.PopsUpWindow,
                            Parms = new List<string>() { rigJob.Id.ToString(), rigJob.CallSheetNumber.ToString(), MenuJobdateCancel },
                            DialogName = "",
                            ControllerName = RigBoardController,
                            ActionName = "CancelRigJob"
                        });

                    }
                    break;
                case JobLifeStatus.Scheduled:
                    if (styledCell.PropertyValue != string.Empty)
                    {
                        list.Add(new ContextMenu()
                        {
                            MenuName = MenuJobdateCallOut,
                            ProcessingMode = ProcessingMode.PopsUpWindow,
                            Parms = new List<string>() { rigJob.CallSheetNumber.ToString(), "CallOut", rigJob.Id.ToString() },
                            DialogName = "",
                            ControllerName = RigBoardController,
                            ActionName = "UpdateJobDate"
                        });
                        list.Add(new ContextMenu()
                        {
                            MenuName = MenuJobdateReschedule,
                            ProcessingMode = ProcessingMode.PopsUpWindow,
                            Parms = new List<string>() { rigJob.CallSheetNumber.ToString(), "Reschedule", rigJob.Id.ToString() },
                            DialogName = "",
                            ControllerName = RigBoardController,
                            ActionName = "UpdateJobDate"
                        });
                        list.Add(new ContextMenu()
                        {
                            MenuName = MenuJobdateOnHold,
                            ProcessingMode = ProcessingMode.PopsUpWindow,
                            Parms = new List<string>() { rigJob.CallSheetNumber.ToString(), "OnHold", rigJob.Id.ToString() },
                            DialogName = "",
                            ControllerName = RigBoardController,
                            ActionName = "UpdateJobDate"
                        });
                        list.Add(new ContextMenu()
                        {
                            MenuName = MenuJobdateCancel,
                            ProcessingMode = ProcessingMode.PopsUpWindow,
                            Parms = new List<string>() { rigJob.Id.ToString(), rigJob.CallSheetNumber.ToString(), MenuJobdateCancel },
                            DialogName = "",
                            ControllerName = RigBoardController,
                            ActionName = "CancelRigJob"
                        });
                        list.Add(new ContextMenu()
                        {
                            MenuName = MenuJobdateComplete,
                            DialogName = "Post Job Check List",
                            ProcessingMode = ProcessingMode.PopsUpWindow,
                            Parms = new List<string>() { rigJob.Id.ToString() },

                            ControllerName = RigBoardController,
                            ActionName = "completejob"
                        });

                    }
                    break;
                case JobLifeStatus.Dispatched:
                    list.Add(new ContextMenu()
                    {
                        MenuName = MenuJobdateCancel,
                        ProcessingMode = ProcessingMode.PopsUpWindow,
                        Parms = new List<string>() { rigJob.Id.ToString(), rigJob.CallSheetNumber.ToString(), MenuJobdateCancel },
                        DialogName = "",
                        ControllerName = RigBoardController,
                        ActionName = "CancelRigJob"
                    });
                    list.Add(new ContextMenu()
                    {
                        MenuName = MenuJobdatePostpone,
                       
                        ProcessingMode = ProcessingMode.PopsUpWindow,
                        Parms = new List<string>() { rigJob.Id.ToString() },
                        ControllerName = RigBoardController,
                        ActionName = "PostponeJob"
                    });
                    break;
                case JobLifeStatus.InProgress:
                    if (styledCell.PropertyValue != string.Empty)
                    {
                        list.Add(new ContextMenu()
                        {
                            MenuName =MenuJobdateComplete,
                            DialogName = "Post Job Check List",
                            ProcessingMode = ProcessingMode.PopsUpWindow,
                            Parms = new List<string>() { rigJob.Id.ToString() },

                            ControllerName = RigBoardController,
                            ActionName = "CompleteJob"
                        });
                        list.Add(new ContextMenu()
                        {
                            MenuName = MenuJobdatePostpone,
                            ProcessingMode = ProcessingMode.PopsUpWindow,
                            Parms = new List<string>() { rigJob.Id.ToString() },
                            ControllerName = RigBoardController,
                            ActionName = "PostponeJob"
                        });
                    }
                    break;
                case JobLifeStatus.Completed:
                    break;
            }



            return list;
        }

        #endregion

        #region Compute Job information from entity

        private List<ContextMenu> SetJobContextMenus(RigJob rigJob)
        {
            if (rigJob.JobLifeStatus==JobLifeStatus.Pending||rigJob.JobLifeStatus==JobLifeStatus.Confirmed||rigJob.JobLifeStatus==JobLifeStatus.Scheduled||rigJob.JobLifeStatus==JobLifeStatus.Dispatched)
            {
                List<ContextMenu> list = new List<ContextMenu>
                {
                    new ContextMenu
                    {
                        MenuName = MenuAssignPlugLoadingHead,
                        ProcessingMode = ProcessingMode.PopsUpWindow,
                        Parms =
                            new List<string>
                            {
                                rigJob.Id.ToString(),
                                rigJob.CallSheetNumber.ToString()
                            },
                        ControllerName =RigBoardController,
                        ActionName = "AssignPlugLoadingHead"
                    },
                    new ContextMenu
                    {
                        MenuName = MenuAssignWitsBox,
                        ProcessingMode = ProcessingMode.PopsUpWindow,
                        Parms =
                            new List<string>
                            {
                                rigJob.Id.ToString(),
                                rigJob.CallSheetNumber.ToString()
                            },
                        ControllerName =RigJobController,
                        ActionName = "AssignWitsBox"
                    },
                    new ContextMenu
                    {
                        MenuName = MenuAssignNubbin,
                        ProcessingMode = ProcessingMode.PopsUpWindow,
                        Parms =
                            new List<string>
                            {
                                rigJob.Id.ToString(),
                                rigJob.CallSheetNumber.ToString()
                            },
                        ControllerName =RigJobController,
                        ActionName = "AssignNubbin"
                    },
                    new ContextMenu
                    {
                        MenuName = MenuAssignSwedge,
                        ProcessingMode = ProcessingMode.PopsUpWindow,
                        Parms =
                            new List<string>
                            {
                                rigJob.Id.ToString(),
                                rigJob.CallSheetNumber.ToString()
                            },
                        ControllerName =RigJobController,
                        ActionName = "AssignSwedge"
                    }
                };
                return list;
            }

            return null;
        }

        #endregion



        #region Compute bl information from entity


        private StyledCell GetPreflushStyledCell(string propertyName, RigJob rigJob, List<Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul> productHauls)
        {
            StyledCell styledCell = null;
            if (rigJob.JobLifeStatus == JobLifeStatus.None)
            {
                styledCell = new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache);
                styledCell.Style = styledCell.ComputeStyle(propertyName, null, this.GetDownRigSuffix(rigJob));
            }
            else
            {
                BlendProductHaulModel rigJobBl1 = this.BlendProductHaulModel1;
                if (string.IsNullOrEmpty(rigJobBl1?.Quantity))
                {
                    styledCell = new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache);
                }
                else
                {
                    styledCell = new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache)
                    {
                        PropertyValue = string.IsNullOrEmpty(rigJobBl1.Quantity) || rigJob.JobLifeStatus == (JobLifeStatus.Completed) || rigJob.JobLifeStatus == (JobLifeStatus.Canceled) ? string.Empty : rigJobBl1.Quantity + rigJobBl1.Units.Replace("m3","m\xB3")

                    };
                    styledCell = this.SetBlStyledCellByStatus(styledCell, rigJobBl1, rigJob);
                    styledCell.ContextMenus = styledCell.IsDisplayMenu ? (rigJob.JobLifeStatus == (JobLifeStatus.Completed) || rigJob.JobLifeStatus == (JobLifeStatus.Canceled) ? null : this.SetOneBlContextMenus(rigJob, rigJobBl1, productHauls)) : null;
                }
            }
            styledCell.IsNeedRowMerge = false;

            return styledCell;

        }
        private StyledCell GetBlendStyledCell(string propertyName, RigJob rigJob, List<Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul> productHauls)
        {
            StyledCell styledCell;
            if (rigJob.JobLifeStatus == JobLifeStatus.None)
            {
                styledCell = new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache);
                styledCell.Style = styledCell.ComputeStyle(propertyName, null, this.GetDownRigSuffix(rigJob));
            }
            else
            {

                BlendProductHaulModel rigJobBl2 = this.BlendProductHaulModel2;
                if (string.IsNullOrEmpty(rigJobBl2?.Quantity))
                {
                    styledCell = new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache);
                }
                else
                {
                    styledCell = new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache)
                    {
                        PropertyValue = string.IsNullOrEmpty(rigJobBl2.Quantity) || rigJob.JobLifeStatus==(JobLifeStatus.Completed) || rigJob.JobLifeStatus==(JobLifeStatus.Canceled) ? string.Empty : rigJobBl2.Quantity + rigJobBl2.Units.Replace("m3","m\xB3")
                    };
                    styledCell = this.SetBlStyledCellByStatus(styledCell, rigJobBl2, rigJob);
                    styledCell.ContextMenus = styledCell.IsDisplayMenu ? (rigJob.JobLifeStatus==(JobLifeStatus.Completed) || rigJob.JobLifeStatus==(JobLifeStatus.Canceled) ? null : this.SetOneBlContextMenus(rigJob, rigJobBl2, productHauls)) : null;
                }
            }
            styledCell.IsNeedRowMerge = false;

            return styledCell;
        }

        private StyledCell GetDisplacementStyledCell(string propertyName, RigJob rigJob, List<Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul> productHauls)
        {
            StyledCell styledCell;
            if (rigJob.JobLifeStatus == JobLifeStatus.None)
            {
                styledCell = new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache);
                styledCell.Style = styledCell.ComputeStyle(propertyName, null, this.GetDownRigSuffix(rigJob));
            }
            else
            {

                BlendProductHaulModel rigJobBl2 = this.BlendProductHaulModel3;
                if (string.IsNullOrEmpty(rigJobBl2?.Quantity))
                {
                    styledCell = new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache);
                }
                else
                {
                    styledCell = new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache)
                    {
                        PropertyValue = string.IsNullOrEmpty(rigJobBl2.Quantity) || rigJob.JobLifeStatus == (JobLifeStatus.Completed) || rigJob.JobLifeStatus == (JobLifeStatus.Canceled) ? string.Empty : rigJobBl2.Quantity + rigJobBl2.Units.Replace("m3","m\xB3")
                    };
                    styledCell = this.SetBlStyledCellByStatus(styledCell, rigJobBl2, rigJob);
                    styledCell.ContextMenus = styledCell.IsDisplayMenu ? (rigJob.JobLifeStatus == (JobLifeStatus.Completed) || rigJob.JobLifeStatus == (JobLifeStatus.Canceled) ? null : this.SetOneBlContextMenus(rigJob, rigJobBl2, productHauls)) : null;
                }
            }
            styledCell.IsNeedRowMerge = false;

            return styledCell;
        }
/*
        private StyledCell GetBl3StyledCell(string propertyName, RigJob rigJob, List<Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul> productHauls)
        {
            StyledCell styledCell;
            if (rigJob.JobLifeStatus == JobLifeStatus.None)
            {
                styledCell = new StyledCell(propertyName, this, this.LoggedUser, this._memoryCache);
                styledCell.Style = styledCell.ComputeStyle(propertyName, null, this.GetDownRigSuffix(rigJob));
                styledCell = this.SetCellMerge(styledCell);

                return styledCell;
            }

            List<BlendProductHaulModel> rigJobBl3 = this.BlendProductHaulModel3;
            if (rigJobBl3 == null || rigJobBl3.Count.Equals(0))
            {
                styledCell = new StyledCell(propertyName, this, this.LoggedUser, this._memoryCache);
                styledCell = this.SetCellMerge(styledCell);

                return styledCell;
            }
            else
            {
                if (rigJobBl3.Count.Equals(1))
                {
                    BlendProductHaulModel blendSection = this.BlendProductHaulModel3.FirstOrDefault();
                    if (!string.IsNullOrEmpty(blendSection?.Quantity))
                    {
                        styledCell = new StyledCell(propertyName, this, this.LoggedUser, this._memoryCache)
                        {
                            PropertyValue = string.IsNullOrEmpty(blendSection.Quantity) || rigJob.JobLifeStatus==(JobLifeStatus.Completed) || rigJob.JobLifeStatus==(JobLifeStatus.Canceled) ? string.Empty : blendSection.Quantity + "t"
                        };
                        styledCell = this.SetBlStyledCellByStatus(styledCell, blendSection, rigJob);
                        styledCell.ContextMenus = styledCell.IsDisplayMenu ? (rigJob.JobLifeStatus==(JobLifeStatus.Completed) || rigJob.JobLifeStatus==(JobLifeStatus.Canceled) ? null : this.SetOneBlContextMenus(rigJob, blendSection, productHauls)) : null;
                        styledCell = this.SetCellMerge(styledCell);

                        return styledCell;
                    }

                    return this.SetCellMerge(new StyledCell(propertyName, this, this.LoggedUser, this._memoryCache));
                }

                StyledCell styledCellMore = new StyledCell(propertyName, this, this.LoggedUser, this._memoryCache) {PropertyValue = rigJob.JobLifeStatus==(JobLifeStatus.Completed) || rigJob.JobLifeStatus==(JobLifeStatus.Canceled) ? string.Empty : More,};
                string notice = string.Empty;
                foreach (BlendProductHaulModel item in rigJobBl3)
                {
                    if (item != null)
                    {
                        notice += this.GetBlHoverNoticeByStatus(item, rigJob);
                    }
                }

                styledCellMore.Notice = notice;
                styledCellMore = this.SetCellMerge(styledCellMore);
                styledCellMore.ContextMenus = styledCellMore.IsDisplayMenu ? (rigJob.JobLifeStatus==(JobLifeStatus.Completed) || rigJob.JobLifeStatus==(JobLifeStatus.Canceled) ? null : this.SetMoreBlContextMenus(rigJob)) : null;

                return styledCellMore;
            }
        }
        */
        private StyledCell SetBlStyledCellByStatus(StyledCell styledCell, BlendProductHaulModel bl, RigJob rigJob)
        {
            if (bl != null)
            {
                styledCell.Notice = this.GetBlHoverNoticeByStatus(bl, rigJob);
                switch (bl.BlendProductHaulStatus)
                {
                    case BlendProductHaulStatus.NeedAHaul:
                        styledCell.Style = StyleBlNeedahaul;
                        break;
                    case BlendProductHaulStatus.PartialScheduled:
                        styledCell.Style = StyleBlPartialScheduled;
                        break;
                    case BlendProductHaulStatus.FullyScheduled:
                        styledCell.Style = StyleBlHaulScheduled;
                        break;
                    case BlendProductHaulStatus.PartialOnLocation:
                        styledCell.Style = StyleBlPartialOnLocation;
                        break;
                    case BlendProductHaulStatus.FullyOnLocation:
                        styledCell.Style = StyleBlOnLocation;
                        break;
                    case BlendProductHaulStatus.GoWithCrew:
                        styledCell.Style = StyleBlGowithcrew;
                        break;
                }
            }

            return styledCell;
        }

        private string GetBlHoverNoticeByStatus(BlendProductHaulModel bl, RigJob rigJob)
        {
            StringBuilder notice = new StringBuilder();

            notice.AppendFormat("Stage Number:{0}", bl.StageNumber);
            notice.AppendLine();
            notice.AppendFormat("Category:{0}", bl.Category);
            notice.AppendLine();
            //notice.AppendFormat("Base Blend Name:{0}", bl.BaseBlendName);
            //Blend:Proteus Core + 0.25% CFR-2 + 2% Cacl2 + 2% FWC-2 + 0.15% CDF-6P
            notice.AppendFormat("Blend:{0}", bl.BaseBlendName);
            foreach (var blend in bl.BlendAdditiveSections)
            {
                if(blend!=null)
                    notice.AppendFormat(" + {0}{1} {2}", blend.Amount, blend.AdditiveAmountUnit.Description, blend.AdditiveType.Name);
            }

            notice.AppendLine();
            notice.AppendFormat("Amount:{0}", bl.Quantity ?? string.Empty);
            notice.AppendLine();
            notice.AppendFormat("Units:{0}", bl.Units ?? string.Empty);
            notice.AppendLine();
            notice.AppendFormat("Density:{0}", bl.Density ?? string.Empty);
            notice.AppendLine();

            return rigJob.JobLifeStatus==(JobLifeStatus.Completed) || rigJob.JobLifeStatus==(JobLifeStatus.Canceled) ? string.Empty : notice.ToString();
        }

        //Apirl 25, 2024 Tongtao 192_PR_StandardizeMTSDownloadName: add mts print menu
        private List<ContextMenu> SetOneBlContextMenus(RigJob rigJob, BlendProductHaulModel bl, List<Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul> productHauls)
        {
            List<ContextMenu> list = new List<ContextMenu>();

            ContextMenu fristContextMenu = new ContextMenu()
            {
                MenuName = MenuNeedAHaul,
                ProcessingMode = ProcessingMode.NoPopsUpWindow,
                Parms = new List<string>() {bl.BlendSectionId.ToString(),true.ToString()},
                ControllerName = ProductHaulController,
                ActionName = "NeedHaul"
            };

            this.IsHaveGowithCrew(fristContextMenu, bl.IsNeedHaul, productHauls);

            if (bl.IsNeedHaul)
            {
                if (bl.ProductHaulForBlendModels == null || bl.ProductHaulForBlendModels.Count.Equals(0))
                {
                    fristContextMenu.MenuName = MenuDoNotNeedAHaul;
                    fristContextMenu.Parms = new List<string>() {bl.BlendSectionId.ToString(), false.ToString()};

                }
                else
                {
                    fristContextMenu.MenuName = MenuDoNotNeedAHaul;
                    fristContextMenu.IsDisabled = true;
                }
            }

            ContextMenu secondContextMenu = this.SetScheduleProductHaulMenuForBl(bl, rigJob, true, MenuScheduleProductHaul, !bl.IsNeedHaul);

            ContextMenu thirdContextMenu = new ContextMenu()
            {
                MenuName = MenuOnLocation,
                ProcessingMode = ProcessingMode.HaveNextMenu,
                MenuList = this.SetOnLocationMenuForBl(bl)
            };
//            this.IsAllOnLocation(thirdContextMenu, productHauls);

            ContextMenu cancelContextMenu = new ContextMenu()
            {
                MenuName = MenuCancelProductHaul,
                ProcessingMode = ProcessingMode.HaveNextMenu,
                MenuList = this.SetCancelProductHaulMenuForBl(bl, rigJob)
            };
//            this.IsAllOnLocation(cancelContextMenu, productHauls);

            ContextMenu reScheduleContextMenu = new ContextMenu()
            {
                MenuName = MenuReScheduleProductHaul,
                ProcessingMode = ProcessingMode.HaveNextMenu,
                MenuList = this.SetReScheduleProductHaulMenuForBl(bl, rigJob)

            };
//            this.IsAllOnLocation(reScheduleContextMenu, productHauls);


            ContextMenu blendContextMenu = this.SetUpdateTheBlendMenuForBl(bl, true, MenuUpdateBlend);

            ContextMenu scheduleBlendMenu = this.SetScheduleBlendMenuForBl(bl, rigJob, true, MenuScheduleBlend);
            ContextMenu reScheduleBlendMenu = new ContextMenu()
            {
                MenuName = MenuRescheduleBlend,
                ProcessingMode = ProcessingMode.HaveNextMenu,
                MenuList = this.SetReScheduleBlendMenuForBl(bl, rigJob)
                
            };
            ContextMenu cancelBlendMenu = new ContextMenu()
            {
                MenuName = MenuCancelBlend,
                ProcessingMode = ProcessingMode.HaveNextMenu,
                MenuList = this.SetCancelBlendMenuForBl(bl, rigJob)
            };
            ContextMenu haulBlendMenu = new ContextMenu()
            {
                MenuName = MenuHaulBlend,
                ProcessingMode = ProcessingMode.HaveNextMenu,
                MenuList = this.SetHaulBlendMenuForBl(bl, rigJob)
            };

            ContextMenu printMtsMenu = new ContextMenu()
            {
                MenuName = MenuPrintMTS,
                ProcessingMode = ProcessingMode.HaveNextMenu,
                MenuList = this.SetPrintMTSMenuForBin(productHauls, true)
            };


            list.Add(fristContextMenu);
            list.Add(secondContextMenu);
            list.Add(reScheduleContextMenu);
            list.Add(cancelContextMenu);
            list.Add(thirdContextMenu);
            list.Add(blendContextMenu);
            list.Add(printMtsMenu);

            list.Add(scheduleBlendMenu);
            list.Add(reScheduleBlendMenu);
            list.Add(cancelBlendMenu);
            list.Add(haulBlendMenu);
            return list;
        }





        private ContextMenu SetNeedHaulMenuForBl(BlendProductHaulModel bl)
        {
            string menu = (bl.BaseBlendName + "_" + bl.Quantity).Replace("'", "%%%");
            ContextMenu contextMenu = new ContextMenu()
            {
                MenuName = menu,
                ProcessingMode = ProcessingMode.NoPopsUpWindow,
                Parms = new List<string>() {bl.BlendSectionId.ToString(),true.ToString()},
                ControllerName = ProductHaulController,
                ActionName = "NeedHaul",
            };

            if (bl.IsNeedHaul)
            {
                contextMenu.IsDisabled = true;
            }

            return contextMenu;
        }

        private void IsHaveGowithCrew(ContextMenu menu, bool isNeedHaul,List<Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul> productHauls)
        {
            bool isHaveGowithCrew = false;
            if (productHauls != null && productHauls.Count > 0)
            {
                foreach (Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul productHaul in productHauls)
                {
                    if (productHaul != null && productHaul.ShippingLoadSheets != null && productHaul.ShippingLoadSheets.Count > 0)
                    {
                        if (productHaul.ShippingLoadSheets.Any(p => p.IsGoWithCrew == true))
                        {
                            isHaveGowithCrew = true;
                            break;
                        }
                    }
                }
            }

            if (!isNeedHaul && isHaveGowithCrew)
            {
                menu.IsDisabled = true;
            }
        }

        private void IsAllOnLocation(ContextMenu menu, List<Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul> productHauls)
        {
            bool isAllOnLocation = true;
            if (productHauls != null && productHauls.Count > 0)
            {
                if (productHauls.Any(p => p.ProductHaulLifeStatus != ProductHaulStatus.OnLocation))
                {
                    isAllOnLocation = false;
                }
                
            }

            if (isAllOnLocation)
            {
                menu.IsDisabled = true;
            }
        }


        private ContextMenu SetDontNeedHaulMenuForBl(BlendProductHaulModel bl)
        {
            string menu = (bl.BaseBlendName + "_" + bl.Quantity).Replace("'", "%%%");
            ContextMenu contextMenu = new ContextMenu()
            {
                MenuName = menu,
                ProcessingMode = ProcessingMode.NoPopsUpWindow,
                Parms = new List<string>() { bl.BlendSectionId.ToString(), false.ToString() },
                ControllerName = ProductHaulController,
                ActionName = "NeedHaul",
            };

            if ((bl.IsNeedHaul && bl.ProductHaulForBlendModels != null && bl.ProductHaulForBlendModels.Count>0)|| !bl.IsNeedHaul)
            {
                contextMenu.IsDisabled = true;
            }

            return contextMenu;
        }

        private ContextMenu SetScheduleProductHaulMenuForBl(BlendProductHaulModel bl, RigJob rigJob, bool isOnlyOne,string menuName, bool isDisabled)
        {
            string menu = (bl.BaseBlendName + "_" + bl.Quantity).Replace("'", "%%%");
            ContextMenu contextMenu = new ContextMenu()
            {
                MenuName = isOnlyOne ? menuName : menu,
                ProcessingMode = ProcessingMode.PopsUpWindow,
                Parms = new List<string>() { rigJob?.CallSheetNumber.ToString(), rigJob?.CallSheetId.ToString(), bl.BlendSectionId.ToString(), rigJob?.Rig.Id.ToString(),rigJob?.Id.ToString() },
                ControllerName = ProductHaulController,
                ActionName = "ScheduleProductHaulFromRigJobBlend",
            };
            return contextMenu;
        }

        private List<ContextMenu> SetReScheduleProductHaulMenuForBl(BlendProductHaulModel bl, RigJob rigJob)
        {
            List<ContextMenu> list = new List<ContextMenu>();
            if (bl?.ProductHaulForBlendModels != null && bl.ProductHaulForBlendModels.Count > 0)
            {
                foreach (ProductHaulForBlendModel item in bl.ProductHaulForBlendModels)
                {
                    if (item.ProductHaulId==0) { continue; }
                    string menu = "Blend in Testing";
                    if (item.ProductHaulId != 0)
                    {
                        menu = item.IsGoWithCrew
                            ? MenuGowithCrew
                            : Utility.GetDateTimeValue(item.ExpectedOnLocationTime, ShortTimeFormat);
                     
                        menu = item.CrewName + "-" + menu;
                    }
                    ContextMenu contextMenu = new ContextMenu()
                    {
                        MenuName = menu.Replace("'", "%%%"),
                        ProcessingMode = ProcessingMode.PopsUpWindow,
                        Parms = new List<string>() { item.ProductHaulId.ToString() ,bl.CallSheetId.ToString(),rigJob.Id.ToString()},
                        ControllerName = ProductHaulController,
                        ActionName = "RescheduleProductHaul",
                        IsDisabled = item.DisableRescheduleMenu ,
                        MenuStyle = GetProductHualMenuStyle(item.ProductHaulLifeStatus),
                        MenuTips = item.ProductHaulLifeStatus.ToString(),
                        MenuList = this.SetReScheduleProductHaulLoadMenuForBl(item, rigJob, bl.BlendSectionId)
                    };
                    list.Add(contextMenu);
                }
            }
            return list;
        }

        private List<ContextMenu> SetReScheduleProductHaulLoadMenuForBl(ProductHaulForBlendModel productHaulForBlendModel, RigJob rigJob, int blendSectionId)
        {
            List<ContextMenu> list = new List<ContextMenu>();

            if (productHaulForBlendModel?.ProductHaulLoadModels != null && productHaulForBlendModel.ProductHaulLoadModels.Count > 0)
            {
                foreach (ProductHaulLoadModel item in productHaulForBlendModel.ProductHaulLoadModels)
                {
                    double enteredBlendWeight = item.IsTotalBlendTonnage ? item.TotalBlendWeight : item.BaseBlendWeight;
                    string menu =  item.BaseBlendName + " - " + item.HaulAmount / 1000 + "t"; //+" - " + enteredBlendWeight / 1000 +"t";
                    string tips = item.Status.ToString();
                    if (item.BlendShippingStatus != BlendShippingStatus.Empty)
                    {
                        tips += "|" + item.BlendShippingStatus.ToString();
                    }
                    if (item.BlendTestingStatus != BlendTestingStatus.None)
                    {
                        tips += "|" + item.BlendTestingStatus.ToString();
                    }
                    ContextMenu contextMenu = new ContextMenu()
                    {
                        MenuName = menu.Replace("'", "%%%"),
                        ProcessingMode = ProcessingMode.PopsUpWindow,
                        Parms = new List<string>() { item.ProductHaulLoadId.ToString(), rigJob?.Rig.Id.ToString(), rigJob?.CallSheetId.ToString(), rigJob?.Id.ToString(), productHaulForBlendModel.ProductHaulId.ToString() },
                        ControllerName = ProductHaulController,
                        ActionName = "RescheduleProductHaulLoad",
                        MenuStyle = item.BlendSectionId == blendSectionId ? GetProductHualLoadMenuStyle(item.Status) : "",
                        MenuTips = tips,
                        IsDisabled = true
                        // productHaulForBlendModel.DisableRescheduleMenu || blendSectionId != item.BlendSectionId || item.Status != ProductHaulLoadStatus.Scheduled
                    };
                    list.Add(contextMenu);
                }
            }
            return list;
        }

        private ContextMenu SetUpdateTheBlendMenuForBl(BlendProductHaulModel bl, bool isOnlyOne, string menuName)
        {
            string menu = (bl.BaseBlendName + "_" + bl.Quantity).Replace("'", "%%%");
            ContextMenu contextMenu = new ContextMenu()
            {
                MenuName = isOnlyOne ? menuName : menu,
                ProcessingMode = ProcessingMode.PopsUpWindow,
                Parms = new List<string>() { bl.BlendSectionId.ToString() },
                ControllerName = ProductHaulController,
                ActionName = "GetBlendSectionById"
            };
            return contextMenu;
        }

        private List<ContextMenu> SetCancelProductHaulMenuForBl(BlendProductHaulModel bl,RigJob rigJob)
        {
            List<ContextMenu> list = new List<ContextMenu>();
            
            if (bl?.ProductHaulForBlendModels != null && bl.ProductHaulForBlendModels.Count > 0)
            {
                foreach (ProductHaulForBlendModel item in bl.ProductHaulForBlendModels)
                {
                    if (item.ProductHaulId==0) { continue; }
                    string menu = "Blend in Testing";
                    if (item.ProductHaulId>0)
                    {
                        menu = item.IsGoWithCrew ? MenuGowithCrew : Utility.GetDateTimeValue(item.ExpectedOnLocationTime, LongDateTimeFormat);
                        menu = item.CrewName + "-" + menu;
                    }

                    ContextMenu contextMenu = new ContextMenu()
                    {
                        MenuName = menu.Replace("'", "%%%"),
                        ProcessingMode = ProcessingMode.PopsUpWindow,
                        Parms = new List<string>() { item.ProductHaulId.ToString(), rigJob.Id.ToString() },
                        ControllerName = ProductHaulController,
                        ActionName = "CancelProductHaul",
                        IsDisabled = item.DisableCancelMenu||item.ProductHaulId==0,
                        MenuStyle =  GetProductHualMenuStyle(item.ProductHaulLifeStatus),
                        MenuTips = item.ProductHaulLifeStatus.ToString(),
                        MenuList = item?.ProductHaulLoadModels != null && item.ProductHaulLoadModels.Count > 0 ?this.SetCancelProductHaulLoadMenuForBl(item, bl.BlendSectionId) : null,
                    };
                    list.Add(contextMenu);
                }
            }

            return list;
        }

        private List<ContextMenu> SetCancelProductHaulLoadMenuForBl(ProductHaulForBlendModel productHaulForBlendModel, int blendSectionId)
        {
            List<ContextMenu> list = new List<ContextMenu>();

            if (productHaulForBlendModel?.ProductHaulLoadModels != null && productHaulForBlendModel.ProductHaulLoadModels.Count > 0)
            {
                foreach (ProductHaulLoadModel item in productHaulForBlendModel.ProductHaulLoadModels)
                {
                    //double enteredBlendWeight = item.IsTotalBlendTonnage ? item.TotalBlendWeight : item.BaseBlendWeight;
                    string menu = item.BaseBlendName + " - " + item.HaulAmount / 1000 + "t";
                    string tips = item.Status.ToString();
                    if (item.BlendShippingStatus != BlendShippingStatus.Empty)
                    {
                        tips += "|" + item.BlendShippingStatus.ToString();
                    }
                    if (item.BlendTestingStatus != BlendTestingStatus.None)
                    {
                        tips += "|" + item.BlendTestingStatus.ToString();
                    }
                    ContextMenu contextMenu = new ContextMenu()
                    {
                        MenuName = menu.Replace("'", "%%%"),
                        ProcessingMode = ProcessingMode.PopsUpWindow,
                        Parms = new List<string>() { item.ProductHaulLoadId.ToString() },
                        ControllerName = ProductHaulController,
                        ActionName = "CancelProductHaulLoad",
                        MenuStyle = item.BlendSectionId == blendSectionId ? GetProductHualLoadMenuStyle(item.Status) : "",
                        MenuTips =  tips,
                        IsDisabled = true
                    };
                    list.Add(contextMenu);
                }
            }

            return list;
        }

        private List<ContextMenu> SetOnLocationMenuForBl(BlendProductHaulModel bl)
        {
            List<ContextMenu> list = new List<ContextMenu>();
            if (bl?.ProductHaulForBlendModels != null && bl.ProductHaulForBlendModels.Count > 0)
            {
                foreach (ProductHaulForBlendModel item in bl.ProductHaulForBlendModels)
                {
                    if(item.ProductHaulId==0) continue;
                    string menu = item.IsGoWithCrew ? MenuGowithCrew : Utility.GetDateTimeValue(item.ExpectedOnLocationTime, LongDateTimeFormat);
                    menu = item.CrewName + "-" + menu;
                    
                    ContextMenu contextMenu = new ContextMenu()
                    {
                        MenuName = menu,
                        ProcessingMode = ProcessingMode.PopsUpWindow,
                        Parms = new List<string>() {item.ProductHaulId.ToString(), bl.BlendSectionId.ToString() },
                        ControllerName = ProductHaulController,
                        ActionName = "OnLocationProductHaul",
                        IsDisabled = true, ////Disable on location ability to allow user use Job Board more
//                        IsDisabled = item.ProductHaulLifeStatus!=ProductHaulStatus.Scheduled && item.ProductHaulLifeStatus != ProductHaulStatus.InProgress,
                        MenuStyle=GetProductHualMenuStyle(item.ProductHaulLifeStatus),
                        MenuTips = item.ProductHaulLifeStatus.ToString(),
                        MenuList = this.SetOnLocationProductHaulLoadMenuForBl(item, bl.BlendSectionId)
                    };
                    list.Add(contextMenu);
                }
            }

            return list;
        }
        
        private List<ContextMenu> SetOnLocationProductHaulLoadMenuForBl(ProductHaulForBlendModel productHaulForBlendModel, int blendSectionId)
        {
            List<ContextMenu> list = new List<ContextMenu>();

            if (productHaulForBlendModel?.ProductHaulLoadModels != null && productHaulForBlendModel.ProductHaulLoadModels.Count > 0)
            {
                foreach (ProductHaulLoadModel item in productHaulForBlendModel.ProductHaulLoadModels)
                {
                    //double enteredBlendWeight = item.IsTotalBlendTonnage ? item.TotalBlendWeight : item.BaseBlendWeight;
                    string menu = item.BaseBlendName + " - " + item.HaulAmount / 1000 + "t";
                    string tips = item.Status.ToString();
                    if (item.BlendShippingStatus != BlendShippingStatus.Empty)
                    {
                        tips += "|" + item.BlendShippingStatus.ToString();
                    }
                    if (item.BlendTestingStatus != BlendTestingStatus.None)
                    {
                        tips += "|" + item.BlendTestingStatus.ToString();
                    }
                    ContextMenu contextMenu = new ContextMenu()
                    {
                        MenuName = menu.Replace("'", "%%%"),
                        ProcessingMode = ProcessingMode.PopsUpWindow,
                        Parms = new List<string>() { item.ProductHaulLoadId.ToString(),productHaulForBlendModel.ProductHaulId.ToString() },
                        ControllerName = ProductHaulController,
                        ActionName = "OnLocationProdHaulLoad",
                        //IsDisabled = item.Status != ProductHaulLoadStatus.Scheduled||item.ShippingStatus==ShippingStatus.OnLocation || item.BlendSectionId != blendSectionId,
                        IsDisabled = true,//Disable on location ability to allow user use Job Board more
                        MenuStyle = item.BlendSectionId == blendSectionId?GetProductHualLoadMenuStyle(item.Status):"",
                        MenuTips = tips


                    };
                    list.Add(contextMenu);
                }
            }

            return list;
        }
        private ContextMenu SetScheduleBlendMenuForBl(BlendProductHaulModel bl, RigJob rigJob, bool isOnlyOne, string menuName)
        {
            string menu = (bl.BaseBlendName + "_" + bl.Quantity).Replace("'", "%%%");
            ContextMenu contextMenu = new ContextMenu()
            {
                MenuName = isOnlyOne ? menuName : menu,
                ProcessingMode = ProcessingMode.PopsUpWindow,
                Parms = new List<string>() { rigJob?.CallSheetNumber.ToString(), rigJob?.CallSheetId.ToString(), bl.BlendSectionId.ToString(), rigJob?.Rig.Id.ToString(), rigJob?.Id.ToString() },
                ControllerName = ProductHaulController,
                ActionName = "ScheduleBlendFromRigJobBlend",
                IsHaveSplitLine = true
            };

            return contextMenu;
        }


        private List<ContextMenu> SetReScheduleBlendMenuForBl(BlendProductHaulModel bl, RigJob rigJob)
        {
            List<ContextMenu> list = new List<ContextMenu>();

            if (bl?.ProductHaulForBlendModels != null && bl.ProductHaulForBlendModels.Count > 0)
            {
                List<int> ids = new List<int>();
                foreach (ProductHaulForBlendModel item in bl.ProductHaulForBlendModels)
                {
                    //if (item.ProductHaulId>0 && item.ProductHaulLoadModels.FirstOrDefault(p => p.BlendShippingStatus != BlendShippingStatus.ParitialHaulScheduled) == null) { continue; }
                    if (item.ProductHaulLoadModels != null && item.ProductHaulLoadModels.Count > 0)
                    {
                        foreach (ProductHaulLoadModel productHaulLoadModel in item.ProductHaulLoadModels)
                        {
                            if(ids.Contains(productHaulLoadModel.ProductHaulLoadId))
                            {
                                continue;
                            }
                            else
                            {
                                ids.Add(productHaulLoadModel.ProductHaulLoadId);
                            }
                            //if (productHaulLoadModel.BlendShippingStatus != BlendShippingStatus.Empty&& productHaulLoadModel.BlendShippingStatus != BlendShippingStatus.ParitialHaulScheduled) continue;
                            double enteredBlendWeight = productHaulLoadModel.IsTotalBlendTonnage ? productHaulLoadModel.TotalBlendWeight : productHaulLoadModel.BaseBlendWeight;
                            string loadBaseBlendName = productHaulLoadModel.BaseBlendName;
                            //Change the formatting precision from two decimal places to three decimal places
                            string menu = loadBaseBlendName + " - " + Math.Round(enteredBlendWeight / 1000, 3) + "t";
                            string tips = productHaulLoadModel.Status.ToString();
                            if (productHaulLoadModel.BlendShippingStatus != BlendShippingStatus.Empty)
                            {
                                tips += "|" + productHaulLoadModel.BlendShippingStatus.ToString();
                            }
                            if (productHaulLoadModel.BlendTestingStatus != BlendTestingStatus.None)
                            {
                                tips += "|" + productHaulLoadModel.BlendTestingStatus.ToString();
                            }
                            ContextMenu contextMenu = new ContextMenu()
                            {
                                MenuName = menu.Replace("'", "%%%"),
                                ProcessingMode = ProcessingMode.PopsUpWindow,
                                Parms = new List<string>() { productHaulLoadModel.ProductHaulLoadId.ToString(), rigJob?.Rig.Id.ToString(), rigJob?.CallSheetId.ToString(), rigJob?.Id.ToString() },
                                ControllerName = ProductHaulController,
                                ActionName = "RescheduleBlendFromRigJobBlend",
                                MenuStyle = productHaulLoadModel.BlendSectionId == bl.BlendSectionId ? GetProductHualLoadMenuStyle(productHaulLoadModel.Status) : "",
                                MenuTips = tips,
                                IsDisabled = productHaulLoadModel.Status != ProductHaulLoadStatus.Scheduled||productHaulLoadModel.BlendShippingStatus==BlendShippingStatus.OnLocation
                            };
                            list.Add(contextMenu);
                        }
                    }
                          
                }
            }

            return list;
        }


        private List<ContextMenu> SetCancelBlendMenuForBl(BlendProductHaulModel bl, RigJob rigJob)
        {
            List<ContextMenu> list = new List<ContextMenu>();

            if (bl?.ProductHaulForBlendModels != null && bl.ProductHaulForBlendModels.Count > 0)
            {
                List<int> ids = new List<int>();
                foreach (ProductHaulForBlendModel item in bl.ProductHaulForBlendModels)
                {
                    //if (item.ProductHaulId>0) { continue; }
                    if (item.ProductHaulLoadModels != null && item.ProductHaulLoadModels.Count > 0)
                    {
                        foreach (ProductHaulLoadModel productHaulLoadModel in item.ProductHaulLoadModels)
                        {
                            if (ids.Contains(productHaulLoadModel.ProductHaulLoadId))
                            {
                                continue;
                            }
                            else
                            {
                                ids.Add(productHaulLoadModel.ProductHaulLoadId);
                            }
                            //if (productHaulLoadModel.BlendShippingStatus != BlendShippingStatus.Empty && productHaulLoadModel.BlendShippingStatus != BlendShippingStatus.ParitialHaulScheduled) continue;
                            double enteredBlendWeight = productHaulLoadModel.IsTotalBlendTonnage ? productHaulLoadModel.TotalBlendWeight : productHaulLoadModel.BaseBlendWeight;
                            //Change the formatting precision from two decimal places to three decimal places
                            string menu = productHaulLoadModel.BaseBlendName + " - " + Math.Round(enteredBlendWeight / 1000, 3) + "t";

                            string tips = productHaulLoadModel.Status.ToString();
                            if (productHaulLoadModel.BlendShippingStatus != BlendShippingStatus.Empty)
                            {
                                tips += "|" + productHaulLoadModel.BlendShippingStatus.ToString();
                            }
                            if (productHaulLoadModel.BlendTestingStatus != BlendTestingStatus.None)
                            {
                                tips += "|" + productHaulLoadModel.BlendTestingStatus.ToString();
                            }
                            ContextMenu contextMenu = new ContextMenu()
                            {
                                MenuName = menu.Replace("'", "%%%"),
                                ProcessingMode = ProcessingMode.PopsUpWindow,
                                Parms = new List<string>() { productHaulLoadModel.ProductHaulLoadId.ToString(),productHaulLoadModel.ProductHaulId.ToString() },
                                ControllerName = ProductHaulController,
                                ActionName = "CancelBlendRequest",
                                MenuStyle = productHaulLoadModel.BlendSectionId == bl.BlendSectionId ? GetProductHualLoadMenuStyle(productHaulLoadModel.Status) : "",
                                MenuTips = tips,
                                IsDisabled = productHaulLoadModel.Status != ProductHaulLoadStatus.Scheduled||productHaulLoadModel.BlendShippingStatus==BlendShippingStatus.OnLocation
                            };
                            list.Add(contextMenu);
                        }
                    }


                }
            }

            return list;
        }

        private List<ContextMenu> SetHaulBlendMenuForBl(BlendProductHaulModel bl, RigJob rigJob)
        {
            List<ContextMenu> list = new List<ContextMenu>();

            if (bl?.ProductHaulForBlendModels != null && bl.ProductHaulForBlendModels.Count > 0)
            {
                List<int> ids = new List<int>();
                foreach (ProductHaulForBlendModel item in bl.ProductHaulForBlendModels)
                {
                    //if (item.ProductHaulId>0) { continue; }
                    if (item.ProductHaulLoadModels != null && item.ProductHaulLoadModels.Count > 0)
                    {
                        foreach (ProductHaulLoadModel productHaulLoadModel in item.ProductHaulLoadModels)
                        {
                            if (ids.Contains(productHaulLoadModel.ProductHaulLoadId))
                            {
                                continue;
                            }
                            else
                            {
                                ids.Add(productHaulLoadModel.ProductHaulLoadId);
                            }
                            //if (productHaulLoadModel.BlendShippingStatus != BlendShippingStatus.Empty) continue;
                            double enteredBlendWeight = productHaulLoadModel.IsTotalBlendTonnage ? productHaulLoadModel.TotalBlendWeight : productHaulLoadModel.BaseBlendWeight;

                            //Change the formatting precision from two decimal places to three decimal places
                            string menu = productHaulLoadModel.BaseBlendName + " - " +   Math.Round(enteredBlendWeight / 1000, 3) + "t";
                            string tips = productHaulLoadModel.Status.ToString();
                            if (productHaulLoadModel.BlendShippingStatus != BlendShippingStatus.Empty)
                            {
                                tips += "|" + productHaulLoadModel.BlendShippingStatus.ToString();
                            }
                            if (productHaulLoadModel.BlendTestingStatus != BlendTestingStatus.None)
                            {
                                tips += "|" + productHaulLoadModel.BlendTestingStatus.ToString();
                            }   
                            if (productHaulLoadModel.BaseBlendName.IndexOf("'")>0) 
                            {
                                var name = "Haul Blend - " + productHaulLoadModel.BaseBlendName.Replace("'", "\'");
                            }
                            ContextMenu contextMenu = new ContextMenu()
                            {
                                MenuName = menu.Replace("'", "%%%"),
                                DialogName = "Haul Blend - " + productHaulLoadModel.BaseBlendName.Replace("'", "%%%"),
                                ProcessingMode = ProcessingMode.PopsUpWindow,
                                Parms = new List<string>() { productHaulLoadModel.ProductHaulLoadId.ToString(), productHaulLoadModel.ProductHaulId.ToString(), rigJob.Id.ToString(), rigJob.Rig.Id.ToString() },
                                ControllerName = ProductHaulController,
                                ActionName = "HaulBlendFromRigJobBlend",
                                MenuStyle = productHaulLoadModel.BlendSectionId == bl.BlendSectionId ? GetProductHualLoadMenuStyle(productHaulLoadModel.Status) : "",
                                MenuTips = tips,
                                //Nov 24, 2023 Tongtao P45_Q4_130: set haulBlend menu abled 
                                IsDisabled = !(productHaulLoadModel.Status == ProductHaulLoadStatus.Scheduled
                                        || productHaulLoadModel.Status == ProductHaulLoadStatus.Blending
                                        || productHaulLoadModel.Status == ProductHaulLoadStatus.BlendCompleted
                                        || productHaulLoadModel.Status == ProductHaulLoadStatus.Stored)

                            };
                            list.Add(contextMenu);
                        }
                    }


                }
            }

            return list;
        }

        private List<ContextMenu> SetCancelBlendSecondMenuForBl(ProductHaulForBlendModel productHaulForBlendModel, int blendSectionId)
        {
            List<ContextMenu> list = new List<ContextMenu>();

            if (productHaulForBlendModel?.ProductHaulLoadModels != null && productHaulForBlendModel.ProductHaulLoadModels.Count > 0)
            {
                foreach (ProductHaulLoadModel item in productHaulForBlendModel.ProductHaulLoadModels)
                {
                    double enteredBlendWeight = item.IsTotalBlendTonnage ? item.TotalBlendWeight : item.BaseBlendWeight;
                    //string menu = item.BaseBlendName + " - " + enteredBlendWeight / 1000 + "t";
                    string menu = item.BaseBlendName + " - " + Math.Round(enteredBlendWeight / 1000, 3) + "t";
                    ContextMenu contextMenu = new ContextMenu()
                    {
                        MenuName = menu.Replace("'", "%%%"),
                        ProcessingMode = ProcessingMode.PopsUpWindow,
                        Parms = new List<string>() { item.ProductHaulLoadId.ToString() },
                        ControllerName = ProductHaulController,
                        ActionName = "CancelProductHaulLoad",
                        MenuStyle = item.BlendSectionId == blendSectionId ? GetProductHualLoadMenuStyle(item.Status) : "",
                        MenuTips = item.Status.ToString(),
                        IsDisabled = productHaulForBlendModel.DisableCancelMenu || blendSectionId != item.BlendSectionId || (productHaulForBlendModel.ProductHaulLoadModels.Count.Equals(1) && productHaulForBlendModel.ProductHaulId > 0) || item.Status != ProductHaulLoadStatus.Scheduled
                    };
                    list.Add(contextMenu);
                }
            }

            return list;
        }

        private string GetProductHualMenuStyle(ProductHaulStatus productHaulLifeStatus)
        {
            string menuStyle = "";
            switch (productHaulLifeStatus)
            {
                case ProductHaulStatus.OnLocation:
                    menuStyle = StyleOnLocation;
                    break;
                case ProductHaulStatus.Scheduled:
                    menuStyle = StyleScheduled;
                    break;
                default:
                    menuStyle = "";
                    break;
            }
            return menuStyle;
        }
        private string GetProductHualLoadMenuStyle(ProductHaulLoadStatus productHaulLoadLifeStatus)
        {
            string menuStyle = "";
            switch (productHaulLoadLifeStatus)
            {
                case ProductHaulLoadStatus.OnLocation:
                    menuStyle = StyleOnLocation;
                    break;
                case ProductHaulLoadStatus.Scheduled:
                    menuStyle = StyleScheduled;
                    break;
                case ProductHaulLoadStatus.Blending:
                case ProductHaulLoadStatus.BlendCompleted:
                    menuStyle = StyleBlend;
                    break;
                case ProductHaulLoadStatus.Loaded:
                    menuStyle = StyleLoaded;
                    break;
                default:
                    menuStyle = "";
                    break;
            }
            return menuStyle;
        }
        #endregion

        #region Compute crew information from entity

        private StyledCell GetCrewStyledCell(string propertyName, RigJob rigJob)
        {
            StyledCell styledCell = new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache) { PropertyValue = this.GetCrewContent() };
            styledCell.ContextMenus = rigJob.JobLifeStatus==(JobLifeStatus.Alerted) || rigJob.JobLifeStatus==(JobLifeStatus.Canceled) || rigJob.JobLifeStatus==(JobLifeStatus.Completed) || rigJob.JobLifeStatus==(JobLifeStatus.None) ? null : this.SetCrewContextMenus(rigJob);

            styledCell = this.SetCellMerge(styledCell);
            styledCell.Style = styledCell.ComputeStyle(propertyName, null, this.GetDownRigSuffix(rigJob));
            styledCell.Notice = this.RigJobCrewSectionModels.Count > 0 || this.CrewModels.Count > 0 || this.ThirdPartyCrewModels.Count > 0 ? this.GetCrewNotice() : null;

            return styledCell;
        }

        private string GetCrewContent()
        {
            string crewContent = string.Empty;

            foreach (RigJobCrewSectionModel rigJobCrewSectionModel in this.RigJobCrewSectionModels)
            {
                crewContent += $"{rigJobCrewSectionModel.CrewName}\n";
            }

            foreach (CrewModel crewModel in this.CrewModels)
            {
                if (!string.IsNullOrWhiteSpace(crewModel.Name))
                {
                    string[] crews = crewModel.Name.Split('|');
                    string unitNumber = crews.Last().Trim();
                    crewContent += $"{unitNumber}\n";
                }
            }

            foreach (ThirdPartyCrewModel thirdPartyCrewModel in this.ThirdPartyCrewModels)
            {
                crewContent += $"{thirdPartyCrewModel.Name}\n";
            }

            return crewContent.Length>0?crewContent.Substring(0,crewContent.Length - 1):String.Empty;
        }

        private string GetCrewNotice()
        {
            string notice = string.Empty;
            foreach (RigJobCrewSectionModel rigJobCrewSectionModel in this.RigJobCrewSectionModels)
            {
                notice += $"\n{rigJobCrewSectionModel.HomeDistrictName} Crew {{{rigJobCrewSectionModel.CrewDescription}}} is "+ rigJobCrewSectionModel.JobCrewSectionStatusName;
            }

            foreach (CrewModel crewModel in this.CrewModels)
            {
                notice += $"\n{crewModel.HomeDistrictName} Bulker Crew {crewModel.Description}";
            }

            foreach (ThirdPartyCrewModel thirdPartyCrewModel in this.ThirdPartyCrewModels)
            {
                notice += $"\nThird Party Bulker Crew {{{thirdPartyCrewModel.ContractorCompanyName+"-"+thirdPartyCrewModel.ThirdPartyUnitNumber}}} {thirdPartyCrewModel.Description}";
            }

            return notice;
        }

        private List<ContextMenu> SetCrewContextMenus(RigJob rigJob)
        {
            List<ContextMenu> list = new List<ContextMenu>();
            if (rigJob.JobLifeStatus==(JobLifeStatus.Confirmed) || rigJob.JobLifeStatus==(JobLifeStatus.Scheduled))
            {
                list.Add(new ContextMenu
                {
                    MenuName = MenuAssignACrew,
                    ProcessingMode = ProcessingMode.PopsUpWindow,
                    Parms = new List<string> { rigJob.Id.ToString() },
                    ControllerName = RigBoardController,
                    ActionName = "AssignACrew"
                });
                if (this.RigJobCrewSectionModels.Count!=0)
                {
                    if (this.RigJobCrewSectionModels.Find(p=>p.JobCrewSectionStatusId.Equals(6))==null)
                    {
                        list.Add(new ContextMenu
                        {
                            MenuName = MenAdjustJobDuration,
                            ProcessingMode = ProcessingMode.PopsUpWindow,
                            Parms = new List<string> { rigJob.Id.ToString() },
                            ControllerName = RigBoardController,
                            ActionName = "AdjustJobDuration",
                        });
                    }
                }                                 
            }
                   
            list.Add(new ContextMenu
            {
                MenuName = MenuWithdrawCrew,
                ProcessingMode = ProcessingMode.HaveNextMenu,
                MenuList = this.GetWithdrawCrewContextMenus(rigJob),
                IsDisabled = this.RigJobCrewSectionModels.Find(p => p.JobCrewSectionStatusId.Equals(1)) == null
            });

            if (rigJob.JobLifeStatus==(JobLifeStatus.Confirmed) || rigJob.JobLifeStatus==(JobLifeStatus.Scheduled))
            {
                list.Add(new ContextMenu
                {
                    MenuName = MenuCallCrew,
                    ProcessingMode = ProcessingMode.PopsUpWindow,
                    Parms = new List<string> { rigJob.Id.ToString() },
                    ControllerName = RigBoardController,
                    ActionName = "CallAllCrews",
                    IsDisabled = this.RigJobCrewSectionModels.Find(p => p.JobCrewSectionStatusId.Equals(1)) == null
                });
            }
            
            list.Add(new ContextMenu
            {
                MenuName = MenuLogOnDuty,
                ProcessingMode = ProcessingMode.PopsUpWindow,
                Parms = new List<string> { rigJob.Id.ToString() },
                ControllerName = RigBoardController,
                ActionName = "LogOnDuty",
                IsDisabled = this.RigJobCrewSectionModels.Find(p => p.JobCrewSectionStatusId.Equals(4)) == null
            });

            list.Add(new ContextMenu
            {
                MenuName = MenuLogOffDuty,
                ProcessingMode = ProcessingMode.PopsUpWindow,
                Parms = new List<string> { rigJob.Id.ToString() },
                ControllerName = RigBoardController,
                ActionName = "LogOffDuty",
                IsDisabled = this.RigJobCrewSectionModels.Find(p => p.JobCrewSectionStatusId.Equals(5)) == null
            });

            return list;
        }

        private List<ContextMenu> GetWithdrawCrewContextMenus(RigJob rigJob)
        {
            List<ContextMenu> list = new List<ContextMenu>();
            foreach (RigJobCrewSectionModel rigJobCrewSectionModel in this.RigJobCrewSectionModels)
            {
                list.Add(new ContextMenu
                {
                    MenuName = rigJobCrewSectionModel.CrewDescription,
                    ProcessingMode = ProcessingMode.PopsUpWindow,
                    Parms = new List<string> { rigJob.Id.ToString(), rigJobCrewSectionModel.CrewId.ToString(), rigJobCrewSectionModel.JobCrewSectionStatusId.ToString() },
                    ControllerName = RigBoardController,
                    ActionName = "WithdrawACrew"
                });
            }
            return list;
        }

        #endregion

        #region Compute Notes information from entity

        private StyledCell GetNotesStyledCell(string propertyName, RigJob rigJob, List<BinInformation> rigBinSections)
        {
            string binContents = string.Empty;
            string binNotices = string.Empty;

            foreach (var binInformation in rigBinSections)
            {
                var blendName = binInformation.BlendChemical?.Name?.Replace(" + Additives", "");
                var blendDescription = binInformation.BlendChemical?.Description?.Replace(" + Additives", "");
                var blendCategory = string.Empty;
                bool useblendCategory = false;
                //Nov 24,2013 AW develop: Roll back TongTao's change on Nov 3 and after.

                //Nov 30,2013 Tongtao develop: Update Rig Notes Bin Information
                string blendCategoryName = string.Empty;
                //Nov 30, 2023 zhangyuan 203_PR_UpdateBinNotes: Add BinNote Contents
                //DEC 1, 2023 Tongtao develop: Update Notes show text(if blendCategory not found show blendname ) and Notes tooltip show text
                //Dec 07, 2023 Tongtao P45_Q4_212:  Update Notes show text(Only display the BlendCategory if the corresponding ProductHaulLoad is related to the current RigJob; otherwise, display the name of the Base Blend)
                var binNote = this.BinSectionModel.BinNotes?.FirstOrDefault(p => p.Bin.Id == binInformation.Bin.Id && p.PodIndex == binInformation.PodIndex);

                if (binInformation.LastProductHaulLoadId != 0)
                {
                    ProductHaulLoad productHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(binInformation.LastProductHaulLoadId);

                    if (productHaulLoad != null && rigJob.CallSheetId != 0)
                    {
	                    if (rigJob.CallSheetId == productHaulLoad.CallSheetId)
	                    {
		                    useblendCategory = true;
	                    }

	                    blendCategory = productHaulLoad.BlendCategory.Name + " ";
                    }
                }

                if ((string.IsNullOrEmpty(blendName)) && (string.IsNullOrEmpty(blendDescription)))
                {
                    binContents = binContents + binInformation.Name + ": " + (binNote != null && !string.IsNullOrEmpty(binNote.Description) ? (binNote.Description) : "") + "\n";
                    binNotices = binNotices + binInformation.Name + ": " + (binNote != null && !string.IsNullOrEmpty(binNote.Description) ? (binNote.Description) : "") + "\n";
                }
                else
                {
                    string binInfos = binInformation.BlendChemical == null ? "Unknown" : (!string.IsNullOrEmpty(blendCategory.Trim()) && useblendCategory ? blendCategory : (!string.IsNullOrEmpty(blendName) ? blendName : "Unknown"));
                    binContents = binContents + binInformation.Name + ": " + binInformation.Quantity + "t " + binInfos + (binNote != null && !string.IsNullOrEmpty(binNote.Description) ? ("," + binNote.Description) : "") + "\n";
                    binNotices = binNotices + binInformation.Name + ": " + blendCategory + binInformation.Quantity + "t " + binInformation.BlendChemical?.Description + (binNote != null && !string.IsNullOrEmpty(binNote.Description) ? ("," + binNote.Description) : "") + "\n";
                }
            }

            StyledCell styledCell;
            if (!string.IsNullOrEmpty(binContents) && binContents.Length > 1)
            {
                binContents = binContents.Substring(0, binContents.Length - 1);
                binNotices = binNotices.Substring(0, binNotices.Length - 1);

                var cellContent = rigJob.Notes + "\n~~~~~~~~~~\n" + binContents;
                var cellNotice = rigJob.Notes + "\n~~~~~~~~~~\n" + binNotices;

                styledCell = new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache)
                    {PropertyValue = cellContent, Notice = cellNotice};
            }
            else
            {
                styledCell = new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache)
                    {PropertyValue = rigJob.Notes, Notice = rigJob.Notes};
            }

            styledCell.ContextMenus = styledCell.IsDisplayMenu ? this.SetNotesContextMenus(rigJob) : null;

            styledCell = this.SetCellMerge(styledCell);
            styledCell.Style = styledCell.ComputeStyle(propertyName, null, this.GetDownRigSuffix(rigJob));

            return styledCell;
        }

        private List<ContextMenu> SetNotesContextMenus(RigJob rigJob)
        {
            List<ContextMenu> list = new List<ContextMenu>();
            list.Add(new ContextMenu()
            {
                MenuName = MenuUpdateNotes,
                ProcessingMode = ProcessingMode.PopsUpWindow,
                Parms = new List<string>() { rigJob.Id.ToString(), rigJob.CallSheetNumber.ToString(), MenuUpdateNotes },
                DialogName = DialogUpdateNotes,
                ControllerName = RigBoardController,
                ActionName = "GetRigJobById"
            });

            return list;
        }

        #endregion

        #region Compute ConsultantContacts information from entity

        public StyledCell GetConsultantContactsStyledCell(string propertyName, RigJob rigJob)
        {
            if (this.ConsultantViewModel == null)
                return new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache) { IsNeedRowMerge = false };

            StyledCell styledCell = this.GetConsultantContactsStyledCell(propertyName, this.ConsultantViewModel, rigJob);
            styledCell.IsNeedRowMerge = false;

            return styledCell;
        }

        private StyledCell GetConsultantContactsStyledCell(string propertyName, ConsultantViewModel consultantViewModel, RigJob rigJob)
        {
            StyledCell styledCell = new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache) {PropertyValue = this.ConsultantViewModel.DisplayContent ?? " "};
            styledCell.ContextMenus = styledCell.IsDisplayMenu ? this.SetConsultantContactsContextMenus(rigJob) : null;
            styledCell = this.SetConsultantStyledCellByShift(consultantViewModel, styledCell, out var statusName);
            styledCell.Style = rigJob.JobLifeStatus==(JobLifeStatus.Alerted) ? null : styledCell.ComputeStyle(propertyName, statusName, this.GetDownRigSuffix(rigJob));
            if (!consultantViewModel.ConsultantId.Equals(0) && !rigJob.JobLifeStatus.Equals(JobLifeStatus.Alerted))
            {
                if (statusName!=null)
                {
                    if (statusName.Equals("DayShift"))
                    {
                        styledCell.Notice = NoticeConsultantDay;
                    }
                    else if (statusName.Equals("NightShift"))
                    {
                        styledCell.Notice = NoticeConsultantNight;
                    }
                    else
                    {
                        styledCell.Notice = NoticeConsultant24;
                    }

                    if(!string.IsNullOrEmpty(consultantViewModel.Email))
                        styledCell.Notice= styledCell.Notice +" / " + consultantViewModel.Email;
                }
               
            }
            return styledCell;
        }

        private StyledCell SetConsultantStyledCellByShift(ConsultantViewModel consultantViewModel, StyledCell styledCell, out string statusName)
        {
            statusName = string.Empty;
            if (consultantViewModel != null)
            {
                statusName = consultantViewModel.WorkShiftTypeName;
                if (statusName!=null)
                {
                    statusName = statusName.Replace(" ", "");
                }
                
            }

            

            return styledCell;
        }

        private List<ContextMenu> SetConsultantContactsContextMenus(RigJob rigJob)
        {
            List<ContextMenu> list = new List<ContextMenu>();
            if (rigJob.JobLifeStatus==(JobLifeStatus.Alerted)) return list;
            list.Add(new ContextMenu()
            {
                MenuName = MenuAddAConsultant,
                ProcessingMode = ProcessingMode.PopsUpWindow,
                Parms = new List<string>() { this.ConsultantViewModel.CallSheetId.ToString(), rigJob.Id.ToString() },
                DialogName = DialogAddAConsultant,
                ControllerName = RigBoardController,
                ActionName = "AddAConsultant",
                IsDisabled = this.GetAddaConsultantIsDisabled(rigJob),
            });

            list.Add(new ContextMenu()
            {
                MenuName = MenuRemoveConsultant,
                ProcessingMode = ProcessingMode.NoPopsUpWindow,
                Parms = new List<string>() {this.ConsultantViewModel.CallSheetNumber.ToString(), this.ConsultantViewModel.IsFirst.ToString(), rigJob.Id.ToString() },
                ControllerName = RigBoardController,
                ActionName = "RemoveConsultant",
                IsDisabled = this.ConsultantViewModel.IsRemoveDisabled,
            });
            list.Add(new ContextMenu()
            {
                MenuName = MenuCreateNewConsultant,
                ProcessingMode = ProcessingMode.PopsUpWindow,
                Parms = new List<string>() {},
                DialogName = DialogAddNewConsultant,
                ControllerName = RigBoardController,
                ActionName = "CreateConsultant",
                IsHaveSplitLine = true
            });
            list.Add(new ContextMenu()
            {
                MenuName = MenuUpdateConsultant,
                ProcessingMode = ProcessingMode.PopsUpWindow,
                Parms = new List<string>() {this.ConsultantViewModel.ConsultantId.ToString()},
                DialogName = DialogUpdateConsultant,
                ControllerName = RigBoardController,
                ActionName = "GetClientConsultantById",
                IsDisabled = this.ConsultantViewModel.ConsultantId.Equals(0)
            });
            list.Add(new ContextMenu()
            {
                MenuName = MenuAssignToDayShift,
                ProcessingMode = ProcessingMode.NoPopsUpWindow,
                Parms = new List<string>() { this.ConsultantViewModel.ConsultantId.ToString(), "1" },
                ControllerName = RigBoardController,
                ActionName = "UpdateWorkShift",
                IsDisabled = this.ConsultantViewModel.ConsultantId.Equals(0)
            });
            list.Add(new ContextMenu()
            {
                MenuName = MenuAssignToNightShift,
                ProcessingMode = ProcessingMode.NoPopsUpWindow,
                Parms = new List<string>() { this.ConsultantViewModel.ConsultantId.ToString(), "2" },
                ControllerName = RigBoardController,
                ActionName = "UpdateWorkShift",
                IsDisabled = this.ConsultantViewModel.ConsultantId.Equals(0)
            });
            list.Add(new ContextMenu()
            {
                MenuName = MenuAssignTo24HourShift,
                ProcessingMode = ProcessingMode.NoPopsUpWindow,
                Parms = new List<string>() { this.ConsultantViewModel.ConsultantId.ToString(), "0" },
                ControllerName = RigBoardController,
                ActionName = "UpdateWorkShift",
                IsDisabled = this.ConsultantViewModel.ConsultantId.Equals(0)
            });
           
            return list;
        }

        private bool GetAddaConsultantIsDisabled(RigJob rigJob)
        {
            if (rigJob.JobLifeStatus == JobLifeStatus.None)
            {
                return true;
            }
            if (rigJob?.ClientConsultant1 != null && rigJob?.ClientConsultant2 != null)
            {
                if (rigJob.ClientConsultant1.Id > 0 || !string.IsNullOrWhiteSpace(rigJob.ClientConsultant1.Name) || !string.IsNullOrWhiteSpace(rigJob.ClientConsultant1.Phone2) || !string.IsNullOrWhiteSpace(rigJob.ClientConsultant1.Email))
                {
                    if (rigJob.ClientConsultant2.Id > 0 || !string.IsNullOrWhiteSpace(rigJob.ClientConsultant2.Name) || !string.IsNullOrWhiteSpace(rigJob.ClientConsultant2.Phone2) || !string.IsNullOrWhiteSpace(rigJob.ClientConsultant2.Email))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        #endregion

        #region Cell Merge

        public StyledCell SetCellMerge(StyledCell styledCell)
        {
            styledCell.IsNeedRowMerge = true;
            styledCell.RowMergeNumber = this.RowMergeNumber;

            return styledCell;
        }

        #endregion

        #region Bin
        //Apirl 25, 2024 Tongtao 192_PR_StandardizeMTSDownloadName: add mts print menu
        protected override List<ContextMenu> SetBinContextMenus(BinSectionModel binSectionModel, RigJob rigJob,
            BinInformation rigBinSections, List<Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul> productHauls, List<ProductHaulLoad> productHaulLoads)
        {
            List<ContextMenu> list = new List<ContextMenu>();
            if (rigBinSections != null)
            {
                list.Add(new ContextMenu()
                {
                    MenuName = MenuScheduleProductHaulForBin,
                    ProcessingMode = ProcessingMode.PopsUpWindow,
                    Parms = new List<string>()
                    {
                        binSectionModel.BinId.ToString(), "0", rigJob.Rig.Id.ToString(),
                        "0", binSectionModel.BinNumber,
                        rigJob.Rig.Name, binSectionModel.BinSectionId.ToString(),
                        BinSectionModel.Name, rigJob.ServicePoint.Id.ToString(), string.Empty,"Parameter Ending"
                    },
                    ControllerName = ProductHaulController,
                    IsDisabled = (binSectionModel.BinId <= 0),
                    ActionName = "ScheduleProductHaulToRigBin",
                });
                list.Add(new ContextMenu()
                {
                    MenuName = MenuRescheduleProductHaulForBin,
                    ProcessingMode = ProcessingMode.HaveNextMenu,
                    MenuList = this.SetRescheduleProductHaulForRigBoardBin(productHaulLoads, binSectionModel,
                        productHauls, rigJob)
                });

                list.Add(new ContextMenu()
                {
                    MenuName = MenuCancelProductHaulForBin,
                    ProcessingMode = ProcessingMode.HaveNextMenu,
                    MenuList = this.SetCancelProductHaulForBin(productHaulLoads, binSectionModel, productHauls,
                        rigJob)
                });
                list.Add(new ContextMenu()
                {
                    MenuName = MenuOnLocationForBin,
                    ProcessingMode = ProcessingMode.HaveNextMenu,
                    MenuList = this.SetOnLocationMenuForBin(productHaulLoads, binSectionModel, productHauls)
                });

                list.Add(new ContextMenu()
                {
                    MenuName = MenuPrintMTS,
                    ProcessingMode = ProcessingMode.HaveNextMenu,
                    MenuList = this.SetPrintMTSMenuForBin(productHauls)
                });

                //Dec 22, 2023 zhangyuan 195_PR_Haulback: Add HaulBack Menu
                list.Add(new ContextMenu()
                {
                    MenuName = MenuHaulBack,
                    ProcessingMode = ProcessingMode.PopsUpWindow,
                    Parms = new List<string>()
                    {
                        rigJob.Rig.Id.ToString(),
                        rigJob.Id.ToString(),
                        binSectionModel.BinSectionId.ToString()
                    },
                    ControllerName = ProductHaulController,
                    IsDisabled = (binSectionModel.BinId <= 0)|| binSectionModel.Quantity<0.001,
                    ActionName = "HaulBack",
                });
                list.Add(new ContextMenu()
                {
                    MenuName = MenuTransferBlend,
                    ProcessingMode = ProcessingMode.PopsUpWindow,
                    //Parms = new List<string>()
                    //{
                    //    binSectionModel.BinId.ToString(), binSectionModel.BlendDescription,
                    //    //Nov 3, 2023 Tongtao P45_Q4_105: Modify numerical precision to 3 decimal places
                    //    binSectionModel.Quantity.ToString("0.000"), binSectionModel.BinNumber
                    //},
                    //Nov 14, 2023 zhangyuan P63_Q4_174: Modify TransferBlend params
                    Parms = new List<string>()
                    {
                        binSectionModel.BinSectionId.ToString(),
                        "rigjob"
                    },
                    ControllerName = ProductHaulController,
                    ActionName = "TransferBlend",
                    IsDisabled = string.IsNullOrEmpty(binSectionModel.BlendDescription) ||
                                 Math.Abs(binSectionModel.Quantity) < 0.001,
                    IsHaveSplitLine = true
                });


                list.Add(new ContextMenu()
                {
                    MenuName = MenuAdjustBlendAmount,
                    ProcessingMode = ProcessingMode.PopsUpWindow,
                    // Dec 28, 2023 zhangyuan 243_PR_AddBlendDropdown:Add  paramter flag
                    Parms = new List<string>() { binSectionModel.BinSectionId.ToString(), "rigboard" },
                    ControllerName = BinBoardController,
                    IsDisabled = (binSectionModel.BinId <= 0),
                    ActionName = "UpdateQuantity",
                });

                list.Add(new ContextMenu()
                {
                    MenuName = MenuEmptyBin,
                    ProcessingMode = ProcessingMode.PopsUpWindow,
                    IsDisabled = binSectionModel.BinId > 0 ? false : true,
                    Parms = new List<string> { binSectionModel.BinSectionId.ToString() },
                    ControllerName = BulkPlantController,
                    ActionName = "EmptyBin"
                });
            }
            else
            {
                ContextMenu isNeedBinMenu = new ContextMenu()
                {
                    ProcessingMode = ProcessingMode.NoPopsUpWindow,
                    Parms = new List<string>() { rigJob.Id.ToString() },
                    DialogName = "",
                    ControllerName = RigBoardController,
                    IsHaveSplitLine = true
                };

                if (!(bool)rigJob.IsNeedBins)
                {
                    isNeedBinMenu.MenuName = MenuBinNotAssigned;
                    isNeedBinMenu.ActionName = "NeedBin";
                }
                else
                {
                    isNeedBinMenu.MenuName = MenuDoNotNeedBin;
                    isNeedBinMenu.ActionName = "NotNeedBin";
                    isNeedBinMenu.IsDisabled = productHaulLoads != null && !productHaulLoads.Count.Equals(0);
                }

                list.Add(isNeedBinMenu);
            }

            list.Add(new ContextMenu()
            {
                MenuName = MenuAssignABin,
                ProcessingMode = ProcessingMode.PopsUpWindow,
                Parms = new List<string>() { rigJob.Id.ToString() },
                DialogName = DialogAddNewBin,
                ControllerName = RigBoardController,
                ActionName = "AssignABin",
                IsDisabled = false
            });
            if (rigBinSections != null)
            {
                list.Add(new ContextMenu()
                {
                    MenuName = MenuRemoveABin,
                    ProcessingMode = ProcessingMode.PopsUpWindow,
                    Parms = new List<string>()
                        { binSectionModel.BinId.ToString(), rigJob.CallSheetId.ToString(), rigJob?.Id.ToString() },
                    ControllerName = RigBoardController,
                    ActionName = "RemoveABin",
                    IsDisabled = binSectionModel.BinId.Equals(0)
                });
            }
            //Nov 29, 2023 zhangyuan 203_PR_UpdateBinNotes: add UpdateBinNotes
            list.Add(new ContextMenu()
            {
                MenuName = MenuUpdateBinNotes,
                ProcessingMode = ProcessingMode.PopsUpWindow,
                Parms = new List<string>() { binSectionModel.BinId.ToString(), binSectionModel.PodIndex.ToString() },
                DialogName = MenuUpdateBinNotes,
                ControllerName = RigBoardController,
                ActionName = "UpdateBinNotes",
                IsDisabled = binSectionModel.BinId.Equals(0)
            });
            return list;
        }

        //Apirl 25, 2024 Tongtao 192_PR_StandardizeMTSDownloadName: add mts print menu
        protected List<ContextMenu> SetPrintMTSMenuForBin(
    List<Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul>
        productHauls = null, bool isBulker = false)
        {
            List<ContextMenu> list = new List<ContextMenu>();
            if (productHauls != null)
            {
                var productHaulList = new List<Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul>();
                if (isBulker)
                {
                    productHaulList =
                       productHauls.FindAll(p =>
                           p.ProductHaulLifeStatus != ProductHaulStatus.OnLocation);
                }
                else
                {
                    productHaulList =
                       productHauls.FindAll(p =>
                           p.ProductHaulLifeStatus != ProductHaulStatus.OnLocation &&
                           p.ProductHaulLifeStatus != ProductHaulStatus.Loaded);
                }

                if (productHaulList.Count > 0)
                {
                    foreach (var item in productHaulList)
                    {
                        string menu = item.IsGoWithCrew
                            ? MenuGowithCrewForBin
                            : Utility.GetDateTimeValue(item.ExpectedOnLocationTime, ShortTimeFormatForBin);
                        menu = item.Crew.Description + "-" + menu;
                        ContextMenu contextMenu = new ContextMenu()
                        {
                            MenuName = menu.Replace("'", "%%%"),
                            ProcessingMode = ProcessingMode.OpenInNewTab,
                            Parms = new List<string>() { item.Id.ToString() },
                            ControllerName = ProductHaulController,
                            ActionName = "PrintMTS",
                            MenuTips = item.ProductHaulLifeStatus.ToString()
                        };
                        list.Add(contextMenu);
                    }
                }
            }

            return list;
        }



        /*protected override List<ContextMenu> SetReScheduleBlendMenuForBin(List<ProductHaulLoad> productHaulLoadList,
            BinSectionModel binSectionModel,
            List<Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul>
                productHaulList = null)
        {
            List<ContextMenu> list = new List<ContextMenu>();

            if (productHaulList != null && productHaulList.Count > 0)
            {
                List<int> productHualIds = new List<int>();
                foreach (Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul productHaul
                    in productHaulList)
                {
                    if (productHualIds.Contains(productHaul.Id))
                    {
                        continue;
                    }
                    productHualIds.Add(productHaul.Id);
                    string menu = string.Empty;
                    if (productHaul.Id != 0)
                    {
                        menu = productHaul.Name??"No Product Haul Name";
                    }


                    ContextMenu contextMenu = new ContextMenu()
                    {
                        MenuName = menu.Replace("'", "%%%"),
                        ProcessingMode = ProcessingMode.PopsUpWindow,
                        Parms = new List<string>() {productHaul.Id.ToString(), "0", "0"},
                        ControllerName = ProductHaulController,
                        ActionName = "RescheduleProductHaul",
                        IsDisabled = productHaul.ProductHaulLifeStatus != ProductHaulStatus.Scheduled || productHaul.ShippingLoadSheets.FindAll(p=>p.CallSheetId >0 || p.Destination !=binSectionModel.Name).Count>0,
                        MenuStyle = GetProductHualMenuStyle(productHaul.ProductHaulLifeStatus),
                        MenuTips = productHaul.ProductHaulLifeStatus.ToString(),
                        MenuList = this.SetReScheduleProductHaulMenuForRigBin(productHaul.ProductHaulLoads,binSectionModel,productHaul)
                    };
                    list.Add(contextMenu);
                }
            }

            return list;
        }*/

        /*private List<ContextMenu> SetReScheduleProductHaulMenuForRigBin(List<ProductHaulLoad> productHaulLoadList,
            BinSectionModel binSectionModel,
            Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul productHaul)
        {
            List<ContextMenu> list = new List<ContextMenu>();

            if (productHaulLoadList != null && productHaulLoadList.Count > 0 && productHaul != null && productHaul.ProductHaulLoads.Count > 0)
            {
                string actionName = "RescheduleBlendFromRigBin";
                foreach (var productHaulLoad in productHaul.ProductHaulLoads)
                {
                    double enteredBlendWeight = productHaulLoad.IsTotalBlendTonnage ? productHaulLoad.TotalBlendWeight : productHaulLoad.BaseBlendWeight;
                    string menu = productHaulLoad.BlendChemical?.Name + " - " + enteredBlendWeight / 1000 + "t";
                    ContextMenu contextMenu = new ContextMenu()
                    {
                        MenuName = menu.Replace("'", "%%%"),
                        ProcessingMode = ProcessingMode.PopsUpWindow,
                        Parms = new List<string>() { productHaulLoad.Id.ToString(), binSectionModel.BinSectionId.ToString(), binSectionModel.Name },
                        ControllerName = ProductHaulController,
                        IsDisabled =productHaulLoad.ProductHaulLoadLifeStatus!=ProductHaulLoadStatus.Scheduled || productHaulLoad.CallSheetId>0 || !(productHaulLoad.Bin.Id==binSectionModel.BinId && productHaulLoad.PodIndex==binSectionModel.PodIndex),
                        ActionName = actionName,
                        MenuStyle = (productHaulLoad.Bin.Id == binSectionModel.BinId && productHaulLoad.PodIndex == binSectionModel.PodIndex) ? GetProductHualLoadMenuStyle(productHaulLoad.ProductHaulLoadLifeStatus) : "",
                        MenuTips = productHaulLoad.ProductHaulLoadLifeStatus.ToString()
                    };
                    list.Add(contextMenu);
                }
            }

            return list;
        }*/
        /*protected override List<ContextMenu> SetCancelBlendMenuForBin(List<ProductHaulLoad> phlList, BinSectionModel binSectionModel,
            List<Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul>
                productHaulList = null, RigJob rigJob = null)
        {
            List<ContextMenu> list = new List<ContextMenu>();
           
            if (productHaulList != null && productHaulList.Count > 0)
            {
                List<int> productHualIds = new List<int>();
                foreach (Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul productHaul
                    in productHaulList)
                {
                    if (productHualIds.Contains(productHaul.Id))
                    {
                        continue;
                    }
                    productHualIds.Add(productHaul.Id);
                        string menu = string.Empty;
                    if (productHaul.Id != 0)
                    {
                        menu = productHaul.IsGoWithCrew
                            ? MenuGowithCrew
                            : Utility.GetDateTimeValue(productHaul.ExpectedOnLocationTime, ShortTimeFormat);

                        menu = productHaul.Crew.Description + "-" + menu;
                    }
                    ContextMenu contextMenu = new ContextMenu()
                    {
                        MenuName = menu.Replace("'", "%%%"),
                        ProcessingMode = ProcessingMode.PopsUpWindow,
                        Parms = new List<string>() { productHaul.Id.ToString(), rigJob.Id.ToString() },
                        ControllerName = ProductHaulController,
                        ActionName = "CancelProductHaul",
                        IsDisabled = productHaul.ShippingLoadSheets.FindAll(p => p.CallSheetId > 0|| p.Destination != binSectionModel.Name).Count > 0 || productHaul.ShippingLoadSheets.Count>1,
                        MenuStyle = GetProductHualMenuStyle(productHaul.ProductHaulLifeStatus),
                        MenuTips = productHaul.ProductHaulLifeStatus.ToString(),
                        MenuList = this.SetCancelProductHaulLoadMenuForRigBin(productHaul, binSectionModel)
                    };
                    list.Add(contextMenu);
                }
            }

            return list;
        }*/
        /*private List<ContextMenu> SetCancelProductHaulLoadMenuForRigBin(Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul productHaul,BinSectionModel binSectionModel)
        {
            List<ContextMenu> list = new List<ContextMenu>();
            if (productHaul.ProductHaulLoads != null && productHaul.ProductHaulLoads.Count > 0)
            {
                foreach (var item in productHaul.ProductHaulLoads)
                {
                    double enteredBlendWeight = item.IsTotalBlendTonnage ? item.TotalBlendWeight : item.BaseBlendWeight;
                    var shippingLoadSheet = productHaul.ShippingLoadSheets.FirstOrDefault(p => p.ProductHaulLoad.Id == item.Id);
                    //string menu = item.BlendChemical?.Name + " - " + enteredBlendWeight / 1000 + "t";
                    string menu = item.BlendChemical?.Name + " - " + shippingLoadSheet.LoadAmount / 1000 + "t";
                    ContextMenu contextMenu = new ContextMenu()
                    {
                        MenuName = menu.Replace("'", "%%%"),
                        ProcessingMode = ProcessingMode.PopsUpWindow,
                        Parms = new List<string>() { item.Id.ToString() },
                        ControllerName = ProductHaulController,
                        IsDisabled=true,
                        //IsDisabled=productHaul.ProductHaulLoads.Count==1 ||item.CallSheetId>0|| !(item.Bin.Id == binSectionModel.BinId && item.PodIndex == binSectionModel.PodIndex),
                        ActionName = "CancelProductHaulLoad",
                        MenuStyle = (item.Bin.Id == binSectionModel.BinId && item.PodIndex == binSectionModel.PodIndex) ? GetProductHualLoadMenuStyle(item.ProductHaulLoadLifeStatus) : "",
                        MenuTips = item.ProductHaulLoadLifeStatus.ToString()
                    };
                    list.Add(contextMenu);
                }
            }

            return list;
        }*/


        protected override List<ContextMenu> SetOnLocationMenuForBin(List<ProductHaulLoad> phlList, BinSectionModel binSectionModel,
            List<Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul>
                productHaulList = null)
        {
            List<ContextMenu> list = new List<ContextMenu>();
            if (productHaulList != null && productHaulList.Count > 0)
            {
                List<int> productHualIds = new List<int>();
                foreach (Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul productHaul
                    in productHaulList)
                {

                    if (productHaul != null)
                    {
                        if (productHualIds.Contains(productHaul.Id))
                        {
                            continue;
                        }
                        productHualIds.Add(productHaul.Id);
                        string menu = string.Empty;
                        if (productHaul.Id != 0)
                        {
                            menu = productHaul.IsGoWithCrew
                                ? MenuGowithCrew
                                : Utility.GetDateTimeValue(productHaul.ExpectedOnLocationTime, ShortTimeFormat);

                            menu = productHaul.Crew.Description + "-" + menu;
                        }

                        ContextMenu contextMenu = new ContextMenu()
                        {
                            MenuName = menu,
                            ProcessingMode = ProcessingMode.PopsUpWindow,
                            Parms = new List<string>() { productHaul.Id.ToString() },
                            ControllerName = ProductHaulController,
                            ActionName = "OnLocationProductHaul",
                            IsDisabled = true, //Disable on location ability to allow user use Job Board more
//                            IsDisabled = productHaul.ShippingLoadSheets.FirstOrDefault().ShippingStatus == ShippingStatus.OnLocation || productHaul.ProductHaulLifeStatus != ProductHaulStatus.Scheduled,
                            MenuStyle = GetProductHualMenuStyle(productHaul.ProductHaulLifeStatus),
                            MenuTips = productHaul.ProductHaulLifeStatus.ToString(),
                            MenuList = this.SetOnLocationProductHaulLoadMenuForBin(binSectionModel, productHaul)
                        };
                        list.Add(contextMenu);
                    }
                }
            }

            return list;
        }

        private List<ContextMenu> SetOnLocationProductHaulLoadMenuForBin(BinSectionModel binSectionModel, Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul productHaul)
        {
            List<ContextMenu> list = new List<ContextMenu>();

            if (productHaul?.ShippingLoadSheets != null && productHaul.ShippingLoadSheets.Count > 0)
            {
                foreach (ShippingLoadSheet shippingLoadSheet in productHaul.ShippingLoadSheets)
                {
                    var item = eServiceOnlineGateway.Instance.GetProductHaulLoadById(shippingLoadSheet.ProductHaulLoad
                        .Id);
                    if (item != null)
                    {
//                        double enteredBlendWeight = item.IsTotalBlendTonnage ? item.TotalBlendWeight : item.BaseBlendWeight;
                        //string menu = item.BlendChemical?.Name + " - " + enteredBlendWeight / 1000 + "t";
                        string menu = item.BlendChemical?.Name + " - " + shippingLoadSheet.LoadAmount / 1000 + "t";
                        ContextMenu contextMenu = new ContextMenu()
                        {
                            MenuName = menu.Replace("'", "%%%"),
                            ProcessingMode = ProcessingMode.PopsUpWindow,
                            Parms = new List<string>() { item.Id.ToString(), productHaul.Id.ToString() },
                            ControllerName = ProductHaulController,
                            ActionName = "OnLocationProdHaulLoad",
                            //IsDisabled = shippingLoadSheet.ShippingStatus == ShippingStatus.OnLocation || item.ProductHaulLoadLifeStatus != ProductHaulLoadStatus.Scheduled ||
                            //             item.CallSheetId > 0,
                            IsDisabled = true,//Disable on location ability to allow user use Job Board more
                            MenuStyle = (item.Bin.Id == binSectionModel.BinId &&
                                         item.PodIndex == binSectionModel.PodIndex)
                                ? GetProductHualLoadMenuStyle(item.ProductHaulLoadLifeStatus)
                                : "",
                            MenuTips = item.ProductHaulLoadLifeStatus.ToString()
                        };
                        list.Add(contextMenu);
                    }
                }
            }

            return list;
        }

        private List<ContextMenu> SetReScheduleProductHaulLoadForRigBoardBin(List<ProductHaulLoad> phlList, Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul productHaul, BinSectionModel binSectionModel, RigJob rigJob)
        {
            List<ContextMenu> list = new List<ContextMenu>();

            if (productHaul.ShippingLoadSheets != null && productHaul.ShippingLoadSheets.Count > 0)
            {
                foreach (ShippingLoadSheet shippingLoadSheet in productHaul.ShippingLoadSheets)
                {
                    string actionName = "RescheduleBlendFromRigBin";
                    if (IsBulkPlant)
                    {
                        actionName = "RescheduleBlendFromBulkPlantBin";
                    }

//                    var item = eServiceOnlineGateway.Instance.GetProductHaulLoadById(shippingLoadSheet.ProductHaulLoad.Id);
                    var item = phlList?.Find(p => p.Id == shippingLoadSheet.ProductHaulLoad.Id);
                    if (item != null)
                    {
//                        double enteredBlendWeight =item.IsTotalBlendTonnage ? item.TotalBlendWeight : item.BaseBlendWeight;
                        //string menu = item.BlendChemical?.Name + " - " + enteredBlendWeight / 1000 + "t";
                        string menu = item.BlendChemical?.Name + " - " + shippingLoadSheet.LoadAmount / 1000 + "t";
                        string tips = item.ProductHaulLoadLifeStatus.ToString();
                        if (item.BlendShippingStatus != BlendShippingStatus.Empty)
                        {
                            tips += "|" + item.BlendShippingStatus.ToString();
                        }

                        if (item.BlendTestingStatus != BlendTestingStatus.None)
                        {
                            tips += "|" + item.BlendTestingStatus.ToString();
                        }

                        ContextMenu contextMenu = new ContextMenu()
                        {
                            MenuName = menu.Replace("'", "%%%"),
                            ProcessingMode = ProcessingMode.PopsUpWindow,
                            Parms = new List<string>()
                            {
                                item.Id.ToString(), rigJob?.Rig.Id.ToString(), binSectionModel.Name,
                                rigJob?.CallSheetId.ToString(), rigJob?.Id.ToString()
                            },
                            ControllerName = ProductHaulController,
                            ActionName = actionName,
                            MenuTips = tips,
                            IsDisabled = item.ProductHaulLoadLifeStatus != ProductHaulLoadStatus.Scheduled ||
                                         item.BlendShippingStatus == BlendShippingStatus.OnLocation ||
                                         item.CallSheetId > 0

                        };
                        list.Add(contextMenu);
                    }
                }
            }

            return list;
        }
        protected override List<ContextMenu> SetRescheduleProductHaulForRigBoardBin(List<ProductHaulLoad> phlList, BinSectionModel binSectionModel, List<Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul> productHaulList = null, RigJob rigJob = null)
        {
            List<ContextMenu> list = new List<ContextMenu>();
            if (productHaulList != null && productHaulList.Count > 0)
            {
                foreach (var item in productHaulList)
                {
                    if (item != null)
                    {
                        string menu = item.IsGoWithCrew? MenuGowithCrewForBin: Utility.GetDateTimeValue(item.ExpectedOnLocationTime, ShortTimeFormatForBin);
                        menu = item.Crew.Description + "-" + menu;
                        ContextMenu contextMenu = new ContextMenu()
                        {
                            MenuName = menu.Replace("'", "%%%"),
                            ProcessingMode = ProcessingMode.PopsUpWindow,
                            //Parms = new List<string>() { item.Id.ToString(), rigJob?.CallSheetId.ToString(), rigJob?.Id.ToString() },
                            //Apr 10, 2024 zhangyuan 317_Fix_Crew_Assigned_Notification_missing: Modify  RescheduleProductHaul in the bin column should not use the default Rigjob
                            Parms = new List<string>() { item.Id.ToString(), rigJob?.CallSheetId.ToString(), "0" },
                            ControllerName = ProductHaulController,
                            ActionName = "RescheduleProductHaul",
                            //MenuStyle = item.BlendSectionId == blendSectionId ? GetProductHualLoadMenuStyle(item.Status) : "",           
                            MenuTips = item.ProductHaulLifeStatus.ToString(),
                            //Apirl 26, 2024 Tongtao 191_PR_AllowReschedulingProductHaulBeforeOnlocation: Allow rescheduling product haul before OnLocation status
                            IsDisabled = item.ProductHaulLifeStatus == ProductHaulStatus.Empty || item.ProductHaulLifeStatus == ProductHaulStatus.Pending
                                        || item.ProductHaulLifeStatus == ProductHaulStatus.Returned || item.ProductHaulLifeStatus == ProductHaulStatus.OnLocation,
                            MenuList = SetReScheduleProductHaulLoadForRigBoardBin(phlList, item, binSectionModel, rigJob)
                        };
                        list.Add(contextMenu);
                    }
                }
            }
            return list;
        }

        protected override List<ContextMenu> SetCancelProductHaulForBin(List<ProductHaulLoad> phlList, BinSectionModel binSectionModel, List<Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul> productHaulList = null, RigJob rigJob = null)
        {
            List<ContextMenu> list = new List<ContextMenu>();
            if (productHaulList != null && productHaulList.Count > 0)
            {
                foreach (var item in productHaulList)
                {
                    if (item != null)
                    {
                        string menu = item.IsGoWithCrew? MenuGowithCrewForBin: Utility.GetDateTimeValue(item.ExpectedOnLocationTime, ShortTimeFormatForBin);
                        menu = item.Crew.Description + "-" + menu;
                        ContextMenu contextMenu = new ContextMenu()
                        {
                            MenuName = menu.Replace("'", "%%%"),
                            ProcessingMode = ProcessingMode.PopsUpWindow,
                            Parms = new List<string>() { item.Id.ToString(), rigJob?.Rig.Id.ToString(), rigJob?.CallSheetId.ToString(), rigJob?.Id.ToString() },
                            ControllerName = ProductHaulController,
                            ActionName = "CancelProductHaul",
                            //MenuStyle = item.BlendSectionId == blendSectionId ? GetProductHualLoadMenuStyle(item.Status) : "",
                            MenuTips = item.ProductHaulLifeStatus.ToString(),
                            IsDisabled = item.ProductHaulLifeStatus != ProductHaulStatus.Scheduled,

                            MenuList = SetCancelProductHaulLoadForRigBoardBin(item, rigJob)
                        };
                        list.Add(contextMenu);
                    }
                }
            }
            return list;
        }
        #endregion

        #endregion
    }
}