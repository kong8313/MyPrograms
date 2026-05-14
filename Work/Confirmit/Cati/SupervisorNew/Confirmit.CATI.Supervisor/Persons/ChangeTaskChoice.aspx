<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/Main.Master"
    CodeBehind="ChangeTaskChoice.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Persons.ChangeTaskChoice" %>

<%@ Register TagPrefix="controls" TagName="SelectSurvey" Src="~/Persons/Controls/SelectSurvey.ascx" %>
<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="server">
    <controls:Dialog ID="dialog" runat="server" Mode="Modal" HideHeader="true">
        <OKButton OnClick="OKButtonClick" ResName="Save" />
        <Content>
            <div class="flex-panel flex-panel-column" style="height: 100%;">
                <div class="flex-panel flex-panel-row flex-panel-row--justify" style="margin: 0px 20px;">
                    <asp:Label ID="lblTaskChoice" runat="server" Text="<%$CPResource:TaskChoice%>" />
                    <controls:TaskChoiceDropDownList ID="ddlTaskChoice" runat="server" AutoPostBack="true" OnSelectedIndexChanged="SelectedChoiceChanged" />
                </div>
                <asp:Panel ID="choicePanel" runat="server" Visible="false">
                    <controls:SelectTaskChoicePermissions ID="m_SelectTaskChoicePermissions" runat="server" />
                </asp:Panel>
                <div class="flex-panel--all-awailable-space">
                    <asp:Panel ID="surveyAssignmentPanel" runat="server" Visible="false" style="height: 100%;">
                        <controls:SelectSurvey ID="surveyList" runat="server" />
                    </asp:Panel>
                </div>
            </div>
        </Content>
    </controls:Dialog>
</asp:Content>
