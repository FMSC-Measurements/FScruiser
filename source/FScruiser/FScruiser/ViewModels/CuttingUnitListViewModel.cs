using FMSC.ORM.EntityModel.Attributes;
using FScruiser.Util;
using FScruiser.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.ViewModels
{
    [EntitySource(SourceName = "Stratum")]
    public class StratumModel
    {
        [Field(Name = "Code")]
        public string StratumCode { get; set; }

        [Field(Name = "Method")]
        public string Method { get; set; }
    }

    [EntitySource(SourceName = "CuttingUnit")]
    public class CuttingUnitModel
    {
        [Field(Name = "CuttingUnit_CN")]
        public long? CuttingUnit_CN { get; set; }

        [Field(Name = "Code")]
        public string CuttingUnitCode
        {
            get;
            set;
        }

        public CuttingUnitModel()
        {
            Strata = new StratumModel[]
            {
                new StratumModel() {StratumCode = "01", Method =  "FIX"},
                new StratumModel() {StratumCode = "02", Method = "STR" }
            };
        }

        public IEnumerable<StratumModel> Strata { get; set; }

        Command<CuttingUnitModel> _showDataEntryCommand;

        public ICommand ShowDataEntryCommand
        {
            get
            {
                return _showDataEntryCommand ?? (_showDataEntryCommand = new Command<CuttingUnitModel>((unit) => ShowDataEntry(unit)));
            }
        }

        public void ShowDataEntry(CuttingUnitModel unit)
        {
            var page = new DataEntryPage(unit.CuttingUnitCode);
            Application.Current.MainPage.Navigation.PushModalAsync(page);
        }
    }

    public class CuttingUnitListViewModel : Xamarin.Forms.BindableObject
    {
        FMSC.ORM.Core.DatastoreRedux Datastore { get; set; }

        public string Test { get; set; } = "Test2";

        public CuttingUnitListViewModel()
        {
            CuttingUnits.AddRange(new CuttingUnitModel[]
            {
                new CuttingUnitModel() {CuttingUnitCode = "01" },
                new CuttingUnitModel() {CuttingUnitCode = "02" }
            });

            //CuttingUnits = datastore.From<CuttingUnitModel>().Query();
        }

        public ObservableCollection<CuttingUnitModel> CuttingUnits { get; set; } = new ObservableCollection<CuttingUnitModel>();
    }
}