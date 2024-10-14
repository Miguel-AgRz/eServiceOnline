using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using eServiceOnline.Data;
using MetaShare.Common.Core.CommonService;
using Microsoft.Extensions.Caching.Memory;
using MetaShare.Common.Foundation.Permissions;

namespace eServiceOnline.SecurityControl
{
    public class SecurityUtility
    {
        public static bool HavePermission(string permission, string userName, IMemoryCache cache)
        {
            var secUser = cache.Get<User>(userName);
            if (secUser != null && secUser.Groups != null)
            {
                foreach (Group gr in secUser.Groups)
                {
                    foreach (UserRight prm in gr.Permissions)
                    {
                        if (prm.Name.Trim().ToLower() == permission.Trim().ToLower())
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
/*        public static bool HavePermission(string permissionName, string userName, IMemoryCache cache)
        {
            return true;
            bool hasPermission = false;

            var userGroupSanjelUserService = ServiceFactory.Instance.GetService<IUserGroupSanjelUserService>();
            List<UserGroupSanjelUser> userGroupSanjelUsers = userGroupSanjelUserService.SelectBy(new UserGroupSanjelUser(), userGroupSanjelUser=>userGroupSanjelUser.SanjelUser.Name==userName);
            if (userGroupSanjelUsers != null)
            {
                int[] userGroupId =  userGroupSanjelUsers.Select(p => p.UserGroup.Id).Distinct().Except(new []{0}).ToArray();

                if (userGroupId.Length > 0)
                {
                    IUserGroupUserPermissionService userGroupUserPermissionService =
                        ServiceFactory.Instance.GetService<IUserGroupUserPermissionService>();

                    var userGroupUserPermissions = userGroupUserPermissionService.SelectBy(new UserGroupUserPermission(),
                        userGroupUserPermission => userGroupUserPermission.UserPermission.Name == permissionName &&
                                                   userGroupId.Contains(userGroupUserPermission.UserGroup.Id));
                    if (userGroupUserPermissions.Count == 1) hasPermission = true;
                }

            }

            return hasPermission;
        }*/
        public static void SetSecurityData(string userName, IMemoryCache cache)
        {
            User secUser = null;
            
            if (!cache.TryGetValue(userName, out secUser))
            {
                secUser = eServiceWebContext.Instance.GetSecuredUserByApplicationAndUserName("Sanjel eService", userName);
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(5))
                    .SetPriority(CacheItemPriority.Normal);

                if (secUser != null)
                {
                    cache.Set(userName, secUser, cacheEntryOptions);
                }
                        
            }
        }
    }
}
