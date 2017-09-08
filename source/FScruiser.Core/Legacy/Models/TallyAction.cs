using FScruiser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace FScruiser.Core.Legacy.Models
{
    public class TallyAction
    {
        [XmlIgnore]
        public TreeEstimate TreeEstimate { get; set; }

        [XmlAttribute]
        public String Time { get; set; }

        [XmlAttribute]
        public int KPI { get; set; }

        private long _countCN;

        [XmlAttribute]
        public long CountCN
        {
            get { return _countCN; }
            set { _countCN = value; }
        }

        private long _treeEstCN;

        [XmlAttribute]
        public long TreeEstimateCN
        {
            get { return _treeEstCN; }
            set { _treeEstCN = value; }
        }

        private long _treeCN;

        public long TreeCN
        {
            get { return _treeCN; }
            set { _treeCN = value; }
        }

        private CountTree _count;

        public CountTree Count
        {
            get { return _count; }
            set { _count = value; }
        }

        private Tree _treeRecord;

        public Tree TreeRecord
        {
            get { return _treeRecord; }
            set { _treeRecord = value; }
        }



        public TallyAction()
        {
            Time = DateTime.Now.ToString("hh:mm");
        }

        //public TallyAction(CountTree count)

        //public TallyAction(Tree treeRecord, CountTree count) : this(count)

        public override string ToString()
        {
            string stCode = "--";
            string sgCode = "---";

            if (Count != null)
            {
                var sg = Count.SampleGroup;
                if (sg != null && sg.Stratum.Code != null)
                {
                    stCode = sg.Stratum.Code;
                    if(stCode.Length > 2) { stCode = stCode.Substring(0, 2); }

                    sgCode = sg.Code;
                    if(sgCode.Length > 4) { sgCode = sgCode.Substring(0, 4); }
                }
            }

            string[] a = new string[3];
            a[0] = stCode + " " + sgCode;
            if(KPI != 0)
            {
                a[1] = KPI.ToString("' ['#']'");
            }
            if(TreeRecord != null)
            {
                a[2] = string.Format(" #{0} {1}", TreeRecord.TreeNumber, TreeRecord.CountOrMeasure);
            }

            return string.Concat(a);
        }
    }
}
