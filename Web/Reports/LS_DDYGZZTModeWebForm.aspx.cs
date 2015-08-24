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
    public partial class DDYGZZTModeWebForm : System.Web.UI.Page
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
            this.HiddenForStartWorkState.Value = "";
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
            //string workState = (this.HiddenForStartWorkState.Value);
            string workState = (this.HiddenForStartWorkState.Value);
            string time = Convert.ToString(this.WorkStateTime.Text);
            string name = Convert.ToString(this.DispatcherName.Text);
            string personCode = Convert.ToString(this.WorkNumber.Text);
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
                case "LS_DDYGZZT.rdlc"://调度员工作状态流水表                   
                    DataTable dt = dal.Get_LS_DDYGZZT(start, end, workState, time, name, personCode);
                    this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForLS_DDYGZZT", dt));
                    break;
                case "TJ_DDYGZZT.rdlc"://调度员工作状态统计表                   
                    DataTable dt1 = dalt.Get_TJ_DDYGZZT(start, end, workState, name, personCode, time);
                    this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForTJ_DDYGZZT", dt1));
                    break;
                default:
                    break;
            }
            this.ReportViewer1.LocalReport.Refresh();
        }
    }
}