using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.Model
{
    /// <summary>
    /// 说明：任务列表-出车信息
    /// 时间：2015-01-20
    /// </summary>
    public class M_TaskPartInfo
    {
        /// <summary>
        /// 任务编码
        /// </summary>
        public string TaskCode { get; set; }
        /// <summary>
        /// 首次受理时刻
        /// </summary>
        public string FirstAcceptTime { get; set; }
        /// <summary>
        /// 患者姓名
        /// </summary>
        public string PatientName { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string ContactTelephone { get; set; }
        /// <summary>
        /// 事件类型
        /// </summary>
        public string EventType { get; set; }
        /// <summary>
        /// 现场地址
        /// </summary>
        public string SceneAddress { get; set; }
        /// <summary>
        /// 派车时刻
        /// </summary>
        public string SendCarTime { get; set; }
        /// <summary>
        /// 受理类型
        /// </summary>
        public string AcceptType { get; set; }
        /// <summary>
        /// 分站名称
        /// </summary>
        public string Station { get; set; }
        /// <summary>
        /// 实际标识
        /// </summary>
        public string ActualLogo { get; set; }
        /// <summary>
        /// 司机
        /// </summary>
        public string Driver { get; set; }
        /// <summary>
        /// 医生
        /// </summary>
        public string Doctor { get; set; }
        /// <summary>
        /// 护士
        /// </summary>
        public string Nurse { get; set; }
        /// <summary>
        /// 担架工
        /// </summary>
        public string StretcherBearers { get; set; }
        /// <summary>
        /// 出车结果
        /// </summary>
        public string OutCarResults { get; set; }
        /// <summary>
        /// 病历数
        /// </summary>
        public string PatientNumber { get; set; }
        /// <summary>
        /// 收费数
        /// </summary>
        public string ChargeNumber { get; set; }
        /// <summary>
        /// 是否提交
        /// </summary>
        public string IsSubmit { get; set; }

    }
}
