using eServiceOnline.Models.Commons;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch;
using Sesi.SanjelData.Entities.Common.BusinessEntities.HumanResources;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Organization;

namespace eServiceOnline.Models.RigBoard
{
    public class ConsultantViewModel : ModelBase<ClientConsultant>
    {
        public int CallSheetId { get; set; }
        public int CallSheetNumber { get; set; }
        public int ConsultantId { get; set; }
        public string Name { get; set; }
        public string Cell { get; set; }
        public string Phone2 { get; set; }
        public string Email { get; set; }
        public int ClientId { get; set; }
        public int WorkShiftTypeId { get; set; }
        public string WorkShiftTypeName { get; set; }
        public int Count { get; set; }
        public bool IsAddDisabled { get; set; }
        public bool IsRemoveDisabled { get; set; }
        public string DisplayContent { get; set; }
        public bool IsFirst { get; set; }

        public string PhoneNumber
        {
            get
            {
                var phoneNumber = string.Empty;

                if (!string.IsNullOrEmpty(this.Cell))
                {
                    phoneNumber = phoneNumber + this.Cell;
                    if (!string.IsNullOrEmpty(this.Phone2))
                    {
                        phoneNumber = phoneNumber + "/" + this.Phone2;
                    }
                }
                else
                {
                    phoneNumber = phoneNumber + this.Phone2;
                }

                return phoneNumber;
            }
        }

        public override void PopulateTo(ClientConsultant clientConsultant)
        {
            if (clientConsultant != null)
            {
                clientConsultant.Id = this.ConsultantId;
                clientConsultant.Name = this.Name;
                clientConsultant.Cell = this.Cell;
                clientConsultant.Phone2 = this.Phone2;
                clientConsultant.Email = this.Email;

                if (clientConsultant.Client != null)
                {
                    clientConsultant.Client.Id = this.ClientId;
                }
                else
                {
                    clientConsultant.Client = new ClientCompany() {Id = this.ClientId};
                }

                if (clientConsultant.WorkShift != null)
                {
                    clientConsultant.WorkShift.Id = this.WorkShiftTypeId;
                    clientConsultant.WorkShift.Name = this.WorkShiftTypeName;
                }
                else
                {
                    clientConsultant.WorkShift = new ShiftType() {Id = this.WorkShiftTypeId, Name = this.WorkShiftTypeName};
                }
            }
        }

        public override void PopulateFrom(ClientConsultant clientConsultant)
        {
            if (clientConsultant != null)
            {
                this.ConsultantId = clientConsultant.Id;
                this.Name = clientConsultant.Name;
                this.Cell = clientConsultant.Cell;
                this.Phone2 = clientConsultant.Phone2;
                this.Email = clientConsultant.Email;

                if (clientConsultant.Client != null)
                {
                    this.ClientId = clientConsultant.Client.Id;
                }

                if (clientConsultant.WorkShift != null)
                {
                    this.WorkShiftTypeId = clientConsultant.WorkShift.Id;
                    this.WorkShiftTypeName = clientConsultant.WorkShift.Name;
                }
            }
        }

        public void PopulateFromRigJob(ClientConsultant clientConsultant, RigJob rigJob)
        {
            this.CallSheetNumber = rigJob.CallSheetNumber;
            this.CallSheetId = rigJob.CallSheetId;
            if (clientConsultant != null)
            {
                this.ConsultantId = clientConsultant.Id;
                this.DisplayContent = clientConsultant.Name + " " + clientConsultant.Cell;
                this.IsRemoveDisabled = this.Count.Equals(1);
                this.IsAddDisabled = this.Count >= 2;
                if (clientConsultant.WorkShift != null)
                {
                    this.WorkShiftTypeId = clientConsultant.WorkShift.Id;
                    this.WorkShiftTypeName = clientConsultant.WorkShift.Name;
                }

                this.Email = clientConsultant.Email;
            }
            else
            {
                this.IsRemoveDisabled =true;
                this.IsAddDisabled = false;
            }
        }
    }
}