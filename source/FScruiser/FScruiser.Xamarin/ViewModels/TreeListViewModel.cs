using FScruiser.Models;
using FScruiser.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.ViewModels
{
    public class TreeListFilter
    {
        public Stratum Stratum { get; set; }
        public Plot Plot { get; set; }
    }

    public class TreeListViewModel : FreshMvvm.FreshBasePageModel
    {
        private IList<Tree> _trees;

        public ICuttingUnitDataService DataService { get; set; }

        public IList<Tree> Trees
        {
            get { return _trees; }
            protected set
            {
                _trees = value;
                RaisePropertyChanged();
            }
        }

        public event EventHandler<Tree> TreeSelected;

        public ICommand EditTreeCommand =>
            new Command<Tree>((tree) => SelectTree(tree));

        public ICommand DeleteTreeCommand =>
            new Command<Tree>((tree) => DeleteTree(tree));

        public void DeleteTree(Tree tree)
        {
            throw new NotImplementedException();
        }

        public void SelectTree(Tree tree)
        {
            TreeSelected?.Invoke(this, tree);
        }

        public TreeListViewModel(ICuttingUnitDataService dataService)
        {
            DataService = dataService;
        }

        public Stratum Stratum { get; set; }
        public Plot Plot { get; set; }

        public override void Init(object initData)
        {
            base.Init(initData);
            var filter = initData as TreeListFilter;
            if (filter != null)
            {
                Stratum = filter.Stratum;
                Plot = filter.Plot;
            }

            LoadTrees();
        }

        private async void LoadTrees()
        {
            var trees = await Task.Run(() =>
            {
                return DataService.GetTrees(Stratum, Plot);
            });
            Trees = trees.ToList();
        }
    }
}