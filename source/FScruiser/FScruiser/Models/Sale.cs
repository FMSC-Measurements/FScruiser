using Backpack.EntityModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Models
{
    [EntitySource("Sale")]
    public class Sale
    {
        [Field("Name")]
        public string SaleName { get; set; }

        [Field("Number")]
        public string SaleNumber { get; set; }

        public string Region { get; set; }

        public string Forest { get; set; }

        public string District { get; set; }
    }
}