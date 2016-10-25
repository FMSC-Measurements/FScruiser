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
        //[PrimaryKeyField]
        public long CountTree_CN { get; set; }

        public long CuttingUnit_CN { get; set; }

        [Field(SQLExpression = "SampleGroup.Stratum_CN", Alias = "Stratum_CN")]
        public long Stratum_CN { get; set; }

        public long SampleGroup_CN { get; set; }

        public long? TreeDefaultValue_CN { get; set; }

        int _treeCount;

        public int TreeCount
        {
            get { return _treeCount; }
            set
            {
                _treeCount = value;
                OnPropertyChanged();
            }
        }

        public int SumKPI { get; set; }

        public Sampler Sampler { get; set; }

        [Field(SQLExpression = "Tally.Description", Alias = "Description")]
        public string Description { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}