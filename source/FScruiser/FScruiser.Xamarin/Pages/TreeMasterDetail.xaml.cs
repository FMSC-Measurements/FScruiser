using FScruiser.Models;
using FScruiser.Services;
using FScruiser.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace FScruiser.Pages
{
    public partial class TreeMasterDetail : MasterDetailPage
    {
        public TreeMasterDetail(ICuttingUnitDataService dataService)
        {
            TreeEditController = new TreeEditViewModel(dataService);
            TreeList = new TreeListViewModel(dataService);

            InitializeComponent();

            TreeListView.BindingContext = TreeList;
            TreeEditView.BindingContext = TreeEditController;
        }

        public ICuttingUnitDataService DataService { get; set; }
        public TreeEditViewModel TreeEditController { get; set; }
        public TreeListViewModel TreeList { get; set; }

        public Stratum Stratum
        {
            get { return TreeList.Stratum; }
            set { TreeList.Stratum = value; }
        }

        public Plot Plot
        {
            get { return TreeList.Plot; }
            set { TreeList.Plot = value; }
        }

        private void TreeListPage_TreeSelected(object sender, Models.Tree e)
        {
            TreeEditController.Tree = e;
        }
    }
}