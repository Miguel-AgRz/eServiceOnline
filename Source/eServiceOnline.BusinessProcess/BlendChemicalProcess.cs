using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eServiceOnline.BusinessProcess.Interface;
using eServiceOnline.Gateway;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Products;

namespace eServiceOnline.BusinessProcess
{
    public class BlendChemicalProcess: IBlendChemicalProcess
    {
        public List<BlendChemical> GetBlendChemicalCollectionByPaginated(int i, int pageNumber, int pageSize, out int count)
        {
            List<BlendChemical> blendChemicals = eServiceOnlineGateway.Instance.GetBlendChemicals();
            count = blendChemicals.Count;
            List<BlendChemical> pagedBlendChemicals = blendChemicals.OrderByDescending(p => p.Id).ToList();
            pagedBlendChemicals = pagedBlendChemicals.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();

            return pagedBlendChemicals;
        }
    }
}
