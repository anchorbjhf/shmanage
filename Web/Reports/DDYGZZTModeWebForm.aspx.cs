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
        protected void Page_Load(object sender, EventArgs e)
        {
            InitPage();
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindReportDataSource();
        }
        private void InitPage()
        {
            this.StartDate.Text = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss");
            this.EndDate.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            BindReportDataSource();
        }

        protected void BindReportDataSource()
        {
            
            DateTime start = Convert.ToDateTime(this.StartDate.Text);
            DateTime end = Convert.ToDateTime(this.EndDate.Text);
            string workState = Convert.ToString(this.StartWorkState.Text);
            string time = Convert.ToString(this.WorkStateTime.Text);
            string name = Convert.ToString(this.DispatcherName.Text);
            string personCode = Convert.ToString(this.WorkNumber.Text);     

            this.ReportViewer1.LocalReport.DataSources.Clear();
            //this.ReportViewer1.LocalReport.ReportPath = Server.MapPath(@"~/Reports/LS_DDYGZZT.rdlc");
            LSDAL dal = new LSDAL();
            DataTable dt = dal.Get_LS_DDYGZZT(start, end, workState, time, name, personCode);
            this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForLS_DDYGZZT", dt));

            this.ReportViewer1.LocalReport.Refresh();
        }

    }
}