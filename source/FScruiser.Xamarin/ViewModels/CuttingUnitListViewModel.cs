using FScruiser.Models;
using FScruiser.XF.Services;
using Prism.Ioc;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    public class CuttingUnitListViewModel : ViewModelBase
    {
        private IEnumerable<CuttingUnit> _units;

        public IContainerExtension Container { get; protected set; }

        public bool IsFileNotOpen => Units == null;

        public IEnumerable<CuttingUnit> Units
        {
            get { return _units; }
            set { SetValue(ref _units, value); }
        }

        public ICuttingUnitDatastoreProvider CuttingUnitDatastoreProvider { get; }

        public CuttingUnitListViewModel(ICuttingUnitDatastoreProvider cuttingUnitDatastoreProvider, INavigationService navigationService) : base(navigationService)
        {
            CuttingUnitDatastoreProvider = cuttingUnitDatastoreProvider ?? throw new ArgumentNullException(nameof(cuttingUnitDatastoreProvider));

            MessagingCenter.Subscribe<object, string>(this, Messages.CRUISE_FILE_OPENED, (sender, path) =>
            {
                Refresh();
            });
        }

        public void SelectUnit(CuttingUnit unit)
        {
            if (unit == null) { throw new ArgumentNullException(nameof(unit)); }

            MessagingCenter.Send<string>(unit.Code, Messages.CUTTING_UNIT_SELECTED);
        }

        protected override void Refresh(INavigationParameters parameters)
        {
            var datastore = CuttingUnitDatastoreProvider.CuttingUnitDatastore;
            if (datastore != null)
            {
                Units = datastore.GetUnits();
                base.RaisePropertyChanged(nameof(IsFileNotOpen));
            }
        }
    }
}