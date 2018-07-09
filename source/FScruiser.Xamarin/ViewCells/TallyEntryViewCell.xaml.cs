using System.Windows.Input;
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

        protected override void RefreshDrawer(bool isSelected)
        {
            _drawer.IsVisible = isSelected;
        }
    }
}