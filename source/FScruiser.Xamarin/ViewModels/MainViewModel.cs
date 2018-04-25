using FScruiser.Models;
using FScruiser.Services;
using FScruiser.XF.Pages;
using FScruiser.XF.ViewModels;
using Plugin.FilePicker;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    public class MainViewModel : ViewModelBase, IDisposable
    {
        private ICruiseDataService _dataService;
        private Command _openFileCommand;

        public ICruiseDataService DataService
        {
            get { return _dataService; }
            set { SetValue(ref _dataService, value); }
        }

        public IEnumerable<CuttingUnit> Units
        {
            get { return _units; }
            set { SetValue(ref _units, value); }
        }

        public ICommand OpenFileCommand => _openFileCommand ?? (_openFileCommand = new Command(ShowSelectCruiseAsync));

        public MainViewModel(INavigation navigation) : base(navigation)
        {
            DataService = App.ServiceService.CruiseDataService;

            MessagingCenter.Subscribe<MainViewModel>(this, "FileChanged", (sender) =>
            {
                DataService = App.ServiceService.CruiseDataService;
            });
        }

        public void Init() { }

        async void ShowSelectCruiseAsync()
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

                await LoadCruiseAsync(filePath);
            }
            catch(Exception ex)
            {

            }
        }

        async Task LoadCruiseAsync(string path)
        {
            if (File.Exists(path) == false) { throw new FileNotFoundException($"Could Not Locate Cruise at {path}"); }

            //await Task.Run(() => { DataService = new CruiseDataService(path); });
            DataService = new CruiseDataService(path);
            Units = DataService.Units;
        }

        public void ShowUnit(CuttingUnit unit)
        {
            if (unit == null) { throw new ArgumentNullException(nameof(unit)); }


            var dataService = new CuttingUnitDataService(DataService.Path);

            var view = new UnitTreeTallyPage();
            var viewModel = new UnitTreeTallyViewModel(view.Navigation, App.ServiceService);
            view.BindingContext = viewModel;
            viewModel.Init(dataService, unit.Code);

            Navigation.PushAsync(view);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls
        private IEnumerable<CuttingUnit> _units;

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
        #endregion

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