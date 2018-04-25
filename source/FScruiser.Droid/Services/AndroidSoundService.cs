using FScruiser.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Droid.Services
{
    public class AndroidSoundService : ISoundService
    {
        public void SignalInsuranceTree()
        {
#if !DEBUG 
            throw new NotImplementedException(); 
#endif
        }

        public void SignalInvalidAction()
        {
#if !DEBUG
            throw new NotImplementedException(); 
#endif
        }

        public void SignalMeasureTree()
        {
#if !DEBUG
            throw new NotImplementedException(); 
#endif
        }

        public void SignalTally(bool force)
        {
#if !DEBUG
            throw new NotImplementedException(); 
#endif
        }
    }
}
