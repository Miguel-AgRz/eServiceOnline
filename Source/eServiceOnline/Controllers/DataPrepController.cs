using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using eServiceOnline.BusinessProcess;
using eServiceOnline.Data;
using eServiceOnline.Gateway;
using MetaShare.Common.Core.Services;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Sanjel.Common.BusinessEntities.LandSurveySystems;
using Sesi.SanjelData.Entities.BusinessEntities.Lab;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Crew;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ResourceSchedule;
using Sesi.SanjelData.Entities.BusinessEntities.WellSite;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment;
using Sesi.SanjelData.Entities.Common.BusinessEntities.HumanResources;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Operation;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Organization;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Products;
using Sesi.SanjelData.Entities.Common.BusinessEntities.WellSite;
using Sesi.SanjelData.Entities.Common.Model.Scheduling;
using Sesi.SanjelData.Services.Interfaces.BusinessEntities.Lab;
using Sesi.SanjelData.Services.Interfaces.BusinessEntities.Operation.Crew;
using Sesi.SanjelData.Services.Interfaces.BusinessEntities.Operation.Dispatch;
using Sesi.SanjelData.Services.Interfaces.BusinessEntities.Operation.Schedule.ResourceSchedule;
using Sesi.SanjelData.Services.Interfaces.Common.BusinessEntities.Operation;
using Sesi.SanjelData.Services.Interfaces.Common.BusinessEntities.Organization;
using Sesi.SanjelData.Services.Interfaces.Common.BusinessEntities.Products;
using Sesi.SanjelData.Services.Interfaces.Common.Model.Scheduling;
using IEmployeeService = Sesi.SanjelData.Services.Interfaces.Common.BusinessEntities.HumanResources.IEmployeeService;
using IRigService = Sesi.SanjelData.Services.Interfaces.Common.BusinessEntities.WellSite.IRigService;
using ITruckUnitService = Sesi.SanjelData.Services.Interfaces.Common.BusinessEntities.Equipment.ITruckUnitService;
using ISanjelCrewService = Sesi.SanjelData.Services.Interfaces.BusinessEntities.Operation.Crew.ISanjelCrewService;
using eServiceOnline.Models.CrewBoard;
using eServiceOnline.Models.WorkerBoard;
using eServiceOnline.Models.UnitBoard;
using Sesi.SanjelData.Entities.BusinessEntities.Sales;
using Sesi.SanjelData.Entities.Common.BusinessEntities.HSE;
using Sesi.SanjelData.Services.Interfaces.Common.BusinessEntities.HSE;
using Sesi.SanjelData.Services.Interfaces.Common.Entities.General;

namespace eServiceOnline.Controllers
{
    public class DataPrepController : eServiceOnlineController
    {
        private readonly IeServiceWebContext _context;
        private IMemoryCache _memoryCache;


        private Dictionary<string, string> specialtyProducts = new Dictionary<string, string>()
        {
            {"ACW-4","71,66,67,85"},
            {"ASM-3","72,70,88"},
            {"ASM-5","70,72,88"},
            {"ASM-6","70,72,88,65,65"},
            {"Bulk Barite","70,72,88,85,67"},
            {"CDM-10","65,65,85,67,71,66"},
            {"CDM-4","85,72,70,88"},
            {"CFL-12","72,70,88"},
            {"CFL-4","72,70,88"},
            {"CFR","67,85,65,65,72,70,88,71,66"},
            {"CFR-2","72,70,88"},
            {"CFR-5","72,70,88,65,65"},
            {"D139 Foam Stabilizer","67,85,65,65,72,70,88,71,66"},
            {"D270 Fulcrum*","67,85,65,65,72,70,88"},
            {"DF-1","67,85,65,65,72,70,88"},
            {"FA-3","67,85,65,65,72,70,88,71,66"},
            {"FFA-2","67,85,65,65,72,70,88,71,66"},
            {"FS-1","67,85,65,65,72,70,88,71,66"},
            {"GCA-2","65,65,72,70,88"},
            {"Hematite","67,85,65,65,72,70,88,71,66"},
            {"HTR-3","67,85,65,65,72,70,88,71,66"},
            {"HTR-3A","67,85,65,65,72,70,88,71,66"},
            {"IPG-4","67,85,65,65,72,70,88,71,66"},
            {"LCC-9","67,85,71,66,70,72,88"},
            {"Liquid Latex","67,85,65,65,72,70,88,71,66"},
            {"NaCl","67,85,65,65,72,70,88,71,66"},
            {"SA-3P","67,85,65,65,72,70,88,71,66"},
            {"SAPP","65,65,70,72,88"},
            {"SCC-2L","67,85,65,65,72,70,88,71,66"},
            {"WG-1L","67,85,65,65,72,70,88,71,66"},
            {"100 Mesh","71,66,67,85"},
            {"FFA-1 Formation Filming Agent","67,85,65,65,72,70,88,71,66"},
            {"KCl","67,85,65,65"},
            {"MT-1 Mud Thinner","65,65,70,72,88"},
            {"SPC-II","67,85,72,70,88"},
        };

        private Dictionary<int, int> thirdPartyCrews = new Dictionary<int, int>()
        {
            { 38, 72 },
            { 42, 61 },
            { 67, 72 },
            { 71, 72 },
            { 73, 71 },
            { 70, 71 },
            { 87, 71 },
            { 96, 67 },
            { 114, 71 },
            { 116, 71 },
            { 111, 67 },
            { 115, 71 },
            { 120, 71 },
            { 123, 67 },
            { 39, 72 },
            { 126, 67 },
            { 75, 71 },
            { 98, 71 },
            { 125, 67 },
            { 107, 71 },
            { 128, 71 },
            { 119, 71 },
            { 110, 67 },
            { 90, 67 },
            { 25, 72 },
            { 89, 67 },
            { 24, 72 }
        };

        private Dictionary<string, int> employeeRotations = new Dictionary<string, int>()
        {

            { "Adam Wang", 4 },

        };

        public DataPrepController(IMemoryCache memoryCache)
        {
            this._context = eServiceWebContext.Instance;
            _memoryCache = memoryCache;

        }

        //Sample: http://localhost:44703/DataPrep/PopulateBulkerCrewStatus
        public ActionResult PopulateBulkerCrewStatus()
        {
            string message = "Succeed";
            bool result = true;

            ISanjelCrewService sanjelCrewService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ISanjelCrewService>();
            if (sanjelCrewService == null) throw new Exception("ISanjelCrewService must be registered in service factory");

            IBulkerCrewLogService bulkerCrewLogService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IBulkerCrewLogService>();
            if (bulkerCrewLogService == null) throw new Exception("IBulkerCrewLogService must be registered in service factory");

            IThirdPartyBulkerCrewService thirdPartyBulkerCrewService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IThirdPartyBulkerCrewService>();
            if (thirdPartyBulkerCrewService == null) throw new Exception("IThirdPartyBulkerCrewService must be registered in service factory");


            var crewList = sanjelCrewService.SelectAll();
            var crewLogList = bulkerCrewLogService.SelectAll();
            var thirdPartycrewList = thirdPartyBulkerCrewService.SelectAll();
            foreach (var crew in crewList)
            {
                var log = crewLogList.Find(p => p.SanjelCrew.Id == crew.Id);
                if (log == null)
                {
                    log = new BulkerCrewLog()
                    {
                        SanjelCrew = crew,
                        CrewStatus = BulkerCrewStatus.OffDuty,
                        LastUpdatedTime = DateTime.Now,
                        EnrouteTime = DateTime.MinValue
                    };

                    bulkerCrewLogService.Insert(log);
                }
            }

            foreach (var crew in thirdPartycrewList)
            {
                var log = crewLogList.Find(p => p.ThirdPartyBulkerCrew.Id == crew.Id);
                if (log == null)
                {
                    log = new BulkerCrewLog()
                    {
                        ThirdPartyBulkerCrew = crew,
                        CrewStatus = BulkerCrewStatus.OffDuty,
                        LastUpdatedTime = DateTime.Now,
                        EnrouteTime = DateTime.MinValue
                    };

                    bulkerCrewLogService.Insert(log);
                }
            }

            return new JsonResult(new { result, message });

        }

        //Sample: http://localhost:44703/DataPrep/PopulateSpeciatlyProducts
        public ActionResult PopulateSpeciatlyProducts()
        {
            string message = "Succeed";
            bool result = true;



            IBlendChemicalService blendChemicalService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IBlendChemicalService>();
            if (blendChemicalService == null) throw new Exception("IBlendChemicalService must be registered in service factory");
            ISpecialtyProductService specialtyProductService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ISpecialtyProductService>();
            if (specialtyProductService == null) throw new Exception("ISpecialtyProductService must be registered in service factory");
            IServicePointService servicePointService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IServicePointService>();
            if (servicePointService == null) throw new Exception("ISpecialtyProductService must be registered in service factory");

            List<ServicePoint> servicePoints = servicePointService.SelectAll();
            List<BlendChemical> blendChemicals = blendChemicalService.SelectAll();

            foreach (var specialtyProduct in specialtyProducts)
            {
                var blendChemical = blendChemicals.Find((p => p.Name == specialtyProduct.Key));
                if (blendChemical != null)
                {
                    string[] servicePointIds = specialtyProduct.Value.Split(',');
                    foreach (var servicePointId in servicePointIds)
                    {
                        var servicePoint = servicePoints.Find(p => p.Id == Int32.Parse(servicePointId));
                        if (servicePoint != null)
                        {
                            SpecialtyProduct sp = new SpecialtyProduct()
                            {
                                BlendChemical = blendChemical,
                                ServicePoint = servicePoint
                            };
                            specialtyProductService.Insert(sp);
                        }
                    }
                }
            }

            return new JsonResult(new { result, message });
        }

        //Use this api for initial schedule population
        //Sample: http://localhost:44703/DataPrep/PopulateRotationForAllFieldEmployee
        public ActionResult PopulateRotationForAllFieldEmployee()
        {
            string message = "Succeed";
            bool result = true;

            DateTime startDate = new DateTime(2023, 6, 12);
            DateTime endDate = startDate.AddDays(120);


            IEmployeeService employeeService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IEmployeeService>();
            if (employeeService == null) throw new Exception("employeeService must be registered in service factory");
            IWorkerScheduleService workerScheduleService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IWorkerScheduleService>();
            if (workerScheduleService == null) throw new Exception("workerScheduleService must be registered in service factory");


            List<Employee> employees = employeeService.SelectAll();
//            employeeService.SelectAll().FindAll(p => p.BonusPosition != null && p.BonusPosition.Id != 0);

            foreach (var employee in employees)
            {
                if (employeeRotations.ContainsKey(employee.Name))
                {
                    workerScheduleService.UpdateWorkRotationSchedule(employee.Id, employeeRotations[employee.Name],
                        startDate, endDate);
                }
            }

            
            return new JsonResult(new { result, message });
        }

        /*
        public int UpdateWorkRotationSchedule(int employeeId, int rotationTemplateId, DateTime startDate, DateTime endDate)
        {
            IWorkerScheduleService workerScheduleService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IWorkerScheduleService>();
            if (workerScheduleService == null) throw new Exception("workerScheduleService must be registered in service factory");
            DateTime queryEndDateTime = endDate.AddDays(1);
            List<WorkerSchedule> futureRotationSchedules = workerScheduleService.SelectBy(null, schedule=>schedule.Worker.Id == employeeId && (schedule.Type == WorkerScheduleType.OffShift || schedule.Type == WorkerScheduleType.OnShift) && schedule.StartTime < queryEndDateTime &&  schedule.EndTime>startDate).OrderBy(p=>p.StartTime).ToList();

            LinkedList<WorkerSchedule> futureSchedules = new LinkedList<WorkerSchedule>(futureRotationSchedules);
            if (futureSchedules.Count > 0)
            {
                if (futureSchedules.First.Value.StartTime < startDate)
                {
                    var oldSchedule = new WorkerSchedule();
                    oldSchedule.Name = futureSchedules.First.Value.Name;
                    oldSchedule.Description = futureSchedules.First.Value.Description;
                    oldSchedule.Type = futureSchedules.First.Value.Type;
                    oldSchedule.StartTime = futureSchedules.First.Value.StartTime;
                    oldSchedule.EndTime = startDate.AddSeconds(-1);
                    oldSchedule.Rotation = futureSchedules.First.Value.Rotation;
                    oldSchedule.Worker = futureSchedules.First.Value.Worker;
                    oldSchedule.WorkingServicePoint = futureSchedules.First.Value.WorkingServicePoint;
                    workerScheduleService.Insert(oldSchedule);
                    futureSchedules.First.Value.StartTime = startDate;
                }

                if (futureSchedules.Last.Value.EndTime > queryEndDateTime.AddSeconds(-1))
                {
                    var oldSchedule = new WorkerSchedule();
                    oldSchedule.Name = futureSchedules.Last.Value.Name;
                    oldSchedule.Description = futureSchedules.Last.Value.Description;
                    oldSchedule.Type = futureSchedules.Last.Value.Type;
                    oldSchedule.StartTime = queryEndDateTime;
                    oldSchedule.EndTime = futureSchedules.Last.Value.EndTime;
                    oldSchedule.Rotation = futureSchedules.Last.Value.Rotation;
                    oldSchedule.Worker = futureSchedules.Last.Value.Worker;
                    oldSchedule.WorkingServicePoint = futureSchedules.Last.Value.WorkingServicePoint;
                    workerScheduleService.Insert(oldSchedule);
                    futureSchedules.Last.Value.EndTime = queryEndDateTime.AddSeconds(-1);
                }
            }

            LinkedList<WorkerSchedule> rotationSchedules =
                BuildRotationSchedules(employeeId, rotationTemplateId, startDate, queryEndDateTime.AddSeconds(-1));
            LinkedListNode<WorkerSchedule> rotationSchedule = rotationSchedules.First;
            LinkedListNode<WorkerSchedule> futureSchedule = futureSchedules.First;

            while (true)
            {
                if (rotationSchedule != null && futureSchedule != null)
                {
                    futureSchedule.Value.Name = rotationSchedule.Value.Name;
                    futureSchedule.Value.Description = rotationSchedule.Value.Description;
                    futureSchedule.Value.Type = rotationSchedule.Value.Type;
                    futureSchedule.Value.StartTime = rotationSchedule.Value.StartTime;
                    futureSchedule.Value.EndTime = rotationSchedule.Value.EndTime;
                    futureSchedule.Value.Rotation = rotationSchedule.Value.Rotation;
                    futureSchedule.Value.Worker = rotationSchedule.Value.Worker;
                    futureSchedule.Value.WorkingServicePoint = rotationSchedule.Value.WorkingServicePoint;
                    workerScheduleService.Update(futureSchedule.Value);
                    rotationSchedule = rotationSchedule.Next;
                    futureSchedule = futureSchedule.Next;
                    continue;
                }
                break;
            }

            if (futureSchedule == null)
            {
                if (rotationSchedule != null)
                {
                    while (true)
                    {
                        workerScheduleService.Insert(rotationSchedule.Value);
                        rotationSchedule = rotationSchedule.Next;
                        if (rotationSchedule == null) break;
                    }
                }
            }

            if (rotationSchedule == null)
            {
                if (futureSchedule != null)
                {
                    while (true)
                    {
                        workerScheduleService.Delete(futureSchedule.Value);
                        futureSchedule = futureSchedule.Next;
                        if (futureSchedule == null) break;
                    }
                }
            }

            return 1;
        }

        private LinkedList<WorkerSchedule> BuildRotationSchedules(int employeeId, int rotationId, DateTime startDateTime, DateTime endDateTime)
        {
            IRotationTemplateService rotationTemplateService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRotationTemplateService>();
            RotationTemplate rotationTemplate = rotationTemplateService.SelectById(new RotationTemplate(){Id =rotationId});

            IEmployeeService employeeService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IEmployeeService>();
            Employee employee = employeeService.SelectById(new Employee(){Id =employeeId});

            DateTime absoluteDayOne = new DateTime(2022,05,01);
            LinkedList<WorkerSchedule> rotationSchedules = new LinkedList<WorkerSchedule>();
            DateTime rotationWorkDayOne = absoluteDayOne.AddDays(rotationTemplate.StartDay + 7*(rotationTemplate.RotationOrder - 1));
            int totalCycleDays = rotationTemplate.OffDays + rotationTemplate.WorkDays;

            for (int j = 0; ; j++)
            {
                DateTime latestCycleStartDate = rotationWorkDayOne.AddDays(j * totalCycleDays);
                if(latestCycleStartDate.AddDays(totalCycleDays) <= startDateTime) continue;
                if (latestCycleStartDate > endDateTime) break;

                if (latestCycleStartDate.AddDays(rotationTemplate.WorkDays) < startDateTime)
                {
                    //Start day falls in Off Shift
                    DateTime offShiftStartTime = startDateTime;
                    DateTime offShiftEndTime = latestCycleStartDate.AddDays(totalCycleDays) > endDateTime? endDateTime: latestCycleStartDate.AddDays(totalCycleDays).AddSeconds(-1);
                    WorkerSchedule offShiftSchedule = new WorkerSchedule()
                    {
                        Name = rotationTemplate.Name, Description = rotationTemplate.Description, Worker = employee,
                        WorkingServicePoint = employee.ServicePoint, Type = WorkerScheduleType.OffShift,
                        StartTime = offShiftStartTime, EndTime = offShiftEndTime, Rotation = rotationTemplate
                    };
                    rotationSchedules.AddLast(offShiftSchedule);
                }
                else
                {
                    //Start day falls in On Shift
                    DateTime onShiftStartTime = startDateTime>latestCycleStartDate?startDateTime:latestCycleStartDate;
                    DateTime onShiftEndTime = latestCycleStartDate.AddDays(rotationTemplate.WorkDays).AddSeconds(-1)<endDateTime?latestCycleStartDate.AddDays(rotationTemplate.WorkDays).AddSeconds(-1):endDateTime;
                    WorkerSchedule onShiftSchedule = new WorkerSchedule()
                    {
                        Name = rotationTemplate.Name, Description = rotationTemplate.Description, Worker = employee,
                        WorkingServicePoint = employee.ServicePoint, Type = WorkerScheduleType.OnShift,
                        StartTime = onShiftStartTime, EndTime = onShiftEndTime, Rotation = rotationTemplate
                    };
                    rotationSchedules.AddLast(onShiftSchedule);

                    if (latestCycleStartDate.AddDays(rotationTemplate.WorkDays).AddSeconds(-1) < endDateTime)
                    {

                        DateTime offShiftStartTime = latestCycleStartDate.AddDays(rotationTemplate.WorkDays);
                        DateTime offShiftEndTime = latestCycleStartDate.AddDays(totalCycleDays).AddSeconds(-1)<endDateTime?latestCycleStartDate.AddDays(totalCycleDays).AddSeconds(-1):endDateTime;
                        WorkerSchedule offShiftSchedule = new WorkerSchedule()
                        {
                            Name = rotationTemplate.Name, Description = rotationTemplate.Description, Worker = employee,
                            WorkingServicePoint = employee.ServicePoint, Type = WorkerScheduleType.OffShift,
                            StartTime = offShiftStartTime, EndTime = offShiftEndTime, Rotation = rotationTemplate
                        };
                        rotationSchedules.AddLast(offShiftSchedule);
                    }

                }
            }

            return rotationSchedules;

        }
        */


        //Use this api for initial schedule population
        //Sample: http://localhost:44703/DataPrep/PopulateAvailableForAllUnits
        public ActionResult PopulateAvailableForAllUnits()
        {
            string message = "Succeed";
            bool result = true;

            ITruckUnitService truckUnitService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ITruckUnitService>();
            if (truckUnitService == null) throw new Exception("truckUnitService must be registered in service factory");
            IUnitScheduleService unitScheduleService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IUnitScheduleService>();
            if (unitScheduleService == null) throw new Exception("unitScheduleService must be registered in service factory");


            List<TruckUnit> truckUnits = truckUnitService.SelectAll();
            foreach (var truckUnit in truckUnits)
            {
                UnitSchedule unitSchedule = new UnitSchedule() { TruckUnit = truckUnit,WorkingServicePoint = truckUnit.ServicePoint, Type = UnitScheduleType.Planned, StartTime = DateTime.Today, EndTime = DateTime.MaxValue };
                unitScheduleService.Insert(unitSchedule);
            }

            return new JsonResult(new { result, message });
        }

        //Sample: http://localhost:44703/DataPrep/TestGetAllCrews
        public ActionResult TestGetAllCrews()
        {            
            string message = "Succeed";
            bool result = true;

            ISanjelCrewService sanjelCrewService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ISanjelCrewService>();
            if (sanjelCrewService == null) throw new Exception("sanjelCrewService must be registered in service factory");

            List<SanjelCrew> sanjelCrews = sanjelCrewService.SelectCrewList();
            Collection<int> jobLifeStatuses = new Collection<int>() {2,3,4,5,7};

            IRigJobService rigjobService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigJobService>();
            if (rigjobService == null) throw new Exception("sanjelCrewService must be registered in service factory");

            List<RigJob> rigJob = rigjobService.SelectBy(new RigJob(), rigjob=>jobLifeStatuses.Contains((int)rigjob.JobLifeStatus));

            
            return new JsonResult(new { result, message });

        }

        //Sample: http://localhost:44703/DataPrep/TestGetSanjeCrewSchedule
        public ActionResult TestGetSanjeCrewSchedule()
        {
            string message = "Succeed";
            bool result = true;
            DateTime dd = DateTime.Now.AddDays(-45);
            ISanjelCrewScheduleService sanjelCrewScheduleService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ISanjelCrewScheduleService>();
            if (sanjelCrewScheduleService == null) throw new Exception("sanjelCrewService must be registered in service factory");

            List<SanjelCrewSchedule> sanjelCrewSchedules = sanjelCrewScheduleService.SelectBy(new SanjelCrewSchedule()
                { SanjelCrew = new SanjelCrew() { Id = 3294 } }, sanjelCrewSchedule=>sanjelCrewSchedule.StartTime>dd);

            List<int> crewIds = new List<int>() { 3294, 3423 };
             sanjelCrewSchedules = sanjelCrewScheduleService.SelectBy(new SanjelCrewSchedule()
                { }, sanjelCrewSchedule=>crewIds.Contains(sanjelCrewSchedule.SanjelCrew.Id) && sanjelCrewSchedule.StartTime>dd);
            return new JsonResult(new { result, message });
        }
        //Sample: http://localhost:44703/DataPrep/TestGetActiveCrews
        public ActionResult TestGetActiveCrews()
        {            
            string message = "Succeed";
            bool result = true;

            ISanjelCrewService sanjelCrewService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ISanjelCrewService>();
            if (sanjelCrewService == null) throw new Exception("sanjelCrewService must be registered in service factory");

            List<SanjelCrew> sanjelCrews = sanjelCrewService.SelectActiveCrewList();
            Collection<int> jobLifeStatuses = new Collection<int>() {2,3,4,5,7};

            IRigJobService rigjobService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigJobService>();
            if (rigjobService == null) throw new Exception("sanjelCrewService must be registered in service factory");

            List<RigJob> rigJobs = rigjobService.SelectBy(new RigJob(), rigjob=>jobLifeStatuses.Contains((int)rigjob.JobLifeStatus));

            IRigJobSanjelCrewSectionService rigJobSanjelCrewSectionService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigJobSanjelCrewSectionService>();
            if (rigjobService == null) throw new Exception("rigJobSanjelCrewSectionService must be registered in service factory");

            List<RigJobSanjelCrewSection> rigJobSanjelCrewSections = rigJobSanjelCrewSectionService.SelectRigJobSanjelCrewSectionList();

            List<RigJobSanjelCrewSection> assignedCrewSectionList =
                rigJobSanjelCrewSections.Where(p => p.RigJob.Id == 70000).ToList();
//                rigJobSanjelCrewSectionService.SelectRigJobSanjelCrewSectionListByRigJob(rigJob=>rigJob.Id == 70000);

            return new JsonResult(new { result, message });

        }

        public ActionResult PopulateTestType()
        {
            string message = "Succeed";
            bool result = true;

            ITestRequestService testRequestService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ITestRequestService>();

            List<TestRequest> testRequests = testRequestService.SelectByJoin(p=>p.Id > 0, p=>p.TestIterations != null);
            foreach (var testRequest in testRequests)
            {
                var testTypeDescription = string.Empty;
                List<string> typetest = new List<string>();

                foreach (var testRequestTestIteration in testRequest.TestIterations)
                {
                    List<LabTest> list =
                        JsonConvert.DeserializeObject<List<LabTest>>(testRequestTestIteration
                            .LabTestListJson);

                    foreach (LabTest items in list)
                    {
                        switch (items.Type.ToString())
                        {
                            case "ThickeningTime":
                                typetest.Add("TT");
                                break;
                            case "FluidLoss":
                                typetest.Add("FL");
                                break;
                            case "FreeWater":
                                typetest.Add("FW");
                                break;
                            case "Rheology":
                                typetest.Add("Rheo");
                                break;
                            case "CompressiveStrength":
                                typetest.Add("CS");
                                break;
                            case "StaticGelStrengthAnalyzers":
                                typetest.Add("SGSA");
                                break;
                            case "CriticalInterval":
                                typetest.Add("CI");
                                break;
                            case "WaterAnalysis":
                                typetest.Add("WA");
                                break;
                            case "SpecificGravity":
                                typetest.Add("SG");
                                break;
                        }

                    }

                }
                testRequest.Description = String.Join(" ,", typetest.Distinct());

                testRequestService.Update(testRequest);
            }


            return new JsonResult(new { result, message });
        }

        ////Sample: http://localhost:44703/DataPrep/TestFormatAllCrews
        public ActionResult TestFormatAllCrews()
        {

            string message = "Succeed";
            bool result = true;

            ISanjelCrewService sanjelCrewService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ISanjelCrewService>();
            if (sanjelCrewService == null) throw new Exception("sanjelCrewService must be registered in service factory");


            List<SanjelCrew> sanjelCrews = sanjelCrewService.SelectCrewList();

            foreach (SanjelCrew crew in sanjelCrews)
            {
                if (crew.Type.Name != null)
                {
                    if (crew.Type.Name.Contains("Bulker Crew"))
                    {
                        crew.Name = CrewBoardProcess.BuildCrewName(crew);

                        this._context.UpdateCrew(crew);
                    }
                }
            }

            return new JsonResult(new { result, message });

        }

        ////Sample: http://localhost:44703/DataPrep/FixRecipeUnitDescription
        public ActionResult FixRecipeUnitDescription()
        {

            string message = "Succeed";
            bool result = true;

            IBlendRecipeService blendRecipeService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IBlendRecipeService>();
            if (blendRecipeService == null) throw new Exception("blendChemicalSectionService must be registered in service factory");
            IBlendAdditiveMeasureUnitService blendAdditiveMeasureUnitService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IBlendAdditiveMeasureUnitService>();
            if (blendAdditiveMeasureUnitService == null) throw new Exception("blendAdditiveMeasureUnitService must be registered in service factory");


            List<BlendAdditiveMeasureUnit> units = blendAdditiveMeasureUnitService.SelectAll();
            List<BlendRecipe> blendChemicalSections = blendRecipeService.SelectAll();

            foreach (BlendRecipe blendRecipe in blendChemicalSections)
            {
	            blendRecipe.Unit = units.FirstOrDefault(p => p.Id == blendRecipe.Unit.Id);
	            blendRecipeService.Update(blendRecipe);
            }

            /*
            IBlendChemicalSectionService blendChemicalSectionService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IBlendChemicalSectionService>();
            if (blendChemicalSectionService == null) throw new Exception("blendChemicalSectionService must be registered in service factory");
            IBlendAdditiveMeasureUnitService blendAdditiveMeasureUnitService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IBlendAdditiveMeasureUnitService>();
            if (blendAdditiveMeasureUnitService == null) throw new Exception("blendAdditiveMeasureUnitService must be registered in service factory");


            List<BlendAdditiveMeasureUnit> units = blendAdditiveMeasureUnitService.SelectAll();
            List<BlendChemicalSection> blendChemicalSections = blendChemicalSectionService.SelectAll();

            foreach (BlendChemicalSection blendChemicalSection in blendChemicalSections)
            {
	            if (string.IsNullOrEmpty(blendChemicalSection.Unit.Description) ||
	                blendChemicalSection.Unit.Description.Equals(blendChemicalSection.Unit.Name))
	            {
		            blendChemicalSection.Unit = units.FirstOrDefault(p => p.Id == blendChemicalSection.Unit.Id);
		            blendChemicalSectionService.Update(blendChemicalSection);
	            }
            }
            */
            return new JsonResult(new { result, message });

        }

        ////Sample: http://localhost:44703/DataPrep/SetThirdPartyCrewServicePoint
        public ActionResult SetThirdPartyCrewServicePoint()
        {

            string message = "Succeed";
            bool result = true;

            IThirdPartyBulkerCrewService thirdPartyBulkerCrewService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IThirdPartyBulkerCrewService>();
            if (thirdPartyBulkerCrewService == null) throw new Exception("thirdPartyBulkerCrewService must be registered in service factory");

            IServicePointService servicePointService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IServicePointService>();
            if (servicePointService == null) throw new Exception("servicePointService must be registered in service factory");

            List<ThirdPartyBulkerCrew> thirdPartyBulkerCrews = thirdPartyBulkerCrewService.SelectAll();

            foreach (ThirdPartyBulkerCrew crew in thirdPartyBulkerCrews)
            {
                crew.ServicePoint =
                    servicePointService.SelectById(new ServicePoint() { Id = thirdPartyCrews[crew.Id] });
                thirdPartyBulkerCrewService.Update(crew);

            }

            return new JsonResult(new { result, message });

        }

        ////Sample: http://localhost:44703/DataPrep/TestUidService
        public ActionResult TestUidService()
        {
            string uid = string.Empty;

           IEntityUidService entitytUidService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IEntityUidService>();
           if (entitytUidService == null) throw new Exception("thirdPartyBulkerCrewService must be registered in service factory");

           uid = entitytUidService.GetUid("Program");
           uid += entitytUidService.GetUid(nameof(SalesProject));

            return new JsonResult(new { uid });

        }

        ////Sample: http://localhost:44703/DataPrep/PopulatePrimaryIncidentIncidentType
        public ActionResult PopulatePrimaryIncidentIncidentType()
        {
            Dictionary<int, int[]> primaryIncidentIncidentTypeDictionary = new Dictionary<int, int[]>()
            {
                { 2, new int[]{30,31,60,34,35,6,36,26,37,38,64,42,46,49,65,66,51,52,53,67,54,55,68,59,71,70,72,73}},
                { 3, new int[]{60,32,33,34,35,39,61,62,43,44,45,47,49,66}},
                { 4, new int[]{63,74,75,76,77}}
            };

            IPrimaryIncidentService primaryIncidentService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IPrimaryIncidentService>();
            IIncidentTypeService incidentTypeService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IIncidentTypeService>();
            IPrimaryIncidentIncidentTypeService primaryIncidentIncidentTypeService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IPrimaryIncidentIncidentTypeService>();

            foreach (var data in primaryIncidentIncidentTypeDictionary)
            {
	            PrimaryIncident primaryIncident = primaryIncidentService.SelectById(new PrimaryIncident() { Id = data.Key });

	            foreach (var incidentTypeId in data.Value)
	            {
		            IncidentType incidentType =
			            incidentTypeService.SelectById(new IncidentType() { Id = incidentTypeId });
		            PrimaryIncidentIncidentType primaryIncidentIncidentType =
			            new PrimaryIncidentIncidentType()
				            { PrimaryIncident = primaryIncident, IncidentType = incidentType };
		            primaryIncidentIncidentTypeService.Insert(primaryIncidentIncidentType);
	            }

            }

            return new JsonResult(new { });

        }

        ////Sample: http://localhost:44703/DataPrep/PopulatePrimaryIncidentPerformanceMetric
        public ActionResult PopulatePrimaryIncidentPerformanceMetric()
        {
            Dictionary<int, int[]> primaryIncidentPerformanceMetricDictionary = new Dictionary<int, int[]>()
            {
                { 2, new int[]{81,82,83,84,58,59,86,90,89,10,41,93,61,94,63,66,38,96,98,99,21,69,100,101,70,102,103,27,54,29,104,105,106,75,107,55,78,79,108,111,112,113,114,115, 118}},
                { 3, new int[]{85,92,69,77}},
                { 4, new int[]{83,84,59,86,87,88,91,10,63,97,54,29,105,55,109,110,111,116,117}}
            };
            IPrimaryIncidentService primaryIncidentService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IPrimaryIncidentService>();
            IPerformanceMetricService performanceMetricService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IPerformanceMetricService>();
            IPrimaryIncidentPerformanceMetricService primaryIncidentActualSeverityService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IPrimaryIncidentPerformanceMetricService>();

            foreach (var data in primaryIncidentPerformanceMetricDictionary)
            {
                PrimaryIncident primaryIncident = primaryIncidentService.SelectById(new PrimaryIncident() { Id = data.Key });

                foreach (var actualSeverityId in data.Value)
                {
                    PerformanceMetric performanceMetric =
                        performanceMetricService.SelectById(new PerformanceMetric() { Id = actualSeverityId });
                    PrimaryIncidentPerformanceMetric primaryIncidentPerformanceMetric =
	                    new PrimaryIncidentPerformanceMetric()
		                    { PrimaryIncident = primaryIncident, PerformanceMetric = performanceMetric };
                    primaryIncidentActualSeverityService.Insert(primaryIncidentPerformanceMetric);
                }

            }

            return new JsonResult(new { });
        }

        ////Sample: http://localhost:44703/DataPrep/PopulatePrimaryIncidentActualSeverity
        public ActionResult PopulatePrimaryIncidentActualSeverity()
        {
            Dictionary<int, int[]> primaryIncidentActualSeverityDictionary = new Dictionary<int, int[]>()
            {
                { 2, new int[]{21,3,23,25,24,28,30,29,33,35,14,34,17,39,38,37,42,45,49,51,53,55}},
                { 3, new int[]{22,3,25,26,30,35,39,38,42,43,44,45,46,47,48,49,50,51,52,53,54,55}},
                { 4, new int[]{21,3,23,28,33,17}}
            };
            IPrimaryIncidentService primaryIncidentService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IPrimaryIncidentService>();
            ISeverityMatrixtypeService severityMatrixtypeService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ISeverityMatrixtypeService>();
            IPrimaryIncidentActualSeverityService primaryIncidentActualSeverityService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IPrimaryIncidentActualSeverityService>();

            foreach (var primaryIncidentActualSeverityData in primaryIncidentActualSeverityDictionary)
            {
                PrimaryIncident primaryIncidents = primaryIncidentService.SelectById(new PrimaryIncident() { Id = primaryIncidentActualSeverityData.Key });

                foreach (var actualSeverityId in primaryIncidentActualSeverityData.Value)
                {
                    SeverityMatrixtype severityMatrixtype =
                        severityMatrixtypeService.SelectById(new SeverityMatrixtype() { Id = actualSeverityId });
                    PrimaryIncidentActualSeverity primaryIncidentActualSeverity = new PrimaryIncidentActualSeverity()
                    { PrimaryIncident = primaryIncidents, ActualSeverity = severityMatrixtype };
                    primaryIncidentActualSeverityService.Insert(primaryIncidentActualSeverity);
                }

            }
            return new JsonResult("Success");
        }

        ////Sample: http://localhost:44703/DataPrep/PopulateIncidentTypePerformanceMetric
        public ActionResult PopulateIncidentTypePerformanceMetric()
        {
	        Dictionary<int, int[]> incidentTypePerformanceMetricDictionary = new Dictionary<int, int[]>()
	        {
		        { 30, new int[] { 59, 108, 98, 99, 21, 69, 102, 27 } },
		        { 31, new int[] { 59, 108, 98, 99, 21, 69, 102, 27 } },
		        { 6, new int[] { 61, 78, 79 } },
		        { 36, new int[] { 82, 89, 94, 66, 96, 69, 70, 114, 107, 115, 75, 100, 101 } },
		        { 26, new int[] { 81, 82, 90, 89, 10, 108, 93, 94, 38, 96, 69, 104 } },
		        { 37, new int[] { 83, 84, 59, 90, 89, 10, 41, 111, 93, 38, 29, 75 } },
		        { 38, new int[] { 81, 82, 90, 89, 93, 108, 38, 54, 63 } },
		        { 73, new int[] { 69 } },
		        { 64, new int[] { 96, 69, 115 } },
		        { 42, new int[] { 108, 94, 96, 69, 104 } },
		        { 65, new int[] { 114, 100, 101 } },
		        { 46, new int[] { 58, 96, 69, 27, 118 } },
		        { 51, new int[] { 69, 27, 106, 75 } },
		        { 52, new int[] { 63, 96, 112, 113, 69 } },
		        { 53, new int[] { 90, 89, 75 } },
		        { 67, new int[] { 89, 94 } },
		        { 54, new int[] { 82, 89, 94, 66, 96, 69, 70, 114, 107, 115, 75, 100, 101 } },
		        { 68, new int[] { 114, 107, 118, 115, 92, 77, 69, 85 } },
		        { 59, new int[] { 70, 106 } },
		        { 71, new int[] { 96, 21, 69, 103, 115 } },
		        { 70, new int[] { 96, 21, 69, 103, 115 } },
		        { 72, new int[] { 69 } },
		        {
			        35,
			        new int[]
			        {
				        81, 82, 83, 84, 58, 85, 59, 86, 87, 88, 90, 89, 91, 10, 92, 41, 93, 61, 94, 63, 66, 38, 96, 98,
				        99, 21, 69, 100, 101, 70, 102, 103, 27, 54, 29, 104, 105, 106, 75, 107, 55, 77, 78, 79, 116, 117
			        }
		        },
		        {
			        55,
			        new int[]
			        {
				        81, 82, 83, 84, 58, 85, 59, 86, 87, 88, 90, 89, 91, 10, 92, 41, 93, 61, 94, 63, 66, 38, 96, 98,
				        99, 21, 69, 100, 101, 70, 102, 103, 27, 54, 29, 104, 105, 106, 75, 107, 55, 77, 78, 79, 116, 117
			        }
		        },
		        {
			        60,
			        new int[]
			        {
				        81, 82, 83, 84, 58, 85, 59, 86, 87, 88, 90, 89, 91, 10, 92, 41, 93, 61, 94, 63, 66, 38, 96, 98,
				        99, 21, 69, 100, 101, 70, 102, 103, 27, 54, 29, 104, 105, 106, 75, 107, 55, 77, 78, 79, 116, 117
			        }
		        },
		        { 32, new int[] { 92, 77, 69, 85 } },
		        { 33, new int[] { 92, 77, 69, 85 } },
		        { 34, new int[] { 92, 77, 69, 85, 63, 112, 113, 114 } },
		        { 39, new int[] { 92, 77, 69, 85 } },
		        { 61, new int[] { 92, 77, 69, 85 } },
		        { 62, new int[] { 92, 77, 69, 85 } },
		        { 43, new int[] { 92, 77, 69, 85 } },
		        { 44, new int[] { 92, 77, 69, 85 } },
		        { 45, new int[] { 92, 77, 69, 85 } },
		        { 47, new int[] { 92, 77, 69, 85 } },
		        { 49, new int[] { 92, 77, 69, 85 } },
		        { 66, new int[] { 92, 77, 69, 85 } },
		        { 74, new int[] { 83, 84, 59, 86, 109, 110, 87, 88, 91, 10, 111, 63, 54, 29, 55, 105, 116, 117 } },
		        { 75, new int[] { 83, 84, 59, 86, 109, 110, 87, 88, 91, 10, 111, 63, 54, 29, 55, 105, 116, 117 } },
		        { 76, new int[] { 83, 84, 59, 86, 109, 110, 87, 88, 91, 10, 111, 63, 54, 29, 55, 105, 116, 117 } },
		        { 77, new int[] { 83, 84, 59, 86, 109, 110, 87, 88, 91, 10, 111, 63, 54, 29, 55, 105, 116, 117 } },
		        { 63, new int[] { 83, 84, 59, 86, 109, 110, 87, 88, 91, 10, 111, 63, 54, 29, 55, 105, 116, 117 } }
	        };
            IIncidentTypeService incidentTypeService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IIncidentTypeService>();
            IPerformanceMetricService performanceMetricService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IPerformanceMetricService>();
            IIncidentTypePerformanceMetricService incidentTypePerformanceMetricService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IIncidentTypePerformanceMetricService>();

            foreach (var incidentTypePerformanceMetricData in incidentTypePerformanceMetricDictionary)
            {
	            IncidentType incidentType = incidentTypeService.SelectById(new IncidentType() { Id = incidentTypePerformanceMetricData.Key });

                foreach (var performanceMetricId in incidentTypePerformanceMetricData.Value)
                {
                    PerformanceMetric performanceMetric =
                        performanceMetricService.SelectById(new PerformanceMetric() { Id = performanceMetricId });
                    IncidentTypePerformanceMetric incidentTypePerformanceMetric = new IncidentTypePerformanceMetric()
                    { IncidentType = incidentType, PerformanceMetric = performanceMetric };
                    incidentTypePerformanceMetricService.Insert(incidentTypePerformanceMetric);
                }

            }
            return new JsonResult("Success");
        }


    }




    public class LabTest
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public LabTestType Type { get; set; }
        public List<TestParameter> Parameters { get; set; }
    }

    public class TestParameter
    {
        public TestParameter(string key, string value)
        {
            Key = key;
            Value = value;
        }
        public string Key { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
    }


}
