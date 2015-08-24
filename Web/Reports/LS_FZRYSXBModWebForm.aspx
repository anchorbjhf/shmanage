<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LS_FZRYSXBModWebForm.aspx.cs" Inherits="Anke.SHManage.Web.Reports.TJ_FZRYSXBModWebForm" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <script src="../Content/EasyUI/jquery.js"></script>
    <script src="../Content/EasyUI/jquery.easyui.js"></script>
    <script src="../Content/EasyUI/locale/easyui-lang-zh_CN.js"></script>
    <link href="../Content/EasyUI/themes/icon.css" rel="stylesheet" />
    <link href="../Content/EasyUI/themes/default/easyui.css" rel="stylesheet" />
    <script src="../Scripts/jquery.AnkeExtend.js"></script>
    <script type="text/javascript">
        //页面加载
        $(document).ready(function () {
            //alert("你好");
            fillCombobox();
            //$.InitDateTime();
            //$('#StartWorkState').combobox({
            //    multiple : true
            //});
            function fillCombobox() {
                //alert("你好");
                //BelongCenter 所属分中心的下拉框的值
                $.EUIcombobox("#BelongCenter", {
                    url: "../IM/AllDictionary/GetCenterName",
                    valueField: 'Name',
                    textField: 'Name',
                    editable: false,
                    OneOption: [{
                        ID: "",
                        Name: "--请选择--"
                    }],
                    onSelect: function (rec) {
                    var urlstr3 = '@Url.Content("~/IM/AllDictionary/GetMaterial?mType=")' + rec.id;
                    $("#Material").combogrid({
                        prompt: '--请选择--',
                        panelWidth: 320,
                        idField: 'ID',
                        textField: 'Name',
                });
            }
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <table>
            <tr>
                <td>查询时间：</td>
                <td>
                    <asp:TextBox ID="StartDate" class="easyui-datetimebox" Style="width: 150px" runat="server"></asp:TextBox>
                    
                </td>
                <td>至</td>
                <td>
                    <asp:TextBox ID="EndDate" class="easyui-datetimebox" Style="width: 150px" runat="server"></asp:TextBox>
                    
                </td>
                <td>所属中心：</td>
                <td>
                    <asp:TextBox ID="BelongCenter" class="easyui-combobox" Style="width: 150px" runat="server"></asp:TextBox>
                </td>
                <td>所属分站：</td>
                <td>
                    <asp:TextBox ID="BelongSation" class="easyui-combobox" Style="width: 150px" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>职务：</td>
                <td>
                    <asp:TextBox ID="BelongRole" class="easyui-combobox" Style="width: 150px" runat="server"></asp:TextBox>
                </td>
                <td>车号：</td>
                <td>
                    <asp:TextBox ID="CarNumber" class="easyui-textbox" Style="width: 150px" runat="server"></asp:TextBox>
                </td>
                <td>工号：</td>
                <td>
                    <asp:TextBox ID="WorkNumber" class="easyui-textbox" Style="width: 150px" runat="server"></asp:TextBox>
                </td>
                <td>姓名：</td>
                <td>
                    <asp:TextBox ID="Name" class="easyui-textbox" Style="width: 150px" runat="server"></asp:TextBox>
                </td>
                <td>&nbsp;
                   <asp:Button ID="Button1" runat="server" Text="查询" />
                    
                    &nbsp;
                </td>
            </tr>
        </table>
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" Height="100%" Width="100%"></rsweb:ReportViewer>
    </form>
</body>
</html>
