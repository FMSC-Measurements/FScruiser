using FScruiser.Views;
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
    public class CruiseFile
    {
        public CruiseFile(FileInfo fileInfo)
        { _fileInfo = fileInfo; }

        FileInfo _fileInfo;

        public string FileName => _fileInfo?.Name;
        public string Path => _fileInfo?.FullName;
    }

    public class MainPageViewModel
    {
        public ICommand CruiseSelectedCommand => new Command<CruiseFile>(x => CruiseFileSelected(x));

        public IEnumerable<CruiseFile> CruiseFiles => GetCruiseFiles();

        IEnumerable<CruiseFile> GetCruiseFiles()
        {
            foreach (var path in App.FolderService.CruiseFolders)
            {
                var fi = new FileInfo(path);
                yield return new CruiseFile(fi);
            }
        }

        async void CruiseFileSelected(CruiseFile cruiseFile)
        {
            var page = new CuttingUnitList();
            await App.Current.MainPage.Navigation.PushModalAsync(page);
        }
    }
}