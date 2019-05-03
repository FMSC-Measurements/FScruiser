using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Services
{
    public interface ISoundService
    {
        void SignalMeasureTree();

        void SignalInsuranceTree();

        void SignalTally(bool force = false);

        void SignalInvalidAction();
    }
}
