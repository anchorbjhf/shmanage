using Anke.SHManage.Model;
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
    public partial class IMBalanceTotalModeWebForm : System.Web.UI.Page
    {

        string type = "MaterialType-15','MaterialType-16','MaterialType-17','MaterialType-18','MaterialType-19','MaterialType-20','MaterialType-22','MaterialType-23','MaterialType-24','MaterialType-25','MaterialType-21";
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                this.TextMonth.Text = DateTime.Now.Year + DateTime.Now.AddMonths(-1).ToString("MM");
                Data_Binding(type);
            }
        }
        private void Data_Binding(string type)
        {

            string month = this.TextMonth.Text;

            string newtype = type;

            this.ReportViewer1.LocalReport.DataSources.Clear();
            this.ReportViewer1.LocalReport.ReportPath = Server.MapPath(@"~/Reports/TJ_IMBalanceTotal.rdlc");
            TJDAL dal = new TJDAL();
            DataTable dt = dal.Get_BalanceTotal(month, newtype);
            this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForTJ_BalanceTotal", dt));
            this.ReportViewer1.LocalReport.Refresh();
        }

        protected void btnDrug_Click(object sender, EventArgs e)
        {
            //TJDAL dal = new TJDAL();
            // List<CheckModelExt> list = dal.getlistbypid();
            //将以下取string 写活。
            TJDAL dal = new TJDAL();
            List<CheckModelExt> lm = dal.GetTDictionaryIDByParentID("MaterialType-12");

            string ret = "";
            for (int i = 0; i < lm.Count; i++)
            {
                ret += lm[i].ID.ToString() + "','";
            }
            ret = ret.TrimEnd(',');

            type = ret;
            Data_Binding(type);
        }

        protected void btnCar_Click(object sender, EventArgs e)
        {
            TJDAL dal = new TJDAL();
            List<CheckModelExt> lm = dal.GetTDictionaryIDByParentID("MaterialType-9999");

            string ret = "";
            for (int i = 0; i < lm.Count; i++)
            {
                ret += lm[i].ID.ToString() + "','";
            }
            ret = ret.TrimEnd(',');

            type = ret;
            Data_Binding(type);
        }
    }
}