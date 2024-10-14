using eServiceOnline.Gateway;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule;
using Sesi.SanjelData.Entities.Common.BusinessEntities.WellSite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eServiceOnline.BusinessProcess
{
    public class BulkPlantProcess
    {

        public static List<RigJob> GetBulkPlantRigJobInformation(Collection<int> rigJobIds,Collection<int> servicePointIds)
        {
            List<RigJob> rigJobCollection = GetBulkPlantRigJobCollectionByRequestedConditionAndPaginated(rigJobIds,servicePointIds);

            return rigJobCollection;
        }

        private static List<RigJob> GetBulkPlantRigJobCollectionByRequestedConditionAndPaginated(Collection<int> rigJobIds,Collection<int> servicePointIds)
        {
            List<RigJob> allRigJobs = servicePointIds.Count > 0 ? eServiceOnlineGateway.Instance.GetRigJobsByIdsServicePoints(rigJobIds,servicePointIds) : eServiceOnlineGateway.Instance.GetRigJobsByIds(rigJobIds) ;

            return allRigJobs;
        }

    }
}
