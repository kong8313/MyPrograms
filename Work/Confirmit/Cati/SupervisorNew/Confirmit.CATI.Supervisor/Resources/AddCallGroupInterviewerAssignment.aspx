<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true"
    Codebehind="AddCallGroupInterviewerAssignment.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Resources.AddCallGroupInterviewerAssignment" %>
    
<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="Server">
    <controls:Dialog runat="server" ID="dialogControl" Mode="Modal" HideHeader="true">
        <OKButton OnClick="OKButtonClick" Text = "<%$CPResource:AddInterviewers%>" />
        <Content>
            <controls:Grid ID="grid" runat="server" PrimaryKeyColumn="PersonSID" EnableSorting="true" SortIndicator="Ascending" 
                HintText="<%$CPResource:CallGroupInterviewerAssignmentWarning%>" GridName="Interviewers"
            >
                <Columns>
                    <controls:GeneralGridColumn HeaderText="<%$CPResource:ID%>" Key="PersonSID" SearchColumnType="Number"
                        DataFieldName="PersonSID" Width="50px" Hidden="true" />
                    <controls:GeneralGridColumn HeaderText="<%$CPResource:Name%>" Key="PersonName" SearchColumnType="Text"
                        DataFieldName="PersonName" Width="150px" SortIndicator="Ascending" />
                    <controls:GeneralGridColumn HeaderText="<%$CPResource:CallGroup%>" Key="CallGroupName" SearchColumnType="Text"
                        DataFieldName="CallGroupName" Width="150px" SortIndicator="Ascending" />
                    <controls:GeneralGridColumn HeaderText="<%$CPResource:Description%>" Key="PersonDescription" SearchColumnType="Text"
                        DataFieldName="PersonDescription" Width="100%" />                    
                </Columns>
            </controls:Grid>
        </Content>
    </controls:Dialog>
</asp:Content>
