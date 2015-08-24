using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Anke.SHManage.Web.Areas.BB.Controllers
{
    public class LSController : Controller
    {
        //
        // GET: /BB/LS/

        //public void TestLS()
        //{
        //    string aspxUrl = "~/Reports/TimeModeWebForm.aspx";
        //    Response.Redirect(aspxUrl);
        //}

        /// <summary>
        /// 重大突发性灾害事故流水表
        /// </summary>
        public void LS_ZDTFXZHSG()
        {          
            string ReportName = "LS_ZDTFXZHSG.rdlc";
            string aspxUrl = "~/Reports/TimeModeWebForm.aspx?ReportName=" + ReportName;
            Response.Redirect(aspxUrl);
        }
        /// <summary>
        /// 调度日志流水表
        /// </summary>
        public void LS_DDRZ()
        {
            string ReportName = "LS_DDRZ.rdlc";
            string aspxUrl = "~/Reports/TimeModeWebForm.aspx?ReportName=" + ReportName;
            Response.Redirect(aspxUrl);
        }
        /// <summary>
        /// 发送通知流水表
        /// </summary>
        public void LS_FSTZ()
        {
            string ReportName = "LS_FSTZ.rdlc";
            string aspxUrl = "~/Reports/TimeModeWebForm.aspx?ReportName=" + ReportName;
            Response.Redirect(aspxUrl);
        }
        /// <summary>
        /// 日常急救到达率分段（时间段）表
        /// </summary>
        public void LS_RCJJDDLFDSJD()
        {
            string ReportName = "LS_RCJJDDLFDSJD.rdlc";
            string aspxUrl = "~/Reports/TimeModeWebForm.aspx?ReportName=" + ReportName;
            Response.Redirect(aspxUrl);
        }
        /// <summary>
        /// 手机定位成功率统计表
        /// </summary>
        public void LS_SJDWCGL()
        {
            string ReportName = "LS_SJDWCGL.rdlc";
            string aspxUrl = "~/Reports/TimeModeWebForm.aspx?ReportName=" + ReportName;
            Response.Redirect(aspxUrl);
        }
        /// <summary>
        /// 危重病人病情预报登记表
        /// </summary>
        public void LS_WZBRBQYBDJ()
        {
            string ReportName = "LS_WZBRBQYBDJ.rdlc";
            string aspxUrl = "~/Reports/TimeModeWebForm.aspx?ReportName=" + ReportName;
            Response.Redirect(aspxUrl);
        }
        /// <summary>
        /// 医院搁置担架统计表
        /// </summary>
        public void LS_YYGZDJ()
        {
            string ReportName = "LS_YYGZDJ.rdlc";
            string aspxUrl = "~/Reports/TimeModeWebForm.aspx?ReportName=" + ReportName;
            Response.Redirect(aspxUrl);
        }
        /// <summary>
        /// 医院搁置救护车担架影响出车统计表
        /// </summary>
        public void LS_YYGZJHCDJYXCC()
        {
            string ReportName = "LS_YYGZJHCDJYXCC.rdlc";
            string aspxUrl = "~/Reports/TimeModeWebForm.aspx?ReportName=" + ReportName;
            Response.Redirect(aspxUrl);
        }
        /// <summary>
        /// 调度员工作状态流水表
        /// </summary>
        public void LS_DDYGZZT()
        {
            string ReportName = "LS_DDYGZZT.rdlc";
            string aspxUrl = "~/Reports/LS_DDYGZZTModeWebForm.aspx?ReportName=" + ReportName;
            Response.Redirect(aspxUrl);
        }
        /// <summary>
        /// 分站人员上下班流水表
        /// </summary>
        public void LS_FZRYSXB()
        {

            string aspxUrl = "~/Reports/LS_FZRYSXBModeWebForm.aspx";
            Response.Redirect(aspxUrl);
        }
        /// <summary>
        /// 病历填写时间流水表
        /// </summary>
        public void LS_BLTXSJ()
        {

            string aspxUrl = "~/Reports/LS_BLTXSJModeWebForm.aspx";
            Response.Redirect(aspxUrl);
        }
        //分中心暂停调用流水表
        public void LS_FZXZTDY()
        {

            string aspxUrl = "~/Reports/LS_FZXZTDYModeWebForm.aspx";
            Response.Redirect(aspxUrl);
        }
        //呼救事件流水表
        public void LS_HJSJ()
        {

            string aspxUrl = "~/Reports/LS_HJSJModeWebForm.aspx";
            Response.Redirect(aspxUrl);
        }
        //急救病历流水表(最新)
        //public void LS_JJBL()
        //{

        //    string aspxUrl = "~/Reports/LS_JJBLModeWebForm.aspx";
        //    Response.Redirect(aspxUrl);
        //}
        //来电记录流水表
        public void LS_LDJL()
        {

            string aspxUrl = "~/Reports/LS_LDJLModeWebForm.aspx";
            Response.Redirect(aspxUrl);
        }
        //日常急救到达率分段统计表（月）
        public void LS_RCJJDDLFD()
        {
            string ReportName = "LS_RCJJDDLFD.rdlc";
            string aspxUrl = "~/Reports/LS_RCJJDDLFDModeWebForm.aspx?ReportName=" + ReportName;
            Response.Redirect(aspxUrl);
        }
        //受理记录流水表
        public void LS_SLJL()
        {

            string aspxUrl = "~/Reports/LS_SLJLModeWebForm.aspx";
            Response.Redirect(aspxUrl);
        }
        //司机出车大于5分钟流水表
        public void LS_SJCCQK()
        {
            string ReportName = "LS_SJCCQK.rdlc";
            string aspxUrl = "~/Reports/TJ_SJCCDY5FZModeWebForm.aspx?ReportName=" + ReportName;
            Response.Redirect(aspxUrl);
        }
        //病历流水表
        public void LS_BL()
        {
            string ReportName = "LS_BL.rdlc";
            string aspxUrl = "~/Reports/LS_BLModeWebForm.aspx?ReportName=" + ReportName;
            Response.Redirect(aspxUrl);
        }
    }
}
