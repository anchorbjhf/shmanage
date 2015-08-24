using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.Model.ViewModel
{
    public class LoginInfo
    {
        [Required]
        public string LoginName { get; set; }

        [Required]
        public string PassWord { get; set; }
    }
}
