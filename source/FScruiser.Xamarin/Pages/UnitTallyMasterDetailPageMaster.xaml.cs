using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FScruiser.XF.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UnitTallyMasterDetailPageMaster : ContentPage
    {
        public ListView ListView;

        public UnitTallyMasterDetailPageMaster()
        {
            InitializeComponent();

            BindingContext = new UnitTallyMasterDetailPageMasterViewModel();
            ListView = MenuItemsListView;
        }

        class UnitTallyMasterDetailPageMasterViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<UnitTallyMasterDetailPageMenuItem> MenuItems { get; set; }
            
            public UnitTallyMasterDetailPageMasterViewModel()
            {
                MenuItems = new ObservableCollection<UnitTallyMasterDetailPageMenuItem>(new[]
                {
                    new UnitTallyMasterDetailPageMenuItem { Id = 0, Title = "Page 1" },
                    new UnitTallyMasterDetailPageMenuItem { Id = 1, Title = "Page 2" },
                    new UnitTallyMasterDetailPageMenuItem { Id = 2, Title = "Page 3" },
                    new UnitTallyMasterDetailPageMenuItem { Id = 3, Title = "Page 4" },
                    new UnitTallyMasterDetailPageMenuItem { Id = 4, Title = "Page 5" },
                });
            }
            
            #region INotifyPropertyChanged Implementation
            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                if (PropertyChanged == null)
                    return;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion
        }
    }
}