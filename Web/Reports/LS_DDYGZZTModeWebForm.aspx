<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LS_DDYGZZTModeWebForm.aspx.cs" Inherits="Anke.SHManage.Web.Reports.DDYGZZTModeWebForm" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>调度员工作状态流水表</title>
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
            //$.InitDateTime();           

        });
        function fillCombobox() {

            //取DispatcherName 调度员姓名的下拉框的值
            var urlstr = "../BB/AllDictionary/GetPersonName";
            $.EUIcombobox("#DispatcherName", {
                url: urlstr,
                valueField: 'Name',
                textField: 'Name',
                editable: false,
                OneOption: [{
                    ID: "",
                    Name: "--请选择--"
                }]
            });
            //开始工作类型
            $('#StartWorkState').combobox({
                valueField: 'ID',
                textField: 'Name',
                method: 'get',
                editable: false,
                panelHeight: 'auto',
                multiple: true,
                prompt: '--请选择--',
                data: [
                {
                    ID: '0',
                    Name: '登录',
                }, {
                    ID: '1',
                    Name: '电话受理'
                },
                {
                    ID: '2',
                    Name: '手工受理',
                }, {
                    ID: '3',
                    Name: '就绪'
                },
            {
                ID: '4',
                Name: '暂停',
            }, {
                ID: '5',
                Name: '离席'
            }
                ],
                onSelect: function () {
                    $('#HiddenForStartWorkState').val($('#StartWorkState').combobox('getValues'));
                },
                onUnselect: function () {
                    $('#HiddenForStartWorkState').val($('#StartWorkState').combobox('getValues'));
                }
            })
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
                <td style="text-align: center">至</td>
                <td>
                    <asp:TextBox ID="EndDate" class="easyui-datetimebox" Style="width: 150px" runat="server"></asp:TextBox>

                </td>
                <td>&nbsp;&nbsp;&nbsp;&nbsp; 开始工作状态：</td>
                <td>
                    <asp:HiddenField ID="HiddenForStartWorkState" runat="server" />
                    <select id="StartWorkState" class="easyui-combobox" style="width: 150px" data-options="editable:false,multiple:true">
                        <%--<option value="登录">'登录'</option> 
                        <option value="电话受理">'电话受理'</option> 
                        <option value="手工受理">'手工受理'</option>
                        <option value="就绪">'就绪'</option>
                        <option value="暂停">'暂停'</option>
                        <option value="离席">'离席'</option>--%>
                    </select>
                </td>
                <td>&nbsp;&nbsp;&nbsp;&nbsp; 工作状态大于(分钟)：</td>
                <td>
                    <asp:TextBox ID="WorkStateTime" class="easyui-textbox" Style="width: 150px" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="text-align: right">姓名：</td>
                <td>
                    <asp:TextBox ID="DispatcherName" class="easyui-combobox" Style="width: 150px" runat="server"></asp:TextBox>
                </td>
                <td style="text-align: right">&nbsp;&nbsp;&nbsp;&nbsp; 工号：</td>
                <td>
                    <asp:TextBox ID="WorkNumber" class="easyui-textbox" Style="width: 150px" runat="server"></asp:TextBox>
                </td>
                <td>&nbsp;&nbsp;&nbsp;
                    <asp:Button ID="btnSearch" runat="server" Text="查询" OnClick="btnSearch_Click" Height="21px" />
                    &nbsp;
                        <%--<a href="javascript:void(0)" id="btnsearch" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="bindGrid()">查询</a>--%>
                </td>
            </tr>
        </table>
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" Height="98%" Width="100%"></rsweb:ReportViewer>
    </form>
</body>
</html>
