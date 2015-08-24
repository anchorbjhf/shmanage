using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.Model
{
    //药品、耗材、救治措施树形结构
    public class M_ChargeItemInfo
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string ParentID { get; set; }
        public string TypeID { get; set; }
        public string Unit { get; set; }
        public string Specification { get; set; }
        public string PinYin { get; set; }
        public string Remark { get; set; }
        public string SN { get; set; }
        public string Number { get; set; }
        public string GiveMedicineWay { get; set; }
    }
}
