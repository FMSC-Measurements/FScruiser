using CruiseDAL.DataObjects;
using FScruiser.Models;
using FScruiser.Models.Validators;
using FScruiser.Services;
using FScruiser.Util;
using FScruiser.Validation;
using FScruiser.XF.Services;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FScruiser.XF.ViewModels
{
    public class TreeEditViewModel : ViewModelBase
    {
        private static readonly TreeDefaultProxy NULL_TREE_DEFAULT = new TreeDefaultProxy() { Species = "", LiveDead = "" };

        protected ICuttingUnitDatastore Datastore { get; set; }
        protected IDialogService DialogService { get; set; }

        private TreeValidator _treeValidator;

        public bool UseSimplifiedTreeFields { get; set; } = false;

        private IEnumerable<TreeDefaultProxy> _treeDefaults;

        private bool _suspendSave;

        public event EventHandler<IEnumerable<TreeFieldSetupDO>> TreeFieldsChanged;

        public event EventHandler ErrorsAndWarningsChanged;

        private ValidationResult _lastValidationResult;
        public IEnumerable<ValidationError> ErrorsAndWarnings => _lastValidationResult?.Errors.OrEmpty();

        public IEnumerable<TreeFieldSetupDO> TreeFields
        {
            get
            {
                if (UseSimplifiedTreeFields)
                {
                    return _treeFieldsExtended.Where(x => FScruiser.Constants.LESS_IMPORTANT_TREE_FIELDS.Contains(x.Field) == false);
                }
                else
                {
                    return _treeFieldsExtended;
                }
            }
        }

        private IEnumerable<TreeFieldSetupDO> _treeFieldsExtended;

        public IEnumerable<TreeFieldSetupDO> TreeFieldsExtended
        {
            get { return _treeFieldsExtended; }
            set
            {
                SetValue(ref _treeFieldsExtended, value);
                TreeFieldsChanged?.Invoke(this, value);
                RaisePropertyChanged(nameof(TreeFields));
            }
        }

        #region Stratum

        private IEnumerable<StratumProxy> _strata;

        public IEnumerable<StratumProxy> Strata
        {
            get { return _strata; }
            set
            {
                SetValue(ref _strata, value);
                RaisePropertyChanged(nameof(StrataCodes));
            }
        }

        public IEnumerable<string> StrataCodes => Strata.OrEmpty().Select(x => x.Code).ToArray();

        public string StratumCode
        {
            get { return Tree?.StratumCode; }
            set
            {
                if (OnStratumChanging(value))
                {
                    var oldValue = Tree.StratumCode;
                    Tree.StratumCode = value;
                    OnStratumChanged(oldValue, value);
                }
            }
        }

        private void OnStratumChanged(string oldValue, string newValue)
        {
            var tree = Tree;

            //Dataservice.LogMessage($"Tree Stratum Tree_GUID:{tree.Tree_GUID} OldStratumCode:{oldValue} NewStratumCode:{newValue}", "I");

            //if (SampleGroups.Any(x => x.Code == newValue) == false)
            //{
            //    Tree.SampleGroupCode = "";
            //}
            try
            {
                _suspendSave = true;
                tree.SampleGroupCode = "";
                tree.Species = "";
                tree.LiveDead = "";
            }
            finally
            { _suspendSave = false; }

            //SaveTree(tree);

            RefreshValidationRules(tree);
            RefreshErrorsAndWarnings(tree);
            RefreshSampleGroups(tree);
            RefreshTreeDefaults(tree);
            RefreshTreeFields(tree);
        }

        private bool OnStratumChanging(string newStratum)
        {
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

        private SampleGroupProxy _nullSampleGroup = new SampleGroupProxy { Code = "" };

        public IEnumerable<SampleGroupProxy> SampleGroups
        {
            get { return _sampleGroups; }
            set
            {
                SetValue(ref _sampleGroups, value);
                RaisePropertyChanged(nameof(SampleGroupCodes));
            }
        }

        public IEnumerable<string> SampleGroupCodes => SampleGroups.OrEmpty().Select(x => x.Code).ToArray();

        public string SampleGroupCode
        {
            get { return Tree?.SampleGroupCode; }
            set
            {
                if (OnSampleGroupChanging(value))
                {
                    var oldValue = Tree.SampleGroupCode;
                    Tree.SampleGroupCode = value;
                    OnSampleGroupChanged(oldValue, value);
                }
            }
        }

        private void OnSampleGroupChanged(string oldValue, string newValue)
        {
            var tree = Tree;

            //Dataservice.LogMessage($"Tree SampleGroupCanged, Tree_GUID:{Tree.Tree_GUID}, OldSG:{oldValue}, NewSG:{newValue}", "high");

            try
            {
                _suspendSave = true;
                tree.Species = "";
                tree.LiveDead = "";
            }
            finally
            {
                _suspendSave = false;
            }
            //SaveTree(tree);

            RefreshValidationRules(tree);
            RefreshErrorsAndWarnings(tree);
            RefreshTreeDefaults(tree);
        }

        private bool OnSampleGroupChanging(string newSG)
        {
            var currSG = SampleGroupCode;

            if (string.IsNullOrWhiteSpace(newSG)) { return false; }
            if (string.IsNullOrWhiteSpace(currSG) == false
                && currSG == newSG) { return false; }

            if (string.IsNullOrWhiteSpace(currSG) == false)
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
            else
            {
                return true;
            }
        }

        #endregion SampleGroup

        #region TreeDefault

        private IEnumerable<SampleGroupProxy> _sampleGroups;

        public IEnumerable<TreeDefaultProxy> TreeDefaults
        {
            get { return _treeDefaults; }
            set { SetValue(ref _treeDefaults, value); }
        }

        public TreeDefaultProxy TreeDefault
        {
            get
            {
                return FindTreeDefault(Tree);
            }

            set
            {
                var tree = Tree;
                var existingTreeDefault = FindTreeDefault(tree);
                if (value == existingTreeDefault) { return; }

                try
                {
                    if (value != null)
                    {
                        tree.Species = value.Species;
                        tree.LiveDead = value.LiveDead;
                    }
                    else
                    {
                        tree.Species = "";
                        tree.LiveDead = "";
                    }
                }
                finally { _suspendSave = false; }

                RefreshValidationRules(tree);
                RefreshErrorsAndWarnings(tree);
            }
        }

        protected TreeDefaultProxy FindTreeDefault(Tree tree)
        {
            return TreeDefaults.OrEmpty().Where(x => x.Species == tree.Species && x.LiveDead == tree.LiveDead).FirstOrDefault();
        }

        #endregion TreeDefault

        #region tree property

        private Tree _tree;

        public Tree Tree
        {
            get { return _tree; }
            protected set
            {
                if (_tree != null) { _tree.PropertyChanged -= _tree_PropertyChanged; }
                SetValue(ref _tree, value);
                if (value != null) { value.PropertyChanged += _tree_PropertyChanged; }

                RaisePropertyChanged(nameof(this.StratumCode));
                RaisePropertyChanged(nameof(this.SampleGroupCode));
                //RaisePropertyChanged(nameof(this.TreeDefault));
            }
        }

        private void _tree_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var property = e.PropertyName;
            if (property == nameof(Tree.Species)
                || property == nameof(Tree.StratumCode)
                || property == nameof(Tree.SampleGroupCode)
                || property == nameof(Tree.LiveDead)) { return; }//when these property change we will be saving the tree manualy

            RefreshErrorsAndWarnings(Tree);
        }

        #endregion tree property

        public TreeEditViewModel(ICuttingUnitDatastoreProvider datastoreProvider
            , IDialogService dialogService)
        {
            Datastore = datastoreProvider.CuttingUnitDatastore;
            DialogService = dialogService;
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);
            Tree = null;//unwire tree
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            LoadData(parameters);
            base.OnNavigatedTo(parameters);
        }

        private void LoadData(INavigationParameters parameters)
        {
            var tree_guid = parameters.GetValue<string>("Tree_Guid");

            var datastore = Datastore;

            var tree = Tree = datastore.GetTree(tree_guid);

            if (tree == null)
            {
                Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Tree Read Error", new Dictionary<string, string>() { { "params", parameters.ToString() } });

                System.Diagnostics.Debug.Write("Error::::Tree was null when it souldn't have");
                return;
            }

            var unitCode = tree.UnitCode;
            Strata = datastore.GetStrataProxiesByUnitCode(unitCode).ToArray();

            RefreshSampleGroups(tree);
            RefreshTreeDefaults(tree);
            RefreshTreeFields(tree);
            RefreshValidationRules(tree);
            RefreshErrorsAndWarnings(tree);
        }

        private void RefreshSampleGroups(Tree tree)
        {
            var stratum = tree.StratumCode;
            var sampleGroups = Datastore.GetSampleGroupProxies(StratumCode);
            _sampleGroups = sampleGroups.Prepend(_nullSampleGroup).ToArray();
            RaisePropertyChanged(nameof(this.SampleGroups));
        }

        private void RefreshTreeDefaults(Tree tree)
        {
            var sampleGroup = tree.SampleGroupCode;
            var stratum = tree.StratumCode;
            var treeDefaults = Datastore.GetTreeDefaultProxies(stratum, sampleGroup);
            _treeDefaults = treeDefaults.Prepend(NULL_TREE_DEFAULT).ToArray();
            RaisePropertyChanged(nameof(this.TreeDefaults));
            RaisePropertyChanged(nameof(this.TreeDefault));
        }

        private void RefreshTreeFields(Tree tree)
        {
            var stratumCode = tree.StratumCode;

            TreeFieldsExtended = Datastore.GetTreeFieldsByStratumCode(stratumCode).ToArray();
        }

        protected void RefreshValidationRules(Tree tree)
        {
            if (tree == null) { return; }

            _treeValidator = new TreeValidator();
            if (!string.IsNullOrWhiteSpace(tree.StratumCode)
                && !string.IsNullOrWhiteSpace(tree.SampleGroupCode)
                && !string.IsNullOrWhiteSpace(tree.Species)
                && !string.IsNullOrWhiteSpace(tree.LiveDead))
            {
                var treeAuditRules = Datastore.GetTreeAuditRules(tree.StratumCode, tree.SampleGroupCode, tree.Species, tree.LiveDead);
                _treeValidator.Rules.AddRange(treeAuditRules);
            }
        }

        protected void RefreshErrorsAndWarnings(Tree tree)
        {
            if (tree == null) { return; }

            _lastValidationResult = _treeValidator.Validate(tree, TreeFields.Select(x => x.Field)
                .Append("Heights")
                .Append("Diameters"));

            RaiseErrorsAndWarningsChanged();
        }

        protected void RaiseErrorsAndWarningsChanged()
        {
            ErrorsAndWarningsChanged?.Invoke(this, null);
            RaisePropertyChanged(nameof(ErrorsAndWarnings));
        }

        public void SaveTree()
        {
            SaveTree(Tree);
        }

        private void SaveTree(Tree tree)
        {
            if (_suspendSave) { return; }
            if (tree != null)
            {
                Datastore.UpdateTree(Tree);
                Datastore.UpdateTreeErrors(tree.Tree_GUID, ErrorsAndWarnings);
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