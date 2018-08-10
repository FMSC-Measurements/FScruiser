using FScruiser.Models;
using System;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FScruiser.XF.ViewCells
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TallyEntryViewCell : TallyEntryViewCell_Base
    {
        public ICommand UntallyCommand
        {
            get { return _untallyButton.Command; }
            set { _untallyButton.Command = value; }
        }

        public object UntallyCommandParameter
        {
            get { return _untallyButton.CommandParameter; }
            set { _untallyButton.CommandParameter = value; }
        }

        public TallyEntryViewCell()
        {
            InitializeComponent();

            _untallyButton.Clicked += _untallyButton_Clicked;
        }

        protected void _untallyButton_Clicked(object sender, EventArgs e)
        {
            MessagingCenter.Send<object, TallyEntry>(this, Messages.UNTALLY_CLICKED, BindingContext as TallyEntry);
        }

        protected override void RefreshDrawer(bool isSelected)
        {
            _drawer.IsVisible = isSelected;
        }
    }
}