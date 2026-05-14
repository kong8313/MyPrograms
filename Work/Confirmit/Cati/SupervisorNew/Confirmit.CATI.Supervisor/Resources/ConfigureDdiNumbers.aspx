<%@ Page Language="C#" MasterPageFile="~/MasterPages/RightFrameWithInfo.master" AutoEventWireup="true" 
    CodeBehind="ConfigureDdiNumbers.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Resources.ConfigureDdiNumbers" %>

<asp:Content ID="Content" ContentPlaceHolderID="RightFrameContent" runat="server">
    <style type="text/css">
        tbody tr td.warning {
        color: red;
    }
    </style>

        <controls:Grid ID="m_grid" runat="server" HideSelectedColumn="false" SortedColumnName="TelephoneNumber"
                   PrimaryKeyColumn="TelephoneNumber" EnablePaging="true" PageSize="50" ShowFullToolbarBorders="False" OnDblClickCommand="Edit" >
        <Commands>
            <Controls:ViewCommand Key="ViewDefaultMessages" Caption="<%$CPResource:DdiNumbersDefaultRecordedMessagesTooltip%>" Url="Resources/DdiNumbersRecordedMessages.aspx" Image="message" SelectMode="No" />
            <Controls:OverlayCommand Key="Add" Title="<%$CPResource:AddDDINumber%>" Caption="<%$CPResource:AddDDINumber%>" SelectMode="No" Image="plus" RefreshOwner="True" Width="430" Height="180" DialogMode="Create" Url="Resources/DdiNumberProperties.aspx" ShowInCurrentFrame="True" />
            <Controls:OverlayCommand Key="Edit" Title="<%$CPResource:EditDDINumber%>" Caption="<%$CPResource:EditDDINumber%>" SelectMode="SingleRow" Image="edit" RefreshOwner="True" Width="430" Height="180" DialogMode="ViewEdit" IDName="TelephoneNumber" IDColumnName="TelephoneNumber" Url="Resources/DdiNumberProperties.aspx" ShowInCurrentFrame="True" />
            <Controls:ViewCommand Key="ViewTelephoneSpecificMessages" Caption="<%$CPResource:DdiNumbersTelephoneSpecificRecordedMessagesTooltip%>" SelectMode="SingleRow" Url="Resources/DdiNumbersRecordedMessages.aspx" Image="message" IDName="TelephoneNumber" IDColumnName="TelephoneNumber"  />
            <Controls:Command Key="Delete" Caption="<%$CPResource:DeleteDDINumber%>" SelectMode="MultiRow" OnServerClick="DeleteDdiNumbers" Image="delete" Confirmation="cnfr_DelDDINumber"/>
        </Commands>
        <ToolbarItems>  
            <controls:ToolbarCommandButton Key="ViewDefaultMessages" />
            <controls:ToolbarCommandButton Key="Add" />
            <controls:ToolbarCommandButton Key="Edit" />
            <controls:ToolbarCommandButton Key="Delete" />
        </ToolbarItems>
        <DataMenuItems>
            <controls:DataMenuItem Key="Add" />
            <controls:DataMenuItem Key="Edit" />
            <controls:DataMenuItem Key="Delete" />
            <controls:DataMenuItem Key="ViewTelephoneSpecificMessages" />
        </DataMenuItems>
        <Columns>
            <controls:GeneralGridColumn DataFieldName="TelephoneNumber" HeaderText="<%$CPResource:TelephoneNumber%>" SearchColumnType="Text" Key="TelephoneNumber" Width="100%"  />
            <controls:GeneralGridColumn DataFieldName="SurveyId" HeaderText="<%$CPResource:SurveyID%>" SearchColumnType="Text" Key="SurveyId" Width="100%"  />
            <controls:GeneralGridColumn DataFieldName="SurveyName" HeaderText="<%$CPResource:ProjectName%>" SearchColumnType="Text" Key="SurveyName" Width="100%"  />
            <controls:GeneralGridColumn DataFieldName="DialerName" HeaderText="<%$CPResource:DialerName%>" SearchColumnType="Text" Key="DialerName" Width="100%"  />
            <controls:GeneralGridColumn DataFieldName="DialerId" HeaderText="<%$CPResource:DialerId%>" SearchColumnType="Text" Key="DialerId" Width="100%"   />
            <controls:GeneralGridColumn DataFieldName="HasOverridingMessages" HeaderText="<%$CPResource:HasOverridingMessages%>" SearchColumnType="Text" Key="HasOverridingMessages" Width="150"   />
        </Columns>
    </controls:Grid>
</asp:Content>