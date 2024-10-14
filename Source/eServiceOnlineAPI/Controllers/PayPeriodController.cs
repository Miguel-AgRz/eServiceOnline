using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MetaShare.Common.Core.CommonService;
using Microsoft.AspNetCore.Mvc;
using Sesi.SanjelData.Entities.BusinessEntities.VariablePay;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Organization;
using Sesi.SanjelData.Services.Interfaces.BusinessEntities.VariablePay;
using Sesi.SanjelData.Services.Interfaces.Common.BusinessEntities.Organization;

namespace eServiceOnline.WebAPI.Controllers
{
    [Route("[controller]/[action]/{payPeriodId}")]
    [ApiController]
    public class PayPeriodController: ControllerBase
    {
        //Sample: http://localhost:52346/PayPeriod/GenerateDefaultPayPeriodSummary/1
        public ActionResult GenerateDefaultPayPeriodSummary(int payPeriodId)
        {
            IPayPeriodSummaryService payPeriodSummaryService = ServiceFactory.Instance.GetService<IPayPeriodSummaryService>();
            IPayPeriodService payPeriodService = ServiceFactory.Instance.GetService<IPayPeriodService>();
            PayPeriod payPeriod = payPeriodService.SelectById(new PayPeriod(){Id=payPeriodId});

            if (payPeriod != null)
            {
                List<ServicePoint> servicePoints =
                    ServiceFactory.Instance.GetService<IServicePointService>().SelectAll();
                foreach (var servicePoint in servicePoints)
                {
                    var payPeriodSummaryList =
                        payPeriodSummaryService.SelectByPayPeriodServicePoint(payPeriodId, servicePoint.Id);
                    if (payPeriodSummaryList == null || payPeriodSummaryList.Count == 0)
                    {
                        var payPeriodSummary = new PayPeriodSummary()
                        {
                            Name=payPeriod.Name,
                            ServicePoint=servicePoint,
                            PayPeriod = payPeriod,
                            Status = PayrollStatus.Open,
                            UniqueId = Guid.NewGuid().ToString(),
                            Amount = 0
                        };
                        payPeriodSummaryService.Insert(payPeriodSummary);
                    }
                }
                return new JsonResult(true);
            }

            return new JsonResult(false);
        }
        public ActionResult GenerateNextDefaultPayPeriodSummary()
        {
            DateTime today = DateTime.Now;



            return new JsonResult(true);
        }

    }
}
