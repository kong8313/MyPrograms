<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/Main.Master"
    CodeBehind="InterviewersSelectionPage.aspx.cs" Inherits="Confirmit.CATI.Supervisor.SurveysInterviewersSelection.InterviewersSelectionPage" %>

<%@ Register TagPrefix="controls" TagName="Dg" Src="Controls/DoubleInterviewersGrid.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="Content">
    <controls:Dialog ID="dialogControl" runat="server" EnableViewState="true" HideHeader="True" PutActionButtonsInsideGridIfPossible="False"
        Mode="Modal">
        <OKButton Text="Save selected" OnClick="Save"></OKButton>
        <Content>
            <div class="flex-panel flex-panel-column" style="height: 100%;">
                <controls:Hint runat="server" Text="Select interviewers/groups in order to filter data. Note that if no interviewers/groups are selected all interviewers will be used." />
                <controls:UpdatePanel runat="server" ChildrenAsTriggers="True" UpdateMode="Always" class="flex-panel--all-awailable-space" style="margin: 0px 20px 20px 20px;">
                    <ContentTemplate>
                        <controls:Dg runat="server" ID="doubleGrid" />
                    </ContentTemplate>
                </controls:UpdatePanel>
            </div>
        </Content>
    </controls:Dialog>
</asp:Content>
