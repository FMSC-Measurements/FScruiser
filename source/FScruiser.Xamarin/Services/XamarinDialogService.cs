using FScruiser.Models;
using FScruiser.Services;
using FScruiser.XF.Pages;
using FScruiser.XF.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FScruiser.XF.Services
{
    public class XamarinDialogService : IDialogService
    {
        private TaskCompletionSource<int?> _askKpiTcs;

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

        public Task<string> AskValue(string prompt, params string[] values)
        {
            return App.Current.MainPage.DisplayActionSheet(prompt, "Cancel", null, values);
        }

        public Task<int?> AskKPIAsync(int max, int min = 1)
        {
            var newTcs = new TaskCompletionSource<int?>();

            if (System.Threading.Interlocked.CompareExchange(ref _askKpiTcs, newTcs, null) != null)//if _askKpiTcs == null then _askKpiTcs = newTcs; return origianl value of _askKpiTcs
            {
                throw new InvalidOperationException("only one dialog can be active at a time");
            }

            var view = new AskKpiPage() { MinKPI = min, MaxKPI = max };

            void handelClose(object sender, AskKPIResult ea)
            {
                {
                    var tcs = System.Threading.Interlocked.Exchange(ref _askKpiTcs, null);//_askKpiTcs = null; return original value of _askKpiTcs

                    view.HandleClosed -= handelClose;

                    if (ea.DialogResult == DialogResult.Cancel)
                    {
                        tcs?.SetResult(null);
                    }
                    else if (ea.IsSTM)
                    {
                        tcs?.SetResult(-1);
                    }
                    else
                    {
                        tcs?.SetResult(ea.KPI);
                    }
                }
            }

            view.HandleClosed += handelClose;

            App.Current.MainPage.Navigation.PushModalAsync(view);

            return _askKpiTcs.Task;
        }

        public Task<bool> AskYesNoAsync(string message, string caption, bool defaultNo = false)
        {
            return App.Current.MainPage.DisplayAlert(caption, message, "Yes", "No");
        }

        public Task ShowEditTreeAsync(Tree tree, ICuttingUnitDataService dataService)
        {
            var navigation = App.Current.MainPage.Navigation;

            var view = new TreeEditPage2();
            var viewModel = new TreeEditViewModel(tree);
            view.BindingContext = viewModel;
            viewModel.InitAsync();

            return App.Current.MainPage.Navigation.PushModalAsync(view);
        }

        public Task ShowMessageAsync(string message, string caption = null)
        {
            return App.Current.MainPage.DisplayAlert(caption, message, "OK");
        }
    }
}