using System.Collections.Generic;
using System.Collections.ObjectModel;
using eServiceOnline.Data;
using eServiceOnline.Models.Commons;
using eServiceOnline.Models.NubinsBoard;
using eServiceOnline.Models.WitsBoxBoard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment;
using Syncfusion.JavaScript;

namespace eServiceOnline.Controllers
{
    public class WitsBoxBoardController : Controller
    {
        // GET
        private readonly eServiceWebContext _context;
        public WitsBoxBoardController()
        {
            _context = new eServiceWebContext();
        }

        // GET
        public IActionResult GetPageWitsBoxModels([FromBody] DataManager dataManager)
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

            List<WitsBox> witsBoxs = this._context.GetWitsBoxByServicePoint(pageSize, pageNumber, servicePoints, out count);
            List<WitsBoxVIewModel> data = this.GetwitsBoxsVIewModels(witsBoxs);

            return this.Json(new { result = data, count });
        }

        private List<WitsBoxVIewModel> GetwitsBoxsVIewModels(List<WitsBox> witsBoxs)
        {
            List<WitsBoxVIewModel> models = new List<WitsBoxVIewModel>();
            foreach (var witsBox in witsBoxs)
            {
                WitsBoxVIewModel model = new WitsBoxVIewModel();
                model.WitsBoxInformation = this._context.GetWitsBoxInformationByWitsBoxId(witsBox.Id);
                model.PopulateFrom(witsBox);
                models.Add(model);
            }
            return models;
        }
        public IActionResult UpdateNotes(List<string> parms)
        {
            WitsBoxModel model = new WitsBoxModel();
            model.Id = int.Parse(parms[0]);
            model.Notes = this._context.GetWitsBoxInformationByWitsBoxId(model.Id)?.Notes;
            return PartialView("_UpdateNotes", model);
        }
        [HttpPost]
        public IActionResult UpdateNotes(WitsBoxModel model)
        {
            this._context.UpdateWitsBoxNotes(model.Id, model.Notes);
            return RedirectToAction("Index", "ResourceBoard");
        }
    }
}