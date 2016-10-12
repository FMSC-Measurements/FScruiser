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
        public string Field { get; set; }
        public string Heading { get; set; }
    }
}