using FScruiser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.ViewModels
{
    public class PlotEditViewModel : FreshMvvm.FreshBasePageModel
    {
        public Plot Plot { get; set; }

        public override void Init(object initData)
        {
            Plot = initData as Plot;

            base.Init(initData);
        }
    }
}