﻿using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FScruiser.Util
{
    public class INPC_Base
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propName)
        {
            RaisePropertyChanged(new PropertyChangedEventArgs(propName));
        }

        protected void RaisePropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        public void SetValue<tTarget>(ref tTarget target, tTarget value, [CallerMemberName] string propName = null)
        {
            target = value;
            if (propName != null) { RaisePropertyChanged(propName); }
        }
    }
}