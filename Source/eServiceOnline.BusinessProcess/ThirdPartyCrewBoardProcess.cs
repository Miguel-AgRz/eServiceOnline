using System;
using System.Collections.Generic;
using System.Linq;
using eServiceOnline.Gateway;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Crew;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ResourceSchedule;

namespace eServiceOnline.BusinessProcess
{
    public class ThirdPartyCrewBoardProcess
    {
        public static List<ThirdPartyBulkerCrew> GetThirdPartyBulkerCrewsByPage(int pageSize, int pageNumber, out int count)
        {
            List<ThirdPartyBulkerCrew> thirdPartyBulkerCrews = eServiceOnlineGateway.Instance.GetThirdPartyBulkerCrews();
            count = thirdPartyBulkerCrews.Count;
            thirdPartyBulkerCrews = thirdPartyBulkerCrews.Skip(pageSize*(pageNumber - 1)).Take(pageSize).ToList();

            return thirdPartyBulkerCrews;
        }

        public static int CreateThirdPartyBulkerCrew(ThirdPartyBulkerCrew thirdPartyBulkerCrew)
        {
            BuildContractorCompany(thirdPartyBulkerCrew);

            return eServiceOnlineGateway.Instance.CreateThirdPartyBulkerCrew(thirdPartyBulkerCrew);
        }

        private static void BuildContractorCompany(ThirdPartyBulkerCrew thirdPartyBulkerCrew)
        {
            if (thirdPartyBulkerCrew.ContractorCompany != null)
            {
                thirdPartyBulkerCrew.Description = thirdPartyBulkerCrew.ContractorCompany.Name + " | " + thirdPartyBulkerCrew.ThirdPartyUnitNumber;
                thirdPartyBulkerCrew.Name = thirdPartyBulkerCrew.ThirdPartyUnitNumber;
                thirdPartyBulkerCrew.Type = eServiceOnlineGateway.Instance.GetCrewTypeById(3);
            }
        }

        public static List<ThirdPartyBulkerCrew> GetAllThirdPartyBulkerCrews()
        {
            return eServiceOnlineGateway.Instance.GetThirdPartyBulkerCrews();
        }

        public static int UpdateThirdPartyBulkerCrew(ThirdPartyBulkerCrew thirdPartyBulkerCrew)
        {
            BuildContractorCompany(thirdPartyBulkerCrew);
            return eServiceOnlineGateway.Instance.UpdateThirdPartyBulkerCrew(thirdPartyBulkerCrew);
        }

        public static List<ThirdPartyBulkerCrewSchedule> GetThirdPartyCrewScheduleByThirdPartyCrewId(int thirdPartyCrewId)
        {
            List<ThirdPartyBulkerCrewSchedule> thirdPartyBulkerCrewSchedules = eServiceOnlineGateway.Instance.GetThirdPartyCrewScheduleByThirdPartyCrewId(thirdPartyCrewId);
            thirdPartyBulkerCrewSchedules = thirdPartyBulkerCrewSchedules.FindAll(s => s.EndTime > DateTime.Now).ToList();
            if (thirdPartyBulkerCrewSchedules.Count != 0)
            {

                foreach (var thirdPartyBulkerCrewSchedule in thirdPartyBulkerCrewSchedules)
                {
                    thirdPartyBulkerCrewSchedule.RigJobThirdPartyBulkerCrewSection = eServiceOnlineGateway.Instance.GetRigJobThirdPartyBulkerCrewSectionById(thirdPartyBulkerCrewSchedule?.RigJobThirdPartyBulkerCrewSection?.Id ?? 0);
                    if (thirdPartyBulkerCrewSchedule.RigJobThirdPartyBulkerCrewSection != null)
                    {
                        thirdPartyBulkerCrewSchedule.RigJobThirdPartyBulkerCrewSection.RigJob = eServiceOnlineGateway.Instance.GetRigJobById(thirdPartyBulkerCrewSchedule.RigJobThirdPartyBulkerCrewSection?.RigJob?.Id ?? 0);
                    }
                }
            }
            return thirdPartyBulkerCrewSchedules.OrderBy(p => p.EndTime).ToList();
        }
    }
}