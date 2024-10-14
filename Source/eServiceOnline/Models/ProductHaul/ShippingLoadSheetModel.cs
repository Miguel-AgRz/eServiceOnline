using eServiceOnline.Models.ModelBinder;
using Microsoft.AspNetCore.Mvc;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule;
using System.Collections.Generic;
using Sesi.SanjelData.Entities.BusinessEntities.WellSite;
using Sesi.SanjelData.Entities.Common.BusinessEntities.WellSite;

namespace eServiceOnline.Models.ProductHaul
{
    [ModelBinder(BinderType = typeof(CommonModelBinder<ShippingLoadSheetModel>))]
    public class ShippingLoadSheetModel
    {
        public List<BlendUnloadSheet> BlendUnloadSheetModels { set; get; }

        public double LoadAmount { set; get; }

        public int ShippingLoadSheetId { set; get; }
        public int RigId { set; get; }

        public string RigName { set; get; }
        public int ClientId { set; get; }
        public string ClientName { set; get; }

        public int BulkPlantId { set; get; }
        public string BulkPlantName { set; get; }
        public bool IsGoWithCrew { set; get; }
        public string ClientRepresentative { set; get; }
        public int CallSheetNumber { set; get; }
        public int CallSheetId { set; get; }
        public double Quantity { set; get; }

        public void InitializeShippingLoadSheets(Rig rig, List<BinInformation> binInformations)
        {

            this.RigName = rig.Name;
            this.RigId = rig.Id;
            this.BlendUnloadSheetModels = new List<BlendUnloadSheet>();
            foreach (var item in binInformations)
            {
                this.BlendUnloadSheetModels.Add(new BlendUnloadSheet()
                {
                    UnloadAmount = 0,
                    DestinationStorage = item
                });
            }

        }
    }
}
