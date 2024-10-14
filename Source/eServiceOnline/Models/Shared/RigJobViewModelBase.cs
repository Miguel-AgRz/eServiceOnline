using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eServiceOnline.Controllers;
using eServiceOnline.Gateway;
using eServiceOnline.Models.Commons;
using eServiceOnline.Models.RigBoard;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule;
using Sesi.SanjelData.Entities.BusinessEntities.WellSite;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Organization;
using Sesi.SanjelData.Entities.Common.BusinessEntities.WellSite;

namespace eServiceOnline.Models.Shared
{
    public class RigJobViewModelBase: ModelBase<RigJob>
    {
        public const string RigBoardController = "RigBoard";
        public const string ProductHaulController = "ProductHaul";
        public const string RigJobController = "RigJob";
        public const string BinBoardController = "BinBoard";
        public const string BulkPlantController = "BulkPlant";


        #region Bin

        public const string StyleBinAssignedAndConfirmed = "assignedandconfirmed";
        public const string StyleBinAssignedNotConfirmed = "assignednotconfirmed";
        public const string StyleBinNotAssigned = "notassigned";
        public const string StyleOnLocation = "menuStyle_Onlocation";
        public const string StyleScheduled = "menuStyle_Scheduled";
        public const string StyleBlend = "menuStyle_Blend";
        public const string StyleLoaded = "menuStyle_Loaded";
        public const string MenuAssignABin = "Assign a Bin";
        public const string MenuBinNotAssigned = "Need Bins";
        public const string MenuDoNotNeedBin = "Don’t need bin";
        public const string MenuRemoveABin = "Release Bin";
        public const string DialogAddNewBin = "Assign Bin";
        public const string DialogNeedBins = "needbins";
        public const string MenuScheduleBlend = "Schedule Blend Request";
        public const string MenuRescheduleBlend = "Reschedule Blend Request";
        public const string MenuCancelBlend = "Cancel Blend Request";
        public const string MenuTransferBlend = "Transfer Blend";
        public const string MenuHaulBlend = "Haul Blend";
        public const string MenuAdjustBlendAmount = "Adjust Blend Amount";
        public const string MenuEmptyBin = "Empty Bin";
        public const string MenuOnLocationForBin = "On Location";
        //public const string MenuRescheduleBlendHaulForBin = "Reschedule Product Haul";
        //public const string MenuCancelBlendHaulForBin = "Cancel Product Haul";
        public const string MenuGowithCrewForBin = "Go with Crew";
        public const string ShortTimeFormatForBin = "H:mm";

        public const string MenuScheduleProductHaulForBin = "Schedule Product Haul";
        public const string MenuRescheduleProductHaulForBin = "Reschedule Product Haul";
        public const string MenuCancelProductHaulForBin = "Cancel Product Haul";

        public const string MenuLoadBlendToBin= "Load Blend to Bin";
        public const string MenuUpdateBinNotes = "Update Bin Notes";

        public const string MenuHaulBack = "Back Haul";
        public const string MenuPrintMTS = "Print MTS";

        #endregion


        public BinSectionModel BinSectionModel { get; set; }
        public int RowMergeNumber { get; set; }
        public StyledCell SequenceNumber { get; set; }
        public StyledCell Rig { get; set; }
        public StyledCell Bin { get; set; }
        public int Sequence { get; set; }

        public bool IsBulkPlant { set; get; } = false;


        public override void PopulateFrom(RigJob rigJob)
        {
            if (rigJob == null) throw new Exception("entity must be instance of class RigJob.");

            this.SequenceNumber = this.GetSequenceNumberStyledCell("SequenceNumber");
            this.Rig = this.GetRigStyledCell("Rig", rigJob);
        }

        #region Compute sequence number

        protected StyledCell GetSequenceNumberStyledCell(string propertyName)
        {
            StyledCell styledCell = new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache) {PropertyValue = this.Sequence.ToString()};
            styledCell = this.SetCellMerge(styledCell);

            return styledCell;
        }

        #endregion

        #region Cell Merge

        public StyledCell SetCellMerge(StyledCell styledCell)
        {
            styledCell.IsNeedRowMerge = true;
            styledCell.RowMergeNumber = this.RowMergeNumber;

            return styledCell;
        }

        #endregion

        #region Compute Bin information from entity

        public StyledCell GetBinStyledCell(string propertyName,RigJob rigJob, BinInformation rigBinSections, List<Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul> productHauls, List<ProductHaulLoad> productHaulLoads)
        {
            if (this.BinSectionModel == null)
                return new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache) {IsNeedRowMerge = false };

            StyledCell styledCell = this.GetBinStyledCell(propertyName, this.BinSectionModel, rigJob, rigBinSections, productHauls, productHaulLoads);
            styledCell.IsNeedRowMerge = false;

            return styledCell;
        }

        //Jan 2, 2024 zhangyuan 244_PR_AddBulkers: Add Set the value of Bin column
        public StyledCell GetProductHaulBinStyledCell(string propertyName, RigJob rigJob , BinInformation rigBinSections, Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul productHaul, ShippingLoadSheet shippingLoadSheet, List<ProductHaulLoad> productHaulLoads)
        {
            if (this.BinSectionModel == null)
                return new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache) { IsNeedRowMerge = true };

            StyledCell styledCell = this.GetProductHaulBinStyledCell(propertyName, this.BinSectionModel, rigJob, null, productHaul, shippingLoadSheet, productHaulLoads);
            styledCell.IsNeedRowMerge = true;

            return styledCell;
        }

        private StyledCell GetBinStyledCell(string propertyName, BinSectionModel binSectionModel, RigJob rigJob,
            BinInformation rigBinSections, List<Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul> productHauls, List<ProductHaulLoad> productHaulLoads)
        {
            if (binSectionModel == null || (rigJob != null && rigJob.JobLifeStatus.Equals(JobLifeStatus.Alerted)))
                return new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache);

            string statusName;
            StyledCell styledCell = new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache) {PropertyValue = binSectionModel.Name ?? " "};
            if (binSectionModel.BinSectionStatus == BinSectionStatus.BinIsNotAssigned && string.IsNullOrEmpty(binSectionModel.Name))
            {
                styledCell.PropertyValue = (rigJob != null && (bool)rigJob.IsNeedBins && binSectionModel.IsFirst) ? "Needed" : " ";
            }
                
            styledCell = this.SetBinStyledCellByStatus(binSectionModel, styledCell, out statusName);
            styledCell.Style = styledCell.ComputeStyle(propertyName, statusName, this.GetDownRigSuffix(rigJob));
            styledCell.ContextMenus = styledCell.IsDisplayMenu ? this.SetBinContextMenus(binSectionModel, rigJob, rigBinSections, productHauls, productHaulLoads) : null;

            //Nov 30,2013 Tongtao develop: Change tooltips over bin
            string binNotice = string.Empty;

            if (rigBinSections != null)
            {
                ProductHaulLoad productHaulLoad = null;

                string blendDescription = rigBinSections.BlendChemical?.Description?.Replace(" + Additives", "");
                //Dec 1,2013 Tongtao develop: remove Quantity number format
                binNotice = string.IsNullOrEmpty(blendDescription) ? string.Empty : rigBinSections.Quantity + "t";


                productHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(rigBinSections.LastProductHaulLoadId);

                if (productHaulLoad != null)
                {
                    binNotice += string.IsNullOrEmpty(binNotice) ? "" : "-" + (productHaulLoad.CallSheetNumber != 0
                        ? "CS:" + productHaulLoad.CallSheetNumber.ToString()
                        : productHaulLoad.ProgramId + "." + productHaulLoad.ProgramVersion.ToString().PadLeft(2, '0'))
                        + "/" + (productHaulLoad.JobType?.Name ?? "")
                        + "/" + (productHaulLoad.BlendCategory?.Name ?? "");
                }

                binNotice += !string.IsNullOrEmpty(blendDescription) ? (string.IsNullOrEmpty(binNotice) ? blendDescription : "-" + blendDescription) : "";
            }

            styledCell.Notice = string.IsNullOrEmpty(binNotice)?(rigBinSections != null ? (rigBinSections.Name + " - " + " #" + rigBinSections.Bin.Name + (rigBinSections.Bin.PodCount > 1 ? (" Pod " + rigBinSections.PodIndex) : "")) : "") :binNotice;

            //styledCell.Notice = rigBinSections != null ? (rigBinSections.BlendChemical?.Description?.Replace(" + Additives", "")) : "";
            return styledCell;
        }

        //Jan 2, 2024 zhangyuan 244_PR_AddBulkers: Add Set the value of Bin column
        private StyledCell GetProductHaulBinStyledCell(string propertyName, BinSectionModel binSectionModel, RigJob rigJob,
           BinInformation rigBinSections, Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul productHaul, ShippingLoadSheet shippingLoadSheet, List<ProductHaulLoad> productHaulLoads)
        {
            if (binSectionModel == null || (rigJob != null && rigJob.JobLifeStatus.Equals(JobLifeStatus.Alerted)))
                return new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache);

            string statusName;
            StyledCell styledCell = new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache) { PropertyValue = binSectionModel.Name ?? " " };
            if (binSectionModel.BinSectionStatus == BinSectionStatus.BinIsNotAssigned && string.IsNullOrEmpty(binSectionModel.Name))
            {
                styledCell.PropertyValue = (rigJob != null ) ? "Needed" : " ";
            }

            styledCell = this.SetBinStyledCellByStatus(binSectionModel, styledCell, out statusName);
            styledCell.Style = styledCell.ComputeStyle(propertyName, statusName, this.GetDownRigSuffix(rigJob));
            styledCell.ContextMenus = this.SetProductHaulBinContextMenus(productHaul);

            //productHaulNotice
            string menu = productHaul.IsGoWithCrew ? MenuGowithCrewForBin : Utility.GetDateTimeValue(productHaul.ExpectedOnLocationTime, ShortTimeFormatForBin);
            menu = productHaul.Crew.Description + "-" + menu;
            styledCell.Notice = menu.Replace("'", "%%%");

            return styledCell;
        }
        private StyledCell SetBinStyledCellByStatus(BinSectionModel item, StyledCell styledCell, out string statusName)
        {
            statusName = string.Empty;
            switch (item.BinSectionStatus)
            {
                case BinSectionStatus.BinIsAssignedAndConfirmedOnLocation:
                    statusName = StyleBinAssignedAndConfirmed;
                    break;
                case BinSectionStatus.BinIsAssignedButNotConfirmedOnLocation:
                    statusName = StyleBinAssignedNotConfirmed;
                    break;
                case BinSectionStatus.BinIsNotAssigned:
                    statusName = StyleBinNotAssigned;
                    break;
            }

            return styledCell;
        }

        protected virtual List<ContextMenu> SetBinContextMenus(BinSectionModel binSectionModel, RigJob rigJob,
            BinInformation rigBinSections, List<Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul> productHauls, List<ProductHaulLoad> productHaulLoads)
        {
            return null;
        }
        //Jan 2, 2024 zhangyuan 244_PR_AddBulkers: Add Set the value of Bin column
        protected virtual List<ContextMenu> SetProductHaulBinContextMenus(Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul productHaul)
        {
            return null;
        }
        protected virtual List<ContextMenu> SetReScheduleBlendMenuForBin(List<ProductHaulLoad> productHaulLoadList,
            BinSectionModel binSectionModel, List<Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul> productHaulList = null)
        {
            return null;
        }
        protected virtual List<ContextMenu> SetCancelBlendMenuForBin(List<ProductHaulLoad> phlList,BinSectionModel binSectionModel, List<Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul> productHaulList = null, RigJob rigJob=null)
        {
            return null;
        }
        protected virtual List<ContextMenu> SetHaulBlendMenuForBin(List<ProductHaulLoad> phlList, BinSectionModel binSectionModel, List<Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul> productHaulList = null, RigJob rigJob = null)
        {
            return null;
        }
        //protected virtual List<ContextMenu> SetRescheduleProductHualMenuForBin(List<ProductHaulLoad> phlList, BinSectionModel binSectionModel, List<Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul> productHaulList = null, RigJob rigJob = null)
        //{
        //    List<ContextMenu> list = new List<ContextMenu>();

        //    if (productHaulList != null && productHaulList.Count > 0)
        //    {
        //        foreach (var item in productHaulList)
        //        {
        //               string  menu = item.IsGoWithCrew
        //                    ? MenuGowithCrewForBin
        //                    : Utility.GetDateTimeValue(item.ExpectedOnLocationTime, ShortTimeFormatForBin);
                   
        //            menu = item.Crew.Description + "-" + menu;
                    
        //            ContextMenu contextMenu = new ContextMenu()
        //            {
        //                MenuName = menu.Replace("'", "%%%"),
        //                ProcessingMode = ProcessingMode.PopsUpWindow,
        //                Parms = new List<string>() { item.Id.ToString() },
        //                ControllerName = ProductHaulController,
        //                ActionName = "RescheduleProductHaul"
        //            };
        //            list.Add(contextMenu);
        //        }
        //    }

        //    return list;
        //}

        protected virtual List<ContextMenu> SetRescheduleProductHaulForRigBoardBin(List<ProductHaulLoad> phlList, BinSectionModel binSectionModel, List<Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul> productHaulList = null, RigJob rigJob = null)
        {

            return null;
        }

        protected virtual List<ContextMenu> SetCancelProductHaulForBin(List<ProductHaulLoad> phlList, BinSectionModel binSectionModel, List<Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul> productHaulList = null, RigJob rigJob = null)
        {
            return null;
        }

        protected List<ContextMenu> SetCancelProductHaulLoadForRigBoardBin( Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul productHaul, RigJob rigJob)
        {
            List<ContextMenu> list = new List<ContextMenu>();

            if (productHaul.ShippingLoadSheets != null && productHaul.ShippingLoadSheets.Count > 0)
            {
                foreach (ShippingLoadSheet shippingLoadSheet in productHaul.ShippingLoadSheets)
                {
                    var item = eServiceOnlineGateway.Instance.GetProductHaulLoadById(shippingLoadSheet.ProductHaulLoad
                        .Id);
                    if (item != null)
                    {

                        //                    double enteredBlendWeight = item.IsTotalBlendTonnage ? item.TotalBlendWeight : item.BaseBlendWeight;
                        //string menu = item.BlendChemical?.Name + " - " + enteredBlendWeight / 1000 + "t";
                        string menu = item.BlendChemical?.Name + " - " + shippingLoadSheet.LoadAmount / 1000 + "t";
                        string tips = item.ProductHaulLoadLifeStatus.ToString();
                        if (item.BlendShippingStatus != BlendShippingStatus.Empty)
                        {
                            tips += "|" + item.BlendShippingStatus.ToString();
                        }

                        if (item.BlendTestingStatus != BlendTestingStatus.None)
                        {
                            tips += "|" + item.BlendTestingStatus.ToString();
                        }

                        ContextMenu contextMenu = new ContextMenu()
                        {
                            MenuName = menu.Replace("'", "%%%"),
                            ProcessingMode = ProcessingMode.PopsUpWindow,
                            Parms = new List<string>()
                            {
                                item.Id.ToString(), rigJob?.Rig.Id.ToString(), rigJob?.CallSheetId.ToString(),
                                rigJob?.Id.ToString()
                            },
                            ControllerName = ProductHaulController,
                            ActionName = "CancelProductHaulLoad",
                            MenuTips = tips,
                            IsDisabled = true
                        };
                        list.Add(contextMenu);
                    }
                }
            }

            return list;
        }
        //protected virtual List<ContextMenu> SetCancelProductHualMenuForBin(List<ProductHaulLoad> phlList, BinSectionModel binSectionModel, List<Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul> productHaulList = null, RigJob rigJob = null)
        //{
        //    List<ContextMenu> list = new List<ContextMenu>();

        //    if (productHaulList != null && productHaulList.Count > 0)
        //    {
        //        foreach (var item in productHaulList)
        //        {
        //            string menu = item.IsGoWithCrew
        //                    ? MenuGowithCrewForBin
        //                    : Utility.GetDateTimeValue(item.ExpectedOnLocationTime, ShortTimeFormatForBin);

        //            menu = item.Crew.Description + "-" + menu;
        //            ContextMenu contextMenu = new ContextMenu()
        //            {
        //                MenuName = menu.Replace("'", "%%%"),
        //                ProcessingMode = ProcessingMode.PopsUpWindow,
        //                Parms = new List<string>() { item.Id.ToString() },
        //                ControllerName = ProductHaulController,
        //                ActionName = "CancelProductHaul"
        //            };
        //            list.Add(contextMenu);
        //        }
        //    }

        //    return list;
        //}
        protected virtual List<ContextMenu> SetOnLocationMenuForBin(List<ProductHaulLoad> phlList, BinSectionModel binSectionModel,
            List<Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul>
                productHaulList = null)
        {
            /*List<ContextMenu> list = new List<ContextMenu>();
            if (productHaulList != null && productHaulList.Count > 0)
            {
                foreach (var item in productHaulList)
                {
                    string menu = item.IsGoWithCrew
                         ? MenuGowithCrewForBin
                         : Utility.GetDateTimeValue(item.ExpectedOnLocationTime, ShortTimeFormatForBin);
                    menu = item.Crew.Description + "-" + menu;
                    ContextMenu contextMenu = new ContextMenu()
                    {
                        MenuName = menu.Replace("'", "%%%"),
                        ProcessingMode = ProcessingMode.PopsUpWindow,
                        Parms = new List<string>() { item.Id.ToString() },
                        ControllerName = ProductHaulController,
                        ActionName = "OnLocationProductHaul",
                        //MenuStyle = item.BlendSectionId == blendSectionId ? GetProductHualLoadMenuStyle(item.Status) : "",
                        MenuTips = item.ProductHaulLifeStatus.ToString(),
                        IsDisabled=item.ProductHaulLifeStatus!=ProductHaulStatus.Scheduled,
                        MenuList = SetOnLocationProductHaulLoadMenuForBin(item, item.Id)
                    };
                    list.Add(contextMenu);
                }
            }
            return list;*/
            return null;
        }
        /*private List<ContextMenu> SetOnLocationProductHaulLoadMenuForBin(Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul productHaul,int productHaulId)
        {
            List<ContextMenu> list = new List<ContextMenu>();

            if (productHaul.ProductHaulLoads != null && productHaul.ProductHaulLoads.Count > 0)
            {
                foreach (ProductHaulLoad item in productHaul.ProductHaulLoads)
                {

                    double enteredBlendWeight = item.IsTotalBlendTonnage ? item.TotalBlendWeight : item.BaseBlendWeight;
                    var shippingLoadSheet=productHaul.ShippingLoadSheets.FirstOrDefault(p => p.ProductHaulLoad.Id == item.Id);
                    //string menu = item.BlendChemical?.Name + " - " + enteredBlendWeight / 1000 + "t";
                    string menu = item.BlendChemical?.Name + " - " + shippingLoadSheet.LoadAmount / 1000 + "t";
                    string tips = item.ProductHaulLoadLifeStatus.ToString();
                    if (item.BlendShippingStatus != BlendShippingStatus.Empty)
                    {
                        tips += "|" + item.BlendShippingStatus.ToString();
                    }
                    if (item.BlendTestingStatus != BlendTestingStatus.None)
                    {
                        tips += "|" + item.BlendTestingStatus.ToString();
                    }
                    ContextMenu contextMenu = new ContextMenu()
                    {
                        MenuName = menu.Replace("'", "%%%"),
                        ProcessingMode = ProcessingMode.PopsUpWindow,
                        Parms = new List<string>() { item.Id.ToString(),productHaulId.ToString() },
                        ControllerName = ProductHaulController,
                        ActionName = "OnLocationProdHaulLoad",
                        MenuTips = tips,
                        IsDisabled =productHaul.ShippingLoadSheets.FirstOrDefault(p=>p.ProductHaulLoad.Id==item.Id).ShippingStatus==ShippingStatus.OnLocation|| item.ProductHaulLoadLifeStatus!=ProductHaulLoadStatus.Scheduled||item.BlendShippingStatus==BlendShippingStatus.OnLocation
                       
                    };
                    list.Add(contextMenu);


                }
            }

            return list;
        }*/
        #endregion

        public virtual void PopulateFromBin(RigJob rigJob, BinInformation rigBinSections, List<Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul> productHauls, List<ProductHaulLoad> productHaulLoads)
        {
            this.Bin = this.GetBinStyledCell("Bin", rigJob, rigBinSections, productHauls, productHaulLoads);
        }
        //Jan 2, 2024 zhangyuan 244_PR_AddBulkers: Add Set the value of Bin column
        public virtual void PopulateFromProductHaulBin(RigJob rigJob, BinInformation rigBinSections, Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul productHaul, ShippingLoadSheet shippingLoadSheet, List<ProductHaulLoad> productHaulLoads)
        {
            this.Bin = this.GetProductHaulBinStyledCell("Bin", rigJob, rigBinSections, productHaul,shippingLoadSheet, productHaulLoads);
        }

        protected virtual StyledCell GetRigStyledCell(string propertyName, RigJob rigJob)
        {
            Rig rig = rigJob.Rig;
            StyledCell styledCell = new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache) {PropertyValue = rig.Name};

            styledCell = this.SetCellMerge(styledCell);

            return styledCell;
        }


    }
}
