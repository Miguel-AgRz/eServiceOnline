using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using MetaShare.Common.Core.Entities;
using MetaShare.Common.ServiceModel.Services;
using Sanjel.BusinessEntities.Jobs;
using Sanjel.BusinessEntities.Programs;
using Sanjel.EService.MicroService.Interfaces;
using Sanjel.Services.Interfaces;
using Sesi.SanjelData.Entities.BusinessEntities.BulkPlant;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Crew;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ResourceSchedule;
using Sesi.SanjelData.Entities.BusinessEntities.WellSite;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment;
using Sesi.SanjelData.Entities.Common.BusinessEntities.HumanResources;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Operation;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Organization;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Products;
using Sesi.SanjelData.Entities.Common.BusinessEntities.WellSite;
using Sesi.SanjelData.Entities.Common.Model.Scheduling;
using Sesi.SanjelData.Services.Interfaces.BusinessEntities.BulkPlant;
using Sesi.SanjelData.Services.Interfaces.BusinessEntities.Operation.Crew;
using Sesi.SanjelData.Services.Interfaces.BusinessEntities.Operation.Dispatch;
using Sesi.SanjelData.Services.Interfaces.BusinessEntities.Operation.Schedule.ProductSchedule;
using Sesi.SanjelData.Services.Interfaces.BusinessEntities.Operation.Schedule.ResourceSchedule;
using Sesi.SanjelData.Services.Interfaces.BusinessEntities.WellSite;
using Sesi.SanjelData.Services.Interfaces.Common.BusinessEntities.Equipment;
using Sesi.SanjelData.Services.Interfaces.Common.BusinessEntities.HumanResources;
using Sesi.SanjelData.Services.Interfaces.Common.BusinessEntities.Operation;
using Sesi.SanjelData.Services.Interfaces.Common.BusinessEntities.Organization;
using Sesi.SanjelData.Services.Interfaces.Common.BusinessEntities.Products;
using Sesi.SanjelData.Services.Interfaces.Common.BusinessEntities.WellSite;
using CallSheet = Sanjel.BusinessEntities.CallSheets.CallSheet;
using UnitSection=Sanjel.BusinessEntities.Sections.Common.UnitSection;
using ThirdPartyUnitSection=Sanjel.Common.EService.Sections.Common.ThirdPartyUnitSection;

using Sesi.SanjelData.Entities.Common.BusinessEntities.Inventory;
using Sesi.SanjelData.Services.Interfaces.Common.BusinessEntities.Inventory;
using Sanjel.BusinessEntities.ServiceReports;
using Sesi.SanjelData.Daos.Interfaces.BusinessEntities.Sales;
using Sesi.SanjelData.Entities.BusinessEntities.BaseEntites;
using Sesi.SanjelData.Entities.BusinessEntities.Sales;
using Sesi.SanjelData.Services.Common.BusinessEntities.Organization;
using Sesi.SanjelData.Services.Interfaces.BusinessEntities.BaseEntites;
using Sesi.SanjelData.Services.Interfaces.BusinessEntities.Sales;
using Sesi.SanjelData.Services.Interfaces.Common.Model.Scheduling;
using BlendSection = Sesi.SanjelData.Entities.BusinessEntities.BaseEntites.BlendSection;
using ICallSheetService = Sesi.SanjelData.Services.Interfaces.BusinessEntities.Operation.Dispatch.ICallSheetService;

//using BlendSection = Sanjel.BusinessEntities.Sections.Common.BlendSection;

namespace eServiceOnline.Gateway
{
    public class eServiceOnlineGateway: IeServiceOnlineGateway
    {
        private static eServiceOnlineGateway instance = new eServiceOnlineGateway();

        public static eServiceOnlineGateway Instance
        {
            get { return instance; }
        }

        #region Retrieve Data from MDD generated backend

        public List<SanjelCrewSchedule> GetCrewScheduleByCrewId(int crewId)
        {
            ISanjelCrewScheduleService crewScheduleService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ISanjelCrewScheduleService>();
            return crewScheduleService.SelectBy(new SanjelCrewSchedule { SanjelCrew = new SanjelCrew {Id = crewId}}, new List<string>() {"SanjelCrewId"});         
        }

        public List<SanjelCrewSchedule> GetSanjelCrewSchedulesBySanjelCrew(int sanjelCrewId)
        {
            ISanjelCrewScheduleService sanjelCrewScheduleService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ISanjelCrewScheduleService>();
            if (sanjelCrewScheduleService == null) throw new Exception("sanjelCrewScheduleService must be registered in service factory");

            return sanjelCrewScheduleService.SelectBy(new SanjelCrewSchedule { SanjelCrew = new SanjelCrew { Id = sanjelCrewId } }, new List<string>() { "SanjelCrewId" });
        }
        public SanjelCrewSchedule GetSanjelCrewScheduleById(int id, bool isAggregatedChildren=false)
        {
            ISanjelCrewScheduleService sanjelCrewScheduleService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ISanjelCrewScheduleService>();
            if (sanjelCrewScheduleService == null) throw new Exception("sanjelCrewScheduleService must be registered in service factory");

            return sanjelCrewScheduleService.SelectById(new SanjelCrewSchedule(){Id = id},isAggregatedChildren);
        }

        public List<UnitSchedule> GetUnitScheduleByCrewScheduleId(int id)
        {
            IUnitScheduleService unitScheduleService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IUnitScheduleService>();
            return unitScheduleService.SelectBy(new UnitSchedule(), p => p.SanjelCrewSchedule.Id == id);
       
        }

        public List<WorkerSchedule> GetWorkerScheduleByCrewScheduleId(int id)
        {
            IWorkerScheduleService workerScheduleService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IWorkerScheduleService>();
            return workerScheduleService.SelectBy(new WorkerSchedule(), p => p.SanjelCrewSchedule.Id == id);

        }

        public SanjelCrewSchedule GetSanjelCrewSchedulesByRigJobSanjelCrewSection(
            RigJobSanjelCrewSection rigJobSanjelCrewSection)
        {
            ISanjelCrewScheduleService sanjelCrewScheduleService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ISanjelCrewScheduleService>();
            if (sanjelCrewScheduleService == null) throw new Exception("sanjelCrewScheduleService must be registered in service factory");

//            return sanjelCrewScheduleService.SelectBy(new SanjelCrewSchedule { RigJobSanjelCrewSection = rigJobSanjelCrewSection } , new List<string>() { "RigJobSanjelCrewSectionid" }).FirstOrDefault();
            return sanjelCrewScheduleService
                .SelectByJoin(p => p.RigJobSanjelCrewSection.Id == rigJobSanjelCrewSection.Id,
                    schedule => schedule.WorkerSchedule != null && schedule.UnitSchedule != null).FirstOrDefault();
        }

        public ThirdPartyBulkerCrewSchedule GetThirdPartyCrewScheduleById(int id)
        { 
            IThirdPartyBulkerCrewScheduleService thirdPartyBulkerCrewScheduleService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IThirdPartyBulkerCrewScheduleService>();
            if (thirdPartyBulkerCrewScheduleService == null) throw new Exception("thirdPartyBulkerCrewScheduleService must be registered in service factory");
            return thirdPartyBulkerCrewScheduleService.SelectById(new ThirdPartyBulkerCrewSchedule {Id = id});
        }

        public RigJobThirdPartyBulkerCrewSection GetRigJobThirdPartyBulkerCrewSectionById(int id)
        {
            IRigJobThirdPartyBulkerCrewSectionService rigJobThirdPartyBulkerCrewSectionService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigJobThirdPartyBulkerCrewSectionService>();
            if (rigJobThirdPartyBulkerCrewSectionService == null) throw new Exception("rigJobThirdPartyBulkerCrewSectionService must be registered in service factory");
            return rigJobThirdPartyBulkerCrewSectionService.SelectById(new RigJobThirdPartyBulkerCrewSection {Id = id});
        }

        public RigJobThirdPartyBulkerCrewSection GetRigJobThirdPartyBulkerCrewSectionsByProductHual(int productHaulId)
        {
            IRigJobThirdPartyBulkerCrewSectionService rigJobThirdPartyBulkerCrewSectionService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigJobThirdPartyBulkerCrewSectionService>();
            if (rigJobThirdPartyBulkerCrewSectionService == null) throw new Exception("rigJobThirdPartyBulkerCrewSectionService must be registered in service factory");

            return rigJobThirdPartyBulkerCrewSectionService.SelectBy(new RigJobThirdPartyBulkerCrewSection { ProductHaul = new ProductHaul { Id = productHaulId } }, new List<string> { "ProductHaulId" }).FirstOrDefault();
        }

        public List<ProductHaul> GetProductHaulByCrewScheduleId(int crewScheduleId)
        {
            IProductHaulService productHaulService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IProductHaulService>();
            return productHaulService.SelectBy(new ProductHaul {Schedule = new Schedule {Id = crewScheduleId}}, new List<string>() {"ScheduleId"});
        }

        public List<ThirdPartyBulkerCrewSchedule> GetThirdPartyCrewScheduleByThirdPartyCrewId(int thirdPartyCrewId)
        {
           IThirdPartyBulkerCrewScheduleService thirdPartyBulkerCrewScheduleService= MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IThirdPartyBulkerCrewScheduleService>();
//          return thirdPartyBulkerCrewScheduleService.SelectBy(new ThirdPartyBulkerCrewSchedule {ThirdPartyBulkerCrew = new ThirdPartyBulkerCrew() {Id = thirdPartyCrewId}}, new List<string>(){ "ThirdPartyBulkerCrewId" });       
          return thirdPartyBulkerCrewScheduleService.SelectByJoin(p=>p.ThirdPartyBulkerCrew.Id == thirdPartyCrewId, x=>x.RigJobThirdPartyBulkerCrewSection != null);       
        }
        public ThirdPartyBulkerCrewSchedule GetThirdPartyCrewScheduleByRigJobThirdPartyBulkerCrewSection(RigJobThirdPartyBulkerCrewSection rigJobThirdPartyBulkerCrewSection)
        {
            IThirdPartyBulkerCrewScheduleService thirdPartyBulkerCrewScheduleService= MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IThirdPartyBulkerCrewScheduleService>();
            return thirdPartyBulkerCrewScheduleService.SelectBy(new ThirdPartyBulkerCrewSchedule {RigJobThirdPartyBulkerCrewSection = rigJobThirdPartyBulkerCrewSection}, new List<string>(){ "RigJobThirdPartyBulkerCrewSectionid" }).FirstOrDefault();       
        }

        public int UpdateThirdPartyCrewSchedule(ThirdPartyBulkerCrewSchedule thirdPartyBulkerCrewSchedule)
        {
           IThirdPartyBulkerCrewScheduleService thirdPartyBulkerCrewScheduleService= MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IThirdPartyBulkerCrewScheduleService>();
           return thirdPartyBulkerCrewScheduleService.Update(thirdPartyBulkerCrewSchedule);
        }

        public int UpdateRigJobThirdPartyCrewSection(RigJobThirdPartyBulkerCrewSection rigJobThirdPartyBulkerCrewSection)
        {
           IRigJobThirdPartyBulkerCrewSectionService rigJobThirdPartyBulkerCrewSectionService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigJobThirdPartyBulkerCrewSectionService>();
           return rigJobThirdPartyBulkerCrewSectionService.Update(rigJobThirdPartyBulkerCrewSection);
        }

        public int DeleteThirdPartyBulkerCrew(int crewId)
        {
           IThirdPartyBulkerCrewService thirdPartyBulkerCrewService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IThirdPartyBulkerCrewService>();
           return thirdPartyBulkerCrewService.Delete(new ThirdPartyBulkerCrew {Id = crewId});
        }

        public int UpdateSanjelCrewNote(SanjelCrewNote note)
        {
          ISanjelCrewNoteService sanjelCrewNoteService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ISanjelCrewNoteService>();
          return sanjelCrewNoteService.Update(note);
        }

        public SanjelCrewNote GetSanjelCrewNote(int id)
        {
            ISanjelCrewNoteService sanjelCrewNoteService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ISanjelCrewNoteService>();
            return sanjelCrewNoteService.SelectById(new SanjelCrewNote {Id = id});
        }

        public SanjelCrewNote GetSanjelCrewNoteBySanjelCrew(int sanjelCrewId)
        {
            ISanjelCrewNoteService sanjelCrewNoteService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ISanjelCrewNoteService>();
            List<SanjelCrewNote> sanjelCrewNotes= sanjelCrewNoteService.SelectBy(new SanjelCrewNote { SanjelCrew = new SanjelCrew{Id = sanjelCrewId } },new List<string>(){ "SanjelCrewId" });
            return sanjelCrewNotes.FirstOrDefault();
        }

        public int CreateSanjelCrewNote(SanjelCrewNote note)
        {
            ISanjelCrewNoteService sanjelCrewNoteService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ISanjelCrewNoteService>();
            return sanjelCrewNoteService.Insert(note);
        }

        public ThirdPartyBulkerCrewNote GeThirdPartyBulkerCrewNoteByThirdPartyBulkerCrew(int thirdPartyBulkerCrewId)
        {
           IThirdPartyBulkerCrewNoteService thirdPartyBulkerCrewNoteService= MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IThirdPartyBulkerCrewNoteService>();
           return thirdPartyBulkerCrewNoteService.SelectBy(new ThirdPartyBulkerCrewNote {ThirdPartyBulkerCrew = new ThirdPartyBulkerCrew {Id = thirdPartyBulkerCrewId}}, new List<string>() { "ThirdPartyBulkerCrewId" }).FirstOrDefault();
        }

        public int CreateThirdPartyBulkerCrewNote(ThirdPartyBulkerCrewNote thirdPartyBulkerCrewNote)
        {
            IThirdPartyBulkerCrewNoteService thirdPartyBulkerCrewNoteService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IThirdPartyBulkerCrewNoteService>();
            return thirdPartyBulkerCrewNoteService.Insert(thirdPartyBulkerCrewNote);
        }

        public int UpdateThirdPartyBulkerCrewNote(ThirdPartyBulkerCrewNote thirdPartyBulkerCrewNote)
        {
            IThirdPartyBulkerCrewNoteService thirdPartyBulkerCrewNoteService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IThirdPartyBulkerCrewNoteService>();
            return thirdPartyBulkerCrewNoteService.Update(thirdPartyBulkerCrewNote);
        }

        public EmployeeNote GetEmployeeNoteByEmployee(int employeeId)
        {
           IEmployeeNoteService employeeNoteService= MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IEmployeeNoteService>();
           return employeeNoteService.SelectBy(new EmployeeNote {Employee = new Employee {Id = employeeId}}, new List<string>() {"EmployeeId"}).FirstOrDefault();
        }

        public int CreateEmployeeNote(EmployeeNote employeeNote)
        {
            IEmployeeNoteService employeeNoteService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IEmployeeNoteService>();
            return employeeNoteService.Insert(employeeNote);
        }

        public int UpdateEmployeeNote(EmployeeNote employeeNote)
        {
            IEmployeeNoteService employeeNoteService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IEmployeeNoteService>();
            return employeeNoteService.Update(employeeNote);
        }

        public TruckUnitNote GetTruckUnitNoteByTruckUnit(int truckUnitId)
        {
          ITruckUnitNoteService truckUnitNoteService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ITruckUnitNoteService>();
          return truckUnitNoteService.SelectBy(new TruckUnitNote {TruckUnit = new TruckUnit {Id = truckUnitId}}, new List<string>() {"TruckUnitId"}).FirstOrDefault();
        }

        public int CreateTruckUnitNote(TruckUnitNote truckUnitNote)
        {
            ITruckUnitNoteService truckUnitNoteService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ITruckUnitNoteService>();
            return truckUnitNoteService.Insert(truckUnitNote);
        }

        public int UpdateTruckUnitNote(TruckUnitNote truckUnitNote)
        {
            ITruckUnitNoteService truckUnitNoteService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ITruckUnitNoteService>();
            return truckUnitNoteService.Update(truckUnitNote);
        }

        public EmployeeProfile GetEmployeeProfileByEmployee(int employeeId)
        {
           IEmployeeProfileService employeeProfileService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IEmployeeProfileService>();
           if (employeeProfileService == null) throw new Exception("employeeProfileService must be registered in service factory");
           return employeeProfileService.SelectBy(new EmployeeProfile {Employee = new Employee {Id = employeeId}}, new List<string>() { "EmployeeId" }).FirstOrDefault();
        }

        public int CreateEmployeeProfile(EmployeeProfile employeeProfile)
        {
            IEmployeeProfileService employeeProfileService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IEmployeeProfileService>();
            if (employeeProfileService == null) throw new Exception("employeeProfileService must be registered in service factory");
            return employeeProfileService.Insert(employeeProfile);
        }

        public int UpdateEmployeeProfile(EmployeeProfile employeeProfile)
        {
            IEmployeeProfileService employeeProfileService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IEmployeeProfileService>();
            if (employeeProfileService == null) throw new Exception("employeeProfileService must be registered in service factory");
            return employeeProfileService.Update(employeeProfile);
        }

        public List<RigJobThirdPartyBulkerCrewSection> GetRigJobThirdPartyBulkerCrewSectionByRigJob(int rigJobId)
        {
           IRigJobThirdPartyBulkerCrewSectionService rigJobThirdPartyBulkerCrewSectionService= MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigJobThirdPartyBulkerCrewSectionService>();
           return rigJobThirdPartyBulkerCrewSectionService.SelectBy(new RigJobThirdPartyBulkerCrewSection {RigJob = new RigJob {Id = rigJobId}}, new List<string>() { "RigJobId" });
        }

        public RigJobThirdPartyBulkerCrewSection GetRigJobThirdPartyBulkerCrewSectionByProductHaul(int productHaulId)
        {
            IRigJobThirdPartyBulkerCrewSectionService rigJobThirdPartyBulkerCrewSectionService= MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigJobThirdPartyBulkerCrewSectionService>();
            return rigJobThirdPartyBulkerCrewSectionService.SelectBy(new RigJobThirdPartyBulkerCrewSection {ProductHaul = new ProductHaul {Id = productHaulId}}, new List<string>() { "ProductHaulId" }).FirstOrDefault();
        }

        public Sesi.SanjelData.Entities.BusinessEntities.BaseEntites.BlendSection GetBlendSectionByBlendSectionId(int blendSectionId)
        {
            DateTime startDateTime = DateTime.Now;

            if (blendSectionId <= 0) return null;
            ICallSheetBlendSectionService callSheetBlendSectionService= MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ICallSheetBlendSectionService>();
            CallSheetBlendSection callSheetBlendSection = null;
            if (callSheetBlendSectionService == null) return null;
            try
            {
                callSheetBlendSection = callSheetBlendSectionService.SelectByJoin(p=>p.Id==blendSectionId, blend=>blend.CallSheetBlendAdditiveSections != null).FirstOrDefault();
            }
            catch (Exception ex)
            {
                ;
            }

            Debug.WriteLine("\t\tGetBlendSectionByBlendSectionId - {0,21}", DateTime.Now.Subtract(startDateTime));

            Sesi.SanjelData.Entities.BusinessEntities.BaseEntites.BlendSection blendSection = new BlendSection();
            if (callSheetBlendSection != null)
            {
                blendSection = (BlendSection)callSheetBlendSection;
                if (blendSection.BlendAdditiveSections == null)
                    blendSection.BlendAdditiveSections =
                        new List<Sesi.SanjelData.Entities.BusinessEntities.BaseEntites.BlendAdditiveSection>();

                foreach (var callSheetBlendAdditiveSection in callSheetBlendSection.CallSheetBlendAdditiveSections)
                {
                    blendSection.BlendAdditiveSections.Add(
                        (Sesi.SanjelData.Entities.BusinessEntities.BaseEntites.BlendAdditiveSection)
                        callSheetBlendAdditiveSection);
                }

            }

            return blendSection;
        }

        public int CreateServicePointNote(ServicePointNote servicePointNote)
        {
            IServicePointNoteService servicePointNoteService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IServicePointNoteService>();
            if (servicePointNoteService == null) throw new Exception("employeeProfileService must be registered in service factory");
            return servicePointNoteService.Insert(servicePointNote);
        }

        public ServicePointNote GetServicePointNoteByServicePointId(int servicePointId)
        {
            IServicePointNoteService servicePointNoteService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IServicePointNoteService>();
            
            return servicePointNoteService.SelectBy(new ServicePointNote{ServicePoint = new ServicePoint{Id = servicePointId}},new List<string>(){ "ServicePointId" } ).FirstOrDefault();
        }

        public int UpdateServicePointNote(ServicePointNote servicePointNote)
        {
            IServicePointNoteService servicePointNoteService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IServicePointNoteService>();
            if (servicePointNoteService == null) throw new Exception("employeeProfileService must be registered in service factory");
            return servicePointNoteService.Update(servicePointNote);
        }

        public List<ProductHaulLoad> GetProductHaulLoadByBinId(int binId, int podIndex)
        {
            IProductHaulLoadService productHaulLoadService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IProductHaulLoadService>();
            if (productHaulLoadService==null) throw new Exception("productHaulLoadService must be registered in service factory");
            {

                return productHaulLoadService.SelectBy(new ProductHaulLoad {Bin = new Bin {Id = binId }, PodIndex =podIndex}, new List<string>() {"BinId", "PodIndex"});
            }
        }
        public List<ProductHaulLoad> GetNotOnLocationProductHaulLoadByBinId(int binId, int podIndex)
        {
            IProductHaulLoadService productHaulLoadService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IProductHaulLoadService>();
            if (productHaulLoadService==null) throw new Exception("productHaulLoadService must be registered in service factory");
            {

                return productHaulLoadService.SelectBy(new ProductHaulLoad (), productHaulLoad=>productHaulLoad.Bin.Id == binId && productHaulLoad.PodIndex == podIndex && productHaulLoad.ProductHaulLoadLifeStatus != ProductHaulLoadStatus.OnLocation);
            }
        }
        public List<ProductHaulLoad> GetProductHaulLoadByBinId(int binId, Collection<int> productHaulLoadLifeStatuses)
        {
            IProductHaulLoadService productHaulLoadService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IProductHaulLoadService>();
            if (productHaulLoadService==null) throw new Exception("productHaulLoadService must be registered in service factory");
            {

                return productHaulLoadService.SelectBy(new ProductHaulLoad (), productHaulLoad => productHaulLoad.Bin.Id == binId && productHaulLoadLifeStatuses.Contains((int)productHaulLoad.ProductHaulLoadLifeStatus));
            }
        }

        public PlugLoadingHeadInformation GetPlugLoadingHeadInformationByPlugLoadingHeadId(int plugLoadingHeadId)
        {
            IPlugLoadingHeadInformationService plugLoadingHeadInformationService= MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IPlugLoadingHeadInformationService>();
            if (plugLoadingHeadInformationService == null) throw new Exception("plugLoadingHeadInformationService must be registered in service factory");
            return plugLoadingHeadInformationService.SelectBy(new PlugLoadingHeadInformation{PlugLoadingHead = new PlugLoadingHead{Id = plugLoadingHeadId}},new List<string>(){ "PlugLoadingHeadId" } ).FirstOrDefault();
        }

        public List<PlugLoadingHead> GetAllPlugLoadingHeads()
        {
          IPlugLoadingHeadService plugLoadingHeadService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IPlugLoadingHeadService>();
          if (plugLoadingHeadService == null) throw new Exception("productHaulLoadService must be registered in service factory");
          return plugLoadingHeadService.SelectAll();
        }

        public List<PlugLoadingHeadInformation> GetPlugLoadingHeadInformations()
        {
            IPlugLoadingHeadInformationService plugLoadingHeadInformationService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IPlugLoadingHeadInformationService>();
            if (plugLoadingHeadInformationService == null) throw new Exception("plugLoadingHeadInformationService must be registered in service factory");
            return plugLoadingHeadInformationService.SelectAll();
        }

        public int CreatePlugLoadingHeadInformation(PlugLoadingHeadInformation plugLoadingHeadInformation)
        {
            IPlugLoadingHeadInformationService plugLoadingHeadInformationService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IPlugLoadingHeadInformationService>();
            if (plugLoadingHeadInformationService == null) throw new Exception("plugLoadingHeadInformationService must be registered in service factory");
            return plugLoadingHeadInformationService.Insert(plugLoadingHeadInformation,false);
        }

        public int UpdatePlugLoadingHeadInformation(PlugLoadingHeadInformation plugLoadingHeadInformation)
        {
            IPlugLoadingHeadInformationService plugLoadingHeadInformationService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IPlugLoadingHeadInformationService>();
            if (plugLoadingHeadInformationService == null) throw new Exception("plugLoadingHeadInformationService must be registered in service factory");
            return plugLoadingHeadInformationService.Update(plugLoadingHeadInformation);
        }

        public PlugLoadingHead GetPlugLoadingHeadById(int id)
        {
            IPlugLoadingHeadService plugLoadingHeadService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IPlugLoadingHeadService>();
            if (plugLoadingHeadService == null) throw new Exception("productHaulLoadService must be registered in service factory");
            return plugLoadingHeadService.SelectById(new PlugLoadingHead {Id = id});
        }

        public List<PlugLoadingHeadInformation> GetPlugLoadingHeadInformationByRigJobId(int rigJobId)
        {
           IPlugLoadingHeadInformationService plugLoadingHeadInformationService= MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IPlugLoadingHeadInformationService>();
           if (plugLoadingHeadInformationService == null) throw new Exception("plugLoadingHeadInformationService must be registered in service factory");
           return plugLoadingHeadInformationService.SelectByRigJob(rigJobId);
        }

        public ManifoldInformation GetManifoldInformationByManifoldId(int manifoldId)
        {
           IManifoldInformationService manifoldInformationService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IManifoldInformationService>();
           if (manifoldInformationService == null) throw new Exception("manifoldInformationService must be registered in service factory");
           return manifoldInformationService.SelectBy(new ManifoldInformation(){Manifold = new Manifold{Id = manifoldId}},new List<string>(){ "ManifoldId" } ).FirstOrDefault();
        }

        public List<Manifold> GetAllManifolds()
        {
            IManifoldService manifoldService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IManifoldService>();
            if (manifoldService == null) throw new Exception("manifoldService must be registered in service factory");
            return manifoldService.SelectAll();
        }

        public List<ManifoldInformation> GetAllManifoldInformations()
        {
            IManifoldInformationService manifoldInformationService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IManifoldInformationService>();
            if (manifoldInformationService == null) throw new Exception("manifoldInformationService must be registered in service factory");
            return manifoldInformationService.SelectAll();
        }

        public int CreateManifoldInformation(ManifoldInformation manifoldInformation)
        {
            IManifoldInformationService manifoldInformationService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IManifoldInformationService>();
            if (manifoldInformationService == null) throw new Exception("manifoldInformationService must be registered in service factory");
            return manifoldInformationService.Insert(manifoldInformation,false);
        }

        public int UpdateManifoldInformation(ManifoldInformation manifoldInformation)
        {
            IManifoldInformationService manifoldInformationService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IManifoldInformationService>();
            if (manifoldInformationService == null) throw new Exception("manifoldInformationService must be registered in service factory");
            return manifoldInformationService.Update(manifoldInformation);
        }

        public Manifold GetManifoldById(int id)
        {
            IManifoldService manifoldService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IManifoldService>();
            if (manifoldService == null) throw new Exception("manifoldService must be registered in service factory");
            return manifoldService.SelectById(new Manifold{Id = id});
        }

        public TopDrivceAdaptorInformation GetTopDrivceAdaptorInformationByTopDrivceAdaptorId(int topDrivceAdaptorId)
        {
            ITopDrivceAdaptorInformationService topDrivceAdaptorInformationService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ITopDrivceAdaptorInformationService>();
            if (topDrivceAdaptorInformationService == null) throw new Exception("topDrivceAdaptorInformationService must be registered in service factory");
            return topDrivceAdaptorInformationService.SelectBy(new TopDrivceAdaptorInformation(){TopDriveAdaptor = new TopDriveAdaptor(){Id =topDrivceAdaptorId}},new List<string>(){ "TopDriveAdaptorId" } ).FirstOrDefault();
        }

        public List<TopDriveAdaptor> GetAllTopDriveAdaptors()
        {
            ITopDriveAdaptorService topDriveAdaptorService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ITopDriveAdaptorService>();
            if (topDriveAdaptorService == null) throw new Exception("topDriveAdaptorService must be registered in service factory");
            return topDriveAdaptorService.SelectAll();
        }

        public List<TopDrivceAdaptorInformation> GetAllTopDrivceAdaptorInformations()
        {
            ITopDrivceAdaptorInformationService topDrivceAdaptorInformationService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ITopDrivceAdaptorInformationService>();
            if (topDrivceAdaptorInformationService == null) throw new Exception("topDrivceAdaptorInformationService must be registered in service factory");
            return topDrivceAdaptorInformationService.SelectAll();
        }

        public int CreateTopDrivceAdaptorInformation(TopDrivceAdaptorInformation topDrivceAdaptorInformation)
        {
            ITopDrivceAdaptorInformationService topDrivceAdaptorInformationService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ITopDrivceAdaptorInformationService>();
            if (topDrivceAdaptorInformationService == null) throw new Exception("topDrivceAdaptorInformationService must be registered in service factory");
            return topDrivceAdaptorInformationService.Insert(topDrivceAdaptorInformation, false);
        }

        public int UpdateTopDrivceAdaptorInformation(TopDrivceAdaptorInformation topDrivceAdaptorInformation)
        {
            ITopDrivceAdaptorInformationService topDrivceAdaptorInformationService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ITopDrivceAdaptorInformationService>();
            if (topDrivceAdaptorInformationService == null) throw new Exception("topDrivceAdaptorInformationService must be registered in service factory");
            return topDrivceAdaptorInformationService.Update(topDrivceAdaptorInformation);
        }

        public TopDriveAdaptor GetTopDriveAdaptorById(int id)
        {
            ITopDriveAdaptorService topDriveAdaptorService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ITopDriveAdaptorService>();
            if (topDriveAdaptorService == null) throw new Exception("topDriveAdaptorService must be registered in service factory");
            return topDriveAdaptorService.SelectById(new TopDriveAdaptor {Id = id});
        }

        public PlugLoadingHeadSubInformation GetPlugLoadingHeadSubInformationByPlugLoadingHeadSubId(int plugLoadingHeadSubId)
        {
            IPlugLoadingHeadSubInformationService plugLoadingHeadSubInformationService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IPlugLoadingHeadSubInformationService>();
            if (plugLoadingHeadSubInformationService == null) throw new Exception("plugLoadingHeadSubInformationService must be registered in service factory");
            return plugLoadingHeadSubInformationService.SelectBy(new PlugLoadingHeadSubInformation{PlugLoadingHeadSub = new PlugLoadingHeadSub{Id = plugLoadingHeadSubId}},new List<string>(){ "PlugLoadingHeadSubId" }).FirstOrDefault();
        }

        public List<PlugLoadingHeadSub> GetAllPlugLoadingHeadSubs()
        {
            IPlugLoadingHeadSubService plugLoadingHeadSubService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IPlugLoadingHeadSubService>();
            if (plugLoadingHeadSubService == null) throw new Exception("plugLoadingHeadSubService must be registered in service factory");
            return plugLoadingHeadSubService.SelectAll();
        }

        public List<PlugLoadingHeadSubInformation> GetAllPlugLoadingHeadSubInformations()
        {
            IPlugLoadingHeadSubInformationService plugLoadingHeadSubInformationService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IPlugLoadingHeadSubInformationService>();
            if (plugLoadingHeadSubInformationService == null) throw new Exception("plugLoadingHeadSubInformationService must be registered in service factory");
            return plugLoadingHeadSubInformationService.SelectAll();
        }

        public int CreatePlugLoadingHeadSubInformation(PlugLoadingHeadSubInformation plugLoadingHeadSubInformation)
        {
            IPlugLoadingHeadSubInformationService plugLoadingHeadSubInformationService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IPlugLoadingHeadSubInformationService>();
            if (plugLoadingHeadSubInformationService == null) throw new Exception("plugLoadingHeadSubInformationService must be registered in service factory");
            return plugLoadingHeadSubInformationService.Insert(plugLoadingHeadSubInformation, false);
        }

        public int UpdatePlugLoadingHeadSubInformation(PlugLoadingHeadSubInformation plugLoadingHeadSubInformation)
        {
            IPlugLoadingHeadSubInformationService plugLoadingHeadSubInformationService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IPlugLoadingHeadSubInformationService>();
            if (plugLoadingHeadSubInformationService == null) throw new Exception("plugLoadingHeadSubInformationService must be registered in service factory");
            return plugLoadingHeadSubInformationService.Update(plugLoadingHeadSubInformation);
        }

        public PlugLoadingHeadSub GetPlugLoadingHeadSubById(int id)
        {
            IPlugLoadingHeadSubService plugLoadingHeadSubService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IPlugLoadingHeadSubService>();
            if (plugLoadingHeadSubService == null) throw new Exception("plugLoadingHeadSubService must be registered in service factory");
            return plugLoadingHeadSubService.SelectById(new PlugLoadingHeadSub {Id = id});
        }

        public PlugLoadingHeadInformation GetPlugLoadingHeadInformationByManifoldId(int manifoldId)
        {
            IPlugLoadingHeadInformationService plugLoadingHeadInformationService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IPlugLoadingHeadInformationService>();
            if (plugLoadingHeadInformationService == null) throw new Exception("plugLoadingHeadInformationService must be registered in service factory");
            return plugLoadingHeadInformationService.SelectBy(new PlugLoadingHeadInformation { Manifold = new Manifold() { Id = manifoldId } }, new List<string>() { "ManifoldId" }).FirstOrDefault();
        }

        public List<Nubbin> GetAllNubbins()
        {
            INubbinService nubbinService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<INubbinService>();
            if (nubbinService == null) throw new Exception("nubbinService must be registered in service factory");
            return nubbinService.SelectAll();
        }

        public List<NubbinInformation> GetAllNubbinInformation()
        {
            INubbinInformationService nubbinInformationService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<INubbinInformationService>();
            if (nubbinInformationService == null) throw new Exception("nubbinInformationService must be registered in service factory");
            return nubbinInformationService.SelectAll();
        }

        public NubbinInformation GetNubbinInformationByNubbinId(int nubbinId)
        {
            INubbinInformationService nubbinInformationService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<INubbinInformationService>();
            if (nubbinInformationService == null) throw new Exception("nubbinInformationService must be registered in service factory");
            return nubbinInformationService.SelectByNubbin(nubbinId).FirstOrDefault();
        }

        public List<NubbinInformation> GetNubbinInformationByServicePointAndEquipmentStatus(int workingServicePointId, EquipmentStatus equipmentStatus)
        {
            INubbinInformationService nubbinInformationService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<INubbinInformationService>();
            if (nubbinInformationService == null) throw new Exception("nubbinInformationService must be registered in service factory");

            return nubbinInformationService.SelectByEquipmentStatusWorkingServicePoint((int)equipmentStatus, workingServicePointId);
        }

        public Nubbin GetNubbinById(int id)
        {
            INubbinService nubbinService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<INubbinService>();
            if (nubbinService == null) throw new Exception("nubbinService must be registered in service factory");

            return nubbinService.SelectById(new Nubbin{Id = id});
        }

        public int CreateNubbinInformation(NubbinInformation nubbinInformation)
        {
            INubbinInformationService nubbinInformationService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<INubbinInformationService>();
            if (nubbinInformationService == null) throw new Exception("nubbinInformationService must be registered in service factory");

            return nubbinInformationService.Insert(nubbinInformation);
        }

        public int UpdateNubbinInformation(NubbinInformation nubbinInformation)
        {
            INubbinInformationService nubbinInformationService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<INubbinInformationService>();
            if (nubbinInformationService == null) throw new Exception("nubbinInformationService must be registered in service factory");

            return nubbinInformationService.Update(nubbinInformation, false);
        }

        public List<NubbinInformation> GetNubbinInformationByRigJobId(int rigJobId)
        {
            INubbinInformationService nubbinInformationService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<INubbinInformationService>();
            if (nubbinInformationService == null) throw new Exception("nubbinInformationService must be registered in service factory");
            return nubbinInformationService.SelectByRigJob(rigJobId);
        }

        public List<Swedge> GetAllSwedge()
        {
           ISwedgeService swedgeService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ISwedgeService>();
           if (swedgeService == null) throw new Exception("swedgeService must be registered in service factory");
           return swedgeService.SelectAll();
        }

        public List<SwedgeInformation> GetSwedgeInformation()
        {
            ISwedgeInformationService swedgeInformationService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ISwedgeInformationService>();
            if (swedgeInformationService == null) throw new Exception("swedgeInformationService must be registered in service factory");
            return swedgeInformationService.SelectAll();
        }

        public SwedgeInformation GetSwedgeInformationBySwedgeId(int swedgeId)
        {
            ISwedgeInformationService swedgeInformationService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ISwedgeInformationService>();
            if (swedgeInformationService == null) throw new Exception("swedgeInformationService must be registered in service factory");
            return swedgeInformationService.SelectBySwedge(swedgeId).FirstOrDefault();
        }

        public List<SwedgeInformation> GetSwedgeInformationByServicePointAndEquipmentStatus(int workingServicePointId, EquipmentStatus equipmentStatus)
        {
            ISwedgeInformationService swedgeInformationService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ISwedgeInformationService>();
            if (swedgeInformationService == null) throw new Exception("swedgeInformationService must be registered in service factory");

            return swedgeInformationService.SelectByEquipmentStatusWorkingServicePoint(workingServicePointId, (int)equipmentStatus);
        }

        public Swedge GetSwedgeById(int id)
        {
            ISwedgeService swedgeService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ISwedgeService>();
            if (swedgeService == null) throw new Exception("swedgeService must be registered in service factory");

            return swedgeService.SelectById(new Swedge{Id = id});
        }

        public int CreateSwedgeInformation(SwedgeInformation swedgeInformation)
        {
            ISwedgeInformationService swedgeInformationService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ISwedgeInformationService>();
            if (swedgeInformationService == null) throw new Exception("swedgeInformationService must be registered in service factory");

            return swedgeInformationService.Insert(swedgeInformation);
        }

        public int UpdateSwedgeInformation(SwedgeInformation swedgeInformation)
        {
            ISwedgeInformationService swedgeInformationService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ISwedgeInformationService>();
            if (swedgeInformationService == null) throw new Exception("swedgeInformationService must be registered in service factory");

            return swedgeInformationService.Update(swedgeInformation, false);
        }

        public List<SwedgeInformation> GetSwedgeInformationByRigJobId(int rigJobId)
        {
            ISwedgeInformationService swedgeInformationService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ISwedgeInformationService>();
            if (swedgeInformationService == null) throw new Exception("swedgeInformationService must be registered in service factory");
            return swedgeInformationService.SelectByRigJob(rigJobId);
        }

        public List<WitsBox> GetWitsBox()
        {
          IWitsBoxService witsBoxService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IWitsBoxService>();
          if (witsBoxService == null) throw new Exception("witsBoxService must be registered in service factory");
          return witsBoxService.SelectAll();
        }

        public List<WitsBoxInformation> GetWitsBoxInformation()
        {
            IWitsBoxInformationService witsBoxInformationService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IWitsBoxInformationService>();
            if (witsBoxInformationService == null) throw new Exception("witsBoxInformationService must be registered in service factory");
            return witsBoxInformationService.SelectAll();
        }

        public List<WitsBoxInformation> GetWitsBoxInformationByRigJobId(int rigJobId)
        {
            IWitsBoxInformationService witsBoxInformationService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IWitsBoxInformationService>();
            if (witsBoxInformationService == null) throw new Exception("witsBoxInformationService must be registered in service factory");
            return witsBoxInformationService.SelectByRigJob(rigJobId);
        }

        public int CreateBlendChemical(BlendChemical blendChemical)
        {
            IBlendChemicalService blendChemicalService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IBlendChemicalService>();
            if (blendChemicalService == null) throw new Exception("blendChemicalService must be registered in service factory");

            return blendChemicalService.Insert(blendChemical);
        }

        public int CreateProduct(Product product)
        {
            IProductService productService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IProductService>();
            if (productService == null) throw new Exception("productService must be registered in service factory");

            return productService.Insert(product);
        }

        public int CreateBlendRecipe(BlendRecipe blendRecipe)
        {
            IBlendRecipeService blendRecipeService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IBlendRecipeService>();
            if (blendRecipeService == null) throw new Exception("BlendRecipeService must be registered in service factory");

            return blendRecipeService.Insert(blendRecipe);
        }

        public int CreateBlendFluidType(BlendFluidType blendFluidType)
        {
            IBlendFluidTypeService blendFluidTypeService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IBlendFluidTypeService>();
            if (blendFluidTypeService == null) throw new Exception("blendFluidTypeService must be registered in service factory");

            return blendFluidTypeService.Insert(blendFluidType);
        }

        public int CreateAdditiveType(AdditiveType additiveType)
        {
            IAdditiveTypeService additiveTypeService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IAdditiveTypeService>();
            if (additiveTypeService == null) throw new Exception("additiveTypeService must be registered in service factory");

            return additiveTypeService.Insert(additiveType);
        }

        public int CreateBlendLog(BlendLog blendLog)
        {
            IBlendLogService blendLogService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IBlendLogService>();
            if (blendLogService == null) throw new Exception("blendLogService must be registered in service factory");

            return blendLogService.Insert(blendLog,true);
        }

        public BlendLog GetBlendLogByProductHaulLoadId(int productHaulLoadId)
        {
            IBlendLogService blendLogService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IBlendLogService>();
            if (blendLogService == null) throw new Exception("blendLogService must be registered in service factory");

            var blendLogs = blendLogService.SelectBy(new BlendLog(){ProductHaulLoadId = productHaulLoadId}, new List<string>(){"ProductHaulLoadId"}).FirstOrDefault();

            return blendLogService.SelectById(blendLogs, true);
        }
        public int UpdateBlendLog(BlendLog blendLog)
        {
            IBlendLogService blendLogService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IBlendLogService>();
            if (blendLogService == null) throw new Exception("blendLogService must be registered in service factory");

            return blendLogService.Update(blendLog);
        }

        public int CreateBlendCut(BlendCut blendCut)
        {
            IBlendCutService blendLogService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IBlendCutService>();
            if (blendLogService == null) throw new Exception("blendCutService must be registered in service factory");

            return blendLogService.Insert(blendCut,true);
        }

        public int UpdateBlendCut(BlendCut blendCut)
        {
            IBlendCutService blendLogService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IBlendCutService>();
            if (blendLogService == null) throw new Exception("blendCutService must be registered in service factory");

            return blendLogService.Update(blendCut,true);
        }

        public BlendCut GetBlendCutByProductHaulLoadAndBlendCutSequenceNumber(in int blendCutProductHaulLoadId, in int blendCutSequenceNumber)
        {
            IBlendCutService blendLogService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IBlendCutService>();
            if (blendLogService == null) throw new Exception("blendCutService must be registered in service factory");

            var blendCut = blendLogService.SelectBy(new BlendCut(){ProductHaulLoadId = blendCutProductHaulLoadId, SequenceNumber = blendCutSequenceNumber}, new List<string>(){ "ProductHaulLoadId", "SequenceNumber" } ).FirstOrDefault();
            if (blendCut != null)
                return blendLogService.SelectById(blendCut, true);
            else
                return null;
        }
        public WitsBoxInformation GetWitsBoxInformationByWitsBoxId(int witsBoxId)
        {
            IWitsBoxInformationService witsBoxInformationService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IWitsBoxInformationService>();
            if (witsBoxInformationService == null) throw new Exception("witsBoxInformationService must be registered in service factory");
            return witsBoxInformationService.SelectByWitsBox(witsBoxId).FirstOrDefault();
        }


        public List<SanjelCrew> GetCrewList()
        {
            ISanjelCrewService crewService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ISanjelCrewService>();
            if (crewService == null) throw new Exception("crewService must be registered in service factory");

            return crewService.SelectAll();
        }

        public List<SanjelCrew> GetCrewList(Collection<int> servicePointIds)
        {
            ISanjelCrewService crewService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ISanjelCrewService>();
            if (crewService == null) throw new Exception("crewService must be registered in service factory");
            if (servicePointIds.Count == 0)
                return crewService.SelectAll();
            else
                return crewService.SelectBy(new SanjelCrew(), p=>servicePointIds.Contains(p.HomeServicePoint.Id) || servicePointIds.Contains((p.WorkingServicePoint.Id)));
        }
        public List<Employee> GetEmployeeList()
        {
            IEmployeeService employeeService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IEmployeeService>();
            if (employeeService == null) throw new Exception("employeeService must be registered in service factory");
            List<Employee> employees = employeeService.SelectAll().OrderBy(s=>s.LastName).ToList();

            return employees;
        }

        public List<Employee> GetEmployeesByServicePoint(int servicePointId)
        {

            IEmployeeService employeeService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IEmployeeService>();
            if (employeeService == null) throw new Exception("employeeService must be registered in service factory");
            List<Employee> employees = employeeService.SelectBy(new Employee { ServicePoint = new ServicePoint { Id = servicePointId } }, new List<string> { "ServicePointId" });

            return employees;
        }

        public List<TruckUnit> GetTruckUnitList()
        {
            ITruckUnitService truckUnitService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ITruckUnitService>();
            if (truckUnitService == null) throw new Exception("truckUnitService must be registered in service factory");
            List<TruckUnit> truckUnits = truckUnitService.SelectAll().OrderBy(s=>s.UnitNumber).ToList();

            return truckUnits;
        }

        public List<TruckUnit> GetTruckUnitsByServicePoint(int servicePointId)
        {
            ITruckUnitService truckUnitService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ITruckUnitService>();
            if (truckUnitService == null) throw new Exception("truckUnitService must be registered in service factory");
            List<TruckUnit> truckUnits = truckUnitService.SelectBy(new TruckUnit { ServicePoint = new ServicePoint { Id = servicePointId } }, new List<string> { "ServicePointId" });

            return truckUnits;
        }

        public CrewType GetCrewTypeById(int id)
        {
            ICrewTypeService crewTypeService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ICrewTypeService>();
            if (crewTypeService == null) throw new Exception("crewTypeService must be registered in service factory");
            CrewType crewType = crewTypeService.SelectById(new CrewType { Id = id });

            return crewType;
        }

        public int CreateCrew(SanjelCrew crew)
        {
            ISanjelCrewService crewService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ISanjelCrewService>();
            if (crewService == null) throw new Exception("crewService must be registered in service factory");

            return crewService.Insert(crew, true);
        }

        public int CreateBulkerCrewLog(BulkerCrewLog bulkerCrewLog)
        {
            IBulkerCrewLogService bulkerCrewLogService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IBulkerCrewLogService>();
            if (bulkerCrewLogService == null) throw new Exception("bulkerCrewLogService must be registered in service factory");

            return bulkerCrewLogService.Insert(bulkerCrewLog, true);
        }

        public int DeleteBulkerCrewLog(BulkerCrewLog bulkerCrewLog)
        {
            IBulkerCrewLogService bulkerCrewLogService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IBulkerCrewLogService>();
            if (bulkerCrewLogService == null) throw new Exception("bulkerCrewLogService must be registered in service factory");

            return bulkerCrewLogService.Delete(bulkerCrewLog, true);
        }


        public SanjelCrew GetCrewById(int id,bool withChildren=false)
        {
            ISanjelCrewService crewService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ISanjelCrewService>();
            if (crewService == null) throw new Exception("crewService must be registered in service factory");
            SanjelCrew crew = crewService.SelectById(new SanjelCrew { Id = id }, withChildren);
            return crew;
        }

        public SanjelCrew GetSanjelCrewById(int id)
        {
            ISanjelCrewService crewService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ISanjelCrewService>();
            if (crewService == null) throw new Exception("crewService must be registered in service factory");
            SanjelCrew crew = crewService.SelectByJoin(p=>p.Id==id, q=>q.SanjelCrewTruckUnitSection != null && q.SanjelCrewWorkerSection != null).FirstOrDefault();
            return crew;
        }

        public TruckUnit GetTruckUnitById(int id)
        {
            ITruckUnitService truckUnitService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ITruckUnitService>();
            if (truckUnitService == null) throw new Exception("truckUnitService must be registered in service factory");
            TruckUnit truckUnit = truckUnitService.SelectById(new TruckUnit { Id = id });

            return truckUnit;
        }

        public Employee GetEmployeeById(int id)
        {
            IEmployeeService employeeService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IEmployeeService>();
            if (employeeService == null) throw new Exception("employeeService must be registered in service factory");
            Employee employee = employeeService.SelectById(new Employee { Id = id });

            return employee;
        }

        public int CreateTruckUnitSection(SanjelCrewTruckUnitSection truckUnitSection)
        {
            ISanjelCrewTruckUnitSectionService truckUnitSectionService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ISanjelCrewTruckUnitSectionService>();
            if (truckUnitSectionService == null) throw new Exception("truckUnitSectionService must be registered in service factory");

            return truckUnitSectionService.Insert(truckUnitSection, false);
        }

        public int CreateWorkerSection(SanjelCrewWorkerSection workerSection)
        {
            ISanjelCrewWorkerSectionService workerSectionService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ISanjelCrewWorkerSectionService>();
            if (workerSectionService == null) throw new Exception("workerSectionService must be registered in service factory");

            return workerSectionService.Insert(workerSection, false);
        }

        public List<SanjelCrewTruckUnitSection> GetTruckUnitSectionsByCrew(int crewId)
        {
            ISanjelCrewTruckUnitSectionService truckUnitSectionService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ISanjelCrewTruckUnitSectionService>();
            if (truckUnitSectionService == null) throw new Exception("truckUnitSectionService must be registered in service factory");
            List<SanjelCrewTruckUnitSection> truckUnitSections = truckUnitSectionService.SelectBy(new SanjelCrewTruckUnitSection { SanjelCrew = new SanjelCrew{Id = crewId}}, new List<string>{"SanjelCrewId"});

            return truckUnitSections;
        }

        public List<SanjelCrew> GetSanjelCrewsWithChildren(List<SanjelCrew> sanjelCrews)
        {
            ISanjelCrewService crewService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ISanjelCrewService>();
            if (crewService == null) throw new Exception("crewService must be registered in service factory");

            return crewService.SelectWithChildren(sanjelCrews);
        }

        public List<SanjelCrewWorkerSection> GetWorkerSectionsByCrew(int crewId)
        {
            ISanjelCrewWorkerSectionService workerSectionService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ISanjelCrewWorkerSectionService>();
            if (workerSectionService == null) throw new Exception("workerSectionService must be registered in service factory");
            List<SanjelCrewWorkerSection> crewWorkerSections = workerSectionService.SelectBy(new SanjelCrewWorkerSection { SanjelCrew = new SanjelCrew { Id = crewId } }, new List<string> { "SanjelCrewId" });

            return crewWorkerSections;
        }

        public SanjelCrewTruckUnitSection GetTruckUnitSectionByUnitAndCrew(int truckUnitId, int crewId)
        {
            ISanjelCrewTruckUnitSectionService truckUnitSectionService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ISanjelCrewTruckUnitSectionService>();
            if (truckUnitSectionService == null) throw new Exception("truckUnitSectionService must be registered in service factory");
            SanjelCrewTruckUnitSection truckUnitSection = truckUnitSectionService.SelectBy(new SanjelCrewTruckUnitSection { TruckUnit = new TruckUnit{Id = truckUnitId}, SanjelCrew = new SanjelCrew { Id = crewId } }, new List<string> { "TruckUnitId", "SanjelCrewId" }).FirstOrDefault();

            return truckUnitSection;
        }

        public int DeleteTruckUnitSection(SanjelCrewTruckUnitSection truckUnitSection)
        {
            ISanjelCrewTruckUnitSectionService truckUnitSectionService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ISanjelCrewTruckUnitSectionService>();
            if (truckUnitSectionService == null) throw new Exception("truckUnitSectionService must be registered in service factory");

            return truckUnitSectionService.Delete(truckUnitSection, false);
        }

        public SanjelCrewWorkerSection GetWorkerSectionByWorkerAndCrew(int workerId, int crewId)
        {
            ISanjelCrewWorkerSectionService workerSectionService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ISanjelCrewWorkerSectionService>();
            if (workerSectionService == null) throw new Exception("workerSectionService must be registered in service factory");
            SanjelCrewWorkerSection workerSection = workerSectionService.SelectBy(new SanjelCrewWorkerSection { Worker = new Employee { Id = workerId }, SanjelCrew = new SanjelCrew { Id = crewId } }, new List<string> { "WorkerId", "SanjelCrewId" }).FirstOrDefault();

            return workerSection;
        }

        public int DeleteWorkerSection(SanjelCrewWorkerSection workerSection)
        {
            ISanjelCrewWorkerSectionService workerSectionService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ISanjelCrewWorkerSectionService>();
            if (workerSectionService == null) throw new Exception("workerSectionService must be registered in service factory");

            return workerSectionService.Delete(workerSection, false);
        }

        public List<CrewPosition> GetCrewPositions()
        {
            ICrewPositionService crewPositionService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ICrewPositionService>();
            if (crewPositionService == null) throw new Exception("crewPositionService must be registered in service factory");

            return crewPositionService.SelectAll();
        }

        public CrewPosition GetCrewPositionById(int id)
        {
            ICrewPositionService crewPositionService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ICrewPositionService>();
            if (crewPositionService == null) throw new Exception("crewPositionService must be registered in service factory");

            return crewPositionService.SelectById(new CrewPosition{Id = id});
        }

        public List<ServicePoint> GetServicePoints()
        {
            IServicePointService servicePointService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IServicePointService>();
            if (servicePointService == null) throw new Exception("servicePointService must be registered in service factory");

            return servicePointService.SelectAll();
        }

        public List<CrewType> GetCrewTypes()
        {
            ICrewTypeService crewTypeService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ICrewTypeService>();
            if (crewTypeService == null) throw new Exception("crewTypeService must be registered in service factory");

            return crewTypeService.SelectAll();
        }

        public int UpdateCrew(SanjelCrew crew, bool isUpdateChildren = false)
        {
            ISanjelCrewService crewService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ISanjelCrewService>();
            if (crewService == null) throw new Exception("crewService must be registered in service factory");

            return crewService.Update(crew, isUpdateChildren);
        }

        public int UpdateWorker(Employee employee)
        {
            IEmployeeService employeeService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IEmployeeService>();
            if (employeeService==null) throw  new Exception("employeeService must be registered in service factory");
            return employeeService.Update(employee, false);
        }

        public List<RigJobSanjelCrewSection> GetRigJobCrewSectionsByCrew(int crewId)
        {
            IRigJobSanjelCrewSectionService rigJobCrewSectionService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigJobSanjelCrewSectionService>();
            if (rigJobCrewSectionService == null) throw new Exception("rigJobCrewSectionService must be registered in service factory");

            return rigJobCrewSectionService.SelectBy(new RigJobSanjelCrewSection { SanjelCrew = new SanjelCrew{Id = crewId}}, new List<string> { "SanjelCrewId" });
        }

        public RigJobSanjelCrewSection GetRigJobCrewSectionsByProductHaul(int productHaulId)
        {
            IRigJobSanjelCrewSectionService rigJobCrewSectionService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigJobSanjelCrewSectionService>();
            if (rigJobCrewSectionService == null) throw new Exception("rigJobCrewSectionService must be registered in service factory");

            return rigJobCrewSectionService.SelectBy(new RigJobSanjelCrewSection { ProductHaul = new ProductHaul { Id = productHaulId } }, new List<string> { "ProductHaulId" }).FirstOrDefault();
        }

        public int CreateRigJobCrewSection(RigJobSanjelCrewSection rigJobCrewSection)
        {
            IRigJobSanjelCrewSectionService rigJobCrewSectionService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigJobSanjelCrewSectionService>();
            if (rigJobCrewSectionService == null) throw new Exception("rigJobCrewSectionService must be registered in service factory");

            return rigJobCrewSectionService.Insert(rigJobCrewSection, false);
        }

        public int CreateRigJobThirdPartyBulkerCrewSection(RigJobThirdPartyBulkerCrewSection rigJobThirdPartyBulkerCrewSection)
        {
           IRigJobThirdPartyBulkerCrewSectionService rigJobThirdPartyBulkerCrewSectionService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigJobThirdPartyBulkerCrewSectionService>();
           if (rigJobThirdPartyBulkerCrewSectionService == null) throw new Exception("rigJobThirdPartyBulkerCrewSectionService must be registered in service factory");
           return rigJobThirdPartyBulkerCrewSectionService.Insert(rigJobThirdPartyBulkerCrewSection);
        }

        public List<RigJobSanjelCrewSection> GetRigJobCrewSectionByRigJob(int rigJobId)
        {
            IRigJobSanjelCrewSectionService rigJobCrewSectionService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigJobSanjelCrewSectionService>();
            if (rigJobCrewSectionService == null) throw new Exception("rigJobCrewSectionService must be registered in service factory");

            return rigJobCrewSectionService.SelectByRigJob(rigJobId);
        }

        public int UpdateRigJobCrewSection(RigJobSanjelCrewSection rigJobCrewSection)
        {
            IRigJobSanjelCrewSectionService rigJobCrewSectionService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigJobSanjelCrewSectionService>();
            if (rigJobCrewSectionService == null) throw new Exception("rigJobCrewSectionService must be registered in service factory");

            return rigJobCrewSectionService.Update(rigJobCrewSection, false);
        }

//        public RigJobCrewSectionStatus GetJobCrewSectionStatusById(int id)
//        {
//            IRigJobCrewSectionStatusService rigJobCrewSectionStatusService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigJobCrewSectionStatusService>();
//            if (rigJobCrewSectionStatusService == null) throw new Exception("rigJobCrewSectionStatusService must be registered in service factory");
//
//            return rigJobCrewSectionStatusService.SelectById(new RigJobCrewSectionStatus { Id = id});
//        }
        public RigJobSanjelCrewSection GetRigJobCrewSection(int rigJobId, int crewId)
        {
            IRigJobSanjelCrewSectionService rigJobCrewSectionService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigJobSanjelCrewSectionService>();
            if (rigJobCrewSectionService == null) throw new Exception("rigJobCrewSectionService must be registered in service factory");

            return rigJobCrewSectionService.SelectBy(new RigJobSanjelCrewSection { RigJob = new RigJob { Id = rigJobId }, SanjelCrew = new SanjelCrew { Id = crewId }}, new List<string> { "RigJobId", "SanjelCrewId"}).OrderByDescending(p => p.Id).FirstOrDefault();
        }
        public RigJobThirdPartyBulkerCrewSection GetRigJobThirdPartyBulkerCrewSection(int rigJobId, int crewId)
        {
            IRigJobThirdPartyBulkerCrewSectionService rigJobThirdPartyBulkerCrewSectionService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigJobThirdPartyBulkerCrewSectionService>();
            if (rigJobThirdPartyBulkerCrewSectionService == null) throw new Exception("rigJobThirdPartyBulkerCrewSectionService must be registered in service factory");

            return rigJobThirdPartyBulkerCrewSectionService.SelectBy(new RigJobThirdPartyBulkerCrewSection { RigJob = new RigJob { Id = rigJobId }, ThirdPartyBulkerCrew = new ThirdPartyBulkerCrew() { Id = crewId } }, new List<string> { "RigJobId", "ThirdPartyBulkerCrewId" }).OrderByDescending(p => p.Id).FirstOrDefault();
        }

        public int DeleteRigJobThirdPartyBulkerCrewSection(int id)
        {
            IRigJobThirdPartyBulkerCrewSectionService rigJobThirdPartyBulkerCrewSectionService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigJobThirdPartyBulkerCrewSectionService>();
            if (rigJobThirdPartyBulkerCrewSectionService == null) throw new Exception("rigJobThirdPartyBulkerCrewSectionService must be registered in service factory");

            return rigJobThirdPartyBulkerCrewSectionService.Delete(new RigJobThirdPartyBulkerCrewSection{Id =id });
        }
        public RigJobSanjelCrewSection GetRigJobCrewSection(int rigJobId, int crewId, int jobCrewSectionStatusId)
        {
            IRigJobSanjelCrewSectionService rigJobCrewSectionService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigJobSanjelCrewSectionService>();
            if (rigJobCrewSectionService == null) throw new Exception("rigJobCrewSectionService must be registered in service factory");

            return rigJobCrewSectionService.SelectBy(new RigJobSanjelCrewSection { RigJob = new RigJob { Id = rigJobId }, SanjelCrew = new SanjelCrew{Id = crewId}, RigJobCrewSectionStatus = (RigJobCrewSectionStatus) jobCrewSectionStatusId }, new List<string> { "RigJobId", "SanjelCrewId", "RigJobCrewSectionStatus" }).FirstOrDefault();
        }

        public SanjelCrewSchedule GetCrewScheduleByJobCrewSection(int jobCrewSectionId)
        {
            ISanjelCrewScheduleService crewScheduleService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ISanjelCrewScheduleService>();
            if (crewScheduleService == null) throw new Exception("crewScheduleService must be registered in service factory");

            return crewScheduleService.SelectBy(new SanjelCrewSchedule { RigJobSanjelCrewSection = new RigJobSanjelCrewSection { Id = jobCrewSectionId}}, new List<string>{ "RigJobSanjelCrewSectionId" }).FirstOrDefault();
        }

        public SanjelCrewSchedule GetCrewScheduleByJobCrewSection(int jobCrewSectionId, bool withChildren)
        {
            ISanjelCrewScheduleService crewScheduleService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ISanjelCrewScheduleService>();
            if (crewScheduleService == null) throw new Exception("crewScheduleService must be registered in service factory");

            return crewScheduleService.SelectByJoin(p => p.RigJobSanjelCrewSection.Id == jobCrewSectionId,
                q => q.UnitSchedule != null && q.WorkerSchedule != null).FirstOrDefault();
        }

        public ThirdPartyBulkerCrewSchedule GetThirdPartyBulkerCrewSchedule(int rigJobThirdPartyBulkerCrewSectionId)
        {
            IThirdPartyBulkerCrewScheduleService thirdPartyBulkerCrewScheduleService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IThirdPartyBulkerCrewScheduleService>();
            if (thirdPartyBulkerCrewScheduleService == null) throw new Exception("thirdPartyBulkerCrewScheduleService must be registered in service factory");

            return thirdPartyBulkerCrewScheduleService.SelectBy(new ThirdPartyBulkerCrewSchedule { RigJobThirdPartyBulkerCrewSection = new RigJobThirdPartyBulkerCrewSection() { Id = rigJobThirdPartyBulkerCrewSectionId } }, new List<string> { "RigJobThirdPartyBulkerCrewSectionId" }).FirstOrDefault();
        }
        public int DeleteRigJobCrewSection(RigJobSanjelCrewSection rigJobCrewSection)
        {
            IRigJobSanjelCrewSectionService rigJobCrewSectionService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigJobSanjelCrewSectionService>();
            if (rigJobCrewSectionService == null) throw new Exception("rigJobCrewSectionService must be registered in service factory");

            return rigJobCrewSectionService.Delete(rigJobCrewSection, false);
        }

        public RigJobSanjelCrewSection GetRigJobCrewSectionById(int id)
        {
            IRigJobSanjelCrewSectionService rigJobCrewSectionService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigJobSanjelCrewSectionService>();
            if (rigJobCrewSectionService == null) throw new Exception("rigJobCrewSectionService must be registered in service factory");

            return rigJobCrewSectionService.SelectById(new RigJobSanjelCrewSection { Id = id});
        }

        public ServicePoint GetServicePointById(int id)
        {
            IServicePointService servicePointService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IServicePointService>();
            if (servicePointService == null) throw new Exception("servicePointService must be registered in service factory");

            return servicePointService.SelectById(new ServicePoint {Id = id});
        }

        public int DeleteCrew(int id)
        {
            ISanjelCrewService crewService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ISanjelCrewService>();
            if (crewService == null) throw new Exception("crewService must be registered in service factory");

            return crewService.Delete(new SanjelCrew{Id = id},true);
        }

        public List<ThirdPartyBulkerCrew> GetThirdPartyBulkerCrews()
        {
            IThirdPartyBulkerCrewService thirdPartyBulkerCrewService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IThirdPartyBulkerCrewService>();
            if (thirdPartyBulkerCrewService == null) throw new Exception("thirdPartyBulkerCrewService must be registered in service factory");

            return thirdPartyBulkerCrewService.SelectAll();
        }

        public ThirdPartyBulkerCrew GetThirdPartyBulkerCrewById(int id)
        {
            IThirdPartyBulkerCrewService thirdPartyBulkerCrewService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IThirdPartyBulkerCrewService>();
            if (thirdPartyBulkerCrewService == null) throw new Exception("thirdPartyBulkerCrewService must be registered in service factory");

            return thirdPartyBulkerCrewService.SelectById(new ThirdPartyBulkerCrew{Id = id});
        }

        public int UpdateThirdPartyBulkerCrew(ThirdPartyBulkerCrew thirdPartyBulkerCrew)
        {
            IThirdPartyBulkerCrewService thirdPartyBulkerCrewService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IThirdPartyBulkerCrewService>();
            if (thirdPartyBulkerCrewService == null) throw new Exception("thirdPartyBulkerCrewService must be registered in service factory");

            return thirdPartyBulkerCrewService.Update(thirdPartyBulkerCrew, false);
        }

        public Collection<ContractorCompany> GetAllContractorCompanies()
        {
            IContractorCompanyService contractorCompanyService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IContractorCompanyService>();
            if (contractorCompanyService == null) throw new Exception("contractorCompanyService must be registered in service factory");

            return new Collection<ContractorCompany>(contractorCompanyService.SelectAll());
        }

        public int CreateThirdPartyBulkerCrew(ThirdPartyBulkerCrew thirdPartyBulkerCrew)
        {
            IThirdPartyBulkerCrewService thirdPartyBulkerCrewService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IThirdPartyBulkerCrewService>();
            if (thirdPartyBulkerCrewService == null) throw new Exception("thirdPartyBulkerCrewService must be registered in service factory");

            return thirdPartyBulkerCrewService.Insert(thirdPartyBulkerCrew, false);
        }

        public ContractorCompany GetContractorCompanyById(int id)
        {
            IContractorCompanyService contractorCompanyService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IContractorCompanyService>();
            if (contractorCompanyService == null) throw new Exception("contractorCompanyService must be registered in service factory");

            return contractorCompanyService.SelectById(new ContractorCompany{Id = id});
        }



        public WitsBox GetWitsBoxById(int id)
        {
            IWitsBoxService witsBoxService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IWitsBoxService>();
            if (witsBoxService == null) throw new Exception("witsBoxService must be registered in service factory");

            return witsBoxService.SelectById(new WitsBox{Id = id});
        }

        public int CreateWitsBoxInformation(WitsBoxInformation witsBoxInformation)
        {
            IWitsBoxInformationService witsBoxInformationService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IWitsBoxInformationService>();
            if (witsBoxInformationService == null) throw new Exception("witsBoxInformationService must be registered in service factory");

            return witsBoxInformationService.Insert(witsBoxInformation);
        }

        public int UpdateWitsBoxInformation(WitsBoxInformation witsBoxInformation)
        {
            IWitsBoxInformationService witsBoxInformationService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IWitsBoxInformationService>();
            if (witsBoxInformationService == null) throw new Exception("witsBoxInformationService must be registered in service factory");

            return witsBoxInformationService.Update(witsBoxInformation, false);
        }

        public List<WitsBoxInformation> GetWitsBoxInformationByServicePointAndEquipmentStatus(int workingServicePointId, EquipmentStatus equipmentStatus)
        {
            IWitsBoxInformationService witsBoxInformationService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IWitsBoxInformationService>();
            if (witsBoxInformationService == null) throw new Exception("witsBoxInformationService must be registered in service factory");

            return witsBoxInformationService.SelectByEquipmentStatusWorkingServicePoint(workingServicePointId, (int)equipmentStatus);
        }

        #endregion

        #region Retrieve Data from Sanjel MicroService

        public List<ProductLoadSection> GetProductLoadSectionsByProductLoadId(int productHaulLoadId)
        {
            IProductLoadSectionService productLoadSectionService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IProductLoadSectionService>();
            if (productLoadSectionService == null) throw new Exception("productLoadSectionService must be registered in service factory");

            return productLoadSectionService.SelectBy(new ProductLoadSection{ProductHaulLoad = new ProductHaulLoad{Id = productHaulLoadId}}, new List<string>(){ "ProductHaulLoadId" });
        }

        public RigJob GetRigJobById(int id)
        {
            IRigJobService rigJobService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigJobService>();
            if (rigJobService == null) throw new Exception("rigJobService must be registered in service factory");
            
            return rigJobService.SelectById(new RigJob {Id = id});
        }


        public int UpdateRigJob(RigJob rigJob)
        {
            IRigJobService rigJobService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigJobService>();
            if (rigJobService == null) throw new Exception("rigJobService must be registered in service factory");

            return rigJobService.Update(rigJob);
        }


        public List<RigJob> GetRigJobsByRigId(int rigId)
        {
            IRigJobService rigJobService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigJobService>();
            if (rigJobService == null) throw new Exception("rigJobService must be registered in service factory");

            return rigJobService.SelectWithChildren(rigJobService.SelectBy(new RigJob{Rig = new Rig{Id = rigId}}, new List<string>{"RigId"}));
        }

        public List<RigJob> GetRigJobs()
        {
            IRigJobService rigJobService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigJobService>();
            if (rigJobService == null) throw new Exception("rigJobService must be registered in service factory");

            return rigJobService.SelectAll();
        }

        public List<RigJob> GetRigJobsWithReferenceData()
        {
            IRigJobService rigJobService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigJobService>();
            if (rigJobService == null) throw new Exception("rigJobService must be registered in service factory");

            return rigJobService.SelectAll(true);
        }

        public List<RigJob> GetListedRigJobByRigId(int rigId)
        {
            IRigJobService rigJobService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigJobService>();
            if (rigJobService == null) throw new Exception("rigJobService must be registered in service factory");

            return rigJobService.SelectWithChildren(rigJobService.SelectBy(new RigJob { Rig = new Rig { Id = rigId }, IsListed = true}, new List<string> { "RigId", "IsListed" }));
        }

        public List<RigJob> GetUnListedRigJobByRigId(int rigId)
        {
            IRigJobService rigJobService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigJobService>();
            if (rigJobService == null) throw new Exception("rigJobService must be registered in service factory");

            return rigJobService.SelectWithChildren(rigJobService.SelectBy(new RigJob { Rig = new Rig { Id = rigId }, IsListed = false }, new List<string> { "RigId", "IsListed" }));
        }

        public List<RigJob> GetRigJobsByServicePoints(Collection<int> servicePointIds)
        {
            IRigJobService rigJobService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigJobService>();
            if (rigJobService == null) throw new Exception("rigJobService must be registered in service factory");


            return rigJobService.SelectBy(new RigJob(), rigJob => servicePointIds.Contains(rigJob.ServicePoint.Id));
        }
        public List<RigJob> GetRigJobsByIds(Collection<int> rigJobIds)
        {
            IRigJobService rigJobService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigJobService>();
            if (rigJobService == null) throw new Exception("rigJobService must be registered in service factory");

            return rigJobService.SelectBy(new RigJob(), rigJob => rigJobIds.Contains(rigJob.Id));
        }
        public List<RigJob> GetRigJobsByIdsServicePoints(Collection<int> rigJobIds, Collection<int> servicePointIds)
        {
            IRigJobService rigJobService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigJobService>();
            if (rigJobService == null) throw new Exception("rigJobService must be registered in service factory");


            return rigJobService.SelectBy(new RigJob(), rigJob =>  rigJobIds.Contains(rigJob.Id)).Where(p=>servicePointIds.Contains(p.ServicePoint.Id)).ToList();
        }

        public List<RigJob> GetRigJobsWithChildren(List<RigJob> rigJobs)
        {
            IRigJobService rigJobService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigJobService>();
            if (rigJobService == null) throw new Exception("rigJobService must be registered in service factory");

            return rigJobService.SelectWithChildren(rigJobs);
        }

        public RigJob GetRigJobByCallSheetNumber(int callSheetNumber)
        {
            IRigJobService rigJobService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigJobService>();
            if (rigJobService == null) throw new Exception("rigJobService must be registered in service factory");

            return rigJobService.SelectBy(new RigJob { }, p=>p.CallSheetNumber == callSheetNumber).FirstOrDefault();
//            return rigJobService.SelectBy(new RigJob { CallSheetNumber = callSheetNumber }, new List<string> { "CallSheetNumber"}).FirstOrDefault();
        }
        public List<RigJob> GetRigJobsByProgramId(string programId)
        {
            IRigJobService rigJobService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigJobService>();
            if (rigJobService == null) throw new Exception("rigJobService must be registered in service factory");

            return rigJobService.SelectBy(new RigJob { ProgramId = programId }, new List<string> { "ProgramId" });
        }
        //        public JobLifeStatus GetJobLifeStatusByName(string name)
        //        {
        //            IJobLifeStatusService jobLifeStatusService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IJobLifeStatusService>();
        //            if (jobLifeStatusService == null) throw new Exception("jobLifeStatusService must be registered in service factory");
        //
        //            return jobLifeStatusService.SelectBy(new JobLifeStatus { Name = name }, new List<string> { "Name" }).FirstOrDefault();
        //        }
        //
        //        public JobLifeStatus GetJobLifeStatusById(int id)
        //        {
        //            IJobLifeStatusService jobLifeStatusService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IJobLifeStatusService>();
        //            if (jobLifeStatusService == null) throw new Exception("jobLifeStatusService must be registered in service factory");
        //
        //            return jobLifeStatusService.SelectById(new JobLifeStatus {Id = id});
        //        }

        public ProductHaulLoad GetProductHaulLoadById(int id, bool withiChildren = false)
        {
            IProductHaulLoadService productHaulLoadService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IProductHaulLoadService>();
            if (productHaulLoadService == null) throw new Exception("productHaulLoadService must be registered in service factory");

            return productHaulLoadService.SelectById(new ProductHaulLoad{Id = id}, withiChildren);
        }

        public Collection<ThirdPartyUnitSection> GetThirdPartyUnitSectionsByProductHaul(ProductHaul productHaul)
        {
            IThirdPartyUnitSectionMicroService thirdPartyUnitSectionService = ServiceFactory.Instance.GetService(typeof(IThirdPartyUnitSectionMicroService)) as IThirdPartyUnitSectionMicroService;
            if (thirdPartyUnitSectionService == null) throw new Exception("thirdPartyUnitSectionService must be registered in service factory");

            return thirdPartyUnitSectionService.GetThirdPartyUnitSectionCollectionByProductHaulId(productHaul.Id);
        }

        public ThirdPartyUnitSection UpdateThirdPartyUnitSection(ThirdPartyUnitSection thirdPartyUnitSection)
        {
            IThirdPartyUnitSectionMicroService thirdPartyUnitSectionService = ServiceFactory.Instance.GetService(typeof(IThirdPartyUnitSectionMicroService)) as IThirdPartyUnitSectionMicroService;
            if (thirdPartyUnitSectionService == null) throw new Exception("thirdPartyUnitSectionService must be registered in service factory");

            return thirdPartyUnitSectionService.UpdateThirdPartyUnitSection(thirdPartyUnitSection);
        }

        public Collection<UnitSection> GetUnitSectionsByProductHaul(ProductHaul productHaul)
        {
            IUnitSectionMicroService unitSectionMicroService = ServiceFactory.Instance.GetService(typeof(IUnitSectionMicroService)) as IUnitSectionMicroService;
            if (unitSectionMicroService == null) throw new Exception("unitSectionService must be registered in service factory");


            return unitSectionMicroService.GetUnitSectionCollectionByProductHaulId(productHaul.Id);
        }

        /*
        public UnitSection UpdateUnitSection(UnitSection unitSection)
        {
            IUnitSectionMicroService unitSectionMicroService = ServiceFactory.Instance.GetService(typeof(IUnitSectionMicroService)) as IUnitSectionMicroService;
            if (unitSectionMicroService == null) throw new Exception("unitSectionService must be registered in service factory");

            return unitSectionMicroService.UpdateUnitSection(unitSection);
        }
        */

        public List<ProductLoadSection> GetProductLoadSectionsByProductLoad(ProductHaulLoad productHaulLoad)
        {
            IProductLoadSectionService productLoadSectionService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IProductLoadSectionService>();
            if (productLoadSectionService == null) throw new Exception("productLoadSectionService must be registered in service factory");

            return productLoadSectionService.SelectBy(new ProductLoadSection { ProductHaulLoad = productHaulLoad }, new List<string> { "ProductHaulLoadId" });
        }

        public int DeleteProductLoadSection(ProductLoadSection productLoadSection)
        {
            IProductLoadSectionService productLoadSectionService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IProductLoadSectionService>();
            if (productLoadSectionService == null) throw new Exception("productLoadSectionService must be registered in service factory");

            return productLoadSectionService.Delete(productLoadSection);
        }

        public int DeleteProductHaulLoad(ProductHaulLoad productHaulLoad, bool withChildren = false)
        {
            IProductHaulLoadService productHaulLoadService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IProductHaulLoadService>();
            if (productHaulLoadService == null) throw new Exception("productHaulLoadService must be registered in service factory");

            return productHaulLoadService.Delete(productHaulLoad, withChildren);
        }

        public List<ProductHaulLoad> GetProductHaulLoadsByProductHual(ProductHaul productHaul)
        {
            IProductHaulLoadService productHaulLoadService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IProductHaulLoadService>();
            if (productHaulLoadService == null) throw new Exception("productHaulLoadService must be registered in service factory");

            return productHaulLoadService.SelectBy(new ProductHaulLoad { ProductHaul = productHaul }, new List<string> { "ProductHaulId" });
        }       
        public int DeleteProductHaul(ProductHaul productHaul, bool withChildren = false)
        {
            IProductHaulService productHaulService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IProductHaulService>();
            if (productHaulService == null) throw new Exception("productHaulService must be registered in service factory");

            return productHaulService.Delete(productHaul, withChildren);
        }

        public int DeleteUnitSection(UnitSection unitSection)
        {
            IUnitSectionMicroService unitSectionMicroService = ServiceFactory.Instance.GetService(typeof(IUnitSectionMicroService)) as IUnitSectionMicroService;
            if (unitSectionMicroService == null) throw new Exception("unitSectionService must be registered in service factory");

            return unitSectionMicroService.DeleteUnitSection(unitSection);
        }

        public int DeleteThirdPartyUnitSection(ThirdPartyUnitSection thirdPartyUnitSection)
        {
            IThirdPartyUnitSectionMicroService thirdPartyUnitSectionService = ServiceFactory.Instance.GetService(typeof(IThirdPartyUnitSectionMicroService)) as IThirdPartyUnitSectionMicroService;
            if (thirdPartyUnitSectionService == null) throw new Exception("thirdPartyUnitSectionService must be registered in service factory");

            return thirdPartyUnitSectionService.DeleteThirdPartyUnitSection(thirdPartyUnitSection);
        }

        public Collection<UnitSection> GetUnitSectionsByCallSheetId(int callSheetId)
        {
            IUnitSectionMicroService unitSectionMicroService = ServiceFactory.Instance.GetService(typeof(IUnitSectionMicroService)) as IUnitSectionMicroService;
            if (unitSectionMicroService == null) throw new Exception("unitSectionService must be registered in service factory");

            return unitSectionMicroService.GetUnitSectionByRootId(callSheetId);
        }

        public Rig GetRigById(int id)
        {
            IRigService rigService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigService>();
            if (rigService == null) throw new Exception("rigService must be registered in service factory");

            return rigService.SelectById(new Rig{Id = id});
        }

        public Rig GetRigByName(string name)
        {
            IRigService rigService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigService>();
            if (rigService == null) throw new Exception("rigService must be registered in service factory");

            return rigService.SelectBy(new Rig{Name = name}, new List<string>{"Name"}).FirstOrDefault();
        }

        public List<Rig> GetBulkPlants()
        {
            IRigService rigService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigService>();
            if (rigService == null) throw new Exception("rigService must be registered in service factory");

            return rigService.SelectByOperationSiteType((int)OperationSiteType.BulkPlant);
        }

        public int UpdateRig(Rig rig)
        {
            IRigService rigService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigService>();
            if (rigService == null) throw new Exception("rigService must be registered in service factory");

            return rigService.Update(rig);
        }

        public int CreateRig(Rig rig)
        {
            IRigService rigService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigService>();
            if (rigService == null) throw new Exception("rigService must be registered in service factory");

            return rigService.Insert(rig);
        }

        public int CreateRigJob(RigJob rigJob)
        {
            IRigJobService rigJobService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigJobService>();
            if (rigJobService == null) throw new Exception("rigJobService must be registered in service factory");

            return rigJobService.Insert(rigJob);
        }

        public List<ProductHaulLoad> GetProductHaulLoadsByCallSheetNumber(int callSheetNumber)
        {
            IProductHaulLoadService productHaulLoadService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IProductHaulLoadService>();
            if (productHaulLoadService == null) throw new Exception("productHaulLoadService must be registered in service factory");

            return productHaulLoadService.SelectBy(new ProductHaulLoad { CallSheetNumber = callSheetNumber }, new List<string> { "CallSheetNumber" });
        }

        public List<ProductHaulLoad> GetProductHaulLoads(Pager pager = null)
        {
            IProductHaulLoadService productHaulLoadService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IProductHaulLoadService>();
            if (productHaulLoadService == null) throw new Exception("productHaulLoadService must be registered in service factory");
            if(pager != null)
                return productHaulLoadService.SelectAll(pager);
            else
            {
                return productHaulLoadService.SelectAll();
            }
        }

        public List<ProductHaulLoad> GetScheduledProductHaulLoads()
        {
            IProductHaulLoadService productHaulLoadService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IProductHaulLoadService>();
            if (productHaulLoadService == null) throw new Exception("productHaulLoadService must be registered in service factory");

            return productHaulLoadService.SelectByProductHaulLoadLifeStatus((int)ProductHaulLoadStatus.Scheduled);
        }
        
        public List<ProductHaulLoad> GetBlendCompletedProductHaulLoads()
        {
            IProductHaulLoadService productHaulLoadService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IProductHaulLoadService>();
            if (productHaulLoadService == null) throw new Exception("productHaulLoadService must be registered in service factory");

            return productHaulLoadService.SelectByProductHaulLoadLifeStatus((int)ProductHaulLoadStatus.BlendCompleted);
        }
        public List<ProductHaulLoad> GetBlendingProductHaulLoads()
        {
            IProductHaulLoadService productHaulLoadService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IProductHaulLoadService>();
            if (productHaulLoadService == null) throw new Exception("productHaulLoadService must be registered in service factory");

            return productHaulLoadService.SelectByProductHaulLoadLifeStatus((int)ProductHaulLoadStatus.Blending);
        }

        public List<ProductHaulLoad> GetLoadedProductHaulLoads()
        {
            IProductHaulLoadService productHaulLoadService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IProductHaulLoadService>();
            if (productHaulLoadService == null) throw new Exception("productHaulLoadService must be registered in service factory");

            return productHaulLoadService.SelectByProductHaulLoadLifeStatus((int)ProductHaulLoadStatus.Loaded);
        }


        public List<ProductHaulLoad> GetProductHaulLoadsByServicePointAndStatuses(int servicePointId, int[] statuss)
        {
            IProductHaulLoadService productHaulLoadService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IProductHaulLoadService>();
            if (productHaulLoadService == null) throw new Exception("productHaulLoadService must be registered in service factory");

            List<ProductHaulLoad> allLoads = productHaulLoadService.SelectProductHaulLoadByProductHaulLoadLifeStatuss(statuss);

            return allLoads.FindAll(p => p.ServicePoint.Id == servicePointId);
        }

        public List<ProductHaulLoad> GetProductHaulLoadsByServicePointAndStatus(int servicePointId, ProductHaulLoadStatus status)
        {
            IProductHaulLoadService productHaulLoadService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IProductHaulLoadService>();
            if (productHaulLoadService == null) throw new Exception("productHaulLoadService must be registered in service factory");

            return productHaulLoadService.SelectByProductHaulLoadLifeStatusServicePoint((int)status, servicePointId);
        }

        public List<ProductHaulLoad> GetScheduledProductHaulLoadsByServicePoint(int servicePointId)
        {
            IProductHaulLoadService productHaulLoadService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IProductHaulLoadService>();
            if (productHaulLoadService == null) throw new Exception("productHaulLoadService must be registered in service factory");

            return productHaulLoadService.SelectByProductHaulLoadLifeStatusServicePoint((int)ProductHaulLoadStatus.Scheduled, servicePointId);
        }


        public int DeleteRigJob(RigJob rigJob)
        {
            IRigJobService rigJobService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigJobService>();
            if (rigJobService == null) throw new Exception("rigJobService must be registered in service factory");

            return rigJobService.Delete(rigJob);
        }

//        public RigStatus GetRigStatusByName(string name)
//        {
//            IRigStatusService rigStatusService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigStatusService>();
//            if (rigStatusService == null) throw new Exception("rigStatusService must be registered in service factory");
//
//            return rigStatusService.SelectBy(new RigStatus{Name = name}, new List<string>{"Name"}).FirstOrDefault();
//        }

//        public RigStatus GetRigStatusById(int id)
//        {
//            IRigStatusService rigStatusService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigStatusService>();
//            if (rigStatusService == null) throw new Exception("rigStatusService must be registered in service factory");
//
//            return rigStatusService.SelectById(new RigStatus {Id = id});
//        }

        public int UpdateProductHaulLoad(ProductHaulLoad productHaulLoad, bool withChildren=false)
        {
            IProductHaulLoadService productHaulLoadService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IProductHaulLoadService>();
            if (productHaulLoadService == null) throw new Exception("productHaulLoadService must be registered in service factory");

            return productHaulLoadService.Update(productHaulLoad, withChildren);
        }

//        public ProductHaulStatus GetProductHaulStatusByName(string name)
//        {
//            IProductHaulStatusService productHaulStatusService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IProductHaulStatusService>();
//            if (productHaulStatusService == null) throw new Exception("productHaulStatusService must be registered in service factory");
//
//            return productHaulStatusService.SelectBy(new ProductHaulStatus { Name = name }, new List<string> { "Name" }).FirstOrDefault();
//        }
//
//        public ProductHaulLoadStatus GetProductHaulLoadStatusByName(string name)
//        {
//            IProductHaulLoadStatusService productHaulLoadStatusService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IProductHaulLoadStatusService>();
//            if (productHaulLoadStatusService == null) throw new Exception("productHaulLoadStatusService must be registered in service factory");
//
//            return productHaulLoadStatusService.SelectBy(new ProductHaulLoadStatus { Name = name }, new List<string> { "Name" }).FirstOrDefault();
//        }

        public int UpdateProductHaul(ProductHaul productHaul, bool isUpdateChildren= true)
        {
            IProductHaulService productHaulService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IProductHaulService>();
            if (productHaulService == null) throw new Exception("productHaulService must be registered in service factory");

            return productHaulService.Update(productHaul,isUpdateChildren);
        }

        public List<ProductHaul> GetProductHauls()
        {
            IProductHaulService productHaulService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IProductHaulService>();
            if (productHaulService == null) throw new Exception("productHaulService must be registered in service factory");
            
            return productHaulService.SelectAll();
        }

        public List<ProductHaul> GetScheduledProductHauls()
        {
            IProductHaulService productHaulService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance
                .GetService<IProductHaulService>();
            if (productHaulService == null)
                throw new Exception("productHaulService must be registered in service factory");

            return productHaulService.SelectBy(new ProductHaul() { ProductHaulLifeStatus =  ProductHaulStatus.Scheduled }, new List<string>() { "ProductHaulLifeStatus" });
        }

        public List<ProductHaul> GetScheduledProductHaulsWithShippingLoadSheets()
        {
	        IProductHaulService productHaulService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance
		        .GetService<IProductHaulService>();
	        if (productHaulService == null)
		        throw new Exception("productHaulService must be registered in service factory");

	        return productHaulService.SelectByJoin(p => p.ProductHaulLifeStatus == ProductHaulStatus.Scheduled,
		        p => p.ShippingLoadSheets != null);
        }


        public int CreateProductHaulLoad(ProductHaulLoad productHaulLoad,bool flag=false)
        {
            IProductHaulLoadService productHaulLoadService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IProductHaulLoadService>();
            if (productHaulLoadService == null) throw new Exception("productHaulLoadService must be registered in service factory");

            return productHaulLoadService.Insert(productHaulLoad, flag);
        }

        public int CreateProductLoadSection(ProductLoadSection productLoadSection)
        {
            IProductLoadSectionService productLoadSectionService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IProductLoadSectionService>();
            if (productLoadSectionService == null) throw new Exception("productLoadSectionService must be registered in service factory");

            return productLoadSectionService.Insert(productLoadSection);
        }

        public ThirdPartyUnitSection GetThirdPartyUnitSectionByCallSheetAndProductHaul(Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch.CallSheet callSheet, ProductHaul productHaul)
        {
            IThirdPartyUnitSectionMicroService thirdPartyUnitSectionService = ServiceFactory.Instance.GetService(typeof(IThirdPartyUnitSectionMicroService)) as IThirdPartyUnitSectionMicroService;
            if (thirdPartyUnitSectionService == null) throw new Exception("thirdPartyUnitSectionService must be registered in service factory");

            return thirdPartyUnitSectionService.GetThirdPartyUnitSectionByCallSheetIdAndProductHaulId(callSheet.Id, productHaul.Id);
        }

        public UnitSection GetUnitSectionByCallSheetAndProductHaul(Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch.CallSheet callSheet, ProductHaul productHaul)
        {
            IUnitSectionMicroService unitSectionMicroService = ServiceFactory.Instance.GetService(typeof(IUnitSectionMicroService)) as IUnitSectionMicroService;
            if (unitSectionMicroService == null) throw new Exception("unitSectionService must be registered in service factory");


            return unitSectionMicroService.GetUnitSectionByCallSheetIdAndProductHaulId(callSheet.Id,productHaul.Id);
        }

        public int CreateProductHaul(ProductHaul productHaul, bool isUpdateChildren = true)
        {
            IProductHaulService productHaulService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IProductHaulService>();
            if (productHaulService == null) throw new Exception("productHaulService must be registered in service factory");

            return productHaulService.Insert(productHaul, isUpdateChildren);
        }

        public ClientCompany GetClientCompanyById(int id)
        {
            IClientCompanyService clientCompanyService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IClientCompanyService>();
            if (clientCompanyService == null) throw new Exception("clientCompanyService must be registered in service factory");

            return clientCompanyService.SelectById(new ClientCompany{Id = id});
        }

        public int UpdateClientCompany(ClientCompany clientCompany)
        {
            IClientCompanyService clientCompanyService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IClientCompanyService>();
            if (clientCompanyService == null) throw new Exception("clientCompanyService must be registered in service factory");

            return clientCompanyService.Update(clientCompany);
        }

        public ClientConsultant GetClientConsultantById(int id)
        {
            IClientConsultantService clientConsultantService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IClientConsultantService>();
            if (clientConsultantService == null) throw new Exception("clientConsultantService must be registered in service factory");

            return clientConsultantService.SelectById(new ClientConsultant {Id = id});
        }

        public int UpdateClientConsultant(ClientConsultant clientConsultant)
        {
            IClientConsultantService clientConsultantService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IClientConsultantService>();
            if (clientConsultantService == null) throw new Exception("clientConsultantService must be registered in service factory");

            return clientConsultantService.Update(clientConsultant);
        }

        public Job GetJobByUniqueId(string jobUniqueId)
        {
            IJobMicroService jobMicroService = ServiceFactory.Instance.GetService(typeof(IJobMicroService)) as IJobMicroService;
            if (jobMicroService == null) throw new Exception("jobMicroService must be registered in service factory.");
            Job job = jobMicroService.GetJobByUniqueId(jobUniqueId);

            return job;
        }

        public UnitSection CreateUnitSection(UnitSection unitSection)
        {
            IUnitSectionMicroService unitSectionMicroService = ServiceFactory.Instance.GetService(typeof(IUnitSectionMicroService)) as IUnitSectionMicroService;
            if (unitSectionMicroService == null) throw new Exception("unitSectionService must be registered in service factory");

            return unitSectionMicroService.CreateUnitSection(unitSection);
        }

        public Collection<UnitSection> GetUnitSectionsByCrewId(int crewId)
        {
            IUnitSectionMicroService unitSectionMicroService = ServiceFactory.Instance.GetService(typeof(IUnitSectionMicroService)) as IUnitSectionMicroService;
            if (unitSectionMicroService == null) throw new Exception("unitSectionService must be registered in service factory");

            return unitSectionMicroService.GetUnitSectionByCrewId(crewId);
        }

        public ThirdPartyUnitSection CreateThirdPartyUnitSection(ThirdPartyUnitSection thirdPartyUnitSection)
        {
            IThirdPartyUnitSectionMicroService thirdPartyUnitSectionService = ServiceFactory.Instance.GetService(typeof(IThirdPartyUnitSectionMicroService)) as IThirdPartyUnitSectionMicroService;
            if (thirdPartyUnitSectionService == null) throw new Exception("thirdPartyUnitSectionService must be registered in service factory");

            return thirdPartyUnitSectionService.CreateThirdPartyUnitSection(thirdPartyUnitSection);
        }

        public ProductHaul GetProductHaulById(int id)
        {
            IProductHaulService productHaulService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IProductHaulService>();
            if (productHaulService == null) throw new Exception("productHaulService must be registered in service factory");

//            return productHaulService.SelectById(new ProductHaul{Id = id}, true);
            return productHaulService.SelectByJoin(p=>p.Id==id, q=>q.ShippingLoadSheets != null && q.PodLoad != null).FirstOrDefault();
        }
        public List<ProductHaul> GetProductHaulLoadsByProductHaulIds(Collection<int> productHaulIds)
        {
            IProductHaulService productHaulService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IProductHaulService>();
            if (productHaulService == null) throw new Exception("productHaulService must be registered in service factory");

            return productHaulService.SelectBy(new ProductHaul(),  productHaul=>productHaulIds.Contains(productHaul.Id));
        }

        public CallSheet GetCallSheetById(int id)
        {
            Sanjel.Services.Interfaces.ICallSheetService callSheetService = ServiceFactory.Instance.GetService(typeof(Sanjel.Services.Interfaces.ICallSheetService)) as Sanjel.Services.Interfaces.ICallSheetService;
            if (callSheetService == null) throw new Exception("callSheetMicroService can not be null.");
            return callSheetService.GetCallSheetById(id);
        }

        #endregion

        #region schedule
        public List<SanjelCrewSchedule> GetCrewSchedules()
        {
            ISanjelCrewScheduleService crewScheduleService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ISanjelCrewScheduleService>();
            if (crewScheduleService == null) throw new Exception("crewScheduleService must be registered in service factory");
            
           return crewScheduleService.SelectAll();
        }
        public List<SanjelCrewSchedule> GetCrewSchedules(Collection<int> servicePoints, DateTime startTime, DateTime endTime)
        {
            ISanjelCrewScheduleService crewScheduleService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ISanjelCrewScheduleService>();
            if (crewScheduleService == null) throw new Exception("crewScheduleService must be registered in service factory");
            List<SanjelCrewSchedule>  crewSchedules;
            if (servicePoints == null || servicePoints.Count == 0)
            {
                crewSchedules = crewScheduleService.SelectBy(new SanjelCrewSchedule(),
                    sanjelCrewSchedule =>
                        sanjelCrewSchedule.StartTime > startTime && sanjelCrewSchedule.EndTime < endTime);
            }
            else
            {
                crewSchedules = crewScheduleService.SelectBy(new SanjelCrewSchedule(),
                    sanjelCrewSchedule => sanjelCrewSchedule.StartTime > startTime &&
                                          sanjelCrewSchedule.EndTime < endTime &&
                                          servicePoints.Contains(sanjelCrewSchedule.WorkingServicePoint.Id));
            }

            return crewSchedules;
        }

        public List<UnitSchedule> GetUnitSchedules()
        {
            IUnitScheduleService unitScheduleService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IUnitScheduleService>();
            if (unitScheduleService == null) throw new Exception("unitScheduleService must be registered in service factory");
            
            return unitScheduleService.SelectAll();
        }

        public List<UnitSchedule> GetUnitSchedules(Collection<int> servicePoints, DateTime startTime, DateTime endTime)
        {
            IUnitScheduleService unitScheduleService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IUnitScheduleService>();
            if (unitScheduleService == null) throw new Exception("unitScheduleService must be registered in service factory");
            List<UnitSchedule>  unitSchedules;
            if (servicePoints == null || servicePoints.Count == 0)
            {
                unitSchedules = unitScheduleService.SelectBy(new UnitSchedule(), 
                    unitSchedule =>
                        unitSchedule.StartTime > startTime && unitSchedule.EndTime < endTime);
            }
            else
            {
                unitSchedules = unitScheduleService.SelectBy(new UnitSchedule(),
                    unitSchedule => unitSchedule.StartTime > startTime &&
                                          unitSchedule.EndTime < endTime &&
                                          servicePoints.Contains(unitSchedule.WorkingServicePoint.Id));
            }

            return unitSchedules;
        }

        public List<UnitSchedule> GetUnitSchedulesByTruckUnit(int truckUnitId)
        {
            IUnitScheduleService unitScheduleService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IUnitScheduleService>();
            if (unitScheduleService == null) throw new Exception("unitScheduleService must be registered in service factory");

            return unitScheduleService.SelectBy(new UnitSchedule{TruckUnit = new TruckUnit{Id = truckUnitId}}, new List<string>{ "TruckUnitId" });
        }

        public List<WorkerSchedule> GetWorkerSchedules()
        {
            IWorkerScheduleService workerScheduleService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IWorkerScheduleService>();
            if (workerScheduleService == null) throw new Exception("unitScheduleService must be registered in service factory");

            return workerScheduleService.SelectAll();
        }

        public List<WorkerSchedule> GetWorkerSchedules(Collection<int> servicePoints, DateTime startTime, DateTime endTime)
        {
            IWorkerScheduleService workerScheduleService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IWorkerScheduleService>();
            if (workerScheduleService == null) throw new Exception("unitScheduleService must be registered in service factory");
            List<WorkerSchedule>  workerSchedules;
            if (servicePoints == null || servicePoints.Count == 0)
            {
                workerSchedules = workerScheduleService.SelectBy(new WorkerSchedule(), 
                    workerSchedule =>
                        workerSchedule.StartTime > startTime && workerSchedule.EndTime < endTime);
            }
            else
            {
                workerSchedules = workerScheduleService.SelectBy(new WorkerSchedule(),
                    workerSchedule => workerSchedule.StartTime > startTime &&
                                          workerSchedule.EndTime < endTime &&
                                          servicePoints.Contains(workerSchedule.WorkingServicePoint.Id));
            }

            return workerSchedules;
        }

        public List<WorkerSchedule> GetWorkerSchedulesByWorker(int workerId)
        {
            IWorkerScheduleService workerScheduleService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IWorkerScheduleService>();
            if (workerScheduleService == null) throw new Exception("workerScheduleService must be registered in service factory");

            return workerScheduleService.SelectBy(new WorkerSchedule { Worker = new Employee { Id = workerId } }, new List<string> { "WorkerId" });
        }

        public List<WorkerSchedule> GetWorkerSchedulesByQuery(Expression<Func<WorkerSchedule, bool>> query)
        {
	        IWorkerScheduleService workerScheduleService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IWorkerScheduleService>();
	        if (workerScheduleService == null) throw new Exception("workerScheduleService must be registered in service factory");

	        return workerScheduleService.SelectBy(new WorkerSchedule(), query);
        }

        public SanjelCrewSchedule GetCrewScheduleById(int id)
        {
            ISanjelCrewScheduleService crewScheduleService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ISanjelCrewScheduleService>();
            if (crewScheduleService == null) throw new Exception("crewScheduleService must be registered in service factory");
            return crewScheduleService.SelectById(new SanjelCrewSchedule { Id = id}, true);
        }

        public UnitSchedule GetUnitScheduleById(int id)
        {
            IUnitScheduleService unitScheduleService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IUnitScheduleService>();
            if (unitScheduleService == null) throw new Exception("unitScheduleService must be registered in service factory");
            return unitScheduleService.SelectById(new UnitSchedule {Id = id});
        }

        public WorkerSchedule GetWorkerScheduleById(int id)
        {
            IWorkerScheduleService workerScheduleService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IWorkerScheduleService>();
            if (workerScheduleService == null) throw new Exception("unitScheduleService must be registered in service factory");
            return workerScheduleService.SelectById(new WorkerSchedule {Id = id});
        }

        public int DeleteCrewSchedule(int id,bool isUpdateChildren = false)
        {
            ISanjelCrewScheduleService crewScheduleService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ISanjelCrewScheduleService>();
            if (crewScheduleService == null) throw new Exception("crewScheduleService must be registered in service factory");
            return crewScheduleService.Delete(new SanjelCrewSchedule { Id = id}, isUpdateChildren);
        }
        public int DeletethirdPartyBulkerCrewSchedule(int id)
        {
            IThirdPartyBulkerCrewScheduleService thirdPartyBulkerCrewScheduleService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IThirdPartyBulkerCrewScheduleService>();
            if (thirdPartyBulkerCrewScheduleService == null) throw new Exception("crewScheduleService must be registered in service factory");
            return thirdPartyBulkerCrewScheduleService.Delete(new ThirdPartyBulkerCrewSchedule() { Id = id });
        }
        public int DeleteUnitSchedule(int id)
        {
            IUnitScheduleService unitScheduleService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IUnitScheduleService>();
            if (unitScheduleService == null) throw new Exception("unitScheduleService must be registered in service factory");
            return unitScheduleService.Delete(new UnitSchedule {Id = id});
        }

        public int DeleteWorkSchedule(int id)
        {
            IWorkerScheduleService workerScheduleService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IWorkerScheduleService>();
            if (workerScheduleService == null) throw new Exception("unitScheduleService must be registered in service factory");
            return workerScheduleService.Delete(new WorkerSchedule {Id = id});
        }

        public int UpdateSanjelCrewSchedule(SanjelCrewSchedule crewSchedule, bool isUpdateChildren=false)
        {
            ISanjelCrewScheduleService crewScheduleService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ISanjelCrewScheduleService>();
            if (crewScheduleService == null) throw new Exception("crewScheduleService must be registered in service factory");
            return crewScheduleService.Update(crewSchedule,isUpdateChildren);
        }

        public int UpdateWorkerSchedule(WorkerSchedule workerSchedule)
        {
            IWorkerScheduleService workerScheduleService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IWorkerScheduleService>();
            if (workerScheduleService == null) throw new Exception("unitScheduleService must be registered in service factory");
            return workerScheduleService.Update(workerSchedule);
        }

        public int UpdateUnitSchedule(UnitSchedule unitSchedule)
        {
            IUnitScheduleService unitScheduleService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IUnitScheduleService>();
            if (unitScheduleService == null) throw new Exception("unitScheduleService must be registered in service factory");
            return unitScheduleService.Update(unitSchedule);
        }

        public int InsertCrewSchedule(SanjelCrewSchedule crewSchedule, bool isUpdateChildren = false)
        {
            ISanjelCrewScheduleService crewScheduleService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ISanjelCrewScheduleService>();
            if (crewScheduleService == null) throw new Exception("crewScheduleService must be registered in service factory");
            return crewScheduleService.Insert(crewSchedule, isUpdateChildren);
        }

        public int InsertThirdPartyBulkerCrewSchedule(ThirdPartyBulkerCrewSchedule thirdPartyBulkerCrewSchedule)
        {
           IThirdPartyBulkerCrewScheduleService thirdPartyBulkerCrewScheduleService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IThirdPartyBulkerCrewScheduleService>();
           if (thirdPartyBulkerCrewScheduleService == null) throw new Exception("thirdPartyBulkerCrewScheduleService must be registered in service factory");
           return thirdPartyBulkerCrewScheduleService.Insert(thirdPartyBulkerCrewSchedule);
        }

        public int InsertWorkerSchedule(WorkerSchedule workerSchedule)
        {
            IWorkerScheduleService workerScheduleService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IWorkerScheduleService>();
            if (workerScheduleService == null) throw new Exception("unitScheduleService must be registered in service factory");
            return workerScheduleService.Insert(workerSchedule);
        }

        public int InsertUnitSchedule(UnitSchedule unitSchedule)
        {
            IUnitScheduleService unitScheduleService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IUnitScheduleService>();
            if (unitScheduleService == null) throw new Exception("unitScheduleService must be registered in service factory");
            return unitScheduleService.Insert(unitSchedule);
        }

        /*public UnitSchedule GetUnitScheduleByCrewScheduleAndTruckUnit(int crewScheduleId, int truckUnitId)
        {
            IUnitScheduleService unitScheduleService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IUnitScheduleService>();
            if (unitScheduleService == null) throw new Exception("unitScheduleService must be registered in service factory");
            UnitSchedule unitSchedule = unitScheduleService.SelectBy( new UnitSchedule {TruckUnit = new TruckUnit { Id = truckUnitId },OwnerId = crewScheduleId}, new List<string> { "TruckUnitId", "Owner_id" }).FirstOrDefault();
            return unitSchedule;
        }*/

        /*
        public WorkerSchedule GetWorkerScheduleByCrewScheduleAndWorker(int crewScheduleId, int workerId)
        {
            IWorkerScheduleService workerScheduleService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IWorkerScheduleService>();
            if (workerScheduleService == null) throw new Exception("workerScheduleService must be registered in service factory");
            WorkerSchedule workerSchedules = workerScheduleService.SelectBy(new WorkerSchedule { Worker = new Employee { Id = workerId },OwnerId = crewScheduleId}, new List<string> { "WorkerId", "Owner_id" }).FirstOrDefault();            
            return workerSchedules;
        }
        */

        /*
        public List<UnitScheduleType> UnitScheduleTypes()
        {
            IUnitScheduleTypeService unitScheduleTypeService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IUnitScheduleTypeService>();
            if (unitScheduleTypeService == null) throw new Exception("unitScheduleStatusService must be registered in service factory");
            return unitScheduleTypeService.SelectAll();
        }

        public List<WorkerScheduleType> WorkerScheduleTypes()
        {
            IWorkerScheduleTypeService workerScheduleTypeService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IWorkerScheduleTypeService>();
            if (workerScheduleTypeService == null) throw new Exception("workerScheduleStatusService must be registered in service factory");
            return workerScheduleTypeService.SelectAll();
        }
        */

        /*
        public List<UnitSchedule> GetUnitSchedulesByCrewSchedule(int crewScheduleId)
        {
            IUnitScheduleService unitScheduleService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IUnitScheduleService>();
            if (unitScheduleService == null) throw new Exception("unitScheduleService must be registered in service factory");
            List<UnitSchedule> unitSchedules = unitScheduleService.SelectBy(new UnitSchedule { OwnerId = crewScheduleId }, new List<string> { "Owner_Id" });

            return unitSchedules;
        }

        public List<WorkerSchedule> GetWorkerSchedulesByCrewSchedule(int crewScheduleId)
        {
            IWorkerScheduleService workerScheduleService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IWorkerScheduleService>();
            if (workerScheduleService == null) throw new Exception("unitScheduleService must be registered in service factory");
            List<WorkerSchedule> workerSchedules = workerScheduleService.SelectBy(new WorkerSchedule { OwnerId = crewScheduleId }, new List<string> { "Owner_Id" });

            return workerSchedules;
        }
        */

        public SanjelCrewTruckUnitSection GetCrewTruckUnitSectionById(int id)
        {
            ISanjelCrewTruckUnitSectionService crewTruckUnitSectionService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ISanjelCrewTruckUnitSectionService>();
            if (crewTruckUnitSectionService == null) throw new Exception("crewTruckUnitSectionService must be registered in service factory");

            return crewTruckUnitSectionService.SelectById(new SanjelCrewTruckUnitSection { Id = id});
        }

        public SanjelCrewWorkerSection GetCrewWorkerSectionById(int id)
        {
            ISanjelCrewWorkerSectionService crewWorkerSectionService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ISanjelCrewWorkerSectionService>();
            if (crewWorkerSectionService == null) throw new Exception("crewWorkerSectionService must be registered in service factory");

            return crewWorkerSectionService.SelectById(new SanjelCrewWorkerSection { Id = id });
        }



        #endregion

        #region bin
        public List<BinInformation> GetBinInformationsByRigId(int rigId)
        {
            IBinInformationService binInformationService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IBinInformationService>();
           if (binInformationService == null) throw new Exception("BinInformationService must be registered in service factory");
           return binInformationService.SelectByRig(rigId);
//           return binInformationService.SelectBy(new BinInformation() {Rig = new Rig {Id = rigId}}, new List<string>() {"Rigid"});
        }

        public BinInformation GetBinInformationByBinIdAndPodIndex(int binId,int podIndex)
        {
             IBinInformationService binInformationService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IBinInformationService>();
            if (binInformationService == null) throw new Exception("BinInformationService must be registered in service factory");
            /*
            var binInformations = binInformationService.SelectByBin(binId);
            var binInformation = binInformations.FirstOrDefault();
            if(podIndex>0)
            {
                binInformation =  binInformations.FirstOrDefault(p=>p.PodIndex==podIndex);
            }
            if (binInformation == null) return null;
            return binInformationService.SelectById(new BinInformation(){ Id = binInformation.Id});
*/
           return binInformationService.SelectBy(new BinInformation() {Bin = new Bin {Id = binId}, PodIndex = podIndex}, new List<string>() {"Binid", "PodIndex"}).FirstOrDefault();
        }

        public List<BinInformation> GetBinInformationsByBinIdAndPodIndex(int binId,int podIndex)
        {
            IBinInformationService binInformationService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IBinInformationService>();
            if (binInformationService == null) throw new Exception("BinInformationService must be registered in service factory");
            return binInformationService.SelectBy(new BinInformation() {Bin = new Bin {Id = binId}, PodIndex = podIndex}, new List<string>() {"Binid", "PodIndex"});
        }


        public List<BinInformation> GetBinInformationByBinId(int binId)
        {
            IBinInformationService binInformationService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IBinInformationService>();
            if (binInformationService == null) throw new Exception("BinInformationService must be registered in service factory");
            return binInformationService.SelectBy(new BinInformation() {Bin = new Bin {Id = binId} }, new List<string>() {"Binid"});
        }

        public BinInformation GetBinInformationById(int Id)
        {
            IBinInformationService binInformationService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IBinInformationService>();
            if (binInformationService == null) throw new Exception("BinInformationService must be registered in service factory");

            return binInformationService.SelectById(new BinInformation() { Id = Id });
        }

        public List<BinInformation> GetBinInformations()
        {
            IBinInformationService binInformationService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IBinInformationService>();
            if (binInformationService == null) throw new Exception("BinInformationService must be registered in service factory");

            return binInformationService.SelectAll();
        }

/*
        public List<BinInformation> GetBinInformationsByCallSheetId(int callSheetId)
        {
             IBinInformationService binInformationService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IBinInformationService>();
            if (BinInformationService == null) throw new Exception("BinInformationService must be registered in service factory");
            return BinInformationService.SelectBy(new BinInformation() { CallSheet = new Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch.CallSheet { Id = callSheetId } }, new List<string>() { "CallSheetId" });
            
        }
*/

        public BinInformation CreateBinInformation(BinInformation binAssignment)
        {
             IBinInformationService binInformationService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IBinInformationService>();
            if (binInformationService == null) throw new Exception("BinInformationService must be registered in service factory");
            binInformationService.Insert(binAssignment);
            return binAssignment;
        }

        public int UpdateBinInformation(BinInformation BinInformation)
        {
             IBinInformationService binInformationService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IBinInformationService>();
            if (binInformationService == null) throw new Exception("BinInformationService must be registered in service factory");

            return binInformationService.Update(BinInformation);
        }

        public int DeleteBinInformation(BinInformation BinInformation)
        {
             IBinInformationService binInformationService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IBinInformationService>();
            if (binInformationService == null) throw new Exception("BinInformationService must be registered in service factory");

            return binInformationService.Delete(BinInformation);
        }
        //Nov 30, 2023 zhangyuan 203_PR_UpdateBinNotes: modify Add PodIndex
        public BinNote GetBinNoteByBinAndPodIndex(Bin bin,int podIndex)
        {
            IBinNoteService binNoteService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IBinNoteService>();
            if (binNoteService == null) throw new Exception("binNoteService must be registered in service factory");

            return binNoteService.SelectBy(new BinNote{Bin = bin, PodIndex = podIndex }, new List<string> {"BinId", "PodIndex" }).FirstOrDefault();
        }
        //Nov 30, 2023 zhangyuan 203_PR_UpdateBinNotes: Add func
        public List<BinNote> GetBinNoteByBins(int[] binIds)
        {
            IBinNoteService binNoteService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IBinNoteService>();
            if (binNoteService == null) throw new Exception("binNoteService must be registered in service factory");

            return binNoteService.SelectBinNoteByBins(binIds, true).Where(p => p.EffectiveEndDateTime > DateTime.Now).ToList();
        }

        public int UpdateBinNote(BinNote binNote)
        {
            IBinNoteService binNoteService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IBinNoteService>();
            if (binNoteService == null) throw new Exception("binNoteService must be registered in service factory");

            return binNoteService.Update(binNote);
        }

        public int CreateBinNote(BinNote binNote)
        {
            IBinNoteService binNoteService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IBinNoteService>();
            if (binNoteService == null) throw new Exception("binNoteService must be registered in service factory");

            return binNoteService.Insert(binNote);
        }

        public void CreateConsultant(ClientConsultant clientConsultant)
        {
            IClientConsultantService clientConsultantService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IClientConsultantService>();
            if (clientConsultantService == null) throw new Exception("clientConsultantService must be registered in service factory");
            clientConsultantService.Insert(clientConsultant);
        }

        public Collection<ClientConsultant> GetClientConsultantCollection()
        {
            IClientConsultantService clientConsultantService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IClientConsultantService>();
            if (clientConsultantService == null) throw new Exception("clientConsultantService must be registered in service factory");
            return new Collection<ClientConsultant>(clientConsultantService.SelectAll());
        }

        public int DeleteClientConsultant(ClientConsultant clientConsultant)
        {
            IClientConsultantService clientConsultantService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IClientConsultantService>();
            if (clientConsultantService == null) throw new Exception("clientConsultantService must be registered in service factory");
            return clientConsultantService.Delete(clientConsultant);
        }

        public ClientConsultant GetConsultantById(int id)
        {
            IClientConsultantService clientConsultantService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IClientConsultantService>();
            if (clientConsultantService == null) throw new Exception("clientConsultantService must be registered in service factory");
            return clientConsultantService.SelectById(new ClientConsultant{Id = id});
        }
        public List<ClientCompany> GetClientCompanyInfo()
        {
            IClientCompanyService clientCompanyService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IClientCompanyService>();
            if (clientCompanyService == null) throw new Exception("clientCompanyService must be registered in service factory");
            return clientCompanyService.SelectAll();
        }

        public List<DrillingCompany> GetDrillingCompanyInfo()
        {
            IDrillingCompanyService drillingCompanyService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IDrillingCompanyService>();
            if (drillingCompanyService == null) throw new Exception("drillingCompanyService must be registered in service factory");
            return drillingCompanyService.SelectAll();
        }

        public List<ShiftType> GetWorkShiftInfo()
        {
          IShiftTypeService shiftTypeService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IShiftTypeService>();
          if (shiftTypeService == null) throw new Exception("shiftTypeService must be registered in service factory");
          return shiftTypeService.SelectAll();
        }

        public ShiftType GetWorkShiftById(int id)
        {
            IShiftTypeService shiftTypeService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IShiftTypeService>();
            if (shiftTypeService == null) throw new Exception("shiftTypeService must be registered in service factory");

            return shiftTypeService.SelectById(new ShiftType{Id = id});
        }

        public List<ProductHaulLoad> GetProductHaulLoadByBlendSectionId(int blendSectionId)
        {
            IProductHaulLoadService productHaulLoadService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IProductHaulLoadService>();
            if (productHaulLoadService == null) throw new Exception("productHaulLoadService must be registered in service factory");

            return productHaulLoadService.SelectBy(new ProductHaulLoad{BlendSectionId = blendSectionId}, new List<string>{"BlendSectionId"});
        }

        public List<Rig> GetRigByDrillingCompanyId(int drillingCompanyId)
        {
            
            IRigService rigService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigService>();
            if (rigService == null) throw new Exception("rigService must be registered in service factory");
            return rigService.SelectBy(new Rig() {DrillingCompany = new DrillingCompany {Id = drillingCompanyId}}, new List<string>() { "DrillingCompanyId" });
        }

        public Bin GetBinById(int binId)
        {
           IBinService binService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IBinService>();
           if (binService == null) throw new Exception("binService must be registered in service factory");
           return binService.SelectById(new Bin {Id = binId});
        }

        public BlendAdditiveMeasureUnit GetBlendAdditiveMeasureUnitbyName(string name)
        {
           IBlendAdditiveMeasureUnitService blendAdditiveMeasureUnitService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IBlendAdditiveMeasureUnitService>();
           if (blendAdditiveMeasureUnitService==null) throw new Exception("blendAdditiveMeasureUnitService must be registered in service factory");
           return blendAdditiveMeasureUnitService.SelectBy(new BlendAdditiveMeasureUnit {Name = name}, new List<string>() {"Name"}).FirstOrDefault();
        }

        public RigSizeType GetRigSizeTypeById(int id)
        {
            IRigSizeTypeService rigSizeTypeService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigSizeTypeService>();
            if (rigSizeTypeService == null) throw new Exception("rigSizeTypeService must be registered in service factory");
            return rigSizeTypeService.SelectById(new RigSizeType() {Id = id});
        }

        public RigSize GetRigSizeById(int id)
        {
            IRigSizeService rigSizeService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigSizeService>();
            if (rigSizeService == null) throw new Exception("rigSizeService must be registered in service factory");
            return rigSizeService.SelectById(new RigSize { Id = id });
        }

        public ThreadType GetThreadTypeById(int id)
        {
            IThreadTypeService threadTypeService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IThreadTypeService>();
            if (threadTypeService == null) throw new Exception("threadTypeService must be registered in service factory");
            return threadTypeService.SelectById(new ThreadType() { Id = id });
        }

//        public List<ProductHaul> GetProductHaulsByCrewId(int id)
//        {
//            IProductHaulService productHaulService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IProductHaulService>();
//            if (productHaulService == null) throw new Exception("productHaulService must be registered in service factory");
//            return productHaulService.SelectBy(new ProductHaul{Crew = new Crew{Id = id}},new List<string>(){"CrewId"} );
//        }

        public DrillingCompany GetDrillingCompanyById(int id)
        {
            IDrillingCompanyService drillingCompanyService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IDrillingCompanyService>();
            if (drillingCompanyService == null) throw new Exception("drillingCompanyService must be registered in service factory");
            return drillingCompanyService.SelectById(new DrillingCompany() { Id = id });
        }

        #endregion

        public List<ProductHaulLoad> GetProductHaulLoadCollectionByServicePoint(int servicePointId, ProductHaulLoadStatus productHaulLoadStatus)
        {
            IProductHaulLoadService productHaulLoadService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IProductHaulLoadService>();
            if (productHaulLoadService==null) throw new Exception("productHaulLoadService must be registered in service factory");
            {
                if (servicePointId == 0)
                    return productHaulLoadService.SelectByProductHaulLoadLifeStatus((int) productHaulLoadStatus);
                else
                    return productHaulLoadService.SelectByProductHaulLoadLifeStatusServicePoint((int)productHaulLoadStatus, servicePointId);
            }
        }

        public List<ProductHaulLoad> GetProductHaulLoadCollectionByServicePoint(int servicePointId, ProductHaulLoadStatus[] productHaulLoadLifeStatus)
        {
            IProductHaulLoadService productHaulLoadService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IProductHaulLoadService>();
            if (productHaulLoadService==null) throw new Exception("productHaulLoadService must be registered in service factory");
            {
                return productHaulLoadService.SelectBy(new ProductHaulLoad {ServicePoint = new ServicePoint() {Id = servicePointId}}, p=>productHaulLoadLifeStatus.Contains(p.ProductHaulLoadLifeStatus));
            }
        }

        public BinLoadHistory CreateBinLoadHistory(BinLoadHistory binLoadHistory)
        {
            IBinLoadHistoryService binLoadHistoryService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IBinLoadHistoryService>();
            if (binLoadHistoryService == null) throw new Exception("IBinLoadHistoryService must be registered in service factory");
            binLoadHistoryService.Insert(binLoadHistory);
            return binLoadHistory;
        }

        public List<BlendChemical> GetBlendChemicals()
        {
            IBlendChemicalService blendChemicalService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IBlendChemicalService>();
            if (blendChemicalService == null) throw new Exception("blendChemicalService must be registered in service factory");

            return blendChemicalService.SelectAll(true);
        }

        public SanjelCompany GetSanjelCompanyById(int id)
        {
                ISanjelCompanyService sanjelCompanyService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ISanjelCompanyService>();
                if (sanjelCompanyService == null) throw new Exception("sanjelCompanyService must be registered in service factory");
                return sanjelCompanyService.SelectById(id, DateTime.Now);
        }

        public void CreateDrillingCompany(DrillingCompany sanjel)
        {
            IDrillingCompanyService drillingCompanyService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IDrillingCompanyService>();
            if (drillingCompanyService == null) throw new Exception("drillingCompanyService must be registered in service factory");
            drillingCompanyService.Insert(sanjel);
            return;
        }

        public DrillingCompany GetDrillingCompanyByShortName(string sanjel)
        {
            IDrillingCompanyService drillingCompanyService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IDrillingCompanyService>();
            if (drillingCompanyService == null) throw new Exception("drillingCompanyService must be registered in service factory");
            return drillingCompanyService.SelectBy(new DrillingCompany() {ShortName = sanjel},
                new List<string>() {"ShortName"}).FirstOrDefault();
        }

        public ClientCompany GetClientCompanyByName(string sanjel)
        {
            IClientCompanyService drillingCompanyService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IClientCompanyService>();
            if (drillingCompanyService == null) throw new Exception("drillingCompanyService must be registered in service factory");
            return drillingCompanyService.SelectBy(new ClientCompany() {Name = sanjel},
                new List<string>() {"Name"}).FirstOrDefault();
        }


        public Program GetProgramByProgramId(string programId)
        {
            IProgramService programService = ServiceFactory.Instance.GetService(typeof(IProgramService)) as IProgramService;

            if (programService == null) throw new Exception("drillingCompanyService must be registered in service factory");
            return programService.GetProgramByProgramId(programId).OrderByDescending(p=>p.Revision).FirstOrDefault();
        }

        public JobDesign GetJobDesignByProgramId(string programId)
        {
            IJobDesignService programService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IJobDesignService>();

            if (programService == null) throw new Exception("programService must be registered in service factory");
            return programService.SelectByJoin(p=>p.ProgramId == programId, s=>s.JobDesignPumpingJobSection!=null).OrderByDescending(p=>p.Revision).FirstOrDefault();
        }

        public JobDesign GetJobDesignByProgramIdAndRevision(string programId,int revision)
        {
            IJobDesignService programService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IJobDesignService>();

            if (programService == null) throw new Exception("programService must be registered in service factory");
            return programService.SelectByJoin(p => p.ProgramId == programId && p.Revision == revision, s => s.JobDesignPumpingJobSection != null).OrderByDescending(p => p.Revision).FirstOrDefault();
        }


        public Bin GetBinByNumber(string binNumber)
        {
            IBinService binService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IBinService>();
            if (binService == null) throw new Exception("binService must be registered in service factory");
            return binService.SelectBy(new Bin {Name= binNumber}, new List<string>(){"Name"}).FirstOrDefault();
        }

        public Sesi.SanjelData.Entities.BusinessEntities.BaseEntites.BlendSection GetProgramBlendSectionByBlendSectionId(int blendSectionId)
        {
            DateTime startDateTime = DateTime.Now;

            if (blendSectionId <= 0) return null;
            IJobDesignBlendSectionService jobDesignBlendSectionService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IJobDesignBlendSectionService>();

            JobDesignBlendSection jobDesignBlendSection = null;
            if (jobDesignBlendSectionService == null) return null;
            try
            {
//                jobDesignBlendSection = jobDesignBlendSectionService.SelectByJoin(p=>p.Id==blendSectionId, (blend, table)=>blend.JobDesignBlendAdditiveSections != null && table.Name=="PRG_BLEND_BLEND_ADDTV_SCTNS").FirstOrDefault();
                jobDesignBlendSection = jobDesignBlendSectionService.SelectByJoin(p=>p.Id==blendSectionId, blend=>blend.JobDesignBlendAdditiveSections != null ).FirstOrDefault();
            }
            catch (Exception ex)
            {
                ;
            }

            Debug.WriteLine("\tGetProgramBlendSectionByBlendSectionId - {0,21}", DateTime.Now.Subtract(startDateTime));

            Sesi.SanjelData.Entities.BusinessEntities.BaseEntites.BlendSection blendSection = new BlendSection();
            if (jobDesignBlendSection != null)
            {
                blendSection = (BlendSection)jobDesignBlendSection;
                if (blendSection.BlendAdditiveSections == null)
                    blendSection.BlendAdditiveSections =
                        new List<Sesi.SanjelData.Entities.BusinessEntities.BaseEntites.BlendAdditiveSection>();

                foreach (var callSheetBlendAdditiveSection in jobDesignBlendSection.JobDesignBlendAdditiveSections)
                {
                    blendSection.BlendAdditiveSections.Add(
                        (Sesi.SanjelData.Entities.BusinessEntities.BaseEntites.BlendAdditiveSection)
                        callSheetBlendAdditiveSection);
                }

            }

            return blendSection;
        }

        /*
        public BinInformation GetBinInformationByBinNumber(string binNumber)
        {
            Bin bin = GetBinByNumber(binNumber);
            if (bin == null) return null;
            BinInformation binInformation = GetBinInformationByBinIdAndPodIndex(bin.Id,1);
            return binInformation;
        }
        */

        public List<RigJob> GetRigJobsByQuery(Pager pager, Expression<Func<RigJob, bool>> query)
        {
            IRigJobService rigJobService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigJobService>();
            if (rigJobService == null) throw new Exception("rigJobService must be registered in service factory");
            return pager!=null ? rigJobService.SelectBy(pager,new RigJob(), query) : rigJobService.SelectBy(new RigJob(), query);
        }

        public List<CallSheetBlendSection> GetTestingCallSheetBlendSection()
        {
            ICallSheetBlendSectionService callSheetBlendSectionService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ICallSheetBlendSectionService>();
            if (callSheetBlendSectionService == null) throw new Exception("callSheetBlendSectionService must be registered in service factory");

            Expression<Func<CallSheetBlendSection, bool>> filter = c => c.IsNeedFieldTesting == true && c.Quantity > 0;

            return callSheetBlendSectionService.SelectBy(new CallSheetBlendSection(), filter);
        }

        public List<BinInformation> GetBinInformationByQuery(Expression<Func<BinInformation, bool>> query)
        {
            IBinInformationService binInformationService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IBinInformationService>();
            if (binInformationService == null) throw new Exception("binInformationService must be registered in service factory");

            return binInformationService.SelectBy(new BinInformation(),  query);
        }

        
        public List<BinInformation> GetBinInformationByRigIds(int[] rigIds)
        {
            IBinInformationService binInformationService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IBinInformationService>();
            if (binInformationService == null) throw new Exception("rigJobService must be registered in service factory");
            return binInformationService.SelectBinInformationByRigs(rigIds, true).Where(p=>p.EffectiveEndDateTime>DateTime.Now).ToList();
        }

        public List<JobType> GetJobTypes()
        {
            IJobTypeService jobTypeService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IJobTypeService>();
            if (jobTypeService == null) throw new Exception("jobTypeService must be registered in service factory");

            return jobTypeService.SelectAll();
        }

        public List<JobIntervalType> GetJobIntervals()
        {
            IJobIntervalTypeService jobIntervalTypeService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IJobIntervalTypeService>();
            if (jobIntervalTypeService == null) throw new Exception("jobIntervalTypeService must be registered in service factory");

            return jobIntervalTypeService.SelectAll();
        }

        public int UpdateJobType(JobType jobType)
        {
            IJobTypeService jobTypeService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IJobTypeService>();
            if (jobTypeService == null) throw new Exception("jobTypeService must be registered in service factory");

            return jobTypeService.Update(jobType);
        }

        public List<SanjelCrewSchedule> GetFutureCrewSchedules(int crewId, DateTime dateTime)
        {
            ISanjelCrewScheduleService crewScheduleService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ISanjelCrewScheduleService>();
            if (crewScheduleService == null) throw new Exception("crewScheduleService must be registered in service factory");
            return crewScheduleService.SelectBy(new SanjelCrewSchedule(), sanjelCrewSchedule=> sanjelCrewSchedule.SanjelCrew.Id== crewId && sanjelCrewSchedule.StartTime> dateTime);
        }

        public List<ProductHaulLoad> GetProductHaulLoadsByBulkPlantAndStatus(int bulkPlantId, ProductHaulLoadStatus status)
        {
            IProductHaulLoadService productHaulLoadService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IProductHaulLoadService>();
            if (productHaulLoadService == null) throw new Exception("productHaulLoadService must be registered in service factory");

            return productHaulLoadService.SelectByBulkPlantProductHaulLoadLifeStatus(bulkPlantId, (int)status);
        }

        public List<ProductHaulLoad> GetScheduledProductHaulLoadsByBulkPlant(int bulkPlantId)
        {
            return GetProductHaulLoadsByBulkPlantAndStatus(bulkPlantId, ProductHaulLoadStatus.Scheduled);
        }

        public List<ProductHaulLoad> GetProductHaulLoadsBy(int bulkPlantId, Collection<int> productHaulLoadLifeStatutsCollection)
        {
            IProductHaulLoadService productHaulLoadService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IProductHaulLoadService>();
            if (productHaulLoadService == null) throw new Exception("productHaulLoadService must be registered in service factory");

            return productHaulLoadService.SelectBy(new ProductHaulLoad(), productHaulLoad=>productHaulLoad.BulkPlant.Id==bulkPlantId &&  productHaulLoadLifeStatutsCollection.Contains((int)productHaulLoad.ProductHaulLoadLifeStatus));
        }
        public List<ProductHaulLoad> GetProductHaulLoadByQuery(Expression<Func<ProductHaulLoad, bool>> query)
        {
            IProductHaulLoadService productHaulLoadService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IProductHaulLoadService>();
            if (productHaulLoadService == null) throw new Exception("productHaulLoadService must be registered in service factory");

            return productHaulLoadService.SelectBy(new ProductHaulLoad(), query);
        }

        public List<RigJobSanjelCrewSection> GetRigJobSanjelCrewSectionsByQuery(Expression<Func<RigJobSanjelCrewSection, bool>> query)
        {
            IRigJobSanjelCrewSectionService rigJobSanjelCrewSectionService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigJobSanjelCrewSectionService>();
            if (rigJobSanjelCrewSectionService == null) throw new Exception("rigJobSanjelCrewSectionService must be registered in service factory");

            return rigJobSanjelCrewSectionService.SelectBy(new RigJobSanjelCrewSection(), query);
        }
        public List<BlendUnloadSheet> GetBlendUnloadSheetByQuery(Expression<Func<BlendUnloadSheet, bool>> query)
        {
            IBlendUnloadSheetService blendUnloadSheetService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IBlendUnloadSheetService>();
            if (blendUnloadSheetService == null) throw new Exception("productHaulLoadService must be registered in service factory");

            return blendUnloadSheetService.SelectBy(new BlendUnloadSheet(), query);
        }

        public List<BlendUnloadSheet> GetBlendUnloadSheetByshippingLoadSheetId(int shippingLoadSheetId)
        {
            IBlendUnloadSheetService blendUnloadSheetService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IBlendUnloadSheetService>();
            if (blendUnloadSheetService == null) throw new Exception("productHaulLoadService must be registered in service factory");

            return blendUnloadSheetService.SelectByShippingLoadSheet(shippingLoadSheetId);
        }


        public int CreateBlendTankLog(BlendTankLog blendTankLog)
        {
            IBlendTankLogService blendTankLogService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IBlendTankLogService>();
            if (blendTankLogService == null) throw new Exception("blendTankLogService must be registered in service factory");

            return blendTankLogService.Insert(blendTankLog, true);
        }
        public BlendTankLog GetBlendTankLogById(int blendTankLogId)
        {
            IBlendTankLogService blendTankLogService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IBlendTankLogService>();
            if (blendTankLogService == null) throw new Exception("blendTankLogService must be registered in service factory");

            return blendTankLogService.SelectById(new BlendTankLog { Id = blendTankLogId });
        }

        #region Product Hauls
        public List<ShippingLoadSheet> GetShippingLoadSheetByProductHaulLoads(List<int> productHaulLoadIds)
        {
            IShippingLoadSheetService shippingLoadSheetService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IShippingLoadSheetService>();
            if (shippingLoadSheetService == null) throw new Exception("podLoadService must be registered in service factory");
            return shippingLoadSheetService.SelectBy(new ShippingLoadSheet(), shippingLoadSheet=>productHaulLoadIds.Contains(shippingLoadSheet.ProductHaulLoad.Id));
        }
        public List<ShippingLoadSheet> GetShippingLoadSheetByProductHaulLoadId(int productHaulLoadId)
        {
            IShippingLoadSheetService shippingLoadSheetService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IShippingLoadSheetService>();
            if (shippingLoadSheetService == null) throw new Exception("podLoadService must be registered in service factory");
            return shippingLoadSheetService.SelectBy(new ShippingLoadSheet(), shippingLoadSheet=>shippingLoadSheet.ProductHaulLoad.Id == productHaulLoadId);
        }
        public List<ShippingLoadSheet> GetShippingLoadSheetByProductHaulId(int productHaulId)
        {
            IShippingLoadSheetService shippingLoadSheetService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IShippingLoadSheetService>();
            if (shippingLoadSheetService == null) throw new Exception("podLoadService must be registered in service factory");
            return shippingLoadSheetService.SelectByJoin(p=>p.ProductHaul.Id==productHaulId, s=>s.BlendUnloadSheets!=null);
        }
        public ShippingLoadSheet GetShippingLoadSheetById(int Id,bool isAggregatedChildren=false)
        {
            IShippingLoadSheetService shippingLoadSheetService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IShippingLoadSheetService>();
            if (shippingLoadSheetService == null) throw new Exception("podLoadService must be registered in service factory");
            return shippingLoadSheetService.SelectById(new ShippingLoadSheet(){ Id=Id},isAggregatedChildren);
        }
        public void DeleteShippingLoadSheet(ShippingLoadSheet shippingLoadSheet)
        {
            IShippingLoadSheetService shippingLoadSheetService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IShippingLoadSheetService>();
            if (shippingLoadSheetService == null) throw new Exception("podLoadService must be registered in service factory");
            shippingLoadSheetService.Delete(shippingLoadSheet,true);
        }
        public List<ProductHaulLoad> GetProductHaulLoadByIds(List<int> productHaulLoadIds)
        {
            IProductHaulLoadService productHaulLoadService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IProductHaulLoadService>();
            if (productHaulLoadService == null) throw new Exception("productHaulLoadService must be registered in service factory");

                return productHaulLoadService.SelectBy(new ProductHaulLoad(), productHaulLoad=>productHaulLoadIds.Contains(productHaulLoad.Id));
        }
        
        public List<ProductHaul> GetProductHaulByIds(List<int> productHaulIds)
        {
            IProductHaulService productHaulService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IProductHaulService>();
            if (productHaulService == null) throw new Exception("productHaulService must be registered in service factory");

                return productHaulService.SelectBy(new ProductHaul(), productHaul=>productHaulIds.Contains(productHaul.Id));
        }
        #endregion Product Hauls
        public List<PodLoad> GetPodLoadsByProductHaul(int productHaulId)
        {
            IPodLoadService podLoadService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IPodLoadService>();
            if (podLoadService == null) throw new Exception("podLoadService must be registered in service factory");

            return podLoadService.SelectByProductHaul(productHaulId);
        }

        public void CreateShippingLoadSheet(ShippingLoadSheet shippingLoadSheet)
        {
            IShippingLoadSheetService shippingLoadSheetService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IShippingLoadSheetService>();
            if (shippingLoadSheetService == null) throw new Exception("podLoadService must be registered in service factory");
            shippingLoadSheetService.Insert(shippingLoadSheet,true);
        }
        public void CreatePodLoad(PodLoad podLoad)
        {
            IPodLoadService podLoadService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IPodLoadService>();
            if (podLoadService == null) throw new Exception("podLoadService must be registered in service factory");
            podLoadService.Insert(podLoad);
        }
        public void  UpdatePodLoad(PodLoad podLoad)
        {
            IPodLoadService podLoadService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IPodLoadService>();
            if (podLoadService == null) throw new Exception("podLoadService must be registered in service factory");
            podLoadService.Update(podLoad);
        }

        public void UpdateShippingLoadSheet(ShippingLoadSheet shippingLoadSheet, bool isUpdateChildren = true)
        {
            IShippingLoadSheetService shippingLoadSheetService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IShippingLoadSheetService>();
            if (shippingLoadSheetService == null) throw new Exception("podLoadService must be registered in service factory");
            shippingLoadSheetService.Update(shippingLoadSheet, isUpdateChildren);
        }

        public List<ShippingLoadSheet> GetShippingLoadSheetsByDestinationStorage(BinInformation assignedBinSection)
        {
            IBlendUnloadSheetService blendUnloadSheetService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IBlendUnloadSheetService>();
            if (blendUnloadSheetService == null) throw new Exception("podLoadService must be registered in service factory");
            List<BlendUnloadSheet> blendUnloadSheets = blendUnloadSheetService.SelectByDestinationStorage(assignedBinSection.Id);

            if (blendUnloadSheets.Count > 0)
            {
                List<int> shippingLoadSheetIds =
                    blendUnloadSheets.Select(p => p.ShippingLoadSheet.Id).Distinct().ToList();

                return GetShippingLoadSheetsByIds(shippingLoadSheetIds);
            }

            return null;
        }

        public List<ShippingLoadSheet> GetShippingLoadSheetsByIds(List<int> shippingLoadSheetIds)
        {
            IShippingLoadSheetService shippingLoadSheetService = MetaShare.Common.Core.CommonService.ServiceFactory
                .Instance.GetService<IShippingLoadSheetService>();
            if (shippingLoadSheetService == null)
                throw new Exception("shippingLoadSheetService must be registered in service factory");
            return shippingLoadSheetService.SelectBy(new ShippingLoadSheet(),
                shippingLoadSheet => shippingLoadSheetIds.Contains(shippingLoadSheet.Id));
        }

        public List<ShippingLoadSheet> GetShippingLoadSheetsBySourceStorageIds(List<int> storageIds)
        {
            IShippingLoadSheetService shippingLoadSheetService = MetaShare.Common.Core.CommonService.ServiceFactory
                .Instance.GetService<IShippingLoadSheetService>();
            if (shippingLoadSheetService == null)
                throw new Exception("shippingLoadSheetService must be registered in service factory");
            return shippingLoadSheetService.SelectBy(new ShippingLoadSheet(),
                shippingLoadSheet => storageIds.Contains(shippingLoadSheet.SourceStorage.Id) && shippingLoadSheet.ProductHaul.Id !=0);
        }
        public List<ShippingLoadSheet> GetNotOnLocationShippingLoadSheetsBySourceStorageIds(List<int> storageIds)
        {
	        IShippingLoadSheetService shippingLoadSheetService = MetaShare.Common.Core.CommonService.ServiceFactory
		        .Instance.GetService<IShippingLoadSheetService>();
	        if (shippingLoadSheetService == null)
		        throw new Exception("shippingLoadSheetService must be registered in service factory");
	        return shippingLoadSheetService.SelectBy(new ShippingLoadSheet(),
		        shippingLoadSheet => storageIds.Contains(shippingLoadSheet.SourceStorage.Id) && shippingLoadSheet.ShippingStatus != ShippingStatus.OnLocation);
        }
        public List<ShippingLoadSheet> GetShippingLoadSheetByRigIds(List<int> rigIds)
        {
	        IShippingLoadSheetService shippingLoadSheetService = MetaShare.Common.Core.CommonService.ServiceFactory
		        .Instance.GetService<IShippingLoadSheetService>();
	        if (shippingLoadSheetService == null)
		        throw new Exception("shippingLoadSheetService must be registered in service factory");
	        return shippingLoadSheetService.SelectBy(new ShippingLoadSheet(),
		        shippingLoadSheet => rigIds.Contains(shippingLoadSheet.Rig.Id));
        }
        //Jan 2, 2024 zhangyuan 244_PR_AddBulkers: Add ShippingLoadSheets data acquisition
        public List<ShippingLoadSheet> GetShippingLoadSheetsBySourceStorageId(int sourceStorageId)
        {
            IShippingLoadSheetService shippingLoadSheetService = MetaShare.Common.Core.CommonService.ServiceFactory
                .Instance.GetService<IShippingLoadSheetService>();
            if (shippingLoadSheetService == null)
                throw new Exception("shippingLoadSheetService must be registered in service factory");
            return shippingLoadSheetService.SelectByJoin(
                shippingLoadSheet => shippingLoadSheet.SourceStorage.Id == sourceStorageId &&
                                     shippingLoadSheet.ProductHaul.Id > 0,
                q => q.ProductHaul != null&& q.SourceStorage != null);
        }


        public Collection<PurchasePrice> GetPurchasePricesAsOfDate(DateTime effectiveDateTime)
        {
            IPurchasePriceService purchasePriceService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IPurchasePriceService>();
            return new Collection<PurchasePrice>(purchasePriceService.SelectAllByDateTime(effectiveDateTime));
        }

        /*
        public Job GetJobByUniqueId(string jobUniqueId)
        {
            if (string.IsNullOrEmpty(jobUniqueId)) return null;

            Sanjel.Services.Interfaces.IJobService jobService = MetaShare.Common.ServiceModel.Services.ServiceFactory.Instance.GetService(typeof(Sanjel.Services.Interfaces.IJobService)) as Sanjel.Services.Interfaces.IJobService;
            if (jobService == null) throw new Exception("jobService must be registered in service factory");

            return jobService?.GetJobByUniqueId(jobUniqueId);
        }
        */

        public Program GetProgramById(int programId)
        {
            if (programId <= 0) return null;

            IProgramService programService = ServiceFactory.Instance.GetService(typeof(IProgramService)) as IProgramService;
            if (programService == null) throw new Exception("programService must be registered in service factory");

            return programService?.GetProgramById(programId);
        }

        public ServiceReport GetServiceReportByUniqueId(string uniqueId)
        {
            if (string.IsNullOrEmpty(uniqueId)) return null;

            IServiceReportService serviceReportrService = ServiceFactory.Instance.GetService(typeof(IServiceReportService)) as IServiceReportService;
            if (serviceReportrService == null) throw new Exception("serviceReportrService must be registered in service factory");

            return serviceReportrService?.GetServiceReportByUniqueId(uniqueId);
        }

        public (decimal, decimal) GetGeoLocationByNormalizedLocation(string wellLocation)
        {
            ILocationLookupService locationLookupService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance
                .GetService<ILocationLookupService>();
            if (locationLookupService == null)
                throw new Exception("locationLookupService must be registered in service factory");
            var location = locationLookupService.SelectBy(new LocationLookup(),
                locationLookup => locationLookup.WellLocation.Contains(wellLocation)).FirstOrDefault();

            if (location != null)
            {
                return (location.Latitude, location.Longitude);
            }

            return (0, 0);
        }


        public void CreateUnitLocation(UnitLocation unitLocation)
        {
            IUnitLocationService unitLocationService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IUnitLocationService>();
            if (unitLocationService == null) throw new Exception("podLoadService must be registered in service factory");
            unitLocationService.Insert(unitLocation);
        }

        public List<ProductHaul> GetProductHaulByQuery(Expression<Func<ProductHaul, bool>> query)
        {
            IProductHaulService productHaulService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IProductHaulService>();
            if (productHaulService == null) throw new Exception("productHaulLoadService must be registered in service factory");

            return productHaulService.SelectBy(new ProductHaul(), query);
        }

        public List<ShippingLoadSheet> GetShippingLoadSheetsByCallSheetNumber(int callSheetNumber)
        {
            IShippingLoadSheetService shippingLoadSheetService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IShippingLoadSheetService>();
            if (shippingLoadSheetService == null) throw new Exception("podLoadService must be registered in service factory");
            return shippingLoadSheetService.SelectBy(new ShippingLoadSheet(), shippingLoadSheet=>shippingLoadSheet.CallSheetNumber == callSheetNumber);
        }

        public List<ThirdPartyBulkerCrew> GetThirdPartyBulkerCrewsByServicePoints(Collection<int> servicePoints)
        {
            IThirdPartyBulkerCrewService thirdPartyBulkerCrewService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IThirdPartyBulkerCrewService>();
            if (thirdPartyBulkerCrewService == null) throw new Exception("thirdPartyBulkerCrewService must be registered in service factory");
            List<ThirdPartyBulkerCrew> thirdPartyCrews = new List<ThirdPartyBulkerCrew>();
            if(servicePoints.Count>0)
                thirdPartyCrews = thirdPartyBulkerCrewService.SelectBy(new ThirdPartyBulkerCrew(), p => servicePoints.Contains(p.ServicePoint.Id));
            else
            {
                thirdPartyCrews = thirdPartyBulkerCrewService.SelectAll();
            }
            return thirdPartyCrews;
        }

        public List<CallSheetBlendSection> GetBlendSectionsByCallSheetAndBlend(int callSheetNumber)
        {
            ICallSheetService callSheetService= MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ICallSheetService>();


            var callSheet = callSheetService.SelectBy(
                new Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch.CallSheet(),
                p => p.CallSheetNumber == callSheetNumber).FirstOrDefault();
            if (callSheet != null)
            {
                ICallSheetBlendSectionService callSheetBlendSectionService = MetaShare.Common.Core.CommonService
                    .ServiceFactory.Instance.GetService<ICallSheetBlendSectionService>();

                if (callSheetBlendSectionService == null) return null;
                try
                {
                   return callSheetBlendSectionService.SelectByJoin(p => p.CallSheet.Id == callSheet.Id,
                        blend => blend.CallSheetBlendAdditiveSections != null);


                }
                catch (Exception ex)
                {
                    return null;
                }
            }

            return null;
        }

        public IList<JobDesignBlendSection> GetBlendSectionByJobDesignPumpingSectionId(int id)
        {
            IJobDesignBlendSectionService jobDesignBlendSectionService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IJobDesignBlendSectionService>();

            if (jobDesignBlendSectionService == null) throw new Exception("programService must be registered in service factory");
            return jobDesignBlendSectionService.SelectBy(new JobDesignBlendSection(), p=>p.JobDesignPumpingJobSection.Id == id);
        }

        public List<ShippingLoadSheet> GetShippingLoadSheetsByStatus(ShippingStatus status)
        {
            IShippingLoadSheetService shippingLoadSheetService = MetaShare.Common.Core.CommonService.ServiceFactory
                .Instance.GetService<IShippingLoadSheetService>();
            if (shippingLoadSheetService == null)
                throw new Exception("shippingLoadSheetService must be registered in service factory");
            return shippingLoadSheetService.SelectBy(new ShippingLoadSheet(),
                shippingLoadSheet => shippingLoadSheet.ShippingStatus ==status);
        }
        public List<RigJobThirdPartyBulkerCrewSection> GetActiveThirdPartyBulkerCrewAssignmentByCrewId(int crewId)
        {
            IRigJobThirdPartyBulkerCrewSectionService rigJobThirdPartyBulkerCrewSectionService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigJobThirdPartyBulkerCrewSectionService>();
            if (rigJobThirdPartyBulkerCrewSectionService == null) throw new Exception("rigJobThirdPartyBulkerCrewSectionService must be registered in service factory");

            return rigJobThirdPartyBulkerCrewSectionService.SelectByJoin(p => p.ThirdPartyBulkerCrew.Id == crewId && p.RigJobCrewSectionStatus != RigJobCrewSectionStatus.Removed && p.RigJobCrewSectionStatus != RigJobCrewSectionStatus.LogOffDuty &&
                                                                              p.ThirdPartyBulkerCrew.Id != 0 && p.ProductHaul.Id != 0,
                q => q.ProductHaul != null && q.ThirdPartyBulkerCrew != null);

        }
        public BulkerCrewLog GetThirdPartyBulkerCrewLog(int thirdPartyBulkerCrewId)
        {
            IBulkerCrewLogService bulkerCrewLogService = MetaShare.Common.Core.CommonService.ServiceFactory
                .Instance.GetService<IBulkerCrewLogService>();

            if (bulkerCrewLogService == null)
                throw new Exception("bulkerCrewLogService must be registered in service factory");

            return bulkerCrewLogService.SelectByThirdPartyBulkerCrew(thirdPartyBulkerCrewId).FirstOrDefault();
        }
        public int UpdateBulkerCrewLog(BulkerCrewLog bulkerCrewLog)
        {
            IBulkerCrewLogService bulkerCrewLogService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IBulkerCrewLogService>();

            if (bulkerCrewLogService == null) throw new Exception("bulkerCrewLogService must be registered in service factory");

            return bulkerCrewLogService.Update(bulkerCrewLog, false);
        }
        public List<RigJobSanjelCrewSection> GetActiveBulkerCrewAssignmentByCrewId(int crewId)
        {
            IRigJobSanjelCrewSectionService rigJobSanjelCrewSectionService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigJobSanjelCrewSectionService>();
            if (rigJobSanjelCrewSectionService == null) throw new Exception("rigJobSanjelCrewSectionService must be registered in service factory");

            return rigJobSanjelCrewSectionService.SelectByJoin(p => p.SanjelCrew.Id == crewId && p.RigJobCrewSectionStatus != RigJobCrewSectionStatus.Removed && p.RigJobCrewSectionStatus != RigJobCrewSectionStatus.LogOffDuty &&
                                                                    p.SanjelCrew.Id != 0 && p.ProductHaul.Id != 0,
                q => q.ProductHaul != null && q.SanjelCrew != null);

        }
        public BulkerCrewLog GetBulkerCrewLog(int sanjelCrewId)
        {
            IBulkerCrewLogService bulkerCrewLogService = MetaShare.Common.Core.CommonService.ServiceFactory
                .Instance.GetService<IBulkerCrewLogService>();

            if (bulkerCrewLogService == null)
                throw new Exception("bulkerCrewLogService must be registered in service factory");

            return bulkerCrewLogService.SelectBySanjelCrew(sanjelCrewId).FirstOrDefault();
        }

        public int UpdateRigJobSanjelCrewSection(RigJobSanjelCrewSection rigJobSanjelCrewSection)
        {
            IRigJobSanjelCrewSectionService rigJobSanjelCrewSectionService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigJobSanjelCrewSectionService>();
            if (rigJobSanjelCrewSectionService == null) throw new Exception("rigJobSanjelCrewSectionService must be registered in service factory");
            return rigJobSanjelCrewSectionService.Update(rigJobSanjelCrewSection);
        }
        public List<SanjelCrew> GetPlannedBulkerCrewList(DateTime onLocationTime)
        {
            var currentDateTime = onLocationTime == DateTime.MinValue ? DateTime.Now : onLocationTime;
            ISanjelCrewScheduleService crewScheduleService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ISanjelCrewScheduleService>();
            if (crewScheduleService == null) throw new Exception("crewService must be registered in service factory");

            var plannedCrewSchedules = crewScheduleService.SelectBy(new SanjelCrewSchedule(), p =>
                p.StartTime <= currentDateTime && p.EndTime > currentDateTime && p.Type == CrewScheduleType.Planned);
            var plannedCrewIds = plannedCrewSchedules.Select(p => p.SanjelCrew.Id).Distinct().ToList();
            var bulkerCrewlist = CacheData.SanjelCrews.Where(p => p.Type.Id == 2 && plannedCrewIds.Contains(p.Id))
                .ToList();
            return bulkerCrewlist;
        }

        public int CreateServicePoint(ServicePoint  servicePoint)
        {
            IServicePointService servicePointService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IServicePointService>();

            if (servicePointService == null) throw new Exception("servicePointService must be registered in service factory");

            return servicePointService.Insert(servicePoint, false);
        }


        public int DeleteServicePoint(int servicePointId)
        {
            IServicePointService servicePointService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IServicePointService>();

            if (servicePointService == null) throw new Exception("servicePointService must be registered in service factory");

            return servicePointService.Delete(new ServicePoint { Id = servicePointId }); 

        }

        //public ProductHaul GetProductHaulByCrew(int crewId)
        //{
        //    IProductHaulService productHaulService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IProductHaulService>();
        //    if (productHaulService == null) throw new Exception("productHaulService must be registered in service factory");

        //    return productHaulService.SelectByCrew(crewId).FirstOrDefault();

        //}


        public List<ProductHaul> GetProductHaulByCrew(int crewId)
        {
            IProductHaulService productHaulService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IProductHaulService>();
            if (productHaulService == null) throw new Exception("productHaulService must be registered in service factory");

            return productHaulService.SelectByCrew(crewId).ToList();

        }


        public List<Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch.CallSheet> SelecCallSheetById(int id )
        {
            ICallSheetService callSheetService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ICallSheetService>();

            if (callSheetService == null) throw new Exception("productHaulService must be registered in service factory");

            return callSheetService.SelectAll().Where(p => p.Id == id).ToList();
        }

        public List<CallSheetBlendSection> SelectAllBlendSection()
        {
            ICallSheetBlendSectionService blendSectionService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<ICallSheetBlendSectionService>();

            if (blendSectionService == null) throw new Exception("blendSectionService must be registered in service factory");

            return blendSectionService.SelectAll().ToList();
        }

        public List<ProductHaul> GetProductHaulListByBulkPlant(int bulkPlantId,
	        ProductHaulStatus productHaulStatus = ProductHaulStatus.Empty)
        {
            IProductHaulService productHaulService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance
                .GetService<IProductHaulService>();
            if (productHaulService == null)
                throw new Exception("productHaulService must be registered in service factory");
            if(productHaulStatus == ProductHaulStatus.Empty)
	            return productHaulService.SelectByJoin(p => p.BulkPlant.Id == bulkPlantId, q => q.ShippingLoadSheets != null);
            else
	            return productHaulService.SelectByJoin(p => p.BulkPlant.Id == bulkPlantId && p.ProductHaulLifeStatus == productHaulStatus, q => q.ShippingLoadSheets != null);
        }
        public List<BlendUnloadSheet> GetBlendUnloadSheetByshippingLoadSheetIds(int[] shippingLoadSheetIds)
        {
            IBlendUnloadSheetService blendUnloadSheetService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IBlendUnloadSheetService>();
            if (blendUnloadSheetService == null) throw new Exception("productHaulLoadService must be registered in service factory");

            return blendUnloadSheetService.SelectBlendUnloadSheetByShippingLoadSheets(shippingLoadSheetIds,true).FindAll(p=>p.IsEffective);
        }
        public List<ShippingLoadSheet> GetShippingLoadSheetsBySourceStorageIdAndStatus(int binInformationId, ShippingStatus scheduled)
        {
	        IShippingLoadSheetService shippingLoadSheetService = MetaShare.Common.Core.CommonService.ServiceFactory
		        .Instance.GetService<IShippingLoadSheetService>();
	        if (shippingLoadSheetService == null)
		        throw new Exception("shippingLoadSheetService must be registered in service factory");
	        return shippingLoadSheetService.SelectBy(new ShippingLoadSheet(),
		        shippingLoadSheet => shippingLoadSheet.SourceStorage.Id == binInformationId && shippingLoadSheet.ShippingStatus == scheduled);
        }
        public List<ShippingLoadSheet> GetShippingLoadSheetsByProductHaulLoadId(int productHaulLoadId, ShippingStatus scheduled)
        {
            IShippingLoadSheetService shippingLoadSheetService = MetaShare.Common.Core.CommonService.ServiceFactory
                .Instance.GetService<IShippingLoadSheetService>();
            if (shippingLoadSheetService == null)
                throw new Exception("shippingLoadSheetService must be registered in service factory");
            return shippingLoadSheetService.SelectBy(new ShippingLoadSheet(),
                shippingLoadSheet => shippingLoadSheet.ProductHaulLoad.Id == productHaulLoadId && shippingLoadSheet.ShippingStatus == scheduled);
        }

        public int CreateBlendSample(BlendSample blendSample)
        {
	        IBlendSampleService blendSampleService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IBlendSampleService>();
	        if (blendSampleService == null) throw new Exception("blendSampleService must be registered in service factory");

	        return blendSampleService.Insert(blendSample);
        }
        public int UpdateBlendSample(BlendSample blendSample)
        {
	        IBlendSampleService blendSampleService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IBlendSampleService>();
	        if (blendSampleService == null) throw new Exception("blendSampleService must be registered in service factory");

	        return blendSampleService.Update(blendSample);
        }

        public List<BlendSample> GetBlendSampleByQuery(Expression<Func<BlendSample, bool>> query)
        {
	        IBlendSampleService blendSampleService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IBlendSampleService>();
	        if (blendSampleService == null) throw new Exception("blendSampleService must be registered in service factory");

	        return blendSampleService.SelectBy(new BlendSample(), query);
        }

        public RotationTemplate GetRotationTemplateById(int rotationId)
        {
	        IRotationTemplateService rotationTemplateService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRotationTemplateService>();
	        if (rotationTemplateService == null) throw new Exception("blendSampleService must be registered in service factory");

	        return rotationTemplateService.SelectById(new RotationTemplate(){Id = rotationId});
        }

        public int UpdateWorkRotationSchedule(int employeeId, int rotationIndex, DateTime startDateTime, DateTime endDateTime)
        {
	        IWorkerScheduleService workerScheduleService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IWorkerScheduleService>();
	        if (workerScheduleService == null) throw new Exception("workerScheduleService must be registered in service factory");

	        return workerScheduleService.UpdateWorkRotationSchedule(employeeId, rotationIndex, startDateTime,
		        endDateTime);
        }

        //Feb 06, 2024 Tongtao 288_PR_LoadtobulkerWithThirdParty: get RigJobThirdPartyBulkerCrewSection  by productHaulId
        public RigJobThirdPartyBulkerCrewSection GetRigJobThirdPartyBulkerCrewSectionByProductHaulId(int productHaulId)
        {
            IRigJobThirdPartyBulkerCrewSectionService rigJobThirdPartyBulkerCrewSectionService = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigJobThirdPartyBulkerCrewSectionService>();
            if (rigJobThirdPartyBulkerCrewSectionService == null) throw new Exception("rigJobThirdPartyBulkerCrewSectionService must be registered in service factory");


            return rigJobThirdPartyBulkerCrewSectionService.SelectBy(new RigJobThirdPartyBulkerCrewSection { ProductHaul = new ProductHaul { Id = productHaulId } }, new List<string> { "ProductHaulId" }).FirstOrDefault();
        }

        public List<RigJobThirdPartyBulkerCrewSection> GetRigJobThirdPartyBulkerCrewSectionByQuery(Expression<Func<RigJobThirdPartyBulkerCrewSection, bool>> query)
        {
	        IRigJobThirdPartyBulkerCrewSectionService rigJobThirdPartyBulkerCrewSection = MetaShare.Common.Core.CommonService.ServiceFactory.Instance.GetService<IRigJobThirdPartyBulkerCrewSectionService>();
	        if (rigJobThirdPartyBulkerCrewSection == null) throw new Exception("blendSampleService must be registered in service factory");

	        return rigJobThirdPartyBulkerCrewSection.SelectBy(new RigJobThirdPartyBulkerCrewSection(), query);
        }
    }
}
