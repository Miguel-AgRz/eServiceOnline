using Sesi.SanjelData.Entities.BusinessEntities.Operation.Crew;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Organization;

namespace eServiceOnline.Models.ThirdPartyCrewBoard
{
    public class ThirdPartyCrewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ContractorCompanyId { get; set; }

        public int ServicePointId { get; set; }

        public string ContractorCompanyName { get; set; }

        public string ServicePointName { get; set; }

        public string ThirdPartyUnitNumber { get; set; }
        public string SupplierContactName { get; set; }
        public string SupplierContactNumber { get; set; }
        public string Notes { get; set; }

        public void PopulateFrom(ThirdPartyBulkerCrew entity)
        {
            this.Id = entity.Id;
            this.Name = entity.Name;
            this.ThirdPartyUnitNumber = entity.ThirdPartyUnitNumber;
            this.SupplierContactName = entity.SupplierContactName;
            this.SupplierContactNumber = entity.SupplierContactNumber;
            this.Notes = entity.Notes;
            if (entity.ContractorCompany != null)
            {
                this.ContractorCompanyId = entity.ContractorCompany.Id;
                this.ContractorCompanyName = entity.ContractorCompany.Name;
            }
            if (entity.ServicePoint != null)
            {
                this.ServicePointId = entity.ServicePoint.Id;
                this.ServicePointName = entity.ServicePoint.Name;
            }

        }

        public void PopulateTo(ThirdPartyBulkerCrew entity)
        {
            entity.Id = this.Id;
            entity.Name = this.Name;
            entity.ThirdPartyUnitNumber = this.ThirdPartyUnitNumber;
            entity.SupplierContactName = this.SupplierContactName;
            entity.SupplierContactNumber = this.SupplierContactNumber;
            entity.Notes = this.Notes;
            entity.ContractorCompany = new ContractorCompany
            {
                Id = this.ContractorCompanyId,
                Name = this.ContractorCompanyName
            };
            entity.ServicePoint = new ServicePoint
            {
                Id = this.ServicePointId,
                Name = this.ServicePointName
            };
        }

    }
}
