﻿using FreshMvvm;
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

        public IList<CuttingUnit> CuttingUnits { get; set; }

        //public IList<StratumModel> Strata { get; set; }

        public CruiseViewModel(ICruiseDataService dataservice)
        {
            Dataservice = dataservice;
        }

        public override void Init(object initData)
        {
            base.Init(initData);
            CuttingUnits = Dataservice.GetUnits().ToList();
        }

        public ICommand ShowDataEntryCommand =>
            new Command<CuttingUnit>
            (
                unit => ShowDataEntry(unit)
            );

        public void ShowDataEntry(CuttingUnit unit)
        {
            var cruiseFile = FreshIOC.Container.Resolve<CruiseFile>();
            FreshIOC.Container.Register<ICuttingUnitDataService>(new CuttingUnitDataService(unit, cruiseFile));

            CoreMethods.PushPageModel<UnitLevelNavigationViewModel>(unit);
        }
    }
}