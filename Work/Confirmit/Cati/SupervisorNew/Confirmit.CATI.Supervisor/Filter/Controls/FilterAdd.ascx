<%@ Control Language="c#" AutoEventWireup="True" CodeBehind="FilterAdd.ascx.cs" Inherits="Confirmit.CATI.Supervisor.Filter.Controls.FilterAdd" %>

<%@ Import Namespace="Confirmit.CATI.Supervisor.Resources" %>
<script>
    var aSigns = new Array();
    var aOperations = new Array();
    var aDefValues = new Array();

    function InitializeConstants() {
        aSigns[FilterOperator.Less + ""] = "<";
        aSigns[FilterOperator.Bigger + ""] = ">";
        aSigns[FilterOperator.Equal + ""] = "=";
        aSigns[FilterOperator.LessEqual + ""] = "<=";
        aSigns[FilterOperator.BiggerEqual + ""] = ">=";
        aSigns[FilterOperator.NotEqual + ""] = "<>";
        aSigns[FilterOperator.Like + ""] = "LIKE";
        aSigns[FilterOperator.Subfilter + ""] = "SUBFILTER";

        aOperations[VariableTypes.Integer + ""] = new Array(FilterOperator.Less, FilterOperator.Bigger, FilterOperator.Equal, FilterOperator.NotEqual);
        aOperations[VariableTypes.String + ""] = new Array(FilterOperator.Like, FilterOperator.Less, FilterOperator.Bigger, FilterOperator.Equal, FilterOperator.NotEqual);
        aOperations[VariableTypes.Date + ""] = new Array(FilterOperator.Less, FilterOperator.Bigger, FilterOperator.Equal, FilterOperator.NotEqual);
        aOperations[VariableTypes.Decimal + ""] = new Array(FilterOperator.Less, FilterOperator.Bigger, FilterOperator.Equal, FilterOperator.NotEqual);
        aOperations[VariableTypes.Subfilter + ""] = null;
        aOperations[VariableTypes.PredefinedValue + ""] = new Array(FilterOperator.Equal, FilterOperator.NotEqual);

        aDefValues[VariableTypes.Integer + ""] = "0";
        aDefValues[VariableTypes.String + ""] = "";
        aDefValues[VariableTypes.Date + ""] = "";
        aDefValues[VariableTypes.Decimal + ""] = "0.0";
        aDefValues[VariableTypes.Subfilter + ""] = null;
    }

    var SelectedRow = null;

    function ClearSelection() {
        if (SelectedRow != null) {
            SelectedRow.style.backgroundColor = "#ffffff";
            SelectedRow = null;
        }
    }

    function SelectVariable(evt) {
        ClearSelection();
        SelectedRow = findInParents(evt.target._node, "tr");
        SelectedRow.style.backgroundColor = "#BEC9E2";
    }

    function GetImageByTableType(iTableType) {
        switch (iTableType) {
            case TableTypes.Call:
                return ( "<%=BaseRelativePath("svgimages/call.svg")%>");
                break;
            case TableTypes.ShiftType:
                return ( "<%=BaseRelativePath("svgimages/call.svg")%>");
                break;
            case TableTypes.Resource:
                return ( "<%=BaseRelativePath("svgimages/call.svg")%>");
                break;
            case TableTypes.Interview:
                return ( "<%=BaseRelativePath("svgimages/receipt.svg")%>");
                break;
            case TableTypes.QSLVariables:
                return ( "<%=BaseRelativePath("svgimages/data_usage.svg")%>");
                break;
            case TableTypes.CFVariables:
                return ( "<%=BaseRelativePath("svgimages/question_answer.svg")%>");
                break;
            case TableTypes.Quotas:
                return ( "<%=BaseRelativePath("svgimages/filter_list.svg")%>");
                break;
            case TableTypes.Appointment:
                return ( "<%=BaseRelativePath("svgimages/time.svg")%>");
                break;
            case TableTypes.Subfilter:
                return ( "<%=BaseRelativePath("svgimages/filter_list.svg")%>");
                break;
            default:
                return ( "<%=BaseRelativePath("svgimages/receipt.svg")%>");
                break;
        }
    }
    function convertDateToCalendarFormat(date) {
        var textdate = date.toString();
        var myArray = textdate.split(' ');
        var input = myArray[0] + ', ' + myArray[2] + ' ' + myArray[1] + ' ' + myArray[3] + ' ' + myArray[6].substring(0, 4);
        var output = Date.parse(input);
        return new Date(output);
    }

    function convertDateToStoreFormat(value) {
        //from DDMMYYYY H:M:S to YYYY-MM-DD H:M:S
        if (value == "") return "";
        var dat_time = value.split(' ');
        var dat_str = dat_time[0];
        var time_str = dat_time[1];
        if (dat_str == "" || time_str == "") return "";
        var dat_info = dat_str.split('.');
        return dat_info[2] + "-" + dat_info[1] + "-" + dat_info[0] + " " + time_str;
    }


    function convertDateToUserFormat(date, format) {
        var year = date.getFullYear();
        var s_year = date.getFullYear().toString();

        var month = (date.getMonth() + 1);
        var s_month = (date.getMonth() + 1).toString();

        var day = date.getDate();
        var s_day = date.getDate().toString();

        var s_hour = date.getHours().toString();
        if (date.getHours() < 10)
            s_hour = "0" + s_hour;

        var s_minutes = date.getMinutes().toString();
        if (date.getMinutes() < 10)
            s_minutes = "0" + s_minutes;

        var s_seconds = date.getSeconds().toString();
        if (date.getSeconds() < 10)
            s_seconds = "0" + s_seconds;

        var res = format;

        if (format.indexOf("yyyy") != -1) res = res.replace("yyyy", s_year);
        if (format.indexOf("yy") != -1) res = res.replace("yy", s_year.substring(2, 4));

        if (format.indexOf("mm") != -1) {
            if (month > 0 && month < 10)
                res = res.replace("mm", "0" + s_month);
            else
                res = res.replace("mm", s_month);
        }
        if (format.indexOf("m") != -1) res = res.replace("m", month);


        if (format.indexOf("dd") != -1) {
            if (day > 0 && day < 10)
                res = res.replace("dd", "0" + s_day);
            else
                res = res.replace("dd", s_day);
        }
        if (format.indexOf("d") != -1) res = res.replace("d", day);

        res += " " + s_hour + ":" + s_minutes + ":" + s_seconds;

        return res;
    }


    function AddVariable() {
        var tree = $find("<%=variablesTree.TreeClientId %>");

        if (!tree.get_selectedNodes())
            return;

        var el = tree.get_selectedNodes()[0];

        if (!el) return;
        if (el.get_level() != 1) return;

        var elementInfo = Y.JSON.parse(el.get_valueString());

        var tbl = document.getElementById( "<%=TblFields.ClientID%>");

        var row = tbl.insertRow(-1);
        row.setAttribute("TableType", elementInfo.TableType);
        row.setAttribute("Column", elementInfo.Column);
        row.setAttribute("VarType", elementInfo.VarType);
        row.setAttribute("IsBackground", elementInfo.IsBackground);
        row.setAttribute("VarTitle", el.get_text());
        Y.one(row).on("click", SelectVariable);

        var cell = row.insertCell(-1);
        cell.innerHTML =
            "<nobr><img src=\"" + GetImageByTableType(elementInfo.TableType) +
            "\" align=\"absmiddle\">&nbsp;" + el.get_text() + "</nobr>";

        //HERE
        switch (elementInfo.VarType) {
            case VariableTypes.Date:
                var dropdownTemplate = document.getElementById("dropdown-template");
                var cloned = dropdownTemplate.cloneNode(true);
                var ctrl = cloned.childNodes[0];
                ctrl.style.display = "block";
                for (var i = 0; i < aOperations[elementInfo.VarType + ""].length; i++) {
                    var j = aOperations[elementInfo.VarType + ""][i];
                    var opt = document.createElement("OPTION");
                    opt.text = aSigns[j + ""];
                    opt.value = j;
                    ctrl.add(opt);
                    if (aSigns[j + ""] == "=")
                        ctrl.selectedIndex = i;
                }
                var cell = row.insertCell(-1);
                cell.appendChild(cloned);

                var hdn = document.getElementById("hdn");
                var ctrl = hdn.cloneNode(true);
                var cell = row.insertCell(-1);
                cell.appendChild(ctrl);

                var txt = document.getElementById("txt");
                var ctrl = txt.cloneNode(true);
                ctrl.style.display = "block";
                ctrl.style.width = "100%";
                ctrl.readOnly = true;
                ctrl.value = aDefValues[elementInfo.VarType + ""];
                cell.appendChild(ctrl);

                var btn = document.getElementById("<%=btn.ClientID%>");
                var ctrl = btn.cloneNode(true);
                ctrl.style.display = "block";
                ctrl.style.width = "30";
                var cell = row.insertCell(-1);
                cell.appendChild(ctrl);
                break;

            case VariableTypes.Subfilter:
                var hdn = document.getElementById("hdn");
                var ctrl = hdn.cloneNode(true);
                ctrl.value = FilterOperator.Subfilter;
                var cell = row.insertCell(-1);
                cell.innerHTML = "&nbsp;";
                cell.appendChild(ctrl);

                var hdn = document.getElementById("hdn");
                var ctrl = hdn.cloneNode(true);
                ctrl.value = elementInfo.Value;
                var cell = row.insertCell(-1);
                cell.colSpan = 2;
                cell.innerHTML = "&nbsp;";
                cell.appendChild(ctrl);

                break;

            default:
                var dropdownTemplate = document.getElementById("dropdown-template");
                var cloned = dropdownTemplate.cloneNode(true);
                var ctrl = cloned.childNodes[0];
                ctrl.style.display = "block";
                if (elementInfo.TableType == TableTypes.ShiftType && elementInfo.Column == "Name") {
                    var opt = document.createElement("OPTION");
                    opt.text = aSigns[FilterOperator.Equal + ""];
                    opt.value = FilterOperator.Equal;
                    ctrl.add(opt);
                }
                else {
                    for (var i = 0; i < aOperations[elementInfo.VarType + ""].length; i++) {
                        var j = aOperations[elementInfo.VarType + ""][i];
                        var opt = document.createElement("OPTION");
                        opt.text = aSigns[j + ""];
                        opt.value = j;
                        ctrl.add(opt);
                        if (aSigns[j + ""] == "=")
                            ctrl.selectedIndex = i;
                    }
                }
                var cell = row.insertCell(-1);
                cell.appendChild(cloned);

                var elementId = null;
                if (elementInfo.Column == "TransientState")
                    elementId = "<%=ddlITS.ClientID%>";

                if (elementInfo.Column == "CallState")
                    elementId = "<%=ddlState.ClientID%>";

                if (elementInfo.Column == "ReviewStatus")
                    elementId = "<%=ddlReviewStatus.ClientID%>";

                if (elementId != null) {
                    var btn = document.getElementById(elementId).parentNode;
                    var ctrl = btn.cloneNode(true);
                    ctrl.style.display = "block";
                    var cell = row.insertCell(-1);
                    cell.colSpan = 2;
                    cell.appendChild(ctrl);
                    break;
                }

                var txt = document.getElementById("txt");
                var ctrl = txt.cloneNode(true);
                ctrl.value = aDefValues[elementInfo.VarType + ""];
                if (elementInfo.Column != "TransientState") {
                    ctrl.style.display = "block";
                }
                var cell = row.insertCell(-1);
                cell.appendChild(ctrl);
                cell.colSpan = 2;


        }
    }

    function RemoveVariable() {
        var tbl = document.getElementById( "<%=TblFields.ClientID%>");

        if (SelectedRow != null) {
            tbl.deleteRow(SelectedRow.rowIndex);
            SelectedRow = null;
        }
    }

    function ShowCalendar(evt) {
        evt = Y.Event.getEvent(evt);
        var strg = "<%=BaseRelativePath("DateDialog.aspx")%>";
        var btn = evt.target._node;
        var tr = findInParents(btn, "tr");
        var hdn = tr.cells[2].children[0];
        var txt = tr.cells[2].children[1];
        if (hdn.value != "") strg += "?date=" + hdn.value;

        var settings = { height: "280px", width: "260px", calledWindow: window };

        overlay.overlayClosedEvent.on(function (args) {
            if (args.result !== true)
                return;

            var resultText = args.data;
            var result = Y.JSON.parse(resultText);
            txt.value = result.text;
            hdn.value = convertDateToUserFormat(new Date(result.dateMilliseconds), "yyyy-mm-dd");
        });

        overlay.show("Select Date and Time", strg, null, settings, null);
    }

    function GetXML() {
        var tbl = document.getElementById("<%=TblFields.ClientID%>");

        if (ValidateConditions(tbl) == false) {
            return false;
        }

        var strg = "";
        strg += "<vars>";
        for (var i = 1; i < tbl.rows.length; i++) {
            strg += "<var>";
            strg += "<TableType>" + tbl.rows[i].getAttribute("TableType") + "</TableType>";
            strg += "<Column>" + tbl.rows[i].getAttribute("Column") + "</Column>";
            strg += "<VarType>" + tbl.rows[i].getAttribute("VarType") + "</VarType>";
            strg += "<Sign>" + (tbl.rows[i].cells[1].children[0].children.length > 0
                ? tbl.rows[i].cells[1].children[0].children[0].value
                : tbl.rows[i].cells[1].children[0].value) + "</Sign>";
            strg += "<Value>" + (tbl.rows[i].cells[2].children[0].children.length > 0
                ? tbl.rows[i].cells[2].children[0].children[0].value
                : tbl.rows[i].cells[2].children[0].value) + "</Value>";
            strg += "<Disable>0</Disable>";
            strg += "<IsBackground>" + tbl.rows[i].getAttribute("IsBackground") + "</IsBackground>";
            strg += "</var>";
        }
        strg += "</vars>";

        var re = /</g;
        strg = strg.replace(re, "&lt;");

        var re = />/g;
        strg = strg.replace(re, "&gt;");

        document.getElementById( "<%=HdnFields.ClientID%>").value = strg;
    }

    //checks for each condition value
    //If value is empty set focus to input field and return false.
    function ValidateConditions(conditionTable) {
        for (var i = 1; i < conditionTable.rows.length; i++) {
            var propertyName = conditionTable.rows[i].getAttribute("VarTitle");
            var propertyType = conditionTable.rows[i].getAttribute("VarType");
            var inputControl = conditionTable.rows[i].cells[2].children[0];
            var propertyValue = inputControl.children.length === 0 ? inputControl.value : inputControl.children[0].value;

            if (propertyValue == null)
                propertyValue = "";

            if ((propertyType != VariableTypes.String || propertyName == 'Shift Type') && propertyValue.replace(/ /g, "") == "") {
                var message = '<%=Strings.PleaseEnterValue%>' + propertyName;
            }
            else if (propertyType == VariableTypes.Integer && propertyValue.trim().replace(/^\d+$/, "") != "") {
                var message = '<%=Strings.IncorrectParameterFormat%>';
            }
            else if (propertyType == VariableTypes.Decimal && propertyValue.trim().replace(/^(\-)?(\d[.]?)+(\d|$)$/, "") != "") {
                var message = '<%=Strings.IncorrectParameterFormat%>';
            }

            if (message != undefined) {
                alert(message);
                try {
                    inputControl.focus();
                }
                catch (ex) { }

                return false;
            }
        }
        return true;
    }

    Y.on("load", function () {
        Y.all(".igdt_Node").on("dblclick", AddVariable);
    });
</script>
<style type="text/css">
    input[type="radio"] {
        padding: 0px 3px;
    }

    table#inputs {
        border-width: 0px;
        width: 100%;
        padding: 2px 0px;
    }

        table#inputs tr {
            height: 25px;
        }

        table#inputs > tbody > tr > td:first-child {
            white-space: nowrap;
            width: 100px;
            padding: 1px 5px;
        }

        table#inputs select {
            width: 100%;
            max-width: 400px;
        }

        table#inputs input[type="text"] {
            width: 100%;
            max-width: 400px;
        }

    .criteria > label {
        padding-right: 25px;
        padding-bottom: 3px;
        padding-left: 10px;
        padding-top: 3px;
        vertical-align: 20%
    }

    table.fields > tbody > tr > td {
        border-right: 1px solid #c5c1b1;
        border-left: 1px solid white;
        border-bottom: 1px solid #d1cdbb;
        overflow: hidden;
    }

        table.fields > tbody > tr > td:first-child {
            border-left: 1px solid transparent;
        }

    table.fields .plain_button {
        width: 100%;
        min-width: 30px;
        padding: 0px;
    }

    .igspl_VSplitBar {
        border-top: 1px solid #4a7ac9;
    }

    .middle-menu-panel{
        display: flex;
        align-items: center;
        min-width: 70px;
    }

</style>
<controls:Dialog runat="server" ID="dialogControl" EnableViewState="true" HideHeader="True" Mode="Modal" ShowBottomBorder="True">
    <OKButton OnClick="BtnSave_ServerClick" OnClientClick="if(GetXML() == false) return;" />
    <Content>
        <main class="content-panel flex-panel flex-panel-column">
            <div id="hiddens" class="hidden">
                <input type="hidden" id="HdnFields" name="HdnFields" value="" runat="server" />
                <div class="dropdown-control" id="dropdown-template"><select id="slt" name="slt" class="plain_dropdown"></select></div>
                <input type="text" id="txt" name="txt" class="plain_textbox" />
                <controls:ImageButton runat="server" ID="btn" name="btn" IsSubmit="False" ImageName="time" OnClientClick="ShowCalendar(event);" />
                <input type="hidden" id="hdn" name="hdn" value="" />
                <controls:DropDownList ID="ddlITS" runat="server" />
                <controls:DropDownList ID="ddlState" runat="server" />
                <controls:DropDownList ID="ddlReviewStatus" runat="server" />
            </div>
            <div>
                <table id="inputs" class="settings-table settings-table--default-columns settings-table--no-min-width">
                    <tr>
                        <td>
                            <%=Strings.Name %>
                            <controls:TextFieldValidator ID="tfvName" ControlToValidate="EdtName" IsRequired="true"
                                FieldRequredErrorMessage="Err_EmptyName" ValidationErrorMessage="ErrorIncorrectValue"
                                Text="*" runat="server" />
                        </td>
                        <td>
                            <controls:TextBox ID="EdtName" runat="server" MaxLength="255" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <%=Strings.Description %>
                            <controls:TextFieldValidator ID="tfxvDescription" ControlToValidate="EdtDescription"
                                IsRequired="false" ValidationErrorMessage="ErrorIncorrectValue" Text="*" runat="server" />
                        </td>
                        <td>
                            <controls:TextBox ID="EdtDescription" runat="server" MaxLength="255" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <%=Strings.Criteria %>
                        </td>
                        <td>
                            <controls:RadioButtonList RepeatDirection="Horizontal" RepeatLayout="Flow" runat="server" ID="rblOperator" CssClass="criteria" />
                        </td>
                    </tr>
                </table>
            </div>
            <div class="flex-panel--all-awailable-space flex-panel flex-panel row" style="overflow: hidden; margin-top: 10px; margin-bottom: 20px;">
                <div style="flex: 3 1 auto; min-width: 300px; border: 1px solid rgba(18,24,33,0.12)" class="flex-panel flex-panel-column panel-with-middle-menu">
                    <div style="overflow: auto; flex: 1 1 auto;">
                        <controls:VariablesTreeControl runat="server" ID="variablesTree" EnableViewState="true" ViewStateMode="Enabled" />
                    </div>
                </div>
                <div class="middle-menu-panel">
                    <div>
                        <controls:XpMenuItem runat="server" ID="addVariable" ImageName="arrow_forward" TextAndImage="True" OnClientClick="AddVariable();return false;" />
                        <controls:XpMenuItem runat="server" ID="removeVariable" ImageName="arrow_back" TextAndImage="True" OnClientClick="RemoveVariable(); return false;" />
                    </div>
                </div>
                <div style="flex: 7 1 auto; padding-left: 10px; border: 1px solid rgba(18,24,33,0.12)" class="flex-panel flex-panel-column panel-with-middle-menu">
                    <div style="overflow-y: auto; overflow-x: hidden;">
                        <table cellspacing="0" id="TblFields" runat="server" style="width: 100%; table-layout: fixed; min-width: 300px" class="generic-grid filters-grid">
                            <tr>
                                <th style="width: 50%;">
                                    <%=Strings.Column%>
                                </th>
                                <th style="width: 60px;">
                                    <%=Strings.Operation%>
                                </th>
                                <th style="width: 50%; text-align: right;">
                                    <%=Strings.Value%>
                                </th>
                                <th style="width: 38px; padding: 0px; border-width: 1px 0px"></th>
                            </tr>
                        </table>
                    </div>
                </div>
            </div>
        </main>
    </Content>
</controls:Dialog>
