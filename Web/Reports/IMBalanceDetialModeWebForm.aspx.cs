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
    public partial class NothingModeWebForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Data_Binding();
            }
        }
        private void Data_Binding()
        {
            string ReportTime = Server.UrlDecode(Request.QueryString["ReportTime"]);
            string MaterialType = Request.QueryString["MaterialType"];

            this.ReportViewer1.LocalReport.DataSources.Clear();
            this.ReportViewer1.LocalReport.ReportPath = Server.MapPath(@"~/Reports/TJ_IMBalanceDetial.rdlc");
            TJDAL dal = new TJDAL();
            DataTable dt = dal.Get_BalanceDetial(ReportTime, MaterialType);
            this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForTJ_BalanceDetial", dt));
            this.ReportViewer1.LocalReport.Refresh();
        }
    }
}