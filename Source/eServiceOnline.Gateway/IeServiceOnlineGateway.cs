using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MetaShare.Common.Core.Entities;
using Sanjel.BusinessEntities.Jobs;
using Sanjel.BusinessEntities.Sections.Common;
using Sesi.SanjelData.Entities.BusinessEntities.BulkPlant;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Crew;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ResourceSchedule;
using Sesi.SanjelData.Entities.BusinessEntities.WellSite;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment;
using Sesi.SanjelData.Entities.Common.BusinessEntities.HumanResources;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Organization;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Products;
using Sesi.SanjelData.Entities.Common.BusinessEntities.WellSite;
using Sesi.SanjelData.Entities.Common.Model.Scheduling;
using CallSheet = Sanjel.BusinessEntities.CallSheets.CallSheet;
using UnitSection = Sanjel.BusinessEntities.Sections.Common.UnitSection;
using ThirdPartyUnitSection = Sanjel.Common.EService.Sections.Common.ThirdPartyUnitSection;
//using BlendSection = Sanjel.BusinessEntities.Sections.Common.BlendSection;

namespace eServiceOnline.Gateway
{
    public interface IeServiceOnlineGateway
    {
        #region Retrieve Data from MDD generated backend

        List<SanjelCrew> GetCrewList();
        List<Employee> GetEmployeeList();
        List<Employee> GetEmployeesByServicePoint(int servicePointId);
        List<TruckUnit> GetTruckUnitList();
        List<TruckUnit> GetTruckUnitsByServicePoint(int servicePointId);
        CrewType GetCrewTypeById(int id);
        int CreateCrew(SanjelCrew crew);
        SanjelCrew GetCrewById(int id,bool withChildren=false);
        TruckUnit GetTruckUnitById(int id);
        Employee GetEmployeeById(int id);
        int CreateTruckUnitSection(SanjelCrewTruckUnitSection truckUnitSection);
        int CreateWorkerSection(SanjelCrewWorkerSection workerSection);
        List<SanjelCrewTruckUnitSection> GetTruckUnitSectionsByCrew(int crewId);
        List<SanjelCrew> GetSanjelCrewsWithChildren(List<SanjelCrew> sanjelCrews);
        List<SanjelCrewWorkerSection> GetWorkerSectionsByCrew(int crewId);
        SanjelCrewTruckUnitSection GetTruckUnitSectionByUnitAndCrew(int truckUnitId, int crewId);
        SanjelCrewWorkerSection GetWorkerSectionByWorkerAndCrew(int workerId, int crewId);
        int DeleteTruckUnitSection(SanjelCrewTruckUnitSection truckUnitSection);
        int DeleteWorkerSection(SanjelCrewWorkerSection workerSection);
        List<CrewPosition> GetCrewPositions();
        CrewPosition GetCrewPositionById(int id);
        List<ServicePoint> GetServicePoints();
        List<CrewType> GetCrewTypes();
        int UpdateCrew(SanjelCrew crew, bool isUpdateChildren = false);
        int UpdateWorker(Employee employee);
        List<RigJobSanjelCrewSection> GetRigJobCrewSectionsByCrew(int crewId);
        RigJobSanjelCrewSection GetRigJobCrewSectionsByProductHaul(int productHaulId);
        int CreateRigJobCrewSection(RigJobSanjelCrewSection rigJobCrewSection);
        int CreateRigJobThirdPartyBulkerCrewSection(RigJobThirdPartyBulkerCrewSection rigJobThirdPartyBulkerCrewSection);
        List<RigJobSanjelCrewSection> GetRigJobCrewSectionByRigJob(int rigJobId);
        int UpdateRigJobCrewSection(RigJobSanjelCrewSection rigJobCrewSection);

        RigJobSanjelCrewSection GetRigJobCrewSection(int rigJobId, int crewId);
        RigJobSanjelCrewSection GetRigJobCrewSection(int rigJobId, int crewId, int jobCrewSectionStatusId);
        int DeleteRigJobThirdPartyBulkerCrewSection(int id);
        RigJobThirdPartyBulkerCrewSection GetRigJobThirdPartyBulkerCrewSection(int rigJobId, int crewId);
        SanjelCrewSchedule GetCrewScheduleByJobCrewSection(int jobCrewSectionId);
        int DeleteRigJobCrewSection(RigJobSanjelCrewSection rigJobCrewSection);
        ThirdPartyBulkerCrewSchedule GetThirdPartyBulkerCrewSchedule(int rigJobThirdPartyBulkerCrewSectionId);
        RigJobSanjelCrewSection GetRigJobCrewSectionById(int id);
        ServicePoint GetServicePointById(int id);
        int DeleteCrew(int id);
        List<ThirdPartyBulkerCrew> GetThirdPartyBulkerCrews();
        ThirdPartyBulkerCrew GetThirdPartyBulkerCrewById(int id);
        int UpdateThirdPartyBulkerCrew(ThirdPartyBulkerCrew thirdPartyBulkerCrew);
        Collection<ContractorCompany> GetAllContractorCompanies();
        int CreateThirdPartyBulkerCrew(ThirdPartyBulkerCrew thirdPartyBulkerCrew);
        ContractorCompany GetContractorCompanyById(int id);
        WitsBoxInformation GetWitsBoxInformationByWitsBoxId(int witsBoxId);
        WitsBox GetWitsBoxById(int id);
        int CreateWitsBoxInformation(WitsBoxInformation witsBoxInformation);
        int UpdateWitsBoxInformation(WitsBoxInformation witsBoxInformation);
        List<WitsBoxInformation> GetWitsBoxInformationByServicePointAndEquipmentStatus(int workingServicePointId, EquipmentStatus equipmentStatus);

        #endregion Retrieve Data from MDD generated backend

        #region Retrieve Data from Sanjel MicroService

        List<ProductLoadSection> GetProductLoadSectionsByProductLoadId(int productHaulLoadId);
        RigJob GetRigJobById(int id);
        int UpdateRigJob(RigJob rigJob);
        List<RigJob> GetRigJobsByRigId(int rigId);
        List<RigJob> GetRigJobs();
        List<RigJob> GetRigJobsWithReferenceData();
        List<RigJob> GetListedRigJobByRigId(int rigId);
        List<RigJob> GetUnListedRigJobByRigId(int rigId);
        List<RigJob> GetRigJobsByServicePoints(Collection<int> servicePointIds);
        List<RigJob> GetRigJobsWithChildren(List<RigJob> rigJobs);
        RigJob GetRigJobByCallSheetNumber(int callSheetNumber);
        List<RigJob> GetRigJobsByProgramId(string programId);

        ProductHaulLoad GetProductHaulLoadById(int id, bool withiChildren = false);
        Collection<ThirdPartyUnitSection> GetThirdPartyUnitSectionsByProductHaul(ProductHaul productHaul);
        ThirdPartyUnitSection UpdateThirdPartyUnitSection(ThirdPartyUnitSection thirdPartyUnitSection);
        Collection<UnitSection> GetUnitSectionsByProductHaul(ProductHaul productHaul);
//        UnitSection UpdateUnitSection(UnitSection unitSection);
        List<ProductLoadSection> GetProductLoadSectionsByProductLoad(ProductHaulLoad productHaulLoad);
        int DeleteProductLoadSection(ProductLoadSection productLoadSection);
        int DeleteProductHaulLoad(ProductHaulLoad productHaulLoad, bool withChildren = false);
        List<ProductHaulLoad> GetProductHaulLoadsByProductHual(ProductHaul productHaul);
        int DeleteProductHaul(ProductHaul productHaul, bool withChildren = false);
        int DeleteUnitSection(UnitSection unitSection);
        int DeleteThirdPartyUnitSection(ThirdPartyUnitSection thirdPartyUnitSection);
        Collection<UnitSection> GetUnitSectionsByCallSheetId(int callSheetId);
        Rig GetRigById(int id);
        Rig GetRigByName(string name);
        List<Rig> GetBulkPlants();
        int UpdateRig(Rig rig);
        int CreateRig(Rig rig);
        int CreateRigJob(RigJob rigJob);
        List<ProductHaulLoad> GetProductHaulLoadsByCallSheetNumber(int callSheetNumber);
        List<ProductHaulLoad> GetProductHaulLoads(Pager pager = null);
        int DeleteRigJob(RigJob rigJob);

        int UpdateProductHaulLoad(ProductHaulLoad productHaulLoad, bool withChildren = false);

        int UpdateProductHaul(ProductHaul productHaul, bool withChildren = true);
        List<ProductHaul> GetProductHauls();
        int CreateProductHaulLoad(ProductHaulLoad productHaulLoad,bool flag=false);
        int CreateProductLoadSection(ProductLoadSection productLoadSection);
        ThirdPartyUnitSection GetThirdPartyUnitSectionByCallSheetAndProductHaul(Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch.CallSheet callSheet, ProductHaul productHaul);
        UnitSection GetUnitSectionByCallSheetAndProductHaul(Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch.CallSheet callSheet, ProductHaul productHaul);
        int CreateProductHaul(ProductHaul productHaul, bool withChildren = true);
        ClientCompany GetClientCompanyById(int id);
        int UpdateClientCompany(ClientCompany clientCompany);
        ClientConsultant GetClientConsultantById(int id);
        int UpdateClientConsultant(ClientConsultant clientConsultant);
        Job GetJobByUniqueId(string jobUniqueId);

        #endregion

        #region Schedule

        List<SanjelCrewSchedule> GetCrewSchedules();
        List<SanjelCrewSchedule> GetCrewSchedules(Collection<int> servicePoints, DateTime startTime, DateTime endTime);
        List<UnitSchedule> GetUnitSchedules();
        List<UnitSchedule> GetUnitSchedules(Collection<int> servicePoints, DateTime startTime, DateTime endTime);
        List<UnitSchedule> GetUnitSchedulesByTruckUnit(int truckUnitId);
        List<WorkerSchedule> GetWorkerSchedules();
        List<WorkerSchedule> GetWorkerSchedules(Collection<int> servicePoints, DateTime startTime, DateTime endTime);
        List<WorkerSchedule> GetWorkerSchedulesByWorker(int workerId);

        SanjelCrewSchedule GetCrewScheduleById(int id);
        UnitSchedule GetUnitScheduleById(int id);
        WorkerSchedule GetWorkerScheduleById(int id);

//        int DeleteCrewSchedule(int id);
        int DeletethirdPartyBulkerCrewSchedule(int id);
        int DeleteUnitSchedule(int id);
        int DeleteWorkSchedule(int id);

//        int UpdateSanjelCrewSchedule(SanjelCrewSchedule crewSchedule);
        int UpdateWorkerSchedule(WorkerSchedule workerSchedule);

        int UpdateUnitSchedule(UnitSchedule unitSchedule);

        int InsertCrewSchedule(SanjelCrewSchedule crewSchedule, bool isUpdateChildren=false);
        int InsertThirdPartyBulkerCrewSchedule(ThirdPartyBulkerCrewSchedule crewSchedule);
        int InsertWorkerSchedule(WorkerSchedule workerSchedule);
        int InsertUnitSchedule(UnitSchedule unitSchedule);
//        UnitSchedule GetUnitScheduleByCrewScheduleAndTruckUnit(int crewScheduleId, int truckUnitId);
//        WorkerSchedule GetWorkerScheduleByCrewScheduleAndWorker(int crewScheduleId, int workerId);

//        List<UnitScheduleType> UnitScheduleTypes();
//        List<WorkerScheduleType> WorkerScheduleTypes();
//        List<UnitSchedule> GetUnitSchedulesByCrewSchedule(int crewScheduleId);
//        List<WorkerSchedule> GetWorkerSchedulesByCrewSchedule(int crewScheduleId);

        UnitSection CreateUnitSection(UnitSection unitSection);
        Collection<UnitSection> GetUnitSectionsByCrewId(int crewId);
        ThirdPartyUnitSection CreateThirdPartyUnitSection(ThirdPartyUnitSection thirdPartyUnitSection);
        ProductHaul GetProductHaulById(int id);
        CallSheet GetCallSheetById(int id);
        SanjelCrewTruckUnitSection GetCrewTruckUnitSectionById(int id);
        SanjelCrewWorkerSection GetCrewWorkerSectionById(int id);

        #endregion


        #region bin
        List<BinInformation> GetBinInformationsByRigId(int rigId);
        BinInformation GetBinInformationByBinIdAndPodIndex(int binId,int podIndex);
        BinInformation GetBinInformationById(int Id);
        List<BinInformation> GetBinInformations();
        //        List<RigBinSection> GetRigBinSectionsByCallSheetId(int callSheetId);
        BinInformation CreateBinInformation(BinInformation binAssignment);
        int UpdateBinInformation(BinInformation binInformation);
        int DeleteBinInformation(BinInformation binInformation);
        BinNote GetBinNoteByBinAndPodIndex(Bin bin, int podIndex);
        int UpdateBinNote(BinNote binNote);
        int CreateBinNote(BinNote binNote);

        #endregion

        void CreateConsultant(ClientConsultant clientConsultant);
        Collection<ClientConsultant> GetClientConsultantCollection();
        int DeleteClientConsultant(ClientConsultant clientConsultant);
        ClientConsultant GetConsultantById(int id);
        List<ClientCompany>  GetClientCompanyInfo();
        List<DrillingCompany> GetDrillingCompanyInfo();
        DrillingCompany GetDrillingCompanyById(int id);
        List<ShiftType> GetWorkShiftInfo();
        ShiftType GetWorkShiftById(int id);
        List<ProductHaulLoad> GetProductHaulLoadByBlendSectionId(int blendSectionId);
        List<Rig> GetRigByDrillingCompanyId(int drillingCompanyId);
        Bin GetBinById(int binId);
        BlendAdditiveMeasureUnit GetBlendAdditiveMeasureUnitbyName(string name);
        RigSizeType GetRigSizeTypeById(int id);
        RigSize GetRigSizeById(int id);
        ThreadType GetThreadTypeById(int id);

        List<SanjelCrewSchedule> GetCrewScheduleByCrewId(int crewId);
        List<SanjelCrewSchedule> GetSanjelCrewSchedulesBySanjelCrew(int sanjelCrewId);
        List<UnitSchedule> GetUnitScheduleByCrewScheduleId(int id);
        List<WorkerSchedule> GetWorkerScheduleByCrewScheduleId(int id);
        SanjelCrewSchedule GetSanjelCrewSchedulesByRigJobSanjelCrewSection(
            RigJobSanjelCrewSection rigJobSanjelCrewSection);

        ThirdPartyBulkerCrewSchedule GetThirdPartyCrewScheduleById(int id);
        RigJobThirdPartyBulkerCrewSection GetRigJobThirdPartyBulkerCrewSectionById(int id);
        RigJobThirdPartyBulkerCrewSection GetRigJobThirdPartyBulkerCrewSectionsByProductHual(int productHaulId);

        List<ProductHaul> GetProductHaulByCrewScheduleId(int crewScheduleId);

        List<ThirdPartyBulkerCrewSchedule> GetThirdPartyCrewScheduleByThirdPartyCrewId(int thirdPartyCrewId);
        ThirdPartyBulkerCrewSchedule GetThirdPartyCrewScheduleByRigJobThirdPartyBulkerCrewSection(RigJobThirdPartyBulkerCrewSection rigJobThirdPartyBulkerCrewSection);
        int UpdateThirdPartyCrewSchedule(ThirdPartyBulkerCrewSchedule thirdPartyBulkerCrewSchedule);
        int UpdateRigJobThirdPartyCrewSection(RigJobThirdPartyBulkerCrewSection rigJobThirdPartyBulkerCrewSection);

        int DeleteThirdPartyBulkerCrew(int crewId);

        int UpdateSanjelCrewNote(SanjelCrewNote note);
        SanjelCrewNote GetSanjelCrewNote(int id);
        SanjelCrewNote GetSanjelCrewNoteBySanjelCrew(int sanjelCrewId);
        int CreateSanjelCrewNote(SanjelCrewNote note);

        ThirdPartyBulkerCrewNote GeThirdPartyBulkerCrewNoteByThirdPartyBulkerCrew(int thirdPartyBulkerCrewId);
        int CreateThirdPartyBulkerCrewNote(ThirdPartyBulkerCrewNote thirdPartyBulkerCrewNote);

        int UpdateThirdPartyBulkerCrewNote(ThirdPartyBulkerCrewNote thirdPartyBulkerCrewNote);

        EmployeeNote GetEmployeeNoteByEmployee(int employeeId);

        int CreateEmployeeNote(EmployeeNote employeeNote);
        int UpdateEmployeeNote(EmployeeNote employeeNote);


        TruckUnitNote GetTruckUnitNoteByTruckUnit(int truckUnitId);
        int CreateTruckUnitNote(TruckUnitNote truckUnitNote);
        int UpdateTruckUnitNote(TruckUnitNote truckUnitNote);

        EmployeeProfile GetEmployeeProfileByEmployee(int employeeId);
        int CreateEmployeeProfile(EmployeeProfile employeeProfile);
        int UpdateEmployeeProfile(EmployeeProfile employeeProfile);
        List<RigJobThirdPartyBulkerCrewSection> GetRigJobThirdPartyBulkerCrewSectionByRigJob(int rigJobId);
        RigJobThirdPartyBulkerCrewSection GetRigJobThirdPartyBulkerCrewSectionByProductHaul(int productHaulId);

        Sesi.SanjelData.Entities.BusinessEntities.BaseEntites.BlendSection GetBlendSectionByBlendSectionId(int blendSectionId);

        List<PodLoad> GetPodLoadsByProductHaul(int productHaulId);
       

        void CreateShippingLoadSheet(ShippingLoadSheet shippingLoadSheet);

        void CreatePodLoad(PodLoad podLoad); 
        void UpdatePodLoad(PodLoad podLoad);
        #region District Notes

        int CreateServicePointNote(ServicePointNote servicePointNote);
        ServicePointNote GetServicePointNoteByServicePointId(int servicePointId);
        int UpdateServicePointNote(ServicePointNote servicePointNote);

        #endregion


        #region MyRegion

        List<ProductHaulLoad> GetProductHaulLoadByBinId(int binId, int podIndex);


        #endregion


        #region PlugLoadingHead 

        PlugLoadingHeadInformation GetPlugLoadingHeadInformationByPlugLoadingHeadId(int plugLoadingHeadId);
        List<PlugLoadingHead> GetAllPlugLoadingHeads();
        List<PlugLoadingHeadInformation> GetPlugLoadingHeadInformations();

        int CreatePlugLoadingHeadInformation(PlugLoadingHeadInformation plugLoadingHeadInformation);
        int UpdatePlugLoadingHeadInformation(PlugLoadingHeadInformation plugLoadingHeadInformation);
        PlugLoadingHead GetPlugLoadingHeadById(int id);

        List<PlugLoadingHeadInformation> GetPlugLoadingHeadInformationByRigJobId(int rigJobId);
        #endregion

        #region maniflod

        ManifoldInformation GetManifoldInformationByManifoldId(int manifoldId);
        List<Manifold> GetAllManifolds();
        List<ManifoldInformation> GetAllManifoldInformations();

        int CreateManifoldInformation(ManifoldInformation manifoldInformation);
        int UpdateManifoldInformation(ManifoldInformation manifoldInformation);

        Manifold GetManifoldById(int id);

        #endregion

        #region TopDriveAdaptor

        TopDrivceAdaptorInformation GetTopDrivceAdaptorInformationByTopDrivceAdaptorId(int topDrivceAdaptorId);
        List<TopDriveAdaptor> GetAllTopDriveAdaptors();
        List<TopDrivceAdaptorInformation> GetAllTopDrivceAdaptorInformations();

        int CreateTopDrivceAdaptorInformation(TopDrivceAdaptorInformation topDrivceAdaptorInformation);
        int UpdateTopDrivceAdaptorInformation(TopDrivceAdaptorInformation topDrivceAdaptorInformation);

        TopDriveAdaptor GetTopDriveAdaptorById(int id);
        #endregion

        #region PlugLoadingHeadSub

        PlugLoadingHeadSubInformation GetPlugLoadingHeadSubInformationByPlugLoadingHeadSubId(int plugLoadingHeadSubId);
        List<PlugLoadingHeadSub> GetAllPlugLoadingHeadSubs();

        List<PlugLoadingHeadSubInformation> GetAllPlugLoadingHeadSubInformations();

        int CreatePlugLoadingHeadSubInformation(PlugLoadingHeadSubInformation plugLoadingHeadSubInformation);
        int UpdatePlugLoadingHeadSubInformation(PlugLoadingHeadSubInformation plugLoadingHeadSubInformation);

        PlugLoadingHeadSub GetPlugLoadingHeadSubById(int id);

        PlugLoadingHeadInformation GetPlugLoadingHeadInformationByManifoldId(int manifoldId);

        #endregion


        #region Nubin

        List<Nubbin> GetAllNubbins();
        List<NubbinInformation> GetAllNubbinInformation();

        NubbinInformation GetNubbinInformationByNubbinId(int nubbinId);
        List<NubbinInformation> GetNubbinInformationByServicePointAndEquipmentStatus(int workingServicePointId, EquipmentStatus equipmentStatus);
        Nubbin GetNubbinById(int id);
        int CreateNubbinInformation(NubbinInformation nubbinInformation);
        int UpdateNubbinInformation(NubbinInformation nubbinInformation);

        List<NubbinInformation> GetNubbinInformationByRigJobId(int rigJobId);
        #endregion


        #region Swedge

        List<Swedge> GetAllSwedge();
        List<SwedgeInformation> GetSwedgeInformation();
        SwedgeInformation GetSwedgeInformationBySwedgeId(int swedgeId);
        List<SwedgeInformation> GetSwedgeInformationByServicePointAndEquipmentStatus(int workingServicePointId, EquipmentStatus equipmentStatus);
        Swedge GetSwedgeById(int id);
        int CreateSwedgeInformation(SwedgeInformation swedgeInformation);
        int UpdateSwedgeInformation(SwedgeInformation swedgeInformation);
        List<SwedgeInformation> GetSwedgeInformationByRigJobId(int rigJobId);
        #endregion


        #region WitsBox

        List<WitsBox> GetWitsBox();
        List<WitsBoxInformation> GetWitsBoxInformation();
        List<WitsBoxInformation> GetWitsBoxInformationByRigJobId(int rigJobId);
        #endregion

        #region Blend Chemical

        int CreateBlendChemical(BlendChemical blendChemical);
        int CreateProduct(Product product);
        int CreateBlendRecipe(BlendRecipe blendRecipe);
        int CreateBlendFluidType(BlendFluidType blendFluidType);
        int CreateAdditiveType(AdditiveType additiveType);




        #endregion BlendChemical

        #region BulkPlant API

        int CreateBlendLog(BlendLog blendLog);
        BlendLog GetBlendLogByProductHaulLoadId(int productHaulLoadId);


        #endregion BulkPlant API

    }

}
