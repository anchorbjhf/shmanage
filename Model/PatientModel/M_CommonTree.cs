using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.Model
{
    public class CommonTree
    {
        public List<CommonTree> children { get; set; }
        public string id { get; set; }
        public string text { get; set; }
        public string state { get; set; }
        public string ParentID { get; set; }

        public string Name { get; set; }
        public string TypeID { get; set; }
        public string Specification { get; set; }
        public string Unit { get; set; }
        public string PinYin { get; set; }
        public string Remark { get; set; }
        public int SN { get; set; }
        public string Number { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string GiveMedicineWay { get; set; }

    }
}
