using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.Model.ViewModel
{ /// <summary>
    /// 部门视图模型
    /// </summary>
    public class Department
    {
        public int ID { get; set; }
        [Required]
        [DisplayName("部门父级")]
        public int ParentID { get; set; }
        [Required]
        [DisplayName("部门名称")]
        public string Name { get; set; }
        [Required]
        [DisplayName("部门备注")]
        public string Remark { get; set; }
        [Required]
        [DisplayName("是否启用")]
        public bool IsActive { get; set; }
        [Required]
        [DisplayName("顺序号")]
        public int SN { get; set; }
        [DisplayName("分站ID")]
        public string DispatchSationID { get; set; }
        [DisplayName("中心ID")]
        public string DispatchSubCenterID { get; set; }


        public Model.P_Department ToPOCO()
        {
            var o = new P_Department()
            {
                ID = this.ID,
                Name = this.Name,
                ParentID = this.ParentID,
                IsActive = this.IsActive,
                Remark = this.Remark,
                SN = this.SN,
                DispatchSationID = this.DispatchSationID,
                DispatchSubCenterID = this.DispatchSubCenterID
            };
            return o;
        }
    }
}
