using Anke.SHManage.BLL;
using Anke.SHManage.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Anke.SHManage.Model;
using System.Data;
using System.Text;
using Anke.SHManage.MSSQLDAL;
namespace Anke.SHManage.Web.Areas.IM.Controllers
{
    public class AllDictionaryController : Controller
    {
        //
        // GET: /IM/AllDictionary/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 根据类型编码，获取病历相关字典表信息
        /// </summary>
        /// <param name="TypeCode"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetMSDictionaryInfo(string TypeCode)
        {
            try
            {
                M_DictionaryBLL bll = new M_DictionaryBLL();
                var result = bll.GetMSDictionaryInfos(TypeCode, "");

                return Json(result);
            }
            catch
            {
                return this.Json("");
            }
        }
        //取字典表TDictionary的OtherType的树
        [HttpPost]
        public ActionResult GetOtherType()
        {
            try
            {             
                List<TDictionary> mlist = new TDictionaryBLL().getOtherTypeID();               
                List<Model.EasyUIModel.TreeNode> list = Model.TDictionary.ToTreeNodes(mlist);
                return Json(list);
            }
            catch (Exception)
            {
                return Json("");
            }
        }
        //取字典表TDictionary的救治措施的树
        [HttpPost]
        public ActionResult GetMeasureType()
        {
            try
            {            
                List<TDictionary> mlist = new TDictionaryBLL().getMeasureTypeID();             
                List<Model.EasyUIModel.TreeNode> list = Model.TDictionary.ToTreeNodes(mlist);
                return Json(list);
            }
            catch (Exception)
            {
                return Json("");
            }
        }

        [HttpPost]
        public ActionResult GetMaterialTypeByUserInfo()
        {
            try
            {
                List<string> mlist = UserOperateContext.Current.Session_StorageRelated.listUserStorageMaterialType;
                string instr = "";
                foreach (string item in mlist)
                {
                    instr += "'" + item + "',";
                }
                instr = instr.Substring(0, instr.Length - 1);
                List<TDictionary> listD = new TDictionaryBLL().getTDicRecursion(instr).Where(e => e.IsActive == true).ToList();
                List<Model.EasyUIModel.TreeNode> list = Model.TDictionary.ToTreeNodes(listD);
                return Json(list);
            }
            catch (Exception)
            {
                return Json("");
            }

        }
        //Balance页面单用
        [HttpPost]
        public ActionResult GetMaterialTypeForBalanceList()
        {
            try
            {
                //List<string> mlist = UserOperateContext.Current.Session_StorageRelated.listUserStorageMaterialType;
                //string instr = "";
                //foreach (string item in mlist)
                //{
                //    instr += "'" + item + "',";
                //}
               // instr = instr.Substring(0, instr.Length - 1);
                string instr = "'MaterialType-9999','MaterialType-9998'";
                List<TDictionary> listD = new TDictionaryBLL().getTDicRecursion(instr).Where(e => e.IsActive == true).ToList();
                List<Model.EasyUIModel.TreeNode> list = Model.TDictionary.ToTreeNodes(listD);
                return Json(list);
            }
            catch (Exception)
            {
                return Json("");
            }
        }

        [HttpPost]
        public ActionResult GetAllMaterialTypeByUserInfo()
        {
            try
            {
                List<string> mlist = UserOperateContext.Current.Session_StorageRelated.listUserStorageMaterialType;
                string instr = "";
                foreach (string item in mlist)
                {
                    instr += "'" + item + "',";
                }
                instr = instr.Substring(0, instr.Length - 1);
                List<TDictionary> listD = new TDictionaryBLL().getTDicRecursion(instr);
                List<Model.EasyUIModel.TreeNode> list = Model.TDictionary.ToTreeNodes(listD);
                return Json(list);
            }
            catch (Exception)
            {
                return Json("");
            }
        }
        /// <summary>
        /// 获取全部仓库字典表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetAllStorage()
        {
            try
            {
                List<Model.EasyUIModel.TreeNode> list = Model.I_Storage.ToTreeNodes(new I_StorageBLL().GetStorageListby("1=1"));
                return Json(list);
            }
            catch
            {
                return this.Json("");
            }
        }
        /// <summary>
        /// 获取厂库字典表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetStorage()
        {
            try
            {
                List<int> slist = UserOperateContext.Current.Session_StorageRelated.listUserStorage;
                string instr = "";
                foreach (int item in slist)
                {
                    instr += item + ",";
                }
                instr = instr.Substring(0, instr.Length - 1);
                List<Model.EasyUIModel.TreeNode> list = Model.I_Storage.ToTreeNodes(new I_StorageBLL().GetStorageListby("StorageID>0 and StorageID in (" + instr + ")"));
                return Json(list);
            }
            catch
            {
                return this.Json("");
            }
        }
        /// <summary>
        /// 获取申请仓库字典表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetApplyStorage()
        {
            try
            {
                string instr = AppConfig.GetConfigString("ApplyStorageCode");
                var list = new I_StorageBLL().GetStorageExtListby("StorageID>0 and StorageID in (" + instr + ")");
                return Json(list);
            }
            catch
            {
                return this.Json("");
            }
        }
        [HttpPost]
        public ActionResult GetStorageCombo()
        {
            try
            {
                List<int> slist = UserOperateContext.Current.Session_StorageRelated.listUserStorage;
                string instr = "";
                foreach (int item in slist)
                {
                    instr += item + ",";
                }
                instr = instr.Substring(0, instr.Length - 1);
                var list = new I_StorageBLL().GetStorageExtListby("StorageID>0 and StorageID in (" + instr + ")");
                return Json(list);
            }
            catch (Exception)
            {
                return this.Json("");
            }
        }
        //根据物资类型获取 物资流水 
        [HttpPost]
        public ActionResult GetMaterial(string mType)
        {
            try
            {
                var list = new I_MaterialBLL().GetMaterialListBy(mType);
                return this.Json(list);
            }
            catch
            {
                return this.Json("");
            }
        }
        //根据措施类型获取 措施流水 
        [HttpPost]
        public ActionResult GetMeasure(string measureType)
        {
            try
            {
                var list = new I_MaterialBLL().GetMeasureListBy(measureType);
                return this.Json(list);
            }
            catch
            {
                return this.Json("");
            }
        }
        //取所有措施流水
        [HttpPost]
        public ActionResult GetMeasureList()
        {
            try
            {
                var list = new I_MaterialBLL().GetMeasureList();
                return this.Json(list);
            }
            catch
            {
                return this.Json("");
            }
        }
      

        //取所有物资流水
        [HttpPost]
        public ActionResult GetMaterialList()
        {
            try
            {
                var list = new I_MaterialBLL().GetMaterialList();
                return this.Json(list);
            }
            catch
            {
                return this.Json("");
            }
        }
        /// <summary>
        /// 获取所有仓储人员
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetUsers()
        {
            var list = new I_StoragePersonBLL().GetALLStoragePersons();
            return this.Json(list);
        }
        /// <summary>
        /// 根据人员查询人员下属仓库
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetStogageByUser(string userId)
        {
            try
            {
                int uId = Convert.ToInt32(userId);
                var list = new I_StorageBLL().GetStorageListby(uId).Select(e => e.ToDictionary());
                return Json(list);
            }
            catch (Exception)
            {
                return this.Json("");
            }
        }
    }
}
