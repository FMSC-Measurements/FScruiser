using FScruiser.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace FScruiser.XF.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public ServiceService ServiceService { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public ViewModelBase()
        {
            ServiceService = App.ServiceService;
        }

        public ViewModelBase(ServiceService serviceService)
        {
            ServiceService = serviceService;
        }

        public abstract Task InitAsync();

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