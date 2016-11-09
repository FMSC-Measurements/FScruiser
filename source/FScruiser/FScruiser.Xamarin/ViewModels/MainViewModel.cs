using FScruiser.Models;
using FScruiser.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.ViewModels
{
    public class MainViewModel : FreshMvvm.FreshBasePageModel
    {
        public IEnumerable<CruiseModel> CruiseFiles => FindCruiseFiles();

        ICruiseFolderService FolderService { get; set; }

        public MainViewModel(ICruiseFolderService folderService)
        {
            FolderService = folderService;
        }

        IEnumerable<CruiseModel> FindCruiseFiles()
        {
            foreach (var path in FolderService.CruiseFolders)
            {
                var fi = new FileInfo(path);
                yield return new CruiseModel(fi);
            }
        }

        public ICommand ShowCruiseCommand =>
                            new Command<CruiseModel>(
                file => { ShowCruise(file); });

        void ShowCruise(CruiseModel cruise)
        {
            var cruiseFile = new CruiseFile { Path = cruise.Path };
            FreshMvvm.FreshIOC.Container.Register<CruiseFile>(cruiseFile);

            FreshMvvm.FreshIOC.Container.Register<ICruiseDataService>(new CruiseDataService(cruiseFile));

            Task t = CoreMethods.PushPageModel<CruiseViewModel>(cruise);
            var ex = t.Exception;
        }
    }
}