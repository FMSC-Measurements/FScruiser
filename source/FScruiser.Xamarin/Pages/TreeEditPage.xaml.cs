using CruiseDAL.DataObjects;
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
        private bool _countMeasurePickerUpdating;
        private Picker _countMeasurePicker;

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

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            ViewModel = BindingContext as TreeEditViewModel;
        }

        private void WireUpViewModel(TreeEditViewModel viewModel)
        {
            if (viewModel.TreeFields != null)
            {
                UpdateTreeFields(_viewModel);
            }

            viewModel.PropertyChanged += ViewModel_PropertyChanged;
            viewModel.TreeChanging += ViewModel_TreeChanging;
            viewModel.TreeChanged += ViewModel_TreeChanged;
        }

        private void UnwireViewModel(TreeEditViewModel viewModel)
        {
            viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            viewModel.TreeChanging -= ViewModel_TreeChanging;
            viewModel.TreeChanged -= ViewModel_TreeChanged;
        }

        private void ViewModel_TreeChanged(object sender, Tree e)
        {
            UpdateCountMeasurePicker(_countMeasurePicker);
            UpdateSampleGroupPicker(_sampleGroupPicker);
            UpdateSpeciesPicker(_speciesPicker);
            WireupTreePropertyChanged(e);
        }

        private void ViewModel_TreeChanging(object sender, Tree e)
        {
            UnwireTreePropertyChanged(e);
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TreeEditViewModel.Stratum))
            {
                UpdateTreeFields(ViewModel);
            }
        }

        protected async void UpdateTreeFields(TreeEditViewModel viewModel)
        {
            if (ViewModel != null)
            {
                var view = await MakeTreeFields(viewModel.TreeFields);
                this.Content = new ScrollView { Content = view };
            }
        }

        private Task<View> MakeTreeFields(IEnumerable<TreeFieldSetupDO> treeFields)
        {
            _speciesPicker = null;
            _sampleGroupPicker = null;
            _countMeasurePicker = null;
            return Task.Run(() =>
            {
                var containerView = new Grid();
                containerView.ColumnDefinitions.Add(new ColumnDefinition() { Width = 50 });
                containerView.ColumnDefinitions.Add(new ColumnDefinition() { Width = 100 });
                containerView.SetBinding(BindingContextProperty, nameof(TreeEditViewModel.Tree));
                var index = 0;
                foreach (var field in treeFields)
                {
                    containerView.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                    var header = new Label() { Text = field.Heading };
                    containerView.Children.Add(header, 0, index);

                    containerView.Children.Add(MakeEditView(field), 1, index);
                    index++;
                }
                return (View)containerView;
            });
        }

        protected View MakeEditView(TreeField field)
        {
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
                case nameof(Tree.CountOrMeasure):
                    {
                        editView = _countMeasurePicker = MakeCountMeasurePicker();
                        break;
                    }
                case nameof(Tree.Aspect):
                case nameof(Tree.CrownRatio):
                case nameof(Tree.DBH):
                case nameof(Tree.DBHDoubleBarkThickness):
                case nameof(Tree.DefectCode):
                case nameof(Tree.Diameter):
                case nameof(Tree.DiameterAtDefect):
                case nameof(Tree.DRC):
                case nameof(Tree.FormClass):
                case nameof(Tree.Height):
                case nameof(Tree.HeightToFirstLiveLimb):
                case nameof(Tree.KPI)://int
                case nameof(Tree.MerchHeightPrimary):
                case nameof(Tree.MerchHeightSecondary):
                case nameof(Tree.PoleLength):
                case nameof(Tree.RecoverablePrimary):
                case nameof(Tree.SeenDefectPrimary):
                case nameof(Tree.SeenDefectSecondary):
                case nameof(Tree.Slope):
                case nameof(Tree.TopDIBPrimary):
                case nameof(Tree.TopDIBSecondary):
                case nameof(Tree.TotalHeight):
                case nameof(Tree.TreeCount):
                case nameof(Tree.TreeNumber):
                case nameof(Tree.UpperStemDOB):
                case nameof(Tree.UpperStemHeight):
                case nameof(Tree.VoidPercent):
                    {
                        editView = new Entry();
                        ((InputView)editView).Keyboard = Keyboard.Numeric;
                        editView.SetBinding(Entry.TextProperty, field.Field);
                        break;
                    }
                default:
                    {
                        editView = new Entry();
                        editView.SetBinding(Entry.TextProperty, field.Field);
                        break;
                    }
            }

            return editView;
        }

        #region CM picker

        private Picker MakeCountMeasurePicker()
        {
            var picker = new Picker();
            picker.Items.Add(string.Empty);
            picker.Items.Add("C");
            picker.Items.Add("M");
            UpdateCountMeasurePicker(picker);
            picker.SelectedIndexChanged += CountMeasurePicker_SelectedIndexChanged;

            return picker;
        }

        private void UpdateCountMeasurePicker(Picker picker)
        {
            if (picker == null) { return; }
            try
            {
                _countMeasurePickerUpdating = true;
                var index = picker.Items.IndexOf(ViewModel?.Tree?.CountOrMeasure);
                picker.SelectedIndex = (index > 0) ? index : 0;
            }
            finally
            {
                _countMeasurePickerUpdating = false;
            }
        }

        private void CountMeasurePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_countMeasurePickerUpdating == true) { return; }
            var picker = sender as Picker;
            if (picker == null) { return; }
            var index = picker.SelectedIndex;
            var value = picker.Items.ElementAtOrDefault(index) ?? string.Empty;
            var tree = picker.BindingContext as Tree;
            if (tree == null) { return; }
            tree.CountOrMeasure = value;
        }

        #endregion CM picker

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
            //get picker value
            var index = picker.SelectedIndex;
            var selectedValue = picker.Items.ElementAtOrDefault(index);

            //find samplegroup that matches value or default to null
            var sampleGroup = (string.IsNullOrEmpty(selectedValue)) ? (SampleGroup)null
                : ViewModel.SampleGroups.FirstOrDefault((sg) => sg.Code == selectedValue);

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

                var sgCode = ViewModel.Tree?.SampleGroup?.Code ?? String.Empty;
                var index = view.Items.IndexOf(sgCode);
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
            var selectedValue = picker.Items.ElementAtOrDefault(index) ?? String.Empty;
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
                var spCode = ViewModel.Tree?.TreeDefaultValue?.Species ?? String.Empty;
                var index = view.Items.IndexOf(spCode);
                view.SelectedIndex = (index > 0) ? index : 0;
            }
            finally
            {
                _speciesPickerUpdating = false;
            }
        }

        #endregion Sp Picker

        #region Tree PropertyChanged

        private void UnwireTreePropertyChanged(Tree e)
        {
            if (e != null)
            {
                e.PropertyChanged -= Tree_PropertyChanged;
            }
        }

        private void WireupTreePropertyChanged(Tree e)
        {
            if (e != null)
            {
                e.PropertyChanged += Tree_PropertyChanged;
            }
        }

        private void Tree_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //switch (e.PropertyName)
            //{
            //    case nameof(Tree.SampleGroup):
            //        {
            //            UpdateSampleGroupPicker(_sampleGroupPicker);
            //            break;
            //        }
            //    case nameof(Tree.TreeDefaultValue):
            //        {
            //            UpdateSpeciesPicker(_speciesPicker);
            //            break;
            //        }
            //    case nameof(Tree.CountOrMeasure):
            //        {
            //            UpdateCountMeasurePicker(_countMeasurePicker);
            //            break;
            //        }
            //}
        }

        #endregion Tree PropertyChanged
    }
}