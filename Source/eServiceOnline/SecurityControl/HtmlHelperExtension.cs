using System;
using eServiceOnline.Data;
using MetaShare.Common.Foundation.Permissions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eServiceOnline.SecurityControl
{
    public static class HtmlHelperExtension
    {
        public static IHtmlContent ActionLink(this IHtmlHelper htmlHelper,
            string linkText, string actionName, string controllerName, object routeValues, object htmlAttributes, User extension)
        {
            IHtmlContent htmlContent = HtmlString.Empty;
           var loggedUser = htmlHelper.ViewContext.HttpContext.User.Identity.Name.Split('\\')[1];
//           var loggedUser = "awang";
            string permission = String.Format("{0}{1}", controllerName, actionName);
            Boolean getAccess = VerifyPermission(permission, loggedUser);
            if (getAccess)
                htmlContent = htmlHelper.ActionLink(linkText, actionName, controllerName, routeValues, htmlAttributes);

            return htmlContent;
        }

        public static bool VerifyPermission(string permission, string userName)
        {
            User secUser = eServiceWebContext.Instance.GetSecuredUserByApplicationAndUserName("Sanjel eService", userName);
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
    }
}