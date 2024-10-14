using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule;
using Sesi.SanjelData.Entities.Common.BusinessEntities.HumanResources;
using Sesi.SanjelData.Entities.Common.BusinessEntities.WellSite;

namespace eServiceOnline.BusinessProcess.Interface
{
    public interface IRigBoardProcess
    {
        bool UpdateRigJobStatusToComplete(int id);
        bool UpdateRigJobStatusToCancel(int id,string notes);
        void CreateOrUpdateUnitSectionsByProductHaul(ProductHaul productHaul);
        bool UpdateRigStatus(int rigId, RigStatus newStatus);
        Collection<RigJob> GetAllRigJobInformation(int pageNumber, int pageSize, Collection<int> servicePointIds, Collection<int> rigTypes, Collection<int> jobLifeStatuses, bool isShowJobAlert, out int count);
        bool UnassignBinToRig(int binId, int callSheetId, int rigJobId);
        int DeleteRigJob(RigJob rigJob);
        bool ActivateARig(int rigId);
        void CreateOrUpdateThirdPartyUnitSections(ProductHaul producthaul);
        void UpdateClientConsultant(bool isUpdateRigJob, ClientConsultant clientConsultant);
        void UpdateRigInfo(Rig rig);
        void UpdateProductHaulAndLoadsOnLocation(int productHaulId, DateTime onLocationTime);
        List<ProductHaul> GetExistingProductHaulCollection();
        void UpdateProductHaulAndCreateLoad(ProductHaulLoad productHaulLoad, int productHaulId, int callSheetId);
        void CreateUnitSectionOrThirdPartyUnitSection(ProductHaul productHaul, ProductHaulLoad productHaulLoad, int callSheetId);
        void UpdateProductHaulAndHaulLoads(ProductHaul productHaul, bool isGoWithCrew, DateTime expectedOnLocationTime);

//        void UpdateProductHaulAndHaulLoad(ProductHaul productHaul, ProductHaulLoad productHaulLoad, int callSheetId);
        void UpdateHaulLoadAndUnitSections(ProductHaul productHaul, ProductHaulLoad productHaulLoad, int callSheetId);
        void CreateProductHaulAndUpdateHaulLoad(ProductHaul productHaul, ProductHaulLoad productHaulLoad, int callSheetId);
        void UpdateProductHaulLoadOnLocation(ProductHaulLoad productHaulLoad, DateTime onLocationTime);
        void DeleteHaulLoadAndUnitSections(int productHaulLoadId,int callSheetId);
        void CreateProductLoadAndUpdateUnitSection(ProductHaulLoad productHaulLoad,int productHaulId,int callSheetId);
        void UpdateCompanyShortName(int rigJobId, int clientCompanyId, string companyShortName);
        void UpdateJobDateTime(int rigJobId, DateTime jobDateTime);
    }
}