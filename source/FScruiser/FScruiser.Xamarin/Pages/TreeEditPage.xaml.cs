using FScruiser.Models;
using FScruiser.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace FScruiser.Pages
{
    public partial class TreeEditPage : ContentPage
    {
        private Picker _speciesPicker;
        private bool _speciesPickerUpdating;
        private bool _sampleGroupPickerUpdating;
        private Picker _sampleGroupPicker;
        private TreeEditViewModel _viewModel;

        public TreeEditPage()
        {
            InitializeComponent();
        }

        public TreeEditViewModel ViewModel
        {
            get { return _viewModel; }
            set
            {
                if (_viewModel == value) { return; }
                if (_viewModel != null) { UnwireViewModel(_viewModel); }
                _viewModel = value;
                if (_viewModel != null) { WireUpViewModel(_viewModel); }
            }
        }

        private void WireUpViewModel(TreeEditViewModel viewModel)
        {
            if (viewModel.TreeFields != null)
            {
                UpdateTreeFields(_viewModel);
            }

            viewModel.PropertyChanged += ViewModel_PropertyChanged;
            viewModel.TreeChanging += UnwireTree_PropertyChanged;
            viewModel.TreeChanged += WireupTree_PropertyChanged;
        }

        private void UnwireViewModel(TreeEditViewModel viewModel)
        {
            viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            viewModel.TreeChanging -= UnwireTree_PropertyChanged;
            viewModel.TreeChanged -= WireupTree_PropertyChanged;
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            ViewModel = BindingContext as TreeEditViewModel;
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TreeEditViewModel.Stratum))
            {
                UpdateTreeFields(ViewModel);
            }
        }

        protected void UpdateTreeFields(TreeEditViewModel viewModel)
        {
            if (ViewModel != null)
            {
                StackLayout stackLayout = MakeTreeFields(viewModel.TreeFields);
                this.Content = new ScrollView { Content = stackLayout };
            }
        }

        private StackLayout MakeTreeFields(IEnumerable<TreeField> treeFields)
        {
            _speciesPicker = null;
            _sampleGroupPicker = null;
            var stackLayout = new StackLayout();
            stackLayout.SetBinding(BindingContextProperty, nameof(TreeEditViewModel.Tree));
            foreach (var field in treeFields)
            {
                var cell = MakeEditView(field);
                stackLayout.Children.Add(cell);
            }
            return stackLayout;
        }

        protected View MakeEditView(TreeField field)
        {
            var grid = new Grid()
            {
                RowDefinitions =
                {
                    new RowDefinition {Height = GridLength.Auto }
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition {Width = GridLength.Auto },
                    new ColumnDefinition {Width = GridLength.Star }
                }
            };

            grid.Children.Add(new Label { Text = field.Heading }, 0, 0);

            View editView = null;
            switch (field.Field)
            {
                case nameof(Tree.SampleGroup):
                    {
                        editView = _sampleGroupPicker = MakeSampleGroupPicker();
                        break;
                    }
                case nameof(Tree.Species):
                    {
                        editView = _speciesPicker = MakeSpeciesPicker();
                        break;
                    }
                default:
                    {
                        editView = new Entry();
                        editView.SetBinding(Entry.TextProperty, field.Field);
                        break;
                    }
            }

            grid.Children.Add(editView, 1, 0);

            return grid;
        }

        #region Sg Picker

        Picker MakeSampleGroupPicker()
        {
            var view = new Picker();

            UpdateSampleGroupPicker(view);

            view.SelectedIndexChanged += SampleGroupPicker_SelectedIndexChanged;

            return view;
        }

        private void SampleGroupPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_sampleGroupPickerUpdating) { return; }
            var picker = sender as Picker;
            if (picker == null) { return; }
            var index = picker.SelectedIndex;
            if (index < 0 || index >= ViewModel.SampleGroups.Count()) { return; }
            var sampleGroup = ViewModel.SampleGroups.ElementAt(index);
            ViewModel.Tree.SampleGroup = sampleGroup;
            UpdateSpeciesPicker(_speciesPicker);
        }

        private void UpdateSampleGroupPicker(Picker view)
        {
            if (view == null) { return; }
            try
            {
                _sampleGroupPickerUpdating = true;
                if (view.Items.Count > 0)
                { view.Items.Clear(); }
                view.Items.Add(string.Empty);
                foreach (var sg in ViewModel.SampleGroups)
                {
                    view.Items.Add(sg.Code);
                }

                var index = view.Items.IndexOf(ViewModel.Tree?.SampleGroup.Code);
                view.SelectedIndex = (index > 0) ? index : 0;
            }
            finally
            {
                _sampleGroupPickerUpdating = false;
            }
        }

        #endregion Sg Picker

        #region Sp Picker

        Picker MakeSpeciesPicker()
        {
            var view = new Picker();

            UpdateSpeciesPicker(view);

            view.SelectedIndexChanged += SpeciesPicker_SelectedIndexChanged;
            return view;
        }

        private void SpeciesPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_speciesPickerUpdating) { return; }
            var picker = sender as Picker;
            if (picker == null) { return; }

            var index = picker.SelectedIndex;
            var selectedValue = picker.Items[index];
            var tdv = ViewModel.TreeDefaults.FirstOrDefault(x => x.Species == selectedValue);
            ViewModel.Tree.TreeDefaultValue = tdv;
        }

        private void UpdateSpeciesPicker(Picker view)
        {
            if (view == null) { return; }
            try
            {
                _speciesPickerUpdating = true;
                if (view.Items.Count > 0) { view.Items.Clear(); }

                view.Items.Add(string.Empty);//default empty option at index 0
                foreach (var sp in ViewModel.TreeDefaults)
                {
                    view.Items.Add(sp.Species);
                }

                //set selected index
                var index = view.Items.IndexOf(ViewModel.Tree?.TreeDefaultValue?.Species);
                view.SelectedIndex = (index > 0) ? index : 0;
            }
            finally
            {
                _speciesPickerUpdating = false;
            }
        }

        #endregion Sp Picker

        #region Tree PropertyChyanged

        private void UnwireTree_PropertyChanged(object sender, Tree e)
        {
            if (e != null)
            {
                e.PropertyChanged -= Tree_PropertyChanged;
            }
        }

        private void WireupTree_PropertyChanged(object sender, Tree e)
        {
            if (e != null)
            {
                e.PropertyChanged += Tree_PropertyChanged;
            }
        }

        private void Tree_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Tree.SampleGroup):
                    {
                        UpdateSpeciesPicker(_speciesPicker);
                        break;
                    }
            }
        }

        #endregion Tree PropertyChyanged
    }
}