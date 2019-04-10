using FScruiser.Models;
using FScruiser.Validation;
using FScruiser.XF.Util;
using FScruiser.XF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FScruiser.XF.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TreeEditPage2 : ContentPage
    {
        private Color _altRowColor;

        #region ErrorsAndWarnings

        /// <summary>
        /// Identifies the <see cref="ErrorsAndWarnings"/> bindable property.
        /// </summary>
        public static readonly BindableProperty ErrorsAndWarningsProperty =
            BindableProperty.Create(nameof(ErrorsAndWarnings),
              typeof(IEnumerable<ValidationError>),
              typeof(TreeEditPage2),
              defaultValue: default(IEnumerable<ValidationError>),
              defaultBindingMode: BindingMode.Default,
              propertyChanged: (bindable, oldValue, newValue) => ((TreeEditPage2)bindable).OnErrorsAndWarningsChanged((IEnumerable<ValidationError>)oldValue, (IEnumerable<ValidationError>)newValue));

        /// <summary>
        /// Invoked after changes have been applied to the <see cref="ErrorsAndWarnings"/> property.
        /// </summary>
        /// <param name="oldValue">The old value of the <see cref="ErrorsAndWarnings"/> property.</param>
        /// <param name="newValue">The new value of the <see cref="ErrorsAndWarnings"/> property.</param>
        protected void OnErrorsAndWarningsChanged(IEnumerable<ValidationError> oldValue, IEnumerable<ValidationError> newValue)
        {
            _errorMessageContainer.Children.Clear();

            if (newValue != null)
            {
                foreach (var error in newValue)
                {
                    //var newRow = new StackLayout
                    //{
                    //    Orientation = StackOrientation.Horizontal,
                    //    BackgroundColor = Color.Red
                    //};

                    var row = new Label { Text = error.Message };

                    switch (error.Level)
                    {
                        case ValidationLevel.Error: { row.BackgroundColor = Color.OrangeRed; break; }
                        case ValidationLevel.Warning: { row.BackgroundColor = Color.Gold; break; }
                        case ValidationLevel.Info: { row.BackgroundColor = Color.DodgerBlue; break; }
                    }

                    _errorMessageContainer.Children.Add(row);
                }
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="ErrorsAndWarnings" /> property. This is a bindable property.
        /// </summary>
        /// <value>
        ///
        /// </value>
        public IEnumerable<ValidationError> ErrorsAndWarnings
        {
            get { return (IEnumerable<ValidationError>)GetValue(ErrorsAndWarningsProperty); }
            set { SetValue(ErrorsAndWarningsProperty, value); }
        }

        #endregion ErrorsAndWarnings

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
                viewModel.TreeFieldsChanged += ViewModel_TreeFieldsChanged;
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if (BindingContext is TreeEditViewModel viewModel)
            {
                viewModel.TreeFieldsChanged -= ViewModel_TreeFieldsChanged;
            }
        }

        private void ViewModel_TreeFieldsChanged(object sender, IEnumerable<TreeFieldSetup> e)
        {
            UpdateTreeFields((TreeEditViewModel)sender);
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

        private View MakeTreeFields(IEnumerable<TreeFieldSetup> treeFields)
        {
            if (treeFields == null) { throw new ArgumentNullException(nameof(treeFields)); }

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
                if(field.Field == "Species")
                { header.Text = "Sp/LD"; }

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