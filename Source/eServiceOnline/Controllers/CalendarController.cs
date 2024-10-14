using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using eServiceOnline.Data;
using eServiceOnline.Models;
using eServiceOnline.Models.Calendar;
using eServiceOnline.Models.Commons;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Crew;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ResourceSchedule;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment;
using Sesi.SanjelData.Entities.Common.BusinessEntities.HumanResources;
using static System.String;

namespace eServiceOnline.Controllers
{
    public class CalendarController : eServicePageController
    {
        public ActionResult Index(string selectedDistricts = null)
        {
            ViewBag.HighLight = "Calendar";
            ViewBag.VersionNumber = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

            SetDistrictSelection(selectedDistricts);

            return View();
        }
        [HttpPost]
        public IActionResult GetContextMenu(List<string> param)
        {
            List<ContextMenu> contextMenus=ContextMenuFactory.ContextMenuFactory.GetContextMenus(param);
            return Json(contextMenus);
        }

        private Collection<int> GetServicePoints()
        {
            Collection<int> servicePoints=new Collection<int>();
            if (this.HttpContext.Session.GetString("ServicePoint") != null) 
            {
                RetrievalCondition retrieval = JsonConvert.DeserializeObject<RetrievalCondition>(this.HttpContext.Session.GetString("ServicePoint"));
                List<Collection<int>> resuhSet = retrieval.ResuhSet(retrieval);
                servicePoints = Utility.GetSearchCollections(resuhSet[0]);
            }
            return servicePoints;
        }

        [HttpPost]
        public IActionResult GetData(string startTime, string viewStyle, string viewType)
        {
            DateTime calendarStartTime = DateTime.Parse(startTime);
            int days = 1;
            if (viewStyle.Equals("TimelineDay"))
            {
                days = 1;
            }
            else if (viewStyle.Equals("TimelineWeek"))
            {
                days = 7;
            }
            else if (viewStyle.Equals("TimelineMonth"))
            {
                days = 31;
            }
            DateTime calendarEndTime = calendarStartTime.AddDays(days);

            if(viewType == "Crews")
                return Json(GetCrewViewScheduleDataModel(calendarStartTime,calendarEndTime));
            else if (viewType == "Workers")
                return Json(GetWorkerViewScheduleDataModel(calendarStartTime,calendarEndTime));
            else if (viewType == "Units")
                return Json(GetUnitViewScheduleDataModel(calendarStartTime, calendarEndTime));
            else 
                return null;

        }

        [HttpGet]
        public IActionResult GetCrewData()
        {
            var viewScheduleDataModel = GetCrewViewScheduleDataModel(DateTime.Now, DateTime.Now.AddDays(1));
            return Json(viewScheduleDataModel);
        }

        private ViewScheduleDataModel GetCrewViewScheduleDataModel(DateTime startTime, DateTime endTime)
        {
            ViewScheduleDataModel viewScheduleDataModel = new ViewScheduleDataModel();

            List<ScheduleModel> models = new List<ScheduleModel>();
            List<SanjelCrewSchedule> crewSchedules = eServiceWebContext.Instance.GetCrewSchedules(GetServicePoints(),startTime,endTime);
            foreach (var crewSchedule in crewSchedules)
            {
                if (IsNullOrEmpty(crewSchedule.Name))  
                    continue;
                ScheduleModel model = new ScheduleModel();
                model.PopulateFrom(crewSchedule);
                models.Add(model);
            }
            List<ScheduleStyleModel> scheduleStyleModels = new List<ScheduleStyleModel>();
            
            List<SanjelCrew> crews = eServiceWebContext.Instance.GetAllCrewsInfo(GetServicePoints()).OrderBy(s => s.Name)
                    .ToList();
            
            foreach (var crew in crews)
            {
                if (IsNullOrEmpty(crew.Name))  
                    continue;
                ScheduleStyleModel scheduleStyleModel = new ScheduleStyleModel();
                scheduleStyleModel.PopulateFrom(crew);
                scheduleStyleModels.Add(scheduleStyleModel);
            }

            viewScheduleDataModel.ScheduleModels = models;
            viewScheduleDataModel.ScheduleStyleModels = scheduleStyleModels;
            return viewScheduleDataModel;
        }

        public IActionResult GetWorkData()
        {
            ViewScheduleDataModel viewScheduleDataModel = GetWorkerViewScheduleDataModel(DateTime.Now, DateTime.Now.AddDays(1));
            return Json(viewScheduleDataModel);
        }
        private ViewScheduleDataModel GetWorkerViewScheduleDataModel(DateTime startTime, DateTime endTime)
        {
            ViewScheduleDataModel viewScheduleDataModel = new ViewScheduleDataModel();

            List<ScheduleModel> models = new List<ScheduleModel>();
            Collection<int> servicePoints = this.GetServicePoints();

            List<WorkerSchedule> workerSchedules = eServiceWebContext.Instance.GetWorkerSchedules(GetServicePoints(),startTime,endTime);          
            foreach (var workerSchedule in workerSchedules)
            {
                ScheduleModel model = new ScheduleModel();
                model.PopulateFrom(workerSchedule);
                models.Add(model);               
            }


            List<ScheduleStyleModel> scheduleStyleModels = new List<ScheduleStyleModel>();
            List<Employee> employeesList;
//            if (servicePoints.Count !=0)
//            {
                List<Employee> newEmployees = new List<Employee>();
                foreach (var workerSchedule in workerSchedules)
                {
                    Employee employee = new Employee();
                    employee = eServiceWebContext.Instance.GetEmployeeById(workerSchedule.Worker?.Id ?? 0);
                    if(employee != null) newEmployees.Add(employee);
                }
                /*
                List<Employee> employees = eServiceWebContext.Instance.GetEmployeeList().Where(p=> servicePoints.Contains(p.ServicePoint.Id)).OrderBy(s => s.LastName).ToList();
                //取2个集合的所有employee
                List<Employee> finalEmployees = employees.Union(newEmployees).ToList();
                //去掉id相同的employee
                employeesList = finalEmployees.Where((x, i) => finalEmployees.FindIndex(z => z.Id == x.Id) == i).OrderBy(s=>s.LastName).ToList();
                */

               
                /*foreach (var employee in employeesList)
                {
                    ScheduleStyleModel scheduleStyleModel = new ScheduleStyleModel();
                    scheduleStyleModel.PopulateFrom(employee);
                    scheduleStyleModels.Add(scheduleStyleModel);
                }*/
            /*
            }
            else
            {
                employeesList = eServiceWebContext.Instance.GetEmployeeList();

            }
            */

            foreach (Employee employee in newEmployees)
            {
                ScheduleStyleModel scheduleStyleModel = new ScheduleStyleModel();
                scheduleStyleModel.PopulateFrom(employee);
                scheduleStyleModels.Add(scheduleStyleModel);
            }

            viewScheduleDataModel.ScheduleModels = models;
            viewScheduleDataModel.ScheduleStyleModels = scheduleStyleModels;

             return viewScheduleDataModel;
        }

        public IActionResult GetUnitData()
        {
            ViewScheduleDataModel viewScheduleDataModel = GetUnitViewScheduleDataModel(DateTime.Now, DateTime.Now.AddDays(1));
            return Json(viewScheduleDataModel);
        }
        private ViewScheduleDataModel GetUnitViewScheduleDataModel(DateTime startTime, DateTime endTime)
        {
            ViewScheduleDataModel viewScheduleDataModel = new ViewScheduleDataModel();

            List<ScheduleModel> models = new List<ScheduleModel>();
            Collection<int> servicePoints = this.GetServicePoints();
            List<UnitSchedule> unitSchedules = eServiceWebContext.Instance.GetUnitSchedules(GetServicePoints(),startTime,endTime); 
           
            foreach (var unitSchedule in unitSchedules)
            {
                ScheduleModel model = new ScheduleModel();
                model.PopulateFrom(unitSchedule);
                models.Add(model);
               
            }
      
            List<ScheduleStyleModel> scheduleStyleModels = new List<ScheduleStyleModel>();
            List<TruckUnit> truckUnitsList;
//            if (servicePoints.Count!=0)
//            {
                List<TruckUnit> newtruckUnits = new List<TruckUnit>();
                foreach (var unitSchedule in unitSchedules)
                {
                    TruckUnit truckUnit = new TruckUnit();
                    truckUnit = eServiceWebContext.Instance.GetTruckUnitById(unitSchedule.TruckUnit?.Id ?? 0);
                    if(truckUnit != null) newtruckUnits.Add(truckUnit);
                }

                /*
                List<TruckUnit> truckUnits = eServiceWebContext.Instance.GetTruckUnitList().Where(p=> servicePoints.Contains(p.ServicePoint.Id)).OrderBy(s => s.UnitNumber).ToList();

                List<TruckUnit> finalTruckUnits = truckUnits.Union(newtruckUnits).ToList();

                truckUnitsList = finalTruckUnits.Where((x, i) => finalTruckUnits.FindIndex(z => z.Id == x.Id) == i).OrderBy(s=>s.UnitNumber).ToList();
                */

                /*foreach (var truckUnit in truckUnitsList)
                {
                    ScheduleStyleModel scheduleStyleModel = new ScheduleStyleModel();
                    scheduleStyleModel.PopulateFrom(truckUnit);
                    scheduleStyleModels.Add(scheduleStyleModel);
                }*/
            /*}
            else
            {
                truckUnitsList = eServiceWebContext.Instance.GetTruckUnitList();
            }*/

            foreach (var truckUnit in newtruckUnits)
            {
                ScheduleStyleModel scheduleStyleModel = new ScheduleStyleModel();
                scheduleStyleModel.PopulateFrom(truckUnit);
                scheduleStyleModels.Add(scheduleStyleModel);
            }

            viewScheduleDataModel.ScheduleModels = models;
            viewScheduleDataModel.ScheduleStyleModels = scheduleStyleModels;

            return viewScheduleDataModel;
        }

        public string GetSessionValue()
        {
            RetrievalCondition retrievalEntity=new RetrievalCondition();
            string retrieval= JsonConvert.SerializeObject(retrievalEntity);
            if (this.HttpContext.Session.GetString("ServicePoint") != null)
            {
                retrieval = this.HttpContext.Session.GetString("ServicePoint");
            }
            return retrieval;
        }

        public IActionResult AddWorkSchedule(List<string> parms)
        {
            ScheduleModel model=new ScheduleModel();
            List<Employee> employeeList = eServiceWebContext.Instance.GetEmployeeList().Where(p => GetServicePoints().Contains(p.ServicePoint.Id)).OrderBy(s => s.LastName).ToList();
            this.ViewData["employees"] = this.WorkerItem(parms[1],model);

            List<SelectListItem> employScheduleStatus = Enum.GetValues(typeof(WorkerScheduleType)).Cast<WorkerScheduleType>().Select(v => new SelectListItem {
                Text = v.ToString(),
                Value = ((int)v).ToString()
            }).ToList();
            this.ViewData["employScheduleStatus"] = employScheduleStatus;          
            model.StartTime = this.GetTime(parms[0]);
            model.EndTime = this.GetTime(parms[0]).AddHours(1);
            return PartialView("_AddWorkerSchedule", model);
        }

        private List<SelectListItem> WorkerItem(string workerName, ScheduleModel model)
        {
            string[] workernameArray = workerName.Split(',');
            List<SelectListItem> workerItems= eServiceWebContext.Instance.GetEmployeeList().OrderBy(s => s.LastName).Select(p => new SelectListItem() { Text = $"{p.LastName}, {p.FirstName} ({p.EmployeeNumber})", Value = p.Id.ToString(), Selected = p.LastName == workernameArray[0].ToString() }).ToList();
            model.OwnerId = eServiceWebContext.Instance.GetEmployeeList().Find(p => p.LastName == workernameArray[0] && p.FirstName == workernameArray[1]).Id;
            return workerItems;
        }
        private DateTime GetTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }
        [HttpPost]
        public IActionResult AddWorkSchedule(ScheduleModel model)
        {
            WorkerSchedule workerSchedule=new WorkerSchedule();
            model.PopulateTo(workerSchedule);
            eServiceWebContext.Instance.InsertWorkerSchedule(workerSchedule);
            return RedirectToAction("Index");
        }
       
        public IActionResult AddUnitSchedule(List<string> parms)
        {
            ScheduleModel model = new ScheduleModel();
            List<SelectListItem> unitScheduleStatusItem =Enum.GetValues(typeof(UnitScheduleType)).Cast<UnitScheduleType>().Select(v => new SelectListItem {
                    Text = v.ToString(),
                    Value = ((int)v).ToString()
                }).ToList();
            model.StartTime = this.GetTime(parms[0]);
            model.EndTime = this.GetTime(parms[0]).AddHours(1);
            this.ViewData["truckUnits"] = this.UnitSelectListItems(parms[1],model);
            this.ViewData["unitScheduleStatus"] = unitScheduleStatusItem;
            return PartialView("_AddUnitSchedule", model);
            
        }
        public JsonResult VerifyThirdPartyCrewSchedule(int thirdPartyBulkerCrewId, DateTime startTime, double duration=0,int rigJobId=0)
        {
            DateTime endTime = eServiceWebContext.Instance.GetRigJobAssginCrewEndTime(rigJobId);
            string messageInfo;
            messageInfo = eServiceWebContext.Instance.VerifyThirdPartyBulkerCrewSchedule(thirdPartyBulkerCrewId, startTime, rigJobId==0 ? startTime.AddHours(duration) : endTime == DateTime.MinValue ? startTime.AddHours(8) : endTime);

            return this.Json(messageInfo);
        }
        public JsonResult VerifySanjelCrewSchedule(int crewId, DateTime startTime, double duration = 0, int rigJobId = 0)
        {
            DateTime endTime = eServiceWebContext.Instance.GetRigJobAssginCrewEndTime(rigJobId);

                            
            string messageInfo = eServiceWebContext.Instance.VerifySanjelCrewSchedule(crewId, startTime, rigJobId == 0 ? startTime.AddHours(duration) : endTime==DateTime.MinValue?startTime.AddHours(8):endTime);
            return this.Json(messageInfo);
        }
        public JsonResult VerifyUnitSchedule(int truckUnitId, DateTime startTime, DateTime endTime)
        {
            string messageInfo = eServiceWebContext.Instance.VerifyUnitSchedule(truckUnitId, startTime, endTime);
          
            return this.Json(messageInfo);
        }
        public JsonResult VerifyWorkerSchedule(int workerId, DateTime startTime, DateTime endTime)
        {
            string messageInfo = eServiceWebContext.Instance.VerifyWorkerSchedule(workerId, startTime, endTime);
        
            return this.Json(messageInfo);
        }
        private List<SelectListItem> UnitSelectListItems(string unitNumber, ScheduleModel model)
        {
            List<SelectListItem> unitSelectListItems = eServiceWebContext.Instance.GetTruckUnitList().OrderBy(s => s.UnitNumber)
                    .Select(p => new SelectListItem { Text = p.UnitNumber, Value = p.Id.ToString(),Selected =p.UnitNumber==unitNumber }).ToList();
            model.OwnerId = eServiceWebContext.Instance.GetTruckUnitList().Find(p => p.UnitNumber == unitNumber).Id;
            return unitSelectListItems;
        }

        [HttpPost]
        public IActionResult AddUnitSchedule(ScheduleModel model)
        {
            UnitSchedule unitSchedule=new UnitSchedule();
            model.PopulateTo(unitSchedule);
            eServiceWebContext.Instance.InsertUnitSchedule(unitSchedule);
            return RedirectToAction("Index");
        }

        public IActionResult AddCrewSchedule()
        {
            ScheduleModel model = new ScheduleModel();
            List<SanjelCrew> crewList = eServiceWebContext.Instance.GetAllCrewsInfo(GetServicePoints()).OrderBy(s => s.Name).ToList();
            List<SelectListItem> crewItems =
                crewList.Select(p => new SelectListItem { Text = p.Name, Value = p.Id.ToString() }).ToList();
            this.ViewData["Crews"] = crewItems;
            return PartialView("_AddCrewSchedule", model);
        }

        [HttpPost]
        public IActionResult AddCrewSchedule(ScheduleModel model)
        {
            SanjelCrewSchedule crewSchedule =new SanjelCrewSchedule();
            model.PopulateTo(crewSchedule);
            eServiceWebContext.Instance.InsertCrewSchedule(crewSchedule);
            return RedirectToAction("Index");
        }

        public IActionResult UpdateCrewSchedule(List<string> parms)
        {
            ScheduleModel model = new ScheduleModel();
            SanjelCrewSchedule crewSchedule = eServiceWebContext.Instance.GetCrewScheduleById(Int32.Parse(parms[0]));
            model.PopulateFrom(crewSchedule);
            List<SanjelCrew> crewList = eServiceWebContext.Instance.GetAllCrewsInfo(GetServicePoints());
            List<SelectListItem> crewItems =
                crewList.Select(p => new SelectListItem { Text = p.Name, Value = p.Id.ToString() }).ToList();
            this.ViewData["Crews"] = crewItems;
            return PartialView("_UpdateCrewSchedule", model);
        }

        [HttpPost]
        public IActionResult UpdateCrewSchedule(ScheduleModel model)
        {
            SanjelCrewSchedule crewSchedule =new SanjelCrewSchedule();
            model.PopulateTo(crewSchedule);
            eServiceWebContext.Instance.UpdateCrewSchedule(crewSchedule);
            return RedirectToAction("Index");
        }

        public IActionResult UpdateUnitSchedule(List<string> parms)
        {
            ScheduleModel model = new ScheduleModel();
            UnitSchedule unitSchedule = eServiceWebContext.Instance.GetUnitScheduleById(Int32.Parse(parms[0]));
            model.PopulateFrom(unitSchedule);
            List<TruckUnit> truckUnits = eServiceWebContext.Instance.GetTruckUnitList();
            List<SelectListItem> trSelectListItems =
                truckUnits.Select(p => new SelectListItem { Text = p.UnitNumber, Value = p.Id.ToString() }).ToList();
            this.ViewData["truckUnits"] = trSelectListItems;

            List<SelectListItem> unitScheduleStatusItem =Enum.GetValues(typeof(UnitScheduleType)).Cast<UnitScheduleType>().Select(v => new SelectListItem {
                    Text = v.ToString(),
                    Value = ((int)v).ToString()
                }).ToList();
            this.ViewData["unitScheduleStatus"] = unitScheduleStatusItem;
            return PartialView("_UpdateUnitSchedule", model);
        }

        [HttpPost]
        public IActionResult UpdateUnitSchedule(ScheduleModel model)
        {
            UnitSchedule unitSchedule=new UnitSchedule();
            model.PopulateTo(unitSchedule);
            eServiceWebContext.Instance.UpdateUnitSchedule(unitSchedule);
            return RedirectToAction("Index");
        }

        public IActionResult UpdateWorkSchedule(List<string> parms)
        {
            ScheduleModel model = new ScheduleModel();
            WorkerSchedule workerSchedule = eServiceWebContext.Instance.GetWorkerScheduleById(Int32.Parse(parms[0]));
            model.PopulateFrom(workerSchedule);
            List<Employee> employeeList = eServiceWebContext.Instance.GetEmployeeList();
            List<SelectListItem> employeeItems =
                employeeList.Select(p => new SelectListItem { Text = $"{p.LastName}, {p.FirstName} ({p.EmployeeNumber})", Value = p.Id.ToString() }).ToList();
            this.ViewData["employees"] = employeeItems;

            List<SelectListItem> employScheduleStatus =Enum.GetValues(typeof(WorkerScheduleType)).Cast<WorkerScheduleType>().Select(v => new SelectListItem {
                Text = v.ToString(),
                Value = ((int)v).ToString()
            }).ToList();
            this.ViewData["employScheduleStatus"] = employScheduleStatus;
            return PartialView("_UpdateWorkerSchedule", model);
        }

        [HttpPost]
        public IActionResult UpdateWorkSchedule(ScheduleModel model)
        {
            WorkerSchedule workerSchedule=new WorkerSchedule();
            model.PopulateTo(workerSchedule);
            eServiceWebContext.Instance.UpdateWorkerSchedule(workerSchedule);
            return RedirectToAction("Index");
        }

        public IActionResult CancelWorkSchedule(List<string> parms)
        {
            eServiceWebContext.Instance.DeleteWorkSchedule(Int32.Parse(parms[0]));
            return Json(true);
        }

        public IActionResult CancelUnitSchedule(List<string> parms)
        {
            eServiceWebContext.Instance.DeleteUnitSchedule(Int32.Parse(parms[0]));
            return Json(true);
        }

        public IActionResult CancelCrewSchedule(List<string> parms)
        {
            eServiceWebContext.Instance.DeleteCrewSchedule(Int32.Parse(parms[0]));
            return Json(true);
        }


        public IActionResult DetailCrewSchedule(List<string> parms) {
            ScheduleModel model = new ScheduleModel();
            SanjelCrewSchedule crewSchedule = eServiceWebContext.Instance.GetCrewScheduleById(Int32.Parse(parms[0]));
            model.PopulateFrom(crewSchedule);
            RigJobSanjelCrewSection rigJobCrewSection = eServiceWebContext.Instance.GetRigJobCrewSectionById(crewSchedule.RigJobSanjelCrewSection.Id);
            model.ScheduleTypeName = eServiceWebContext.Instance.GetCrewById(rigJobCrewSection.SanjelCrew.Id)?.Name;
            return PartialView("_DetailCrewSchedule",model);
        }
        public IActionResult DetailUnitSchedule(List<string> parms)
        {
            ScheduleModel model = new ScheduleModel();
            UnitSchedule unitSchedule = eServiceWebContext.Instance.GetUnitScheduleById(Int32.Parse(parms[0]));
            model.PopulateFrom(unitSchedule);
            model.ScheduleTypeName = eServiceWebContext.Instance.GetTruckUnitList().Find(s => s.Id == unitSchedule.TruckUnit.Id)?.UnitNumber;
            string scheduleCategory = unitSchedule.Type.ToString();
            model.StatusName = scheduleCategory ?? "working";
            return PartialView("_DetailUnitSchedule",model);
        }
        public IActionResult DetailWorkerSchedule(List<string> parms)
        {
            ScheduleModel model = new ScheduleModel();
            WorkerSchedule workerSchedule = eServiceWebContext.Instance.GetWorkerScheduleById(Int32.Parse(parms[0]));
            model.PopulateFrom(workerSchedule);
            Employee employee = eServiceWebContext.Instance.GetEmployeeById(workerSchedule.Worker.Id);
            string employeeName= $"{employee.LastName}, {employee.FirstName}";
            model.ScheduleTypeName = employeeName;
            string scheduleCategory = workerSchedule.ToString();
            model.StatusName = scheduleCategory??"working";
            return PartialView("_DetailWorkerSchedule",model);
        }
    }
}