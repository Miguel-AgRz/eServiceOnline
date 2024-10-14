using eServiceOnline.Models.ModelBinder;
using Microsoft.AspNetCore.Mvc;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Products;
using System;
using System.Linq;
using System.Net;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Organization;
using ServicePoint = Sesi.SanjelData.Entities.Common.BusinessEntities.Organization.ServicePoint;

namespace eServiceOnline.Models.ProductHaul
{
    [ModelBinder(BinderType = typeof(CommonModelBinder<ProductLoadInfoModel>))]
    public class ProductLoadInfoModel
    {
        public int ProductHaulLoadId { get; set; }
        public int CallSheetNumber { get; set; }
        public int CallSheetId { get; set; }
        public string BaseBlend { get; set; }
        public int BaseBlendSectionId { get; set; }
        public string Category { get; set; }
        public int CategoryId { get; set; }
        public int CustomerId { get; set; }
        public string ClientName { get; set; }
        public int ClientId { get; set; }
        public double MixWater { get; set; }
        public bool IsTotalBlendTonnage { get; set; }
        public bool IsBlendTest { get; set; }
        public int ProgramId { get; set; }
        public string ProgramNumber { get; set; }

        public string LoggedUser { set; get; }
        public double Amount { get; set; }
        public DateTime OnLocationTime { get; set; }
        public DateTime ExpectedOnLocationTime { get; set; }
        public int BinId { get; set; }
        public string BinNumber { get; set; }
        public DateTime EstimatedLoadTime { get; set; }
        public int JobTypeId { get; set; }
        public string JobTypeName { get; set; }
        public int BulkPlantId { get; set; }
        public string BulkPlantName { get; set; }
        public double TotalBlendWeight { get; set; }
        public double BaseBlendWeight { get; set; }
        public string BlendChemicalDescription { get; set; }
        public string BaseBlendUnit { get; set; }
        public string Comments { get; set; }
        public int RigId { get; set; }
        public string RigName { get; set; }
        public double RemainsAmount { set; get; }

        public int PodIndex { set; get; }
        public string BinInformationName { set; get; }
        public int BinInformationId { set; get; }

        public string ProductHaulDescription { set; get; }
        public int ServicePointId { set; get; }
        public string ServicePointName { set; get; }
        public string ClientRepresentative { set; get; }


        public void PopulateFromHaulLoad(ProductHaulLoad productHaulLoad)
        {
            if (productHaulLoad != null && productHaulLoad.Id > 0)
            {
                this.ProductHaulLoadId = productHaulLoad.Id;
                this.CallSheetNumber = productHaulLoad.CallSheetNumber;
                this.BaseBlendSectionId = productHaulLoad.BlendSectionId;
                BlendChemical blendChemical = productHaulLoad.BlendChemical;
                this.BaseBlend = blendChemical == null ? "" : (((string.IsNullOrWhiteSpace(blendChemical.Description) ? blendChemical.Name : blendChemical.Description)).Split('+').FirstOrDefault());
                this.Category = productHaulLoad.BlendCategory == null ? "" : (string.IsNullOrEmpty(productHaulLoad.BlendCategory.Description) ? productHaulLoad.BlendCategory.Name : productHaulLoad.BlendCategory.Description);
                this.CategoryId = productHaulLoad.BlendCategory == null ? 0 : productHaulLoad.BlendCategory.Id;
                this.MixWater = productHaulLoad.MixWater;

                //Change the formatting precision from two decimal places to three decimal places
                this.Amount = Math.Round(productHaulLoad.TotalBlendWeight / 1000, 3);
                this.TotalBlendWeight = Math.Round(productHaulLoad.TotalBlendWeight / 1000, 3);

                this.BaseBlendWeight = productHaulLoad.BaseBlendWeight/1000;
                this.BlendChemicalDescription = productHaulLoad.BlendChemical?.Description;
                this.IsTotalBlendTonnage = productHaulLoad.IsTotalBlendTonnage;
                this.OnLocationTime = productHaulLoad.OnLocationTime;
                this.ExpectedOnLocationTime = productHaulLoad.ExpectedOnLocationTime;
                this.EstimatedLoadTime = productHaulLoad.EstmatedLoadTime;


                //Nov 7, 2023 tongtao P45_Q4_167: When ProgramId is not empty, return the combination of ProgramId and Revision.
                if (!string.IsNullOrEmpty(productHaulLoad.ProgramId))
                {
                    this.ProgramNumber = productHaulLoad.ProgramId + "." + productHaulLoad.ProgramVersion.ToString("D2");
                }

                this.BaseBlendUnit = (productHaulLoad.Unit == null || string.IsNullOrEmpty(productHaulLoad.Unit.Description)) ? "t" : productHaulLoad.Unit.Description;
                this.IsBlendTest = productHaulLoad.IsBlendTest;
                this.RigName = productHaulLoad.Rig?.Name;
                this.RigId = productHaulLoad.Rig == null ? 0 : productHaulLoad.Rig.Id;
                this.ClientName = productHaulLoad.Customer?.Name;
                //Change the formatting precision from two decimal places to three decimal places
                this.RemainsAmount = Math.Round(productHaulLoad.RemainsAmount / 1000, 3);
                this.PodIndex = productHaulLoad.PodIndex;
                this.ClientRepresentative = productHaulLoad.ClientRepresentative;
                if (productHaulLoad.Bin != null)
                {
                    this.BinId = productHaulLoad.Bin.Id;
                    this.BinNumber = productHaulLoad.Bin.Name;
                }

                if (productHaulLoad.BulkPlant != null && productHaulLoad.BulkPlant.Id > 0)
                {
                    this.BulkPlantId = productHaulLoad.BulkPlant.Id;
                    this.BulkPlantName = productHaulLoad.BulkPlant.Name;
                }
                if (productHaulLoad.JobType != null && productHaulLoad.JobType.Id > 0)
                {
                    this.JobTypeId = productHaulLoad.JobType.Id;
                    this.JobTypeName = productHaulLoad.JobType.Name;
                }

                if (productHaulLoad.ServicePoint != null && productHaulLoad.ServicePoint.Id > 0)
                {
                    this.ServicePointId = productHaulLoad.ServicePoint.Id;
                    this.ServicePointName = productHaulLoad.ServicePoint.Name;
                }
            }
        }

        public void PopulateToHaulLoad(ProductHaulLoad productHaulLoad)
        {
            if (productHaulLoad != null)
            {
                productHaulLoad.Id=this.ProductHaulLoadId;
                productHaulLoad.CallSheetNumber = this.CallSheetNumber;
                productHaulLoad.CallSheetId = this.CallSheetId;
                productHaulLoad.BlendSectionId=this.BaseBlendSectionId;
                productHaulLoad.BlendCategory = new BlendCategory() { Id=this.CategoryId};
                productHaulLoad.MixWater = this.MixWater;
                productHaulLoad.IsTotalBlendTonnage = this.IsTotalBlendTonnage;
                if(productHaulLoad.IsTotalBlendTonnage)
                {
                    productHaulLoad.TotalBlendWeight = this.Amount*1000;
                }
                else
                {
                    productHaulLoad.BaseBlendWeight = this.Amount * 1000;
                }
                productHaulLoad.OnLocationTime = this.OnLocationTime;
                productHaulLoad.EstmatedLoadTime = this.EstimatedLoadTime;
                productHaulLoad.ExpectedOnLocationTime = this.ExpectedOnLocationTime;
                //productHaulLoad.ProgramId = this.ProgramNumber;
                // Nov 7, 2023 tongtao P45_Q4_167: When the ProgramNumber format is ProgramId.Revision, split it and save the ProgramId and Revision into two separate fields.
                if (!string.IsNullOrEmpty(this.ProgramNumber))
                {
                    if (this.ProgramNumber.Contains('.'))
                    {
                        productHaulLoad.ProgramId = this.ProgramNumber.Split('.')[0].Trim();

                        try
                        {
                            productHaulLoad.ProgramVersion = Convert.ToInt32(this.ProgramNumber.Split('.')[1].Trim());
                        }
                        catch
                        {
                            productHaulLoad.ProgramVersion = 0;
                        }
                    }
                }
                else
                {
                    productHaulLoad.ProgramId = this.ProgramNumber;
                }

                productHaulLoad.IsBlendTest = this.IsBlendTest;
                productHaulLoad.Bin = new Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment.Bin() { Id = this.BinId };
                productHaulLoad.JobType = new Sesi.SanjelData.Entities.Common.BusinessEntities.Operation.JobType() { Id = this.JobTypeId };
                productHaulLoad.ServicePoint = new ServicePoint() { Id = this.ServicePointId, Name = this.ServicePointName };
                productHaulLoad.ClientRepresentative = this.ClientRepresentative;

            }
        }
    }
}
