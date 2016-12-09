using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Models
{
    [Table("Sale")]
    public class Sale
    {
        [Key]
        public long RowID { get; set; }

        public string SaleName { get; set; }

        public string SaleNumber { get; set; }

        public string Region { get; set; }

        public string Forest { get; set; }

        public string District { get; set; }
    }
}