using System.Collections.Generic;
using System.Linq;
using MetaShare.Common.Core.Entities;
using Sesi.LocalData.Entities.PostJobReport;
using MetaShare.Common.Core.Services;
using MetaShare.Common.Core.Services.Version;
using Sesi.LocalData.Daos.Interfaces.PostJobReport;
using Sesi.LocalData.Services.Interfaces.PostJobReport;

/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Services.PostJobReport
{
	public class PostJobReportService : ObjectVersionService<Entities.PostJobReport.PostJobReport>, IPostJobReportService
	{
		public PostJobReportService() : base(typeof (IPostJobReportDao))
		{
		}
		/*add customized code between this region*/
        public int CreatePostJobReport(Entities.PostJobReport.PostJobReport postJobReport)
        {
            postJobReport.Id = 0;
            postJobReport.SystemId = 0;
            var existingPostJobReport = this.SelectBy(postJobReport, new List<string> {"JobUniqueId"});
            if (existingPostJobReport != null)
            {
                foreach (var jobReport in existingPostJobReport)
                {
                    this.Delete(jobReport);
                }
            }

            return this.Insert(postJobReport);
        }

        public int UpdatePostJobReport(Entities.PostJobReport.PostJobReport postJobReport)
        {
            var existingPostJobReports = this.SelectBy(postJobReport, new List<string> {"JobUniqueId"});
            if (existingPostJobReports == null || existingPostJobReports.Count == 0)
            {
                return this.Insert(postJobReport);
            }
            else if (existingPostJobReports.Count > 1)
            {
                foreach (var jobReport in existingPostJobReports)
                {
                    this.Delete(jobReport);
                }
                return this.Insert(postJobReport);
            }
            else
            {
                var existingPostJobReport = existingPostJobReports.FirstOrDefault();
                existingPostJobReport.CallSheetNumber = postJobReport.CallSheetNumber;
                existingPostJobReport.SurfaceLocation = postJobReport.SurfaceLocation;
                existingPostJobReport.RevisedDirection = postJobReport.RevisedDirection;
                existingPostJobReport.AdditionalInformation = postJobReport.AdditionalInformation;
                existingPostJobReport.ClientName = postJobReport.ClientName;
                existingPostJobReport.IsDirectionRevised = postJobReport.IsDirectionRevised;
                existingPostJobReport.RigName = postJobReport.RigName;
                existingPostJobReport.JobType = postJobReport.JobType;
                existingPostJobReport.DownHoleLocation = postJobReport.DownHoleLocation;
                existingPostJobReport.JobDate = postJobReport.JobDate;

                return this.Update(existingPostJobReport);
            }

        }
        /*add customized code between this region*/

    }
}
