using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

namespace Anke.SHManage.Utility
{
    public class SecurityHelper
    {
        /// <summary>
        /// 使用 票据对象 将 用户数据 加密成字符串
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public static string EncryptUserReleted(string userData)
        {
            //将用户数据 存入 票据对象
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, "AnkeManagement", DateTime.Now, DateTime.Now, true, userData);
            //将票据对象 加密成字符串（可逆）
            string strData = FormsAuthentication.Encrypt(ticket);
            return strData;
        }

        /// <summary>
        /// 加密字符串 解密
        /// </summary>
        /// <param name="cryptograph">加密字符串</param>
        /// <returns></returns>
        public static string DecryptUserInfo(string cryptograph)
        {
            //将 加密字符串 解密成 票据对象
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cryptograph);
            //1.2 将票据里的 用户数据 返回
            return ticket.UserData;
        }


        /// <summary>
        /// 返回 MD5 加密字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetMD5(string s)
        {
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(s);
            bytes = md5.ComputeHash(bytes);
            md5.Clear();

            string ret = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                ret += Convert.ToString(bytes[i], 16).PadLeft(2, '0');
            }

            return ret.PadLeft(32, '0');
        }


    }
}
