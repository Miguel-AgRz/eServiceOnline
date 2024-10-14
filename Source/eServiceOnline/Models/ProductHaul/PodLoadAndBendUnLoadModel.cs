using eServiceOnline.Models.ModelBinder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule;
using System.Collections.Generic;

namespace eServiceOnline.Models.ProductHaul
{
    //[ModelBinder(BinderType = typeof(CommonModelBinder<PodLoadAndBendUnLoadModel>))]
    public class PodLoadAndBendUnLoadModel
    {
        public List<BlendUnloadSheet> BlendUnloadSheetModels { set; get; }
        public List<PodLoad> PodLoadModels { set; get; }

        public double LoadAmount { set; get; }

        public int ShippingLoadSheetId { set; get; }
        public int RigId { set; get; }

        public string RigName { set; get; }

        public bool IsGoWithCrew { set; get; }

        public int CallSheetNumber { set; get; }
        public string ProgramId { set; get; }

        public string Blend { set; get; }

        public bool IsCheckShippingLoadSheet { set; get; }

        //Dec 7, 2023 zhangyuan 224_PR_ShowCallSheetList: add view model define 
        public List<SelectListItem> CallSheetNumbers { set; get; }

        public int CallSheetId { set; get; }

        //Feb 05, 2023 tongtao 279_PR_CouldNotChangeCallSheet: add  view model IsRigJobBlend 

        public bool IsRigJobBlend { set; get; }

        public string FromRigBulkPlant { set; get; }

        public string FromBin { set; get; }

    }


}
