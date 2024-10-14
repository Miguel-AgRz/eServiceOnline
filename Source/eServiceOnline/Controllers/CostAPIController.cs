using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using eServiceOnline.Data;
using eServiceOnline.Gateway;
using eServiceOnline.Models.ProductHaul;
using MetaShare.Common.Foundation;
using MetaShare.Common.Foundation.EntityBases;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Sanjel.BusinessEntities.Sections.Common;
using Sanjel.Common.BusinessEntities.Mdd;
using Sanjel.Common.BusinessEntities.Mdd.Products;
using Sanjel.Common.BusinessEntities.Reference;
using Sanjel.Common.Domain.UnitOfMeasure;
using sesi.SanjelLibrary.BlendLibrary;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Products;
using Sesi.SanjelData.Entities.Common.Entities.General;
using Sesi.SanjelLibrary.MapperLibrary;
using AdditiveType = Sanjel.Common.BusinessEntities.Reference.AdditiveType;
using BlendFluidType = Sanjel.Common.BusinessEntities.Reference.BlendFluidType;

namespace eServiceOnline.Controllers
{
    public class CostAPIController : eServiceOnlineController
    {
        public ActionResult GetCost(string recipe)
        {
            List<Ingredient> ingredientList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Ingredient>>(recipe);
            string[] servicePoints = {"D603", "D612", "D617", "D604", "D616", "D615", "D606", "D607"};
            List<ServicePointCost> servicePointCosts = new List<ServicePointCost>();

            BlendChemical blendChemical = BuildFromScratch(ingredientList);

            foreach (var servicePoint in servicePoints)
            {

                var cost = CostCalculator.CalculateCost1(CacheData.BlendChemicals, CacheData.BlendRecipes, EServiceReferenceData.Data.PurchasePriceCollection, EServiceReferenceData.Data.ProductCollection,blendChemical,
                    0, true, servicePoint,
                    new BlendAdditiveMeasureUnit()
                    {
                        Id = 8, Name = "Tonne", Abbreviation = "t",
                        MeaureUnitType = new MeasureUnitType() {Id = 2, Name = "Weight"}
                    });

                servicePointCosts.Add(new ServicePointCost(){SP = servicePoint, TotalCost = cost});
            }

            string responseXml = Serializer.XmlSerialize(typeof(List<ServicePointCost>), servicePointCosts);

            return Content(responseXml);
        }

        [ResponseCache(NoStore =true, Location =ResponseCacheLocation.None)]
        public ActionResult GetCostWithBreakdown(string recipe, string districts)
        {

            CostRecipe costRecipe = Newtonsoft.Json.JsonConvert.DeserializeObject<CostRecipe>(recipe);
//            string[] servicePoints = {"D603", "D612", "D617", "D604", "D616", "D615", "D606", "D607"};
            string[] servicePoints = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(districts);
            List<ServicePointCost> servicePointCosts = new List<ServicePointCost>();

            BlendChemical blendChemical = BuildRecipe(costRecipe);

            BlendAdditiveMeasureUnit unitType = CommonEntityBase.FindItem(
                EServiceReferenceData.Data.BlendAdditiveMeasureUnitCollection,
                p => p.Abbreviation == costRecipe.CostUnitPer);
            blendChemical.BlendRecipe.Unit = unitType;
            double baseQuantity = costRecipe.Ingredients[0].Quantity;
            string baseUnit = costRecipe.Ingredients[0].UOM;
            BlendChemical baseBlendChemical = CommonEntityBase.FindItem(
                EServiceReferenceData.Data.BlendChemicalCollection,
                p => p.Name == costRecipe.Ingredients[0].Material);

            /*  First Version Cost calculation
            foreach (var servicePoint in servicePoints)
            {
                var totalCost = BlendCostItem.CalculateCost1(EServiceReferenceData.Data.PurchasePriceCollection, blendChemical,
                    0, true, servicePoint, unitType) * baseQuantity;

                Collection<IngredientCost> ingredientCosts = new Collection<IngredientCost>();

                foreach (var costRecipeIngredient in costRecipe.Ingredients)
                {
                    BlendChemical ingredientBlendChemical = CommonEntityBase.FindItem(
                        EServiceReferenceData.Data.BlendChemicalCollection,
                        p => p.Name == costRecipeIngredient.Material);
                    BlendAdditiveMeasureUnit ingredientMeasureUnit = CommonEntityBase.FindItem(
                        EServiceReferenceData.Data.BlendAdditiveMeasureUnitCollection,
                        p => p.Abbreviation == costRecipe.CostUnitPer);

                    double ingredientQuantity = 0.0;
                    if (baseUnit == "t")
                    {
                        if (costRecipeIngredient.UOM == "Percent")
                        {
                            ingredientQuantity = baseQuantity * 1000 * costRecipeIngredient.Quantity / 100;
                            ingredientMeasureUnit = CommonEntityBase.FindItem(
                                EServiceReferenceData.Data.BlendAdditiveMeasureUnitCollection,
                                p => p.Abbreviation == "kg");
                        }
                        else if (costRecipeIngredient.UOM == "% BWOW")
                        {
                            ingredientQuantity = baseQuantity * 1000 * baseBlendChemical.MixWater * costRecipeIngredient.Quantity / 100;
                            ingredientMeasureUnit = CommonEntityBase.FindItem(
                                EServiceReferenceData.Data.BlendAdditiveMeasureUnitCollection,
                                p => p.Abbreviation == "kg");
                        }
                        else
                        {
                            ingredientQuantity = costRecipeIngredient.Quantity;
                            ingredientMeasureUnit = CommonEntityBase.FindItem(
                                EServiceReferenceData.Data.BlendAdditiveMeasureUnitCollection,
                                p => p.Abbreviation == costRecipeIngredient.UOM);
                        }
                    }

                    IngredientCost ingredientCost = new IngredientCost()
                    {
                        Cost = BlendCostItem.CalculateCost1(EServiceReferenceData.Data.PurchasePriceCollection,
                            ingredientBlendChemical,
                            0, true, servicePoint, ingredientMeasureUnit) * ingredientQuantity
                    };
                    ingredientCosts.Add(ingredientCost);
                }
                servicePointCosts.Add(new ServicePointCost(){SP = servicePoint, TotalCost = totalCost, IngredientCostList = ingredientCosts });
            }
            */

            foreach (var servicePoint in servicePoints)
            {

                var totalCost = CostCalculator.CalculateCost2(CacheData.BlendChemicals, CacheData.BlendRecipes, EServiceReferenceData.Data.PurchasePriceCollection, EServiceReferenceData.Data.ProductCollection, blendChemical,
                    0, true, servicePoint, unitType);

                Collection<IngredientCost> ingredientCosts = new Collection<IngredientCost>();

                foreach (var ingredientCost in totalCost.Item3)
                {
                    ingredientCosts.Add(new IngredientCost(){Cost = ingredientCost.Value.Item1 });
                }


                servicePointCosts.Add(new ServicePointCost(){SP = servicePoint, TotalCost = totalCost.Item1, IngredientCostList = ingredientCosts });
            }

            string responseXml = Serializer.XmlSerialize(typeof(List<ServicePointCost>), servicePointCosts);

            return Content(responseXml);
        }

        private BlendChemical BuildRecipe(CostRecipe costRecipe)
        {
            BlendChemical blendChemical = null;
            if (costRecipe != null && costRecipe.Ingredients != null && costRecipe.Ingredients.Count > 0)
            {
                if (costRecipe.Ingredients[0].UOM == "kg")
                {
                    blendChemical = BuildFromScratch(costRecipe.Ingredients);
                }
                else if (costRecipe.Ingredients[0].UOM == "t" || costRecipe.Ingredients[0].UOM == "m3")
                {
                    var baseBlendChemical = CommonEntityBase.FindItem(
                        EServiceReferenceData.Data.BlendChemicalCollection,
                        p => p.Name.Trim() == costRecipe.Ingredients[0].Material.Trim());
                    BlendSection blendSection = new BlendSection();
                    blendSection.BlendFluidType = new BlendFluidType() { Id = baseBlendChemical.BaseBlendType.Id, Description = baseBlendChemical.BaseBlendType.Name};
                    blendSection.BlendAmountUnit = ObjectBase.FindItem(MeasureUnit.BlendAmountUnits,
                        p => p.Abbreviation == costRecipe.Ingredients[0].UOM);
                    blendSection.Quantity = costRecipe.Ingredients[0].Quantity;
                    blendSection.BlendAdditiveSections = new Collection<BlendAdditiveSection>();

                    int i = 0;
                    foreach (var costRecipeIngredient in costRecipe.Ingredients)
                    {
                        i++;
                        if(i==1) continue;
                        var additiveBlendChemical = CommonEntityBase.FindItem(
                            EServiceReferenceData.Data.BlendChemicalCollection,
                            p => p.Name.Trim().ToLower() == costRecipeIngredient.Material.Trim().ToLower());

                        BlendAdditiveSection blendAdditiveSection = new BlendAdditiveSection();
                        blendAdditiveSection.BaseName = blendSection.BlendFluidType.Description;
                        blendAdditiveSection.AdditiveType = new AdditiveType()
                        {
                            Id = additiveBlendChemical.AdditiveType.Id,
                            Description = additiveBlendChemical.AdditiveType.Name
                        };
                        blendAdditiveSection.Amount = costRecipeIngredient.Quantity;
                        blendAdditiveSection.AdditiveAmountUnit = ObjectBase.FindItem(MeasureUnit.BlendAdditiveUnits,
                            p => p.Name == costRecipeIngredient.UOM);
                        blendSection.BlendAdditiveSections.Add(blendAdditiveSection);
                    }

                    // Build Blend Section and use existing conversion to build blend chemical

                    //blendChemical = BlendSection.CovertToBlendChemicalFromBlendSection(
                    //    EServiceReferenceData.Data.BlendChemicalCollection, blendSection,
                    //    EServiceReferenceData.Data.AdditionMethodCollection,
                    //    EServiceReferenceData.Data.AdditiveBlendMethodCollection,
                    //    EServiceReferenceData.Data.BlendAdditiveMeasureUnitCollection, CacheData.BlendRecipes);
                    var targetBlendSection = GenericMapper.Map<BlendSection, Sesi.SanjelData.Entities.BusinessEntities.BaseEntites.BlendSection>(blendSection);
                    blendChemical = BlendCalculator.CovertToBlendChemicalFromBlendSection(
                        EServiceReferenceData.Data.BlendChemicalCollection, targetBlendSection,
                        EServiceReferenceData.Data.AdditionMethodCollection,
                        EServiceReferenceData.Data.AdditiveBlendMethodCollection,
                        EServiceReferenceData.Data.BlendAdditiveMeasureUnitCollection, CacheData.BlendRecipes);

                }
                else if (costRecipe.Ingredients[0].UOM == "Percent")
                {
                    //use "Cost Unit Per" to be UOM of base, and build blend section
                }
                else
                {
                    throw new Exception("Base Ingredient is not properly set.");
                }

                blendChemical.Yield = costRecipe.Yield;
                blendChemical.MixWater = costRecipe.MixWater;

            }
            return blendChemical;
        }


        private BlendChemical BuildFromScratch(List<Ingredient> ingredientList)
        {
            BlendChemical blendChemical = new BlendChemical();
            blendChemical.BlendRecipe = new BlendRecipe(){ Id = 99999};
            blendChemical.BlendRecipe.BlendChemicalSections= new List<BlendChemicalSection>();

            AdditionMethod recipeAdditionMethod = new AdditionMethod(){Id = 2 , Name="BWOB"};

            for (int i = 0; i < ingredientList.Count; i++)
            {
                var ingredientBlendChemical = CommonEntityBase.FindItem(EServiceReferenceData.Data.BlendChemicalCollection,
                    p => p.Name == ingredientList[i].Material);
                if (ingredientBlendChemical == null) break;
//                ingredientBlendChemical.Product =  CommonEntityBase.FindItem(EServiceReferenceData.Data.ProductCollection,
//                    p => p.Id == ingredientBlendChemical.Product.Id);

                if (i == 0) //first record always base
                {
                    if (ingredientList[i].Quantity == 1000)
                        recipeAdditionMethod = new AdditionMethod() {Id = 1, Name = "BWOC"};

                }

                BlendChemicalSection bledChemicalSection = new BlendChemicalSection()
                {
                    BlendChemical = ingredientBlendChemical,
                    AdditionMethod = recipeAdditionMethod,
                    Amount = ingredientList[i].Quantity,
                    IsBaseBlend = true,
                    AdditiveBlendMethod = new AdditiveBlendMethod(){Id = 1, Name = "Preblend" },
                    Unit = CommonEntityBase.FindItem(EServiceReferenceData.Data.BlendAdditiveMeasureUnitCollection, p=>p.Abbreviation==ingredientList[i].UOM)
                };
                blendChemical.BlendRecipe.BlendChemicalSections.Add(bledChemicalSection);            }

            return blendChemical;
        }

        internal class CostRecipe
        {
            public double Yield { get; set; }
            public double MixWater { get; set; }
            public string BlendName { get; set; }
            public string CostUnitPer { get; set; }
            public List<Ingredient> Ingredients { get; set; }
        }

        internal class Ingredient
        {
            public string Material { get; set; }
            public double Quantity { get; set; }
            public string UOM { get; set; }
        }

    }
    [Serializable]
    public class ServicePointCost
    {
        [XmlAttribute]
        public string SP { get; set; }
        [XmlAttribute]
        public double TotalCost { get; set; }

        [XmlElement] 
        public Collection<IngredientCost> IngredientCostList { get; set; }
    }

    [Serializable]
    public class IngredientCost
    {
        [XmlAttribute]
        public double Cost { get; set; } 
    }
}