using FMSC.ORM.EntityModel.Attributes;

namespace FScruiser.Models
{
    public class TallyPopulation_Plot : TallyPopulation_Base
    {
        [Field(Alias = "InCruise", PersistanceFlags = PersistanceFlags.Never)]
        public bool InCruise { get; set; }

        [Field("IsEmpty")]
        public string IsEmpty { get; set; }

        public bool IsEmptyBool => IsEmpty == "True";
    }
}