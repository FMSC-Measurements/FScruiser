namespace FScruiser.Services
{
    public class ServiceService
    {
        public ICruiseFileService CruiseFileService { get; set; }
        public ICruiseDataService CruiseDataService { get; set; }
        //public ICuttingUnitDataService CuttingUnitDataService { get; set; }
        public ICuttingUnitDatastore Datastore { get; set; }
        public ITallySettingsDataService TallySettingsDataService { get; set; }

        public ISoundService SoundService { get; set; }
        public IDialogService DialogService { get; set; }
        public ISampleSelectorDataService SampleSelectorRepository { get; set; }
    }
}