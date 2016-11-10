using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace FScruiser.Models
{
    [Table("Stratum")]
    public class Stratum
    {
        [Key]
        public long Stratum_CN { get; set; }

        public string Code { get; set; }

        public string Method { get; set; }

        public int KZ3ppnt { get; set; }

        public bool IsPlotStratum => CruiseMethods.PLOT_METHODS.Contains(Method);
    }
}