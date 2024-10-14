using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Reflection;

namespace eServiceOnline.PrintingService.Reports.MTSData.Model
{
    public  class MTSModel
    {
        public string MtsSerialNumber { get; set; }

        public MTSProductHaulInfoModel ProductHaulInfoItem { get; set; }

        public Collection<MTSPodLoadModel> PodLoadItems { get; set; }

        public Collection<MTSProductHaulLoadModel> ProductHaulLoadItems { get; set; }

        public Collection<MTSBlendUnloadSheetModel> BlendUnloadSheetItems { get; set; }

        public DataTable ProductHaulDataSet { get; set; }
        public DataTable PodLoadDataSet { get; set; }
        public DataTable ProductHaulLoadDataSet { get; set; }
        public DataTable BlendUnloadSheetDataSet { get; set; }
        public DataTable PrintSignDataSet { get; set; }

        public string LatestVersionInfo { get; set; }


        public MTSModel()
        {
            ProductHaulInfoItem = new MTSProductHaulInfoModel();
            PodLoadItems = new Collection<MTSPodLoadModel>();
            ProductHaulLoadItems = new Collection<MTSProductHaulLoadModel>();
            BlendUnloadSheetItems = new Collection<MTSBlendUnloadSheetModel>();
        }

        public MTSModel(string mtsSerialNumber,string latestVersionInfo,
            MTSProductHaulInfoModel productHaulInfoItem,
            Collection<MTSPodLoadModel> podLoadItems,
            Collection<MTSProductHaulLoadModel> productHaulLoadItems,
            Collection<MTSBlendUnloadSheetModel> blendUnloadSheetItems)
        {
            MtsSerialNumber = mtsSerialNumber;
            ProductHaulInfoItem = productHaulInfoItem;
            PodLoadItems = podLoadItems;
            ProductHaulLoadItems = productHaulLoadItems;
            BlendUnloadSheetItems = blendUnloadSheetItems;
            LatestVersionInfo = latestVersionInfo;

            ProductHaulDataSet = this.CreateDataSetProductHaul();
            PodLoadDataSet = this.CreateDataSetPodLoad();
            ProductHaulLoadDataSet = this.CreateDataSetProductHaulLoad();
            BlendUnloadSheetDataSet = this.CreateDataSetBlendUnloadSheet();
            PrintSignDataSet = this.CreateDataSetPrint();
        }


        #region Methods

        #region Create ProductHaul DataSet
        private DataTable CreateDataSetProductHaul()
        {
            List<MTSProductHaulTableModel> list = ConvertProductHaulToPrintTable();
            return BuildDataTable(list, "rowProductHaul", null);
        }

        private List<MTSProductHaulTableModel> ConvertProductHaulToPrintTable()
        {
            var listTable = new List<MTSProductHaulTableModel>();

            var line1 = new MTSProductHaulTableModel();
            line1.LableOne = "Date";
            line1.ValueOne = ProductHaulInfoItem.CreateDateTime;
            line1.LableTwo = "Unit";
            line1.ValueTwo = ProductHaulInfoItem.UnitNum;
            line1.LableThree = "Est Load Time";
            line1.ValueThree = ProductHaulInfoItem.EstimatedLoadTime;
            listTable.Add(line1);


            var line2 = new MTSProductHaulTableModel();
            line2.LableOne = "Go with Crew";
            line2.ValueOne = ProductHaulInfoItem.GoWithDriverName;
            line2.LableTwo = "Driver";
            line2.ValueTwo = ProductHaulInfoItem.DriverName;
            line2.LableThree = "Exp On Loc Time";
            line2.ValueThree = ProductHaulInfoItem.EstimatedLocationTime;
            listTable.Add(line2);


            var line3 = new MTSProductHaulTableModel();
            line3.LableOne = "Destination";
            line3.ValueOne = ProductHaulInfoItem.RigName;
            line3.LableTwo = "From";
            line3.ValueTwo = ProductHaulInfoItem.Station;
            line3.LableThree = "Client";
            line3.ValueThree = ProductHaulInfoItem.Client;
            listTable.Add(line3);


            var line4 = new MTSProductHaulTableModel();
            line4.LableOne = "Dispatched By";
            line4.ValueOne = ProductHaulInfoItem.DispatchedBy;
            line4.LableTwo = "Location";
            line4.ValueTwo = ProductHaulInfoItem.Location;
            line4.LableThree = "Client Rep";
            line4.ValueThree = ProductHaulInfoItem.ClientRep;
            listTable.Add(line4);

            return listTable;
        }

        #endregion


        #region Create PodLoad DataSet

        private DataTable CreateDataSetPodLoad()
        {
            List<MTSPodLoadModel> list = PodLoadItems.ToList();

            var podLoadHeaders = new Dictionary<string, string>
            {
                ["PodIndex"] = "Pod # (Front to Rear)",
                ["BaseTonnage"] = "Base Tonnage",
                ["ProductDes"] = "Product",
                ["TotalTonnage"] = "Total Tonnage",
                ["TempDesc"] = "Temp",
            };

            return BuildDataTable(list, "rowPodItem", podLoadHeaders);
        }

        #endregion


        #region Create ProductHaulLoad DataSet
        private DataTable CreateDataSetProductHaulLoad()
        {
            List<MTSProductHaulLoadModel> list = ProductHaulLoadItems.ToList();

            var productHaulLoadHeaders = new Dictionary<string, string>
            {
                ["BlendRequestNum"] = "BlendRequest #",
                ["BaseTonnage"] = "Base Tonnage",
                ["ProductDes"] = "Product",
                ["TotalTonnage"] = "Total Tonnage",
                ["Sample"] = "Sample",
                ["RequestedBy"] = "Requested By",
                ["BlendedBy"] = "Blended By"
            };

            return BuildDataTable(list, "rowProductHaulLoadItem", productHaulLoadHeaders);
        }

        #endregion


        #region Create BlendUnloadSheet DataSet

        private DataTable CreateDataSetBlendUnloadSheet()
        {
            List<MTSBlendUnloadSheetModel> list = BlendUnloadSheetItems.ToList();

            var blendUnloadSheetHeaders = new Dictionary<string, string>
            {
                ["BinNum"] = "Bin #",
                ["BaseTonnage"] = "Base Tonnage",
                ["ProductDes"] = "Product",
                ["TotalTonnage"] = "Total Tonnage"
            };

            return BuildDataTable(list, "rowBlendItem", blendUnloadSheetHeaders);
        }

        #endregion


        #region Create PrintSign DataSet
        private DataTable CreateDataSetPrint()
        {
            List<MTSPrintInfoModel> list = ConvertPrintInfoToPrintTable();

            return BuildDataTable(list, "rowPrintInfo", null);
        }

        private List<MTSPrintInfoModel> ConvertPrintInfoToPrintTable()
        {
            var listTable = new List<MTSPrintInfoModel>();

            var line1 = new MTSPrintInfoModel();
            line1.PrintOne = "Printed By";
            line1.SignOne = " ";
            line1.PrintTwo = "Loaded By";
            line1.SignTwo = " ";
            listTable.Add(line1);

            return listTable;
        }

        #endregion

        private DataTable BuildDataTable<T>(List<T> items, string rowName, Dictionary<string, string> headerValues)
        {
            DataTable dataTable = new DataTable();

            if (items == null || items.Count == 0)
                return dataTable;

            dataTable.Columns.Add("RowName", typeof(string));

            PropertyInfo[] properties = typeof(T).GetProperties();

            foreach (var prop in properties)
            {
                Type propertyType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                dataTable.Columns.Add(prop.Name, propertyType);
            }

            if (headerValues != null)
            {
                DataRow firstRow = dataTable.NewRow();
                firstRow["RowName"] = rowName + "Header";
                foreach (var header in headerValues)
                {
                    firstRow[header.Key] = header.Value;
                }
                dataTable.Rows.InsertAt(firstRow, 0);
            }

            foreach (var item in items)
            {
                DataRow row = dataTable.NewRow();
                row["RowName"] = rowName;
                foreach (var prop in properties)
                {
                    object value = prop.GetValue(item, null);
                    row[prop.Name] = value ?? DBNull.Value;
                }
                dataTable.Rows.Add(row);
            }
            return dataTable;
        }


        #endregion
    }
}