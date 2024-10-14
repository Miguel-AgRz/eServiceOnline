using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using eServiceOnline.Data;
using eServiceOnline.Gateway;
using eServiceOnline.Models.BinBoard;
using eServiceOnline.Models.Commons;
using eServiceOnline.Models.UnitBoard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule;
using Sesi.SanjelData.Entities.BusinessEntities.WellSite;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Products;
using Sesi.SanjelData.Entities.Common.BusinessEntities.WellSite;
using Syncfusion.JavaScript;

namespace eServiceOnline.Controllers
{
    public class BinBoardController : eServiceOnlineController
    {
        private readonly eServiceWebContext _context;

        public BinBoardController()
        {
            this._context = new eServiceWebContext();
        }

        public IActionResult GetPageBinModels([FromBody] DataManager dataManager)
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
            List<BinInformation> binInformations = this._context.GetBinInformationsByServicePoint(pageSize, pageNumber, servicePoints, out count);
            List<BinViewModel> data = this.GetBinViewModel(binInformations);

            return this.Json(new { result = data, count });
        }

        private List<BinViewModel> GetBinViewModel(List<BinInformation> binInformations)
        {
            List<BinViewModel> data = new List<BinViewModel>();
            foreach (BinInformation binInformation in binInformations)
            {
                BinViewModel model = new BinViewModel();
                //Nov 30, 2023 zhangyuan 203_PR_UpdateBinNotes: modify Add PodIndex
                model.NoteModel = this.GetNoteModel(binInformation.Bin, binInformation.PodIndex);              
                model.PopulateFrom(binInformation);
                data.Add(model);
            }

            return data;
        }

      

        public List<SelectListItem> GetBinInformationsByBulkPlantId(int bulkPlantId, int binInformationId = 0)
        {
            return GetBinInformationsByRigId(bulkPlantId, binInformationId);
        }
        public List<SelectListItem> GetBinInformationsByRigId(int rigId, int binInformationId = 0)
        {
 
            List<SelectListItem> selectListItems=  this._context.GetBinInformationsByRigId(rigId).OrderBy(s => s.Name)
                .Select(p => new SelectListItem { Text = p.Name, Value = p.Id.ToString(),Selected =p.Id==binInformationId }).ToList();
            return selectListItems;

        }


        public ActionResult GetBinsByBulkPlantId(int bulkPlantId)
        {
            return this.GetBinsByRigId(bulkPlantId);
        }
        //Nov 13, 2023 zhangyuan P63_Q4_174: add TransferBlend 
        public ActionResult GetBinInformationById(int binInformationId)
        {
            BinInformation binInformations = this._context.GetBinInformationById(binInformationId);
            if(binInformations!=null)
                return this.Json(binInformations);
            else
            {
                return null;
            }
        }


        public ActionResult GetBinsByRigId(int rigId)
        {
            if (rigId.Equals(0)) return this.Json(new List<SelectListItem>());

            List<Bin> binInformations = this._context.GetBinCollectionByRig(new Rig() {Id = rigId});

            if (binInformations.Count.Equals(0)) return this.Json(new List<SelectListItem>());

            List<SelectListItem> binList = binInformations.Count.Equals(0)
                ? new List<SelectListItem>()
                : binInformations.Select(p => new SelectListItem {Text = p.Name, Value = p.Id.ToString()}).ToList();
            binList = binList.OrderBy(p => p.Text).ToList();

            return this.Json(binList);
        }

        public ActionResult GetBinsByRigJobId(int rigJobId)
        {
            if (rigJobId.Equals(0)) return this.Json(new List<SelectListItem>());

            var rigJob = this._context.GetRigJobById(rigJobId);
            if (rigJob==null) return this.Json(new List<SelectListItem>());

            List<Bin> binInformations = this._context.GetBinCollectionByRig(new Rig() {Id = rigJob.Rig.Id});

            if (binInformations.Count.Equals(0)) return this.Json(new List<SelectListItem>());

            List<SelectListItem> binList = binInformations.Count.Equals(0)
                ? new List<SelectListItem>()
                : binInformations.Select(p => new SelectListItem {Text = p.Name, Value = p.Id.ToString()}).ToList();
            binList = binList.OrderBy(p => p.Text).ToList();

            return this.Json(binList);
        }

        //Nov 30, 2023 zhangyuan 203_PR_UpdateBinNotes: modify Add PodIndex
        private NoteModel GetNoteModel(Bin bin,int podIndex)
        {
            NoteModel noteModel = new NoteModel();
            BinNote note = this._context.GetBinNoteByBinAndPodIndex(bin, podIndex);
            noteModel.Id = bin.Id;
            noteModel.Notes = note?.Description ?? string.Empty;
            noteModel.PodIndex = podIndex;
            noteModel.ReturnActionName = "Index";
            noteModel.ReturnControllerName = "ResourceBoard";
            noteModel.CallingControllerName = "BinBoard";
            noteModel.CallingMethodName = "UpdateNotes";
            noteModel.PostControllerName = "BinBoard";
            noteModel.PostMethodName = "UpdateNotes";

            return noteModel;
        }


        public override IActionResult UpdateNotes(NoteModel model)
        {
            //Nov 30, 2023 zhangyuan 203_PR_UpdateBinNotes: modify Add PodIndex
            eServiceWebContext.Instance.UpdateBinNote(model.Id, model.Notes, model.PodIndex);
            return this.RedirectToAction(model.ReturnActionName, model.ReturnControllerName);
        }

        public IActionResult UpdateCapacity(List<string> parms)
        {
            BinInformationModel model=new BinInformationModel();
            var binInformation = this._context.GetBinInformationById(Convert.ToInt32(parms[0]));
            model.BinId = binInformation.Bin.Id;
            model.Capacity = binInformation.Capacity;
            return PartialView("_UpdateCapacity", model);
        }
        [HttpPost]
        public IActionResult UpdateCapacity(BinInformationModel model)
        {
            this._context.UpdateCapacity(model.BinId,model.Capacity,model.PodIndex);
            return RedirectToAction("Index","ResourceBoard");

        }
        // Dec 27, 2023 zhangyuan 243_PR_AddBlendDropdown:Add Get blendChemical list
        public void GetBlendInfo()
        {
            List<BlendChemical> blendChemicals = new List<BlendChemical>(CacheData.BlendChemicals).FindAll(p=>p.BaseBlendType.Id>0);
            List<SelectListItem> baseBlend = blendChemicals.Select(p => new SelectListItem { Text = p.Name, Value = p.Id.ToString() }).OrderBy(s=>s.Text).ToList();
            this.ViewData["baseBlend"] = baseBlend;
        }

        public IActionResult UpdateQuantity(List<string> parms)
        {
            BinInformationModel model=new BinInformationModel();
            var binInformation = this._context.GetBinInformationById(Convert.ToInt32(parms[0])); 
            ViewBag.isBulkPlant = parms[1].ToLower().Equals("bulkplant");

            model.BinId = binInformation.Bin.Id;
            model.Quantity = binInformation.Quantity;
            model.Blend = binInformation.BlendChemical.Description;
            // Dec 27, 2023 zhangyuan 243_PR_AddBlendDropdown:Add Get blendId Model
            if (ViewBag.isBulkPlant)
            {
                model.LastProductHaulLoadId = binInformation.LastProductHaulLoadId;
                model.BlendId = binInformation.BlendChemical.Id;
                if (string.IsNullOrEmpty(model.Blend)|| model.LastProductHaulLoadId==0)
                {
                    GetBlendInfo();
                }
            }

            model.PodIndex = binInformation.PodIndex;
            return PartialView("_UpdateQuantity",model);
        }
        [HttpPost]
        public IActionResult UpdateQuantity(BinInformationModel model)
        {
            this._context.UpdateQuantity(model.BinId,model.Quantity, model.Description, LoggedUser,model.PodIndex, model.BlendId);
            // Jan 10, 2024 zhangyuan 243_PR_AddBlendDropdown: modify fix Redirect
            string url = Request.Headers["Referer"].ToString();
            return Redirect(url);

        }
        public IActionResult UpdateBlend(List<string> parms)
        {
            BinInformationModel model = new BinInformationModel();
            var binInformation= this._context.GetBinInformationById(Convert.ToInt32(parms[0]));
            model.BinId = binInformation.Bin.Id;
            model.BlendId = binInformation.BlendChemical!=null? binInformation.BlendChemical.Id:0;
            ViewBag.BlendList = this._context.BlendChemicals.OrderBy(s=>s.Name).ToList();
            return PartialView("_UpdateBlend", model);
        }
        [HttpPost]
        public IActionResult UpdateBlend(BinInformationModel model)
        {
            this._context.UpdateBinformationBlend(model.BinId, model.BlendId, LoggedUser,model.PodIndex);
            return RedirectToAction("Index", "ResourceBoard");

        }
        public ActionResult GetBinById(int binId)
        {
            if (binId.Equals(0) || binId.Equals(0)) return this.Json(new List<SelectListItem>());

            Bin bin = eServiceWebContext.Instance.GetBinById(binId);

            return this.Json(bin);
        }

    }
}
