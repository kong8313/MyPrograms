<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true"
    CodeBehind="ScriptView.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Script.ScriptView" %>

<%@ Import Namespace="Confirmit.CATI.Supervisor.Resources" %>
<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="Server">
    <controls:Dialog runat="server" ID="dialog" HideButtons="true" Mode="Frame" HideHeader="true">
        <Content>
            <controls:Tabs runat="server" ID="tabs" EnableViewState="False" Style="height: 100%; width: 100%">
                <PostBackOptions EnableLoadOnDemandUrl="True"></PostBackOptions>
                <ClientEvents Loaded="WebTabLoaded"></ClientEvents>
                <Tabs>
                    <controls:TabItem runat="server" TextId="Rules" Key="Rules" ContentUrl="ScriptViewTabs/ScriptViewSchedulingRulesNew.aspx" />
                    <controls:TabItem runat="server" TextId="Shifts" Key="Shifts" ContentUrl="ScriptViewTabs/ScriptViewShiftsNew.aspx" />
                    <controls:TabItem runat="server" TextId="ShiftTypes" Key="ShiftTypes" ContentUrl="ScriptViewTabs/ScriptViewShiftTypesNew.aspx" />
                    <controls:TabItem runat="server" TextId="Params" Key="Params" ContentUrl="ScriptViewTabs/ScriptViewParams.aspx" />
                    <controls:TabItem runat="server" TextId="CustomScript" Key="CustomScript" ContentUrl="ScriptViewTabs/ScriptViewCustom.aspx" />
                </Tabs>
            </controls:Tabs>
        </Content>
    </controls:Dialog>

    <script type="text/javascript">

        function WebTabLoaded(sender) {
            sender.set_width(document.body.offsetWidth + "px");
            Y.on("windowresize", function () {
                sender.set_width(document.body.offsetWidth + "px");
            });

            Y.all('.igtab_THContent iframe').on('resize', function (event) {
                event.target.setStyle("height", "99%").setStyle("height", "100%");
            });
        }

        var scriptChanged = false;

        Y.on('beforeunload', function (e) {
            if (top.isDataChanged() && scriptChanged) {
                e.returnValue = '<%=Strings.PageWasNotSavedMessage %>';
                e.preventDefault();
            } else {
                scriptChanged = false;
            }
        });

        Common.onGlobalEvent('ScriptViewChanged', function () {
            scriptChanged = true;
        });

        Common.onGlobalEvent('ScriptViewSaved', function () {
            scriptChanged = false;
        });
    </script>
</asp:Content>
