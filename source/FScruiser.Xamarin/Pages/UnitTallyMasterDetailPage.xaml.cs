using FScruiser.Services;
using FScruiser.XF.ViewModels;
using Plugin.FilePicker;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FScruiser.XF.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UnitTallyMasterDetailPage : MasterDetailPage
    {

        public UnitTallyMasterDetailPage()
        {
            InitializeComponent();

            MenuItemsListView.ItemSelected += ListView_ItemSelected;

            var serviceService = App.ServiceService;
            var viewModel = new MasterViewModel(Navigation);
            MasterPage.BindingContext = viewModel;

            MessagingCenter.Subscribe<object>(this, Messages.CRUISE_FILE_SELECTED, (o) =>
            {
                IsPresented = false;
            });
        }

        private async void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as NavigationListItem;
            if (item == null)
                return;

            await ShowPageFromNavigationListItemAsync(item);

            MenuItemsListView.SelectedItem = null;
        }

        private void _selectFile_Tapped(object sender, EventArgs ea)
        {
            IsPresented = false;
        }

        private async System.Threading.Tasks.Task ShowPageFromNavigationListItemAsync(NavigationListItem item)
        {
            var navigation = Detail.Navigation;
            if (item.ResetsNavigation)
            {
                await navigation.PopToRootAsync();
            }
            else
            {
                var page = item.MakePage();
                page.Title = item.Title;

                page.SetValue(NavigationPage.HasBackButtonProperty, false);

                await Detail.Navigation.PushAsync(page);
            }

            IsPresented = false;
        }
    }

    public class MasterViewModel : NavigationViewModelBase
    {
        private Command _selectFileCommand;

        public ICommand SelectFileCommand => _selectFileCommand ?? (_selectFileCommand = new Command(SelectFileAsync));

        public IEnumerable<NavigationListItem> NavigationListItems { get; set; }

        public string CurrentFilePath
        {
            get
            {
                var cruiseDataService = ServiceService?.CruiseDataService;
                if(cruiseDataService != null)
                {
                    var path = cruiseDataService.Path ?? "Open File";

                    return (path.Length > 20) ? "..." + path.Substring(path.Length - 20) : path;
                }
                else
                {
                    return "Open File";
                }
            }
        }

        public MasterViewModel(INavigation navigation) : base(navigation)
        {


            NavigationListItems = new NavigationListItem[]
            {
                    new NavigationListItem {Title = "Cutting Units", MakePage = () => new CuttingUnitListPage(), MakeViewModel = () => new ViewModels.CuttingUnitListViewModel(), ResetsNavigation = true },
                    new NavigationListItem {Title = "Tally", MakePage = () => new UnitTreeTallyPage(), MakeViewModel = () => new UnitTreeTallyViewModel()},
                    new NavigationListItem {Title="Trees", MakePage = ()=> new TreeListPage(), MakeViewModel = ()=> new TreeListViewModel()}
            };

            MessagingCenter.Subscribe<object>(this, Messages.CRUISE_FILE_SELECTED, (o) =>{
                RaisePropertyChanged(nameof(this.CurrentFilePath));
            });
        }

        public override Task InitAsync()
        {
            return null;
        }

        private async void SelectFileAsync(object obj)
        {
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
            //Check path is valid
            if (File.Exists(path) == false)
            {
                Debug.WriteLine($"           Cruise File Not Found {path}");
                return;
            }

            ServiceService.CruiseDataService = new CruiseDataService(path);
            ServiceService.CuttingUnitDataSercie = null;

            MessagingCenter.Send<object>(this, Messages.CRUISE_FILE_SELECTED);
        }
    }

    public class NavigationListItem
    {
        public string Title { get; set; }

        public Func<Page> MakePage { get; set; }

        public Func<ViewModelBase> MakeViewModel { get; set; }

        public bool ResetsNavigation { get; set; } = false;

        public bool IsEnabled { get; set; }
    }
}