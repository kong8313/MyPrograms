<%@ Page Language="C#" MasterPageFile="~/MasterPages/RightFrameWithInfo.Master" AutoEventWireup="true"
    CodeBehind="DbUpdateLog.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Resources.DbUpdateLog" %>

<asp:Content ID="Content" ContentPlaceHolderID="RightFrameContent" runat="Server">
    <controls:Grid ID="m_grid" runat="server" OnDblClickCommand="ViewDetails" HideSelectedColumn="True" GridName="<%$CPResource:DbUpdateLogs%>"
                   PrimaryKeyColumn="DbLogId" ShowFullToolbarBorders="False">
        <Columns>                         
            <controls:GeneralGridColumn HeaderText="<%$CPResource:ID%>" Key="DbLogId" SearchColumnType="Number"
                                        DataFieldName="DbLogId" Width="80" />
            <controls:GeneralGridColumn HeaderText="<%$CPResource:ScriptVersion%>" Key="ScriptVersion"
                                        SearchColumnType="Text" SearchColumnName="ScriptVersion" Width="110" />
            <controls:GeneralGridColumn HeaderText="<%$CPResource:Description%>" Key="Description"
                                        SearchColumnType="Text" SearchColumnName="Description" Width="100%" />
            <controls:GeneralGridColumn HeaderText="<%$CPResource:ScriptAppliedDate%>" Key="ScriptAppliedDate"
                                        DataFieldName="ScriptAppliedDate" SearchColumnType="DateTime" Width="140" />
            <controls:GeneralGridColumn HeaderText="<%$CPResource:Duration%>" Key="Duration"
                                        DataFieldName="Duration" SearchColumnType="Number" Width="90" />
            <controls:GeneralGridColumn HeaderText="<%$CPResource:IsAppliedDuringDBCreation%>" Key="IsAppliedDuringDBCreation"
                                        SearchColumnType="TextDropDown" SearchColumnName="IsAppliedDuringDBCreation" Width="130" />
            <controls:GeneralGridColumn HeaderText="<%$CPResource:DbUpdateUtilityVersion%>" Key="DbUpateUtilityVersion" SearchColumnType="Text"
                                        DataFieldName="DbUpateUtilityVersion" Width="90" />
            <controls:GeneralGridColumn HeaderText="<%$CPResource:ActiveUser%>" Key="ActiveUser"
                                        SearchColumnType="Text" Width="100" DataFieldName="ActiveUser" />
        </Columns>
        <Commands>
            <controls:OverlayCommand Key="ViewDetails" RefreshListFrame="true" Title="<%$CPResource:Details%>" Caption="<%$CPResource:Details%>" URL="Resources/DbUpdateLogProperties.aspx"
                                     IDColumnName="DbLogId" IDName="DbLogId" Image="view" Width="800" Top="20" Height="700" SelectMode="SingleRow" RefreshInfoFrame="True" />
        </Commands>
        <ToolbarItems>
            <controls:ToolbarCommandButton Key="ViewDetails" />
        </ToolbarItems>
        <DataMenuItems>
            <controls:DataMenuItem Key="ViewDetails" />
        </DataMenuItems>
    </controls:Grid>
</asp:Content>
