using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.Model
{
   public class EventInfo
    {
        /// <summary>
        /// 首次呼救电话
        /// </summary>
        public string callPhone { get; set; }
        /// <summary>
        /// 受理次数
        /// </summary>
        public int acceptTimes { get; set; }
        /// <summary>
        /// 事件名称
        /// </summary>
        public string eventName { get; set; }
        /// <summary>
        /// 首次受理时刻
        /// </summary>
        public string firstAcceptTime { get; set; }
        /// <summary>
        /// 首次调度员
        /// </summary>
        public string firstDispatcher { get; set; }
        /// <summary>
        /// 派车次数
        /// </summary>
        public int sendCarTimes { get; set; }
        /// <summary>
        /// 正常完成
        /// </summary>
        public int finishedTimes { get; set; }

    }
}
