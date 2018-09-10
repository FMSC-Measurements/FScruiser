using FScruiser.Models;
using FScruiser.Services;
using FScruiser.Util;
using FScruiser.XF.Services;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    public class TreeListViewModel : ViewModelBase, INavigatedAware
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

        public string UnitCode { get; set; }

        public ICuttingUnitDatastore Datastore { get; set; }

        public IDialogService DialogService { get; set; }

        public event EventHandler TreeAdded;

        public TreeListViewModel(IDialogService dialogService
            , INavigationService navigationService
            , ICuttingUnitDatastoreProvider datastoreProvider) : base(navigationService)
        {
            DialogService = dialogService;
            Datastore = datastoreProvider.CuttingUnitDatastore;
        }

        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            if(UnitCode == null) //don't reload param if navigating backwards
            {
                UnitCode = parameters.GetValue<string>("UnitCode");
            }

            Trees = Datastore.GetTreeStubsByUnitCode(UnitCode).ToObservableCollection();

            base.OnNavigatedTo(parameters);
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
            try
            {
                NavigationService.NavigateAsync("Tree", new NavigationParameters() { { "Tree_Guid", obj.Tree_GUID }, }, useModalNavigation: true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERROR::::" + ex);
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "nav_path", "/Main/Navigation/CuttingUnits" } });
            }
        }

        private void DeleteTree(TreeStub tree)
        {
            if (tree == null) { return; }

            Datastore.DeleteTree(tree.Tree_GUID);
        }
    }
}