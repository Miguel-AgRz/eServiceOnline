using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eServiceOnline.Models.ProductHaul;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ResourceSchedule;

namespace eServiceOnline.Models.CrewBoard
{
    public class CrewScheduleViewModel
    {
        public SanjelCrewSchedule CrewSchedule { get; set; }
        public ProductHaulModel ProductHaulModel { get; set; }
    }
}
