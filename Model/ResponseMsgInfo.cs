using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.Model
{

    /// <summary>
    /// 反馈消息
    /// </summary>
    public class ResponseMsgInfo
    {
        /// <summary>
        /// 反馈消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 结果 ok error nologin
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// 重新定向地址
        /// </summary>
        public string RedirectURL { get; set; }

        /// <summary>
        /// 数据对象
        /// </summary>
        public object Data { get; set; }
    }
}
