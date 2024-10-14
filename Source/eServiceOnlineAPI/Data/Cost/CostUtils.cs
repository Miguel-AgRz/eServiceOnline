using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;

using Sesi.SanjelData.Entities.Common.BusinessEntities.Products;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Inventory;
using eServiceOnline.Gateway;
using System.Linq;

using eServiceOnline.WebAPI.Data.SharePointSync;
using sesi.SanjelLibrary.BlendLibrary;
using Sesi.SanjelData.Entities.BusinessEntities.BaseEntites;

//using Sesi.SanjelData.Entities.BusinessEntities.BaseEntites;

namespace eServiceOnline.WebAPI.Data.Cost
{
    public class CostUtils
    {
        //static string inventory_ConnectionStr = SharePointUtils.GetConnectionString(@"sanjel27\DW", "SBS_VIEWS");

        static public Dictionary<int, string> DistrictSBS = new Dictionary<int, string>()
        {
            { 61, "D607" },
            { 62, "D675" },
            { 65, "D606" },
            { 66, "D615" },
            { 67, "D602" },
            { 69, "D600" },
            { 70, "D617" },
            { 71, "D604" },
            { 72, "D616" },
            { 78, "D651" },
            { 81, "D603" },
            { 85, "D612" },
            { 87, "D653" },
            { 88, "D618" },
            { 89, "D619" }
        };

        static public Dictionary<string, string> DistrictSBSByName = new Dictionary<string, string>()
        {
            { "Lloydminster", "D607" },
            { "Calgary - Head Office", "D675" },
            { "Lac La Biche", "D606" },
            { "Fort St John", "D615" },
            { "Fort St. John", "D615" },
            { "Edmonton", "D602" },
            { "Brooks", "D600" },
            { "Swift Current", "D617" },
            { "Grande Prairie", "D604" },
            { "Estevan", "D616" },
            { "Calgary - Maintenance", "D651" },
            { "Edson", "D603" },
            { "Red Deer", "D612" },
            { "Calgary - Lab", "D653" },
            { "Kindersley", "D618" },
        };

        public static string GetUnitName(string unit)
        {
            string result;

            switch (unit.ToUpper())
            {
                case "SCM":
                    result = "SCM";
                    break;
                case "T":
                case "TONNES":
                    result = "Tonnes";
                    break;
                case "KG":
                case "KGS":
                case "KILOGRAMS":
                    result = "Kilograms";
                    break;
                case "L":
                case "LITRES":
                    result = "Litres";
                    break;
                case "M3":
                case "CUBIC METERS":
                    result = "Cubic Meters";
                    break;
                case "%":
                case "PERCENT":
                    result = "Percent";
                    break;
                case "KG/M3":
                    result = "Kg/m3";
                    break;
                case "L/M3":
                    result = "l/m3";
                    break;
                case "% BWOW":
                    result = "% BWOW";
                    break;
                default:
                    result = unit;
                    break;
            };

            return result;
        }


        public class OutputCost : Chemical
        {
            public string PbCode { get; set; }
            public bool IsDetail { get; set; }
        }

        public class Blend : Chemical
        {
            public int Idx { get; set; }
            public string WaterMix { get; set; }
            public List<Additive> Additives { get; set; }
        }

        public class InputBlends
        {
            public string District { get; set; }
            public bool WithDetails { get; set; }
            public Collection<Blend> Blends { get; set; }
        }

        public class OutputAvailability : Chemical
        {
            public double InventoryQuantity { get; set; }
            public string InventoryUnit { get; set; }
        }

        public class Additive : Chemical
        {
        }

        public class Chemical
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string IIN { get; set; }
            public double? Quantity { get; set; }
            public string Unit { get; set; }
            public double Cost { get; set; }
        }

        public static void ProcessBlendSectionCost(BlendSection bs, string servicePointSbsId, double freightCost, ref Collection<CostUtils.OutputCost> outputCostCollection, DateTime costAsOfDate, DateTime chemicalsAsOfDate, bool includeDetails)
        {
            PrepareBlendSection(ref bs);
            BlendChemical blendChemical = ConvertToBlendChemical(bs, chemicalsAsOfDate);

            if (blendChemical != null)
            {
	            var product = (blendChemical.Product != null && blendChemical.Product.Id !=0) ? CacheData.Products.FirstOrDefault(p => p.Id == blendChemical.Product.Id):null;
                try
                {
                    var totalCost = 0.0;
                    var totalQantity = 0.0;
                    bool useBaseBlendQantity = false;

                    Collection<BlendChemicalSection> allBlendBreakDowns = GetAllBreakDowns(bs, blendChemical);

                    foreach (BlendChemicalSection bcs in allBlendBreakDowns)
                    {

                        var blendChemical1 = CacheData.BlendChemicals.FirstOrDefault(p => p.Id == bcs.BlendChemical.Id);
                        if (blendChemical1 == null)
                            throw new Exception("Blend Chemical Id: " + bcs.BlendChemical.Id +
                                                "doesn't exist in master data");
                        var product1 = CacheData.Products.FirstOrDefault(p => p.Id == blendChemical1.Product.Id);

                        var cost = CostCalculator.CalculateCost(EServiceReferenceData.Data.BlendChemicalCollection, GetPurchasePriceCollection(costAsOfDate), EServiceReferenceData.Data.BlendRecipeCollection, blendChemical1, freightCost, true, servicePointSbsId, EServiceReferenceData.Data.ProductCollection);
                        totalCost += cost * bcs.Amount;

                        var adjustedQuantity = bcs.Amount;

                        switch ((bs.BlendAmountUnit == null ? "" : bs.BlendAmountUnit.Name).ToLower())
                        {
                            case "t":
                            case "mt":
                            case "tonne":
                            case "tonnes":
                            case "tone":
                            case "tones":
                                switch (bcs.Unit.Name.ToLower())
                                {
                                    case "t":
                                    case "mt":
                                    case "tonne":
                                    case "tonnes":
                                    case "tone":
                                    case "tones":
                                        adjustedQuantity = adjustedQuantity;
                                        break;
                                    case "kg":
                                    case "kilogram":
                                    case "kilograms":
                                        adjustedQuantity = adjustedQuantity / 1000;
                                        break;
                                    case "m3":
                                    case "cubic meter":
                                    case "cubic meters":
                                        adjustedQuantity = adjustedQuantity * blendChemical1.Density / 1000;
                                        break;
                                    case "l":
                                    case "liter":
                                    case "liters":
                                        adjustedQuantity = adjustedQuantity / 1000 * blendChemical1.Density / 1000;
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case "m3":
                            case "cubic meter":
                            case "cubic meters":
                                //switch (bcs.Unit.Name.ToLower())
                                //{
                                //    case "t":
                                //    case "mt":
                                //    case "tonne":
                                //    case "tonnes":
                                //    case "tone":
                                //    case "tones":
                                //        adjustedQuantity = adjustedQuantity * 1000 / bcs.BlendChemical.Density;
                                //        break;
                                //    case "kg":
                                //    case "kilogram":
                                //    case "kilograms":
                                //        adjustedQuantity = adjustedQuantity / bcs.BlendChemical.Density;
                                //        break;
                                //    case "m3":
                                //    case "cubic meter":
                                //    case "cubic meters":
                                //        //adjustedQuantity = adjustedQuantity;
                                //        break;
                                //    case "l":
                                //    case "liter":
                                //    case "liters":
                                //        adjustedQuantity = adjustedQuantity / 1000;
                                //        break;
                                //    default:
                                //        break;
                                //}
                                useBaseBlendQantity = true;
                                break;
                            default:
                                break;
                        }

                        totalQantity += adjustedQuantity;

                        if (includeDetails)
                            outputCostCollection.Add(
                                new CostUtils.OutputCost()
                                {
                                    Id = bs.Id,
                                    Cost = cost * bcs.Amount,
                                    Name = blendChemical1.Name,
                                    IIN = product1 == null ? "" : product1.InventoryNumber ?? "",
                                    PbCode = product1 == null ? "" : product1?.PriceCode.ToString() ?? "",
                                    Quantity = bcs.Amount,
                                    Unit = bcs.Unit.Name,
                                    IsDetail = true
                                });
                    }

                    var blend = string.IsNullOrEmpty(blendChemical.Description) ? blendChemical.Name : blendChemical.Description;
                    var inn = product?.InventoryNumber ?? "";
                    var code = product == null ? "" : (product.PriceCode == 0 ? "" : product.PriceCode.ToString());

                    outputCostCollection.Add(
                        new CostUtils.OutputCost()
                        {
                            Id = bs.Id,
                            Cost = totalCost,
                            Name = blend,
                            IIN = inn,
                            PbCode = code,
                            Quantity = useBaseBlendQantity ? bs.Quantity ?? 0 : totalQantity,
                            Unit = bs.BlendAmountUnit == null ? "" : bs.BlendAmountUnit.Name,
                            IsDetail = false
                        });
                }
                catch (Exception ex)
                {
                    var code = ex.Message.ToString();
                    code = code.Length <= 49 ? code : code.Substring(0, 49);
                    outputCostCollection.Add(
                        new CostUtils.OutputCost()
                        {
                            Id = bs.Id,
                            Cost = 0,
                            Name = bs.BlendFluidType.Name,
                            IIN = "",
                            PbCode = code,
                            Quantity = bs.Quantity ?? 0,
                            Unit = bs.BlendAmountUnit == null ? "" : bs.BlendAmountUnit.Name,
                            IsDetail = false
                        });
                }
            }
            else
            {
                outputCostCollection.Add(
                    new CostUtils.OutputCost()
                    {
                        Id = bs.Id,
                        Cost = 0,
                        Name = bs.BlendFluidType.Name,
                        IIN = "",
                        PbCode = "NoChemical",
                        Quantity = bs.Quantity ?? 0,
                        Unit = bs.BlendAmountUnit == null ? "" : bs.BlendAmountUnit.Name,
                        IsDetail = false
                    });
            }
        }

        public static void ProcessBlendSectionAvailability(BlendSection bs, string servicePointSbsId, ref Collection<CostUtils.OutputAvailability> outputAvailabilityCollection, DateTime chemicalsAsOfDate)
        {
            CostUtils.PrepareBlendSection(ref bs);
            BlendChemical blendChemical = CostUtils.ConvertToBlendChemical(bs, chemicalsAsOfDate);

            if (blendChemical != null)
            {
                try
                {
                    Collection<BlendChemicalSection> allBlendBreakDowns = CostUtils.GetAllBreakDowns(bs, blendChemical);
                    //string warehouse = @"'%20'";
                    string warehouse = servicePointSbsId.Substring(1, 3) + "20";
                    string iinList = "";

                    foreach (BlendChemicalSection bcs in allBlendBreakDowns)
                    {
                        if (!String.IsNullOrEmpty(bcs.BlendChemical.Product.InventoryNumber))
                        {
                            if (iinList.Length == 0)
                                iinList += "(";

                            iinList += (iinList.Length == 1 ? "" : ",") + "'" + bcs.BlendChemical.Product.InventoryNumber + "'";
                        }
                    }

                    if (iinList.Length > 0) iinList += ")";

                    Collection<CostUtils.Chemical> avail = CostUtils.GetInventoryAvailability(warehouse, iinList);

                    foreach (BlendChemicalSection bcs in allBlendBreakDowns)
                    {
                        CostUtils.Chemical cm = avail.FirstOrDefault(c => c.IIN == (bcs.BlendChemical.Product.InventoryNumber ?? "-"));

                        string name = cm != null ? cm.Name : (bcs.Name != null ? bcs.Name : bcs.BlendChemical.Name);

                        outputAvailabilityCollection.Add(
                            new CostUtils.OutputAvailability()
                            {
                                Id = bs.Id,  //bcs.Id,
                                //Name = cm != null ? cm.Name : "NA", //bcs.Name,
                                Name = name,
                                IIN = bcs.BlendChemical.Product.InventoryNumber ?? "",
                                Quantity = bcs.Amount,
                                Unit = bcs.Unit.Name,
                                InventoryQuantity = cm != null ? cm.Quantity ?? 0 : -1,
                                InventoryUnit = cm != null ? cm.Unit : "NA"
                            });
                    }
                }
                catch (Exception ex)
                {
                    var code = ex.Message.ToString();
                    code = code.Length <= 49 ? code : code.Substring(0, 49);
                    outputAvailabilityCollection.Add(
                        new CostUtils.OutputAvailability()
                        {
                            Id = bs.Id,
                            Name = bs.BlendFluidType.Name,
                            IIN = "",
                            Quantity = bs.Quantity ?? 0,
                            Unit = bs.BlendAmountUnit.Name,
                            InventoryQuantity = -1,
                            InventoryUnit = "NA"
                        });
                }
            }
            else
            {
                outputAvailabilityCollection.Add(
                    new CostUtils.OutputAvailability()
                    {
                        Id = bs.Id,
                        Name = bs.BlendFluidType.Name,
                        IIN = "",
                        Quantity = bs.Quantity ?? 0,
                        Unit = bs.BlendAmountUnit.Name,
                        InventoryQuantity = -1,
                        InventoryUnit = "NA"
                    });
            }
        }

        public static Collection<PurchasePrice> GetPurchasePriceCollection(DateTime costAsOfDate)
        {
            Collection<PurchasePrice> purchasePriceCollection = eServiceOnlineGateway.Instance.GetPurchasePricesAsOfDate(costAsOfDate);

            return purchasePriceCollection;
        }

        private static void PrepareBlendSection(ref BlendSection bs)
        {
            //Replace "LITEmix PRO RD" (294) with "LITEmix PRO" (289) for cost calculation as per Jason's sudgestion
            //Commented on Dec 02, 2020 to check if an original issue was fixed (nobody remember now what it was)
            //if (bs.BlendFluidType != null && bs.BlendFluidType.Id == 294)
            //    bs.BlendFluidType.Id = 289;

            //Remove empty records in additives list
            if (bs.BlendAdditiveSections != null && bs.BlendAdditiveSections.Count > 0)
            {
                for (int i = bs.BlendAdditiveSections.Count - 1; i >= 0; i--)
                {
                    if (bs.BlendAdditiveSections[i] == null || bs.BlendAdditiveSections[i].AdditiveType == null || bs.BlendAdditiveSections[i].AdditiveType.Name == "" || bs.BlendAdditiveSections[i].AdditiveType.Id == 0)
                        bs.BlendAdditiveSections.RemoveAt(i);
                }
            }
        }

        private static BlendChemical ConvertToBlendChemical(BlendSection bs, DateTime chemicalsAsOfDate)
        {
                //blendChemical = BlendSection.CovertToBlendChemicalFromBlendSection(blendChemicalList, bs, additionMethodList, additiveBlendMethodList, blendAdditiveMeasureUnitList, CacheData.BlendRecipes);
            BlendChemical blendChemical = BlendCalculator.CovertToBlendChemicalFromBlendSection(CacheData.BlendChemicals, bs, CacheData.AdditionMethods, CacheData.AdditiveBlendMethods, CacheData.BlendAdditiveMeasureUnits, CacheData.BlendRecipes);
            
            return blendChemical;
        }

        private static Collection<BlendChemicalSection> GetAllBreakDowns(BlendSection bs, BlendChemical blendChemical)
        {
	        Collection<BlendChemicalSection> allBlendBreakDowns = new Collection<BlendChemicalSection>();
	        if (blendChemical.BlendRecipe != null)
	        {
		        //var blendQuantity = (bs.Quantity ?? 0.0) * (bs.BlendAmountUnit.Abbreviation.Equals("m3") || bs.BlendAmountUnit.Abbreviation.Equals("t") ? 1000 : 1);
		        var blendQuantity = (bs.Quantity ?? 0.0) * 1;

		        Collection<BlendAdditiveMeasureUnit> blendAdditiveMeasureUnitList =
			        EServiceReferenceData.Data.BlendAdditiveMeasureUnitCollection;
		        BlendAdditiveMeasureUnit unit =
			        blendAdditiveMeasureUnitList.FirstOrDefault(p => p.Name == bs.BlendAmountUnit.Name);

		        BlendChemicalSection baseBlendSection =
			        blendChemical.BlendRecipe.BlendChemicalSections.FirstOrDefault(p => p.IsBaseBlend);
		        List<BlendChemicalSection> additiveBlendSections =
			        blendChemical.BlendRecipe.BlendChemicalSections.Where(p => !p.IsBaseBlend).ToList();
		        BlendChemical baseBlend = baseBlendSection?.BlendChemical;

		        Collection<BlendChemicalSection> baseBlendBreakDowns;
		        Collection<BlendChemicalSection> additiveBlendBreakDowns;
		        Collection<BlendChemicalSection> additionalBlendBreakDowns;
		        double totalBlendWeight;
		        double baseBlendWeight;
                bool isAirborneHarzard = false;
		        //BlendBreakDownCalculator.GetAllBlendBreakDown(blendChemical.BlendRecipe, blendQuantity, false, bs.MixWaterRequirement ?? 1.0, out allBlendBreakDowns, out baseBlendBreakDowns, out additiveBlendBreakDowns, out additionalBlendBreakDowns, out totalBlendWeight, out baseBlendWeight);
                BlendCalculator.GetAllBlendBreakDown1(CacheData.BlendChemicals, CacheData.BlendRecipes,
			        blendChemical.BlendRecipe, blendQuantity, null, false, bs.MixWaterRequirement, unit,

                    out allBlendBreakDowns, out baseBlendBreakDowns, out additiveBlendBreakDowns,
			        out additionalBlendBreakDowns, out totalBlendWeight, out baseBlendWeight, out isAirborneHarzard);
	        }

	        return allBlendBreakDowns;
        }

        private static Collection<Chemical> GetInventoryAvailability(string warehouse, string iinList)
        {
            Collection<Chemical> avail = new Collection<Chemical>();

            SqlConnection sqlConn = null;
            SqlCommand cmd = null;
            SqlDataAdapter da = null;
            DataTable dt = null;

            string cmdStr = String.Format(
                    "SELECT [Warehouse],[Location],[Item Number],[Product Name],[Physical Inventory],[Unit] FROM SBS_VIEWS.dbo.InventorySummaryView where Warehouse like {0} and [Item Number] in {1}"
                    , warehouse, iinList);

            try
            {
                sqlConn = new SqlConnection(SharePointUtils.GetConnectionString(@"sanjel27\DW", "SBS_VIEWS"));
                cmd = new SqlCommand(cmdStr, sqlConn);
                cmd.CommandType = CommandType.Text;
                da = new SqlDataAdapter(cmd);
                dt = new DataTable();
                sqlConn.Open();

                da.Fill(dt);
            }
            catch (Exception ex)
            {
                if (sqlConn != null && sqlConn.State != ConnectionState.Closed)
                    sqlConn.Close();
                sqlConn.Dispose();
                dt.Dispose();
                da.Dispose();
                cmd.Dispose();

                throw ex;
            }

            if (sqlConn.State != ConnectionState.Closed)
                sqlConn.Close();

            sqlConn.Dispose();

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    avail.Add(
                        new Chemical()
                        {
                            Id = 0,
                            Name = dr["Product Name"].ToString(),
                            IIN = dr["Item Number"].ToString(),
                            Quantity = double.Parse(dr["Physical Inventory"].ToString()),
                            Unit = dr["Unit"].ToString()
                        });
                }
            }
            dt.Dispose();
            da.Dispose();
            cmd.Dispose();

            return avail;
        }

    }
}
