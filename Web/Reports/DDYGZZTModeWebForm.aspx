<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DDYGZZTModeWebForm.aspx.cs" Inherits="Anke.SHManage.Web.Reports.DDYGZZTModeWebForm" %>

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
            fillCombobox();
            //$.InitDateTime();
            //$('#StartWorkState').combobox({
            //    multiple : true
            //});
        });
        function fillCombobox() {
            alert("hi");
            //取StartWorkState 工作状态的下拉框的值
            //$("#StartWorkState").combobox({
            //    valueField: 'ID',
            //    textField: 'Name',
            //    editable: false,
            //    multiple: true,
            //    onLoadSuccess: function (param) {
            //        $("#StartWorkState").append("<option value=''>=全部=</option>");
            //    }
            //});
            //$("#StartWorkState").append("<option value='电话受理'>电话受理</option>");
            //$("#StartWorkState").append("<option value='手工受理'>手工受理</option>");
            //$("#StartWorkState").combobox({});
            //option: [{
            //    ID: "",
            //    Name: "--请选择--"
            //},
            //{
            //    ID: "电话受理",
            //    Name: "电话受理"
            //},
            //{
            //    ID: "手工受理",
            //    Name: "手工受理"
            //}, {
            //    ID: "就绪",
            //    Name: "就绪"
            //}, {
            //    ID: "暂停",
            //    Name: "暂停"
            //}, {
            //    ID: "离席",
            //    Name: "离席"
            //}]

            //取DispatcherName 调度员姓名的下拉框的值
            //var urlstr = '@Url.Content("~/IM/AllDictionary/GetPersonName")';
            //$.EUIcombobox("#DispatcherName", {
            //    url:'@Url.Content("~/IM/AllDictionary/GetPersonName")',
            //    valueField: 'ID',
            //    textField: 'Name',
            //    editable: false,
            //    OneOption: [{
            //        ID: "",
            //        Name: "--请选择--"
            //    }]
            //});
            $('#DispatcherName').combobox({
                prompt: '--请选择--',
                url: "../IM/AllDictionary/GetPersonName",//ajax后台取数据路径，返回的是json格式的数据
                valueField: 'ID',
                textField: 'Name',
                editable: false,
                method: 'POST',
                //disabled: true
            });


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
                <td>开始工作状态：</td>
                <td>
                    <asp:DropDownList ID="StartWorkState" class="easyui-combobox" Style="width: 150px" data-options="editable:false,multiple:true" runat="server">
                        <%--<asp:ListItem Value="电话受理,手工受理,就绪,暂停,离席">全部</asp:ListItem>--%>
                        <asp:ListItem Value="电话受理">电话受理</asp:ListItem>
                        <asp:ListItem Value="手工受理">手工受理</asp:ListItem>
                        <asp:ListItem Value="就绪">就绪</asp:ListItem>
                        <asp:ListItem Value="暂停">暂停</asp:ListItem>
                        <asp:ListItem Value="离席">离席</asp:ListItem>
                    </asp:DropDownList>

                </td>
                <td>工作状态大于(分钟)：</td>
                <td>
                    <asp:TextBox ID="WorkStateTime" class="easyui-textbox" Style="width: 150px" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>姓名：</td>
                <td>
                    <asp:TextBox ID="DispatcherName" class="easyui-combobox" Style="width: 150px" runat="server"></asp:TextBox>
                </td>
                <td>工号：</td>
                <td>
                    <asp:TextBox ID="WorkNumber" class="easyui-textbox" Style="width: 150px" runat="server"></asp:TextBox>
                </td>
                <td>&nbsp;<asp:Button ID="btnSearch" runat="server" Text="查询" OnClick="btnSearch_Click" />
                    &nbsp;
                        <%--<a href="javascript:void(0)" id="btnsearch" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="bindGrid()">查询</a>--%>
                </td>
            </tr>
        </table>
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" Height="100%" Width="100%"></rsweb:ReportViewer>
    </form>
</body>
</html>
