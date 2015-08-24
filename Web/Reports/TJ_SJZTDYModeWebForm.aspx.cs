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
    public partial class TJ_SJZTDYModeWebForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
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
            this.StartDate.Text = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd 00:00:00");
            this.EndDate.Text = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
            BindReportDataSource();
        }

        protected void BindReportDataSource()
        {

            DateTime start = Convert.ToDateTime(this.StartDate.Text);
            DateTime end = Convert.ToDateTime(this.EndDate.Text);
            string center = (this.Center.Text).ToString();
            string station = (this.Station.Text).ToString();
            string name = Convert.ToString(this.Name.Text);
            string workCode = Convert.ToString(this.WorkCode.Text);
            string carNumber = (this.CarNumber.Text).ToString();
            string start1 = start.ToString();
            string end1 = end.ToString();

            this.ReportViewer1.LocalReport.DataSources.Clear();
            this.ReportViewer1.LocalReport.ReportPath = Server.MapPath(@"~/Reports/TJ_SJZTDY.rdlc");
            Microsoft.Reporting.WebForms.ReportParameter st = new Microsoft.Reporting.WebForms.ReportParameter("StartTime", start1);
            Microsoft.Reporting.WebForms.ReportParameter ed = new Microsoft.Reporting.WebForms.ReportParameter("EndTime", end1);
            this.ReportViewer1.LocalReport.SetParameters(new Microsoft.Reporting.WebForms.ReportParameter[] { st, ed });
            TJDAL dal = new TJDAL();
            DataTable dt = dal.Get_SJZTDY(start, end, center, station, name, workCode, carNumber);
            this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForTJ_SJZTDY", dt));
            this.ReportViewer1.LocalReport.Refresh();

        }      
    }
}