using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlTypes;
using System.Linq;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using eServiceOnline.BusinessProcess;
using eServiceOnline.Gateway;
using MetaShare.Common.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using PnP.Framework.Entities;
using Sanjel.Common.ApiServices.CommonService;
using Sesi.SanjelData.Entities.BusinessEntities.BulkPlant;
using Sesi.SanjelData.Entities.BusinessEntities.Lab;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Crew;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ResourceSchedule;
using Sesi.SanjelData.Entities.BusinessEntities.WellSite;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Operation;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Products;
using Sesi.SanjelData.Entities.Common.BusinessEntities.WellSite;
using Sesi.SanjelData.Services.Interfaces.BusinessEntities.Operation.Schedule.ProductSchedule;
using Sesi.SanjelData.Services.Interfaces.Common.BusinessEntities.Products;
using JsonProperty = System.Text.Json.JsonProperty;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace eServiceOnlineAPI.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class BulkPlantAPIController : ControllerBase
    {

        #region POST APIS

        [HttpPost]
        [RequestSizeLimit(29715200)]
        public RequestResult PostTest(string json)
        {
            return new RequestResult{IsSucceed = true, Message = "success", Data = json};
//            return new JsonResult(json);
        }

        [HttpPost]
        public  RequestResult PostTestJson(TestJson json)
        {
            return new RequestResult{IsSucceed = true, Message = "success", Data = json.Name };
        }

        public class TestJson
        {
            public string Name { get; set; }
        }

        // http://localhost:52346/bulkplantapi/PostBlendTankLog
        [HttpPost]
        public RequestResult PostBlendTankLog(BlendTankLog blendTankLog)
        {
            //string blendTankLogStr = GetBlendTankLog();
            var rtn = 0;

            rtn = eServiceOnlineGateway.Instance.CreateBlendTankLog(blendTankLog);

            if (rtn == 1)
            {
                var json = eServiceOnlineGateway.Instance.GetBlendTankLogById(blendTankLog.Id);
                if (json != null)
                    return new RequestResult
                    {
                        IsSucceed = true,
                        Message = "success",
                        Data = JsonConvert.SerializeObject(json,
                        new JsonSerializerSettings() { ContractResolver = GetBlendLogResolver() })
                    };
                else
                    return new RequestResult { IsSucceed = false, Message = "failed", Data = new JsonResult(null) };
            }
            else
            {
                return new RequestResult { IsSucceed = false, Message = "failed", Data = new JsonResult(null) };
            }
        }
/*
        public string GetBlendTankLog()
        {
            BlendTankLog blendTankLog = new BlendTankLog()
            {
                //Id = 
                EventTimestamp = DateTime.Now,
                EventName = "TestEvent",
                BlendName = "TestBlend",
                ProductHaulLoadId = 123,
                CutIndex = 1,
                BlendTankWeight = 123.45,
                Rate = 32.15
            };

            return JsonConvert.SerializeObject(blendTankLog,
                new JsonSerializerSettings() { ContractResolver = GetBlendLogResolver() });
        }
*/

        // http://localhost:52346/bulkplantapi/PostBlendLog
        [HttpPost]
        public  RequestResult PostBlendLog(BlendLog blendLog)
        {
            var rtn = 0;
            var exsitingBlendLog = eServiceOnlineGateway.Instance.GetBlendLogByProductHaulLoadId(blendLog.ProductHaulLoadId);
            if(exsitingBlendLog == null)
                  rtn = eServiceOnlineGateway.Instance.CreateBlendLog(blendLog);
            else
            {
                exsitingBlendLog.BlendServicePointId = blendLog.BlendServicePointId;
                exsitingBlendLog.BlendServicePointName = blendLog.BlendServicePointName;
                exsitingBlendLog.CallSheetNumber = blendLog.CallSheetNumber;
                exsitingBlendLog.MtsNumber = blendLog.MtsNumber;
                exsitingBlendLog.SampleNumber = blendLog.SampleNumber;
                exsitingBlendLog.StartTime = blendLog.StartTime;
                exsitingBlendLog.EndTime = blendLog.EndTime;
                exsitingBlendLog.TotalBlendTime = blendLog.TotalBlendTime;
                exsitingBlendLog.CurrentLocation = blendLog.CurrentLocation;
                exsitingBlendLog.BlendDate = blendLog.BlendDate;
                exsitingBlendLog.Status = blendLog.Status;
                exsitingBlendLog.TransferDate = blendLog.TransferDate;
                exsitingBlendLog.ClientCompany = blendLog.ClientCompany;
                exsitingBlendLog.StoragePod1Total = blendLog.StoragePod1Total;
                exsitingBlendLog.StoragePod2Total = blendLog.StoragePod2Total;
                exsitingBlendLog.StoragePod3Total = blendLog.StoragePod3Total;
                exsitingBlendLog.StoragePod4Total = blendLog.StoragePod4Total;
                exsitingBlendLog.BlendTemp = blendLog.BlendTemp;
                exsitingBlendLog.BulkPlantOperator = blendLog.BulkPlantOperator;
                exsitingBlendLog.ClientSystemStamp = blendLog.ClientSystemStamp;
                exsitingBlendLog.ClientVersionStamp = blendLog.ClientVersionStamp;
                exsitingBlendLog.BulkPlantId = blendLog.BulkPlantId;
                exsitingBlendLog.BulkPlantName = blendLog.BulkPlantName;

                rtn = eServiceOnlineGateway.Instance.UpdateBlendLog(exsitingBlendLog);
            }

            if (rtn == 1)
            {
                var json = eServiceOnlineGateway.Instance.GetBlendLogByProductHaulLoadId(blendLog.ProductHaulLoadId);
                if(json != null)
                    return new RequestResult {IsSucceed = true, Message = "success", Data = JsonConvert.SerializeObject(json,
                        new JsonSerializerSettings() {ContractResolver = GetBlendLogResolver() })};
                else
                    return new RequestResult {IsSucceed = false, Message = "failed", Data = new JsonResult(null)};
            }
            else
            {
                return new RequestResult {IsSucceed = false, Message = "failed", Data = new JsonResult(null)};
            }
        }
        // http://localhost:52346/bulkplantapi/PostBlendCut
        [HttpPost]
        public  RequestResult PostBlendCut(BlendCut blendCut)
        {
            BlendCut originalBlendCut =
                eServiceOnlineGateway.Instance.GetBlendCutByProductHaulLoadAndBlendCutSequenceNumber(
                    blendCut.ProductHaulLoadId, blendCut.SequenceNumber);
            if (originalBlendCut == null)
            {
               eServiceOnlineGateway.Instance.CreateBlendCut(blendCut);
            }
            else
            {
                originalBlendCut.ActualWeight = blendCut.ActualWeight;
                originalBlendCut.BlendTemp = blendCut.BlendTemp;
                originalBlendCut.StartTime = blendCut.StartTime;
                originalBlendCut.EndTime = blendCut.EndTime;
                originalBlendCut.TargetWeight = blendCut.TargetWeight;
                originalBlendCut.TransferCount = blendCut.TransferCount;
                originalBlendCut.BulkPlantOperator = blendCut.BulkPlantOperator;
                foreach (var cutDetail in originalBlendCut.CutDetails)
                {
                    var newDetail = blendCut.CutDetails.Find(p => p.SequenceNumber == cutDetail.SequenceNumber);
                    if (newDetail == null) originalBlendCut.CutDetails.Remove(cutDetail);
                }
                foreach (var blendCutCutDetail in blendCut.CutDetails)
                {
                    var originalCutDetail = originalBlendCut.CutDetails.Find(p => p.SequenceNumber == blendCutCutDetail.SequenceNumber);
                    if(originalCutDetail == null)
                    {
                        originalBlendCut.CutDetails.Add(blendCutCutDetail);
                    }
                    else
                    {
                        originalCutDetail.BlendChemicalId = blendCutCutDetail.BlendChemicalId;
                        originalCutDetail.BlendChemicalName = blendCutCutDetail.BlendChemicalName;
                        originalCutDetail.ActualWeight = blendCutCutDetail.ActualWeight;
                        originalCutDetail.StartTime = blendCutCutDetail.StartTime;
                        originalCutDetail.StopTime = blendCutCutDetail.StopTime;
                        originalCutDetail.LotNumber = blendCutCutDetail.LotNumber;
                        originalCutDetail.ReportedWeight = blendCutCutDetail.ReportedWeight;
                        originalCutDetail.SourceLocation = blendCutCutDetail.SourceLocation;
                        originalCutDetail.TargetWeight = blendCutCutDetail.TargetWeight;
                    }
                }
                eServiceOnlineGateway.Instance.UpdateBlendCut(originalBlendCut);
            }

            var updatedBlendCut = eServiceOnlineGateway.Instance.GetBlendCutByProductHaulLoadAndBlendCutSequenceNumber(blendCut.ProductHaulLoadId, blendCut.SequenceNumber);

            if(updatedBlendCut != null)
                return new RequestResult {IsSucceed = true, Message = "success", Data = SerializeBlendCut(updatedBlendCut)};
            else
                return new RequestResult {IsSucceed = false, Message = "failed", Data = new JsonResult(null)};
        }

        private static string SerializeVersionedEntity(object updatedBlendCut)
        {
	        var resolver = new PropertyRenameAndIgnoreSerializerContractResolver();
	        IgnoreCommonProperties(resolver);

            return JsonConvert.SerializeObject(updatedBlendCut,
		        new JsonSerializerSettings() { ContractResolver = GetBlendCutResolver() });
        }



        private static string SerializeBlendCut(BlendCut updatedBlendCut)
        {
            return JsonConvert.SerializeObject(updatedBlendCut,
                new JsonSerializerSettings() {ContractResolver = GetBlendCutResolver() });
        }

        private static PropertyRenameAndIgnoreSerializerContractResolver GetBlendCutResolver()
        {
            var resolver = new PropertyRenameAndIgnoreSerializerContractResolver();
            resolver.IgnoreProperty(typeof(CutDetail), "blendCut");
            IgnoreCommonProperties(resolver);
            return resolver;
        }
        private static PropertyRenameAndIgnoreSerializerContractResolver GetBulkPlantLoadResolver()
        {
            var resolver = new PropertyRenameAndIgnoreSerializerContractResolver();
            resolver.IgnoreProperty(typeof(BulkPlantLoad), "bulkPlantLoad");
            return resolver;
        }
        private static PropertyRenameAndIgnoreSerializerContractResolver GetBlendLogResolver()
        {
            var resolver = new PropertyRenameAndIgnoreSerializerContractResolver();
            IgnoreCommonProperties(resolver);
            return resolver;
        }

        private static void IgnoreCommonProperties(PropertyRenameAndIgnoreSerializerContractResolver resolver)
        {
            resolver.IgnoreProperty(typeof(ObjectVersion), "version", "systemId", "modifiedUserId", "modifiedUserName",
                "modifiedDateTime", "operationType", "effectiveStartDateTime", "effectiveEndDateTime");
            resolver.IgnoreProperty(typeof(MetaShare.Common.Core.Entities.Common), "ownerId", "entityStatus", "name", "description");
            resolver.IgnoreProperty(typeof(Common<int>), "id");
        }

        #endregion POST APIS


        #region GET APIS

        //Sample: http://localhost:5000/BulkPlantAPI/GetBulkPlantList
        public ActionResult GetBulkPlantList()
        {
            List<Rig> bulkPlants = eServiceOnlineGateway.Instance.GetBulkPlants();
            List<BulkPlant> data = new List<BulkPlant>();
            foreach (var bulkPlant in bulkPlants)
            {
                data.Add(new BulkPlant(){Id=bulkPlant.Id,Name=bulkPlant.Name});
            }
            return new JsonResult(data, new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }

        internal class BulkPlant
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        //Sample: http://localhost:5000/BulkPlantAPI/GetBulkPlantStorageListByBulkPlant?bulkPlantId=2340
        public ActionResult GetBulkPlantStorageListByBulkPlant(int bulkPlantId)
        {
            List<BinInformation> binInformations = eServiceOnlineGateway.Instance.GetBinInformationByQuery(binInformation=>binInformation.Rig.Id == bulkPlantId);
            List<BulkPlantStorage> data = new List<BulkPlantStorage>();
            foreach (var storage in binInformations)
            {
                data.Add(new BulkPlantStorage()
                {
                    StorageId = storage.Id, StorageName = storage.Name, BinId = storage.Bin.Id, BinNumber = storage.Bin.Name, PodIndex = storage.PodIndex, Volume = storage.Volume, ProductName = storage.BlendChemical.Description, Quantity = storage.Quantity, Capacity = storage.Capacity
                });
            }
            return new JsonResult(data, new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }
        internal class BulkPlantStorage
        {
            public int StorageId { get; set; }
            public string StorageName { get; set; }
            public int BinId { get; set; }
            public string BinNumber { get; set; }
            public int PodIndex { get; set; }
            public double Capacity { get; set; }
            public double Quantity { get; set; }
            public string ProductName { get; set; }
            public double Volume { get; set; }
        }
        //Sample: http://localhost:5001/BulkPlantAPI/GetScheduledProductHaulLoadList?servicePointId=61
        public ActionResult GetScheduledProductHaulLoadList(int servicePointId)
        {
            List<ProductHaulLoad> producthoHaulLoads =
                eServiceOnlineGateway.Instance.GetProductHaulLoadsByServicePointAndStatus(servicePointId, ProductHaulLoadStatus.Scheduled);

            var data = BulkPlantLoads(producthoHaulLoads);

            return new JsonResult(data, new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        }
        //Sample: http://localhost:5001/BulkPlantAPI/GetScheduledProductHaulLoadListByBulkPlant?bulkPlantId=2340
        public ActionResult GetScheduledProductHaulLoadListByBulkPlant(int bulkPlantId)
        {
            List<ProductHaulLoad> producthoHaulLoads =
                eServiceOnlineGateway.Instance.GetProductHaulLoadsByBulkPlantAndStatus(bulkPlantId, ProductHaulLoadStatus.Scheduled);

            var data = BulkPlantLoads(producthoHaulLoads);

            return new JsonResult(data, new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, PropertyNamingPolicy = JsonNamingPolicy.CamelCase});

        }

        //Sample: http://localhost:52346/BulkPlantAPI/GetBlendingProductHaulLoadList?servicePointId=61
        public ActionResult GetBlendingProductHaulLoadList(int servicePointId)
        {
            List<ProductHaulLoad> producthoHaulLoads =
                eServiceOnlineGateway.Instance.GetProductHaulLoadsByServicePointAndStatus(servicePointId, ProductHaulLoadStatus.Blending);

            var data = BulkPlantLoads(producthoHaulLoads);

            return new JsonResult(data, new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        }
        //Sample: http://localhost:52346/BulkPlantAPI/GetBlendingProductHaulLoadListByBulkPlant?bulkPlantId=2340
        public ActionResult GetBlendingProductHaulLoadListByBulkPlant(int bulkPlantId)
        {
            List<ProductHaulLoad> producthoHaulLoads =
                eServiceOnlineGateway.Instance.GetProductHaulLoadsByBulkPlantAndStatus(bulkPlantId, ProductHaulLoadStatus.Blending);

            var data = BulkPlantLoads(producthoHaulLoads);

            return new JsonResult(data, new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        }
        //Sample: http://localhost:52346/BulkPlantAPI/GetBlendCompletedProductHaulLoadList?servicePointId=61
        public ActionResult GetBlendCompletedProductHaulLoadList(int servicePointId)
        {
            List<ProductHaulLoad> producthoHaulLoads =
                eServiceOnlineGateway.Instance.GetProductHaulLoadsByServicePointAndStatus(servicePointId, ProductHaulLoadStatus.BlendCompleted);

            var data = BulkPlantLoads(producthoHaulLoads);

            return new JsonResult(data, new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }
        //Sample: http://localhost:52346/BulkPlantAPI/GetBlendCompletedProductHaulLoadListByBulkPlant?bulkPlantId=2340
        public ActionResult GetBlendCompletedProductHaulLoadListByBulkPlant(int bulkPlantId)
        {
            List<ProductHaulLoad> producthoHaulLoads =
                eServiceOnlineGateway.Instance.GetProductHaulLoadsByServicePointAndStatus(bulkPlantId, ProductHaulLoadStatus.BlendCompleted);

            var data = BulkPlantLoads(producthoHaulLoads);

            return new JsonResult(data, new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }
        //Sample: http://localhost:52346/BulkPlantAPI/GetLoadedProductHaulLoadList?servicePointId=61
        public ActionResult GetLoadedProductHaulLoadList(int servicePointId)
        {
            List<ProductHaulLoad> producthoHaulLoads =
                eServiceOnlineGateway.Instance.GetProductHaulLoadsByServicePointAndStatus(servicePointId, ProductHaulLoadStatus.Loaded);

            var data = BulkPlantLoads(producthoHaulLoads);

            return new JsonResult(data, new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }
        //Sample: http://localhost:52346/BulkPlantAPI/GetProductHaulLoadById?productHaulLoadId=52814
        public ActionResult GetProductHaulLoadById(int productHaulLoadId)
        {
	        ProductHaulLoad productHaulLoad =
		        eServiceOnlineGateway.Instance.GetProductHaulLoadById(productHaulLoadId);
	        List<ProductHaulLoad> productHaulLoads = new List<ProductHaulLoad>();
            productHaulLoads.Add(productHaulLoad);

            var data = BulkPlantLoads(productHaulLoads);

	        return new JsonResult(data, new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }
        //Sample: http://localhost:52346/BulkPlantAPI/GetLoadedProductHaulLoadListByBulkPlant?bulkPlantId=2340
        public ActionResult GetLoadedProductHaulLoadListByBulkPlant(int bulkPlantId)
        {
            List<ProductHaulLoad> producthoHaulLoads =
                eServiceOnlineGateway.Instance.GetProductHaulLoadsByServicePointAndStatus(bulkPlantId, ProductHaulLoadStatus.Loaded);

            var data = BulkPlantLoads(producthoHaulLoads);

            return new JsonResult(data, new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }

        //Sample: http://localhost:52348/BulkPlantAPI/GetProductHaulLoadList?servicePointId=61
        public ActionResult GetProductHaulLoadList(int servicePointId)
        {
            List<ProductHaulLoad> producthoHaulLoads =
                eServiceOnlineGateway.Instance.GetProductHaulLoadsByServicePointAndStatus(servicePointId, ProductHaulLoadStatus.Scheduled);

            var data = BulkPlantLoads(producthoHaulLoads);

            return new JsonResult(data, new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }
        //Sample: http://localhost:52346/BulkPlantAPI/GetProductHaulLoadListByBulkPlant?bulkPlantId=2346
        public ActionResult GetProductHaulLoadListByBulkPlant(int bulkPlantId)
        {
            List<ProductHaulLoad> producthoHaulLoads = null;
            if (bulkPlantId == 0)
                producthoHaulLoads = eServiceOnlineGateway.Instance.GetScheduledProductHaulLoads();
            else
                producthoHaulLoads = eServiceOnlineGateway.Instance.GetScheduledProductHaulLoadsByBulkPlant(bulkPlantId);

            var data = BulkPlantLoads(producthoHaulLoads);

            return new JsonResult(data, new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        }
        //Sample: http://localhost:52346/BulkPlantAPI/GetBlendCut?productHaulLoadId=50277&cutSequencenumber
        public string GetBlendCut(int productHaulLoadId, int cutSequencenumber)
        {
	        BlendCut blendCut = eServiceOnlineGateway.Instance.GetBlendCutByProductHaulLoadAndBlendCutSequenceNumber(productHaulLoadId, cutSequencenumber);

	        return  SerializeBlendCut(blendCut);
        }


        private static List<BulkPlantLoad> BulkPlantLoads(List<ProductHaulLoad> productHaulLoads)
        {
            List<BulkPlantLoad> data = new List<BulkPlantLoad>();

            List<int> productLoadIds = productHaulLoads.Select(p => p.Id).Distinct().ToList();
//            List<ProductHaul> productHauls = eServiceOnlineGateway.Instance.GetProductHaulByIds(productHaulIds);

            IShippingLoadSheetService shippingLoadSheetService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IShippingLoadSheetService>();
            if (shippingLoadSheetService == null) throw new Exception("podLoadService must be registered in service factory");

            var shippingLoadSheets =
                shippingLoadSheetService.SelectByJoin(p => productLoadIds.Contains(p.ProductHaulLoad.Id),
                    p => p.BlendUnloadSheets != null && p.ProductHaul !=null);
            foreach (var productHaulLoad in productHaulLoads)
            {
                /*
                var rigJob =
                    eServiceOnlineGateway.Instance.GetRigJobByCallSheetNumber(productHaulLoad.CallSheetNumber);
                if (rigJob == null) continue;
                
                if (rigJob.JobLifeStatus == JobLifeStatus.Canceled ||
                    rigJob.JobLifeStatus == JobLifeStatus.Deleted || rigJob.JobLifeStatus == JobLifeStatus.Alerted ||
                    rigJob.JobLifeStatus == JobLifeStatus.Completed) continue;
                    */
                
//                ProductHaul productHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulLoad.ProductHaul.Id, false);

//                ProductHaul productHaul = productHauls.Find(p => p.Id == productHaulLoad.ProductHaul.Id);
//                if (productHaulLoad.ProductHaulLoadLifeStatus == ProductHaulLoadStatus.OnLocation) continue;

/*                IShippingLoadSheetService shippingLoadSheetService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IShippingLoadSheetService>();
                if (shippingLoadSheetService == null) throw new Exception("podLoadService must be registered in service factory");


                var shippingLoadSheet =
                    shippingLoadSheetService.SelectByJoin(p => p.ProductHaulLoad.Id == productHaulLoad.Id,
                        p => p.BlendUnloadSheets != null && p.ProductHaul !=null).FirstOrDefault();
*/

                var bulkPlantLoad = new BulkPlantLoad()
                {
                    CallSheetNumber = productHaulLoad.CallSheetNumber != 0? productHaulLoad.CallSheetNumber.ToString(): productHaulLoad.ProgramId,
                    CustomerName = productHaulLoad.Customer.Name,
                    JobType = productHaulLoad.JobType.Name,
                    JobDateTime = productHaulLoad.JobDate.ToUniversalTime()
                        .ToString("yyyy\'-\'MM\'-\'dd\'T\'HH\':\'mm\':\'ss\'.\'fffZ"),
                    ExpectedOnLocationTime = productHaulLoad.ExpectedOnLocationTime.ToUniversalTime()
                        .ToString("yyyy\'-\'MM\'-\'dd\'T\'HH\':\'mm\':\'ss\'.\'fffZ"),
                    Blend = productHaulLoad.BlendChemical.Name,
                    BlendChemicalId = productHaulLoad.BlendChemical.Id,
                    Tonnage = productHaulLoad.BaseBlendWeight / 1000,
                    BinNumber = productHaulLoad.Bin.Name ?? "",
                    IsGoWithCrew = productHaulLoad.IsGoWithCrew,
                    ProductHaulLoadId = productHaulLoad.Id,
                    StartSNJLocation = productHaulLoad.BulkPlant.Name,
                    DestinationLocation = productHaulLoad.WellLocation,
                    Category = productHaulLoad.BlendCategory.Name,
                    RigName = productHaulLoad.Rig!=null?productHaulLoad.Rig.Name:"",
                    LocationId = productHaulLoad.ServicePoint.Id,
                    ProgramNumber = productHaulLoad.ProgramId,
                    ProgramRevision = productHaulLoad.ProgramVersion,
                    DispatchedBy = productHaulLoad.DispatchBy,
                    PodIndex = productHaulLoad.PodIndex,
                    StorageName = eServiceOnlineGateway.Instance.GetBinInformationByBinIdAndPodIndex(productHaulLoad.Bin.Id, productHaulLoad.PodIndex)?.Name,
                    ServicePointId=productHaulLoad.ServicePoint.Id,
                    ServicePointName= productHaulLoad.ServicePoint.Name,
                    BulkPlantId= productHaulLoad.BulkPlant.Id,
                    BulkPlantName= productHaulLoad.BulkPlant.Name,
                    IsAirborneHazard = productHaulLoad.IsAirborneHazard,
                    NeedFlush = productHaulLoad.NeedFlush
                };
                var shippingLoadSheet = shippingLoadSheets.FindAll(p => p.ProductHaulLoad.Id == productHaulLoad.Id)
                    .FirstOrDefault();
                if (shippingLoadSheet != null)
                {
                    if (string.IsNullOrEmpty(bulkPlantLoad.StorageName))
                    {
                        bulkPlantLoad.RigName = shippingLoadSheet.Rig?.Name ?? "";
                        string storage = string.Empty;
                        foreach (var blendUnloadSheet in shippingLoadSheet.BlendUnloadSheets)
                        {
                            if(Math.Abs(blendUnloadSheet.UnloadAmount)>0.01)
                                storage += blendUnloadSheet.DestinationStorage.Name + ",";
                        }
                        bulkPlantLoad.StorageName = storage.TrimEnd(',');
                    }

                    if (!shippingLoadSheet.ProductHaul.IsThirdParty)
                    {
                        bulkPlantLoad.BulkerUnit = shippingLoadSheet.ProductHaul.BulkUnit == null ? "" : shippingLoadSheet.ProductHaul.BulkUnit.Name;
                        bulkPlantLoad.Driver = shippingLoadSheet.ProductHaul.Driver == null ? "" : shippingLoadSheet.ProductHaul.Driver.Name;
                    }
                    else
                    {
                        bulkPlantLoad.BulkerUnit = shippingLoadSheet.ProductHaul.ThirdPartyUnitNumber;
                        bulkPlantLoad.Driver = shippingLoadSheet.ProductHaul.SupplierContactName;
                    }
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
            public int BlendChemicalId { get; set; }
            public double Tonnage { get; set; }
            public string BulkerUnit { get; set; }
            public string Driver { get; set; }
            public string BinNumber { get; set; }
            public bool IsGoWithCrew { get; set; }
            public int ProductHaulLoadId { get; set; }
            public string StartSNJLocation { get; set; }
            public int LocationId { get; set; }
            public string DestinationLocation { get; set; }
            public string Category { get; set; }
            public string RigName { get; set; }
            public string ProgramNumber { get; set; }
            public int ProgramRevision { get; set; }
            public string DispatchedBy { get; set; }
            public int PodIndex { get; set; }
            public string StorageName { get; set; }
            public int ServicePointId { get; set; }
            public string ServicePointName { get; set; }
            public int BulkPlantId { get; set; }
            public string BulkPlantName { get; set; }

            public bool IsAirborneHazard { get; set; }
            public bool NeedFlush { get; set; }
        }

        //Sample: http://localhost:52346/BulkPlantAPI/GetBulkDensityByProduct?productName=LCS 1600
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

            return new JsonResult(item);
        }

        //Sample: http://localhost:52346/BulkPlantAPI/GetAllBulkDensities
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

            return new JsonResult(data, new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
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
            public double ToleranceFactor { get; set; }
            public bool IsAirborneHazard { get; set; }
            public string ToleranceRiskLevel { get; set; }
            public bool NeedFlush { get; set; }
        }
        //Sample: http://localhost:52348/BulkPlantAPI/GetAllBlendChemicals
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
                blendChemicalDetail.ToleranceFactor = blendChemical.ToleranceFactor??0.0;
                blendChemicalDetail.IsAirborneHazard =  blendChemical.IsAirborneHazard;
                blendChemicalDetail.ToleranceRiskLevel = blendChemical.ToleranceRiskLevel.ToString();
                blendChemicalDetail.NeedFlush = blendChemical.NeedFlush;

                data.Add(blendChemicalDetail);
            }

            return new JsonResult(data, new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        }

        //Sample: http://localhost:52346/BulkPlantAPI/GetAllOpenJobs
        public ActionResult GetAllOpenJobs()
        {
            List<string> data = new List<string>();

            List<RigJob> rigJobs = eServiceOnlineGateway.Instance.GetRigJobs();
            foreach (var rigJob in rigJobs)
            {
                if(rigJob.CallSheetNumber != 0 && rigJob.JobLifeStatus != JobLifeStatus.Completed && rigJob.JobLifeStatus != JobLifeStatus.Alerted && rigJob.JobLifeStatus != JobLifeStatus.Canceled && rigJob.JobLifeStatus != JobLifeStatus.None  && rigJob.JobLifeStatus != JobLifeStatus.Deleted)
                    data.Add(rigJob.CallSheetNumber.ToString());
            }
            
            return new JsonResult(data, new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }

        
        //Sample: http://localhost:52346/BulkPlantAPI/GetProductLoadList?servicePointId=61
        public ActionResult GetProductLoadList(int servicePointId)
        {
            List<ProductHaulLoad> producthoHaulLoads = null;
            if (servicePointId == 0)
                producthoHaulLoads = eServiceOnlineGateway.Instance.GetScheduledProductHaulLoads();
            else
                producthoHaulLoads = eServiceOnlineGateway.Instance.GetScheduledProductHaulLoadsByServicePoint(servicePointId);

            var data = BulkPlantLoads(producthoHaulLoads);

            return new JsonResult(data, new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }

        //Sample: http://localhost:5000/BulkPlantAPI/GetNotLoadedProductLoadList?servicePointId=61

        public ActionResult GetNotLoadedProductLoadList(int servicePointId)
        {
            List<ProductHaulLoad> producthoHaulLoads = new List<ProductHaulLoad>();
            if (servicePointId == 0)
            {
                return new JsonResult(null);
            }
            else
            {
                List<ProductHaulLoadStatus> ids = new List<ProductHaulLoadStatus>() {ProductHaulLoadStatus.Scheduled,ProductHaulLoadStatus.Blending, ProductHaulLoadStatus.BlendCompleted};

                //producthoHaulLoads = eServiceOnlineGateway.Instance.GetProductHaulLoadsByServicePointAndStatuses(servicePointId, new []{(int)ProductHaulLoadStatus.Scheduled,(int)ProductHaulLoadStatus.Blending, (int)ProductHaulLoadStatus.BlendCompleted});
                producthoHaulLoads = eServiceOnlineGateway.Instance.GetProductHaulLoadByQuery(p =>
                    p.ServicePoint.Id == servicePointId && (p.ProductHaulLoadLifeStatus== ProductHaulLoadStatus.Scheduled || p.ProductHaulLoadLifeStatus== ProductHaulLoadStatus.Blending ||p.ProductHaulLoadLifeStatus== ProductHaulLoadStatus.BlendCompleted));
                var loadedProductHaulLoads = eServiceOnlineGateway.Instance.GetProductHaulLoadByQuery(p =>
	                p.ServicePoint.Id == servicePointId && (p.ProductHaulLoadLifeStatus == ProductHaulLoadStatus.Loaded || p.ProductHaulLoadLifeStatus == ProductHaulLoadStatus.Stored)).Where(p=>p.EstmatedLoadTime.AddDays(2)>DateTime.Now);
                /*                var loads1 =eServiceOnlineGateway.Instance.GetProductHaulLoadsByServicePointAndStatus(servicePointId,ProductHaulLoadStatus.Scheduled);
                                                var loads2 =eServiceOnlineGateway.Instance.GetProductHaulLoadsByServicePointAndStatus(servicePointId,ProductHaulLoadStatus.Blending);
                                                var loads3 =eServiceOnlineGateway.Instance.GetProductHaulLoadsByServicePointAndStatus(servicePointId,ProductHaulLoadStatus.BlendCompleted);

                                                producthoHaulLoads.AddRange(loads1);
                                                producthoHaulLoads.AddRange(loads2);
                                                producthoHaulLoads.AddRange(loads3);*/
                producthoHaulLoads.AddRange(loadedProductHaulLoads);
                var data = BulkPlantLoads(producthoHaulLoads);

                return new JsonResult(data, new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            }
        }
        //Sample: http://localhost:5000/BulkPlantAPI/GetNotLoadedProductLoadListByBulkPlant?bulkPlantId=2346

        public ActionResult GetNotLoadedProductLoadListByBulkPlant(int bulkPlantId)
        {
            List<ProductHaulLoad> producthoHaulLoads = new List<ProductHaulLoad>();
            if (bulkPlantId == 0)
            {
                return new JsonResult(null);
            }
            else
            {
                var statusList = new Collection<int>() {(int) ProductHaulLoadStatus.Scheduled, (int) ProductHaulLoadStatus.Blending, (int) ProductHaulLoadStatus.BlendCompleted };
                var loads4 = eServiceOnlineGateway.Instance.GetProductHaulLoadsBy(bulkPlantId, statusList);
                var loadedProductHaulLoads = eServiceOnlineGateway.Instance.GetProductHaulLoadByQuery(p =>
	                p.BulkPlant.Id == bulkPlantId && (p.ProductHaulLoadLifeStatus == ProductHaulLoadStatus.Loaded || p.ProductHaulLoadLifeStatus == ProductHaulLoadStatus.Stored)).Where(p => p.EstmatedLoadTime.AddDays(2) > DateTime.Now);
					producthoHaulLoads.AddRange(loadedProductHaulLoads);

                var data = BulkPlantLoads(loads4);

                return new JsonResult(data, new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            }
        }


        //Sample: http://localhost:52346/BulkPlantAPI/GetLoadSheet?productLoadId=19001
        public ActionResult GetLoadSheet(int productLoadId)
        {
//            ProductHaulLoad productHaulLoadHeader = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productLoadId);
            IProductHaulLoadService productHaulLoadService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IProductHaulLoadService>();
            ProductHaulLoad productHaulLoad = productHaulLoadService
                .SelectByJoin(p => p.Id == productLoadId, p => p.AllProductLoadList != null).FirstOrDefault();
            if (productHaulLoad == null) return new JsonResult("Error: Invalid Product Haul Load Id");
            
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
            data.BlendDescription = productHaulLoad.BlendChemical.Description;
            data.BaseBlendWeight = productHaulLoad.BaseBlendWeight / 1000;
            data.TotalBlendWeight = productHaulLoad.TotalBlendWeight / 1000;
            data.BlendWeightUnit = productHaulLoad.Unit.Description;
            data.BaseTotalRatio = productHaulLoad.BaseBlendWeight / productHaulLoad.TotalBlendWeight;
            data.Details = GetBlendDetails(productHaulLoad.BlendSectionId, productHaulLoad.BaseBlendWeight,productHaulLoad.CallSheetNumber==0);
            data.ClientName = productHaulLoad.Customer.Name;
            data.CallSheetNumber = productHaulLoad.CallSheetNumber;
            data.ProgramId = productHaulLoad.ProgramId;
            data.ProgramRevision = productHaulLoad.ProgramVersion;
            data.RigName = productHaulLoad.Rig?.Name;
            data.JobType = productHaulLoad.JobType?.Name;
            if (productHaulLoad.Bin != null && productHaulLoad.Bin.Id !=0)
            {
	            var destination = CacheData.BinInformations.FirstOrDefault(p =>
		            p.Bin.Id == productHaulLoad.Bin.Id && p.PodIndex == productHaulLoad.PodIndex);
	            data.DestinationStorage = destination?.Name;
	            data.DestinationStorageId = destination?.Id ?? 0;
	            data.DestinationType = "Bin";
            }
            else
            {
	            var shippingLoadSheet =
		            eServiceOnlineGateway.Instance.GetShippingLoadSheetByProductHaulLoadId(productHaulLoad.Id).FirstOrDefault();
	            if (shippingLoadSheet != null)
	            {
		            var productHaul =
			            eServiceOnlineGateway.Instance.GetProductHaulById(shippingLoadSheet.ProductHaul.Id);
		            {
			            if (productHaul != null)
			            {
				            if (!productHaul.IsThirdParty)
				            {
					            data.DestinationStorage = productHaul.BulkUnit?.Name;
					            data.DestinationStorageId = productHaul.BulkUnit?.Id ?? 0;
					            data.DestinationType = "Bulker";
				            }
				            else
				            {
						            data.DestinationStorage = productHaul.Crew?.Description;
						            data.DestinationStorageId = productHaul.Crew?.Id ?? 0;
						            data.DestinationType = "ThirdParty";
                            }
			            }
                    }
	            }
            }

            data.EstimatedLoadTime = productHaulLoad.EstmatedLoadTime.ToUniversalTime()
	            .ToString("yyyy\'-\'MM\'-\'dd\'T\'HH\':\'mm\':\'ss\'.\'fffZ");
            data.DispatchedBy = productHaulLoad.DispatchBy;
            data.BlendRequestId = productHaulLoad.Id;
            data.BulkPlantId = productHaulLoad.BulkPlant.Id;
            data.BulkPlantName = productHaulLoad.BulkPlant.Name;
            data.IsAirborneHazard = productHaulLoad.IsAirborneHazard;
            data.NeedFlush = productHaulLoad.NeedFlush;

            /*
            var productList =
                ProductHaulModel.ProductLoadList(
                    new Collection<ProductLoadSection>(productHaulLoad.AllProductLoadList));
            */
            foreach (var productLoadSection in productHaulLoad.AllProductLoadList)
            {
                LoadSheetDetail loadSheetDetail = new LoadSheetDetail();
                loadSheetDetail.Product = productLoadSection.BlendChemical.Name;
                loadSheetDetail.BlendChemicalId = productLoadSection.BlendChemical.Id;
                loadSheetDetail.ProductWeight = productLoadSection.RequiredAmount;
                loadSheetDetail.ProductWeightUnit = productLoadSection.BlendAdditiveMeasureUnit.Description;
                loadSheetDetail.BlendMode = productLoadSection.AdditiveBlendMethod.Id;
                if (productLoadSection.BlendChemical.Name.Equals(baseChemical?.Name))
                {
                    loadSheetDetail.IsBaseCement = true;
                }
                loadSheetDetail.IsFromBase = productLoadSection.IsFromBase;
                
                data.Products.Add(loadSheetDetail);
            }

            return new JsonResult(data, new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        }

        private List<BlendDetail> GetBlendDetails(int blendSectionId, double baseTonnage, bool isFromProgram)
        {
            var blendSection = isFromProgram?eServiceOnlineGateway.Instance.GetProgramBlendSectionByBlendSectionId(blendSectionId):eServiceOnlineGateway.Instance.GetBlendSectionByBlendSectionId(blendSectionId);
            var blendDetails = new List<BlendDetail>();
            BlendDetail baseBlendDetail = new BlendDetail(){Product = blendSection.BlendFluidType.Name, Amount = baseTonnage/1000, AmountUnit = blendSection.BlendAmountUnit.Description, BlendMode = 1, IsBaseBlend = true };
            blendDetails.Add(baseBlendDetail);
            foreach (var blendSectionBlendAdditiveSection in blendSection.BlendAdditiveSections)
            {
                BlendDetail additiveDetail = new BlendDetail(){Product = blendSectionBlendAdditiveSection.AdditiveType.Name, Amount = blendSectionBlendAdditiveSection.Amount, AmountUnit = blendSectionBlendAdditiveSection.AdditiveAmountUnit.Description, BlendMode = blendSectionBlendAdditiveSection.AdditiveBlendMethod.Id, IsBaseBlend = false };
                blendDetails.Add(additiveDetail);
            }

            return blendDetails;
        }

        private static BlendChemical GetBaseChemical(BlendChemical blend)
        {
//            IBlendRecipeService blendRecipeService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IBlendRecipeService>();
//            if (blendRecipeService == null) throw new Exception("blendChemicalService must be registered in service factory");

            if (blend.BlendRecipe == null || blend.BlendRecipe.Id == 0) return blend;

//            var blendRecipe = 
                
                //blendRecipeService.SelectByJoin(p => p.Id == blend.BlendRecipe.Id, p => p.BlendChemicalSections != null).FirstOrDefault();

  //          if (blendRecipe == null || blendRecipe.Id == 0) return blend;

            BlendChemical baseChemical = null;

            BlendChemical baseBlend = blend.BlendRecipe.BlendChemicalSections.Find(p => p.IsBaseBlend)?
                .BlendChemical;
            if (baseBlend != null)
            {
                BlendChemical blendBase =
                    CacheData.BlendChemicals.FirstOrDefault(p => p.Id == baseBlend.Id);
                baseChemical = GetBaseChemical(blendBase);
            }

            return baseChemical;
        }

        internal class LoadSheet
        {
            public List<BlendDetail> Details { set; get; }
            public string BlendDescription { set; get; }
            public double BaseBlendWeight { set; get; }
            public double TotalBlendWeight { set; get; }
            public string BlendWeightUnit { set; get; }
            public double BaseTotalRatio { set; get; }
            public double MixWater { set; get; }
            public string MixWaterUnit { set; get; }
            public double Yield { set; get; }
            public string YieldUnit { set; get; }
            public double Density { set; get; }
            public string DensityUnit { set; get; }
            public double SackWeight { set; get; }
            public string SackWeightUnit { set; get; }
            public List<LoadSheetDetail> Products { set; get; }
            //followings are new Q3_2023
            public string ClientName { get; set; }
            public int CallSheetNumber { get; set; }
            public string ProgramId { get; set; }
            public int ProgramRevision { get; set; }
            public string RigName { get; set; }
            public string JobType { get; set; }
            public string DestinationStorage { get; set; }//BinInformation Name or bulker unit number or Third party crew name
            public int DestinationStorageId { get; set; }//BinInformation Id or bulker unit id or Third party Crew id
            public string DestinationType { get; set; }//Bulker or Bin or ThirdParty
            public string EstimatedLoadTime { get; set; }
            public string DispatchedBy { get; set; }
            public int BlendRequestId { get; set; }
            public string BulkPlantName { get; set; }
            public int BulkPlantId { get; set; }
            public bool IsAirborneHazard { get; set; }
            public bool NeedFlush { get; set; }


        }

        internal class LoadSheetDetail
        {
            public string Product { set; get; }
            public int BlendChemicalId { set; get; }
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

        #region ProductHaul
        //Sample: http://localhost:52348/BulkPlantAPI/GetScheduledProductHaulListByBulkPlant?bulkPlantId=2351
        public ActionResult GetScheduledProductHaulListByBulkPlant(int bulkPlantId)
        {
            var productHauls =
                eServiceOnlineGateway.Instance.GetProductHaulListByBulkPlant(bulkPlantId, ProductHaulStatus.Scheduled);
            var data = LoadProductHauls(productHauls);

            return new JsonResult(data, new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        }

        //Sample: http://localhost:52348/BulkPlantAPI/GetProductHaulDetail?productHaulId=43244
        public ActionResult GetProductHaulDetail(int productHaulId)
        {
            ProductHaul producthoHaul = eServiceOnlineGateway.Instance.GetProductHaulById(productHaulId);

            var data = LoadProductHaulDetail(producthoHaul);
            return new JsonResult(data, new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        }

        private static List<BulkPlantProductHaul> LoadProductHauls(List<ProductHaul> productHauls)
        {
            List<BulkPlantProductHaul> bulkPlantProductHauls = new List<BulkPlantProductHaul>();
            foreach (var productHaul in productHauls)
            {
                BulkPlantProductHaul bulkPlantProductHaul = new BulkPlantProductHaul();
                bulkPlantProductHaul.Id = productHaul.Id;
                bulkPlantProductHaul.EstimatedLoadTime = productHaul.EstimatedLoadTime;
                bulkPlantProductHaul.ExpOnLocationTime = productHaul.ExpectedOnLocationTime;
                bulkPlantProductHaul.Crew = productHaul.Crew.Name;
                bulkPlantProductHaul.ProductHaulDescription = productHaul.Description;
                bulkPlantProductHaul.Rig = productHaul.ShippingLoadSheets.FirstOrDefault()?.Rig.Name;
                bulkPlantProductHaul.WellLocation = productHaul.ShippingLoadSheets.FirstOrDefault()?.Destination;
                bulkPlantProductHaul.GoWithCrew = productHaul.IsGoWithCrew;
                bulkPlantProductHaul.Station = productHaul.BulkPlant.Name;
                bulkPlantProductHaul.Status = productHaul.ProductHaulLifeStatus.ToString();
                bulkPlantProductHaul.IsAirborneHazard = productHaul.IsAirborneHazard;
                bulkPlantProductHauls.Add(bulkPlantProductHaul);
            }

            return bulkPlantProductHauls;
        }

        private static BulkPlantProductHaulDetail LoadProductHaulDetail(ProductHaul productHaul)
        {
            BulkPlantProductHaulDetail bulkPlantProductHaul = new BulkPlantProductHaulDetail();

            bulkPlantProductHaul.Id = productHaul.Id;
            bulkPlantProductHaul.Date = DateTime.Now;
            bulkPlantProductHaul.Unit = productHaul.IsThirdParty
                ? productHaul.ThirdPartyUnitNumber ?? string.Empty
                : productHaul.TractorUnit?.Name ?? string.Empty;
            bulkPlantProductHaul.Driver = productHaul.Driver?.Name ?? string.Empty;
            bulkPlantProductHaul.GoWithCrew = productHaul.IsGoWithCrew;
            bulkPlantProductHaul.Station = productHaul.BulkPlant?.Name ?? string.Empty;
            bulkPlantProductHaul.IsAirborneHazard = productHaul.IsAirborneHazard;
            bulkPlantProductHaul.ShippingLoadSheets = new List<ShippingLoad>();
            foreach (var shippingLoadSheet in productHaul.ShippingLoadSheets)
            {
                ShippingLoad shippingLoad = new ShippingLoad();
                shippingLoad.Id = shippingLoadSheet.Id;
                shippingLoad.GoWithCrew = shippingLoadSheet.IsGoWithCrew;
                shippingLoad.EstimatedLoadTime = shippingLoadSheet.EstimatedLoadTime;
                shippingLoad.ExpOnLocationTime = shippingLoadSheet.ExpectedOnLocationTime;
                shippingLoad.Client = shippingLoadSheet.ClientName??string.Empty;
                shippingLoad.ClientRep = shippingLoadSheet.ClientRepresentative??string.Empty;
                shippingLoad.Rig = shippingLoadSheet.Rig?.Name ?? string.Empty;
                shippingLoad.Location = shippingLoadSheet.Destination ?? string.Empty;

                shippingLoad.DispatchedBy = shippingLoadSheet.ModifiedUserName ?? string.Empty;
                shippingLoad.ProductHaulLoadId = shippingLoadSheet.ProductHaulLoad.Id;
                shippingLoad.IsAirborneHazard = shippingLoadSheet.IsAirborneHazard;
                shippingLoad.OffloadInstructions = new List<OffloadInstruction>();
                var shippingLoadSheetBlendUnloadSheets =
                    eServiceOnlineGateway.Instance.GetBlendUnloadSheetByQuery(p =>
                        p.ShippingLoadSheet.Id == shippingLoadSheet.Id);
                foreach (var blendUnloadSheet in shippingLoadSheetBlendUnloadSheets)
                {
                    OffloadInstruction offloadInstruction = new OffloadInstruction();
                    var binInformationId = blendUnloadSheet.DestinationStorage.Id;
                    var binInformation = eServiceOnlineGateway.Instance.GetBinInformationById(binInformationId);
                    offloadInstruction.Bin = binInformation.Bin?.Name ?? string.Empty;
                    offloadInstruction.BaseTonnage = blendUnloadSheet.BaseTonnage / 1000;
                    offloadInstruction.BaseTonnageUnit = blendUnloadSheet.BaseTonnageUnit?.Description ?? string.Empty;
                    offloadInstruction.Product = shippingLoadSheet.BlendDescription ?? string.Empty;
                    offloadInstruction.TotalTonnage = blendUnloadSheet.UnloadAmount / 1000;
                    offloadInstruction.TotalTonnageUnit = blendUnloadSheet.UnloadAmountUnit?.Description ?? string.Empty;
//TODO                    offloadInstruction.IsAirborneHazard = blendUnloadSheet.IsAirborneHazard;
                    shippingLoad.OffloadInstructions.Add(offloadInstruction);


                }

                bulkPlantProductHaul.ShippingLoadSheets.Add(shippingLoad);
            }

            bulkPlantProductHaul.BulkerPodLoads = new List<BulkerPodLoad>();
            foreach (var podLoad in productHaul.PodLoad)
            {
                BulkerPodLoad bulkerPodLoad = new BulkerPodLoad();
                bulkerPodLoad.PodIndex = podLoad.PodIndex;
                bulkerPodLoad.ShippingLoadSheetId = podLoad.ShippingLoadSheet.Id;
                bulkerPodLoad.BaseTonnage = podLoad.BaseTonnage / 1000;
                bulkerPodLoad.BaseTonnageUnit = podLoad.BaseTonnageUnit?.Description ?? string.Empty;
                var shippingLoadSheet =
	                productHaul.ShippingLoadSheets.Find(p => p.Id == bulkerPodLoad.ShippingLoadSheetId);
                bulkerPodLoad.Product = shippingLoadSheet?.BlendDescription ?? string.Empty;
                bulkerPodLoad.TotalTonnage = podLoad.LoadAmount / 1000;
                bulkerPodLoad.TotalTonnageUnit = podLoad.LoadAmountUnit?.Description ?? string.Empty;
                bulkerPodLoad.Temp = 0;
//TODO                bulkerPodLoad.IsAirborneHazard = podLoad.IsAirborneHazard;
                bulkPlantProductHaul.BulkerPodLoads.Add(bulkerPodLoad);
            }

            bulkPlantProductHaul.TotalWeightsSamples = new List<TotalWeightsSample>();

            //Following Logic is not correct, it should be summarized from shipping load sheet
            foreach (var productHaulLoadId in bulkPlantProductHaul.ShippingLoadSheets.Select(p => p.ProductHaulLoadId)
                         .Distinct())
            {
                var productHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productHaulLoadId);

                TotalWeightsSample totalWeightsSample = new TotalWeightsSample();
                totalWeightsSample.BlendRequestId = productHaulLoadId;
                totalWeightsSample.BaseTonnageUnit = productHaulLoad.Unit?.Description ?? string.Empty;
                totalWeightsSample.Product = productHaulLoad.BlendChemical?.Description ?? string.Empty;
                totalWeightsSample.TotalTonnageUnit = productHaulLoad.TotalTonnageUnit?.Description ?? string.Empty;
                totalWeightsSample.Sample = productHaulLoad.IsBlendTest;
                totalWeightsSample.IsAirborneHazard = productHaulLoad.IsAirborneHazard;

                totalWeightsSample.RequestedBy = productHaulLoad.ModifiedUserName ?? string.Empty;
                var blendLog = eServiceOnlineGateway.Instance.GetBlendLogByProductHaulLoadId(productHaulLoadId);
                totalWeightsSample.BlendedBy = blendLog == null ? "" : blendLog.BulkPlantOperator ?? string.Empty;


                var shippingLoadSheetIds = productHaul.ShippingLoadSheets.Select(p => p.Id).Distinct().Except(new[] { 0 }).ToList(); ;
                var blendUnloadSheets =
                    eServiceOnlineGateway.Instance.GetBlendUnloadSheetByshippingLoadSheetIds(shippingLoadSheetIds.ToArray()).Distinct();

                double calcBaseTonnage = 0;
                double calcTotalTonnage = 0;
                var calcBlendUnloadSheets = blendUnloadSheets.Where(p =>
                    p.ShippingLoadSheet.ProductHaulLoad.Id == productHaulLoadId);
                foreach (var calcBlendUnloadSheet in calcBlendUnloadSheets)
                {
                    calcBaseTonnage += calcBlendUnloadSheet.BaseTonnage / 1000;
                    calcTotalTonnage += calcBlendUnloadSheet.UnloadAmount / 1000;
                }

                totalWeightsSample.BaseTonnage = calcBaseTonnage;
                totalWeightsSample.TotalTonnage = calcTotalTonnage;


                bulkPlantProductHaul.TotalWeightsSamples.Add(totalWeightsSample);
            }

            return bulkPlantProductHaul;
        }

        internal class BulkPlantProductHaul
        {
            public int Id { set; get; }
            public DateTime EstimatedLoadTime { set; get; }
            public DateTime ExpOnLocationTime { set; get; }
            public string Crew { set; get; }
            public string ProductHaulDescription { set; get; }
            public string Rig { set; get; }
            public string WellLocation { set; get; }

            public bool GoWithCrew { set; get; }
            public string Station { set; get; }
            public string Status { set; get; }
            public bool IsAirborneHazard { set; get; }
        }

        internal class BulkPlantProductHaulDetail
        {
            public int Id { set; get; }
            public DateTime Date { set; get; }
            public string Unit { set; get; }
            public string Driver { set; get; }
            public bool GoWithCrew { set; get; }
            public string Station { set; get; }
            public bool IsAirborneHazard { set; get; }
            public List<ShippingLoad> ShippingLoadSheets { set; get; }
            public List<BulkerPodLoad> BulkerPodLoads { set; get; }
            public List<TotalWeightsSample> TotalWeightsSamples { set; get; }

        }

        internal class ShippingLoad
        {
            public int Id { set; get; }
            public bool GoWithCrew { set; get; }
            public DateTime EstimatedLoadTime { set; get; }
            public DateTime ExpOnLocationTime { set; get; }
            public string Client { set; get; }
            public string ClientRep { set; get; }
            public string Rig { set; get; }
            public string Location { set; get; }
            public string DispatchedBy { set; get; }
            public int ProductHaulLoadId { set; get; }
            public bool IsAirborneHazard { set; get; }
            public List<OffloadInstruction> OffloadInstructions { set; get; }
        }

        internal class BulkerPodLoad
        {
            public int PodIndex { set; get; }
            public int ShippingLoadSheetId { set; get; }
            public double BaseTonnage { set; get; }
            public string BaseTonnageUnit { set; get; }
            public string Product { set; get; }
            public double TotalTonnage { set; get; }
            public string TotalTonnageUnit { set; get; }
            public double Temp { set; get; }
            public bool IsAirborneHazard { set; get; }

        }

        internal class TotalWeightsSample
        {
            public int BlendRequestId { set; get; }
            public double BaseTonnage { set; get; }
            public string BaseTonnageUnit { set; get; }
            public string Product { set; get; }
            public double TotalTonnage { set; get; }
            public string TotalTonnageUnit { set; get; }
            public bool Sample { set; get; }
            public string RequestedBy { set; get; }
            public string BlendedBy { set; get; }
            public bool IsAirborneHazard { set; get; }
        }

        internal class OffloadInstruction
        {
            public string Bin { set; get; }
            public double BaseTonnage { set; get; }
            public string BaseTonnageUnit { set; get; }
            public string Product { set; get; }
            public double TotalTonnage { set; get; }
            public string TotalTonnageUnit { set; get; }
            public bool IsAirborneHazard { set; get; }
        }

        #endregion

        #endregion GET APIS

        #region SET APIS

        //Sample: http://localhost:52346/BulkPlantAPI/SetProductHaulLoadBlending?productLoadId=19001
        public ActionResult SetProductHaulLoadBlending(int productLoadId)
        {
            int success = 0;
            ProductHaulLoad productHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productLoadId);
            if (productHaulLoad != null)
            {
                productHaulLoad.ProductHaulLoadLifeStatus = ProductHaulLoadStatus.Blending;
                success = eServiceOnlineGateway.Instance.UpdateProductHaulLoad(productHaulLoad);
            }

            return new JsonResult(success);
        }

        //Sample: http://localhost:52346/BulkPlantAPI/SetProductHaulLoadLoaded?productLoadId=19001
        public ActionResult SetProductHaulLoadLoaded(int productLoadId)
        {
            //Following logic needs be moved to Process class to be shared across whole solution
            int success = 0;
            ProductHaulLoad productHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productLoadId);
            if (productHaulLoad != null && productHaulLoad.ProductHaulLoadLifeStatus != ProductHaulLoadStatus.Loaded && productHaulLoad.ProductHaulLoadLifeStatus != ProductHaulLoadStatus.OnLocation)
            {
                if (productHaulLoad.Bin != null && productHaulLoad.Bin.Id != 0)
                {
                    BinInformation binInformation =
                        eServiceOnlineGateway.Instance.GetBinInformationByBinIdAndPodIndex(productHaulLoad.Bin.Id, productHaulLoad.PodIndex);
                    if (binInformation?.Rig != null && binInformation.Rig.Id != 0)
                    {
                        Rig rig = eServiceOnlineGateway.Instance.GetRigById(binInformation.Rig.Id);
                        if (rig.OperationSiteType == OperationSiteType.BulkPlant)
                        {
                            if (productHaulLoad.BlendChemical.Description != binInformation.BlendChemical.Description)
                            //If blend description are not same, empty old one and fill up new one
                            {
                                binInformation.BlendChemical = null;
                                binInformation.Quantity = 0;
                                eServiceOnlineGateway.Instance.UpdateBinInformation(binInformation);
                                binInformation.BlendChemical = productHaulLoad.BlendChemical;
                                var subs = binInformation.BlendChemical.Description.Split('+');
                                var baseBlendName = subs[0].TrimEnd();
                                var baseBlend =
                                    EServiceReferenceData.Data.BlendChemicalCollection.FirstOrDefault(p =>
                                        p.Name == baseBlendName);
                                if (baseBlend != null)
                                {
                                    binInformation.Capacity = binInformation.Volume * baseBlend.BulkDensity * 1000;
                                }

                            }
                            //If blend description are same, add it up
                            binInformation.Quantity += productHaulLoad.TotalBlendWeight;
                            if (productHaulLoad.IsBlendTest)
                                binInformation.BlendTestingStatus = BlendTestingStatus.Requested;
                            eServiceOnlineGateway.Instance.UpdateBinInformation(binInformation);
                        }
                    }
                }
                
                productHaulLoad.ProductHaulLoadLifeStatus = ProductHaulLoadStatus.Loaded;
                success = eServiceOnlineGateway.Instance.UpdateProductHaulLoad(productHaulLoad);

            }

            return new JsonResult(success);
        }

        //Sample: http://localhost:52346/BulkPlantAPI/SetProductHaulLoadBlendCompleted?productLoadId=19001
        public ActionResult SetProductHaulLoadBlendCompleted(int productLoadId) 
        {
            int success = 0;
            string loggedUser = "BPAVS";
            ProductHaulLoad productHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productLoadId);
            if (productHaulLoad != null)
            {
	            productHaulLoad.ProductHaulLoadLifeStatus = ProductHaulLoadStatus.BlendCompleted;
	            success = eServiceOnlineGateway.Instance.UpdateProductHaulLoad(productHaulLoad);
            }

            return new JsonResult(success);
        }

        //Sample: http://localhost:52346/BulkPlantAPI/SetBlendCompletedProductHaulLoadToScheduled?productLoadId=19001
        public ActionResult SetBlendCompletedProductHaulLoadToScheduled(int productLoadId)
        {
            string successMessage;
            var productHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productLoadId);
            if (productHaulLoad.ProductHaulLoadLifeStatus != ProductHaulLoadStatus.BlendCompleted)
                successMessage = "The product haul load is not Blend Completed status";
            else
            {
                productHaulLoad.ProductHaulLoadLifeStatus = ProductHaulLoadStatus.Scheduled;
                var success = eServiceOnlineGateway.Instance.UpdateProductHaulLoad(productHaulLoad);
                successMessage = success == 1 ? "Success" : "Failed";
            }

            return new JsonResult(successMessage);
        }
        //Sample: http://localhost:52346/BulkPlantAPI/SetBlendingProductHaulLoadToScheduled?productLoadId=19001
        public ActionResult SetBlendingProductHaulLoadToScheduled(int productLoadId)
        {
            string successMessage;
            var productHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productLoadId);
            if (productHaulLoad.ProductHaulLoadLifeStatus != ProductHaulLoadStatus.Blending)
                successMessage = "The product haul load is not Blending status";
            else
            {
                productHaulLoad.ProductHaulLoadLifeStatus = ProductHaulLoadStatus.Scheduled;
                var success = eServiceOnlineGateway.Instance.UpdateProductHaulLoad(productHaulLoad);
                successMessage = success == 1 ? "Success" : "Failed";
            }

            return new JsonResult(successMessage);
        }
        //Sample: http://localhost:52346/BulkPlantAPI/SetLoadedProductHaulLoadToScheduled?productLoadId=24988
        public ActionResult SetLoadedProductHaulLoadToScheduled(int productLoadId)
        {
            string successMessage;
            var productHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productLoadId);
            if (productHaulLoad.ProductHaulLoadLifeStatus != ProductHaulLoadStatus.Loaded)
                successMessage = "The product haul load is not Loaded status";
            else
            {
                productHaulLoad.ProductHaulLoadLifeStatus = ProductHaulLoadStatus.Scheduled;
                var success = eServiceOnlineGateway.Instance.UpdateProductHaulLoad(productHaulLoad);
                successMessage = success == 1 ? "Success" : "Failed";
            }

            return new JsonResult(successMessage);
        }
        //Sample: http://localhost:52346/BulkPlantAPI/SetProductHaulLoadScheduled?productLoadId=19001
        public ActionResult SetProductHaulLoadScheduled(int productLoadId)
        {
            string successMessage;
            var productHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productLoadId);
                productHaulLoad.ProductHaulLoadLifeStatus = ProductHaulLoadStatus.Scheduled;
                var success = eServiceOnlineGateway.Instance.UpdateProductHaulLoad(productHaulLoad);
                successMessage = success == 1 ? "Success" : "Failed";

            return new JsonResult(successMessage);
        }

        #endregion SET APIS     
       
        #region RESET APIs for support

        //Sample: http://localhost:52346/BulkPlantAPI/ResetProductHaulLoadScheduled?productLoadId=19001
        public ActionResult ResetProductHaulLoadScheduled(int productLoadId)
        {
            int success = 0;
            ProductHaulLoad productHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productLoadId);
            if (productHaulLoad != null)
            {
                productHaulLoad.ProductHaulLoadLifeStatus = ProductHaulLoadStatus.Blending;
                success = eServiceOnlineGateway.Instance.UpdateProductHaulLoad(productHaulLoad);
            }
            else
            {
                IProductHaulLoadService productHaulLoadService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IProductHaulLoadService>();
                if (productHaulLoadService == null) throw new Exception("productHaulLoadService must be registered in service factory");

                productHaulLoad = productHaulLoadService.SelectAllVersions(productLoadId).OrderByDescending(p=>p.SystemId).FirstOrDefault();

                if (productHaulLoad != null)
                {
                    productHaulLoad.EffectiveEndDateTime = DateTime.MaxValue;
                    productHaulLoad.EntityStatus = 0;
                    productHaulLoadService.Update(productHaulLoad);
                }
                
            }

            return new JsonResult(success);
        }

        #endregion RESET APIs for support
        #region LabSample
        public class BpBlendSample
        {
            public int ProductLoadId { get; set; }
            public DateTime DateCollected { get; set;}
            public double Amount { set; get; }
            public string AmountUnit { get; set; }
            public int CutIndex { get; set; }
            public int SampleIndex { get; set; }
            public int ContainerCount { get; set; }
            public string Comments { get; set; }
            public int BulkPlantId { get; set; }
            public int StorageId { get; set; }
            public string StorageName { get; set; }
            public string SampleBarCode { get; set; }
            public string SampleType { get; set; }
        }

        public string GetBlendSample()
        {
            BpBlendSample bpBlendSample = new BpBlendSample()
            {
                ProductLoadId = 22222,
                DateCollected = DateTime.Now,
                Amount = 5,
                AmountUnit = "kg",
                CutIndex = 1,
                SampleIndex = 1,
                ContainerCount = 1,
                Comments = "Test",
                BulkPlantId = 2351,
                StorageId = 2970,
                StorageName = "Silo 1",
                SampleBarCode="FeatureTBD",
                SampleType = "Cement"
            };

            return JsonConvert.SerializeObject(bpBlendSample,
                new JsonSerializerSettings() { ContractResolver = GetBlendSampleResolver() });
        }
        private static PropertyRenameAndIgnoreSerializerContractResolver GetBlendSampleResolver()
        {
            var resolver = new PropertyRenameAndIgnoreSerializerContractResolver();
            resolver.IgnoreProperty(typeof(BpBlendSample), "blendSample");
            IgnoreCommonProperties(resolver);
            return resolver;
        }


        // http://localhost:52348/bulkplantapi/PostBlendSample
        [HttpPost]
        public RequestResult PostBlendSample(BpBlendSample blendSample)
        {
	        int rtn = 0;
	        bool isInputValid = true;
            string message =String.Empty;

            if (blendSample.ProductLoadId == 0)
            {
	            message += "Blend Request Id cannot be 0";
	            isInputValid = false;
            }

            if (blendSample.CutIndex == 0)
            {
	            message += "CutIndex cannot be 0";
	            isInputValid = false;
            }

            if (blendSample.SampleIndex == 0)
            {
	            message += "SampleIndex cannot be 0";
	            isInputValid = false;
            }

            if (isInputValid)
            {
	            BlendSample serverBlendSample = eServiceOnlineGateway.Instance.GetBlendSampleByQuery(p =>
		            p.ProductHaulLoad.Id == blendSample.ProductLoadId && p.SampleIndex == blendSample.SampleIndex &&
		            p.CutIndex == blendSample.CutIndex)?.FirstOrDefault();

	            if (serverBlendSample == null)
	            {
		            serverBlendSample = new BlendSample();
		            ProductHaulLoad productHaulLoad =
			            eServiceOnlineGateway.Instance.GetProductHaulLoadById(blendSample.ProductLoadId);
		            if (productHaulLoad == null)
		            {
			            message += "Blend Request #" + blendSample.ProductLoadId + " doesn't exist.";
		            }
		            else
		            {
			            serverBlendSample.ProductHaulLoad = productHaulLoad;
			            serverBlendSample.DateCollected = blendSample.DateCollected;
			            serverBlendSample.SampleBarCode = blendSample.SampleBarCode;
			            serverBlendSample.Amount = blendSample.Amount;
			            serverBlendSample.AmountUnit = blendSample.AmountUnit;
			            serverBlendSample.CutIndex = blendSample.CutIndex;
			            serverBlendSample.SampleIndex = blendSample.SampleIndex;
			            serverBlendSample.ContainerCount = blendSample.ContainerCount;
			            serverBlendSample.Comments = blendSample.Comments;
			            serverBlendSample.BulkPlantId = blendSample.BulkPlantId;
			            serverBlendSample.StorageId = blendSample.StorageId;
			            serverBlendSample.StorageName = blendSample.StorageName;
			            serverBlendSample.SampleType = blendSample.SampleType;
			            rtn = eServiceOnlineGateway.Instance.CreateBlendSample(serverBlendSample);
		            }
	            }
	            else
	            {
		            serverBlendSample.DateCollected = blendSample.DateCollected;
		            serverBlendSample.SampleBarCode = blendSample.SampleBarCode;
		            serverBlendSample.Amount = blendSample.Amount;
		            serverBlendSample.AmountUnit = blendSample.AmountUnit;
		            serverBlendSample.ContainerCount = blendSample.ContainerCount;
		            serverBlendSample.Comments = blendSample.Comments;
		            serverBlendSample.BulkPlantId = blendSample.BulkPlantId;
		            serverBlendSample.StorageId = blendSample.StorageId;
		            serverBlendSample.StorageName = blendSample.StorageName;
		            serverBlendSample.SampleType = blendSample.SampleType;
		            rtn = eServiceOnlineGateway.Instance.UpdateBlendSample(serverBlendSample);
	            }
	            if (rtn == 1)
	            {
		            serverBlendSample = eServiceOnlineGateway.Instance.GetBlendSampleByQuery(p =>
			            p.ProductHaulLoad.Id == blendSample.ProductLoadId && p.SampleIndex == blendSample.SampleIndex &&
			            p.CutIndex == blendSample.CutIndex)?.FirstOrDefault();

		            AutoloadBlend(blendSample.ProductLoadId, "BPAVS");


                    if (serverBlendSample != null)
		            {
			            BpBlendSample returnedSample = new BpBlendSample();
			            returnedSample.ProductLoadId = serverBlendSample.ProductHaulLoad.Id;
			            returnedSample.DateCollected = serverBlendSample.DateCollected;
			            returnedSample.SampleBarCode = serverBlendSample.SampleBarCode;
			            returnedSample.Amount = serverBlendSample.Amount;
			            returnedSample.AmountUnit = serverBlendSample.AmountUnit;
			            returnedSample.CutIndex = serverBlendSample.CutIndex;
			            returnedSample.SampleIndex = serverBlendSample.SampleIndex;
			            returnedSample.ContainerCount = serverBlendSample.ContainerCount;
			            returnedSample.Comments = serverBlendSample.Comments;
			            returnedSample.BulkPlantId = serverBlendSample.BulkPlantId;
			            returnedSample.StorageId = serverBlendSample.StorageId;
			            returnedSample.StorageName = serverBlendSample.StorageName;
			            returnedSample.SampleType = serverBlendSample.SampleType;

			            return new RequestResult
			            {
				            IsSucceed = true,
				            Message = "success",
				            Data = JsonConvert.SerializeObject(returnedSample,
					            new JsonSerializerSettings() { ContractResolver = GetBlendSampleResolver() })
			            };
		            }
		            else
			            return new RequestResult { IsSucceed = false, Message = message, Data = new JsonResult(null) };
	            }
	            else
	            {
		            return new RequestResult { IsSucceed = false, Message = message, Data = new JsonResult(null) };
	            }
            }

            return new RequestResult { IsSucceed = false, Message = message, Data = new JsonResult(null) };

        }

        private void AutoloadBlend(int productHaulLoadId, string loggedUser)
        {
	        var productHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productHaulLoadId);
	        if (productHaulLoad != null && productHaulLoad.ProductHaulLoadLifeStatus==ProductHaulLoadStatus.BlendCompleted)
	        {
	            //if load to silo, load it automatically if silo is empty
	            if (productHaulLoad.Bin != null && productHaulLoad.Bin.Id != 0)
	            {
	                string loadResult=BinProcess.LoadBlendToBin(productHaulLoad.Bin, productHaulLoad.PodIndex,
	                    productHaulLoad.BlendChemical.Name,
	                    productHaulLoad.BlendChemical.Description, productHaulLoad.TotalBlendWeight,
	                    productHaulLoad,
	                    null, loggedUser);
	                if (loadResult == "Succeed")
	                {
	                    BinProcess.CreateShippingLoadSheetByLoadBlendToBin(loggedUser, productHaulLoad);

	                    productHaulLoad.ProductHaulLoadLifeStatus = ProductHaulLoadStatus.Stored;
	                    productHaulLoad.ModifiedUserName = loggedUser;
	                    eServiceOnlineGateway.Instance.UpdateProductHaulLoad(productHaulLoad);
	                }
	            }
	            //if one stop product haul, load the bulker directly
	            else
	            {
	                //Not implement this for now. Logistics dispatch can load blend to bulker explicitly on Job Board or Bulk Plant board.
	            }

	        }
        }

        #endregion LabSample

        #region MTS
        //Sample: http://localhost:5001/BulkPlantAPI/GetScheduledProductHauList?servicePointId=61
        public ActionResult GetScheduledProductHaulList(int servicePointId)
        {

            string data = null;

            return new JsonResult(data, new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        }

        //Sample: http://localhost:52346/BulkPlantAPI/SetProductHaulLoaded?productHaulId=19001
        public ActionResult SetProductHaulLoaded(int productHaulId)
        {
            string data = null;

            return new JsonResult(data, new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }

        //Sample: http://localhost:52346/BulkPlantAPI/UpdateProductHaul
        [HttpPost]
        public ActionResult UpdateProductHaul(object productHaul)
        {
            string data = null;

            return new JsonResult(data, new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }



        #endregion MTS
    }
    #region camel case resolver

    public class PropertyRenameAndIgnoreSerializerContractResolver : CamelCasePropertyNamesContractResolver
    {
        private readonly Dictionary<Type, HashSet<string>> _ignores;
        private readonly Dictionary<Type, Dictionary<string, string>> _renames;

        public PropertyRenameAndIgnoreSerializerContractResolver()
        {
            _ignores = new Dictionary<Type, HashSet<string>>();
            _renames = new Dictionary<Type, Dictionary<string, string>>();
        }

        public void IgnoreProperty(Type type, params string[] jsonPropertyNames)
        {
            if (!_ignores.ContainsKey(type))
                _ignores[type] = new HashSet<string>();

            foreach (var prop in jsonPropertyNames)
                _ignores[type].Add(prop);
        }

        public void RenameProperty(Type type, string propertyName, string newJsonPropertyName)
        {
            if (!_renames.ContainsKey(type))
                _renames[type] = new Dictionary<string, string>();

            _renames[type][propertyName] = newJsonPropertyName;
        }

        protected override Newtonsoft.Json.Serialization.JsonProperty CreateProperty(MemberInfo member,
            MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (IsIgnored(property.DeclaringType, property.PropertyName))
            {
                property.ShouldSerialize = i => false;
                property.Ignored = true;
            }

            if (IsRenamed(property.DeclaringType, property.PropertyName, out var newJsonPropertyName))
                property.PropertyName = newJsonPropertyName;

            return property;
        }

        private bool IsIgnored(Type type, string jsonPropertyName)
        {
            if (!_ignores.ContainsKey(type))
                return false;

            return _ignores[type].Contains(jsonPropertyName);
        }

        private bool IsRenamed(Type type, string jsonPropertyName, out string newJsonPropertyName)
        {
            Dictionary<string, string> renames;

            if (!_renames.TryGetValue(type, out renames) ||
                !renames.TryGetValue(jsonPropertyName, out newJsonPropertyName))
            {
                newJsonPropertyName = null;
                return false;
            }

            return true;
        }

        #endregion camel case resolver
    }

}