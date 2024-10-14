using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using eServiceOnline.Data;
using eServiceOnline.Models.Commons;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace eServiceOnline.Controllers
{
    public class eServiceOnlineController : Controller
    {
        private MetaShare.Logging.ILogger _logger;

        public MetaShare.Logging.ILogger Logger
        {
            get
            {
                if (this._logger != null)
                    return this._logger;

                this._logger = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<MetaShare.Logging.ILogger>();

                return this._logger;
            }
        }

        //On IIS, the website Authentication should be Windows Authentication
        public string LoggedUser
        {
            get
            {
                if (this.User == null)
                {
                    throw new Exception("this.User is null");
                }
                if (this.User.Identity == null)
                {
                    throw new Exception("this.User.Identity is null");

                }
                if (string.IsNullOrEmpty(this.User.Identity.Name))
                {
                    throw new Exception("this.User.Identity.Name is null");
                }
                var loggedUser = this.User.Identity.Name == null ? string.Empty : this.User.Identity.Name.Split('\\')[1];
//                var loggedUser = "awang";
                return loggedUser;
            }
        }

        public ActionResult ProcessContextMenu(ContextMenu model)
        {
            if (!string.IsNullOrEmpty(model.ControllerName) && !string.IsNullOrEmpty(model.ActionName))
            {
                return this.RedirectToAction(model.ActionName, model.ControllerName, new { Parms = model.Parms });
            }

            return this.RedirectToAction("Index", "RigBoard");
        }

        //Nov 30, 2023 zhangyuan 203_PR_UpdateBinNotes: modify Add PodIndex params
        public IActionResult UpdateNotes(List<string> parms)
        {
            NoteModel noteModel = new NoteModel();
            noteModel.Id = int.Parse(parms[0]);
            noteModel.PodIndex = int.Parse(parms[1]);
            noteModel.ReturnActionName = parms[2];
            noteModel.ReturnControllerName = parms[3];
            noteModel.PostControllerName = parms[4];
            noteModel.PostMethodName = parms[5];
            if (parms.Count > 6)
            {
                var base64EncodedBytes = System.Convert.FromBase64String(parms[6]);
                noteModel.Notes = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            }
            else
                noteModel.Notes = String.Empty;

            return PartialView("_UpdateNotes", noteModel);

        }
        public IActionResult UpdateProfile(List<string> parms)
        {
            ProfileModel profileModel = new ProfileModel();
            profileModel.Id = int.Parse(parms[0]);
            profileModel.ReturnActionName = parms[1];
            profileModel.ReturnControllerName = parms[2];
            profileModel.PostControllerName = parms[3];
            profileModel.PostMethodName = parms[4];
            profileModel.Profile = parms.Count > 5 ? parms[5] : string.Empty;
            return PartialView("_UpdateProfile", profileModel);

        }

        [HttpPost]
        public virtual IActionResult UpdateNotes(NoteModel model)
        {
            return null;
        }
        [HttpPost]
        public virtual IActionResult UpdateProfile(ProfileModel model)
        {
            return null;
        }

        protected List<SelectListItem> GetEnumValues(Type type)
        {
            List<SelectListItem> selectListItems  = new List<SelectListItem>();
            
            foreach(int i in Enum.GetValues(type))
            {
                SelectListItem selectListItem = new SelectListItem { Text = this.GetEnumDescription(type,i), Value = i.ToString() };
                selectListItems.Add(selectListItem);
            }
            
            return selectListItems;
        }

        public string GetEnumDescription(Type type, int value)
        {
            string name = Enum.GetName(type, value);
            FieldInfo fieldInfo = type.GetField(name);

            object[] attrs = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if(attrs != null && attrs.Length > 0)
            {
                return ((DescriptionAttribute)attrs[0]).Description;
            }
            return value.ToString();
        }

    }
}
