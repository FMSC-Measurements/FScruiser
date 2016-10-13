using Backpack.EntityModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Models
{
    [EntitySource(SourceName = "Stratum")]
    public class StratumModel
    {
        [Field(Name = "Code")]
        public string StratumCode { get; set; }

        public string Method { get; set; }
    }
}