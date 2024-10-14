using System;
using System.Collections.Generic;
using eServiceOnline.Models.RigBoard;
using eServiceOnline.SecurityControl;
using Microsoft.Extensions.Caching.Memory;
using MetaShare.Common.Foundation.Permissions;

namespace eServiceOnline.Models.Commons
{
    public class StyledCell
    {
        public const string DefaultStyle = "generalStyle";

        public string PropertyName { get; set; }
        public string PropertyValue { get; set; }
        public string Style { get; set; }
        public string Notice { get; set; }
        public List<ContextMenu> ContextMenus { get; set; }
        public bool IsNeedRowMerge { get; set; }
        public int RowMergeNumber { get; set; }
        public IMemoryCache _memoryCache { get; set; }

        public string LoggedUser { get; set; }

        public bool IsDisplayMenu
        {
            get
            {
                if (_memoryCache !=null && !string.IsNullOrEmpty(LoggedUser) && ViewModelType != null)
                {
                    return SecurityUtility.HavePermission(ViewModelType.Name + '_' + PropertyName, LoggedUser, _memoryCache);
                }
                return false;
            }
        }

        private Type ViewModelType { get; set; }

        public StyledCell(string propertyName, Type viewModelType, string userName, IMemoryCache memoryCache)
        {
            this.PropertyName = propertyName;
            if (viewModelType !=null || memoryCache !=null)
            {
                this.ContextMenus = new List<ContextMenu>();
                this.ViewModelType = viewModelType;
                this._memoryCache = memoryCache;
                this.LoggedUser = userName;
            }
        }

        public string ComputeStyle(string propertyName, string statusName, string suffix)
        {
            string style;
            if (string.IsNullOrEmpty(statusName))
            {
                style = DefaultStyle;
            }
            else
            {
                style = propertyName.ToLower() + "-" + statusName.ToLower();
            }

            if (!string.IsNullOrEmpty(suffix))
            {
                style = style + "-" + suffix.ToLower();
            }

            return style;
        }
    }
}