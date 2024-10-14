using System.Collections.Generic;
using System.Linq;
using MetaShare.Common.Core.Entities;
using Sesi.LocalData.Entities.PostJobReport;
using MetaShare.Common.Core.Services;
using MetaShare.Common.Core.Services.Version;
using Sesi.LocalData.Daos.Interfaces.PostJobReport;
using Sesi.LocalData.Services.Interfaces.PostJobReport;

/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Services.PostJobReport
{
	public class StorageInfoService : ObjectVersionService<StorageInfo>, IStorageInfoService
	{
		public StorageInfoService() : base(typeof (IStorageInfoDao))
		{
		}
		/*add customized code between this region*/
        public int CreateStorageInfo(StorageInfo storageInfo)
        {
            storageInfo.Id = 0;
            storageInfo.SystemId = 0;

            var storageInfos = this.SelectBy(storageInfo, new List<string> {"JobUniqueId","Name"});
            if (storageInfos != null)
            {
                foreach (var info in storageInfos)
                {
                    this.Delete(info);
                }
            }

            return this.Insert(storageInfo);
        }

        public int UpdateStorageInfo(StorageInfo storageInfo)
        {
            storageInfo.Id = 0;
            storageInfo.SystemId = 0;

            var storageInfos = this.SelectBy(storageInfo, new List<string> {"JobUniqueId", "Name"});
            if (storageInfos == null || storageInfos.Count == 0)
            {
                return this.Insert(storageInfo);
            }
            else if (storageInfos.Count > 1)
            {
                foreach (var info in storageInfos)
                {
                    this.Delete(info);
                }
                return this.Insert(storageInfo);
            }
            else
            {
                var existingStorageInfo = storageInfos.FirstOrDefault();
                existingStorageInfo.JobNumber = storageInfo.JobNumber;
                existingStorageInfo.Remains = storageInfo.Remains;
                existingStorageInfo.ScaleReading = storageInfo.ScaleReading;
                existingStorageInfo.PumpedWithAdds = storageInfo.PumpedWithAdds;
                existingStorageInfo.IsReadyForNext = storageInfo.IsReadyForNext;
                existingStorageInfo.PumpedWoAdds = storageInfo.PumpedWoAdds;
                existingStorageInfo.InitialTonnage = storageInfo.InitialTonnage;
                existingStorageInfo.StorageType = storageInfo.StorageType;
                existingStorageInfo.BlendName = storageInfo.BlendName;

                return this.Update(existingStorageInfo);
            }
        }

        public int DeleteStorageInfo(StorageInfo storageInfo)
        {
            var storageInfos = this.SelectBy(storageInfo, new List<string> {"JobUniqueId", "Name"});
            if (storageInfos != null && storageInfos.Count > 0)
            {
                return this.Delete(storageInfos.FirstOrDefault());
            }

            return 0;
        }

        /*add customized code between this region*/

    }
}
