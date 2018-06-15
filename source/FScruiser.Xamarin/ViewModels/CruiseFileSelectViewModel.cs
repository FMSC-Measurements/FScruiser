using FScruiser.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    public class CruiseFileSelectViewModel : ViewModelBase
    {
        private IEnumerable<FileGroup> _fileGroups;
        private FileGroup _selectedFolder;

        public ICruiseFileService FileService => ServiceService.CruiseFileService;

        public FileGroup SelectedFolder
        {
            get { return _selectedFolder; }
            set { SetValue(ref _selectedFolder, value); }
        }

        public IEnumerable<FileGroup> FileGroups
        {
            get { return _fileGroups; }
            set { SetValue(ref _fileGroups, value); }
        }

        public CruiseFileSelectViewModel()
        {
        }

        public void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs eventArgs)
        {
            var selectedFile = eventArgs.SelectedItem as FileInfo;

            LoadCruiseFile(selectedFile);
        }

        public void LoadCruiseFile(FileInfo file)
        {
            if (file.Exists == false) { throw new FileNotFoundException("cruise file not found", file.FullName); }

            ServiceService.CruiseDataService = new CruiseDataService(file.FullName);
            ServiceService.CuttingUnitDataService = null;
            MessagingCenter.Send<object>(this, Messages.CRUISE_FILE_SELECTED);
        }

        public Task InitAsync()
        {
            return Task.Run(() =>
            {
                var fileService = FileService;
                var fileGroups = FileService.CruiseFilesGroups.ToArray();

                FileGroups = fileGroups;
                SelectedFolder = fileGroups.FirstOrDefault();
            });
        }
    }
}
