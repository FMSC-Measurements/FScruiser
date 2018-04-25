using FScruiser.Util;

namespace FScruiser.Services
{
    public class TallySettingsDataService : INPC_Base, ITallySettingsDataService
    {
        private bool _enableCruisePopup;
        private bool _enableAskEnterTreeData;

        public bool EnableCruiserPopup
        {
            get { return _enableCruisePopup; }
            set { SetValue(ref _enableCruisePopup, value); }
        }

        public bool EnableAskEnterTreeData
        {
            get { return _enableAskEnterTreeData; }
            set { SetValue(ref _enableAskEnterTreeData, value); }
        }
    }
}