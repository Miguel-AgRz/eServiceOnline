using System;
using System.Security.Principal;
using eServiceOnline.Data;
using MetaShare.Common.Foundation.Logging;
using MetaShare.Common.Foundation.Permissions;

namespace eServiceOnline.SecurityControl
{
    public class LoggingUtility
    {
        public static User WriteAccessRecord(string userName)
        {
            User user = eServiceWebContext.Instance.GetSecuredUserByApplicationAndUserName("Sanjel eService", userName);
            if (user != null)
            {
                AccessRecord accessRecord = eServiceWebContext.Instance.GetAccessRecordByUserId(user.Id);
                if (accessRecord == null)
                {
                    AccessRecord createAccessRecord = new AccessRecord
                    {
                        User = user,
                        LoginTime = DateTime.Now,
                        IsLogin = true
                    };
                    eServiceWebContext.Instance.CreateAccessRecord(createAccessRecord);
                }
                else
                {
                    AccessRecord currentAccessRecord = accessRecord;
                    currentAccessRecord.LoginTime = DateTime.Now;
                    currentAccessRecord.IsLogin = true;
                    eServiceWebContext.Instance.UpdateAccessRecord(currentAccessRecord, accessRecord);
                }
            }
            return user;
        }

        public static void LogoutAccess(int userId, string userName)
        {
            AccessRecord accessRecord;
            if (userId != 0)
            {
                accessRecord = eServiceWebContext.Instance.GetAccessRecordByUserId(userId);
            }
            else
            {
                User user = eServiceWebContext.Instance.GetSecuredUserByApplicationAndUserName("Sanjel eService", userName);
                accessRecord = eServiceWebContext.Instance.GetAccessRecordByUserId(user.Id);
            }
            
            if (!accessRecord.Id.Equals(0))
            {
                AccessRecord currentAccessRecord = accessRecord;
                currentAccessRecord.LogoutTime = DateTime.Now;
                currentAccessRecord.IsLogin = false;
                eServiceWebContext.Instance.UpdateAccessRecord(currentAccessRecord, accessRecord);
            }
        }
    }
}
