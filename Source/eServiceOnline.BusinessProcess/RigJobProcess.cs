using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eServiceOnline.BusinessProcess.Interface;
using eServiceOnline.Gateway;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Crew;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule;

namespace eServiceOnline.BusinessProcess
{
    public class RigJobProcess
    {
        /*public static bool CompleteRigJob(int rigJobId, string jobUniqueId = null)
        {
            var rigJob = eServiceOnlineGateway.Instance.GetRigJobById(rigJobId);
            if (rigJob.JobLifeStatus == JobLifeStatus.Completed)
            {
                //The job has been completed from RigBoard, then update the unique id only
                rigJob.JobUniqueId = jobUniqueId;
            }
            else
            {
                //release pumper crew
                RigBoardProcess.ReleaseCrew(rigJobId, true, DateTime.Now);

                //TODO: Need to align with latest shipping load sheet on location, move following logic to process
                //product haul on location
                //1. set product load on location
                var productLoads =
                    eServiceOnlineGateway.Instance.GetProductHaulLoadsByCallSheetNumber(rigJob.CallSheetNumber);
                List<int> productHaulIds = new List<int>();
                if (productLoads != null)
                {
                    foreach (var productHaulLoad in productLoads)
                    {
                        if (!productHaulIds.Contains(productHaulLoad.ProductHaul.Id))
                            productHaulIds.Add(productHaulLoad.ProductHaul.Id);
                        RigBoardProcess.SetProductHaulLoadOnLocation(productHaulLoad, rigJob.JobDateTime, "eService");
                    }
                }
                //2. Check if product haul all loads are on location, then set product haul on location and release bulker crew
                if (productHaulIds.Count != 0)
                {
                    foreach (var productHaulId in productHaulIds)
                    {
                        ProductHaulProcess.SetProductHaulOnLocation(productHaulId, DateTime.Now, "eService");
                    }
                }
                //return equipments

                rigJob.JobLifeStatus = JobLifeStatus.Completed;
            }

            eServiceOnlineGateway.Instance.UpdateRigJob(rigJob);
            UpdateListedRigJob(rigJob.Rig.Id, rigJobId);
            return true;
        }*/

        /*
        public static bool CompleteRigJobByCallSheetNumber(int callSheetNumber)
        {
            int rigJobId = eServiceOnlineGateway.Instance.GetRigJobByCallSheetNumber(callSheetNumber).Id;
            return CompleteRigJob(rigJobId);
        }
        */

        public static RigJob UpdateListedRigJob(int rigId, int rigJobId=0)
        {
            RigJob rigJob = null;
            List<RigJob> rigjobs = eServiceOnlineGateway.Instance.GetRigJobsByRigId(rigId);

            if (rigjobs != null && rigjobs.Count != 0)
            {
                var currentListedJob = rigjobs.FirstOrDefault(p => p.IsListed == true);

                //Find earlist In Progress job
                var listedCandidate = rigjobs.Where(p => p.JobLifeStatus == JobLifeStatus.InProgress).OrderBy(p => p.JobDateTime).FirstOrDefault();

                if (listedCandidate == null)
                {
                    //Find earlist upcoming job
//                    listedCandidate = rigjobs.Where(p => p.JobLifeStatus.Equals(JobLifeStatus.Pending) || p.JobLifeStatus.Equals(JobLifeStatus.Confirmed) || p.JobLifeStatus.Equals(JobLifeStatus.Scheduled)) || p.JobLifeStatus.Equals(JobLifeStatus.Dispatched)).OrderBy(p => p.JobDateTime).FirstOrDefault());
                    listedCandidate = rigjobs.Where(p => p.JobLifeStatus == JobLifeStatus.Pending || p.JobLifeStatus == JobLifeStatus.Confirmed || p.JobLifeStatus == JobLifeStatus.Scheduled || p.JobLifeStatus == JobLifeStatus.Dispatched).OrderBy(p => p.JobDateTime).FirstOrDefault();
                    if (listedCandidate == null)
                    {
                        //Find last canceled/completed/Down job
                        listedCandidate = rigjobs.Where(p => p.JobLifeStatus == JobLifeStatus.Canceled || p.JobLifeStatus == JobLifeStatus.Completed || p.JobLifeStatus == JobLifeStatus.None).OrderByDescending(p => p.JobDateTime).ThenByDescending(p => p.Id).FirstOrDefault();
                    }
                }

                if (listedCandidate != null)
                {
                    if (currentListedJob == null)
                    {
                        listedCandidate.IsListed = true;
                        eServiceOnlineGateway.Instance.UpdateRigJob(listedCandidate);
                    }
                    else
                    {
                        if (currentListedJob.Id != listedCandidate.Id)
                        {
                            currentListedJob.IsListed = false;
                            listedCandidate.IsListed = true;
                            listedCandidate.Notes = currentListedJob.Notes;
                            eServiceOnlineGateway.Instance.UpdateRigJob(currentListedJob);
                            eServiceOnlineGateway.Instance.UpdateRigJob(listedCandidate);
                        }

                    }
                }

                if (rigJobId != 0) rigJob = eServiceOnlineGateway.Instance.GetRigJobById(rigJobId);
            }

            return rigJob;
        }
    }
}
