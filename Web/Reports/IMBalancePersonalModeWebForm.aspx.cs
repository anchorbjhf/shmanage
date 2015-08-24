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
    public partial class IMBalancePersonalModeWebForm : System.Web.UI.Page
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
             this.textMonth.Text = DateTime.Now.Year + DateTime.Now.AddMonths(-1).ToString("MM");
            BindReportDataSource();
        }

        protected void BindReportDataSource()
        {
            string textMonth = Convert.ToString(this.textMonth.Text);
            //将月份转换成那个月的起始时间和结束时间
            int mm = Convert.ToInt32(textMonth.Substring(4, 2));
            int year = Convert.ToInt32(textMonth.Substring(0, 4));
            int nextMM, nextYear, lastYear, lastMM;
            if (mm == 12)
            {
                nextMM = 1;
                nextYear = year + 1;
            }
            else
            {
                nextMM = mm + 1;
                nextYear = year;
            }
            if (mm == 1)
            {
                lastMM = 12;
                lastYear = year - 1;
            }
            else
            {
                lastMM = mm - 1;
                lastYear = year;
            }
            DateTime beginTime = Convert.ToDateTime(year.ToString() + "-" + mm.ToString() + "-1 00:00:00");
            DateTime endTime = Convert.ToDateTime(nextYear.ToString() + "-" + nextMM.ToString() + "-1 00:00:00");

            string MaterialType = Convert.ToString(this.MaterialType.Text);

            string DeliveryStorage = Convert.ToString(this.DeliveryStorage.Text);
          
            this.ReportViewer1.LocalReport.DataSources.Clear();
            this.ReportViewer1.LocalReport.ReportPath = Server.MapPath(@"~/Reports/TJ_IMBalancePersonal.rdlc");
            TJDAL dal = new TJDAL();
            DataTable dt = dal.Get_BalancePersonal(beginTime, endTime, MaterialType, DeliveryStorage, textMonth);
            this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForTJ_BalancePersonal", dt));

            this.ReportViewer1.LocalReport.Refresh();
        }

    }
}