using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using eServiceOnline.BusinessProcess.Interface;
using eServiceOnline.Gateway;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Crew;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch;
using Sesi.SanjelData.Entities.Common.BusinessEntities.WellSite;
using RigJob = Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch.RigJob;
using UnitSection = Sanjel.BusinessEntities.Sections.Common.UnitSection;
using ThirdPartyUnitSection = Sanjel.Common.EService.Sections.Common.ThirdPartyUnitSection;
namespace eServiceOnline.BusinessProcess
{
    public class UpcomingJobsProcess : IUpcomingJobsProcess
    {
        #region Get All UpComingJob Information

        public List<RigJob> GetUpComingJobsInformation(int pageNumber, int pageSize, Collection<int> servicePointIds, int windowStart, int windowEnd, out int count)
        {
            List<RigJob> rigJobCollection = this.GetUpComingJobCollectionByRequestedConditionAndPaginated(pageNumber, pageSize, DateTime.Now.AddDays(windowStart), DateTime.Now.AddDays(windowEnd), new Collection<int>() {(int) JobLifeStatus.Deleted, (int) JobLifeStatus.None,(int) JobLifeStatus.Alerted}, servicePointIds, out count);

            return rigJobCollection;
        }

        public List<RigJob> GetUpComingJobCollectionByRequestedConditionAndPaginated(int pageNumber, int pageSize, DateTime startDateTime, DateTime endDateTime, Collection<int> rigJobStatus, Collection<int> servicePointIds, out int count)
        {
            List<RigJob> rigJobs = servicePointIds.Count > 0 ? eServiceOnlineGateway.Instance.GetRigJobsByServicePoints(servicePointIds) : eServiceOnlineGateway.Instance.GetRigJobs();
            if (rigJobStatus.Count > 0)
            {
                rigJobs = rigJobs.Where(p => !rigJobStatus.Contains((int) p.JobLifeStatus)).ToList();
            }
            rigJobs = rigJobs.Where(p => p.JobDateTime >= startDateTime && p.JobDateTime <= endDateTime).ToList();
            rigJobs = rigJobs.Where(p => p.RigStatus!=(RigStatus.Deactivated)).OrderBy(p => p.JobDateTime).ToList();
            count = rigJobs.Count;
            rigJobs = rigJobs.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();
            rigJobs = eServiceOnlineGateway.Instance.GetRigJobsWithChildren(rigJobs);

            return rigJobs;
        }

        public List<SanjelCrew> GetCrewsByRigJob(RigJob rigJob)
        {
            List<SanjelCrew> crews = new List<SanjelCrew>();

            if (rigJob.JobLifeStatus==(JobLifeStatus.Dispatched) || rigJob.JobLifeStatus==(JobLifeStatus.InProgress))
            {
                List<RigJobSanjelCrewSection> rigJobCrewSections = eServiceOnlineGateway.Instance.GetRigJobCrewSectionByRigJob(rigJob.Id);
                if (rigJobCrewSections != null && rigJobCrewSections.Count > 0)
                {
                    foreach (RigJobSanjelCrewSection rigJobCrewSection in rigJobCrewSections)
                    {
                        crews.Add(rigJobCrewSection.SanjelCrew);
                    }
                }
            }

            return eServiceOnlineGateway.Instance.GetSanjelCrewsWithChildren(crews);
        }

        #endregion
    }
}