using FScruiser.Models;
using FScruiser.Models.Validators;
using FScruiser.Services;
using FScruiser.Util;
using FScruiser.XF.Constants;
using FScruiser.XF.Services;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    public class TreeEditViewModel : ViewModelBase
    {

        private bool _suspendSave;
        private Command _showLogsCommand;
        private IEnumerable<string> _stratumCodes;
        private IEnumerable<string> _sampleGroupCodes;
        private IEnumerable<SubPopulation> _subPopulations;
        private IEnumerable<TreeError> _errorsAndWarnings;
        private IEnumerable<TreeFieldValue> _treeFieldValues;
        private Tree _tree;
        private TreeValidator _treeValidator = new TreeValidator();

        protected ICuttingUnitDatastore Datastore { get; set; }
        protected IDialogService DialogService { get; set; }

        public bool UseSimplifiedTreeFields { get; set; } = false;

        public IEnumerable<TreeError> ErrorsAndWarnings
        {
            get => _errorsAndWarnings;
            set => SetValue(ref _errorsAndWarnings, value);
        }

        public IEnumerable<TreeFieldValue> TreeFieldValues
        {
            get => _treeFieldValues;
            set => SetValue(ref _treeFieldValues, value);
        }

        public Tree Tree
        {
            get { return _tree; }
            protected set
            {
                SetValue(ref _tree, value);
                RaisePropertyChanged(nameof(TreeNumber));
                RaisePropertyChanged(nameof(StratumCode));
                RaisePropertyChanged(nameof(SampleGroupCode));
                RaisePropertyChanged(nameof(SubPopulation));
            }
        }

        #region TreeNumber

        public int TreeNumber
        {
            get
            {
                return Tree?.TreeNumber ?? 0;
            }
            set
            {
                var tree = Tree;
                if (tree == null) { return; }
                var oldValue = tree.TreeNumber;
                if (OnTreeNumberChanging(tree, oldValue, value))
                {
                    Tree.TreeNumber = value;
                    OnTreeNumberChanged(oldValue, value);
                }
            }
        }

        private void OnTreeNumberChanged(int oldValue, int value)
        {
            SaveTree();
        }

        private bool OnTreeNumberChanging(Tree tree, int oldValue, int newValue)
        {
            if(oldValue == newValue) { return false; }



            if (Datastore.IsTreeNumberAvalible(tree.CuttingUnitCode, newValue, tree.PlotNumber))
            {
                return true;
            }
            else
            {
                DialogService.ShowMessageAsync("Tree Number already taken");
                return false;
            }
        }

        #endregion TreeNumber

        #region Stratum

        public IEnumerable<string> StratumCodes
        {
            get => _stratumCodes;
            set => SetValue(ref _stratumCodes, value);
        }

        public string StratumCode
        {
            get { return Tree?.StratumCode; }
            set
            {
                var tree = Tree;
                if (tree == null) { return; }
                var oldValue = Tree.StratumCode;
                if (OnStratumChanging(oldValue, value))
                {
                    tree.StratumCode = value;
                    OnStratumChanged(tree, oldValue, value);
                }
            }
        }

        private void OnStratumChanged(Tree tree, string oldValue, string newValue)
        {
            //Dataservice.LogMessage($"Tree Stratum Tree_GUID:{tree.Tree_GUID} OldStratumCode:{oldValue} NewStratumCode:{newValue}", "I");

            //if (SampleGroups.Any(x => x.Code == newValue) == false)
            //{
            //    Tree.SampleGroupCode = "";
            //}
            try
            {
                _suspendSave = true;
                tree.SampleGroupCode = null;
            }
            finally
            { _suspendSave = false; }

            SaveTree(tree);

            RefreshErrorsAndWarnings(tree);
            RefreshSampleGroups(tree);
            RefreshSubPopulations(tree);
            RefreshTreeFieldValues(tree);
        }

        private bool OnStratumChanging(string oldValue, string newStratum)
        {
            if (oldValue == newStratum) { return false; }
            if (string.IsNullOrWhiteSpace(newStratum)) { return false; }
            var curStratumCode = StratumCode;
            if (string.IsNullOrWhiteSpace(curStratumCode) == false
                && curStratumCode == newStratum)
            { return false; }

            if (curStratumCode != null)
            {
                //if (!DialogService.AskYesNoAsync("You are changing the stratum of a tree" +
                //    ", are you sure you want to do this?", "!").Result)
                //{
                //    return false;//do not change stratum
                //}
                //else
                //{
                return true;
                //}
            }
            else
            {
                return true;
            }
        }

        #endregion Stratum

        #region SampleGroup

        public IEnumerable<string> SampleGroupCodes
        {
            get { return _sampleGroupCodes; }
            set
            {
                SetValue(ref _sampleGroupCodes, value);
            }
        }

        public string SampleGroupCode
        {
            get { return Tree?.SampleGroupCode; }
            set
            {
                var tree = Tree;
                if(tree == null) { return; }
                var oldValue = tree.SampleGroupCode;
                if (OnSampleGroupChanging(oldValue, value))
                {
                    tree.SampleGroupCode = value;
                    OnSampleGroupChanged(tree, oldValue, value);
                }
            }
        }

        private void OnSampleGroupChanged(Tree tree, string oldValue, string newValue)
        {
            //Dataservice.LogMessage($"Tree SampleGroupCanged, Tree_GUID:{Tree.Tree_GUID}, OldSG:{oldValue}, NewSG:{newValue}", "high");

            try
            {
                _suspendSave = true;
                tree.Species = null;
                tree.LiveDead = null;
            }
            finally
            {
                _suspendSave = false;
            }
            SaveTree(tree);

            RefreshErrorsAndWarnings(tree);
            RefreshSubPopulations(tree);
        }

        private bool OnSampleGroupChanging(string oldValue, string newSG)
        {
            if (string.IsNullOrWhiteSpace(newSG)) { return false; }
            if(oldValue == newSG) { return false; }
            if (string.IsNullOrWhiteSpace(oldValue)) { return true; }
            else
            {
                //TODO find a way to conferm sampleGroup canges
                //if (!DialogService.AskYesNoAsync("You are changing the Sample Group of a tree, are you sure you want to do this?"
                //    , "!"
                //    , true).Result)
                //{
                //    return false;
                //}
                //else
                //{
                return true;
                //}
            }
        }

        #endregion SampleGroup

        #region SubPopulation

        public IEnumerable<SubPopulation> SubPopulations
        {
            get => _subPopulations;
            set
            {
                SetValue(ref _subPopulations, value);
                RaisePropertyChanged(nameof(SubPopulation));
            }
        }

        public SubPopulation SubPopulation
        {
            get
            {
                var tree = Tree;
                if(tree == null) { return null; }

                return SubPopulations.OrEmpty()
                .Where(x => x.Species == tree.Species && x.LiveDead == tree.LiveDead)
                .FirstOrDefault();
            }

            set
            {
                var tree = Tree;
                if(tree == null) { return; }

                if (value != null)
                {
                    tree.Species = value.Species;
                    tree.LiveDead = value.LiveDead;
                }
                else
                {
                    tree.Species = null;
                    tree.LiveDead = null;
                }
                OnSubPopulationChanged(tree);
            }
        }

        protected void OnSubPopulationChanged(Tree tree)
        {
            SaveTree(tree);
            RefreshErrorsAndWarnings(tree);
        }

        #endregion SubPopulation

        public ICommand ShowLogsCommand => _showLogsCommand ?? (_showLogsCommand = new Command(ShowLogs));

        public TreeEditViewModel(ICuttingUnitDatastoreProvider datastoreProvider
            , IDialogService dialogService)
        {
            Datastore = datastoreProvider.CuttingUnitDatastore;
            DialogService = dialogService;
        }

        public TreeEditViewModel(ICuttingUnitDatastoreProvider datastoreProvider
            , IDialogService dialogService, INavigationService navigationService) : base(navigationService)
        {
            Datastore = datastoreProvider.CuttingUnitDatastore;
            DialogService = dialogService;
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);

            Tree = null;//unwire tree
        }

        protected override void Refresh(INavigationParameters parameters)
        {
            var treeID = parameters.GetValue<string>(NavParams.TreeID);

            var datastore = Datastore;

            var tree = datastore.GetTree(treeID);

            var unitCode = tree.CuttingUnitCode;
            var stratumCodes = datastore.GetStratumCodesByUnit(unitCode);
            StratumCodes = stratumCodes;

            RefreshSampleGroups(tree);
            RefreshSubPopulations(tree);
            RefreshTreeFieldValues(tree);
            RefreshErrorsAndWarnings(tree);

            Tree = tree;
        }

        private void RefreshSampleGroups(Tree tree)
        {
            var stratum = tree.StratumCode;
            var sampleGroups = Datastore.GetSampleGroupCodes(stratum);
            SampleGroupCodes = sampleGroups;
        }

        private void RefreshSubPopulations(Tree tree)
        {
            var stratumCode = tree.StratumCode;
            var sampleGroupCode = tree.SampleGroupCode;

            var subPopulations = Datastore.GetSubPopulations(stratumCode, sampleGroupCode);
            SubPopulations = subPopulations;
        }

        private void RefreshTreeFieldValues(Tree tree)
        {
            var treeFieldValues = Datastore.GetTreeFieldValues(tree.TreeID);

            foreach (var tfv in treeFieldValues)
            {
                tfv.PropertyChanged += treeFieldValue_PropertyChanged;
            }

            TreeFieldValues = treeFieldValues;
        }

        private void treeFieldValue_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var treeFieldValue = (TreeFieldValue)sender;
            Datastore.UpdateTreeFieldValue(treeFieldValue);
            RefreshErrorsAndWarnings();
        }

        public void RefreshErrorsAndWarnings()
        {
            RefreshErrorsAndWarnings(Tree);
        }

        protected void RefreshErrorsAndWarnings(Tree tree)
        {
            if (tree == null) { return; }

            ErrorsAndWarnings = Datastore.GetTreeErrors(tree.TreeID);
        }

        public void ShowLogs()
        {
            NavigationService.NavigateAsync("Logs", new NavigationParameters($"Tree_Guid={Tree.TreeID}"));
        }

        protected void SaveTree()
        {
            SaveTree(Tree);
        }

        protected void SaveTree(Tree tree)
        {
            if (_suspendSave) { return; }
            if (tree != null)
            {
                Datastore.UpdateTree(tree);
            }
        }

        //public static void SetTreeTDV(Tree tree, TreeDefaultValueDO tdv)
        //{
        //    if (tdv != null)
        //    {
        //        tree.TreeDefaultValue_CN = tdv.TreeDefaultValue_CN;
        //        tree.Species = tdv.Species;

        //        tree.LiveDead = tdv.LiveDead;
        //        tree.Grade = tdv.TreeGrade;
        //        tree.FormClass = tdv.FormClass;
        //        tree.RecoverablePrimary = tdv.Recoverable;
        //        //tree.HiddenPrimary = tdv.HiddenPrimary;//#367
        //    }
        //    else
        //    {
        //        tree.TreeDefaultValue_CN = null;
        //        tree.Species = string.Empty;
        //        tree.LiveDead = string.Empty;
        //        tree.Grade = string.Empty;
        //        tree.FormClass = 0;
        //        tree.RecoverablePrimary = 0;
        //        //this.HiddenPrimary = 0;
        //    }
        //}
    }
}