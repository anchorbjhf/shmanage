using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Anke.SHManage.Utility
{
    public static class JsonHelper
    {
        static JavaScriptSerializer jss = new JavaScriptSerializer();
        /// <summary>
        /// 讲J对象转换成Json字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Obj2JsonStr(object obj)
        {
            return jss.Serialize(obj);
        }
        /// <summary>
        /// JSON反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strJSonContent"></param>
        /// <returns></returns>
        public static T GetJsonInfoBy<T>(string strJSonContent)
        {
            try
            {
                return jss.Deserialize<T>(strJSonContent);
            }
            catch(Exception ex)
            {
                string a = ex.Message;
                return default(T);
            }
        }
    }
}
