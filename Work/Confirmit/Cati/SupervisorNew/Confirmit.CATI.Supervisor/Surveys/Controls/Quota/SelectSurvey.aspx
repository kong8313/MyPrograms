<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true"
    CodeBehind="SelectSurvey.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Surveys.Controls.Quota.SelectSurvey" %>

<%@ Import Namespace="Confirmit.CATI.Supervisor.Resources" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="Server">
    <controls:Dialog runat="server" ID="dialogControl" Mode="Modal" HideHeader="true">
        <OKButton OnClick="SelectSurveyButtonClick" Text="Select Survey"/>
        <Content>
            <controls:UpdatePanel runat="server" ChildrenAsTriggers="True" UpdateMode="Always" style="height: 100%"  >
                <ContentTemplate>
                    <controls:Grid ID="surveyListGrid" runat="server" PrimaryKeyColumn="Id" GridNameWidth="100%" HasMultySelectionCheckBox="false" TopToolbarLayout="DoubleMenu"
                        HideSelectedColumn="true" OnDblClickCommand="SelectSurvey" SortedColumnName="DefaultOrderID"  SortIndicator="Descending">
                        <Commands>
                            <controls:Command Key="SelectSurvey" OnServerClick="SelectSurveyButtonClick" />
                        </Commands>
                        <LeftToolbarItems>
                            <controls:CheckBox ID="cbRecent" runat="server" AutoPostBack="True" Text="<%$CPResource:Recent%>" Checked="True"/>
                       </LeftToolbarItems>
                        <Columns>
                            <controls:GeneralGridColumn HeaderText="ID" Key="Id" SearchColumnType="Number" DataFieldName="Id"
                                Width="50" Hidden="True" />
                            <controls:GeneralGridColumn HeaderText="DefaultOrderID" Key="DefaultOrderID" SearchColumnType="Number" DataFieldName="DefaultOrderID"
                                                        Width="50" Hidden="True" />
                            <controls:GeneralGridColumn HeaderText="<%$CPResource:ProjectId%>" Key="ConfirmitID" SearchColumnType="Text"
                                DataFieldName="ConfirmitID" Width="100px" SortIndicator="Ascending" />
                            <controls:GeneralGridColumn HeaderText="<%$CPResource:ProjectName%>" Key="Name" SearchColumnType="Text"
                                DataFieldName="Name" Width="100%" />
                        </Columns>
                    </controls:Grid>
                </ContentTemplate>
            </controls:UpdatePanel>
        </Content>
    </controls:Dialog>
</asp:Content>
