using System.Collections.Generic;
using System.Linq;
using eServiceOnline.Data;
using eServiceOnline.Models.NubbinBoard;
using eServiceOnline.Models.SwedgeBoard;
using eServiceOnline.Models.WitsBoxBoard;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment;

namespace eServiceOnline.Controllers
{
    public class RigJobController : eServiceOnlineController
    {
        private readonly IeServiceWebContext _context;

        public RigJobController()
        {
            this._context = eServiceWebContext.Instance;
        }

        #region Assign Wits Box

        public IActionResult AssignWitsBox(List<string> parms)
        {
            RigJob rigJob = this._context.GetRigJobById(int.Parse(parms[0]));
            int servicePointId = rigJob.ServicePoint?.Id ?? 0;
            WitsBoxModel witsBoxModel = new WitsBoxModel
            {
                ServicePointId = servicePointId,
                RigJobId = int.Parse(parms[0]),
                CallSheetNumber = int.Parse(parms[1])
            };
            this.ViewBag.WitsBoxList = this.GetWitsBoxListByServicePoint(servicePointId);

            return this.PartialView("_AssignWitsBox", witsBoxModel);
        }

        private List<SelectListItem> GetWitsBoxListByServicePoint(int servicePointId)
        {
            List<WitsBox> witsBoxes = this._context.GetWitsBoxListByServicePoint(servicePointId);
            List<SelectListItem> witsBoxItems = witsBoxes.Select(p => new SelectListItem {Text =$"{p.Name} {p.Id}",Value = p.Id.ToString()}).ToList();

            return witsBoxItems;
        }

        [HttpPost]
        public IActionResult AssignWitsBoxToRigJob(WitsBoxModel model)
        {
            this._context.AssignWitsBoxToRigJob(model);
            return this.RedirectToAction("Index", "RigBoard");
        }

        #endregion

        #region Assign Nubbin

        public IActionResult AssignNubbin(List<string> parms)
        {
            RigJob rigJob = this._context.GetRigJobById(int.Parse(parms[0]));
            int servicePointId = rigJob.ServicePoint?.Id ?? 0;
            NubbinModel nubbinModel = new NubbinModel
            {
                ServicePointId = servicePointId,
                RigJobId = int.Parse(parms[0]),
                CallSheetNumber = int.Parse(parms[1])
            };
            this.ViewBag.NubbinList = this.GetNubbinListByServicePoint(servicePointId);

            return this.PartialView("_AssignNubbin", nubbinModel);
        }

        private List<SelectListItem> GetNubbinListByServicePoint(int servicePointId)
        {
            List<Nubbin> nubbins = this._context.GetEffectiveNubbinsByServicePoint(servicePointId);
            List<SelectListItem> nubbinItems = nubbins.Select(p => new SelectListItem { Text = $"{p.NubbinSize.Name} {p.NubbinThreadType.Name} {p.Id}", Value = p.Id.ToString() }).ToList();

            return nubbinItems;
        }

        [HttpPost]
        public IActionResult AssignNubbinToRigJob(NubbinModel model)
        {
            this._context.AssignNubbinToRigJob(model);
            return this.RedirectToAction("Index", "RigBoard");
        }

        #endregion

        #region Assign Swedge

        public IActionResult AssignSwedge(List<string> parms)
        {
            RigJob rigJob = this._context.GetRigJobById(int.Parse(parms[0]));
            int servicePointId = rigJob.ServicePoint?.Id ?? 0;
            SwedgeModel swedgeModel = new SwedgeModel
            {
                ServicePointId = servicePointId,
                RigJobId = int.Parse(parms[0]),
                CallSheetNumber = int.Parse(parms[1])
            };
            this.ViewBag.SwedgeList = this.GetSwedgeListByServicePoint(servicePointId);

            return this.PartialView("_AssignSwedge", swedgeModel);
        }

        private List<SelectListItem> GetSwedgeListByServicePoint(int servicePointId)
        {
            List<Swedge> swedges = this._context.GetEffectiveSwedgesByServicePoint(servicePointId);
            List<SelectListItem> swedgeItems = swedges.Select(p => new SelectListItem { Text = $"{p.SwedgeSize.Name} {p.SwedgeThreadType.Name} {p.Id}", Value = p.Id.ToString() }).ToList();

            return swedgeItems;
        }

        [HttpPost]
        public IActionResult AssignSwedgeToRigJob(SwedgeModel model)
        {
            this._context.AssignSwedgeToRigJob(model);
            return this.RedirectToAction("Index", "RigBoard");
        }


        #endregion

    }
}