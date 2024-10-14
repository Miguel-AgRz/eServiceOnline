using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eServiceOnline.Models.ModelBinder;
using Microsoft.AspNetCore.Mvc;

namespace eServiceOnline.Models.ProductHaul
{
    [ModelBinder(BinderType = typeof(CommonModelBinder<BulkPlantBinLoadModel>))]
    public class BulkPlantBinLoadModel
    {
        public int BulkPlantId { get; set; }
        public string BulkPlantName { get; set; }
        public string BinInformationName { set; get; }
        public int BinInformationId { set; get; }
        public int CallSheetNumber { get; set; }
        public string ClientName { get; set; }
        public int ClientId { get; set; }
        public string BaseBlend { get; set; }
        public int BaseBlendSectionId { get; set; }
        public string BlendChemicalDescription { get; set; }
        //Dec 11, 2023 zhangyuan 226_PR_CalcRemainsAmount: And  calculate remains amount
        public double Amount { get; set; }
        public double RemainsAmount { set; get; }

    }
}
