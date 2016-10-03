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
    public class StratumListViewModel : FreshMvvm.FreshBasePageModel
    {
        public IList<UnitStratum> Strata { get; set; }

        DatastoreRedux Datastore { get; set; }

        public StratumListViewModel(DatastoreRedux dataStore)
        {
            Datastore = dataStore;
        }

        public override void Init(object initData)
        {
            var unit = initData as CuttingUnitModel;

            Strata = Datastore.From<UnitStratum>()
                .Where($"CuttingUnit_CN = {unit.CuttingUnit_CN}").Read().ToList();

            base.Init(initData);
        }

        public ICommand ShowTalliesCommand =>
            new Command<UnitStratum>(s => ShowTallies(s));

        public void ShowTallies(UnitStratum stratum)
        {
            CoreMethods.PushPageModel<StratumTalliesViewModel>(stratum);
        }
    }
}