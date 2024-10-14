using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using eServiceOnline.Data;
using eServiceOnline.Models.ProductHaul;
using Microsoft.VisualStudio.Shell.Interop;
using Sesi.SanjelData.Entities.BusinessEntities.BaseEntites;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule;


namespace eServiceOnline.Models.RigBoard
{
    public class BlendProductHaulModel
    {
        public int CallSheetId { get; set; }
        public int BlendSectionId { get; set; }
        public int StageNumber { get; set; }
        public string Category { get; set; }
        public string BaseBlendName { get; set; }
        public string Quantity { get; set; }
        public string Units { get; set; }
        public string Density { get; set; }
        public string Bulker { get; set; }
        public string Driver { get; set; }
        public bool IsNeedHaul { get; set; }
        public List<ProductHaulForBlendModel> ProductHaulForBlendModels { get; set; }
        public BlendProductHaulStatus BlendProductHaulStatus { get; set; }

        public List<BlendAdditiveSection> BlendAdditiveSections { get; set; }


        public void PopulateFrom(BlendSection blendSection, List<Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul> productHauls, List<ProductHaulLoad> productHaulLoads, List<ShippingLoadSheet> shippingLoadSheets)
        {
            if (blendSection != null)
            {
                this.CallSheetId = blendSection.OwnerId;
                this.BlendSectionId = blendSection.Id;
                this.StageNumber = blendSection.Interval;
                this.Category = blendSection.BlendCategory?.Description;
                this.BaseBlendName = blendSection.BlendFluidType.Name;
                var quantityString= blendSection.Quantity==null||Math.Abs(blendSection.Quantity.Value)<0.01?string.Empty:blendSection.Quantity.ToString();
                var builder = new StringBuilder();
                if (!string.IsNullOrEmpty(quantityString) && blendSection.IsNeedFieldTesting)
                {
                    builder.Append('\xD83E');
                    builder.Append('\xDDEA');
                }

                builder.AppendLine(quantityString);

                this.Quantity = builder.ToString();
                this.Units = blendSection.BlendAmountUnit == null ? string.Empty : blendSection.BlendAmountUnit.Description;
                this.Density =  Math.Abs(blendSection.Density)<0.01?string.Empty:blendSection.Density.ToString();
                this.IsNeedHaul = blendSection.IsNeedHaul;
                this.BlendAdditiveSections = blendSection.BlendAdditiveSections;

                List<ProductHaulLoad> productHaulLoadList = productHaulLoads==null?new List<ProductHaulLoad>() : productHaulLoads.Where(p => p.BlendSectionId == blendSection.Id).ToList();

                if (productHauls != null)
                {
                    List<int> productHaulLoadIds = productHaulLoadList.Select(p => p.Id).Distinct().ToList();
                    List<Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul> productHauls2 = new List<Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul>();
                    foreach (var productHaul in productHauls)
                    {
                        var productHaulShippingLoadSheets =shippingLoadSheets.FindAll(p => productHaulLoadIds.Contains(p.ProductHaulLoad.Id) && p.ProductHaul.Id == productHaul.Id);
                        if (productHaulShippingLoadSheets.Count > 0 && productHauls2.FindAll(p=>p.Id == productHaul.Id).Count == 0)
                        {
                            productHaul.ShippingLoadSheets = productHaulShippingLoadSheets;
                            productHauls2.Add(productHaul);
                        }
                    }
                    this.GetProductHauls(productHauls2, productHaulLoadList);

                    int count = 0;
                    double totalHauledAmount = this.GetHaulAmount(productHauls2, shippingLoadSheets, null, blendSection.Id, out count);
                    double totalOnLocationAmount = this.GetHaulAmount(productHauls2, shippingLoadSheets, true, blendSection.Id, out count);

                    if (blendSection.IsNeedHaul)
                    {
                        //need confirmed
                        this.GetBlendProductHaulStatusByQuantity(totalHauledAmount, totalOnLocationAmount, blendSection.Quantity);
                    }
                    else
                    {
                        if (!count.Equals(0))
                        {
                            this.GetBlendProductHaulStatusByQuantity(totalHauledAmount, totalOnLocationAmount, blendSection.Quantity);
                        }
                        else
                        {
                            this.BlendProductHaulStatus = BlendProductHaulStatus.GoWithCrew;
                        }
                    }

                }

                List<ProductHaulLoad> scheduledBlendRequest = productHaulLoadList.FindAll(p => p.ProductHaul?.Id == 0);
                if (scheduledBlendRequest.Count > 0)
                {
                    if(this.ProductHaulForBlendModels == null)
                        this.ProductHaulForBlendModels = new List<ProductHaulForBlendModel>();

                    ProductHaulForBlendModel productHaulForBlendModel=new ProductHaulForBlendModel();
                    productHaulForBlendModel.PopulateFrom(new Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul(), scheduledBlendRequest);
                    this.ProductHaulForBlendModels.Add(productHaulForBlendModel);
                }

            }
        }

        private void GetBlendProductHaulStatusByQuantity(double totalHauledAmount, double totalOnLocationAmount, double? quantity)
        {
            if (quantity != null)
            {
                if (Math.Abs(totalHauledAmount) < 0.001)
                {
                    this.BlendProductHaulStatus = BlendProductHaulStatus.NeedAHaul;
                }
                else if (totalHauledAmount >= quantity.Value)
                {
                    if (Math.Abs(totalOnLocationAmount) < 0.001)
                    {
                        this.BlendProductHaulStatus = BlendProductHaulStatus.FullyScheduled;
                    }
                    else if (totalOnLocationAmount >= totalHauledAmount)
                    {
                        this.BlendProductHaulStatus = BlendProductHaulStatus.FullyOnLocation;
                    }
                    else
                    {
                        this.BlendProductHaulStatus = BlendProductHaulStatus.PartialOnLocation;
                    }
                }
                else
                {
                    this.BlendProductHaulStatus = BlendProductHaulStatus.PartialScheduled;
                }
            }
        }

        private void GetProductHauls(List<Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul> productHaulList, List<ProductHaulLoad> productHaulLoads1)
        {
            if (productHaulList != null && productHaulList.Count > 0)
            {
                this.ProductHaulForBlendModels = new List<ProductHaulForBlendModel>();
                foreach (Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul item in productHaulList)
                {
                    ProductHaulForBlendModel productHaulForBlendModel=new ProductHaulForBlendModel();
                    productHaulForBlendModel.PopulateFrom(item, productHaulLoads1);
                    this.ProductHaulForBlendModels.Add(productHaulForBlendModel);
                }
            }
        }

        //isOnLocation is null, get all amount; isOnLocation is true, get on location amount; isOnlocation is false, get not on location amount
        private double GetHaulAmount(List<Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul> productHauls, List<ShippingLoadSheet> shippingLoadSheets1 , bool? isOnLocation, int blendSectionId, out int count)
        {
            if (shippingLoadSheets1 == null)
            {
                count = 0;
                return 0.0;

            }
            double amount = 0;
            int number = 0;
            if (productHauls != null && productHauls.Count > 0)
            {
                foreach (Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul item in productHauls)
                {
                    List<ShippingLoadSheet> shippingLoadSheets = shippingLoadSheets1.FindAll(p=>p.ProductHaul.Id == item.Id && p.BlendSectionId == blendSectionId);
                    number += shippingLoadSheets.Count;
                    if (isOnLocation == null)
                    {
                        foreach (ShippingLoadSheet productHaulLoad in shippingLoadSheets)
                        {
                            amount += productHaulLoad.LoadAmount / 1000;
                        }
                    }
                    else
                    {
                        foreach (ShippingLoadSheet productHaulLoad in shippingLoadSheets)
                        {
                            if ((isOnLocation.Value && productHaulLoad.ShippingStatus==ShippingStatus.OnLocation) || (!isOnLocation.Value && productHaulLoad.ShippingStatus!= ShippingStatus.OnLocation))
                              amount += productHaulLoad.LoadAmount / 1000;
                        }
                    }
                }
            }
            count = number;
            return amount;
        }
    }

    public enum BlendProductHaulStatus
    {
        NeedAHaul = 1,
        PartialScheduled = 2,
        FullyScheduled = 3,
        PartialOnLocation = 4,
        FullyOnLocation = 5,
        GoWithCrew = 6,
    }

    public class ProductHaulForBlendModel
    {
        public int ProductHaulId { get; set; }
        public double BaseBlendWeight { get; set; }
        public bool IsOnLocation { get; set; }
        public bool DisableCancelMenu { get; set; }
        public bool DisableRescheduleMenu { get; set; }
        public string DriverFName { get; set; }
        public string DriverMName { get; set; }
        public string DriverLName { get; set; }
        public string UnitNumber { get; set; }
        public bool IsThirdParty { get; set; }
        public string SupplierCompanyName { get; set; }
        public string ThirdPartyUnitNumber { get; set; }
        public bool IsGoWithCrew { get; set; }
        public DateTime ExpectedOnLocationTime { get; set; }
//        public List<ProductHaulLoad> ProductHaulLoads { get; set; }
        public List<ProductHaulLoadModel> ProductHaulLoadModels { get; set; } = new List<ProductHaulLoadModel>();
        public string CrewName { get; set; }
        public bool IsBlendTest { get; set; }

        public ProductHaulStatus ProductHaulLifeStatus {set;get;}

        public void PopulateFrom(Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule.ProductHaul productHaul, List<ProductHaulLoad> productHaulLoads1)
        {
            this.ProductHaulLifeStatus = productHaul.ProductHaulLifeStatus;
            this.ProductHaulId = productHaul.Id;
            this.IsOnLocation = productHaul.ProductHaulLifeStatus == ProductHaulStatus.OnLocation;
            this.ExpectedOnLocationTime= productHaul.ExpectedOnLocationTime;
            if (this.ProductHaulId != 0)
            {
                this.CrewName = productHaul.Name;

                if (productHaul.ShippingLoadSheets != null && productHaul.ShippingLoadSheets.Count > 0)
                {
                    List<int> productHaulLoadIds = productHaul.ShippingLoadSheets.Select(p => p.ProductHaulLoad.Id)
                        .Distinct().ToList();
                    var productHaulLoads =
                        productHaulLoads1.FindAll(p => productHaulLoadIds.Contains(p.Id)).ToList();
                    if (productHaulLoads.Count > 0)
                    {
                        DateTime expectedOnLocationTime = this.GetDateFromProductLoad(productHaulLoads);
                        if(expectedOnLocationTime>DateTime.MinValue)
                        {
                            this.ExpectedOnLocationTime = expectedOnLocationTime;
                        }
                        
                        foreach (var productHaulLoad in productHaulLoads)
                        {
                            var shippingLoadSheet = productHaul.ShippingLoadSheets.FirstOrDefault(p => p.ProductHaulLoad.Id == productHaulLoad.Id);
                            ProductHaulLoadModel productHaulLoadModel = new ProductHaulLoadModel();
                            productHaulLoadModel.PopulateFrom(productHaulLoad);
                            productHaulLoadModel.ShippingStatus = shippingLoadSheet.ShippingStatus;
                            productHaulLoadModel.HaulAmount = shippingLoadSheet.LoadAmount;
                            this.ProductHaulLoadModels.Add(productHaulLoadModel);
                        }
                    }
                    //Feb 8, 2024 AW: Disable reschedul/cancel product haul only relate to product haul itself
                    this.DisableCancelMenu = productHaul.ProductHaulLifeStatus == ProductHaulStatus.InProgress ||
                                                                       productHaul.ProductHaulLifeStatus == ProductHaulStatus.OnLocation ||
                                                                       productHaul.ProductHaulLifeStatus == ProductHaulStatus.OnLocation;

                    //Apirl 28, 2024 Tongtao 191_PR_AllowReschedulingProductHaulBeforeOnlocation: Allow rescheduling product haul before OnLocation status
                    this.DisableRescheduleMenu = productHaul.ProductHaulLifeStatus == ProductHaulStatus.Empty || productHaul.ProductHaulLifeStatus == ProductHaulStatus.Pending
                                        || productHaul.ProductHaulLifeStatus == ProductHaulStatus.Returned || productHaul.ProductHaulLifeStatus == ProductHaulStatus.OnLocation;

                    this.IsGoWithCrew = productHaul.IsGoWithCrew;
                }
            }
            else
            {
                foreach (var productHaulLoad in productHaulLoads1.FindAll(p=>p.ProductHaul.Id==0))
                {
                    ProductHaulLoadModel productHaulLoadModel = new ProductHaulLoadModel();
                    productHaulLoadModel.PopulateFrom(productHaulLoad);
                    this.ProductHaulLoadModels.Add(productHaulLoadModel);
                }

                //this.DisableCancelMenu = productHaul.ProductHaulLifeStatus==ProductHaulStatus.InProgress || productHaul.ProductHaulLifeStatus==ProductHaulStatus.OnLocation;
                this.DisableCancelMenu = productHaul.ProductHaulLifeStatus != ProductHaulStatus.Scheduled;
                this.IsGoWithCrew = productHaul.IsGoWithCrew;
            }
        }

        private DateTime GetDateFromProductLoad(List<ProductHaulLoad> productHaulLoads)
        {
            DateTime expectedOnLocationTime = DateTime.MaxValue;
            /*
            DateTime dateTime = DateTime.MaxValue;
            foreach (ProductHaulLoad item in productHaulLoads)
            {
                dateTime = dateTime > item.ExpectedOnLocationTime ? item.ExpectedOnLocationTime : dateTime;
            }
            expectedOnLocationTime = dateTime;
            */
            expectedOnLocationTime = productHaulLoads.OrderByDescending(x => x.ExpectedOnLocationTime)
                .FirstOrDefault().ExpectedOnLocationTime;

            return expectedOnLocationTime;
        }

    }
}