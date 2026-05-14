<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true"
    CodeBehind="SupervisorToCallCenterAssignment.aspx.cs" Inherits="Confirmit.CATI.Supervisor.CallCenters.SupervisorToCallCenterAssignment" %>

<%@ Register TagPrefix="Controls" TagName="CallCentersList" Src="~/CallCenters/Controls/CallCentersList.ascx" %>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
    <Controls:Dialog runat="server" ID="dialogControl" HideHeader="True" Mode="Modal">
        <OKButton OnClick="SaveButtonClick"  Text="<%$CPResource:AssignToCallCenter%>"/>
        <content>
            <controls:CallCentersList runat="server" ID="_callCenters" EnableMultiSelection="False" 
                                      ReadOnly="True" RightToolbarButtons="None" />
        </content>
    </Controls:Dialog>
</asp:Content>
