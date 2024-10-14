using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eServiceOnline.Models.Commons;
using Sesi.SanjelData.Entities.BusinessEntities.WellSite;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment;

namespace eServiceOnline.Models.BinBoard
{
    public class BinViewModel : ModelBase<BinInformation>
    {
        #region const

        public const string BinBoardController = "BinBoard";
        public const string InYard = "Yard";
        public const string MenuUpdateNotes = "Update Notes";

        #endregion

        #region blend

        public const string MenuUpdateBlend = "Update Blend";

        #endregion
        #region QTY

        public const string MenuUpdateQTY = "Update Quantity";

        #endregion

        #region MyRegion

        public const string MenuUpdateCTY = "Update Capacity";

        #endregion
        #region Constructor

        public BinViewModel()
        {
            this.BinInformationModel = new BinInformationModel();
        }

        #endregion

        #region Properties

        public StyledCell BinNumber { get; set; }
        public StyledCell Location { get; set; }
        public StyledCell WorkServicePoint { get; set; }
        public StyledCell Blend { get; set; }
        public StyledCell Quantity { get; set; }
        public StyledCell Capacity { get; set; }
        public StyledCell Notes { get; set; }

        public BinInformationModel BinInformationModel { get; set; }


        #endregion

        #region Methods

        public override void PopulateFrom(BinInformation binInformation)
        {
            if (binInformation == null) throw new Exception("entity must be instance of class BinInformation.");
            BinInformationModel.PopulateFrom(binInformation);
            this.BinNumber = this.GetBinNUmberStyledCell("BinNumber");          
            this.Location = this.GetLocationStyledCell("Location");
            this.WorkServicePoint = this.GetWorkServicePointStyledCell("WorkServicePoint");
            this.Blend = this.GetBlendStyledCell("Blend");
            this.Quantity = this.GetQuantityStyledCell("Quantity");
            this.Capacity = this.GetCapacityStyledCell("Capacity");
            this.Notes = this.GetNotesStyledCell("Notes");
        }

        #region Compute bin number information from entity

        private StyledCell GetBinNUmberStyledCell(string propertyName)
        {
            StyledCell styledCell = new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue = BinInformationModel.Name };

            return styledCell;
        }

        #endregion

        #region Compute location information from entity

        private StyledCell GetLocationStyledCell(string propertyName)
        {
//            BinInformationModel rigBinSectionModel = this.BinInformationModels.Count > 0 ? this.BinInformationModels.FirstOrDefault() : null;
            StyledCell styledCell = new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue = BinInformationModel.RigName == null ? InYard : BinInformationModel.RigName };

            return styledCell;
        }

        #endregion

        #region Compute WorkServicePoint information from entity

        private StyledCell GetWorkServicePointStyledCell(string propertyName)
        {
          
            StyledCell styledCell = new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue = BinInformationModel?.ServicePointName };

            return styledCell;
        }

        #endregion


        #region Compute Blend information from entity
        private StyledCell GetBlendStyledCell(string propertyName)
        {
            StyledCell styledCell = new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue = BinInformationModel?.Blend,ContextMenus = this.SetBlendCotnextMenu()};

            return styledCell;
        }

        #endregion

        #region Compute Quantity information from entity

        private StyledCell GetQuantityStyledCell(string propertyName)
        {
            StyledCell styledCell = new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue = BinInformationModel?.Quantity+" t",ContextMenus = this.SetQuantityCotnextMenu()};

            return styledCell;
        }

        #endregion

        #region Compute Quantity information from entity

        private StyledCell GetCapacityStyledCell(string propertyName)
        {
            StyledCell styledCell = new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue = BinInformationModel?.Capacity + " t",ContextMenus = this.SetCapacityCotnextMenu()};

            return styledCell;
        }

        #endregion
        #region Compute ntoes information from entity

        private StyledCell GetNotesStyledCell(string propertyName)
        {
            //Nov 30, 2023 zhangyuan 203_PR_UpdateBinNotes: modify change informationId to binId
            StyledCell styledCell = new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue = this.NoteModel.Notes, ContextMenus = this.SetNoteContextMenus(BinInformationModel.BinId.ToString()) };

            return styledCell;
        }
        private List<ContextMenu> SetBlendCotnextMenu()
        {
            List<ContextMenu> list = new List<ContextMenu>
            {
                new ContextMenu
                {
                    MenuName = MenuUpdateBlend,
                    ProcessingMode = ProcessingMode.PopsUpWindow,
                    ControllerName =BinBoardController,
                    ActionName = "UpdateBlend",
                    Parms =new List<string>{BinInformationModel.Id.ToString()},
                    IsDisabled = BinInformationModel.Quantity>0,
                }
            };
            return list;
        }
        private List<ContextMenu> SetQuantityCotnextMenu()
        {
            List<ContextMenu> list = new List<ContextMenu>
            {
                new ContextMenu
                {
                    MenuName = MenuUpdateQTY,
                    ProcessingMode = ProcessingMode.PopsUpWindow,
                    ControllerName =BinBoardController,
                    ActionName = "UpdateQuantity",
                    Parms =new List<string>{BinInformationModel.Id.ToString()},
//                    IsDisabled = BinInformationModel.Blend==null
                }
            };
            return list;
        }
        private List<ContextMenu> SetCapacityCotnextMenu()
        {
            List<ContextMenu> list = new List<ContextMenu>
            {
                new ContextMenu
                {
                    MenuName = MenuUpdateCTY,
                    ProcessingMode = ProcessingMode.PopsUpWindow,
                    ControllerName =BinBoardController,
                    ActionName = "UpdateCapacity",
                    Parms =new List<string>{BinInformationModel.Id.ToString()}
                }
            };
            return list;
        }

        #endregion

        #endregion
    }
}
