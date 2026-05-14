<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/Main.Master"
    CodeBehind="SurveysSelectionPage.aspx.cs" Inherits="Confirmit.CATI.Supervisor.SurveysInterviewersSelection.SurveysSelectionPage" %>
    <%@Register tagPrefix="controls" tagName="Dg" src="Controls/DoubleSurveysGrid.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="Content">
    <controls:Dialog ID="dialogControl"   runat="server" EnableViewState="true" HideHeader="True"
        Mode="Modal" PutActionButtonsInsideGridIfPossible="False">
        <OKButton Text="Save selected" OnClick="SaveSelected"></OKButton>
        <Content>
            <div class="flex-panel flex-panel-column double-surveys-grid">
                <controls:Hint runat="server" Text="<%$CPResource:SurveysSelectionPage_SelectSurveysHint%>" ID="hint" />
                <div style="flex: 1 1 auto; margin: 0px 20px 20px 20px;">
                    <controls:UpdatePanel runat="server" ChildrenAsTriggers="True" UpdateMode="Always" style="height: 100%"  >
                        <ContentTemplate>
                            <controls:dg runat="server" ID="doubleGrid" />
                        </ContentTemplate>
                    </controls:UpdatePanel>
                </div>
            </div>
        </Content>
    </controls:Dialog>
</asp:Content>
