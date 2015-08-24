using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Transactions;
using System.Data.Entity;
using System.Reflection;
using Anke.SHManage.Model;
using Anke.SHManage.Utility;
using Anke.SHManage.IDAL;

namespace Anke.SHManage.MSSQLDAL
{
    public partial class M_PatientRecordDAL : IM_PatientRecordDAL
    {
        IDALContext dalContext = new DALContextFactory().GetDALContext();
        AKSHManageEntities db = DBContextFactory.GetDBContext() as AKSHManageEntities;

        #region 查询方法

        #region 出车信息

        #region 出车信息--填写病历列表
        public object GetTasks(int page, int rows, string order, string sort, DateTime m_BeginTime, DateTime m_EndTime
           , string m_LinkPhone, int m_AlarmEventType, string m_LocalAddr, string m_TaskResult, int m_TaskAbendReason
           , string m_CenterCode, string m_StationCode, string m_AmbCode, string m_Driver, string m_Doctor, string m_Litter, string m_IsCharge
           , string m_IsFill, string m_Nurse, string m_PatientName, string searchBound, string isTest, M_UserLoginInfo loginInfo
           , string m_CPRIFSuccess, string m_PatientState)
        {
            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(SqlHelper.MainConnectionString);//取管理库的链接字符串
                StringBuilder sbSQL = new StringBuilder();//sql语句
                sbSQL.Append("declare @BeginTime datetime ");
                sbSQL.Append("declare @EndTime datetime ");
                sbSQL.Append("set @BeginTime='" + m_BeginTime + "' ");
                sbSQL.Append("set @EndTime='" + m_EndTime + "' ");
                sbSQL.Append("declare @TaskCodeB char(22) ");
                sbSQL.Append("declare @TaskCodeE char(22) ");
                sbSQL.Append("set @TaskCodeB = convert(char(8),@BeginTime,112)+'00000000000000' ");
                sbSQL.Append("set @TaskCodeE = convert(char(8),dateadd(day,1,@EndTime),112)+'00000000000000' ");

                sbSQL.Append("SELECT  t.任务编码,ae.首次受理时刻,ae.联系电话, 事件类型=tzae.名称");
                sbSQL.Append(",ae.患者姓名,ae.现场地址,派车时刻=acc.发送指令时刻,受理类型=tzac.名称,s.中心编码,t.分站编码,");
                sbSQL.Append("分站名称=s.名称,amb.实际标识,");
                //sbSQL.Append("分站名称=s.名称,amb.实际标识,司机=dbo.GetStr(t.任务编码,3),医生=dbo.GetStr(t.任务编码,4),担架工=dbo.GetStr(t.任务编码,6),护士=dbo.GetStr(t.任务编码,5),");
                sbSQL.Append("出车结果=case when t.是否正常结束=1 then '正常' else '异常结束' end, ");
                //病历数
                sbSQL.Append("病历数= (SELECT COUNT(*) FROM ").Append(builder.InitialCatalog).Append(".dbo.M_PatientRecord ");
                sbSQL.Append(" p WHERE p.TaskCode=t.任务编码) ");
                //收费-添加多收费后 修改为有一个欠费则显示欠费
                //sbSQL.Append(",收费数=isnull((SELECT 收费数=case when sum(case when 是否欠费=1 then 1 else 0 end)>0  then '欠费' else '已收' end FROM ").Append(builder.InitialCatalog).Append(".dbo.M_PatientCharge cr Where cr.TaskCode=t.任务编码 group by TaskCode ),'未收')  ");
                sbSQL.Append(@"into #tempX 
                        FROM TTask t 
                        JOIN TAcceptEvent acc ON t.事件编码=acc.事件编码 AND t.受理序号=acc.受理序号 
                        JOIN TAlarmEvent ae ON t.事件编码=ae.事件编码 
                        LEFT JOIN TStation s ON t.分站编码=s.编码 
                        LEFT JOIN TAmbulance amb ON t.车辆编码=amb.车辆编码 
                        LEFT JOIN TZAlarmEventType tzae ON tzae.编码=ae.事件类型编码 
                        LEFT JOIN TZAcceptEventType tzac ON tzac.编码=acc.受理类型编码 ");
                StringBuilder sbWhereClause = new StringBuilder();
                sbWhereClause.Append("where t.任务编码>@TaskCodeB and t.任务编码<=@TaskCodeE  ");
                WhereClauseUtility.AddDateTimeGreaterThan("ae.首次受理时刻", m_BeginTime, sbWhereClause);
                WhereClauseUtility.AddDateTimeLessThan("ae.首次受理时刻", m_EndTime, sbWhereClause);
                WhereClauseUtility.AddStringEqual("ae.是否测试", isTest, sbWhereClause);//是否测试
                WhereClauseUtility.AddStringLike("ae.患者姓名", m_PatientName, sbWhereClause);//患者姓名
                WhereClauseUtility.AddStringLike("ae.联系电话", m_LinkPhone, sbWhereClause);
                WhereClauseUtility.AddIntEqual("ae.事件类型编码", m_AlarmEventType, sbWhereClause);
                WhereClauseUtility.AddStringLike("ae.现场地址", m_LocalAddr, sbWhereClause);
                //WhereClauseUtility.AddStringEqual("t.责任调度人编码", m_DisCode, sbWhereClause);
                WhereClauseUtility.AddStringEqual("t.是否正常结束", m_TaskResult, sbWhereClause);
                WhereClauseUtility.AddIntEqual("t.异常结束原因编码", m_TaskAbendReason, sbWhereClause);
                WhereClauseUtility.AddStringEqual("t.车辆编码", m_AmbCode, sbWhereClause);
                if (m_IsCharge == "True")
                {
                    if (sbWhereClause.Length > 0)
                        sbWhereClause.Append(" AND (SELECT COUNT(*) FROM ").Append(builder.InitialCatalog).Append(".dbo.M_PatientCharge cr Where cr.IFArrearage='False' and cr.TaskCode=t.任务编码) >0");
                    else
                        sbWhereClause.Append(" WHERE (SELECT COUNT(*) FROM ").Append(builder.InitialCatalog).Append(".dbo.M_PatientCharge cr Where cr.IFArrearage='False' and cr.TaskCode=t.任务编码) >0");
                }
                else if (m_IsCharge == "False")
                {
                    if (sbWhereClause.Length > 0)
                        sbWhereClause.Append(" AND (SELECT COUNT(*) FROM ").Append(builder.InitialCatalog).Append(".dbo.M_PatientCharge cr Where cr.TaskCode=t.任务编码) =0");
                    else
                        sbWhereClause.Append(" WHERE (SELECT COUNT(*) FROM ").Append(builder.InitialCatalog).Append(".dbo.M_PatientCharge cr Where cr.TaskCode=t.任务编码) =0");
                }
                else if (m_IsCharge == "Qian")
                {
                    if (sbWhereClause.Length > 0)
                        sbWhereClause.Append(" AND (SELECT COUNT(*) FROM ").Append(builder.InitialCatalog).Append(".dbo.M_PatientCharge cr Where cr.IFArrearage='True' and cr.TaskCode=t.任务编码) >0");
                    else
                        sbWhereClause.Append(" WHERE (SELECT COUNT(*) FROM ").Append(builder.InitialCatalog).Append(".dbo.M_PatientCharge cr Where cr.IFArrearage='True' and cr.TaskCode=t.任务编码) >0");
                }
                if (m_IsFill == "True")
                {
                    if (sbWhereClause.Length > 0)
                        sbWhereClause.Append(" AND (SELECT COUNT(*) FROM ").Append(builder.InitialCatalog).Append(".dbo.M_PatientRecord p WHERE p.TaskCode=t.任务编码)>0");
                    else
                        sbWhereClause.Append(" WHERE (SELECT COUNT(*) FROM ").Append(builder.InitialCatalog).Append(".dbo.M_PatientRecord p WHERE p.TaskCode=t.任务编码)>0");
                }
                else if (m_IsFill == "False")
                {
                    if (sbWhereClause.Length > 0)
                        sbWhereClause.Append(" AND (SELECT COUNT(*) FROM ").Append(builder.InitialCatalog).Append(".dbo.M_PatientRecord p WHERE p.TaskCode=t.任务编码)=0 ");
                    else
                        sbWhereClause.Append(" WHERE (SELECT COUNT(*) FROM ").Append(builder.InitialCatalog).Append(".dbo.M_PatientRecord p WHERE p.TaskCode=t.任务编码)=0");
                }
                if (m_CPRIFSuccess != "-1")
                {
                    if (m_CPRIFSuccess =="ROSC")
                        sbWhereClause.Append(" AND (SELECT COUNT(*) FROM ").Append(builder.InitialCatalog).Append(".dbo.M_PatientRecord p WHERE p.TaskCode=t.任务编码 and p.CPRIFSuccess='" + m_CPRIFSuccess + "')>0 ");
                    else if (m_CPRIFSuccess == "已审核")
                        sbWhereClause.Append(" AND (SELECT COUNT(*) FROM ").Append(builder.InitialCatalog).Append(".dbo.M_PatientRecordCPR p WHERE p.TaskCode=t.任务编码 and p.CenterIFAuditForXFFS=1 )>0 ");
                    else if (m_CPRIFSuccess == "未审核")
                        sbWhereClause.Append(" AND (SELECT COUNT(*) FROM ").Append(builder.InitialCatalog).Append(".dbo.M_PatientRecordCPR p WHERE p.TaskCode=t.任务编码 and p.CenterIFAuditForXFFS=0 )>0 ");
                    else if (m_CPRIFSuccess == "通过")
                        sbWhereClause.Append(" AND (SELECT COUNT(*) FROM ").Append(builder.InitialCatalog).Append(".dbo.M_PatientRecordCPR p WHERE p.TaskCode=t.任务编码 and p.CenterAuditResult='" + m_CPRIFSuccess + "' )>0 ");
                    else if (m_CPRIFSuccess == "未通过")
                        sbWhereClause.Append(" AND (SELECT COUNT(*) FROM ").Append(builder.InitialCatalog).Append(".dbo.M_PatientRecordCPR p WHERE p.TaskCode=t.任务编码 and p.CenterAuditResult='" + m_CPRIFSuccess + "' )>0 ");
                    //else
                    //    sbWhereClause.Append(" WHERE (SELECT COUNT(*) FROM ").Append(builder.InitialCatalog).Append(".dbo.M_PatientRecord p WHERE p.TaskCode=t.任务编码 and p.CPRIFSuccess='" + m_CPRIFSuccess + "')>0 ");
                }
                if (m_PatientState != "-1")
                {
                    if (sbWhereClause.Length > 0)
                        sbWhereClause.Append(" AND (SELECT COUNT(*) FROM ").Append(builder.InitialCatalog).Append(".dbo.M_PatientRecord p WHERE p.TaskCode=t.任务编码 and p.MedicalStateCode='" + m_PatientState + "')>0 ");
                    else
                        sbWhereClause.Append(" WHERE (SELECT COUNT(*) FROM ").Append(builder.InitialCatalog).Append(".dbo.M_PatientRecord p WHERE p.TaskCode=t.任务编码 and p.MedicalStateCode='" + m_PatientState + "')>0 ");
                }
                if (searchBound == "4")
                {
                    if (loginInfo.WorkCode != "")
                        sbWhereClause.Append(" and (SELECT COUNT(*) FROM TTaskPersonLink ttpl WHERE t.任务编码=ttpl.任务编码 and ttpl.人员编码 = (select 编码 from TPerson where 工号='" + loginInfo.WorkCode + "'))>0 ");

                    else
                        sbWhereClause.Append(" and 1=2 ");
                }

                sbSQL.Append(sbWhereClause);

                sbSQL.Append(" select 任务编码1=t.任务编码,司机=dbo.GetStr(t.任务编码,3),医生=dbo.GetStr(t.任务编码,4),担架工=dbo.GetStr(t.任务编码,6),护士=dbo.GetStr(t.任务编码,5)");
                sbSQL.Append(@"into #tempY 
                        FROM TTask t 
                        JOIN TAlarmEvent ae ON t.事件编码=ae.事件编码 ");
                sbSQL.Append("where t.任务编码>@TaskCodeB and t.任务编码<=@TaskCodeE  ");
                WhereClauseUtility.AddDateTimeGreaterThan("ae.首次受理时刻", m_BeginTime, sbSQL);
                WhereClauseUtility.AddDateTimeLessThan("ae.首次受理时刻", m_EndTime, sbSQL);
                WhereClauseUtility.AddStringEqual("ae.是否测试", isTest, sbSQL);//是否测试

                sbSQL.Append(" select identity(int,1,1) as 行号, * into #tempZ ");
                sbSQL.Append(" from #tempX t ");
                sbSQL.Append(" left join #tempY t1 on t.任务编码=t1.任务编码1 ");
                sbSQL.Append(" where 1=1 ");

                //if (m_IsFill == "True")
                //{
                //    sbSQL.Append(" AND 病历数>0");
                //}
                //else if (m_IsFill == "False")
                //{
                //    sbSQL.Append(" AND 病历数=0 ");
                //}
                switch (searchBound)//检索范围
                {
                    case "1"://全部
                        WhereClauseUtility.AddStringEqual("中心编码", m_CenterCode, sbSQL);
                        WhereClauseUtility.AddStringEqual("分站编码", m_StationCode, sbSQL);
                        WhereClauseUtility.AddStringLike("司机", m_Driver, sbSQL);
                        WhereClauseUtility.AddStringLike("医生", m_Doctor, sbSQL);
                        WhereClauseUtility.AddStringLike("担架工", m_Litter, sbSQL);
                        WhereClauseUtility.AddStringLike("护士", m_Nurse, sbSQL);
                        break;
                    case "2"://分中心
                        WhereClauseUtility.AddStringEqual("分站编码", m_StationCode, sbSQL);
                        WhereClauseUtility.AddStringEqual("中心编码", loginInfo.DispatchSubCenterID, sbSQL);
                        WhereClauseUtility.AddStringLike("司机", m_Driver, sbSQL);
                        WhereClauseUtility.AddStringLike("医生", m_Doctor, sbSQL);
                        WhereClauseUtility.AddStringLike("担架工", m_Litter, sbSQL);
                        WhereClauseUtility.AddStringLike("护士", m_Nurse, sbSQL);
                        break;
                    case "3"://分站
                        WhereClauseUtility.AddStringEqual("中心编码", m_CenterCode, sbSQL);
                        WhereClauseUtility.AddStringEqual("分站编码", loginInfo.DispatchSationID, sbSQL);
                        WhereClauseUtility.AddStringLike("司机", m_Driver, sbSQL);
                        WhereClauseUtility.AddStringLike("医生", m_Doctor, sbSQL);
                        WhereClauseUtility.AddStringLike("担架工", m_Litter, sbSQL);
                        WhereClauseUtility.AddStringLike("护士", m_Nurse, sbSQL);
                        break;
                    case "4"://本人
                        //WhereClauseUtility.AddStringEqual("中心编码", m_CenterCode, sbSQL);
                        //WhereClauseUtility.AddStringEqual("分站编码", m_StationCode, sbSQL);
                        break;
                }
                sbSQL.Append(@"ORDER BY 首次受理时刻 desc ");

                sbSQL.Append(@"select top " + rows + " A.*  from #tempZ A where 行号>" + (page - 1) * rows + " order by 行号 ");
                sbSQL.Append(@"SELECT count(*) FROM #tempZ t ");
                sbSQL.Append(" drop table #tempX,#tempY,#tempZ ");
                DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sbSQL.ToString(), null);
                List<M_TaskPartInfo> list = new List<M_TaskPartInfo>();
                if (ds.Tables[0].Rows.Count > 0)
                {
                    M_TaskPartInfo info = new M_TaskPartInfo();
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        info = new M_TaskPartInfo();
                        info.TaskCode = dr["任务编码"].ToString();
                        info.PatientName = dr["患者姓名"].ToString();
                        info.FirstAcceptTime = dr["首次受理时刻"].ToString();
                        info.ContactTelephone = dr["联系电话"].ToString();
                        info.EventType = dr["事件类型"].ToString();
                        info.SceneAddress = dr["现场地址"].ToString();
                        info.AcceptType = dr["受理类型"].ToString();
                        info.Station = dr["分站名称"].ToString();
                        info.ActualLogo = dr["实际标识"].ToString();
                        info.Driver = dr["司机"].ToString();
                        info.Doctor = dr["医生"].ToString();
                        info.Nurse = dr["护士"].ToString();
                        info.OutCarResults = dr["出车结果"].ToString();
                        info.PatientNumber = dr["病历数"].ToString();
                        //info.ChargeNumber = dr["收费数"].ToString();
                        //info.IsSubmit = dr["是否提交"].ToString();

                        list.Add(info);
                    }
                }
                else
                {
                    list = null;
                    var resultK = new { total = 0, rows = 0 };//当查询没有数据返回;
                    return resultK;
                }
                int total = Convert.ToInt32(ds.Tables[1].Rows[0][0]);
                var result = new { total = total, rows = list };
                //return list;
                return result;
            }
            catch (Exception ex)
            {
                LogUtility.Error("M_PatientRecordDAL/GetTasks("+m_BeginTime+" -- "+ m_EndTime+")", ex.ToString());
                return null;
            }
        }
        #endregion

        #region 出车信息-根据任务编码查询病历列表
        /// <summary>
        /// 根据任务编码查询病历列表
        /// </summary>
        /// <param name="taskCode"></param>
        /// <returns></returns>
        public List<M_PatientRecord> GetPatientCommonByTask(string taskCode)
        {
            try
            {
                StringBuilder sbSQL = new StringBuilder();
                sbSQL.Append(" select TaskCode,PatientOrder,Name,Sex,Age=case when AgeType='不详' then Age else Age+AgeType end ");
                sbSQL.Append(" ,AgeType,LocalAddress,SendAddress,FirstAidEffect,DiseaseCooperation ");
                sbSQL.Append(" ,SubmitLogo,SubmitTime,PatientVersion,MedicalState=case when MedicalStateCode=0 then '暂存' when MedicalStateCode=1 then '已提交' else '' end ");
                sbSQL.Append(",IsCharge=isnull((SELECT IsCharge=case when cr.PatientOrder>0 then '已收' end FROM M_PatientCharge cr Where cr.TaskCode=pr.TaskCode and cr.PatientOrder=pr.PatientOrder),'未收')  ");
                sbSQL.Append(" from M_PatientRecord pr");
                //sbSQL.Append(" left join  ").Append(builder.InitialCatalog).Append(".dbo.TPerson tp on tp.编码=tpr.经办人编码 ");
                sbSQL.Append(" where pr.TaskCode='").Append(taskCode).Append("' ");
                DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sbSQL.ToString(), null);
                List<M_PatientRecord> list = new List<M_PatientRecord>();
                if (ds.Tables[0].Rows.Count > 0)
                {
                    M_PatientRecord info = new M_PatientRecord();
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        info = new M_PatientRecord();
                        info.TaskCode = dr["TaskCode"].ToString();
                        info.PatientOrder = Convert.ToInt32(dr["PatientOrder"]);
                        info.Name = dr["Name"].ToString();
                        info.Sex = dr["Sex"].ToString();
                        info.Age = dr["Age"].ToString();
                        info.LocalAddress = dr["LocalAddress"].ToString();
                        info.SendAddress = dr["SendAddress"].ToString();
                        info.FirstAidEffect = dr["FirstAidEffect"].ToString();
                        info.DiseaseCooperation = dr["DiseaseCooperation"].ToString();
                        info.SubmitLogo = DBConvert.ConvertDBTypeToNullableBool(dr["SubmitLogo"]);
                        info.SubmitTime = DBConvert.ConvertDBTypeToNullable(dr["SubmitTime"]);
                        info.PatientVersion = dr["PatientVersion"].ToString();//事件类型
                        info.ForArea = dr["MedicalState"].ToString();//病历状态--借助区域字段
                        info.ForHelpTelephone = dr["IsCharge"].ToString();//是否收费--借助区域字段
                        list.Add(info);
                    }
                }
                else
                {
                    list = null;
                }
                return list;
            }
            catch (Exception ex)
            {
                LogUtility.Error("M_PatientRecordDAL/GetPatientCommonByTask()", ex.ToString());
                return null;
            }
        }
        #endregion

        #endregion

        #region 根据病种分类的名称串来获取主诉和现病史
        /// <summary>
        /// 根据病种分类的名称串来获取主诉和现病史
        /// <param name="TemplateName">模版名称</param>
        /// <returns></returns>
        public object GetAllTemplate(string TemplateName)
        {
            List<string> AlarmReasons = new List<string>();
            List<string> HistoryOfPresentIllness = new List<string>();
            List<M_ZCaseTemplate> listmodel = new List<M_ZCaseTemplate>();
            string ret = "";//主诉串
            string xbs = "";//现病史串
            string[] names = TemplateName.Split(',');
            for (int i = 0; i < names.Length; i++)
            {
                string name = names[i].ToString();
                var list = from t in db.M_ZCaseTemplate
                           where t.Name == name
                           select new
                            {
                                ID = t.ID,
                                Name = t.Name,
                                AlarmReason = t.AlarmReason,
                                HistoryOfPresentIllness = t.HistoryOfPresentIllness,
                                SN = t.SN,
                                IsActive = t.IsActive
                            };
                //if (!string.IsNullOrEmpty(names[i]))
                //{
                //    list = list.Where(p => p.Name.Contains(names[i]));//like
                //}
                if (list.ToList().Count > 0)
                {
                    AlarmReasons.Add((list.FirstOrDefault().AlarmReason == "" || list.FirstOrDefault().AlarmReason == null) ? "" : list.FirstOrDefault().AlarmReason);//主诉
                    HistoryOfPresentIllness.Add((list.FirstOrDefault().HistoryOfPresentIllness == "" || list.FirstOrDefault().HistoryOfPresentIllness == null) ? "" : list.FirstOrDefault().HistoryOfPresentIllness);//现病史
                }
            }
            if (AlarmReasons.Count > 0)
                ret = AlarmReasons.Aggregate((commaText, next) => commaText + " " + next);
            if (HistoryOfPresentIllness.Count > 0)
                xbs = HistoryOfPresentIllness.Aggregate((commaText, next) => commaText + " " + next);

            listmodel.Add(new M_ZCaseTemplate
            {
                AlarmReason = ret,
                HistoryOfPresentIllness = xbs
            });
            var result = listmodel.ToList();
            return result;

        }
        #endregion

        #region 根据任务编码获取调度信息
        /// <summary>
        /// 根据任务编码获取调度信息
        /// </summary>
        /// <param name="taskCode"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public M_AttemperData GetAttemperData(string taskCode, string state)
        {
            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(SqlHelper.AttemperConnectionString);//取调度库的链接字符串

                StringBuilder sbSQL = new StringBuilder();
                sbSQL.Append(@"SELECT 
                         TaskCode = tt.任务编码,Name = tae.患者姓名,Sex = tae.性别,Age = CONVERT(varchar(20),tae.年龄),
                         ForHelpPhone =tae.呼救电话,LinkPerson =tae.联系人,ContactTelephone = tae.联系电话,CallOrder=tt.用户流水号,
                         AlarmTypeCode =CONVERT(varchar(20),tae.事件类型编码),AlarmType = tzae.名称,Area = tae.区域,
                         XCoordinate = CONVERT(varchar(20),tae.X坐标),YCoordinate = CONVERT(varchar(20),tae.Y坐标),AlarmReason = tae.主诉,
                         LocalAddress = tae.现场地址,SendAddress = tae.送往地点,StationCode =tt.分站编码,Station =ts.名称,
                         AmbulanceCode =tt.车辆编码,TelephoneRingingTime =tt.电话振铃时刻,GenerationTaskTime =tt.生成任务时刻,
                         DrivingTime =tt.出车时刻,ArriveSceneTime =tt.到达现场时刻,LeaveSceneTime=tt.离开现场时刻,ArriveDestinationTime =tt.到达医院时刻,
                         CompleteTime =tt.完成时刻,BackTime =tt.返回站中时刻,Disptcher = tp.姓名,
                         OutResult = case when tt.是否正常结束 = 1 then '正常' else '异常结束' end,");
                sbSQL.Append("Driver = ").Append(builder.InitialCatalog).Append(".dbo.GetStr(tt.任务编码,3),");
                sbSQL.Append("Doctor = ").Append(builder.InitialCatalog).Append(".dbo.GetStr(tt.任务编码,4),");
                sbSQL.Append("Nurse = ").Append(builder.InitialCatalog).Append(".dbo.GetStr(tt.任务编码,5),");
                sbSQL.Append("StretcherBearers = ").Append(builder.InitialCatalog).Append(".dbo.GetStr(tt.任务编码,6),");
                sbSQL.Append("FirstAider = ").Append(builder.InitialCatalog).Append(".dbo.GetStr(tt.任务编码,7) ");

                sbSQL.Append("FROM ").Append(builder.InitialCatalog).Append(".dbo.TTask tt ");
                sbSQL.Append("LEFT JOIN  ").Append(builder.InitialCatalog).Append(".dbo.TAlarmEvent tae on tt.事件编码=tae.事件编码 ");
                sbSQL.Append("LEFT JOIN  ").Append(builder.InitialCatalog).Append(".dbo.TPerson tp on tt.责任调度人编码=tp.编码  ");
                sbSQL.Append("LEFT JOIN  ").Append(builder.InitialCatalog).Append(".dbo.TStation ts on tt.分站编码=ts.编码  ");
                sbSQL.Append("LEFT JOIN  ").Append(builder.InitialCatalog).Append(".dbo.TZTaskAbendReason tztar on tt.异常结束原因编码=tztar.编码 ");
                sbSQL.Append("LEFT JOIN  ").Append(builder.InitialCatalog).Append(".dbo.TZAlarmEventType tzae ON tae.事件类型编码 = tzae.编码 ");
                sbSQL.Append("where tt.任务编码='" + taskCode + "' ");

                M_AttemperData result = db.Database.SqlQuery<M_AttemperData>(sbSQL.ToString()).FirstOrDefault();

                return result;
            }
            catch (Exception ex)
            {
                LogUtility.Error("M_PatientRecordDAL/GetAttemperData()", ex.ToString());
                return null;
            }
        }
        #endregion

        #region 获取病历序号最大值
        public int GetPatientMaxOrder(string taskCode)
        {
            M_PatientRecord pInfo = new M_PatientRecord();
            int PatientOrder = 0;
            var query = from pr in db.M_PatientRecord
                        where pr.TaskCode == taskCode
                        select pr.PatientOrder;
            if (query.Count() > 0)
                PatientOrder = query.Max() + 1;
            else
                PatientOrder = 1;
            return PatientOrder;
        }
        #endregion

        #endregion

        #region 获取病历主表及子表信息

        #region 获取病历主表及附表信息
        /// <summary>
        /// 获取病历主表及附表信息
        /// </summary>
        /// <param name="taskCode"></param>
        /// <param name="patientOrder"></param>
        /// <param name="info"></param>
        /// <param name="pra"></param>
        /// <param name="prCPR"></param>
        public void GetPatientInfo(string taskCode, int patientOrder, out object info
            , out M_PatientRecordAppend pra, out M_PatientRecordCPR prCPR)
        {
            info = (from pi in db.M_PatientRecord
                    where pi.TaskCode == taskCode && pi.PatientOrder == patientOrder
                    select pi).FirstOrDefault();//病历主表

            pra = (from pi in db.M_PatientRecordAppend
                   where pi.TaskCode == taskCode && pi.PatientOrder == patientOrder
                   select pi).FirstOrDefault();//病历附表--体检等信息

            prCPR = (from pi in db.M_PatientRecordCPR
                     where pi.TaskCode == taskCode && pi.PatientOrder == patientOrder
                     select pi).FirstOrDefault();//心肺复苏
        }
        /// <summary>
        /// PAD 查询已有病历
        /// </summary>
        /// <param name="taskCode"></param>
        /// <param name="patientOrder"></param>
        /// <param name="info"></param>
        /// <param name="pra"></param>
        public void GetPatientInfo(string taskCode, int patientOrder, out object info
           , out M_PatientRecordAppend pra)
        {
            info = dalContext.IM_PatientRecordDAL.GetModelWithOutTrace(a => a.PatientOrder == patientOrder && a.TaskCode == taskCode);

            pra = dalContext.IM_PatientRecordAppendDAL.GetModelWithOutTrace(a => a.PatientOrder == patientOrder && a.TaskCode == taskCode);
               
        }
        #endregion

        #region 获取病历救治记录列表
        public List<M_PatientRecordRescue> GetPRRescueList(int page, int rows, ref int rowCounts, string taskCode, int patientOrder)
        {
            var q = from ps in db.M_PatientRecordRescue
                    where ps.TaskCode == taskCode && ps.PatientOrder == patientOrder
                    select ps;

            rowCounts = q.Count();
            var t = q.OrderBy(u => u.PeriodOfTimeCode).ThenBy(u => u.DisposeOrder).Skip((page - 1) * rows).Take(rows).ToList();
            return t;
        }
        #endregion

        #region 获取病历救治记录主表
        public void GetPRRescueInfo(string taskCode, int patientOrder, string rescueRecordCode, int disposeOrder
            , out M_PatientRecordRescue prrInfo)
        {
            prrInfo = (from pi in db.M_PatientRecordRescue
                       where pi.TaskCode == taskCode && pi.PatientOrder == patientOrder && pi.RescueRecordCode == rescueRecordCode && pi.DisposeOrder == disposeOrder
                       select pi).FirstOrDefault();//救治记录主表

        }
        #endregion

        #region 获取救治记录序号最大值--暂时没用
        public string GetRescueRecordMaxCode(string taskCode, int patientOrder)
        {
            M_PatientRecordRescue pInfo = new M_PatientRecordRescue();
            string RescueRecordCode = "";
            var query = from pr in db.M_PatientRecordRescue
                        where pr.TaskCode == taskCode && pr.PatientOrder == patientOrder
                        select pr.RescueRecordCode;
            if (query.Count() > 0)
                RescueRecordCode = taskCode + (query.Count() + 1);
            else
                RescueRecordCode = taskCode + "1";
            return RescueRecordCode;
        }
        #endregion

        #endregion


        #region 新增

        #region 新增病历主表和附表
        public bool Insert(object info, M_PatientRecordAppend pra
            , M_PatientRecordCPR prCPR, List<M_PatientRecordDiag> prDiag
            , List<M_PatientRecordECGImpressions> prECG)
        {
            using (var tran = db.Database.BeginTransaction())
            {
                try
                {
                    M_PatientRecord pInfo = (M_PatientRecord)info;

                    db.M_PatientRecord.Add(pInfo);//插入病历主表

                    if (pra != null)
                    {
                        db.M_PatientRecordAppend.Add(pra);//插入病历附表--体检等
                    }
                    if (prCPR != null)
                    {
                        //prCPR.RegistrationTime = DateTime.Now;//登记时间
                        db.M_PatientRecordCPR.Add(prCPR);//插入病历附表--心肺复苏
                    }
                    if (prDiag != null)
                    {
                        db.M_PatientRecordDiag.AddRange(prDiag);//插入初步诊断子表
                    }
                    if (prECG != null)
                    {
                        db.M_PatientRecordECGImpressions.AddRange(prECG);//插入心电图印象子表
                    }

                    db.SaveChanges();//将在此上下文中所做的所有更改保存到基础数据库
                    tran.Commit(); //提交事务
                    //transaction.Complete();//这一句如果注释后，则上面的保存不会被提交到DB中
                    return true;
                }
                catch (Exception ex)
                {
                    LogUtility.Error("M_PatientRecordDAL/Insert()", ex.ToString());
                    if (tran != null)
                        tran.Rollback();  //回滚事务
                    return false;
                }
            }
        }
        #endregion

        #region 新增救治记录
        //新增救治记录
        public bool InsertPRRescue(M_PatientRecordRescue info, List<M_PatientRecordMeasure> measureSCO
            , List<M_PatientRecordDrug> drugSCO, List<M_PatientRecordSanitation> sanitationSCO
            , List<M_PatientRecordLossDrug> lossDrugSCO, List<M_PatientRecordLossSanitation> lossSanitationSCO)
        {
            using (var tran = db.Database.BeginTransaction())
            {
                try
                {
                    string RescueRecordCode = "";
                    M_PatientRecordRescue q = (from pr in db.M_PatientRecordRescue
                                               orderby pr.RescueRecordCode descending
                                               where pr.TaskCode == info.TaskCode && pr.PatientOrder == info.PatientOrder
                                               select pr).FirstOrDefault();
                    if (q != null)
                        RescueRecordCode = info.TaskCode + (Convert.ToInt32(q.RescueRecordCode.Replace(info.TaskCode, "")) + 1).ToString();
                    //RescueRecordCode = info.TaskCode + (query.Count() + 1);
                    else
                        RescueRecordCode = info.TaskCode + "1";
                    info.RescueRecordCode = RescueRecordCode;//救治记录编码
                    db.M_PatientRecordRescue.Add(info);//插入救治记录

                    if (info != null)
                    {
                        if (measureSCO != null)
                        {
                            List<M_PatientRecordMeasure> measure = new List<M_PatientRecordMeasure>();
                            for (int k = 0; k < measureSCO.Count; k++)
                            {
                                M_PatientRecordMeasure SInfo = measureSCO[k];
                                //M_PatientRecordMeasure MeasureInfo = measureSCO[k];
                                I_Material c = (from m in db.I_Material
                                                where m.ID == SInfo.RescueMeasureCode
                                                select m).FirstOrDefault();
                                SInfo.TaskCode = SInfo.TaskCode;
                                SInfo.PatientOrder = SInfo.PatientOrder;
                                SInfo.RescueRecordCode = RescueRecordCode;//救治记录编码
                                SInfo.DisposeOrder = SInfo.DisposeOrder;//处理序号
                                SInfo.RescueMeasureCode = c.ID;//救治措施编码
                                SInfo.RescueMeasureName = c.Name;//救治措施
                                SInfo.OtherTypeID = c.OtherTypeID;//
                                SInfo.NumberOfTimes = SInfo.NumberOfTimes;//次数
                                SInfo.Price = c.RealPrice;//单价
                                int FeeScale = c.FeeScale == null ? 1 : Convert.ToInt32(c.FeeScale);
                                int Dosage = 1;
                                if (FeeScale == -1)
                                {
                                    Dosage = 1;
                                }
                                else if (FeeScale == 1)
                                {
                                    Dosage = Convert.ToInt32(SInfo.NumberOfTimes);
                                }
                                else
                                {
                                    if (FeeScale < 0 && (-FeeScale <= Convert.ToInt32(SInfo.NumberOfTimes)))
                                    {
                                        Dosage = -FeeScale;
                                    }
                                    else
                                    {
                                        Dosage = Convert.ToInt32(SInfo.NumberOfTimes);
                                    }
                                }

                                SInfo.TotalPrice = c.RealPrice * Dosage;//合计金额
                                SInfo.ChargeWay = Decimal.Round(decimal.Parse(c.RealPrice.ToString()), 2, MidpointRounding.AwayFromZero) + "元/" + c.TDictionary2.Name;//收费方式
                                SInfo.Remark = SInfo.Remark;
                                SInfo.SelectOrder = k + 1;
                                measure.Add(SInfo);
                                //measureSCO.Add(SInfo);
                                //db.M_PatientRecordMeasure.Add(SInfo);//插入救治措施子表
                            }
                            db.M_PatientRecordMeasure.AddRange(measure);//插入救治措施子表
                        }
                        if (drugSCO != null)
                        {
                            List<M_PatientRecordDrug> drug = new List<M_PatientRecordDrug>();
                            for (int l = 0; l < drugSCO.Count; l++)
                            {
                                M_PatientRecordDrug DInfo = drugSCO[l];
                                I_Material c = (from m in db.I_Material
                                                where m.ID == DInfo.DrugCode
                                                select m).FirstOrDefault();
                                DInfo.TaskCode = DInfo.TaskCode;
                                DInfo.PatientOrder = DInfo.PatientOrder;
                                DInfo.RescueRecordCode = RescueRecordCode;
                                DInfo.DisposeOrder = DInfo.DisposeOrder;//处理序号
                                DInfo.DrugCode = c.ID;
                                DInfo.DrugName = c.Name;
                                DInfo.GiveMedicineWay = DInfo.GiveMedicineWay;
                                string ChargeWay = "";
                                string RealPrice = Decimal.Round(decimal.Parse(c.RealPrice.ToString()), 2, MidpointRounding.AwayFromZero).ToString();
                                if (c.FeeScale > 1)
                                { ChargeWay = RealPrice + "元/" + "1-" + c.FeeScale + c.TDictionary2.Name; }
                                else
                                { ChargeWay = RealPrice + "元/" + c.TDictionary2.Name; }
                                DInfo.ChargeWay = ChargeWay;//收费方式
                                DInfo.Specifications = c.Specification;//规格
                                DInfo.Unit = c.TDictionary2.Name;//单位
                                DInfo.Dosage = DInfo.Dosage;//用量
                                DInfo.Price = c.RealPrice;//单价
                                int FeeScale = c.FeeScale == null ? 1 : Convert.ToInt32(c.FeeScale);
                                int Dosage = 1;
                                if (Convert.ToInt32(DInfo.Dosage) % FeeScale > 0)
                                {
                                    Dosage = Convert.ToInt32(Math.Round(Convert.ToDouble(DInfo.Dosage / FeeScale), 0)) + 1;
                                }
                                else
                                {
                                    Dosage = Convert.ToInt32(Math.Round(Convert.ToDouble(DInfo.Dosage / FeeScale), 0));
                                }
                                DInfo.TotalPrice = c.RealPrice * Dosage;//合计金额
                                DInfo.TotalDose = DInfo.TotalDose;//合计用量
                                DInfo.Remark = DInfo.Remark;//备注
                                DInfo.SelectOrder = l + 1;//选择序号
                                DInfo.FeeScale = c.FeeScale;
                                drug.Add(DInfo);
                                //db.M_PatientRecordDrug.Add(DInfo);//插入药品子表
                            }
                            db.M_PatientRecordDrug.AddRange(drug);//插入药品子表
                        }
                        if (sanitationSCO != null)
                        {
                            List<M_PatientRecordSanitation> sanitation = new List<M_PatientRecordSanitation>();
                            for (int k = 0; k < sanitationSCO.Count; k++)
                            {
                                M_PatientRecordSanitation SInfo = sanitationSCO[k];
                                //M_PatientRecordSanitation SInfo = null;
                                I_Material c = (from m in db.I_Material
                                                where m.ID == SInfo.SanitationCode
                                                select m).FirstOrDefault();
                                SInfo.TaskCode = SInfo.TaskCode;
                                SInfo.PatientOrder = SInfo.PatientOrder;
                                SInfo.RescueRecordCode = RescueRecordCode;
                                SInfo.DisposeOrder = SInfo.DisposeOrder;//处理序号
                                SInfo.SanitationCode = c.ID;
                                SInfo.SanitationName = c.Name;
                                SInfo.NumberOfTimes = SInfo.NumberOfTimes;//次数
                                SInfo.Price = c.RealPrice;
                                SInfo.TotalPrice = c.RealPrice * SInfo.NumberOfTimes;//合计金额
                                SInfo.ChargeWay = Decimal.Round(decimal.Parse(c.RealPrice.ToString()), 2, MidpointRounding.AwayFromZero) + "元/" + c.TDictionary2.Name;//收费方式
                                SInfo.SelectOrder = k + 1;
                                sanitation.Add(SInfo);
                            }
                            db.M_PatientRecordSanitation.AddRange(sanitation);//插入耗材子表
                        }
                        if (lossDrugSCO != null)
                        {
                            List<M_PatientRecordLossDrug> lossDrug = new List<M_PatientRecordLossDrug>();
                            for (int k = 0; k < lossDrugSCO.Count; k++)
                            {
                                M_PatientRecordLossDrug SInfo = lossDrugSCO[k];
                                I_Material c = (from m in db.I_Material
                                                where m.ID == SInfo.DrugCode
                                                select m).FirstOrDefault();
                                SInfo.TaskCode = SInfo.TaskCode;
                                SInfo.PatientOrder = SInfo.PatientOrder;
                                SInfo.RescueRecordCode = RescueRecordCode;
                                SInfo.DisposeOrder = SInfo.DisposeOrder;
                                SInfo.DrugCode = c.ID;
                                SInfo.DrugName = c.Name;//药品名称
                                SInfo.GiveMedicineWay = SInfo.GiveMedicineWay;//应该是空
                                string ChargeWay = "";
                                string RealPrice = Decimal.Round(decimal.Parse(c.RealPrice.ToString()), 2, MidpointRounding.AwayFromZero).ToString();
                                if (c.FeeScale > 1)
                                { ChargeWay = RealPrice + "元/" + "1-" + c.FeeScale + c.TDictionary2.Name; }
                                else
                                { ChargeWay = RealPrice + "元/" + c.TDictionary2.Name; }
                                SInfo.ChargeWay = ChargeWay;//收费方式
                                SInfo.Specifications = c.Specification;//规格
                                SInfo.Unit = c.TDictionary2.Name;//单位
                                SInfo.Dosage = SInfo.Dosage;//用量
                                SInfo.Price = c.RealPrice;//单价
                                SInfo.TotalPrice = c.RealPrice * SInfo.Dosage;//合计金额
                                SInfo.TotalDose = SInfo.TotalDose;//合计用量
                                SInfo.Remark = SInfo.Remark;//备注
                                SInfo.SelectOrder = k + 1;
                                lossDrug.Add(SInfo);
                            }
                            db.M_PatientRecordLossDrug.AddRange(lossDrug);//插入损耗药品子表
                        }
                        if (lossSanitationSCO != null)
                        {
                            List<M_PatientRecordLossSanitation> lossSanitation = new List<M_PatientRecordLossSanitation>();
                            for (int k = 0; k < lossSanitationSCO.Count; k++)
                            {
                                M_PatientRecordLossSanitation SInfo = lossSanitationSCO[k];
                                I_Material c = (from m in db.I_Material
                                                where m.ID == SInfo.SanitationCode
                                                select m).FirstOrDefault();
                                SInfo.TaskCode = SInfo.TaskCode;
                                SInfo.PatientOrder = SInfo.PatientOrder;
                                SInfo.RescueRecordCode = RescueRecordCode;//救治记录编码
                                SInfo.DisposeOrder = SInfo.DisposeOrder;//处理序号
                                SInfo.SanitationCode = c.ID;
                                SInfo.SanitationName = c.Name;
                                SInfo.NumberOfTimes = SInfo.NumberOfTimes;//次数
                                SInfo.Price = c.RealPrice;
                                SInfo.ChargeWay = Decimal.Round(decimal.Parse(c.RealPrice.ToString()), 2, MidpointRounding.AwayFromZero) + "元/" + c.TDictionary2.Name;//收费方式
                                SInfo.TotalPrice = c.RealPrice * SInfo.NumberOfTimes;//合计金额
                                SInfo.Remark = SInfo.Remark;
                                SInfo.SelectOrder = k + 1;
                                lossSanitation.Add(SInfo);
                            }
                            db.M_PatientRecordLossSanitation.AddRange(lossSanitation);//插入损耗卫生材料子表
                        }
                    }

                    db.SaveChanges();//将在此上下文中所做的所有更改保存到基础数据库
                    tran.Commit(); //提交事务
                    //transaction.Complete();//这一句如果注释后，则上面的保存不会被提交到DB中
                    return true;
                }
                catch (Exception ex)
                {
                    LogUtility.Error("M_PatientRecordDAL/InsertPRRescue()", ex.ToString());
                    if (tran != null)
                        tran.Rollback();  //回滚事务
                    return false;
                }
            }
        }
        #endregion

        #endregion

        #region 修改病历主表和附表
        public bool Update(object info, M_PatientRecordAppend pra
            , M_PatientRecordCPR prCPR, List<M_PatientRecordDiag> prDiag
            , List<M_PatientRecordECGImpressions> prECG)
        {
            using (var tran = db.Database.BeginTransaction())
            {
                try
                {
                    #region 修改病历主表

                    M_PatientRecord pInfo = (M_PatientRecord)info;
                    List<M_PatientRecordTrace> crackList = new List<M_PatientRecordTrace>();//病历修改记录
                    int j = 0;//病历修改记录主键
                    DateTime modifyTime = DateTime.Now;//修改时间
                    string modifyPerson = pInfo.LastUpdatePerson;//修改人

                    M_PatientRecord originInfo = dalContext.IM_PatientRecordDAL.GetModelWithOutTrace(a => a.PatientOrder == pInfo.PatientOrder && a.TaskCode == pInfo.TaskCode);

                    var entry = db.Entry(pInfo);
                    if (entry.State == EntityState.Detached)
                    {

                        //如果已经被上下文追踪
                        if (originInfo != null)
                        {
                            //originInfo = pInfo;//修改病历主表
                            pInfo.ForHelpTelephone = originInfo.ForHelpTelephone;
                            pInfo.OriginalTaskType = originInfo.OriginalTaskType;//原事件类型--不修改
                            pInfo.ForArea = originInfo.ForArea;
                            pInfo.AgentCode = originInfo.AgentCode;
                            pInfo.AgentWorkID = originInfo.AgentWorkID;//填写人工号
                            pInfo.AgentName = originInfo.AgentName;//填写人姓名
                            pInfo.BeginFillPatientTime = originInfo.BeginFillPatientTime;//开始填写病历时间
                            pInfo.MedicalRecordGenerationTime = originInfo.MedicalRecordGenerationTime;//病历生成时间
                            if (originInfo.FormCompleteLogo == true)
                            {
                                pInfo.FormCompleteLogo = originInfo.FormCompleteLogo;//归档标识
                                pInfo.FormCompleteTime = originInfo.FormCompleteTime;//归档时间
                            }
                            pInfo.ChargeOrder = originInfo.ChargeOrder;
                            if (originInfo.SubmitLogo == true)
                            {
                                pInfo.SubmitLogo = originInfo.SubmitLogo;//提交标志
                                pInfo.SubmitTime = originInfo.SubmitTime;//提交时间
                                pInfo.MedicalStateCode = originInfo.MedicalStateCode;//不修改病历状态
                            }
                            pInfo.SubCenterIFSpotChecks = originInfo.SubCenterIFSpotChecks;//分中心是否抽查
                            pInfo.SubCenterPerson = originInfo.SubCenterPerson;
                            pInfo.SubCenterSpotChecksTime = originInfo.SubCenterSpotChecksTime;
                            pInfo.SubCenterSpotChecksResult = originInfo.SubCenterSpotChecksResult;
                            pInfo.SubCenterSpotChecksRmark = originInfo.SubCenterSpotChecksRmark;
                            pInfo.CenterIFSpotChecks = originInfo.CenterIFSpotChecks;//中心是否抽查
                            pInfo.CenterSpotChecksPerson = originInfo.CenterSpotChecksPerson;
                            pInfo.CenterSpotChecksTime = originInfo.CenterSpotChecksTime;
                            pInfo.CenterSpotChecksResult = originInfo.CenterSpotChecksResult;
                            pInfo.CenterSpotChecksRmark = originInfo.CenterSpotChecksRmark;

                            //pInfo.IMEI = originInfo.IMEI;

                            db.M_PatientRecord.Attach(pInfo);//将给定实体附加到集的基础上下文中，将实体以“未更改”的状态放置到上下文；
                            db.Entry(pInfo).State = EntityState.Modified;


                        }
                        //else//如果不在当前上下文追踪
                        //{
                        //    db.Entry(pInfo).State = EntityState.Modified;
                        //}

                    }
                    #endregion

                    #region 修改病历附表--体检等
                    M_PatientRecordAppend praInfo = dalContext.IM_PatientRecordAppendDAL.GetModelWithOutTrace(a => a.PatientOrder == pra.PatientOrder && a.TaskCode == pra.TaskCode);
                    if (praInfo != null)
                    {
                        pra.PhysicalExaminationSupplement = praInfo.PhysicalExaminationSupplement;//体检补充描述，应该没有，数据库应该删除
                        db.M_PatientRecordAppend.Attach(pra);//将给定实体附加到集的基础上下文中，将实体以“未更改”的状态放置到上下文；
                        db.Entry(pra).State = EntityState.Modified;//修改病历附表--体检等
                    }
                    #endregion

                    #region 修改病历附表--心肺复苏
                    M_PatientRecordCPR prCPRInfo = dalContext.IM_PatientRecordCPRDAL.GetModelWithOutTrace(a => a.PatientOrder == prCPR.PatientOrder && a.TaskCode == prCPR.TaskCode);
                    if (prCPRInfo != null)
                    {
                        prCPR.EmergencyTime = prCPRInfo.EmergencyTime;//急救时长(页面没有显示，最后删除)
                        prCPR.ECGMonitoringTime = prCPRInfo.ECGMonitoringTime;//心电监护时间(页面没有显示，最后删除)
                        prCPR.BeforeResuscitationECGDiagnosis = prCPRInfo.BeforeResuscitationECGDiagnosis;
                        prCPR.BeforeResuscitationSaO2 = prCPRInfo.BeforeResuscitationSaO2;
                        prCPR.IFLeaveHospital = prCPRInfo.IFLeaveHospital;//中心审核人员填写
                        prCPR.LeaveHospitalDate = prCPRInfo.LeaveHospitalDate;
                        prCPR.LeaveHospitalCPC = prCPRInfo.LeaveHospitalCPC;
                        prCPR.VerifyResults = prCPRInfo.VerifyResults;
                        prCPR.VerifyPerson = prCPRInfo.VerifyPerson;
                        prCPR.VerifyTime = prCPRInfo.VerifyTime;
                        prCPR.RegistrationPerson = prCPRInfo.RegistrationPerson;
                        prCPR.RegistrationTime = prCPRInfo.RegistrationTime;
                        prCPR.CenterIFAuditForXFFS = prCPRInfo.CenterIFAuditForXFFS;//中心审核心肺复苏
                        prCPR.CenterAuditPerson = prCPRInfo.CenterAuditPerson;
                        prCPR.CenterAuditTime = prCPRInfo.CenterAuditTime;
                        prCPR.CenterAuditResult = prCPRInfo.CenterAuditResult;
                        prCPR.CenterNotThroughReason = prCPRInfo.CenterNotThroughReason;
                        db.M_PatientRecordCPR.Attach(prCPR);//将给定实体附加到集的基础上下文中，将实体以“未更改”的状态放置到上下文；
                        db.Entry(prCPR).State = EntityState.Modified;//修改病历附表--心肺复苏
                    }
                    #endregion

                    #region 修改初步诊断子表
                    var dqry0 = from infos in db.M_PatientRecordDiag
                                where infos.TaskCode == pInfo.TaskCode && infos.PatientOrder == pInfo.PatientOrder
                                select infos;
                    foreach (var inf in dqry0)
                        db.M_PatientRecordDiag.Remove(inf);//删除初步诊断子表
                    //db.M_PatientRecordDiag.RemoveRange(prDiag);//删除初步诊断子表
                    if (prDiag != null)
                    {
                        db.M_PatientRecordDiag.AddRange(prDiag);//插入初步诊断子表
                    }
                    #endregion

                    #region 修改心电图印象子表
                    var dqry1 = from infos in db.M_PatientRecordECGImpressions
                                where infos.TaskCode == pInfo.TaskCode && infos.PatientOrder == pInfo.PatientOrder
                                select infos;
                    foreach (var inf in dqry1)
                        db.M_PatientRecordECGImpressions.Remove(inf);//删除心电图印象
                    //db.M_PatientRecordECGImpressions.RemoveRange(prECG);//删除心电图印象
                    if (prECG != null)
                    {
                        db.M_PatientRecordECGImpressions.AddRange(prECG);//插入心电图印象子表
                    }
                    #endregion

                    #region 提交病历后修改病历痕迹
                    int flag = 0;//病历状态：0.普通保存(还没有提交)；1.提交；2.提交后的保存
                    //提交后保存的
                    if (originInfo.MedicalStateCode == 1)
                    {
                        flag = 2;//提交后的保存
                        #region 插入病历主表痕迹
                        //获得所有property的信息
                        PropertyInfo[] proOrigin = originInfo.GetType().GetProperties();
                        //PropertyInfo[] proNew = pInfo.GetType().GetProperties();
                        for (int i = 0; i < proOrigin.Length; i++)
                        {
                            object obj1 = proOrigin[i].GetValue(originInfo, null);
                            object obj2 = proOrigin[i].GetValue(info, null);
                            obj1 = obj1 == null ? "" : obj1;
                            obj2 = obj2 == null ? "" : obj2;
                            //proOrigin[i].Name != "LastModifyTime" && proOrigin[i].Name != "LastModifyPerson"
                            if (!obj1.Equals(obj2) && proOrigin[i].Name != "FormCompleteLogo" && proOrigin[i].Name != "FormCompleteTime"
                                && proOrigin[i].Name != "SubmitLogo" && proOrigin[i].Name != "SubmitTime" && proOrigin[i].Name != "MedicalStateCode"
                                && proOrigin[i].Name != "OutStationCode")
                            {
                                j = j + 1;
                                M_PatientRecordTrace crackInfo = new M_PatientRecordTrace();
                                var query = from a in db.M_PatientRecordTrace
                                            select a.ID;
                                if (query.Count() > 0 && j == 1)
                                {
                                    crackInfo.ID = query.Max() + 1;
                                    j = crackInfo.ID;
                                }
                                else if (j > 1)
                                { crackInfo.ID = j; }
                                else
                                { crackInfo.ID = 1; }
                                crackInfo.ModifyTime = modifyTime;//修改时间
                                crackInfo.ModifyPerson = modifyPerson;//修改人
                                crackInfo.TaskCode = pInfo.TaskCode;//任务编码
                                crackInfo.PatientOrder = pInfo.PatientOrder;//序号
                                crackInfo.ModifyItem = GetDescriptionByTableName("M_PatientRecord", proOrigin[i].Name);//修改项
                                crackInfo.OriginalValue = obj1.ToString();//原值
                                crackInfo.NewValue = obj2.ToString();//新值
                                crackInfo.OwnershipType = 1;
                                crackList.Add(crackInfo);
                            }
                        }
                        #endregion

                        #region 如果病历提交插入病历附表--体检等修改痕迹
                        //获得所有property的信息
                        PropertyInfo[] proOrigin1 = praInfo.GetType().GetProperties();
                        //PropertyInfo[] proNew1 = pra.GetType().GetProperties();
                        for (int i = 0; i < proOrigin1.Length; i++)
                        {
                            object obj1 = proOrigin1[i].GetValue(praInfo, null);
                            object obj2 = proOrigin1[i].GetValue(pra, null);
                            obj1 = obj1 == null ? "" : obj1;
                            obj2 = obj2 == null ? "" : obj2;
                            if (!obj1.Equals(obj2) && proOrigin1[i].Name != "OpenReactionScore" && proOrigin1[i].Name != "LanguageReactionScore" && proOrigin1[i].Name != "MotionResponseScore"
                                && proOrigin1[i].Name != "GCSScore" && proOrigin1[i].Name != "PositionScore" && proOrigin1[i].Name != "DamageWayScore"
                                && proOrigin1[i].Name != "CirculationChangeScore" && proOrigin1[i].Name != "BreathingChangeScore" && proOrigin1[i].Name != "ConsciousnessChangeScore"
                                && proOrigin1[i].Name != "TIScore" && proOrigin1[i].Name != "HeartRatePerMinuteScore" && proOrigin1[i].Name != "BreathingScore"
                                && proOrigin1[i].Name != "MuscleTensionScore" && proOrigin1[i].Name != "LaryngealReflexScore" && proOrigin1[i].Name != "SkinColorScore"
                                && proOrigin1[i].Name != "ApgarScore" && proOrigin1[i].Name != "SymptomPendingInvestigationCodes" && proOrigin1[i].Name != "FirstImpressionCodes"
                                && proOrigin1[i].Name != "ECGImpressionCodes" && proOrigin1[i].Name != "ECGImpressionRetestICodes" && proOrigin1[i].Name != "ECGImpressionRetestIICodes")
                            {
                                j = j + 1;
                                M_PatientRecordTrace crackInfo = new M_PatientRecordTrace();
                                var query = from a in db.M_PatientRecordTrace
                                            select a.ID;
                                if (query.Count() > 0 && j == 1)
                                {
                                    crackInfo.ID = query.Max() + 1;
                                    j = crackInfo.ID;
                                }
                                else if (j > 1)
                                { crackInfo.ID = j; }
                                else
                                { crackInfo.ID = 1; }
                                crackInfo.ModifyTime = modifyTime;//修改时间
                                crackInfo.ModifyPerson = modifyPerson;//修改人
                                crackInfo.TaskCode = pra.TaskCode;//任务编码
                                crackInfo.PatientOrder = pra.PatientOrder;//序号
                                crackInfo.ModifyItem = GetDescriptionByTableName("M_PatientRecordAppend", proOrigin1[i].Name);//修改项
                                crackInfo.OriginalValue = obj1.ToString();//原值
                                crackInfo.NewValue = obj2.ToString();//新值
                                crackInfo.OwnershipType = 2;
                                crackList.Add(crackInfo);
                            }
                        }
                        #endregion

                        #region 如果病历提交插入病历附表--心肺复苏
                        //获得所有property的信息
                        PropertyInfo[] proOrigin2 = prCPRInfo.GetType().GetProperties();
                        //PropertyInfo[] proNew2 = prCPR.GetType().GetProperties();
                        for (int i = 0; i < proOrigin2.Length; i++)
                        {
                            object obj1 = proOrigin2[i].GetValue(prCPRInfo, null);
                            object obj2 = proOrigin2[i].GetValue(prCPR, null);
                            obj1 = obj1 == null ? "" : obj1;
                            obj2 = obj2 == null ? "" : obj2;
                            if (!obj1.Equals(obj2))
                            {
                                j = j + 1;
                                M_PatientRecordTrace crackInfo = new M_PatientRecordTrace();
                                var query = from a in db.M_PatientRecordTrace
                                            select a.ID;
                                if (query.Count() > 0 && j == 1)
                                {
                                    crackInfo.ID = query.Max() + 1;
                                    j = crackInfo.ID;
                                }
                                else if (j > 1)
                                { crackInfo.ID = j; }
                                else
                                { crackInfo.ID = 1; }
                                crackInfo.ModifyTime = modifyTime;//修改时间
                                crackInfo.ModifyPerson = modifyPerson;//修改人
                                crackInfo.TaskCode = prCPRInfo.TaskCode;//任务编码
                                crackInfo.PatientOrder = prCPRInfo.PatientOrder;//序号
                                crackInfo.ModifyItem = GetDescriptionByTableName("M_PatientRecordCPR", proOrigin2[i].Name);//修改项
                                crackInfo.OriginalValue = obj1.ToString();//原值
                                crackInfo.NewValue = obj2.ToString();//新值
                                crackInfo.OwnershipType = 3;
                                crackList.Add(crackInfo);
                            }
                        }
                        #endregion

                    }
                    #endregion

                    if (crackList.Count > 0)
                    {
                        db.M_PatientRecordTrace.AddRange(crackList);//插入病历修改记录表
                    }

                    db.SaveChanges();//将在此上下文中所做的所有更改保存到基础数据库
                    tran.Commit(); //提交事务
                    return true;
                }
                catch (Exception ex)
                {
                    LogUtility.Error("M_PatientRecordDAL/Update(WEB)", ex.ToString());
                    if (tran != null)
                        tran.Rollback();  //回滚事务
                    return false;
                }
            }
        }
        public bool Update(object info, M_PatientRecordAppend pra)
        {
            using (var tran = db.Database.BeginTransaction())
            {
                try
                {
                    #region 修改病历主表

                    M_PatientRecord pInfo = (M_PatientRecord)info;
                    List<M_PatientRecordTrace> crackList = new List<M_PatientRecordTrace>();//病历修改记录
                    int j = 0;//病历修改记录主键
                    DateTime modifyTime = DateTime.Now;//修改时间
                    string modifyPerson = pInfo.LastUpdatePerson;//修改人

                    M_PatientRecord originInfo = dalContext.IM_PatientRecordDAL.GetModelWithOutTrace(a => a.PatientOrder == pInfo.PatientOrder && a.TaskCode == pInfo.TaskCode);

                    var entry = db.Entry(pInfo);
                    if (entry.State == EntityState.Detached)
                    {

                        //如果已经被上下文追踪
                        if (originInfo != null)
                        {
                            //originInfo = pInfo;//修改病历主表
                            pInfo.ForHelpTelephone = originInfo.ForHelpTelephone;
                            pInfo.OriginalTaskType = originInfo.OriginalTaskType;//原事件类型--不修改
                            pInfo.ForArea = originInfo.ForArea;
                            pInfo.AgentCode = originInfo.AgentCode;
                            pInfo.AgentWorkID = originInfo.AgentWorkID;//填写人工号
                            pInfo.AgentName = originInfo.AgentName;//填写人姓名
                            pInfo.BeginFillPatientTime = originInfo.BeginFillPatientTime;//开始填写病历时间
                            pInfo.MedicalRecordGenerationTime = originInfo.MedicalRecordGenerationTime;//病历生成时间
                            if (originInfo.FormCompleteLogo == true)
                            {
                                pInfo.FormCompleteLogo = originInfo.FormCompleteLogo;//归档标识
                                pInfo.FormCompleteTime = originInfo.FormCompleteTime;//归档时间
                            }
                            pInfo.ChargeOrder = originInfo.ChargeOrder;
                            if (originInfo.SubmitLogo == true)
                            {
                                pInfo.SubmitLogo = originInfo.SubmitLogo;//提交标志
                                pInfo.SubmitTime = originInfo.SubmitTime;//提交时间
                                pInfo.MedicalStateCode = originInfo.MedicalStateCode;//不修改病历状态
                            }
                            pInfo.SubCenterIFSpotChecks = originInfo.SubCenterIFSpotChecks;//分中心是否抽查
                            pInfo.SubCenterPerson = originInfo.SubCenterPerson;
                            pInfo.SubCenterSpotChecksTime = originInfo.SubCenterSpotChecksTime;
                            pInfo.SubCenterSpotChecksResult = originInfo.SubCenterSpotChecksResult;
                            pInfo.SubCenterSpotChecksRmark = originInfo.SubCenterSpotChecksRmark;
                            pInfo.CenterIFSpotChecks = originInfo.CenterIFSpotChecks;//中心是否抽查
                            pInfo.CenterSpotChecksPerson = originInfo.CenterSpotChecksPerson;
                            pInfo.CenterSpotChecksTime = originInfo.CenterSpotChecksTime;
                            pInfo.CenterSpotChecksResult = originInfo.CenterSpotChecksResult;
                            pInfo.CenterSpotChecksRmark = originInfo.CenterSpotChecksRmark;

                            //pInfo.IMEI = originInfo.IMEI;
                            
                            db.M_PatientRecord.Attach(pInfo);//将给定实体附加到集的基础上下文中，将实体以“未更改”的状态放置到上下文；
                            db.Entry(pInfo).State = EntityState.Modified;


                        }
                        //else//如果不在当前上下文追踪
                        //{
                        //    db.Entry(pInfo).State = EntityState.Modified;
                        //}

                    }
                    #endregion

                    #region 修改病历附表--体检等
                    M_PatientRecordAppend praInfo = dalContext.IM_PatientRecordAppendDAL.GetModelWithOutTrace(a => a.PatientOrder == pra.PatientOrder && a.TaskCode == pra.TaskCode);
                    if (praInfo != null)
                    {
                        pra.PhysicalExaminationSupplement = praInfo.PhysicalExaminationSupplement;//体检补充描述，应该没有，数据库应该删除
                        db.M_PatientRecordAppend.Attach(pra);//将给定实体附加到集的基础上下文中，将实体以“未更改”的状态放置到上下文；
                        db.Entry(pra).State = EntityState.Modified;//修改病历附表--体检等
                    }
                    #endregion
                    db.SaveChanges();//将在此上下文中所做的所有更改保存到基础数据库
                    tran.Commit(); //提交事务
                    return true;
                }
                catch (Exception ex)
                {
                    LogUtility.Error("M_PatientRecordDAL/Update(PAD)", ex.ToString());
                    if (tran != null)
                        tran.Rollback();  //回滚事务
                    return false;
                }
            }
        }
        #endregion

        #region 修改病历--救治记录主表和子表
        public bool UpdatePRRescue(M_PatientRecordRescue prrInfo, List<M_PatientRecordMeasure> measureSCO
            , List<M_PatientRecordDrug> drugSCO, List<M_PatientRecordSanitation> sanitationSCO
            , List<M_PatientRecordLossDrug> lossDrugSCO, List<M_PatientRecordLossSanitation> lossSanitationSCO)
        {
            using (var tran = db.Database.BeginTransaction())
            {
                try
                {
                    List<M_PatientRecordTrace> crackList = new List<M_PatientRecordTrace>();//病历修改记录
                    int j = 0;//病历修改记录主键
                    DateTime modifyTime = DateTime.Now;//修改时间
                    string modifyPerson = prrInfo.LastUpdatePerson;//修改人
                    M_PatientRecord PRoriginInfo = dalContext.IM_PatientRecordDAL.GetModelWithOutTrace(a => a.PatientOrder == prrInfo.PatientOrder && a.TaskCode == prrInfo.TaskCode);

                    string RescueRecordCode = "";
                    var entry = db.Entry(prrInfo);
                    bool origin = false;
                    if (entry.State == EntityState.Detached)
                    {
                        M_PatientRecordRescue originInfo = dalContext.IM_PatientRecordRescueDAL.GetModelWithOutTrace(a => a.PatientOrder == prrInfo.PatientOrder && a.TaskCode == prrInfo.TaskCode && a.RescueRecordCode == prrInfo.RescueRecordCode);
                        //如果已经被上下文追踪
                        if (originInfo != null)
                        {
                            origin = true;
                            RescueRecordCode = originInfo.RescueRecordCode;
                            prrInfo.RescueXFFSMouldChoose = originInfo.RescueXFFSMouldChoose;//心肺复苏模板选择
                            db.M_PatientRecordRescue.Attach(prrInfo);//将给定实体附加到集的基础上下文中，将实体以“未更改”的状态放置到上下文；
                            db.Entry(prrInfo).State = EntityState.Modified;


                            #region 提交病历后修改病历痕迹
                            int flag = 0;//病历状态：0.普通保存(还没有提交)；1.提交；2.提交后的保存
                            //提交后保存的
                            if (PRoriginInfo != null)
                            {
                                if (PRoriginInfo.MedicalStateCode == 1)
                                {
                                    flag = 2;//提交后的保存
                                    #region 插入病历救治记录痕迹
                                    //获得所有property的信息
                                    PropertyInfo[] proOrigin = originInfo.GetType().GetProperties();
                                    PropertyInfo[] proNew = prrInfo.GetType().GetProperties();
                                    for (int i = 0; i < proOrigin.Length; i++)
                                    {
                                        object obj1 = proOrigin[i].GetValue(originInfo, null);
                                        object obj2 = proOrigin[i].GetValue(prrInfo, null);
                                        obj1 = obj1 == null ? "" : obj1;
                                        obj2 = obj2 == null ? "" : obj2;
                                        if (!obj1.Equals(obj2) && proOrigin[i].Name != "MeasureCodes" && proOrigin[i].Name != "DrugCodes"
                                             && proOrigin[i].Name != "SanitationCodes" && proOrigin[i].Name != "LossDrugCodes" && proOrigin[i].Name != "LossSanitationCodes")
                                        {
                                            j = j + 1;
                                            M_PatientRecordTrace crackInfo = new M_PatientRecordTrace();
                                            var query = from a in db.M_PatientRecordTrace
                                                        select a.ID;
                                            if (query.Count() > 0 && j == 1)
                                            {
                                                crackInfo.ID = query.Max() + 1;
                                                j = crackInfo.ID;
                                            }
                                            else if (j > 1)
                                            { crackInfo.ID = j; }
                                            else
                                            { crackInfo.ID = 1; }
                                            crackInfo.ModifyTime = modifyTime;//修改时间
                                            crackInfo.ModifyPerson = modifyPerson;//修改人
                                            crackInfo.TaskCode = prrInfo.TaskCode;//任务编码
                                            crackInfo.PatientOrder = prrInfo.PatientOrder;//序号
                                            crackInfo.ModifyItem = GetDescriptionByTableName("M_PatientRecordRescue", proOrigin[i].Name);//修改项
                                            crackInfo.OriginalValue = obj1.ToString();//原值
                                            crackInfo.NewValue = obj2.ToString();//新值
                                            crackInfo.OwnershipType = 5;
                                            crackList.Add(crackInfo);
                                        }
                                    }
                                    #endregion
                                }
                            }
                            #endregion
                        }

                    }
                    if (prrInfo != null && origin == true)
                    {
                        #region 修改救治措施子表
                        var dqry0 = from infos in db.M_PatientRecordMeasure
                                    where infos.TaskCode == prrInfo.TaskCode && infos.PatientOrder == prrInfo.PatientOrder && infos.RescueRecordCode == prrInfo.RescueRecordCode
                                    select infos;
                        foreach (var inf in dqry0)
                            db.M_PatientRecordMeasure.Remove(inf);//删除救治措施子表
                        if (measureSCO != null)
                        {
                            List<M_PatientRecordMeasure> measure = new List<M_PatientRecordMeasure>();
                            for (int k = 0; k < measureSCO.Count; k++)
                            {
                                M_PatientRecordMeasure SInfo = measureSCO[k];
                                //M_PatientRecordMeasure MeasureInfo = measureSCO[k];
                                I_Material c = (from m in db.I_Material
                                                where m.ID == SInfo.RescueMeasureCode
                                                select m).FirstOrDefault();
                                SInfo.TaskCode = SInfo.TaskCode;
                                SInfo.PatientOrder = SInfo.PatientOrder;
                                SInfo.RescueRecordCode = RescueRecordCode;//救治记录编码
                                SInfo.DisposeOrder = SInfo.DisposeOrder;//处理序号
                                SInfo.RescueMeasureCode = c.ID;//救治措施编码
                                SInfo.RescueMeasureName = c.Name;//救治措施
                                SInfo.OtherTypeID = c.OtherTypeID;//
                                SInfo.NumberOfTimes = SInfo.NumberOfTimes;//次数
                                SInfo.Price = c.RealPrice;//单价
                                int FeeScale = c.FeeScale == null ? 1 : Convert.ToInt32(c.FeeScale);
                                int Dosage = 1;
                                if (FeeScale == -1)
                                {
                                    Dosage = 1;
                                }
                                else if (FeeScale == 1)
                                {
                                    Dosage = Convert.ToInt32(SInfo.NumberOfTimes);
                                }
                                else
                                {
                                    if (FeeScale < 0 && (-FeeScale <= Convert.ToInt32(SInfo.NumberOfTimes)))
                                    {
                                        Dosage = -FeeScale;
                                    }
                                    else
                                    {
                                        Dosage = Convert.ToInt32(SInfo.NumberOfTimes);
                                    }
                                }

                                SInfo.TotalPrice = c.RealPrice * Dosage;//合计金额
                                SInfo.ChargeWay = Decimal.Round(decimal.Parse(c.RealPrice.ToString()), 2, MidpointRounding.AwayFromZero) + "元/" + c.TDictionary2.Name;//收费方式
                                SInfo.Remark = SInfo.Remark;
                                SInfo.SelectOrder = k + 1;
                                measure.Add(SInfo);
                                //measureSCO.Add(SInfo);
                                //db.M_PatientRecordMeasure.Add(SInfo);//插入救治措施子表
                            }
                            db.M_PatientRecordMeasure.AddRange(measure);//插入救治措施子表
                        }
                        #endregion

                        #region 修改药品子表
                        var dqry1 = from infos in db.M_PatientRecordDrug
                                    where infos.TaskCode == prrInfo.TaskCode && infos.PatientOrder == prrInfo.PatientOrder && infos.RescueRecordCode == prrInfo.RescueRecordCode
                                    select infos;
                        foreach (var inf in dqry1)
                            db.M_PatientRecordDrug.Remove(inf);//删除药品子表
                        if (drugSCO != null)
                        {
                            List<M_PatientRecordDrug> drug = new List<M_PatientRecordDrug>();
                            for (int l = 0; l < drugSCO.Count; l++)
                            {
                                M_PatientRecordDrug DInfo = drugSCO[l];
                                I_Material c = (from m in db.I_Material
                                                where m.ID == DInfo.DrugCode
                                                select m).FirstOrDefault();
                                DInfo.TaskCode = DInfo.TaskCode;
                                DInfo.PatientOrder = DInfo.PatientOrder;
                                DInfo.RescueRecordCode = RescueRecordCode;
                                DInfo.DisposeOrder = DInfo.DisposeOrder;//处理序号
                                DInfo.DrugCode = c.ID;
                                DInfo.DrugName = c.Name;
                                DInfo.GiveMedicineWay = DInfo.GiveMedicineWay;
                                string ChargeWay = "";
                                string RealPrice = Decimal.Round(decimal.Parse(c.RealPrice.ToString()), 2, MidpointRounding.AwayFromZero).ToString();
                                if (c.FeeScale > 1)
                                { ChargeWay = RealPrice + "元/" + "1-" + c.FeeScale + c.TDictionary2.Name; }
                                else
                                { ChargeWay = RealPrice + "元/" + c.TDictionary2.Name; }
                                DInfo.ChargeWay = ChargeWay;//收费方式
                                DInfo.Specifications = c.Specification;//规格
                                DInfo.Unit = c.TDictionary2.Name;//单位
                                DInfo.Dosage = DInfo.Dosage;//用量
                                DInfo.Price = c.RealPrice;//单价
                                int FeeScale = c.FeeScale == null ? 1 : Convert.ToInt32(c.FeeScale);
                                int Dosage = 1;
                                if (Convert.ToInt32(DInfo.Dosage) % FeeScale > 0)
                                {
                                    Dosage = Convert.ToInt32(Math.Round(Convert.ToDouble(DInfo.Dosage / FeeScale), 0)) + 1;
                                }
                                else
                                {
                                    Dosage = Convert.ToInt32(Math.Round(Convert.ToDouble(DInfo.Dosage / FeeScale), 0));
                                }
                                DInfo.TotalPrice = c.RealPrice * Dosage;//合计金额
                                DInfo.TotalDose = DInfo.TotalDose;//合计用量
                                DInfo.Remark = DInfo.Remark;//备注
                                DInfo.SelectOrder = l + 1;//选择序号
                                DInfo.FeeScale = c.FeeScale;
                                drug.Add(DInfo);
                                //db.M_PatientRecordDrug.Add(DInfo);//插入药品子表
                            }
                            db.M_PatientRecordDrug.AddRange(drug);//插入药品子表
                        }
                        #endregion

                        #region 修改耗材子表
                        var dqry2 = from infos in db.M_PatientRecordSanitation
                                    where infos.TaskCode == prrInfo.TaskCode && infos.PatientOrder == prrInfo.PatientOrder && infos.RescueRecordCode == prrInfo.RescueRecordCode
                                    select infos;
                        foreach (var inf in dqry2)
                            db.M_PatientRecordSanitation.Remove(inf);//删除耗材子表
                        if (sanitationSCO != null)
                        {
                            List<M_PatientRecordSanitation> sanitation = new List<M_PatientRecordSanitation>();
                            for (int k = 0; k < sanitationSCO.Count; k++)
                            {
                                M_PatientRecordSanitation SInfo = sanitationSCO[k];
                                //M_PatientRecordSanitation SInfo = null;
                                I_Material c = (from m in db.I_Material
                                                where m.ID == SInfo.SanitationCode
                                                select m).FirstOrDefault();
                                SInfo.TaskCode = SInfo.TaskCode;
                                SInfo.PatientOrder = SInfo.PatientOrder;
                                SInfo.RescueRecordCode = RescueRecordCode;
                                SInfo.DisposeOrder = SInfo.DisposeOrder;//处理序号
                                SInfo.SanitationCode = c.ID;
                                SInfo.SanitationName = c.Name;
                                SInfo.NumberOfTimes = SInfo.NumberOfTimes;//次数
                                SInfo.Price = c.RealPrice;
                                SInfo.TotalPrice = c.RealPrice * SInfo.NumberOfTimes;//合计金额
                                SInfo.ChargeWay = Decimal.Round(decimal.Parse(c.RealPrice.ToString()), 2, MidpointRounding.AwayFromZero) + "元/" + c.TDictionary2.Name;//收费方式
                                SInfo.SelectOrder = k + 1;
                                sanitation.Add(SInfo);
                            }
                            db.M_PatientRecordSanitation.AddRange(sanitation);//插入耗材子表
                        }
                        #endregion

                        #region 修改损耗药品子表
                        var dqry3 = from infos in db.M_PatientRecordLossDrug
                                    where infos.TaskCode == prrInfo.TaskCode && infos.PatientOrder == prrInfo.PatientOrder && infos.RescueRecordCode == prrInfo.RescueRecordCode
                                    select infos;
                        foreach (var inf in dqry3)
                            db.M_PatientRecordLossDrug.Remove(inf);//删除损耗药品子表
                        if (lossDrugSCO != null)
                        {
                            List<M_PatientRecordLossDrug> lossDrug = new List<M_PatientRecordLossDrug>();
                            for (int k = 0; k < lossDrugSCO.Count; k++)
                            {
                                M_PatientRecordLossDrug SInfo = lossDrugSCO[k];
                                I_Material c = (from m in db.I_Material
                                                where m.ID == SInfo.DrugCode
                                                select m).FirstOrDefault();
                                SInfo.TaskCode = SInfo.TaskCode;
                                SInfo.PatientOrder = SInfo.PatientOrder;
                                SInfo.RescueRecordCode = RescueRecordCode;
                                SInfo.DisposeOrder = SInfo.DisposeOrder;
                                SInfo.DrugCode = c.ID;
                                SInfo.DrugName = c.Name;//药品名称
                                SInfo.GiveMedicineWay = SInfo.GiveMedicineWay;//应该是空
                                string ChargeWay = "";
                                string RealPrice = Decimal.Round(decimal.Parse(c.RealPrice.ToString()), 2, MidpointRounding.AwayFromZero).ToString();
                                if (c.FeeScale > 1)
                                { ChargeWay = RealPrice + "元/" + "1-" + c.FeeScale + c.TDictionary2.Name; }
                                else
                                { ChargeWay = RealPrice + "元/" + c.TDictionary2.Name; }
                                SInfo.ChargeWay = ChargeWay;//收费方式
                                SInfo.Specifications = c.Specification;//规格
                                SInfo.Unit = c.TDictionary2.Name;//单位
                                SInfo.Dosage = SInfo.Dosage;//用量
                                SInfo.Price = c.RealPrice;//单价
                                SInfo.TotalPrice = c.RealPrice * SInfo.Dosage;//合计金额
                                SInfo.TotalDose = SInfo.TotalDose;//合计用量
                                SInfo.Remark = SInfo.Remark;//备注
                                SInfo.SelectOrder = k + 1;
                                lossDrug.Add(SInfo);
                            }
                            db.M_PatientRecordLossDrug.AddRange(lossDrug);//插入损耗药品子表
                        }
                        #endregion

                        #region 修改损耗药品子表
                        var dqry4 = from infos in db.M_PatientRecordLossSanitation
                                    where infos.TaskCode == prrInfo.TaskCode && infos.PatientOrder == prrInfo.PatientOrder && infos.RescueRecordCode == prrInfo.RescueRecordCode
                                    select infos;
                        foreach (var inf in dqry4)
                            db.M_PatientRecordLossSanitation.Remove(inf);//删除损耗药品子表
                        if (lossSanitationSCO != null)
                        {
                            List<M_PatientRecordLossSanitation> lossSanitation = new List<M_PatientRecordLossSanitation>();
                            for (int k = 0; k < lossSanitationSCO.Count; k++)
                            {
                                M_PatientRecordLossSanitation SInfo = lossSanitationSCO[k];
                                I_Material c = (from m in db.I_Material
                                                where m.ID == SInfo.SanitationCode
                                                select m).FirstOrDefault();
                                SInfo.TaskCode = SInfo.TaskCode;
                                SInfo.PatientOrder = SInfo.PatientOrder;
                                SInfo.RescueRecordCode = RescueRecordCode;//救治记录编码
                                SInfo.DisposeOrder = SInfo.DisposeOrder;//处理序号
                                SInfo.SanitationCode = c.ID;
                                SInfo.SanitationName = c.Name;
                                SInfo.NumberOfTimes = SInfo.NumberOfTimes;//次数
                                SInfo.Price = c.RealPrice;
                                SInfo.ChargeWay = Decimal.Round(decimal.Parse(c.RealPrice.ToString()), 2, MidpointRounding.AwayFromZero) + "元/" + c.TDictionary2.Name;//收费方式
                                SInfo.TotalPrice = c.RealPrice * SInfo.NumberOfTimes;//合计金额
                                SInfo.Remark = SInfo.Remark;
                                SInfo.SelectOrder = k + 1;
                                lossSanitation.Add(SInfo);
                            }
                            db.M_PatientRecordLossSanitation.AddRange(lossSanitation);//插入损耗卫生材料子表
                        }
                        #endregion
                    }

                    db.SaveChanges();//将在此上下文中所做的所有更改保存到基础数据库
                    tran.Commit(); //提交事务
                    return true;
                }
                catch (Exception ex)
                {
                    LogUtility.Error("M_PatientRecordDAL/UpdatePRRescue()", ex.ToString());
                    if (tran != null)
                        tran.Rollback();  //回滚事务
                    return false;
                }
            }
        }
        #endregion

        #region 删除

        #region 删除病历主表信息和附表
        public bool Delete(string TaskCode, int PatientOrder)
        {
            using (var tran = db.Database.BeginTransaction())
            {
                try
                {
                    M_PatientRecord originInfo = db.M_PatientRecord.FirstOrDefault(a => a.PatientOrder == PatientOrder && a.TaskCode == TaskCode);
                    db.M_PatientRecord.Remove(originInfo);//删除病历主表信息

                    M_PatientRecordAppend pra = db.M_PatientRecordAppend.FirstOrDefault(a => a.PatientOrder == PatientOrder && a.TaskCode == TaskCode);
                    if (pra != null)
                        db.M_PatientRecordAppend.Remove(pra);//删除病历附表信息--体检等

                    M_PatientRecordCPR prCPR = db.M_PatientRecordCPR.FirstOrDefault(a => a.PatientOrder == PatientOrder && a.TaskCode == TaskCode);
                    if (prCPR != null)
                        db.M_PatientRecordCPR.Remove(prCPR);//删除病历附表信息--心肺复苏

                    var dqry0 = from infos in db.M_PatientRecordDiag
                                where infos.TaskCode == TaskCode && infos.PatientOrder == PatientOrder
                                select infos;
                    foreach (var inf in dqry0)
                        db.M_PatientRecordDiag.Remove(inf);//删除初步诊断子表
                    //db.M_PatientRecordDiag.RemoveRange(prDiag);//删除初步诊断子表

                    var dqry1 = from infos in db.M_PatientRecordECGImpressions
                                where infos.TaskCode == TaskCode && infos.PatientOrder == PatientOrder
                                select infos;
                    foreach (var inf in dqry1)
                        db.M_PatientRecordECGImpressions.Remove(inf);//删除心电图印象
                    //db.M_PatientRecordECGImpressions.RemoveRange(prECG);//删除心电图印象

                    //删除救治记录主表和相关子表
                    M_PatientRecordRescue prr = db.M_PatientRecordRescue.FirstOrDefault(a => a.PatientOrder == PatientOrder && a.TaskCode == TaskCode);
                    if (prr != null)
                        db.M_PatientRecordRescue.Remove(prr);//删除救治记录主表

                    var dqry3 = from infos in db.M_PatientRecordMeasure
                                where infos.TaskCode == TaskCode && infos.PatientOrder == PatientOrder
                                select infos;
                    foreach (var inf in dqry3)
                        db.M_PatientRecordMeasure.Remove(inf);//删除救治记录--救治措施子表

                    var dqry4 = from infos in db.M_PatientRecordDrug
                                where infos.TaskCode == TaskCode && infos.PatientOrder == PatientOrder
                                select infos;
                    foreach (var inf in dqry4)
                        db.M_PatientRecordDrug.Remove(inf);//删除救治记录--药品子表

                    var dqry5 = from infos in db.M_PatientRecordSanitation
                                where infos.TaskCode == TaskCode && infos.PatientOrder == PatientOrder
                                select infos;
                    foreach (var inf in dqry5)
                        db.M_PatientRecordSanitation.Remove(inf);//删除救治记录--耗材子表

                    var dqry6 = from infos in db.M_PatientRecordLossDrug
                                where infos.TaskCode == TaskCode && infos.PatientOrder == PatientOrder
                                select infos;
                    foreach (var inf in dqry6)
                        db.M_PatientRecordLossDrug.Remove(inf);//删除救治记录--损耗药品子表

                    var dqry7 = from infos in db.M_PatientRecordLossSanitation
                                where infos.TaskCode == TaskCode && infos.PatientOrder == PatientOrder
                                select infos;
                    foreach (var inf in dqry7)
                        db.M_PatientRecordLossSanitation.Remove(inf);//删除救治记录--损耗耗材子表

                    db.SaveChanges();//将在此上下文中所做的所有更改保存到基础数据库
                    tran.Commit(); //提交事务
                    return true;
                }
                catch (Exception ex)
                {
                    LogUtility.Error("M_PatientRecordDAL/Delete()", ex.ToString());
                    if (tran != null)
                        tran.Rollback();  //回滚事务
                    return false;
                }
            }
        }
        #endregion

        #region 删除病历--救治记录和子表
        public bool DeletePRRescue(string TaskCode, int PatientOrder, string RescueRecordCode, int DisposeOrder)
        {
            using (var tran = db.Database.BeginTransaction())
            {
                try
                {
                    //删除救治记录主表和相关子表
                    M_PatientRecordRescue prr = db.M_PatientRecordRescue.FirstOrDefault(a => a.PatientOrder == PatientOrder && a.TaskCode == TaskCode && a.RescueRecordCode == RescueRecordCode);
                    if (prr != null)
                        db.M_PatientRecordRescue.Remove(prr);//删除救治记录主表

                    var dqry3 = from infos in db.M_PatientRecordMeasure
                                where infos.TaskCode == TaskCode && infos.PatientOrder == PatientOrder && infos.RescueRecordCode == RescueRecordCode
                                select infos;
                    foreach (var inf in dqry3)
                        db.M_PatientRecordMeasure.Remove(inf);//删除救治记录--救治措施子表

                    var dqry4 = from infos in db.M_PatientRecordDrug
                                where infos.TaskCode == TaskCode && infos.PatientOrder == PatientOrder && infos.RescueRecordCode == RescueRecordCode
                                select infos;
                    foreach (var inf in dqry4)
                        db.M_PatientRecordDrug.Remove(inf);//删除救治记录--药品子表

                    var dqry5 = from infos in db.M_PatientRecordSanitation
                                where infos.TaskCode == TaskCode && infos.PatientOrder == PatientOrder && infos.RescueRecordCode == RescueRecordCode
                                select infos;
                    foreach (var inf in dqry5)
                        db.M_PatientRecordSanitation.Remove(inf);//删除救治记录--耗材子表

                    var dqry6 = from infos in db.M_PatientRecordLossDrug
                                where infos.TaskCode == TaskCode && infos.PatientOrder == PatientOrder && infos.RescueRecordCode == RescueRecordCode
                                select infos;
                    foreach (var inf in dqry6)
                        db.M_PatientRecordLossDrug.Remove(inf);//删除救治记录--损耗药品子表

                    var dqry7 = from infos in db.M_PatientRecordLossSanitation
                                where infos.TaskCode == TaskCode && infos.PatientOrder == PatientOrder && infos.RescueRecordCode == RescueRecordCode
                                select infos;
                    foreach (var inf in dqry7)
                        db.M_PatientRecordLossSanitation.Remove(inf);//删除救治记录--损耗耗材子表

                    db.SaveChanges();//将在此上下文中所做的所有更改保存到基础数据库
                    tran.Commit(); //提交事务
                    return true;
                }
                catch (Exception ex)
                {
                    LogUtility.Error("M_PatientRecordDAL/DeletePRRescue()", ex.ToString());
                    if (tran != null)
                        tran.Rollback();  //回滚事务
                    return false;
                }
            }
        }
        #endregion

        #endregion

        #region 中心心肺复苏审核
        public bool UpdateAuditCPR(M_PatientRecordCPR info)
        {
            using (var tran = db.Database.BeginTransaction())
            {
                try
                {
                    M_PatientRecordCPR prCPRInfo = dalContext.IM_PatientRecordCPRDAL.GetModelWithOutTrace(a => a.PatientOrder == info.PatientOrder && a.TaskCode == info.TaskCode);
                    if (prCPRInfo != null)
                    {
                        info.Witness = prCPRInfo.Witness;
                        info.CarToBeforeCPR = prCPRInfo.CarToBeforeCPR;
                        info.CarToBeforeDefibrillation = prCPRInfo.CarToBeforeDefibrillation;
                        info.EmergencyTime = prCPRInfo.EmergencyTime;
                        info.ECGMonitoringTime = prCPRInfo.ECGMonitoringTime;
                        info.CardiacArrestReason = prCPRInfo.CardiacArrestReason;
                        info.CardiacArrestReasonSupplement = prCPRInfo.CardiacArrestReasonSupplement;
                        info.BeforeResuscitationECGDiagnosis = prCPRInfo.BeforeResuscitationECGDiagnosis;
                        info.BeforeResuscitationSaO2 = prCPRInfo.BeforeResuscitationSaO2;
                        info.AfterResuscitationBP = prCPRInfo.AfterResuscitationBP;
                        info.AfterResuscitationSaO2 = prCPRInfo.AfterResuscitationSaO2;
                        info.PulsationAppearTime = prCPRInfo.PulsationAppearTime;
                        info.BreatheAppearTime = prCPRInfo.BreatheAppearTime;
                        info.AfterResuscitationECGDiagnosis = prCPRInfo.AfterResuscitationECGDiagnosis;
                        info.IFAdmittedToHospital = prCPRInfo.IFAdmittedToHospital;
                        //info.DoctorFollowUp = prCPRInfo.DoctorFollowUp;
                        db.M_PatientRecordCPR.Attach(info);//将给定实体附加到集的基础上下文中，将实体以“未更改”的状态放置到上下文；
                        db.Entry(info).State = EntityState.Modified;//修改病历附表--心肺复苏
                    }
                    db.SaveChanges();//将在此上下文中所做的所有更改保存到基础数据库
                    tran.Commit(); //提交事务
                    return true;
                }
                catch (Exception ex)
                {
                    LogUtility.Error("M_PatientRecordDAL/UpdateAuditCPR()", ex.ToString());
                    if (tran != null)
                        tran.Rollback();  //回滚事务
                    return false;
                }
            }
        }
        #endregion

        #region 更新抽查病历
        public bool UpdateSpotChecks(M_PatientRecord info, int orderNumber)
        {
            try
            {
                StringBuilder strSQL = new StringBuilder();
                switch (orderNumber)
                {
                    case -1://分中心抽查
                        strSQL.Append(@"update M_PatientRecord set SubCenterIFSpotChecks=1,SubCenterPerson='" + info.SubCenterPerson + "',SubCenterSpotChecksTime='" + DateTime.Now + "',SubCenterSpotChecksResult='" + info.SubCenterSpotChecksResult + "' ");
                        strSQL.Append(@" 
                                    ,SubCenterSpotChecksRmark='" + info.SubCenterSpotChecksRmark + "' ");
                        strSQL.Append(@" 
                                    where TaskCode='" + info.TaskCode + "' and PatientOrder=" + info.PatientOrder + "  ");
                        break;
                    case -2://中心抽查
                        strSQL.Append(@"update M_PatientRecord set CenterIFSpotChecks=1,CenterSpotChecksPerson='" + info.CenterSpotChecksPerson + "',CenterSpotChecksTime='" + DateTime.Now + "',CenterSpotChecksResult='" + info.CenterSpotChecksResult + "' ");
                        strSQL.Append(@" 
                                    ,CenterSpotChecksRmark='" + info.CenterSpotChecksRmark + "' ");
                        strSQL.Append(@" 
                                    where TaskCode='" + info.TaskCode + "' and PatientOrder=" + info.PatientOrder + "  ");
                        break;
                    default:
                        break;
                }

                int ds = SqlHelper.ExecuteNonQuery(SqlHelper.MainConnectionString, CommandType.Text, strSQL.ToString(), null);
                return ds > 0 ? true : false;
            }
            catch (Exception ex)
            {
                LogUtility.Error("M_PatientRecordDAL/UpdateSpotChecks()", ex.ToString());
                return false;
            }
        }
        #endregion

        #region 更新回访
        public bool UpdateFollowUp(string TaskCode, int PatientOrder, string DoctorFollowUp)
        {
            try
            {
                StringBuilder strSQL = new StringBuilder();
                strSQL.Append(@"update M_PatientRecordCPR set DoctorFollowUp='" + DoctorFollowUp + "' ");
                strSQL.Append(@" where TaskCode='" + TaskCode + "' and PatientOrder=" + PatientOrder + "  ");

                int ds = SqlHelper.ExecuteNonQuery(SqlHelper.MainConnectionString, CommandType.Text, strSQL.ToString(), null);
                return ds > 0 ? true : false;
            }
            catch (Exception ex)
            {
                LogUtility.Error("M_PatientRecordDAL/UpdateFollowUp()", ex.ToString());
                return false;
            }
        }
        #endregion

        #region 根据表名来获取对应的字段说明
        public string GetDescriptionByTableName(string tableName, string FieldName)
        {
            //
            StringBuilder sbSQL = new StringBuilder();//sql语句
            sbSQL.Append(" SELECT Id=objs.name,Name =isnull(props.[value],'')");
            sbSQL.Append(" FROM syscolumns cols ");
            sbSQL.Append(" inner join sysobjects objs on cols.id= objs.id and  objs.xtype='U' and  objs.name<>'dtproperties' ");
            sbSQL.Append(" left join sys.extended_properties props on cols.id=props.major_id and cols.colid=props.minor_id ");
            sbSQL.Append(" WHERE objs.name='" + tableName + "' ");
            sbSQL.Append(" and cols.name='" + FieldName + "' ");

            var Templist = db.Database.SqlQuery<M_CheckModel>(sbSQL.ToString());
            string mDescription = Templist.FirstOrDefault().Name;
            return mDescription;

        }
        #endregion

        #region PAD用--根据用户流水号取任务编码20150714
        public string GetTaskCodeByTaskOrder(string TaskOrder)
        {
            try
            {
                if (TaskOrder == "")
                {
                    TaskOrder = "100402zx1014";//测试用
                }
                string Part = TaskOrder.Substring(0, 5);
                StringBuilder sbSQL = new StringBuilder();
                sbSQL.Append("declare @TaskCodeB char(22) ");
                sbSQL.Append("declare @TaskCodeE char(22) ");
                sbSQL.Append("set @TaskCodeB = '20'+'" + Part + "'+'000000000000' ");
                sbSQL.Append("set @TaskCodeE = '20'+'" + Part + "'+'999999999999' ");

                sbSQL.Append(" select 任务编码");
                sbSQL.Append(" from TTask tt");
                sbSQL.Append(" where 任务编码>@TaskCodeB and tt.任务编码<=@TaskCodeE  ");
                sbSQL.Append(" and 用户流水号='").Append(TaskOrder).Append("' ");
                DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sbSQL.ToString(), null);
                string TaskCode = "";
                if (ds.Tables[0].Rows.Count > 0)
                {
                    TaskCode = ds.Tables[0].Rows[0][0].ToString();
                }
                else
                {
                    TaskCode = "";
                }
                return TaskCode;
            }
            catch (Exception ex)
            {
                LogUtility.Error("M_PatientRecordDAL/GetTaskCodeByTaskOrder()", ex.ToString());
                return "";
            }
        }
        #endregion
    }
}
