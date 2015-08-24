using Anke.SHManage.Model;
using Anke.SHManage.Model.ViewModel;
using Anke.SHManage.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.BLL
{
    /// <summary>
    /// 用户登录相关业务方法
    /// </summary>
    public class LoginUserBLL
    {
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <returns></returns>
        public bool Login(string loginName, string pwd)
        {
            /*
             * 1. 根据用户名密查询用户是否存在
             * 2. 根据用户ID查找所用权限 
             */
            string pwdMd5 = SecurityHelper.GetMD5(pwd);  //MD5
            bool result = false;
            P_UserBLL bll = new P_UserBLL();
            P_User user = bll.GetListBy(u => u.LoginName == loginName && u.PassWord == pwdMd5).Select(u=>u.ToExtModle()).FirstOrDefault(); //查找用户名 密码
            if (user != null)
            {
                UserOperateContext.Current.Session_UsrInfo = user;
                UserPermissionBLL upbll = new UserPermissionBLL();  //实例化权限业务
                UserOperateContext.Current.Session_UsrPermission = upbll.GetUserPermission(user.ID); //保存到session中

                UserOperateContext.Current.Session_UsrRole = new P_UserRoleBLL().GetListBy(ur => ur.UserID == user.ID).Select(ur => ur.RoleID).ToList(); //把用户角色存入 session中

                StorageRelatedInfo srInfo = new StorageRelatedInfo();
                //获取 用户仓储列表
                List<int> listStorageID = new I_StoragePersonBLL().GetListBy(sp => sp.UserID == user.ID).Select(sp => sp.StorageID).ToList();
                srInfo.listUserStorage = listStorageID;

                //获取物资类型列表
                List<string> listMaterialType = upbll.GetStorageMaterialType(user.ID);
                srInfo.listUserStorageMaterialType = listMaterialType;
                UserOperateContext.Current.Session_StorageRelated = srInfo;  //存session信息

                result = true;
            }

            return result;
        }


        /// <summary>
        /// 通过工号登录
        /// </summary>
        /// <param name="workCode"></param>
        /// <returns></returns>
        public bool Login(string LoginName)
        {
            bool result = false;
            P_UserBLL bll = new P_UserBLL();
            P_User user = bll.GetListBy(u => u.LoginName == LoginName).Select(u => u.ToExtModle()).FirstOrDefault(); //查找用户名 密码
            if (user != null)
            {
                UserOperateContext.Current.Session_UsrInfo = user;
                UserPermissionBLL upbll = new UserPermissionBLL();  //实例化权限业务
                UserOperateContext.Current.Session_UsrPermission = upbll.GetUserPermission(user.ID); //保存到session中

                UserOperateContext.Current.Session_UsrRole = new P_UserRoleBLL().GetListBy(ur=>ur.UserID==user.ID).Select(ur=>ur.RoleID).ToList(); //把用户角色存入 session中

                StorageRelatedInfo srInfo = new StorageRelatedInfo();
                //获取 用户仓储列表
                List<int> listStorageID = new I_StoragePersonBLL().GetListBy(sp => sp.UserID == user.ID).Select(sp => sp.StorageID).ToList();
                srInfo.listUserStorage = listStorageID;

                //获取物资类型列表
                List<string> listMaterialType = upbll.GetStorageMaterialType(user.ID);
                srInfo.listUserStorageMaterialType = listMaterialType;
                UserOperateContext.Current.Session_StorageRelated = srInfo;  //存session信息

                result = true;
            }

            return result;
        }



        /// <summary>
        /// 判断当前用户是否登陆
        /// </summary>
        /// <returns></returns>
        public bool IsLogin()
        {
            //验证用户是否登陆(Session)
            if (UserOperateContext.Current.Session_UsrInfo == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
