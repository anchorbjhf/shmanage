using Anke.SHManage.Model.BasicEventInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.IDAL
{
   public  interface IAcceptEventDAL
    {
       /// <summary>
        /// 受理相关信息
       /// </summary>
       /// <param name="code">事件编码</param>
       /// <param name="orderNum">受理序号</param>
       /// <returns></returns>
       AcceptEventInfo GetAcceptEventInfoByCode(string code, int orderNum);

    /// <summary>
    /// 获取事件节点(分配tabs使用）
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
       AcceptEventInfo GetEventNode(string code);
       /// <summary>
       /// 事件相关信息
       /// </summary>
       /// <param name="code"></param>
       /// <param name="orderNum"></param>
       /// <returns></returns>
     
    }
}
