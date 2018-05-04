using FScruiser.Models;
using FScruiser.Services;
using FScruiser.XF.Pages;
using Plugin.FilePicker;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    public class MainViewModel : ViewModelBase, IDisposable
    {
        private IEnumerable<CuttingUnit> _units;
        private Command _openFileCommand;

        public ServiceService ServiceService { get; protected set; }

        public ICruiseDataService DataService => ServiceService.CruiseDataService;

        public IEnumerable<CuttingUnit> Units
        {
            get { return _units; }
            set { SetValue(ref _units, value); }
        }

        public ICommand OpenFileCommand => _openFileCommand ?? (_openFileCommand = new Command(ShowSelectCruiseAsync));

        public MainViewModel(ServiceService serviceService, INavigation navigation) : base(navigation)
        {
            ServiceService = serviceService;

            MessagingCenter.Subscribe<MainViewModel>(this, "FileChanged", (sender) =>
            {
                Init();
            });
        }

        public override void Init()
        {
            var dataService = DataService;
            if (dataService != null)
            {
                Units = DataService.Units;
            }
        }

        private async void ShowSelectCruiseAsync()
        {
            //var viewModel = new CruiseFileSelectViewModel(Navigation);
            //var view = new CruiseFileSelectPage() { BindingContext = viewModel };

            //viewModel.Init(App.ServiceService.CruiseFileService);
            //Navigation.PushModalAsync(view);

            try
            {
                var fileData = await CrossFilePicker.Current.PickFile();
                if (fileData == null) { return; }//user canceled file picking

                var filePath = fileData.FilePath;

                LoadCruiseAsync(filePath);
            }
            catch (Exception ex)
            {
            }
        }

        private void LoadCruiseAsync(string path)
        {
            if (File.Exists(path) == false) { throw new FileNotFoundException($"Could Not Locate Cruise at {path}"); }

            ServiceService.CruiseDataService = new CruiseDataService(path);

            Init();
        }

        public void ShowUnit(CuttingUnit unit)
        {
            if (unit == null) { throw new ArgumentNullException(nameof(unit)); }

            var cruiseDataService = DataService;
            var unitDataService = new CuttingUnitDataService(cruiseDataService.Path, unit);
            ServiceService.CuttingUnitDataSercie = unitDataService;

            MessagingCenter.Send(this, "UnitSelected");

            //var view = new UnitTreeTallyPage();
            //var viewModel = new UnitTreeTallyViewModel(view.Navigation, App.ServiceService);
            //view.BindingContext = viewModel;
            //viewModel.Init(dataService);

            //Navigation.PushAsync(view);
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

        //void ShowCruise(CruiseModel cruise)
        //{
        //    var cruiseFile = new CruiseFile { Path = cruise.Path };
        //    FreshMvvm.FreshIOC.Container.Register<CruiseFile>(cruiseFile);

        //    FreshMvvm.FreshIOC.Container.Register<ICruiseDataService>(new CruiseDataService(cruiseFile));

        //    Task t = CoreMethods.PushPageModel<CruiseViewModel>(cruise);
        //    var ex = t.Exception;
        //}
    }
}