using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.Utility
{
    public class DBConvert
    {

        #region 跟时间有关的转换
        /// <summary>
        /// 将时间范型转为数据库类型，处理空值
        /// </summary>
        /// <param name="arg">可为空时间范型</param>
        /// <returns>数据库类型</returns>
        public static string ConvertNullableToDBType(Nullable<DateTime> arg)
        {
            return arg.Equals(null) ? "null" : arg.Value.ToString();
        }
        /// <summary>
        /// 将数据库类型转为时间范型，处理空值
        /// </summary>
        /// <param name="arg">数据库值</param>
        /// <returns>时间范型</returns>
        public static Nullable<DateTime> ConvertDBTypeToNullable(object arg)
        {
            return arg.Equals(DBNull.Value) ? (Nullable<DateTime>)null : (Nullable<DateTime>)arg;
        }
        /// <summary>
        /// 页面上，get时，转为{0:yyyy-MM-dd HH:mm:ss}或""
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static string ConvertDateTimeToNullable(Nullable<DateTime> arg)
        {
            return arg.Equals(null) ? "" : arg.Value.ToString();
        }
        /// <summary>
        /// 页面上，get时，转为{0:yyyy-MM-dd}或""
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static string ConvertDateTimeToDate(Nullable<DateTime> arg)
        {
            return arg.Equals(DBNull.Value) ? "" : String.Format("{0:yyyy-MM-dd}", arg);
        }
        /// <summary>
        /// 页面上，get时，转为{0:yyyy-MM}或""
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static string ConvertDateTimeToMonth(Nullable<DateTime> arg)
        {
            return arg.Equals(DBNull.Value) ? "" : String.Format("{0:yyyy-MM}", arg);
        }
        /// <summary>
        /// 页面上，get时，转为{0:yyyy}或""
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static string ConvertDateTimeToYear(Nullable<DateTime> arg)
        {
            return arg.Equals(DBNull.Value) ? "" : String.Format("{0:yyyy}", arg);
        }
        /// <summary>
        /// 页面上，get时，转为{0:MM-dd}或""
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static string ConvertDateTimeToShotDate(Nullable<DateTime> arg)
        {
            return arg.Equals(DBNull.Value) ? "" : String.Format("{0:MM-dd}", arg);
        }
        /// <summary>
        /// 页面上，get时，转为{0:yyyy-MM}或""
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static string ConvertDateTimeToShotMonth(Nullable<DateTime> arg)
        {
            return arg.Equals(DBNull.Value) ? "" : String.Format("{0:yyyy-MM-1}", arg);
        }
        /// <summary>
        /// 页面上，get时，转为{0:HH:mm:ss}或""
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static string ConvertDateTimeToTime(Nullable<DateTime> arg)
        {
            return arg.Equals(DBNull.Value) ? "" : String.Format("{0:HH:mm:ss}", arg);
        }
        /// <summary>
        /// 页面上，get时，转为{0:HH:mm}或""
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static string ConvertDateTimeToShotTime(Nullable<DateTime> arg)
        {
            return arg.Equals(DBNull.Value) ? "" : String.Format("{0:HH:mm}", arg);
        }
        /// <summary>
        /// 页面上，update/insert时，时间范型“”转为null
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static Nullable<DateTime> ConvertNullableToNullableTime(string arg)
        {
            return arg.Equals("") ? (Nullable<DateTime>)null : (Nullable<DateTime>)Convert.ToDateTime(arg);
        }

        public static string ConvertNowToCurrentDate()
        {
            return ConvertDateTimeToDate(DateTime.Now);
        }
        #endregion

        #region 跟整型有关的转换
        /// <summary>
        /// 将整型转为数据库类型,处理空值
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static string ConvertNullableToDBString(Nullable<int> arg)
        {
            return arg.Equals(null) ? "null" : arg.Value.ToString();
        }
        public static string ConvertNullableToDBType(int arg)
        {
            string strNull = "null";
            return arg.Equals(-1) ? strNull : arg.ToString();
        }

        public static object ConvertArrayToDBType(byte[] args)
        {
            return args == null ? (object)DBNull.Value : (object)args;
        }
        public static byte[] ConvertDBTypeToArray(object arg)
        {
            return arg == (object)DBNull.Value ? null : (byte[])arg;
        }
        /// <summary>
        /// 将数据库类型转为int，处理空值
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static Nullable<int> ConvertDBTypeToNullableInt(object arg)
        {
            return arg.Equals(DBNull.Value) ? (Nullable<int>)null : (Nullable<int>)arg;
        }
        public static int ConvertDBTypeToInt(object arg)
        {
            int intValue = -1;
            return arg.Equals(DBNull.Value) ? intValue : (int)arg;
        }
        /// <summary>
        /// 页面上，get时，int空值转为“”
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static string ConvertIntToNullable(Nullable<int> arg)
        {
            return arg.Equals(null) ? "" : arg.Value.ToString();
        }
        /// <summary>
        /// 将整型转为数据库类型,处理空值
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static string ConvertIntTypeToNullable(Nullable<int> arg)
        {
            return arg.Equals(null) ? "null" : arg.Value.ToString();
        }
        /// <summary>
        /// 页面上，update/insert时，int""值转为null
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static Nullable<int> ConvertNullableToNullableInt(string arg)
        {
            return arg.Equals("") ? (Nullable<int>)null : (Nullable<int>)Convert.ToInt32(arg);
        }
        #endregion

        #region 跟单精度有关的转换
        /// <summary>
        /// 将单精度范型转为数据库类型，处理空值
        /// </summary>
        /// <param name="arg">可为空单精度范型</param>
        /// <returns>数据库类型</returns>
        public static string ConvertSingleNullToDBType(Nullable<Single> arg)
        {
            return arg.Equals(null) ? "null" : arg.Value.ToString();
        }
        /// <summary>
        /// 将数据库类型转为单精度范型，处理空值
        /// </summary>
        /// <param name="arg">数据库值</param>
        /// <returns>单精度范型</returns>
        public static Nullable<Single> ConvertDBTypeToSingleNull(object arg)
        {
            return arg.Equals(DBNull.Value) ? (Nullable<Single>)null : (Nullable<Single>)Convert.ToSingle(arg);
        }
        /// <summary>
        /// 页面上，get时，单精度型转为" "
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static string ConvertSingleToNullable(Nullable<Single> arg)
        {
            return arg.Equals(null) ? "" : arg.Value.ToString();
        }
        public static string ConvertSingleNullToDBString(Nullable<Single> arg)
        {
            string strNull = "null";
            return arg.HasValue ? arg.ToString() : strNull;
        }
        /// <summary>
        /// 页面上，update/insert时，float""值转为null
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static Nullable<Single> ConvertNullableToNullableSingle(string arg)
        {
            return arg.Equals("") ? (Nullable<Single>)null : Convert.ToSingle(arg);
        }

        #endregion

        #region 双精度
        /// <summary>
        /// 页面上，update/insert时，double""值转为null
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static Nullable<Double> ConvertNullableToNullableDouble(string arg)
        {
            return arg.Equals("") ? (Nullable<Double>)null : Convert.ToSingle(arg);
        }
        #endregion
        //---------------------------------
        public static Nullable<bool> ConvertStringToNullableBool(object arg)
        {
            return arg.Equals("-1") ? (Nullable<bool>)null : (Nullable<bool>)(Convert.ToBoolean(arg));
        }
        public static Nullable<bool> ConvertDBTypeToNullableBool(object arg)
        {
            return arg.Equals(DBNull.Value) ? (Nullable<bool>)null : (Nullable<bool>)(Convert.ToBoolean(arg));
        }

        public static Nullable<decimal> ConvertDBTypeToNullableDecimal(object arg)
        {
            return arg.Equals(DBNull.Value) ? (Nullable<decimal>)null : (Nullable<decimal>)(Convert.ToDecimal(arg));
        }

      

        public static int ConvertBoolToIntDBType(bool arg)
        {
            return Convert.ToInt32(arg);
        }
        public static string ConvertStringToString(object arg)
        {
            return arg.Equals(DBNull.Value) ? "" : arg.ToString();
        }
        public static string ConvertBoolToCheckType(bool arg)
        {
            return arg.Equals(true) ? "checked" : "";
        }

        public static string ConvertSQLWhereToLike()
        {
            return "";
        }
        public static string ConvertSQLWhere()
        {
            return "";
        }
        /**/
        ///  <summary> 
        /// 过滤sql中非法字符 
        ///  </summary> 
        ///  <param name="value">要过滤的字符串 </param> 
        ///  <returns>string </returns> 
        public static string Filter(string fString)
        {
            if (string.IsNullOrEmpty(fString)) return string.Empty;
            fString = fString.Trim();
            fString = fString.Replace(";", "；");    //分号过滤
            fString = fString.Replace("--", "——");//--过滤
            fString = fString.Replace("'", "’");//单引号过滤
            return fString;
        }
    }
}
