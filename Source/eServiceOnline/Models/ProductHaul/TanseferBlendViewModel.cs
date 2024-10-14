using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//Nov 13, 2023 zhangyuan P63_Q4_174: add TransferBlend 
namespace eServiceOnline.Models.ProductHaul
{
    public class TanseferBlendViewModel
    {
        public int BinInformationId { set; get; }
        public string BlendToLoadDescription { set; get; }
        public string BlendInBinDescription { set; get; }
        public string BinInformationName { set; get; }
        public string ToBinInformationName { set; get; }
        public int ToBinInformationId { set; get; }
        public string BulkPlantOrRigName { set; get; }
        public int BulkPlantOrRigId { set; get; }
        public string BlendQuantity { set; get; }
        public bool IsSameBlendInBin { set; get; }
        public bool IsBinEmpty { set; get; }
        public bool IsOnlyOneTransferBin { set; get; }
        public double TransferQuantity { set; get; }
        // 1:rigjob 2:bulkplant
        public int ShowPageType { set; get; }
    }
}
