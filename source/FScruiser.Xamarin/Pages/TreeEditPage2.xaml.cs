using CruiseDAL.DataObjects;
using FScruiser.Models;
using FScruiser.XF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FScruiser.XF.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TreeEditPage2 : ContentPage
    {
        private Color _altRowColor;
        private Grid _grid;

        public TreeEditPage2()
        {
            InitializeComponent();

            _altRowColor = (Color)App.Current.Resources["black_12"];
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is TreeEditViewModel viewModel)
            {
                UpdateTreeFields(viewModel);
            }
        }

        protected override void OnDisappearing()
        {
            if (BindingContext is TreeEditViewModel viewModel)
            {
                viewModel.SaveTree();
            }
            base.OnDisappearing();
        }

        protected async void UpdateTreeFields(TreeEditViewModel viewModel)
        {
            if (viewModel != null)
            {
                var view = await MakeTreeFields(viewModel.TreeFields);
                _editViewsHost.Content = view;

                //this.Content = new ScrollView { Content = view };
            }
        }

        private Task<View> MakeTreeFields(IEnumerable<TreeFieldSetupDO> treeFields)
        {
            return Task.Run(() =>
            {
                

                _grid = new Grid();
                _grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = 50 });
                _grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = 100 });
                //containerView.SetBinding(BindingContextProperty, nameof(TreeEditViewModel.Tree));
                var index = 0;
                foreach (var field in treeFields)
                {
                    _grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                    if (index % 2 == 0) //alternate row color
                    {
                        _grid.Children.Add(new BoxView { Color = _altRowColor }, 0, 2, index, index + 1);
                    }

                    var header = new Label() { Text = field.Heading };
                    _grid.Children.Add(header, 0, index);

                    var editView = MakeEditView(field);

                    _grid.Children.Add(editView, 1, index);
                    index++;
                }
                return (View)_grid;
            });
        }

        protected View MakeEditView(TreeFieldSetupDO field)
        {
            View editView = null;
            switch (field.Field)
            {
                case nameof(Tree.Stratum):
                    {
                        editView = MakeStratumPicker();
                        break;
                    }
                case nameof(Tree.SampleGroup):
                    {
                        editView = MakeSampleGroupPicker();
                        break;
                    }
                case nameof(Tree.Species):
                    {
                        editView = MakeSpeciesPicker();
                        break;
                    }
                case nameof(Tree.CountOrMeasure):
                    {
                        editView = MakeCountMeasurePicker();
                        break;
                    }
                case nameof(Tree.Aspect):
                case nameof(Tree.CrownRatio):
                case nameof(Tree.DBH):
                case nameof(Tree.DBHDoubleBarkThickness):
                //case nameof(Tree.Diameter):
                case nameof(Tree.DiameterAtDefect):
                case nameof(Tree.DRC):
                case nameof(Tree.FormClass):
                //case nameof(Tree.Height):
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
                case nameof(Tree.UpperStemDiameter):
                case nameof(Tree.UpperStemHeight):
                case nameof(Tree.VoidPercent):
                    {
                        editView = new Entry();
                        ((InputView)editView).Keyboard = Keyboard.Numeric;
                        ((Entry)editView).Completed += _entry_Completed;
                        Xamarin.Forms.PlatformConfiguration.AndroidSpecific.Entry.SetImeOptions(editView, Xamarin.Forms.PlatformConfiguration.AndroidSpecific.ImeFlags.Next);
                        editView.SetBinding(Entry.TextProperty, $"Tree.{field.Field}");
                        break;
                    }
                case nameof(Tree.DefectCode):
                    {
                        editView = new Entry();
                        ((InputView)editView).Keyboard = Keyboard.Default;
                        ((Entry)editView).Completed += _entry_Completed;
                        Xamarin.Forms.PlatformConfiguration.AndroidSpecific.Entry.SetImeOptions(editView, Xamarin.Forms.PlatformConfiguration.AndroidSpecific.ImeFlags.Next);
                        editView.SetBinding(Entry.TextProperty, $"Tree.{field.Field}");
                        break;
                    }
                default:
                    {
                        editView = new Entry();
                        ((Entry)editView).Completed += _entry_Completed;
                        Xamarin.Forms.PlatformConfiguration.AndroidSpecific.Entry.SetImeOptions(editView, Xamarin.Forms.PlatformConfiguration.AndroidSpecific.ImeFlags.Next);
                        editView.SetBinding(Entry.TextProperty, $"Tree.{field.Field}");
                        break;
                    }
            }

            
            return editView;
        }

        private void _entry_Completed(object sender, EventArgs e)
        {
            if(sender != null && sender is View view)
            {
                var indexOfChild = _grid.Children.IndexOf(view);
                var nextChild = _grid.Children.Skip(indexOfChild+1).SkipWhile(x => x is Label || x is BoxView).FirstOrDefault();
                nextChild?.Focus();
            }
        }

        private View MakeStratumPicker()
        {
            var view = new Picker();
            view.SetBinding(Picker.ItemsSourceProperty, nameof(TreeEditViewModel.Strata));
            view.SetBinding(Picker.SelectedItemProperty, nameof(TreeEditViewModel.Stratum));
            view.ItemDisplayBinding = new Binding(nameof(Stratum.Code));

            return view;
        }

        private Picker MakeCountMeasurePicker()
        {
            var picker = new Picker();
            picker.Items.Add(string.Empty);
            picker.Items.Add("C");
            picker.Items.Add("M");
            picker.SetBinding(Picker.SelectedItemProperty, $"Tree.{nameof(Tree.CountOrMeasure)}");

            return picker;
        }

        private Picker MakeSampleGroupPicker()
        {
            var view = new Picker();
            view.SetBinding(Picker.ItemsSourceProperty, nameof(TreeEditViewModel.SampleGroups));
            view.SetBinding(Picker.SelectedItemProperty, nameof(TreeEditViewModel.SampleGroup));
            view.ItemDisplayBinding = new Binding(nameof(SampleGroup.Code));

            return view;
        }

        private Picker MakeSpeciesPicker()
        {
            var view = new Picker();
            view.SetBinding(Picker.ItemsSourceProperty, nameof(TreeEditViewModel.TreeDefaults));
            view.SetBinding(Picker.SelectedItemProperty, nameof(TreeEditViewModel.TreeDefaultValue));
            view.ItemDisplayBinding = new Binding(nameof(TreeDefaultValueDO.Species));

            return view;
        }
    }
}