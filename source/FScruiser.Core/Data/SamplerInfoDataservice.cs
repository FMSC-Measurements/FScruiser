using CruiseDAL;
using FScruiser.Models;
using FScruiser.Services;
using System.Collections.Generic;
using System.Linq;

namespace FScruiser.Data
{
    public class SamplerInfoDataservice : DataserviceBase, ISampleInfoDataservice
    {
        protected IDeviceInfoService DeviceInfo { get; }
        //protected Func<string> GetCurrentDeviceID { get; }
        //protected Func<string> GetCurrentDeviceName { get; }

        public Device CurrentDevice { get; }

        public SamplerInfoDataservice(string path, IDeviceInfoService deviceInfoService) : base(path)
        {
            DeviceInfo = deviceInfoService;

            CurrentDevice = GetCurrentDevice();
        }

        public SamplerInfoDataservice(CruiseDatastore_V3 database, IDeviceInfoService deviceInfoService) : base(database)
        {
            DeviceInfo = deviceInfoService;

            CurrentDevice = GetCurrentDevice();
        }

        public SamplerInfo GetSamplerInfo(string stratumCode, string sampleGroupCode)
        {
            return Database.Query<SamplerInfo>(
@"SELECT
    sg.*,
    st.Method
FROM SampleGroup_V3 AS sg
    JOIN Stratum AS st ON st.Code = sg.StratumCode
WHERE sg.StratumCode = @p1
    AND sg.SampleGroupCode = @p2
;", stratumCode, sampleGroupCode).FirstOrDefault();
        }

        public SamplerState GetSamplerState(string stratumCode, string sampleGroupCode)
        {
            var deviceID = DeviceInfo.GetUniqueDeviceID();

            return Database.Query<SamplerState>(
@"SELECT
    ss.*
FROM SamplerState AS ss
WHERE ss.StratumCode = @p1
    AND ss.SampleGroupCode = @p2
    AND ss.DeviceID = @p3;", stratumCode, sampleGroupCode, deviceID).FirstOrDefault();
        }

        public void UpsertSamplerState(SamplerState samplerState)
        {
            var deviceID = DeviceInfo.GetUniqueDeviceID();

            Database.Execute2(
@"INSERT INTO SamplerState (
    DeviceID,
    StratumCode,
    SampleGroupCode,
    SampleSelectorType,
    BlockState,
    SystematicIndex,
    Counter,
    InsuranceIndex,
    InsuranceCounter
) VALUES (
    @DeviceID,
    @StratumCode,
    @SampleGroupCode,
    @SampleSelectorType,
    @BlockState,
    @SystematicIndex,
    @Counter,
    @InsuranceIndex,
    @InsuranceCounter
)
ON CONFLICT (DeviceID, StratumCode, SampleGroupCode) DO
UPDATE SET
        BlockState = @BlockState,
        SystematicIndex = @SystematicIndex,
        Counter = @Counter,
        InsuranceIndex = @InsuranceIndex,
        InsuranceCounter = @InsuranceCounter
    WHERE DeviceID = @DeviceID AND StratumCode = @StratumCode AND SampleGroupCode = @SampleGroupCode;",
                new
                {
                    DeviceID = deviceID,
                    samplerState.BlockState,
                    samplerState.Counter,
                    samplerState.InsuranceCounter,
                    samplerState.InsuranceIndex,
                    samplerState.SystematicIndex,
                    samplerState.SampleSelectorType,
                    samplerState.SampleGroupCode,
                    samplerState.StratumCode,
                }
            );
        }

        public IEnumerable<Device> GetDevices()
        {
            return Database.Query<Device>(
@"SELECT d.*, max(ss.ModifiedDate) FROM DEVICE
LEFT JOIN SamplerState AS ss USING (DeviceID)
GROUP BY DeviceID;");
        }

        protected Device GetCurrentDevice()
        {
            var deviceID = DeviceInfo.GetUniqueDeviceID();

            var device = Database.Query<Device>("SELECT * FROM Device WHERE DeviceID = @p1;", deviceID).FirstOrDefault();

            if (device == null)
            {
                device = new Device
                {
                    DeviceID = deviceID,
                    Name = DeviceInfo.GetDeviceName(),
                };

                Database.Insert(device);
            }

            return device;
        }

        public void CopySamplerStates(string deviceFrom, string deviceTo)
        {
            Database.Execute(
@"INSERT INTO SamplerState (
    DeviceID,
    StratumCode,
    SampleGroupCode,
    SampleSelectorType,
    BlockState,
    SystematicIndex,
    Counter,
    InsuranceIndex,
    InsuranceCounter
) SELECT @p2,
    ss.StratumCode,
    ss.SampleGroupCode,
    ss.SampleSelectorType,
    ss.BlockState,
    ss.SystematicIndex,
    ss.Counter,
    ss.InsuranceIndex,
    ss.InsuranceCounter
    FROM SamplerState AS ss
    WHERE ss.DeviceID = @p1;", deviceFrom, deviceTo);
        }

        //public virtual string GetCurrentDeviceName();

        //public virtual string GetCurrentDeviceID();
    }
}