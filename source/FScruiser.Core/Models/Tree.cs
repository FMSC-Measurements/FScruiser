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
    [Table("Tree")]
    public class Tree : INotifyPropertyChanged
    {
        private Stratum _stratum;
        private SampleGroup _sampleGroup;
        private TreeDefaultValue _treeDefaultValue;
        private double _dbh;
        private double _dbhdbt;
        private int _treeNumber;

        #region keyFields

        [Key]
        public long Tree_CN { get; set; }

        public Guid Tree_GUID { get; set; }

        [Required]
        public long CuttingUnit_CN { get; set; }

        [Required]
        public long Stratum_CN { get; set; }

        public long? SampleGroup_CN { get; set; }
        public long? TreeDefaultValue_CN { get; set; }
        public long? Plot_CN { get; set; }

        public int TreeNumber
        {
            get { return _treeNumber; }
            set
            {
                _treeNumber = value;
                OnPropertyChanged();
            }
        }

        public string Species { get; set; }

        [ForeignKey(nameof(Stratum_CN))]
        public Stratum Stratum
        {
            get { return _stratum; }
            set
            {
                _stratum = value;
                OnPropertyChanged();
            }
        }

        [ForeignKey(nameof(SampleGroup_CN))]
        public SampleGroup SampleGroup
        {
            get { return _sampleGroup; }
            set
            {
                _sampleGroup = value;
                OnPropertyChanged();
            }
        }

        [ForeignKey(nameof(TreeDefaultValue_CN))]
        public TreeDefaultValue TreeDefaultValue
        {
            get { return _treeDefaultValue; }
            set
            {
                _treeDefaultValue = value;
                OnPropertyChanged();
            }
        }

        #endregion keyFields

        #region metaFields

        public double Diameter => Math.Max(DBH, DRC);

        public double Height => Math.Max(TotalHeight, MerchHeightPrimary);

        #endregion metaFields

        public double Aspect { get; set; }

        public string ClearFace { get; set; }

        public double CrownRatio { get; set; }

        public string CountOrMeasure { get; set; }

        public double DBH
        {
            get { return _dbh; }
            set
            {
                _dbh = value;
                OnPropertyChanged();
            }
        }

        public double DBHDoubleBarkThickness
        {
            get { return _dbhdbt; }
            set
            {
                _dbhdbt = value;
                OnPropertyChanged();
            }
        }

        public double? DefectCode { get; set; }

        public double DiameterAtDefect { get; set; }

        public double DRC { get; set; }

        public double FormClass { get; set; }

        public string Grade { get; set; }

        public double HeightToFirstLiveLimb { get; set; }

        public string Initials { get; set; }

        public bool IsFallBuckScale { get; set; }

        public int KPI { get; set; }

        public string LiveDead { get; set; }

        public double MerchHeightPrimary { get; set; }

        public double MerchHeightSecondary { get; set; }

        public double PoleLength { get; set; }

        public double RecoverablePrimary { get; set; }

        public string Remarks { get; set; }

        public double SeenDefectPrimary { get; set; }

        public double SeenDefectSecondary { get; set; }

        public bool STM { get; set; }

        public double Slope { get; set; }

        public double TopDIBPrimary { get; set; }

        public double TopDIBSecondary { get; set; }

        public double TotalHeight { get; set; }

        public int TreeCount { get; set; }

        public double UpperStemDOB { get; set; }

        public double UpperStemHeight { get; set; }

        public double VoidPercent { get; set; }

        [Required]
        public string CreatedBy { get; set; } = "Default";

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string property = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

            switch (property)
            {
                case nameof(DBH):
                case nameof(DBHDoubleBarkThickness):
                    {
                        OnPropertyChanged(nameof(Diameter));
                        break;
                    }
                case nameof(TotalHeight):
                case nameof(MerchHeightPrimary):
                    {
                        OnPropertyChanged(nameof(Height));
                        break;
                    }
            }
        }
    }
}