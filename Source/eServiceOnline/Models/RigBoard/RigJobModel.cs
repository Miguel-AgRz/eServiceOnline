using System;
using eServiceOnline.Models.Commons;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Organization;
using CallSheet = Sanjel.BusinessEntities.CallSheets.CallSheet;
using ClientConsultant = Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch.ClientConsultant;
using JobType = Sesi.SanjelData.Entities.Common.BusinessEntities.Operation.JobType;
using Rig = Sesi.SanjelData.Entities.Common.BusinessEntities.WellSite.Rig;

namespace eServiceOnline.Models.RigBoard
{
    public class RigJobModel : ModelBase<CallSheet>
    {
        public int Id { get; set; }
        public string Company { get; set; }
        public string WellLocation { get; set; }
        public string DownHoleLocation { get; set; }
        public string RigCompany { get; set; }
        public string RigNumber  { get; set; }
        public string RigName{ get; set; }
        public int RigId { get; set; }
        public string JobType { get; set; }
        public string JobDate { get; set; }
        public string Notes { get; set; }
        public string ConsultantPhone { get; set; }
        public int CallSheetNumber { get; set; }
        public int CallSheetId { get; set; }
        public bool IsProjectRig { get; set; }
        public bool IsServiceRig { get; set; }
        public string ServicePoint { get; set; }
        public string LSD { get; set; }
        public int ServicePointId { get; set; }
        public DateTime Date { get; set; }
        public int ConsultantId { get; set; }
        public string ConsultantName { get; set; }
        public int ClientCompanyId { get; set; }
        public bool IsReleaseCrew { get; set; }

        public override void PopulateFrom(CallSheet callSheet)
        {
            if (callSheet !=null)
            {
                this.Company = callSheet.Header.HeaderDetails.CompanyInformation.ClientCompany;
                this.WellLocation = callSheet.Header.HeaderDetails.WellLocationInformation.WellLocation;
                this.DownHoleLocation = callSheet.Header.HeaderDetails.WellLocationInformation.DownHoleWellLocation;
                this.RigCompany = callSheet.Header.HeaderDetails.WellLocationInformation.RigContractor != null ? callSheet.Header.HeaderDetails.WellLocationInformation.RigContractor.Name : string.Empty;
                this.RigNumber  = callSheet.Header.HeaderDetails.WellLocationInformation.RigNumber ?? string.Empty;
                this.RigName = callSheet.Header.HeaderDetails.WellLocationInformation.Rig?.Name ?? string.Empty;
                this.JobType = callSheet.Header.WellInformation.JobData.JobType.Description;
                this.JobDate = callSheet.Header.HeaderDetails.FirstCall.ExpectedTimeOnLocation.ToShortDateString();
                this.Notes = string.Empty;
                this.ConsultantPhone = callSheet.Header.HeaderDetails.CallInformation.CalloutConsultant + " " +callSheet.Header.HeaderDetails.CallInformation.CalloutConsultantCellNumber;
                this.ServicePoint = callSheet.Header.WellInformation.JobData.ServicePoint.Name;
                this.CallSheetNumber = callSheet.CallSheetNumber;
                this.CallSheetId = callSheet.Id;
                this.ServicePointId = callSheet.Header.WellInformation.JobData.ServicePoint.Id;
            }
        }

        public void PopulateFromJobAlert(RigJob entity)
        {
            this.Id = entity.Id;
            if (entity.Rig != null)
            {
                this.RigId = entity.Rig.Id;
                this.IsProjectRig = entity.IsProjectRig;
                this.IsServiceRig = entity.IsServiceRig;
                this.RigName = entity.Rig.Name;
            }
            this.Date = (DateTime) entity.JobDateTime;
            this.Notes = entity.Notes;
            this.LSD = entity.SurfaceLocation;
            if (entity.ClientCompany != null)
            {
                this.ClientCompanyId = entity.ClientCompany.Id;
                this.RigCompany = entity.ClientCompany.ShortName;
            }
            if (entity.ServicePoint != null)
            {
                this.ServicePointId = entity.ServicePoint.Id;
                this.ServicePoint = entity.ServicePoint.Name;
            }
            if (entity.ClientConsultant1 != null)
            {
                this.ConsultantId = entity.ClientConsultant1.Id;
                this.ConsultantName = entity.ClientConsultant1.Name;
            }
        }

        public void PopulateToJobAlert(RigJob entity)
        {
            entity.Rig = new Rig()
            {
                Id = this.RigId,
                IsProjectRig =  this.IsProjectRig,
                IsServiceRig =  this.IsServiceRig,
                Name = this.RigName
            };
            entity.JobType = new JobType(){Name = "Job Alert"};
            entity.JobDateTime = this.Date;
            entity.Notes = this.Notes;
            entity.SurfaceLocation = this.LSD;
            entity.ClientCompany = new ClientCompany
            {
                ShortName = this.RigCompany,
                Id = this.ClientCompanyId
            };
            entity.ClientConsultant1 = new ClientConsultant()
            {
                Id = this.ConsultantId,
                Name = this.ConsultantName
            };
            entity.ServicePoint =new ServicePoint()
            {
                Id = this.ServicePointId,
                Name = this.ServicePoint
            };
        }
    }
}
