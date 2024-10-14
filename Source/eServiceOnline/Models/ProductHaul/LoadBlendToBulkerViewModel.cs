using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eServiceOnline.Models.ProductHaul
{
    //Nov 13, 2023 Tongtao P45_Q4_175: add a viewModel for"Load Blend to Bulker"
    //Nov 17, 2023 Tongtao P45_Q4_175:Add Blend,Destination,CallSheetNumber,ClientName for "Load Blend to Bulker" page show
    //Dec 07, 2023 Tongtao P45_Q4_175:Add PodLoads,Crew,CallSheetNumber,IsGoWithCrew for "Load Blend to Bulker" page show
    //Dec 08, 2023 Tongtao 175_PR_LoadToBulker:Add IsThirdParty and change Crew to CrewDescription  for "Load Blend to Bulker" page show
    //Jan 10, 2024 Tongtao :add LoadAmount and ExpectedOnLocationTime
    public class LoadBlendToBulkerViewModel
    {
        public int ProductHaulId { set; get; }
        public int ShippingLoadSheetId { set; get; }
        public int SourceStorageId { set; get; }

        public string  Blend { set; get; }

        public string Destination { set; get; }

        public string CallSheetNumber  { set; get; }

        public string ClientName { set; get; }


        public string ProgramId { set; get; }


        public string CrewDescription { set; get; }

        public int CrewId { set; get; }

        public bool IsThirdParty { get; set; }

        public List<PodLoad> PodLoads { set; get; }

        public bool IsGoWithCrew { get; set; }

        public string ExpectedOlTime { get; set; }

        public string LoadAmount { get; set; }
    }
}