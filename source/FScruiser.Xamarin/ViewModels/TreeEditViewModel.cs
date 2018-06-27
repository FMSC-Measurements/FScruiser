using CruiseDAL.DataObjects;
using FScruiser.Models;
using FScruiser.Services;
using FScruiser.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FScruiser.XF.ViewModels
{
    public class TreeEditViewModel : ViewModelBase, IDisposable
    {
        private Tree _tree;
        private IEnumerable<TreeFieldSetupDO> _treeFields;

        private bool _useSimplifiedTreeFields;
        private IEnumerable<SampleGroupProxy> _sampleGroups;
        private IEnumerable<TreeDefaultProxy> _treeDefaults;
        private IEnumerable<StratumProxy> _strata;

        protected ICuttingUnitDataService Dataservice => ServiceService.CuttingUnitDataService;
        protected IDialogService DialogService => ServiceService.DialogService;

        public event EventHandler<IEnumerable<TreeFieldSetupDO>> TreeFieldsChanged;

        public IEnumerable<TreeFieldSetupDO> TreeFields
        {
            get
            {
                return _treeFields;
            }
            set
            {
                SetValue(ref _treeFields, value);
                TreeFieldsChanged?.Invoke(this, value);
            }
        }

        #region Stratum

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
            tree.SampleGroupCode = "";
            tree.Species = "";
            tree.LiveDead = "";

            SaveTree(tree);

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

            tree.Species = "";
            tree.LiveDead = "";
            SaveTree(tree);

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

        private TreeDefaultProxy _nullTreeDefault = new TreeDefaultProxy() { Species = "", LiveDead = "" };

        public IEnumerable<TreeDefaultProxy> TreeDefaults
        {
            get { return _treeDefaults; }
            set { SetValue(ref _treeDefaults, value); }
        }

        public TreeDefaultProxy TreeDefault
        {
            get
            {
                var tree = Tree;
                return TreeDefaults.OrEmpty().Where(x => x.Species == tree.Species && x.LiveDead == tree.LiveDead).FirstOrDefault();
            }

            set
            {
                var tree = Tree;
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
                SaveTree();
            }
        }

        public Tree Tree
        {
            get { return _tree; }
            protected set
            {
                if (_tree != null) { _tree.PropertyChanged -= _tree_PropertyChanged; }
                SetValue(ref _tree, value);
                RaisePropertyChanged(nameof(this.StratumCode));
                RaisePropertyChanged(nameof(this.SampleGroupCode));
                //RaisePropertyChanged(nameof(this.TreeDefault));

                if (_tree != null) { _tree.PropertyChanged += _tree_PropertyChanged; }
            }
        }

        public TreeEditViewModel(bool useSimplifiedTreeFields = false) : base()
        {
            _useSimplifiedTreeFields = useSimplifiedTreeFields;
        }

        public void Init(string tree_guid)
        {
            var tree = Tree = Dataservice.GetTree(tree_guid);
            Strata = Dataservice.GetStratumProxies().ToArray();

            if (tree == null)
            {
                System.Diagnostics.Debug.Write("Tree was null when it souldn't have");
                return;
            }

            RefreshSampleGroups(tree);
            RefreshTreeDefaults(tree);
            RefreshTreeFields(tree);
        }

        private void RefreshSampleGroups(Tree tree)
        {
            

            var stratum = tree.StratumCode;
            var sampleGroups = Dataservice.GetSampleGroupProxies(StratumCode);
            _sampleGroups = sampleGroups.Prepend(_nullSampleGroup).ToArray();
            RaisePropertyChanged(nameof(this.SampleGroups));
        }

        private void RefreshTreeDefaults(Tree tree)
        {

            var sampleGroup = tree.SampleGroupCode;
            var stratum = tree.StratumCode;
            var treeDefaults = Dataservice.GetTreeDefaultProxies(stratum, sampleGroup);
            _treeDefaults = treeDefaults.Prepend(_nullTreeDefault).ToArray();
            RaisePropertyChanged(nameof(this.TreeDefaults));
            RaisePropertyChanged(nameof(this.TreeDefault));
        }

        private void RefreshTreeFields(Tree tree)
        {
            var stratumCode = tree.StratumCode;

            if (_useSimplifiedTreeFields)
            {
                TreeFields = Dataservice.GetSimplifiedTreeFieldsByStratumCode(stratumCode);
            }
            else
            {
                TreeFields = Dataservice.GetTreeFieldsByStratumCode(stratumCode);
            }
        }

        private void _tree_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Tree.Species)
                || e.PropertyName == nameof(Tree.LiveDead)
                || e.PropertyName == nameof(Tree.StratumCode)
                || e.PropertyName == nameof(Tree.SampleGroupCode))
            { return; }//when these properties change we will call save manualy

            SaveTree();
        }

        public void SaveTree()
        {
            SaveTree(Tree);
            
        }

        void SaveTree(Tree tree)
        {
            if (tree != null)
            {
                Dataservice.UpdateTree(Tree);
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

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Tree = null;
                }
                disposedValue = true;
            }
            else
            {
                throw new ObjectDisposedException(nameof(TreeEditViewModel));
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion IDisposable Support
    }
}