<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LS_SLJLModeWebForm.aspx.cs" Inherits="Anke.SHManage.Web.Reports.LS_SLJLModeWebForm" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>受理记录流水表</title>
    <script src="../Content/EasyUI/jquery.js"></script>
    <script src="../Content/EasyUI/jquery.easyui.js"></script>
    <script src="../Content/EasyUI/locale/easyui-lang-zh_CN.js"></script>
    <link href="../Content/EasyUI/themes/icon.css" rel="stylesheet" />
    <link href="../Content/EasyUI/themes/default/easyui.css" rel="stylesheet" />
    <script src="../Scripts/jquery.AnkeExtend.js"></script>
    <script type="text/javascript">
        //页面加载
        $(document).ready(function () {
            fillCombobox();

        });
        function fillCombobox() {

            //取受理类型下拉框的值
            $('#AcceptType').combobox({
                url: "../BB/AllDictionary/GetAcceptType",
                valueField: 'ID',
                textField: 'Name',
                editable: false,
                multiple: true,
                onSelect: function () {
                    $('#HiddenForAcceptType').val($('#AcceptType').combobox('getValues'));
                },
                onUnselect: function () {
                    $('#HiddenForAcceptType').val($('#AcceptType').combobox('getValues'));
                }
            })
            $('#AcceptType').combobox('clear');


            //取电话类型下拉框的值
            $('#CallType').combobox({
                url: "../BB/AllDictionary/GetCallType",
                valueField: 'ID',
                textField: 'Name',
                editable: false,
                multiple: true,
                onSelect: function () {
                    $('#HiddenForCallType').val($('#CallType').combobox('getValues'));
                },
                onUnselect: function () {
                    $('#HiddenForCallType').val($('#CallType').combobox('getValues'));
                }
            });
            $('#CallType').combobox('clear');

        };


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
                <td>受理类型：</td>
                <td>
                    <asp:HiddenField ID="HiddenForAcceptType" runat="server" />
                    <select id="AcceptType" class="easyui-combobox" style="width: 150px" data-options="editable:false,multiple:true">
                    </select>
                </td>
                <td>电话类型：</td>
                <td>
                    <asp:HiddenField ID="HiddenForCallType" runat="server" />
                    <select id="CallType" class="easyui-combobox" style="width: 150px" data-options="editable:false,multiple:true">
                    </select>
                </td>

                <td>&nbsp;
                   <asp:Button ID="btnSearch" runat="server" Text="查询" OnClick="btnSearch_Click" />

                    &nbsp;
                </td>
            </tr>
        </table>
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" Height="100%" Width="100%"></rsweb:ReportViewer>
    </form>
</body>
</html>
