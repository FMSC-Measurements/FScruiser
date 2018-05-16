using CruiseDAL.DataObjects;
using FMSC.ORM.EntityModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Models
{
    public class Tree : TreeDO
    {
        private bool _hasFieldData;

        [IgnoreField]
        public bool HasFieldData
        {
            get { return _hasFieldData; }
            set {
                _hasFieldData = value;
                NotifyPropertyChanged(nameof(HasFieldData));
                NotifyPropertyChanged(nameof(TallyFeedStatus));
            }
        }

        protected override void NotifyPropertyChanged(string name)
        {
            base.NotifyPropertyChanged(name);

            if (PropertyChangedEventsDisabled) { return; }//HACK/DEBT the base class implementation requires us to check that PropertyChangedEventsDisabled

            //forward property changed events
            switch (name)
            {
                case nameof(TotalHeight):
                case nameof(MerchHeightPrimary):
                case nameof(UpperStemHeight):
                    {
                        NotifyPropertyChanged(nameof(Height));
                        break;
                    }
                case nameof(DBH):
                case nameof(DRC):
                case nameof(DBHDoubleBarkThickness):
                    {
                        NotifyPropertyChanged(nameof(Diameter));
                        break;
                    }
            }

            if (name != nameof(HasFieldData) && name != nameof(TallyFeedStatus))
            { HasFieldData = true; }
        }

        [IgnoreField]
        public string TallyFeedStatus
        {
            get
            {
                if (base.HasErrors())
                {
                    return "Error";
                }
                else if (HasFieldData == false)
                {
                    return "NoData";
                }
                else
                {
                    return "HasData";
                }
            }
        }

        public double Height => new float[] { TotalHeight, MerchHeightPrimary, UpperStemHeight }.Max();

        public float Diameter => new float[] { DBH, DRC, DBHDoubleBarkThickness }.Max();
    }
}
