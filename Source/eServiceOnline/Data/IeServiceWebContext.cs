using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using eServiceOnline.BusinessProcess;
using eServiceOnline.Models.NubbinBoard;
using eServiceOnline.Models.ProductHaul;
using eServiceOnline.Models.RigBoard;
using eServiceOnline.Models.SwedgeBoard;
using eServiceOnline.Models.WitsBoxBoard;
using MetaShare.Common.Foundation.Logging;
using MetaShare.Common.Foundation.Permissions;
using Sanjel.BusinessEntities.Jobs;
using Sanjel.BusinessEntities.Sections.Common;
using Sanjel.BusinessEntities.Sections.Header;
using Sanjel.Common.EService.Sections.Common;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Crew;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ResourceSchedule;
using Sesi.SanjelData.Entities.BusinessEntities.WellSite;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment;
using Sesi.SanjelData.Entities.Common.BusinessEntities.HumanResources;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Organization;
using Sesi.SanjelData.Entities.Common.BusinessEntities.WellSite;
using Sesi.SanjelData.Entities.Common.Model.Scheduling;
using BlendSection = Sesi.SanjelData.Entities.BusinessEntities.BaseEntites.BlendSection;
using CallSheet = Sanjel.BusinessEntities.CallSheets.CallSheet;
using UnitSection = Sanjel.BusinessEntities.Sections.Common.UnitSection;

namespace eServiceOnline.Data
{
    public interface IeServiceWebContext
    {
        Collection<int> GetBulkPlantRigJobIds();
        #region List Rig Board


        #endregion List Rig Board

        List<DistrictModel> GetDistricts();
        #region Product Haul
//        ProductHaulModel CreateProductHaul(ProductHaulModel productHaulModel);

        ProductLoadInfoModel CreateBlendRequest(ProductLoadInfoModel productLoadInfoModel);
//        void RescheduleProductHaulLoad(ProductHaulModel model);
        void RescheduleBlendRequest(ProductLoadInfoModel productLoadInfoModel, bool? IsRigJobBin = false);

        void CreateHaulBlend(int RigJobId, string LoggedUser,ProductLoadInfoModel ProductLoadInfoModel, ProductHaulInfoModel ProductHaulInfoModel, ShippingLoadSheetModel ShippingLoadSheetModel,bool? IsProductHual=false);
     
//        void RescheduleProductHaul(ProductHaulModel model);

//        void RescheduleProductHaul1(ProductHaulInfoModel model,int rigJobId, string userName);
        void RescheduleProductHaul1(RescheduleProductHaulViewModel model, int rigJobId, string userName);

        void ReschedulePodLoadAndBlendUnLoad(List<PodLoadAndBendUnLoadModel> podLoadAndBendUnLoadModels,int productHaulId, string userName);
        List<ProductHaulLoad> GetProductHaulLoadsByShippingLoadSheets(List<ShippingLoadSheet> shippingLoadSheets);
        #endregion Product Haul
        void CreateRig(Rig rig);
        List<ProductHaulModel> GetProductHaulCollectionByCallSheetNumber(int callSheetNumber);
        CallSheet GetCallSheetByNumber(int callSheetNumber);
        RigJobModel GetRigJobFromCallSheet(CallSheet callSheet);
        Collection<ProductHaulModel> GetProductHaulModelCollection();
        ProductHaul GetProductHaulById(int id);
//        void DeleteProductHaul(ProductHaulModel model);
//        void UpdateProductHaulAndHaulLoads(ProductHaul productHaul, bool isGoWithCrew, DateTime expectedOnLocationTime,int rigJobId,int crewId, bool originalHaulIsThirdParty);
        ProductHaul GetProductHaulInfoById(int id);
        WellLocationInformation GetWellLocationInfoByCallSheetNumber(int callSheetNumber);
        void UpdateWellLocationInfoByCallSheet(int rigJobId, int callSheetNumber, WellLocationInformation wellLocationInformation);
        Collection<BinSection> GetBinSectionCollctionByRootId(int rootId);
        CallSheet GetCallSheetByIdForBin(int id);
        CallSheet GetCallSheetByNumberForRigJob(int callSheetNumber);
        Rig GetRigInfoByRigId(int id);
        ClientCompany GetClientCompanyById(int id);
        Bin GetBinById(int binId);
        List<ShiftType> GetWorkShiftInfo();
        ShiftType GetWorkShiftById(int id);
        void CreateConsultant(ClientConsultant clientConsultant);
        FirstCall GetConsultantContactsByCallSheetNumber(int callSheetNumber);
        void UpdateConsultantInfo(int callSheetNumber, FirstCall consultantContacts);
        RigJob GetRigJobById(int id);
        RigJob UpdateRigJob(RigJob currentVersion, RigJob originalVersion);
        Collection<BlendSection> GetBlendSectionCollectionByRootIdIsCallSheetId(int rootId);
        ClientConsultant GetConsultantById(int id);
        Collection<ClientConsultant> GetClientConsultantCollection();
        int DeleteClientConsultant(ClientConsultant clientConsultant);
        void UpdateClientConsultant(bool isUpdateRigJob, ClientConsultant clientConsultant);
        RigJob CreateRigJobByCallSheet(int callsheetNumber);
        RigJob GetRigJobByCallsheetNumber(int callsheetNumber);
        bool AdjustJobDuration(int rigJobId, int estJobDuration);
        void ReleaseCrew(int rigJobId, bool isCompleteJob, DateTime jobCompleteTime);
//        void UpdateCrewScheduleByRigJob(RigJob rigJob);
        SanjelCrewSchedule GetCrewScheduleByJobCrewSection(int jobCrewSetionId);
        List<SanjelCrewWorkerSection> GetCrewWorkerSections(int id);
        List<Bin> GetBinCollectionByRig(Rig rig);

        List<BinInformation> GetBinInformationsByRigId(int rigId);
        List<DrillingCompany> GetDrillingCompanyInfo();
        List<ClientCompany> GetClientCompanyInfo();
        DrillingCompany GetDrillingCompanyById(int id);

        #region Rig Column Operation

        void UpdateRigInfo(Rig rig);
        bool UpdateRigStatus(int rigId, RigStatus newStatus);
        #endregion Rig Column Operation

        #region Refactor new interface

        CallSheet GetCallSheetById(int id, bool isMicroService = false);
        CallSheet UpdateCallSheet(CallSheet currentVersion, CallSheet originalVersion, bool isMicroService = true);
        RigJob GetMicroRigJobByCallSheetNumber(int callSheetNumber);
        RigJob UpdateMicroRigJob(RigJob currentVersion, RigJob originalVersion);
        RigJob CreateMicroRigJob(RigJob rigJob);
        Collection<JobPackage> GetJobPackageByCallSheetNumber(int callSheetNumber, bool isMicroService = true);
        Sanjel.BusinessEntities.Sections.Common.BlendSection GetBlendSectionById(int id);
        BinSection CreateBinSection(BinSection binSection);
        Sanjel.BusinessEntities.Sections.Common.BlendSection UpdateBlendSection(Sanjel.BusinessEntities.Sections.Common.BlendSection newBlendSection, Sanjel.BusinessEntities.Sections.Common.BlendSection oldBlendSection);
        List<RigJob> GetAllRigJobInformation(int pageNumber, int pageSize, Collection<int> servicePointIds, Collection<int> rigTypes, Collection<int> jobLifeStatuses, bool isShowJobAlert, bool isShowFutureJobs,  out int count);

        List<RigJob> GetBulkPlantRigJobInformation(Collection<int> rigJobIds, Collection<int> servicePointIds);
        bool ChangeRigJobStatusToComplete(int id, DateTime jobCompleteTime);
        bool ChangeRigJobStatusToCancel(int id, string notes);
//        void CreateOrUpdateUnitSectionsByProductHaul(ProductHaul productHaul,int crewId);
        BinInformation CreateBinInformation(BinInformation binAssignment);

        void AssignBinToRig(int binId, int rigJobId,Collection<BinInformation> binInformationList);

        List<BinInformation> GetBinInformationCollectionByRig(Rig rig);
        bool UnassignBinToRig(int binId, int callSheetId, int rigJobId);
        User GetSecuredUserByApplicationAndUserName(string applicationName, string userName);
        AccessRecord GetAccessRecordByUserId(int userId);
        AccessRecord CreateAccessRecord(AccessRecord accessRecord);
        AccessRecord UpdateAccessRecord(AccessRecord currentVersion, AccessRecord originalVersion);
        int DeleteRigJob(RigJob rigJob);
        bool ActivateARig(int rigId);
        List<Rig> GetDeactivateRigByDrillingCompanyId(int drillingCompanyId);
        List<Rig> GetRigByDrillingCompanyId(int drillingCompanyId);
        List<RigJob> GetRigJobCollectionForOperation();
        List<RigJob> GetUpComingJobsInformation(int pageNumber, int pageSize, Collection<int> servicePointIds, int windowStart, int windowEnd, out int count);
//        void CreateOrUpdateThirdPartyUnitSections(ProductHaul producthaul,int crewId);
        Rig GetRigByName(string rigName);
//        void UpdateProductHaulAndLoadsOnLocation(int productHaulId, DateTime onLocationTime, string loggedUser);

        void UpdateProductHaulOnLocation(OnLocationProductHaulViewModel model);
        List<ProductHaul> GetProductHaulCollectionBycallSheetNumber(int callSheetNumber);
        List<ProductHaulLoad> GetProductHaulLoadListByCallSheetNumber(int callSheetNumber);
//        int DeleteProductHaulLoadById(ProductHaulLoadModel model);
        List<ProductHaulLoad> GetProductHaulLoadCollectionByProductHaulId(int productHaulId);
        List<ProductHaul> GetExistingProductHaulCollection();
//        void AddNewProductLoadToExistingProductHaul(ProductHaulModel productHaulModel);
        ProductHaulLoad GetProductHaulLoadById(int productHaulLoadId);
        void UpdateProductHaulLoadOnly(ProductHaulLoad productHaulLoad);
//        void UpdateProductHaulAndHaulLoad(ProductHaul productHaul, ProductHaulLoad productHaulLoad, bool isCreateProductHaul, int callSheetId, int rigJobId,int crewId = 0);
//        void UpdateShippingLoadSheetOnLocation(ShippingLoadSheet shippingLoadSheet, DateTime onLocationTime, string userName);
        void UpdateCompanyShortName(int rigJobId, int clientCompanyId, string companyShortName);
        List<BlendChemicalModel> GetBlendChemicalPageInfo(int i, int pageNumber, int pageSize, out int count);
        List<ProductHaulLoadModel> GetProductHaulPageInfo(int i, int servicePointId, int pageNumber, int pageSize, out int count);
        ProductHaulModel GetProductHaulModelByProductHaulLoadId(int productHaulId);
//        void NotUseOriginalHaul(ProductHaulModel model, string callSheetId);
        List<ProductLoadSection> GetProductLoadSectionCollectionByProductLoadId(int productHaulLoadId);
        DateTime GetCallCrewTime(int rigJobId);
        int AssignACrew(int crewId, int rigJobId, DateTime callCrewTime, int duration);
        List<RigJobSanjelCrewSection> GetRigJobCrewSectionByRigJob(int rigJobId);
        void CallAllCrews(int rigJobId);

        void WithdrawACrew(int rigJobId, int crewId, int jobCrewSectionStatusId);
        ServicePoint GetServicePointById(int id);

        int UpdateRigBinSection(BinInformation rigBinSection);
        List<ProductHaulLoad> GetProductHaulLoadByBlendSectionId(int blendSectionId);
        Sesi.SanjelData.Entities.BusinessEntities.BaseEntites.BlendSection GetBlendSectionByBlendSectionId(int blendSectionId);
        List<BlendSection> GetBlendSectionsByCallSheetId(int callSheetId);
        //Nov 1, 2023 AW P45_Q4_105: Change get blend section method to improve the performance
        List<CallSheetBlendSection> GetBlendSectionsByCallSheetIdsAndBlendName(List<int> callSheetIds, string blendName);
        Job GetJobByUniqueId(string jobUniqueId);
        List<SanjelCrew> GetCrewsByRigJob(RigJob rigJob);

        #endregion

        #region Crew Board Operation

        SanjelCrew GetCrewById(int id);
        Employee GetEmployeeById(int id);
        List<SanjelCrew> GetAllCrewsInfo(Collection<int> servicePointIds);
        List<SanjelCrew> GetSanjelCrews();
        List<Employee> GetEmployeeList();
        List<Employee> GetActivatedEmployees(int crewId);
        List<TruckUnit> GetTruckUnitList();
        List<TruckUnit> GetActivatedTruckUnits(int crewId);
        CrewType GetCrewTypeById(int id);
        int CreateCrew(SanjelCrew crew, int homeDistrictId, int primaryTruckUnitId = 0, int secondaryTruckUnitId = 0, int supervisorId = 0, int crewMemberId = 0);
        List<TruckUnit> GetTruckUnitsByCrew(int crewId);
        List<Employee> GetEmployeesByCrew(int crewId);
        void AddUnitToCrew(int truckUnitId, int crewId);
        void AddWorkerToCrew(int workerId, int crewId, int crewPositionId);
        void RemoveUnitFromCrew(int truckUnitId, int crewId);
        void RemoveWorkerFromCrew(int workerId, int crewId);
        List<CrewPosition> GetCrewPositions();
        List<ServicePoint> GetServicePoints();
        List<CrewType> GetCrewTypesForSanjelCrew();
        void LogOnDuty(int rigJobId);
        void LogOffDuty(int rigJobId);
        List<SanjelCrewSchedule> GetFutureCrewSchedules(int crewId, DateTime dateTime);
        List<SanjelCrew> GetEffectiveCrews(DateTime startTime, double duration, int workingDistrict, int rigJobId);
        int UpdateCrew(SanjelCrew crew);
        int UpdateWorker(Employee employee);
        List<SanjelCrew> GetCrewsByServicePoint(Collection<int> servicePoints, out int count);
        List<TruckUnit> GetUnitsByServicePoint(int pageSize, int pageNumber, Collection<int> servicePoints, out int count);
        List<Employee> GetWorkerListByServicePoints(int pageSize, int pageNumber, Collection<int> servicePoints, out int count);
        RigJobSanjelCrewSection GetRigJobCrewSectionById(int id);
        List<TruckUnit> GetTruckUnitsByServicePoints(int pageSize, int pageNumber, Collection<int> servicePoints, out int count);
        void RemoveCrew(int crewId);
        void AssignCrewToAnotherDistrict(int crewId, int workingDistrictId);

        void RemoveAllWorker(int crewId);
        #endregion

        #region schedule

        List<SanjelCrewSchedule> GetCrewSchedules();
        List<SanjelCrewSchedule> GetCrewSchedules(Collection<int> servicePoints, DateTime startTime, DateTime endTime);
        List<UnitSchedule> GetUnitSchedules();
        List<UnitSchedule> GetUnitSchedules(Collection<int> servicePoints, DateTime startTime, DateTime endTime);
        List<WorkerSchedule> GetWorkerSchedules();
        List<WorkerSchedule> GetWorkerSchedules(Collection<int> servicePoints, DateTime startTime, DateTime endTime);
        SanjelCrewSchedule GetCrewScheduleById(int id);
        UnitSchedule GetUnitScheduleById(int id);
        WorkerSchedule GetWorkerScheduleById(int id);
        int DeleteCrewSchedule(int id);
        int DeleteUnitSchedule(int id);
        int DeleteWorkSchedule(int id);
        int UpdateCrewSchedule(SanjelCrewSchedule crewSchedule);
        int UpdateWorkerSchedule(WorkerSchedule workerSchedule);
        int UpdateUnitSchedule(UnitSchedule unitSchedule);
        int InsertCrewSchedule(SanjelCrewSchedule crewSchedule);
        int InsertWorkerSchedule(WorkerSchedule workerSchedule);
        int InsertUnitSchedule(UnitSchedule unitSchedule);
//        List<UnitScheduleType> UnitScheduleTypes();
//        List<WorkerScheduleType> WorkerScheduleTypes();
        SanjelCrewTruckUnitSection GetCrewTruckUnitSectionById(int id);
        SanjelCrewWorkerSection GetCrewWorkerSectionById(int id);
        string VerifyUnitSchedule(int truckUnitId, DateTime startTime, DateTime endTime);
        string VerifyWorkerSchedule(int workerId, DateTime startTime, DateTime endTime);
        string VerifyThirdPartyBulkerCrewSchedule(int thirdPartyBulkerCrewId, DateTime startTime, DateTime endTime);
        string VerifySanjelCrewSchedule(int sanjelCrewId, DateTime startTime, DateTime endTime);

        #endregion

        #region Third Party Crew Board Operation

        List<ThirdPartyBulkerCrew> GetThirdPartyCrews(int pageSize, int pageNumber, out int count);
        List<ThirdPartyBulkerCrew> GetThirdPartyCrews();
        ThirdPartyBulkerCrew GetThirdPartyBulkerCrewById(int id);
        int UpdateThirdPartyBulkerCrew(ThirdPartyBulkerCrew thirdPartyBulkerCrew);
        Collection<ContractorCompany> GetAllContractorCompanies();
        int CreateThirdPartyBulkerCrew(ThirdPartyBulkerCrew thirdPartyBulkerCrew);
        ContractorCompany GetContractorCompanyById(int id);

        #endregion


        RigSizeType GetRigSizeTypeById(int id);
        RigSize GetRigSizeById(int id);
        ThreadType GetThreadTypeById(int id);
//        double GetDuration(int crewId, int rigJobId,bool isThridParty);
        TruckUnit GetTruckUnitById(int id);
        List<SanjelCrewSchedule> GetCrewScheduleByCrewId(int crewId);
        ThirdPartyBulkerCrewSchedule GetThirdPartyCrewScheduleById(int id);
        List<ProductHaul> GetProductHaulByCrewScheduleId(int crewScheduleId);

        List<ThirdPartyBulkerCrewSchedule> GetThirdPartyCrewScheduleByThirdPartyCrewId(int thirdPartyCrewId);

        int DeleteThirdPartyBulkerCrew(int crewId);

        #region CrewBoard

        int CreateSanjelCrewNote(SanjelCrewNote sanjelCrewNote);
        SanjelCrewNote GetSanjelCrewNoteBySanjelCrewId(int sanjelCrewId);
        int UpdateSanjelCrewNote(int sanjelCrewId, string notes);

        #endregion CrewBoard

        #region thirdpartyboard
        ThirdPartyBulkerCrewNote GeThirdPartyBulkerCrewNoteByThirdPartyBulkerCrew(int thirdPartyBulkerCrewId);
        int CreateThirdPartyBulkerCrewNote(ThirdPartyBulkerCrewNote thirdPartyBulkerCrewNote);
        int UpdateThirdPartyBulkerCrewNote(int thirdPartyBulkerCrewId, string notes);

        #endregion
        #region workerBoard
        EmployeeNote GetEmployeeNoteByEmployee(int employeeId);
        int UpdateEmployeeNote(int employeeId, string notes);

        int UpdateEmployeeProfile(int employeeId, string profile);
        EmployeeProfile GetEmployeeProfileByEmployee(int employeeId);
        #endregion

        #region TruckUnitBoard
        TruckUnitNote GetTruckUnitNoteByTruckUnit(int truckUnitId);
        int UpdateTruckUnitNote(int truckUnitId,string notes);

        #endregion

        #region Bin Board

        List<Bin> GetBinsByServicePoint(int pageSize, int pageNumber, Collection<int> servicePoints, out int count);
        List<BinInformation> GetBinInformationsByServicePoint(int pageSize, int pageNumber, Collection<int> servicePoints, out int count);
        BinInformation GetBinInformationByBinId(int binId,int podIndex);
        //Nov 30, 2023 zhangyuan 203_PR_UpdateBinNotes: modify Add PodIndex
        BinNote GetBinNoteByBinAndPodIndex(Bin bin, int podIndex);
        int UpdateBinNote(int binId, string notes, int podIndex);
        List<ProductHaulLoad> GetProductHaulLoadByBinId(int binId, int podIndex);
        List<ProductHaulLoad> GetProductHaulLoadByBinId(int binId, Collection<int> productHaulLoadLifeStatuses);

        // Dec 27, 2023 zhangyuan 243_PR_AddBlendDropdown:Add blendId Parameter
        void UpdateQuantity(int binId, double quantity, string description, string username,int podIndex,int blendId);
        void UpdateCapacity(int binId,double capacity, int podIndex);

        void UpdateBinformationBlend(int binId, int blendId, string username, int podIndex);
        #endregion






        DateTime GetRigJobAssginCrewEndTime(int rigJobId);
        /*
                int UpdateNotes(int id,string note,string type);
                string GetNotes(int id, string type);
        */


        #region District Notes

        int CreateServicePointNote(ServicePointNote servicePointNote);
        ServicePointNote GetServicePointNoteByServicePointId(int servicePointId);
        int UpdateServicePointNote(int servicePointId, string notes);

        #endregion


        #region PlugLoadingHeadBoard

        PlugLoadingHeadInformation GetPlugLoadingHeadInformationByPlugLoadingHeadId(int plugLoadingHeadId);

        List<PlugLoadingHead> GetPlugLoadingHeadsByServicePoint(int pageSize, int pageNumber, Collection<int> servicePoints, out int count);

        void UpdatePlugLoadingHeadNote(int plugLoadingHeadId, string notes);
        #endregion

        #region manifold board

        ManifoldInformation GetManifoldInformationByManifoldId(int manifoldId);
        List<Manifold> GetManifoldsByServicePoint(int pageSize, int pageNumber, Collection<int> servicePoints, out int count);
        void UpdateManifoldNote(int manifoldId, string notes);
        #endregion

        #region topDrivceAdaptor Board

        TopDrivceAdaptorInformation GetTopDrivceAdaptorInformationByTopDrivceAdaptorId(int topDrivceAdaptorId);
        List<TopDriveAdaptor> GeTopDriveAdaptorByServicePoint(int pageSize, int pageNumber, Collection<int> servicePoints, out int count);

        void UpdateTopDriveAdaptorNotes(int topDriveAdaptorId, string notes);
        #endregion


        #region PlugLoadingHeadSub Board

        PlugLoadingHeadSubInformation GetPlugLoadingHeadSubInformationByPlugLoadingHeadSubId(int plugLoadingHeadSubId);
        List<PlugLoadingHeadSub> GetPlugLoadingHeadSubsByServicePoint(int pageSize, int pageNumber, Collection<int> servicePoints, out int count);
        void UpdatePlugLoadingHeadSubNotes(int plugLoadingHeadSubId, string notes);
        #endregion


        #region assgin plug loading head

        List<PlugLoadingHead> GetPlugLoadingHeadsByServicePoint(int servicePointId);
        List<PlugLoadingHeadSub> GetPlugLoadingHeadSubsByServicePoint(int servicePointId);
        List<TopDriveAdaptor> GetTopDriveAdaptorsByServicePoint(int servicePointId);
        List<Manifold> GetManifoldsByServicePoint(int servicePointId);
        Manifold PlugLoadingHeadhasManifold(int plugLoadingHeadId);

        void AssignPlugLoadingHead(AssginPlugLoadingHeadModel model);
        List<PlugLoadingHeadInformation> GetPlugLoadingHeadInformationByRigJobId(int rigJobId);

        void ReturnEquipments(List<ReturnEquipentsModel> models);


        List<NubbinInformation> GetNubbinInformationByRigJobId(int rigJobId);
        List<SwedgeInformation> GetSwedgeInformationByRigJobId(int rigJobId);
        List<WitsBoxInformation> GetWitsBoxInformationByRigJobId(int rigJobId);
        #endregion


        #region NubbinsBoard

        List<Nubbin> GetNubbinsByServicePoint(int pageSize, int pageNumber, Collection<int> servicePoints, out int count);
        NubbinInformation GetNubbinInformationByNubbinId(int nubbinId);
        void UpdateNubbinNotes(int nubbinId, string notes);
        #endregion

        #region Swedge board

        List<Swedge> GetSwedgeByServicePoint(int pageSize, int pageNumber, Collection<int> servicePoints, out int count);
        SwedgeInformation GetSwedgeInformationBySwedgeId(int swedgeId);

        void UpdateSwedgeNotes(int swedgeId, string notes);
        #endregion

        #region WitsBox Board

        List<WitsBox> GetWitsBoxByServicePoint(int pageSize, int pageNumber, Collection<int> servicePoints, out int count);
        WitsBoxInformation GetWitsBoxInformationByWitsBoxId(int witsBoxId);
        void UpdateWitsBoxNotes(int witsBoxId, string notes);
        #endregion

        #region Assign Wits Box

        void AssignWitsBoxToRigJob(WitsBoxModel witsBoxModel);
        List<WitsBox> GetWitsBoxListByServicePoint(int servicePointId);

        #endregion

        #region Assign Nubbin

        List<Nubbin> GetEffectiveNubbinsByServicePoint(int servicePointId);
        void AssignNubbinToRigJob(NubbinModel model);

        #endregion

        #region Assign Swedge

        List<Swedge> GetEffectiveSwedgesByServicePoint(int servicePointId);
        void AssignSwedgeToRigJob(SwedgeModel model);

        #endregion

        List<ProductHaulLoad> GetProductHaulLoadCollectionByServicePoint(int servicePointId);
        List<ProductHaulLoad> GetScheduledProductHaulLoadsByServicePoint(int servicePointId);

        List<ProductHaulLoad> GetBlendingProductHaulLoadsByServicePoint(int servicePointId);
        List<ProductHaulLoad> GetLoadedProductHaulLoadsByServicePoint(int servicePointId);
        List<ProductHaulLoad> GetBlendCompletedProductHaulLoadsByServicePoint(int servicePointId);
        int SetProductHaulLoadBlending(int productLoadId);
        int SetProductHaulLoadLoaded(int productLoadId);
        int SetProductHaulLoadBlendCompleted(int productLoadId);
        string SetBlendCompletedProductHaulLoadToScheduled(int productLoadId);
        string SetBlendingProductHaulLoadToScheduled(int productLoadId);
        string SetLoadedProductHaulLoadToScheduled(int productLoadId);
        string SetProductHaulLoadScheduled(int productLoadId);
        List<ProductHaul> GetProductHaulCollectionByProductHaulIds(Collection<int> productHaulIds);

        BinInformation GetBinInformationById(int id);
        bool DeactivateRig(int rigId);
        void UpdateCallSheetAndRigJob(int rigJobId);
        void AlignPumperCrewAssignment(int rigJobId);

        void ScheduleProductHaul(ScheduleProductHaulFromRigJobBlendViewModel model);

        void UpdateRigJobCrewSchedule(RigJob currentRigJob);
        string SetBulkerCrewStatusByCrewId(int crewId, BulkerCrewStatus status, bool isThirdParty, string username);
        List<ShippingLoadSheet> GetScheduledShippingLoadSheetsByStorage(int binInformationId);
        //Dec 11, 2023 zhangyuan 226_PR_CalcRemainsAmount: And  calculate remains amount
        List<ShippingLoadSheet> GetShippingLoadSheetsByProductHaulLoadId(int productHaulLoadId);
        WorkerSchedule GetShiftScheduleEndDate(int employeeId);
        int ExtendShiftScheduleEndDate(int employeeId, int rotationIndex, DateTime startDateTime, DateTime endDateTime);
    }
}