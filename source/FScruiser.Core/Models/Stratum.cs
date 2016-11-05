using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace FScruiser.Models
{
    [Table("Stratum")]
    public class Stratum
    {
        [Key]
        public long? Stratum_CN { get; set; }

        public string Code { get; set; }

        //[Field(SQLExpression = "Stratum.Method", Alias = "CruiseMethod")]
        public string CruiseMethod { get; set; }

        //[Field(SQLExpression = "Stratum.KZ3ppnt", Alias = "KZ3ppnt")]
        public int KZ3ppnt { get; set; }

        public bool IsPlotStratum => CruiseMethods.PLOT_METHODS.Contains(CruiseMethod);
    }
}