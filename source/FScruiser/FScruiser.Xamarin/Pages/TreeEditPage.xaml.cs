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

        public TreeEditPage()
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            UpdateTreeFields();
            ViewModel.TreeChanging += ViewModel_TreeChanging;
            ViewModel.TreeChanged += ViewModel_TreeChanged;

            base.OnBindingContextChanged();
        }

        public TreeEditViewModel ViewModel => BindingContext as TreeEditViewModel;

        protected void UpdateTreeFields()
        {
            var viewModel = ViewModel;
            if (ViewModel != null)
            {
                var stackLayout = new StackLayout();

                foreach (var field in viewModel.TreeFields)
                {
                    var cell = MakeEditView(field);
                    cell.BindingContext = viewModel.Tree;
                    stackLayout.Children.Add(cell);
                }

                this.Content = new ScrollView { Content = stackLayout };
            }
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

            View editView = null;

            switch (field.Field)
            {
                case nameof(Tree.SampleGroup):
                    {
                        break;
                    }
                default:
                    {
                        editView = new Label { Text = field.Heading };
                        break;
                    }
            }

            grid.Children.Add(new Label { Text = field.Heading }, 0, 0);

            View editControl = new Entry();
            editControl.SetBinding(Entry.TextProperty, field.Field);

            grid.Children.Add(editControl, 1, 0);

            return grid;
        }

        View MakeSampleGroupEditView()
        {
            var view = new Picker();
            view.Items.Add(null);
            foreach (var sg in ViewModel.Stratum.SampleGroups)
            {
                view.Items.Add(sg.Code);
            }

            view.SelectedIndex = view.Items.IndexOf(ViewModel.Tree?.SampleGroup.Code);
            view.SelectedIndexChanged += SampleGroupPicker_SelectedIndexChanged;

            return view;
        }

        View MakeSpeciesPicker()
        {
            var view = new Picker();

            UpdateSpeciesPicker(view);

            view.SelectedIndexChanged += SpeciesPicker_SelectedIndexChanged;
            return view;
        }

        private void SpeciesPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = sender as Picker;
            if (picker == null) { return; }

            var sampleGroup = ViewModel.Tree.SampleGroup;
            if (sampleGroup != null)
            {
                var index = picker.SelectedIndex;
                var selectedValue = picker.Items[index];
                var tdv = sampleGroup.TreeDefaults.Find(x => x.Species == selectedValue);
                ViewModel.Tree.TreeDefaultValue = tdv;
            }
            else
            {
                ViewModel.Tree.TreeDefaultValue = null;
            }
        }

        private void UpdateSpeciesPicker(Picker view)
        {
            if (view.Items.Count > 0) { view.Items.Clear(); }

            view.Items.Add(null);
            var sampleGroup = ViewModel.Tree?.SampleGroup;
            if (sampleGroup != null)
            {
                foreach (var sp in ViewModel.Tree.SampleGroup.TreeDefaults)
                {
                    view.Items.Add(sp.Species);
                }
            }
        }

        private void SampleGroupPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = sender as Picker;
            if (picker == null) { return; }
            var index = picker.SelectedIndex;
            if (index < 0 || index >= ViewModel.Stratum.SampleGroups.Count) { return; }
            var sampleGroup = ViewModel.Stratum.SampleGroups[index];
            ViewModel.Tree.SampleGroup = sampleGroup;
            UpdateSpeciesPicker(_speciesPicker);
        }

        private void ViewModel_TreeChanging(object sender, Tree e)
        {
            if (e != null)
            {
                e.PropertyChanged -= Tree_PropertyChanged;
            }
        }

        private void ViewModel_TreeChanged(object sender, Tree e)
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
    }
}