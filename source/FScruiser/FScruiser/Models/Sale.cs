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

        [Field("Region")]
        public string Region { get; set; }

        [Field("Forest")]
        public string Forest { get; set; }

        [Field("District")]
        public string District { get; set; }
    }
}