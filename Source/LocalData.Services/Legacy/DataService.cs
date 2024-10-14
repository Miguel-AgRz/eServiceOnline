using System.Collections.Generic;
using MetaShare.Common.Core.Entities;
using Sesi.LocalData.Entities.Legacy;
using MetaShare.Common.Core.Services;
using Sesi.LocalData.Daos.Interfaces.Legacy;
using Sesi.LocalData.Services.Interfaces.Legacy;

/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Services.Legacy
{
	public class DataService : Service<Data>, IDataService
	{
		public DataService() : base(typeof (IDataDao))
		{
		}
		/*add customized code between this region*/
		/*add customized code between this region*/

	}
}
