using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eServiceOnline.Models.ProductHaul
{
    public class LoadBlendToBinViewModel
    {
        public int ProductHaulLoadId { set; get; }
        public string BlendChemicalDescription { set; get; }
        public string BlendInBinDescription { set; get; }
        public string BinInformationName { set; get; }
        public string BulkPlantName { set; get; }
        public string BlendQuantity { set; get; }
        public bool IsSameBlendInBin { set; get; }
        public bool IsBinEmpty { set; get; }
    }
   
}
