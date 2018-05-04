namespace FScruiser.Services
{
    public class ServiceService
    {
        public ICruiseFileService CruiseFileService { get; set; }
        public ICruiseDataService CruiseDataService { get; set; }
        public ICuttingUnitDataService CuttingUnitDataSercie { get; set; }
        public ITallySettingsDataService TallySettingsDataService { get; set; }

        public ISoundService SoundService { get; set; }
        public IDialogService DialogService { get; set; }
    }
}