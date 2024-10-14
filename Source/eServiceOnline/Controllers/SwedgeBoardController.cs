using System.Collections.Generic;
using System.Collections.ObjectModel;
using eServiceOnline.Data;
using eServiceOnline.Models.Commons;
using eServiceOnline.Models.NubinsBoard;
using eServiceOnline.Models.SwedgeBoard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment;
using Syncfusion.JavaScript;

namespace eServiceOnline.Controllers
{
    public class SwedgeBoardController : Controller
    {
        // GET
        private readonly eServiceWebContext _context;
        public SwedgeBoardController()
        {
            _context = new eServiceWebContext();
        }

        // GET
        public IActionResult GetPaSwedgeModels([FromBody] DataManager dataManager)
        {
            int pageSize = dataManager.Take;
            int pageNumber = dataManager.Skip / pageSize + 1;
            int count = 0;

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

            List<Swedge> swedges = this._context.GetSwedgeByServicePoint(pageSize, pageNumber, servicePoints, out count);
            List<SwedgeVIewModel> data = this.GetSwedgeVIewModels(swedges);

            return this.Json(new { result = data, count });
        }

        private List<SwedgeVIewModel> GetSwedgeVIewModels(List<Swedge> swedges)
        {
            List<SwedgeVIewModel> models=new List<SwedgeVIewModel>();
            foreach (var swedge in swedges)
            {
                SwedgeVIewModel model=new SwedgeVIewModel();
                model.SwedgeInformation = this._context.GetSwedgeInformationBySwedgeId(swedge.Id);
                model.PopulateFrom(swedge);
                models.Add(model);
            }
            return models;
        }

        public IActionResult UpdateNotes(List<string> parms)
        {
            SwedgeModel model = new SwedgeModel();
            model.Id = int.Parse(parms[0]);
            model.Notes = this._context.GetSwedgeInformationBySwedgeId(model.Id)?.Notes;
            return PartialView("_UpdateNotes", model);
        }
        [HttpPost]
        public IActionResult UpdateNotes(SwedgeModel model)
        {
            this._context.UpdateSwedgeNotes(model.Id, model.Notes);
            return RedirectToAction("Index", "ResourceBoard");
        }
    }
}