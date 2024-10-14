using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eServiceOnline.PrintingService.Reports.MTSData.Model
{
    public class MTSBlendUnloadSheetModel
    {
        public string ProductHaulId { get; set; }

        public string BinNum { get; set; }

        public string BaseTonnage { get; set; }

        public string ProductDes { get; set; }

        public string TotalTonnage { get; set; }
    }
}
