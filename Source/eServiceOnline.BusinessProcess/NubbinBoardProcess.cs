using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using eServiceOnline.Gateway;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment;

namespace eServiceOnline.BusinessProcess
{
    public class NubbinBoardProcess
    {
        public static List<Nubbin> GetNubinsByServicePoint(int pageSize, int pageNumber, Collection<int> servicePoints, out int count)
        {
            List<Nubbin> allNubbins = eServiceOnlineGateway.Instance.GetAllNubbins();
            List<Nubbin> nubbins = allNubbins;

            if (servicePoints.Count > 0)
            {
                nubbins = nubbins.Where(p => servicePoints.Contains(p.HomeServicePoint.Id)).ToList();
                List<NubbinInformation> nubbinInformations = eServiceOnlineGateway.Instance.GetAllNubbinInformation().Where(p => servicePoints.Contains(p.WorkingServicePoint.Id)).ToList();

                foreach (NubbinInformation nubbinInformation in nubbinInformations)
                {
                    Nubbin nubbin = nubbins.Find(p => p.Id == nubbinInformation.Nubbin.Id);

                    if (nubbin == null)
                    {
                        Nubbin item = allNubbins.Find(p => p.Id == nubbinInformation.Nubbin.Id);
                        if (item != null)
                            nubbins.Add(item);
                    }
                }
            }

            count = nubbins.Count;
            nubbins = nubbins.OrderBy(p => p.Name).ToList();
            nubbins = nubbins.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();

            return nubbins;
        }

        public static void UpdateNotes(int nubbinId, string notes)
        {
            NubbinInformation nubbinInformation = eServiceOnlineGateway.Instance.GetNubbinInformationByNubbinId(nubbinId);
            if (nubbinInformation == null)
            {
                NubbinInformation newNubbinInformation = new NubbinInformation();
                newNubbinInformation.Nubbin = eServiceOnlineGateway.Instance.GetNubbinById(nubbinId);
                newNubbinInformation.Notes = notes;
                newNubbinInformation.WorkingServicePoint = newNubbinInformation.Nubbin.HomeServicePoint;
                newNubbinInformation.EquipmentStatus = EquipmentStatus.Yard;
                eServiceOnlineGateway.Instance.CreateNubbinInformation(newNubbinInformation);
            }
            else
            {
                nubbinInformation.Notes = notes;
                eServiceOnlineGateway.Instance.UpdateNubbinInformation(nubbinInformation);
            }
        }
    }
}