using System;
using System.Collections.Generic;
using System.Linq;
using eServiceOnline.Models.Calendar;
using eServiceOnline.Models.Commons;
using eServiceOnline.Models.ProductHaul;
using eServiceOnline.Models.UnitBoard;
using eServiceOnline.Models.WorkerBoard;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Crew;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule;

namespace eServiceOnline.Models.CrewBoard
{
    public class CrewViewModel : ModelBase<SanjelCrew>
    {
        #region const
        public const string CrewBoardController = "CrewBoard";
        public const string ComonController = "eServiceOnline";
        public const string StyleCrewIsUnavailable = "crew-unavailable";
        public const string StyleCrewIsOnDuty = "crew-onduty";

        #region Crew

        public const string MenuAddNewCrew = "Add A Crew";
        public const string MenuRemoveCrew = "Remove Crew";
        public const string MenuAssignDistrict = "Assign To Another District";

        #endregion

        #region Unit

        public const string MenuAddUnit = "Add Unit";
        public const string MenuRemoveUnit = "Remove Unit";

        #endregion

        #region Worker

        public const string MenuAddWorker = "Add Worker";
        public const string MenuRemoveAllWorker = "Remove all Workers";
        public const string MenuRemoveWorker = "Remove Worker";

        #endregion

        #region Job Info

        public const string MenuLogOnDuty = "Log On Duty";
        public const string MenuLogOffDuty = "Log Off Duty";

        #endregion

        #region Notes


        #endregion

        #region Rotation

        public const string MenuUpdateRotationOrder = "Update Rotation Order";

        #endregion

        #endregion const

        #region Constructor

        public CrewViewModel()
        {
            this.UnitModels = new List<TruckUnitModel>();
            this.EmployeeModels = new List<EmployeeModel>();
            this.ScheduleModels = new List<ScheduleModel>();
        }
            
        #endregion Constructor

        #region Properties

        public List<TruckUnitModel> UnitModels { get; set; }
        public List<EmployeeModel> EmployeeModels { get; set; }
        public List<ScheduleModel> ScheduleModels { get; set; }

        public StyledCell Units { get; set; }
        public StyledCell Workers { get; set; }
        public StyledCell JobInformation { get; set; }
        public StyledCell Notes { get; set; }
        public string CrewType { get; set; }
        public int CrewTypeId { get; set; }
        public StyledCell Rotation { get; set; }

        public List<CrewScheduleViewModel> CrewScheduleViewModels { get; set; }
        public string JobInfo { get; set; }

        #endregion

        #region Methods

        public override void PopulateFrom(SanjelCrew crew)
        {
            if (crew == null) throw new Exception("entity must be instance of class SanjelCrew.");
            this.Units = this.GetUnitStyledCell("Unit", crew);
            this.Workers = this.GetWorkersStyledCell("Worker", crew);
            this.JobInformation = this.GetJobInformationStyledCell("JobInformation", crew);
            this.Notes = this.GetNotesStyledCell("Notes", crew);
            this.CrewType = this.GetCrewTypeStyledCell("Type", crew);
            this.CrewTypeId = this.GetCrewTypeIdStyledCell("TypeId", crew);
            this.Rotation = this.GetRotationStyledCell("Rotation", crew);
        }

        #region Compute unit information from entity

        private StyledCell GetUnitStyledCell(string propertyName, SanjelCrew crew)
        {
            if (this.UnitModels == null || this.UnitModels.Count < 1)
            {
                return new StyledCell(propertyName, null, this.LoggedUser, null) { IsNeedRowMerge = false, ContextMenus = this.SetUnitContextMenus(crew) };
            }
            string unitNumbers = string.Empty;
            foreach (TruckUnitModel unitModel in this.UnitModels)
            {
                unitNumbers += " | " + unitModel.UnitNumber;
            }
            StyledCell styledCell = new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue = unitNumbers.Substring(2), ContextMenus = this.SetUnitContextMenus(crew) };

            return styledCell;
        }

        private List<ContextMenu> SetUnitContextMenus(SanjelCrew crew)
        {
            List<ContextMenu> list = new List<ContextMenu>();
            list.Add(new ContextMenu
            {
                MenuName = MenuAddUnit,
                ProcessingMode = ProcessingMode.PopsUpWindow,
                Parms = new List<string>() { crew.Id.ToString()},
                ControllerName = CrewBoardController,
                ActionName = "AddUnit"
            });

            if (this.UnitModels != null && this.UnitModels.Count > 0)
            {
                list.Add(new ContextMenu
                {
                    MenuName = MenuRemoveUnit,
                    ProcessingMode = ProcessingMode.HaveNextMenu,
                    MenuList = this.GetRemoveUnitContextMenus(crew)
                });
            }

            list.Add(new ContextMenu
            {
                MenuName = MenuAddNewCrew,
                ProcessingMode = ProcessingMode.PopsUpWindow,
                ControllerName = CrewBoardController,
                ActionName = "CreateCrew",
                IsHaveSplitLine = true
            });

            list.Add(new ContextMenu
            {
                MenuName = MenuRemoveCrew,
                ProcessingMode = ProcessingMode.PopsUpWindow,
                Parms = new List<string> { crew.Id.ToString() },
                ControllerName = CrewBoardController,
                ActionName = "RemoveCrew",
                IsDisabled = this.ScheduleModels.Count > 0
            });

            /*list.Add(new ContextMenu
            {
                MenuName = MenuAssignDistrict,
                ProcessingMode = ProcessingMode.PopsUpWindow,
                Parms = new List<string> { crew.Id.ToString(), crew.WorkingServicePoint.Id.ToString() },
                ControllerName = CrewBoardController,
                ActionName = "AssignToAnotherDistrict",
                IsDisabled = this.ScheduleModels.Count > 0
            });*/

            return list;
        }

        private List<ContextMenu> GetRemoveUnitContextMenus(SanjelCrew crew)
        {
            List<ContextMenu> list = new List<ContextMenu>();
            foreach (TruckUnitModel item in this.UnitModels)
            {
                ContextMenu menu = new ContextMenu
                {
                    MenuName = item.UnitNumber,
                    ProcessingMode = ProcessingMode.PopsUpWindow,
                    Parms = new List<string> { item.Id.ToString(), crew.Id.ToString() },
                    ControllerName = CrewBoardController,
                    ActionName = "RemoveUnit"
                };
                list.Add(menu);
            }
            return list;
        }
        #endregion

        #region Compute workers information from entity

        private StyledCell GetWorkersStyledCell(string propertyName, SanjelCrew crew)
        {

            if (this.EmployeeModels == null || this.EmployeeModels.Count < 1)
            {
                return new StyledCell(propertyName, null, this.LoggedUser, null) { IsNeedRowMerge = false, ContextMenus = this.SetWorkersContextMenus(crew)};
            }
            string workers = string.Empty;
            foreach (EmployeeModel workerModel in this.EmployeeModels)
            {
                workers += " | " + workerModel.PreferedName;
            }
            StyledCell styledCell = new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue = workers.Substring(2), ContextMenus = this.SetWorkersContextMenus(crew)};

            return styledCell;
        }

        private List<ContextMenu> SetWorkersContextMenus(SanjelCrew crew)
        {
            List<ContextMenu> list = new List<ContextMenu>();
            list.Add(new ContextMenu()
            {
                MenuName = MenuAddWorker,
                ProcessingMode = ProcessingMode.PopsUpWindow,
                Parms = new List<string>() { crew.Id.ToString()},
                ControllerName = CrewBoardController,
                ActionName = "AddWorker"
            });

            if (this.EmployeeModels != null && this.EmployeeModels.Count > 0)
            {
                list.Add(new ContextMenu
                {
                    MenuName = MenuRemoveWorker,
                    ProcessingMode = ProcessingMode.HaveNextMenu,
                    MenuList = this.GetRemoveWorkerContextMenus(crew)
                });
                list.Add(new ContextMenu
                {
                    MenuName = MenuRemoveAllWorker,
                    ProcessingMode = ProcessingMode.PopsUpWindow,
                    ControllerName = CrewBoardController,
                    ActionName = "RemoveAllWorkers",
                    Parms = new List<string> {crew.Id.ToString() },
                });
            }

            list.Add(new ContextMenu()
            {
                MenuName = MenuAddNewCrew,
                ProcessingMode = ProcessingMode.PopsUpWindow,
                ControllerName = CrewBoardController,
                ActionName = "CreateCrew",
                IsHaveSplitLine = true
            });

            list.Add(new ContextMenu
            {
                MenuName = MenuRemoveCrew,
                ProcessingMode = ProcessingMode.PopsUpWindow,
                Parms = new List<string> { crew.Id.ToString() },
                ControllerName = CrewBoardController,
                ActionName = "RemoveCrew",
                IsDisabled = this.ScheduleModels.Count > 0
            });

            return list;
        }

        private List<ContextMenu> GetRemoveWorkerContextMenus(SanjelCrew crew)
        {
            List<ContextMenu> list = new List<ContextMenu>();
            foreach (EmployeeModel item in this.EmployeeModels)
            {
                ContextMenu menu = new ContextMenu
                {
                    MenuName = item.PreferedName,
                    ProcessingMode = ProcessingMode.PopsUpWindow,
                    Parms = new List<string> { item.Id.ToString(), crew.Id.ToString() },
                    ControllerName = CrewBoardController,
                    ActionName = "RemoveWorker"
                };
                list.Add(menu);
            }
            return list;
        }

        #endregion

        #region Compute job information from entity

        private StyledCell GetJobInformationStyledCell(string propertyName, SanjelCrew crew)
        {
            string jobInfo = string.Empty;
            if (this.ScheduleModels?.Count > 0)
            {
                if (!crew.Type.Name.Equals("Bulker Crew"))
                {
                    foreach (ScheduleModel scheduleModel in this.ScheduleModels)
                    {
                        if (scheduleModel.EndTime > DateTime.Now)
                        {
                            var scheduleModels = ScheduleModels.FindAll(s => s.Id != scheduleModel.Id);
                            List<ScheduleModel> conflictSchedules = scheduleModels.ToList().FindAll(p => (p.StartTime >= scheduleModel.StartTime && p.StartTime < scheduleModel.EndTime) || (p.EndTime > scheduleModel.StartTime && p.EndTime <= scheduleModel.EndTime) || (p.StartTime <= scheduleModel.StartTime && p.EndTime >= scheduleModel.EndTime));
                            if (conflictSchedules.Count!=0)
                            {
                                jobInfo = scheduleModel.Comment;
                                foreach (var item in conflictSchedules)
                                {
                                    jobInfo = jobInfo +"|"+ item.Comment;
                                }
                            }
                            else
                            {
                                jobInfo = scheduleModel.Comment;
                            }
                            break;
                        }
                    }
                }
                else
                {
                    foreach (ScheduleModel scheduleModel in this.ScheduleModels)
                    {
                        if (scheduleModel.EndTime > DateTime.Now)
                        {                            
                            jobInfo = scheduleModel.Comment;
                            break;                                                      
                        }
                    }
                }                                                
            }                                     
            return new StyledCell(propertyName, null, this.LoggedUser, null){ PropertyValue = jobInfo };
        }
       
        #endregion

        #region Compute notes from entity

        private StyledCell GetNotesStyledCell(string propertyName, SanjelCrew crew)
        {
            return new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue =this.NoteModel.Notes, ContextMenus = SetNoteContextMenus(crew.Id.ToString()) };
        }


        #endregion

        #region Compute type from entity

        private string GetCrewTypeStyledCell(string propertyName, SanjelCrew crew)
        {
            return crew.Type.Name;
        }

        private int GetCrewTypeIdStyledCell(string propertyName, SanjelCrew crew)
        {
            return crew.Type.Id;
        }

        #endregion

        #region Compute Rotation Order from entity

        private StyledCell GetRotationStyledCell(string propertyName, SanjelCrew crew)
        {
            return new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue = crew.Rotation, ContextMenus = this.SetRotationContextMenus(crew) };
        }

        private List<ContextMenu> SetRotationContextMenus(SanjelCrew crew)
        {
            List<ContextMenu> list = new List<ContextMenu>();
            list.Add(new ContextMenu
            {
                MenuName = MenuUpdateRotationOrder,
                ProcessingMode = ProcessingMode.PopsUpWindow,
                Parms = new List<string> { crew.Id.ToString() },
                ControllerName = CrewBoardController,
                ActionName = "UpdateRotationOrder"
            });

            return list;
        }

        #endregion

        #endregion
    }
}
