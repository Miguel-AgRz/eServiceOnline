using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;
using eServiceOnline.Gateway;
using MetaShare.Common.Foundation;
using MetaShare.Common.Foundation.EntityBases;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Sanjel.Common.BusinessEntities.Mdd.Products;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Products;
using Sesi.SanjelData.Entities.Common.Entities.General;
using eServiceOnline.WebAPI.Data.Cost;
using Sanjel.BusinessEntities.Jobs;
using Sanjel.BusinessEntities.ServiceReports;
using sesi.SanjelLibrary.BlendLibrary;
using Sesi.SanjelData.Entities.BusinessEntities.BaseEntites;
using Sesi.SanjelLibrary.MapperLibrary;

namespace eServiceOnlineAPI.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class CostAPIController : ControllerBase
    {

        public ActionResult GetBlendsCost(string jsonBlends)
        {
            Collection<CostUtils.OutputCost> outputCostCollection = new Collection<CostUtils.OutputCost>();
            CostUtils.InputBlends inputBlends = JsonConvert.DeserializeObject<CostUtils.InputBlends>(jsonBlends.ToString());
            var freightCost = 0.00;
            string servicePointSbsId = "D675"; //HO by default

            int bsId = 1;
            BlendFluidType bft;
            AdditiveType at;


            if (inputBlends != null && inputBlends.Blends != null)
            {
                servicePointSbsId = CostUtils.DistrictSBSByName[inputBlends.District];
                foreach (CostUtils.Blend blend in inputBlends.Blends)
                {
                    bft = blend.Id > 0
                        ? EServiceReferenceData.Data.BlendFluidTypeCollection.FirstOrDefault(b => b.Id == blend.Id)
                        : EServiceReferenceData.Data.BlendFluidTypeCollection.FirstOrDefault(b => (b.Name ?? "").Trim().ToUpper() == blend.Name.Trim().ToUpper());

                    if (bft == null)
                        throw (new Exception("Blend Name was not found for '" + blend.Name + "'! "));

                    if (blend.Unit == "0")
                        blend.Unit = "Cubic Meters";

                    double waterMix = 0.0;
                    double.TryParse(blend.WaterMix, out waterMix);

                    waterMix = waterMix <= 0.0 ? 1.0 : waterMix;

                    BlendSection bs =
                        new BlendSection()
                        {
                            Id = blend.Idx,  //bsId++,
                            BlendFluidType = new BlendFluidType() { Id = bft.Id, Name = bft.Name },
                            Quantity = blend.Quantity,
                            BlendAmountUnit = CacheData.BlendAdditiveMeasureUnits.FirstOrDefault(c => c.Name == CostUtils.GetUnitName(blend.Unit)),
                            MixWaterRequirement = waterMix
                        };

                    if (blend.Additives != null && blend.Additives.Count > 0)
                    {
                        int i = 1;
                        foreach (CostUtils.Additive add in (blend.Additives))
                        {
                            at = add.Id > 0
                                ? EServiceReferenceData.Data.AdditiveTypeCollection.FirstOrDefault(a => a.Id == add.Id)
                                : EServiceReferenceData.Data.AdditiveTypeCollection.FirstOrDefault(a => (a.Name ?? "").Trim().ToUpper() == add.Name.Trim().ToUpper());

                            BlendAdditiveSection bas = null;

                            if (at == null)
                            {
                                throw (new Exception("Additive Name was not found for '" + add.Name + "'! "));
                            }
                            else
                            {
                                bas =
                                    new BlendAdditiveSection()
                                    {
                                        Id = i++,
                                        AdditiveType = new AdditiveType() { Id = at.Id, Name = at.Name },
                                        AdditiveBlendMethod = new AdditiveBlendMethod(),
                                        AdditiveAmountUnit = CacheData.BlendAdditiveMeasureUnits.FirstOrDefault(c => c.Name == CostUtils.GetUnitName(add.Unit)),
                                        Amount = add.Quantity??0,
                                        BaseName = ""
                                    };
                            }

                            if (bs.BlendAdditiveSections == null)
                                bs.BlendAdditiveSections = new List<BlendAdditiveSection>();


                            bs.BlendAdditiveSections.Add(bas);
                        }
                    }

                    CostUtils.ProcessBlendSectionCost(bs, servicePointSbsId, freightCost, ref outputCostCollection, DateTime.Now, DateTime.Now, inputBlends.WithDetails);
                    }
                }
            //return new JsonResult(new { costCollection = outputCostCollection }, new System.Text.Json.JsonSerializerOptions() { PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase });
            return new JsonResult(new { costCollection = outputCostCollection }, new System.Text.Json.JsonSerializerOptions());
        }

        public ActionResult GetProductsCostByProgramId(int programId, int jobSectionId, bool byEndOfMonthWac)
        {
            Collection<CostUtils.OutputCost> outputCostCollection = new Collection<CostUtils.OutputCost>();
            DateTime programDate = DateTime.MinValue;
            string programNumber = "";
            string servicePointSbsId = "";
            var freightCost = 0.00;

            Sanjel.BusinessEntities.Programs.Program program = eServiceOnlineGateway.Instance.GetProgramById(programId);
            if (program != null)
            {
                programDate = program.ProgramGeneratedDate ?? DateTime.Now;
                programNumber = program.ProgramId;
                servicePointSbsId = CostUtils.DistrictSBS[(program.JobData != null && program.JobData.ServicePoint != null) ? program.JobData.ServicePoint.Id : -1] ?? "";
            }

            if (programDate != DateTime.MinValue && servicePointSbsId != "")
            {
                DateTime costAsOfDate = programDate.Date.AddMonths(byEndOfMonthWac ? 1 : 0);
                DateTime chemAsOfDate = DateTime.Now;
                Sanjel.BusinessEntities.Programs.ProgramPumpingJobSection jobSection = null;

                if (program.PumpingJobSections != null)
                {
                    foreach (Sanjel.BusinessEntities.Programs.ProgramPumpingJobSection js in program.PumpingJobSections)
                    {
                        if (js.Id == jobSectionId)
                        {
                            jobSection = js;
                            break;
                        }
                    }
                }

                if (
                    jobSection != null
                    && jobSection.ProductSection != null
                    && jobSection.ProductSection.BlendSections != null
                    && jobSection.ProductSection.BlendSections.Any()
                    && EServiceReferenceData.Data.BlendChemicalCollection != null
                    && CostUtils.GetPurchasePriceCollection(costAsOfDate) != null
                    )
                {
                    foreach (var blendSection in jobSection.ProductSection.BlendSections)
                    {
	                    BlendSection bs = GenericMapper.Map<Sanjel.BusinessEntities.Sections.Common.BlendSection, BlendSection>(blendSection);
                        CostUtils.ProcessBlendSectionCost(bs, servicePointSbsId, freightCost, ref outputCostCollection, costAsOfDate, chemAsOfDate, false);
                    }
                }
            }

            return new JsonResult(new { ProgramId = programId, JobSectionId = jobSectionId, ProgramNumber = programNumber, ProgramDate = programDate, costCollection = outputCostCollection }, new System.Text.Json.JsonSerializerOptions());
        }

        public ActionResult GetProductsCostByJobUniqueId(string uniqueId, bool byEndOfMonthWac)
        {
            Collection<CostUtils.OutputCost> outputCostCollection = new Collection<CostUtils.OutputCost>();
            DateTime jobDate = DateTime.MinValue;
            int jobNumber = 0;
            string servicePointSbsId = "";
            var freightCost = 0.00;

            Job job = eServiceOnlineGateway.Instance.GetJobByUniqueId(uniqueId);
            if (job != null)
            {
                jobDate = job.JobDateTime;
                jobNumber = job.JobNumber;
                servicePointSbsId = CostUtils.DistrictSBS[(job.JobData != null && job.JobData.ServicePoint != null) ? job.JobData.ServicePoint.Id : -1] ?? "";
            }

            if (jobDate != DateTime.MinValue && servicePointSbsId != "")
            {
                DateTime costAsOfDate = jobDate.Date.AddMonths(byEndOfMonthWac ? 1 : 0);
                DateTime chemAsOfDate = DateTime.Now;
                PumpingServiceReport sr = (PumpingServiceReport)eServiceOnlineGateway.Instance.GetServiceReportByUniqueId(uniqueId);

                if (
                    sr != null
                    && sr.PumpingSection != null
                    && sr.PumpingSection.ProductSection != null
                    && sr.PumpingSection.ProductSection.BlendSections != null
                    && sr.PumpingSection.ProductSection.BlendSections.Any()
                    && EServiceReferenceData.Data.BlendChemicalCollection != null
                    && CostUtils.GetPurchasePriceCollection(costAsOfDate) != null
                    )
                {
                    foreach (var blendSection in sr.PumpingSection.ProductSection.BlendSections)
                    {
	                    BlendSection bs = GenericMapper.Map<Sanjel.BusinessEntities.Sections.Common.BlendSection, BlendSection>(blendSection);

                        CostUtils.ProcessBlendSectionCost(bs, servicePointSbsId, freightCost, ref outputCostCollection, costAsOfDate, chemAsOfDate, false);
                    }
                }
            }
            return new JsonResult(new { jobUniqueId = uniqueId, JobNumber = jobNumber, JobDate = jobDate, costCollection = outputCostCollection }, new System.Text.Json.JsonSerializerOptions());
        }

        public string Get()
        {
            return "hello";
        }

        public IActionResult GetCost(string recipe)
        {

            List<Ingredient> ingredientList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Ingredient>>(recipe);
            string[] servicePoints = {"D603", "D612", "D617", "D604", "D616", "D615", "D606", "D607"};
            List<ServicePointCost> servicePointCosts = new List<ServicePointCost>();

            BlendChemical blendChemical = BuildFromScratch(ingredientList);

            foreach (var servicePoint in servicePoints)
            {
                var cost = CostCalculator.CalculateCost1(CacheData.BlendChemicals, CacheData.BlendRecipes, EServiceReferenceData.Data.PurchasePriceCollection, EServiceReferenceData.Data.ProductCollection, blendChemical,
                    0, true, servicePoint,
                    new BlendAdditiveMeasureUnit()
                    {
                        Id = 8, Name = "Tonne", Description = "t",
                        MeaureUnitType = new MeasureUnitType() {Id = 2, Name = "Weight"}
                    });

                servicePointCosts.Add(new ServicePointCost(){SP = servicePoint, TotalCost = cost});
            }

            string responseXml = Serializer.XmlSerialize(typeof(List<ServicePointCost>), servicePointCosts);

            return Content(responseXml);
        }

        [HttpGet]
//        [HttpGet("{recipe}/{districts}")]
        [ResponseCache(NoStore =true, Location =ResponseCacheLocation.None)]
        public ActionResult GetCostWithBreakdown(string recipe, string districts)
        {

            CostRecipe costRecipe = Newtonsoft.Json.JsonConvert.DeserializeObject<CostRecipe>(recipe);
//            string[] servicePoints = {"D603", "D612", "D617", "D604", "D616", "D615", "D606", "D607"};
            string[] servicePoints = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(districts);
            List<ServicePointCost> servicePointCosts = new List<ServicePointCost>();

            BlendChemical blendChemical = BuildRecipe(costRecipe);

            BlendAdditiveMeasureUnit unitType = 
                EServiceReferenceData.Data.BlendAdditiveMeasureUnitCollection.FirstOrDefault(
                p => p.Description == costRecipe.CostUnitPer);
            double baseQuantity = costRecipe.Ingredients[0].Quantity;
            string baseUnit = costRecipe.Ingredients[0].UOM;
            BlendChemical baseBlendChemical = EServiceReferenceData.Data.BlendChemicalCollection.FirstOrDefault(p => p.Name == costRecipe.Ingredients[0].Material);

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
                    ingredientCosts.Add(new IngredientCost(){Cost = ingredientCost.Value.Item1});
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
                    var baseBlendChemical = 
                        EServiceReferenceData.Data.BlendChemicalCollection.FirstOrDefault(p => p.Name.Trim() == costRecipe.Ingredients[0].Material.Trim());
                    BlendSection blendSection = new BlendSection();
                    blendSection.BlendFluidType = new BlendFluidType() { Id = baseBlendChemical.BaseBlendType.Id, Description = baseBlendChemical.BaseBlendType.Name};
                    blendSection.BlendAmountUnit = CacheData.BlendAdditiveMeasureUnits.FirstOrDefault(p => p.Abbreviation == costRecipe.Ingredients[0].UOM);
                    blendSection.Quantity = costRecipe.Ingredients[0].Quantity;
                    blendSection.BlendAdditiveSections = new List<BlendAdditiveSection>();

                    int i = 0;
                    foreach (var costRecipeIngredient in costRecipe.Ingredients)
                    {
                        i++;
                        if(i==1) continue;
                        var additiveBlendChemical = EServiceReferenceData.Data.BlendChemicalCollection.FirstOrDefault(p => p.Name == costRecipeIngredient.Material);

                        BlendAdditiveSection blendAdditiveSection = new BlendAdditiveSection();
                        blendAdditiveSection.BaseName = blendSection.BlendFluidType.Description;
                        blendAdditiveSection.AdditiveType = new AdditiveType()
                        {
                            Id = additiveBlendChemical.AdditiveType.Id,
                            Description = additiveBlendChemical.AdditiveType.Name
                        };
                        blendAdditiveSection.Amount = costRecipeIngredient.Quantity;
                        blendAdditiveSection.AdditiveAmountUnit = CacheData.BlendAdditiveMeasureUnits.FirstOrDefault(p => p.Name == costRecipeIngredient.UOM);
                        blendSection.BlendAdditiveSections.Add(blendAdditiveSection);
                    }

                    // Build Blend Section and use existing conversion to build blend chemical

                    //blendChemical = BlendSection.CovertToBlendChemicalFromBlendSection(
                    //    EServiceReferenceData.Data.BlendChemicalCollection, blendSection,
                    //    EServiceReferenceData.Data.AdditionMethodCollection,
                    //    EServiceReferenceData.Data.AdditiveBlendMethodCollection,
                    //    EServiceReferenceData.Data.BlendAdditiveMeasureUnitCollection, CacheData.BlendRecipes);
//                    var targetBlendSection = Mapper.Map<Sanjel.BusinessEntities.Sections.Common.BlendSection, BlendSection>(blendSection);
                    blendChemical = BlendCalculator.CovertToBlendChemicalFromBlendSection(
                        EServiceReferenceData.Data.BlendChemicalCollection, blendSection,
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
                var ingredientBlendChemical =EServiceReferenceData.Data.BlendChemicalCollection.FirstOrDefault(p => p.Name == ingredientList[i].Material);
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
                    Unit = EServiceReferenceData.Data.BlendAdditiveMeasureUnitCollection.FirstOrDefault(p=>p.Description==ingredientList[i].UOM)
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