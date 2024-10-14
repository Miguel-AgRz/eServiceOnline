using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MetaShare.Common.Core.Entities;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule;
using Object = System.Object;

namespace eServiceOnline.Data
{
    public class ProductHaulLoadComparer: IEqualityComparer<ProductHaulLoad>
    {
        public bool Equals(ProductHaulLoad x, ProductHaulLoad y)
        {
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null.
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            //Check whether the Id properties are equal.
            return x.Id == y.Id;
        }

        public int GetHashCode(ProductHaulLoad obj)
        {
            //Check whether the object is null

            //Get hash code for the Name field if it is not null.
            int hashProductName = obj.Name == null ? 0 : obj.Name.GetHashCode();

            //Get hash code for the Code field.
            int hashProductCode = obj.Description==null?0:obj.Description.GetHashCode();

            //Calculate the hash code for the product.
            return hashProductName ^ hashProductCode;
        }
    }
    public class ShippingLoadSheetComparer: IEqualityComparer<ShippingLoadSheet>
    {
        public bool Equals(ShippingLoadSheet x, ShippingLoadSheet y)
        {
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null.
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            //Check whether the Id properties are equal.
            return x.Id == y.Id;
        }

        public int GetHashCode(ShippingLoadSheet obj)
        {
            //Check whether the object is null

            //Get hash code for the Name field if it is not null.
            int hashProductName = obj.Name == null ? 0 : obj.Name.GetHashCode();

            //Get hash code for the Code field.
            int hashProductCode = obj.Description==null?0:obj.Description.GetHashCode();

            //Calculate the hash code for the product.
            return hashProductName ^ hashProductCode;
        }
    }
    public class CommonComparer: IEqualityComparer<MetaShare.Common.Core.Entities.Common>
    {
        public bool Equals(MetaShare.Common.Core.Entities.Common x, MetaShare.Common.Core.Entities.Common y)
        {
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null.
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            //Check whether the Id properties are equal.
            return x.Id == y.Id;
        }

        public int GetHashCode(MetaShare.Common.Core.Entities.Common obj)
        {
            //Check whether the object is null

            //Get hash code for the Name field if it is not null.
            int hashProductName = obj.Name == null ? 0 : obj.Name.GetHashCode();

            //Get hash code for the Code field.
            int hashProductCode = obj.Description==null?0:obj.Description.GetHashCode();

            //Calculate the hash code for the product.
            return hashProductName ^ hashProductCode;
        }
    }

}
