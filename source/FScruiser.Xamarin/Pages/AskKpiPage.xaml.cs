using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FScruiser.XF.Pages
{
    public class AskKPIResult
    {
        public DialogResult DialogResult { get; set; }
        public bool IsSTM { get; set; }
        public int KPI { get; set; }
    }

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AskKpiPage : ContentPage
	{
		public AskKpiPage()
		{
			InitializeComponent ();

            foreach(var btn in _grid.Children.OfType<Button>())
            {
                btn.Clicked += Btn_Clicked;
            }
		}

        public event EventHandler<AskKPIResult> HandleClosed;


        public int? MinKPI { get; set; }

        public int? MaxKPI { get; set; }

        public int MaxValueLength => 6;

        public string DisplayValue
        {
            get { return _kpiLabel.Text; }
            protected set { _kpiLabel.Text = value; }
        }        

        public string DialogResult { get; private set; }

        private void Btn_Clicked(object sender, EventArgs e)
        {
            if(sender == null) { return; }
            var btn = (Button)sender;

            var btnText = btn.Text;

            switch(btnText)
            {
                case "C":      { DisplayValue = "";      break; }
                case "OK":     { Close(btnText);  break; }
                case "Cancel": { Close(btnText); break; }
                case "STM":    { Close("STM");   break; }
                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                case "6":
                case "7":
                case "8":
                case "9":
                case "0":
                    {
                        NotifyNumClicked(btnText);
                        break;
                    }
            }

        }

        private void NotifyNumClicked(string btnText)
        {
            if(DisplayValue == "STM") { DisplayValue = ""; }
            if(DisplayValue == null || DisplayValue.Length < MaxValueLength)
            {
                DisplayValue = DisplayValue + btnText;
            }
        }

        public int? GetUserEnteredValue()
        {
            var displayValue = DisplayValue;
            if(displayValue == "STM")
            {
                return -1;
            }

            if(int.TryParse(DisplayValue, out int value))
            { return value; }
            else { return null; }
        }

        protected override bool OnBackButtonPressed()
        {
            HandleClosed?.Invoke(this, new AskKPIResult { DialogResult = Pages.DialogResult.Cancel });
            return base.OnBackButtonPressed();
        }

        private void Close(string dialogResult)
        {
            DialogResult = dialogResult;
            if(dialogResult == "Cancel")
            {
                SendBackButtonPressed();
                return;
            }
            else
            {
                if(String.IsNullOrWhiteSpace(DisplayValue))
                {
                    DisplayAlert("", "No Value Entered", "OK");
                    return;
                }
                else if(DisplayValue == "STM")
                {
                    HandleClosed?.Invoke(this, new AskKPIResult { IsSTM = true, DialogResult = Pages.DialogResult.OK });
                }
                else if( int.TryParse(DisplayValue, out int i_value))
                {
                    if(MinKPI != null && i_value < MinKPI.Value)
                    {
                        DisplayAlert("", $"Value Must be Greater or Equal to {MinKPI}", "OK");
                        return;
                    }
                    else if(MaxKPI != null && i_value > MaxKPI.Value)
                    {
                        DisplayAlert("", $"Value Must be Less Than or Equal to {MaxKPI}", "OK");
                        return;
                    }
                    else
                    {
                        HandleClosed?.Invoke(this, new AskKPIResult { KPI = i_value, DialogResult = Pages.DialogResult.OK });
                    }
                }
                else
                {
                    DisplayAlert("", "Invalid Value", "OK");
                    return;
                }
            }
            Navigation.PopModalAsync();
            
        }
    }
}