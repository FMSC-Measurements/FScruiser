using FScruiser.Models;
using FScruiser.Services;
using FScruiser.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    public class TreeListViewModel : ViewModelBase
    {
        private ICommand _deleteTreeCommand;
        private Command<TreeStub> _editTreeCommand;
        private ICollection<TreeStub> _trees;
        private Command _addTreeCommand;

        public ICollection<TreeStub> Trees
        {
            get { return _trees; }
            protected set
            {
                SetValue(ref _trees, value);
            }
        }

        public ICommand AddTreeCommand => _addTreeCommand ?? (_addTreeCommand = new Command(AddTreeAsync));

        public ICommand DeleteTreeCommand => _deleteTreeCommand ?? (_deleteTreeCommand = new Command<TreeStub>(DeleteTree));

        public ICommand EditTreeCommand => _editTreeCommand ?? (_editTreeCommand = new Command<TreeStub>(ShowEditTree));

        //public ICuttingUnitDataService DataService => ServiceService.CuttingUnitDataService;

        public ICuttingUnitDatastore Datastore => ServiceService.Datastore;

        public IDialogService DialogService => ServiceService.DialogService;

        public string UnitCode { get; }

        public event EventHandler TreeAdded;

        public TreeListViewModel(string unitCode)
        {
            UnitCode = unitCode;
        }

        public async Task InitAsync()
        {
            Trees = Datastore.GetTreeStubsByUnitCode(UnitCode).ToObservableCollection();
        }

        public async void AddTreeAsync()
        {
            var datastore = Datastore;

            var stratumCodes = datastore.GetStrataProxiesByUnitCode(UnitCode).Select(x => x.Code).ToArray();

            var stratumCode = await DialogService.AskValueAsync("Select Stratum", stratumCodes);
            if (stratumCode != null)
            {
                var tree_guid = datastore.CreateTree(UnitCode, stratumCode);
                var newTree = datastore.GetTreeStub(tree_guid);
                _trees.Add(newTree);
                OnTreeAdded(null);
            }
        }

        public void OnTreeAdded(EventArgs e)
        {
            TreeAdded?.Invoke(this, e);
        }

        public void ShowEditTree(TreeStub obj)
        {
            var dialogSerive = ServiceService.DialogService;

            dialogSerive.ShowEditTreeAsync(obj.Tree_GUID);
        }

        private void DeleteTree(TreeStub tree)
        {
            if(tree == null) { return;  }

            Datastore.DeleteTree(tree.Tree_GUID);
        }
    }
}