using Backpack.EntityModel.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Models
{
    [EntitySource("CountTree", JoinCommands = "JOIN SampleGroup USING (SampleGroup_CN) JOIN Stratum USING (Stratum_CN) JOIN Tally USING (Tally_CN)")]
    public class TallyPopulation : INotifyPropertyChanged
    {
        int _sumKPI;
        int _treeCount;

        [PrimaryKeyField(Name = "CountTree_CN")]
        public long CountTree_CN { get; set; }

        public long CuttingUnit_CN { get; set; }

        public long SampleGroup_CN { get; set; }

        [Field(SQLExpression = "SampleGroup.Stratum_CN", Alias = "Stratum_CN")]
        public long Stratum_CN { get; set; } // we need this so we can create trees from a tally pop, but is otherwise unnecessary

        public long? TreeDefaultValue_CN { get; set; }

        [Field(SQLExpression = "Tally.Description", Alias = "Description")]
        public string Description { get; set; }

        [Field(Alias = "StratumCode", SQLExpression = "Stratum.Code")]
        public string StratumCode { get; set; } // may need this if we want to resolve sampler using sample group code

        [Field(SQLExpression = "SampleGroup.Code", Alias = "SampleGroupCode")]
        public string SampleGroupCode { get; set; }

        public int SumKPI
        {
            get { return _sumKPI; }
            set
            {
                _sumKPI = value;
                OnPropertyChanged();
            }
        }

        public int TreeCount
        {
            get { return _treeCount; }
            set
            {
                _treeCount = value;
                OnPropertyChanged();
            }
        }

        [IgnoreField]
        public Sampler Sampler { get; set; }

        #region INotifyPropertyChanged members

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged members
    }
}