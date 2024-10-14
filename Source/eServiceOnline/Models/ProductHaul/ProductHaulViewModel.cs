using System.Collections.Generic;

namespace eServiceOnline.Models.ProductHaul
{
    public class ProductHaulViewModel
    {
        public ProductHaulViewModel()
        {
            this.DistrictList = new List<DistrictModel>();
            this.ProductHaulLoadModelList = new List<ProductHaulLoadModel>();
        }
        public IEnumerable<DistrictModel> DistrictList { get; set; }
        public IEnumerable<ProductHaulLoadModel> ProductHaulLoadModelList { get; set; }
        public DistrictModel DistrictModel { get; set; }

    }
}
