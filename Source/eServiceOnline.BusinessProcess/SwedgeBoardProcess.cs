using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using eServiceOnline.Gateway;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment;

namespace eServiceOnline.BusinessProcess
{
    public class SwedgeBoardProcess
    {
        public static List<Swedge> GetSwedgesByServicePoint(int pageSize, int pageNumber, Collection<int> servicePoints, out int count)
        {
            List<Swedge> allSedge = eServiceOnlineGateway.Instance.GetAllSwedge();
            List<Swedge> swedges = allSedge;

            if (servicePoints.Count > 0)
            {
                swedges = swedges.Where(p => servicePoints.Contains(p.HomeServicePoint.Id)).ToList();
                List<SwedgeInformation> swedgeInformations = eServiceOnlineGateway.Instance.GetSwedgeInformation().Where(p => servicePoints.Contains(p.WorkingServicePoint.Id)).ToList();

                foreach (SwedgeInformation swedgeInformation in swedgeInformations)
                {
                    Swedge swedge = swedges.Find(p => p.Id == swedgeInformation.Swedge.Id);

                    if (swedge == null)
                    {
                        Swedge item = allSedge.Find(p => p.Id == swedgeInformation.Swedge.Id);
                        if (item != null)
                            swedges.Add(item);
                    }
                }
            }

            count = swedges.Count;
            swedges = swedges.OrderBy(p => p.Name).ToList();
            swedges = swedges.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();

            return swedges;
        }

        public static void UpdateNotes(int swedgeId, string notes)
        {
            SwedgeInformation swedgeInformation = eServiceOnlineGateway.Instance.GetSwedgeInformationBySwedgeId(swedgeId);
            if (swedgeInformation == null)
            {
                SwedgeInformation newSwedgeInformation = new SwedgeInformation();
                newSwedgeInformation.Swedge = eServiceOnlineGateway.Instance.GetSwedgeById(swedgeId);
                newSwedgeInformation.Notes = notes;
                newSwedgeInformation.WorkingServicePoint = newSwedgeInformation.Swedge.HomeServicePoint;
                newSwedgeInformation.EquipmentStatus = EquipmentStatus.Yard;
                eServiceOnlineGateway.Instance.CreateSwedgeInformation(newSwedgeInformation);
            }
            else
            {
                swedgeInformation.Notes = notes;
                eServiceOnlineGateway.Instance.UpdateSwedgeInformation(swedgeInformation);
            }
        }
    }
}