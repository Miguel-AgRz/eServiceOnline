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
	public class WITS_SETTINGService : Service<WITS_SETTING>, IWITS_SETTINGService
	{
		public WITS_SETTINGService() : base(typeof (IWITS_SETTINGDao))
		{
		}
		/*add customized code between this region*/
		/*add customized code between this region*/

	}
}
