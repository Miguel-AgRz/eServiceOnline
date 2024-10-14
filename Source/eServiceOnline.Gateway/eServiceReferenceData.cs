using System;

namespace eServiceOnline.Gateway
{
    public static class EServiceReferenceData
    {
        private static bool _isRetrieve = true;

        private static  MicroReferenceData _data;
        private static DateTime UpdateTime;

        public static MicroReferenceData Data
        {
            get
            {
                if (_isRetrieve||UpdateTime.AddMinutes(2)<DateTime.Now)
                {
                    SetReferenceData();
                    _isRetrieve = false;
                    UpdateTime = DateTime.Now;
                }

                return _data;
            }
        }
        public static void SetReferenceData()
        {
            ReferenceDataMicroService service = new ReferenceDataMicroService();
            if (service == null) throw new Exception("IReferenceDataService must be registered");

            try
            {
                // need add version info
                _data = service.GetUpdatedReferenceData(null);
            }
            catch(Exception ex)
            {
                _isRetrieve = false;
            }
        }
    }
}