using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Configuration;
using Anke.SHManage.IDAL;

namespace Anke.SHManage.Utility
{
    public sealed class DataAccess
    {
        private static readonly string m_Path = "Anke.SHManage.MSSQLDAL";//读取SQL操作类路径
        private DataAccess() { }

        /// <summary>
        /// 病历工厂
        /// </summary>
        /// <returns></returns>
        public static IM_PatientRecordDAL CreateM_PatientRecordDAL()
        {
            string className = m_Path + ".M_PatientRecordDAL";
            return (IM_PatientRecordDAL)Assembly.Load(m_Path).CreateInstance(className);
        }
        public static IM_DictionaryTreeDAL CreateM_DictionaryTreeDAL()
        {
            string className = m_Path + ".M_DictionaryTreeDAL";
            return (IM_DictionaryTreeDAL)Assembly.Load(m_Path).CreateInstance(className);
        }
        public static IM_DictionaryDAL CreateM_DictionaryDAL()
        {
            string className = m_Path + ".M_DictionaryDAL";
            return (IM_DictionaryDAL)Assembly.Load(m_Path).CreateInstance(className);
        }



        /// <summary>
        /// 手机用
        /// </summary>
        /// <returns></returns>
        public static IMobileTaskDAL GetMobileTaskDAL()
        {
            string className = m_Path + ".MobileTaskDAL";
            return (IMobileTaskDAL)Assembly.Load(m_Path).CreateInstance(className);
        }
        /// <summary>
        /// 基础事件（受理）
        /// </summary>
        /// <returns></returns>
        public static IAcceptEventDAL CreateAcceptEventDAL()
        {
            string className = m_Path + ".AcceptEventDAL";
            return (IAcceptEventDAL)Assembly.Load(m_Path).CreateInstance(className);
        }
        public static IAlarmEventDAL CreateAlarmEventDAL()
        {
            string className = m_Path + ".AlarmEventDAL";
            return (IAlarmEventDAL)Assembly.Load(m_Path).CreateInstance(className);
        }
    }
}
