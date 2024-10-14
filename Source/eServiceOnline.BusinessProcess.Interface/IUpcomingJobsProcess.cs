using System.Collections.Generic;
using System.Collections.ObjectModel;
using RigJob = Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch.RigJob;

namespace eServiceOnline.BusinessProcess.Interface
{
    public interface IUpcomingJobsProcess
    {
        List<RigJob> GetUpComingJobsInformation(int pageNumber, int pageSize, Collection<int> servicePointIds, int windowStart, int windowEnd, out int count);
    }
}