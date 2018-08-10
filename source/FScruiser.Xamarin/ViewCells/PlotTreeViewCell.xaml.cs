using FScruiser.Models;
using FScruiser.XF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FScruiser.XF.ViewCells
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PlotTreeViewCell : TallyEntryViewCell_Base
    {
        private TreeEditViewModel _treeViewModel;

        public PlotTreeViewCell ()
		{
            InitializeComponent();

            _editButton.Clicked += _editButton_Clicked;
            _deleteButton.Clicked += _deleteButton_Clicked;
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
                    _treeViewModel.Dispose();
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

        private void _editButton_Clicked(object sender, EventArgs e)
        {
            TreeViewModel?.SaveTree();
            var tree = BindingContext as TreeStub_Plot;
            MessagingCenter.Send<object, string>(this, Messages.EDIT_TREE_CLICKED, tree?.Tree_GUID);
        }

        protected void _deleteButton_Clicked(object sender, EventArgs e)
        {
            var tree = BindingContext as TreeStub_Plot;
            MessagingCenter.Send<object, string>(this, Messages.DELETE_TREE_CLICKED, tree?.Tree_GUID);
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

        private void _treeViewModel_TreeFieldsChanged(object sender, System.Collections.Generic.IEnumerable<CruiseDAL.DataObjects.TreeFieldSetupDO> e)
        {
            var view = MakeEditControlContainer(TreeViewModel.TreeFields);

            _treeEditScrollView.Content = view;
            view.BindingContext = TreeViewModel;
        }

        protected override void OnIsSelectedChanged(bool isSelected)
        {
            if (isSelected)
            {
                var tree = BindingContext as TreeStub_Plot;
                if (tree != null)
                {
                    var treeEditViewModel = TreeViewModel = new TreeEditViewModel(true);
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

        private void RefreshTree()
        {
            var tree = BindingContext as TreeStub_Plot;
            if (tree != null)
            {
                TreeViewModel?.Init(tree.Tree_GUID);
            }
        }

        protected override void RefreshDrawer(bool isSelected)
        {
            _treeEditPanel.IsVisible = isSelected;
        }

        private View MakeEditControlContainer(System.Collections.Generic.IEnumerable<CruiseDAL.DataObjects.TreeFieldSetupDO> treeFields)
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
    }
}