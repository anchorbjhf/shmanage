using Anke.SHManage.MSSQLDAL;
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
    public partial class LS_LDJLModeWebForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitPage();

            }
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindReportDataSource();
            this.HiddenForResult.Value = "";            
        }
        private void InitPage()
        {
            this.StartDate.Text = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd 00:00:00");
            this.EndDate.Text = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
            BindReportDataSource();
        }

        protected void BindReportDataSource()
        {

            DateTime start = Convert.ToDateTime(this.StartDate.Text);
            DateTime end = Convert.ToDateTime(this.EndDate.Text);
            string callNumber = (this.callNumber.Text).ToString();
            string actionResult = Convert.ToString(this.HiddenForResult.Value);
            string start1 = start.ToString();
            string end1 = end.ToString();
            this.ReportViewer1.LocalReport.DataSources.Clear();
            this.ReportViewer1.LocalReport.ReportPath = Server.MapPath(@"~/Reports/LS_LDJL.rdlc");
            Microsoft.Reporting.WebForms.ReportParameter st = new Microsoft.Reporting.WebForms.ReportParameter("StartTime", start1);
            Microsoft.Reporting.WebForms.ReportParameter ed = new Microsoft.Reporting.WebForms.ReportParameter("EndTime", end1);
            this.ReportViewer1.LocalReport.SetParameters(new Microsoft.Reporting.WebForms.ReportParameter[] { st, ed });
            LSDAL dal = new LSDAL();
            DataTable dt = dal.Get_LS_LDJL(start, end, callNumber, actionResult);
            this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForLS_LDJL", dt));

            this.ReportViewer1.LocalReport.Refresh();
        }

    }
}