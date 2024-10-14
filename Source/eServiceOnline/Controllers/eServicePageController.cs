using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eServiceOnline.Models.Commons;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace eServiceOnline.Controllers
{
    public class 
        eServicePageController : eServiceOnlineController
    {
        public void UpdateSessionValue(RetrievalCondition retrieval)
        {
            string retrievalStr = JsonConvert.SerializeObject(retrieval);
            this.HttpContext.Session.SetString("ServicePoint", retrievalStr);
        }


        public void CleanAllSessionValue()
        {
            var retrievalStr = this.HttpContext.Session.GetString("ServicePoint");
            RetrievalCondition retrieval = JsonConvert.DeserializeObject<RetrievalCondition>(retrievalStr);

            retrieval.BR = 0;
            retrieval.RD = 0;
            retrieval.Edm = 0;
            retrieval.Eds = 0;
            retrieval.NW = 0;
            retrieval.GP = 0;
            retrieval.FSJ = 0;
            retrieval.EST = 0;
            retrieval.SC = 0;
            retrieval.KD = 0;
            retrieval.LLD = 0;
            retrieval.LLB = 0;

            this.HttpContext.Session.SetString("ServicePoint", JsonConvert.SerializeObject(retrieval));
        }

        public void SetDistrictSelection(string selectedDistricts)
        {
            
            if (!string.IsNullOrEmpty(selectedDistricts))
            {
                var retrievalStr = this.HttpContext.Session.GetString("ServicePoint");
                RetrievalCondition retrieval= null;
                if ( retrievalStr == null)
                {
                    retrieval = new RetrievalCondition();
                }
                else
                {
                    retrieval = JsonConvert.DeserializeObject<RetrievalCondition>(retrievalStr);
                }
                List<int> districtIdList = StringToIntList(selectedDistricts).Distinct().ToList();

                retrieval.BR = districtIdList.Contains(69) ? 69 : 0;
                retrieval.RD = districtIdList.Contains(85) ? 85 : 0;
                retrieval.Edm = districtIdList.Contains(67) ? 67 : 0;
                retrieval.Eds = districtIdList.Contains(81) ? 81 : 0;
                retrieval.GP = districtIdList.Contains(71) ? 71 : 0;
                retrieval.FSJ = districtIdList.Contains(66) ? 66 : 0;
                retrieval.NW = districtIdList.Contains(89) ? 89 : 0;
                retrieval.EST = districtIdList.Contains(72) ? 72 : 0;
                retrieval.SC = districtIdList.Contains(70) ? 70 : 0;
                retrieval.KD = districtIdList.Contains(88) ? 88 : 0;
                retrieval.LLD = districtIdList.Contains(61) ? 61 : 0;
                retrieval.LLB = districtIdList.Contains(65) ? 65 : 0;

                retrievalStr = JsonConvert.SerializeObject(retrieval);
                this.HttpContext.Session.SetString("ServicePoint", retrievalStr);
            }
        }

        public static IEnumerable<int> StringToIntList(string str) {
            if (String.IsNullOrEmpty(str))
                yield break;

            foreach(var s in str.Split(',')) {
                int num;
                if (int.TryParse(s, out num))
                    yield return num;
            }
        }
    }
}
