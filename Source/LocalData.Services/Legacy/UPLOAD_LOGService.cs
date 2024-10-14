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
	public class UPLOAD_LOGService : Service<UPLOAD_LOG>, IUPLOAD_LOGService
	{
		public UPLOAD_LOGService() : base(typeof (IUPLOAD_LOGDao))
		{
		}
		/*add customized code between this region*/
		/*add customized code between this region*/

	}
}
