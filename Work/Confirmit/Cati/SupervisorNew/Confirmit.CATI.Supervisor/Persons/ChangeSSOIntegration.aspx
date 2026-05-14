<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/Main.Master"
    CodeBehind="ChangeSSOIntegration.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Persons.ChangeSSOIntegration" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="server">
    <controls:Dialog ID="dialog" runat="server" Mode="Modal" HideHeader="true">
        <OKButton OnClick="OKButtonClick" ResName="ChangeButtonText" />
        <Content>
            <div class="flex-panel flex-panel-column" style="height: 100%;">
                <div class="flex-panel flex-panel-row flex-panel-row--justify" style="margin: 0px 20px;">
                    <asp:Label ID="lblSSOIntegration" runat="server" Text="<%$CPResource:SSOIntegration%>" />
                    <controls:DropDownList ID="ddlSSOIntegration" runat="server">
                        <asp:ListItem Text="<%$CPResource:NoSSO%>" />
                        <asp:ListItem Selected="true" Text="<%$CPResource:DefaultSSO%>" />
                    </controls:DropDownList>
                    <controls:HelpTextViewer ID="ChangeSSO" runat="server" HelpTextId="ChangeSSOHelpText" />
                </div>
            </div>
        </Content>
    </controls:Dialog>
</asp:Content>
