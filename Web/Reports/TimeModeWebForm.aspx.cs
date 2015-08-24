using Anke.SHManage.MSSQLDAL;
using Anke.SHManage.MSSQLDAL.TJDAL;
using Anke.SHManage.Web.Reports.DataSetForPrintTableAdapters;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Anke.SHManage.Web.Reports
{
    public partial class TimeModeWebForm : System.Web.UI.Page
    {      
        private string reportName;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitPage();
            }
            else
            {
                reportName = Convert.ToString(ViewState["reportName"]);
            }
            
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindReportDataSource();
        }

        private void InitPage()
        {         
            this.StartDate.Text = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd 00:00:00");
            this.EndDate.Text = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
            
            if (Request.QueryString["ReportName"] != null)
            {
                reportName = Request.QueryString["ReportName"];
                ViewState["reportName"] = reportName;
            }
            else
            {
                reportName = "LS_ZDTFXZHSG.rdlc";
                ViewState["reportName"] = reportName;
            }
            BindReportDataSource();
        }    

         protected void BindReportDataSource()
         {
             DateTime start = Convert.ToDateTime(this.StartDate.Text);
             DateTime end = Convert.ToDateTime(this.EndDate.Text);
             string start1 = start.ToString();
             string end1 = end.ToString();
             this.ReportViewer1.LocalReport.DataSources.Clear();
             this.ReportViewer1.LocalReport.ReportPath = Server.MapPath(@"~/Reports/" + reportName);
             Microsoft.Reporting.WebForms.ReportParameter st = new Microsoft.Reporting.WebForms.ReportParameter("StartTime", start1);
             Microsoft.Reporting.WebForms.ReportParameter ed = new Microsoft.Reporting.WebForms.ReportParameter("EndTime", end1);
             this.ReportViewer1.LocalReport.SetParameters(new Microsoft.Reporting.WebForms.ReportParameter[] { st, ed });
             LSDAL dal = new LSDAL();
             TJDAL dalt = new TJDAL();
             switch (reportName)  
             {
                 case "LS_ZDTFXZHSG.rdlc"://重大突发性灾害事故流水表 
                     this.Page.Title = "重大突发性灾害事故流水表";
                     DataTable dt = dal.Get_LS_ZDTFXZHSG(start, end);
                     this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForLS_ZDTFXZHSG", dt));
                     break;
                 case "LS_DDRZ.rdlc"://调度日志流水表                   
                     this.Page.Title = "调度日志流水表";
                     DataTable dt1 = dal.Get_LS_DDRZ(start, end);
                     this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForLS_DDRZ", dt1));
                     break;
                 case "LS_FSTZ.rdlc"://发送通知流水表               
                     this.Page.Title = "发送通知流水表";
                     DataTable dt2 = dal.Get_LS_FSTZ(start, end);
                     this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForLS_FSTZ", dt2));
                     break;
                 case "LS_RCJJDDLFDSJD.rdlc"://日常急救到达率分段（时间段）表   
                     this.Page.Title = "日常急救到达率分段（时间段）表";
                     DataTable dt3 = dal.Get_LS_RCJJDDLFDSJD(start, end);
                     this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForLS_RCJJDDLFDSJD", dt3));
                     DataTable dte = dal.Get_LS_RCJJDDLFDSJDSum(start, end);
                     this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForLS_RCJJDDLFDSJDSum", dte));
                     break;
                 case "LS_SJDWCGL.rdlc"://手机定位成功率统计表     
                     this.Page.Title = "手机定位成功率统计表";
                     DataTable dt4 = dal.Get_LS_SJDWCGL(start, end);
                     this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForLS_SJDWCGL", dt4));
                     break;
                 case "LS_WZBRBQYBDJ.rdlc"://危重病人病情预报登记表    
                     this.Page.Title = "危重病人病情预报登记表";
                     DataTable dt5 = dal.Get_LS_WZBRBQYBDJ(start, end);
                     this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForLS_WZBRBQYBDJ", dt5));
                     break;
                 case "LS_YYGZDJ.rdlc"://医院搁置担架统计表      
                     this.Page.Title = "医院搁置担架统计表";
                     DataTable dt6 = dal.Get_LS_YYGZDJ(start, end);
                     this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForLS_YYGZDJ", dt6));
                     break;
                 case "LS_YYGZJHCDJYXCC.rdlc"://医院搁置救护车担架影响出车统计表   
                     this.Page.Title = "医院搁置救护车担架影响出车统计表";
                     DataTable dt7 = dal.Get_LS_YYGZJHCDJYXCC(start, end);
                     this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForLS_YYGZJHCDJYXCC", dt7));
                     break;
                 case "TJ_CCCSFD.rdlc"://出车次数分段统计表        
                     this.Page.Title = "出车次数分段统计表";
                     DataTable dt8 = dalt.Get_TJ_CCCSFD(start, end);
                     this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForTJ_CCCSFD", dt8));
                     break;
                 case "TJ_CCQK.rdlc"://出车情况统计表           
                     this.Page.Title = "出车情况统计表";
                     DataTable dt9 = dalt.Get_TJ_CCQK(start, end);
                     this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForTJ_CCQK", dt9));
                     break;
                 case "TJ_DDYGZL.rdlc"://调度员工作量统计表      
                     this.Page.Title = "调度员工作量统计表";
                     DataTable dt10 = dalt.Get_TJ_DDYGZL(start, end);
                     this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForTJ_DDYGZL", dt10));
                     break;
                 case "TJ_DDYGZL2.rdlc"://调度员工作量统计表（二）    
                     this.Page.Title = "调度员工作量统计表（二）";
                     DataTable dt11 = dalt.Get_TJ_DDYGZL2(start, end);
                     this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForTJ_DDYGZL2", dt11));
                     break;
                 case "TJ_DDYGZXL.rdlc"://调度员工作效率统计表      
                     this.Page.Title = "调度员工作效率统计表";
                     DataTable dt12 = dalt.Get_TJ_DDYGZXL(start, end);
                     this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForTJ_DDYGZXL", dt12));
                     break;
                 case "TJ_DJJ.rdlc"://对讲机统计表               
                     this.Page.Title = "对讲机统计表";
                     DataTable dt13 = dalt.Get_TJ_DJJ(start, end);
                     this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForTJ_DJJ", dt13));
                     break;
                 case "TJ_FCQKMX2.rdlc"://放车情况明细统计表（二）     
                     this.Page.Title = "放车情况明细统计表（二）";
                     DataTable dt14 = dalt.Get_TJ_FCQKMX2(start, end);
                     this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForTJ_FCQKMX2", dt14));
                     break;
                 case "TJ_FCQKMX1.rdlc"://放车情况明细统计表（一）    
                     this.Page.Title = "放车情况明细统计表（一）";
                     DataTable dt15 = dalt.Get_TJ_FCQKMX1(start, end);
                     this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForTJ_FCQKMX1", dt15));
                     DataTable dt16 = dalt.Get_TJ_FCQKMX1BT1(start, end);
                     this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForTJ_FCQKMX1_BING1", dt16));
                     DataTable dt17 = dalt.Get_TJ_FCQKMX1BT2(start, end);
                     this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForTJ_FCQKMX1_BING2", dt17));
                     DataTable dt18 = dalt.Get_TJ_FCQKMX1ZX(start, end);
                     this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForTJ_FCQKMX1_ZHEXIAN1", dt18));
                     break;
                 case "TJ_FCYQYSJGX.rdlc"://放车与区域时间关系统计表        
                     this.Page.Title = "放车与区域时间关系统计表";
                     DataTable dt19 = dalt.Get_TJ_FCYQYSJGX(start, end);
                     this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForTJ_FCYQYSJGX", dt19));
                     break;
                 case "TJ_FZXBGFSXB.rdlc"://分中心上下班问题统计表       
                     this.Page.Title = "分中心上下班问题统计表";
                     DataTable dt20 = dalt.Get_TJ_FZXSXBWT(start, end);
                     this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForTJ_FZXBGFSXB", dt20));
                     break;
                 case "TJ_FZXZTDY.rdlc"://分中心暂停调用统计表         
                     this.Page.Title = "分中心暂停调用统计表";
                     DataTable dt21 = dalt.Get_TJ_FZXZTDY(start, end);
                     this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForTJ_FZXZTDY", dt21));
                     break;
                 case "TJ_HJBZQK.rdlc"://呼病种情况统计表            
                     this.Page.Title = "呼病种情况统计表";
                     DataTable dt22 = dalt.Get_TJ_HJBZQK(start, end);
                     this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForTJ_HJBZQK", dt22));
                     DataTable dt23 = dalt.Get_TJ_HJBZQK_XBYJB(start, end);
                     this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForTJ_HJBZQK_XBYJB", dt23));
                     DataTable dt24 = dalt.Get_TJ_HJBZQK_NLYJB(start, end);
                     this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForTJ_HJBZQK_NLYJB", dt24));
                     break;
                 case "TJ_HJDHPDFZ.rdlc"://呼救电话排队峰值统计表         
                     this.Page.Title = "呼救电话排队峰值统计表";
                     DataTable dt25 = dalt.Get_TJ_HJDHPDFZ(start, end);
                     this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetFor_TJ_HJDHPDFZ", dt25));
                     break;
                 case "TJ_HCYQYSJGX.rdlc"://回车与区域时间关系统计表      
                     this.Page.Title = "回车与区域时间关系统计表";
                     DataTable dt26 = dalt.Get_TJ_HCYQYSJGX(start, end);
                     this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForTJ_HCYQYSJGX", dt26));
                     break;
                 case "TJ_JJFYSJRB.rdlc"://急救反应时间(日报)统计表       
                     this.Page.Title = "急救反应时间(日报)统计表";
                     DataTable dt27 = dalt.Get_TJ_JJFYSJRB(start, end);
                     this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForTJ_JJFYSJRB", dt27));
                     DataTable dt28 = dalt.Get_TJ_JJFYSJRB1(start, end);
                     this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForTJ_JJFYSJRB1", dt28));
                     DataTable dt29 = dalt.Get_TJ_JJFYSJRB2(start, end);
                     this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForTJ_JJFYSJRB2", dt29));
                     DataTable dt30 = dalt.Get_TJ_JJFYSJRB_CenterTime(start, end);
                     this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForTJ_JJFYSJRB_CenterTime", dt30));
                     break;
                 case "TJ_JBYSJGX.rdlc"://疾病与时间关系统计表          
                     this.Page.Title = "疾病与时间关系统计表";
                     DataTable dt32 = dalt.Get_TJ_JBYSJGX(start, end);
                     this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForTJ_JBYSJGX", dt32));
                     break;
                 case "TJ_RJRCCCSFB.rdlc"://人均日出车次数分布统计表      
                     this.Page.Title = "人均日出车次数分布统计表";
                     DataTable dt33 = dalt.Get_TJ_RJRCCCSFB(start, end);
                     this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForTJ_RJRCCCSFB", dt33));
                     break;
                 case "TJ_SWDDLX.rdlc"://送往地点类型统计表          
                     this.Page.Title = "送往地点类型统计表";
                     DataTable dt34 = dalt.Get_TJ_SWDDLX(start, end);
                     this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForTJ_SWDDLX", dt34));
                     break;
                 case "TJ_SWDD.rdlc"://送往地点统计表          
                     this.Page.Title = "送往地点统计表";
                     DataTable dt35 = dalt.Get_TJ_SWDD(start, end);
                     this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForTJ_SWDD", dt35));
                     break;
                 case "TJ_WZBRBQYBDJ.rdlc"://危重病人病情预报登记统计表        
                     this.Page.Title = "危重病人病情预报登记统计表";
                     DataTable dt36 = dalt.Get_TJ_WZBRBQYBDJ(start, end);
                     this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForTJ_WZBRBQYBDJ", dt36));
                     break;
                 case "TJ_WJSFCSJFX.rdlc"://未及时放车时间分析表       
                     this.Page.Title = "未及时放车时间分析表";
                     DataTable dt37 = dalt.Get_TJ_WJSFCSJFX(start, end);
                     this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForTJ_WJSFCSJFX", dt37));
                     break;
                 case "TJ_XCDD.rdlc"://现场地点统计表         
                     this.Page.Title = "现场地点统计表";
                     DataTable dt38 = dalt.Get_TJ_XCDD(start, end);
                     this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForTJ_XCDD", dt38));
                     break;
                 case "TJ_ZFZXCS.rdlc"://转分中心次数统计表         
                     this.Page.Title = "转分中心次数统计表";
                     DataTable dt39 = dalt.Get_TJ_ZFZXCS(start, end);
                     this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForTJ_ZFZXCS", dt39));
                     break;
                 case "TJ_ZYSJ.rdlc"://转院数据统计表        
                     this.Page.Title = "转院数据统计表";
                     DataTable dt40 = dalt.Get_TJ_ZYSJ(start, end);
                     this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForTJ_ZYSJ", dt40));
                     break;               
                 default:
                     break;                 
             }
             this.ReportViewer1.LocalReport.Refresh();
         }
      
    }
}