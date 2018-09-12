using FScruiser.Services;
using Prism.Navigation;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged, INavigatedAware
    {
        protected INavigationService NavigationService { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        //public abstract Task InitAsync();

        protected ViewModelBase()
        { }

        protected ViewModelBase(INavigationService navigationService)
        {
            NavigationService = navigationService;
        }

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

        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {
            
        }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {
            //MessagingCenter.Send<object, string>(this, Messages.PAGE_NAVIGATED_TO, parameters.ToString());
        }
    }
}