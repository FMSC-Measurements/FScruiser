using FScruiser.Models;
using FScruiser.Services;
using System;
using System.Collections.Generic;
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

        public ICuttingUnitDataService DataService => ServiceService.CuttingUnitDataSercie;
        public ServiceService ServiceService { get; private set; }

        public TreeListViewModel(ServiceService serviceService) : base(null)
        {
            ServiceService = serviceService;
        }

        public override void Init()
        {
            var dataService = DataService;
            if (dataService != null)
            {
                Trees = DataService.Trees;
            }
        }

        private void ShowEditTree(Tree obj)
        {
            
        }

        private void DeleteTree(Tree tree)
        {
            throw new NotImplementedException();
        }
    }
}