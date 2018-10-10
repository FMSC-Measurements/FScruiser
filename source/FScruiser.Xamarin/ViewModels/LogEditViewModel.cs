using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FScruiser.Models;
using FScruiser.Services;
using FScruiser.XF.Services;
using Prism.Navigation;

namespace FScruiser.XF.ViewModels
{
    public class LogEditViewModel : ViewModelBase
    {
        private Log _log;
        private IEnumerable<LogFieldSetup> _logFields;

        

        public Log Log
        {
            get => _log;
            set => SetValue(ref _log, value);
        }

        public ICuttingUnitDatastore Datastore { get; set; }

        public IEnumerable<LogFieldSetup> LogFields { get => _logFields; set => SetValue(ref _logFields, value); }

        public LogEditViewModel(INavigationService navigationService, ICuttingUnitDatastoreProvider datastoreProvider) : base(navigationService)
        {
            Datastore = datastoreProvider.CuttingUnitDatastore;
        }

        protected override void Refresh(INavigationParameters parameters)
        {
            var log_guid = parameters.GetValue<string>("Log_Guid");

            Log = Datastore.GetLog(log_guid);

            LogFields = Datastore.GetLogFields(Log.Tree_GUID);
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);

            SaveLog();
        }

        public void SaveLog()
        {
            var log = Log;
            if (log != null)
            {
                Datastore.UpdateLog(log);
            }
        }

    }
}
