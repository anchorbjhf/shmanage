using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.Model
{
    public class AlarmEventInfo
    {
        private string m_EventCode;//事件编码
        // [Column(IsPrimaryKey = true, Name = "事件编码", DbType = "char(16)", Storage = "m_EventCode")]
        public string EventCode
        {
            get { return m_EventCode; }
            set { m_EventCode = value; }
        }
        private string m_FirstAlarmCall;//首次呼救电话
        //[Column(Name = "首次呼救电话", DbType = "varchar(14) not null", Storage = "m_FirstAlarmCall")]
        public string FirstAlarmCall
        {
            get { return m_FirstAlarmCall; }
            set { m_FirstAlarmCall = value; }
        }
        private string m_EvetnName;//事件名称
        // [Column(Name="事件名称",DbType="varchar(200) not null",Storage="m_EvetnName",UpdateCheck=UpdateCheck.Never)]
        public string EvetnName
        {
            get { return m_EvetnName; }
            set { m_EvetnName = value; }
        }
        private int m_EventSourceCode;//事件来源编码
        // [Column(Name="事件来源编码",DbType="int not null",Storage="m_EventSourceCode",UpdateCheck=UpdateCheck.Never)]
        public int EventSourceCode
        {
            get { return m_EventSourceCode; }
            set { m_EventSourceCode = value; }
        }
        private int m_EventTypeCode;//事件类型编码
        //  [Column(Name="事件类型编码",DbType="int not null",Storage="m_EventTypeCode",UpdateCheck=UpdateCheck.Never)]
        public int EventTypeCode
        {
            get { return m_EventTypeCode; }
            set { m_EventTypeCode = value; }
        }
        private int m_AccidentTypeCode;//事故类型编码
        //  [Column(Name="事故类型编码",DbType="int not null",Storage="m_AccidentTypeCode",UpdateCheck=UpdateCheck.Never)]
        public int AccidentTypeCode
        {
            get { return m_AccidentTypeCode; }
            set { m_AccidentTypeCode = value; }
        }
        private int m_AccidentLevelCode;//事故等级编码
        //   [Column(Name="事故等级编码",DbType="int not null",Storage="m_AccidentLevelCode",UpdateCheck=UpdateCheck.Never)]
        public int AccidentLevelCode
        {
            get { return m_AccidentLevelCode; }
            set { m_AccidentLevelCode = value; }
        }
        private bool m_IsTest;//是否测试
        //  [Column(Name="是否测试",DbType="bit not null",Storage="m_IsTest",UpdateCheck=UpdateCheck.Never)]
        public bool IsTest
        {
            get { return m_IsTest; }
            set { m_IsTest = value; }
        }
        private int m_AcceptCount;//受理次数
        // [Column(Name="受理次数",DbType="int not null",Storage="m_AcceptCount",UpdateCheck=UpdateCheck.Never)]
        public int AcceptCount
        {
            get { return m_AcceptCount; }
            set { m_AcceptCount = value; }
        }
        private int m_CancelAcceptCount;//撤消受理数
        //  [Column(Name="撤消受理数",DbType="int not null",Storage="m_CancelAcceptCount",UpdateCheck=UpdateCheck.Never)]
        public int CancelAcceptCount
        {
            get { return m_CancelAcceptCount; }
            set { m_CancelAcceptCount = value; }
        }
        private int m_TransactTaskCount;//执行任务总数
        // [Column(Name="执行任务总数",DbType="int not null",Storage="m_TransactTaskCount",UpdateCheck=UpdateCheck.Never)]
        public int TransactTaskCount
        {
            get { return m_TransactTaskCount; }
            set { m_TransactTaskCount = value; }
        }
        private int m_NonceTransactTaskCount;//当前执行任务数
        //  [Column(Name = "当前执行任务数", DbType = "int not null", Storage = "m_NonceTransactTaskCount", UpdateCheck = UpdateCheck.Never)]
        public int NonceTransactTaskCount
        {
            get { return m_NonceTransactTaskCount; }
            set { m_NonceTransactTaskCount = value; }
        }
        private int m_BreakTaskCount;//中止任务数
        // [Column(Name="中止任务数",DbType="int not null",Storage="m_BreakTaskCount",UpdateCheck=UpdateCheck.Never)]
        public int BreakTaskCount
        {
            get { return m_BreakTaskCount; }
            set { m_BreakTaskCount = value; }
        }
        private string m_FirstDisptcher;//首次调度员编码
        // [Column(Name="首次调度员编码",DbType="char(5) not null",Storage="m_FirstDisptcher",UpdateCheck=UpdateCheck.Never)]
        public string FirstDisptcher
        {
            get { return m_FirstDisptcher; }
            set { m_FirstDisptcher = value; }
        }

        private Nullable<DateTime> m_FirstAcceptTime = null;//首次受理时刻
        // [Column(Name="首次受理时刻",DbType="datetime not null",Storage="m_FirstAcceptTime",UpdateCheck=UpdateCheck.Never)]
        public Nullable<DateTime> FirstAcceptTime
        {
            get { return m_FirstAcceptTime; }
            set { m_FirstAcceptTime = value; }
        }
        private Nullable<DateTime> m_HangUpTime;//挂起时刻
        //  [Column(Name="挂起时刻",DbType="datetime not null",Storage="m_HangUpTime",UpdateCheck=UpdateCheck.Never)]
        public Nullable<DateTime> HangUpTime
        {
            get { return m_HangUpTime; }
            set { m_HangUpTime = value; }
        }
        private Nullable<DateTime> m_BespeakTime;//预约时刻
        // [Column(Name="预约时刻",DbType="datetime not null",Storage="m_BespeakTime",UpdateCheck=UpdateCheck.Never)]
        public Nullable<DateTime> BespeakTime
        {
            get { return m_BespeakTime; }
            set { m_BespeakTime = value; }
        }
        private Nullable<DateTime> m_FirstSendAmbTime;//首次派车时刻
        // [Column(Name="首次派车时刻",DbType="datetime not null",Storage="m_FirstSendAmbTime",UpdateCheck=UpdateCheck.Never)]
        public Nullable<DateTime> FirstSendAmbTime
        {
            get { return m_FirstSendAmbTime; }
            set { m_FirstSendAmbTime = value; }
        }
        private bool m_IsHangUp;//是否挂起
        //  [Column(Name="是否挂起",DbType="bit not null",Storage="m_IsHangUp",UpdateCheck=UpdateCheck.Never)]
        public bool IsHangUp
        {
            get { return m_IsHangUp; }
            set { m_IsHangUp = value; }
        }
        private bool m_IsLabel;//是否标注
        // [Column(Name="是否标注",DbType="bit not null",Storage="m_IsLabel",UpdateCheck=UpdateCheck.Never)]
        public bool IsLabel
        {
            get { return m_IsLabel; }
            set { m_IsLabel = value; }
        }

        private string m_AccidentCode;
        /// <summary>
        /// 所属事故编码
        /// </summary>
        // [Column(Name = "所属事故编码", DbType = "char(16)", Storage = "m_AccidentCode", UpdateCheck = UpdateCheck.Never)]
        public string AccidentCode
        {
            get { return m_AccidentCode; }
            set { m_AccidentCode = value; }
        }
        private double m_X;//X坐标
        //  [Column(Name="X坐标",DbType="float not null",Storage="m_X",UpdateCheck=UpdateCheck.Never)]
        public double X
        {
            get { return m_X; }
            set { m_X = value; }
        }
        private double m_Y;//Y坐标
        // [Column(Name="Y坐标",DbType="float not null",Storage="m_Y",UpdateCheck=UpdateCheck.Never)]
        public double Y
        {
            get { return m_Y; }
            set { m_Y = value; }
        }
        private string m_Area;//区域
        //  [Column(Name="区域",DbType="varchar(100)",Storage="m_Area",UpdateCheck=UpdateCheck.Never)]
        public string Area
        {
            get { return m_Area; }
            set { m_Area = value; }
        }
        private string m_TieUpDeskCode;//占用台号
        // [Column(Name="占用台号",DbType="char(2)",Storage="m_TieUpDeskCode",UpdateCheck=UpdateCheck.Never)]
        public string TieUpDeskCode
        {
            get { return m_TieUpDeskCode; }
            set { m_TieUpDeskCode = value; }
        }
        private int m_CenterCode;//中心编码
        // [Column(Name="中心编码",DbType="int not null",Storage="m_CenterCode",UpdateCheck=UpdateCheck.Never)]
        public int CenterCode
        {
            get { return m_CenterCode; }
            set { m_CenterCode = value; }
        }
        private bool m_IsFinish;//表单完成标志
        // [Column(Name="表单完成标志",DbType="bit not null",Storage="m_IsFinish",UpdateCheck=UpdateCheck.Never)]
        public bool IsFinish
        {
            get { return m_IsFinish; }
            set { m_IsFinish = value; }
        }

        /// <summary>
        /// 事件来源
        /// </summary>
        private string m_EventSource;

        public string EventSource
        {
            get { return m_EventSource; }
            set { m_EventSource = value; }
        }
        /// <summary>
        /// 事件类型
        /// </summary>
        private string m_EventType;

        public string EventType
        {
            get { return m_EventType; }
            set { m_EventType = value; }
        }
        /// <summary>
        /// 事故类型
        /// </summary>
        private string m_AccidentType;

        public string AccidentType
        {
            get { return m_AccidentType; }
            set { m_AccidentType = value; }
        }
        /// <summary>
        /// 事故等级
        /// </summary>
        private string m_AccidentLevel;

        public string AccidentLevel
        {
            get { return m_AccidentLevel; }
            set { m_AccidentLevel = value; }
        }

        private string m_FirstDisptcherName;
        /// <summary>
        /// 首次受理调度名称
        /// </summary>
        public string FirstDisptcherName
        {
            get { return m_FirstDisptcherName; }
            set { m_FirstDisptcherName = value; }
        }
        private string m_FirstDisptcherWorkID;
        /// <summary>
        /// 首次调度员工号
        /// 2013.04.11 刘爱青 add
        /// </summary>
        public string FirstDisptcherWorkID
        {
            get { return m_FirstDisptcherWorkID; }
            set { m_FirstDisptcherWorkID = value; }
        }
        private string m_CenterName;
        /// <summary>
        /// 中心名称
        /// </summary>
        public string CenterName
        {
            get { return m_CenterName; }
            set { m_CenterName = value; }
        }
    }
}
