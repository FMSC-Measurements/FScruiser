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

    public class TreeListViewModel : FreshMvvm.FreshBasePageModel
    {
        public ICuttingUnitDataService DataService { get; set; }

        public IList<Tree> Trees { get; protected set; }

        //public event EventHandler<Tree> TreeSelected;

        //public ICommand EditTreeCommand =>
        //    new Command<Tree>((x) => SelectTree(x));

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

            Trees = DataService.GetTrees(Stratum, Plot).ToList();
        }
    }
}