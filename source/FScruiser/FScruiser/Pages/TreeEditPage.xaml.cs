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
        public TreeEditPage()
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            var model = BindingContext as TreeEditViewModel;

            var stackLayout = new StackLayout { BindingContext = model.Tree };

            foreach (var field in model.TreeFields)
            {
                stackLayout.Children.Add(MakeCell(field));
            }

            this.Content = new ScrollView { Content = stackLayout };
        }

        protected View MakeCell(TreeField field)
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

            View editControl = new Entry();
            editControl.SetBinding(Entry.TextProperty, field.Field);

            grid.Children.Add(editControl, 1, 0);

            return grid;
        }
    }
}