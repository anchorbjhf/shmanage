using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.Model
{
   public   class I_StoragePersonExt
    {
        /// <summary>
        /// 人员编码
        /// </summary>
        public int uid { get; set; }
       /// <summary>
       /// 部门名称
       /// </summary>
        public string depName{ get; set; }
       /// <summary>
       /// 角色名称
       /// </summary>
        public string roleName { get; set; }
       /// <summary>
        /// 姓名
       /// </summary>
        public string userName { get; set; }
       /// <summary>
       /// 工号
       /// </summary>
        public string workCode { get; set; }
       /// <summary>
       /// 仓库名称
       /// </summary>
        public string storage { get; set; }

    }
}
