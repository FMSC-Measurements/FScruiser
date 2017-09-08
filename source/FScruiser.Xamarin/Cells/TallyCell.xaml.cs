using FScruiser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.XF.Cells
{
    public partial class TallyCell : ViewCell
    {
        #region Command

        public static readonly BindableProperty CommandProperty = BindableProperty.Create("Command", typeof(ICommand), typeof(TallyCell), default(ICommand),
            propertyChanging: (bindable, oldvalue, newvalue) =>
            {
                var cell = (TallyCell)bindable;
                var oldcommand = (ICommand)oldvalue;
                if (oldcommand != null)
                    oldcommand.CanExecuteChanged -= cell.OnCommandCanExecuteChanged;
            }, propertyChanged: (bindable, oldvalue, newvalue) =>
            {
                var cell = (TallyCell)bindable;
                var newcommand = (ICommand)newvalue;
                if (newcommand != null)
                {
                    newcommand.CanExecuteChanged += cell.OnCommandCanExecuteChanged;
                    cell.OnCommandCanExecuteChanged(cell, EventArgs.Empty);
                }
            });

        private void OnCommandCanExecuteChanged(object sender, EventArgs e)
        {
            IsEnabled = Command?.CanExecute(CommandParameter) ?? false;
        }

        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create("CommandParameter", typeof(object), typeof(TallyCell), default(object),
            propertyChanged: (bindable, oldvalue, newvalue) =>
            {
                var cell = (TallyCell)bindable;
                cell.OnCommandCanExecuteChanged(cell, EventArgs.Empty);
            });

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        #endregion Command

        public TallyCell()
        {
            InitializeComponent();

            if(App.InDesignMode)
            {                
                var st = new Stratum() { Code = "St" };
                var sg = new SampleGroup() { Code = "Sg", Stratum = st };                
                var tally = new Tally() { Description = $"{sg.Code}/{st.Code}" };

                var pop = new TallyPopulation() { SampleGroup = sg, Tally = tally, TreeCount = 100};

                BindingContext = pop;
            }
        }

        private void ViewCell_Tapped(object sender, EventArgs e)
        {
            Command?.Execute(CommandParameter);
        }
    }
}