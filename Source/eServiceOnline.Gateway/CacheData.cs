using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Crew;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch;
using Sesi.SanjelData.Entities.BusinessEntities.WellSite;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment;
using Sesi.SanjelData.Entities.Common.BusinessEntities.HumanResources;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Operation;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Organization;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Products;
using Sesi.SanjelData.Entities.Common.BusinessEntities.WellSite;


namespace eServiceOnline.Gateway
{
    public class CacheData
    {
	    public static Collection<Product> Products
	    {
		    get
		    {
			    MicroReferenceData data = EServiceReferenceData.Data;
			    if (data != null)
			    {
				    return data.ProductCollection;
			    }
			    return null;
		    }
	    }
        public static Collection<BlendChemical> BlendChemicals
        {
            get
            {
                MicroReferenceData data = EServiceReferenceData.Data ;
                if (data != null)
                {
                    return data.BlendChemicalCollection;
                }
                return null;
            }
        }
        public static Collection<BlendRecipe> BlendRecipes
        {
            get
            {
                MicroReferenceData data = EServiceReferenceData.Data ;
                if (data != null)
                {
                    return data.BlendRecipeCollection;
                }
                return null;
            }
        }

        public static Collection<AdditionMethod> AdditionMethods
        {
            get
            {
                MicroReferenceData data = EServiceReferenceData.Data ;
                if (data != null)
                {
                    return data.AdditionMethodCollection;
                }
                return null;
            }
        }

        public static Collection<AdditiveBlendMethod> AdditiveBlendMethods
        {
            get
            {
                MicroReferenceData data = EServiceReferenceData.Data ;
                if (data != null)
                {
                    return data.AdditiveBlendMethodCollection;
                }
                return null;
            }
        }
        public static Collection<BlendAdditiveMeasureUnit> BlendAdditiveMeasureUnits
        {
            get
            {
                MicroReferenceData data = EServiceReferenceData.Data;
                if (data != null)
                {
                    return data.BlendAdditiveMeasureUnitCollection;
                }

                return null;
            }
        }

        public static Collection<ClientCompany> AllClientCompanies
        {
            get { return EServiceReferenceData.Data.ClientCompanyCollection; }
        }
        public static Collection<DrillingCompany> AllDrillCompanies
        {
            get { return EServiceReferenceData.Data.DrillCompanyCollection; }
        }
        public static Collection<Rig> Rigs
        {
            get { return EServiceReferenceData.Data.RigCollection; }
        }

        public static Collection<ClientConsultant> ClientConsultants
        {
            get { return EServiceReferenceData.Data.ClientConsultantCollection; }
        }

        public static Collection<ServicePoint> ServicePointCollections
        {
            get { return EServiceReferenceData.Data.ServicePointCollection; }
        }

        public static Collection<Employee> Employees
        {
            get { return EServiceReferenceData.Data.EmployeeCollection; }
        }

        public static Collection<TruckUnit> TruckUnits
        {
            get { return EServiceReferenceData.Data.TruckUnitCollection; }
        }

        public static Collection<RigSize> StandardSizeTypes
        {
            get { return EServiceReferenceData.Data.RigSizeCollection; }
        }

        public static Collection<RigSizeType> RigSizeTypes
        {
            get
            {
                return EServiceReferenceData.Data.RigSizeTypeCollection;
            }
        }

        public static Collection<ThreadType> ThreadTypes
        {
            get
            {
                return EServiceReferenceData.Data.ThreadTypeCollection;
            }
        }

        public static Collection<BinType> BinTypes
        {
            get { return EServiceReferenceData.Data.BinTypeCollection; }
        }

        public static Collection<BlendPrimaryCategory> PrimaryCategories
        {
            get { return EServiceReferenceData.Data.BlendPrimaryCategoryCollection; }
        }
        public static Collection<Bin> Bins
        {
            get { return EServiceReferenceData.Data.BinCollection; }
        }
        public static Collection<JobType> JobTypes
        {
            get { return EServiceReferenceData.Data.JobTypeCollection; }
        }
        public static Collection<BinInformation> BinInformations
        {
            get { return EServiceReferenceData.Data.BinInformationCollection; }
        }
        public static Collection<SanjelCrew> SanjelCrews
        {
            get { return EServiceReferenceData.Data.SanjelCrewCollection; }
        }

    }

    /*public static class BlendChemicalLoader
    {
        public static Collection<BlendChemical> BlendChemicalsLoad(Collection<BlendChemical> originalBlendChemicals, Collection<BlendRecipe> blendRecipes, Collection<BlendChemicalSection> blendChemicalSections)
        {
            if (originalBlendChemicals == null) throw new ArgumentNullException("originalBlendChemicals");
            if (blendRecipes == null) throw new ArgumentNullException("blendRecipes");
            if (blendChemicalSections == null) throw new ArgumentNullException("blendChemicalSections");

            Collection<BlendChemical> loadedBlendChemicals = new Collection<BlendChemical>();
            foreach (BlendChemical originalBlendChemical in originalBlendChemicals)
            {
                BlendChemical loadedBlendChemical = originalBlendChemical;
                BlendChemicalLoad(ref loadedBlendChemical, originalBlendChemicals, blendRecipes, blendChemicalSections);
                loadedBlendChemicals.Add(loadedBlendChemical);
            }
            return loadedBlendChemicals;
        }

        private static void BlendChemicalLoad(ref BlendChemical blendChemical, Collection<BlendChemical> originalBlendChemicals, Collection<BlendRecipe> blendRecipes, Collection<BlendChemicalSection> blendChemicalSections)
        {
            if (blendChemical == null) throw new ArgumentNullException("blendChemical");
            if (originalBlendChemicals == null) throw new ArgumentNullException("originalBlendChemicals");
            if (blendRecipes == null) throw new ArgumentNullException("blendRecipes");
            if (blendChemicalSections == null) throw new ArgumentNullException("blendChemicalSections");


            BlendChemical newBlendChemical = null;

            foreach (BlendChemical origBlendChemical in originalBlendChemicals)
            {
                if (origBlendChemical.Id == blendChemical.Id)
                {
                    newBlendChemical = origBlendChemical;
                    break;
                }
            }


            if (newBlendChemical != null && newBlendChemical.BlendRecipe != null && newBlendChemical.BlendRecipe.Id > 0 && newBlendChemical.BlendRecipe.BlendChemicalSections != null && newBlendChemical.BlendRecipe.BlendChemicalSections.Count == 0)
            {
                int blendChemicalRecipeId = newBlendChemical.BlendRecipe.Id;

                //                BlendRecipe  newBlendRecipe = ObjectBase.FindItem(blendRecipes, p => p.Id == blendChemicalRecipeId);
                BlendRecipe newBlendRecipe = blendRecipes.ToList().Find(p => p.Id == blendChemicalRecipeId);

                if (newBlendRecipe != null && newBlendRecipe.Id > 0)
                {
                                        Collection<BlendChemicalSection> findBlendChemicalSections = new Collection<BlendChemicalSection>();
                                        foreach (var blendChemicalSection in blendChemicalSections)
                                        {
                                            if(blendChemicalSection.OwnerId == newBlendRecipe.Id)
                                                findBlendChemicalSections.Add(blendChemicalSection);
                                        }

//                    Collection<BlendChemicalSection> findBlendChemicalSections = ObjectBase.FindItems(blendChemicalSections, p => p.OwnerId == newBlendRecipe.Id);
                    //                        blendChemicalSections.ToList().Find(p => p.RootId == newBlendRecipe.Id);

                    if (findBlendChemicalSections.Count > 0)
                    {
                        foreach (BlendChemicalSection findChemicalSection in findBlendChemicalSections)
                        {
                            if (findChemicalSection.BlendChemical != null && findChemicalSection.BlendChemical.Id > 0)
                            {
                                BlendChemical findChemical = findChemicalSection.BlendChemical;
                                BlendChemicalLoad(ref findChemical, originalBlendChemicals, blendRecipes, blendChemicalSections);
                                findChemicalSection.BlendChemical = findChemical;
                                newBlendRecipe.BlendChemicalSections.Add(findChemicalSection);
                            }
                        }
                    }
                }

                newBlendChemical.BlendRecipe = newBlendRecipe;
            }

            blendChemical = newBlendChemical;
        }
    }*/
}