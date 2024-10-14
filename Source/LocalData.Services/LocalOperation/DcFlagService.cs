using System.Collections.Generic;
using MetaShare.Common.Core.Entities;
using Sesi.LocalData.Entities.LocalOperation;
using MetaShare.Common.Core.Services;
using Sesi.LocalData.Daos.Interfaces.LocalOperation;
using Sesi.LocalData.Services.Interfaces.LocalOperation;

/*add customized code between this region*/
/*add customized code between this region*/

namespace Sesi.LocalData.Services.LocalOperation
{
	public class DcFlagService : Service<DcFlag>, IDcFlagService
	{
		public DcFlagService() : base(typeof (IDcFlagDao))
		{
		}
		/*add customized code between this region*/
		/*add customized code between this region*/

	}
}
