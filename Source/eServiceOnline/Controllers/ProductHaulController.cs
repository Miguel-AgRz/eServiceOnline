using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using eServiceOnline.Data;
using eServiceOnline.Gateway;
using eServiceOnline.Models.ProductHaul;
using eServiceOnline.SecurityControl;
using eServiceOnline.BusinessProcess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Crew;
using Syncfusion.JavaScript;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule;
using Sesi.SanjelData.Entities.Common.BusinessEntities.HumanResources;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Organization;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch;
using Sesi.SanjelData.Entities.BusinessEntities.WellSite;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Operation;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Products;
using CallSheet = Sanjel.BusinessEntities.CallSheets.CallSheet;
using Rig = Sesi.SanjelData.Entities.Common.BusinessEntities.WellSite.Rig;
using TruckUnit = Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment.TruckUnit;
using Bin = Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment.Bin;
using JobType = Sanjel.Common.BusinessEntities.Lookup.JobType;
using BlendSection = Sesi.SanjelData.Entities.BusinessEntities.BaseEntites.BlendSection;
using MetaShare.Common.Core.CommonService;
using System.Configuration;
using System.IO;
using Sesi.SanjelData.Entities.BusinessEntities.BulkPlant;
using System.Globalization;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using eServiceOnline.PrintingService.Reports.MTSData.Model;

namespace eServiceOnline.Controllers
{
    public class ProductHaulController : eServiceOnlineController
    {
        private readonly eServiceWebContext _context;
        public static List<ProductHaulModel> ordersdata = new List<ProductHaulModel>();
        public const string Ascending = "ascending";
        public const string Descending = "descending";
        private IMemoryCache _memoryCache;
        private const string PrintFileType = ".pdf";


        public ProductHaulController(IMemoryCache memoryCache)
        {
            this._context = new eServiceWebContext();
            _memoryCache = memoryCache;
        }

        public ActionResult Index(int pointId)
        {
            this.GeTruckUnits();
            this.GetEmployees();
            this.ViewBag.HighLight = "ProductHaul";
            ViewBag.VersionNumber = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            ProductHaulViewModel model = new ProductHaulViewModel();
            model.DistrictList = this._context.GetDistrictsWithAll();
            model.DistrictModel = new DistrictModel() { Id = pointId };
            this.ViewBag.HavePermission = SecurityUtility.HavePermission("ProductHaul_Form_CRUD_Control", LoggedUser, _memoryCache);

            return View(model);
        }

        public ActionResult GetPagedProductHaulLoads([FromBody] DataManager dataManager, int servicePointId)
        {
            DateTime startDateTime = DateTime.Now;
            int pageSize = dataManager.Take;
            int pageNumber = dataManager.Skip/pageSize + 1;
            int count = 180;
            List<ProductHaulLoadModel> data = eServiceWebContext.Instance.GetProductHaulPageInfo(0, servicePointId, pageNumber, pageSize, out count);

            if (dataManager.Sorted != null && dataManager.Sorted.Count.Equals(1))
            {
                Sort sort = dataManager.Sorted.First();
                data = Utility.Sort<ProductHaulLoadModel>(data, sort.Direction, sort.Name);
            }

            Debug.WriteLine("\t\tGetPagedProductHauls - {0,21}", DateTime.Now.Subtract(startDateTime));
            return this.Json(new {result = data, count = count});
        }

        public ActionResult GetProductLoadSectionModels([FromBody] DataManager dataManager)
        {
            DateTime startDateTime = DateTime.Now;

            if (dataManager.Where == null || dataManager.Where.Count == 0) throw new Exception("dataManager.Where can not be null.");
            WhereFilter whereFilter = dataManager.Where.Find(p => p.Field.Equals("ProductHaulLoadId"));

            if (whereFilter == null) throw new Exception("whereFilter can not be null.");
            int productHaulLoadId = Int32.Parse(whereFilter.value.ToString());
            List<ProductLoadSection> productLoadSections = this._context.GetProductLoadSectionCollectionByProductLoadId(productHaulLoadId);
            List<ProductLoadSectionModel> data = Utility.CovertFromEntityCollectionToModelCollection<ProductLoadSection, ProductLoadSectionModel>(new List<ProductLoadSection>(productLoadSections));

            Debug.WriteLine("\t\tGetProductLoadSectionModels - {0,21}", DateTime.Now.Subtract(startDateTime));
            return this.Json(new {result = data, count = data.Count});
        }

        public ActionResult Update([FromBody] CRUDModel<ProductHaulModel> myObject)
        {
            var ord = myObject.Value;
            ProductHaulModel val = ordersdata.FirstOrDefault(or => or.CallSheetNumber == ord.CallSheetNumber);
            val.CallSheetNumber = ord.CallSheetNumber;
            val.Category = ord.Category;
            val.BaseBlend = ord.BaseBlend;
            val.Amount = ord.Amount;
            val.MixWater = ord.MixWater;
            val.Comments = ord.Comments;

            val.SackWeight = ord.SackWeight;
            val.Yield = ord.Yield;
            val.BulkVolume = ord.BulkVolume;
            val.Density = ord.Density;

            return this.Json(myObject.Value);
        }

        public ActionResult CancelProductHaul(List<string> parms)
        {
            CancelProductViewModel model = new CancelProductViewModel();
            model.ProductHaulId = Convert.ToInt32(parms[0]);
            var productHaul = eServiceOnlineGateway.Instance.GetProductHaulById(model.ProductHaulId);
            List<CheckShipingLoadSheetModel> CheckShipingLoadSheetModels= new List<CheckShipingLoadSheetModel>();
            foreach(ShippingLoadSheet shippingLoadSheet in productHaul.ShippingLoadSheets)
            {
                CheckShipingLoadSheetModels.Add(new CheckShipingLoadSheetModel() { 
                 IsChecked=true,
                 IsReadOnly=true,
                 ShippingLoadSheetModel= shippingLoadSheet
                });
            }
            model.CheckShipingLoadSheetModels = CheckShipingLoadSheetModels;
            return this.PartialView("../ProductHaul/_CancelProductHaul", model);
        }

        

        private OnLocationProductHaulViewModel GetShippingLoadsInfo(string productHaulId)
        {
            OnLocationProductHaulViewModel model = new OnLocationProductHaulViewModel() {ProductHaulId = Convert.ToInt32(productHaulId) };
            model.ShippingLoadSheetModels  =eServiceOnlineGateway.Instance.GetShippingLoadSheetByProductHaulId(model.ProductHaulId);
            if (model.ShippingLoadSheetModels != null && model.ShippingLoadSheetModels.Count > 0)
            {
                bool sameLocation = true;
                string firstLocation = string.Empty;
                foreach (ShippingLoadSheet item in model.ShippingLoadSheetModels)
                {
                    if (item != null)
                    {
                        if (string.IsNullOrEmpty(firstLocation))
                        {
                            firstLocation = item.Destination;
                        }
                        else
                        {
                            if (!string.Equals(firstLocation, item.Destination))
                                sameLocation = false;
                        }
                    }
                }
                model.IsSameLocation =sameLocation;
                model.OnLocationTime = DateTime.Now;
            }
            return model;
        }

        [HttpPost]
        public ActionResult CancelProductHaul(CancelProductViewModel model)
        {
            model.LoggedUser = this.LoggedUser;
            if (model.ProductHaulId > 0)
            {
                this._context.DeleteProductHaul(model);
            }
            string url = Request.Headers["Referer"].ToString();
            return Redirect(url);
            
        }

        public ActionResult CancelProductHaulLoad(List<string> parms)
        {
            CancelProductHualLoadViewModel model=new CancelProductHualLoadViewModel() { ProductHaulLoadId=Convert.ToInt32(parms[0])};
            return this.PartialView("../ProductHaul/_CancelProductHaulLoad", model);
        }

        [HttpPost]
        public ActionResult CancelProductHaulLoad(CancelProductHualLoadViewModel model)
        {

            if (model.ProductHaulLoadId > 0)
            {
                this._context.DeleteProductHaulLoadById(model.ProductHaulLoadId,this.LoggedUser);
            }

            //return this.RedirectToAction("Index", "RigBoard");
            string url = Request.Headers["Referer"].ToString();
            return Redirect(url);
        }

        public ActionResult CancelBlendRequest(List<string> parms)
        {
            CancelBlendViewModel model = new CancelBlendViewModel();
            model.ProductHaulId = Convert.ToInt32(parms[1]);
            model.ProductHaulLoadId=Convert.ToInt32(parms[0]);
  
            return this.PartialView("../ProductHaul/_CancelBlendRequest", model);
        }
        [HttpPost]
        public ActionResult CancelBlendRequest(CancelBlendViewModel model)
        {
            model.LoggedUser = this.LoggedUser;
            if (model.ProductHaulLoadId > 0)
            {
                this._context.DeleteProductHaulLoadById(model.ProductHaulLoadId, this.LoggedUser);
            }

            return this.RedirectToAction("Index", "RigBoard");
       
        }

        public ActionResult ScheduleProductHaulFromRigJobBlend(List<string> parms)
        {
            ScheduleProductHaulFromRigJobBlendViewModel model = new ScheduleProductHaulFromRigJobBlendViewModel();
            List<BlendSectionModel> blendSectionModels = new List<BlendSectionModel>();           
            BlendSectionModel blendSectionModel = new BlendSectionModel();
            var rigId = Convert.ToInt32(Convert.ToInt32(parms[3]));
            var blendSection = this._context.GetBlendSectionByBlendSectionId(Convert.ToInt32(parms[2]));
            model.ProductLoadInfoModel.IsBlendTest = blendSection.IsNeedFieldTesting;
            blendSectionModel.PopulateFrom(blendSection);
            blendSectionModels.Add(blendSectionModel);
            model.ProductLoadInfoModel.CallSheetNumber = Convert.ToInt32(parms[0]);
            model.RigJobId = Convert.ToInt32(Convert.ToInt32(parms[4]));
            model.ProductLoadInfoModel.RigId = rigId;
            
            model.ProductLoadInfoModel.MixWater = blendSectionModel.MixWater;
            model.ProductHaulInfoModel.ExpectedOnLocationTime = DateTime.Now;
            model.ProductHaulInfoModel.EstimatedLoadTime = DateTime.Now;
            this.ViewBag.BlendSectionList = blendSectionModels;
            this.ViewBag.CallSheetId = Convert.ToInt32(parms[1]);
            var rig=EServiceReferenceData.Data.RigCollection.FirstOrDefault(p=>p.Id== rigId);
            var binInformations = eServiceOnlineGateway.Instance.GetBinInformationsByRigId(rigId);

            model.PopulateFromShippingLoadSheet(rig, binInformations);

            model.PopulateFromHaul();
            
            List<Rig> rigs = new List<Rig>();
            rigs.Add(rig);
            ViewBag.AllRigs = rigs;

            return this.PartialView("../ProductHaul/_ScheduleProductHaulFromRigJobBlend", model);
        }
        public List<SelectListItem> GetExistingProductHauls(int productHaulId = 0)
        {
            return eServiceWebContext.Instance.GetExistingProductHaulCollection().OrderByDescending(a => a.EstimatedLoadTime)
                .Select(p => new SelectListItem { Text = p.Name, Value = p.Id.ToString(),Selected =p.Id==productHaulId }).ToList();
        }

        [HttpPost]
        public ActionResult ScheduleProductHaulFromRigJobBlend(ScheduleProductHaulFromRigJobBlendViewModel model)
        {
            model.LoggedUser = this.LoggedUser;

            this._context.ScheduleProductHaul(model);

            return this.RedirectToAction("Index", "RigBoard");
        }
        //Nov 28, 2023 zhangyuan 201_PR_AddingDifferrentBlend: Get BlendDescription details
        public JsonResult GetOriginBlendSectionDescription(int sectionId, int callSheetId)
        {
            var blendDescription = "";
            var blendSection = callSheetId > 0
                ? this._context.GetBlendSectionByBlendSectionId(sectionId)
                : this._context.GetProgramBlendSectionByBlendSectionId(sectionId);
            BlendChemical blendChemical = null;
            BlendCategory blendCategory = null;
            if (blendSection != null && blendSection.Id > 0)
                (blendChemical, blendCategory) = this._context.GetBlendChemicalByBlendSection(blendSection);
            blendDescription = blendChemical?.Description;
            return Json(blendDescription);
        }


        public List<SelectListItem> GetSanjelCrews(int crewId=0)
        {
            List<SelectListItem> selectListItems = RigBoardProcess.GetSanjeBulkerCrew().OrderBy(s => s.Name)
                .Select(p => new SelectListItem { Text = p.Description, Value = p.Id.ToString(),Selected =p.Id==crewId }).ToList();

//            ViewData["sanjelCrewsList"] = selectListItems;
            return selectListItems;
        }
        public ActionResult SetSanjelCrews(int crewId=0)
        {
            List<SelectListItem> selectListItems = eServiceWebContext.Instance.GetSanjelCrews().OrderBy(s => s.Name)
                .Select(p => new SelectListItem { Text = p.Description, Value = p.Id.ToString(),Selected =p.Id==crewId }).ToList();

            ViewData["sanjelCrewsList"] = selectListItems;
            ViewData["isStatic"]=false;
            return PartialView("_SanjelCrew");
        }


        private List<ThirdPartyBulkerCrew> GetThirdPartyBulkerCrew(int crewId=0)
        {
            List<SelectListItem> selectListItems = eServiceWebContext.Instance.GetThirdPartyCrews().OrderBy(s => s.Name)
                .Select(p => new SelectListItem { Text = p.Description, Value = p.Id.ToString(),Selected =p.Id==crewId }).ToList();
            ViewData["thirdPartyCrewsList"] = selectListItems;
            return null;
        }

        public List<SelectListItem> GetBulkPlants(int bulkPlantId=0)
        {
            List<SelectListItem> selectListItems=  eServiceOnlineGateway.Instance.GetBulkPlants().OrderBy(s => s.Name)
                .Select(p => new SelectListItem { Text = p.Name, Value = p.Id.ToString(),Selected =p.Id==bulkPlantId }).ToList();
//            ViewData["bulkPlantSelectList"] = selectListItems;
            return selectListItems;
        }

        public void GetBinAssigements(int rigId)
        {
            List<BinInformation> binAssignments =
                eServiceWebContext.Instance.GetBinInformationCollectionByRig(new Rig{Id = rigId});
            List<SelectListItem> binseSelectList = new List<SelectListItem>();
//            binseSelectList.Add(new SelectListItem(){Text = "None",Value=""});
            binseSelectList.AddRange(binAssignments.Select(p => new SelectListItem {Text = p.Bin.Name, Value = p.Bin.Id.ToString()}).ToList());
            ViewData["binseSelectList"] = binseSelectList;
        }

        private List<BinInformation> GetBinInformations(int rigId,int binId, int podIndex = 1)
        {
            if(binId==0 || rigId==0)
            {
                ViewData["LoadBinSelectLists"] = new List<SelectListItem>();
            }
            else
                ViewData["LoadBinSelectLists"] = eServiceWebContext.Instance.GetBinInformationsByRigId(rigId).OrderBy(s => s.Name)
                .Select(p => new SelectListItem { Text = p.Name, Value = p.Id.ToString(),Selected =p.Bin.Id==binId && p.PodIndex==podIndex }).ToList();

            return null;
        }

        public List<Employee> GetEmployees()
        {
            List<Employee> employeeList = this._context.GetEmployeeList().OrderBy(a => a.LastName).ToList();
            List<SelectListItem> employeeItems = employeeList.Select(p => new SelectListItem { Text = p.PreferedFirstName, Value = p.Id.ToString() }).ToList();
            ViewData["employeeItems"] = employeeItems;

            return employeeList;
        }

        public void GeTruckUnits()
        {
            List<TruckUnit> truckUnits = CacheData.TruckUnits.OrderBy(a => a.UnitNumber).ToList();
            List<SelectListItem> truckUnitItems = truckUnits.Select(p => new SelectListItem { Text = p.UnitNumber, Value = p.Id.ToString() }).ToList();
            ViewData["truckUnitItems"] = truckUnitItems;
        }

        public void SupplierCompanies()
        {
            List<ContractorCompany> contractorCompanies = eServiceWebContext.Instance.GetAllContractorCompanies().OrderBy(a => a.Name).ToList();
            List<SelectListItem> contractorCompany = contractorCompanies.Select(p => new SelectListItem { Text = p.Name, Value = p.Id.ToString() }).ToList();
            ViewData["contractorCompany"] = contractorCompany;
        }

        private string GetTheDisplayDate(int productHaulId)
        {
            DateTime dateTime = DateTime.MaxValue;
            List<ProductHaulLoad> productHaulLoads = eServiceWebContext.Instance.GetProductHaulLoadCollectionByProductHaulId(productHaulId);
            foreach (ProductHaulLoad item in productHaulLoads)
            {
                dateTime = dateTime > item.ExpectedOnLocationTime ? item.ExpectedOnLocationTime : dateTime;
            }

            return dateTime.ToString("MMM d,H:mm");
        }

        public ActionResult VerifyAmountAndMixWater(ProductHaulModel model)
        {
            string validateStr = ProductHaulValidation.ValidateAmountAndMixWater(model.CallSheetNumber, model.BaseBlendSectionId, model.Amount, model.MixWater, model.IsTotalBlendTonnage, model.ProductHaulLoadId, model.ProgramNumber);
            if (!validateStr.Equals("Pass"))
            {
                if (validateStr.Equals("MixWaterIsRequired"))
                {
                    return this.Json("Required");
                }
                return this.Json(false);
            }

            return this.Json(true);
        }


        private void CreateProductHaulCompanyWithThridPartyCrew(ProductHaulModel model)
        {
            if (model.ProductHaulInfoModel.ThirdPartyBulkerCrewId!=0)
            {
                ThirdPartyBulkerCrew thirdPartyBulkerCrew = eServiceWebContext.Instance.GetThirdPartyBulkerCrewById(model.ProductHaulInfoModel.ThirdPartyBulkerCrewId);                
                if (thirdPartyBulkerCrew != null) model.SupplierCompanyId = thirdPartyBulkerCrew.ContractorCompany.Id;
                model.SupplierCompanyName = thirdPartyBulkerCrew?.ContractorCompany.Name;
                model.ThirdPartyUnitNumber = thirdPartyBulkerCrew?.ThirdPartyUnitNumber;
                model.SupplierContactName = thirdPartyBulkerCrew?.SupplierContactName;
                model.SupplierContactNumber = thirdPartyBulkerCrew?.SupplierContactNumber;
            }
        }

        private void CreateProductHaulEmployeeWithCrew(ProductHaulModel model)
        {
            if (model.ProductHaulInfoModel.CrewId!=0)
            {
                if (!model.ProductHaulInfoModel.IsThirdParty)
                {
                 List<SanjelCrewWorkerSection>  crewWorkerSections=  eServiceWebContext.Instance.GetCrewWorkerSections(model.ProductHaulInfoModel.CrewId);
                 model.DriverFName = crewWorkerSections.FirstOrDefault()?.Worker?.FirstName;
                 model.DriverMName = crewWorkerSections.FirstOrDefault()?.Worker?.MiddleName;
                 model.DriverLName = crewWorkerSections.FirstOrDefault()?.Worker?.LastName;
                 model.Driver2FName = crewWorkerSections.ElementAtOrDefault(1)?.Worker?.FirstName;
                 model.Driver2MName = crewWorkerSections.ElementAtOrDefault(1)?.Worker?.MiddleName;
                 model.Driver2LName = crewWorkerSections.ElementAtOrDefault(1)?.Worker?.LastName;
                }
                else
                {
                    //TODO thrid party
                }
                
            }
        }

        public ActionResult GetBlendSectionModelCollectionByCallSheetNumber(int callSheetNumber)
        {
            CallSheet callSheet = this._context.GetCallSheetByNumber(callSheetNumber);

            if (callSheet != null)
            {
                List<BlendSectionModel> blendSectionModelList = this._context.GetBlendSectionsFromCallSheetId(callSheet.Id);

                return this.Json(blendSectionModelList);
            }

            return this.Json("null");
        }

        public ActionResult GetBlendChemicalCollection()
        {
            return this.Json(this._context.BaseBlendChemicals);
        }

        public ActionResult GetBlendSectionModelCollectionByCallSheetId(int callSheet)
        {
            List<BlendSectionModel> blendSectionModelList = this._context.GetBlendSectionsFromCallSheetId(callSheet);

            return this.Json(blendSectionModelList);
        }

        private bool IsExistCallSheet(int callSheetNumber)
        {
            var callSheet = this._context.GetCallSheetByNumber(callSheetNumber);
            return callSheet != null;
        }

        public ActionResult NeedHaul(List<string> parms)
        {
            try
            {
                int id = Int32.Parse(parms[0]);
                bool isNeedHaul=Boolean.Parse(parms[1]);
                Sanjel.BusinessEntities.Sections.Common.BlendSection blendSection=eServiceWebContext.Instance.GetBlendSectionById(id);

                blendSection.IsNeedHaul = isNeedHaul;
                eServiceWebContext.Instance.UpdateBlendSection(blendSection, blendSection);

                return this.Json(true);
            }
            catch
            {
                return this.Json(false);
            }
        }

        public ActionResult OnLocationProductHaul(List<string> parms)
        {
            OnLocationProductHaulViewModel model = this.GetShippingLoadsInfo(parms[0]);

            return this.PartialView("../ProductHaul/_OnLocationProdHaul", model);
        }

        public ActionResult PrintMTS(List<string> parms)
        {
	        int productHaulId = Int32.Parse(parms[0]);
	        string url = "/producthaul/printproducthaulpdf?producthaulid=" + productHaulId;
	        return this.Json(url);
        }

        /*public ActionResult PrintMTS(List<string> parms)
        {

	        int productHaulId =Int32.Parse(parms[0]);

	        string printOutFilePath = ConfigurationManager.AppSettings["printOutFilePath"];

	        if (!Directory.Exists(printOutFilePath))
	        {
		        Directory.CreateDirectory(printOutFilePath);
	        }

	        if (productHaulId == 0)
	        {
		        return null;
	        }

	        ProductHaul productHaul = this._context.GetProductHaulById(productHaulId);

	        if (productHaul == null) return null;

	        string fileName = "";

	        try
	        {
		        List<MTSModel> list = CreateMTSModelByproductHaul(productHaul);

		        MtsItemsModel model = new MtsItemsModel(list);

		        if (!System.IO.File.Exists(printOutFilePath))
		        {
			        fileName = productHaul.Id + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".pdf";

			        string modelJson = JsonConvert.SerializeObject(model);

			        string basePath = AppDomain.CurrentDomain.BaseDirectory;
			        string xmlLayoutFilePath = $"{basePath}Reports\\MTSData\\Option\\MTSData.xml";
			        string cssStyleFilePath = $"{basePath}Reports\\MTSData\\Option\\MTSData.css";
			        string outputPath = $"{printOutFilePath}\\{fileName}";

			        sesi.SanjelLibrary.PrintingLibrary.Services.PrintingService.PrintFrom(modelJson, xmlLayoutFilePath, cssStyleFilePath, outputPath);

//			        return new FileContentResult(myfile, "application/pdf");

			        return File(System.IO.File.ReadAllBytes(outputPath), "application/pdf");
		        }
	        }
	        catch (Exception ex)
	        {

	        }

	        return null;
        }*/
        [HttpPost]
        public ActionResult OnLocationProductHaul(OnLocationProductHaulViewModel model)
        {
            model.LoggedUser = LoggedUser;
            eServiceWebContext.Instance.UpdateProductHaulOnLocation(model);
            //eServiceWebContext.Instance.UpdateProductHaulAndLoadsOnLocation(Convert.ToInt32(model.ProductHaulId), model.OnLocationTime, LoggedUser);

            string url = Request.Headers["Referer"].ToString();
            return Redirect(url);
        }

        public ActionResult OnLocationProdHaulLoad(List<string> parms)
        {
            //ProductHaulModel model = new ProductHaulModel();
            //ProductHaulLoad productHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(Convert.ToInt32(parms[0]));
            //model.PopulateFromHaulLoad(productHaulLoad);
            //BlendSection blendSection = this._context.GetBlendSectionById(productHaulLoad.BlendSectionId);
            //this.ViewBag.BlendSection = blendSection;
            //this.ViewBag.ShowTime = Convert.ToDateTime(model.OnLocationTime).ToString("MM/dd/yyyy HH:mm");

            int productHaulId = Convert.ToInt32(parms[1]);
            int productHualLoadId = Convert.ToInt32(parms[0]);
            OnLocationProductHaulLoadViewModel model=new OnLocationProductHaulLoadViewModel();
            model.ProductHaulLoadId = productHualLoadId;
            var shippingLoadSheet = eServiceOnlineGateway.Instance.GetShippingLoadSheetByProductHaulId(productHaulId).FirstOrDefault(p=>p.ProductHaulLoad.Id==productHualLoadId);
            var productHaulLoad=eServiceOnlineGateway.Instance.GetProductHaulLoadById(model.ProductHaulLoadId);
            model.OnLocationTime = DateTime.Now;
           
            model.ShippingLoadSheetId = shippingLoadSheet.Id;
            model.ProductHaulLoadModel = productHaulLoad;
            model.ShippingLoadSheetModel = shippingLoadSheet;

            BlendSection blendSection = this._context.GetBlendSectionByBlendSectionId(productHaulLoad.BlendSectionId);
            this.ViewBag.BlendSection = blendSection;

            return this.PartialView("../ProductHaul/_OnLocationProdHaulLoad", model);
        }

        [HttpPost]
        public ActionResult OnLocationProdHaulLoad(OnLocationProductHaulLoadViewModel model)
        {
            //ProductHaulLoad productHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(model.ProductHaulLoadId);
            //eServiceWebContext.Instance.UpdateProductHaulLoadOnLocation(productHaulLoad, model.OnLocationTime, LoggedUser);

            var shippingLoadSheet = eServiceOnlineGateway.Instance.GetShippingLoadSheetById(model.ShippingLoadSheetId);
            if (shippingLoadSheet != null)
            {
                shippingLoadSheet.ShippingStatus = ShippingStatus.OnLocation;
                shippingLoadSheet.OnLocationTime = model.OnLocationTime;
                eServiceOnlineGateway.Instance.UpdateShippingLoadSheet(shippingLoadSheet);

                var productHaul = eServiceOnlineGateway.Instance.GetProductHaulById(shippingLoadSheet.ProductHaul.Id);
                bool allOnlocation = true;
                foreach (var item in productHaul.ShippingLoadSheets)
                {
                    if (item.ShippingStatus != ShippingStatus.OnLocation)
                    {
                        allOnlocation = false;
                    }
                }
                if (allOnlocation)
                {
                    productHaul.ExpectedOnLocationTime = model.OnLocationTime;
                    productHaul.ProductHaulLifeStatus = ProductHaulStatus.OnLocation;
                    //eServiceOnlineGateway.Instance.UpdateProductHaul(productHaul,false);
                    ProductHaulProcess.SetProductHaulOnLocation(productHaul.Id, model.OnLocationTime, model.LoggedUser);
                }
                var productHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(model.ProductHaulLoadId);

                List<int> ids = new List<int>();
                ids.Add(productHaulLoad.Id);
                var shippingLoadSheetsAll = eServiceOnlineGateway.Instance.GetShippingLoadSheetByProductHaulLoads(ids).FindAll(p => p.ShippingStatus == ShippingStatus.Scheduled);
                if (productHaulLoad.BlendShippingStatus == BlendShippingStatus.HaulScheduled && shippingLoadSheetsAll.Count == 0)
                {
                    productHaulLoad.BlendShippingStatus = BlendShippingStatus.OnLocation;
                    productHaulLoad.ProductHaulLoadLifeStatus = ProductHaulLoadStatus.OnLocation;
                    productHaulLoad.ModifiedUserName = this.LoggedUser;
                    productHaulLoad.OnLocationTime = model.OnLocationTime;
                    eServiceOnlineGateway.Instance.UpdateProductHaulLoad(productHaulLoad, false);
                }

            }

            string url = Request.Headers["Referer"].ToString();
            return Redirect(url);
        }

        public ActionResult RescheduleProductHaul(List<string> parms)
        {
            var productHaulId=Convert.ToInt32(parms[0]);
            var productHaul=eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);
            RescheduleProductHaulViewModel model = new RescheduleProductHaulViewModel();
            model.RigJobId= Convert.ToInt32(parms[2]);
            model.PopulateFromHaul(productHaul);
            model.OriginalProductHaulId = model.ProductHaulInfoModel.ProductHaulId;
            //model.ProductHaulInfoModel.EstimatedLoadTime = DateTime.Now;
            //model.ProductHaulInfoModel.ExpectedOlTime = DateTime.Now;
            //Jan 12, 2024 tongtao 257_PR_EstimatedLoadTimeExpectedOnLocationTimeUpdateBug: Get EstimatedLoadTime and  ExpectedOlTime from DataBase
            model.ProductHaulInfoModel.EstimatedLoadTime = productHaul.EstimatedLoadTime;
            model.ProductHaulInfoModel.ExpectedOlTime = productHaul.ExpectedOnLocationTime;

            this.GetCrewName(model.ProductHaulInfoModel);
            List<PodLoadAndBendUnLoadModel> podLoadAndBendUnLoadModels = new List<PodLoadAndBendUnLoadModel>();
            var podLoads = productHaul.PodLoad;
            podLoads.ForEach(p => p.LoadAmount = p.LoadAmount / 1000 );

            foreach (var shippingLoadSheetItem in productHaul.ShippingLoadSheets)
            {
                var shippingLoadSheet = eServiceOnlineGateway.Instance.GetShippingLoadSheetById(shippingLoadSheetItem.Id,true);
                //Nov 1, 2023 AW P45_Q4_105: Prevent the null exception when the shipping load  sheet is not linked to a blend request
                var productHaulLoad =  (shippingLoadSheet.ProductHaulLoad != null && shippingLoadSheet.ProductHaulLoad.Id != 0) ? eServiceOnlineGateway.Instance.GetProductHaulLoadById(shippingLoadSheet.ProductHaulLoad.Id) :null;

                PodLoadAndBendUnLoadModel podLoadAndBendUnLoadModel = new PodLoadAndBendUnLoadModel();
                podLoadAndBendUnLoadModel.IsCheckShippingLoadSheet = true;
                var unloadSheet = shippingLoadSheet.BlendUnloadSheets.FirstOrDefault();
                podLoadAndBendUnLoadModel.RigId=unloadSheet==null?0:(unloadSheet.DestinationStorage==null?0:unloadSheet.DestinationStorage.Rig?.Id ?? 0);
                //Dec 11, 2023 zhangyuan 224_PR_ShowCallSheetList: Add Reschedule Load Rig
                if (podLoadAndBendUnLoadModel.RigId == 0 && shippingLoadSheet.Rig?.Id != 0)
                {
                    podLoadAndBendUnLoadModel.RigId = shippingLoadSheet.Rig?.Id??0;
                }

                podLoadAndBendUnLoadModel.FromRigBulkPlant = shippingLoadSheet.BulkPlant?.Name;
                podLoadAndBendUnLoadModel.FromBin = shippingLoadSheet.SourceStorage?.Name;

                podLoadAndBendUnLoadModel.RigName=shippingLoadSheet.Rig?.Name;
                podLoadAndBendUnLoadModel.LoadAmount=shippingLoadSheet.LoadAmount/1000;
                //Dec 7, 2023 zhangyuan 224_PR_ShowCallSheetList: Modify show ProgramId And Version
                //Jan 25, 2023 Tongtao 273_PR_UnloadChangeHasErrorAfterRigChangeRig: when ProgramId is empty or null,not add ProgramVersion,only return null
                podLoadAndBendUnLoadModel.ProgramId = productHaulLoad == null || string.IsNullOrEmpty(productHaulLoad.ProgramId)? null:productHaulLoad.ProgramId + "." + productHaulLoad.ProgramVersion.ToString("D2");
                podLoadAndBendUnLoadModel.CallSheetNumber = shippingLoadSheet.CallSheetNumber;
                podLoadAndBendUnLoadModel.ShippingLoadSheetId = shippingLoadSheet.Id;
                podLoadAndBendUnLoadModel.Blend =shippingLoadSheet.BlendDescription;
                podLoadAndBendUnLoadModel.IsGoWithCrew = shippingLoadSheet.IsGoWithCrew;

                podLoadAndBendUnLoadModel.BlendUnloadSheetModels = new List<BlendUnloadSheet>();

                //Feb 05, 2023 tongtao 279_PR_CouldNotChangeCallSheet: check ProductHaul is RigJobBlend
                if (string.IsNullOrEmpty(shippingLoadSheet.ProgramId) && (shippingLoadSheet.SourceStorage.Id == 0))
                {
                    podLoadAndBendUnLoadModel.IsRigJobBlend = true;
                }
                else
                {
                    podLoadAndBendUnLoadModel.IsRigJobBlend = false;
                }

                List<BinInformation> binInformations;

                //Nov 1, 2023 AW P45_Q4_105: Fix bug eService_Items#108, Reschedule Product Haul not show all bins
                //Nov 28, 2023 Tongtao P45_Q4_108: When there is no data in the BlendUnloadSheet table, obtain the original Bin information corresponding to the rig for show on the page
                if (podLoadAndBendUnLoadModel.RigId != 0)
                {
                    binInformations =
                       eServiceOnlineGateway.Instance.GetBinInformationsByRigId(podLoadAndBendUnLoadModel.RigId);
                }
                else
                {
                    binInformations =
                       eServiceOnlineGateway.Instance.GetBinInformationsByRigId(shippingLoadSheet.Rig.Id);
                }

                foreach (var item in binInformations)
                {
                    var existingBlendUnloadSheet =
                        shippingLoadSheet.BlendUnloadSheets.Find(p => p.DestinationStorage.Id == item.Id);
                    if (existingBlendUnloadSheet != null)
                    {
                        existingBlendUnloadSheet.UnloadAmount /= 1000;
                        podLoadAndBendUnLoadModel.BlendUnloadSheetModels.Add(existingBlendUnloadSheet);
                    }
                    else
                        podLoadAndBendUnLoadModel.BlendUnloadSheetModels.Add(new BlendUnloadSheet()
                        {
                            UnloadAmount = 0,
                            DestinationStorage = item
                        });
                }

                List<PodLoad> shippingPodLoads = new List<PodLoad>();
                foreach (var podLoad in podLoads)
                {
	                var shippingPodLoad = new PodLoad();
	                shippingPodLoad.PodIndex = podLoad.PodIndex;
	                shippingPodLoad.ProductHaul = podLoad.ProductHaul;
	                shippingPodLoad.LoadAmount = podLoad.LoadAmount;
	                shippingPodLoad.LoadAmountUnit = podLoad.LoadAmountUnit;
	                shippingPodLoad.BaseTonnage = podLoad.BaseTonnage;
	                shippingPodLoad.BaseTonnageUnit = podLoad.BaseTonnageUnit;
	                if (podLoad.ShippingLoadSheet == null || podLoad.ShippingLoadSheet.Id == 0)
	                {
		                shippingPodLoad.ShippingLoadSheet = shippingLoadSheet;
	                }
	                else
	                {
		                shippingPodLoad.ShippingLoadSheet = podLoad.ShippingLoadSheet;
	                }
                    shippingPodLoads.Add(shippingPodLoad);
                }
                podLoadAndBendUnLoadModel.PodLoadModels = shippingPodLoads;

                //Dec 7, 2023 zhangyuan 224_PR_ShowCallSheetList: add Load CallSheetList
                //Nov 7, 2023 AW develop: Get call sheet number list by program id
                if (productHaulLoad != null)
                {
                    var baseBlendName = GetBaseBlendName(productHaulLoad.BlendChemical.Description);
                    podLoadAndBendUnLoadModel.CallSheetNumbers =
                        GetCallSheetNumberByBaseBlend(baseBlendName, productHaulLoad.ProgramId);
                }
                else
                {
                    //Dec 11, 2023 zhangyuan 224_PR_ShowCallSheetList:  If there is no productHaulLoad record,Retrieve only from ShippingLoadSheet(Blend In Bin)
                    var baseBlendName = GetBaseBlendName(shippingLoadSheet.BlendDescription);
                    podLoadAndBendUnLoadModel.CallSheetNumbers = GetCallSheetNumberByBaseBlend(baseBlendName);
                }
                //Dec 11, 2023 zhangyuan 224_PR_ShowCallSheetList: Rigjob Bin column  Call Sheet Number Maybe Null
                if (podLoadAndBendUnLoadModel.CallSheetNumber>0&&!podLoadAndBendUnLoadModel.CallSheetNumbers.Exists(p=>p.Value== podLoadAndBendUnLoadModel.CallSheetNumber.ToString()))
                {
                    podLoadAndBendUnLoadModel.CallSheetNumbers = new List<SelectListItem>();
                    podLoadAndBendUnLoadModel.CallSheetNumbers.Add(new SelectListItem()
                    {
                        Text = podLoadAndBendUnLoadModel.CallSheetNumber.ToString(),
                        Value = podLoadAndBendUnLoadModel.CallSheetNumber.ToString(),
                        Selected = true
                    });
                }
                podLoadAndBendUnLoadModels.Add(podLoadAndBendUnLoadModel);
            }
            model.PodLoadAndBendUnLoadModels = podLoadAndBendUnLoadModels;
            //Dec 7, 2023 zhangyuan 224_PR_ShowCallSheetList: add Load Rigs
            //jan 25, 2024 tongtao 274_PR_RigCouldNotChange:Insert a blank value into Rig collection
            Rig emptyRig = new Rig();
            EServiceReferenceData.Data.RigCollection.Insert(0, emptyRig);
            ViewBag.AllRigs = EServiceReferenceData.Data.RigCollection.OrderBy(p => p.Name);
            return this.PartialView("../ProductHaul/_RescheduleProductHaul", model);
        }

        public JsonResult VerifyCrewSchedule(int crewId, DateTime estimatedLoadTime, double duration, bool isThirdParty, bool isGoWithCrew,int rigJobId)
        {
            DateTime endTime = this._context.GetRigJobAssginCrewEndTime(rigJobId);
            if (isGoWithCrew)
            {
                if (endTime==DateTime.MinValue)
                {
                    endTime = estimatedLoadTime.AddHours(8);
                }
            }
            else
            {
                endTime = estimatedLoadTime.AddHours(duration);
            }
            DateTime startTime = estimatedLoadTime;
            string messageInfo = isThirdParty ? eServiceWebContext.Instance.VerifyThirdPartyBulkerCrewSchedule(crewId, startTime, endTime) : eServiceWebContext.Instance.VerifySanjelCrewSchedule(crewId, startTime, endTime);
          
            return this.Json(messageInfo);
        }

        [HttpPost]
        public ActionResult RescheduleProductHaul1(RescheduleProductHaulViewModel model)
        {
	        //change product haul

	        string userName = this.LoggedUser;
	        eServiceWebContext.Instance.RescheduleProductHaul1(model, model.RigJobId, userName);
	        string url = Request.Headers["Referer"].ToString();
	        return Redirect(url);
        }

        public ActionResult GetBlendSectionById(List<string> parms)
        {
            BlendSection blendSection = eServiceWebContext.Instance.GetBlendSectionByBlendSectionId(Convert.ToInt32(parms[0]));
            BlendSectionModel model = new BlendSectionModel();
            model.PopulateFrom(blendSection);

            return this.PartialView("../RigBoard/_UpdateTheBlend", model);
        }

        public ActionResult UpdateTheBlend(BlendSectionModel model)
        {
            Sanjel.BusinessEntities.Sections.Common.BlendSection blendSection = eServiceWebContext.Instance.GetBlendSectionById(model.Id);
            Sanjel.BusinessEntities.Sections.Common.BlendSection newBlendSection = blendSection;
            newBlendSection.Quantity = model.Amount;
            eServiceWebContext.Instance.UpdateBlendSection(newBlendSection, blendSection);

            return this.RedirectToAction("Index", "RigBoard");
        }
        //TODO: Not used any more, clean up after confirmed.

        private void GetCrewName(ProductHaulInfoModel model)
        {
            if (model.IsThirdParty)
            {
                RigJobThirdPartyBulkerCrewSection crewSection =
                    eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionsByProductHual(model.ProductHaulId);

                model.OrigThirdPartyBulkerCrewId = model.ThirdPartyBulkerCrewId = crewSection.ThirdPartyBulkerCrew.Id;
            }
            else
            {
                RigJobSanjelCrewSection crewSection =
                    eServiceOnlineGateway.Instance.GetRigJobCrewSectionsByProductHaul(model.ProductHaulId);
                if (crewSection != null)
                {
                    model.OrigCrewId = model.CrewId = crewSection.SanjelCrew.Id;
                }
            }
        }

        public ActionResult RescheduleProductHaulLoad(List<string> parms)
        {
            int productHaulLoadId=Convert.ToInt32(parms[0]);
            int rigId=Convert.ToInt32(parms[1]);

            RescheduleProductHaulLoadViewModel model = new RescheduleProductHaulLoadViewModel();
            model.RigJobId = Convert.ToInt32(parms[3]);

            var rig = eServiceWebContext.Instance.GetRigInfoByRigId(rigId);
            
            var productHaulLoad = eServiceWebContext.Instance.GetProductHaulLoadById(productHaulLoadId);

            model.ProductLoadInfoModel.PopulateFromHaulLoad(productHaulLoad);

            model.OrigBinInformationId =model.ProductLoadInfoModel.BinInformationId;
            model.OrigBulkPlantId = model.ProductLoadInfoModel.BulkPlantId;

            model.ProductHaulLoadId = productHaulLoadId;
            List<BlendSectionModel> blendSectionModels = new List<BlendSectionModel>();
            BlendSectionModel blendSectionModel = new BlendSectionModel();

            var blendSection = this._context.GetBlendSectionByBlendSectionId(productHaulLoad.BlendSectionId);

            blendSectionModel.PopulateFrom(blendSection);
            blendSectionModels.Add(blendSectionModel);
            this.ViewBag.BlendSectionList = blendSectionModels;

            List<Rig> rigs = new List<Rig>();
            rigs.Add(rig);
            ViewBag.AllRigs = rigs;
            return this.PartialView("../ProductHaul/_RescheduleProductHaulLoad", model);
        }

        public ActionResult RescheduleBlendFromRigJobBlend(List<string> parms)
        {
            int productHualLoadId = Convert.ToInt32(parms[0]);
            var productHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productHualLoadId);
            ReScheduleBlendFromRigJobBlendViewModel model = new ReScheduleBlendFromRigJobBlendViewModel();
            model.ProductLoadInfoModel.PopulateFromHaulLoad(productHaulLoad);
            if (model.ProductLoadInfoModel.BinId != 0)
            {
                var binInformation =
                    eServiceWebContext.Instance.GetBinInformationByBinId(model.ProductLoadInfoModel.BinId, model.ProductLoadInfoModel.PodIndex);
                if (binInformation != null)
                {
                    model.OrigBinInformationId = binInformation.Id;
                    model.ProductLoadInfoModel.BinInformationId = binInformation.Id;
                    model.ProductLoadInfoModel.BinInformationName = binInformation.Name;
                }
            }
            model.ProductHaulInfoModel.EstimatedLoadTime = DateTime.Now;
            model.OrigBulkPlantId = model.ProductLoadInfoModel.BulkPlantId;
            model.ProductHaulLoadId = model.ProductLoadInfoModel.ProductHaulLoadId;

            return this.PartialView("../ProductHaul/_RescheduleBlendFromRigJobBlend", model);
        }


        public List<SelectListItem> GetThirdPartyBulkerCrewList(int crewId)
        {
            return eServiceOnlineGateway.Instance.GetThirdPartyBulkerCrews().OrderBy(s => s.Name)
                .Select(p => new SelectListItem
                    {Text = p.Description, Value = p.Id.ToString(), Selected = p.Id == crewId }).ToList();
        }

        public List<SelectListItem> GetSanjelBulkerCrewList(int crewId, DateTime onLocationTime)
        {
            return eServiceOnlineGateway.Instance.GetPlannedBulkerCrewList(onLocationTime).OrderBy(s => s.Name)
                .Select(p => new SelectListItem { Text = p.Description, Value = p.Id.ToString(),Selected =p.Id==crewId }).ToList();
        }

        [HttpPost]
        public ActionResult RescheduleProductHaulLoad(RescheduleProductHaulLoadViewModel model)
        {
            //eServiceWebContext.Instance.RescheduleProductHaulLoad(model);

            ProductHaulLoad originalProductHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(model.ProductHaulLoadId);
            if (originalProductHaulLoad == null) throw new Exception("product haul load id is not passed in or passed wrongly.");

            var UnLoad =originalProductHaulLoad.TotalBlendWeight- originalProductHaulLoad.RemainsAmount;
            bool isBlendRequirementChanged = false;
            if (model.ProductLoadInfoModel.IsTotalBlendTonnage != originalProductHaulLoad.IsTotalBlendTonnage || (model.ProductLoadInfoModel.IsTotalBlendTonnage && Math.Abs(model.ProductLoadInfoModel.Amount * 1000 - originalProductHaulLoad.TotalBlendWeight) > 0.1) || (!model.ProductLoadInfoModel.IsTotalBlendTonnage && Math.Abs(model.ProductLoadInfoModel.Amount * 1000 - originalProductHaulLoad.BaseBlendWeight) > 0.1))
            {
                originalProductHaulLoad.IsTotalBlendTonnage = model.ProductLoadInfoModel.IsTotalBlendTonnage;
                if (originalProductHaulLoad.IsTotalBlendTonnage)
                {
                    originalProductHaulLoad.TotalBlendWeight = model.ProductLoadInfoModel.Amount * 1000;
                }
                else
                {
                    originalProductHaulLoad.BaseBlendWeight = model.ProductLoadInfoModel.Amount * 1000;
                }
                isBlendRequirementChanged = true;
            }
            if (isBlendRequirementChanged)
            {
                BlendSection blendSection = null;
                if (originalProductHaulLoad.CallSheetNumber > 0)
                {
                    blendSection = this._context.GetBlendSectionByBlendSectionId(originalProductHaulLoad.BlendSectionId);
                }
                else
                {
                    blendSection = this._context.GetProgramBlendSectionByBlendSectionId(originalProductHaulLoad.BlendSectionId);
                }
                BlendChemical blendChemical = null;
                BlendCategory blendCategory = null;
                (blendChemical, blendCategory) = this._context.GetBlendChemicalByBlendSection(blendSection);
                originalProductHaulLoad = this._context.GetCalculatedProductHaulLoad(blendChemical, originalProductHaulLoad,
                    originalProductHaulLoad.IsTotalBlendTonnage);
            }
            if (Math.Abs(model.ProductLoadInfoModel.MixWater - originalProductHaulLoad.MixWater) > 0.001)
            {
                originalProductHaulLoad.MixWater = model.ProductLoadInfoModel.MixWater;
            }
            originalProductHaulLoad.BulkPlant = eServiceOnlineGateway.Instance.GetRigById(model.ProductLoadInfoModel.BulkPlantId);
            if (model.ProductLoadInfoModel.BinInformationId == 0)
            {
                originalProductHaulLoad.Bin = new Bin();
                originalProductHaulLoad.PodIndex = 0;
            }
            else
            {
                var binInformation = eServiceOnlineGateway.Instance.GetBinInformationById(model.ProductLoadInfoModel.BinInformationId);
                if (binInformation == null) throw new Exception("Bin information doesn't exist.");
                if (originalProductHaulLoad.Bin.Id != binInformation.Bin.Id || originalProductHaulLoad.PodIndex != binInformation.PodIndex)
                {
                    originalProductHaulLoad.Bin = binInformation.Bin;
                    originalProductHaulLoad.PodIndex = binInformation.PodIndex;
                }
            }
            originalProductHaulLoad.RemainsAmount = originalProductHaulLoad.TotalBlendWeight - UnLoad;
            originalProductHaulLoad.Comments = model.ProductLoadInfoModel.Comments;

            eServiceOnlineGateway.Instance.UpdateProductHaulLoad(originalProductHaulLoad);

            return this.RedirectToAction("Index", "RigBoard");
        }

        private string GetJobType(string programId,int jobTypeId)
        {
            string jobTypeName = "";
            var jobDesign = eServiceOnlineGateway.Instance.GetJobDesignByProgramId(programId);
            if (jobDesign != null)
            {
                if (jobDesign.JobDesignPumpingJobSection != null && jobDesign.JobDesignPumpingJobSection.Count > 0)
                {
                    jobTypeName = jobDesign.JobDesignPumpingJobSection.FirstOrDefault(x => x.JobType.Id == jobTypeId)?.Name;
                    /*
                    foreach (var programPumpingJobSection in jobDesign.JobDesignPumpingJobSection)
                    {
                        if(programPumpingJobSection.JobType.Id==jobTypeId)
                        {
                            jobTypeName = programPumpingJobSection.JobType.Name;
                        }
                    }
                */
                }
            }
            return jobTypeName;
        }
        public ActionResult RescheduleBlendFromBulkPlantBin(List<string> parms)
        {
            RescheduleBlendFromBulkPlantBinModel model = new RescheduleBlendFromBulkPlantBinModel();
            this.GetBulkPlants();
            var productHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(Convert.ToInt32(parms[0]));
            model.ProductLoadInfoModel.PopulateFromHaulLoad(productHaulLoad);
            model.ProductHaulLoadId = productHaulLoad.Id;
            model.ProductLoadInfoModel.BinInformationId = Convert.ToInt32(parms[1]);
            model.ProductLoadInfoModel.BinInformationName = parms[2];

            return this.PartialView("../ProductHaul/_RescheduleBlendFromBulkPlantBin", model);
            
        }
        public ActionResult RescheduleBlendFromRigBin(List<string> parms)
        {
            RescheduleBlendFromBulkPlantBinModel model = new RescheduleBlendFromBulkPlantBinModel();
            this.GetBulkPlants();

            var productHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(Convert.ToInt32(parms[0]));
            model.ProductLoadInfoModel.PopulateFromHaulLoad(productHaulLoad);
            model.ProductHaulLoadId = productHaulLoad.Id;
      

            return this.PartialView("../ProductHaul/_RescheduleBlendFromRigBin", model);

        }
        public ActionResult RescheduleProductHaulFromRigBin(List<string> parms)
        {
            ProductHaulModel model = eServiceWebContext.Instance.GetProductHaulModelByProductHaulLoadId(Convert.ToInt32(parms[0]));
   
            model.OrigBulkPlantId = model.BulkPlantId;
            model.BinInformationId = Convert.ToInt32(parms[1]);
            model.BinInformationName = parms[2];
            if (model.ProductHaulId != 0)
                model.ProductHaulInfoModel.IsExistingHaul = true;
            model.OriginalProductHaulId = model.ProductHaulId;
            // model.JobTypeName = this.GetJobType(model.ProgramNumber, model.JobTypeId);

            this.ViewBag.ProgramList = new List<Program>();
            this.ViewBag.BlendSectionList = new List<BlendSectionModel>();
            this.ViewData["binSelectList"] = new List<SelectListItem>();
            this.ViewBag.JobTypeList = new List<JobType>();

            return this.PartialView("../ProductHaul/_RescheduleBlendFromRigBin", model);

        }
        [HttpPost]
        public ActionResult RescheduleBlendFromBulkPlantBin(RescheduleBlendFromBulkPlantBinModel model)
        {

            eServiceWebContext.Instance.RescheduleBlendRequest(model.ProductLoadInfoModel);

            string url = Request.Headers["Referer"].ToString();
            return Redirect(url);
        }
        [HttpPost]
        public ActionResult RescheduleBlendFromRigBin(ProductHaulModel model)
        {

            eServiceWebContext.Instance.RescheduleBlendRequest(model.ProductLoadInfoModel,true);

            string url = Request.Headers["Referer"].ToString();
            return Redirect(url);
        }
        
        public JsonResult VerifyQualityOfTheBin(int binInformationId,double amount, int blendSectionId, int callSheetNumber, string binNumber)
        {
            string result = string.Empty;
            if (binInformationId==0 && string.IsNullOrEmpty(binNumber)) return new JsonResult(result);
            BinInformation binInformation = this._context.GetBinInformationById(binInformationId);
            List<ProductHaulLoad> productHaulLoads = this._context.GetProductHaulLoadByBinId(binInformation.Bin.Id, binInformation.PodIndex).FindAll(s=>s.ProductHaulLoadLifeStatus!=ProductHaulLoadStatus.OnLocation);
            double loadQuantity = 0;
            BlendChemical blendChemical = this._context.GetBlendChemicalByBlendSectionId(blendSectionId, callSheetNumber);
            foreach (var productHaulLoad in productHaulLoads)
            {
                loadQuantity += productHaulLoad.IsTotalBlendTonnage ? productHaulLoad.TotalBlendWeight/1000 : productHaulLoad.BaseBlendWeight/1000;
                //TODO: Verify blend being scheduled against the blend on the way to the bin.
            }
            if (binInformation!=null)
            {
                bool isEmptyBin = binInformation.BlendChemical == null || (string.IsNullOrEmpty(binInformation.BlendChemical.Name) && string.IsNullOrEmpty(binInformation.BlendChemical.Description));
                bool isSameChemical = isEmptyBin ||  (!string.IsNullOrEmpty(binInformation.BlendChemical.Description) && binInformation.BlendChemical.Description.Equals(blendChemical.Description)) || (!string.IsNullOrEmpty(binInformation.BlendChemical.Name) && binInformation.BlendChemical.Name.Equals(blendChemical.Name));
                string remainingCapacity =
                    $"Bin {binInformation.Name} remaining loadable capacity is {binInformation.Capacity - binInformation.Quantity}t.";
                string onRoad = loadQuantity > 0 ? $"<br />{loadQuantity} t on the way, " : "";
                string binLoaded = binInformation.Quantity > 0 ? $"<br />{binInformation.Quantity}t in the storage." : "";
                string scheduledAmount = $"<br />Currently scheduled {amount}t. ";
                string willContinue = "<br />Do you want to continue the operation?";
                result = isSameChemical == false
                    ? $"Alert: You are loading different blend to Bin {binInformation.Name}. <br />Blend in bin: {binInformation.BlendChemical.Description??binInformation.BlendChemical.Name} <br /> You are loading: {blendChemical.Description}<br />":"";
                result = result + (binInformation.Quantity+ loadQuantity + amount > binInformation.Capacity ? "Alert: Bin is overloaded. <br /> " + remainingCapacity + onRoad + binLoaded + scheduledAmount + willContinue : "");
            }
            return new JsonResult(result);
        }
        public ActionResult ScheduleBlend(List<string> parms)
        {
            var callSheetId = Convert.ToInt32(parms[1]);
            this.GetBulkPlants();
            this.ViewBag.ProgramList = new List<Program>();
            this.ViewBag.BlendSectionList = new List<BlendSectionModel>();
            this.ViewData["binSelectList"] = new List<SelectListItem>();
            this.ViewBag.JobTypeList = new List<JobType>();
            int rigId = Convert.ToInt32(parms[4]);
            int binInformationId = Convert.ToInt32(parms[5]);
            int binId = Convert.ToInt32(parms[0]);
            this.GetBinInformations(rigId, binId);
            var binInformation=eServiceOnlineGateway.Instance.GetBinInformationById(binInformationId);

            ProductHaulModel model = new ProductHaulModel();
            model.BinId = binId;
            model.BinNumber = parms[2];
            model.BinInformationId = binInformationId;
            model.BinInformationName = parms[6];
            model.RigId = rigId;
            model.BulkPlantId = rigId;
            model.ProductLoadInfoModel.BinId = binInformation.Bin.Id;
            model.ProductLoadInfoModel.PodIndex = binInformation.PodIndex;
            model.ProductLoadInfoModel.BulkPlantId = rigId;
            model.ProductLoadInfoModel.BulkPlantName=eServiceOnlineGateway.Instance.GetRigById(rigId)?.Name;
            model.ProductLoadInfoModel.RigId = rigId;
            model.ProductLoadInfoModel.BinNumber = model.BinNumber;
            model.ProductLoadInfoModel.BinInformationId =model.BinInformationId;
            model.ProductLoadInfoModel.BinId = model.BinId;
            model.ProductLoadInfoModel.BinInformationName = model.BinInformationName;
            model.LoggedUser = this.LoggedUser;
            model.ProductHaulInfoModel.EstimatedLoadTime = DateTime.Now;
            model.ProductHaulInfoModel.ExpectedOnLocationTime = DateTime.Now;
            
            var rig = eServiceOnlineGateway.Instance.GetRigById(rigId);
            if (rig.OperationSiteType == OperationSiteType.Rig || rig.OperationSiteType == OperationSiteType.ProjectBulkPlant)
            {
                return this.PartialView("../ProductHaul/_ScheduleBlendToRigBin", model);
            }
            else
            {
                model.BulkPlantName = parms[3];
                return this.PartialView("../ProductHaul/_ScheduleBlendToBulkPlantBin", model);
            }
        }
        public ActionResult ScheduleBlendFromRigJobBlend(List<string> parms)
        {
            ScheduleBlendFromRigJobBlendViewModel model = new ScheduleBlendFromRigJobBlendViewModel();

            List<BlendSectionModel> blendSectionModels = new List<BlendSectionModel>();
            BlendSectionModel blendSectionModel = new BlendSectionModel();
            var blendSection = this._context.GetBlendSectionByBlendSectionId(Convert.ToInt32(parms[2]));
            model.ProductLoadInfoModel.IsBlendTest = blendSection.IsNeedFieldTesting;
            blendSectionModel.PopulateFrom(blendSection);
            blendSectionModels.Add(blendSectionModel);
            model.ProductLoadInfoModel.CallSheetNumber = Convert.ToInt32(parms[0]);
            model.RigJobId = Convert.ToInt32(Convert.ToInt32(parms[4]));
            model.ProductLoadInfoModel.RigId = Convert.ToInt32(Convert.ToInt32(parms[3]));

            model.ProductLoadInfoModel.MixWater = blendSectionModel.MixWater;
            model.ProductHaulInfoModel.ExpectedOnLocationTime = DateTime.Now;
            model.ProductHaulInfoModel.EstimatedLoadTime = DateTime.Now;
            this.ViewBag.BlendSectionList = blendSectionModels;
            this.ViewBag.CallSheetId = Convert.ToInt32(parms[1]);



            return this.PartialView("../ProductHaul/_ScheduleBlendFromRigJobBlend", model);

        }
        public ActionResult ScheduleProductHaulToRigBin(List<string> parms)
        {

            ScheduleProductHaulToRigBinViewModel model = new ScheduleProductHaulToRigBinViewModel();
            List<BlendSectionModel> blendSectionModels = new List<BlendSectionModel>();
            BlendSectionModel blendSectionModel = new BlendSectionModel();
            var rigId = Convert.ToInt32(Convert.ToInt32(parms[2]));
            blendSectionModels.Add(blendSectionModel);
            model.RigJobId = Convert.ToInt32(parms[3]);
            model.ProductLoadInfoModel.RigId = rigId;

            model.ProductLoadInfoModel.MixWater = blendSectionModel.MixWater;
            model.ProductHaulInfoModel.ExpectedOnLocationTime = DateTime.Now;
            model.ProductHaulInfoModel.EstimatedLoadTime = DateTime.Now;
            model.ProductLoadInfoModel.ClientRepresentative = parms[9];
            this.ViewBag.BlendSectionList = blendSectionModels;
            this.ViewBag.CallSheetId = Convert.ToInt32(parms[1]);
            int binId=Convert.ToInt32(parms[0]);
            model.OrigBinId = binId;
            var rig = EServiceReferenceData.Data.RigCollection.FirstOrDefault(p => p.Id == rigId);
            var binInformations = eServiceOnlineGateway.Instance.GetBinInformationsByRigId(rigId);

            model.OrigBinInformationId = Convert.ToInt32(parms[6]);
            model.PopulateFromShippingLoadSheet(rig, binInformations);

            model.PopulateFromHaul();

            List<Rig> rigs = new List<Rig>();
            rigs.Add(rig);
            ViewBag.AllRigs = rigs;
            this.ViewBag.JobTypeList = new List<JobType>();

            return this.PartialView("../ProductHaul/_ScheduleProductHaulToRigBin", model);
        }
        [HttpPost]
        public ActionResult ScheduleProductHaulToRigBin(ScheduleProductHaulToRigBinViewModel model)
        {
            model.LoggedUser = this.LoggedUser;

            this._context.ScheduleProductHaul(model);
            return this.RedirectToAction("Index", "RigBoard");
        }
        [HttpPost]
        public ActionResult ScheduleBlendFromRigJobBlend(ScheduleBlendFromRigJobBlendViewModel model)
        {
            model.ProductLoadInfoModel.LoggedUser = this.LoggedUser;

            eServiceWebContext.Instance.CreateBlendRequest(model.ProductLoadInfoModel);

            return this.RedirectToAction("Index", "RigBoard");
        }

        [HttpPost]
        public ActionResult ReScheduleBlendFromRigJobBlend(ReScheduleBlendFromRigJobBlendViewModel model)
        {
            model.ProductLoadInfoModel.LoggedUser = this.LoggedUser;

            eServiceWebContext.Instance.RescheduleBlendRequest(model.ProductLoadInfoModel);

            return this.RedirectToAction("Index", "RigBoard");
        }

        private List<ClientCompany> GetCustomers()
        {
            List<ClientCompany> clientCompanies = eServiceWebContext.Instance.GetClientCompanyInfo();

            this.ViewBag.CustomerList = clientCompanies;
            return clientCompanies;
        }

        [HttpPost]
        public ActionResult ScheduleBlendToBulkPlantBin(ProductHaulModel model)
        {
            model.LoggedUser = this.LoggedUser;
            model.ProductLoadInfoModel.LoggedUser = this.LoggedUser;
            var binInformation = eServiceWebContext.Instance.GetBinInformationById(model.BinInformationId);
            if(binInformation!=null)
            {
                model.PodIndex = binInformation.PodIndex;
            }

            //this._context.CreateProductHaul(model);
            this._context.CreateBlendRequest(model.ProductLoadInfoModel);
            return this.RedirectToAction("Index", "BulkPlant");
        }

        //Nov 13, 2023 zhangyuan P63_Q4_174: modify TransferBlend view page
        public ActionResult TransferBlend(List<string> parms)
        {
            TanseferBlendViewModel model = new TanseferBlendViewModel();

            var binInformationId = Convert.ToInt32(parms[0]);
            BinInformation binInformation =
                eServiceWebContext.Instance.GetBinInformationById(binInformationId);
            model.BinInformationId = binInformationId;
            model.BinInformationName = binInformation.Name;
            model.BlendToLoadDescription = binInformation.BlendChemical.Description;
            model.BlendQuantity = binInformation.Quantity.ToString("##.###");
            model.BulkPlantOrRigName = binInformation.Rig.Name;
            model.BulkPlantOrRigId = binInformation.Rig.Id;

            var binInformations = eServiceWebContext.Instance
                .GetBinInformationsByRigId(model.BulkPlantOrRigId).Where(p => p.Id != binInformationId)
                .OrderBy(s => s.Name);
            this.ViewBag.ToStorageList = binInformations;
            var binCount = binInformations.Count();
            model.IsOnlyOneTransferBin = binCount <= 1 ? true : false;
            if (model.IsOnlyOneTransferBin)
            {
                var ToBinInformation = binInformations.FirstOrDefault();
                if (ToBinInformation != null)
                {

                    model.ToBinInformationId = ToBinInformation.Id;
                    model.ToBinInformationName = ToBinInformation.Name;
                    model.IsSameBlendInBin =
                        binInformation.BlendChemical.Description == ToBinInformation.BlendChemical.Description &&
                        binInformation.BlendChemical.Name == ToBinInformation.BlendChemical.Name;
                    model.BlendInBinDescription = ToBinInformation.BlendChemical.Description;
                    model.IsBinEmpty = Math.Abs(ToBinInformation.Quantity) < 0.001;
                }
                else
                {
                    model.ToBinInformationId = 0;
                    model.ToBinInformationName = "";
                    model.IsSameBlendInBin = true;
                    model.BlendInBinDescription = "";
                    model.IsBinEmpty = true;
                    this.ViewBag.IsNotAnthorBin = true;
                }

            }

            if (parms[1].ToLower().Equals("rigjob"))
            {
                model.ShowPageType = 1;
                return this.PartialView("../ProductHaul/_TransferBlendToRigJobBin", model);
            }
            else if (parms[1].ToLower().Equals("bulkplant"))
            {
                model.ShowPageType = 2;
                return this.PartialView("../ProductHaul/_TransferBlendToBulkPlantBin", model);
            }
            else
            {
                throw new Exception("parms[1] can not be null.");
            }

        }

        // Nov 14, 2023 zhangyuan P63_Q4_174:Add save TransferBlend
     [HttpPost]
     public ActionResult TransferBlend(TanseferBlendViewModel model)
     {
         this._context.TransferBlendToBin(this.LoggedUser, model.ShowPageType, model.BinInformationId, model.ToBinInformationId,model.TransferQuantity);
            string url = Request.Headers["Referer"].ToString();
         return Redirect(url);
     }

     public ActionResult HualBlendFromRigBulkPlantBin(List<string> parms)
     {
         HaulBlendFromBulkPlantBinViewModel model=new HaulBlendFromBulkPlantBinViewModel();

         ViewBag.AllRigs = EServiceReferenceData.Data.RigCollection.OrderBy(p=>p.Name);
         var productHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(Convert.ToInt32(parms[0]));
         var binInformation = eServiceOnlineGateway.Instance.GetBinInformationById(Convert.ToInt32(parms[1]));

         model.ProductLoadInfoModel.PopulateFromHaulLoad(productHaulLoad);
         model.ProductLoadInfoModel.BinInformationName = binInformation.Name;
         model.ProductLoadInfoModel.BinInformationId = binInformation.Id;

         if (model.ProductLoadInfoModel.CallSheetNumber != 0)
         {
             var binInformations = eServiceOnlineGateway.Instance.GetBinInformationsByRigId(productHaulLoad.Rig.Id);
             model.ShippingLoadSheetModel.InitializeShippingLoadSheets(productHaulLoad.Rig, binInformations);
         }


         model.ShippingLoadSheetModel.Quantity = model.ProductLoadInfoModel.RemainsAmount;

         model.ProductHaulInfoModel.InitializeProductHaulSchedule();
         model.ShippingLoadSheetModel.BlendUnloadSheetModels = new List<BlendUnloadSheet>();
         model.RigJobId = Convert.ToInt32(parms[2]);

         //Nov 7, 2023 AW develop: Get call sheet number list by program id
         var baseBlendName = GetBaseBlendName(productHaulLoad.BlendChemical.Description);
            ViewBag.CallSheetNumbers = GetCallSheetNumberByBaseBlend(baseBlendName, productHaulLoad.ProgramId);

         return this.PartialView("../ProductHaul/_HaulBlendFromRigBulkPlantBin", model);
     }

     [HttpPost]
     public ActionResult HaulBlendFromRigBulkPlantBin(HaulBlendFromBulkPlantBinViewModel model)
     {
         model.LoggedUser = this.LoggedUser;
         model.PopulateToModel();
         //this._context.CreateBlendRequest(model.ProductLoadInfoModel);
         this._context.CreateHaulBlend( 0, model.LoggedUser, model.ProductLoadInfoModel, model.ProductHaulInfoModel, model.ShippingLoadSheetModel);

         string url = Request.Headers["Referer"].ToString();
         return Redirect(url);
     }

     public ActionResult LoadBlendToBin(List<string> parms)
     {
         LoadBlendToBinViewModel model = new LoadBlendToBinViewModel();

         var productHaulLoadId = Convert.ToInt32(parms[0]);
         ProductHaulLoad productHaulLoad = eServiceWebContext.Instance.GetProductHaulLoadById(productHaulLoadId);

         if (productHaulLoad == null) return null;
         model.ProductHaulLoadId = productHaulLoad.Id;
         BinInformation binInformation =
             eServiceWebContext.Instance.GetBinInformationByBinId(productHaulLoad.Bin.Id, productHaulLoad.PodIndex);
         model.BinInformationName = binInformation.Name;
         model.BlendInBinDescription = binInformation.BlendChemical.Description;
         model.BlendChemicalDescription = productHaulLoad.BlendChemical.Description;
         model.BlendQuantity = (productHaulLoad.TotalBlendWeight / 1000).ToString("##.###");
         model.BulkPlantName = productHaulLoad.BulkPlant.Name;
         model.IsSameBlendInBin =
             productHaulLoad.BlendChemical.Description == binInformation.BlendChemical.Description;
         model.IsBinEmpty = Math.Abs(binInformation.Quantity) < 0.01;

         return this.PartialView("../ProductHaul/_LoadBlendToBin", model);
     }
     [HttpPost]
     public ActionResult LoadBlendToBin(LoadBlendToBinViewModel model)
     {
         this._context.LoadBlendToBin(this.LoggedUser, model.ProductHaulLoadId);
         string url = Request.Headers["Referer"].ToString();
         return Redirect(url);
     }
     public ActionResult HaulBlendFromBulkPlantBin(List<string> parms) 
     {
	     var productHaulLoadId = Convert.ToInt32(parms[0]);
	     //Nov 3, 2023 AW P45_Q4_105: Disable the logic of Blend in Bin with productHaulLoadId. The id is only for reference for product tracing purpose, the correctness is not guaranteed. Later analysis can help business process optimizaiton
	     //Haul product in the bin without specify the Load Id. Specify everything by user
	     HaulBlendFromBulkPlantBinViewModel model = new HaulBlendFromBulkPlantBinViewModel();
		// Nov 22, 2023 AW P45_Q4_175:Insert a blank value into Rig collection
        Rig emptyRig = new Rig();
        EServiceReferenceData.Data.RigCollection.Insert(0, emptyRig);
        ViewBag.AllRigs = EServiceReferenceData.Data.RigCollection.OrderBy(p => p.Name);

        model.BulkPlantBinLoadModel.BinInformationId = Convert.ToInt32(parms[1]);
         model.BulkPlantBinLoadModel.Amount = Convert.ToDouble(parms[3]);
         model.BulkPlantBinLoadModel.BulkPlantId = Convert.ToInt32(parms[4]);
         model.BulkPlantBinLoadModel.BulkPlantName = parms[5];
         model.BulkPlantBinLoadModel.BlendChemicalDescription = parms[6];
         model.BulkPlantBinLoadModel.BaseBlend = GetBaseBlendName(model.BulkPlantBinLoadModel.BlendChemicalDescription);

         model.BulkPlantBinLoadModel.BinInformationName = parms[7];

         model.ProductHaulInfoModel.InitializeProductHaulSchedule();
         model.ShippingLoadSheetModel.BlendUnloadSheetModels = new List<BlendUnloadSheet>();

         var scheduledShippingLoadSheetByBin =
             eServiceWebContext.Instance.GetScheduledShippingLoadSheetsByStorage(model.BulkPlantBinLoadModel.BinInformationId).Where(p => p.ProductHaulLoad.Id == productHaulLoadId).ToList();

        model.ProductLoadInfoModel.ProductHaulLoadId = productHaulLoadId;
        var totalScheduledAmount = scheduledShippingLoadSheetByBin.Sum(p => p.LoadAmount);
        model.BulkPlantBinLoadModel.RemainsAmount =
            Math.Round(model.BulkPlantBinLoadModel.Amount - totalScheduledAmount / 1000, 3);
        model.ShippingLoadSheetModel.Quantity = model.BulkPlantBinLoadModel.RemainsAmount;


        ViewBag.CallSheetNumbers = GetCallSheetNumberByBaseBlend(model.BulkPlantBinLoadModel.BaseBlend);

        return this.PartialView("../ProductHaul/_HaulBlendFromBulkPlantBin", model);
    }

     private string GetBaseBlendName(string blendChemicalDescription)
     {
	     string pattern = @"(?<Name>[\w\s\d\(\)\*'\-%\/#:]+)($|(\s\+.+))";
	     var matches = Regex.Matches(blendChemicalDescription, pattern);
	     if (matches.Count == 1)
	     {
		     return matches[0].Groups["Name"].Captures[0].Value.Trim();
	     }

	     return string.Empty;
     }

     [HttpPost]
        public ActionResult HaulBlendFromBulkPlantBin(HaulBlendFromBulkPlantBinViewModel model)
        {
            model.LoggedUser = this.LoggedUser;
            model.PopulateToModel();
            //this._context.CreateBlendRequest(model.ProductLoadInfoModel);
            this._context.HaulBlendFromBulkPlantBin(model.LoggedUser, model.ProductLoadInfoModel, model.ProductHaulInfoModel, model.ShippingLoadSheetModel, model.BulkPlantBinLoadModel);

            string url = Request.Headers["Referer"].ToString();
            return Redirect(url);
        }

        public ActionResult HaulBlendFromRigJobBlend(List<string> parms)
        {
            this.ViewBag.JobTypeList = new List<JobType>();
            HaulBlendFromRigJobBlendViewModel model = new HaulBlendFromRigJobBlendViewModel();
            int productHaulLoadId = Convert.ToInt32(parms[0]);
            int rigJobId = Convert.ToInt32(parms[2]);
            int rigId = Convert.ToInt32(parms[3]);
            ProductHaulLoad load = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productHaulLoadId, true);
            BinInformation binInformation =
                eServiceOnlineGateway.Instance.GetBinInformationByBinIdAndPodIndex(load.Bin.Id, load.PodIndex);
            model.RigJobId = rigJobId;
            model.ProductLoadInfoModel.PopulateFromHaulLoad(load);
            model.ProductLoadInfoModel.Amount = model.ProductLoadInfoModel.TotalBlendWeight;
            if (binInformation != null)
            {
                model.ProductLoadInfoModel.BinInformationName = binInformation.Name;
                model.ProductLoadInfoModel.BinInformationId = binInformation.Id;
            }
            
            model.ProductHaulInfoModel.InitializeProductHaulSchedule();

            model.ShippingLoadSheetModel.Quantity = model.ProductLoadInfoModel.RemainsAmount;

            ViewBag.AllRigs = EServiceReferenceData.Data.RigCollection;
            var binInformations=eServiceOnlineGateway.Instance.GetBinInformationsByRigId(rigId);

            model.ShippingLoadSheetModel.InitializeShippingLoadSheets(load.Rig, binInformations);

            return this.PartialView("../ProductHaul/_HaulBlendFromRigJobBlend", model);
        }
        [HttpPost]
        public ActionResult HaulBlendFromRigJobBlend(HaulBlendFromRigJobBlendViewModel model)
        {
            model.LoggedUser = this.LoggedUser;
            model.PopulateToModel();

            eServiceWebContext.Instance.CreateHaulBlend(model.RigJobId,model.LoggedUser, model.ProductLoadInfoModel, model.ProductHaulInfoModel,model.ShippingLoadSheetModel);
            return this.RedirectToAction("Index", "RigBoard");
        }

        [HttpPost]
        public List<PodLoad> GetPodLoadsByProductHaul(int productHualId)
        {
            var list=eServiceOnlineGateway.Instance.GetPodLoadsByProductHaul(productHualId);
            list.ForEach(p=>p.LoadAmount=p.LoadAmount/1000);
            return list;
        }
        [HttpPost]
        public List<string> GetCallSheetNumberByProgramId(string programId)
        {
            var rigJobs = eServiceOnlineGateway.Instance.GetRigJobsByProgramId(programId);
            List<string> list = new List<string>();
            list.Add(string.Empty);
            foreach(var rigJob in rigJobs)
            {
                if (rigJob.CallSheetNumber <= 0 || rigJob.JobLifeStatus == JobLifeStatus.Completed || rigJob.JobLifeStatus==JobLifeStatus.Canceled||rigJob.JobLifeStatus==JobLifeStatus.Deleted) continue;
                list.Add(rigJob.CallSheetNumber.ToString());
            }
            return list;
        }
        [HttpPost]
        public List<string> GetCallSheetNumberByProgramIdAndJobType(string programId, int jobTypeId)
        {
            var rigJobs = eServiceOnlineGateway.Instance.GetRigJobsByProgramId(programId).FindAll(p=>p.JobType.Id==jobTypeId);
            List<string> list = new List<string>();
            list.Add(string.Empty);
            foreach(var rigJob in rigJobs)
            {
                if (rigJob.CallSheetNumber <= 0 || rigJob.JobLifeStatus == JobLifeStatus.Completed || rigJob.JobLifeStatus==JobLifeStatus.Canceled||rigJob.JobLifeStatus==JobLifeStatus.Deleted) continue;
                list.Add(rigJob.CallSheetNumber.ToString());
            }

            return list;
        }

        //Nov 7, 2023 AW develop: Get call sheet number list by program id
        public List<SelectListItem> GetCallSheetNumberByBaseBlend(string baseBlendName, string programId = null)
        {
            var rigJobs =  eServiceOnlineGateway.Instance.GetRigJobsByQuery(null, rigJob=>rigJob.JobLifeStatus == JobLifeStatus.Pending || rigJob.JobLifeStatus == JobLifeStatus.Confirmed || rigJob.JobLifeStatus == JobLifeStatus.Dispatched || rigJob.JobLifeStatus == JobLifeStatus.InProgress || rigJob.JobLifeStatus==JobLifeStatus.Scheduled);

            if (programId != null)
	            rigJobs = rigJobs.FindAll(p => p.ProgramId == programId);

            List<int> callSheetIds = rigJobs.Select(p => p.CallSheetId).Distinct().ToList();

            //Nov 1, 2023 AW P45_Q4_105: Change get blend section method to improve the performance

            /*
            string additivesSuffix = " + Additives";
            string baseBlendNanme = blendChemical.Name.EndsWith(additivesSuffix)
	            ? blendChemical.Name.Substring(0, blendChemical.Name.LastIndexOf(additivesSuffix, StringComparison.Ordinal))
	            : blendChemical.Name;
            */
            var callSheetBlendSections = eServiceWebContext.Instance.GetBlendSectionsByCallSheetIdsAndBlendName(callSheetIds, baseBlendName);

            List<string>  callSheetFilteredIds = callSheetBlendSections.Select(p=>p.CallSheet.Name).Distinct().ToList();

            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem() { Text = "", Value = "", Selected = true });
            foreach (var callSheetFilteredId in callSheetFilteredIds)
            {
	            SelectListItem selectListItem = new SelectListItem()
	            {
		            Text = callSheetFilteredId,
		            Value = callSheetFilteredId,
                    Selected = false
	            };
                selectListItems.Add(selectListItem);
            }
            return selectListItems;
        }

        //jan 23, 2024 tongtao 273_PR_UnloadChangeHasErrorAfterRigChangeRig: when reschedule producthaul,get blend unload amout by shippingLoadSheetId
        [HttpPost]
        public ShippingLoadSheetModel GetShippingLoadSheetByRig(int rigId, int shippingLoadSheetId = 0)
        {
            var rig = eServiceOnlineGateway.Instance.GetRigById(rigId);
            ShippingLoadSheetModel shippingLoadSheetModel = new ShippingLoadSheetModel();

            shippingLoadSheetModel.RigName = rig.Name;
            shippingLoadSheetModel.RigId = rig.Id;

            var binInformations = eServiceOnlineGateway.Instance.GetBinInformationsByRigId(rig.Id);

            shippingLoadSheetModel.BlendUnloadSheetModels = new List<BlendUnloadSheet>();

            if (shippingLoadSheetId > 0)
            {
                var blendUnloadSheets = eServiceOnlineGateway.Instance.GetBlendUnloadSheetByshippingLoadSheetId(shippingLoadSheetId);


                foreach (var item in binInformations)
                {
                    shippingLoadSheetModel.BlendUnloadSheetModels.Add(new BlendUnloadSheet()
                    {
                        UnloadAmount = blendUnloadSheets?.FirstOrDefault(p => p.DestinationStorage.Id == item.Id)?.UnloadAmount / 1000 ?? 0,
                        DestinationStorage = item
                    });

                }
            }
            else
            {
                foreach (var item in binInformations)
                {
                    shippingLoadSheetModel.BlendUnloadSheetModels.Add(new BlendUnloadSheet()
                    {
                        UnloadAmount = 0,
                        DestinationStorage = item
                    });
                }
            }
            return shippingLoadSheetModel;
        }
        [HttpPost]
        public ShippingLoadSheetModel GetShippingLoadSheetByCallSheetNumber(int callSheetNumber)
        {
            RigJob rigJob=eServiceOnlineGateway.Instance.GetRigJobByCallSheetNumber(callSheetNumber);
        
            ShippingLoadSheetModel shippingLoadSheetModel = new ShippingLoadSheetModel();
            shippingLoadSheetModel.RigName = rigJob.Rig.Name;
            shippingLoadSheetModel.RigId = rigJob.Rig.Id;
            shippingLoadSheetModel.ClientName = rigJob.ClientCompany.Name;
            shippingLoadSheetModel.ClientId = rigJob.ClientCompany.Id;
            //Dec 7, 2023 zhangyuan 224_PR_ShowCallSheetList: And return callsheetId
            shippingLoadSheetModel.CallSheetId = rigJob.CallSheetId;
            var binInformations = eServiceOnlineGateway.Instance.GetBinInformationsByRigId(rigJob.Rig.Id);
            shippingLoadSheetModel.BlendUnloadSheetModels = new List<BlendUnloadSheet>();

            foreach (var item in binInformations)
            {
                shippingLoadSheetModel.BlendUnloadSheetModels.Add(new BlendUnloadSheet()
                {
                    UnloadAmount = 0,
                    DestinationStorage = item
                });
            }
            return shippingLoadSheetModel;
        }


        #region Load Blend To Bulker

        //Nov 13, 2023 Tongtao P45_Q4_175:Add a method to open page for"Load Blend to Bulker"
        //Nov 17, 2023 Tongtao P45_Q4_175:Add Blend,Destination,CallSheetNumber,ClientName for "Load Blend to Bulker" page show
        //Dec 07, 2023 Tongtao P45_Q4_175:Add PodLoads,Crew,CallSheetNumber,IsGoWithCrew for "Load Blend to Bulker" page show
        //Dec 08, 2023 Tongtao 175_PR_LoadToBulker:Add IsThirdParty and change Crew to CrewDescription  for "Load Blend to Bulker" page show
        //Jan 10, 2024 Tongtao:set LoadAmount and ExpectedOnLocationTime info,ProgramId 
        public ActionResult LoadBlendToBulker(List<string> parms)
        {
            LoadBlendToBulkerViewModel model = new LoadBlendToBulkerViewModel();

            model.ProductHaulId = Convert.ToInt32(parms[0]);
            model.ShippingLoadSheetId = Convert.ToInt32(parms[1]);
            model.SourceStorageId = Convert.ToInt32(parms[2]);

            ShippingLoadSheet shippingLoadSheet = eServiceOnlineGateway.Instance.GetShippingLoadSheetById(model.ShippingLoadSheetId);

            ProductHaul productHaul = eServiceOnlineGateway.Instance.GetProductHaulById(shippingLoadSheet.ProductHaul.Id);

            ProductHaulLoad productHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(shippingLoadSheet.ProductHaulLoad.Id);

            model.IsGoWithCrew = productHaul.IsGoWithCrew;

            model.IsThirdParty = productHaul.IsThirdParty;

            foreach (PodLoad podLoad in productHaul.PodLoad)
            {
                if (podLoad.LoadAmount > 0)
                {
                    podLoad.LoadAmount = podLoad.LoadAmount / 1000;
                }
            }

            model.PodLoads = productHaul.PodLoad;

            model.CrewDescription = productHaul.Crew.Description;

            model.CrewId = productHaul.Crew.Id;

            model.Destination = shippingLoadSheet.Destination;

            model.Blend = shippingLoadSheet.BlendDescription;

            model.CallSheetNumber = shippingLoadSheet.CallSheetNumber.ToString();

            model.ExpectedOlTime = productHaul.ExpectedOnLocationTime.ToString("M/d/yyyy HH:mm");

            model.LoadAmount = shippingLoadSheet.LoadAmount / 1000 + "t";

            model.ClientName = shippingLoadSheet.ClientName;


            if (productHaulLoad != null)
            {
                model.ProgramId = productHaulLoad.ProgramId;
            }

            return this.PartialView("_LoadBlendToBulker", model);
        }


        //Nov 13, 2023 Tongtao P45_Q4_175: Add post methods about Data processing for"Load Blend to Bulker"
        //Feb 06, 2024 Tongtao 288_PR_LoadtobulkerWithThirdParty: when crew is third party,use RigJobThirdPartyBulkerCrewSection status
        [HttpPost]
        public ActionResult LoadBlendToBulker(LoadBlendToBulkerViewModel model)
        {
            string userName = this.LoggedUser;

            string result = BinProcess.LoadBlendToBulker(model.ShippingLoadSheetId, userName, model.SourceStorageId, model.PodLoads);

            if (result == "Succeed")
            {
	            //4. Set Product Haul as Loaded if all shippingloadsheets are loaded
	            //If the status of all ShippingLoadSheet of a ProductHaul is Loaded, then the status of the ProductHaul must also be updated to Loaded.
	            var productHaul = eServiceOnlineGateway.Instance.GetProductHaulById(model.ProductHaulId);

                List<ShippingLoadSheet> shippingLoadSheets = productHaul.ShippingLoadSheets.Where(p => p.ShippingStatus != ShippingStatus.Loaded && (p.SourceStorage.Id == 0 || p.BulkPlant.Id == productHaul.BulkPlant.Id)).ToList();

                RigJobCrewSectionStatus rigJobCrewSectionStatus = productHaul.IsThirdParty
                    ? eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionByProductHaulId(productHaul.Id).RigJobCrewSectionStatus
                    : eServiceOnlineGateway.Instance.GetRigJobCrewSectionsByProductHaul(productHaul.Id).RigJobCrewSectionStatus;


                //Nov 17, 2023 Tongtao P45_Q4_175: Only update ProductHaul
                if (shippingLoadSheets.Count == 0)
                {
                    CrewProcess.ChangeSanjelCrewStatus1(productHaul.Crew.Id,
                        productHaul.Id, BulkerCrewStatus.Loaded, rigJobCrewSectionStatus,
                        productHaul.ProductHaulLifeStatus, productHaul.IsThirdParty, userName);
                }
                else
                {
                    //Jan 31, 2024 AW: Only set Loading once if multiple loads
                    if (productHaul.ProductHaulLifeStatus == ProductHaulStatus.Scheduled)
                    {
                        productHaul.ProductHaulLifeStatus = ProductHaulStatus.Loading;
                        eServiceOnlineGateway.Instance.UpdateProductHaul(productHaul);
                    }
                }

            }

            string url = Request.Headers["Referer"].ToString();
            return Redirect(url);
        }

        //Feb 03, 2024 Tongtao 246_PR_MTSPrinting: Add print mts print function
        //Apirl 05, 2024 Tongtao 192_PR_StandardizeMTSDownloadName: modify file name format
        public ActionResult PrintProductHaulPdf(int productHaulId)
        {
	        string printOutFilePath = ConfigurationManager.AppSettings["printOutFilePath"];

            if (!Directory.Exists(printOutFilePath))
            {
                Directory.CreateDirectory(printOutFilePath);
            }

            if (productHaulId == 0)
            {
                return null;
            }

            ProductHaul productHaul = this._context.GetProductHaulById(productHaulId);

            if (productHaul == null) return null;

            string fileName = "";

            string shortName = "";

            List<MTSModel> list=  CreateMTSModelByproductHaul(productHaul);

            MtsItemsModel model = new MtsItemsModel(list);

            if (!System.IO.File.Exists(printOutFilePath))
            {
                if (!list[0].ProductHaulInfoItem.IsBackHaul)
                {
                    var shortnameList = CacheData.AllClientCompanies.Where(p => p.Name.Equals(list[0].ProductHaulInfoItem.Client.Trim())).ToList();


                    if (shortnameList.Count == 0 )
                    {
                        shortName = String.Empty;
                    }
                    else if (string.IsNullOrEmpty(shortnameList.First().ShortName))
                    {
                        shortName = list[0].ProductHaulInfoItem.Client;
                    } 
                    else
                    {
                        shortName = shortnameList.First().ShortName;
                    }

                    fileName = (!string.IsNullOrEmpty(shortName)? (shortName + "_"): string.Empty) +
                               (!string.IsNullOrEmpty(list[0].ProductHaulInfoItem.WellLocation)? (list[0].ProductHaulInfoItem.WellLocation + "_"):string.Empty) + 
                               (!string.IsNullOrEmpty(list[0].ProductHaulInfoItem.RigName)?(                      list[0].ProductHaulInfoItem.RigName + "_"):string.Empty) + 
                               list[0].ProductHaulInfoItem.BaseBlend  +"_" + list[0].MtsSerialNumber + "_" + 
                               DateTime.Now.ToString("yyyy") + "_" + DateTime.Now.ToString("MM") + "_" + DateTime.Now.ToString("dd") +
                                "_" + DateTime.Now.ToString("HH") + "." + DateTime.Now.ToString("mm") + "." + DateTime.Now.ToString("ss") + ".pdf";
                }
                else
                {
                    fileName =  list[0].ProductHaulInfoItem.RigName + "_" + list[0].ProductHaulInfoItem.BulkPlant + "_" +
                        list[0].ProductHaulInfoItem.BaseBlend + "_" +
                        list[0].MtsSerialNumber + "_" +
                        DateTime.Now.ToString("yyyy") + "_" + DateTime.Now.ToString("MM") + "_" + DateTime.Now.ToString("dd")
                        + "_" + DateTime.Now.ToString("HH") + "." + DateTime.Now.ToString("mm") + "." + DateTime.Now.ToString("ss") + ".pdf";

                }

                fileName = fileName.Replace("\\", "_").Replace("/", "_").Replace("*", "").Replace("?", "").Replace(":", "").Replace("<", "").Replace(">", "").Replace("|", "");

                string modelJson = JsonConvert.SerializeObject(model);

                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                string xmlLayoutFilePath = $"{basePath}Reports\\MTSData\\Option\\MTSData.xml";
                string cssStyleFilePath = $"{basePath}Reports\\MTSData\\Option\\MTSData.css";
                string outputPath = $"{printOutFilePath}\\{fileName}";

                sesi.SanjelLibrary.PrintingLibrary.Services.PrintingService.PrintFrom(modelJson, xmlLayoutFilePath, cssStyleFilePath, basePath,outputPath);

                byte[] fileBytes = System.IO.File.ReadAllBytes(outputPath);

                Response.Headers.Add("Content-Disposition", "attachment; filename=" + fileName);

                return File(fileBytes, "application/pdf");
            }

            return null;
        }

        //Feb 03, 2025 Tongtao 246_PR_MTSPrinting:create print model by ProductHaul info
        //Feb 06, 2025 Tongtao 246_PR_MTSPrinting:change data soure and format
        //Feb 26, 2025 Tongtao 246_PR_MTSPrinting:when BaseAmount equals 0,not show BaseAmount unit
        //Feb 26, 2025 Tongtao 246_PR_MTSPrinting:add last version
        //Feb 28, 2025 Tongtao 246_PR_MTSPrinting:one ShippingLoadSheet show one page, and modify mts number format
        private List<MTSModel> CreateMTSModelByproductHaul(ProductHaul productHaul)
        {
            List<MTSModel> list = new List<MTSModel>();

            int? nProductHaulLoadId = null;

            var shippingItems = productHaul.ShippingLoadSheets;

            List<int> shippingLoadSheetIds = productHaul.ShippingLoadSheets.Select(p => p.Id).ToList();
            List<ShippingLoadSheet> shippingLoadSheets =
	            eServiceOnlineGateway.Instance.GetShippingLoadSheetsByIds(shippingLoadSheetIds);
            List<BlendUnloadSheet> blendUnloadSheets = eServiceOnlineGateway.Instance.GetBlendUnloadSheetByshippingLoadSheetIds(shippingLoadSheetIds.ToArray());

            int shippingLoadSheetOrder = 1;


            foreach (ShippingLoadSheet shippingItem in shippingLoadSheets)
            {

                    MTSProductHaulInfoModel productHaulInfo = new MTSProductHaulInfoModel();

                    Collection<MTSPodLoadModel> collectionPodLoad = new Collection<MTSPodLoadModel>();

                    Collection<MTSProductHaulLoadModel> collectionProductHaulLoad = new Collection<MTSProductHaulLoadModel>();

                    Collection<MTSBlendUnloadSheetModel> collectionBlendUnloadSheet = new Collection<MTSBlendUnloadSheetModel>();

                    CallSheet callSheet = this._context.GetCallSheetByNumber(shippingItem.CallSheetNumber);

                    //ProductHaul
                    productHaulInfo.CreateDateTime = DateTime.Now.ToString("MM/dd/yy", CultureInfo.CreateSpecificCulture("en-US"));
                    productHaulInfo.UnitNum = productHaul.TractorUnit.Name;
                    productHaulInfo.DriverName = productHaul.Driver.Name;
                    productHaulInfo.GoWithDriverName = productHaul.IsGoWithCrew ? "YES" : "NO";
                    productHaulInfo.EstimatedLoadTime = productHaul.EstimatedLoadTime.ToString("MM/dd/yy HH:mm");
                    productHaulInfo.EstimatedLocationTime = productHaul.ExpectedOnLocationTime.ToString("MM/dd/yy HH:mm");
                    productHaulInfo.Station = shippingItem.BulkPlant.Name + (shippingItem.SourceStorage.Id!=0?("/"+shippingItem.SourceStorage.Name):"");
                    productHaulInfo.Client = shippingItem.ClientName??String.Empty;
                    productHaulInfo.ClientRep = shippingItem.ClientRepresentative;
                    productHaulInfo.RigName = shippingItem.Rig.Name;
                    productHaulInfo.Location = callSheet==null?"": callSheet.Header.HeaderDetails.WellLocationInformation.WellLocation;
                    productHaulInfo.WellLocation = callSheet == null ? "" : callSheet.Header.HeaderDetails.WellLocationInformation.DownHoleWellLocation;
                    productHaulInfo.DispatchedBy = productHaul.ModifiedUserName;
                    productHaulInfo.BaseBlend = shippingItem.Name;

                    if (shippingItem.ProductHaulLoad.Id == 0 && !shippingItem.BulkPlant.Name.Contains("Bulk Plant"))
                    {
                        productHaulInfo.IsBackHaul = true;
                    }

                    productHaulInfo.BulkPlant = shippingItem.BulkPlant.Name;

                    foreach (ShippingLoadSheet shippingLoadSheetByBlend in shippingItems)
                    {
                        if (shippingLoadSheetByBlend.Id == shippingItem.Id)
                        {
                                MTSProductHaulLoadModel productHaulLoadModel = new MTSProductHaulLoadModel();

                                productHaulLoadModel.BlendRequestNum = shippingLoadSheetByBlend.ProductHaulLoad.Id.ToString();

                                productHaulLoadModel.ProductDes = shippingLoadSheetByBlend.BlendDescription;

                                productHaulLoadModel.Sample = "";

                                productHaulLoadModel.RequestedBy = shippingLoadSheetByBlend.ModifiedUserName;

                                productHaulLoadModel.TotalTonnage = (shippingLoadSheetByBlend.LoadAmount / 1000).ToString("##.###") + " t";

                                var blendLog = eServiceOnlineGateway.Instance.GetBlendLogByProductHaulLoadId(shippingLoadSheetByBlend.ProductHaulLoad.Id);

                                if (blendLog != null)
                                {
                                    productHaulLoadModel.BlendedBy = blendLog.BulkPlantOperator;
                                }

                                productHaulLoadModel.BaseTonnage = shippingLoadSheetByBlend.BaseAmount == 0 ? "" : (shippingLoadSheetByBlend.BaseAmount / 1000.0).ToString("##.###") + " t";

                                collectionProductHaulLoad.Add(productHaulLoadModel);
                        }
                    }

                    List<PodLoad> podLoads = productHaul.PodLoad.OrderBy(p => p.PodIndex).ToList();

                    foreach (PodLoad podLoad in podLoads)
                    {
                        MTSPodLoadModel mTSPodLoadModel = new MTSPodLoadModel();

                        mTSPodLoadModel.PodIndex = (podLoad.PodIndex+1).ToString();

                        if (podLoad.ShippingLoadSheet.Id != 0)
                        {
                            ShippingLoadSheet shippingLoadSheet = shippingLoadSheets.Find(p=>p.Id==podLoad.ShippingLoadSheet.Id);

                            if (shippingLoadSheet.Id  == shippingItem.Id)
                            {
                                mTSPodLoadModel.ProductDes = shippingLoadSheet.BlendDescription;
                                mTSPodLoadModel.TotalTonnage = (podLoad.LoadAmount / 1000).ToString("##.###") + " t";
                                mTSPodLoadModel.BaseTonnage = (podLoad.BaseTonnage / 1000).ToString("##.###") + " t";
                                mTSPodLoadModel.BaseTonnage = podLoad.BaseTonnage == 0 ? "" : (podLoad.BaseTonnage / 1000.0).ToString("##.###") + " t";
                            }
                        }

                        mTSPodLoadModel.TempDesc = "";

                        collectionPodLoad.Add(mTSPodLoadModel);
                    }


                    if (shippingLoadSheetIds.Count() > 0)
                    {
	                    var slipBlendUnloadSheets =
		                    blendUnloadSheets.FindAll(p => p.ShippingLoadSheet.Id == shippingItem.Id);

                        foreach (BlendUnloadSheet blendUnloadSheet in slipBlendUnloadSheets)
                        {
                            MTSBlendUnloadSheetModel mTSBlendUnloadSheetModel = new MTSBlendUnloadSheetModel();

                            mTSBlendUnloadSheetModel.BinNum = blendUnloadSheet.DestinationStorage.Name;
                            mTSBlendUnloadSheetModel.ProductDes = blendUnloadSheet.ShippingLoadSheet.BlendDescription;
                            mTSBlendUnloadSheetModel.TotalTonnage = (blendUnloadSheet.UnloadAmount / 1000).ToString("##.###") + " t";
                            mTSBlendUnloadSheetModel.BaseTonnage = (blendUnloadSheet.BaseTonnage / 1000).ToString("##.###") + " t";
                            mTSBlendUnloadSheetModel.BaseTonnage = blendUnloadSheet.BaseTonnage == 0 ? "" : (blendUnloadSheet.BaseTonnage / 1000).ToString("##.###") + " t";

                            collectionBlendUnloadSheet.Add(mTSBlendUnloadSheetModel);
                        }
                    }

                    nProductHaulLoadId = shippingItem.ProductHaulLoad.Id;
                //[MTS Number] = MTS - 53567 - L1 -[ProgramNumber.Ver].

                string productHaulNum = "0000000";

                if (shippingItem.CallSheetNumber == 0 && (shippingItem.ProgramId != "0"&& shippingItem.ProgramId!=null))
                {
                    productHaulNum = shippingItem.ProgramId;
                }
                else if (shippingItem.CallSheetNumber != 0 )
                {
                    productHaulNum = shippingItem.CallSheetNumber.ToString();
                }


                string mtsSerialNumber = "MTS" + "-" + productHaul.Id + "-L"+ shippingLoadSheetOrder+ "-" + productHaulNum;
                    string latestVersionInfo = "v " + productHaul.Version.ToString() + " - "  + productHaul.ModifiedDateTime.ToString("g") + " by " + productHaul.ModifiedUserName;
                    MTSModel mTSModel = new MTSModel(mtsSerialNumber, latestVersionInfo, productHaulInfo, collectionPodLoad, collectionProductHaulLoad, collectionBlendUnloadSheet);


                    shippingLoadSheetOrder++;
                    list.Add(mTSModel);

            }

            return list;
        }
        #endregion

        //Dec 22, 2023 zhangyuan 195_PR_Haulback: Add view Page
        public ActionResult HaulBack(List<string> parms)
        {

            HaulBackFromRigJobBinViewModel model = new HaulBackFromRigJobBinViewModel();

            var binInformation = eServiceOnlineGateway.Instance.GetBinInformationById(Convert.ToInt32(parms[2]));

            var rigId = Convert.ToInt32(Convert.ToInt32(parms[0]));
            var rig = EServiceReferenceData.Data.RigCollection.FirstOrDefault(p => p.Id == rigId);
            //source bin
            model.PopulateFromBinSection(binInformation);
            model.ShippingLoadSheetModel.RigId = 0;
            model.ShippingLoadSheetModel.BulkPlantId = rig.Id;
            model.ShippingLoadSheetModel.BulkPlantName = rig.Name;
            model.ShippingLoadSheetModel.BlendUnloadSheetModels = new List<BlendUnloadSheet>();
            model.ShippingLoadSheetModel.Quantity = binInformation.Quantity;

            model.ProductHaulInfoModel.InitializeProductHaulSchedule();
            // Feb 2, 2024 zhangyuan 195_PR_Haulback: modify rig to rig or bulkplant 
            List<OperationSiteType> operationSiteTypes = new List<OperationSiteType>
                { OperationSiteType.BulkPlant, OperationSiteType.Rig, OperationSiteType.ProjectBulkPlant };
            ViewBag.Rigs = CacheData.Rigs.OrderBy(a => a.Name)
                .Where(p => operationSiteTypes.IndexOf(p.OperationSiteType)>-1).ToList(); 

            return this.PartialView("../ProductHaul/_HaulBackFromRigJobBin", model);

        }
        // Dec 22, 2023 zhangyuan 195_PR_Haulback: Add HaulBack Save Logic
        [HttpPost]
        public ActionResult HaulBackFromRigJobBin(HaulBackFromRigJobBinViewModel model)
        {
            model.LoggedUser = this.LoggedUser;
            model.PopulateToModel();
            this._context.HaulBackFromRigJobBin(model.RigJobId,model.LoggedUser, model.ProductHaulInfoModel, model.ShippingLoadSheetModel, model.HaulBackFromBinModel, model.BulkPlantBinLoadModel);

            string url = Request.Headers["Referer"].ToString();
            return Redirect(url);
        }

    }
} 