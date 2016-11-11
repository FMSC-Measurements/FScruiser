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
    public class TreeListViewModel : FreshMvvm.FreshBasePageModel
    {
        public ICuttingUnitDataService DataService { get; set; }

        public IList<Tree> Trees { get; protected set; }

        public event EventHandler<Tree> TreeSelescted;

        public ICommand EditTreeCommand =>
            new Command<Tree>((x) => SelectTree(x));

        public TreeListViewModel(ICuttingUnitDataService dataService)
        {
            DataService = dataService;
        }

        public Stratum Stratum { get; set; }
        public Plot Plot { get; set; }

        public override void Init(object initData)
        {
            base.Init(initData);

            Trees = DataService.GetTrees(Stratum, Plot).ToList();
        }

        protected void SelectTree(Tree tree)
        {
            TreeSelected?.Invoke(this, tree);
        }
    }
}