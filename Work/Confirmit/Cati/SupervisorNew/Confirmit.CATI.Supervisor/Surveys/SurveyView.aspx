<%@ Page AutoEventWireup="true" CodeBehind="SurveyView.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Surveys.SurveyView"
    Language="C#" MasterPageFile="~/MasterPages/Main.Master" %>
<%@ Import Namespace="Confirmit.CATI.Supervisor.Classes" %>

<asp:Content ID="Content" runat="server" ContentPlaceHolderID="Content">
    <script type="text/javascript">
        function SelectedIndexChanged(sender, args) {
            PageMethods.SetSelectedTab(sender.getTabAt(args.get_tabIndex()).get_key());
        }
    </script>
    <controls:Dialog runat="server" Mode="Frame" ID="dialog" HideHeader="true" HideButtons="true">
        <Content>
            <controls:Tabs runat="server" ID="tabs" EnableViewState="False" style="height: 100%; width: 100%">
                <ClientEvents SelectedIndexChanged="SelectedIndexChanged"/>
                <PostBackOptions EnableLoadOnDemandUrl="True"  />
                <Tabs>
                    <controls:TabItem runat="server" TextId="SrvTabs_General" Key="General" ContentUrl="SurveyViewTabs/SurveyViewGeneral.aspx"/>
                    <controls:TabItem runat="server" TextId="SrvTabs_Summary" Key="Summary" ContentUrl="SurveyViewTabs/SurveyViewSummary.aspx" />
                    <controls:TabItem runat="server" TextId="Assignment" Key="Assignment" ContentUrl="SurveyViewTabs/SurveyViewAssignment.aspx" />
                    <controls:TabItem runat="server" TextId="Quotas" Key="Quotas" ContentUrl="SurveyViewTabs/SurveyViewQuotas.aspx"  />
                    <controls:TabItem runat="server" TextId="InterviewerSearchTabName" Key="AvailableFieldsInConsole"
                        ContentUrl="SurveyViewTabs/SurveyViewAvailableFieldsInConsole.aspx" />
                    <controls:TabItem runat="server" TextId="SrvTabs_SchedulingParams" Key="SchedulingParams"
                        ContentUrl="SurveyViewTabs/SurveyViewSchedulingParams.aspx" />
                    <controls:TabItem runat="server" TextId="AdvancedFilters" Key="Filters" ContentUrl="SurveyViewTabs/SurveyViewFilters.aspx" />
                    <controls:TabItem runat="server" TextId="DialerSettings" Key="DialerSettings" ContentUrl="SurveyViewTabs/SurveyViewDialerSettings.aspx" />
                </Tabs>
            </controls:Tabs>
        </Content>
    </controls:Dialog>
</asp:Content>
