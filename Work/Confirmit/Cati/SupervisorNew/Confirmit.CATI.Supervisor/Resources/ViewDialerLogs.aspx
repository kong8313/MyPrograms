<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true"
    CodeBehind="ViewDialerLogs.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Resources.ViewDialerLogs" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="Server">
    <controls:Dialog runat="server" ID="dialogControl" Mode="Modal" HideHeader="true" HideButtons="True">
        <Content>
            <controls:Hint ID="dialerLogsNotAvailableHint" Text="<%$CPResource:ViewDialerLogsNotAvailableFunctionality%>" runat="server" Visible="False"/>
            <controls:Grid ID="grid" runat="server" PrimaryKeyColumn="Name" GridNameWidth="100%"
                SortedColumnName="LastWriteTimeUtc" SortIndicator="Descending" KeepSelection="True"
                HideSelectedColumn="True" OnDblClickCommand="DownloadLog" PageSize="30" GridName="<%$CPResource:Files%>">
                <Commands>
                    <controls:Command Key="DownloadLog" SelectMode="SingleRow" Caption="<%$CPResource:DownloadLog%>" OnServerClick="DownloadLog" Image="export" />
                </Commands>
                <ToolbarItems>
                    <controls:ToolbarCommandButton Key="DownloadLog" />
                </ToolbarItems>
                <DataMenuItems>
                    <controls:DataMenuItem Key="DownloadLog" />
                </DataMenuItems>

                <Columns>
                    <controls:GeneralGridColumn DataFieldName="Name" HeaderText="<%$CPResource:Name%>" 
                                                SearchColumnType="Text" Key="Name" />
                    <controls:GeneralGridColumn DataFieldName="Length" HeaderText="<%$CPResource:SizeInBytes%>"
                                                SearchColumnType="Number" Key="Length" Width="80" DataFormatString="{0:N0}" CssClass="textAlignRight"/>
                    <controls:GeneralGridColumn DataFieldName="CreationTimeUtc" HeaderText="<%$CPResource:CreationTimeUtc%>"
                                                SearchColumnType="DateTime" Key="CreationTimeUtc" Width="140" />
                    <controls:GeneralGridColumn DataFieldName="LastWriteTimeUtc" HeaderText="<%$CPResource:LastWriteTimeUtc%>"
                                                SearchColumnType="DateTime" Key="LastWriteTimeUtc" Width="140" />
                </Columns>
            </controls:Grid>
        </Content>
    </controls:Dialog>
</asp:Content>
