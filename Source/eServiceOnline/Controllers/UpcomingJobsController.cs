using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Reflection;
using eServiceOnline.Data;
using eServiceOnline.Models.Commons;
using eServiceOnline.Models.ProductHaul;
using eServiceOnline.Models.RigBoard;
using eServiceOnline.Models.UpcomingJobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Crew;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule;
using Syncfusion.JavaScript;
using RigJob = Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch.RigJob;
using UnitSection = Sanjel.BusinessEntities.Sections.Common.UnitSection;
namespace eServiceOnline.Controllers
{
    public class UpcomingJobsController : eServicePageController
    {
        private readonly eServiceWebContext _context;
        public static List<RigJobModel> ordersdata = new List<RigJobModel>();

        public UpcomingJobsController()
        {
            this._context = new eServiceWebContext();
        }

        public ActionResult Index(string selectedDistricts = null)
        {
            UpcomingJobsViewModel model = new UpcomingJobsViewModel();
            ViewBag.VersionNumber = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            SetDistrictSelection(selectedDistricts);

            return this.View(model);
        }

        public ActionResult GetPagedRigJobModels([FromBody]DataManager dataManager)
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
            List<Collection<int>> resuhSet = retrieval.ResuhSet(retrieval);
            Collection<int> servicePoints = Utility.GetSearchCollections(resuhSet[0]);

            string windowStartConfiguration = ConfigurationManager.AppSettings["WindowStart"];
            var windowStart = -1;
            if (!string.IsNullOrEmpty(windowStartConfiguration))
                windowStart = int.Parse(windowStartConfiguration);

            string windowEndConfiguration = ConfigurationManager.AppSettings["WindowEnd"];
            var windowEnd = 3;
            if (!string.IsNullOrEmpty(windowEndConfiguration))
                windowEnd = int.Parse(windowEndConfiguration);
            List<RigJob> rigJobs = new List<RigJob>();
            rigJobs = this._context.GetUpComingJobsInformation(pageNumber, pageSize, servicePoints, windowStart, windowEnd, out count);
            List<UpcomingJobsViewModel> data = this.GetUpcomingJobsViewModelList(rigJobs);

            return this.Json(new { result = data, count });
        }

        #region Get UpcomingJobsViewModelList

        private List<UpcomingJobsViewModel> GetUpcomingJobsViewModelList(List<RigJob> rigJobs)
        {
            List<UpcomingJobsViewModel> data = new List<UpcomingJobsViewModel>();
            foreach (RigJob rigJob in rigJobs)
            {
                List<ProductHaul> productHauls = null;
                UpcomingJobsViewModel model = new UpcomingJobsViewModel();
                productHauls = this._context.GetProductHaulCollectionBycallSheetNumber(rigJob.CallSheetNumber);

                model.ProductHaulModels = this.GetProductHaulModels(productHauls);
                model.UnitAndWorkerModels = this.GetUnitAndWorkerModels(rigJob);
                model.PopulateFrom(rigJob);

                data.Add(model);
            }

            return data;
        }

        private List<UnitAndWorkerModel> GetUnitAndWorkerModels(RigJob rigJob)
        {
            List<SanjelCrew> crews = this._context.GetCrewsByRigJob(rigJob);
            List<UnitAndWorkerModel> unitAndWorkerModels = new List<UnitAndWorkerModel>();
            if (crews != null && crews.Count > 0)
            {
                foreach (SanjelCrew sanjelCrew in crews)
                {
                    UnitAndWorkerModel unitAndWorkerModel = new UnitAndWorkerModel();
                    unitAndWorkerModel.PopulateFrom(sanjelCrew.SanjelCrewWorkerSection);
                    unitAndWorkerModel.PopulateFrom(sanjelCrew.SanjelCrewTruckUnitSection);
                    unitAndWorkerModels.Add(unitAndWorkerModel);
                }
            }

            return unitAndWorkerModels;
        }

        private List<ProductHaulModel> GetProductHaulModels(List<ProductHaul> productHauls)
        {
            List<ProductHaulModel> productHaulModels = new List<ProductHaulModel>();
            foreach (ProductHaul productHaul in productHauls)
            {
                ProductHaulModel model = new ProductHaulModel();
                model.PopulateFrom(productHaul);
                productHaulModels.Add(model);
            }

            return productHaulModels;
        }

        private List<UnitSectionModel> GetUnitSectionModels(Collection<Sanjel.BusinessEntities.Sections.Common.UnitSection> unitSections)
        {
            List<UnitSectionModel> unitSectionModels = new List<UnitSectionModel>();
            foreach (UnitSection unitSection in unitSections)
            {
                UnitSectionModel model = new UnitSectionModel();
                model.PopulateFrom(unitSection);
                unitSectionModels.Add(model);
            }

            return unitSectionModels;
        }

        #endregion

        public ActionResult Update([FromBody] CRUDModel<RigJobModel> myObject)
        {
            var ord = myObject.Value;
            RigJobModel val = ordersdata.FirstOrDefault(or => or.CallSheetNumber == ord.CallSheetNumber);
            if (val != null)
            {
                val.CallSheetNumber = ord.CallSheetNumber;
                val.Company = ord.Company;
                val.ConsultantPhone = ord.ConsultantPhone;
                val.Company = ord.Company;
                val.JobType = ord.JobType;
                val.JobDate = ord.JobDate;
                val.WellLocation = ord.WellLocation;
                val.Notes = ord.Notes;
            }
            return Json(myObject.Value);
        }

        public ActionResult Insert([FromBody] CRUDModel<RigJobModel> value)
        {
            ordersdata.Insert(ordersdata.Count, value.Value);
            return Json(ordersdata);
        }

        public ActionResult Delete([FromBody] CRUDModel<RigJobModel> value)
        {
            ordersdata.Remove(ordersdata.FirstOrDefault(or => or.CallSheetNumber == int.Parse(value.Key.ToString())));
            return Json(value);
        }
    }
}