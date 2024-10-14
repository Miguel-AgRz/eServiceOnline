using System.Collections.Generic;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment;

namespace eServiceOnline.Models.RigBoard
{
    public class ReturnEquipentsModel
    {
        public int Id { get; set; }
        public EquipentType EquipentType { get; set; }
        public string EquipentName { get; set; }
        public int ServicePointId { get; set; }
        public int RigJobId { get; set; }
        public bool WhetherToReturn { get; set; }


       
    }

    public enum EquipentType
    {
        PlugLoadingHead,
        Manifold,
        TopDrivceAdaptor,
        PlugLoadingHeadSub,
        Swedge,
        WitsBox,
        Nubbin
    }
}