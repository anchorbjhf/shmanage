using Anke.SHManage.Model;
using Anke.SHManage.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Anke.SHManage.BLL
{
    /// <summary>
    /// MVC Contrller扩展类
    /// </summary>
    public static class ControllerExtension
    {
        /// <summary>
        ///  发送返回统一格式的JsonResult
        /// </summary>
        /// <param name="controller">控制器方法</param>
        /// <param name="message">消息</param>
        /// <param name="result">结果 ok error noLogin</param>
        /// <param name="data">数据</param>
        /// <param name="redirectURL">跳转URL</param>
        /// <returns></returns>
        public static JsonResult JsonResult(this Controller controller, E_JsonResult result, string message, object data, string redirectURL)
        {
            ResponseMsgInfo rpInfo;
            if (redirectURL != null && redirectURL != "")
            {
               // rpInfo = new ResponseMsgInfo()
               //{
               //    Result = result.ToString(),
               //    Message = message,
               //    Data = data,
               //    RedirectURL = controller.Url.Content(redirectURL)  //注意转换成绝对路径
               //};
                rpInfo = new ResponseMsgInfo();
                rpInfo.Result = result.ToString();
                rpInfo.Message = message;
                rpInfo.Data = data;
                rpInfo.RedirectURL = controller.Url.Content(redirectURL);  //注意转换成绝对路径
            }
            else
            {
                rpInfo = new ResponseMsgInfo()
               {
                   Result = result.ToString(),
                   Message = message,
                   Data = data
               };
            }


            JsonResult jsr = new JsonResult();
            jsr.Data = rpInfo;
            jsr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsr;
        }
    }
}
