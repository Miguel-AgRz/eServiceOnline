using System.Collections.Generic;
using eServiceOnline.Models.RigBoard;

namespace eServiceOnline.Models.ProductHaul
{
    public class LoadViewModel
    {
        public LoadViewModel()
        {
            this.RigJob = new RigJobModel();
            this.ProductList = new List<BlendSectionModel>();
            this.LoadList =  new List<ProductHaulModel>();
        }

        public RigJobModel RigJob { get; set; }
        public IEnumerable<BlendSectionModel> ProductList { get; set; }
        public IEnumerable<ProductHaulModel> LoadList { get; set; }

        public IEnumerable<ProductLoadSectionModel> ProductLoadSections
        {
            get
            {
                List<ProductLoadSectionModel> productLoadSectionModels = new List<ProductLoadSectionModel>();
                if (this.LoadList != null)
                {
                    foreach (ProductHaulModel productHaulModel in this.LoadList)
                    {
                        if (productHaulModel.ProductLoadSections != null)
                        {
                            foreach (ProductLoadSectionModel productLoadSection in productHaulModel.ProductLoadSections)
                            {
                                productLoadSection.ProductHaulId = productHaulModel.ProductHaulId;
                                productLoadSectionModels.Add(productLoadSection);
                            }
                        }
                    }
                }
                return productLoadSectionModels;
            }
        }
    }
}
