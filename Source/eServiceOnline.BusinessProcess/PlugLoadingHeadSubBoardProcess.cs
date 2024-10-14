using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using eServiceOnline.Gateway;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment;

namespace eServiceOnline.BusinessProcess
{
    public class PlugLoadingHeadSubBoardProcess
    {
        public static List<PlugLoadingHeadSub> GetPlugLoadingHeadSubsByServicePoint(int pageSize, int pageNumber, Collection<int> servicePoints, out int count)
        {
            List<PlugLoadingHeadSub> allPlugLoadingHeadSubs = eServiceOnlineGateway.Instance.GetAllPlugLoadingHeadSubs();
            List<PlugLoadingHeadSub> plugLoadingHeadSubs = allPlugLoadingHeadSubs;

            if (servicePoints.Count > 0)
            {
                allPlugLoadingHeadSubs = allPlugLoadingHeadSubs.Where(p => servicePoints.Contains(p.HomeServicePoint.Id)).ToList();
                List<PlugLoadingHeadSubInformation> plugLoadingHeadInformations = eServiceOnlineGateway.Instance.GetAllPlugLoadingHeadSubInformations().Where(p => servicePoints.Contains(p.WorkingServicePoint.Id)).ToList();

                foreach (PlugLoadingHeadSubInformation plugLoadingHeadSubInformation in plugLoadingHeadInformations)
                {
                    PlugLoadingHeadSub plugLoadingHeadSub = plugLoadingHeadSubs.Find(p => p.Id == plugLoadingHeadSubInformation.PlugLoadingHeadSub.Id);

                    if (plugLoadingHeadSub == null)
                    {
                        PlugLoadingHeadSub loadingHeadSub = allPlugLoadingHeadSubs.Find(p => p.Id == plugLoadingHeadSubInformation.PlugLoadingHeadSub.Id);
                        if (loadingHeadSub != null)
                            plugLoadingHeadSubs.Add(loadingHeadSub);
                    }
                }
            }
            count = plugLoadingHeadSubs.Count;
            plugLoadingHeadSubs = plugLoadingHeadSubs.OrderBy(p => p.Name).ToList();
            plugLoadingHeadSubs = plugLoadingHeadSubs.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();

            return plugLoadingHeadSubs;
        }

        public static void UpdateNotes(int plugLoadingHeadSubId, string notes)
        {
            PlugLoadingHeadSubInformation plugLoadingHeadSubInformation = eServiceOnlineGateway.Instance.GetPlugLoadingHeadSubInformationByPlugLoadingHeadSubId(plugLoadingHeadSubId);
            if (plugLoadingHeadSubInformation == null)
            {
                PlugLoadingHeadSubInformation loadingHeadSubInformation = new PlugLoadingHeadSubInformation();
                loadingHeadSubInformation.PlugLoadingHeadSub = eServiceOnlineGateway.Instance.GetPlugLoadingHeadSubById(plugLoadingHeadSubId);
                loadingHeadSubInformation.Notes = notes;
                loadingHeadSubInformation.WorkingServicePoint = loadingHeadSubInformation.PlugLoadingHeadSub.HomeServicePoint;
                loadingHeadSubInformation.EquipmentStatus = EquipmentStatus.Yard;
                eServiceOnlineGateway.Instance.CreatePlugLoadingHeadSubInformation(loadingHeadSubInformation);
            }
            else
            {
                plugLoadingHeadSubInformation.Notes = notes;
                eServiceOnlineGateway.Instance.UpdatePlugLoadingHeadSubInformation(plugLoadingHeadSubInformation);
            }
        }

        public static List<PlugLoadingHeadSub> GetPlugLoadingHeadSubsByServicePoint(int servicePointId)
        {
            List<PlugLoadingHeadSub> plugLoadingHeadSubs = new List<PlugLoadingHeadSub>();
            List<PlugLoadingHeadSub> allPlugLoadingHeadSubs = eServiceOnlineGateway.Instance.GetAllPlugLoadingHeadSubs();

            if (servicePointId != 0)
            {
                allPlugLoadingHeadSubs = allPlugLoadingHeadSubs.Where(p => p.HomeServicePoint.Id == servicePointId).ToList();
                foreach (var plugLoadingHeadSub in allPlugLoadingHeadSubs)
                {
                    PlugLoadingHeadSubInformation plugLoadingHeadSubInformation = eServiceOnlineGateway.Instance.GetPlugLoadingHeadSubInformationByPlugLoadingHeadSubId(plugLoadingHeadSub.Id);
                    if (plugLoadingHeadSubInformation == null)
                    {
                        plugLoadingHeadSubs.Add(plugLoadingHeadSub);
                    }
                }

                List<PlugLoadingHeadSubInformation> plugLoadingHeadSubInformations = eServiceOnlineGateway.Instance.GetAllPlugLoadingHeadSubInformations().Where(p => p.WorkingServicePoint.Id == servicePointId && p.EquipmentStatus == EquipmentStatus.Yard).ToList();
                foreach (var plugLoadingHeadSubInformation in plugLoadingHeadSubInformations)
                {
                    PlugLoadingHeadSub plugLoadingHeadSub = eServiceOnlineGateway.Instance.GetPlugLoadingHeadSubById(plugLoadingHeadSubInformation.PlugLoadingHeadSub.Id);
                    if (plugLoadingHeadSub != null)
                    {
                        plugLoadingHeadSubs.Add(plugLoadingHeadSub);
                    }
                }

                return plugLoadingHeadSubs.OrderBy(s=>s.Id).ToList();
            }

            return plugLoadingHeadSubs;
        }

        public static void AssignPlugLoadingHeadUpdatePlugLoadingHeadSub(int plugLoadingHeadSubId, int servicePointId, int callsheetNumber, string location)
        {
            PlugLoadingHeadSubInformation plugLoadingHeadSubInformation = eServiceOnlineGateway.Instance.GetPlugLoadingHeadSubInformationByPlugLoadingHeadSubId(plugLoadingHeadSubId);
            if (plugLoadingHeadSubInformation != null)
            {
                plugLoadingHeadSubInformation.CallsheetNumber = callsheetNumber;
                plugLoadingHeadSubInformation.Location = location;
                plugLoadingHeadSubInformation.WorkingServicePoint = eServiceOnlineGateway.Instance.GetServicePointById(servicePointId);
                plugLoadingHeadSubInformation.EquipmentStatus = EquipmentStatus.Assigned;
                eServiceOnlineGateway.Instance.UpdatePlugLoadingHeadSubInformation(plugLoadingHeadSubInformation);
            }
            else
            {
                PlugLoadingHeadSubInformation newPlugLoadingHeadSubInformation = new PlugLoadingHeadSubInformation();
                newPlugLoadingHeadSubInformation.CallsheetNumber = callsheetNumber;
                newPlugLoadingHeadSubInformation.Location = location;
                newPlugLoadingHeadSubInformation.WorkingServicePoint = eServiceOnlineGateway.Instance.GetServicePointById(servicePointId);
                newPlugLoadingHeadSubInformation.EquipmentStatus = EquipmentStatus.Assigned;
                newPlugLoadingHeadSubInformation.PlugLoadingHeadSub = eServiceOnlineGateway.Instance.GetPlugLoadingHeadSubById(plugLoadingHeadSubId);
                eServiceOnlineGateway.Instance.CreatePlugLoadingHeadSubInformation(newPlugLoadingHeadSubInformation);
            }
        }
    }
}