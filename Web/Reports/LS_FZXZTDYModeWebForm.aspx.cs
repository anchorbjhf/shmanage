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
    public partial class LS_FZXZTDYModeWebForm : System.Web.UI.Page
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
            DateTime beginTime = Convert.ToDateTime(this.StartDate.Text);
            DateTime endTime = Convert.ToDateTime(this.EndDate.Text);
            string center = (this.BelongCenter.Text).ToString();
            string station = Convert.ToString(this.BelongSation.Text);
            string carNumber = Convert.ToString(this.CarNumber.Text);
            string workCode = Convert.ToString(this.WorkNumber.Text);
            string name = Convert.ToString(this.Name.Text);
            E_StatisticsPermisson em = UserOperateContext.Current.getMaxPerForStatistics();
            string selfWorkCode = UserOperateContext.Current.Session_UsrInfo.WorkCode;
            string selfCenterID = UserOperateContext.Current.Session_UsrInfo.P_Department.DispatchSubCenterID;
            string selfStationID = UserOperateContext.Current.Session_UsrInfo.P_Department.DispatchSationID;

            string start1 = beginTime.ToString();
            string end1 = endTime.ToString();


            this.ReportViewer1.LocalReport.DataSources.Clear();
            this.ReportViewer1.LocalReport.ReportPath = Server.MapPath(@"~/Reports/LS_FZXZTDY.rdlc");
            Microsoft.Reporting.WebForms.ReportParameter st = new Microsoft.Reporting.WebForms.ReportParameter("StartTime", start1);
            Microsoft.Reporting.WebForms.ReportParameter ed = new Microsoft.Reporting.WebForms.ReportParameter("EndTime", end1);
            this.ReportViewer1.LocalReport.SetParameters(new Microsoft.Reporting.WebForms.ReportParameter[] { st, ed });
            LSDAL dal = new LSDAL();
            DataTable dt = dal.Get_LS_FZXZTDY(beginTime, endTime, center, station, carNumber, workCode, name, em, selfWorkCode, selfCenterID, selfStationID);
            this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForLS_FZXZTDY", dt));

            this.ReportViewer1.LocalReport.Refresh();
        }
    }
}