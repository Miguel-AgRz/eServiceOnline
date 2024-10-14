namespace eServiceOnline.Models.ProductHaul
{
    public class ProductShippingStatus
    {
        public int Id { get; set; }
        public int ProductSectionId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public double Amount { get; set; }
        public double SentAmount { get; set; }
        public double RemainsAmount { get; set; }

    }
}
