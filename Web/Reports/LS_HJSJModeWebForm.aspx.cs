using Anke.SHManage.BLL;
using Anke.SHManage.MSSQLDAL;
using Anke.SHManage.Utility;
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
    public partial class WebForm1 : System.Web.UI.Page
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
            this.alarmType.Text = "";
            //this.HiddenForambulanceState.Value = "";          
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
            string alarmType = Convert.ToString(this.alarmType.Text);
            //string ambulanceState = Convert.ToString(this.HiddenForambulanceState.Value);
            string ambulanceState = Convert.ToString(this.HiddenForambulanceState.Value);
            string callNumber = Convert.ToString(this.callNumber.Text);
            string ambulance = Convert.ToString(this.ambulance.Text);
            string driver = Convert.ToString(this.driver.Text);
            string doctor = Convert.ToString(this.doctor.Text);
            string sceneAddress = Convert.ToString(this.sceneAddress.Text);
            string sendAddress = Convert.ToString(this.sendAddress.Text);
            string name = Convert.ToString(this.name.Text);
            string illReason = Convert.ToString(this.illReason.Text);
           

            string start1 = start.ToString();
            string end1 = end.ToString();

            this.ReportViewer1.LocalReport.DataSources.Clear();
            this.ReportViewer1.LocalReport.ReportPath = Server.MapPath(@"~/Reports/LS_HJSJ.rdlc");
            Microsoft.Reporting.WebForms.ReportParameter st = new Microsoft.Reporting.WebForms.ReportParameter("StartTime", start1);
            Microsoft.Reporting.WebForms.ReportParameter ed = new Microsoft.Reporting.WebForms.ReportParameter("EndTime", end1);
            this.ReportViewer1.LocalReport.SetParameters(new Microsoft.Reporting.WebForms.ReportParameter[] { st, ed });
            LSDAL dal = new LSDAL();
            DataTable dt = dal.Get_LS_HJSJ(start, end, alarmType, ambulanceState, callNumber,
            name, ambulance, driver, doctor, sceneAddress, sendAddress, illReason);
            this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForLS_HJSJ", dt));

            this.ReportViewer1.LocalReport.Refresh();
        }
    }
}