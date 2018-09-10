using System.Collections.Generic;
using System.Linq;
using FScruiser.Services;
using FScruiser.Util;

namespace FScruiser.XF.Services
{
    public class TallySettingsDataService : INPC_Base, ITallySettingsDataService
    {
        public bool EnableCruiserPopup
        {
            get
            {
                var propDict = App.Current.Properties;
                if (propDict.ContainsKey(nameof(EnableCruiserPopup)))
                {
                    return (bool)propDict[nameof(EnableCruiserPopup)];
                }
                else { return true; }
            }
            set
            {
                var propDict = App.Current.Properties;
                if (propDict.ContainsKey(nameof(EnableCruiserPopup)))
                {
                    propDict[nameof(EnableCruiserPopup)] = value;
                }
                else
                {
                    propDict.Add(nameof(EnableCruiserPopup), value);
                }
            }
        }

        public bool EnableAskEnterTreeData
        {
            get
            {
                var propDict = App.Current.Properties;
                if (propDict.ContainsKey(nameof(EnableAskEnterTreeData)))
                {
                    return (bool)propDict[nameof(EnableAskEnterTreeData)];
                }
                else { return false; }
            }
            set
            {
                var propDict = App.Current.Properties;
                if (propDict.ContainsKey(nameof(EnableAskEnterTreeData)))
                {
                    propDict[nameof(EnableAskEnterTreeData)] = value;
                }
                else
                {
                    propDict.Add(nameof(EnableAskEnterTreeData), value);
                }
            }
        }

        public IEnumerable<string> Cruisers
        {
            get
            {
                var propDict = App.Current.Properties;
                if (propDict.ContainsKey(nameof(Cruisers)))
                {
                    var cruisers = (string)propDict[nameof(Cruisers)];
                    return cruisers.Split(' ');
                }
                else
                {
                    return Enumerable.Empty<string>();
                }
            }
        }

        public void AddCruiser(string cruiser)
        {
            if(string.IsNullOrWhiteSpace(cruiser)) { return; }
            cruiser = cruiser.Trim();

            var propDict = App.Current.Properties;
            if (propDict.ContainsKey(nameof(Cruisers)))
            {
                var cruisers = (string)propDict[nameof(Cruisers)];
                cruisers += " " + cruiser;
                propDict[nameof(Cruisers)] = cruisers;
            }
            else
            {
                propDict.Add(nameof(Cruisers), cruiser);
            }
        }

        public void RemoveCruiser(string cruiser)
        {
            var cruisers = Cruisers;

            var propDict = App.Current.Properties;
            if (propDict.ContainsKey(nameof(Cruisers)))
            {
                propDict[nameof(Cruisers)] = string.Join(" ", cruisers.Where(x => x != cruiser).ToArray());
            }
        }
    }
}