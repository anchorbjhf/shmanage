using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.Model.BasicEventInfo
{
    public class AcceptEventInfo
    {
        private string m_EventCode;//事件编码
        //[Column(IsPrimaryKey = true, Name = "事件编码", DbType = "char(16)", Storage = "m_EventCode")]
        public string EventCode
        {
            get { return m_EventCode; }
            set { m_EventCode = value; }
        }

        private int m_AcceptOrder;//受理序号
        //[Column(IsPrimaryKey = true, Name = "受理序号", DbType = "int", Storage = "m_AcceptOrder")]
        public int AcceptOrder
        {
            get { return m_AcceptOrder; }
            set { m_AcceptOrder = value; }
        }

        private int m_TypeId;//受理类型编码
       // [Column(Name = "受理类型编码", DbType = "int", Storage = "m_TypeId", UpdateCheck = UpdateCheck.Never)]
        public int TypeId
        {
            get { return m_TypeId; }
            set { m_TypeId = value; }
        }

        private int m_DetailReasonId;//受理类型子编码
      //  [Column(Name = "受理类型子编码", DbType = "int", Storage = "m_DetailReasonId", UpdateCheck = UpdateCheck.Never)]
        public int DetailReasonId
        {
            get { return m_DetailReasonId; }
            set { m_DetailReasonId = value; }
        }

        private string m_AcceptPersonCode;//责任受理人编码
        //[Column(Name = "责任受理人编码", DbType = "char(5)", Storage = "m_AcceptPersonCode", UpdateCheck = UpdateCheck.Never)]
        public string AcceptPersonCode
        {
            get { return m_AcceptPersonCode; }
            set { m_AcceptPersonCode = value; }
        }
        private string m_AlarmTel;//呼救电话
       // [Column(Name = "呼救电话", DbType = "varchar(14)", Storage = "m_AlarmTel", UpdateCheck = UpdateCheck.Never)]
        public string AlarmTel
        {
            get { return m_AlarmTel; }
            set { m_AlarmTel = value; }
        }

        //private Nullable<DateTime> m_HangUpTime = null;//挂起时刻
        //[Column(Name = "挂起时刻", Storage = "m_HangUpTime", UpdateCheck = UpdateCheck.Never)]
        //public Nullable<DateTime> HangUpTime
        //{
        //    get { return m_HangUpTime; }
        //    set { m_HangUpTime = value; }
        //}

        private Nullable<DateTime> m_RingTime = null;//电话震铃时刻
       // [Column(Name = "电话振铃时刻", Storage = "m_RingTime", UpdateCheck = UpdateCheck.Never)]
        public Nullable<DateTime> RingTime
        {
            get { return m_RingTime; }
            set { m_RingTime = value; }
        }

        private Nullable<DateTime> m_AcceptBeginTime = null;//开始受理时刻
      //  [Column(Name = "开始受理时刻", Storage = "m_AcceptBeginTime", UpdateCheck = UpdateCheck.Never)]
        public Nullable<DateTime> AcceptBeginTime
        {
            get { return m_AcceptBeginTime; }
            set { m_AcceptBeginTime = value; }
        }

        private Nullable<DateTime> m_AcceptEndTime = null;//结束受理时刻
      //  [Column(Name = "结束受理时刻", Storage = "m_AcceptEndTime", UpdateCheck = UpdateCheck.Never)]
        public Nullable<DateTime> AcceptEndTime
        {
            get { return m_AcceptEndTime; }
            set { m_AcceptEndTime = value; }
        }

        private Nullable<DateTime> m_CommandTime = null;//发送指令时刻
      //  [Column(Name = "发送指令时刻", Storage = "m_CommandTime", UpdateCheck = UpdateCheck.Never)]
        public Nullable<DateTime> CommandTime
        {
            get { return m_CommandTime; }
            set { m_CommandTime = value; }
        }

        private string m_LocalAddr;//现场地址
     //   [Column(Name = "现场地址", DbType = "varchar(200)", Storage = "m_LocalAddr", UpdateCheck = UpdateCheck.Never)]
        public string LocalAddr
        {
            get { return m_LocalAddr; }
            set { m_LocalAddr = value; }
        }

        private string m_WaitAddr;//等车地址
     //   [Column(Name = "等车地址", DbType = "varchar(200)", Storage = "m_WaitAddr", UpdateCheck = UpdateCheck.Never)]
        public string WaitAddr
        {
            get { return m_WaitAddr; }
            set { m_WaitAddr = value; }
        }

        private string m_SendAddr;//送往地点
     //   [Column(Name = "送往地点", DbType = "varchar(200)", Storage = "m_SendAddr", UpdateCheck = UpdateCheck.Never)]
        public string SendAddr
        {
            get { return m_SendAddr; }
            set { m_SendAddr = value; }
        }

        private int m_LocalAddrTypeId;//往救地点类型编码
       // [Column(Name = "往救地点类型编码", DbType = "int", Storage = "m_LocalAddrTypeId", UpdateCheck = UpdateCheck.Never)]
        public int LocalAddrTypeId
        {
            get { return m_LocalAddrTypeId; }
            set { m_LocalAddrTypeId = value; }
        }

        private int m_SendAddrTypeId;//送往地点类型编码
     //   [Column(Name = "送往地点类型编码", DbType = "int", Storage = "m_SendAddrTypeId", UpdateCheck = UpdateCheck.Never)]
        public int SendAddrTypeId
        {
            get { return m_SendAddrTypeId; }
            set { m_SendAddrTypeId = value; }
        }

        private string m_LinkMan;//联系人
      //  [Column(Name = "联系人", DbType = "varchar(50)", Storage = "m_LinkMan", UpdateCheck = UpdateCheck.Never)]
        public string LinkMan
        {
            get { return m_LinkMan; }
            set { m_LinkMan = value; }
        }

        private string m_LinkTel;//联系电话
     //   [Column(Name = "联系电话", DbType = "varchar(14)", Storage = "m_LinkTel", UpdateCheck = UpdateCheck.Never)]
        public string LinkTel
        {
            get { return m_LinkTel; }
            set { m_LinkTel = value; }
        }

        private string m_Extension;//分机
       // [Column(Name = "分机", DbType = "varchar(14)", Storage = "m_Extension", UpdateCheck = UpdateCheck.Never)]
        public string Extension
        {
            get { return m_Extension; }
            set { m_Extension = value; }
        }

        private string m_PatientName;//患者姓名
      //  [Column(Name = "患者姓名", DbType = "varchar(50)", Storage = "m_PatientName", UpdateCheck = UpdateCheck.Never)]
        public string PatientName
        {
            get { return m_PatientName; }
            set { m_PatientName = value; }
        }

        private string m_Sex;//性别
      //  [Column(Name = "性别", DbType = "varchar(4)", Storage = "m_Sex", UpdateCheck = UpdateCheck.Never)]
        public string Sex
        {
            get { return m_Sex; }
            set { m_Sex = value; }
        }

        private string m_Age;//年龄
       // [Column(Name = "年龄", DbType = "varchar(10)", Storage = "m_Age", UpdateCheck = UpdateCheck.Never)]
        public string Age
        {
            get { return m_Age; }
            set { m_Age = value; }
        }

        private string m_Folk;//民族
      //  [Column(Name = "民族", DbType = "varchar(50)", Storage = "m_Folk", UpdateCheck = UpdateCheck.Never)]
        public string Folk
        {
            get { return m_Folk; }
            set { m_Folk = value; }
        }

        private string m_National;//国籍
       // [Column(Name = "国籍", DbType = "varchar(50)", Storage = "m_National", UpdateCheck = UpdateCheck.Never)]
        public string National
        {
            get { return m_National; }
            set { m_National = value; }
        }

        private string m_AlarmReason;//主诉
       // [Column(Name = "主诉", DbType = "varchar(100)", Storage = "m_AlarmReason", UpdateCheck = UpdateCheck.Never)]
        public string AlarmReason
        {
            get { return m_AlarmReason; }
            set { m_AlarmReason = value; }
        }

        private string m_Judge;//病种判断
      //  [Column(Name = "病种判断", DbType = "varchar(100)", Storage = "m_Judge", UpdateCheck = UpdateCheck.Never)]
        public string Judge
        {
            get { return m_Judge; }
            set { m_Judge = value; }
        }

        private int m_IllStateId;//病情编码
       // [Column(Name = "病情编码", DbType = "int", Storage = "m_IllStateId", UpdateCheck = UpdateCheck.Never)]
        public int IllStateId
        {
            get { return m_IllStateId; }
            set { m_IllStateId = value; }
        }

        private bool m_IsNeedLitter;//是否需要担架
       // [Column(Name = "是否需要担架", DbType = "bit", Storage = "m_IsNeedLitter", UpdateCheck = UpdateCheck.Never)]
        public bool IsNeedLitter
        {
            get { return m_IsNeedLitter; }
            set { m_IsNeedLitter = value; }
        }

        private int m_PatientCount;//患者人数
       // [Column(Name = "患者人数", DbType = "int", Storage = "m_PatientCount", UpdateCheck = UpdateCheck.Never)]
        public int PatientCount
        {
            get { return m_PatientCount; }
            set { m_PatientCount = value; }
        }

        private string m_SpecialNeed;//特殊要求
     //   [Column(Name = "特殊要求", DbType = "varchar(50)", Storage = "m_SpecialNeed", UpdateCheck = UpdateCheck.Never)]
        public string SpecialNeed
        {
            get { return m_SpecialNeed; }
            set { m_SpecialNeed = value; }
        }

        private bool m_IsLabeled;//是否标注
      //  [Column(Name = "是否标注", DbType = "bit", Storage = "m_IsLabeled", UpdateCheck = UpdateCheck.Never)]
        public bool IsLabeled
        {
            get { return m_IsLabeled; }
            set { m_IsLabeled = value; }
        }

        private double m_X;//X坐标
       // [Column(Name = "X坐标", DbType = "float", Storage = "m_X", UpdateCheck = UpdateCheck.Never)]
        public double X
        {
            get { return m_X; }
            set { m_X = value; }
        }

        private double m_Y;//Y坐标
      //  [Column(Name = "Y坐标", DbType = "float", Storage = "m_Y", UpdateCheck = UpdateCheck.Never)]
        public double Y
        {
            get { return m_Y; }
            set { m_Y = value; }
        }

        private string m_AmbulanceList;//派车列表
      //  [Column(Name = "派车列表", DbType = "varchar(1000)", Storage = "m_AmbulanceList", UpdateCheck = UpdateCheck.Never)]
        public string AmbulanceList
        {
            get { return m_AmbulanceList; }
            set { m_AmbulanceList = value; }
        }

        private string m_BackUpOne;//保留字段1
       // [Column(Name = "保留字段1", DbType = "varchar(100)", Storage = "m_BackUpOne", UpdateCheck = UpdateCheck.Never)]
        public string BackUpOne
        {
            get { return m_BackUpOne; }
            set { m_BackUpOne = value; }
        }
        private string m_BackUpTwo;//保留字段2
      //  [Column(Name = "保留字段2", DbType = "varchar(100)", Storage = "m_BackUpTwo", UpdateCheck = UpdateCheck.Never)]
        public string BackUpTwo
        {
            get { return m_BackUpTwo; }
            set { m_BackUpTwo = value; }
        }

        private string m_Remark;//备注
       // [Column(Name = "备注", DbType = "varchar(1000)", Storage = "m_Remark", UpdateCheck = UpdateCheck.Never)]
        public string Remark
        {
            get { return m_Remark; }
            set { m_Remark = value; }
        }

        private int m_CenterId;//中心编码
      //  [Column(Name = "中心编码", DbType = "int", Storage = "m_CenterId", UpdateCheck = UpdateCheck.Never)]
        public int CenterId
        {
            get { return m_CenterId; }
            set { m_CenterId = value; }
        }
        /// <summary>
        /// 受理类型
        /// </summary>
        private string m_AcceptType;

        public string AcceptType
        {
            get { return m_AcceptType; }
            set { m_AcceptType = value; }
        }

        private string m_Dispatcher;
        /// <summary>
        /// 责任调度员姓名
        /// </summary>
        public string Dispatcher
        {
            get { return m_Dispatcher; }
            set { m_Dispatcher = value; }
        }
        private string m_AcceptPersonWorkID;
        /// <summary>
        /// 责任受理人工号
        /// </summary>
        public string AcceptPersonWorkID
        {
            get { return m_AcceptPersonWorkID; }
            set { m_AcceptPersonWorkID = value; }
        }
        private string m_IllState;//
        /// <summary>
        /// 病情
        /// </summary>
        public string IllState
        {
            get { return m_IllState; }
            set { m_IllState = value; }
        }
        private string m_LocalAddrType;//
        /// <summary>
        /// 往救地点类型
        /// </summary>
        public string LocalAddrType
        {
            get { return m_LocalAddrType; }
            set { m_LocalAddrType = value; }
        }

        private string m_SendAddrType;//
        /// <summary>
        /// 送往地点类型
        /// </summary>
        public string SendAddrType
        {
            get { return m_SendAddrType; }
            set { m_SendAddrType = value; }
        }
    }
}
