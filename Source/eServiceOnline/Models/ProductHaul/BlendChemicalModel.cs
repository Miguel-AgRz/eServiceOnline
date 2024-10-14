namespace eServiceOnline.Models.ProductHaul
{
    public class BlendChemicalModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double BulkDensity { get; set; }
        public double Yield { get; set; }
        public double SpecificGravity { get; set; }
        public double MixWater { get; set; }
        public double Density { get; set; }
        public bool IsBaseEligible { get; set; }
        public bool IsAdditiveEligible { get; set; }
        public int PrimaryCategoryId { get; set; }
        public string PrimaryCategoryName { get; set; }
        public string StatusName { get; set; }
        public int StatusId { get; set; }
        public int PriceCode { get; set; }
        public string InventoryNumber { get; set; }

        public string AERCode { get; set; }
        public int AddsPriceCode { get; set; }
        public int UnitId { get; set; }
        public bool HasRecipe { get; set; }
        public string UnitName { get; set; }
    }
}
