using System.Collections.Generic;
using MetaShare.Common.Core.Entities;
using MetaShare.Common.Core.Services;
using MetaShare.Common.Core.Services.Version;
using Sesi.LocalData.Entities.PostJobReport;
/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Services.Interfaces.PostJobReport
{
	public interface IStorageInfoService : IObjectVersionService<StorageInfo>
	{
	/*add customized code between this region*/
    int CreateStorageInfo(StorageInfo storageInfo);
    int UpdateStorageInfo(StorageInfo storageInfo);
    int DeleteStorageInfo(StorageInfo existingStorageInfo);
    /*add customized code between this region*/
    }
}
