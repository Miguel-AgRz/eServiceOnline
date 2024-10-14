using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eServiceOnline.PrintingService.Reports.MTSData.Model
{
    public class MTSProductHaulLoadModel
    {
        public string ProductHaulId { get; set; }

        public string BlendRequestNum { get; set; }

        public string BaseTonnage { get; set; }

        public string ProductDes { get; set; }

        public string TotalTonnage { get; set; }

        public string Sample { get; set; }

        public string RequestedBy { get; set; }

        public string BlendedBy { get; set; }

        public string BaseBlendName { get; set; }

        public string WellLocation { get; set; }
    }
}
