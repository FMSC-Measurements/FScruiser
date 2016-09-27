using FMSC.ORM.EntityModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Models
{
    [EntitySource("CountTree", JoinCommands = "JOIN SampleGroup USING (SampleGroup_CN) JOIN Tally USING (Tally_CN)")]
    public class CountTree
    {
        [Field("CuttingUnit_CN")]
        public long? CuttingUnit_CN { get; set; }

        [Field("SampleGroup_CN")]
        public long? SampleGroup_CN { get; set; }

        [Field("TreeCount")]
        public int TreeCount { get; set; }

        [Field(SQLExpression = "Tally.Description", Alias = "Description")]
        public string Description { get; set; }
    }
}