using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule;

namespace eServiceOnline.BusinessProcess.Interface
{
    public interface IProductHaulProcess
    {
        int DeleteProductHaulLoad(ProductHaulLoad productHaulLoad);
        List<ProductHaulLoad> GetProductHaulLoadCollectionByPaginated(int callSheetNumber, int servicePointId, int pageNumber, int pageSize, out int totalRecordsCount);
    }
}