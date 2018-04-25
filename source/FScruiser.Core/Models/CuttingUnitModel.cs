using CruiseDAL.DataObjects;
using FMSC.ORM.EntityModel;
using FMSC.ORM.EntityModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FScruiser.Models
{
    public class CuttingUnit : CuttingUnitDO
    {

        public override string ToString()
        {
            return string.Format("{0}: {1} Area: {2}"
                , base.Code
                , base.Description
                , base.Area);
        }
    }
}