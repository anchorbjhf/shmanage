using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Anke.SHManage.Utility
{
    public class AppConfig
    {
        /// <summary>
        /// 得到AppSettings中的配置字符串信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetConfigString(string key)
        {
            string CacheKey = "AppSettings-" + key;
            object objModel = CacheHelper.GetCache(CacheKey);
            if (objModel == null)
            {
                try
                {
                    objModel = ConfigurationManager.AppSettings[key];
                    if (objModel != null)
                    {
                        CacheHelper.SetCache(CacheKey, objModel);
                    }
                }
                catch
                { }
            }
            return objModel.ToString();
        }
        public static string GetConnectionString(string key)
        {
            string CacheKeyNew = "AppSettings-" + key;
            object objModel = CacheHelper.GetCache(CacheKeyNew);
            if (objModel == null)
            {
                try
                {
                    objModel = ConfigurationManager.ConnectionStrings[key].ConnectionString;
                    if (objModel != null)
                    {
                        CacheHelper.SetCache(CacheKeyNew, objModel);
                    }
                }
                catch
                { }
            }
            return objModel.ToString();
        }

        /// <summary>
        /// 得到AppSettings中的配置Bool信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool GetConfigBool(string key)
        {
            bool result = false;
            string cfgVal = GetConfigString(key);
            if (null != cfgVal && string.Empty != cfgVal)
            {
                try
                {
                    result = bool.Parse(cfgVal);
                }
                catch (FormatException)
                {
                    // Ignore format exceptions.
                }
            }
            return result;
        }
        /// <summary>
        /// 得到AppSettings中的配置Decimal信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static decimal GetConfigDecimal(string key)
        {
            decimal result = 0;
            string cfgVal = GetConfigString(key);
            if (null != cfgVal && string.Empty != cfgVal)
            {
                try
                {
                    result = decimal.Parse(cfgVal);
                }
                catch (FormatException)
                {
                    // Ignore format exceptions.
                }
            }

            return result;
        }
        /// <summary>
        /// 得到AppSettings中的配置int信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int GetConfigInt(string key)
        {
            int result = 0;
            string cfgVal = GetConfigString(key);
            if (null != cfgVal && string.Empty != cfgVal)
            {
                try
                {
                    result = int.Parse(cfgVal);
                }
                catch (FormatException)
                {
                    // Ignore format exceptions.
                }
            }

            return result;
        }
        public static Int32 GetInt32ConfigValue(string keyName)
        {
            Regex regex = new Regex("^\\d*$");
            string tempStr;
            try
            {
                tempStr = ConfigurationManager.AppSettings[keyName].ToString();
                if (regex.IsMatch(tempStr))
                {
                    return Convert.ToInt32(tempStr);
                }

                return 0;
            }
            catch
            {
                return 0;
            }
        }
    }
}
