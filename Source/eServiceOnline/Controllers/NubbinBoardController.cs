using System.Collections.Generic;
using System.Collections.ObjectModel;
using eServiceOnline.Data;
using eServiceOnline.Models.Commons;
using eServiceOnline.Models.ManifoldBoard;
using eServiceOnline.Models.NubbinBoard;
using eServiceOnline.Models.NubinsBoard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment;
using Syncfusion.JavaScript;

namespace eServiceOnline.Controllers
{
    public class NubbinBoardController : Controller
    {
        private readonly eServiceWebContext _context;
        public NubbinBoardController()
        {
            _context=new eServiceWebContext();
        }

        // GET
        public IActionResult GetPagNubinsModels([FromBody] DataManager dataManager)
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
            List<Nubbin> nubbins = this._context.GetNubbinsByServicePoint(pageSize, pageNumber, servicePoints, out count);
            
            List<NubbinVIewModel> data = this.GetNubinsVIewModels(nubbins);

            return this.Json(new { result = data,count});
        }

        private List<NubbinVIewModel> GetNubinsVIewModels(List<Nubbin> nubbins)
        {
            List<NubbinVIewModel> data=new List<NubbinVIewModel>();
            foreach (var nubbin in nubbins)
            {
                NubbinVIewModel model=new NubbinVIewModel();
                model.NubbinInformation = this._context.GetNubbinInformationByNubbinId(nubbin.Id);
                model.PopulateFrom(nubbin);
                data.Add(model);
            }

            return data;
        }
        public IActionResult UpdateNotes(List<string> parms)
        {
            NubbinModel model = new NubbinModel();
            model.Id = int.Parse(parms[0]);
            model.Notes = this._context.GetNubbinInformationByNubbinId(model.Id)?.Notes;
            return PartialView("_UpdateNotes", model);
        }
        [HttpPost]
        public IActionResult UpdateNotes(NubbinModel model)
        {
            this._context.UpdateNubbinNotes(model.Id, model.Notes);
            return RedirectToAction("Index", "ResourceBoard");
        }
    }
}