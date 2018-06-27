using FMSC.ORM.EntityModel.Attributes;

namespace FScruiser.Models
{
    [EntitySource("TreeDefaultValue")]
    public class TreeDefaultProxy
    {
        //[PrimaryKeyField(Name = "TreeDefaultValue_CN")]
        //public long? TreeDefaultValue_CN { get; set; }

        [Field(Name = "Species")]
        public string Species { get; set; }

        [Field(Name = "LiveDead")]
        public string LiveDead { get; set; }

        //[Field(Name = "PrimaryProduct")]
        //public string PrimaryProduct { get; set; }

        //[Field(Name = "FIAcode")]
        //public long FIAcode { get; set; }

        public override string ToString()
        {
            if (LiveDead == "D")
            {
                return Species + " - dead";
            }
            return Species;
        }
    }
}