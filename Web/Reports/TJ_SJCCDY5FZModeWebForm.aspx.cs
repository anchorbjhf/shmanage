
using Anke.SHManage.MSSQLDAL;
using Anke.SHManage.MSSQLDAL.TJDAL;
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
    public partial class TJ_SJCCDY5FZModeWebForm : System.Web.UI.Page
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
            this.Time.Text = "5";

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
            string center = (this.Center.Text).ToString();
            string station = (this.Station.Text).ToString();           
            string name = Convert.ToString(this.Name.Text);
            string workCode= Convert.ToString(this.WorkCode.Text);
            string carNumber = (this.CarNumber.Text).ToString();
            int time = Convert.ToInt32(this.Time.Text);
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
                 case "LS_SJCCQK.rdlc"://司机出车大于5分钟流水表                   
                     DataTable dt = dal.Get_LS_SJCCQK(start, end,center,station,name,workCode,carNumber,time);
                     this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForLS_SJCCQK", dt));
                     break;
                 case "TJ_SJCCDY5FZ.rdlc"://司机出车大于5分钟统计表                   
                     DataTable dt1 = dalt.Get_SJCCDY5FZ(start, end,center,station,name,workCode,carNumber,time);
                     this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForTJ_SJCCDY5FZ", dt1));
                     break;
            default:
                     break;
             }
             this.ReportViewer1.LocalReport.Refresh();
         
        }      
    }
}