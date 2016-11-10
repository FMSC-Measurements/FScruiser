using FScruiser.Models;
using FScruiser.Services;
using System;
using System.Collections.Generic;
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

    public class UnitLevelTreeListViewModel : FreshMvvm.FreshBasePageModel
    {
        public ICuttingUnitDataService DataService { get; set; }

        public IList<Tree> Trees { get; protected set; }

        public ICommand EditTreeCommand =>
            new Command<Tree>((x) => EditTree(x));

        public UnitLevelTreeListViewModel(ICuttingUnitDataService dataService)
        {
            DataService = dataService;
        }

        //public Stratum Stratum => Filter?.Stratum;
        //public Plot Plot => Filter.Plot;

        public TreeListFilter Filter { get; set; }

        public override void Init(object initData)
        {
            base.Init(initData);

            Filter = initData as TreeListFilter;

            Trees = DataService.GetTrees(Filter?.Stratum, Filter?.Plot).ToList();
        }

        protected void EditTree(Tree tree)
        {
            CoreMethods.PushPageModel<TreeEditViewModel>(tree);
        }
    }
}