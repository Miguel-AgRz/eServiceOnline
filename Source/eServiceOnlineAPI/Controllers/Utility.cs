using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Principal;
using MetaShare.Common.Core.Entities;
using Sanjel.Common.BusinessEntities.Reference;
using MetaShare.Common.Foundation.EntityBases;
//using Sanjel.Common.Security.Services;
using Employee=Sesi.SanjelData.Entities.Common.BusinessEntities.HumanResources.Employee;

namespace eServiceOnlineAPI.Controllers
{
    public class Utility
    {
        public static string PreferedName(Employee employee)
        {
            string firstToken = employee?.FirstName;
            string secondToken = employee?.LastName;

            if (!string.IsNullOrEmpty(employee?.PreferedFirstName))
                firstToken = employee.PreferedFirstName;

            if (string.IsNullOrEmpty(firstToken) && string.IsNullOrEmpty(secondToken))
                return string.Empty;
            return string.Format("{0}, {1} ({2})", secondToken, firstToken, employee.EmployeeNumber);
        }

    }
}