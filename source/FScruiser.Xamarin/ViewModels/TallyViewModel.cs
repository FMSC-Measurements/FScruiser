using FScruiser.Models;
using FScruiser.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace FScruiser.XF.ViewModels
{
    public class TallyViewModel : FreshMvvm.FreshBasePageModel
    {
        ICuttingUnitDataService _dataService;
        public ICuttingUnitDataService DataService
        {
            get
            {
                return _dataService;
            }
            set
            {
                _dataService = value;
                OnDataServiceChanged();
            }
        }

        private void OnDataServiceChanged()
        {
            
        }

        public CuttingUnit CuttingUnit { get; set; }

        public string CurrentPlotNumber => CurrentPlot?.PlotNumber.ToString() ?? "No Plot";

        public Plot CurrentPlot { get; set; }

        public ICollection<Plot> Plots { get; set; }

        public ICollection<TallyPopulation> Tallies { get; set; }

        public ObservableCollection<TallyFeedEntry> TallyFeed { get; set; } = new ObservableCollection<TallyFeedEntry>();

        public TallyViewModel()
        {
            if (!App.InDesignMode) throw new InvalidOperationException("default constructor is only for design time support");
            else
            {
                var tallies = new List<TallyPopulation>();

                tallies.Add(new TallyPopulation() { });
            }
        }

        public TallyViewModel(ICuttingUnitDataService dataService)
        {
            DataService = dataService;
        }

        public override void Init(object initData)
        {
            base.Init(initData);

            CuttingUnit = initData as CuttingUnit;

            Tallies = DataService.GetAllTallyPopulations().ToArray();

        }
    }
}
