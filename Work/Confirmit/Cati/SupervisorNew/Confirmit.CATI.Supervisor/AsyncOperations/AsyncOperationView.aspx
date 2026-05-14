<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true"
    CodeBehind="AsyncOperationView.aspx.cs" Inherits="Confirmit.CATI.Supervisor.AsyncOperations.AsyncOperationView" %>
    
    
<asp:Content ID="Content" runat="server" ContentPlaceHolderID="Content" >
    <script type="text/javascript">
        function SelectedIndexChanged(sender, args) {
            PageMethods.SetSelectedTab(sender.getTabAt(args.get_tabIndex()).get_key());
        }

    </script>
    <controls:Dialog runat="server" Mode="Frame" ID="dialog1" HideHeader="true" HideButtons="true">
        <Content>
            <controls:Tabs runat="server" ID="tabs" EnableViewState="False" style="height: 100%; width: 100%">
                <ClientEvents SelectedIndexChanged="SelectedIndexChanged"/>
                <PostBackOptions EnableLoadOnDemandUrl="True"  />
                <Tabs>
                    <controls:TabItem runat="server" TextId="Progress" Key="tabProgress" ContentUrl="AsyncOperationProgress.aspx"/>
                    <controls:TabItem runat="server" TextId="Parameters" Key="tabParameters" ContentUrl="AsyncOperationParameters.aspx" />                    
                    <controls:TabItem runat="server" TextId="Specific Parameters" Key="tabSpecificParameters" ContentUrl="AsyncOperationSpecificParameters.aspx" />                    
                </Tabs>
            </controls:Tabs>
        </Content>
    </controls:Dialog>
</asp:Content>

