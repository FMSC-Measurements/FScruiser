using FScruiser.Models;
using FScruiser.Services;
using FScruiser.XF.Pages;
using Plugin.FilePicker;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    public class CuttingUnitListViewModel : ViewModelBase, IDisposable
    {
        private IEnumerable<CuttingUnit> _units;

        public ICruiseDataService DataService => ServiceService.CruiseDataService;

        public bool IsFileNotOpen => Units == null;

        public IEnumerable<CuttingUnit> Units
        {
            get { return _units; }
            set { SetValue(ref _units, value); }
        }

        public CuttingUnitListViewModel()
        {
            MessagingCenter.Subscribe<object>(this, Messages.CRUISE_FILE_SELECTED, (sender) =>
            {
                LoadData();
            });
        }

        private void LoadData()
        {
            var dataService = DataService;
            if (dataService != null)
            {
                Units = DataService.Units;
                base.RaisePropertyChanged(nameof(IsFileNotOpen));
            }
        }

        public override Task InitAsync()
        {
            LoadData();
            return Task.CompletedTask;
        }

        public void SelectUnit(CuttingUnit unit)
        {
            if (unit == null) { throw new ArgumentNullException(nameof(unit)); }

            var cruiseDataService = DataService;
            var unitDataService = new CuttingUnitDataService(cruiseDataService.Path, unit);
            ServiceService.CuttingUnitDataSercie = unitDataService;

            MessagingCenter.Send<object>(this, Messages.CUTTING_UNIT_SELECTED);
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls
        

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~MainViewModel() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}