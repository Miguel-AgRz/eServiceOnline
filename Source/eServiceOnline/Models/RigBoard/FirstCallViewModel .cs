using System;
using eServiceOnline.Models.Commons;
using Sanjel.BusinessEntities.Sections.Header;

namespace eServiceOnline.Models.RigBoard
{
    public class FirstCallViewModel : ModelBase<FirstCall>
    {
        public int CallSheetNumber { get; set; }
        public string AlertByConsultant1 { get; set; }
        public string AlertByConsultantCellNumber1 { get; set; }
        public string AlertByConsultantEmail1 { get; set; }
        public string AlertByConsultant2 { get; set; }
        public string AlertByConsultantCellNumber2 { get; set; }
        public string AlertByConsultantEmail2 { get; set; }
        public string AdditionalContactInfo { get; set; }
        public string ExpectedTimeComments { get; set; }
        public bool IsExpectedTimeOnLocation { get; set; }
        public bool IsWillCallback { get; set; }
        public string WillCallbackComments { get; set; }
        public DateTime ExpectedTimeOnLocation { get; set; }

        public override void PopulateTo(FirstCall consultant)
        {
            if (consultant != null)
            {
                consultant.AlertByConsultant1 = this.AlertByConsultant1;
                consultant.AlertByConsultantCellNumber1 = this.AlertByConsultantCellNumber1;
                consultant.AlertByConsultantEmail1 = this.AlertByConsultantEmail1;
                consultant.AlertByConsultant2 = this.AlertByConsultant2;
                consultant.AlertByConsultantCellNumber2 = this.AlertByConsultantCellNumber2;
                consultant.AlertByConsultantEmail2 = this.AlertByConsultantEmail2;
                consultant.AdditionalContactInfo = this.AdditionalContactInfo;
                consultant.ExpectedTimeComments = this.ExpectedTimeComments;
                consultant.IsExpectedTimeOnLocation = this.IsExpectedTimeOnLocation;
                consultant.IsWillCallback = this.IsWillCallback;
                consultant.WillCallbackComments = this.WillCallbackComments;
                consultant.ExpectedTimeOnLocation = this.ExpectedTimeOnLocation;
            }

        }


        public override void PopulateFrom(FirstCall consultant)
        {
            if (consultant != null)
            {
                this.AlertByConsultant1 = consultant.AlertByConsultant1;
                this.AlertByConsultantCellNumber1 = consultant.AlertByConsultantCellNumber1;
                this.AlertByConsultantEmail1 = consultant.AlertByConsultantEmail1;
                this.AlertByConsultant2 = consultant.AlertByConsultant2;
                this.AlertByConsultantCellNumber2 = consultant.AlertByConsultantCellNumber2;
                this.AlertByConsultantEmail2 = consultant.AlertByConsultantEmail2;
                this.AdditionalContactInfo = consultant.AdditionalContactInfo;
                this.ExpectedTimeComments = consultant.ExpectedTimeComments;
                this.IsExpectedTimeOnLocation = consultant.IsExpectedTimeOnLocation;
                this.IsWillCallback = consultant.IsWillCallback;
                this.WillCallbackComments = consultant.WillCallbackComments;
            }
        }
    }
}