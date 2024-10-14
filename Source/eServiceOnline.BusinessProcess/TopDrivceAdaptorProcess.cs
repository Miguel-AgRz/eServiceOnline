using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using eServiceOnline.Gateway;
using Sesi.SanjelData.Entities.BusinessEntities.WellSite;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment;

namespace eServiceOnline.BusinessProcess
{
    public class TopDrivceAdaptorProcess
    {
        public static List<TopDriveAdaptor> GeTopDriveAdaptorByServicePoint(int pageSize, int pageNumber, Collection<int> servicePoints, out int count)
        {
            List<TopDriveAdaptor> allTopDriveAdaptor =eServiceOnlineGateway.Instance.GetAllTopDriveAdaptors();
            List<TopDriveAdaptor> topDriveAdaptors = allTopDriveAdaptor;

            if (servicePoints.Count > 0)
            {
                topDriveAdaptors = topDriveAdaptors.Where(p => servicePoints.Contains(p.HomeServicePoint.Id)).ToList();
                List<TopDrivceAdaptorInformation> drivceAdaptorInformations = eServiceOnlineGateway.Instance.GetAllTopDrivceAdaptorInformations().Where(p => servicePoints.Contains(p.WorkingServicePoint.Id)).ToList();

                foreach (TopDrivceAdaptorInformation topDrivceAdaptorInformation in drivceAdaptorInformations)
                {
                    TopDriveAdaptor topDriveAdaptor = topDriveAdaptors.Find(p => p.Id == topDrivceAdaptorInformation.TopDriveAdaptor.Id);

                    if (topDriveAdaptor == null)
                    {
                        TopDriveAdaptor driveAdaptor = allTopDriveAdaptor.Find(p => p.Id == topDrivceAdaptorInformation.TopDriveAdaptor.Id);
                        if (driveAdaptor != null)
                            topDriveAdaptors.Add(driveAdaptor);
                    }
                }
            }
            count = topDriveAdaptors.Count;
            topDriveAdaptors = topDriveAdaptors.OrderBy(p => p.Name).ToList();
            topDriveAdaptors = topDriveAdaptors.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();

            return topDriveAdaptors;
        }

        public static void UpdateNotes(int topDriveAdaptorId, string notes)
        {
            TopDrivceAdaptorInformation topDrivceAdaptor = eServiceOnlineGateway.Instance.GetTopDrivceAdaptorInformationByTopDrivceAdaptorId(topDriveAdaptorId);
            if (topDrivceAdaptor == null)
            {
                TopDrivceAdaptorInformation topDrivceAdaptorInformation = new TopDrivceAdaptorInformation();
                topDrivceAdaptorInformation.TopDriveAdaptor = eServiceOnlineGateway.Instance.GetTopDriveAdaptorById(topDriveAdaptorId);
                topDrivceAdaptorInformation.Notes = notes;
                topDrivceAdaptorInformation.WorkingServicePoint = topDrivceAdaptorInformation.TopDriveAdaptor.HomeServicePoint;
                topDrivceAdaptorInformation.EquipmentStatus = EquipmentStatus.Yard;
                eServiceOnlineGateway.Instance.CreateTopDrivceAdaptorInformation(topDrivceAdaptorInformation);
            }
            else
            {
                topDrivceAdaptor.Notes = notes;
                eServiceOnlineGateway.Instance.UpdateTopDrivceAdaptorInformation(topDrivceAdaptor);
            }
        }

        public static List<TopDriveAdaptor> GetTopDriveAdaptorsByServicePoint(int servicePointId)
        {
            List<TopDriveAdaptor> topDriveAdaptors = new List<TopDriveAdaptor>();
            List<TopDriveAdaptor> allTopDriveAdaptors = eServiceOnlineGateway.Instance.GetAllTopDriveAdaptors();

            if (servicePointId != 0)
            {
                allTopDriveAdaptors = allTopDriveAdaptors.Where(p => p.HomeServicePoint.Id == servicePointId).ToList();
                foreach (var topDriveAdaptor in allTopDriveAdaptors)
                {
                    TopDrivceAdaptorInformation topDrivceAdaptorInformation = eServiceOnlineGateway.Instance.GetTopDrivceAdaptorInformationByTopDrivceAdaptorId(topDriveAdaptor.Id);
                    if (topDrivceAdaptorInformation == null)
                    {
                        topDriveAdaptors.Add(topDriveAdaptor);
                    }
                }

                List<TopDrivceAdaptorInformation> topDrivceAdaptorInformations = eServiceOnlineGateway.Instance.GetAllTopDrivceAdaptorInformations().Where(p => p.WorkingServicePoint.Id == servicePointId && p.EquipmentStatus == EquipmentStatus.Yard).ToList();
                foreach (var topDrivceAdaptorInformation in topDrivceAdaptorInformations)
                {
                    TopDriveAdaptor topDriveAdaptor = eServiceOnlineGateway.Instance.GetTopDriveAdaptorById(topDrivceAdaptorInformation.TopDriveAdaptor.Id);
                    if (topDriveAdaptor!=null)
                    {
                        topDriveAdaptors.Add(topDriveAdaptor);
                    }
                }

                return topDriveAdaptors.OrderBy(s=>s.Id).ToList();
            }

            return topDriveAdaptors;
        }

        public static void AssignPlugLoadingHeadUpdateTopDriveAdaptor(int topDriveAdaptorId, int servicePointId, int callsheetNumber, string location)
        {
            TopDrivceAdaptorInformation topDrivceAdaptorInformation = eServiceOnlineGateway.Instance.GetTopDrivceAdaptorInformationByTopDrivceAdaptorId(topDriveAdaptorId);
            if (topDrivceAdaptorInformation != null)
            {
                topDrivceAdaptorInformation.CallsheetNumber = callsheetNumber;
                topDrivceAdaptorInformation.Location = location;
                topDrivceAdaptorInformation.WorkingServicePoint = eServiceOnlineGateway.Instance.GetServicePointById(servicePointId);
                topDrivceAdaptorInformation.EquipmentStatus = EquipmentStatus.Assigned;
                eServiceOnlineGateway.Instance.UpdateTopDrivceAdaptorInformation(topDrivceAdaptorInformation);
            }
            else
            {
                TopDrivceAdaptorInformation newTopDrivceAdaptorInformation = new TopDrivceAdaptorInformation();
                newTopDrivceAdaptorInformation.CallsheetNumber = callsheetNumber;
                newTopDrivceAdaptorInformation.Location = location;
                newTopDrivceAdaptorInformation.WorkingServicePoint = eServiceOnlineGateway.Instance.GetServicePointById(servicePointId);
                newTopDrivceAdaptorInformation.EquipmentStatus = EquipmentStatus.Assigned;
                newTopDrivceAdaptorInformation.TopDriveAdaptor = eServiceOnlineGateway.Instance.GetTopDriveAdaptorById(topDriveAdaptorId);
                eServiceOnlineGateway.Instance.CreateTopDrivceAdaptorInformation(newTopDrivceAdaptorInformation);
            }
        }

    }
}