using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Principal;
using eServiceOnline.Models.Commons;
using MetaShare.Common.Core.Entities;
using Sanjel.Common.BusinessEntities.Reference;
using MetaShare.Common.Foundation.EntityBases;
using Sanjel.Common.Security.Services;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch;
using Employee=Sesi.SanjelData.Entities.Common.BusinessEntities.HumanResources.Employee;
namespace eServiceOnline.Controllers
{
    public class Utility
    {
        public const string Ascending = "ascending";
        public const string Descending = "descending";

        #region sort
        public static List<T> Sort<T>(List<T> data, string sortDirection, string sortPropertyName)
        {
            Type type = typeof(T);
            PropertyInfo propertyInfo = type.GetProperty(sortPropertyName);
            if (propertyInfo == null) throw new Exception("sortPropertyName is not a property in current class.");

            ParameterExpression param = Expression.Parameter(type, type.Name);
            MemberExpression body = Expression.Property(param, sortPropertyName);
            LambdaExpression keySelector = Expression.Lambda(body, param);

            data = SortByProperty(data, propertyInfo, keySelector, sortDirection);

            return data;
        }

        private static List<T> SortByProperty<T>(List<T> data, PropertyInfo propertyInfo, LambdaExpression keySelector, string sortDirection)
        {
            Type propertyType = propertyInfo.PropertyType;

            if (propertyType == typeof(int))
            {
                data = sortDirection.Equals(Descending) ? data.OrderByDescending(keySelector.Compile() as Func<T, int>).ToList() : data.OrderBy(keySelector.Compile() as Func<T, int>).ToList();
            }
            else if (propertyType == typeof(string))
            {
                data = sortDirection.Equals(Descending) ? data.OrderByDescending(keySelector.Compile() as Func<T, string>).ToList() : data.OrderBy(keySelector.Compile() as Func<T, string>).ToList();
            }
            else if (propertyType == typeof(decimal))
            {
                data = sortDirection.Equals(Descending) ? data.OrderByDescending(keySelector.Compile() as Func<T, decimal>).ToList() : data.OrderBy(keySelector.Compile() as Func<T, decimal>).ToList();
            }
            else if (propertyType == typeof(double))
            {
                data = sortDirection.Equals(Descending) ? data.OrderByDescending(keySelector.Compile() as Func<T, double>).ToList() : data.OrderBy(keySelector.Compile() as Func<T, double>).ToList();
            }
            else if (propertyType == typeof(float))
            {
                data = sortDirection.Equals(Descending) ? data.OrderByDescending(keySelector.Compile() as Func<T, float>).ToList() : data.OrderBy(keySelector.Compile() as Func<T, float>).ToList();
            }
            else if (propertyType == typeof(DateTime))
            {
                data = sortDirection.Equals(Descending) ? data.OrderByDescending(keySelector.Compile() as Func<T, DateTime>).ToList() : data.OrderBy(keySelector.Compile() as Func<T, DateTime>).ToList();
            }
            else
            {
                throw new Exception("This type is not support to sort currently.");
            }
            return data;
        }

        #endregion sort

        #region ListModel Transformation

        public static List<TModel> CovertFromEntityCollectionToModelCollection<T, TModel>(List<T> collection) where T:MetaShare.Common.Core.Entities.Common,new () where TModel: ModelBase<T>,new ()
        {
            List < TModel > list=new List<TModel>();
            if (collection != null && collection.Count > 0)
            {
                foreach (T entity in collection)
                {
                    TModel model = new TModel();
                    model.PopulateFrom(entity);
                    list.Add(model);
                }
            }
            
            return list;
        }

        #endregion
        //"MMM dd HH:mm";
        //"MMM dd;

        public static string GetDateTimeValue(DateTime source, string dateTimeFormatPatter)
        {
            if (source == DateTime.MinValue) return string.Empty;

            return source.ToString(dateTimeFormatPatter);
        }


        internal static List<T> GetListFromCommonTypeCollection<T>(Collection<CommonType> commonTypes) where T : CommonType
        {
            List<T> rigSizeTypes = new List<T>();
            if (commonTypes != null)
                foreach (var commonType in commonTypes)
                {
                    var entity = (T) commonType;
                    rigSizeTypes.Add(entity);
                }

            return rigSizeTypes;
        }

        public static Collection<int> GetSearchCollections(Collection<int> resuh)
        {
            Collection<int> collection = new Collection<int>();

            foreach (int id in resuh.Distinct().ToList())
            {
                if (!id.Equals(0))
                {
                    collection.Add(id);
                }
            }

            return collection;
        }

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

        public static string[] SplitString(string str)
        {
           
            if (!string.IsNullOrEmpty(str))
            {
               return  str.Split(new char[] { ',' });
            }
            return null;
        }

        public static string GetClientRepresentative(RigJob rigJob)
        {
            string clientRepresentative = "N/A";
            if (rigJob.ClientConsultant1 != null && !string.IsNullOrEmpty(rigJob.ClientConsultant1.Name))
            {
                clientRepresentative = rigJob.ClientConsultant1.Name;
                if (rigJob.ClientConsultant2 != null && !string.IsNullOrEmpty(rigJob.ClientConsultant2.Name))
                {
                    clientRepresentative += "/" + rigJob.ClientConsultant2.Name;
                }
            }

            return clientRepresentative;
        }

    }

    public static class EnumExtensions
    {
	    public static string GetDescription(this Enum value)
	    {
		    FieldInfo field = value.GetType().GetField(value.ToString());
		    if (field == null)
			    return value.ToString();

		    DescriptionAttribute attribute = field.GetCustomAttribute<DescriptionAttribute>();
		    return attribute != null ? attribute.Description : value.ToString();
	    }
    }

}