using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using eServiceOnline.Controllers;
using eServiceOnline.Models.Commons;
using eServiceOnline.Models.CrewBoard;
using eServiceOnline.Models.RigBoard;
using eServiceOnline.Models.Shared;
using eServiceOnline.Models.ThirdPartyCrewBoard;
using Microsoft.Extensions.Caching.Memory;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule;
using Sesi.SanjelData.Entities.BusinessEntities.WellSite;
using Sesi.SanjelData.Entities.Common.BusinessEntities.WellSite;

namespace eServiceOnline.Models.BulkPlantBoard
{
    public class BulkPlantJobViewModel: RigJobViewModelBase
    {
        #region Constructor

        public BulkPlantJobViewModel(IMemoryCache memoryCache, string loggedUser)
        {
            this._memoryCache = memoryCache;
            this.LoggedUser = loggedUser;
            this.BinSectionModel = new BinSectionModel();
        }

        #endregion Constructor
        public StyledCell Capacity { get; set; }
        public StyledCell BlendInBin { get; set; }
        public StyledCell Quantity { get; set; }
        public StyledCell TestingStatus { get; set; }
        public StyledCell BlendScheduled { get; set; }
        public StyledCell ScheduledQuantity { get; set; }
        public StyledCell BlendingStatus { get; set; }

        public StyledCell RigName { get; set; }

        public StyledCell CSNum { get; set; }

        public StyledCell ClientName { get; set; }


        //Dec 11, 2023 Tongtao 143_PR_AddBlendRequestToBulkPlantBoard: Add BlendRequest id and  Bin Last BlendRequest id On the BulkPlantBoard
        public StyledCell BinReqNum { get; set; }

        public StyledCell BlendReqNum { get; set; }

        public void PopulateFromBinInformationAndProductHaulLoads( RigJob rigJob, BinInformation rigBinSection,ProductHaulLoad productload, ProductHaulLoad blendOrigin)
        {
	        this.BinReqNum = GetBinReqNumStyledCell("BinReqNum", rigJob, rigBinSection);
            this.BlendInBin = GetBlendInBinStyledCell("BlendInBin", rigJob, rigBinSection, blendOrigin);
            this.Quantity = GetQuantityStyledCell("Quantity", rigJob, rigBinSection);
            //this.Capacity = GetCapacityStyledCell("Capacity", rigJob, rigBinSection);
            this.TestingStatus = GetTestingStatusStyledCell("TestingStatus", rigJob, rigBinSection);

            this.RigName = GetRigNameStyledCell("RigName", rigJob, blendOrigin);
            this.CSNum = GetCSNumStyledCell("CS#", rigJob, blendOrigin);
            this.ClientName = GetClientNameStyledCell("ClientName", rigJob, blendOrigin);

            this.BlendScheduled = GetBlendScheduledStyledCell("BlandScheduled", rigJob, productload);
            this.BlendReqNum = GetBlendReqNumStyledCell("BlendReqNum", rigJob, productload);
            this.ScheduledQuantity=GetScheduledQuantityStyledCell("BlandScheduled", rigJob, productload);
            this.BlendingStatus = GetBlendingStatusStyledCell("BlendingStatus", rigJob, productload);




        }

        //Jan 2, 2024 zhangyuan 244_PR_AddBulkers: Add Fill in ProductHaulLoad data From Scheduled ProductHaul
        public void PopulateFromProductHaulLoads(RigJob rigJob, ProductHaulLoad productload, ProductHaulLoad blendOrigin, Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul productHaul, ShippingLoadSheet shippingLoadSheet)
        {
	        this.BinReqNum = GetProductHaulBinReqNumStyledCell("BinReqNum", rigJob, shippingLoadSheet, blendOrigin);
            this.BlendInBin = GetProductHaulBlendInBinStyledCell("BlendInBin", rigJob, productload, shippingLoadSheet, blendOrigin);
            this.Quantity = GetProductHaulQuantityStyledCell("Quantity", rigJob, shippingLoadSheet, blendOrigin);
            //this.Capacity = GetCapacityStyledCell("Capacity", rigJob, rigBinSection);
            this.TestingStatus = GetProductHaulTestingStatusStyledCell("TestingStatus");

            this.RigName = GetRigNameStyledCell("RigName", rigJob, blendOrigin);
            this.CSNum = GetCSNumStyledCell("CS#", rigJob, blendOrigin);
            this.ClientName = GetClientNameStyledCell("ClientName", rigJob, blendOrigin);

            this.BlendScheduled = GetBlendScheduledFromProductHaulStyledCell("BlandScheduled", rigJob, productload, productHaul, shippingLoadSheet);
            this.ScheduledQuantity = GetScheduledQuantityStyledCell("BlandScheduled", rigJob, productload);
            this.BlendingStatus = GetBlendingStatusStyledCell("BlendingStatus", rigJob, productload);

            this.BlendReqNum = GetBlendReqNumStyledCell("BlendReqNum", rigJob, productload);
        }


        #region Compute BlendInBin information from entity

        private StyledCell GetBlendInBinStyledCell(string propertyName,RigJob rigJob, BinInformation rigBinSection, ProductHaulLoad blendOrigin)
        {
            if (rigBinSection == null)
                return this.SetCellMerge(new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache));

            StyledCell styledCell = new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache) { PropertyValue = rigBinSection.BlendChemical?.Name?.Replace(" + Additives", "")?.Trim() ?? "", Notice =  rigBinSection.BlendChemical?.Description };
            styledCell.Style = styledCell.ComputeStyle(propertyName, null, this.GetDownRigSuffix(rigJob));
            styledCell = this.SetCellMerge(styledCell);
            //styledCell.IsNeedRowMerge = false;
            return styledCell;
        }




        #endregion

        #region Compute Quantity information from entity
        private StyledCell GetQuantityStyledCell(string propertyName, RigJob rigJob, BinInformation rigBinSection)
        {
            if (rigBinSection == null)
                return this.SetCellMerge(new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache));

            //Nov 8, 2023 Tongtao P45_Q4_161: format data
            //Change the formatting precision from two decimal places to three decimal places

            
            StyledCell styledCell = new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache) { PropertyValue = rigBinSection.Quantity.ToString("##.###")};

            styledCell.Style = styledCell.ComputeStyle(propertyName, null, this.GetDownRigSuffix(rigJob));
            styledCell = this.SetCellMerge(styledCell);
            //styledCell.IsNeedRowMerge = false;
            return styledCell;
        }
        #endregion

        #region Compute Capacity information from entity
        private StyledCell GetCapacityStyledCell(string propertyName, RigJob rigJob, BinInformation rigBinSection)
        {
            if (rigBinSection == null)
                return this.SetCellMerge(new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache));

            StyledCell styledCell = new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache) { PropertyValue = rigBinSection.Capacity.ToString() };
//            styledCell.Style = styledCell.ComputeStyle(propertyName, null, this.GetDownRigSuffix(rigJob));
            styledCell = this.SetCellMerge(styledCell);
            //styledCell.IsNeedRowMerge = false;
            return styledCell;
        }
        #endregion

        #region Compute ProdectHaul Blend in Bin Column from entity
        private StyledCell GetProductHaulBlendInBinStyledCell(string propertyName, RigJob rigJob, ProductHaulLoad productload, ShippingLoadSheet shippingLoadSheet, ProductHaulLoad blendOrigin)
        {
            if (shippingLoadSheet.ShippingStatus== ShippingStatus.Scheduled)
                return this.SetCellMerge(new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache));

            StyledCell styledCell = new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache) { PropertyValue = blendOrigin == null?String.Empty : blendOrigin.BlendChemical?.Name?.Replace(" + Additives", "")?.Trim() ?? "", Notice = blendOrigin == null ? String.Empty : blendOrigin.BlendChemical?.Description };
            styledCell.Style = styledCell.ComputeStyle(propertyName, null, this.GetDownRigSuffix(rigJob));
            styledCell = this.SetCellMerge(styledCell);
            //styledCell.IsNeedRowMerge = false;
            return styledCell;
        }
        #endregion

        #region Compute ProdectHaul Quantity Column from entity
        private StyledCell GetProductHaulQuantityStyledCell(string propertyName, RigJob rigJob, ShippingLoadSheet shippingLoadSheet, ProductHaulLoad productload)
        {
            if (shippingLoadSheet.ShippingStatus == ShippingStatus.Scheduled)
                return this.SetCellMerge(new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache));

            //Nov 8, 2023 Tongtao P45_Q4_161: format data
            //Change the formatting precision from two decimal places to three decimal places


            StyledCell styledCell = new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache)
            {
                PropertyValue = productload == null?String.Empty : (productload.TotalBlendWeight / 1000).ToString("##.###")
            };

            styledCell.Style = styledCell.ComputeStyle(propertyName, null, this.GetDownRigSuffix(rigJob));
            styledCell = this.SetCellMerge(styledCell);
            //styledCell.IsNeedRowMerge = false;
            return styledCell;
        }
        #endregion

        #region Compute ProdectHaul Req# Column from entity
        private StyledCell GetProductHaulBinReqNumStyledCell(string propertyName, RigJob rigJob, ShippingLoadSheet shippingLoadSheet, ProductHaulLoad productload)
        {
            if (shippingLoadSheet.ShippingStatus == ShippingStatus.Scheduled)
                return this.SetCellMerge(new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache));

            StyledCell styledCell = new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache) { PropertyValue = productload==null?string.Empty:productload.Id.ToString() };
            styledCell.Style = styledCell.ComputeStyle(propertyName, null, this.GetDownRigSuffix(rigJob));
            styledCell = this.SetCellMerge(styledCell);
            return styledCell;
        }
        #endregion

        #region Compute ProdectHaul Testing Column from entity
        private StyledCell GetProductHaulTestingStatusStyledCell(string propertyName)
        {

            StyledCell styledCell = new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache) { PropertyValue = BlendTestingStatus.None.ToString() };
            //            styledCell.Style = styledCell.ComputeStyle(propertyName, null, this.GetDownRigSuffix(rigJob));
            styledCell = this.SetCellMerge(styledCell);
            //styledCell.IsNeedRowMerge = false;
            return styledCell;

        }
        #endregion

        #region Compute BlendScheduled information from entity
        private StyledCell GetBlendScheduledStyledCell(string propertyName, RigJob rigJob, ProductHaulLoad productHaulLoad)
        {
            StyledCell styledCell;
            if (productHaulLoad == null)
            { 
                    styledCell=new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache);
            }
            else
            {
                styledCell = new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache) {PropertyValue= productHaulLoad.BlendChemical?.Name?.Replace(" + Additives","")?.Trim() ?? "", Notice=productHaulLoad.BlendChemical?.Description};
                styledCell.ContextMenus = productHaulLoad.ProductHaulLoadLifeStatus == ProductHaulLoadStatus.BlendCompleted? this.SetBlendContextMenus(productHaulLoad) : null;
            }
            styledCell.Style = styledCell.ComputeStyle(propertyName, null, this.GetDownRigSuffix(rigJob));
            styledCell.IsNeedRowMerge = false;
            return styledCell;
        }


        //Jan 2, 2024 zhangyuan 244_PR_AddBulkers: Add Remove right-click menu
        private StyledCell GetBlendScheduledFromProductHaulStyledCell(string propertyName, RigJob rigJob, ProductHaulLoad productHaulLoad, Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul productHaul, ShippingLoadSheet shippingLoadSheet)
        {
            StyledCell styledCell;
            if (productHaulLoad == null)
            {
                styledCell = new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache);
            }
            else
            {
                styledCell = new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache) { PropertyValue = productHaulLoad.BlendChemical?.Name?.Replace(" + Additives", "")?.Trim() ?? "", Notice = productHaulLoad.BlendChemical?.Description };
                styledCell.ContextMenus = productHaulLoad.ProductHaulLoadLifeStatus == ProductHaulLoadStatus.BlendCompleted ? this.SetBlendFromProductHaulContextMenus(productHaul,shippingLoadSheet) : null;
            }
            styledCell.Style = styledCell.ComputeStyle(propertyName, null, this.GetDownRigSuffix(rigJob));
            styledCell.IsNeedRowMerge = false;
            return styledCell;
        }

        private List<ContextMenu> SetBlendContextMenus(ProductHaulLoad productHaulLoad)
        {
            List<ContextMenu> list = new List<ContextMenu>();
            list.Add(new ContextMenu()
            {
                MenuName = MenuLoadBlendToBin,
                ProcessingMode = ProcessingMode.PopsUpWindow,
                Parms = new List<string>()
                {
                    productHaulLoad.Id.ToString()
                },
                ControllerName = ProductHaulController,
                ActionName = "LoadBlendToBin",
            });

            return list;
        }
        //Jan 15, 2024 zhangyuan 244_PR_AddBulkers: Add overload  right-click menu
        private List<ContextMenu> SetBlendFromProductHaulContextMenus(Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul productHaul, ShippingLoadSheet shippingLoadSheet)
        {
            List<ContextMenu> list = new List<ContextMenu>();
            if (IsBulkPlant)
            {
                if (productHaul != null)
                {
                    //Nov 13, 2023 Tongtao P45_Q4_175: add menu for"Load Blend to Bulker",if bin has not blend info,the menu could not use
                    list.Add(new ContextMenu()
                    {
                        MenuName = "Load Blend to Bulker",
                        ProcessingMode = ProcessingMode.PopsUpWindow,
                        Parms = new List<string>
                            {
                                productHaul.Id.ToString(), shippingLoadSheet.Id.ToString(),
                                shippingLoadSheet.SourceStorage.Id.ToString()
                            },
                        ControllerName = ProductHaulController,
                        ActionName = "LoadBlendToBulker",
                        IsDisabled = false,

                    });
                }

            }

            return list;
        }

        #endregion



        private StyledCell GetRigNameStyledCell(string propertyName, RigJob rigJob, ProductHaulLoad productHaulLoad)
        {
            StyledCell styledCell;
            if (productHaulLoad == null)
            {
                styledCell = new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache);
            }
            else
            {
                styledCell = new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache) { PropertyValue = productHaulLoad.Rig?.Name, Notice = productHaulLoad.Rig?.Description };
                //styledCell.ContextMenus = productHaulLoad.ProductHaulLoadLifeStatus == ProductHaulLoadStatus.BlendCompleted ? this.SetBlendContextMenus(productHaulLoad) : null;
            }
            styledCell.Style = styledCell.ComputeStyle(propertyName, null, this.GetDownRigSuffix(rigJob));

            styledCell = this.SetCellMerge(styledCell);
            return styledCell;
        }

        private StyledCell GetCSNumStyledCell(string propertyName, RigJob rigJob, ProductHaulLoad productHaulLoad)
        {
            StyledCell styledCell;
            if (productHaulLoad == null)
            {
                styledCell = new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache);
            }
            else
            {
                //Nov 8, 2023 Tongtao P45_Q4_161: Program number show  ProgramId + revision
                styledCell = new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache) { PropertyValue = (productHaulLoad.CallSheetNumber != null && productHaulLoad.CallSheetNumber != 0) ? productHaulLoad.CallSheetNumber.ToString() : productHaulLoad.ProgramId.ToString()+"."+ productHaulLoad.ProgramVersion.ToString("D2")};
            }
            styledCell.Style = styledCell.ComputeStyle(propertyName, null, this.GetDownRigSuffix(rigJob));
            styledCell = this.SetCellMerge(styledCell);
            return styledCell;
        }


        private StyledCell GetClientNameStyledCell(string propertyName, RigJob rigJob, ProductHaulLoad productHaulLoad)
        {
            StyledCell styledCell;
            if (productHaulLoad == null)
            {
                styledCell = new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache);
            }
            else
            {
                styledCell = new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache) { PropertyValue = productHaulLoad.Customer?.Name };
            }
            styledCell.Style = styledCell.ComputeStyle(propertyName, null, this.GetDownRigSuffix(rigJob));
            styledCell = this.SetCellMerge(styledCell);
            return styledCell;
        }

        //Dec 11, 2023 Tongtao 143_PR_AddBlendRequestToBulkPlantBoard: Add  Bin Last BlendRequest id On the BulkPlantBoard
        private StyledCell GetBinReqNumStyledCell(string propertyName, RigJob rigJob, BinInformation rigBinSection)
        {
            if (rigBinSection == null)
                return this.SetCellMerge(new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache));

            StyledCell styledCell = new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache) { PropertyValue = rigBinSection.LastProductHaulLoadId == 0 ? "" : rigBinSection.LastProductHaulLoadId.ToString() };
            styledCell.Style = styledCell.ComputeStyle(propertyName, null, this.GetDownRigSuffix(rigJob));
            styledCell = this.SetCellMerge(styledCell);
            return styledCell;
        }

        //Dec 11, 2023 Tongtao 143_PR_AddBlendRequestToBulkPlantBoard: Add BlendRequest id On the BulkPlantBoard
        private StyledCell GetBlendReqNumStyledCell(string propertyName, RigJob rigJob, ProductHaulLoad productHaulLoad)
        {
            StyledCell styledCell;
            if (productHaulLoad == null)
            {
                styledCell = new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache);
            }
            else
            {
                styledCell = new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache) { PropertyValue = productHaulLoad.Id==0?"":productHaulLoad.Id.ToString() };
            }
            styledCell.Style = styledCell.ComputeStyle(propertyName, null, this.GetDownRigSuffix(rigJob));
            styledCell.IsNeedRowMerge = false;
            return styledCell;
        }


        #region Compute ScheduledQuantity information from entity
        private StyledCell GetScheduledQuantityStyledCell(string propertyName, RigJob rigJob, ProductHaulLoad productHaulLoad)
        {
            StyledCell styledCell;
            if (productHaulLoad == null )
            {
                styledCell = new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache);
            }
            else
            {
               
                styledCell = new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache)
                {
                    PropertyValue = (productHaulLoad.TotalBlendWeight / 1000).ToString("##.###")
                };
            }
            styledCell.Style = styledCell.ComputeStyle(propertyName, null, this.GetDownRigSuffix(rigJob));
            //styledCell = this.SetCellMerge(styledCell);
            styledCell.IsNeedRowMerge = false;
            return styledCell;
        }
        #endregion

        #region Compute BlendingStatus information from entity
        private StyledCell GetBlendingStatusStyledCell(string propertyName, RigJob rigJob, ProductHaulLoad productHaulLoad)
        {
            StyledCell styledCell;
            if (productHaulLoad == null)
            {
                styledCell = new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache);
            }
            else
            {
                styledCell = new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache)
                {
                    PropertyValue = productHaulLoad.ProductHaulLoadLifeStatus.ToString()
                };
            }
            styledCell.Style = styledCell.ComputeStyle(propertyName, null, this.GetDownRigSuffix(rigJob));
            //styledCell = this.SetCellMerge(styledCell);
            styledCell.IsNeedRowMerge = false;
            return styledCell;
        }
        #endregion

        #region Compute TestingStatus information from entity
        private StyledCell GetTestingStatusStyledCell(string propertyName, RigJob rigJob,  BinInformation rigBinSection)
        {

            if (rigBinSection == null)
                return this.SetCellMerge(new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache));

            StyledCell styledCell = new StyledCell(propertyName, this.GetType(), this.LoggedUser, this._memoryCache) { PropertyValue = rigBinSection.BlendTestingStatus.ToString() };
//            styledCell.Style = styledCell.ComputeStyle(propertyName, null, this.GetDownRigSuffix(rigJob));
            styledCell = this.SetCellMerge(styledCell);
            //styledCell.IsNeedRowMerge = false;
            return styledCell;

        }
        #endregion

        #region Context Menu
        protected override List<ContextMenu> SetProductHaulBinContextMenus(Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul productHaul)
        {
	        List<Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul>
		        productHauls =
			        new List<Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.
				        ProductHaul>();
            productHauls.Add(productHaul);
	        List<ContextMenu> list = new List<ContextMenu>();
	        list.Add(new ContextMenu()
	        {
		        MenuName = MenuPrintMTS,
		        ProcessingMode = ProcessingMode.HaveNextMenu,
                //Mar 8,2024 zhangyaun 308_PR_BulkerShownLoaded : Modify Distinguish product haul in Bulkers and Bin columns
                MenuList = this.SetPrintMTSMenuForBin(productHauls,true)
	        });

            return list;
        }

        protected override List<ContextMenu> SetBinContextMenus(BinSectionModel binSectionModel, RigJob rigJob,
            BinInformation rigBinSections, List<Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul> productHauls, List<ProductHaulLoad> productHaulLoads)
        {
            List<ContextMenu> list = new List<ContextMenu>();
            if (rigBinSections != null)
            {
                if (IsBulkPlant)
                {
                    list.Add(new ContextMenu()
                    {
                        MenuName = MenuScheduleBlend,
                        ProcessingMode = ProcessingMode.PopsUpWindow,
                        Parms = new List<string>()
                    {
                        binSectionModel.BinId.ToString(), rigJob.CallSheetId.ToString(), binSectionModel.BinNumber,
                        rigJob.Rig.Name, rigJob.Rig.Id.ToString(), binSectionModel.BinSectionId.ToString(),
                        BinSectionModel.Name
                    },
                        ControllerName = ProductHaulController,
                        IsDisabled = (binSectionModel.BinId <= 0),
                        ActionName = "ScheduleBlend",
                    });
                    list.Add(new ContextMenu()
                    {
                        MenuName = MenuRescheduleBlend,
                        ProcessingMode = ProcessingMode.HaveNextMenu,
                        MenuList = this.SetReScheduleBlendMenuForBin(productHaulLoads, binSectionModel, productHauls)
                    });

                    list.Add(new ContextMenu()
                    {
                        MenuName = MenuCancelBlend,
                        ProcessingMode = ProcessingMode.HaveNextMenu,
                        MenuList = this.SetCancelBlendMenuForBin(productHaulLoads, binSectionModel, productHauls, rigJob)
                    });
                    list.Add(new ContextMenu()
                    {
                        MenuName = MenuHaulBlend,
                        ProcessingMode = ProcessingMode.HaveNextMenu,
                        IsHaveSplitLine = true,
                        MenuList = SetHaulBlendMenuForBin(productHaulLoads, binSectionModel, productHauls, rigJob)
                    });
                    list.Add(new ContextMenu()
                    {
                        MenuName = MenuRescheduleProductHaulForBin,
                        ProcessingMode = ProcessingMode.HaveNextMenu,
                        MenuList = this.SetRescheduleProductHaulForRigBoardBin(productHaulLoads, binSectionModel, productHauls, rigJob)
                    });
                    list.Add(new ContextMenu()
                    {
                        MenuName = MenuCancelProductHaulForBin,
                        ProcessingMode = ProcessingMode.HaveNextMenu,
                        MenuList = this.SetCancelProductHaulForBin(productHaulLoads, binSectionModel, productHauls, rigJob)
                    });

                    //Nov 13, 2023 Tongtao P45_Q4_175: add menu for"Load Blend to Bulker",if bin has not blend info,the menu could not use
                    list.Add(new ContextMenu()
                    {
                        MenuName = "Load Blend to Bulker",
                        ProcessingMode = ProcessingMode.HaveNextMenu,
                        MenuList = this.SetLoadBlendToBulkerForRigBoardBin(productHaulLoads, binSectionModel, productHauls, rigJob),
                        IsDisabled = string.IsNullOrEmpty(binSectionModel.BlendDescription)?true:false
                    });

                    list.Add(new ContextMenu()
                    {
                        MenuName = MenuOnLocationForBin,
                        ProcessingMode = ProcessingMode.HaveNextMenu,
                        MenuList = this.SetOnLocationMenuForBin(productHaulLoads, binSectionModel, productHauls)
                    });
                    list.Add(new ContextMenu()
                    {
	                    MenuName = MenuPrintMTS,
	                    ProcessingMode = ProcessingMode.HaveNextMenu,
	                    MenuList = this.SetPrintMTSMenuForBin(productHauls)
                    });
	                list.Add(new ContextMenu()
	                {
	                    MenuName = MenuTransferBlend,
	                    ProcessingMode = ProcessingMode.PopsUpWindow,
	                    //Nov 3, 2023 zhangyuan P63_Q4_174: Modify TransferBlend params
	                    Parms = new List<string>()
	                    {
	                        binSectionModel.BinSectionId.ToString(),
	                        "bulkplant"
	                    },
	                    ControllerName = ProductHaulController,
	                    ActionName = "TransferBlend",
	                    IsDisabled = string.IsNullOrEmpty(binSectionModel.BlendDescription) || Math.Abs(binSectionModel.Quantity) < 0.001,
	                    IsHaveSplitLine=true
	                });
	                list.Add(new ContextMenu()
	                {
	                    MenuName = MenuAdjustBlendAmount,
	                    ProcessingMode = ProcessingMode.PopsUpWindow,
	                    // Dec 28, 2023 zhangyuan 243_PR_AddBlendDropdown:Add  paramter flag
	                    Parms = new List<string>() {binSectionModel.BinSectionId.ToString(), "bulkplant" },
	                    ControllerName = BinBoardController,
	                    IsDisabled = (binSectionModel.BinId <= 0),
	                    ActionName = "UpdateQuantity",
	                });
	             
	                list.Add(new ContextMenu()
	                {
	                    MenuName = MenuEmptyBin,
	                    ProcessingMode = ProcessingMode.PopsUpWindow,
	                    IsDisabled = binSectionModel.BinId > 0 ? false : true,
	                    Parms = new List<string> {binSectionModel.BinSectionId.ToString()},
	                    ControllerName = BulkPlantController,
	                    ActionName = "EmptyBin"
	                });}
            }

            ContextMenu isNeedBinMenu = new ContextMenu()
            {
                ProcessingMode = ProcessingMode.NoPopsUpWindow,
                Parms = new List<string>() { rigJob.Id.ToString() },
                DialogName = "",
                ControllerName = RigBoardController,
                IsHaveSplitLine = true
            };

            if (!(bool)rigJob.IsNeedBins)
            {
                isNeedBinMenu.MenuName = MenuBinNotAssigned;
                isNeedBinMenu.ActionName = "NeedBin";
            }
            else
            {
                isNeedBinMenu.MenuName = MenuDoNotNeedBin;
                isNeedBinMenu.ActionName = "NotNeedBin";
                isNeedBinMenu.IsDisabled =  productHaulLoads !=null && !productHaulLoads.Count.Equals(0);
            }
            list.Add(isNeedBinMenu);

            list.Add(new ContextMenu()
            {
                MenuName = MenuAssignABin,
                ProcessingMode = ProcessingMode.PopsUpWindow,
                Parms = new List<string>() { rigJob.Id.ToString()},
                DialogName = DialogAddNewBin,
                ControllerName = RigBoardController,
                ActionName = "AssignABin",
                IsDisabled = false
            });
            if (rigBinSections != null)
            {
                list.Add(new ContextMenu()
                {
                    MenuName = MenuRemoveABin,
                    ProcessingMode = ProcessingMode.PopsUpWindow,
                    Parms = new List<string>()
                        {binSectionModel.BinId.ToString(), rigJob.CallSheetId.ToString(), rigJob?.Id.ToString()},
                    ControllerName = RigBoardController,
                    ActionName = "RemoveABin",
                    IsDisabled = binSectionModel.BinId.Equals(0)
                });
            }

            return list;
        }
      
        protected override List<ContextMenu> SetReScheduleBlendMenuForBin(List<ProductHaulLoad> productHaulLoadList,
            BinSectionModel binSectionModel,
            List<Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul>
                productHaulList = null)
        {
            List<ContextMenu> list = new List<ContextMenu>();

            if (productHaulLoadList != null && productHaulLoadList.Count > 0)
            {
                string actionName = "RescheduleBlendFromBulkPlantBin";
                if (!IsBulkPlant)
                {
                    actionName = "RescheduleBlendFromRigBin";
                }

                foreach (var item in productHaulLoadList)
                {
                    double enteredBlendWeight = item.IsTotalBlendTonnage ? item.TotalBlendWeight : item.BaseBlendWeight;
                    //Change the formatting precision from two decimal places to three decimal places
                    string menu = item.BlendChemical?.Name + " - " + Math.Round(enteredBlendWeight / 1000, 3) + "t";
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
                            { item.Id.ToString(), binSectionModel.BinSectionId.ToString(), binSectionModel.Name },
                        ControllerName = ProductHaulController,
                        ActionName = actionName,
                        MenuTips = tips,
                        IsDisabled = string.IsNullOrEmpty(item.ProgramId) ||
                                     item.ProductHaulLoadLifeStatus != ProductHaulLoadStatus.Scheduled
                    };
                    list.Add(contextMenu);
                }
            }

            return list;
        }
        protected override List<ContextMenu> SetCancelBlendMenuForBin(List<ProductHaulLoad> phlList,BinSectionModel binSectionModel, List<Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul> productHaulList = null, RigJob rigJob=null)
        {
            List<ContextMenu> list = new List<ContextMenu>();

            if (phlList != null && phlList.Count > 0)
            {
                foreach (var item in phlList)
                {
                    double enteredBlendWeight = item.IsTotalBlendTonnage ? item.TotalBlendWeight : item.BaseBlendWeight;
                    //Change the formatting precision from two decimal places to three decimal places
                    string menu = item.BlendChemical?.Name + " - "  + Math.Round(enteredBlendWeight / 1000, 3) + "t";

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
                        Parms = new List<string>() { item.Id.ToString() },
                        ControllerName = ProductHaulController,
                        ActionName = "CancelProductHaulLoad",
                        MenuTips=tips,
                        IsDisabled= string.IsNullOrEmpty(item.ProgramId) || item.ProductHaulLoadLifeStatus != ProductHaulLoadStatus.Scheduled
                    };
                    list.Add(contextMenu);
                }
            }

            return list;
        }

        protected override List<ContextMenu> SetHaulBlendMenuForBin(List<ProductHaulLoad> productHaulLoads, BinSectionModel binSectionModel, List<Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul> productHaulList = null, RigJob rigJob = null)
        {
            List<ContextMenu> list = new List<ContextMenu>();
            var rig = rigJob == null ? new Rig() : rigJob.Rig;
            if (productHaulLoads != null && productHaulLoads.Count > 0)
            {
                foreach (var productHaulLoad in productHaulLoads)
                {
                    double enteredBlendWeight = productHaulLoad.IsTotalBlendTonnage ? productHaulLoad.TotalBlendWeight : productHaulLoad.BaseBlendWeight;
                    string source = string.Empty;
                    if (productHaulLoad.CallSheetNumber != 0)
                        source = "CS" + productHaulLoad.CallSheetNumber.ToString() + " - " ;
                    else if (string.IsNullOrEmpty(productHaulLoad.ProgramId))
                        source = productHaulLoad.ProgramId + " - " ;

                    //Change the formatting precision from two decimal places to three decimal places
                    string menu = source + productHaulLoad.BlendChemical?.Name + " - " + Math.Round(enteredBlendWeight / 1000, 3) + "t";

                    // Nov 6, 2023 tongtao P45_Q4_169: If a blend request was created from a program and the blend hasn't been loaded to the bin yet，Haul Blend menu format is  "program id.revision - Blend Name xxt".
                    if (!string.IsNullOrEmpty(productHaulLoad.ProgramId))
                    {
                        menu = productHaulLoad.ProgramId + "." + productHaulLoad.ProgramVersion.ToString("D2") + "-" + menu;
                    }
                    string tips = productHaulLoad.ProductHaulLoadLifeStatus.ToString();
                    if (productHaulLoad.BlendShippingStatus != BlendShippingStatus.Empty)
                    {
                        tips += "|" + productHaulLoad.BlendShippingStatus.ToString();
                    }
                    if (productHaulLoad.BlendTestingStatus != BlendTestingStatus.None)
                    {
                        tips += "|" + productHaulLoad.BlendTestingStatus.ToString();     
                    }
                    ContextMenu contextMenu = new ContextMenu()
                    {
                        MenuName = menu.Replace("'", "%%%"),
                        ProcessingMode = ProcessingMode.PopsUpWindow,
                        Parms = new List<string>()
                        {
                            //productHaulLoad.Id.ToString(),binSectionModel.BinSectionId.ToString(), rigJob!=null?rigJob.Id.ToString():"0", binSectionModel.BinId.ToString(), binSectionModel.BlendDescription,
                            //binSectionModel.Quantity.ToString("#.##"), binSectionModel.BinNumber
                            productHaulLoad.Id.ToString(),binSectionModel.BinSectionId.ToString(), rigJob!=null?rigJob.Id.ToString():"0", binSectionModel.BinId.ToString(),
                            binSectionModel.BinNumber!=null?binSectionModel.BinNumber.ToString():"0",
                            binSectionModel.Quantity.ToString("##.##"),
                            binSectionModel.BlendDescription
                        },
                        ControllerName = ProductHaulController,
                        ActionName = "HualBlendFromRigBulkPlantBin",
                        MenuTips=tips,
                        //Nov 27, 2023 Tongtao P45_Q4_130: set haulBlend menu abled when productHaulLoad has ProgramId and productHaulLoad status in (Scheduled,Blending,BlendCompleted)
                        IsDisabled = !(!string.IsNullOrEmpty(productHaulLoad.ProgramId) &&
                                    (productHaulLoad.ProductHaulLoadLifeStatus == ProductHaulLoadStatus.Scheduled ||
                                     productHaulLoad.ProductHaulLoadLifeStatus == ProductHaulLoadStatus.Blending ||
                                     productHaulLoad.ProductHaulLoadLifeStatus == ProductHaulLoadStatus.BlendCompleted))
                    };
                    list.Add(contextMenu);
                }
            }
            if(!string.IsNullOrEmpty(binSectionModel.BlendDescription))
            {
                ContextMenu contextMenu = new ContextMenu()
                {
                    MenuName = "Blend in Bin - " + binSectionModel.BlendDescription + " " + binSectionModel.Quantity.ToString("##.##") + "t",
                    ProcessingMode = ProcessingMode.PopsUpWindow,
                    Parms = new List<string>()
                    {
                        //Nov 24, 2023 AW develop: pass known data in to improve performance 
                        binSectionModel.LastProductHaulLoadId.ToString(),
                        binSectionModel.BinSectionId.ToString(),
                        binSectionModel.BinId.ToString()!=""?binSectionModel.BinId.ToString():"0",
                        binSectionModel.Quantity.ToString("##.###"),
                        rig.Id.ToString(),
                        rig.Name,
                        binSectionModel.BlendDescription,
                        binSectionModel.Name
                    },
                    ControllerName = ProductHaulController,
                    ActionName = "HaulBlendFromBulkPlantBin",
                    MenuTips = "Blend in Bin"
                };
                list.Add(contextMenu);
            }

            return list;
        }

        protected override List<ContextMenu> SetRescheduleProductHaulForRigBoardBin(List<ProductHaulLoad> phlList, BinSectionModel binSectionModel, List<Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul> productHauls = null, RigJob rigJob = null)
        {
            List<ContextMenu> list = new List<ContextMenu>();
            if (productHauls != null)
            {
                //Apirl 28, 2024 Tongtao 191_PR_AllowReschedulingProductHaulBeforeOnlocation: Allow loaded ProductHaul show
                var productHaulList =
                        productHauls.FindAll(p => (p.ProductHaulLifeStatus != ProductHaulStatus.OnLocation));

                if (productHaulList.Count > 0)
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
                            Parms = new List<string>()
                            {
                                item.Id.ToString(), rigJob?.CallSheetId.ToString(), "0"
                            },

                            ControllerName = ProductHaulController,
                            ActionName = "RescheduleProductHaul",
                            //MenuStyle = item.BlendSectionId == blendSectionId ? GetProductHualLoadMenuStyle(item.Status) : "",
                            MenuTips = item.ProductHaulLifeStatus.ToString(),

                            //Apirl 26, 2024 Tongtao 191_PR_AllowReschedulingProductHaulBeforeOnlocation: Allow rescheduling product haul before OnLocation status
                            IsDisabled = item.ProductHaulLifeStatus == ProductHaulStatus.Empty || item.ProductHaulLifeStatus == ProductHaulStatus.Pending
                                        || item.ProductHaulLifeStatus == ProductHaulStatus.Returned,
                            MenuList = SetReScheduleProductHaulLoadForBulkPlntBin(item, binSectionModel, phlList)
                        };
                        list.Add(contextMenu);
                    }
                }
            }

            return list;
        }

        private  List<ContextMenu> SetReScheduleProductHaulLoadForBulkPlntBin( Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul productHaul, BinSectionModel binSectionModel, List<ProductHaulLoad> productHaulLoads)
        {
            List<ContextMenu> list = new List<ContextMenu>();

            if (productHaul.ShippingLoadSheets != null && productHaul.ShippingLoadSheets.Count > 0)
            {
                foreach (ShippingLoadSheet item in productHaul.ShippingLoadSheets)
                {
                    string menu = "Haul " + item.Name + " - " + item.LoadAmount / 1000 + "t";
                    string tips = item.ShippingStatus.ToString();
                    if (item.ShippingStatus != ShippingStatus.Empty)
                    {
                        tips += "|" + item.ShippingStatus.ToString();
                    }

                    if (item.ProductHaulLoad != null && item.ProductHaulLoad.Id != 0)
                    {
                        var productHaulLoad = productHaulLoads.Find(p => p.Id == item.ProductHaulLoad.Id);
                        if (productHaulLoad != null)
                        {
                            string sourceReference = productHaulLoad.CallSheetNumber != 0
                                ? "CS" + productHaulLoad.CallSheetNumber.ToString()
                                : string.IsNullOrEmpty(productHaulLoad.ProgramId)
                                    ? string.Empty
                                    : productHaulLoad.ProgramId;
                            if(!string.IsNullOrEmpty(sourceReference))
                                //Change the formatting precision from two decimal places to three decimal places
                                menu = menu + " from Blend Request: " + sourceReference + " - " + productHaulLoad.BlendChemical.Name + " " + Math.Round(productHaulLoad.TotalBlendWeight / 1000, 3) + "t";
                        }
                    }
                    ContextMenu contextMenu = new ContextMenu()
                    {
                        MenuName = menu.Replace("'", "%%%"),
                        ProcessingMode = ProcessingMode.PopsUpWindow,
                        Parms = new List<string>() { item.Id.ToString() },
                        ControllerName = ProductHaulController,
                        ActionName = "RescheduleBlendFromBulkPlantBin",
                        MenuTips = tips,
                        IsDisabled =true
                    };
                    list.Add(contextMenu);
                }
            }

            return list;
        }
        protected override List<ContextMenu> SetCancelProductHaulForBin(List<ProductHaulLoad> phlList, BinSectionModel binSectionModel, List<Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul> productHauls = null, RigJob rigJob = null)
        {
            List<ContextMenu> list = new List<ContextMenu>();
            if (productHauls != null)
            {
                var productHaulList =
                    productHauls.FindAll(p => p.ProductHaulLifeStatus != ProductHaulStatus.OnLocation&& p.ProductHaulLifeStatus != ProductHaulStatus.Loaded);
                if (productHaulList.Count > 0)
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
                            Parms = new List<string>()
                            {
                                item.Id.ToString(), rigJob?.Rig.Id.ToString(), rigJob?.CallSheetId.ToString(),
                                rigJob?.Id.ToString()
                            },
                            ControllerName = ProductHaulController,
                            ActionName = "CancelProductHaul",
                            //MenuStyle = item.BlendSectionId == blendSectionId ? GetProductHualLoadMenuStyle(item.Status) : "",
                            MenuTips = item.ProductHaulLifeStatus.ToString(),
                            IsDisabled = item.ProductHaulLifeStatus != ProductHaulStatus.Scheduled,
                            MenuList = SetCancelShippingLoadSheetForBulkPlantBin(item, phlList)
                        };
                        list.Add(contextMenu);
                    }
                }
            }

            return list;
        }
        private List<ContextMenu> SetCancelShippingLoadSheetForBulkPlantBin( Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul productHaul, List<ProductHaulLoad> productHaulLoads)
        {
            List<ContextMenu> list = new List<ContextMenu>();

            if (productHaul.ShippingLoadSheets != null && productHaul.ShippingLoadSheets.Count > 0)
            {
                foreach (ShippingLoadSheet item in productHaul.ShippingLoadSheets)
                {
                    string menu = "Haul " + item.Name + " - " + item.LoadAmount / 1000 + "t";
                    string tips = item.ShippingStatus.ToString();
                    if (item.ShippingStatus != ShippingStatus.Empty)
                    {
                        tips += "|" + item.ShippingStatus.ToString();
                    }

                    if (item.ProductHaulLoad != null && item.ProductHaulLoad.Id != 0)
                    {
                        var productHaulLoad = productHaulLoads.Find(p => p.Id == item.ProductHaulLoad.Id);
                        if (productHaulLoad != null)
                        {
                            string sourceReference = productHaulLoad.CallSheetNumber != 0
                                ? "CS" + productHaulLoad.CallSheetNumber.ToString()
                                : string.IsNullOrEmpty(productHaulLoad.ProgramId)
                                    ? string.Empty
                                    : productHaulLoad.ProgramId;
                            if(!string.IsNullOrEmpty(sourceReference))
                                //Change the formatting precision from two decimal places to three decimal places
                                menu = menu + " from Blend Request: " + sourceReference + " - " + productHaulLoad.BlendChemical.Name + " " + Math.Round(productHaulLoad.TotalBlendWeight / 1000, 3) + "t";
                        }
                    }
                    ContextMenu contextMenu = new ContextMenu()
                    {
                        MenuName = menu.Replace("'", "%%%"),
                        ProcessingMode = ProcessingMode.PopsUpWindow,
                        Parms = new List<string>() { item.Id.ToString() },
                        ControllerName = ProductHaulController,
                        ActionName = "CancelShippingLoadSheet",
                        MenuTips = tips,
                        IsDisabled = true
                    };
                    list.Add(contextMenu);
                }
            }

            return list;

        }

        protected override List<ContextMenu> SetOnLocationMenuForBin(List<ProductHaulLoad> phlList, BinSectionModel binSectionModel,
            List<Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul>
                productHauls = null)
        {
            List<ContextMenu> list = new List<ContextMenu>();
            if (productHauls != null)
            {
                var productHaulList =
                    productHauls.FindAll(p => p.ProductHaulLifeStatus != ProductHaulStatus.OnLocation && p.ProductHaulLifeStatus != ProductHaulStatus.Loaded);
                if (productHaulList.Count > 0)
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
                           // IsDisabled = item.ProductHaulLifeStatus != ProductHaulStatus.Scheduled && item.ProductHaulLifeStatus != ProductHaulStatus.InProgress,
                            IsDisabled = true,//Disable on location ability to allow user use Job Board more
                            MenuList = SetOnLocationShippingLoadSheetMenuForBulkPlantBin(item, item.Id)
                        };
                        list.Add(contextMenu);
                    }
                }
            }

            return list;
        }
        protected List<ContextMenu> SetPrintMTSMenuForBin(
	        List<Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul>
		        productHauls = null,bool isBulker = false)
        {
            List<ContextMenu> list = new List<ContextMenu>();
            if (productHauls != null)
            {
                var productHaulList = new List<Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul>();
                if (isBulker)
                {
                     productHaulList =
                        productHauls.FindAll(p =>
                            p.ProductHaulLifeStatus != ProductHaulStatus.OnLocation);
                }
                else
                {
                     productHaulList =
                        productHauls.FindAll(p =>
                            p.ProductHaulLifeStatus != ProductHaulStatus.OnLocation &&
                            p.ProductHaulLifeStatus != ProductHaulStatus.Loaded);
                }

                if (productHaulList.Count > 0)
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
                            ProcessingMode = ProcessingMode.OpenInNewTab,
                            Parms = new List<string>() { item.Id.ToString() },
                            ControllerName = ProductHaulController,
                            ActionName = "PrintMTS",
                            MenuTips = item.ProductHaulLifeStatus.ToString()
                        };
                        list.Add(contextMenu);
                    }
                }
            }

            return list;
        }

        private List<ContextMenu> SetOnLocationShippingLoadSheetMenuForBulkPlantBin(Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul productHaul,int productHaulId)
        {
            List<ContextMenu> list = new List<ContextMenu>();

            if (productHaul.ShippingLoadSheets != null && productHaul.ShippingLoadSheets.Count > 0)
            {
                foreach (ShippingLoadSheet item in productHaul.ShippingLoadSheets)
                {
                    string menu = "Haul " + item.Name + " - " + item.LoadAmount / 1000 + "t";
                    string tips = item.ShippingStatus.ToString();
                    if (item.ShippingStatus != ShippingStatus.Empty)
                    {
                        tips += "|" + item.ShippingStatus.ToString();
                    }
                    ContextMenu contextMenu = new ContextMenu()
                    {
                        MenuName = menu.Replace("'", "%%%"),
                        ProcessingMode = ProcessingMode.PopsUpWindow,
                        Parms = new List<string>() { item.Id.ToString() },
                        ControllerName = ProductHaulController,
                        ActionName = "OnLocationShippingLoadSheet",
                        MenuTips = tips,
                        IsDisabled =item.ShippingStatus==ShippingStatus.OnLocation
                    };
                    list.Add(contextMenu);
                }
            }

            return list;
        }


        //Nov 13, 2023 Tongtao P45_Q4_175: add menu for"Load Blend to Bulker",show producthal has Shipploadsheet 
        protected List<ContextMenu> SetLoadBlendToBulkerForRigBoardBin(List<ProductHaulLoad> phlList, BinSectionModel binSectionModel, List<Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul> productHauls = null, RigJob rigJob = null)
        {
            List<ContextMenu> list = new List<ContextMenu>();

            if (productHauls != null)
            {
                var productHaulList =
                    productHauls.FindAll(p => p.ProductHaulLifeStatus == ProductHaulStatus.Scheduled|| p.ProductHaulLifeStatus == ProductHaulStatus.Loading);
                if (productHaulList.Count > 0)
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
                            Parms = new List<string>()
                            {
                                item.Id.ToString(), rigJob?.CallSheetId.ToString(), rigJob?.Id.ToString()
                            },
                            ControllerName = ProductHaulController,
                            ActionName = "Load Blend to Bulker",
                            MenuTips = item.ProductHaulLifeStatus.ToString(),
                            IsDisabled = item.ProductHaulLifeStatus != ProductHaulStatus.Scheduled && item.ProductHaulLifeStatus != ProductHaulStatus.Loading,
                            MenuList = SetShippingLoadSheetForBulkPlntBin(item, binSectionModel, phlList)
          
                        };

                        if (contextMenu.MenuList.Count > 0)
                        {
                            list.Add(contextMenu);
                        }
                    }
                }
            }

            return list;
        }

        //Nov 13, 2023 Tongtao P45_Q4_175: add menu for"Load Blend to Bulker",show Shipploadsheets 
        private List<ContextMenu> SetShippingLoadSheetForBulkPlntBin(Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul productHaul, BinSectionModel binSectionModel, List<ProductHaulLoad> productHaulLoads)
        {
            List<ContextMenu> list = new List<ContextMenu>();


            if (productHaul.ShippingLoadSheets != null && productHaul.ShippingLoadSheets.Count > 0)
            {
                foreach (ShippingLoadSheet item in productHaul.ShippingLoadSheets)
                {
                    string menu = "Load Blend To Bulker " + item.Name + " - " + item.LoadAmount / 1000 + "t";
                    string tips = item.ShippingStatus.ToString();
                    if (item.ShippingStatus != ShippingStatus.Empty)
                    {
                        tips += "|" + item.ShippingStatus.ToString();
                    }

                    if (binSectionModel.BlendDescription == item.BlendDescription&& item.ShippingStatus== ShippingStatus.Scheduled)
                    {
                        ContextMenu contextMenu = new ContextMenu()
                        {
                            MenuName = menu.Replace("'", "%%%"),
                            ProcessingMode = ProcessingMode.PopsUpWindow,
                            Parms = new List<string> { productHaul.Id.ToString(), item.Id.ToString(), item.SourceStorage.Id.ToString() },
                            ControllerName = ProductHaulController,
                            ActionName = "LoadBlendToBulker",
                            MenuTips = tips
                        };
                        list.Add(contextMenu);
                    }
                }
            }

            return list;
        }


        #endregion Context Menu
    }
}
