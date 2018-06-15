using CruiseDAL.DataObjects;
using FMSC.ORM.EntityModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Models
{
    public class PlotTree : Tree
    {
        [Field(Name = "Plot_CN")]
        public int? Plot_CN { get; set; }

        public PlotDO Plot { get; set; }
    }
}
