using System;
using System.Collections.Generic;
using eServiceOnline.Models.RigBoard;

namespace eServiceOnline.Models.ProductHaul
{
    public class PrintLoadSheetViewModel
    {
        public PrintLoadSheetViewModel()
        {
            this.ProductHaulModel = new ProductHaulModel();
        }

        public string BlendBreakdownSheetSerialNumber => GetBlendBreakdownSheetSerialNumber();

        public string BlendLoadSheetSerialNumber => GetBlendLoadSheetSerialNumber();
        public ProductHaulModel ProductHaulModel { get; set; }



        public static Dictionary<int, int> SbsDistrict { get; } = new Dictionary<int, int>()
        {
            { 61, 607},
            { 62, 675},
            { 65, 606},
            { 66, 615},
            { 67, 602},
            { 69, 600},
            { 70, 617},
            { 71, 604},
            { 72, 616},
            { 78, 651},
            { 81, 603},
            { 85, 612},
            { 87, 653},
            { 88, 618},
            { 89, 619}
        };

        private string GetBlendBreakdownSheetSerialNumber()
        {
            string sn = string.Empty;
            string districtId = ProductHaulModel.ServicePointId == 0?"000": SbsDistrict[ProductHaulModel.ServicePointId].ToString("D3");
            sn = String.Format("BBS-{0,3:D3}-{1,7:D7}-{2,5:D5}", districtId, ProductHaulModel.CallSheetNumber, ProductHaulModel.ProductHaulLoadId);

            return sn;
        }
        private string GetBlendLoadSheetSerialNumber()
        {
            string sn = string.Empty;
            string districtId = ProductHaulModel.ServicePointId == 0?"000": SbsDistrict[ProductHaulModel.ServicePointId].ToString("D3");
            sn = String.Format("BLS-{0,3:D3}-{1,7:D7}-{2,5:D5}", districtId, ProductHaulModel.CallSheetNumber, ProductHaulModel.ProductHaulLoadId);

            return sn;
        }
    }
}
