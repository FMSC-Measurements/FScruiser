using FScruiser.Models;
using FScruiser.XF.ViewModels;
using FScruiser.XF.Views;
using System;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FScruiser.XF.ViewCells
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TallyEntryTreeViewCell : ViewCell
    {
        public ICommand EditCommand
        {
            get { return _editButton.Command; }
            set { _editButton.Command = value; }
        }

        public object EditCommandParameter
        {
            get { return _editButton.CommandParameter; }
            set { _editButton.CommandParameter = value; }
        }

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

        public event EventHandler<TallyEntry> EditClicked;

        public event EventHandler<TallyEntry> UntallyClicked;

        public event EventHandler<bool> IsSelectedChanged;

        private bool _isSelected;
        private TreeEditViewModel _treeViewModel;

        public bool IsSelected
        {
            get { return _isSelected; }
            private set
            {
                if (_isSelected == value) { return; }
                _isSelected = value;
                RaiseIsSelectedChanged();
            }
        }

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
                    _treeViewModel.Dispose();
                }
                _treeViewModel = value;
                if (value != null)
                {
                    _treeViewModel.TreeFieldsChanged += _treeViewModel_TreeFieldsChanged;
                }
            }
        }

        private void _treeViewModel_TreeFieldsChanged(object sender, System.Collections.Generic.IEnumerable<CruiseDAL.DataObjects.TreeFieldSetupDO> e)
        {
            var view = MakeEditControlContainer(TreeViewModel.TreeFields);

            view.BindingContextChanged += View_BindingContextChanged;
            _treeEditScrollView.Content = view;
            view.BindingContext = TreeViewModel;
        }

        private void View_BindingContextChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Write("binding context changed");
        }

        private void _untallyButton_Clicked(object sender, EventArgs e)
        {
            MessagingCenter.Send<object, TallyEntry>(this, Messages.UNTALLY_CLICKED, BindingContext as TallyEntry);
        }

        private void _editButton_Clicked(object sender, EventArgs e)
        {
            MessagingCenter.Send<object, TallyEntry>(this, Messages.EDIT_TREE_CLICKED, BindingContext as TallyEntry);
        }

        private void RaiseIsSelectedChanged()
        {
            var isSelected = IsSelected;
            MessagingCenter.Send<object, bool>(this, Messages.TREECELL_ISELECTED_CHANGED, isSelected);

            if (isSelected)
            {
                var tallyItem = BindingContext as TallyEntry;
                if (tallyItem?.HasTree ?? false)
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

            _treeEditPanel.IsVisible = isSelected;

            base.ForceUpdateSize();
            if (isSelected)
            {
                EnsureVisable();
            }
        }

        private static View MakeEditControlContainer(System.Collections.Generic.IEnumerable<CruiseDAL.DataObjects.TreeFieldSetupDO> treeFields)
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

        private static void _entry_Completed(object sender, EventArgs e)
        {
            if (sender != null && sender is View view)
            {
                var layout = (Grid)view.Parent;

                var indexOfChild = layout.Children.IndexOf(view);
                var nextChild = layout.Children.Skip(indexOfChild + 1).Where(x => x is Entry || x is Picker).FirstOrDefault();
                nextChild?.Focus();
            }
        }

        private void RefreshTree()
        {
            var tallyItem = BindingContext as TallyEntry;
            if (tallyItem?.HasTree ?? false)
            {
                TreeViewModel?.Init(tallyItem.Tree_GUID);
            }
        }

        private void EnsureVisable()
        {
            var item = BindingContext;
            if (item == null) { return; }
            var parent = RealParent;
            if (parent != null && parent is ListView listView)
            {
                listView.ScrollTo(item, ScrollToPosition.MakeVisible, true);
            }
        }

        protected override void OnTapped()
        {
            base.OnTapped();

            IsSelected = true;
        }

        //protected override void OnAppearing()
        //{
        //    base.OnAppearing();

        //    RefreshTree();
        //}

        //protected override void OnDisappearing()
        //{
        //    base.OnDisappearing();

        //    IsSelected = false;
        //}

        protected override void OnPropertyChanging(string propertyName = null)
        {
            if (propertyName == nameof(Parent))
            {
                var parent = RealParent;
                if (parent != null && parent is ListView listView)
                {
                    UnwireListView(listView);
                }
            }

            base.OnPropertyChanging(propertyName);
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();

            var parent = RealParent;
            if (parent != null && parent is ListView listView)
            {
                WireListView(listView);
            }
        }

        protected virtual void UnwireListView(ListView listView)
        {
            listView.ItemSelected -= ListView_ItemSelected;

            //if (listView is CustomListView customListView)
            //{
            //    customListView.Scroll -= CustomListView_Scroll;
            //}
        }

        protected virtual void WireListView(ListView listView)
        {
            listView.ItemSelected += ListView_ItemSelected;
            //if (listView is CustomListView customListView)
            //{
            //    customListView.Scroll += CustomListView_Scroll;
            //}
        }

        //private void CustomListView_Scroll(object sender, System.EventArgs e)
        //{
        //    //IsSelected = false;
        //}

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var myItem = BindingContext;
            var selectedItem = e.SelectedItem;

            if (selectedItem == null || object.ReferenceEquals(myItem, selectedItem) == false)
            {
                if (IsSelected) { IsSelected = false; }
            }
        }

        
    }
}