using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using eServiceOnline.Data;
using eServiceOnline.Models.ProductHaul;
using eServiceOnline.BusinessProcess;
using Microsoft.AspNetCore.Mvc;
using Sanjel.BusinessEntities.CallSheets;

namespace eServiceOnline.Controllers
{
    public class LoadController : Controller
    {
        private readonly eServiceWebContext _context;
        public static List<ProductHaulModel> ordersData = new List<ProductHaulModel>();
        public LoadController()
        {
            this._context = new eServiceWebContext();
        }
        public ActionResult Index(int callSheetNumber)
        {
            CallSheet callSheet = this._context.GetCallSheetByNumber(callSheetNumber);

            LoadViewModel model = new LoadViewModel
            {
                RigJob = this._context.GetRigJobFromCallSheet(callSheet),
                ProductList = this._context.GetBlendSectionsFromCallSheetId(callSheet.Id),
                LoadList = this._context.GetProductHaulCollectionByCallSheetNumber(callSheetNumber)
            };
            model.ProductList = this.CalculateRemaining(model.LoadList, model.ProductList);
            ordersData = (List<ProductHaulModel>) model.LoadList;

            return this.View(model);
        }

        private IEnumerable<BlendSectionModel> CalculateRemaining(IEnumerable<ProductHaulModel> productHaulModels, IEnumerable<BlendSectionModel> blendSectionModels )
        {
            List < BlendSectionModel > blendSectionModelList = new List<BlendSectionModel>();
            if (blendSectionModels !=null)
            {
                foreach (BlendSectionModel blendSectionModel in blendSectionModels)
                {
                    double remaining = blendSectionModel.Sent;
                    if (productHaulModels != null)
                        foreach (ProductHaulModel entity in productHaulModels)
                        {
                            if (entity.BaseBlendSectionId == blendSectionModel.Id)
                            {
                                remaining = entity.IsTotalBlendTonnage ? (remaining + entity.BaseBlendWeight / 1000) : remaining = (remaining + entity.Amount);
                            }
                        }
                    blendSectionModel.Sent = Math.Round(remaining, 2);
                    blendSectionModel.Remains = Math.Round((blendSectionModel.Amount - Math.Round(remaining, 2)),2);
                    blendSectionModelList.Add(blendSectionModel);
                }
            }

            return blendSectionModelList;
        }
        /*
        [HttpPost]
        public async Task<string> Insert(ProductHaulModel value,string submit)
        {
            if (value != null)
            {
                if (value.CallSheetNumber > 0)
                {
                    if (submit.Equals("save"))
                    {
                        string validateStr = ProductHaulValidation.ValidateAmountAndMixWater(value.CallSheetNumber, value.BaseBlendSectionId, value.Amount, value.MixWater, value.IsTotalBlendTonnage, value.ProductHaulId, value.ProgramNumber);
                        if (!validateStr.Equals("Pass"))
                        {
                            return validateStr;
                        }
                    }
                }
                   
                if (value.CallSheetNumber <= 0)
                    value.BaseBlendChemicalId = value.BaseBlendSectionId;
                ProductHaulModel productHaulModel = this._context.CreateProductHaul(value);

                return productHaulModel.ProductHaulId + ","+ productHaulModel.CallSheetNumber;
            }
            
            return null;
        }
        */

        public ActionResult Delete(ProductHaulModel model)
        {
            if (model != null)
            {
//                this._context.DeleteProductHaul(model);
                return this.Json(model);
            }
            return null;
        }

        public ActionResult LoadSheet(int productHaulLoadId = 0, int callSheetNumber = 0)
        {
            if (productHaulLoadId != 0)
            {
                ProductHaulModel productHaulModel = this._context.GetProductHaulModelByProductHaulLoadId(productHaulLoadId);
                CallSheet callSheet = this._context.GetCallSheetByNumber(callSheetNumber);
                if (productHaulModel != null)
                {
                    productHaulModel.BaseBlendWeight = Math.Round(productHaulModel.BaseBlendWeight / 1000, 2, MidpointRounding.AwayFromZero);
                    productHaulModel.TotalBlendWeight = Math.Round(productHaulModel.TotalBlendWeight / 1000, 2, MidpointRounding.AwayFromZero);
                    PrintLoadSheetViewModel printLoadSheetViewModel = new PrintLoadSheetViewModel
                    {
                        ProductHaulModel = productHaulModel
                    };
                    return View(printLoadSheetViewModel);
                }
            }

            return View(new PrintLoadSheetViewModel());                        
        }
    }
}

