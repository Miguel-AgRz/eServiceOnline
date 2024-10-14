using System.Collections.Generic;
using System.Collections.ObjectModel;
using eServiceOnline.Data;
using eServiceOnline.Models.Commons;
using eServiceOnline.Models.PlugLoadingHeadSubBoard;
using eServiceOnline.Models.TopDriveAdaptorBoard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment;
using Syncfusion.JavaScript;

namespace eServiceOnline.Controllers
{
    public class PlugLoadingHeadSubBoardController : Controller
    {
        private readonly eServiceWebContext _context;
        public PlugLoadingHeadSubBoardController()
        {
            this._context = new eServiceWebContext();
        }
        // GET
        public IActionResult Index()
        {
            return
            View();
        }

        public IActionResult GetPlugLoadingHeadSubModels([FromBody] DataManager dataManager)
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
            List<PlugLoadingHeadSub> driveAdaptors = this._context.GetPlugLoadingHeadSubsByServicePoint(pageSize, pageNumber, servicePoints, out count);
            List<PlugLoadingHeadSubViewModel> data = this.GetPlugLoadingHeadSubViewModel(driveAdaptors);

            return this.Json(new { result = data, count });
        }

        private List<PlugLoadingHeadSubViewModel> GetPlugLoadingHeadSubViewModel(List<PlugLoadingHeadSub> plugLoadingHeadSubs)
        {
            List<PlugLoadingHeadSubViewModel> data = new List<PlugLoadingHeadSubViewModel>();
            foreach (PlugLoadingHeadSub plugLoadingHeadSub in plugLoadingHeadSubs)
            {
                PlugLoadingHeadSubViewModel model = new PlugLoadingHeadSubViewModel();

                model.PlugLoadingHeadSubInformation = this._context.GetPlugLoadingHeadSubInformationByPlugLoadingHeadSubId(plugLoadingHeadSub.Id);
                model.PopulateFrom(plugLoadingHeadSub);
                data.Add(model);
            }

            return data;
        }
        public IActionResult UpdateNotes(List<string> parms)
        {
            PlugLoadingHeadSubModel model = new PlugLoadingHeadSubModel();
            model.Id = int.Parse(parms[0]);
            model.Notes = this._context.GetPlugLoadingHeadSubInformationByPlugLoadingHeadSubId(model.Id)?.Notes;
            return PartialView("_UpdateNotes", model);
        }
        [HttpPost]
        public IActionResult UpdateNotes(PlugLoadingHeadSubModel model)
        {
            this._context.UpdatePlugLoadingHeadSubNotes(model.Id, model.Notes);
            return RedirectToAction("Index", "ResourceBoard");
        }
    }
}