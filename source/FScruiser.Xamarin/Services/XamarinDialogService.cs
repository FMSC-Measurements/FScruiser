using FScruiser.Services;
using FScruiser.XF.Pages;
using Prism.Common;
using Prism.Ioc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FScruiser.XF.Services
{
    public class XamarinDialogService : IDialogService
    {
        private TaskCompletionSource<int?> _askKpiTcs;
        private IApplicationProvider _applicationProvider;
        private IContainerExtension _container;

        public XamarinDialogService(IApplicationProvider applicationProvider, IContainerExtension container)
        {
            _applicationProvider = applicationProvider;
            _container = container;
        }

        private Page GetCurrentPage()
        {
            Page page = null;
            if (_applicationProvider.MainPage.Navigation.ModalStack.Count > 0)
                page = _applicationProvider.MainPage.Navigation.ModalStack.LastOrDefault();
            else
                page = _applicationProvider.MainPage.Navigation.NavigationStack.LastOrDefault();

            if (page == null)
                page = _applicationProvider.MainPage;

            return page;
        }

        public Task<bool> AskCancelAsync(string message, string caption, bool defaultCancel)
        {
            //throw new NotImplementedException();
            return Task.FromResult(false);
        }

        public async Task<string> AskCruiserAsync()
        {
            var tallySettings = _container.Resolve<ITallySettingsDataService>();
            var cruisers = tallySettings.Cruisers.ToArray();

            if (cruisers.Count() == 0) { return null; }

            var result = await GetCurrentPage().DisplayActionSheet("Select Cruiser", "Cancel", null, cruisers);

            if (result == "Cancel") { return null; }

            return result;
        }

        public async Task<string> AskValueAsync(string prompt, params string[] values)
        {
            var result = await GetCurrentPage().DisplayActionSheet(prompt, "Cancel", null, values);
            if (result == "Cancel") { result = null; }
            return result;
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

            GetCurrentPage().Navigation.PushModalAsync(view);

            return _askKpiTcs.Task;
        }

        public Task<bool> AskYesNoAsync(string message, string caption, bool defaultNo = false)
        {
            return App.Current.MainPage.DisplayAlert(caption, message, "Yes", "No");
        }

        public Task ShowMessageAsync(string message, string caption = null)
        {
            return GetCurrentPage().DisplayAlert(caption, message, "OK");
        }
    }
}