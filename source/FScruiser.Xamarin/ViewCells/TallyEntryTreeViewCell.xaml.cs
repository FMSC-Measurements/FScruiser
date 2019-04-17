using FScruiser.Models;
using FScruiser.XF.Constants;
using FScruiser.XF.ViewModels;
using Prism.Ioc;
using System;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FScruiser.XF.ViewCells
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TallyEntryTreeViewCell : TallyEntryViewCell_Base
    {
        private TreeEditViewModel _treeViewModel;

        public TallyEntryTreeViewCell()
        {
            InitializeComponent();

            _editButton.Clicked += _editButton_Clicked;
            _untallyButton.Clicked += _untallyButton_Clicked;
        }

        public TreeEditViewModel TreeViewModel
        {
            get { return _treeViewModel; }
            set
            {
                if (_treeViewModel != null)
                {
                    _treeViewModel.TreeFieldsChanged -= _treeViewModel_TreeFieldsChanged;
                    _treeViewModel.ErrorsAndWarningsChanged -= _treeViewModel_ErrorsAndWarningsChanged;
                    _treeViewModel.OnNavigatedFrom(new Prism.Navigation.NavigationParameters());
                }
                _treeViewModel = value;
                if (value != null)
                {
                    _treeViewModel.TreeFieldsChanged += _treeViewModel_TreeFieldsChanged;
                    _treeViewModel.ErrorsAndWarningsChanged += _treeViewModel_ErrorsAndWarningsChanged;
                    _treeViewModel_ErrorsAndWarningsChanged(_treeViewModel, null);
                }
            }
        }

        private void _treeViewModel_ErrorsAndWarningsChanged(object sender, EventArgs e)
        {
            if (sender is TreeEditViewModel viewModel)
            {
                var grid = _treeEditScrollView.Content as Grid;
                if (grid == null) { return; }
                grid.BackgroundColor = (viewModel.ErrorsAndWarnings != null && viewModel.ErrorsAndWarnings.Count() > 0) ? Color.OrangeRed : Color.Transparent;
            }
            
        }

        private void _treeViewModel_TreeFieldsChanged(object sender, System.Collections.Generic.IEnumerable<TreeFieldSetup> e)
        {
            var view = MakeEditControlContainer(TreeViewModel.TreeFields);

            _treeEditScrollView.Content = view;
            view.BindingContext = TreeViewModel;
        }

        private void _editButton_Clicked(object sender, EventArgs e)
        {
            TreeViewModel?.SaveTree();
            if (BindingContext is TallyEntry tallyEntry && tallyEntry != null)
            {
                MessagingCenter.Send<object, string>(this, Messages.EDIT_TREE_CLICKED, tallyEntry.TreeID);
            }
        }

        protected void _untallyButton_Clicked(object sender, EventArgs e)
        {
            MessagingCenter.Send<object, TallyEntry>(this, Messages.UNTALLY_CLICKED, BindingContext as TallyEntry);
        }

        protected override void OnIsSelectedChanged(bool isSelected)
        {
            if (isSelected)
            {
                var tallyItem = BindingContext as TallyEntry;
                if (tallyItem?.TreeID != null)
                {
                    var container = ((App)App.Current).Container;

                    var treeEditViewModel = container.Resolve<TreeEditViewModel>();
                    treeEditViewModel.UseSimplifiedTreeFields = true;

                    TreeViewModel = treeEditViewModel;
                    RefreshTree();
                }
            }
            else
            {
                TreeViewModel?.SaveTree();

                TreeViewModel = null;
                _treeEditScrollView.Content = null;
            }

            base.OnIsSelectedChanged(isSelected);
        }

        protected override void RefreshDrawer(bool isSelected)
        {
            _treeEditPanel.IsVisible = isSelected;
            base.ForceUpdateSize();
        }

        private View MakeEditControlContainer(System.Collections.Generic.IEnumerable<TreeFieldSetup> treeFields)
        {
            var grid = new Grid();

            int counter = 0;
            foreach (var field in treeFields)
            {
                var fieldLabel = new Label() { Text = field.Heading };

                grid.Children.Add(fieldLabel, counter, 0);

                var editControl = Util.TreeEditControlFactory.MakeEditView(field);

                if (editControl is Entry entry)
                {
                    entry.Completed += _entry_Completed;
                }

                grid.Children.Add(editControl, counter, 1);
                counter++;
            }

            return grid;
        }

        private void _entry_Completed(object sender, EventArgs e)
        {
            if (sender != null && sender is View view)
            {
                var layout = (Grid)view.Parent;

                var indexOfChild = layout.Children.IndexOf(view);
                var nextChild = layout.Children.Skip(indexOfChild + 1).Where(x => x is Entry || x is Picker).FirstOrDefault();
                nextChild?.Focus();
            }

            TreeViewModel?.SaveTree();
        }

        private void RefreshTree()
        {
            var tallyEntry = BindingContext as TallyEntry;
            if (tallyEntry?.TreeID != null)
            {
                TreeViewModel?.OnNavigatedTo(new Prism.Navigation.NavigationParameters() { { NavParams.TreeID, tallyEntry.TreeID } });
            }
        }
    }
}