using Anke.SHManage.Model.EasyUIModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.Model
{
    public class M_ZCaseTemplateExt
    {
        public int ID { get; set; }
        public string AlarmReason { get; set; }
        public string Name { get; set; }
        public int SN { get; set; }
        public bool IsActive { get; set; }
        public string HistoryOfPresentIllness { get; set; }
    }

}

