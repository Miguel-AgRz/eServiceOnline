using System;
using System.Collections.Generic;
using System.Text;
using eServiceOnline.Controllers;
using eServiceOnline.Models.Commons;
using eServiceOnline.Models.ProductHaul;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Organization;
using JobType = Sesi.SanjelData.Entities.Common.BusinessEntities.Operation.JobType;
using Rig = Sesi.SanjelData.Entities.Common.BusinessEntities.WellSite.Rig;
using RigJob = Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch.RigJob;

namespace eServiceOnline.Models.UpcomingJobs
{
    public class UpcomingJobsViewModel : ModelBase<RigJob>
    {
        public const string LongDateTimeFormat = "MMM d H:mm";
        public const string ShortDateTimeFormat = "MMM d";
        public const string StyleJobJobTypeState = "job-jobtypestate";
        public const string StyleRigIsTopDrive = "topdrive";
        public const string StyleWrapText = "wrap-text";
        public const string LineSeparator = "<br>";

        public UpcomingJobsViewModel()
        {
            this.ProductHaulModels = new List<ProductHaulModel>();
            this.UnitAndWorkerModels = new List<UnitAndWorkerModel>();
        }

        #region Properties
        public StyledCell Client { get; set; }
        public StyledCell WellLocation { get; set; }
        public StyledCell Rig { get; set; }
        public StyledCell Job { get; set; }
        public StyledCell EtaTime { get; set; }
        public StyledCell Crew { get; set; }
        public StyledCell Unit { get; set; }
        public StyledCell Blends { get; set; }
        public StyledCell Notes { get; set; }
        public StyledCell CallSheetNumber { get; set; }
        public StyledCell Status { get; set; }

        public List<ProductHaulModel> ProductHaulModels { get; set; }
        public List<UnitAndWorkerModel> UnitAndWorkerModels { get; set; }
        #endregion Properties

        #region  Methods

        public override void PopulateFrom(RigJob rigJob)
        {
            if (rigJob == null) throw new Exception("entity must be instance of class RigJob.");

            this.Client = this.GetClientStyledCell("Client", rigJob);
            this.WellLocation = this.GetSurfaceLocationStyledCell("WellLocation", rigJob);
            this.Rig = this.GetRigStyledCell("Rig", rigJob);
            this.Job = this.GetJobStyledCell("Job", rigJob);
            this.EtaTime = this.GetEtaTimeStyledCell("EtaTime", rigJob);
            this.Crew = this.GetCrewStyledCell("Crew", rigJob);
            this.Unit = this.GetUnitStyledCell("Unit", rigJob);
            this.Blends = this.GetBlendsStyledCell("Blends", rigJob);
            this.Notes = this.GetNotesStyledCell("Notes", rigJob);
            this.CallSheetNumber = this.GetCallSheetNumberStyledCell("CallSheetNumber", rigJob);
            this.Status = this.GetStatusStyledCell("Status", rigJob);
        }

        #region Compute company information from entity

        private StyledCell GetClientStyledCell(string propertyName, RigJob rigJob)
        {
            ClientCompany clientCompany = rigJob.ClientCompany;
            if (clientCompany == null)
            {
                return new StyledCell(propertyName, null, this.LoggedUser, null);
            }

            StyledCell styledCell = new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue = string.IsNullOrEmpty(clientCompany.ShortName) ? clientCompany.Name : clientCompany.ShortName };

            return styledCell;
        }

        #endregion

        private StyledCell GetSurfaceLocationStyledCell(string propertyName, RigJob rigJob)
        {
            StyledCell styledCell = new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue = string.IsNullOrEmpty(rigJob.SurfaceLocation) ? rigJob.WellLocation : rigJob.SurfaceLocation };

            return styledCell;
        }

        private StyledCell GetRigStyledCell(string propertyName, RigJob rigJob)
        {
            Rig rigJobRig = rigJob.Rig;
            if (rigJobRig == null) return new StyledCell(propertyName, null, this.LoggedUser, null);

            string statusName = string.Empty;
            StyledCell styledCell = new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue = rigJobRig.Name };
            if (rigJobRig.IsTopDrive)
            {
                statusName = StyleRigIsTopDrive;
            }

            return styledCell;
        }

        private StyledCell GetJobStyledCell(string propertyName, RigJob rigJob)
        {
            JobType rigJobJob = rigJob.JobType;
            if (rigJobJob == null)
                return new StyledCell(propertyName, null, this.LoggedUser, null);

            StyledCell styledCell = new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue = rigJobJob.Name };
            styledCell.Style = rigJob.IsHighProfile ? StyleJobJobTypeState : styledCell.ComputeStyle(styledCell.PropertyName, null, null);

            return styledCell;
        }

        private StyledCell GetEtaTimeStyledCell(string propertyName, RigJob rigJob)
        {
            DateTime? rigJobDate = rigJob.JobDateTime;
            if (rigJobDate == null)
                return new StyledCell(propertyName, null, this.LoggedUser, null);
            StyledCell styledCell = new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue = Utility.GetDateTimeValue(rigJobDate.Value, LongDateTimeFormat) };

            return styledCell;
        }

        private StyledCell GetCrewStyledCell(string propertyName, RigJob rigJob)
        {
            return new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue = this.GetVauleByUnitSectionModels(this.UnitAndWorkerModels, propertyName), Style = StyleWrapText };
        }

        private StyledCell GetUnitStyledCell(string propertyName, RigJob rigJob)
        {
            return new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue = this.GetVauleByUnitSectionModels(this.UnitAndWorkerModels, propertyName), Style = StyleWrapText };
        }

        private string GetVauleByUnitSectionModels(List<UnitAndWorkerModel> unitAndWorkerModels, string propertyName)
        {
            StringBuilder propertyValue = new StringBuilder();
            foreach (UnitAndWorkerModel unitAndWorkerModel in unitAndWorkerModels)
            {
                if (unitAndWorkerModel != null)
                {
                    string partialValue;
                    if (propertyName.Equals("Crew"))
                    {
                        if (!string.IsNullOrWhiteSpace(unitAndWorkerModel.DriverOneName))
                        {
                            propertyValue.Append(unitAndWorkerModel.DriverOneName);
                            partialValue = string.IsNullOrWhiteSpace(unitAndWorkerModel.DriverTwoName) ? LineSeparator : (" / " + unitAndWorkerModel.DriverTwoName + LineSeparator);
                            propertyValue.Append(partialValue);
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(unitAndWorkerModel.TruckUnitNumber))
                        {
                            propertyValue.Append(unitAndWorkerModel.TruckUnitNumber);
                            partialValue = string.IsNullOrWhiteSpace(unitAndWorkerModel.TractorUnitNumber) ? LineSeparator : ("/" + unitAndWorkerModel.TractorUnitNumber + LineSeparator);
                            propertyValue.Append(partialValue);
                        }
                    }
                }
            }

            return propertyValue.ToString();
        }

        private StyledCell GetBlendsStyledCell(string propertyName, RigJob rigJob)
        {
            return new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue = this.GetVauleByProductHaulModels(this.ProductHaulModels), Style = StyleWrapText };
        }

        private string GetVauleByProductHaulModels(List<ProductHaulModel> productHaulModels)
        {
            StringBuilder propertyValue = new StringBuilder();
            foreach (ProductHaulModel productHaulModel in productHaulModels)
            {
                if (productHaulModel != null)
                {
                    double blends = productHaulModel.IsTotalBlendTonnage ? productHaulModel.TotalBlendWeight / 1000 : productHaulModel.BaseBlendWeight / 1000;
                    if (blends > 0)
                    {
                        propertyValue.Append(blends + "t " + productHaulModel.Category + " ");
                        propertyValue.Append(productHaulModel.BaseBlend + " ");
                        string isLocation = productHaulModel.ProductHaulLifeStatus==ProductHaulStatus.OnLocation? " on location" : " needs haul";
                        propertyValue.Append(isLocation + LineSeparator);
                    }
                }
            }

            return propertyValue.ToString();
        }

        private StyledCell GetNotesStyledCell(string propertyName, RigJob rigJob)
        {
            StyledCell styledCell = new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue = string.Empty };

            return styledCell;
        }

        private StyledCell GetCallSheetNumberStyledCell(string propertyName, RigJob rigJob)
        {
            StyledCell styledCell = new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue = rigJob.CallSheetNumber.ToString() };

            return styledCell;
        }

        private StyledCell GetStatusStyledCell(string propertyName, RigJob rigJob)
        {
            StyledCell styledCell = new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue = rigJob.JobLifeStatus.ToString() };

            return styledCell;
        }

        #endregion
    }
}