using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using eServiceOnline.Data;
using eServiceOnline.Models.Commons;
using eServiceOnline.Models.ProductHaul;
using eServiceOnline.Models.ThirdPartyCrewBoard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Crew;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ResourceSchedule;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Organization;
using Syncfusion.JavaScript;

namespace eServiceOnline.Controllers
{
    public class ThirdPartyCrewBoardController : eServiceOnlineController
    {
        private readonly eServiceWebContext _context;

        public ThirdPartyCrewBoardController()
        {
            this._context = new eServiceWebContext();
        }

        public ActionResult GetPagedThirdPartyCrewModels([FromBody] DataManager dataManager)
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

            List<ThirdPartyBulkerCrew> crews = this._context.GetThirdPartyBulkerCrewsByServicePoint(servicePoints, out count);
            List<ThirdPartyBulkerCrew> sortedCrews = crews.OrderBy(p => p.Type.Id).ToList().Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();
            List<ThirdPartyCrewViewModel> data = this.GetThirdPartyCrewViewModelList(sortedCrews).OrderBy(p => p.Unit.PropertyValue).ToList();

            return this.Json(new {result = data, count});
        }

        private List<ThirdPartyCrewViewModel> GetThirdPartyCrewViewModelList(List<ThirdPartyBulkerCrew> thirdPartyBulkerCrews)
        {
            List<ThirdPartyCrewViewModel> data = new List<ThirdPartyCrewViewModel>();
            foreach (ThirdPartyBulkerCrew thirdPartyBulkerCrew in thirdPartyBulkerCrews)
            {
                ThirdPartyCrewViewModel model = new ThirdPartyCrewViewModel();
                model.ThirdPartyBulkerCrewSchedules = this.GetThirdPartyCrewScheduleViewModelByCrewId(thirdPartyBulkerCrew.Id);
                model.NoteModel = this.GetNoteModel(thirdPartyBulkerCrew);
                model.PopulateFrom(thirdPartyBulkerCrew);             
                data.Add(model);
            }

            return data;
        }
        private NoteModel GetNoteModel(ThirdPartyBulkerCrew crew)
        {
            NoteModel noteModel = new NoteModel();
            ThirdPartyBulkerCrewNote note = this._context.GeThirdPartyBulkerCrewNoteByThirdPartyBulkerCrew(crew.Id);
            noteModel.Id = crew.Id;
            noteModel.Notes = note?.Description ?? string.Empty;
            noteModel.ReturnActionName = "Index";
            noteModel.ReturnControllerName = "ResourceBoard";
            noteModel.CallingControllerName = "ThirdPartyCrewBoard";
            noteModel.CallingMethodName = "UpdateNotes";
            noteModel.PostControllerName = "ThirdPartyCrewBoard";
            noteModel.PostMethodName = "UpdateNotes";
            return noteModel;
        }
        private List<ThirdPartyBulkerCrewSchedule> GetThirdPartyCrewScheduleViewModelByCrewId(int thirdPartyBulkerCrewId)
        {
            List<ThirdPartyBulkerCrewSchedule> thirdPartyBulkerCrewSchedules = this._context.GetThirdPartyCrewScheduleByThirdPartyCrewId(thirdPartyBulkerCrewId);
            /*
            List<ThirdPartyBulkerCrewSchedule> newthirdPartyBulkerCrewSchedules=new List<ThirdPartyBulkerCrewSchedule>();
            foreach (var thirdPartyBulkerCrewSchedule in thirdPartyBulkerCrewSchedules)
            {

                ProductHaul productHaul = eServiceWebContext.Instance.GetProductHaulByCrewScheduleId(thirdPartyBulkerCrewSchedule.Id).Find(s=>s.IsThirdParty);
                if (productHaul.IsThirdParty)
                {
                    if (productHaul.ProductHaulLifeStatus==ProductHaulStatus.OnLocation)
                    {
                        continue;
                    }
                }
                newthirdPartyBulkerCrewSchedules.Add(thirdPartyBulkerCrewSchedule);
            }
            */
            return thirdPartyBulkerCrewSchedules.FindAll(s=>s.EndTime>=DateTime.Now).OrderBy(s=>s.EndTime).ToList();
        }

        public ActionResult UpdateThirdPartyCrewNotes(List<string> parms)
        {
            ThirdPartyCrewModel model = new ThirdPartyCrewModel();
            ThirdPartyBulkerCrew thirdPartyBulkerCrew = this._context.GetThirdPartyBulkerCrewById(int.Parse(parms[0]));
            model.PopulateFrom(thirdPartyBulkerCrew);

            return this.PartialView("_UpdateThirdPartyCrewNotes", model);
        }

        [HttpPost]
        public ActionResult UpdateThirdPartyCrewNotes(ThirdPartyCrewModel model)
        {
            ThirdPartyBulkerCrew thirdPartyBulkerCrew = this._context.GetThirdPartyBulkerCrewById(model.Id);
            if (thirdPartyBulkerCrew != null) thirdPartyBulkerCrew.Notes = model.Notes;
            this._context.UpdateThirdPartyBulkerCrew(thirdPartyBulkerCrew);

            return this.RedirectToAction("Index", "ResourceBoard");
        }

        public ActionResult CreateThirdPartyCrew()
        {
            this.GetContractorCompanies();
            this.GetServicePoints();
            return this.PartialView("_CreateThirdPartyCrew");
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


        [HttpPost]
        public ActionResult CreateThirdPartyCrew(ThirdPartyCrewModel model)
        {
            ThirdPartyBulkerCrew thirdPartyBulkerCrew = new ThirdPartyBulkerCrew();
            model.PopulateTo(thirdPartyBulkerCrew);
            if (model.ContractorCompanyId != 0)
            {
                ContractorCompany contractorCompany = this._context.GetContractorCompanyById(model.ContractorCompanyId);
                thirdPartyBulkerCrew.ContractorCompany = contractorCompany;
            }
            else
            {
                thirdPartyBulkerCrew.ContractorCompany = new ContractorCompany{Name = model.ContractorCompanyName};
            }
            this._context.CreateThirdPartyBulkerCrew(thirdPartyBulkerCrew);

            if (!string.IsNullOrEmpty(model.Notes))
            {
                ThirdPartyBulkerCrewNote note=new ThirdPartyBulkerCrewNote
                {
                    Name = model.Notes,
                    Description = model.Notes,
                    ThirdPartyBulkerCrew = thirdPartyBulkerCrew
                };
                this._context.CreateThirdPartyBulkerCrewNote(note);
            }
            return this.RedirectToAction("Index", "ResourceBoard");
        }

        private void GetContractorCompanies()
        {
            List<ContractorCompany> contractorCompanies = this._context.GetAllContractorCompanies().OrderBy(p => p.Name).ToList();
            this.ViewBag.contractorCompanies = contractorCompanies;
        }


        public IActionResult UpdateThirdPartyCrew(List<string> parms)
        {
            ThirdPartyCrewModel model = new ThirdPartyCrewModel();
            this.GetContractorCompanies();
            this.GetServicePoints();
            ThirdPartyBulkerCrew thirdPartyBulkerCrew = this._context.GetThirdPartyBulkerCrewById(int.Parse(parms[0]));
            model.PopulateFrom(thirdPartyBulkerCrew);
            model.Notes = this._context.GeThirdPartyBulkerCrewNoteByThirdPartyBulkerCrew(thirdPartyBulkerCrew.Id)?.Description;
            this.ViewBag.originalcontractorCompany =new {text=thirdPartyBulkerCrew.ContractorCompany.Name, value = thirdPartyBulkerCrew.ContractorCompany.Id};
            return this.PartialView("_UpdateThirdPartyCrew", model);
        }
        [HttpPost]
        public IActionResult UpdateThirdPartyCrew(ThirdPartyCrewModel model)
        {

            ThirdPartyBulkerCrew thirdPartyBulkerCrew = new ThirdPartyBulkerCrew();
            model.PopulateTo(thirdPartyBulkerCrew);
            if (model.ContractorCompanyId != 0)
            {
                ContractorCompany contractorCompany = this._context.GetContractorCompanyById(model.ContractorCompanyId);
                thirdPartyBulkerCrew.ContractorCompany = contractorCompany;
            }
            else
            {
                thirdPartyBulkerCrew.ContractorCompany = new ContractorCompany { Name = model.ContractorCompanyName };
            }
            this._context.UpdateThirdPartyBulkerCrew(thirdPartyBulkerCrew);
            this._context.UpdateThirdPartyBulkerCrewNote(thirdPartyBulkerCrew.Id,model.Notes);
            return this.RedirectToAction("Index", "ResourceBoard");
        }

        public IActionResult RemoveThirdPartyCrew(List<string> parms)
        {
            ThirdPartyCrewModel model=new ThirdPartyCrewModel();
            model.Id = int.Parse(parms[0]);

            return this.PartialView("_RemoveThirdPartyCrew", model);
        }
        [HttpPost]
        public IActionResult RemoveThirdPartyCrew(ThirdPartyCrewModel model)
        {
            this._context.DeleteThirdPartyBulkerCrew(model.Id);
            return this.RedirectToAction("Index", "ResourceBoard");
        }

        public override IActionResult UpdateNotes(NoteModel model)
        {
            eServiceWebContext.Instance.UpdateThirdPartyBulkerCrewNote(model.Id, model.Notes);
            return this.RedirectToAction(model.ReturnActionName, model.ReturnControllerName);
        }
    }
}