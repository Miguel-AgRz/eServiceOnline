using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using eServiceOnline.Gateway;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment;

namespace eServiceOnline.BusinessProcess
{
    public class WitsBoxBoardProcess
    {
        public static List<WitsBox> GetWitsBoxsByServicePoint(int pageSize, int pageNumber, Collection<int> servicePoints, out int count)
        {
            List<WitsBox> allWitsBoxs = eServiceOnlineGateway.Instance.GetWitsBox();
            List<WitsBox> witsBoxs = allWitsBoxs;

            if (servicePoints.Count > 0)
            {
                witsBoxs = witsBoxs.Where(p => servicePoints.Contains(p.HomeServicePoint.Id)).ToList();
                List<WitsBoxInformation> witsBoxInformations = eServiceOnlineGateway.Instance.GetWitsBoxInformation().Where(p => servicePoints.Contains(p.WorkingServicePoint.Id)).ToList();

                foreach (WitsBoxInformation witsBoxInformation in witsBoxInformations)
                {
                    WitsBox witsBox = witsBoxs.Find(p => p.Id == witsBoxInformation.WitsBox.Id);

                    if (witsBox == null)
                    {
                        WitsBox item = allWitsBoxs.Find(p => p.Id == witsBoxInformation.WitsBox.Id);
                        if (item != null)
                            witsBoxs.Add(item);
                    }
                }
            }

            count = witsBoxs.Count;
            witsBoxs = witsBoxs.OrderBy(p => p.Name).ToList();
            witsBoxs = witsBoxs.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();

            return witsBoxs;
        }


        public static void UpdateNotes(int witsBoxId, string notes)
        {
            WitsBoxInformation witsBoxInformation = eServiceOnlineGateway.Instance.GetWitsBoxInformationByWitsBoxId(witsBoxId);
            if (witsBoxInformation == null)
            {
                WitsBoxInformation newWitsBoxInformation = new WitsBoxInformation();
                newWitsBoxInformation.WitsBox = eServiceOnlineGateway.Instance.GetWitsBoxById(witsBoxId);
                newWitsBoxInformation.Notes = notes;
                newWitsBoxInformation.WorkingServicePoint = newWitsBoxInformation.WitsBox.HomeServicePoint;
                newWitsBoxInformation.EquipmentStatus = EquipmentStatus.Yard;
                eServiceOnlineGateway.Instance.CreateWitsBoxInformation(newWitsBoxInformation);
            }
            else
            {
                witsBoxInformation.Notes = notes;
                eServiceOnlineGateway.Instance.UpdateWitsBoxInformation(witsBoxInformation);
            }
        }
    }
}