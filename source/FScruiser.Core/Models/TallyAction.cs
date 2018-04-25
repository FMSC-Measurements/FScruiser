using System;
using System.Xml.Serialization;

namespace FScruiser.Models
{
    [Serializable]
    public class TallyAction
    {
        public TallyAction()
        {
        }

        [XmlAttribute]
        public DateTime Time { get; set; }

        [XmlAttribute]
        public long CountCN { get; set; }

        [XmlAttribute]
        public long TreeEstimateCN { get; set; }

        [XmlAttribute]
        public long TreeCN { get; set; }
    }
}