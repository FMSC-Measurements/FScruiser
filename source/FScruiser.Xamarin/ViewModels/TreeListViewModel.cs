using FScruiser.Models;
using FScruiser.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    public class TreeListViewModel : ViewModelBase
    {
        private ICommand _deleteTreeCommand;
        private Command<Tree> _editTreeCommand;
        private IEnumerable<Tree> _trees;

        public IEnumerable<Tree> Trees
        {
            get { return _trees; }
            protected set
            {
                SetValue(ref _trees, value);
            }
        }

        public ICommand DeleteTreeCommand => _deleteTreeCommand ?? (_deleteTreeCommand = new Command<Tree>(DeleteTree));

        public ICommand EditTreeCommand => _editTreeCommand ?? (_editTreeCommand = new Command<Tree>(ShowEditTree));

        public ICuttingUnitDataService DataService => ServiceService.CuttingUnitDataService;

        public TreeListViewModel()
        {
        }

        public async override Task InitAsync()
        {
            var dataService = DataService;
            if (dataService != null)
            {
                await dataService.RefreshDataAsync();
                Trees = DataService.Trees;
            }
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