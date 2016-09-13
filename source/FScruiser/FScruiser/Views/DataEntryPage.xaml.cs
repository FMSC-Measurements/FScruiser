using FScruiser.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace FScruiser.Views
{
    public partial class DataEntryPage : MasterDetailPage
    {
        public DataEntryPage()
        {
            InitializeComponent();
        }

        public CuttingUnitModel Unit { get; protected set; }

        public DataEntryPage(string unitCode) : this()
        {
            Unit = new CuttingUnitModel() { CuttingUnitCode = unitCode };
        }
    }
}