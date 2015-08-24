<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TimeModeWebForm.aspx.cs" Inherits="Anke.SHManage.Web.Reports.TimeModeWebForm" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title><%=Title%></title>
    <script src="../Content/EasyUI/jquery.js"></script>
    <script src="../Content/EasyUI/jquery.easyui.js"></script>
    <script src="../Content/EasyUI/locale/easyui-lang-zh_CN.js"></script>
    <link href="../Content/EasyUI/themes/icon.css" rel="stylesheet" />
    <link href="../Content/EasyUI/themes/default/easyui.css" rel="stylesheet" />
    <script src="../Scripts/jquery.AnkeExtend.js"></script>

<%--    <script type="text/javascript">
        //页面加载
        $(document).ready(function () {
            $.InitDateTime();
        });
    </script>--%>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
            <table>
                <tr>
                    <td>查询时间：</td>
                    <td>
                        <asp:TextBox ID="StartDate" class="easyui-datetimebox" style="width:150px" runat="server"></asp:TextBox>
                        <%--<input id="StartDate" class="easyui-datetimebox" style="width:150px" data-options="required:true" />--%>
                    </td>
                    <td>至</td>
                    <td>
                        <asp:TextBox ID="EndDate" class="easyui-datetimebox" style="width:150px" runat="server"></asp:TextBox>
                        <%--<input id="EndDate" class="easyui-datetimebox" style="width:150px" data-options="required:true" />--%>
                    </td>
                    <td>
                        &nbsp;<asp:Button ID="btnSearch" runat="server" Text="查询" OnClick="btnSearch_Click" />
                        &nbsp;
                        <%--<a href="javascript:void(0)" id="btnsearch" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="bindGrid()">查询</a>--%>
                    </td>
                </tr>
            </table>
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" Height="100%" Width="100%"></rsweb:ReportViewer>
    </form>
</body>
</html>
