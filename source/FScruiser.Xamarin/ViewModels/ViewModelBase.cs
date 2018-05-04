using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public INavigation Navigation { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;


        public ViewModelBase(INavigation navigation)
        {
            Navigation = navigation;
        }

        public abstract void Init();


        protected virtual void RaisePropertyChanged(string propName)
        {
            RaisePropertyChanged(new PropertyChangedEventArgs(propName));
        }

        protected void RaisePropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        public void SetValue<tTarget>(ref tTarget target, tTarget value, [CallerMemberName] string propName = null)
        {
            target = value;
            if (propName != null) { RaisePropertyChanged(propName); }
        }
    }
}