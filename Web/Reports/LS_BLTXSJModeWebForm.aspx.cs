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
    public partial class LS_BLTXSJModeWebForm : System.Web.UI.Page
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
            this.StartDate.Text = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd 00:00:00");
            this.EndDate.Text = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
            BindReportDataSource();
        }

        protected void BindReportDataSource()
        {

            DateTime start = Convert.ToDateTime(this.StartDate.Text);
            DateTime end = Convert.ToDateTime(this.EndDate.Text);         
            string workCode = Convert.ToString(this.WorkCode.Text);
            string centerID = Convert.ToString(this.Center.Text);
            string stationID = Convert.ToString(this.Station.Text);
            E_StatisticsPermisson em = UserOperateContext.Current.getMaxPerForStatistics();
            string selfWorkCode = UserOperateContext.Current.Session_UsrInfo.WorkCode;
            string selfCenterID = UserOperateContext.Current.Session_UsrInfo.P_Department.DispatchSubCenterID;
            string selfStationID = UserOperateContext.Current.Session_UsrInfo.P_Department.DispatchSationID;
            string start1 = start.ToString();
            string end1 = end.ToString();

            this.ReportViewer1.LocalReport.DataSources.Clear();
            this.ReportViewer1.LocalReport.ReportPath = Server.MapPath(@"~/Reports/LS_BLTXSJ.rdlc");
            Microsoft.Reporting.WebForms.ReportParameter st = new Microsoft.Reporting.WebForms.ReportParameter("StartTime", start1);
            Microsoft.Reporting.WebForms.ReportParameter ed = new Microsoft.Reporting.WebForms.ReportParameter("EndTime", end1);
            this.ReportViewer1.LocalReport.SetParameters(new Microsoft.Reporting.WebForms.ReportParameter[] { st, ed });
            LSDAL dal = new LSDAL();
            DataTable dt = dal.Get_LS_BLTXSJ(start, end, workCode,centerID,stationID, em,selfWorkCode,selfCenterID,selfStationID);
            this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForLS_BLTXSJ", dt));

            this.ReportViewer1.LocalReport.Refresh();
        }
       
    }
}