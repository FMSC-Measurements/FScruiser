using FScruiser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.ViewModels
{
    public class PlotInfoViewModel : FreshMvvm.FreshBasePageModel
    {
        public Plot Plot { get; set; }

        public PlotInfoViewModel()
        {
        }

        public override void Init(object initData)
        {
            base.Init(initData);

            Plot = initData as Plot;
        }
    }
}