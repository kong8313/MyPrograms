<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true"
    CodeBehind="PriorityGroupView.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Resources.PriorityGroupView" %>

<%@ Register TagPrefix="controls" TagName="CallGroupStatuses" Src="Controls/CallGroupStatuses.ascx" %>
<%@ Register TagPrefix="controls" TagName="CallGroupInterviewers" Src="Controls/CallGroupInterviewers.ascx" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="server">
    <script type="text/javascript">
        function SelectedIndexChangedHandler(sender, args) {
            PageMethods.SetSelectedTab(sender.getTabAt(args.get_tabIndex()).get_key());
        }
    </script>
    <controls:Tabs runat="server" ID="tabs" Style="height: 100%; width: 100%" >        
        <Tabs>
            <controls:TabItem runat="server" TextId="Extended Statuses" Key="States">
                <Template>
                    <controls:CallGroupStatuses ID="statusesList" runat="server" />
                </Template>
            </controls:TabItem>
            <controls:TabItem runat="server" TextId="Interviewers" Key="Interviewers">
                <Template>
                    <controls:CallGroupInterviewers ID="interviewersList" runat="server" />
                </Template>
            </controls:TabItem>                    
        </Tabs>
    </controls:Tabs>
        
</asp:Content>
