using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eServiceOnline.PrintingService.Reports.MTSData.Model
{
    public class MtsItemsModel
    {
        public List<MTSModel> MtsItems { get; set; }


        public MtsItemsModel()
        {

        }

        public MtsItemsModel(List<MTSModel> mtsItems)
        {
            MtsItems = mtsItems;
        }
    }
}
