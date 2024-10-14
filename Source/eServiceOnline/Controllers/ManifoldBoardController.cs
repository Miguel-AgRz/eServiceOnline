using System.Collections.Generic;
using System.Collections.ObjectModel;
using eServiceOnline.Data;
using eServiceOnline.Models.Commons;
using eServiceOnline.Models.ManifoldBoard;
using eServiceOnline.Models.PlugLoadingHeadBoard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment;
using Syncfusion.JavaScript;

namespace eServiceOnline.Controllers
{
    public class ManifoldBoardController : Controller
    {
        private readonly eServiceWebContext _context;
        public ManifoldBoardController()
        {
            this._context = new eServiceWebContext();
        }

        // GET
        public IActionResult Index()
        {
            return
            View();
        }

        public IActionResult GetPagManifoldModels([FromBody] DataManager dataManager)
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
            List<Manifold> plugLoadingHeads = this._context.GetManifoldsByServicePoint(pageSize, pageNumber, servicePoints, out count);
            List<ManifoldVIewModel> data = this.GetPlugLoadingHeadViewModel(plugLoadingHeads);

            return this.Json(new { result = data, count });
        }

        private List<ManifoldVIewModel> GetPlugLoadingHeadViewModel(List<Manifold> manifolds)
        {
            List<ManifoldVIewModel> data = new List<ManifoldVIewModel>();
            foreach (Manifold manifold in manifolds)
            {
                ManifoldVIewModel model = new ManifoldVIewModel();

                model.ManifoldInformation = this._context.GetManifoldInformationByManifoldId(manifold.Id);
                model.PopulateFrom(manifold);
                data.Add(model);
            }

            return data;
        }

        public IActionResult UpdateNotes(List<string> parms)
        {
            ManifoldModel model = new ManifoldModel();
            model.Id = int.Parse(parms[0]);
            model.Notes = this._context.GetManifoldInformationByManifoldId(model.Id)?.Notes;
            return PartialView("_UpdateNotes", model);
        }
        [HttpPost]
        public IActionResult UpdateNotes(ManifoldModel model)
        {
            this._context.UpdateManifoldNote(model.Id, model.Notes);
            return RedirectToAction("Index", "ResourceBoard");
        }
    }
}