using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FScruiser.XF.Views
{
    public class CustomListView : ListView
    {
        public event EventHandler Scroll;

        public void RaiseScroll()
        {
            Scroll?.Invoke(this, null);
        }
    }
}
