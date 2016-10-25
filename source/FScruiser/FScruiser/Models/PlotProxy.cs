using Backpack.EntityModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Models
{
    [EntitySource("Plot")]
    public class PlotProxy
    {
        public int PlotNumber { get; set; }

        public bool IsEmpty { get; set; }

        public long Plot_CN { get; set; }

        public Guid Plot_GUID { get; set; }

        public override string ToString()
        {
            var isEmptyExpr = (IsEmpty) ? ":Empty" : string.Empty;
            return $"{PlotNumber} {isEmptyExpr}";
        }
    }
}