using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Products;

namespace eServiceOnline.BusinessProcess.Interface
{
    public interface IBlendChemicalProcess
    {
        List<BlendChemical> GetBlendChemicalCollectionByPaginated(int i, int pageNumber, int pageSize, out int count);
    }
}
