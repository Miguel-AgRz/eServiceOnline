using Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch;

namespace eServiceOnline.Models.RigBoard
{
    public class CompanyInfoModel
    {
        public int CallSheetId { get; set; }
        public string CompanyShortName { get; set; }
        public bool IsCoDCustomer { get; set; }

        public void PopulateFrom(RigJob rigJob)
        {
            if (rigJob != null)
            {
                this.CompanyShortName = string.IsNullOrEmpty(rigJob.ClientCompany.ShortName) ? rigJob.ClientCompany.Name : rigJob.ClientCompany.ShortName;
                this.IsCoDCustomer = rigJob.ClientCompany.IsCODCustomer;
                this.CallSheetId = rigJob.CallSheetId;
            }
        }
    }
}