using Backpack.EntityModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Models
{
    [EntitySource("TreeFieldSetup")]
    public class TreeField
    {
        [Field("Field")]
        public string Field { get; set; }

        [Field("Heading")]
        public string Heading { get; set; }
    }
}