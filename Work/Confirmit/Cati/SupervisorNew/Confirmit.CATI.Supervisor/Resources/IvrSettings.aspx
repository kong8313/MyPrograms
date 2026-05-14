<%@ Page Language="C#" MasterPageFile="~/MasterPages/RightFrameWithInfo.master" AutoEventWireup="true" 
    CodeBehind="IvrSettings.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Resources.IvrSettings" %>

<asp:Content ID="Content" ContentPlaceHolderID="RightFrameContent" runat="server">
    <script type="text/javascript">
        function openIvrStaticSettingsFrame() {
            Y.on("domready", function () {
                openAndSetInfoFrame('IvrStaticSettings.aspx');
            });
        }
    </script>

    <style type="text/css">
        tbody tr td.warning {
        color: red;
    }
    </style>

        <controls:Grid ID="m_grid" runat="server" HideSelectedColumn="false" SortedColumnName="LanguageId"
                   PrimaryKeyColumn="LanguageId" EnablePaging="true" PageSize="50" ShowFullToolbarBorders="False" OnDblClickCommand="Edit" >
        <Commands>
            <Controls:OverlayCommand Key="Add" Title="<%$CPResource:AddIvrSetting%>" Caption="<%$CPResource:AddIvrSetting%>" SelectMode="No" Image="plus" RefreshOwner="True" Width="530" Height="300" DialogMode="Create" Url="Resources/IvrSettingProperties.aspx" />
            <Controls:OverlayCommand Key="Edit" Title="<%$CPResource:EditIvrSetting%>" Caption="<%$CPResource:EditIvrSetting%>" SelectMode="SingleRow" Image="settings" RefreshOwner="True" Width="530" Height="300" DialogMode="ViewEdit" IDName="LanguageId" IDColumnName="LanguageId" Url="Resources/IvrSettingProperties.aspx" />
            <Controls:Command Key="Delete" Caption="<%$CPResource:DeleteIvrSetting%>" SelectMode="MultiRow" OnServerClick="DeleteIvrSettings" Image="delete" Confirmation="cnfr_DelIvrSetting"/>
        </Commands>
        <ToolbarItems>  
            <controls:ToolbarCommandButton Key="Add" />
            <controls:ToolbarCommandButton Key="Edit" />
            <controls:ToolbarCommandButton Key="Delete" />
        </ToolbarItems>
        <DataMenuItems>
            <controls:DataMenuItem Key="Add" />
            <controls:DataMenuItem Key="Edit" />
            <controls:DataMenuItem Key="Delete" />
        </DataMenuItems>
        <Columns>
            <controls:GeneralGridColumn DataFieldName="LanguageId" HeaderText="<%$CPResource:LanguageId%>" SearchColumnType="Number" Key="LanguageId" Width="100"  />
            <controls:GeneralGridColumn DataFieldName="LanguageDescription" HeaderText="<%$CPResource:LanguageDescription%>" SearchColumnType="Text" Key="LanguageDescription" Width="200"  />
            <controls:GeneralGridColumn DataFieldName="WrongInputAudioUrl" HeaderText="<%$CPResource:WrongInputAudioUrl%>" SearchColumnType="Text" Key="WrongInputAudioUrl" Width="100%"  />
            <controls:GeneralGridColumn DataFieldName="WrongInputText" HeaderText="<%$CPResource:WrongInputText%>" SearchColumnType="Text" Key="WrongInputText" Width="100%"  />
            <controls:GeneralGridColumn DataFieldName="WrongInputExitAudioUrl" HeaderText="<%$CPResource:WrongInputExitAudioUrl%>" SearchColumnType="Text" Key="WrongInputExitAudioUrl" Width="100%"   />
            <controls:GeneralGridColumn DataFieldName="WrongInputExitText" HeaderText="<%$CPResource:WrongInputExitText%>" SearchColumnType="Text" Key="WrongInputExitText" Width="100%"   />
        </Columns>
    </controls:Grid>
</asp:Content>