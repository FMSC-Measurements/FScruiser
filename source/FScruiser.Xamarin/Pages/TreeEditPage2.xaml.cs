using CruiseDAL.DataObjects;
using FScruiser.Models;
using FScruiser.XF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FScruiser.XF.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TreeEditPage2 : ContentPage
	{
		public TreeEditPage2 ()
		{
			InitializeComponent ();
		}

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is TreeEditViewModel viewModel)
            {
                UpdateTreeFields(viewModel);
            }
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
                var containerView = new Grid();
                containerView.ColumnDefinitions.Add(new ColumnDefinition() { Width = 50 });
                containerView.ColumnDefinitions.Add(new ColumnDefinition() { Width = 100 });
                //containerView.SetBinding(BindingContextProperty, nameof(TreeEditViewModel.Tree));
                var index = 0;
                foreach (var field in treeFields)
                {
                    containerView.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                    var header = new Label() { Text = field.Heading };
                    containerView.Children.Add(header, 0, index);

                    var editView = MakeEditView(field);
                    if (index % 2 == 0)
                    {
                        editView.BackgroundColor = Color.LightGray;
                        }
                    containerView.Children.Add(editView, 1, index);
                    index++;
                }
                return (View)containerView;
            });
        }

        protected View MakeEditView(TreeFieldSetupDO field)
        {
            View editView = null;
            switch (field.Field)
            {
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
                        editView.SetBinding(Entry.TextProperty, $"Tree.{field.Field}");
                        break;
                    }
                case nameof(Tree.DefectCode):
                    {
                        editView = new Entry();
                        ((InputView)editView).Keyboard = Keyboard.Default;
                        editView.SetBinding(Entry.TextProperty, $"Tree.{field.Field}");
                        break;
                    }
                default:
                    {
                        editView = new Entry();
                        editView.SetBinding(Entry.TextProperty, $"Tree.{field.Field}");
                        break;
                    }
            }

            return editView;
        }

        Picker MakeCountMeasurePicker()
        {
            var picker = new Picker();
            picker.Items.Add(string.Empty);
            picker.Items.Add("C");
            picker.Items.Add("M");
            picker.SetBinding(Picker.SelectedItemProperty, $"Tree.{nameof(Tree.CountOrMeasure)}");

            return picker;
        }

        Picker MakeSampleGroupPicker()
        {
            var view = new Picker();
            view.SetBinding(Picker.ItemsSourceProperty, nameof(TreeEditViewModel.SampleGroups));
            view.SetBinding(Picker.SelectedItemProperty, $"Tree.{nameof(Tree.SampleGroup)}");
            view.ItemDisplayBinding = new Binding(nameof(SampleGroup.Code));

            return view;
        }

        Picker MakeSpeciesPicker()
        {
            var view = new Picker();
            view.SetBinding(Picker.ItemsSourceProperty, nameof(TreeEditViewModel.TreeDefaults));
            view.SetBinding(Picker.SelectedItemProperty, $"Tree.{nameof(Tree.TreeDefaultValue)}");
            view.ItemDisplayBinding = new Binding(nameof(TreeDefaultValueDO.Species));

            return view;
        }
    }
}