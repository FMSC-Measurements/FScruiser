using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FScruiser.Models;
using FScruiser.Services;
using FScruiser.XF.Constants;
using FScruiser.XF.Services;
using Prism.Navigation;
using Xamarin.Forms;
using static FScruiser.Constants;

namespace FScruiser.XF.ViewModels
{
    public class TreeCountEditViewModel : ViewModelBase
    {
        private string _unitCode;
        private int _treeCountDelta;
        private string _editReason;
        private string _remarks;
        private TallyPopulation _tallyPopulation;
        private ICommand _saveTreeCountEditCommand;

        public TreeCountEditViewModel(INavigationService navigationService, ICuttingUnitDatastoreProvider datastoreProvider, IDialogService dialogService) : base(navigationService)
        {
            Datastore = datastoreProvider.CuttingUnitDatastore;
            DialogService = dialogService;
        }

        protected ICuttingUnitDatastore Datastore { get; }
        protected IDialogService DialogService { get; }

        public ICommand SaveTreeCountEditCommand => _saveTreeCountEditCommand ?? (_saveTreeCountEditCommand = new Command(SaveEdit));

        public string UnitCode
        {
            get { return _unitCode; }
            set { SetValue(ref _unitCode, value); }
        }

        public TallyPopulation TallyPopulation
        {
            get => _tallyPopulation;

            set
            {
                SetValue(ref _tallyPopulation, value);
                RaisePropertyChanged(nameof(StratumCode));
                RaisePropertyChanged(nameof(TallyPopulationDescription));
                RaisePropertyChanged(nameof(TreeCount));
            }
        }

        public string StratumCode => TallyPopulation?.StratumCode;

        public string TallyPopulationDescription => TallyPopulation?.TallyDescription;

        public int? TreeCount => TallyPopulation?.TreeCount;

        public int TreeCountDelta
        {
            get { return _treeCountDelta; }
            set
            {
                SetValue(ref _treeCountDelta, value);
                RaisePropertyChanged(nameof(AdjustedTreeCount));
            }
        }

        public int AdjustedTreeCount
        {
            get => (TreeCount ?? 0) + TreeCountDelta;
        }

        public string[] EditReasonOptions => new string[] { "Leftover Trees", "Click Counts", "Paper Recorded Counts", "Other" };

        public string EditReason
        {
            get => _editReason;
            set => SetValue(ref _editReason, value);
        }

        public string Remarks
        {
            get => _remarks;
            set => SetValue(ref _remarks, value);
        }
        

        protected override void Refresh(INavigationParameters parameters)
        {
            var unit = parameters.GetValue<string>(NavParams.UNIT);
            var stratum = parameters.GetValue<string>(NavParams.STRATUM);
            var sampleGroup = parameters.GetValue<string>(NavParams.SAMPLE_GROUP);
            var species = parameters.GetValue<string>(NavParams.SPECIES);
            var liveDead = parameters.GetValue<string>(NavParams.LIVE_DEAD);

            var datastore = Datastore;

            var tallyPopulation = datastore.GetTallyPopulation(unit, stratum, sampleGroup, species, liveDead);

            TallyPopulation = tallyPopulation;
            UnitCode = unit;

        }

        public void SaveEdit()
        {
            var reason = EditReason;
            var treeCountDelta = TreeCountDelta;
            var cruiser = DialogService.AskCruiserAsync();
            if(cruiser == null) { return; }

            var tallyLedger = new TallyLedger(UnitCode, TallyPopulation);
            tallyLedger.TreeCount = treeCountDelta;
            tallyLedger.Reason = reason;
            tallyLedger.Remarks = Remarks;
            tallyLedger.EntryType = TallyLedger_EntryType.TreeCountEdit;

            Datastore.InsertTallyLedger(tallyLedger);

            base.NavigationService.GoBackAsync();
        }

    }
}
