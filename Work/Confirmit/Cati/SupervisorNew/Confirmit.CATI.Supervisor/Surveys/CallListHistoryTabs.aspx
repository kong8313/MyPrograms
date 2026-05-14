<%@ Page Language="C#"   MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true" 
    CodeBehind="CallListHistoryTabs.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Surveys.CallListHistoryTabs" %>

  
<asp:Content ID="Content" runat="server" ContentPlaceHolderID="Content" >
    <script type="text/javascript">
        function SelectedIndexChanged(sender, args) {
            PageMethods.SetSelectedTab(sender.getTabAt(args.get_tabIndex()).get_key());
        }

    </script>
    <controls:Dialog runat="server" Mode="Modal" ID="dialog1" HideHeader="true" HideButtons="true">
        <OKButton Visible="False"></OKButton>
        <Content>
            <controls:Tabs runat="server" ID="tabs" EnableViewState="False" style="height: 100%; width: 100%">
                <ClientEvents SelectedIndexChanged="SelectedIndexChanged"/>
                <PostBackOptions EnableLoadOnDemandUrl="True"  />
                <Tabs>
                    <controls:TabItem runat="server" TextId="CallAttempts" Key="tabCallAttempts" ContentUrl="CallListHistory.aspx"/>
                    <controls:TabItem runat="server" TextId="CallExtendedHistory" Key="tabCallExtendedHistory" ContentUrl="CallExtendedHistory.aspx" />                    
                    <controls:TabItem runat="server" TextId="CallHistoryLoop" Key="tabCallHistoryLoop" ContentUrl="SurveyHistoryLoop.aspx" />   
                    <controls:TabItem runat="server" Text="Scheduling Execution Log" Key="tabSchedulingLog" ContentUrl="SchedulingLog.aspx" />                    
                </Tabs>
            </controls:Tabs>
        </Content>
    </controls:Dialog>
</asp:Content>