using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using eServiceOnline.Gateway;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Products;
using eServiceOnline.WebAPI.Data.Cost;
using Sesi.SanjelData.Entities.BusinessEntities.BaseEntites;


namespace eServiceOnlineAPI.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class InventoryAPIController : ControllerBase
    {
        public ActionResult GetBlendsAvailability(string jsonBlends)
        {
            CostUtils.InputBlends inputBlends = JsonConvert.DeserializeObject<CostUtils.InputBlends>(jsonBlends.ToString());
            Collection<CostUtils.OutputAvailability> outputAvailabilityCollection = new Collection<CostUtils.OutputAvailability>();
            string servicePointSbsId = "D675"; //HO by default

            //int bsId = 1;
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

                    BlendSection bs =
                        new BlendSection()
                        {
                            Id = blend.Idx,  //bsId++,
                            BlendFluidType = new BlendFluidType() { Id = bft.Id, Name = bft.Name },
                            Quantity = blend.Quantity,
                            BlendAmountUnit = EServiceReferenceData.Data.BlendAdditiveMeasureUnitCollection.FirstOrDefault(c => c.Name == CostUtils.GetUnitName(blend.Unit))
                        };

                    if (blend.Additives != null && blend.Additives.Count > 0)
                    {
                        int i = 1;
                        foreach (CostUtils.Additive add in (blend.Additives))
                        {
                            at = add.Id > 0
                                ? EServiceReferenceData.Data.AdditiveTypeCollection.FirstOrDefault(a => a.Id == add.Id)
                                : EServiceReferenceData.Data.AdditiveTypeCollection.FirstOrDefault(a => (a.Name ?? "").Trim().ToUpper() == add.Name.Trim().ToUpper());

                            if (at == null)
                                throw (new Exception("Additive Name was not found for '" + add.Name + "'! "));

                            if (bs.BlendAdditiveSections == null)
                                bs.BlendAdditiveSections = new List<BlendAdditiveSection>();

                            BlendAdditiveSection bas =
                                new BlendAdditiveSection()
                                {
                                    Id = i++,
                                    AdditiveType = new AdditiveType() { Id = at.Id, Name = at.Name },
                                    AdditiveBlendMethod = new AdditiveBlendMethod(),
                                    AdditiveAmountUnit = EServiceReferenceData.Data.BlendAdditiveMeasureUnitCollection.FirstOrDefault(c => c.Name == CostUtils.GetUnitName(add.Unit)),
                                    Amount = add.Quantity??0,
                                    BaseName = ""
                                };
                            bs.BlendAdditiveSections.Add(bas);
                        }
                    }
                    CostUtils.ProcessBlendSectionAvailability(bs, servicePointSbsId, ref outputAvailabilityCollection, DateTime.Now);
                }
            }
            return new JsonResult(new { costCollection = outputAvailabilityCollection }, new System.Text.Json.JsonSerializerOptions());
        }
    }
}