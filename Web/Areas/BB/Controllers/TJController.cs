using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Anke.SHManage.Web.Areas.BB.Controllers
{
    public class TJController : Controller
    {
        //
        // GET: /BB/TJ/

        /// <summary>
        /// 出车次数分段统计表
        /// </summary>
        public void TJ_CCCSFD()
        {
            string ReportName = "TJ_CCCSFD.rdlc";
            string aspxUrl = "~/Reports/TimeModeWebForm.aspx?ReportName=" + ReportName;
            Response.Redirect(aspxUrl);
        }
        /// <summary>
        /// 出车情况统计表
        /// </summary>
        public void TJ_CCQK()
        {
            string ReportName = "TJ_CCQK.rdlc";
            string aspxUrl = "~/Reports/TimeModeWebForm.aspx?ReportName=" + ReportName;
            Response.Redirect(aspxUrl);
        }
        /// <summary>
        /// 调度员工作量统计表
        /// </summary>
        public void TJ_DDYGZL()
        {
            string ReportName = "TJ_DDYGZL.rdlc";
            string aspxUrl = "~/Reports/TimeModeWebForm.aspx?ReportName=" + ReportName;
            Response.Redirect(aspxUrl);
        }
        /// <summary>
        /// 调度员工作量统计表（二）
        /// </summary>
        public void TJ_DDYGZL2()
        {
            string ReportName = "TJ_DDYGZL2.rdlc";
            string aspxUrl = "~/Reports/TimeModeWebForm.aspx?ReportName=" + ReportName;
            Response.Redirect(aspxUrl);
        }
        /// <summary>
        /// 调度员工作效率统计表
        /// </summary>
        public void TJ_DDYGZXL()
        {
            string ReportName = "TJ_DDYGZXL.rdlc";
            string aspxUrl = "~/Reports/TimeModeWebForm.aspx?ReportName=" + ReportName;
            Response.Redirect(aspxUrl);
        }
        /// <summary>
        /// 对讲机统计表
        /// </summary>
        public void TJ_DJJ()
        {
            string ReportName = "TJ_DJJ.rdlc";
            string aspxUrl = "~/Reports/TimeModeWebForm.aspx?ReportName=" + ReportName;
            Response.Redirect(aspxUrl);
        }
        /// <summary>
        /// 放车情况明细统计表（二）
        /// </summary>
        public void TJ_FCQKMX2()
        {
            string ReportName = "TJ_FCQKMX2.rdlc";
            string aspxUrl = "~/Reports/TimeModeWebForm.aspx?ReportName=" + ReportName;
            Response.Redirect(aspxUrl);
        }
        /// <summary>
        /// 放车情况明细统计表（一）
        /// </summary>
        public void TJ_FCQKMX1()
        {
            string ReportName = "TJ_FCQKMX1.rdlc";
            string aspxUrl = "~/Reports/TimeModeWebForm.aspx?ReportName=" + ReportName;
            Response.Redirect(aspxUrl);
        }
        /// <summary>
        /// 放车与区域时间关系统计表
        /// </summary>
        public void TJ_FCYQYSJGX()
        {
            string ReportName = "TJ_FCYQYSJGX.rdlc";
            string aspxUrl = "~/Reports/TimeModeWebForm.aspx?ReportName=" + ReportName;
            Response.Redirect(aspxUrl);
        }
        /// <summary>
        /// 分中心上下班问题统计表
        /// </summary>
        public void TJ_FZXBGFSXB()
        {
            string ReportName = "TJ_FZXBGFSXB.rdlc";
            string aspxUrl = "~/Reports/TimeModeWebForm.aspx?ReportName=" + ReportName;
            Response.Redirect(aspxUrl);
        }
        /// <summary>
        /// 分中心暂停调用统计表
        /// </summary>
        public void TJ_FZXZTDY()
        {
            string ReportName = "TJ_FZXZTDY.rdlc";
            string aspxUrl = "~/Reports/TimeModeWebForm.aspx?ReportName=" + ReportName;
            Response.Redirect(aspxUrl);
        }
        /// <summary>
        /// 呼救病种情况统计表
        /// </summary>
        public void TJ_HJBZQK()
        {
            string ReportName = "TJ_HJBZQK.rdlc";
            string aspxUrl = "~/Reports/TimeModeWebForm.aspx?ReportName=" + ReportName;
            Response.Redirect(aspxUrl);
        }
        /// <summary>
        /// 呼救电话排队峰值统计表
        /// </summary>
        public void TJ_HJDHPDFZ()
        {
            string ReportName = "TJ_HJDHPDFZ.rdlc";
            string aspxUrl = "~/Reports/TimeModeWebForm.aspx?ReportName=" + ReportName;
            Response.Redirect(aspxUrl);
        }
        /// <summary>
        /// 回车与区域时间关系统计表
        /// </summary>
        public void TJ_HCYQYSJGX()
        {
            string ReportName = "TJ_HCYQYSJGX.rdlc";
            string aspxUrl = "~/Reports/TimeModeWebForm.aspx?ReportName=" + ReportName;
            Response.Redirect(aspxUrl);
        }
        /// <summary>
        /// 急救反应时间(日报)统计表
        /// </summary>
        public void TJ_JJFYSJRB()
        {
            string ReportName = "TJ_JJFYSJRB.rdlc";
            string aspxUrl = "~/Reports/TimeModeWebForm.aspx?ReportName=" + ReportName;
            Response.Redirect(aspxUrl);
        }
        /// <summary>
        /// 疾病与时间关系统计表
        /// </summary>
        public void TJ_JBYSJGX()
        {
            string ReportName = "TJ_JBYSJGX.rdlc";
            string aspxUrl = "~/Reports/TimeModeWebForm.aspx?ReportName=" + ReportName;
            Response.Redirect(aspxUrl);
        }
        /// <summary>
        /// 人均日出车次数分布统计表
        /// </summary>
        public void TJ_RJRCCCSFB()
        {
            string ReportName = "TJ_RJRCCCSFB.rdlc";
            string aspxUrl = "~/Reports/TimeModeWebForm.aspx?ReportName=" + ReportName;
            Response.Redirect(aspxUrl);
        }
        /// <summary>
        /// 送往地点类型统计表
        /// </summary>
        public void TJ_SWDDLX()
        {
            string ReportName = "TJ_SWDDLX.rdlc";
            string aspxUrl = "~/Reports/TimeModeWebForm.aspx?ReportName=" + ReportName;
            Response.Redirect(aspxUrl);
        }
        /// <summary>
        /// 送往地点统计表
        /// </summary>
        public void TJ_SWDD()
        {
            string ReportName = "TJ_SWDD.rdlc";
            string aspxUrl = "~/Reports/TimeModeWebForm.aspx?ReportName=" + ReportName;
            Response.Redirect(aspxUrl);
        }
        /// <summary>
        /// 危重病人病情预报登记统计表
        /// </summary>
        public void TJ_WZBRBQYBDJ()
        {
            string ReportName = "TJ_WZBRBQYBDJ.rdlc";
            string aspxUrl = "~/Reports/TimeModeWebForm.aspx?ReportName=" + ReportName;
            Response.Redirect(aspxUrl);
        }
        /// <summary>
        /// 未及时放车时间分析表
        /// </summary>
        public void TJ_WJSFCSJFX()
        {
            string ReportName = "TJ_WJSFCSJFX.rdlc";
            string aspxUrl = "~/Reports/TimeModeWebForm.aspx?ReportName=" + ReportName;
            Response.Redirect(aspxUrl);
        }
        /// <summary>
        /// 现场地点统计表
        /// </summary>
        public void TJ_XCDD()
        {
            string ReportName = "TJ_XCDD.rdlc";
            string aspxUrl = "~/Reports/TimeModeWebForm.aspx?ReportName=" + ReportName;
            Response.Redirect(aspxUrl);
        }
        /// <summary>
        /// 转分中心次数统计表
        /// </summary>
        public void TJ_ZFZXCS()
        {
            string ReportName = "TJ_ZFZXCS.rdlc";
            string aspxUrl = "~/Reports/TimeModeWebForm.aspx?ReportName=" + ReportName;
            Response.Redirect(aspxUrl);
        }
        /// <summary>
        /// 转院数据统计表
        /// </summary>
        public void TJ_ZYSJ()
        {
            string ReportName = "TJ_ZYSJ.rdlc";
            string aspxUrl = "~/Reports/TimeModeWebForm.aspx?ReportName=" + ReportName;
            Response.Redirect(aspxUrl);
        }
        /// <summary>
        /// 反应时间统计表
        /// </summary>
        public void TJ_FYSJ()
        {          
            string aspxUrl = "~/Reports/TJ_FYSJModeWebForm.aspx";
            Response.Redirect(aspxUrl);
        }
        /// <summary>
        /// 呼救事件来源统计表
        /// </summary>
        public void TJ_HJSJLY()
        {
            string aspxUrl = "~/Reports/TJ_HJSJLYWebForm.aspx";
            Response.Redirect(aspxUrl);
        }
        /// <summary>
        /// 急救人员工作效率统计表
        /// </summary>
        public void TJ_JJRYGZXL()
        {
            string aspxUrl = "~/Reports/TJ_JJRYGZXLModeWebForm.aspx";
            Response.Redirect(aspxUrl);
        }
        /// <summary>
        /// 来电记录统计表
        /// </summary>
        public void TJ_LDJL()
        {
            string aspxUrl = "~/Reports/TJ_LDJLModeWebForm.aspx";
            Response.Redirect(aspxUrl);
        }
        /// <summary>
        /// 排队电话每日峰值统计表
        /// </summary>
        public void TJ_PDDHMRFZ()
        {
            string aspxUrl = "~/Reports/TJ_PDDHMRFZModeWebForm.aspx";
            Response.Redirect(aspxUrl);
        }
        /// <summary>
        /// 受理情况峰值统计表
        /// </summary>
        public void TJ_SLQKFZ()
        {
            string aspxUrl = "~/Reports/TJ_SLQKFZModeWebForm.aspx";
            Response.Redirect(aspxUrl);
        }
        /// <summary>
        /// 司机出车大于5分钟统计表
        /// </summary>
        public void TJ_SJCCDY5FZ()
        {
            string ReportName = "TJ_SJCCDY5FZ.rdlc";
            string aspxUrl = "~/Reports/TJ_SJCCDY5FZModeWebForm.aspx?ReportName=" + ReportName;
            Response.Redirect(aspxUrl);
        }
        /// <summary>
        /// 司机暂停调用统计表
        /// </summary>
        public void TJ_SJZTDY()
        {
            string aspxUrl = "~/Reports/TJ_SJZTDYModeWebForm.aspx";
            Response.Redirect(aspxUrl);
        }
        /// <summary>
        /// 业务数据分段统计表
        /// </summary>
        public void TJ_YWSJFD()
        {
            string ReportName = "TJ_YWSJFD.rdlc";
            string aspxUrl = "~/Reports/TJ_YWSJFDModeWebForm.aspx?ReportName=" + ReportName;
            Response.Redirect(aspxUrl);
        }
        /// <summary>
        /// 未接来电统计表
        /// </summary>
        public void TJ_WJLD()
        {
            string aspxUrl = "~/Reports/TJ_WJLDModeWebForm.aspx";
            Response.Redirect(aspxUrl);
        }
        /// <summary>
        /// 调度员工作状态统计表
        /// </summary>
        public void TJ_DDYGZZT()
        {
            string ReportName = "TJ_DDYGZZT.rdlc";
            string aspxUrl = "~/Reports/LS_DDYGZZTModeWebForm.aspx?ReportName=" + ReportName;
            Response.Redirect(aspxUrl);
        }
        /// <summary>
        /// 车辆出动方式统计表
        /// </summary>
        public void TJ_CLCDFS()
        {
            string aspxUrl = "~/Reports/TJ_CLCDFSModeWebForm.aspx";
            Response.Redirect(aspxUrl);
        }
        /// <summary>
        /// 病情分类统计表
        /// </summary>
        public void TJ_BQFL()
        {
            string ReportName = "TJ_BQFL.rdlc";
            string aspxUrl = "~/Reports/TJ_JZCSModeWebForm.aspx?ReportName=" + ReportName;
            Response.Redirect(aspxUrl);
        }
        /// <summary>
        /// 急救效果统计表
        /// </summary>
        public void TJ_JJXG()
        {
            string ReportName = "TJ_JJXG.rdlc";
            string aspxUrl = "~/Reports/TJ_JZCSModeWebForm.aspx?ReportName=" + ReportName;
            Response.Redirect(aspxUrl);
        }
        /// <summary>
        /// 病情预报统计表
        /// </summary>
        public void TJ_BQYB()
        {
            string ReportName = "TJ_BQYB.rdlc";
            string aspxUrl = "~/Reports/TJ_JZCSModeWebForm.aspx?ReportName=" + ReportName;
            Response.Redirect(aspxUrl);
        }
        /// <summary>
        /// 患者死亡统计表
        /// </summary>
        public void TJ_HZSW()
        {
            string ReportName = "TJ_HZSW.rdlc";
            string aspxUrl = "~/Reports/TJ_JZCSModeWebForm.aspx?ReportName=" + ReportName;
            Response.Redirect(aspxUrl);
        }
        /// <summary>
        /// 性别统计表
        /// </summary>
        public void TJ_XB()
        {
            string ReportName = "TJ_XB.rdlc";
            string aspxUrl = "~/Reports/TJ_JZCSModeWebForm.aspx?ReportName=" + ReportName;
            Response.Redirect(aspxUrl);
        }
        /// <summary>
        /// 年龄段统计表
        /// </summary>
        public void TJ_NLD()
        {
            string ReportName = "TJ_NLD.rdlc";
            string aspxUrl = "~/Reports/TJ_JZCSModeWebForm.aspx?ReportName=" + ReportName;
            Response.Redirect(aspxUrl);
        }
        /// <summary>
        /// 事件类型统计表
        /// </summary>
        public void TJ_SJLX()
        {
            string ReportName = "TJ_SJLX.rdlc";
            string aspxUrl = "~/Reports/TJ_JZCSModeWebForm.aspx?ReportName=" + ReportName;
            Response.Redirect(aspxUrl);
        }
        /// <summary>
        /// 任务类型统计表
        /// </summary>
        public void TJ_RWLX()
        {
            string ReportName = "TJ_RWLX.rdlc";
            string aspxUrl = "~/Reports/TJ_JZCSModeWebForm.aspx?ReportName=" + ReportName;
            Response.Redirect(aspxUrl);
        }
        /// <summary>
        /// 病种分类统计表
        /// </summary>
        public void TJ_BZFL()
        {
            string ReportName = "TJ_BZFL.rdlc";
            string aspxUrl = "~/Reports/TJ_JZCSModeWebForm.aspx?ReportName=" + ReportName;
            Response.Redirect(aspxUrl);
        }
        /// <summary>
        /// 救治措施统计表
        /// </summary>
        public void TJ_JZCS()
        {
            string ReportName = "TJ_JZCS.rdlc";
            string aspxUrl = "~/Reports/TJ_JZCSModeWebForm.aspx?ReportName=" + ReportName;
            Response.Redirect(aspxUrl);
        }
        /// 急救评分统计表
        /// </summary>
        public void TJ_JJPF()
        {
            string ReportName = "TJ_JJPF.rdlc";
            string aspxUrl = "~/Reports/TJ_JZCSModeWebForm.aspx?ReportName=" + ReportName;
            Response.Redirect(aspxUrl);
        }
        /// <summary>
        /// 救治耗材统计表
        /// </summary>
        public void TJ_HC()
        {  string ReportName = "TJ_HC.rdlc";
        string aspxUrl = "~/Reports/TJ_JZCSModeWebForm.aspx?ReportName=" + ReportName;
            Response.Redirect(aspxUrl);
        }
        /// <summary>
        /// 药品用药量统计表
        /// </summary>
        public void TJ_YPYYL()
        {
            string ReportName = "TJ_YPYYL.rdlc";
            string aspxUrl = "~/Reports/TJ_JZCSModeWebForm.aspx?ReportName=" + ReportName;
            Response.Redirect(aspxUrl);
        }
        /// <summary>
        /// 器械检查统计表
        /// </summary>
        public void TJ_QXJC()
        {
            string ReportName = "TJ_QXJC.rdlc";
            string aspxUrl = "~/Reports/TJ_JZCSModeWebForm.aspx?ReportName=" + ReportName;
            Response.Redirect(aspxUrl);
        }
        //出车数,病历数,重大事件数统计表
        public void TJ_BLZDSJ()
        {
            string ReportName = "TJ_BLZDSJ.rdlc";
            string aspxUrl = "~/Reports/TJ_JZCSModeWebForm.aspx?ReportName=" + ReportName;
            Response.Redirect(aspxUrl);
        }
        //拒绝治疗、体检、器械检查、提供病史统计表
        public void TJ_JJZLJC()
        {
            string ReportName = "TJ_JJZLJC.rdlc";
            string aspxUrl = "~/Reports/TJ_JZCSModeWebForm.aspx?ReportName=" + ReportName;
            Response.Redirect(aspxUrl);
        }
        //呼救医院统计表
        public void TJ_HJYY()
        {
            string ReportName = "TJ_HJYY.rdlc";
            string aspxUrl = "~/Reports/TJ_JZCSModeWebForm.aspx?ReportName=" + ReportName;
            Response.Redirect(aspxUrl);
        }
        //送达医院统计表
        public void TJ_SDYY()
        {
            string ReportName = "TJ_SDYY.rdlc";
            string aspxUrl = "~/Reports/TJ_JZCSModeWebForm.aspx?ReportName=" + ReportName;
            Response.Redirect(aspxUrl);
        }
        //回访统计表
        public void TJ_HF()
        {
            string ReportName = "TJ_HF.rdlc";
            string aspxUrl = "~/Reports/TJ_JZCSModeWebForm.aspx?ReportName=" + ReportName;
            Response.Redirect(aspxUrl);
        }
        //财务流水表
        public void TJ_BalanceDetial(string ReportTime, string MaterialTypeID)
        {

            string aspxUrl = "~/Reports/IMBalanceDetialModeWebForm.aspx?ReportTime=" + ReportTime + "&MaterialType=" + MaterialTypeID;
            Response.Redirect(aspxUrl);
        }
        //财务汇总表
        public void TJ_BalanceTotal()
        {

            string aspxUrl = "~/Reports/IMBalanceTotalModeWebForm.aspx";
            Response.Redirect(aspxUrl);
        }
        public void TJ_BalancePersonal()
        {

            string aspxUrl = "~/Reports/IMBalancePersonalModeWebForm.aspx";
            Response.Redirect(aspxUrl);
        }
    }
}
