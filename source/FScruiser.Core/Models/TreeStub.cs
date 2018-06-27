using FMSC.ORM.EntityModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Models
{
    [EntitySource("Tree")]
    public class TreeStub
    {
        [Field("Tree_GUID")]
        public string Tree_GUID { get; set; }

        [Field("TreeNumber")]
        public int TreeNumber { get; set; }

        [Field(Alias ="StratumCode", SQLExpression ="Stratum.Code")]
        public string StratumCode { get; set; }

        [Field(Alias = "SgCode", SQLExpression = "SampleGroup.Code")]
        public string SampleGroupCode { get; set; }

        [Field("Species")]
        public string Species { get; set; }

        [Field(Alias = "Height", SQLExpression = "max(TotalHeight, MerchHeightPrimary, UpperStemHeight)")]
        public int Height { get; set; }

        [Field(Alias = "Diameter", SQLExpression = "max(DBH, DRC, DBHDoubleBarkThickness)")]
        public int Diameter { get; set; }
    }
}
