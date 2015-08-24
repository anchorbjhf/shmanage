using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.BLL
{
    /// <summary>
    ///  用户权限业务
    /// </summary>
    public class UserPermissionBLL
    {
        /// <summary>
        /// 根据用户ID 获取权限集合
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public List<Model.P_Permission> GetUserPermission(int userId)
        {
            /*
             * 1.首先根据用户的ID 查询用户的所有角色ID
             * 2.根据所有角色ID查询 多有的角色权限ID
             * 3. 更具权限ID 获取用户权限集合
             * 
             * !!!效率比较低 最好直接用sql语句来获取权限数据
             */

            ////根据用户 id 查到 该用户的所有角色id
            //List<int> listUserRoleId = new P_UserRoleBLL().GetListBy(ur => ur.UserID == userId).Select(role => role.RoleID).ToList();
            ////查出该用户所有角色ID
            //List<int> listRoleId = new P_RoleBLL().GetListBy(role => listUserRoleId.Contains(role.ID)).Select(role => role.ID).ToList();
            ////根据角色 id 查询角色权限 id
            //List<int> listPerIds = new P_RolePermissionBLL().GetListBy(rp => listRoleId.Contains(rp.RoleID)).Select(rp => rp.PermissionID).ToList();
            ////查询所有角色数据
            //List<Model.P_Permission> listPermission = new P_PermissionBLL().GetListBy(p => listPerIds.Contains(p.ID)).Select(p => p.ToPOCO()).ToList();

            //// 根据用户特权查询
            ////查询 用户特权id
            //List<int> speicalPerIds = new P_SpeicalPermissionBLL().GetListBy(sp => sp.UserId == userId).Select(sp => sp.PermissionID).ToList();
            //// 查询 特权数据
            //List<Model.P_Permission> listSpeicalPermission = new P_PermissionBLL().GetListBy(p => speicalPerIds.Contains(p.ID)).Select(p => p.ToPOCO()).ToList();

            ////合并And返回权限集合
            //return listPermission.Concat(listSpeicalPermission).OrderBy(p => p.SN).ToList();


            string sqlStr = @"select * from dbo.P_Permission  pp where pp.IsActive=1 and pp.ID in (
                                (select rp.PermissionID from dbo.P_RolePermission rp where rp.RoleID in (
                                select ur.RoleID from dbo.P_UserRole ur where ur.UserID=@UserID)
                                union
                                select psp.PermissionID from dbo.P_SpeicalPermission psp where psp.UserId = @UserID) 
                              )";

            return new P_PermissionBLL().DALContext.IP_PermissionDAL.ExcuteSqlToList(sqlStr, new SqlParameter("@UserID", userId));
        }



        /// <summary>
        /// 根据用户编码获得 角色物资信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<string> GetStorageMaterialType(int userId)
        {
            string sqlStr = @"select * from I_StorageRole 
                              where RoleID in(select RoleID from [dbo].[P_UserRole] where UserID = @UserID)";

            return new P_PermissionBLL().DALContext.II_StorageRoleDAL.ExcuteSqlToList(sqlStr, new SqlParameter("@UserID", userId)).Select(r=>r.MaterialType).ToList();

        }





    }
}
