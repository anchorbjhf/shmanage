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
    public partial class LS_BLModeWebForm : System.Web.UI.Page
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
            this.StartDate.Text = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
            this.EndDate.Text = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
            BindReportDataSource();
        }
        protected void BindReportDataSource()
        {
            DateTime beginTime = Convert.ToDateTime(this.StartDate.Text);
            DateTime endTime = Convert.ToDateTime(this.EndDate.Text);
            string eventType = Convert.ToString(this.EventType.Text);
            string name = Convert.ToString(this.Name.Text);
            string centerID = Convert.ToString(this.Center.Text);
            string stationID = (this.Station.Text).ToString();
            string diseasesClassification = (this.DiseasesClassification.Text).ToString();
            string doctorAndNurse = Convert.ToString(this.DoctorAndNurse.Text);
            string driver = Convert.ToString(this.Driver.Text);
            string stretcher = Convert.ToString(this.Stretcher.Text);
            string alarmReason = Convert.ToString(this.AlarmReason.Text);
            string illnessClassification = Convert.ToString(this.IllnessClassification.Text);
            string illnessForecast = Convert.ToString(this.IllnessForecast.Text);
            string firstAidEffect = Convert.ToString(this.FirstAidEffect.Text);
            string diseaseCooperation = Convert.ToString(this.DiseaseCooperation.Text);
            string firstImpression = Convert.ToString(this.FirstImpression.Text);
            string deathCase = Convert.ToString(this.DeathCase.Text);
            string deathCertificate = Convert.ToString(this.DeathCertificate.Text);
            string treatmentMeasure = Convert.ToString(this.TreatmentMeasure.Text);
            string start1 = beginTime.ToString();
            string end1 = endTime.ToString();

            this.ReportViewer1.LocalReport.DataSources.Clear();
            this.ReportViewer1.LocalReport.ReportPath = Server.MapPath(@"~/Reports/LS_BL.rdlc");
            Microsoft.Reporting.WebForms.ReportParameter st = new Microsoft.Reporting.WebForms.ReportParameter("startTime", start1);
            Microsoft.Reporting.WebForms.ReportParameter ed = new Microsoft.Reporting.WebForms.ReportParameter("endTime", end1);
            this.ReportViewer1.LocalReport.SetParameters(new Microsoft.Reporting.WebForms.ReportParameter[] { st, ed });
            LSDAL dal = new LSDAL();
            DataTable dt = dal.Get_LS_BL(beginTime, endTime, eventType, name, centerID, stationID, diseasesClassification, doctorAndNurse, driver, stretcher, alarmReason, 
                                         illnessClassification, illnessForecast, firstAidEffect, diseaseCooperation, firstImpression, deathCase, deathCertificate, treatmentMeasure);
            this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForLS_BL", dt));

            this.ReportViewer1.LocalReport.Refresh();
        }
    }
}