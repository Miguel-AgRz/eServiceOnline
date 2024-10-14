using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MetaShare.Common.Core.Proxies;
using MetaShare.Common.Foundation.Factory;
using MetaShare.Common.Foundation.Services;
using MetaShare.Common.Foundation.Versioning;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Crew;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch;
using Sesi.SanjelData.Entities.BusinessEntities.WellSite;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment;
using Sesi.SanjelData.Entities.Common.BusinessEntities.HumanResources;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Inventory;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Operation;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Organization;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Products;
using Sesi.SanjelData.Entities.Common.BusinessEntities.WellSite;
using Sesi.SanjelData.Entities.Common.Entities.General;
using Sesi.SanjelData.Services.Interfaces.BusinessEntities.Operation.Crew;
using Sesi.SanjelData.Services.Interfaces.BusinessEntities.Operation.Dispatch;
using Sesi.SanjelData.Services.Interfaces.BusinessEntities.WellSite;
using Sesi.SanjelData.Services.Interfaces.Common.BusinessEntities.Equipment;
using Sesi.SanjelData.Services.Interfaces.Common.BusinessEntities.HumanResources;
using Sesi.SanjelData.Services.Interfaces.Common.BusinessEntities.Inventory;
using Sesi.SanjelData.Services.Interfaces.Common.BusinessEntities.Operation;
using Sesi.SanjelData.Services.Interfaces.Common.BusinessEntities.Organization;
using Sesi.SanjelData.Services.Interfaces.Common.BusinessEntities.Products;
using Sesi.SanjelData.Services.Interfaces.Common.BusinessEntities.WellSite;
using Sesi.SanjelData.Services.Interfaces.Common.Entities.General;

namespace eServiceOnline.Gateway
{
    public class ReferenceDataMicroService
    {
        public MicroReferenceData GetUpdatedReferenceData(VersionInfo originalVersionInfo)
        {
            IProxyFactory proxyFactory = ProxyFactory.Instance;
            var serviceFactory = MetaShare.Common.Core.CommonService.ServiceFactory.Instance;
            return GetUpdatedReferenceData(originalVersionInfo,proxyFactory, serviceFactory);
        }

        public static MicroReferenceData GetUpdatedReferenceData(VersionInfo originalVersionInfo, IFactory apiProxyFactory, MetaShare.Common.Core.CommonService.ServiceFactory serviceFactory)
        {
            MicroReferenceData referenceData = new MicroReferenceData();

            referenceData.BinTypeCollection = new Collection<BinType>(GetService<IBinTypeService>(serviceFactory).SelectAll());
            referenceData.BinCollection = new Collection<Bin>(GetService<IBinService>(serviceFactory).SelectAll());
            referenceData.BinInformationCollection = new Collection<BinInformation>(GetService<IBinInformationService>(serviceFactory).SelectAll());
            referenceData.CountryCollection = new Collection<Country>(GetService<ICountryService>(serviceFactory).SelectAll());
            referenceData.ProvinceOrStateCollection = new Collection<ProvinceOrState>(GetService<IProvinceOrStateService>(serviceFactory).SelectAll());
            referenceData.ClientCompanyCollection = new Collection<ClientCompany>(GetService<IClientCompanyService>(serviceFactory).SelectAll());
            referenceData.DrillCompanyCollection = new Collection<DrillingCompany>(GetService<IDrillingCompanyService>(serviceFactory).SelectAll());
            referenceData.JobTypeCollection = new Collection<JobType>(GetService<IJobTypeService>(serviceFactory).SelectAll());

            referenceData.EmployeeCollection = new Collection<Employee>(GetService<IEmployeeService>(serviceFactory).SelectAll());
            referenceData.ServicePointCollection =new Collection<ServicePoint>(GetService<IServicePointService>(serviceFactory).SelectAll());
            referenceData.TruckUnitCollection = new Collection<TruckUnit>(GetService<ITruckUnitService>(serviceFactory).SelectAll());

            referenceData.RigCollection = new Collection<Rig>(GetService<IRigService>(serviceFactory).SelectAll());
            referenceData.BulkPlantCollection = new Collection<Rig>(referenceData.RigCollection.Where(p=>p.OperationSiteType==OperationSiteType.BulkPlant).ToList());
            referenceData.ClientConsultantCollection = new Collection<ClientConsultant>(GetService<IClientConsultantService>(serviceFactory).SelectAll());
            referenceData.RigSizeCollection = new Collection<RigSize>(GetService<IRigSizeService>(serviceFactory).SelectAll());
            referenceData.RigSizeTypeCollection = new Collection<RigSizeType>(GetService<IRigSizeTypeService>(serviceFactory).SelectAll());
            referenceData.ThreadTypeCollection = new Collection<ThreadType>(GetService<IThreadTypeService>(serviceFactory).SelectAll());
      
            referenceData.AdditionMethodCollection = new Collection<AdditionMethod>(GetService<IAdditionMethodService>(serviceFactory).SelectAll());
            referenceData.AdditiveBlendMethodCollection = new Collection<AdditiveBlendMethod>(GetService<IAdditiveBlendMethodService>(serviceFactory).SelectAll());
            referenceData.BlendRecipeCollection = new Collection<BlendRecipe>(GetService<IBlendRecipeService>(serviceFactory).SelectByJoin(p=>p.BlendChemicalSections!=null));
            List<BlendChemical> blendList = GetService<IBlendChemicalService>(serviceFactory).SelectAll();
            blendList.ForEach(p=>p.BlendRecipe = referenceData.BlendRecipeCollection.FirstOrDefault(recipe=>recipe.Id == p.BlendRecipe.Id));
            referenceData.BlendChemicalCollection = new Collection<BlendChemical>(blendList);

//            referenceData.BlendChemicalSectionCollection = new Collection<BlendChemicalSection>(GetService<IBlendChemicalSectionService>(serviceFactory).SelectAll());
            referenceData.BlendAdditiveMeasureUnitCollection = new Collection<BlendAdditiveMeasureUnit>(GetService<IBlendAdditiveMeasureUnitService>(serviceFactory).SelectAll());
            referenceData.BonusPositionCollection = new Collection<BonusPosition>(GetService<IBonusPositionService>(serviceFactory).SelectAll());

            referenceData.WitsBoxCollection = new Collection<WitsBox>(GetService<IWitsBoxService>(serviceFactory).SelectAll());
            referenceData.PurchasePriceCollection = new Collection<PurchasePrice>(GetService<IPurchasePriceService>(serviceFactory).SelectAll());
            referenceData.ProductCollection = new Collection<Product>(GetService<IProductService>(serviceFactory).SelectAll());
            referenceData.BlendPrimaryCategoryCollection = new Collection<BlendPrimaryCategory>(GetService<IBlendPrimaryCategoryService>(serviceFactory).SelectAll());
            referenceData.BlendCategoryCollection = new Collection<BlendCategory>(GetService<IBlendCategoryService>(serviceFactory).SelectAll());
//            referenceData.RigJobCollection = new Collection<RigJob>(GetService<IRigJobService>(serviceFactory).SelectBy(new RigJob() { },rigJob=>rigJob.CallSheetNumber>0));

            referenceData.BlendFluidTypeCollection = new Collection<BlendFluidType>(GetService<IBlendFluidTypeService>(serviceFactory).SelectAll());
            referenceData.AdditiveTypeCollection = new Collection<AdditiveType>(GetService<IAdditiveTypeService>(serviceFactory).SelectAll());
            referenceData.SanjelCrewCollection = new Collection<SanjelCrew>(GetService<ISanjelCrewService>(serviceFactory).SelectAll());

            return referenceData;
        }

        private static T GetService<T>(IFactory factory) where T : IService
        {
            return (T)factory.GetProduct(typeof(T));
        }
        private static T GetService<T>(MetaShare.Common.Core.CommonService.ServiceFactory factory)  where  T: MetaShare.Common.Core.CommonService.IService
        {
            return (T)factory.GetService(typeof(T));
        }
    }
}