using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sesi.SanjelData.Entities.Common.BusinessEntities.HumanResources;

namespace eServiceOnline.BusinessProcess
{
    public static class Utility
    {
        public static string PreferredName(Employee employee)
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
