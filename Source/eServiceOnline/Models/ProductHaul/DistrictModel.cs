using ServicePoint = Sesi.SanjelData.Entities.Common.BusinessEntities.Organization.ServicePoint;
namespace eServiceOnline.Models.ProductHaul
{
    public class DistrictModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public void PopulateFrom(ServicePoint servicePoint)  
        {
            if (servicePoint != null)
            {
                this.Id = servicePoint.Id;
                this.Name = servicePoint.Name;
            }
        }       
    }
}
