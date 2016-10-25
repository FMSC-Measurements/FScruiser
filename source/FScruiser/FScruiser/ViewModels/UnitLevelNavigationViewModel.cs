using Backpack;
using FScruiser.Models;
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
        public CuttingUnitModel Unit { get; set; }
        public IList<UnitStratum> Strata { get; set; }

        DatastoreRedux Datastore { get; set; }

        public UnitLevelNavigationViewModel(DatastoreRedux dataStore)
        {
            Datastore = dataStore;
        }

        public override void Init(object initData)
        {
            Unit = initData as CuttingUnitModel;

            Strata = Datastore.From<UnitStratum>()
                .Where($"CuttingUnit_CN = {Unit.CuttingUnit_CN}").Read().ToList();

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