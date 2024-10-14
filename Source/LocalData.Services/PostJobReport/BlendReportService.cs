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
	public class BlendReportService : ObjectVersionService<BlendReport>, IBlendReportService
	{
		public BlendReportService() : base(typeof (IBlendReportDao))
		{
			ServiceAggregationInfo blendConsumptionsServiceAggregation = this.ServiceAggregationInfo.AddCompositeCollectionChild("BlendConsumptions", typeof(Sesi.LocalData.Entities.PostJobReport.BlendConsumption), typeof(Sesi.LocalData.Daos.Interfaces.PostJobReport.IBlendConsumptionDao), "BlendReport");
		}
		/*add customized code between this region*/
        public int CreateBlendReport(BlendReport blendReport)
        {
            blendReport.Id = 0;
            blendReport.SystemId = 0;
            foreach (var blendReportBlendConsumption in blendReport.BlendConsumptions)
            {
                blendReportBlendConsumption.Id = 0;
                blendReportBlendConsumption.SystemId = 0;
            }
            var existingBlendReports = this.SelectBy(blendReport, new List<string> {"JobUniqueId"});
            if (existingBlendReports != null)
            {
                foreach (var report in existingBlendReports)
                {
                    var reportWithChildren = this.SelectById(report, true);
                    if(reportWithChildren != null)
                       this.Delete(reportWithChildren,true);
                }
            }

            return this.Insert(blendReport, true);
        }

        public int UpdateBlendReport(BlendReport blendReport)
        {
            var existingBlendReports = this.SelectBy(blendReport, new List<string> {"JobUniqueId"});
            if (existingBlendReports == null || existingBlendReports.Count == 0)
            {
                return this.Insert(blendReport,true);
            }
            else if (existingBlendReports.Count > 1)
            {
                foreach (var report in existingBlendReports)
                {
                    var reportWithChildren = this.SelectById(report, true);
                    if(reportWithChildren != null)
                        this.Delete(reportWithChildren,true);
                }
                return this.Insert(blendReport,true);
            }
            else
            {
                var existingBlendReport = existingBlendReports.FirstOrDefault();
                var reportWithChildren = this.SelectById(existingBlendReport, true);

                reportWithChildren.TotalPumpedVolume = blendReport.TotalPumpedVolume;
                reportWithChildren.ExpectedCementTop = blendReport.ExpectedCementTop;

                for (int i = reportWithChildren.BlendConsumptions.Count - 1; i >= 0; i--)
                {
                    var incomingBlendConsumption =  blendReport.BlendConsumptions.Find(p =>
                        p.JobIntervalId == reportWithChildren.BlendConsumptions[i].JobIntervalId &&
                        p.JobEventNumber == reportWithChildren.BlendConsumptions[i].JobEventNumber);
                    if(incomingBlendConsumption == null)
                        reportWithChildren.BlendConsumptions.RemoveAt(i);
                }

                List<BlendConsumption> newConsumptions = new List<BlendConsumption>();

                foreach (var blendReportBlendConsumption in blendReport.BlendConsumptions)
                {
                    var existingBlendConsumption =  reportWithChildren.BlendConsumptions.Find(p =>
                        p.JobIntervalId == blendReportBlendConsumption.JobIntervalId &&
                        p.JobEventNumber == blendReportBlendConsumption.JobEventNumber);
                    if(existingBlendConsumption == null)
                        newConsumptions.Add(blendReportBlendConsumption);
                    else
                    {
                        existingBlendConsumption.PumpedVolume = blendReportBlendConsumption.PumpedVolume;
                        existingBlendConsumption.WaterTemperature = blendReportBlendConsumption.WaterTemperature;
                        existingBlendConsumption.BulkTemperature = blendReportBlendConsumption.BulkTemperature;
                        existingBlendConsumption.SlurryTemperature = blendReportBlendConsumption.SlurryTemperature;
                    }
                }

                if (newConsumptions.Count>0)
                    reportWithChildren.BlendConsumptions.AddRange(newConsumptions);

                return this.Update(reportWithChildren,true);
            }
        }
        /*add customized code between this region*/


    }
}
