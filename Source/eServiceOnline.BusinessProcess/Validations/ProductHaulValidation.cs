using System;
using System.Collections.Generic;
using System.Linq;
using eServiceOnline.Gateway;
//using Sanjel.BusinessEntities.Sections.Common;
using Sanjel.Common.BusinessEntities.Mdd.Products;
using Sanjel.Common.Domain.UnitOfMeasure;
using Sesi.SanjelData.Entities.BusinessEntities.BaseEntites;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Products;
using sesi.SanjelLibrary.BlendLibrary;

namespace eServiceOnline.BusinessProcess
{
    public class ProductHaulValidation
    {
        public static string ValidateAmountAndMixWater(int callSheetNumber, int baseBlendSectionId, double amount,
            double mixWater, bool isTotalBlendTonnage, int productHualLoadId, string programNumber)
        {
            Sesi.SanjelData.Entities.BusinessEntities.BaseEntites.BlendSection blendSection = 
                string.IsNullOrEmpty(programNumber)?eServiceOnlineGateway.Instance.GetBlendSectionByBlendSectionId(baseBlendSectionId):
                eServiceOnlineGateway.Instance.GetProgramBlendSectionByBlendSectionId(baseBlendSectionId);
            if (blendSection == null) throw new Exception("blendSection can not be null.");

            foreach (BlendAdditiveSection entity in blendSection.BlendAdditiveSections)
            {
                if (entity.AdditiveAmountUnit.Description.Equals("% BWOW"))
                {
                    if (mixWater <= 0)
                    {
                        return "MixWaterIsRequired";
                    }
                }
            }
            double newBaseAmount;
            double oldBaseAmount;
            CalculateBaseAmountForBlendChemicial(productHualLoadId, blendSection, amount, isTotalBlendTonnage, mixWater, out newBaseAmount, out oldBaseAmount);
            if (CalculateRemainsAmountForBlendChemicial(blendSection, newBaseAmount, oldBaseAmount) < 0)
            {
                return "AmountExceedsTheLimit";
            }

            return "Pass";
        }

        private static void CalculateBaseAmountForBlendChemicial(int productHaulLoadId, BlendSection blendSection,
            double amount, bool isTotalBlendTonnage, double mixWater, out double newBaseAmount,
            out double oldBaseAmount)
        {
            if (productHaulLoadId > 0)
            {
                ProductHaulLoad productHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(productHaulLoadId);
                if (productHaulLoad.BaseBlendWeight == null) throw new Exception("ProductHaul BaseBlendWeight can not be null.");
                oldBaseAmount = productHaulLoad.BaseBlendWeight/1000;
            }
            else
            {
                oldBaseAmount = 0;
            }

            if (isTotalBlendTonnage)
            {
                double totalBlendWeight;
                BlendAdditiveMeasureUnit blendUnit =
                    EServiceReferenceData.Data.BlendAdditiveMeasureUnitCollection.FirstOrDefault(p =>
                        p.Description.Equals(blendSection.BlendAmountUnit.Description));

                BlendChemical blendChemical = BlendCalculator.CovertToBlendChemicalFromBlendSection(CacheData.BlendChemicals, blendSection, CacheData.AdditionMethods, CacheData.AdditiveBlendMethods, CacheData.BlendAdditiveMeasureUnits, CacheData.BlendRecipes);
                BlendCalculator.GetBaseBlendWeightAndTotalBlendWeightConvertor(CacheData.BlendChemicals, CacheData.BlendRecipes, blendChemical.BlendRecipe, amount, true, mixWater, out totalBlendWeight, out newBaseAmount, blendSection.Yield, blendUnit);
            }
            else
            {
                newBaseAmount = amount;
            }
        }

        private static double CalculateRemainsAmountForBlendChemicial(BlendSection blendSection, double newBaseAmount,
            double oldBaseAmount)
        {
            if (blendSection.Quantity == null) throw new Exception("blendSection Quantity can not be null.");
            double remains = blendSection.Quantity??0.0;

            List<ProductHaulLoad> productHaulLoads = eServiceOnlineGateway.Instance.GetProductHaulLoadByBlendSectionId(blendSection.Id);
            if (productHaulLoads != null && productHaulLoads.Count > 0)
            {
                foreach (ProductHaulLoad item in productHaulLoads)
                {
                    remains = remains - (item.BaseBlendWeight/1000);
                }
            }
            remains = remains + oldBaseAmount - newBaseAmount;

            return remains;
        }
    }
}