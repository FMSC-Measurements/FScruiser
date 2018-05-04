using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.XF.Pages
{

    public class UnitTallyMasterDetailPageMenuItem
    {
        public UnitTallyMasterDetailPageMenuItem()
        {
            TargetType = typeof(UnitTallyMasterDetailPageDetail);
        }
        public int Id { get; set; }
        public string Title { get; set; }

        public Type TargetType { get; set; }
    }
}