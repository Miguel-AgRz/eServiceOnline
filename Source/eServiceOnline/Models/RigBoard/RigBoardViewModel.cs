using System.Collections.Generic;
using eServiceOnline.Models.Commons;

namespace eServiceOnline.Models.RigBoard
{
    public class RigBoardViewModel
    {
        public List<RigJobViewModel> RigJobViewModels { get; set; }

        public RetrievalCondition RetrievalCondition { get; set; }
    }
}