using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eServiceOnline.Models.ModelBinder;
using Microsoft.AspNetCore.Mvc;

namespace eServiceOnline.Models.ProductHaul
{
    [ModelBinder(BinderType = typeof(CommonModelBinder<HaulBackFromBinModel>))]
    public class HaulBackFromBinModel
    {
        public string SourceBinInformationName { set; get; }
        public int SourceBinInformationId { set; get; }
        public string SourceBaseBlend { get; set; }
        public string SourceBlendChemicalDescription { get; set; }
        public double SourceAmount { get; set; }
        public int sourceRigId { get; set; }
        public string sourceRigName { get; set; }
        public int DestinationBinInformationId { set; get; }

    }
}
