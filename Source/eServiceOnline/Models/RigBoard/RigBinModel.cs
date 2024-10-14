using Sesi.SanjelData.Entities.BusinessEntities.WellSite;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace eServiceOnline.Models.RigBoard
{
    public class RigBinModel
    {
        public string BinNumber { get; set; }
        public int BinId { get; set; }
        public int BinTypeId { get; set; }
        public string BinTypeDescription { get; set; }
        public int PodCount { set; get; }
        public int RigJobId { set; get; }
        public double Volume { get; set; }
        public Collection<BinInformation> BinInformationList { set; get; } = new Collection<BinInformation>();

        public void PopulateForm(Bin bin)
        {
            this.BinId=bin.Id;
            this.PodCount = bin.PodCount;
            this.Volume=bin.Volume;
        }
    }
}
