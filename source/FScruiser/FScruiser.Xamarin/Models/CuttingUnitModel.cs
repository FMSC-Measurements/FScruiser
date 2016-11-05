using FScruiser.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.Models
{
    [Table("CuttingUnit")]
    public class CuttingUnitModel
    {
        [Key]
        public long CuttingUnit_CN { get; set; }

        public string Code { get; set; }
    }
}