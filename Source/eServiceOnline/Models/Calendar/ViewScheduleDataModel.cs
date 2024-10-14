using System.Collections.Generic;
using eServiceOnline.Controllers;
using eServiceOnline.Models.Commons;
using Sanjel.BusinessEntities.RigJobs;


namespace eServiceOnline.Models.Calendar
{
    public class ViewScheduleDataModel
    {

        public List<ScheduleModel> ScheduleModels { get; set; }
        public List<ScheduleStyleModel> ScheduleStyleModels { get; set; }

       
    }
}