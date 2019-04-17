using FMSC.ORM.EntityModel.Attributes;

namespace FScruiser.Models
{
    public class TallyPopulation_Plot : TallyPopulation_Base
    {
        public bool InCruise { get; set; }

        public bool IsEmpty { get; set; }
    }
}