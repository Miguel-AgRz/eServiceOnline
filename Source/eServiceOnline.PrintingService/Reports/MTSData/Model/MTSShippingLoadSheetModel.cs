using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eServiceOnline.PrintingService.Reports.MTSData.Model
{
    public class MTSShippingLoadSheetModel
    {
        public string ProductHaulId { get; set; }

        public string BlendRequestNum { get; set; }

        public double BaseTonnage { get; set; }

        public string ProductDes { get; set; }

        public double TotalTonnage { get; set; }

        public string Sample { get; set; }

        public string RequestedBy { get; set; }

        public string BlendedBy { get; set; }
    }
}
