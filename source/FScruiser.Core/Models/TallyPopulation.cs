using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Models
{
    //[EntitySource("CountTree", JoinCommands = "JOIN SampleGroup USING (SampleGroup_CN) JOIN Stratum USING (Stratum_CN) JOIN Tally USING (Tally_CN)")]
    [Table("CountTree")]
    public class TallyPopulation : INotifyPropertyChanged
    {
        int _sumKPI;
        int _treeCount;

        [Key]
        public long CountTree_CN { get; set; }

        public long CuttingUnit_CN { get; set; }

        public long SampleGroup_CN { get; set; }

        [ForeignKey(nameof(SampleGroup_CN))]
        public SampleGroup SampleGroup { get; set; }

        public long? TreeDefaultValue_CN { get; set; }

        [ForeignKey(nameof(TreeDefaultValue_CN))]
        public TreeDefaultValue TDV { get; set; }

        [ForeignKey("Tally_CN")]
        public Tally Tally { get; set; }

        public long Tally_CN { get; set; }
        public string Description => Tally.Description;

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

        Sampler _sampler;

        [NotMapped]
        public Sampler Sampler => _sampler ?? (_sampler = new Sampler(this.SampleGroup));

        #region INotifyPropertyChanged members

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged members
    }

    [Table("Tally")]
    public class Tally
    {
        [Key]
        public long Tally_CN { get; set; }

        public string Description { get; set; }

        public string HotKey { get; set; }
    }
}