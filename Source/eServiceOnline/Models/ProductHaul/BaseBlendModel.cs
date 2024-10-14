using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eServiceOnline.Models.ModelBinder;
using Microsoft.AspNetCore.Mvc;

namespace eServiceOnline.Models.ProductHaul
{
    [ModelBinder(BinderType = typeof(CommonModelBinder<BaseBlendModel>))]
    public class BaseBlendModel
    {
        public string BaseBlend { get; set; }
        public int BaseBlendSectionId { get; set; }
        public string BlendChemicalDescription { get; set; }
    }
}
