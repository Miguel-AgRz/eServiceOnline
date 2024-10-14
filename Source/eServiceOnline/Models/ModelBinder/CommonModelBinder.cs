using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace eServiceOnline.Models.ModelBinder
{
    public class CommonModelBinder<T> : IModelBinder where T:class,new()
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var model = new T();

            PropertyInfo[] propertyInfos = model.GetType().GetProperties(BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public);
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                if (bindingContext.ValueProvider.GetValue(propertyInfo.Name).FirstOrDefault() != null)
                {
                    var propertyValue = bindingContext.ValueProvider.GetValue(propertyInfo.Name).FirstOrDefault();
                    TypeConverter typeConverter = TypeDescriptor.GetConverter(propertyInfo.PropertyType);
                    propertyInfo.SetValue(model, ConvertToValueType(propertyValue, propertyInfo.PropertyType));
                }
            }

            bindingContext.Result = ModelBindingResult.Success(model);
            return Task.CompletedTask;
        }

        public object ConvertToValueType(string value, Type targetType)
        {
            object result = null;

            bool isEmpty = string.IsNullOrWhiteSpace(value);

            if (targetType == typeof(string))
            {
                result = value;
            }
            else if (targetType == typeof(Int32) || targetType == typeof(int))
            {
                if (isEmpty)
                {
                    result = 0;
                }
                else
                {
                    int num = 0;
                    Int32.TryParse(value, out num);
                    result = num;
                }
            }
            else if (targetType == typeof(double))
            {
                if (isEmpty)
                {
                    result = 0;
                }
                else
                {
                    double num = 0;
                    double.TryParse(value, out num);
                    result = num;
                }
            }
            else if (targetType == typeof(Guid))
            {
                if (isEmpty)
                {
                    result = Guid.Empty;
                }
                else
                {
                    result = new Guid(value);
                }
            }
            else if (targetType.IsEnum)
            {
                result = Enum.Parse(targetType, value);
            }
            else if (targetType == typeof(byte[]))
            {
                result = Convert.FromBase64String(value);
            }
            else if (targetType == typeof(DateTime))
            {
                if (isEmpty)
                {
                    result = DateTime.MinValue;
                }
                else
                {
                    result = Convert.ToDateTime(value);
                }
            }
            else if (targetType == typeof(bool))
            {
                if (isEmpty)
                {
                    result = false;
                }
                else
                {
                    result = Convert.ToBoolean(value);
                }
            }

            return result;
        }
    }
}
