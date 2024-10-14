using System.Collections.Generic;
using System.Collections.ObjectModel;
using eServiceOnline.Data;
using eServiceOnline.Models.BinBoard;
using eServiceOnline.Models.Commons;
using eServiceOnline.Models.PlugLoadingHeadBoard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Sesi.SanjelData.Entities.BusinessEntities.WellSite;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment;
using Syncfusion.JavaScript;

namespace eServiceOnline.Controllers
{
    public class PlugLoadingHeadBoardController : Controller
    {
        private readonly eServiceWebContext _context;
        public PlugLoadingHeadBoardController()
        {
            this._context = new eServiceWebContext();
        }

        // GET
        public IActionResult Index()
        {
            return
            View();
        }

        public IActionResult GetPagePlugLoadingModels([FromBody] DataManager dataManager)
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
            List<PlugLoadingHead> plugLoadingHeads = this._context.GetPlugLoadingHeadsByServicePoint(pageSize, pageNumber, servicePoints, out count);
            List<PlugLoadingHeadViewModel> data = this.GetPlugLoadingHeadViewModel(plugLoadingHeads);

            return this.Json(new { result = data, count });
        }

        private List<PlugLoadingHeadViewModel> GetPlugLoadingHeadViewModel(List<PlugLoadingHead> plugLoadingHeads)
        {
            List<PlugLoadingHeadViewModel> data = new List<PlugLoadingHeadViewModel>();
            foreach (PlugLoadingHead plugLoadingHead in plugLoadingHeads)
            {
                PlugLoadingHeadViewModel model = new PlugLoadingHeadViewModel();

                model.plugLoadingHeadInformation = this._context.GetPlugLoadingHeadInformationByPlugLoadingHeadId(plugLoadingHead.Id);
                model.PopulateFrom(plugLoadingHead);
                data.Add(model);
            }

            return data;
        }

        public IActionResult UpdateNotes(List<string> parms)
        {
            PlugLoadingHeadModel model=new PlugLoadingHeadModel();
            model.Id = int.Parse(parms[0]);
            model.Notes = this._context.GetPlugLoadingHeadInformationByPlugLoadingHeadId(model.Id)?.Notes;
            return PartialView("_UpdateNotes", model);
        }
        [HttpPost]
        public IActionResult UpdateNotes(PlugLoadingHeadModel model)
        {
            this._context.UpdatePlugLoadingHeadNote(model.Id, model.Notes);
            return RedirectToAction("Index", "ResourceBoard");
        }
    }
}