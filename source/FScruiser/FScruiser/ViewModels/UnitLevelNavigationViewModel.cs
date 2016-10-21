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
        public IList<StratumModel> Strata { get; set; }

        DatastoreRedux Datastore { get; set; }

        public UnitLevelNavigationViewModel(DatastoreRedux dataStore)
        {
            Datastore = dataStore;
        }

        public override void Init(object initData)
        {
            Unit = initData as CuttingUnitModel;

            Strata = Datastore.From<StratumModel>()
                .Join("CuttingUnitStratum", "Using (Stratum_CN)")
                .Where($"CuttingUnit_CN = {Unit.CuttingUnit_CN}").Read().ToList();

            base.Init(initData);
        }

        public ICommand ShowTalliesCommand =>
            new Command<StratumModel>(s => ShowTallies(s));

        public ICommand ShowTreesCommand =>
            new Command(() => ShowTrees());

        private void ShowTrees()
        {
            CoreMethods.PushPageModel<UnitLevelTreeListViewModel>(Unit);
        }

        public void ShowTallies(StratumModel stratum)
        {
            var unitStratum = new UnitStratum
            {
                CuttingUnit_CN = Unit.CuttingUnit_CN,
                Stratum_CN = stratum.Stratum_CN
            };

            CoreMethods.PushPageModel<StratumTalliesViewModel>(unitStratum);
        }
    }
}