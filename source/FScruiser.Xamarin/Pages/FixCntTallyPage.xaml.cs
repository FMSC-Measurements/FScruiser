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
    public partial class FixCntTallyPage : ContentPage
    {
        public FixCntTallyPage()
        {
            InitializeComponent();

            _enableUntallySwitch.Toggled += _enableUntallySwitch_Toggled;
        }

        private void _enableUntallySwitch_Toggled(object sender, ToggledEventArgs e)
        {
            System.Diagnostics.Debug.Write("Toggle");
        }
    }
}