using FScruiser.Models;
using FScruiser.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.ViewModels
{
    public class TreeMasterDetailViewModel : FreshMvvm.FreshBasePageModel
    {
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

        public ICuttingUnitDataService Dataservice { get; set; }
        public TreeListViewModel TreeList { get; set; }
        public TreeEditViewModel TreeEdit { get; set; }

        public TreeMasterDetailViewModel(ICuttingUnitDataService dataService)
        {
            Dataservice = dataService;

            TreeList = new TreeListViewModel(Dataservice);
            TreeList.TreeSelected += TreeList_TreeSelected;
            TreeList.PropertyChanged += TreeList_PropertyChanged;
            TreeEdit = new TreeEditViewModel(Dataservice);
        }

        private void TreeList_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TreeListViewModel.Trees))
            {
                TreeList.SelectTree(TreeList?.Trees.FirstOrDefault());
            }
        }

        public override void Init(object initData)
        {
            base.Init(initData);

            TreeList.Init(initData);
        }

        private void TreeList_TreeSelected(object sender, Tree e)
        {
            if (e == null) { return; }
            TreeEdit.Tree = e;
        }
    }
}