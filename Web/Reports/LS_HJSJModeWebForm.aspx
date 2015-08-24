<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LS_HJSJModeWebForm.aspx.cs" Inherits="Anke.SHManage.Web.Reports.WebForm1" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>呼救事件流水表</title>
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
            //取要车性质的下拉框的值
            $("#alarmType").combobox({
                url: "../BB/AllDictionary/GetAlarmEventType",
                valueField: 'ID',
                textField: 'Name',
                editable: false,
                multiple: true,
                prompt: '--请选择--',
                //onSelect: function (rec) {
                //    var ids = $('#alarmType').combobox('getValues');
                //    $('#HiddenForalarmType').val(ids);
                //}
            });

            //取车辆状态的下拉框  
            $('#ambulanceState').combobox({
                valueField: 'ID',
                textField: 'Name',
                method: 'get',
                editable: false,
                panelHeight: 'auto',
                multiple: true,
                //value: '--请选择--',
                prompt: '--请选择--',
                data: [
                {
                    ID: '10',
                    Name: '完成',
                }, {
                    ID: '11',
                    Name: '未出车'
                },
                {
                    ID: '0',
                    Name: '分配任务',
                }, {
                    ID: '1',
                    Name: '待出动'
                },
            {
                ID: '2',
                Name: '驶向现场',
            }, {
                ID: '3',
                Name: '现场抢救'
            },
                {
                    ID: '4',
                    Name: '载人送院',
                }, {
                    ID: '5',
                    Name: '医院交接'
                },
                {
                    ID: '6',
                    Name: '途中待命',
                }, {
                    ID: '7',
                    Name: '站内待命'
                },
                {
                    ID: '8',
                    Name: '不能调用',
                }, {
                    ID: '9',
                    Name: '暂停调用'

                }],
                onSelect: function () {
                    $('#HiddenForambulanceState').val($('#ambulanceState').combobox('getValues'));
                },
                onUnselect: function () {
                    $('#HiddenForambulanceState').val($('#ambulanceState').combobox('getValues'));
                }
            })
        }
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
                <td style="text-align:center">&nbsp;&nbsp; 至</td>
                <td>
                    <asp:TextBox ID="EndDate" class="easyui-datetimebox" Style="width: 150px" runat="server"></asp:TextBox>

                </td>
                <td>&nbsp;&nbsp; 要车性质：</td>
                <td>
                    <%--<asp:HiddenField ID="HiddenForalarmType" runat="server" />--%>
                    <asp:TextBox ID="alarmType" class="easyui-combobox" Style="width: 150px" runat="server"></asp:TextBox>
                </td>
                <td>&nbsp;&nbsp; 车辆状态：</td>
                <td>
                    <asp:HiddenField ID="HiddenForambulanceState" runat="server" />
                    <select id="ambulanceState" class="easyui-combobox" style="width: 150px" data-options="editable:false,multiple:true">                      
                       <%-- <option value="完成">'完成'</option> 
                        <option value="未出车">'未出车'</option>
                        <option value="分配任务">'分配任务'</option>
                        <option value="待出动">'待出动'</option>
                        <option value="驶向现场">'驶向现场'</option>
                        <option value="现场抢救">'现场抢救'</option>
                        <option value="载人送院">'载人送院'</option>
                        <option value="医院交接">'医院交接'</option>
                        <option value="途中待命">'途中待命'</option>
                        <option value="站内待命">'站内待命'</option>
                        <option value="不能调用">'不能调用'</option>
                        <option value="暂停调用">'暂停调用'</option>--%>
                    </select>            
                </td>   
            </tr>
            <tr>
                <td style="text-align:right">电话：</td>
                <td>
                    <asp:TextBox ID="callNumber" class="easyui-textbox" Style="width: 150px" runat="server"></asp:TextBox>
                </td>
                <td style="text-align:right">&nbsp;&nbsp; 车号：</td>
                <td>
                    <asp:TextBox ID="ambulance" class="easyui-textbox" Style="width: 150px" runat="server"></asp:TextBox>
                </td>
                <td style="text-align:right">&nbsp; 司机：</td>
                <td>
                    <asp:TextBox ID="driver" class="easyui-textbox" Style="width: 150px" runat="server"></asp:TextBox>
                </td>
                <td style="text-align:right">&nbsp;&nbsp; 医生：</td>
                <td>
                    <asp:TextBox ID="doctor" class="easyui-textbox" Style="width: 150px" runat="server"></asp:TextBox>
                </td>
                <td>&nbsp</td>
            </tr>
            <tr>
                <td>现场地址：</td>
                <td>
                    <asp:TextBox ID="sceneAddress" class="easyui-textbox" Style="width: 150px" runat="server"></asp:TextBox>
                </td>
                <td>&nbsp;&nbsp; 送往地点：</td>
                <td>
                    <asp:TextBox ID="sendAddress" class="easyui-textbox" Style="width: 150px" runat="server"></asp:TextBox>
                </td>
                <td style="text-align:right">&nbsp;&nbsp; 患者：</td>
                <td>
                    <asp:TextBox ID="name" class="easyui-textbox" Style="width: 150px" runat="server"></asp:TextBox>
                </td>
                <td style="text-align:right">&nbsp;&nbsp; 病因：</td>
                <td>
                    <asp:TextBox ID="illReason" class="easyui-textbox" Style="width: 150px" runat="server"></asp:TextBox>
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
