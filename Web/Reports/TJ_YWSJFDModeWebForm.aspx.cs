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
    public partial class TJ_YWSJFDModeWebForm : System.Web.UI.Page
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
        }

        private void InitPage()
        {
            this.Year.Text = DateTime.Now.Year.ToString();
            this.Month.Text = DateTime.Now.Month.ToString();
            BindReportDataSource();
        }

        protected void BindReportDataSource()
        {
            string year = (this.Year.Text).ToString();
            string month = (this.Month.Text).ToString();

            this.ReportViewer1.LocalReport.DataSources.Clear();
            this.ReportViewer1.LocalReport.ReportPath = Server.MapPath(@"~/Reports/TJ_YWSJFD.rdlc");
            Microsoft.Reporting.WebForms.ReportParameter st = new Microsoft.Reporting.WebForms.ReportParameter("StartTime", year);
            Microsoft.Reporting.WebForms.ReportParameter ed = new Microsoft.Reporting.WebForms.ReportParameter("EndTime", month);
            this.ReportViewer1.LocalReport.SetParameters(new Microsoft.Reporting.WebForms.ReportParameter[] { st, ed });

            TJDAL dalt = new TJDAL();
            DataTable dt = dalt.Get_YWSJFD(year, month);
            this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForTJ_YWSJFD", dt));

            this.ReportViewer1.LocalReport.Refresh();

        }

    }
}