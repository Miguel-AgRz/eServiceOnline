using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eServiceOnline.Models.ProductHaul;

namespace eServiceOnline.Models.BlendBoard
{
    public class BlendViewModel
    {
        public BlendViewModel()
        {
            BlendChemicalModelList = new List<BlendChemicalModel>();
        }
        public IEnumerable<BlendChemicalModel>BlendChemicalModelList { get; set; }

    }
}
