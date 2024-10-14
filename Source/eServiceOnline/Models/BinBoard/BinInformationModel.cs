using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sesi.SanjelData.Entities.BusinessEntities.WellSite;

namespace eServiceOnline.Models.BinBoard
{
    public class BinInformationModel
    {
        public int Id { get; set; }
        public string Name { set; get; }
        public int RigId { get; set; }
        public int BinId { get; set; }
        public string RigName { get; set; }
        public string BinName { get; set; }
        public string Blend { get; set; }
        public int BlendId { get; set; }
        public double Quantity { get; set; }
        public double Capacity { get; set; }
        public string ServicePointName { get; set; }
        public BinStatus Status { get; set; }
        public string Description { get; set; }
        public int PodIndex { set; get; }
        // Dec 28, 2023 zhangyuan 243_PR_AddBlendDropdown:Model Add LastProductHaulLoadId
        public int LastProductHaulLoadId { set; get; }

        public void PopulateFrom(BinInformation entity)
        {
            if (entity != null)
            {
                this.Id = entity?.Id ?? 0;
                this.Status = entity?.BinStatus ?? 0;

                if (entity.Rig != null)
                {
                    this.RigId = entity.Rig.Id;
                    this.RigName = entity.Rig.Name;

                }

                if (entity.Bin != null)
                {
                    this.BinId = entity.Bin.Id;
                    this.BinName = entity.Bin.Name;
                }

                if (entity.WorkingServicePoint != null)
                {
                    this.ServicePointName = entity.WorkingServicePoint.Name;
                }

                this.Blend = entity.BlendChemical?.Name;
                this.BlendId = entity.BlendChemical?.Id ?? 0;
                this.Quantity = entity.Quantity;
                this.Capacity = entity.Capacity;
                this.PodIndex = entity.PodIndex;
                this.Name = entity.Name;
            }
        }

        public void PopulateTo(BinInformation entity)
        {
            entity.Id = this.Id;
        }

    }
}
