using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace eServiceOnline.Models.Commons
{
    public class RetrievalCondition
    {
        public int BR { get; set; }
        public int RD { get; set; }
        public int Edm { get; set; }
        public int Eds { get; set; }
        public int GP { get; set; }
        public int FSJ { get; set; }
        public int EST { get; set; }
        public int SC { get; set; }
        public int KD { get; set; }
        public int LLD { get; set; }
        public int LLB { get; set; }
        public int NW { get; set; }
        public int DrillingRig { get; set; }
        public int ServiceRig { get; set; }
        public int IsProjectRig { get; set; }

        public int IsPrejobTesting { get; set; }

        public int Alerted { get; set; }
        public int Pending { get; set; }
        public int Confirmed { get; set; }
        public int Dispatched { get; set; }
        public int Scheduled { get; set; }
        public int InProgress { get; set; }
        public int Completed { get; set; }
        public int IsShowFutureJobs { get; set; }
        public bool IsChange { get; set; }
        public int PageNumber { get; set; }
        public List<Collection<int>> ResuhSet(RetrievalCondition retrieval)
        {
            Collection<int> servicePoints = new Collection<int>(){ retrieval.BR,retrieval.RD,retrieval.Edm,retrieval.Eds,retrieval.GP,retrieval.FSJ, retrieval.NW, retrieval.EST,retrieval.SC,retrieval.KD,retrieval.LLD, retrieval.LLB};
            Collection<int> rigTypes = new Collection<int>(){ retrieval.IsProjectRig,retrieval.ServiceRig,retrieval.DrillingRig,retrieval.IsPrejobTesting };
            Collection<int> jobStatuses = new Collection<int>(){retrieval.Alerted,retrieval.Pending,retrieval.Completed,retrieval.Scheduled, retrieval.Confirmed,retrieval.InProgress,retrieval.Dispatched};
            Collection<int> isShowFutureJobs = new Collection<int>(){retrieval.IsShowFutureJobs };

            return new List<Collection<int>>()
            {
                servicePoints,
                rigTypes,
                jobStatuses,
                isShowFutureJobs
            };
        }
    }
}