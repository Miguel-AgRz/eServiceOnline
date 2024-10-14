using System.Collections.Generic;
using System.Reflection;
using eServiceOnline.Data;
using eServiceOnline.Models.OperationBoard;
using Microsoft.AspNetCore.Mvc;
using RigJob = Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch.RigJob;

namespace eServiceOnline.Controllers
{
    public class OperationBoardController : eServiceOnlineController
    {
        private readonly IeServiceWebContext _context;
        public OperationBoardController()
        {
            this._context = eServiceWebContext.Instance;
        }

        public ActionResult Index()
        {
            this.ViewBag.HighLight = "OperationBoard";
            ViewBag.VersionNumber = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            return this.View();
        }

        public List<ScheduleBoardData> GetRigJobData()
       {
           List<ScheduleBoardData> scheduleBoardDatas = new List<ScheduleBoardData>();
           List<RigJob> rigJobList = this._context.GetRigJobCollectionForOperation();
           foreach (var entity in rigJobList)
           {
                ScheduleBoardData scheduleBoardData = new ScheduleBoardData();
               if (entity != null)
               {
                    scheduleBoardData.GetRigJobTransformDisplay(entity);
                    scheduleBoardDatas.Add(scheduleBoardData);
                }
            }
           ScheduleBoardStyle scheduleBoardStyle = new ScheduleBoardStyle();
           this.ViewBag.ScheduleBoardStyles = scheduleBoardStyle.GetScheduleBoardStyles();

           return scheduleBoardDatas;
       }
    }
}
