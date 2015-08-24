using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.Model
{
   public  class I_OverdueExt
    {
       public string MTypeID { get; set; }
       public string Name { get; set; }
       public string MCode { get; set; }
       public string EntryCode { get; set; }
       public System.DateTime EntryData { get; set; }
       public System.DateTime ValidDate { get; set; }
        public string Manufacturer { get; set; }
        public string Vendor { get; set; }

        public string Unit { get; set; }

        public string Specification { get; set; }
        public string Storage { get; set; }
        public double Surplus { get; set; }
        public bool isOverdue { get; set; }
    }
}
