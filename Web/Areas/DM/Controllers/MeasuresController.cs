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
    public class MeasuresController : Controller
    {
        //
        // GET: /PR/Measures/

        /// <summary>
        /// 页面初始化类
        /// </summary>
        /// <returns></returns>
        public ActionResult Measures()
        {
            return View();
        }
        /// <summary>
        /// 物资分类
        /// </summary>
        /// <returns></returns>
        public ActionResult MaterialClass()
        {
            return View();
        }
        /// <summary>
        /// 新增物资分类
        /// </summary>
        /// <returns></returns>
        public ActionResult SaveMaterialType()
        {
            string PID = Request.Form["PID"].ToString();
            string mName = Request.Form["MTypeName"].ToString();
            TDictionary info = new TDictionary();
            return Json("");
        }
        /// <summary>
        /// 修改物资分类名称
        /// </summary>
        /// <returns></returns>
        public ActionResult EditMaterialType()
        {
            string MID = Request.Form["MID"].ToString();
            string mName = Request.Form["MTypeName"].ToString();
            return Json("");
        }
        /// <summary>
        /// 禁用物资分类
        /// </summary>
        /// <returns></returns>
        public ActionResult DisableMaterialType()
        {
            string MID = Request.Form["MID"].ToString();

            return Json("");
        }
        /// <summary>
        /// 启用物资分类
        /// </summary>
        /// <returns></returns>
        public ActionResult EnableMaterialType()
        {
            string MID = Request.Form["MID"].ToString();
            return Json("");
        }


        /// <summary>
        /// 修改措施
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult MeasureEdit()
        {

            string sDetailInfo = Request.Form["sMaterialInfo"].ToString();
            I_Material model = JsonHelper.GetJsonInfoBy<I_Material>(sDetailInfo);
            if (model.RealPrice == null)
            { model.RealPrice = 0; };
            int res = new I_MaterialBLL().Modify(model, "Name", "MTypeID", "Manufacturer", "Vendor", "MCode", "Unit", "Specification", "RealPrice", "Remark", "AlarmCounts", "PinYin", "FeeScale", "LimitMaxPrice", "GiveMedicineWay", "SN", "OtherTypeID", "IsActive");

            if (res > 0)
                return this.JsonResult(Utility.E_JsonResult.OK, "修改成功！", null, null);
            else
                return this.JsonResult(Utility.E_JsonResult.Error, "修改失败", null, null);
        }
        /// <summary>
        /// 新增措施
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult MeasureAdd()
        {
            string sDetailInfo = Request.Form["sMaterialInfo"].ToString();
            I_Material info = JsonHelper.GetJsonInfoBy<I_Material>(sDetailInfo);
            if (info.RealPrice == null)
            { info.RealPrice = 0; };
            int res = new I_MaterialBLL().Add(info);
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
                string isActive = Request.Form["isActive"].ToString();
                string MeasureType = Request.Form["MeasureType"].ToString();
                //List<string> mTypeList = UserOperateContext.Current.getMaterialTypeList(MeasureType);
                string MeasureID = Request.Form["MeasureID"].ToString();

                //获取页容量
                int rows = int.Parse(Request.Form["rows"]);
                //获取请求的页码
                int page = int.Parse(Request.Form["page"]);
                //返回总行数
                int total = 0;

                var list = new I_MaterialBLL().GetMaterialList(page, rows, ref total, isActive, MeasureType, MeasureID);
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
        public ActionResult MeasureDisable()
        {
            int id = Convert.ToInt32(Request.Form["id"]);
            bool disable = Convert.ToBoolean(Request.Form["disable"]);
            I_Material model = new I_Material();
            model.IsActive = disable;
            model.ID = id;
            int res = new I_MaterialBLL().Modify(model, "IsActive");
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
