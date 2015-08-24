using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.Model.ExtentionModel
{
    public partial class P_RolePermission
    {

        /// <summary>
        /// 获取角色权限里的 权限部分
        /// </summary>
        /// <returns></returns>
        public P_Permission GetPermissionPart()
        {
            return new P_Permission()
            {
                //ID = this
                //ParentID = this.Ou_Permission.pParent,
                //Name = this.Ou_Permission.pName,
                //pAreaName = this.Ou_Permission.pAreaName,
                //pControllerName = this.Ou_Permission.pControllerName,
                //pActionName = this.Ou_Permission.pActionName,
                //pFormMethod = this.Ou_Permission.pFormMethod,
                //pOperationType = this.Ou_Permission.pOperationType,
                //pIco = this.Ou_Permission.pIco,
                //pOrder = this.Ou_Permission.pOrder,
                //pIsLink = this.Ou_Permission.pIsLink,
                //pLinkUrl = this.Ou_Permission.pLinkUrl,
                //pIsShow = this.Ou_Permission.pIsShow,
                //pRemark = this.Ou_Permission.pRemark,
                //pIsDel = this.Ou_Permission.pIsDel,
                //pAddTime = this.Ou_Permission.pAddTime
            };
        } 
    }
}
