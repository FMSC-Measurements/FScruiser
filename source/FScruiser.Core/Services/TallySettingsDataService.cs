using System.Collections.Generic;
using FScruiser.Util;

namespace FScruiser.Services
{
    public class TallySettingsDataService : INPC_Base, ITallySettingsDataService
    {
        private bool _enableCruisePopup = true;
        private bool _enableAskEnterTreeData = true;

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

        public ICollection<string> Cruisers
        {
            get => new string[] { "BC", "AB", "CD" };
            set => throw new System.NotImplementedException();
        }
    }
}