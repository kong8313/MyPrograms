<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/Main.Master"
    CodeBehind="GroupsInterviewersSelectionPage.aspx.cs" Inherits="Confirmit.CATI.Supervisor.SurveysInterviewersSelection.GroupsInterviewersSelectionPage" %>
    <%@Register tagPrefix="controls" tagName="Dg" src="Controls/DoubleGroupsInterviewersGrid.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="Content">
    <controls:Dialog ID="dialogControl"  runat="server" EnableViewState="true" HideHeader="True"
        Mode="Modal">
        <OKButton Text="Save selected" OnClick="Save"></OKButton>
        <Content>
            <div style="height: 100%;">
                <div style="position: absolute; top: 0px; width: 100%">
                    <controls:Hint runat="server" Text="Select groups or interviewers in order to filter interviewer list. Note that if no interviewers are selected all interviewers will be used." />
                </div>
                <div style="height: 100%; padding-top: 30px">
                    <controls:UpdatePanel runat="server" ChildrenAsTriggers="True" UpdateMode="Always" style="height: 100%"  >
                        <ContentTemplate>
                            <controls:Dg runat="server" ID="doubleGrid" />
                        </ContentTemplate>
                    </controls:UpdatePanel>
                </div>
            </div>
        </Content>
    </controls:Dialog>
</asp:Content>
