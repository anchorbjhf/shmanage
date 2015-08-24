(function ($) {
    $.extend($,
        {
            GID: function (cid) {
                return document.getElementById(cid);
            },
            GNAME: function (cName) {
                return document.getElementsByName(cName);
            },
            //显示消息：如果有easyui，则调用easyui的message组件显示消息
            showMsg: function (msg, title, funcSuc) {
                //error,question,info,warning
                if ($.messager) {
                    $.messager.alert(title, msg, "info", function () {
                        if (funcSuc) funcSuc();
                    });
                } else {
                    alert(title + "\r\n     " + msg);
                    if (funcSuc) funcSuc();
                }
            },
            //统一处理 返回的json数据格式
            handleJsonResult: function (data, funcSuc, funcErr) {
                if (!data || !data.Result) {
                    return;
                }

                switch (data.Result) {
                    case "OK":
                        if (data.Message && data.Message.trim() != "") { $.showMsg(data.Message, "系统提示", function () { if (funcSuc) funcSuc(data); }); }
                        else if (funcSuc) funcSuc(data);
                        break;
                    case "Error":
                        if (data.Message && data.Message.trim() != "") { $.showMsg(data.Message, "系统提示", function () { if (funcErr) funcErr(data); }); }
                        else if (funcErr) funcErr(data);
                        break;
                    case "NoLogin":
                        $.alertMsg(data.Message, "系统提示", function () { if (window.top) window.top.location = data.RedirectURL; else window.location = data.RedirectURL; });
                        break;
                }
            },
            InitDateTime: function () {
                $(".datebox :text").attr("readonly", "readonly");
                $("#StartDate").datetimebox("setValue", $.GetYesterday());
                $("#EndDate").datetimebox("setValue", $.GetTomorrow());
            },
            InitMonthFirstDateTime: function () {
                $(".datebox :text").attr("readonly", "readonly");
                $("#StartDate").datetimebox("setValue", $.GetMonthFirstday());
                $("#EndDate").datetimebox("setValue", $.GetTomorrow());
            },

            InitDate: function () {
                $(".datebox :text").attr("readonly", "readonly");
                $("#StartDate").datebox("setValue", $.GetYesterday());
                $("#EndDate").datebox("setValue", $.GetTomorrow());
            },
            InitControlDateTime: function (controlID) {
                $(".datebox :text").attr("readonly", "readonly");
                $(controlID).datetimebox("setValue", $.GetToday());
            },
            InitControlDate: function (controlID) {
                $(".datebox :text").attr("readonly", "readonly");
                $(controlID).datebox("setValue", $.GetToday());
            },
            readyonlyDatebox: function (controlID) {
                $(".datebox :text").attr("readonly", "readonly");
                $(controlID).datebox("setValue", "");
            },



            PercentNum: function (num, num2) {
                return (Math.round(num / num2 * 10000) / 100.00 + "%"); //小数点后两位百分比
            },
            GetFuture: function (ADDdays) {
                var today = new Date();
                var future_milliseconds = today.getTime() + 1000 * 60 * 60 * 24 * ADDdays;
                var future = new Date();
                future.setTime(future_milliseconds);
                return future;
            },
            GetYesterday: function () {
                var today = new Date();
                var yesterday_milliseconds = today.getTime() - 1000 * 60 * 60 * 24;
                var yesterday = new Date();
                yesterday.setTime(yesterday_milliseconds);
                var strYear = yesterday.getFullYear();

                var strDay = yesterday.getDate();
                var strMonth = yesterday.getMonth() + 1;
                var hh = today.getHours();
                var mm = today.getMinutes();
                var ss = today.getSeconds();

                var strYesterday = strYear + "-" + strMonth + "-" + strDay + " " + hh + ":" + mm + ":" + ss;
                return strYesterday;
            },
            GetMonthFirstday: function () {
                var today = new Date();
                var strYear = today.getFullYear();
                var strDay = "01";
                var strMonth = today.getMonth() + 1;
                if (strMonth < 10) {
                    strMonth = "0" + strMonth;
                }

                var strMonthFirstday = strYear + "-" + strMonth + "-" + strDay + " " + "00:00:00";
                return strMonthFirstday;
            },
            GetToday: function () {
                var today = new Date();

                var strYear = today.getFullYear();
                var strDay = today.getDate();
                var strMonth = today.getMonth() + 1;

                if (strMonth < 10) {
                    strMonth = "0" + strMonth;
                }
                var hh = today.getHours();
                var mm = today.getMinutes();
                var ss = today.getSeconds();
                var strToday = strYear + "-" + strMonth + "-" + strDay + " " + hh + ":" + mm + ":" + ss;;
                return strToday;
            },
            GetTomorrow: function () {
                var today = new Date();
                var tomorrow_milliseconds = today.getTime() + 1000 * 60 * 60 * 24;
                var tomorrow = new Date();
                tomorrow.setTime(tomorrow_milliseconds);
                var strYear = tomorrow.getFullYear();
                var strDay = tomorrow.getDate();
                var strMonth = tomorrow.getMonth() + 1;
                var hh = today.getHours();
                var mm = today.getMinutes();
                var ss = today.getSeconds();

                var strTomorrow = strYear + "-" + strMonth + "-" + strDay + " " + hh + ":" + mm + ":" + ss;
                return strTomorrow;
            },
            formatTime: function (val, row) {
                if (val == null) {
                    return "无";
                } else {
                    var da = new Date(parseInt(val.replace("/Date(", "").replace(")/", "").split("+")[0]));
                    var hh = da.getHours() < 9 ? "0" + da.getHours() : da.getHours();
                    var mm = da.getMinutes() < 9 ? "0" + da.getMinutes() : da.getMinutes();
                    var ss = da.getSeconds() < 9 ? "0" + da.getSeconds() : da.getSeconds();
                    return da.getFullYear() + "-" + (da.getMonth() + 1) + "-" + da.getDate() + "  " + hh + ":" + mm + ":" + ss;
                }
            },
            formatDate: function (val, row) {
                var da = new Date(parseInt(val.replace("/Date(", "").replace(")/", "").split("+")[0]));
                return da.getFullYear() + "-" + (da.getMonth() + 1) + "-" + da.getDate();
            },
            formatValidityDate: function (val, row) {
                var da = new Date(parseInt(val.replace("/Date(", "").replace(")/", "").split("+")[0]));
                if (da.getFullYear() == "9999")
                    return "无有效期";
                else
                    return da.getFullYear() + "-" + (da.getMonth() + 1) + "-" + da.getDate();
            },
            formatValidityDateToDateTime: function (val) {
                return new Date(parseInt(val.replace("/Date(", "").replace(")/", "").split("+")[0]));
            },
            formatRealBatchNo: function (val, row) {
                if (val.substr(val.length - 8, 8) == "99991231")
                    return "无批次号";
                else
                    return val;
            },
            BindCombox: function (cid, urinfo) {
                $(cid).combobox({
                    prompt: '--请选择--',
                    url: urinfo,//ajax后台取数据路径，返回的是json格式的数据 
                    valueField: 'ID',
                    textField: 'Name',
                    method: 'POST',
                    editable: false
                });
            },
            EUIcombobox: function (cid, o) {
                $.ajax({
                    type: "POST",
                    async: false,
                    url: o.url,
                    dataType: "JSON",
                    data: o.datainfo,
                    success: function (data1, textStatus, jqXHR) {
                        o.data = o.OneOption.concat(data1);
                        delete o.url;
                        delete o.OneOption;
                        delete o.datainfo;
                        $(cid).combobox(o);
                        $(cid).combobox('loadData', o.data);
                        $(cid).combobox('select', o.data[0].ID);
                    }
                });
            },
            EUIcomboboxForReport: function (cid, o) {
                $.ajax({
                    type: "POST",
                    async: false,
                    url: o.url,
                    dataType: "JSON",
                    data: o.datainfo,
                    success: function (data1, textStatus, jqXHR) {
                        o.data = o.OneOption.concat(data1);
                        delete o.url;
                        delete o.OneOption;
                        delete o.datainfo;
                        $(cid).combobox(o);
                        $(cid).combobox('loadData', o.data);

                    }
                });
            },
            EUIcomboboxTree: function (cid, o) {
                $.ajax({
                    type: "POST",
                    async: false,
                    url: o.url,
                    dataType: "JSON",
                    data: o.datainfo,
                    success: function (data1, textStatus, jqXHR) {
                        o.data = o.OneOption.concat(data1);
                        delete o.url;
                        delete o.OneOption;
                        $(cid).combotree(o);
                        $(cid).combotree('loadData', o.data);
                    }
                });
            },
            BindComboxTree: function (cid, urinfo) {
                $(cid).combotree({
                    prompt: '--请选择--',
                    url: urinfo,//ajax后台取数据路径，返回的是json格式的数据 
                    method: 'POST'
                });
            },
            setGridWithSearchBar: function (tid, sid, cid) {
                var height = $(window).height() - 20;
                var heightMargin = $(sid).height() + 50;
                $(tid).layout({
                    width: '100%',
                    height: height
                });
                $(cid).resizeDataGrid(heightMargin, 0);

            },
            setGridWithSearchBarAndBtn: function (tid, sid, cid) {
                var height = $(window).height() - 20;
                var heightMargin = $(sid).height() + 80;
                $(tid).layout({
                    width: '100%',
                    height: height
                });
                $(cid).resizeDataGrid(heightMargin, 0);

            },
            setGridOnly: function (tid, cid) {
                var height = $(window).height() - 20;
                $(tid).layout({
                    width: '100%',
                    height: height
                });
                $(cid).resizeDataGrid(0, 0);
            },
            setDivOnly: function (tid) {
                var height = $(window).height() - 20;
                $(tid).layout({
                    width: '100%',
                    height: height
                });
            },
            initTextBoxNum: function (cid, title, isrequired) {
                $(cid).numberbox({
                    prompt: title,
                    precision: 0,
                    groupSeparator: ',',
                    required: isrequired,
                    iconWidth: 22,
                    icons: [{
                        iconCls: 'icon-add',
                        handler: function (e) {
                            var num1 = $(e.data.target).textbox('getValue');
                            if (num1 == "")
                                $(e.data.target).textbox('setValue', 1);
                            else
                                $(e.data.target).textbox('setValue', parseInt(num1) + 1);
                        }
                    }, {
                        iconCls: 'icon-remove',
                        handler: function (e) {
                            var num1 = $(e.data.target).textbox('getValue');
                            if (parseInt(num1) > 1) {
                                $(e.data.target).textbox('setValue', parseInt(num1) - 1);
                            }
                        }
                    }]
                });
            },
            initDataGridTextBoxNum: function (cid, MaxNum, isrequired) {
                $(cid).numberbox({
                    precision: 0,
                    groupSeparator: ',',
                    required: isrequired,
                    iconWidth: 22,
                    min: 1,
                    max: parseInt(MaxNum),
                    icons: [{
                        iconCls: 'icon-add',
                        handler: function (e) {
                            var num1 = $(e.data.target).textbox('getValue');
                            if (num1 == "") {
                                $(e.data.target).textbox('setValue', 1);
                            } else {
                                if (num1 < MaxNum) {
                                    $(e.data.target).textbox('setValue', parseInt(num1) + 1);
                                }
                            }
                        }
                    }, {
                        iconCls: 'icon-remove',
                        handler: function (e) {
                            var num1 = $(e.data.target).textbox('getValue');
                            if (parseInt(num1) > 1) {
                                $(e.data.target).textbox('setValue', parseInt(num1) - 1);
                            }
                        }
                    }]
                });
            },
            initTextBoxMoney: function (cid, title, isrequired) {
                $(cid).numberbox({
                    prompt: title,
                    precision: 2,
                    groupSeparator: ',',
                    decimalSeparator: '.',
                    prefix: '￥',
                    required: isrequired
                });
            }
        });
    $.fn.extend({
        /** 
         * 修改DataGrid对象的默认大小，以适应页面宽度。 
         *  
         * @param heightMargin 
         *            高度对页内边距的距离。 
         * @param widthMargin 
         *            宽度对页内边距的距离。 
         * @param minHeight 
         *            最小高度。 
         * @param minWidth 
         *            最小宽度。 
         *  
         */
        resizeDataGrid: function (heightMargin, minHeight) {
            var height = $(document.body).height() - heightMargin;
            height = height < minHeight ? minHeight : height;

            $(this).datagrid('resize', {
                height: height,
                width: '100%'
            });
        }
    });
}(jQuery));

///重写Array的删除方法
Array.prototype.remove = function (obj) {
    for (var i = 0; i < this.length; i++) {
        var temp = this[i];
        if (!isNaN(obj)) {
            temp = i;
        }
        if (temp == obj) {
            for (var j = i; j < this.length; j++) {
                this[j] = this[j + 1];
            }
            this.length = this.length - 1;
        }
    }
}



