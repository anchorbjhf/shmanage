using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Anke.SHManage.Model;
using Anke.SHManage.Utility;
using Anke.SHManage.BLL;

namespace Anke.SHManage.Web.Areas.PR.Controllers
{
    public class MedicalController : Controller
    {
        private static object m_SyncRoot = new Object();//互斥对象
        //
        // GET: /Medical/

        #region 任务列表
        /// <summary>
        /// 视图-任务列表
        /// </summary>
        public ActionResult MedicalManagement()
        {
            string menuName = "";
            string startTime = null;
            string endTime = null;
            string searchBound = null;

            P_User pUser = new P_User();
            pUser = UserOperateContext.Current.Session_UsrInfo;//获取登录人信息

            ViewData["MenuName"] = menuName;
            ViewData["WorkCode"] = pUser.WorkCode;//登录人ID

            //this.ViewData["startTime"] = startTime == null ? "2012-10-20" : startTime;//测试
            //this.ViewData["endTime"] = endTime == null ? "2012-10-21" : endTime;//测试
            this.ViewData["startTime"] = startTime == null ? DateTime.Now.AddDays(-AppConfig.GetConfigInt("PRSearchTime")).ToString("yyyy-MM-dd HH:mm:ss") : startTime;
            this.ViewData["endTime"] = endTime == null ? DateTime.Now.AddHours(1).ToString("yyyy-MM-dd HH:mm:ss") : endTime;
            UserOperateContext userOperateContext = new UserOperateContext();
            bool SelectALL = userOperateContext.getGongneng(38);//查看全部出车信息
            bool SelectOwnCenter = userOperateContext.getGongneng(42);//查看本分中心
            bool SelectOwnStation = userOperateContext.getGongneng(43);//查看本分站
            bool SelectOwn = userOperateContext.getGongneng(44);//查看本人出车信息
            bool Insert = userOperateContext.getGongneng(45);//判断登录人有“新增”病历权限
            bool Edit = userOperateContext.getGongneng(46);//判断登录人有“修改”病历权限
            bool Delete = userOperateContext.getGongneng(55);//判断登录人有“删除”病历权限
            bool Look = userOperateContext.getGongneng(56);//判断登录人有“查看”病历权限

            if (SuperRole("SuperRole"))
            {
                this.ViewData["SuperRole"] = "SuperRole";//登录人有超级权限
            }
            else
                this.ViewData["SuperRole"] = "";


            this.ViewData["SelectOwn"] = "";//
            this.ViewData["PatientRole"] = "";//
            if (SelectALL)
            {
                searchBound = "1";
            }
            else
            {
                if (SelectOwnCenter)
                    searchBound = "2";
                else
                {
                    if (SelectOwnStation)
                        searchBound = "3";
                    else
                    {
                        if (SelectOwn)
                        {
                            searchBound = "4";
                            this.ViewData["SelectOwn"] = "SelectOwn";//
                            string role = UserOperateContext.Current.getMaxPerForRole();//获取登录人角色(医生、护士、司机)
                            this.ViewData["PatientRole"] = role;//
                        }
                        else
                        {
                            searchBound = "4";
                            this.ViewData["SelectOwn"] = "";//
                            this.ViewData["PatientRole"] = "";//
                        }
                    }
                }
            }
            if (Insert)
            {
                this.ViewData["RolePermissionInsert"] = "Insert";//登录人有新增病历权限
            }
            else
                this.ViewData["RolePermissionInsert"] = "";//
            if (Edit)
            {
                this.ViewData["RolePermissionEdit"] = "Edit";//登录人有修改病历权限
            }
            else
                this.ViewData["RolePermissionEdit"] = "";//
            if (Delete)
            {
                this.ViewData["RolePermissionDelete"] = "Delete";//登录人有删除病历权限
            }
            else
                this.ViewData["RolePermissionDelete"] = "";//
            if (Look)
            {
                this.ViewData["RolePermissionLook"] = "Look";//登录人有查看病历权限
            }
            else
                this.ViewData["RolePermissionLook"] = "";//

            this.ViewData["searchBound"] = searchBound == null ? "4" : searchBound;//权限

            return View();
        }
        #endregion

        #region 查询病历填写列表
        [HttpPost]
        public ActionResult DataLoad(int page, int rows, string order, string sort, DateTime startTime, DateTime endTime
            , string linkPhone, int alarmEventType, string localAddr, string taskResult, int taskAbendReason
            , string centerCode, string stationCode, string ambCode, string driver, string doctor, string litter, string isCharge
            , string isFill, string nurse, string patientName, string searchBound, string isTest, string CPRIFSuccess, string PatientState)
        {
            try
            {
                P_User pUser = new P_User();
                pUser = UserOperateContext.Current.Session_UsrInfo;//获取登录人信息
                //bool t= UserOperateContext.Current.Session_UsrRole.Contains(10);//判断登录人是否为司机

                M_UserLoginInfo loginInfo = new M_UserLoginInfo();
                loginInfo.LoginName = pUser.LoginName;
                loginInfo.Name = pUser.Name;//姓名
                loginInfo.WorkCode = pUser.WorkCode;//工号
                loginInfo.DispatchSubCenterID = pUser.P_Department.DispatchSubCenterID;//所属分中心
                loginInfo.DispatchSationID = pUser.P_Department.DispatchSationID;//所属分站
                //loginInfo.RoleID = pUser.P_UserRole.RoleID;

                M_PatientRecordBLL M_PateintRecord = new M_PatientRecordBLL();

                var list = M_PateintRecord.GetTasks(page, rows, order, sort, startTime, endTime, linkPhone, alarmEventType, localAddr
                    , taskResult, taskAbendReason, centerCode, stationCode, ambCode, driver, doctor, litter, isCharge, isFill
                    , nurse, patientName, searchBound, isTest, loginInfo, CPRIFSuccess, PatientState);
                //if (list != null)
                //{
                //return this.Json(new { total = list, rows = list }, "appliction/json", JsonRequestBehavior.AllowGet);
                return this.Json(list, "appliction/json", JsonRequestBehavior.AllowGet);
                //}
                //else
                //{
                //   return this.Json(new { total = 0, rows = 0 }, "appliction/json", JsonRequestBehavior.AllowGet);
                //}
            }
            catch
            {
                return this.Json("");
            }
        }
        #endregion

        #region 根据任务编码获取病历信息
        /// <summary>
        /// 根据任务编码获取病历信息
        /// </summary>
        /// <param name="taskCode"></param>
        /// <returns></returns>
        public ActionResult GetPatientCommonByTask(string taskCode)
        {
            try
            {
                M_PatientRecordBLL m_PateintRecord = new M_PatientRecordBLL();
                List<M_PatientRecord> list = m_PateintRecord.GetPatientCommonByTask(taskCode);
                if (list != null)
                {
                    return this.Json(new { total = list.Count, rows = list }, "appliction/json", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return this.Json(new { total = 0, rows = 0 }, "appliction/json", JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                //Dictionary<string, string> dict = new Dictionary<string, string>();
                //dict.Add("InfoID", "0");
                //dict.Add("InfoMessage", ex.Message);
                return this.Json("");
            }
        }
        #endregion

        #region 病历页面初始化
        /// <summary>
        /// 病历页面初始化
        /// </summary>
        /// <param name="TaskCode">任务编码</param>
        /// <param name="PatientOrder">序号</param>
        /// <param name="state">填写状态</param>
        /// <returns></returns>
        public ActionResult AddPatientRecord(string TaskCode, int PatientOrder, string state)
        {
            //, int PatientOrder//测试
            M_PatientRecordBLL prBLL = new M_PatientRecordBLL();
            M_DictionaryBLL dBLL = new M_DictionaryBLL();
            //int PatientOrder = 1;//测试

            M_AttemperData result = prBLL.GetAttemperData(TaskCode, state);//根据任务编码获取调度信息
            ViewBag.Attemper = result;

            ViewData["TaskCode"] = TaskCode == null ? "2012102023555200020101" : TaskCode;
            M_PatientRecord prInfo;//病历主表信息
            M_PatientRecordAppend pra;//病历附表--体检等信息
            M_PatientRecordCPR prCPR;//病历附表--心肺复苏
            if (state == "new")
            {
                int AddPatientOrder = prBLL.GetPatientMaxOrder(TaskCode);
                ViewData["PatientOrder"] = AddPatientOrder;
                prInfo = new M_PatientRecord();
                pra = new M_PatientRecordAppend();
                prCPR = new M_PatientRecordCPR();
                ViewBag.PRInfo = prInfo;//病历主表信息--传到页面
                ViewBag.PRAppendInfo = pra;//病历附表--体检等信息--传到页面
                ViewBag.PRCPRInfo = prCPR;//病历附表--心肺复苏--传到页面

            }
            else
            {
                object oPatientInfo;//病历主表
                prBLL.GetPatientInfo(TaskCode, PatientOrder, out oPatientInfo, out pra, out prCPR);
                if (PatientOrder > 0)
                {
                    prInfo = (M_PatientRecord)oPatientInfo;
                    ViewBag.PRInfo = prInfo;//病历主表信息--传到页面
                    ViewBag.PRAppendInfo = pra;//病历附表--体检等信息--传到页面
                    ViewBag.PRCPRInfo = prCPR;//病历附表--心肺复苏--传到页面
                    ViewData["PatientOrder"] = PatientOrder;
                    if (SuperRole("SuperRole"))
                    {
                        this.ViewData["SuperRole"] = "SuperRole";//登录人有超级权限
                    }
                    else
                        this.ViewData["SuperRole"] = "";

                    string role = UserOperateContext.Current.getMaxPerForRole();//获取登录人角色(医生、护士、司机)
                    this.ViewData["PatientRole"] = role;//
                }
            }
            ViewData["state"] = state == null ? "new" : state;

            if (SuperRole("PRAuditCPR"))
            {
                this.ViewData["PRAuditCPR"] = "PRAuditCPR";//登录人有“心肺复苏审核”权限
            }
            else
                this.ViewData["PRAuditCPR"] = "";

            if (SuperRole("SubCenterSpotChecks"))
            {
                this.ViewData["SubCenterSpotChecks"] = "SubCenterSpotChecks";//登录人有"分中心抽查"权限
            }
            else
                this.ViewData["SubCenterSpotChecks"] = "";

            if (SuperRole("CenterSpotChecks"))
            {
                this.ViewData["CenterSpotChecks"] = "CenterSpotChecks";//登录人有"中心抽查"权限
            }
            else
                this.ViewData["CenterSpotChecks"] = "";

            P_User pUser = new P_User();
            pUser = UserOperateContext.Current.Session_UsrInfo;//获取登录人信息
            ViewData["AgentCode"] = pUser.ID;
            ViewData["AgentWorkID"] = pUser.WorkCode;
            ViewData["AgentName"] = pUser.Name;
            ViewData["BeginFillPatientTime"] = DateTime.Now.ToString();//开始填写病历时间


            #region 为病历页面的CheckBoxList项目赋值
            CheckViewModel model = new CheckViewModel();
            //model.DiseasesClassification = dBLL.GetCheckBoxModelByTableName("M_ZCaseTemplate");//病种分类
            //model.GongShiRen = dBLL.GetCheckBoxModel("GongShiRen");//供史人
            //model.AnamnesisllnessHistory = dBLL.GetCheckBoxModel("PastMedicalHistory");//既往病史
            model.BodyFigure = dBLL.GetCheckBoxModel("PiFu");//皮肤
            model.Head = dBLL.GetCheckBoxModel("Head");//头部
            model.Neck = dBLL.GetCheckBoxModel("Neck");//颈部
            model.Chest = dBLL.GetCheckBoxModel("Chest");//胸部
            model.Lung = dBLL.GetCheckBoxModel("Lung");//肺脏
            model.LungLeft = dBLL.GetCheckBoxModel("LungLeft");//左肺
            model.LungRight = dBLL.GetCheckBoxModel("LungLeft");//右肺
            model.FuBu = dBLL.GetCheckBoxModel("FuBu");//腹部
            model.JiZhu = dBLL.GetCheckBoxModel("JiZhu");//脊柱
            model.Limb = dBLL.GetCheckBoxModel("Limb");//四肢
            #endregion

            #region 为病历页面的RadioButtonList从数据库传值

            var ProvideMedicalHistoryPeople = new object();//供史人
            var DiseasesClassification = new object();//病种分类
            var PastMedicalHistory = new object();//既往病史
            var BabinskiSign = new object();//神经系统--巴氏征
            var ChestExtrusionTest = new object();//胸廓挤压试验
            var PelvicExtrusionTest = new object();//骨盆挤压试验

            lock (m_SyncRoot)
            {
                ProvideMedicalHistoryPeople = CacheHelper.GetCache("ProvideMedicalHistoryPeople");
                DiseasesClassification = CacheHelper.GetCache("DiseasesClassification");
                PastMedicalHistory = CacheHelper.GetCache("PastMedicalHistory");
                BabinskiSign = CacheHelper.GetCache("BabinskiSign");
                ChestExtrusionTest = CacheHelper.GetCache("ChestExtrusionTest");
                PelvicExtrusionTest = CacheHelper.GetCache("PelvicExtrusionTest");
                if (ProvideMedicalHistoryPeople == null)
                {
                    ProvideMedicalHistoryPeople = dBLL.GetCheckBoxOrRadioButtonList("Checkbox", "ProvideMedicalHistoryPeople", "checkbox");
                    CacheHelper.SetCache("ProvideMedicalHistoryPeople", ProvideMedicalHistoryPeople);
                }
                if (DiseasesClassification == null)
                {
                    DiseasesClassification = dBLL.GetCheckBoxListByTableName("M_ZCaseTemplate", "DiseasesClassification", "checkbox");
                    CacheHelper.SetCache("DiseasesClassification", DiseasesClassification);
                }
                if (PastMedicalHistory == null)
                {
                    PastMedicalHistory = dBLL.GetCheckBoxOrRadioButtonList("Checkbox", "PastMedicalHistory", "checkbox");
                    CacheHelper.SetCache("PastMedicalHistory", PastMedicalHistory);
                }
                if (BabinskiSign == null)
                {
                    BabinskiSign = dBLL.GetCheckBoxOrRadioButtonList("Checkbox", "BabinskiSign", "checkbox");
                    CacheHelper.SetCache("BabinskiSign", BabinskiSign);
                }
                if (ChestExtrusionTest == null)
                {
                    ChestExtrusionTest = dBLL.GetCheckBoxOrRadioButtonList("Radio", "ChestExtrusionTest", "radio");
                    CacheHelper.SetCache("ChestExtrusionTest", ChestExtrusionTest);
                }
                if (PelvicExtrusionTest == null)
                {
                    PelvicExtrusionTest = dBLL.GetCheckBoxOrRadioButtonList("Radio", "PelvicExtrusionTest", "radio");
                    CacheHelper.SetCache("PelvicExtrusionTest", PelvicExtrusionTest);
                }
            }
            this.ViewData["ProvideMedicalHistoryPeople"] = ProvideMedicalHistoryPeople;
            this.ViewData["DiseasesClassification"] = DiseasesClassification;
            this.ViewData["PastMedicalHistory"] = PastMedicalHistory;
            this.ViewData["BabinskiSign"] = BabinskiSign;
            this.ViewData["ChestExtrusionTest"] = ChestExtrusionTest;
            this.ViewData["PelvicExtrusionTest"] = PelvicExtrusionTest;

            #endregion

            return View(model);
        }
        #endregion

        #region 病历和收费显示模板页--不用了
        /// <summary>
        /// 病历和收费显示模板页
        /// </summary>
        /// <param name="TaskCode"></param>
        /// <param name="PatientOrder"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public ActionResult PatientRecordIframe(string TaskCode, int PatientOrder, string state)
        {
            //M_PatientRecordBLL prBLL = new M_PatientRecordBLL();
            //if (state == "new")
            //{
            //    int AddPatientOrder = prBLL.GetPatientMaxOrder(TaskCode);
            //    ViewData["patientOrder"] = AddPatientOrder;
            //}
            //else
            //{
            //    this.ViewData["patientOrder"] = PatientOrder;
            //}
            this.ViewData["patientOrder"] = PatientOrder;
            this.ViewData["taskCode"] = TaskCode;
            this.ViewData["state"] = state;
            return View();
        }
        #endregion

        #region 保存病历
        /// <summary>
        /// 保存病历
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public bool SavePatient()
        {
            string state = "";
            M_PatientRecordBLL bll = new M_PatientRecordBLL();
            M_PatientRecord info = null;//病历主表信息
            M_PatientRecordAppend pra = null;//病历附表
            M_PatientRecordCPR prCPR = null;//病历附表--心肺复苏
            List<M_PatientRecordDiag> prDiag = null;//病历子表--初步印象
            List<M_PatientRecordECGImpressions> prECG = null;//病历子表--心电图印象

            string PatientRecord = Request.Form["PatientRecord"].ToString();
            M_AddPatientRecord add = new M_AddPatientRecord();
            add = JsonHelper.GetJsonInfoBy<M_AddPatientRecord>(PatientRecord);
            if (add != null)
            {
                string TaskCode = add.TaskCode;
                int PatientOrder = add.PatientOrder;
                state = add.state;
                info = add.info;//病历主表信息
                pra = add.pra;//病历附表
                prCPR = add.prCPR;//病历附表--心肺复苏

                if (add.prDiag.Count > 0)
                { prDiag = add.prDiag; }//病历子表--初步印象
                else
                { prDiag = null; }//病历子表--初步印象

                if (add.prECG.Count > 0)
                { prECG = add.prECG; }//病历子表--心电图印象
                else
                { prECG = null; }//病历子表--心电图印象
            }

            bool save = false;
            if (info != null)
            {
                try
                {
                    if (state == "new")
                    {
                        save = bll.Insert(info, pra, prCPR, prDiag, prECG);//新增病历主表、附表、子表
                    }
                    else if (state == "edit")
                    {
                        save = bll.Update(info, pra, prCPR, prDiag, prECG);//修改病历主表、附表、子表
                    }
                }
                catch (Exception e)
                {
                    save = false;
                }
            }
            return save;
        }
        #endregion

        #region 删除病历
        [HttpPost]
        public bool DelectPatient()
        {
            bool save = false;
            try
            {
                M_PatientRecordBLL bll = new M_PatientRecordBLL();
                string TaskCode = Request.Form["TaskCode"].ToString();
                int PatientOrder = Convert.ToInt32(Request.Form["PatientOrder"]);
                if (TaskCode != null)
                {
                    save = bll.Delete(TaskCode, PatientOrder);
                }
            }
            catch
            {
                save = false;
            }
            return save;
        }
        #endregion

        #region CheckViewModel
        public class CheckViewModel
        {
            //存储从数据库获取的值
            //public IList<M_CheckModel> DiseasesClassification { get; set; }//病种分类
            public IList<M_CheckModel> GongShiRen { get; set; }//供史人
            //public IList<M_CheckModel> AnamnesisllnessHistory { get; set; }//既往病史
            public IList<M_CheckModel> BodyFigure { get; set; }//皮肤
            public IList<M_CheckModel> Head { get; set; }//头部
            public IList<M_CheckModel> Neck { get; set; }//颈部
            public IList<M_CheckModel> Chest { get; set; }//胸部
            public IList<M_CheckModel> Lung { get; set; }//肺脏
            public IList<M_CheckModel> LungLeft { get; set; }//左肺
            public IList<M_CheckModel> LungRight { get; set; }//右肺
            public IList<M_CheckModel> FuBu { get; set; }//腹部
            public IList<M_CheckModel> JiZhu { get; set; }//脊柱
            public IList<M_CheckModel> Limb { get; set; }//四肢

            //存储从页面获取的值
            //public CheckModels DiseasesClassificationModels { get; set; }//病种分类
            public CheckModels GongShiRenModels { get; set; }//供史人
            //public CheckModels AnamnesisllnessHistoryModels { get; set; }//既往病史
            public CheckModels BodyFigureModels { get; set; }//皮肤
            public CheckModels HeadModels { get; set; }//头部
            public CheckModels NeckModels { get; set; }//颈部
            public CheckModels ChestModels { get; set; }//胸部
            public CheckModels LungModels { get; set; }//肺脏
            public CheckModels LungLeftModels { get; set; }//左肺
            public CheckModels LungRightModels { get; set; }//右肺
            public CheckModels FuBuModels { get; set; }//腹部
            public CheckModels JiZhuModels { get; set; }//脊柱
            public CheckModels LimbModels { get; set; }//四肢
        }
        public class CheckModels
        {
            public string[] CheckModelIDs { get; set; }
        }
        #endregion

        #region 获取字典表树形结构信息
        /// <summary>
        /// 获取字典表树形结构信息
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="ParentID"></param>
        /// <param name="TypeID"></param>
        /// <returns></returns>
        public ActionResult GetAllDictionaryTrees(string tableName, string ParentID, string TypeID)
        {
            try
            {
                M_DictionaryTreeBLL bll = new M_DictionaryTreeBLL();

                var result = new object();
                //增加缓存
                lock (m_SyncRoot)
                {
                    //result = CacheHelper.GetCache(tableName);
                    //if (result == null)
                    //{
                        result = bll.GetModelList(tableName, ParentID, TypeID);//
                    //    CacheHelper.SetCache(tableName, result);
                    //}
                }
                JsonResult j = this.Json(result, "appliction/json", JsonRequestBehavior.AllowGet);
                return j;
            }
            catch (Exception ex)
            {
                LogUtility.Error("MedicalController/GetAllDictionaryTrees()", ex.ToString());
                return this.Json("");
            }
        }
        #endregion

        #region 获取字典表信息

        /// <summary>
        /// 获取事件类型
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetAlarmEventType()
        {
            try
            {
                M_DictionaryBLL bll = new M_DictionaryBLL();
                var result = new object();
                //增加缓存
                lock (m_SyncRoot)
                {
                    result = CacheHelper.GetCache("GetAlarmEventType");
                    if (result == null)
                    {
                        result = bll.GetMainDictionary("TZAlarmEventType", "");
                        CacheHelper.SetCache("GetAlarmEventType", result);
                    }
                }
                return Json(result);
            }
            catch (Exception ex)
            {
                LogUtility.Error("MedicalController/GetAlarmEventType()", ex.ToString());
                return this.Json("");
            }
        }

        /// <summary>
        /// 获取分中心
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetCenter()
        {
            try
            {
                M_DictionaryBLL bll = new M_DictionaryBLL();
                var result = new object();
                //增加缓存
                lock (m_SyncRoot)
                {
                    result = CacheHelper.GetCache("GetCenter");
                    if (result == null)
                    {
                        result = bll.GetMainDictionary("TCenter", "");
                        CacheHelper.SetCache("GetCenter", result);
                    }
                }
                return Json(result);
            }
            catch (Exception e)
            {
                return this.Json("");
            }
        }

        /// <summary>
        /// 获取分站
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetStation(string code)
        {
            try
            {
                M_DictionaryBLL bll = new M_DictionaryBLL();
                var result = new object();
                if (code == "")
                { code = "-1"; }
                //增加缓存
                //lock (m_SyncRoot)
                //{
                //    result = CacheHelper.GetCache("GetStation");
                //    if (result == null)
                //    {
                result = bll.GetMainDictionary("TStation", code);
                //CacheHelper.SetCache("GetStation", result);
                //    }
                //}
                return Json(result);
            }
            catch (Exception e)
            {
                return this.Json("");
            }
        }

        /// <summary>
        /// 获取车辆
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetAmbulance(string code)
        {
            try
            {
                M_DictionaryBLL bll = new M_DictionaryBLL();
                var result = new object();
                if (code == "")
                { code = "-1"; }
                //增加缓存
                //lock (m_SyncRoot)
                //{
                //    result = CacheHelper.GetCache("GetAmbulance");
                //    if (result == null)
                //    {
                result = bll.GetMainDictionary("TAmbulance", code);
                //        CacheHelper.SetCache("GetAmbulance", result);
                //    }
                //}
                return Json(result);
            }
            catch (Exception e)
            {
                return this.Json("");
            }
        }

        /// <summary>
        /// 获取调度员
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetDispatcher()
        {
            try
            {
                M_DictionaryBLL bll = new M_DictionaryBLL();
                var result = new object();
                //增加缓存
                lock (m_SyncRoot)
                {
                    result = CacheHelper.GetCache("GetDispatcher");
                    if (result == null)
                    {
                        result = bll.GetMainDictionary("TPerson", "");
                        CacheHelper.SetCache("GetDispatcher", result);
                    }
                }
                return Json(result);
            }
            catch (Exception e)
            {
                return this.Json("");
            }
        }

        /// <summary>
        /// 获取异常结束原因
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetZTaskAbendReason()
        {
            try
            {
                M_DictionaryBLL bll = new M_DictionaryBLL();
                var result = new object();
                //增加缓存
                lock (m_SyncRoot)
                {
                    result = CacheHelper.GetCache("GetZTaskAbendReason");
                    if (result == null)
                    {
                        result = bll.GetMainDictionary("TZTaskAbendReason", "");
                        CacheHelper.SetCache("GetZTaskAbendReason", result);
                    }
                }
                return Json(result);
            }
            catch (Exception e)
            {
                return this.Json("");
            }
        }

        /// <summary>
        /// 获取医院列表信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetHospitalInfo()
        {
            try
            {
                M_DictionaryBLL bll = new M_DictionaryBLL();
                var result = new object();
                //增加缓存
                lock (m_SyncRoot)
                {
                    result = CacheHelper.GetCache("GetHospitalInfo");
                    if (result == null)
                    {
                        result = bll.GetMainDictionary("THospitalInfo", "");
                        CacheHelper.SetCache("GetHospitalInfo", result);
                    }
                }
                return Json(result);
            }
            catch (Exception e)
            {
                return this.Json("");
            }
        }

        /// <summary>
        /// 根据类型编码，获取病历相关字典表信息
        /// </summary>
        /// <param name="TypeCode"></param>
        /// <returns></returns>
        //[HttpPost]
        public ActionResult GetMSDictionaryInfo(string TypeCode)
        {
            try
            {
                M_DictionaryBLL bll = new M_DictionaryBLL();
                var result = bll.GetMSDictionaryInfos(TypeCode, "1");

                return Json(result);
            }
            catch (Exception e)
            {
                return this.Json("");
            }
        }
        #endregion

        #region 根据病种分类的名称串来获取主诉和现病史
        /// <summary>
        /// 根据病种分类的名称串来获取主诉和现病史
        /// </summary>
        /// <param name="TemplateName"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetAllTemplate(string TemplateName)
        {
            try
            {
                M_PatientRecordBLL bll = new M_PatientRecordBLL();

                var result = bll.GetAllTemplate(TemplateName);

                return Json(result);
            }
            catch (Exception e)
            {
                //Dictionary<string, string> dict = new Dictionary<string, string>();
                //dict.Add("InfoID", "0");
                //dict.Add("InfoMessage", e.Message);
                return this.Json("");
            }
        }
        #endregion

        #region 判断登录人是否有超级权限
        public bool SuperRole(string Roles)
        {
            string strSuperRole = AppConfig.GetConfigString(Roles);
            string[] tcr = strSuperRole.Split(',');
            bool tarray = false;
            for (int i = 0; i < tcr.Length; i++)
            {
                bool UserRole = UserOperateContext.Current.Session_UsrRole.SingleOrDefault(p => p == Convert.ToInt32(tcr[i])) != 0 ? true : false;
                if (UserRole)
                {
                    //判断登录人角色
                    tarray = true;
                    break;
                }
            }
            return tarray;
        }
        #endregion

        #region 保存心肺复苏审核
        public bool SaveAuditCPR()
        {
            M_PatientRecordBLL bll = new M_PatientRecordBLL();

            string AuditCPR = Request.Form["AuditCPR"].ToString();
            M_PatientRecordCPR info = new M_PatientRecordCPR();
            info = JsonHelper.GetJsonInfoBy<M_PatientRecordCPR>(AuditCPR);

            bool save = false;
            if (info != null)
            {
                try
                {
                    save = bll.UpdateAuditCPR(info);//修改病历审核
                }
                catch
                {
                    save = false;
                }
            }
            return save;
        }
        #endregion

        #region 保存抽查病历
        public bool SaveSpotChecks()
        {
            M_PatientRecordBLL bll = new M_PatientRecordBLL();

            string SpotChecks = Request.Form["SpotChecks"].ToString();
            int orderNumber = int.Parse(Request.Form["orderNumber"]);
            M_PatientRecord info = new M_PatientRecord();
            info = JsonHelper.GetJsonInfoBy<M_PatientRecord>(SpotChecks);

            bool save = false;
            if (info != null)
            {
                try
                {
                    save = bll.UpdateSpotChecks(info, orderNumber);//修改病历抽查
                }
                catch
                {
                    save = false;
                }
            }
            return save;
        }
        #endregion

        #region 保存医生回访
        public bool SaveFollowUp()
        {
            M_PatientRecordBLL bll = new M_PatientRecordBLL();

            string TaskCode = Request.Form["TaskCode"].ToString();
            int PatientOrder = int.Parse(Request.Form["PatientOrder"]);
            string DoctorFollowUp = Request.Form["DoctorFollowUp"].ToString();

            bool save = false;
            if (TaskCode != null)
            {
                try
                {
                    save = bll.UpdateFollowUp(TaskCode,PatientOrder,DoctorFollowUp);//修改医生回访
                }
                catch
                {
                    save = false;
                }
            }
            return save;
        }
        #endregion
    }
}
