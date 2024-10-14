using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using eServiceOnline.Gateway;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule;
using Sesi.SanjelData.Entities.BusinessEntities.WellSite;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Organization;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Products;
using Sesi.SanjelData.Entities.Common.BusinessEntities.WellSite;

namespace eServiceOnline.BusinessProcess
{
    public class BinBoardProcess
    {
        public static List<Bin> GetBinsByServicePoint(int pageSize, int pageNumber, Collection<int> servicePoints, out int count)
        {
            List<Bin> allBins = EServiceReferenceData.Data.BinCollection.ToList();
            List<Bin> bins = allBins;

            if (servicePoints.Count > 0)
            {
                bins = bins.Where(p => servicePoints.Contains(p.HomeServicePoint.Id)).ToList();
                List<BinInformation> binInformations = eServiceOnlineGateway.Instance.GetBinInformations().Where(p => servicePoints.Contains(p.WorkingServicePoint.Id)).ToList();

                foreach (BinInformation rigBinSection in binInformations)
                {
                    Bin assignedBin = bins.Find(p => p.Id == rigBinSection.Bin.Id);

                    if (assignedBin == null)
                    {
                        Bin bin = allBins.Find(p => p.Id == rigBinSection.Bin.Id);
                        if (bin != null)
                            bins.Add(bin);
                    }
                }
            }
            count = bins.Count;
            bins = bins.OrderBy(p => p.Name).ToList();
            bins = bins.Skip(pageSize*(pageNumber - 1)).Take(pageSize).ToList();

            return bins;
        }

        public static List<BinInformation> GetBinInformationsByServicePoint(int pageSize, int pageNumber, Collection<int> servicePoints, out int count)
        {
            List<BinInformation> binInformations = new List<BinInformation>();

            if (servicePoints.Count > 0)
            {
                 binInformations = eServiceOnlineGateway.Instance.GetBinInformations().Where(p => servicePoints.Contains(p.WorkingServicePoint.Id)).ToList();

               
            }
            count = binInformations.Count;
            binInformations = binInformations.OrderBy(p => p.Name).ToList();
            binInformations = binInformations.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();

            return binInformations;
        }
        public static void UpdateQuantity(int binId, double quantity, string notes, string username,int podIndex, int blendId)
        {
            BinInformation binInformation = eServiceOnlineGateway.Instance.GetBinInformationByBinIdAndPodIndex(binId,podIndex);
            if (binInformation == null) throw new Exception("Bin Assignment doesn't exist.");
            // Dec 27, 2023 zhangyuan 243_PR_AddBlendDropdown:Add Save blendChemical.
            if (blendId > 0)
            {
                BlendChemical blendChemical = CacheData.BlendChemicals.FirstOrDefault(p => p.Id == blendId);
                binInformation.BlendChemical = blendChemical;
                // Jan 11, 2024 zhangyuan 243_PR_AddBlendDropdown: modify fix blendChemical.Description is null
                binInformation.BlendChemical.Description = blendChemical.Description?? blendChemical.Name;
            }

            binInformation.Quantity = quantity;
                eServiceOnlineGateway.Instance.UpdateBinInformation(binInformation);

                BinLoadHistory binLoadHistory = new BinLoadHistory()
                {
                    BinInformation = binInformation, Description = "Update Blend Quantity",
                    BlendName =  binInformation.BlendChemical.Name, BlendDescription = binInformation.BlendChemical.Description, 
                    InQuantity = 0, OutQuantity = 0, Remains = binInformation.Quantity, Username = username, Notes = notes, TimeStamp = DateTime.Now
                };
                eServiceOnlineGateway.Instance.CreateBinLoadHistory(binLoadHistory);
        }
            
        public static void UpdateCapacity(int binId, double capacity,int podIndex)
        {
            BinInformation binInformation = eServiceOnlineGateway.Instance.GetBinInformationByBinIdAndPodIndex(binId,podIndex);
            if (binInformation != null)
            {             
                binInformation.Capacity = capacity;
                eServiceOnlineGateway.Instance.UpdateBinInformation(binInformation);
            }
            else
            {
                BinInformation newBinInformation = new BinInformation();
                newBinInformation.Bin = eServiceOnlineGateway.Instance.GetBinById(binId);
                newBinInformation.Capacity=capacity;
                newBinInformation.PodIndex = podIndex;
                eServiceOnlineGateway.Instance.CreateBinInformation(newBinInformation);
            }
        }

        public static void UpdateBinInformationFormAssignBin(Bin bin,Rig rig,ServicePoint workingServicePoint, Collection<BinInformation> binInformationList)
        {
            var binInformationListData = eServiceOnlineGateway.Instance.GetBinInformationByBinId(bin.Id);
//Revert the logic to fix the bug:Assign Bin created new BinInformation Record, but not disabled previous record.

            if (binInformationListData == null || binInformationListData.Count == 0)
            {
                foreach (BinInformation binInformation in binInformationList)
                {
                    if(string.IsNullOrEmpty(binInformation.Name))
                    {
                        continue;
                    }
                    binInformation.Bin = bin;
                    binInformation.Rig = rig;
                    binInformation.BinStatus = BinStatus.Assigned;
                    binInformation.WorkingServicePoint = workingServicePoint;
                    eServiceOnlineGateway.Instance.CreateBinInformation(binInformation);
                }

            }
            else
            {
                foreach (var item in binInformationListData)
                {
                    item.Rig = rig;
                    item.BinStatus = BinStatus.Assigned;
                    item.WorkingServicePoint = workingServicePoint;
                    eServiceOnlineGateway.Instance.UpdateBinInformation(item);
                }
            }
        }

        public static void CreateOrUpdateBinformationFormProductHaulLoad(BlendChemical blendChemical, Bin bin, Rig rig,int podIndex)
        {
            BinInformation binInformation = eServiceOnlineGateway.Instance.GetBinInformationByBinIdAndPodIndex(bin.Id,podIndex);
            if (binInformation != null)
            {             
                binInformation.BlendChemical = blendChemical;
                binInformation.Bin = bin;          
                binInformation.Rig = rig;
                eServiceOnlineGateway.Instance.UpdateBinInformation(binInformation);
            }
            else
            {
                BinInformation newBinInformation = new BinInformation();
                newBinInformation.BlendChemical = blendChemical;
                newBinInformation.Bin = bin;
                newBinInformation.Rig = rig;
                eServiceOnlineGateway.Instance.CreateBinInformation(newBinInformation);
            }
        }

        public static void CalculateBinformation(List<ProductHaulLoad> productHaulLoads,int podIndex)
        {
            foreach (var productHaulLoad in productHaulLoads)
            {
                BinInformation binInformation = eServiceOnlineGateway.Instance.GetBinInformationByBinIdAndPodIndex(productHaulLoad?.Bin?.Id ?? 0,podIndex);
                if (binInformation!=null&&binInformation.BlendChemical!=null)
                {
                    binInformation.Quantity += productHaulLoad.IsTotalBlendTonnage ? productHaulLoad.TotalBlendWeight/1000 : productHaulLoad.BaseBlendWeight/1000;
                    eServiceOnlineGateway.Instance.UpdateBinInformation(binInformation);
                }
            }
        }

        public static void UpdateBlend(int binId, int blendId, int podIndex, string username=null)
        {
            BinInformation binInformation = eServiceOnlineGateway.Instance.GetBinInformationByBinIdAndPodIndex(binId,podIndex);
            if(binInformation == null) throw new Exception("Bin assignment doesn't exist.");

            binInformation.BlendChemical = CacheData.BlendChemicals.FirstOrDefault(s=>s.Id==blendId);
            if (binInformation.BlendChemical == null) throw new Exception("Selected blend id is invalid");
                
            binInformation.BlendChemical.Description = null;

            eServiceOnlineGateway.Instance.UpdateBinInformation(binInformation);

            BinLoadHistory binLoadHistory = new BinLoadHistory(){BinInformation = binInformation, Description = "Update Blend Name", BlendName = binInformation.BlendChemical.Description ?? binInformation.BlendChemical.Name, InQuantity = 0, OutQuantity = 0, Remains = binInformation.Quantity, Username = username, TimeStamp = DateTime.Now};
            eServiceOnlineGateway.Instance.CreateBinLoadHistory(binLoadHistory);

        }
    }
}