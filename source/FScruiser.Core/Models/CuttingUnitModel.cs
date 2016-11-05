using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FScruiser.Models
{
    [Table("CuttingUnit")]
    public class CuttingUnit
    {
        [Key]
        public long CuttingUnit_CN { get; set; }

        public string Code { get; set; }
    }
}