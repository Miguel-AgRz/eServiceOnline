using System.Collections.Generic;
using System.Collections.ObjectModel;
using eServiceOnline.Data;
using eServiceOnline.Models.Commons;
using eServiceOnline.Models.CrewBoard;
using eServiceOnline.Models.UnitBoard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment;
using Syncfusion.JavaScript;

namespace eServiceOnline.Controllers
{
    public class UnitBoardController : eServiceOnlineController
    {
        private readonly eServiceWebContext _context;
        public UnitBoardController()
        {
            this._context = new eServiceWebContext();
        }

        // GET
        public IActionResult GetPageUnitModels([FromBody] DataManager dataManager)
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
            List<TruckUnit> truckUnits = this._context.GetTruckUnitsByServicePoints(pageSize, pageNumber, servicePoints, out count);
            List<UnitViewModel> data = this.GetUnitViewModel(truckUnits);
            return this.Json(new { result = data, count = count });
        }

        private List<UnitViewModel> GetUnitViewModel(List<TruckUnit> truckUnits)
        {
            List<UnitViewModel> data = new List<UnitViewModel>();
            foreach (var unit in truckUnits)
            {
                UnitViewModel unitViewModel = new UnitViewModel();
                unitViewModel.NoteModel = this.GetNoteModel(unit);
                unitViewModel.PopulateFrom(unit);
                data.Add(unitViewModel);
            }
            return data;
        }

        private NoteModel GetNoteModel(TruckUnit truckUnit)
        {
            NoteModel noteModel = new NoteModel();
            TruckUnitNote note = this._context.GetTruckUnitNoteByTruckUnit(truckUnit.Id);
            noteModel.Id = truckUnit.Id;
            noteModel.Notes = note?.Description ?? string.Empty;
            noteModel.ReturnActionName = "Index";
            noteModel.ReturnControllerName = "ResourceBoard";
            noteModel.CallingControllerName = "UnitBoard";
            noteModel.CallingMethodName = "UpdateNotes";
            noteModel.PostControllerName = "UnitBoard";
            noteModel.PostMethodName = "UpdateNotes";
            return noteModel;
        }

        public override IActionResult UpdateNotes(NoteModel model)
        {
            eServiceWebContext.Instance.UpdateTruckUnitNote(model.Id, model.Notes);
            return this.RedirectToAction(model.ReturnActionName, model.ReturnControllerName);
        }

    }
}