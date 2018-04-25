using FScruiser.Models;
using FScruiser.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.XF.Services
{
    public class XamarinDialogService : IDialogService
    {
        public bool AskCancel(string message, string caption, bool defaultCancel)
        {
            //throw new NotImplementedException();
            return false;
        }

        public void AskCruiser(Tree tree)
        {
            //throw new NotImplementedException();
        }

        public int? AskKPI(int min, int max)
        {
            //throw new NotImplementedException();
            return 0;
        }

        public bool AskYesNo(string message, string caption)
        {
            //throw new NotImplementedException();
            return false;
        }

        public bool AskYesNo(string message, string caption, bool defaultNo)
        {
            //throw new NotImplementedException();
            return false;
        }

        public void ShowEditTree(Tree tree)
        {
            //throw new NotImplementedException();
        }

        public void ShowMessage(string message)
        {
            //throw new NotImplementedException();
        }

        public void ShowMessage(string message, string caption)
        {
            //throw new NotImplementedException();
        }
    }
}
