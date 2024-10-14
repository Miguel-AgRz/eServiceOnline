using System.Collections.Generic;
using System.Collections.ObjectModel;
using eServiceOnline.Data;
using eServiceOnline.Models.Commons;
using eServiceOnline.Models.PlugLoadingHeadBoard;
using eServiceOnline.Models.TopDriveAdaptorBoard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment;
using Syncfusion.JavaScript;

namespace eServiceOnline.Controllers
{
    public class TopDriveAdaptorBoardController : Controller
    {
        private readonly eServiceWebContext _context;
        public TopDriveAdaptorBoardController()
        {
            this._context = new eServiceWebContext();
        }

        // GET
        public IActionResult Index()
        {
            return
            View();
        }

        public IActionResult GetTopDriveAdaptorModels([FromBody] DataManager dataManager)
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
            List<TopDriveAdaptor> driveAdaptors = this._context.GeTopDriveAdaptorByServicePoint(pageSize, pageNumber, servicePoints, out count);
            List<TopDriveAdaptorVIewModel> data = this.GetTopDriveAdaptorViewModel(driveAdaptors);

            return this.Json(new { result = data, count });
        }

        private List<TopDriveAdaptorVIewModel> GetTopDriveAdaptorViewModel(List<TopDriveAdaptor> topDriveAdaptors)
        {
            List<TopDriveAdaptorVIewModel> data = new List<TopDriveAdaptorVIewModel>();
            foreach (TopDriveAdaptor plugLoadingHead in topDriveAdaptors)
            {
                TopDriveAdaptorVIewModel model = new TopDriveAdaptorVIewModel();

                model.TopDrivceAdaptorInformation = this._context.GetTopDrivceAdaptorInformationByTopDrivceAdaptorId(plugLoadingHead.Id);
                model.PopulateFrom(plugLoadingHead);
                data.Add(model);
            }

            return data;
        }

        public IActionResult UpdateNotes(List<string> parms)
        {
            TopDriveAdaptorModel model = new TopDriveAdaptorModel();
            model.Id = int.Parse(parms[0]);
            model.Notes = this._context.GetTopDrivceAdaptorInformationByTopDrivceAdaptorId(model.Id)?.Notes;
            return PartialView("_UpdateNotes", model);
        }
        [HttpPost]
        public IActionResult UpdateNotes(TopDriveAdaptorModel model)
        {
            this._context.UpdateTopDriveAdaptorNotes(model.Id, model.Notes);
            return RedirectToAction("Index", "ResourceBoard");
        }
    }
}