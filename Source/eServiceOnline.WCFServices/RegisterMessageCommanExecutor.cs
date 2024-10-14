using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using OnlineService;
using Sanjel.Common.Messaging;
using Sesi.LocalData.Services.Interfaces.ChangeManagement;
using Sesi.LocalData.Services.Interfaces.PostJobReport;

namespace eServiceOnline.WCFServices
{
    public class RegisterMessageCommanExecutor
    {
        public static void RegisterAll()
        {
            RegisterMessageCommandExecutor(CreateServiceMessageCommandExecutor(typeof (IBlendConsumptionService)));
            RegisterMessageCommandExecutor(CreateServiceMessageCommandExecutor(typeof (IBlendReportService)));
            RegisterMessageCommandExecutor(CreateServiceMessageCommandExecutor(typeof (IMaintenanceNoteService)));
            RegisterMessageCommandExecutor(CreateServiceMessageCommandExecutor(typeof (IPostJobReportService)));
            RegisterMessageCommandExecutor(CreateServiceMessageCommandExecutor(typeof (IStorageInfoService)));
            RegisterMessageCommandExecutor(CreateServiceMessageCommandExecutor(typeof(IChangeRequestService)));
        }

        public static InstanceMessageCommandExecutor CreateServiceMessageCommandExecutor(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            object service = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService(type);
            if (service != null)
            {
                return new InstanceMessageCommandExecutor(service, type.Name);
            }
            return null;
        }

        public static void RegisterMessageCommandExecutor(MessageCommandExecutor messageCommandExecutor)
        {
            if (messageCommandExecutor != null)
            {
                MessageExecutor.AddExecutor(messageCommandExecutor);
            }
        }
    }
}