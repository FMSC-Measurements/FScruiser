using CruiseDAL.DataObjects;
using FScruiser.XF.Util;
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

        protected void UpdateTreeFields(TreeEditViewModel viewModel)
        {
            if (viewModel != null)
            {
                var view = MakeTreeFields(viewModel.TreeFields);
                _editViewsHost.Content = view;

                //this.Content = new ScrollView { Content = view };
            }
        }

        private View MakeTreeFields(IEnumerable<TreeFieldSetupDO> treeFields)
        {
            if(treeFields == null) { throw new ArgumentNullException(nameof(treeFields)); }

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = 50 });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = 100 });
            //containerView.SetBinding(BindingContextProperty, nameof(TreeEditViewModel.Tree));
            var index = 0;
            foreach (var field in treeFields)
            {
                grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                if (index % 2 == 0) //alternate row color
                {
                    grid.Children.Add(new BoxView { Color = _altRowColor }, 0, 2, index, index + 1);
                }

                var header = new Label() { Text = field.Heading };
                grid.Children.Add(header, 0, index);

                var editView = TreeEditControlFactory.MakeEditView(field);
                if (editView is Entry entry)
                {
                    entry.Completed += _entry_Completed;
                }

                grid.Children.Add(editView, 1, index);
                index++;
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
    }
}