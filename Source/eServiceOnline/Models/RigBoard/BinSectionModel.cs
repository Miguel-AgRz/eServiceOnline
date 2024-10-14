using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Forms.VisualStyles;
using Sanjel.Common.EService.Sections.Common;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch;
using Sesi.SanjelData.Entities.BusinessEntities.WellSite;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment;

namespace eServiceOnline.Models.RigBoard
{
    public class BinSectionModel
    {
        private int count;
        public int RootId { get; set; }
//        public int ParentId { get; set; }
        public int BinSectionId { get; set; }
        public string BinNumber { get; set; }
        public int BinId { get; set; }
        public int BinTypeId { get; set; }
        public string BinTypeDescription { get; set; }
        public double Quantity { get; set; }
        public string BlendDescription { get; set; }
        public string Name { set; get; }
        public int PodIndex { set; get; }
        public int LastProductHaulLoadId { set; get; }

/*
        public DateTime OnLocation { get; set; }
        public DateTime OffLocation { get; set; }
        public string HaulingTime { get; set; }
        public double? VolumePumped { get; set; }
        public double? InitialVolume { get; set; }
*/
        public int Count
        {
            set => count = value;
        }
        //IsFirst property is for display the label "Needed" if no bin assigned
        public bool IsFirst { get; set; }
        public BinSectionStatus BinSectionStatus { get; set; }
        //        public int RigJobId { get; set; }
        public List<BinNote> BinNotes { get; set; }
        public void PopulateFrom(BinInformation rigBinSection, RigJob rigJob)
        {
            if (rigBinSection != null)
            {
                this.BinSectionId = rigBinSection.Id;
                this.Name = rigBinSection.Name;
                this.PodIndex = rigBinSection.PodIndex;
                this.LastProductHaulLoadId = rigBinSection.LastProductHaulLoadId;
                if (rigBinSection.Bin!=null)
                {
                    this.BinNumber = rigBinSection.Bin.Name;
                    if (rigBinSection.Bin.BinType!=null)
                    {
                        this.BinTypeId = rigBinSection.Bin.BinType.Id;
                        this.BinTypeDescription = rigBinSection.Bin.BinType.Description;
                    }               
                    this.BinId = rigBinSection.Bin.Id;
                 
                }

                if (rigBinSection.BlendChemical != null)
                {
                    this.BlendDescription = rigBinSection.BlendChemical.Description;
                    this.Quantity = rigBinSection.Quantity;
                }

                if (rigJob.IsNeedBins)
                {
                    //04/10/19 AW: This part were over-done in the past based on business analysis. We need to review this once we are implementing Bin Tracking
//                    this.BinSectionStatus = rigBinSection.OnLocation == DateTime.MinValue ? BinSectionStatus.BinIsAssignedButNotConfirmedOnLocation : BinSectionStatus.BinIsAssignedAndConfirmedOnLocation;
                    this.BinSectionStatus = BinSectionStatus.BinIsAssignedAndConfirmedOnLocation;
                }
                else
                {
                    this.BinSectionStatus = BinSectionStatus.BinIsNotNeeded;
                }

            }
            else
            {
                this.BinSectionStatus = rigJob.IsNeedBins ? BinSectionStatus.BinIsNotAssigned : BinSectionStatus.BinIsNotNeeded;
                if (rigJob.JobLifeStatus!=JobLifeStatus.Alerted || rigJob.JobLifeStatus!=JobLifeStatus.Completed)
                {
                    this.RootId = rigJob.CallSheetId;
                }

            }
        }

        //Jan 2, 2024 zhangyuan 244_PR_AddBulkers: Add Assign a value to the Model through producthaul
        public void PopulateProductHaulFrom(Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul producthaul)
        {
            if (producthaul != null)
            {
                this.Name = producthaul.IsThirdParty?producthaul.Crew.Name: producthaul.TractorUnit?.Name;
                if (producthaul.IsThirdParty)
                {
	                this.Name = producthaul.Crew.Name;
	            }
                else
                {
	                this.Name = producthaul.TractorUnit == null ? producthaul.Driver?.Name : producthaul.TractorUnit.Name;
                }
            }
        }

    }
    public enum BinSectionStatus
    {
        BinIsAssignedAndConfirmedOnLocation = 1,
        BinIsAssignedButNotConfirmedOnLocation = 2,
        BinIsNotAssigned = 3,
        BinIsNotNeeded = 4,
    }
}