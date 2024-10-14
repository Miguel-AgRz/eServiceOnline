using eServiceOnline.WebAPI.Data.VariablePay;
using Microsoft.AspNetCore.Mvc;

namespace eServiceOnline.WebAPI.Controllers
{
    [Route("[controller]/[action]/{jobUniqueId}")]
    [ApiController]
    public class VariablePayController:ControllerBase
    {
        //Sample: http://localhost:52346/VariablePay/CalculateVariablePayToJob/2262543b-fb62-47e4-ba61-8a577a172751
        public ActionResult CalculateVariablePayToJob(string jobUniqueId)
        {
            return new JsonResult(VariablePayData.CalculateVariablePay(jobUniqueId));
        }

    }
}
