using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eServiceOnline.BusinessProcess.Interface
{
    public interface IBinProcess
    {
        double LoadBlendToBin(int binId, string binNumber, string blend, double quantity, int productHaulId=0, string userName = null);
        double UnloadBlendFromBin(int binId, string binNumber, string blend, double quantity, int jobNumber=0, string userName = null);
        double UpdateBlendInBin(int binId, string binNumber, string blend, double quantity, string userName);
    }
}
