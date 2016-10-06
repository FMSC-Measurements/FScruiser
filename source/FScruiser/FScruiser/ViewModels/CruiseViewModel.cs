﻿using Backpack;
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
    public class CruiseViewModel : FreshMvvm.FreshBasePageModel
    {
        public DatastoreRedux Datastore { get; set; }

        public Sale Sale { get; set; }

        public IList<CuttingUnitModel> CuttingUnits { get; set; }

        //public IList<StratumModel> Strata { get; set; }

        public CruiseViewModel(DatastoreRedux dataStore)
        {
            Datastore = dataStore;
        }

        public override void Init(object initData)
        {
            CuttingUnits = Datastore.From<CuttingUnitModel>().Read().ToList();

            base.Init(initData);
        }

        public ICommand ShowDataEntryCommand =>
            new Command<CuttingUnitModel>
            (
                unit => ShowDataEntry(unit)
            );

        public void ShowDataEntry(CuttingUnitModel unit)
        {
            var masterDetail = new MasterDetailPage();
            masterDetail.Title = unit.CuttingUnitCode;

            masterDetail.Master = FreshMvvm.FreshPageModelResolver.ResolvePageModel<StratumListViewModel>(unit);

            var treePage = FreshMvvm.FreshPageModelResolver.ResolvePageModel<UnitLevelTreeListViewModel>(unit);

            masterDetail.Detail = new NavigationPage(treePage);

            this.CurrentPage.Navigation.PushModalAsync(masterDetail);

            //CoreMethods.PushPageModel<DataEntryViewModel>(unit);
        }
    }
}