using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule;
using System.Collections.Generic;

namespace eServiceOnline.Models.ProductHaul
{
    public class CancelProductViewModel
    {
        public int ProductHaulId { set; get; }
        public int ProductHaulLoadId { set; get; }
        public string LoggedUser { set; get; }
        public List<CheckShipingLoadSheetModel> CheckShipingLoadSheetModels { set; get; }

    }

}
