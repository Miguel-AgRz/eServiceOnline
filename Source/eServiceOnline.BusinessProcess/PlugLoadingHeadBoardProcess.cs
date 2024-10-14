using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using eServiceOnline.Gateway;
using Sesi.SanjelData.Entities.BusinessEntities.WellSite;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment;

namespace eServiceOnline.BusinessProcess
{
    public class PlugLoadingHeadBoardProcess
    {
        public static List<PlugLoadingHead> GetPlugLoadingHeadsByServicePoint(int pageSize, int pageNumber, Collection<int> servicePoints, out int count)
        {
            List<PlugLoadingHead> allPlugLoadingHeads =eServiceOnlineGateway.Instance.GetAllPlugLoadingHeads();
            List<PlugLoadingHead> plugLoadingHeads = allPlugLoadingHeads;

            if (servicePoints.Count > 0)
            {
                plugLoadingHeads = plugLoadingHeads.Where(p => servicePoints.Contains(p.HomeServicePoint.Id)).ToList();
                List<PlugLoadingHeadInformation> plugLoadingHeadInformations =eServiceOnlineGateway.Instance.GetPlugLoadingHeadInformations().Where(p => servicePoints.Contains(p.WorkingServicePoint.Id)).ToList();

                foreach (PlugLoadingHeadInformation plugLoadingHeadInformation in plugLoadingHeadInformations)
                {
                    PlugLoadingHead loadingHead = plugLoadingHeads.Find(p => p.Id == plugLoadingHeadInformation.PlugLoadingHead.Id);

                    if (loadingHead == null)
                    {
                        PlugLoadingHead plugLoadingHead = allPlugLoadingHeads.Find(p => p.Id == plugLoadingHeadInformation.PlugLoadingHead.Id);
                        if (plugLoadingHead != null)
                            plugLoadingHeads.Add(plugLoadingHead);
                    }
                }
            }
            count = plugLoadingHeads.Count;
            plugLoadingHeads = plugLoadingHeads.OrderBy(p => p.Name).ToList();
            plugLoadingHeads = plugLoadingHeads.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();

            return plugLoadingHeads;
        }


        public static void UpdateNotes(int plugLoadingHeadId,string notes)
        {
            PlugLoadingHeadInformation plugLoadingHeadInformation = eServiceOnlineGateway.Instance.GetPlugLoadingHeadInformationByPlugLoadingHeadId(plugLoadingHeadId);
            if (plugLoadingHeadInformation==null)
            {
                PlugLoadingHeadInformation newPlugLoadingHeadInformation =new PlugLoadingHeadInformation();
                newPlugLoadingHeadInformation.PlugLoadingHead = eServiceOnlineGateway.Instance.GetPlugLoadingHeadById(plugLoadingHeadId);
                newPlugLoadingHeadInformation.Notes = notes;
                newPlugLoadingHeadInformation.WorkingServicePoint = newPlugLoadingHeadInformation.PlugLoadingHead.HomeServicePoint;
                newPlugLoadingHeadInformation.EquipmentStatus = EquipmentStatus.Yard;
                eServiceOnlineGateway.Instance.CreatePlugLoadingHeadInformation(newPlugLoadingHeadInformation);
            }
            else
            {
                plugLoadingHeadInformation.Notes = notes;
               
                eServiceOnlineGateway.Instance.UpdatePlugLoadingHeadInformation(plugLoadingHeadInformation);
            }
        }

        public static List<PlugLoadingHead> GetPlugLoadingHeadsByServicePoint(int servicePointId)
        {
            List<PlugLoadingHead> plugLoadingHeads = new List<PlugLoadingHead>();
            List<PlugLoadingHead> allPlugLoadingHeads = eServiceOnlineGateway.Instance.GetAllPlugLoadingHeads();

            if (servicePointId != 0)
            {
                allPlugLoadingHeads = allPlugLoadingHeads.Where(p => p.HomeServicePoint.Id == servicePointId).ToList();
                foreach (var plugLoadingHead in allPlugLoadingHeads)
                {
                    PlugLoadingHeadInformation plugLoadingHeadInformation = eServiceOnlineGateway.Instance.GetPlugLoadingHeadInformationByManifoldId(plugLoadingHead.Id);
                    if (plugLoadingHeadInformation == null)
                    {
                        plugLoadingHeads.Add(plugLoadingHead);
                    }
                }

                List<PlugLoadingHeadInformation> plugLoadingHeadInformations = eServiceOnlineGateway.Instance.GetPlugLoadingHeadInformations().Where(p => p.WorkingServicePoint.Id == servicePointId && p.EquipmentStatus == EquipmentStatus.Yard).ToList();
                foreach (var plugLoadingHeadInformation in plugLoadingHeadInformations)
                {
                    PlugLoadingHead plugLoadingHeadById = eServiceOnlineGateway.Instance.GetPlugLoadingHeadById(plugLoadingHeadInformation.Manifold.Id);
                    if (plugLoadingHeadById != null)
                    {
                        plugLoadingHeads.Add(plugLoadingHeadById);
                    }
                }

                return plugLoadingHeads.OrderBy(s=>s.Id).ToList();
            }

            return plugLoadingHeads;
        }
    }
}