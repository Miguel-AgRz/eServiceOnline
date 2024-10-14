/*using eServiceOnline.Gateway;
using Sanjel.Common.BusinessEntities.Reference;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Crew;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment;

namespace eServiceOnline.BusinessProcess
{
    public class NoteProcess
    {
        public static string GetNote(int id, string type)
        {
            if (type.Equals(typeof(SanjelCrew).Name))
            {
                return eServiceOnlineGateway.Instance.GetSanjelCrewNoteBySanjelCrew(id)?.Description;
            }
            if (type.Equals(typeof(ThirdPartyBulkerCrew).Name))
            {
                
            }
            if (type.Equals(typeof(TruckUnit).Name))
            {

            }
            if (type.Equals(typeof(Employee).Name))
            {

            }

            return string.Empty;
        }

        public static int UpdateNote(int id,string note, string type)
        {
            if (type.Equals(typeof(SanjelCrew).Name))
            {
                return UpdateSanjelCrewNote(id, note);
            }
            if (type.Equals(typeof(ThirdPartyBulkerCrew).Name))
            {

            }
            if (type.Equals(typeof(TruckUnit).Name))
            {

            }
            if (type.Equals(typeof(Employee).Name))
            {

            }

            return 1;
        }

        public static int UpdateSanjelCrewNote(int sanjelCrewId, string notes)
        {
           
             SanjelCrewNote crewNote = eServiceOnlineGateway.Instance.GetSanjelCrewNoteBySanjelCrew(sanjelCrewId);
            if (crewNote != null)
            {
                crewNote.Description = notes;
                crewNote.Name = notes;
                return eServiceOnlineGateway.Instance.UpdateSanjelCrewNote(crewNote);

            }
            else
            {
                SanjelCrewNote sanjelCrewNote = new SanjelCrewNote();
                SanjelCrew sanjelCrew = eServiceOnlineGateway.Instance.GetCrewById(sanjelCrewId);
                sanjelCrewNote.SanjelCrew = sanjelCrew;
                sanjelCrewNote.Description = notes;
                sanjelCrewNote.Name = notes;
              return  eServiceOnlineGateway.Instance.CreateSanjelCrewNote(sanjelCrewNote);
            }
           
        }
    }
}*/