using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.Model.ViewModel
{
    public class Permission
    {
        public Permission()
        {
            this.ID = -1;
            this.ParentName = "无";
        }

        [DisplayName("ID")]
        public int ID { get; set; }

        [DisplayName("父级")]
        public int ParentID { get; set; }

        [DisplayName("权限名")]
        [Required]
        public string Name { get; set; }

        [DisplayName("区域名")]
        [Required]
        public string AreaName { get; set; }

        [DisplayName("控制器名")]
        public string ControllerName { get; set; }

        [DisplayName("方法名")]
        public string ActionName { get; set; }

        [DisplayName("请求方式")]
        public short FormMethod { get; set; }

        [DisplayName("序号")]
        public int SN { get; set; }

        [DisplayName("是否在菜单显示")]
        public bool IsShow { get; set; }

        [DisplayName("备注")]
        [DataType(DataType.MultilineText)]
        public string Remark { get; set; }

        [DisplayName("父级名称")]
        public string ParentName { get; set; }
    }


}
