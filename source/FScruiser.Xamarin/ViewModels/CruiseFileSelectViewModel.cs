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
        private ICruiseFileService _folderService;
        private IEnumerable<FileGroup> _fileGroups;
        private FileGroup _selectedFolder;

        public ICruiseFileService FolderService
        {
            get { return _folderService; }
            set {
                SetValue(ref _folderService, value);
                RaisePropertyChanged(nameof(FileGroups));
            }
        }

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

        public CruiseFileSelectViewModel(INavigation navigation, ICruiseFileService folderService) : base(navigation)
        {
            FolderService = folderService;
        }

        public void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs eventArgs)
        {
            var selectedFile = eventArgs.SelectedItem as FileInfo;

            LoadCruiseFile(selectedFile);
        }

        public void LoadCruiseFile(FileInfo file)
        {
            if (file.Exists == false) { throw new FileNotFoundException("cruise file not found", file.FullName); }

            var dataService = new CruiseDataService(file.FullName);
            App.ServiceService.CruiseDataService = dataService;
        }

        public override void Init()
        {
            FileGroups = FolderService.CruiseFilesGroups.ToArray();
            SelectedFolder = FileGroups.FirstOrDefault();
        }
    }
}
