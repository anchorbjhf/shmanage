using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.Model
{
    public  class I_StorageRoleExt
    {
        /// <summary>
        /// 物资类型编码
        /// </summary>
        public string materialType { get; set; }
        /// <summary>
        /// 角色名称
        /// </summary>
        public string roleName { get; set; }
        /// <summary>
        /// 角色编码
        /// </summary>
        public int roleID { get; set; }
        /// <summary>
        /// 物资类型名称
        /// </summary>
        public string materialTypeName{ get; set; }
    }

}
