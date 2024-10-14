using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eServiceOnline.BusinessProcess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eServiceOnlineAPI.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class RigJobAPIController : ControllerBase
    {
        public ActionResult CompleteRigJob(int callSheetNumber)
        {
//            RigJobProcess.CompleteRigJobByCallSheetNumber(callSheetNumber);
            return Ok(null);
        }
    }
}