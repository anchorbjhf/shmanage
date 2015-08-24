using Anke.SHManage.BLL;
using Anke.SHManage.Model;
using Anke.SHManage.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Anke.SHManage.Web.Areas.DM.Controllers
{
    public class M_ZCaseTemplateController : Controller
    {

        //
        // GET: /DM/M_ZCaseTemplate/
        /// <summary>
        /// 页面初始化类
        /// </summary>
        /// <returns></returns>
        public ActionResult M_ZCaseTemplate()
        {
            return View();
        }
        

        //取所有病种
        [HttpPost]
        public ActionResult GetDiseasesList()
        {
            try
            {
                var list = new M_ZCaseTemplateBLL().GetDiseasesList();
                return this.Json(list);
            }
            catch
            {
                return this.Json("");
            }
        }
     

        /// <summary>
        /// 修改
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DiseaseEdit()
        {

            string DetailInfo = Request.Form["Info"].ToString();
            M_ZCaseTemplate model = JsonHelper.GetJsonInfoBy<M_ZCaseTemplate>(DetailInfo);

            int res = new M_ZCaseTemplateBLL().Modify(model, "ID","Name", "AlarmReason", "HistoryOfPresentIllness", "SN", "IsActive");

            if (res > 0)
                return this.JsonResult(Utility.E_JsonResult.OK, "修改成功！", null, null);
            else
                return this.JsonResult(Utility.E_JsonResult.Error, "修改失败", null, null);
        }
        /// <summary>
        /// 新增
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DiseaseAdd()
        {
            string DetailInfo = Request.Form["Info"].ToString();
            List<M_ZCaseTemplate> list = new M_ZCaseTemplateBLL().GetMaxID();
            
            M_ZCaseTemplate info = JsonHelper.GetJsonInfoBy<M_ZCaseTemplate>(DetailInfo);
            info.ID = list[0].ID + 1;
            int res = new M_ZCaseTemplateBLL().Add(info);         
            if (res > 0)
                return this.JsonResult(Utility.E_JsonResult.OK, "保存成功！", null, null);
            else
                return this.JsonResult(Utility.E_JsonResult.Error, "保存失败", null, null);
        }

    
        /// <summary>
        /// 查询列表信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DataLoad()
        {
            
            try
            {
                string DiseaseID = "";
                string isActive = Request.Form["isActive"].ToString();
                if (Request.Form["ID"] == null)
                {  DiseaseID = ""; }
                else
                {
                     DiseaseID = Request.Form["ID"].ToString();
                }
               
                //获取页容量
                int rows = int.Parse(Request.Form["rows"]);
                //获取请求的页码
                int page = int.Parse(Request.Form["page"]);
                //返回总行数
                int total = 0;

                var list = new M_ZCaseTemplateBLL().GetDiseaseList(page, rows, ref total, isActive, DiseaseID);
                return Json(new { total = total, rows = list }, "appliction/json", JsonRequestBehavior.AllowGet);
            }

            catch (Exception e)
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict.Add("InfoID", "0");
                dict.Add("InfoMessage", e.Message);
                return this.Json(dict);
            }
        }
        /// <summary>
        ///  禁用/启用
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DiseaseDisable()
        {
            int id = Convert.ToInt32(Request.Form["id"]);
            bool disable = Convert.ToBoolean(Request.Form["disable"]);
            M_ZCaseTemplate model = new M_ZCaseTemplate();
            model.IsActive = disable;
            model.ID = id;
            int res = new M_ZCaseTemplateBLL().Modify(model, "IsActive");
            string tInfo = "";
            string finfo = "";
            if (disable)
            {
                tInfo = "设置“<span style='color:darkgreen'>有效</span>”成功！";
                finfo = "设置“<span style='color:darkgreen'>有效</span>”失败！";
            }
            else
            {
                tInfo = "设置“<span style='color:red'>无效</span>”成功！";
                finfo = "设置“<span style='color:red'>无效</span>”失败！";
            }
            if (res > 0)
                return this.JsonResult(Utility.E_JsonResult.OK, tInfo, null, null);
            else
                return this.JsonResult(Utility.E_JsonResult.Error, finfo, null, null);
        }
      

    }
}
