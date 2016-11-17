using FScruiser.Models;
using FScruiser.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.ViewModels
{
    public class TreeEditViewModel : FreshMvvm.FreshBasePageModel, INotifyPropertyChanged
    {
        private Stratum _stratum;
        private Tree _tree;
        private IEnumerable<TreeField> _treeFields;

        public ICuttingUnitDataService Dataservice { get; set; }

        public IEnumerable<TreeField> TreeFields
        {
            get
            {
                return _treeFields;
            }
            set
            {
                _treeFields = value;
                base.RaisePropertyChanged();
            }
        }

        public IEnumerable<TreeDefaultValue> TreeDefaults
        {
            get
            {
                var sg = Tree?.SampleGroup;
                if (sg == null) { return new TreeDefaultValue[0]; }
                if (sg.TreeDefaults == null)
                {
                    sg.TreeDefaults = Dataservice.GetTreeDefaultsBySampleGroup(sg).ToList();
                }
                Debug.Assert(sg.TreeDefaults != null);
                return sg.TreeDefaults;
            }
        }

        public IEnumerable<SampleGroup> SampleGroups
        {
            get
            {
                var stratum = Tree?.Stratum;
                if (stratum == null) { return new SampleGroup[0]; }
                if (stratum.SampleGroups == null)
                {
                    stratum.SampleGroups = Dataservice.GetSampleGroupsByStratum(stratum).ToList();
                }
                return stratum.SampleGroups;
            }
        }

        public event EventHandler<Tree> TreeChanging;

        public event EventHandler<Tree> TreeChanged;

        public Stratum Stratum
        {
            get { return _stratum; }
            set
            {
                if (_stratum == value) { return; }
                _stratum = value;
                OnStratumChanged();
                base.RaisePropertyChanged();
            }
        }

        private void OnStratumChanged()
        {
            if (Stratum != null)
            {
                TreeFields = Dataservice.GetTreeFieldsByStratum(Stratum.Code);
            }
        }

        public Tree Tree
        {
            get { return _tree; }
            set
            {
                OnTreeChanging();
                _tree = value;
                OnTreeChanged();
                RaisePropertyChanged();
            }
        }

        public TreeEditViewModel(ICuttingUnitDataService ds)
        {
            Dataservice = ds;
        }

        private void OnTreeChanged()
        {
            Stratum = _tree?.Stratum;
            TreeChanged?.Invoke(this, _tree);
        }

        private void OnTreeChanging()
        {
            TreeChanging?.Invoke(this, _tree);
        }

        public override void Init(object initData)
        {
        }
    }
}