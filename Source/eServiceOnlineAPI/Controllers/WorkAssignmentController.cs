using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MetaShare.Common.Core.CommonService;
using MetaShare.Common.Core.Proxies;
using Microsoft.AspNetCore.Mvc;
using Sanjel.Common.EService.Sections.Header;
using Sanjel.Services;
using Sanjel.Services.Interfaces;
using Sesi.SanjelData.Entities.BusinessEntities.VariablePay;
using Sesi.SanjelData.Entities.Common.BusinessEntities.HumanResources;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Operation;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Organization;
using Sesi.SanjelData.Entities.Common.Entities.General;
using Sesi.SanjelData.Services.Interfaces.BusinessEntities.VariablePay;
using Sesi.SanjelData.Services.Interfaces.Common.BusinessEntities.HumanResources;
using Sesi.SanjelData.Services.Interfaces.Common.BusinessEntities.Operation;
using Sesi.SanjelData.Services.Interfaces.Common.BusinessEntities.Organization;
using Sesi.SanjelData.Services.Interfaces.Common.Entities.General;

namespace eServiceOnline.WebAPI.Controllers
{
    //Sample: http://localhost:52346/WorkAssignment/SyncWorkAssignments/92be823e-a0e2-45a4-b8ad-bfb9034818d2
    [Route("[controller]/[action]/{jobUniqueId}")]
    [ApiController]
    public class WorkAssignmentController : ControllerBase
    {
        public ActionResult SyncWorkAssignments(string jobUniqueId)
        {
            try
            {
                var jobService = MetaShare.Common.ServiceModel.Services.ServiceFactory.GetService<IJobService>();
                var job = jobService.GetJobByUniqueId(jobUniqueId);
                if(job.JobData.BonusArea == null || string.IsNullOrEmpty(job.JobData.BonusArea.Name))
                    throw new Exception("Job #" + job.JobNumber.ToString() + " Pay Area is not filled.");
                var serviceTicketService =
                    MetaShare.Common.ServiceModel.Services.ServiceFactory.GetService<IServiceTicketService>();
                var serviceTicket = serviceTicketService.GetServiceTicketByUniqueId(jobUniqueId);
                var serviceReportService =
                    MetaShare.Common.ServiceModel.Services.ServiceFactory.GetService<IServiceReportService>();
                var serviceReport = serviceReportService.GetServiceReportByUniqueId(jobUniqueId);

                List<ServicePoint> servicePoints =
                    ServiceFactory.Instance.GetService<IServicePointService>().SelectAll();
                List<WorkType> workTypes = ServiceFactory.Instance.GetService<IWorkTypeService>().SelectAll();
                List<ProvinceOrState> provinceOrStates =
                    ServiceFactory.Instance.GetService<IProvinceOrStateService>().SelectAll();
                List<ClientCompany> clientCompanies =
                    ServiceFactory.Instance.GetService<IClientCompanyService>().SelectAll();
                List<JobRole> jobRoles = ServiceFactory.Instance.GetService<IJobRoleService>().SelectAll();
                List<ServiceLine> serviceLines = ServiceFactory.Instance.GetService<IServiceLineService>().SelectAll();
                List<Employee> employees = ServiceFactory.Instance.GetService<IEmployeeService>().SelectAll();
                List<BonusPosition> bonusPositions =
                    ServiceFactory.Instance.GetService<IBonusPositionService>().SelectAll();
                List<PayArea> payAreas = ServiceFactory.Instance.GetService<IPayAreaService>().SelectAll();
                IWorkAssignmentService workAssignmentService =
                    ServiceFactory.Instance.GetService<IWorkAssignmentService>();
                var serviceLine = serviceLines.Find(p => p.Id == job.JobData.JobType.ServiceLine.Id);
                    
                var unitPersonnelJobBonusSections = serviceReport?.CommonSection?.UnitPersonnel?.JobBonusSections;

                if (unitPersonnelJobBonusSections != null)
                {
                    if (workAssignmentService != null)
                    {
                        var existingWorkAssignments = workAssignmentService.SelectBy(
                            new WorkAssignment() {JobUniqueId = jobUniqueId},
                            new List<string>() {"JobUniqueId"});

                        List<WorkAssignment> workAssignments = new List<WorkAssignment>();

                        string missingEmployee = null;

                        foreach (var unitPersonnelJobBonusSection in unitPersonnelJobBonusSections)
                        {
                            var workAssignment =
                                existingWorkAssignments.FirstOrDefault(p =>
                                    p.SourceId == unitPersonnelJobBonusSection.Id);
                            bool newRecord = false;
                            if (workAssignment == null)
                            {
                                workAssignment = new WorkAssignment();
                                newRecord = true;
                            }

                            if (serviceTicket.BillingInformation.IsCode9000)
                            {
                                workAssignment.JobFlag = JobFlag.Code9000;
                            }
                            else if (job.JobData.JobType.Name.StartsWith("Incomplete"))
                            {
                                workAssignment.JobFlag = JobFlag.Incomplete;
                            }
                            else if (job.JobData.JobType.Name.StartsWith("Incomplete"))
                            {
                                workAssignment.JobFlag = JobFlag.DayRate;
                            }
                            else
                            {
                                workAssignment.JobFlag = JobFlag.Complete;
                            }

                            workAssignment.ServiceLine = serviceLine;
                            if (unitPersonnelJobBonusSection.Employee == null)
                            {
                                missingEmployee += !string.IsNullOrEmpty(missingEmployee)
                                    ? "\n"
                                    : "" + "Job #" + job.JobNumber + " has a work assignment with no employee";
                                continue;
                            }

                            workAssignment.Employee = employees.Find(p => p.Id == unitPersonnelJobBonusSection.Employee.Id);
                            if(workAssignment.Employee == null)
                                workAssignment.Employee =new Employee()
                                {
                                    Id = unitPersonnelJobBonusSection.Employee.Id, 
                                    Name = unitPersonnelJobBonusSection.Employee.Name, 
                                    LastName = unitPersonnelJobBonusSection.Employee.LastName,
                                    FirstName = unitPersonnelJobBonusSection.Employee.FirstName,
                                    PreferedFirstName = unitPersonnelJobBonusSection.Employee.PreferedFirstName

                                };

                            if (!string.IsNullOrEmpty(workAssignment.Employee.PreferedFirstName))
                                workAssignment.Employee.Name = workAssignment.Employee.LastName + ", " + workAssignment.Employee.PreferedFirstName;
                            else 
                                workAssignment.Employee.Name = workAssignment.Employee.LastName + ", " + workAssignment.Employee.FirstName;

                            workAssignment.HrPosition = unitPersonnelJobBonusSection.BonusPosition != null
                                ? bonusPositions.Find(p => p.Id == unitPersonnelJobBonusSection.BonusPosition.Id)
                                : null;

                            var clientCompany = job.CompanyInformation.CompanyInfoSections
                                .FirstOrDefault(p => p.IsClient)?
                                .Company;

                            if (clientCompany != null)
                            {
                                workAssignment.ClientCompany = clientCompanies.Find(p => p.Id == clientCompany.Id);
                            }

                            workAssignment.JobRole = unitPersonnelJobBonusSection.JobRole != null
                                ? jobRoles.Find(p => p.Id == unitPersonnelJobBonusSection.JobRole.Id)
                                : null;
                            workAssignment.WorkingServicePoint = job.JobData.ServicePoint != null
                                ? servicePoints.Find(p => p.Id == job.JobData.ServicePoint.Id)
                                : null;
                            workAssignment.HomeServicePoint =
                                servicePoints.Find(p => p.Id == unitPersonnelJobBonusSection.Employee.DistrictId) ?? workAssignment.WorkingServicePoint;
                            workAssignment.WorkType = unitPersonnelJobBonusSection.Employee != null
                                ? workTypes.Find(p => p.Id == unitPersonnelJobBonusSection.WorkType.Id)
                                : null;
                            workAssignment.StartTime = unitPersonnelJobBonusSection.StartShift;
                            workAssignment.EndTime = unitPersonnelJobBonusSection.EndShift;
                            workAssignment.WorkingHours = unitPersonnelJobBonusSection.WorkingHours ?? 0;
                            workAssignment.StandbyHours = unitPersonnelJobBonusSection.StandbyTimeOffLocation ?? 0;
                            workAssignment.IsEligibleStandby = unitPersonnelJobBonusSection.ReceiveStandbyBonus;
                            workAssignment.IsTwoWay = unitPersonnelJobBonusSection.IsTwoWay;
                            workAssignment.TravelTime = unitPersonnelJobBonusSection.TwowayTravelTime ?? 0;
                            workAssignment.TravelDistance = unitPersonnelJobBonusSection.TravelDistance ?? 0;
                            workAssignment.JobUniqueId = jobUniqueId;
                            workAssignment.JobNumber = job.JobNumber.ToString();
                            workAssignment.JobDateTime = job.JobDateTime;
                            workAssignment.JobRevenue = serviceTicket.AdjustedFieldEstimate;
                            workAssignment.MtsNumber = unitPersonnelJobBonusSection.MtsNumber;
                            workAssignment.WorkDescription = unitPersonnelJobBonusSection.HaulDescription;
                            workAssignment.LoadQuantity = 1;
                            workAssignment.JobProvince =
                                provinceOrStates.Find(p => p.Id == job.WellLocationInformation.ProvinceOrState.Id);
                            workAssignment.WorkLocation = job.WellLocationInformation.DownHoleWellLocation;
                            workAssignment.PayArea = payAreas.Find(p => p.Name == job.JobData.BonusArea.Name) ?? new PayArea();
                            workAssignment.Status = WorkAssignmentStatus.Approved;
                            workAssignment.SourceId = unitPersonnelJobBonusSection.Id;
                            workAssignment.ModifiedUserName = job.ApprovalUser.Name;
                            workAssignment.IsCrewEfficiency = serviceReport.IsCrewEfficiency;
                            workAssignment.IsMultipleWellProject = serviceReport.IsMultipleWellProject;

                            if (newRecord)
                            {
                                workAssignmentService.Insert(workAssignment);
                            }
                            else
                            {
                                workAssignmentService.Update(workAssignment);
                            }

                            workAssignments.Add(workAssignment);
                        }

                        if(!string.IsNullOrEmpty(missingEmployee)) throw new Exception(missingEmployee);

                        if (existingWorkAssignments != null && existingWorkAssignments.Count > 0)
                        {
                            //there were workAssignment records, that means the job is pushed back to approval again.
                            //To be safe. We will delete the old records and recreate the new records, even they are not changed.
                            foreach (var existingWorkAssignment in existingWorkAssignments)
                            {
                                var currentWorkAssignment =
                                    workAssignments.Find(p => p.SourceId == existingWorkAssignment.SourceId);
                                if (currentWorkAssignment == null)
                                    workAssignmentService.Delete(existingWorkAssignment);
                            }
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                return new JsonResult(new {result = false, jobUniqueId, message = ex.Message});
            }

            return new JsonResult(new {result = true, jobUniqueId, message = "Succeed"});
        }
    }
}
