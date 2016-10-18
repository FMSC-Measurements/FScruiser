using Backpack.EntityModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Models
{
    [EntitySource("CountTree", JoinCommands = "JOIN SampleGroup USING (SampleGroup_CN) JOIN Stratum USING (Stratum_CN) JOIN Tally USING (Tally_CN)")]
    public class CountTree
    {
        [PrimaryKeyField]
        public long? CountTree_CN { get; set; }

        public long? CuttingUnit_CN { get; set; }

        public long? SampleGroup_CN { get; set; }

        public int TreeCount { get; set; }

        [Field(SQLExpression = "Tally.Description", Alias = "Description")]
        public string Description { get; set; }

        [Field(SQLExpression = "Stratum.Method", Alias = "CruiseMethod")]
        public string CruiseMethod { get; set; }
    }
}