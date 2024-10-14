using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using eServiceOnline.Models.Commons;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Crew;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ResourceSchedule;

namespace eServiceOnline.Models.ThirdPartyCrewBoard
{
    public class ThirdPartyCrewViewModel : ModelBase<ThirdPartyBulkerCrew>
    {
        #region const

        public const string ThirdPartyCrewBoardController = "ThirdPartyCrewBoard";

        #region Company

        public const string MenuAddCrew = "Add A Crew";
        public const string MenuUpdateCrew = "Update A Crew";
        public const string MenuRemoveCrew = "Remove A Crew";

        #endregion

        #region Notes

        public const string MenuUpdateNotes = "Update Notes";

        #endregion

        #endregion const

        #region Constructor

        public ThirdPartyCrewViewModel(){}
            
        #endregion Constructor

        #region Properties

        public StyledCell Company { get; set; }
        public StyledCell Unit { get; set; }
        public StyledCell Contact { get; set; }
        public StyledCell JobInformation { get; set; }
        public StyledCell Notes { get; set; }

        public List<ThirdPartyBulkerCrewSchedule> ThirdPartyBulkerCrewSchedules { get; set; }

        public NoteModel NoteModel { get; set; }
        #endregion

        #region Methods

        public override void PopulateFrom(ThirdPartyBulkerCrew thirdPartyBulkerCrew)
        {
            if (thirdPartyBulkerCrew == null) throw new Exception("entity must be instance of class ThirdPartyBulkerCrew.");
            this.Company = this.GetCompanyStyledCell("Company", thirdPartyBulkerCrew);
            this.Unit = this.GetUnitStyledCell("Unit", thirdPartyBulkerCrew);
            this.Contact = this.GetContactStyledCell("Contact", thirdPartyBulkerCrew);
            this.JobInformation = this.GetJobInformationStyledCell("JobInformation", thirdPartyBulkerCrew);
            this.Notes = this.GetNotesStyledCell("Notes", thirdPartyBulkerCrew);
        }

        #region Compute company from entity

        private StyledCell GetCompanyStyledCell(string propertyName, ThirdPartyBulkerCrew thirdPartyBulkerCrew)
        {
            if (thirdPartyBulkerCrew == null)
            {
                return new StyledCell(propertyName, null, this.LoggedUser, null) { IsNeedRowMerge = false };
            }
            StyledCell styledCell = new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue = thirdPartyBulkerCrew.ContractorCompany?.Name, ContextMenus = this.SetCompanyContextMenus(thirdPartyBulkerCrew)};

            return styledCell;
        }

        private List<ContextMenu> SetCompanyContextMenus(ThirdPartyBulkerCrew thirdPartyBulkerCrew)
        {
            List<ContextMenu> list = new List<ContextMenu>();
            list.Add(new ContextMenu
            {
                MenuName = MenuAddCrew,
                ProcessingMode = ProcessingMode.PopsUpWindow,
                ControllerName = ThirdPartyCrewBoardController,
                ActionName = "CreateThirdPartyCrew"
            });
            list.Add(new ContextMenu()
            {
                MenuName = MenuUpdateCrew,
                ProcessingMode = ProcessingMode.PopsUpWindow,
                ControllerName = ThirdPartyCrewBoardController,
                Parms = new List<string> { thirdPartyBulkerCrew.Id.ToString() },
                ActionName = "UpdateThirdPartyCrew"
            });
            list.Add(new ContextMenu()
            {
                MenuName = MenuRemoveCrew,
                ProcessingMode = ProcessingMode.PopsUpWindow,
                ControllerName = ThirdPartyCrewBoardController,
                Parms = new List<string> { thirdPartyBulkerCrew.Id.ToString() },
                ActionName = "RemoveThirdPartyCrew",
                IsDisabled = this.ThirdPartyBulkerCrewSchedules.Count > 0
            });
            return list;
        }

        #endregion

        #region Compute unit information from entity

        private StyledCell GetUnitStyledCell(string propertyName, ThirdPartyBulkerCrew thirdPartyBulkerCrew)
        {
            if (thirdPartyBulkerCrew == null)
            {
                return new StyledCell(propertyName, null, this.LoggedUser, null) { IsNeedRowMerge = false };
            }
            StyledCell styledCell = new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue = thirdPartyBulkerCrew.ThirdPartyUnitNumber };

            return styledCell;
        }

        #endregion

        #region Compute contact information from entity

        private StyledCell GetContactStyledCell(string propertyName, ThirdPartyBulkerCrew thirdPartyBulkerCrew)
        {
            if (thirdPartyBulkerCrew == null)
            {
                return new StyledCell(propertyName, null, this.LoggedUser, null) { IsNeedRowMerge = false };
            }
            StyledCell styledCell = new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue = thirdPartyBulkerCrew.SupplierContactName + " " + thirdPartyBulkerCrew.SupplierContactNumber };

            return styledCell;
        }

        #endregion

        #region Compute job information from entity

        private StyledCell GetJobInformationStyledCell(string propertyName, ThirdPartyBulkerCrew thirdPartyBulkerCrew)
        {
            string jobInfo = string.Empty;
            if (thirdPartyBulkerCrew == null)
            {               
                return new StyledCell(propertyName, null, this.LoggedUser, null) { IsNeedRowMerge = false};
            }
            else
            {
                if (this.ThirdPartyBulkerCrewSchedules.Count != 0)
                {
                    foreach (var thirdPartyBulkerCrewSchedule in ThirdPartyBulkerCrewSchedules)
                    {
                        if (thirdPartyBulkerCrewSchedule.EndTime>DateTime.Now)
                        {
                            jobInfo = thirdPartyBulkerCrewSchedule.Description;
                        }
                    }
                }
            }
            StyledCell styledCell = new StyledCell(propertyName, null, this.LoggedUser, null){PropertyValue = jobInfo};

            return styledCell;
        }

        #endregion

        #region Compute notes from entity

        private StyledCell GetNotesStyledCell(string propertyName, ThirdPartyBulkerCrew thirdPartyBulkerCrew)
        {
            return new StyledCell(propertyName, null, this.LoggedUser, null) { PropertyValue = this.NoteModel.Notes, ContextMenus = this.SetNotesContextMenus(thirdPartyBulkerCrew) };
        }

        private List<ContextMenu> SetNotesContextMenus(ThirdPartyBulkerCrew thirdPartyBulkerCrew)
        {
            List<ContextMenu> list = new List<ContextMenu>();
            list.Add(new ContextMenu
            {
                MenuName = MenuUpdateNotes,
                ProcessingMode = ProcessingMode.PopsUpWindow,
                Parms = new List<string> { thirdPartyBulkerCrew.Id.ToString(), NoteModel.ReturnActionName, NoteModel.ReturnControllerName, NoteModel.PostControllerName, NoteModel.PostMethodName, NoteModel.Notes },
                ControllerName = NoteModel.CallingControllerName,
                ActionName = NoteModel.CallingMethodName
            });

            return list;
        }

        #endregion

        #endregion Methods
    }
}
