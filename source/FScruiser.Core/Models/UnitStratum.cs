using CruiseDAL.DataObjects;
using FMSC.ORM.EntityModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Models
{

    public class UnitStratum : StratumDO
    {
        #region stratum members
        [Field(SQLExpression = "Stratum.Code", Alias = "StratumCode")]
        public string StratumCode { get; set; }

        [Field(SQLExpression = "Stratum.Description", Alias = "StratumDescription")]
        public string StratumDescription { get; set; }

        [Field(SQLExpression = "Stratum.Method", Alias = "StratumMethod")]
        public string Method { get; set; }

        [Field(SQLExpression = "Stratum.HotKey", Alias = "StratumHotKey")]
        public string HotKey { get; set; }
        #endregion

        [Field(SQLExpression = "CuttingUnit.Code", Alias = "UnitCode")]
        public string UnitCode { get; set; }
    }
}