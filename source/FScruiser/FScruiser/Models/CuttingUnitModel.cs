using Backpack.EntityModel.Attributes;
using FScruiser.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.Models
{
    [EntitySource(SourceName = "CuttingUnit")]
    public class CuttingUnitModel
    {
        public long CuttingUnit_CN { get; set; }

        [Field(Name = "Code")]
        public string CuttingUnitCode
        {
            get;
            set;
        }
    }
}