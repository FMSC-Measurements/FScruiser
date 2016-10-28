using Backpack;
using FreshMvvm;
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
    public class CruiseViewModel : FreshMvvm.FreshBasePageModel
    {
        public ICruiseDataService Dataservice { get; set; }

        public Sale Sale { get; set; }

        public IList<CuttingUnitModel> CuttingUnits { get; set; }

        //public IList<StratumModel> Strata { get; set; }

        public CruiseViewModel(ICruiseDataService dataservice)
        {
            Dataservice = dataservice;
        }

        public override void Init(object initData)
        {
            CuttingUnits = Dataservice.GetUnits().ToList();

            base.Init(initData);
        }

        public ICommand ShowDataEntryCommand =>
            new Command<CuttingUnitModel>
            (
                unit => ShowDataEntry(unit)
            );

        public void ShowDataEntry(CuttingUnitModel unit)
        {
            //var masterDetail = new MasterDetailPage();
            //masterDetail.Title = unit.CuttingUnitCode;

            //masterDetail.Master = FreshMvvm.FreshPageModelResolver.ResolvePageModel<StratumListViewModel>(unit);

            //var treePage = FreshMvvm.FreshPageModelResolver.ResolvePageModel<UnitLevelTreeListViewModel>(unit);

            //masterDetail.Detail = new NavigationPage(treePage);

            //this.CurrentPage.Navigation.PushModalAsync(masterDetail);

            var dataStore = FreshIOC.Container.Resolve<DatastoreRedux>();
            FreshIOC.Container.Register<ICuttingUnitDataService>(new CuttingUnitDataService(unit, dataStore));

            CoreMethods.PushPageModel<UnitLevelNavigationViewModel>(unit);
        }
    }
}