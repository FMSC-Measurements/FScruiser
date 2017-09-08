using FScruiser.XF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FScruiser.XF.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TallyPage : ContentPage
	{
		public TallyPage ()
		{
			InitializeComponent ();

            if(App.InDesignMode)
            {
                BindingContext = new TallyViewModel();
            }
		}
	}
}