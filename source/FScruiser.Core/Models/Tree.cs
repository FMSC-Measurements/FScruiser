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

            if (!base.PropertyChangedEventsDisabled && name != nameof(HasFieldData) && name != nameof(TallyFeedStatus))
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
    }
}
