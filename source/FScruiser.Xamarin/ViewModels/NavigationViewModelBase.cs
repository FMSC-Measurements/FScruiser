using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    public abstract class NavigationViewModelBase : ViewModelBase
    {
        public INavigation Navigation { get; set; }

        public NavigationViewModelBase(INavigation navigation) : base()
        {
            Navigation = navigation;
        }
    }
}
