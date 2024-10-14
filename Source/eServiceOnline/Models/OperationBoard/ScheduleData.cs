using System;
using System.Collections.Generic;

namespace eServiceOnline.Models.OperationBoard
{
    public class ScheduleData
    {
        public int ProgramId { get; set; }
        public string ProgramName { get; set; }
        public string Comments { get; set; }
        public DateTime ProgramStartTime { get; set; }
        public DateTime ProgramEndTime { get; set; }
        public bool IsAllDay { get; set; }
        public bool IsRecurrence { get; set; }
        public string RecurrenceRule { get; set; }
        public string DownholeLocation { get; set; }
        public string Customer { get; set; }

        // Method that passes the Scheduler appointment data
        public List<ScheduleData> GetSchedulerData()
        {
            List<ScheduleData> data = new List<ScheduleData> {
             new ScheduleData {
                    ProgramName = "Turtle Walk",
                    Comments = "Night out with turtles",
                    ProgramStartTime = DateTime.ParseExact("06/06/2017 16:00:00,000","MM/dd/yyyy HH:mm:ss,fff", System.Globalization.CultureInfo.InvariantCulture),
                    ProgramEndTime = DateTime.ParseExact("06/06/2017 17:00:00,000","MM/dd/yyyy HH:mm:ss,fff", System.Globalization.CultureInfo.InvariantCulture),
                    IsAllDay = true
             },
             new ScheduleData {
                    ProgramName = "Winter Sleepers",
                    Comments = "Long sleep during winter season",
                    ProgramStartTime = DateTime.ParseExact("06/07/2017 08:00:00,000","MM/dd/yyyy HH:mm:ss,fff", System.Globalization.CultureInfo.InvariantCulture),
                    ProgramEndTime = DateTime.ParseExact("06/07/2017 08:00:00,000","MM/dd/yyyy HH:mm:ss,fff", System.Globalization.CultureInfo.InvariantCulture)
             },
             new ScheduleData {
                    ProgramName = "Estivation",
                    Comments = "Sleeping in hot season",
                    ProgramStartTime = DateTime.ParseExact("06/08/2017 11:00:00,000","MM/dd/yyyy HH:mm:ss,fff", System.Globalization.CultureInfo.InvariantCulture),
                    ProgramEndTime = DateTime.ParseExact("06/08/2017 13:00:00,000","MM/dd/yyyy HH:mm:ss,fff", System.Globalization.CultureInfo.InvariantCulture)
             }
           };
            return data;
        }
    }
}
