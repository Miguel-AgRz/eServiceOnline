using System.Collections.Generic;
using System.Collections.ObjectModel;
using eServiceOnline.Data;
using eServiceOnline.Models.Commons;
using eServiceOnline.Models.CrewBoard;
using eServiceOnline.Models.WorkerBoard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Sesi.SanjelData.Entities.Common.BusinessEntities.HumanResources;
using Syncfusion.JavaScript;

namespace eServiceOnline.Controllers
{
    public class WorkerBoardController : eServiceOnlineController
    {
        private readonly eServiceWebContext _context;
        public WorkerBoardController()
        {
            this._context = new eServiceWebContext();
        }
        // GET
        public IActionResult GetPageWorkerModels([FromBody] DataManager dataManager)
        {
            int pageSize = dataManager.Take;
            int pageNumber = dataManager.Skip / pageSize + 1;
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
            List<Employee> employees = this._context.GetWorkerListByServicePoints(pageSize, pageNumber, servicePoints, out count);
            List<WorkerViewModel> data = this.GetWorkerViewModel(employees);
            return this.Json(new { result = data, count = count });
        }

        private List<WorkerViewModel> GetWorkerViewModel(List<Employee> employees)
        {
            List<WorkerViewModel> data = new List<WorkerViewModel>();
            foreach (var employee in employees)
            {
                WorkerViewModel workerViewModel = new WorkerViewModel();
                workerViewModel.NoteModel = this.GetNoteModel(employee);
                workerViewModel.ProfileModel = this.GetProfileModel(employee);
                workerViewModel.PopulateFrom(employee);              
                data.Add(workerViewModel);
            }
            return data;
        }
        private NoteModel GetNoteModel(Employee employee)
        {
            NoteModel noteModel = new NoteModel();
            EmployeeNote note = this._context.GetEmployeeNoteByEmployee(employee.Id);
            noteModel.Id = employee.Id;
            noteModel.Notes = note?.Description ?? string.Empty;
            noteModel.ReturnActionName = "Index";
            noteModel.ReturnControllerName = "ResourceBoard";
            noteModel.CallingControllerName = "WorkerBoard";
            noteModel.CallingMethodName = "UpdateNotes";
            noteModel.PostControllerName = "WorkerBoard";
            noteModel.PostMethodName = "UpdateNotes";
            return noteModel;
        }
        private ProfileModel GetProfileModel(Employee employee)
        {
            ProfileModel profileModel = new ProfileModel();
            EmployeeProfile employeeProfile = this._context.GetEmployeeProfileByEmployee(employee.Id);
            profileModel.Id = employee.Id;
            profileModel.Profile = employeeProfile?.Description ?? string.Empty;
            profileModel.ReturnActionName = "Index";
            profileModel.ReturnControllerName = "ResourceBoard";
            profileModel.CallingControllerName = "WorkerBoard";
            profileModel.CallingMethodName = "UpdateProfile";
            profileModel.PostControllerName = "WorkerBoard";
            profileModel.PostMethodName = "UpdateProfile";
            return profileModel;
        }
        public override IActionResult UpdateNotes(NoteModel model)
        {
            eServiceWebContext.Instance.UpdateEmployeeNote(model.Id, model.Notes);
            return this.RedirectToAction(model.ReturnActionName, model.ReturnControllerName);
        }

        public override IActionResult UpdateProfile(ProfileModel model)
        {
            eServiceWebContext.Instance.UpdateEmployeeProfile(model.Id, model.Profile);
            return this.RedirectToAction(model.ReturnActionName, model.ReturnControllerName);
        }
    }
}