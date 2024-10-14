using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using eServiceOnline.Gateway;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment;

namespace eServiceOnline.BusinessProcess
{
    public class ManifoldProcess
    {
        public static List<Manifold> GetManifoldsByServicePoint(int pageSize, int pageNumber, Collection<int> servicePoints, out int count)
        {
            List<Manifold> allManifolds = eServiceOnlineGateway.Instance.GetAllManifolds();
            List<Manifold> manifolds = allManifolds;

            if (servicePoints.Count > 0)
            {
                manifolds = manifolds.Where(p => servicePoints.Contains(p.HomeServicePoint.Id)).ToList();
                List<ManifoldInformation> manifoldInformations = eServiceOnlineGateway.Instance.GetAllManifoldInformations().Where(p => servicePoints.Contains(p.WorkingServicePoint.Id)).ToList();

                foreach (ManifoldInformation manifoldInformation in manifoldInformations)
                {
                    Manifold manifold = manifolds.Find(p => p.Id == manifoldInformation.Manifold.Id);

                    if (manifold == null)
                    {
                        Manifold item = allManifolds.Find(p => p.Id == manifoldInformation.Manifold.Id);
                        if (item != null)
                            manifolds.Add(item);
                    }
                }
            }

            count = manifolds.Count;
            manifolds = manifolds.OrderBy(p => p.Name).ToList();
            manifolds = manifolds.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();

            return manifolds;
        }

        public static void UpdateNotes(int manifoldId, string notes)
        {
            ManifoldInformation manifoldInformation = eServiceOnlineGateway.Instance.GetManifoldInformationByManifoldId(manifoldId);
            if (manifoldInformation == null)
            {
                ManifoldInformation newManifoldInformation = new ManifoldInformation();
                newManifoldInformation.Manifold = eServiceOnlineGateway.Instance.GetManifoldById(manifoldId);
                newManifoldInformation.Notes = notes;
                newManifoldInformation.WorkingServicePoint = newManifoldInformation.Manifold.HomeServicePoint;
                newManifoldInformation.EquipmentStatus = EquipmentStatus.Yard;
                eServiceOnlineGateway.Instance.CreateManifoldInformation(newManifoldInformation);
            }
            else
            {
                manifoldInformation.Notes = notes;
                eServiceOnlineGateway.Instance.UpdateManifoldInformation(manifoldInformation);
            }
        }

        public static List<Manifold> GetManifoldsByServicePoint(int servicePointId)
        {

            List<Manifold> manifolds = new List<Manifold>();
            List<Manifold> allManifolds = eServiceOnlineGateway.Instance.GetAllManifolds();

            if (servicePointId != 0)
            {
                allManifolds = allManifolds.Where(p => p.HomeServicePoint.Id == servicePointId).ToList();
                foreach (var manifold in allManifolds)
                {
                    ManifoldInformation manifoldInformation = eServiceOnlineGateway.Instance.GetManifoldInformationByManifoldId(manifold.Id);
                    if (manifoldInformation == null)
                    {
                        manifolds.Add(manifold);
                    }
                }

                List<ManifoldInformation> manifoldInformations = eServiceOnlineGateway.Instance.GetAllManifoldInformations().Where(p => p.WorkingServicePoint.Id == servicePointId && p.EquipmentStatus == EquipmentStatus.Yard).ToList();
                foreach (var manifoldInformation in manifoldInformations)
                {
                    Manifold manifold = eServiceOnlineGateway.Instance.GetManifoldById(manifoldInformation.Manifold.Id);
                    if (manifold != null)
                    {
                        PlugLoadingHeadInformation plugLoadingHeadInformation = eServiceOnlineGateway.Instance.GetPlugLoadingHeadInformationByManifoldId(manifold.Id);
                        if (plugLoadingHeadInformation==null)
                        {
                            manifolds.Add(manifold);
                        }
                     
                    }
                }

                return manifolds.OrderBy(s=>s.Id).ToList();
            }

            return manifolds;
        }

        public static void AssignPlugLoadingHeadUpdateManifold(int manifoldId,int servicePointId,int callsheetNumber,string location)
        {
            ManifoldInformation manifoldInformation = eServiceOnlineGateway.Instance.GetManifoldInformationByManifoldId(manifoldId);
            if (manifoldInformation!=null)
            {
                manifoldInformation.CallsheetNumber = callsheetNumber;
                manifoldInformation.Location = location;
                manifoldInformation.WorkingServicePoint = eServiceOnlineGateway.Instance.GetServicePointById(servicePointId);
                manifoldInformation.EquipmentStatus = EquipmentStatus.Assigned;
                eServiceOnlineGateway.Instance.UpdateManifoldInformation(manifoldInformation);
            }
            else
            {
                ManifoldInformation newManifoldInformation=new ManifoldInformation();
                newManifoldInformation.CallsheetNumber = callsheetNumber;
                newManifoldInformation.Location = location;
                newManifoldInformation.WorkingServicePoint = eServiceOnlineGateway.Instance.GetServicePointById(servicePointId);
                newManifoldInformation.EquipmentStatus = EquipmentStatus.Assigned;
                newManifoldInformation.Manifold = eServiceOnlineGateway.Instance.GetManifoldById(manifoldId);
                eServiceOnlineGateway.Instance.CreateManifoldInformation(newManifoldInformation);
            }
        }

        public static Manifold PlugLoadingHeadhasManifold(int plugLoadingHeadId)
        {
            Manifold manifold=new Manifold();
            PlugLoadingHeadInformation plugLoadingHeadInformation = eServiceOnlineGateway.Instance.GetPlugLoadingHeadInformationByPlugLoadingHeadId(plugLoadingHeadId);
            if (plugLoadingHeadInformation != null)
            {
                if (plugLoadingHeadInformation.Manifold != null && plugLoadingHeadInformation.Id != 0)
                {
                    manifold = eServiceOnlineGateway.Instance.GetManifoldById(plugLoadingHeadInformation.Manifold.Id);
                    return manifold;
                }
            }
            return manifold;
        }
    }
}