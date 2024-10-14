using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eServiceOnline.BusinessProcess.Interface
{
    public interface IRigJobProcess
    {
        bool CompleteRigJob(int rigJobId);
        bool CompleteRigJobByCallSheetNumber(int callSheetNumber);
    }
}
