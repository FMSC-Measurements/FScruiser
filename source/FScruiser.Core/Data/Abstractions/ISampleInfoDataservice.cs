using System.Collections.Generic;
using FScruiser.Models;

namespace FScruiser.Data
{
    public interface ISampleInfoDataservice
    {
        void CopySamplerStates(string deviceFrom, string deviceTo);
        //Device GetCurrentDevice();
        IEnumerable<Device> GetDevices();
        SamplerInfo GetSamplerInfo(string stratumCode, string sampleGroupCode);
        SamplerState GetSamplerState(string stratumCode, string sampleGroupCode);
        void UpsertSamplerState(SamplerState samplerState);
    }
}