<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PrintModeWebForm.aspx.cs" Inherits="Anke.SHManage.Web.Reports.PrintModeWebForm" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>打印预览</title>
    <script src="../Scripts/jquery-1.8.2.min.js"></script>
    <script type="text/javascript">
        //function PageInit() {
        //    var ViewHeight = document.documentElement.clientHeight
        //    || document.body.clientHeight
        //    || window.innerHeight;

        //    document.getElementById("ReportViewer1").style.height = ViewHeight - 50;
        //}
        //window.onload = PageInit;
        //$(document).ready(function () {
        //    var height = $(window).height();
        //    document.getElementById("ReportViewer1").Height = height;
        //});
        // //页面动态改动
        //$(window).resize(function () {
        //    var height = $(window).height();
        //    document.getElementById("ReportViewer1").Height = height;
        //});
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" Width="100%" Height="100%"></rsweb:ReportViewer>
    </form>
</body>
</html>
