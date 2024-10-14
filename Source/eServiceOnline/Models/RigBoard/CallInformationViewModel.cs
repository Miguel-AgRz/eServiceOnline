using System;
using System.Collections.Generic;
using Sesi.SanjelData.Entities.Common.BusinessEntities.HumanResources;

namespace eServiceOnline.Models.RigBoard
{
    public class CallInformationViewModel
    {
        public int CallSheetId { get; set; }
        public int CallSheetNumber { get; set; }
        public int SelectedCalloutConsultantId { get; set; }
        public string CalloutConsultantName { get; set; }
        public DateTime RequestedDateTime { get; set; }
        public string Operation { get; set; }
        public List<Employee> Employees { get; set; }
        public int CalloutDispatcherId { get; set; }
        public DateTime CallCrewDateTime { get; set; }
        public DateTime CallDateTime { get; set; }
        public bool IsReleaseCrew { get; set; }
    }
}