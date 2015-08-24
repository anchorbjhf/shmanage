using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.Model
{

    /// <summary>
    /// 收费任务实体
    /// </summary>
    public class MobileTaskInfo
    {

        private string m_RenWuBianMa;
        /// <summary> 
        /// 任务编码
        /// </summary> 
        public string RenWuBianMa
        {
            get { return m_RenWuBianMa; }
            set { this.m_RenWuBianMa = value; }
        }

        private string m_EventType;
        /// <summary>
        /// 事件类型
        /// </summary>
        public string EventType
        {
            get { return m_EventType; }
            set { m_EventType = value; }
        }
        private string m_EventTypeName;
        /// <summary>
        /// 事件类型信息
        /// </summary>
        public string EventTypeName
        {
            get { return m_EventTypeName; }
            set { m_EventTypeName = value; }
        }

        private string m_YongHuLiuShuiHao;
        /// <summary> 
        /// 用户流水号
        /// </summary> 
        public string YongHuLiuShuiHao
        {
            get { return m_YongHuLiuShuiHao; }
            set { this.m_YongHuLiuShuiHao = value; }
        }
        private string m_XianChangDiZhi;
        /// <summary> 
        /// 现场地址
        /// </summary> 
        public string XianChangDiZhi
        {
            get { return m_XianChangDiZhi; }
            set { this.m_XianChangDiZhi = value; }
        }
        private string m_DaoDaXianChangShiKe;
        /// <summary> 
        /// 到达现场时刻
        /// </summary> 
        public string DaoDaXianChangShiKe
        {
            get { return m_DaoDaXianChangShiKe; }
            set { this.m_DaoDaXianChangShiKe = value; }
        }
        private string m_HuanZheXingMing;
        /// <summary> 
        /// 患者姓名
        /// </summary> 
        public string HuanZheXingMing
        {
            get { return m_HuanZheXingMing; }
            set { this.m_HuanZheXingMing = value; }
        }
        private string m_SiJi;
        /// <summary> 
        /// 司机
        /// </summary> 
        public string SiJi
        {
            get { return m_SiJi; }
            set { this.m_SiJi = value; }
        }
        private string m_YiSheng;
        /// <summary> 
        /// 医生
        /// </summary> 
        public string YiSheng
        {
            get { return m_YiSheng; }
            set { this.m_YiSheng = value; }
        }
        private string m_HuShi;
        /// <summary> 
        /// 护士
        /// </summary> 
        public string HuShi
        {
            get { return m_HuShi; }
            set { this.m_HuShi = value; }
        }
        private string m_State;
        /// <summary>
        /// 状态
        /// </summary>
        public string State
        {
            get { return m_State; }
            set { m_State = value; }
        }

    }
}
