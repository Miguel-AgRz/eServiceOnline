using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Caching;
using eServiceOnline.Data;
using eServiceOnline.Gateway;
using eServiceOnline.Models.BlendBoard;
using eServiceOnline.Models.ProductHaul;
using eServiceOnline.SecurityControl;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Products;
using Sesi.SanjelData.Services.Interfaces.Common.BusinessEntities.Products;
using Syncfusion.JavaScript;

namespace eServiceOnline.Controllers
{
    public class BlendBoardController:eServiceOnlineController
    {
        private readonly eServiceWebContext _context;
        public const string Ascending = "ascending";
        public const string Descending = "descending";
        private IMemoryCache _memoryCache;
        public BlendBoardController(IMemoryCache memoryCache)
        {
            this._context = new eServiceWebContext();
            _memoryCache = memoryCache;
        }
        public ActionResult Index()
        {
            this.ViewBag.HighLight = "Blend Chemical";
            ViewBag.VersionNumber = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            BlendViewModel model = new BlendViewModel();
            this.ViewBag.HavePermission = true;
            var categoryList = CacheData.PrimaryCategories.Select(p => new SelectListItem { Text = p.Name, Value = p.Id.ToString() }).ToList();
            var blankItem = new SelectListItem(){Text="", Value="0"};
            categoryList.Insert(0,blankItem);
            this.ViewData["categoryItems"] = categoryList;
            this.ViewData["statusItems"]=this.GetEnumValues(typeof(ProductStatus));
            this.ViewData["unitItems"]=CacheData.BlendAdditiveMeasureUnits.Select(p => new SelectListItem { Text = p.Name, Value = p.Id.ToString() }).ToList();

            return View(model);
        }

        public ActionResult GetPagedBlendChemicals([FromBody] DataManager dataManager)
        {
            DateTime startDateTime = DateTime.Now;
            int pageSize = dataManager.Take;
            int pageNumber = dataManager.Skip/pageSize + 1;
            int count;
            List<BlendChemicalModel> data = eServiceWebContext.Instance.GetBlendChemicalPageInfo(0, pageNumber, pageSize, out count);

            if (dataManager.Sorted != null && dataManager.Sorted.Count.Equals(1))
            {
                Sort sort = dataManager.Sorted.First();
                data = Utility.Sort<BlendChemicalModel>(data, sort.Direction, sort.Name);
            }

            Debug.WriteLine("\t\tGetPagedBlendChemicals - {0,21}", DateTime.Now.Subtract(startDateTime));
            return this.Json(new {result = data, count = count});
        }

        public ActionResult Update([FromBody]CRUDModel<BlendChemicalModel> myObject)
        {

            return Json(myObject.Value);
        }
        [HttpPost]
        public ActionResult Insert(BlendChemicalModel blendChemicalModel)
        {
            BlendChemicalModel modelValue = blendChemicalModel;
            BlendChemical blendChemical = new BlendChemical();
            blendChemical.Name = modelValue.Name;
            blendChemical.IsBaseEligible = modelValue.IsBaseEligible;
            blendChemical.IsAdditiveEligible = modelValue.IsAdditiveEligible;
            blendChemical.Yield = modelValue.Yield;
            blendChemical.Density = modelValue.Density;
            blendChemical.BulkDensity = modelValue.BulkDensity;
            blendChemical.SpecificGravity = modelValue.SpecificGravity;
            blendChemical.MixWater = modelValue.MixWater;
            blendChemical.AERCode = modelValue.AERCode;
            blendChemical.BlendPrimaryCategory = CacheData.PrimaryCategories.FirstOrDefault(p=>p.Name== modelValue.PrimaryCategoryName);
            blendChemical.ProductStatus = (ProductStatus) modelValue.StatusId;

            BlendAdditiveMeasureUnit recipeUnit =
                CacheData.BlendAdditiveMeasureUnits.FirstOrDefault(p => p.Name == modelValue.UnitName);

            /*if (blendChemical.IsBaseEligible)
            {
                BlendFluidType blendFluidType = new BlendFluidType();
                blendFluidType.Name = blendChemical.Name;
//                eServiceOnlineGateway.Instance.CreateBlendFluidType(blendFluidType);
                IBlendFluidTypeService blendFluidTypeService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IBlendFluidTypeService>();
                if (blendFluidTypeService == null) throw new Exception("blendFluidTypeService must be registered in service factory");

                var insertRtn = blendFluidTypeService.Insert(blendFluidType);
                if (insertRtn == 1)
                    blendChemical.BaseBlendType = blendFluidTypeService.SelectBy(blendFluidType, new List<string>() {"Name"}).FirstOrDefault();
            }

            if (blendChemical.IsAdditiveEligible)
            {
                AdditiveType additiveType = new AdditiveType();
                additiveType.Name = blendChemical.Name;
//                eServiceOnlineGateway.Instance.CreateBlendFluidType(blendFluidType);
                IAdditiveTypeService additiveTypeService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IAdditiveTypeService>();
                if (additiveTypeService == null) throw new Exception("blendFluidTypeService must be registered in service factory");

                var insertRtn = additiveTypeService.Insert(additiveType);
                if (insertRtn == 1)
                    blendChemical.AdditiveType = additiveTypeService.SelectBy(additiveType, new List<string>() {"Name"}).FirstOrDefault();
            }

            {
                Product product = new Product();
                product.PriceCode = modelValue.PriceCode;
                product.InventoryNumber = modelValue.InventoryNumber;
                IProductService additiveTypeService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IProductService>();
                if (additiveTypeService == null) throw new Exception("additiveTypeService must be registered in service factory");

                var insertRtn = additiveTypeService.Insert(product);
                if (insertRtn == 1)
                    blendChemical.Product = additiveTypeService.SelectBy(product, new List<string>() {"Name"}).FirstOrDefault();
            }

            if(modelValue.HasRecipe)
            {
                BlendRecipe blendRecipe = new BlendRecipe();
                blendRecipe.Name = blendChemical.Name;
                blendRecipe.Unit = recipeUnit;
                IBlendRecipeService blendRecipeService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IBlendRecipeService>();
                if (blendRecipeService == null) throw new Exception("blendRecipeService must be registered in service factory");

                var insertRtn = blendRecipeService.Insert(blendRecipe);
                if (insertRtn == 1)
                    blendChemical.BlendRecipe = blendRecipeService.SelectBy(blendRecipe, new List<string>() {"Name"}).FirstOrDefault();
            }*/

            CreateBlendChemical(blendChemical, recipeUnit, modelValue.HasRecipe, modelValue.PriceCode, modelValue.InventoryNumber);

            if (blendChemical.IsBaseEligible)
            {
                BlendChemical blendAdds = new BlendChemical();
                blendAdds.Name = blendChemical.Name + " + Additives";
                blendAdds.ProductStatus = blendChemical.ProductStatus;

                CreateBlendChemical(blendAdds, recipeUnit, true, modelValue.AddsPriceCode, "", blendChemical);
            }

            return Json(blendChemical);
        }

        private void CreateBlendChemical(BlendChemical blendChemical, BlendAdditiveMeasureUnit recipeUnit, bool hasRecipe, int priceCode, string inventoryNumber, BlendChemical baseBlend=null)
        {
            if (blendChemical.IsBaseEligible)
            {
                BlendFluidType blendFluidType = new BlendFluidType();
                blendFluidType.Name = blendChemical.Name;
//                eServiceOnlineGateway.Instance.CreateBlendFluidType(blendFluidType);
                IBlendFluidTypeService blendFluidTypeService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IBlendFluidTypeService>();
                if (blendFluidTypeService == null) throw new Exception("blendFluidTypeService must be registered in service factory");

                var insertRtn = blendFluidTypeService.Insert(blendFluidType);
                if (insertRtn == 1)
                    blendChemical.BaseBlendType = blendFluidTypeService.SelectBy(blendFluidType, new List<string>() {"Name"}).FirstOrDefault();
            }

            if (blendChemical.IsAdditiveEligible)
            {
                AdditiveType additiveType = new AdditiveType();
                additiveType.Name = blendChemical.Name;
//                eServiceOnlineGateway.Instance.CreateBlendFluidType(blendFluidType);
                IAdditiveTypeService additiveTypeService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IAdditiveTypeService>();
                if (additiveTypeService == null) throw new Exception("blendFluidTypeService must be registered in service factory");

                var insertRtn = additiveTypeService.Insert(additiveType);
                if (insertRtn == 1)
                    blendChemical.AdditiveType = additiveTypeService.SelectBy(additiveType, new List<string>() {"Name"}).FirstOrDefault();
            }

            {
                Product product = new Product();
                product.Name = blendChemical.Name;
                product.PriceCode = priceCode;
                product.InventoryNumber = inventoryNumber;
                IProductService additiveTypeService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IProductService>();
                if (additiveTypeService == null) throw new Exception("additiveTypeService must be registered in service factory");

                var insertRtn = additiveTypeService.Insert(product);
                if (insertRtn == 1)
                    blendChemical.Product = additiveTypeService.SelectBy(product, new List<string>() {"Name"}).FirstOrDefault();
            }

            if(hasRecipe)
            {
                BlendRecipe blendRecipe = new BlendRecipe();
                blendRecipe.Name = blendChemical.Name;
                blendRecipe.Unit = recipeUnit;


                IBlendRecipeService blendRecipeService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IBlendRecipeService>();
                if (blendRecipeService == null) throw new Exception("blendRecipeService must be registered in service factory");

                var insertRtn = blendRecipeService.Insert(blendRecipe);
                if (insertRtn == 1)
                {
                    blendChemical.BlendRecipe = blendRecipeService.SelectBy(blendRecipe, new List<string>() {"Name"})
                        .FirstOrDefault();
                    if (baseBlend != null)
                    {
                        if(blendRecipe.BlendChemicalSections == null) blendRecipe.BlendChemicalSections = new List<BlendChemicalSection>();

                        BlendChemicalSection blendChemicalSection = new BlendChemicalSection();
                        blendChemicalSection.Name = baseBlend.Name;
                        blendChemicalSection.BlendChemical = baseBlend;
                        blendChemicalSection.Amount = 100;
                        blendChemicalSection.IsBaseBlend = true;
                        blendChemicalSection.AdditionMethod = CacheData.AdditionMethods.FirstOrDefault(p=>p.Name=="BWOC");
                        blendChemicalSection.AdditiveBlendMethod = CacheData.AdditiveBlendMethods.FirstOrDefault(p=>p.Name=="Preblend");
                        blendChemicalSection.Unit = CacheData.BlendAdditiveMeasureUnits.FirstOrDefault(p=>p.Name=="Percent");
                        blendChemicalSection.BlendRecipe = blendChemical.BlendRecipe;

                        IBlendChemicalSectionService blendChemicalSectionService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IBlendChemicalSectionService>();
                        if (blendChemicalSectionService == null) throw new Exception("blendChemicalSectionService must be registered in service factory");

                        blendChemicalSectionService.Insert(blendChemicalSection);
                    }
                }
            }

            {
                IBlendChemicalService blendChemicalService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance
                    .GetService<IBlendChemicalService>();
                if (blendChemicalService == null)
                    throw new Exception("blendChemicalService must be registered in service factory");

                var insertRtn = blendChemicalService.Insert(blendChemical);
                if (insertRtn == 1)
                    blendChemical = blendChemicalService.SelectBy(blendChemical, new List<string>() {"Name"})
                        .FirstOrDefault();
            }
        }
    }
}
