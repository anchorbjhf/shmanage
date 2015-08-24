using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.Model
{

    /// <summary>
    /// 用户扩展
    /// </summary>
    public partial class P_User
    {
        /// <summary>
        /// 生成 很纯洁的 实体对象
        /// </summary>
        /// <returns></returns>
        public P_User ToExtModle()
        {
            P_User info = new P_User()
            {
                ID = this.ID,
                DepID = this.DepID,
                LoginName = this.LoginName,
                WorkCode = this.WorkCode,
                Name = this.Name,
                PassWord = this.PassWord,
                Gender = this.Gender,
                IsActive = this.IsActive,
                SN = this.SN,
                P_Department = this.P_Department.ToPOCO()
            };
            return info;
        }
    }
}
