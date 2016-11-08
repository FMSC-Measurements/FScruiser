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
    public class UnitLevelNavigationViewModel : FreshMvvm.FreshBasePageModel
    {
        public CuttingUnit Unit { get; set; }
        public IList<UnitStratum> Strata { get; set; }

        ICuttingUnitDataService DataService { get; set; }

        public UnitLevelNavigationViewModel(ICuttingUnitDataService dataService)
        {
            DataService = dataService;
        }

        public override void Init(object initData)
        {
            Unit = initData as CuttingUnit;

            Strata = DataService.GetStrata().ToList();

            base.Init(initData);
        }

        public ICommand ShowTalliesCommand =>
            new Command<UnitStratum>(s => ShowTallies(s));

        public ICommand ShowTreesCommand =>
            new Command(() => ShowTrees());

        private void ShowTrees()
        {
            CoreMethods.PushPageModel<UnitLevelTreeListViewModel>(Unit);
        }

        public void ShowTallies(UnitStratum stratum)
        {
            CoreMethods.PushPageModel<StratumTalliesViewModel>(stratum);
        }
    }
}