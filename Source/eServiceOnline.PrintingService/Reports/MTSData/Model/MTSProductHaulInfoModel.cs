using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eServiceOnline.PrintingService.Reports.MTSData.Model
{
    public class MTSProductHaulInfoModel
    {
        public string CreateDateTime { get; set; }

        public string  UnitNum { get; set; }
        public string DriverName { get; set; }
        public string GoWithDriverName { get; set; }

        public string EstimatedLoadTime { get; set; }

        public string EstimatedLocationTime { get; set; }

        public string Station { get; set; }

        public string Client { get; set; }

        public string ClientRep { get; set; }

        public string RigName { get; set; }

        public string Location { get; set; }

        public string DispatchedBy { get; set; }

        public bool IsBackHaul { get; set; }

        public string WellLocation { get; set; }

        public string BaseBlend { get; set; }


        public string BulkPlant { get; set; }

    }
}
