using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.Model.ViewModel
{ /// <summary>
    /// 角色视图模型
    /// </summary>
    public class Role
    {    
        [Required]
        [DisplayName("角色备注")]
        public string Remark { get; set; }
        [Required]
        [DisplayName("是否启用")]
        public bool IsActive { get; set; }
        [Required]
        [DisplayName("顺序号")]
        public int SN { get; set; }
        [DisplayName("角色名")]
        public string Name { get; set; }
        [DisplayName("角色ID")]
        public int ID { get; set; }
        [DisplayName("部门ID")]
        public int DepID { get; set; }


        public Model.P_Role ToPOCO()
        {
            var o = new P_Role()
            {
                ID = this.ID,
                Name = this.Name,
                IsActive = this.IsActive,
                Remark = this.Remark,
                SN = this.SN,
                DepID = this.DepID,            
            };
            return o;
        }
    }
}
