<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true"
    CodeBehind="DeleteCallCenter.aspx.cs" Inherits="Confirmit.CATI.Supervisor.CallCenters.DeleteCallCenter" %>

<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
    <controls:Dialog runat="server" ID="_dialog" HideHeader="True" PutActionButtonsInsideGridIfPossible="False" Mode="Modal">
        <OKButton Text="<%$CPResource:Delete%>" OnClick="Delete" />
        <Content>
            <main class="content-panel">
                <controls:Hint ID="Hint1" runat="server" Text="<%$CPResource:DeleteCallCenterWarning%>" />
                <table class="settings-table--default-columns settings-table">
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="<%$CPResource:DeleteCallCenterConfirmation%>" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server" Text="<%$CPResource:MoveToCallCenterMessage%>" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <controls:DropDownList runat="server" ID="_callCenters" Width="250px" OnDataBound="CallCentersDataBound"
                                MaintainSelectedItemDuringDataBind="True" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server" Text="<%$CPResource:MoveInterviewersMessage%>" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <controls:RadioButtonList runat="server" ID="_interviewersAction" Width="250px" CssClass="radioButtonList">
                                <asp:ListItem Value="0" Text="<%$CPResource:InterviewersActionDelete%>" Selected="True"></asp:ListItem>
                                <asp:ListItem Value="1" Text="<%$CPResource:InterviewersActionMoveToCallCenter%>"></asp:ListItem>
                            </controls:RadioButtonList>
                        </td>
                    </tr>
                </table>
            </main>
            <script>
                function refreshCallCenterInfo() {
                    top.refreshCallCenterInfo();
                }
            </script>
        </Content>
    </controls:Dialog>
</asp:Content>
