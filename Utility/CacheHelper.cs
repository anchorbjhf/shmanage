using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;

namespace Anke.SHManage.Utility
{
    public class CacheHelper
    {

        public static int CacheTimeSeconds = AppConfig.GetInt32ConfigValue("CacheTimeSeconds");
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        public static void SetCache(string key, object obj)
        {
            HttpContext.Current.Cache.Insert(key, obj, null, Cache.NoAbsoluteExpiration, TimeSpan.FromSeconds(CacheTimeSeconds));
        }


        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object GetCache(string key)
        {
            return HttpContext.Current.Cache[key];
        }


        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object RemoveCache(string key)
        {
            return HttpContext.Current.Cache.Remove(key);
        }

    }
}
