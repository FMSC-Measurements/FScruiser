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
                var result = CheckInput(DisplayValue, MinKPI, MaxKPI, out var errorMessage);
                if(result != null)
                {
                    HandleClosed?.Invoke(this, result);
                }
                else
                {
                    DisplayAlert("", errorMessage, "OK");
                }
            }
            Navigation.PopModalAsync();
        }

        public static AskKPIResult CheckInput(string displayValue, int? minKPI, int? maxKPI, out string errorMessage)
        {
            if (String.IsNullOrWhiteSpace(displayValue))
            {
                errorMessage = "No Value Entered";
                return null;
            }
            else if (displayValue == "STM")
            {
                errorMessage = null;
                return new AskKPIResult { IsSTM = true, DialogResult = Pages.DialogResult.OK };
            }
            else if (int.TryParse(displayValue, out int i_value))
            {
                if (minKPI != null && i_value < minKPI.Value && minKPI.Value > 0)
                {
                    errorMessage = $"Value Must be Greater or Equal to {minKPI}";
                    return null;
                }
                else if (maxKPI != null && maxKPI > 0 && (maxKPI.Value > (minKPI ?? 0)) && i_value > maxKPI.Value)
                {
                    errorMessage = $"Value Must be Less Than or Equal to {maxKPI}";
                    return null;
                }
                else
                {
                    errorMessage = null;
                    return new AskKPIResult { KPI = i_value, DialogResult = Pages.DialogResult.OK };
                }
            }
            else
            {
                errorMessage = "Invalid Value";
                return null;
            }
        }
    }
}