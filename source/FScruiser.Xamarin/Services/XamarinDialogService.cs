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
        public Task<bool> AskCancelAsync(string message, string caption, bool defaultCancel)
        {
            //throw new NotImplementedException();
            return Task.FromResult(false);
        }

        public async Task AskCruiserAsync(Tree tree)
        {
            var tallySettings = App.ServiceService.TallySettingsDataService;
            var cruisers = tallySettings.Cruisers.ToArray();

            var result = await App.Current.MainPage.DisplayActionSheet("Select Cruiser", "Cancel", null, cruisers);

            if (result == "Cancel") { return; }

            tree.Initials = result;
        }

        public Task<int?> AskKPI(int min, int max)
        {
            //throw new NotImplementedException();
            return Task.FromResult((int?)1);
        }

        public Task<bool> AskYesNoAsync(string message, string caption)
        {
            //throw new NotImplementedException();
            return Task.FromResult(false);
        }

        public Task<bool> AskYesNoAsync(string message, string caption, bool defaultNo)
        {
            //throw new NotImplementedException();
            return Task.FromResult(false);
        }

        public Task ShowEditTreeAsync(Tree tree)
        {
            //throw new NotImplementedException();
            return Task.FromResult(false);
        }

        public Task ShowMessageAsync(string message)
        {
            //throw new NotImplementedException();
            return Task.FromResult(false);
        }

        public Task ShowMessageAsync(string message, string caption)
        {
            //throw new NotImplementedException();
            return Task.FromResult(false);
        }
    }
}
