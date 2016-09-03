using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace FScruiser.Views
{
    public partial class DataEntryPage : ContentPage
    {
        public DataEntryPage()
        {
            InitializeComponent();
        }

        public DataEntryPage(string unitCode) : this()
        {
        }
    }
}