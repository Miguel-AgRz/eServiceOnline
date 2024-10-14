using System.Collections.ObjectModel;
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

namespace eServiceOnline.Gateway
{
    public class MicroReferenceData
    {
        public Collection<BinType> BinTypeCollection { get; set; }
        public Collection<Bin> BinCollection { get; set; }
        public Collection<BinInformation> BinInformationCollection { get; set; }
        public Collection<Country> CountryCollection { get; set; }
        public Collection<ProvinceOrState> ProvinceOrStateCollection { get; set; }
        public Collection<ClientCompany> ClientCompanyCollection { get; set; }
        public Collection<DrillingCompany> DrillCompanyCollection { get; set; }
        public Collection<JobType> JobTypeCollection { get; set; }
        public Collection<Employee> EmployeeCollection { get; set; }
        public Collection<BonusPosition> BonusPositionCollection { get; set; }
        public Collection<ServicePoint> ServicePointCollection { get; set; }
        public Collection<TruckUnit> TruckUnitCollection { get; set; }
        public Collection<Rig> RigCollection { get; set; }
        public Collection<Rig> BulkPlantCollection { get; set; }
        public Collection<ClientConsultant> ClientConsultantCollection { get; set; }
        public Collection<BlendChemical> BlendChemicalCollection { get; set; }
        public Collection<BlendRecipe> BlendRecipeCollection { get; set; }
        public Collection<BlendChemicalSection> BlendChemicalSectionCollection { get; set; }
        public Collection<BlendAdditiveMeasureUnit> BlendAdditiveMeasureUnitCollection { get; set; }
        public Collection<RigSize> RigSizeCollection { get; set; }
        public Collection<RigSizeType> RigSizeTypeCollection { get; set; }
        public Collection<ThreadType> ThreadTypeCollection { get; set; }
        public Collection<WitsBox> WitsBoxCollection { get; set; }
        public Collection<AdditionMethod> AdditionMethodCollection { get; set; }
        public Collection<AdditiveBlendMethod> AdditiveBlendMethodCollection { get; set; }

        public Collection<PurchasePrice> PurchasePriceCollection { get; set; }
        public Collection<Product> ProductCollection { get; set; }
        public Collection<BlendPrimaryCategory> BlendPrimaryCategoryCollection { get; set; }
        public Collection<BlendCategory> BlendCategoryCollection { get; set; }

        public Collection<RigJob> RigJobCollection { get; set; }
        public Collection<BlendFluidType> BlendFluidTypeCollection { get; set; }
        public Collection<AdditiveType> AdditiveTypeCollection { get; set; }
        public Collection<SanjelCrew> SanjelCrewCollection { get; set; }

    }
}