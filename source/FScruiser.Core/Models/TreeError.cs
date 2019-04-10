using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Models
{
    public class TreeError
    {
        public string TreeID { get; set; }

        public string TreeAuditRuleID { get; set; }

        public string ErrorCode { get; set; }

        public string Message { get; set; }

        public string Field { get; set; }

        public string Resolution { get; set; }

        public string ResolutionInitials { get; set; }
    }
}
