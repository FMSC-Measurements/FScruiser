using CruiseDAL.DataObjects;
using FScruiser.Models;
using FScruiser.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FScruiser.XF.ViewModels
{
    public class TreeEditViewModel : ViewModelBase
    {
        private Tree _tree;
        private IEnumerable<TreeFieldSetupDO> _treeFields;

        private IEnumerable<SampleGroup> _sampleGroups;
        private Dictionary<string, IEnumerable<TreeDefaultValueDO>> _sampleGroupTreeDefaultLookup;
        private IEnumerable<Stratum> _strata;

        protected ICuttingUnitDataService Dataservice => ServiceService.CuttingUnitDataService;
        protected IDialogService DialogService => ServiceService.DialogService;

        public IEnumerable<TreeFieldSetupDO> TreeFields
        {
            get
            {
                return _treeFields;
            }
            set
            {
                SetValue(ref _treeFields, value);
            }
        }

        public CuttingUnitDO CuttingUnit => Tree?.CuttingUnit;
        //public long TreeNumber => Tree?.TreeNumber ?? 0;
        //public string Species => Tree?.TreeDefaultValue?.Species;
        //public string CountOrMeasure => Tree?.CountOrMeasure;
        //public long TreeCount => Tree?.TreeCount;
        //public int KPI
        //{
        //    get { return (int)(Tree?.KPI ?? 0); }
        //    set
        //    {
        //        if (Tree != null)
        //        { Tree.KPI = value; }
        //    }
        //}
        //public bool STM
        //{
        //    get { return Tree?.STM == "Y"; }
        //    set {
        //        if (Tree != null)
        //        { Tree.STM = (value) ? "Y" : "N"; }
        //    }
        //}

        public IEnumerable<Stratum> Strata => _strata;

        public IEnumerable<SampleGroup> SampleGroups => _sampleGroups
            .Where(x => Tree == null || x.Stratum_CN == Tree.Stratum_CN).Prepend(new SampleGroup { Code = "" }).ToList();

        public IEnumerable<TreeDefaultValueDO> TreeDefaults => (_sampleGroupTreeDefaultLookup
            .Where(x => x.Key == Tree?.SampleGroup?.Code)
            .Select(x => x.Value).SingleOrDefault() ?? Enumerable.Empty<TreeDefaultValueDO>()).Prepend(new TreeDefaultValueDO { Species = "" }).ToList();

        public IEnumerable<string> SpeciesOptions { get; protected set; }

        #region Stratum

        public StratumDO Stratum
        {
            get { return Tree?.Stratum; }
            set
            {
                if (OnStratumChanging(value))
                {
                    Tree.Stratum = value;
                    OnStratumChanged(value);
                }
            }
        }

        private void OnStratumChanged(StratumDO value)
        {
            Tree.Species = null;
            Tree.SampleGroup = null;
            SetTreeTDV(Tree, null);

            RaisePropertyChanged(nameof(SampleGroups));
            RaisePropertyChanged(nameof(TreeDefaults));
        }

        private bool OnStratumChanging(StratumDO newStratum)
        {
            if (newStratum == null) { return false; }
            if (Stratum != null
                && Stratum.Stratum_CN == newStratum.Stratum_CN)
            { return false; }

            if (Stratum != null)
            {
                //if (!DialogService.AskYesNoAsync("You are changing the stratum of a tree" +
                //    ", are you sure you want to do this?", "!").Result)
                //{
                //    return false;//do not change stratum
                //}
                //else
                //{
                var tree = Tree;

                //log stratum changed
                Dataservice.LogMessage($"Tree Stratum Changed (Cu:{tree.CuttingUnit.Code} St:{tree.Stratum.Code} -> {newStratum.Code} Sg:{tree.SampleGroup.Code ?? "?"} Tdv_CN:{tree.TreeDefaultValue.TreeDefaultValue_CN?.ToString() ?? "?"} T#: {tree.TreeNumber} P#:{tree.Plot?.PlotNumber.ToString() ?? "?"}", "I");
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

        public SampleGroup SampleGroup
        {
            get { return Tree?.SampleGroup; }
            set
            {
                if (OnSampleGroupChanging(value))
                {
                    Tree.SampleGroup = value;
                    OnSampleGroupChanged(value);
                }
            }
        }

        private void OnSampleGroupChanged(SampleGroupDO value)
        {
            var tdv = Tree.TreeDefaultValue_CN;
            if (tdv != null)
            {
                var sgCode = value.Code;

                if (_sampleGroupTreeDefaultLookup.TryGetValue(sgCode, out var sgTdvs) &&
                    !sgTdvs.Any(x => x.TreeDefaultValue_CN == tdv))
                {
                    SetTreeTDV(Tree, null);
                }
            }

            RaisePropertyChanged(nameof(TreeDefaults));
        }

        private bool OnSampleGroupChanging(SampleGroupDO newSG)
        {
            if (newSG == null) { return false; }
            if (SampleGroup != null
                && SampleGroup.SampleGroup_CN == newSG.SampleGroup_CN) { return false; }

            if (SampleGroup != null)
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
                var tree = Tree;

                Dataservice.LogMessage(String.Format("Tree Sample Group Changed (Cu:{0} St:{1} Sg:{2} -> {3} Tdv_CN:{4} T#: {5}",
                    tree.CuttingUnit.Code,
                    tree.Stratum.Code,
                    tree?.SampleGroup?.Code ?? "?",
                    newSG.Code,
                    tree?.TreeDefaultValue_CN?.ToString() ?? "?",
                    tree.TreeNumber), "high");
                return true;
                //}
            }
            else
            {
                return true;
            }
        }

        #endregion SampleGroup

        public TreeDefaultValueDO TreeDefaultValue
        {
            get { return Tree?.TreeDefaultValue; }
            set
            {
                SetTreeTDV(Tree, value);
            }
        }

        public Tree Tree
        {
            get { return _tree; }
            set
            {
                SetValue(ref _tree, value);
            }
        }

        public TreeEditViewModel(Tree tree)
        {
            Tree = tree;
        }

        public override Task InitAsync()
        {
            _strata = Dataservice.Strata.ToList();
            _sampleGroups = Dataservice.SampleGroups;
            _sampleGroupTreeDefaultLookup = Dataservice.TreeDefaultSampleGroupLookup;

            TreeFields = Dataservice.TreeFields;
            return Task.CompletedTask;
        }

        public void SaveTree()
        {
            var tree = Tree;
            if (tree != null)
            {
                Dataservice.UpdateTree(Tree);
            }
        }

        public static void SetTreeTDV(Tree tree, TreeDefaultValueDO tdv)
        {
            tree.TreeDefaultValue = tdv;
            if (tdv != null)
            {
                tree.Species = tdv.Species;

                tree.LiveDead = tdv.LiveDead;
                tree.Grade = tdv.TreeGrade;
                tree.FormClass = tdv.FormClass;
                tree.RecoverablePrimary = tdv.Recoverable;
                //tree.HiddenPrimary = tdv.HiddenPrimary;//#367
            }
            else
            {
                tree.Species = string.Empty;
                tree.LiveDead = string.Empty;
                tree.Grade = string.Empty;
                tree.FormClass = 0;
                tree.RecoverablePrimary = 0;
                //this.HiddenPrimary = 0;
            }
        }
    }
}