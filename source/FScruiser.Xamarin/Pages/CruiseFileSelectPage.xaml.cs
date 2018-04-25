using FScruiser.XF.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FScruiser.XF.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CruiseFileSelectPage : ContentPage
	{
        protected CruiseFileSelectViewModel ViewModel => BindingContext as CruiseFileSelectViewModel;

		public CruiseFileSelectPage ()
		{
			InitializeComponent ();            
		}

        public void FileListView_ItemSelected(object sender, SelectedItemChangedEventArgs eventArgs)
        {
            if(sender == null) { throw new ArgumentNullException(nameof(sender)); }
            if(eventArgs == null) { throw new ArgumentNullException(nameof(eventArgs)); }

            var selectedFile = (FileInfo)eventArgs.SelectedItem;

            ViewModel.LoadCruiseFile(selectedFile);

            ((ListView)sender).SelectedItem = null;//set selected item to null to make listview item act as button
        }
    }
}