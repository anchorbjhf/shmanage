using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.Model.EasyUIModel
{
    public class TreeNode
    {
        public string id { get; set; }
        public string text { get; set; }
        public string state { get; set; }
        public bool Checked { get; set; }
        public object attributes { get; set; }
        public string iconCls { get; set; }
        public List<TreeNode> children { get; set; }

    }
}
