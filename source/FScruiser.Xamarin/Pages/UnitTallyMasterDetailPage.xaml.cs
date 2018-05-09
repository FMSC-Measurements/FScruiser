using FScruiser.Services;
using FScruiser.XF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FScruiser.XF.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UnitTallyMasterDetailPage : MasterDetailPage
    {
        public ServiceService ServiceService { get; private set; }

        public UnitTallyMasterDetailPage()
        {
            InitializeComponent();

            MenuItemsListView.ItemSelected += ListView_ItemSelected;
        }

        public UnitTallyMasterDetailPage(ServiceService serviceService) : this()
        {
            ServiceService = serviceService;

            var viewModel = new MasterViewModel(serviceService, Navigation);
            MasterPage.BindingContext = viewModel;

            ShowPageFromNavigationListItem(viewModel.NavigationListItems.First());

            //var unitTallyViewModel = new UnitTreeTallyViewModel(Navigation, App.ServiceService, dataService);
            //var unitTallyPage = new UnitTreeTallyPage();
            //unitTallyPage.BindingContext = unitTallyViewModel;
            //unitTallyViewModel.Init();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as NavigationListItem;
            if (item == null)
                return;

            ShowPageFromNavigationListItem(item);

            MenuItemsListView.SelectedItem = null;
        }

        private void ShowPageFromNavigationListItem(NavigationListItem item)
        {
            var page = item.MakePage();
            page.Title = item.Title;

            var viewModel = item.MakeViewModel();
            viewModel.Init();
            page.BindingContext = viewModel;

            Detail = page;
            IsPresented = false;
        }

        private class MasterViewModel : ViewModelBase
        {
            public MasterViewModel(ServiceService serviceService, INavigation navigation) : base(navigation)
            {
                ServiceService = serviceService;
                NavigationListItems = new NavigationListItem[]
                {
                    new NavigationListItem {Title = "Cutting Units", MakePage = () => new CuttingUnitListPage(), MakeViewModel = () => new CuttingUnitListViewModel(ServiceService, navigation) },
                    new NavigationListItem {Title = "Tally", MakePage = () => new UnitTreeTallyPage(), MakeViewModel = () => new UnitTreeTallyViewModel(ServiceService, navigation)},
                    new NavigationListItem {Title="Trees", MakePage = ()=> new TreeListPage(), MakeViewModel = ()=> new TreeListViewModel(serviceService)}
                };
            }

            public ServiceService ServiceService { get; private set; }
            public IEnumerable<NavigationListItem> NavigationListItems { get; set; }

            public override void Init()
            {
            }
        }

        private class NavigationListItem
        {
            public string Title { get; set; }

            public Func<Page> MakePage { get; set; }

            public Func<ViewModelBase> MakeViewModel { get; set; }
        }
    }
}