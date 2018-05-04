using CruiseDAL.DataObjects;
using CruiseDAL.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Models
{
    public class Stratum : StratumDO
    {
        public bool Is3P => CruiseMethods.THREE_P_METHODS.Contains(Method);
    }
}
