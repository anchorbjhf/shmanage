using Anke.SHManage.Model;
using Anke.SHManage.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Anke.SHManage.BLL
{

    /// <summary>
    /// 用户操作类 
    /// 设计目的： 统一操作线程唯一操作类， 仿照asp.net请求流程 统一处理 session  request  response  permission 等信息处理
    /// </summary>
    public class UserOperateContext
    {
        //cookie路径 如果需要用Cookie存储用户信息
        private const string UserCookiePath = "/";

        //关于登录用户的session键统一
        private const string UserInfoKey = "UserInfo";
        private const string UserPermissionKey = "UserPermission";
        private const string UserJsonTreeKey = "UserJsonTree";
        private const string UserStorageRelatedKey = "UserStorageRelated";
        private const string UserRoleKey = "UserRole";


        /// <summary>
        /// 避免并发获取线程唯一用户操作实例实例
        /// </summary>
        public static UserOperateContext Current
        {
            get
            {
                UserOperateContext context = CallContext.GetData(typeof(UserOperateContext).Name) as UserOperateContext;

                if (context == null)
                {
                    context = new UserOperateContext(); //实例化用户操作上下文
                    CallContext.SetData(typeof(UserOperateContext).Name, context);  // 加入线程槽 保证线程唯一
                }

                return context;
            }
        }
        /// <summary>
        /// 根据MaterialTypeID查询所有该ID下的list，如果Mtype为空，折返回该User用户的所有物资类型
        /// </summary>
        /// <param name="Mtype"></param>
        /// <returns></returns>
        public List<string> getMaterialTypeList(string Mtype)
        {
            List<string> mTypeList = new List<string>();
            if (Mtype.Length > 0)
            {
                List<TDictionary> listD = new TDictionaryBLL().getTDicRecursion("'" + Mtype + "'");
                mTypeList = listD.Select(t => t.ID).ToList();
            }
            else
            {
                List<string> mlist = UserOperateContext.Current.Session_StorageRelated.listUserStorageMaterialType;
                string instr = "";
                foreach (string item in mlist)
                {
                    instr += "'" + item + "',";
                }
                instr = instr.Substring(0, instr.Length - 1);
                List<TDictionary> listD = new TDictionaryBLL().getTDicRecursion(instr);
                mTypeList = listD.Select(t => t.ID).ToList();
            }
            return mTypeList;
        }

        #region HTTP相关操作
        /// <summary>
        /// Http上下文 
        /// </summary>
        private HttpContext HttpContext
        {
            get
            {
                return HttpContext.Current;

            }
        }

        /// <summary>
        /// 响应对象
        /// </summary>
        public HttpResponse Response
        {
            get
            {
                return HttpContext.Response;
            }
        }

        /// <summary>
        /// 请求对象
        /// </summary>
        public HttpRequest Request
        {
            get
            {
                return HttpContext.Request;
            }
        }

        /// <summary>
        /// 获取 Session
        /// </summary>
        private System.Web.SessionState.HttpSessionState Session
        {
            get
            {
                System.Web.SessionState.HttpSessionState _session = HttpContext.Session;
                //设置session过期时长
                _session.Timeout = AppConfig.GetInt32ConfigValue("SessionOutTime");
                return _session;
            }
        }

        #endregion

        #region Session操作 存储用户相关信息

        /// <summary>
        /// 当前用户对象
        /// </summary>
        public P_User Session_UsrInfo
        {
            get
            {
                return Session[UserInfoKey] as P_User;
            }
            set
            {
                Session[UserInfoKey] = value;
            }
        }


        /// <summary>
        /// 用户角色列表
        /// </summary>
        public List<int> Session_UsrRole
        {
            get
            {
                return Session[UserRoleKey] as List<int>;
            }
            set
            {
                Session[UserRoleKey] = value;
            }
        }


        /// <summary>
        /// 用户权限集合
        /// </summary>
        public List<P_Permission> Session_UsrPermission
        {
            get
            {
                return Session[UserPermissionKey] as List<P_Permission>;
            }
            set
            {
                Session[UserPermissionKey] = value;
            }
        }

        /// <summary>
        /// EasyUI 菜单Json 字符串
        /// </summary>
        public string Session_UserTreeJsonStr
        {
            get
            {
                //if (Session[UserJsonTreeKey] == null)
                //{
                //将 登陆用户的 权限集合 转成 树节点 集合（其中 IsShow = false的不要生成到树节点集合中）
                List<Model.EasyUIModel.TreeNode> listTree = Model.P_Permission.ToTreeNodes(Session_UsrPermission.Where(p => p.IsShow == true).OrderBy(p => p.SN).ToList());
                Session[UserJsonTreeKey] = JsonHelper.Obj2JsonStr(listTree);
                //}
                return Session[UserJsonTreeKey].ToString();
            }
        }


        /// <summary>
        /// 仓储相关
        /// </summary>
        public StorageRelatedInfo Session_StorageRelated
        {
            get
            {
                return Session[UserStorageRelatedKey] as StorageRelatedInfo;
            }
            set
            {
                Session[UserStorageRelatedKey] = value;
            }
        }
        public bool getGongneng(int id)
        {
            return UserOperateContext.Current.Session_UsrPermission.SingleOrDefault(p => p.ID == id) != null ? true : false;
        }
        public bool getRole(int id)
        {
            return UserOperateContext.Current.Session_UsrRole.SingleOrDefault(p => p == id) != null ? true : false;
        }
        public bool getGongneng(E_IMPermisson eimp)
        {
            int id = Convert.ToInt32(eimp);
            return UserOperateContext.Current.Session_UsrPermission.SingleOrDefault(p => p.ID == id) != null ? true : false;
        }

        /// <summary>
        /// 获取当前用户最大统计权限
        /// </summary>
        /// <returns></returns>
        public E_StatisticsPermisson getMaxPerForStatistics()
        {
            E_StatisticsPermisson retrunValue;
            List<int> listStatisticsPer = new List<int>(new int[] { 57, 58, 59, 60 });
            List<P_Permission> tempList = UserOperateContext.Current.Session_UsrPermission.Where(p => listStatisticsPer.Contains(p.ID)).ToList();
            if (tempList != null)
            {
                retrunValue = (E_StatisticsPermisson)tempList.Max(p => p.ID);
            }
            else
            {
                retrunValue = E_StatisticsPermisson.None;
            }
            return retrunValue;
        }

        #endregion

        #region 获取当前用户角色权限--病历(医生、护士、司机)
        /// <summary>
        /// 获取当前用户角色权限
        /// </summary>
        /// <returns></returns>
        public string getMaxPerForRole()
        {
            string retrunValue="";
            List<int> listRolePer = new List<int>(new int[] { 1, 3, 10 });
            List<int> tempList = UserOperateContext.Current.Session_UsrRole.Where(p => listRolePer.Contains(p)).ToList();
            if (tempList.Count>0)
            {
                switch (tempList.Min(p => p))
                {
                    case 1:
                        retrunValue = "Doctor";
                        break;
                    case 3:
                        retrunValue = "Nurse";
                        break;
                    case 10:
                        retrunValue = "Driver";
                        break;
                    //case 31:
                    //    retrunValue = "FirstAider";
                    //    break;
                    default:
                        break;
                }
            }
            else
            {
                retrunValue = "";
            }
            return retrunValue;
        }
        #endregion

        #region  Cookie 操作暂时不用
        /// <summary>
        /// 从cookie中获取当前用户对象
        /// </summary>
        public string CookieUserValue
        {
            get
            {
                return Response.Cookies[UserInfoKey].Value;
            }
        }


        /// <summary>
        /// cookie 操作
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        private void CookieOper(string key, string value)
        {
            HttpCookie cookie = new HttpCookie(key, value);
            cookie.Expires = DateTime.Now.AddDays(1);
            cookie.Path = UserCookiePath;  //根目录
            Response.Cookies.Add(cookie);   // 添加Cookie信息
        }

        #endregion




    }
}
