using FScruiser.Core.Util;
using FScruiser.Models;
using FScruiser.Services;
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
        private Command<Tree> _editTreeCommand;
        private ICollection<Tree> _trees;
        private Command _addTreeCommand;

        public ICollection<Tree> Trees
        {
            get { return _trees; }
            protected set
            {
                SetValue(ref _trees, value);
            }
        }

        public ICommand AddTreeCommand => _addTreeCommand ?? (_addTreeCommand = new Command(AddTreeAsync));

        public ICommand DeleteTreeCommand => _deleteTreeCommand ?? (_deleteTreeCommand = new Command<Tree>(DeleteTree));

        public ICommand EditTreeCommand => _editTreeCommand ?? (_editTreeCommand = new Command<Tree>(ShowEditTree));

        public ICuttingUnitDataService DataService => ServiceService.CuttingUnitDataService;

        public IDialogService DialogService => ServiceService.DialogService;

        public event EventHandler TreeAdded;

        public TreeListViewModel()
        {
        }

        public async override Task InitAsync()
        {
            var dataService = DataService;
            if (dataService != null)
            {
                await dataService.RefreshDataAsync();
                Trees = DataService.Trees.ToObservableCollection();
            }
        }

        public async void AddTreeAsync()
        {
            var dataService = DataService;

            var stratumCodes = dataService.Strata.Select(x => x.Code).ToArray();

            var stratumCode = await DialogService.AskValue("Select Stratum", stratumCodes);
            if (stratumCode != null)
            {
                var newTree = dataService.CreateTree(stratumCode);
                dataService.AddTree(newTree);
                _trees.Add(newTree);
            }
        }

        public void OnTreeAdded(EventArgs e)
        {
            TreeAdded?.Invoke(this, e);
        }

        public void ShowEditTree(Tree obj)
        {
            var dialogSerive = ServiceService.DialogService;

            dialogSerive.ShowEditTreeAsync(obj, DataService);
        }

        private void DeleteTree(Tree tree)
        {
            throw new NotImplementedException();
        }
    }
}