using Anke.SHManage.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Anke.AnkeOAService
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“AnkeOAService”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 AnkeOAService.svc 或 AnkeOAService.svc.cs，然后开始调试。
    public class AnkeOAService : IAnkeOAService
    {
        public string SaveUserInfo(string jsonUserStr)
        {
            //P_User userinfo = JsonHelper.GetJsonInfoBy<P_User>(jsonUserStr);

            DreamSoftUserInfo dsUserinfo;
            try
            {
                dsUserinfo = XmlHelper.XmlConvert<DreamSoftUserInfo>(jsonUserStr.Trim());
                LogUtility.Debug("SaveUserInfo", "用户信息：" + dsUserinfo.LogName +"解析成功！");   //保存日志信息

                P_User userinfo = new P_User();
                //userinfo.DepID = 1;         //暂时不同步部门
                userinfo.IsActive = int.Parse(dsUserinfo.State) > 0 ? true : false;  //是否激活
                userinfo.LoginName = dsUserinfo.LogName;    //登录名称
                userinfo.Name = dsUserinfo.Name;    //姓名
                userinfo.PassWord = dsUserinfo.Password;       //MD5加密的字符串
                userinfo.WorkCode = dsUserinfo.CardId;  //工号
                //userinfo.WorkCode = dsUserinfo.LogName.Substring(dsUserinfo.LogName.Length - 4);  //工号
                if (dsUserinfo.Sex.Length > 0)
                    userinfo.Gender = int.Parse(dsUserinfo.Sex) > 1 ? "女" : "男";  //性别  1男  2女
                else
                    userinfo.Gender = "不详";

                using (var context = new AKSHManageEntities())
                {
                    //TODO :  先查看是否有人员数据 如果有则更新 没有则新增
                    P_User info = context.P_User.FirstOrDefault(p => p.WorkCode.Equals(userinfo.WorkCode));
                    if (info != null)  //更新用户信息
                    {
                        info.LoginName = userinfo.LoginName;
                        info.PassWord = userinfo.PassWord;
                        info.Name = userinfo.Name;
                        info.Gender = userinfo.Gender;
                        info.IsActive = userinfo.IsActive;
                    }
                    else   //添加用户信息
                    {
                        info = new P_User();
                        info.LoginName = userinfo.LoginName;
                        info.PassWord = userinfo.PassWord;
                        info.Name = userinfo.Name;
                        info.Gender = userinfo.Gender;
                        info.IsActive = userinfo.IsActive;
                        info.WorkCode = userinfo.WorkCode;
                        info.DepID = 1;
                        context.P_User.Add(info);
                    }

                    context.SaveChanges();
                    return "";  //返回空字符串表示成功
                }
            }
            catch (DbEntityValidationException dbEx)
            {
                LogUtility.Error("SaveUserInfo",  "保存用户信息 失败！  原因:" + dbEx.EntityValidationErrors.FirstOrDefault().ValidationErrors.FirstOrDefault().ErrorMessage);   //保存日志信息
                return "保存用户信息 失败！  原因:" + dbEx.EntityValidationErrors.FirstOrDefault().ValidationErrors.FirstOrDefault().ErrorMessage;
            }
            catch (Exception e)
            {
                LogUtility.Error("SaveUserInfo", "保存用户信息 失败！  原因:" + e.Message);   //保存日志信息
                return "保存用户信息 失败！  原因:" + e.Message;
            }
        }
    }
}
