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
	public class JOB_TAGService : Service<JOB_TAG>, IJOB_TAGService
	{
		public JOB_TAGService() : base(typeof (IJOB_TAGDao))
		{
		}
		/*add customized code between this region*/
		/*add customized code between this region*/

	}
}
