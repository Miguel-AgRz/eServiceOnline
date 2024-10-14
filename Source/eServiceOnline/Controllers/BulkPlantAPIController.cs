/*using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using eServiceOnline.Data;
using eServiceOnline.Gateway;
using eServiceOnline.Models.ProductHaul;
using MetaShare.Common.Foundation.EntityBases;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.OLE.Interop;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Crew;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ResourceSchedule;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Products;

namespace eServiceOnline.Controllers
{
    public class BulkPlantAPIController : eServiceOnlineController
    {

        #region GET APIS
        public ActionResult GetScheduledProductHaulLoadList(int servicePointId)
        {
            List<ProductHaulLoad> producthoHaulLoads =
                eServiceWebContext.Instance.GetScheduledProductHaulLoadsByServicePoint(servicePointId);

            var data = BulkPlantLoads(producthoHaulLoads);

            return this.Json(data);

        }

        public ActionResult GetBlendingProductHaulLoadList(int servicePointId)
        {
            List<ProductHaulLoad> producthoHaulLoads =
                eServiceWebContext.Instance.GetBlendingProductHaulLoadsByServicePoint(servicePointId);

            var data = BulkPlantLoads(producthoHaulLoads);

            return this.Json(data);

        }
        public ActionResult GetBlendCompletedProductHaulLoadedList(int servicePointId)
        {
            List<ProductHaulLoad> producthoHaulLoads =
                eServiceWebContext.Instance.GetBlendCompletedProductHaulLoadsByServicePoint(servicePointId);

            var data = BulkPlantLoads(producthoHaulLoads);

            return this.Json(data);

        }
        public ActionResult GetLoadedProductHaulLoadedList(int servicePointId)
        {
            List<ProductHaulLoad> producthoHaulLoads =
                eServiceWebContext.Instance.GetLoadedProductHaulLoadsByServicePoint(servicePointId);

            var data = BulkPlantLoads(producthoHaulLoads);

            return this.Json(data);

        }

        public ActionResult GetProductHaulLoadList(int servicePointId)
        {
            List<ProductHaulLoad> producthoHaulLoads =
                eServiceWebContext.Instance.GetScheduledProductHaulLoadsByServicePoint(servicePointId);

            var data = BulkPlantLoads(producthoHaulLoads);

            return this.Json(data);
        }

        private static List<BulkPlantLoad> BulkPlantLoads(List<ProductHaulLoad> producthoHaulLoads)
        {
            List<BulkPlantLoad> data = new List<BulkPlantLoad>();

            foreach (var productHaulLoad in producthoHaulLoads)
            {
                var rigJob =eServiceOnlineGateway.Instance.GetRigJobByCallSheetNumber(productHaulLoad.CallSheetNumber);
                var bulkPlantLoad = new BulkPlantLoad()
                {
                    CallSheetNumber = productHaulLoad.CallSheetNumber.ToString(),
                    CustomerName = productHaulLoad.Customer.Name,
                    JobType = productHaulLoad.JobType.Name,
                    JobDateTime = productHaulLoad.JobDate.ToUniversalTime()
                        .ToString("yyyy\'-\'MM\'-\'dd\'T\'HH\':\'mm\':\'ss\'.\'fffZ"),
                    ExpectedOnLocationTime = productHaulLoad.ExpectedOnLocationTime.ToUniversalTime()
                        .ToString("yyyy\'-\'MM\'-\'dd\'T\'HH\':\'mm\':\'ss\'.\'fffZ"),
                    Blend = productHaulLoad.BlendChemical.Name,
                    Tonnage = productHaulLoad.BaseBlendWeight / 1000,
                    BinNumber = productHaulLoad.Bin.Name??"",
                    IsGoWithCrew = productHaulLoad.IsGoWithCrew,
                    ProductHaulLoadId = productHaulLoad.Id,
                    StartSNJLocation = productHaulLoad.ServicePoint.Name,
                    DestinationLocation = productHaulLoad.WellLocation,
                    Category = productHaulLoad.BlendCategory.Name,
                    RigName = rigJob?.Rig.Name,
                    LocationId = productHaulLoad.ServicePoint.Id

                };


                ProductHaul productHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulLoad.ProductHaul.Id);

                if (!productHaul.IsThirdParty)
                {
                    SanjelCrewSchedule schedule =
                        eServiceOnlineGateway.Instance.GetCrewScheduleById(productHaul.Schedule?.Id ?? 0);
                    SanjelCrew crew = eServiceOnlineGateway.Instance.GetCrewById(schedule?.SanjelCrew?.Id ?? 0, true);
                    bulkPlantLoad.BulkerUnit = crew?.SanjelCrewTruckUnitSection.ElementAtOrDefault(0)?.TruckUnit.UnitNumber;
                    bulkPlantLoad.Driver = Utility.PreferedName(crew?.SanjelCrewWorkerSection.ElementAtOrDefault(0)?.Worker);
                }
                else
                {
                    ThirdPartyBulkerCrewSchedule schedule =
                        eServiceOnlineGateway.Instance.GetThirdPartyBulkerCrewSchedule(productHaul.Schedule?.Id ?? 0);
                    ThirdPartyBulkerCrew crew =
                        eServiceOnlineGateway.Instance.GetThirdPartyBulkerCrewById(schedule?.ThirdPartyBulkerCrew?.Id ?? 0);
                    bulkPlantLoad.BulkerUnit = crew?.ContractorCompany.Name;
                    bulkPlantLoad.Driver = crew?.SupplierContactName;
                }

                data.Add(bulkPlantLoad);
            }

            return data;
        }

        internal class BulkPlantLoad
        {
            public string CallSheetNumber { get; set; }
            public string CustomerName { get; set; }
            public string JobType { get; set; }
            public string JobDateTime { get; set; }
            public string ExpectedOnLocationTime { get; set; }
            public string Blend { get; set; }
            public double Tonnage { get; set; }
            public string BulkerUnit { get; set; }
            public string Driver { get; set; }
            public string BinNumber { get; set; }
            public bool IsGoWithCrew { get; set; }
            public int ProductHaulLoadId { get; set; }
            public string StartSNJLocation { get; set; }
            public string DestinationLocation { get; set; }
            public string Category { get; set; }
            public string RigName { get; set; }
            public int LocationId { get; set; }
        }

        public ActionResult GetBulkDensityByProduct(string productName)
        {
            BlendChemical blendChemical = CacheData.BlendChemicals.FirstOrDefault(p => p.Name == productName);
            BulkDensity item = new BulkDensity();
            if (blendChemical != null)
            {
                item.Product = blendChemical.Name;
                item.ProductBulkDensity = blendChemical.BulkDensity.ToString("#.##");
                //Current we don't have unit of measure, it is using kg/m3 as default
                item.ProductUnit = "kg/m3";
            }


            return this.Json(item);
        }

        public ActionResult GetAllBulkDensities()
        {
            List<BulkDensity> data = new List<BulkDensity>();
            foreach (BlendChemical blendChemical in CacheData.BlendChemicals)
            {
                BulkDensity item = new BulkDensity();
                item.Product = blendChemical.Name;
                item.ProductBulkDensity = blendChemical.BulkDensity.ToString("#.##");
                //Current we don't have unit of measure, it is using kg/m3 as default
                item.ProductUnit = "kg/m3";

                data.Add(item);
            }

            return this.Json(data);
        }

        internal class BulkDensity
        {
            public string Product { get; set; }
            public string ProductBulkDensity { get; set; }
            public string ProductUnit { get; set; }
        }

        internal class BlendChemicalDetail
        {
            public string ProductName { get; set; }
            public double MixWater { get; set; }
            public double Yield { get; set; }
            public double StandardDensity { get; set; }
            public double BulkDensity { get; set; }
            public double SpecificGravity { get; set; }
        }
        public ActionResult GetAllBlendChemicals()
        {
            List<BlendChemicalDetail> data = new List<BlendChemicalDetail>();
            foreach (BlendChemical blendChemical in CacheData.BlendChemicals)
            {
                var blendChemicalDetail = new BlendChemicalDetail();
                blendChemicalDetail.ProductName = blendChemical.Name;
                blendChemicalDetail.MixWater = blendChemical.MixWater;
                blendChemicalDetail.StandardDensity = blendChemical.Density;
                blendChemicalDetail.Yield = blendChemical.Yield;
                blendChemicalDetail.BulkDensity = blendChemical.BulkDensity;
                blendChemicalDetail.SpecificGravity = blendChemical.SpecificGravity;

                data.Add(blendChemicalDetail);
            }

            return this.Json(data);

        }

        public ActionResult GetAllOpenJobs()
        {
            List<string> data = new List<string>();

            List<RigJob> rigJobs = eServiceOnlineGateway.Instance.GetRigJobs();
            foreach (var rigJob in rigJobs)
            {
                if(rigJob.CallSheetNumber != 0 && rigJob.JobLifeStatus != JobLifeStatus.Completed && rigJob.JobLifeStatus != JobLifeStatus.Alerted && rigJob.JobLifeStatus != JobLifeStatus.Canceled && rigJob.JobLifeStatus != JobLifeStatus.None  && rigJob.JobLifeStatus != JobLifeStatus.Deleted)
                    data.Add(rigJob.CallSheetNumber.ToString());
            }
            
            return this.Json(data);
        }

        /*
        public ActionResult GetProductLoadList(int servicePointId)
        {
            List<ProductHaulLoad> producthoHaulLoads = null;
            if (servicePointId == 0)
                producthoHaulLoads = eServiceOnlineGateway.Instance.GetScheduledProductHaulLoads();
            else
                producthoHaulLoads = eServiceOnlineGateway.Instance.GetScheduledProductHaulLoadsByServicePoint(servicePointId);

            var data = BulkPlantLoads(producthoHaulLoads);

            return this.Json(data);
        }
        #1#

        /*
        public ActionResult GetScheduledProductLoadList(int servicePointId)
        {
            List<ProductHaulLoad> producthoHaulLoads = null;
            if (servicePointId != 0)
                producthoHaulLoads = eServiceOnlineGateway.Instance.GetScheduledProductHaulLoadsByServicePoint(servicePointId);

            var data = BulkPlantLoads(producthoHaulLoads);

            return this.Json(data);
        }
        #1#

        /*public ActionResult GetBlendingProductLoadList(int servicePointId)
        {
            List<ProductHaulLoad> producthoHaulLoads = null;
            if (servicePointId != 0)
                producthoHaulLoads = eServiceWebContext.Instance.GetBlendingProductHaulLoadsByServicePoint(servicePointId);

            var data = BulkPlantLoads(producthoHaulLoads);

            return this.Json(data);

        }#1#

        /*public ActionResult GetLoadedProductLoadList(int servicePointId)
        {
            List<ProductHaulLoad> producthoHaulLoads = null;
            if (servicePointId != 0)
                producthoHaulLoads = eServiceWebContext.Instance.GetLoadedProductHaulLoadsByServicePoint(servicePointId);

            var data = BulkPlantLoads(producthoHaulLoads);

            return this.Json(data);

        }#1#

        public ActionResult GetLoadSheet(int productLoadId)
        {
            ProductHaulLoad productHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productLoadId,true);
            if (productHaulLoad == null) return this.Json("Error: Invalid Product Haul Load Id");
            BlendChemical blendChemical =
                CacheData.BlendChemicals.FirstOrDefault(p => p.Id == productHaulLoad.BlendChemical.Id);

            var baseChemical = GetBaseChemical(blendChemical);
            

            LoadSheet data = new LoadSheet();
            data.MixWater = productHaulLoad.MixWater;
            data.MixWaterUnit = "m3/t";
            data.Yield = productHaulLoad.Yield;
            data.YieldUnit = "m3/t";
            data.Density = productHaulLoad.Density;
            data.DensityUnit = "kg/m3";
            data.SackWeight = productHaulLoad.SackWeight;
            data.SackWeightUnit = "kg/t";
            data.Products = new List<LoadSheetDetail>();
            data.Details = GetBlendDetails(productHaulLoad.BlendSectionId);


            /*
            var productList =
                ProductHaulModel.ProductLoadList(
                    new Collection<ProductLoadSection>(productHaulLoad.AllProductLoadList));
            #1#
            foreach (var productLoadSection in productHaulLoad.AllProductLoadList)
            {
                LoadSheetDetail loadSheetDetail = new LoadSheetDetail();
                loadSheetDetail.Product = productLoadSection.BlendChemical.Name;
                loadSheetDetail.ProductWeight = productLoadSection.RequiredAmount;
                loadSheetDetail.ProductWeightUnit = productLoadSection.BlendAdditiveMeasureUnit.Abbreviation;
                loadSheetDetail.BlendMode = productLoadSection.AdditiveBlendMethod.Id;
                if (productLoadSection.BlendChemical.Name.Equals(baseChemical?.Name))
                {
                    loadSheetDetail.IsBaseCement = true;
                }
                loadSheetDetail.IsFromBase = productLoadSection.IsFromBase;
                
                data.Products.Add(loadSheetDetail);
            }

            return this.Json(data);

        }

        private List<BlendDetail> GetBlendDetails(int blendSectionId)
        {
            var blendSection = eServiceWebContext.Instance.GetBlendSectionByBlendSectionId(blendSectionId);
            var blendDetails = new List<BlendDetail>();
            BlendDetail baseBlendDetail = new BlendDetail(){Product = blendSection.BlendFluidType.Name, Amount = blendSection.Quantity??0, AmountUnit = blendSection.BlendAmountUnit.Abbreviation, BlendMode = 1, IsBaseBlend = true };
            blendDetails.Add(baseBlendDetail);
            foreach (var blendSectionBlendAdditiveSection in blendSection.BlendAdditiveSections)
            {
                BlendDetail additiveDetail = new BlendDetail(){Product = blendSectionBlendAdditiveSection.AdditiveType.Name, Amount = blendSectionBlendAdditiveSection.Amount??0, AmountUnit = blendSectionBlendAdditiveSection.AdditiveAmountUnit.Abbreviation, BlendMode = blendSectionBlendAdditiveSection.AdditionMethod.Id - 1, IsBaseBlend = false };
                blendDetails.Add(additiveDetail);
            }

            return blendDetails;
        }

        private static BlendChemical GetBaseChemical(BlendChemical blendChemical)
        {
            List<BlendChemicalSection> blendChemicalSections = null;
            if (blendChemical != null)
            {
                if (blendChemical.Name.Contains("+ Additives"))
                {
                    BlendChemical baseBlend = blendChemical?.BlendRecipe.BlendChemicalSections.Find(p => p.IsBaseBlend)
                        .BlendChemical;
                    baseBlend = CacheData.BlendChemicals.FirstOrDefault(p => p.Id == baseBlend?.Id);
                    if (baseBlend != null)
                    {
                        blendChemicalSections = blendChemical.BlendRecipe.BlendChemicalSections;
                    }
                }
                else
                {
                    blendChemicalSections = blendChemical.BlendRecipe.BlendChemicalSections;
                }
            }

            BlendChemical baseChemical = blendChemicalSections?.FirstOrDefault(p => p.IsBaseBlend)?.BlendChemical;

            if (baseChemical != null && baseChemical.BlendRecipe != null && baseChemical.BlendRecipe.BlendChemicalSections.Count>0)
            {
                baseChemical = GetBaseChemical(baseChemical);
            }
            return baseChemical;
        }

        internal class LoadSheet
        {
            public List<BlendDetail> Details { set; get; }
            public double MixWater { set; get; }
            public string MixWaterUnit { set; get; }
            public double Yield { set; get; }
            public string YieldUnit { set; get; }
            public double Density { set; get; }
            public string DensityUnit { set; get; }
            public double SackWeight { set; get; }
            public string SackWeightUnit { set; get; }
            public List<LoadSheetDetail> Products { set; get; }

        }

        internal class LoadSheetDetail
        {
            public string Product { set; get; }
            public double ProductWeight { set; get; }
            public string ProductWeightUnit { set; get; }
            //BlendMode: 1-Preblend, 2-PreHydrate, 3-AddOnFly
            public int BlendMode { set; get; }
            public bool IsBaseCement { set; get; }
            public bool IsFromBase { set; get; }
        }

        internal class BlendDetail
        {
            public string Product { set; get; }
            public double Amount { set; get; }
            public string AmountUnit { get; set; }
            public int BlendMode { get; set; }
            public bool IsBaseBlend { get; set; }
        }
        #endregion GET APIS

        #region SET APIS

        public ActionResult SetProductHaulLoadBlending(int productLoadId)
        {
            int success = eServiceWebContext.Instance.SetProductHaulLoadBlending(productLoadId);
            return this.Json(success);
        }

        public ActionResult SetProductHaulLoadLoaded(int productLoadId)
        {
            int success = eServiceWebContext.Instance.SetProductHaulLoadLoaded(productLoadId);
            return this.Json(success);
        }

        public ActionResult SetProductHaulLoadBlendCompleted(int productLoadId) 
        {
            int success = eServiceWebContext.Instance.SetProductHaulLoadBlendCompleted(productLoadId);
            return this.Json(success);
        }

        #endregion SET APIS      
    }
}*/