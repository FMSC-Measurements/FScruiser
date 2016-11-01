using Backpack.EntityModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Models
{
    [EntitySource(SourceName = "CuttingUnitStratum", JoinCommands = "JOIN Stratum USING (Stratum_CN)")]
    public class UnitStratum
    {
        public long CuttingUnit_CN { get; set; }

        public long Stratum_CN { get; set; }

        [Field(SQLExpression = "Stratum.Method", Alias = "CruiseMethod")]
        public string CruiseMethod { get; set; }

        [Field(SQLExpression = "Stratum.Code", Alias = "StratumCode")]
        public string StratumCode { get; set; }

        [Field(SQLExpression = "Stratum.KZ3ppnt", Alias = "KZ3ppnt")]
        public int KZ3ppnt { get; set; }

        public bool IsPlotStratum => CruiseMethods.PLOT_METHODS.Contains(CruiseMethod);
    }
}