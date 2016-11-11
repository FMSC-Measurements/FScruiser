using FScruiser.Models;
using FScruiser.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.ViewModels
{
    public class TreeEditViewModel : FreshMvvm.FreshBasePageModel, INotifyPropertyChanged
    {
        private Stratum _stratum;
        private Tree _tree;

        public ICuttingUnitDataService Dataservice { get; set; }

        public IEnumerable<TreeField> TreeFields => Stratum.TreeFields;

        public event EventHandler<Tree> TreeChanging;

        public event EventHandler<Tree> TreeChanged;

        public Stratum Stratum
        {
            get { return _stratum; }
            set
            {
                if (_stratum == value) { return; }
                _stratum = value;
                base.RaisePropertyChanged();
                base.RaisePropertyChanged(nameof(TreeFields));
            }
        }

        public Tree Tree
        {
            get { return _tree; }
            set
            {
                RaiseTreeChanging();
                _tree = value;
                RaiseTreeChanged();
                RaisePropertyChanged();
            }
        }

        private void RaiseTreeChanged()
        {
            TreeChanged?.Invoke(this, _tree);
        }

        private void RaiseTreeChanging()
        {
            TreeChanging?.Invoke(this, _tree);
        }

        public TreeEditViewModel(ICuttingUnitDataService ds)
        {
            Dataservice = ds;
        }

        public override void Init(object initData)
        {
        }
    }
}