using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using eServiceOnline.Gateway;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Crew;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ResourceSchedule;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment;
using Sesi.SanjelData.Entities.Common.BusinessEntities.HumanResources;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Organization;

namespace eServiceOnline.BusinessProcess
{
    public class CrewBoardProcess
    {
        public static void AddUnitToCrew(int truckUnitId, int crewId, bool isUpdateCrew)
        {
            TruckUnit truckUnit = eServiceOnlineGateway.Instance.GetTruckUnitById(truckUnitId);
            SanjelCrew sanjelCrew = eServiceOnlineGateway.Instance.GetCrewById(crewId, true);
            SanjelCrewTruckUnitSection truckUnitSection = new SanjelCrewTruckUnitSection();
            truckUnitSection.TruckUnit = truckUnit;
            sanjelCrew.SanjelCrewTruckUnitSection.Add(truckUnitSection);

            DateTime dateTime = DateTime.Now;
            List<SanjelCrewSchedule> crewSchedules = GetFutureCrewSchedules(sanjelCrew.Id, dateTime);
            foreach (SanjelCrewSchedule crewSchedule in crewSchedules)
            {
                UnitSchedule unitSchedule = new UnitSchedule
                {
                    StartTime = crewSchedule.StartTime >= dateTime ? crewSchedule.StartTime : dateTime,
                    Name = crewSchedule.Name,
                    EndTime = crewSchedule.EndTime,
                    TruckUnit = truckUnit,
                    OwnerId = crewSchedule.Id,
                    WorkingServicePoint = crewSchedule.WorkingServicePoint,
                    SanjelCrewSchedule = crewSchedule
                };
                eServiceOnlineGateway.Instance.InsertUnitSchedule(unitSchedule);
            }

            if (isUpdateCrew)
            {
                sanjelCrew.Name = BuildCrewName(sanjelCrew);
                sanjelCrew.Description = BuildCrewDescription(sanjelCrew);
                eServiceOnlineGateway.Instance.UpdateCrew(sanjelCrew, true);
            }
           
        }

        public static void AddWorkerToCrew(int workerId, int crewId, int crewPositionId, bool isUpdateCrew)
        {
            Employee worker = eServiceOnlineGateway.Instance.GetEmployeeById(workerId);
            SanjelCrew sanjelCrew = eServiceOnlineGateway.Instance.GetCrewById(crewId, true);
            CrewPosition crewPosition = eServiceOnlineGateway.Instance.GetCrewPositionById(crewPositionId);
            SanjelCrewWorkerSection workerSection = new SanjelCrewWorkerSection();
            workerSection.Worker = worker;
            workerSection.CrewPosition = crewPosition;
            sanjelCrew.SanjelCrewWorkerSection.Add(workerSection);

            DateTime dateTime = DateTime.Now;
            List<SanjelCrewSchedule> crewSchedules = GetFutureCrewSchedules(crewId, dateTime);
            foreach (SanjelCrewSchedule crewSchedule in crewSchedules)
            {
                WorkerSchedule workerSchedule = new WorkerSchedule
                {
                    StartTime = crewSchedule.StartTime >= dateTime ? crewSchedule.StartTime : dateTime,
                    Name = crewSchedule.Name,
                    EndTime = crewSchedule.EndTime,
                    Worker = worker,
                    OwnerId = crewSchedule.Id,
                    WorkingServicePoint = crewSchedule.WorkingServicePoint
                };
                eServiceOnlineGateway.Instance.InsertWorkerSchedule(workerSchedule);
            }

            if (isUpdateCrew)
            {
                sanjelCrew.Name = BuildCrewName(sanjelCrew);
                sanjelCrew.Description = BuildCrewDescription(sanjelCrew);
                eServiceOnlineGateway.Instance.UpdateCrew(sanjelCrew, true);
            }
        }

        public static string BuildCrewName(SanjelCrew sanjelCrew)
        {
            if (sanjelCrew.Type.Name.Equals("Bulker Crew"))
            {
                string crewName = string.Empty;
                foreach (var sanjelCrewWorkerSection in sanjelCrew.SanjelCrewWorkerSection)
                {
                    var worker = CacheData.Employees.FirstOrDefault(p=>p.Id==sanjelCrewWorkerSection.Worker.Id);
                    if(worker != null && !string.IsNullOrEmpty(worker.FirstName))
                        crewName += $"{worker.FirstName}" + " | ";
                }

                foreach (var crewTruckUnitSection in sanjelCrew.SanjelCrewTruckUnitSection)
                {
                    var truckUnit = CacheData.TruckUnits.FirstOrDefault(p=>p.Id==crewTruckUnitSection.TruckUnit.Id);
                    if(truckUnit != null && truckUnit.UnitSubType.Id == 276) 
                        crewName += truckUnit.UnitNumber + " | ";
                }

                return string.IsNullOrEmpty(crewName)?string.Empty: crewName.Trim().TrimEnd('|').Trim();
            }
            else
                return sanjelCrew.SanjelCrewTruckUnitSection.FirstOrDefault()?.TruckUnit.UnitNumber;
        }

        public static string BuildCrewDescription(SanjelCrew sanjelCrew)
        {
            string crewDescription = string.Empty;

            foreach (var workerSection in sanjelCrew.SanjelCrewWorkerSection)
            {
                crewDescription += " | " + $"{workerSection.Worker.Name}";
            }

            foreach (var unitSection in sanjelCrew.SanjelCrewTruckUnitSection)
            {
                crewDescription += " | " + unitSection.TruckUnit.UnitNumber;
            }
            if (!string.IsNullOrEmpty(crewDescription))
            {
                crewDescription = crewDescription.Length > 255 ? crewDescription.Substring(2, 254) : crewDescription.Substring(2);
            }

            return crewDescription;
        }

        public static List<TruckUnit> GetTruckUnitsByCrew(int crewId)
        {
            List<TruckUnit> truckUnits = new List<TruckUnit>();
            List<SanjelCrewTruckUnitSection> truckUnitSections = eServiceOnlineGateway.Instance.GetTruckUnitSectionsByCrew(crewId).OrderBy(p => p.Id).ToList();
            if (truckUnitSections.Count > 0)
            {
                foreach (SanjelCrewTruckUnitSection truckUnitSection in truckUnitSections)
                {
                    if (truckUnitSection.TruckUnit != null)
                    {
                        TruckUnit unit = eServiceOnlineGateway.Instance.GetTruckUnitById(truckUnitSection.TruckUnit.Id);
                        truckUnits.Add(unit);
                    }
                }
            }

            return truckUnits;
        }

        public static List<Employee> GetWorkersByCrew(int crewId)
        {
            List<Employee> workers = new List<Employee>();
            List<SanjelCrewWorkerSection> workerSections = eServiceOnlineGateway.Instance.GetWorkerSectionsByCrew(crewId).OrderBy(p => p.Id).ToList();
            if (workerSections.Count > 0)
            {
                foreach (SanjelCrewWorkerSection workerSection in workerSections)
                {
                    if (workerSection.Worker != null)
                    {
                        Employee worker = eServiceOnlineGateway.Instance.GetEmployeeById(workerSection.Worker.Id);
                        workers.Add(worker);
                    }
                }
            }

            return workers;
        }
        //Refactored
        public static void RemoveUnitFromCrew(int truckUnitId, int crewId)
        {
            SanjelCrew originalSanjelCrew = eServiceOnlineGateway.Instance.GetCrewById(crewId, true);

            DateTime dateTime = DateTime.Now;
            List<SanjelCrewSchedule> crewSchedules = GetFutureCrewSchedules(crewId, dateTime);
            foreach (SanjelCrewSchedule crewSchedule in crewSchedules)
            {
                var originalSanjelCrewSchedule = eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(crewSchedule.Id, true);

                originalSanjelCrewSchedule.UnitSchedule.RemoveAll(p => p.TruckUnit.Id == truckUnitId);

                eServiceOnlineGateway.Instance.UpdateSanjelCrewSchedule(originalSanjelCrewSchedule, true);
            }

            originalSanjelCrew.SanjelCrewTruckUnitSection.RemoveAll(p => p.TruckUnit.Id == truckUnitId);

            originalSanjelCrew.Name = BuildCrewName(originalSanjelCrew);
            originalSanjelCrew.Description = BuildCrewDescription(originalSanjelCrew);
            eServiceOnlineGateway.Instance.UpdateCrew(originalSanjelCrew, true);
        }

        //Factored
        public static void RemoveWorkerFromCrew(int workerId, int crewId)
        {
            SanjelCrew originalSanjelCrew = eServiceOnlineGateway.Instance.GetCrewById(crewId, true);

            DateTime dateTime = DateTime.Now;
            List<SanjelCrewSchedule> crewSchedules = GetFutureCrewSchedules(crewId, dateTime);
            foreach (SanjelCrewSchedule crewSchedule in crewSchedules)
            {
                var originalSanjelCrewSchedule = eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(crewSchedule.Id, true);

                originalSanjelCrewSchedule.WorkerSchedule.RemoveAll(p => p.Worker.Id == workerId);

                eServiceOnlineGateway.Instance.UpdateSanjelCrewSchedule(originalSanjelCrewSchedule, true);
            }

            originalSanjelCrew.SanjelCrewWorkerSection.RemoveAll(p => p.Worker.Id == workerId);

            originalSanjelCrew.Name = BuildCrewName(originalSanjelCrew);
            originalSanjelCrew.Description = BuildCrewDescription(originalSanjelCrew);
            eServiceOnlineGateway.Instance.UpdateCrew(originalSanjelCrew, true);

        }

        public static List<SanjelCrew> GetAllCrewInfo(Collection<int> servicePointIds)
        {
            List<SanjelCrew> crews = eServiceOnlineGateway.Instance.GetCrewList();
            if(servicePointIds!=null && servicePointIds.Count!=0)
                crews = crews.Where(p => servicePointIds.Contains(p.HomeServicePoint.Id) || servicePointIds.Contains(p.WorkingServicePoint.Id)).ToList();

            return crews;
        }

        public static SanjelCrew GetCrewById(int id)
        {
            return eServiceOnlineGateway.Instance.GetCrewById(id);
        }

        public static Employee GetEmployeeById(int id)
        {
            return eServiceOnlineGateway.Instance.GetEmployeeById(id);
        }

        public static List<SanjelCrew> GetEffectiveCrews(DateTime startTime, double duration, int workingDistrict, int rigJobId)
        {
            List<SanjelCrew> effectiveCrews = new List<SanjelCrew>();
            List<SanjelCrew> crews = eServiceOnlineGateway.Instance.GetCrewList().FindAll(p => p.Type.Name.Equals("Pumper Crew") || p.Type.Name.Equals("Spare Crew"));
            List<RigJobSanjelCrewSection> rigJobSanjelCrewSections = eServiceOnlineGateway.Instance.GetRigJobCrewSectionByRigJob(rigJobId);

            if (rigJobSanjelCrewSections != null && rigJobSanjelCrewSections.Count > 0)
            {
                List<int> crewIds = new List<int>();
                foreach (RigJobSanjelCrewSection rigJobSanjelCrewSection in rigJobSanjelCrewSections)
                {
                    if (rigJobSanjelCrewSection.SanjelCrew != null) crewIds.Add(rigJobSanjelCrewSection.SanjelCrew.Id);
                }
                crews = crews.Where(p => !crewIds.Contains(p.Id)).ToList();
            }

            foreach (SanjelCrew crew in crews)
            {
                List<SanjelCrewTruckUnitSection> truckUnitSections = eServiceOnlineGateway.Instance.GetTruckUnitSectionsByCrew(crew.Id);
                List<SanjelCrewWorkerSection> workerSections = eServiceOnlineGateway.Instance.GetWorkerSectionsByCrew(crew.Id);
//                List<CrewSchedule> crewSchedules = new List<CrewSchedule>();
//                List<RigJobCrewSection> rigJobCrewSections = eServiceOnlineGateway.Instance.GetRigJobCrewSectionsByCrew(crew.Id);
//                foreach (RigJobCrewSection rigJobCrewSection in rigJobCrewSections)
//                {
//                    crewSchedules.Add(eServiceOnlineGateway.Instance.GetCrewScheduleByJobCrewSection(rigJobCrewSection.Id));
//                }
//                crewSchedules = crewSchedules.FindAll(p => (p.StartTime >= startTime && p.StartTime < startTime.AddHours(duration)) || (p.EndTime > startTime && p.EndTime <= startTime.AddHours(duration)));
                if (truckUnitSections.Count > 0 && workerSections.Count > 0 && crew.WorkingServicePoint.Id == workingDistrict)
                {
                    effectiveCrews.Add(crew);
                }
            }

            return effectiveCrews;
        }

        public static List<SanjelCrew> GetCrewsByServicePoint(Collection<int> servicePoints, out int count)
        {

            List<SanjelCrew> crews = eServiceOnlineGateway.Instance.GetCrewList(servicePoints);

            /*
            List<SanjelCrewSchedule> crewSchedules = eServiceOnlineGateway.Instance.GetCrewSchedules().Where(p => servicePoints.Contains(p.WorkingServicePoint.Id)).ToList();
            foreach (SanjelCrewSchedule crewSchedule in crewSchedules)
            {
                SanjelCrew sanjelCrew = crews.Find(p => p.Id == crewSchedule.SanjelCrew.Id);
                if (sanjelCrew == null)
                {
                    SanjelCrew crew = eServiceOnlineGateway.Instance.GetCrewById(crewSchedule.SanjelCrew.Id);
                    if (crew != null) crews.Add(crew);
                }
            }
            */

            count = crews.Count;
//            crews = crews.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();

            return crews;
        }

        public static List<TruckUnit> GetUnitsByServicePoint(int pageSize, int pageNumber, Collection<int> servicePoints, out int count)
        {
            List<TruckUnit> truckUnits = eServiceOnlineGateway.Instance.GetTruckUnitList();
            truckUnits = truckUnits.Where(p => servicePoints.Contains(p.ServicePoint.Id)).ToList();
            count = truckUnits.Count;
            truckUnits = truckUnits.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();

            return truckUnits;
        }

        public static List<Employee> GetWorkerListByServicePoints(int pageSize, int pageNumber,
            Collection<int> servicePoints, out int count)
        {
            List<Employee> allEmployees = EServiceReferenceData.Data.EmployeeCollection.ToList();
            List<Employee> employees = allEmployees;

            if (servicePoints.Count > 0)
            {
                employees = employees.Where(p => servicePoints.Contains(p.ServicePoint.Id)).ToList();
                List<WorkerSchedule> workerSchedules = eServiceOnlineGateway.Instance.GetWorkerSchedules().Where(p => servicePoints.Contains(p.WorkingServicePoint.Id)).ToList();
                foreach (WorkerSchedule workecSchedule in workerSchedules)
                {
                    Employee worker = employees.Find(p => p.Id == workecSchedule.Worker.Id);
                    if (worker == null)
                    {
                        worker = allEmployees.Find(p => p.Id == workecSchedule.Worker.Id);
                        if (worker != null) employees.Add(worker);
                    }
                }
            }
            count = employees.Count;
            employees = employees.OrderBy(s => s.LastName).Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();
            return employees;
        }
        public static List<TruckUnit> GetActivatedTruckUnits(int crewId)
        {
            SanjelCrew crew = eServiceOnlineGateway.Instance.GetCrewById(crewId);
            List<TruckUnit> truckUnits = new List<TruckUnit>();
            if (crew.WorkingServicePoint != null) truckUnits = eServiceOnlineGateway.Instance.GetTruckUnitsByServicePoint(crew.WorkingServicePoint.Id);

            return truckUnits;
        }
        public static int UpdateWorker(Employee employee)
        {
            return eServiceOnlineGateway.Instance.UpdateWorker(employee);
        }

        public static List<Employee> GetActivatedEmployees(int crewId)
        {
            SanjelCrew crew = eServiceOnlineGateway.Instance.GetCrewById(crewId);
            List<Employee> employees = new List<Employee>();
            if (crew.WorkingServicePoint != null) employees = eServiceOnlineGateway.Instance.GetEmployeesByServicePoint(crew.WorkingServicePoint.Id);
            employees = employees.FindAll(p => p.BonusPosition != null && p.BonusPosition.Id > 0);

            return employees;
        }

        public static List<TruckUnit> GetTruckUnitsByServicePoints(int pageSize, int pageNumber,
            Collection<int> servicePoints, out int count)
        {
            List<TruckUnit> allTruckUnits = EServiceReferenceData.Data.TruckUnitCollection.ToList();
            List<TruckUnit> truckUnits = allTruckUnits;
            if (servicePoints.Count > 0)
            {
                truckUnits = truckUnits.Where(p => servicePoints.Contains(p.ServicePoint.Id)).ToList();
                List<UnitSchedule> unitSchedules = eServiceOnlineGateway.Instance.GetUnitSchedules().Where(p => servicePoints.Contains(p.WorkingServicePoint.Id)).ToList();
                foreach (UnitSchedule unitSchedule in unitSchedules)
                {
                    TruckUnit scheduledTruckUnit = truckUnits.Find(p => p.Id == unitSchedule.TruckUnit.Id);

                    if (scheduledTruckUnit == null)
                    {
                        var truckUnit = allTruckUnits.Find(p => p.Id == unitSchedule.TruckUnit.Id);
                        if (truckUnit != null)
                        {
                            truckUnits.Add(truckUnit);
                        }
                    }
                }
            }

            List<TruckUnit> pumperUnits = new List<TruckUnit>();
            List<TruckUnit> bulkerUnits = new List<TruckUnit>();
            List<TruckUnit> tractorUnits = new List<TruckUnit>();
            List<TruckUnit> pickupUnits = new List<TruckUnit>();
            List<TruckUnit> unitList = new List<TruckUnit>();
            foreach (var item in truckUnits)
            {
                if (item.UnitMainType != null)
                {
                    if (item.UnitMainType.Description.Equals("ROLLING STOCK BODY JOB"))
                    {
                        pumperUnits.Add(item);
                    }
                    if (item.UnitMainType.Description.Equals("ROLLING STOCK TRAILER"))
                    {
                        bulkerUnits.Add(item);
                    }
                    if (item.UnitMainType.Description.Equals("ROLLING STOCK TRACTOR"))
                    {
                        tractorUnits.Add(item);
                    }
                    if (item.UnitMainType.Description.Equals("LIGHT DUTY VEHICLES"))
                    {
                        pickupUnits.Add(item);
                    }
                }

            }
            unitList.AddRange(pumperUnits);
            unitList.AddRange(bulkerUnits);
            unitList.AddRange(tractorUnits);
            unitList.AddRange(pickupUnits);
            count = unitList.Count;
            unitList = unitList.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();
            return unitList;
        }

        public static void AssignCrewToAnotherDistrict(int crewId, int workingDistrictId)
        {
            SanjelCrew crew = eServiceOnlineGateway.Instance.GetCrewById(crewId);
            if (crew != null)
            {
                ServicePoint workingServicePoint = eServiceOnlineGateway.Instance.GetServicePointById(workingDistrictId);
                crew.WorkingServicePoint = workingServicePoint;
                eServiceOnlineGateway.Instance.UpdateCrew(crew);
            }
        }

        public static int CreateCrew(SanjelCrew crew, int homeDistrictId, int primaryTruckUnitId = 0, int secondaryTruckUnitId = 0, int supervisorId = 0, int crewMemberId = 0)
        {
            try
            {
                ServicePoint servicePoint = eServiceOnlineGateway.Instance.GetServicePointById(homeDistrictId);
                crew.HomeServicePoint = servicePoint;
                crew.WorkingServicePoint = servicePoint;

                List<TruckUnit> truckUnits = new List<TruckUnit>();
                List<Employee> workers = new List<Employee>();

                bool isPumperCrew = crew.Type.Name.Equals("Pumper Crew");

                if(crew.SanjelCrewTruckUnitSection == null) crew.SanjelCrewTruckUnitSection = new List<SanjelCrewTruckUnitSection>();
                if(crew.SanjelCrewWorkerSection == null) crew.SanjelCrewWorkerSection = new List<SanjelCrewWorkerSection>();

                if (supervisorId != 0 )
                {
                    if(isPumperCrew)    
                        AddWorkerToCrew(crew, supervisorId, 1);
                    else
                        AddWorkerToCrew(crew, supervisorId, 2);
                }

                if (crewMemberId != 0)
                {
                    AddWorkerToCrew(crew, crewMemberId, 2);
                }

                if (primaryTruckUnitId != 0)
                {
                    AddTruckUnitToCrew(crew, primaryTruckUnitId);
                }

                if (secondaryTruckUnitId != 0)
                {
                    AddTruckUnitToCrew(crew, secondaryTruckUnitId);
                }

                crew.Name = BuildCrewName(crew);
                crew.Description = BuildCrewDescription(crew);

                eServiceOnlineGateway.Instance.CreateCrew(crew);

                return 1;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        private static void AddTruckUnitToCrew(SanjelCrew crew, int truckUnitId)
        {
            TruckUnit truckUnit = eServiceOnlineGateway.Instance.GetTruckUnitById(truckUnitId);
            SanjelCrewTruckUnitSection sanjelCrewTruckUnitSection = new SanjelCrewTruckUnitSection() { TruckUnit = truckUnit };
            crew.SanjelCrewTruckUnitSection.Add(sanjelCrewTruckUnitSection);
        }

        private static void AddWorkerToCrew(SanjelCrew crew, int crewMemberId, int positionId)
        {
            Employee worker = eServiceOnlineGateway.Instance.GetEmployeeById(crewMemberId);
            CrewPosition crewPosition = eServiceOnlineGateway.Instance.GetCrewPositionById(positionId);
            SanjelCrewWorkerSection sanjelCrewWorkerSection = new SanjelCrewWorkerSection()
                { Worker = worker, CrewPosition = crewPosition };
            crew.SanjelCrewWorkerSection.Add(sanjelCrewWorkerSection);
        }

        public static List<SanjelCrewSchedule> GetFutureCrewSchedules(int crewId, DateTime dateTime)
        {
            List<SanjelCrewSchedule> crewSchedules = eServiceOnlineGateway.Instance.GetFutureCrewSchedules(crewId, dateTime);

            
            /*
            List<RigJobSanjelCrewSection> rigJobCrewSections = eServiceOnlineGateway.Instance.GetRigJobCrewSectionsByCrew(crewId);
            List<SanjelCrewSchedule> crewSchedules = new List<SanjelCrewSchedule>();
            foreach (RigJobSanjelCrewSection rigJobCrewSection in rigJobCrewSections)
            {
                SanjelCrewSchedule crewSchedule = eServiceOnlineGateway.Instance.GetCrewScheduleByJobCrewSection(rigJobCrewSection.Id);
                if (crewSchedule != null) crewSchedules.Add(crewSchedule);
            }
            crewSchedules = crewSchedules.FindAll(p => p?.EndTime >= dateTime).ToList();
            */
            
            return crewSchedules;
        }

        public static List<CrewType> GetCrewTypesForSanjelCrew()
        {
            List<CrewType> crewTypes = eServiceOnlineGateway.Instance.GetCrewTypes();
            //Apr 26, 2023:DRB 2.1 - Hide pumper crew creation from old resource board
            List<CrewType> sanjelCrewTypes = crewTypes.FindAll(p => p.Id != 4 && p.Id != 1);

            return sanjelCrewTypes;
        }
        //Refactored
        public static void RemoveAllWorker(int crewId)
        {

            SanjelCrew originalSanjelCrew = eServiceOnlineGateway.Instance.GetCrewById(crewId, true);

            DateTime dateTime = DateTime.Now;
            List<SanjelCrewSchedule> crewSchedules = GetFutureCrewSchedules(crewId, dateTime);
            foreach (SanjelCrewSchedule crewSchedule in crewSchedules)
            {
                var originalSanjelCrewSchedule = eServiceOnlineGateway.Instance.GetSanjelCrewScheduleById(crewSchedule.Id, true);

                originalSanjelCrewSchedule.WorkerSchedule.Clear();

                eServiceOnlineGateway.Instance.UpdateSanjelCrewSchedule(originalSanjelCrewSchedule, true);
            }

            originalSanjelCrew.SanjelCrewWorkerSection.Clear();

            originalSanjelCrew.Name = BuildCrewName(originalSanjelCrew);
            originalSanjelCrew.Description = BuildCrewDescription(originalSanjelCrew);
            eServiceOnlineGateway.Instance.UpdateCrew(originalSanjelCrew, true);
        }

        public static List<ThirdPartyBulkerCrew> GetThirdPartyBulkerCrewsByServicePoint(Collection<int> servicePoints, out int count)
        {
            List<ThirdPartyBulkerCrew> thirdPartyBulkerCrews = eServiceOnlineGateway.Instance.GetThirdPartyBulkerCrewsByServicePoints(servicePoints);
            
            count = thirdPartyBulkerCrews.Count;

            return thirdPartyBulkerCrews;
        }
    }
}