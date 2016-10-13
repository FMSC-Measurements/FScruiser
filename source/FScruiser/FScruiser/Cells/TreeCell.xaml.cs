using FScruiser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.Cells
{
    public partial class TreeCell : ViewCell
    {
        #region EditCommand

        /// <summary>
        /// Identifies the <see cref="EditCommand"/> bindable property.
        /// </summary>
        public static readonly BindableProperty EditCommandProperty =
            BindableProperty.Create(nameof(EditCommand),
              typeof(ICommand),
              typeof(TreeCell),
              defaultValue: default(ICommand),
              defaultBindingMode: BindingMode.OneWay,
              propertyChanged: (bindable, oldValue, newValue) => ((TreeCell)bindable).OnEditCommandChanged());

        protected void OnEditCommandChanged()
        {
            if (EditCommand != null)
            {
                EditCommand.CanExecuteChanged += EditCommand_CanExecuteChanged;
                EditCommand_CanExecuteChanged(this, EventArgs.Empty);
            }
            else
            { }
        }

        private void Edit_Clicked(object sender, EventArgs e)
        {
            EditCommand?.Execute(EditCommandParameter);
        }

        private void EditCommand_CanExecuteChanged(object sender, EventArgs e)
        {
            Edit.IsEnabled = EditCommand?.CanExecute(EditCommandParameter) ?? false;
        }

        /// <summary>
        /// Gets or sets the <see cref="EditCommand" /> property. This is a bindable property.
        /// </summary>
        /// <value>
        ///
        /// </value>
        public ICommand EditCommand
        {
            get { return (ICommand)GetValue(EditCommandProperty); }
            set { SetValue(EditCommandProperty, value); }
        }

        #endregion EditCommand

        #region DeleteCommand

        /// <summary>
        /// Identifies the <see cref="DeleteCommand"/> bindable property.
        /// </summary>
        public static readonly BindableProperty DeleteCommandProperty =
            BindableProperty.Create(nameof(DeleteCommand),
              typeof(ICommand),
              typeof(TreeCell),
              defaultValue: default(ICommand),
              defaultBindingMode: BindingMode.OneWay,
              propertyChanged: (bindable, oldValue, newValue) => ((TreeCell)bindable).OnDeleteCommandChanged());

        /// <summary>
        /// Invoked before changes are applied to the <see cref="DeleteCommand"/> property.
        /// </summary>
        protected virtual void OnDeleteCommandChanged()
        {
            if (DeleteCommand != null)
            {
                DeleteCommand.CanExecuteChanged += DeleteCommand_CanExecuteChanged;
                DeleteCommand_CanExecuteChanged(this, EventArgs.Empty);
            }
            else
            { }
        }

        protected void DeleteCommand_CanExecuteChanged(object sender, EventArgs e)
        {
            Delete.IsEnabled = DeleteCommand?.CanExecute(DeleteCommandParameter) ?? false;
        }

        private void Delete_Clicked(object sender, EventArgs e)
        {
            DeleteCommand?.Execute(DeleteCommandParameter);
        }

        /// <summary>
        /// Gets or sets the <see cref="DeleteCommand" /> property. This is a bindable property.
        /// </summary>
        /// <value>
        ///
        /// </value>
        public ICommand DeleteCommand
        {
            get { return (ICommand)GetValue(DeleteCommandProperty); }
            set { SetValue(DeleteCommandProperty, value); }
        }

        #endregion DeleteCommand

        #region EditCommandParameter

        /// <summary>
        /// Identifies the <see cref="EditCommandParameterProperty"/> bindable property.
        /// </summary>
        public static readonly BindableProperty EditCommandParameterProperty =
            BindableProperty.Create(nameof(EditCommandParameter), typeof(object), typeof(TreeCell),
              propertyChanged: (bo, o, n) => ((TreeCell)bo).EditCommand_CanExecuteChanged(bo, EventArgs.Empty));

        /// <summary>
        /// Gets or sets the <see cref="EditCommandParameterProperty" /> property. This is a bindable property.
        /// </summary>
        /// <value>
        ///
        /// </value>
        public object EditCommandParameter
        {
            get { return (object)GetValue(EditCommandParameterProperty); }
            set { SetValue(EditCommandParameterProperty, value); }
        }

        #endregion EditCommandParameter

        #region DeleteCommandParameter

        /// <summary>
        /// Identifies the <see cref="DeleteCommandParameter"/> bindable property.
        /// </summary>
        public static readonly BindableProperty DeleteCommandParameterProperty =
            BindableProperty.Create(nameof(DeleteCommandParameter),
              typeof(object),
              typeof(TreeCell),
              defaultValue: default(object),
              defaultBindingMode: BindingMode.OneWay,
              propertyChanged: (bindable, oldValue, newValue) => ((TreeCell)bindable).DeleteCommand_CanExecuteChanged(bindable, EventArgs.Empty));

        /// <summary>
        /// Gets or sets the <see cref="DeleteCommandParameter" /> property. This is a bindable property.
        /// </summary>
        /// <value>
        ///
        /// </value>
        public object DeleteCommandParameter
        {
            get { return (object)GetValue(DeleteCommandParameterProperty); }
            set { SetValue(DeleteCommandParameterProperty, value); }
        }

        #endregion DeleteCommandParameter

        public TreeCell()
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            Edit.CommandParameter = BindingContext;
            Delete.CommandParameter = BindingContext;
        }
    }
}