using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using eServiceOnline.BusinessProcess;
using eServiceOnline.Data;
using eServiceOnline.Gateway;
using eServiceOnline.Models;
using eServiceOnline.Models.Calendar;
using eServiceOnline.Models.Commons;
using eServiceOnline.Models.CrewBoard;
using eServiceOnline.Models.ProductHaul;
using eServiceOnline.Models.UnitBoard;
using eServiceOnline.Models.WorkerBoard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Sanjel.BusinessEntities.Sections.Common;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Crew;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ResourceSchedule;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment;
using Sesi.SanjelData.Entities.Common.BusinessEntities.HumanResources;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Organization;
using Syncfusion.JavaScript;
using RigJob = Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch.RigJob;

namespace eServiceOnline.Controllers
{
    public class CrewBoardController : eServiceOnlineController
    {
        private readonly eServiceWebContext _context;

        public CrewBoardController()
        {
            this._context = new eServiceWebContext();
        }

        public ActionResult GetPagedCrewModels([FromBody] DataManager dataManager)
        {
            int pageSize = dataManager.Take;
            int pageNumber = dataManager.Skip/pageSize + 1;
            int count;

            if (this.HttpContext.Session.GetString("ServicePoint") == null)
            {
                string retrievalstr = JsonConvert.SerializeObject(new RetrievalCondition());
                this.HttpContext.Session.SetString("ServicePoint", retrievalstr);
            }
            RetrievalCondition retrieval = JsonConvert.DeserializeObject<RetrievalCondition>(this.HttpContext.Session.GetString("ServicePoint"));
            if (retrieval.IsChange)
            {
                retrieval.IsChange = false;
                retrieval.PageNumber = 1;
            }
            else
            {
                if (pageNumber != 1) retrieval.PageNumber = pageNumber;
            }

            this.HttpContext.Session.SetString("ServicePoint", JsonConvert.SerializeObject(retrieval));
            List<Collection<int>> resuhSet = retrieval.ResuhSet(retrieval);
            Collection<int> servicePoints = Utility.GetSearchCollections(resuhSet[0]);
            List<SanjelCrew> crews = this._context.GetCrewsByServicePoint(servicePoints, out count);
            //Apr 26, 2023:DRB 2.1 - Hide pumper crew list from old resource board
            crews = crews.FindAll(p => p.Type.Id != 1 && p.Type.Id != 4);
            List<SanjelCrew> sortedCrews = crews.OrderBy(p => p.Type.Id).ToList().Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();
            List<CrewViewModel> data = this.GetCrewViewModelList(sortedCrews).OrderBy(p => p.Units.PropertyValue).ToList();

            return this.Json(new {result = data, count = count });
        }

        private List<CrewViewModel> GetCrewViewModelList(List<SanjelCrew> crews)
        {
            List<CrewViewModel> data = new List<CrewViewModel>();
            foreach (SanjelCrew crew in crews)
            {
                CrewViewModel crewViewModel = new CrewViewModel();
                crewViewModel.UnitModels = this.GetUnitModels(crew);
                crewViewModel.EmployeeModels = this.GetEmployeeModels(crew);
//                crewViewModel.ScheduleModels = this.GetScheduleModels(crew).OrderBy(s=>s.EndTime).ToList();
                crewViewModel.NoteModel = this.GetNoteModel(crew);
                crewViewModel.PopulateFrom(crew);
                data.Add(crewViewModel);
            }

            return data;
        }

        private NoteModel GetNoteModel(SanjelCrew crew)
        {
            NoteModel noteModel=new NoteModel();
            SanjelCrewNote note = this._context.GetSanjelCrewNoteBySanjelCrewId(crew.Id);
            noteModel.Id = crew.Id;
            noteModel.Notes = note?.Description ?? string.Empty;
            noteModel.ReturnActionName = "Index";
            noteModel.ReturnControllerName = "ResourceBoard";
            noteModel.CallingControllerName = "CrewBoard";
            noteModel.CallingMethodName = "UpdateNotes";
            noteModel.PostControllerName = "CrewBoard";
            noteModel.PostMethodName = "UpdateNotes";
            return noteModel;
        }

        private List<ScheduleModel> GetScheduleModels(SanjelCrew crew)
        {
            List<ScheduleModel> list = new List<ScheduleModel>();
            List<SanjelCrewSchedule> crewSchedules = this._context.GetFutureCrewSchedules(crew.Id, DateTime.Now);
            if (crewSchedules != null && crewSchedules.Count > 0)
            {
                foreach (SanjelCrewSchedule crewSchedule in crewSchedules)
                {
                    ScheduleModel model = new ScheduleModel();
                    model.PopulateFrom(crewSchedule);

                    ProductHaul productHaul = eServiceWebContext.Instance.GetProductHaulByCrewScheduleId(crewSchedule.Id).Find(s=>s.IsThirdParty==false);
                    if (productHaul!=null)
                    {
                        if (productHaul.ProductHaulLifeStatus == ProductHaulStatus.OnLocation)
                        {
                            continue;                            
                        }
                    }
                    RigJobSanjelCrewSection rigJobCrewSection = this._context.GetRigJobCrewSectionById(crewSchedule.RigJobSanjelCrewSection.Id);
                    if (rigJobCrewSection != null)
                    {

                        RigJob rigJob = this._context.GetRigJobById(rigJobCrewSection.RigJob.Id);
                        if (rigJob.JobLifeStatus == JobLifeStatus.Canceled || rigJob.JobLifeStatus==JobLifeStatus.Completed || rigJob.JobLifeStatus==JobLifeStatus.Deleted)
                        {
                            continue;

                        }

                        if (rigJob.ClientCompany != null)
                        {
                            model.ClientCompanyName = string.IsNullOrEmpty(rigJob.ClientCompany.ShortName) ? rigJob.ClientCompany.Name : rigJob.ClientCompany.ShortName;
                            model.RigName = rigJob.Rig == null ? string.Empty : rigJob.Rig.Name;
                        }
                                                                                      
                        model.JobType = rigJob.JobType?.Name;
                        model.StatusName = rigJobCrewSection.RigJobCrewSectionStatus.ToString();
                        
                    }
                    list.Add(model);
                }
            }
            return list.OrderBy(s=>s.StartTime).ToList();
        }

        private List<EmployeeModel> GetEmployeeModels(SanjelCrew crew)
        {
            List<EmployeeModel> list = new List<EmployeeModel>();
            List<Employee> employees = this._context.GetEmployeesByCrew(crew.Id);
            if (employees != null && employees.Count > 0)
            {
                foreach (Employee employee in employees)
                {
                    EmployeeModel model = new EmployeeModel();
                    model.PopulateFrom(employee);
                    list.Add(model);
                }
            }

            return list;
        }

        private List<TruckUnitModel> GetUnitModels(SanjelCrew crew)
        {
            List<TruckUnitModel> list = new List<TruckUnitModel>();
            List<TruckUnit> truckUnits = this._context.GetTruckUnitsByCrew(crew.Id);
            if (truckUnits != null && truckUnits.Count > 0)
            {
                foreach (TruckUnit truckUnit in truckUnits)
                {
                    TruckUnitModel model = new TruckUnitModel();
                    model.PopulateFrom(truckUnit);
                    list.Add(model);
                }
            }

            return list;
        }

/*
        public List<Employee> GetActivatedEmployees(int crewId)
        {
            List<Employee> employeeList = this._context.GetActivatedEmployees(crewId).OrderBy(a => a.LastName).ToList();
            List<SelectListItem> employeeItems =
                employeeList.Select(p => new SelectListItem{Text = $"{p.LastName}, {p.FirstName} ({p.EmployeeNumber})",Value = p.Id.ToString()}).ToList();
            this.ViewData["employeeItems"] = employeeItems;

            return employeeList;
        }
*/

/*
        public void GetActivatedTruckUnits(int crewId)
        {
            List<TruckUnit> truckUnits = this._context.GetActivatedTruckUnits(crewId).OrderBy(a => a.UnitNumber).ToList();
            List<SelectListItem> truckUnitItems =
                truckUnits.Select(p => new SelectListItem {Text = p.UnitNumber, Value = p.Id.ToString()}).ToList();
            this.ViewData["truckUnitItems"] = truckUnitItems;
        }
*/

        public void GetCrewPositions()
        {
            List<CrewPosition> crewPositions = this._context.GetCrewPositions();
            List<SelectListItem> crewPositionItems = crewPositions.Select(p => new SelectListItem { Text = p.Name, Value = p.Id.ToString() }).ToList();
            this.ViewData["crewPositionItems"] = crewPositionItems;
        }

        public void GetServicePoints()
        {
            RetrievalCondition retrieval = JsonConvert.DeserializeObject<RetrievalCondition>(this.HttpContext.Session.GetString("ServicePoint"));
            List<Collection<int>> resuhSet = retrieval.ResuhSet(retrieval);
            Collection<int> servicePointIds = Utility.GetSearchCollections(resuhSet[0]);
            int servicePointId = servicePointIds.FirstOrDefault();
            List<ServicePoint> servicePoints = this._context.GetServicePoints().OrderBy(p => p.Name).ToList();
            List<SelectListItem> servicePointItems = servicePoints.Select(p => new SelectListItem { Text = p.Name, Value = p.Id.ToString(), Selected = p.Id == servicePointId }).ToList();
            this.ViewData["servicePointItems"] = servicePointItems;
        }

        public void GetServicePointList()
        {
            List<ServicePoint> servicePoints = this._context.GetServicePoints().OrderBy(p => p.Name).ToList();
            List<SelectListItem> servicePointItems = servicePoints.Select(p => new SelectListItem { Text = p.Name, Value = p.Id.ToString()}).ToList();
            this.ViewData["servicePointList"] = servicePointItems;
        }

        public void GetCrewTypes()
        {
            List<CrewType> crewTypes = this._context.GetCrewTypesForSanjelCrew().OrderBy(a => a.Id).ToList();
            List<SelectListItem> crewTypeItems = crewTypes.Select(p => new SelectListItem { Text = p.Name, Value = p.Id.ToString() }).ToList();
            this.ViewData["crewTypeItems"] = crewTypeItems;
        }

        public ActionResult CreateCrew()
        {
            this.GetCrewTypes();
            this.GetServicePoints();
            this.GetTruckUnitList();
            this.GetEmployeeList();
            CrewModel model = new CrewModel();

            return this.PartialView("_CreateCrew", model);
        }

        [HttpPost]
        public ActionResult CreateCrew(CrewModel model)
        {
            SanjelCrew crew = new SanjelCrew();
            crew.SanjelCrewWorkerSection = new List<SanjelCrewWorkerSection>();
            crew.SanjelCrewTruckUnitSection = new List<SanjelCrewTruckUnitSection>();
            model.PopulateTo(crew);

            crew.Type = this._context.GetCrewTypeById(model.CrewTypeId);


            this._context.CreateCrew(crew, model.HomeDistrictId, model.PrimaryTruckUnitId, model.SecondaryTruckUnitId, model.SupervisorId, model.CrewMemberId);
            if (!string.IsNullOrEmpty(model.Notes))
            {
                SanjelCrewNote note = new SanjelCrewNote
                {
                    Name = model.Notes,
                    Description = model.Notes,
                    SanjelCrew = crew,
                };
                this._context.CreateSanjelCrewNote(note);
            }

            return this.RedirectToAction("Index", "ResourceBoard");
        }

        public ActionResult VerifyCrewSchedule(int crewId)
        {
            List<SanjelCrewSchedule> crewSchedules = this._context.GetFutureCrewSchedules(crewId, DateTime.Now);
            if (crewSchedules.Count > 0)
            {
                return this.Json(new{state=false,data=crewSchedules.FirstOrDefault()});
            }
            return this.Json(new {state=true});
        }

        public ActionResult AddUnit(List<string> parms)
        {
            CrewModel model = new CrewModel();
            model.Id = Int32.Parse(parms[0]);
            this.GetTruckUnitList();

            return this.PartialView("_AddUnit", model);
        }

        public void GetTruckUnitList()
        {
            List<TruckUnit> truckUnits = this._context.GetTruckUnitList();
            List<SelectListItem> truckUnitItems = truckUnits.Select(p => new SelectListItem { Text = p.UnitNumber, Value = p.Id.ToString() }).ToList();
            this.ViewData["truckUnitItems"] = truckUnitItems;
        }

        public void GetEmployeeList()
        {
            List<Employee> employeeList = this._context.GetEmployeeList();
            List<SelectListItem> employeeItems = employeeList.Select(p => new SelectListItem { Text = $"{p.LastName}, {p.FirstName} ({p.EmployeeNumber})", Value = p.Id.ToString() }).ToList();
            this.ViewData["employeeItems"] = employeeItems;
        }

        [HttpPost]
        public ActionResult AddUnit(CrewModel model)
        {
            this._context.AddUnitToCrew(model.TruckUnitId, model.Id);

            return this.RedirectToAction("Index", "ResourceBoard");
        }

        public ActionResult AddWorker(List<string> parms)
        {
            CrewModel model = new CrewModel();
            model.Id = Int32.Parse(parms[0]);
            this.GetEmployeeList();
            this.GetCrewPositions();

            return this.PartialView("_AddWorker", model);
        }

        [HttpPost]
        public ActionResult AddWorker(CrewModel model)
        {
            this._context.AddWorkerToCrew(model.WorkerId, model.Id, model.CrewPositionId);
            return this.RedirectToAction("Index", "ResourceBoard");
        }

        public ActionResult RemoveUnit(List<string> parms)
        {
            CrewModel model = new CrewModel();
            model.Id = Int32.Parse(parms[1]);
            model.TruckUnitId = Int32.Parse(parms[0]);

            return this.PartialView("_RemoveUnit", model);
        }

        [HttpPost]
        public ActionResult RemoveUnit(CrewModel model)
        {
            this._context.RemoveUnitFromCrew(model.TruckUnitId, model.Id);
            return this.RedirectToAction("Index", "ResourceBoard");
        }

        public ActionResult RemoveWorker(List<string> parms)
        {
            CrewModel model = new CrewModel();
            model.Id = Int32.Parse(parms[1]);
            model.WorkerId = Int32.Parse(parms[0]);

            return this.PartialView("_RemoveWorker", model);
        }

        [HttpPost]
        public ActionResult RemoveWorker(CrewModel model)
        {
            this._context.RemoveWorkerFromCrew(model.WorkerId, model.Id);
            return this.RedirectToAction("Index", "ResourceBoard");
        }

        public ActionResult UpdateRotationOrder(List<string> parms)
        {
            CrewModel model = new CrewModel();
            SanjelCrew crew = this._context.GetCrewById(Int32.Parse(parms[0]));
            model.PopulateFrom(crew);

            return this.PartialView("_UpdateRotationOrder", model);
        }

        [HttpPost]
        public ActionResult UpdateRotationOrder(CrewModel model)
        {
            SanjelCrew crew = this._context.GetCrewById(model.Id);
            if (crew != null)
            {
                crew.Rotation = model.Rotation;
            }
            this._context.UpdateCrew(crew);

            return this.RedirectToAction("Index", "ResourceBoard");
        }

/*
        public ActionResult UpdateCrewNotes(List<string> parms)
        {
            CrewModel model = new CrewModel();
            SanjelCrew crew = this._context.GetCrewById(Int32.Parse(parms[0]));
            model.PopulateFrom(crew);

            return this.PartialView("_UpdateCrewNotes", model);
        }

        [HttpPost]
        public ActionResult UpdateCrewNotes(CrewModel model)
        {
            SanjelCrew crew = this._context.GetCrewById(model.Id);
            if (crew != null)
            {
                crew.Notes = model.Notes;
            }
            this._context.UpdateCrew(crew);

            return this.RedirectToAction("Index", "Resource");
        }
*/


        public ActionResult UpdateWorkerProfile(List<string> parms)
        {
            EmployeeModel model =new EmployeeModel();
            Employee employee = this._context.GetEmployeeById(Int32.Parse(parms[0]));
            model.PopulateFrom(employee);
            return this.PartialView("_UpdateWorkerProfile", model);
        }

        [HttpPost]
        public ActionResult UpdateWorkerProfile(EmployeeModel model)
        {
            Employee employee = this._context.GetEmployeeById(model.Id);
            if (employee!=null)
            {
                employee.Description = model.Notes;
            }

            this._context.UpdateWorker(employee);
            return this.RedirectToAction("Index", "ResourceBoard");
        }

        public ActionResult RemoveCrew(List<string> parms)
        {
            CrewModel model = new CrewModel();
            model.Id = Int32.Parse(parms[0]);

            return this.PartialView("_RemoveCrew", model);
        }
        public ActionResult RemoveAllWorkers(List<string> parms)
        {
            CrewModel model = new CrewModel();
            model.Id = Int32.Parse(parms[0]);

            return this.PartialView("_RemoveAllWorkers", model);
        }
        [HttpPost]
        public ActionResult RemoveCrew(CrewModel model)
        {
            this._context.RemoveCrew(model.Id);
            return this.RedirectToAction("Index", "ResourceBoard");
        }

        public ActionResult AssignToAnotherDistrict(List<string> parms)
        {
            this.GetServicePointList();
            CrewModel model = new CrewModel();
            model.Id = Int32.Parse(parms[0]);
            model.WorkingDistrictId = Int32.Parse(parms[1]);

            return this.PartialView("_AssignToAnotherDistrict", model);
        }

        [HttpPost]
        public ActionResult AssignToAnotherDistrict(CrewModel model)
        {
            this._context.AssignCrewToAnotherDistrict(model.Id, model.WorkingDistrictId);
            return this.RedirectToAction("Index", "ResourceBoard");
        }

        #region Notes

        [HttpPost]
        public override IActionResult UpdateNotes(NoteModel model)
        {
            eServiceWebContext.Instance.UpdateSanjelCrewNote(model.Id, model.Notes);
            return this.RedirectToAction(model.ReturnActionName, model.ReturnControllerName);
        }

        #endregion Notes
        [HttpPost]
        public IActionResult RemoveAllWorkers(CrewModel model)
        {
            this._context.RemoveAllWorker(model.Id);
            return this.RedirectToAction("Index","ResourceBoard");
        }
        [HttpGet]
        public async Task<ActionResult> UpdatePumperCrewAssignmentAsync(int rigJobId)
        {
            await Task.Run(()=>eServiceWebContext.Instance.AlignPumperCrewAssignment(rigJobId));
            string message = "Succeed";
            bool result = true;
            return  new JsonResult(new { result, message });
        }
        [HttpGet]
        public ActionResult UpdatePumperCrewAssignment(int rigJobId)
        {
            eServiceWebContext.Instance.AlignPumperCrewAssignment(rigJobId);

            string message = "Succeed";
            bool result = true;
            return new JsonResult(new { result, message });
        }

        [HttpGet]
        public async Task<ActionResult> SetBulkerCrewStatusAsync(int crewId, BulkerCrewStatus status)
        {
            string message = await Task.Run(() => eServiceWebContext.Instance.SetBulkerCrewStatusByCrewId(crewId, status, false, ""));
            bool result = message.Equals("Succeed");
            return new JsonResult(new { result, message });
        }
        [HttpGet]
        public async Task<ActionResult> UpdateBulkerCrewStatusAsync(int crewId, BulkerCrewStatus status, bool isThirdParty, string userName)
        {

            string message = await Task.Run(() => eServiceWebContext.Instance.SetBulkerCrewStatusByCrewId(crewId, status, isThirdParty, string.IsNullOrEmpty(userName)?"DRB":userName));
            bool result = message.Equals("Succeed");
            return new JsonResult(new { result, message });
        }

        [HttpGet]
        public async Task<ActionResult> GetShiftScheduleEndDateAsync(int employeeId)
        {
	        WorkerSchedule workerSchedule = await Task.Run(() => eServiceWebContext.Instance.GetShiftScheduleEndDate(employeeId));
	        if (workerSchedule == null)
		        return new JsonResult("No shift schedules found." );
            else
		        return new JsonResult(new { EmployeeId = employeeId, ScheduleEndTime = workerSchedule.EndTime, ScheduleType = workerSchedule.Type.GetDescription(), RotationId = workerSchedule.Rotation.Id });
        }

        [HttpGet]
        public async Task<ActionResult> ExtendShiftScheduleEndDateAsync(int employeeId, int rotationId, DateTime startDateTime, DateTime endDateTime)
        {
	        int rotationIndex = eServiceOnlineGateway.Instance.GetRotationTemplateById(rotationId).RotationIndex;
	        int result = await Task.Run(() => eServiceWebContext.Instance.ExtendShiftScheduleEndDate(employeeId, rotationIndex, startDateTime, endDateTime));
	        return new JsonResult(new { Succeed = result });
        }


    }
}