<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true"
    CodeBehind="DialerFeaturesView.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Resources.DialerFeaturesView" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="Server">
    <controls:Dialog runat="server" ID="dialogControl" Mode="Modal" HideHeader="true" HideButtons="True">
        <Content>
            <controls:Grid ID="grid" runat="server" PrimaryKeyColumn="Name" GridNameWidth="100%" EnableSorting="False" SortedColumnKey="Id"
                           KeepSelection="True" HideSelectedColumn="True" OnDblClickCommand="OverrideDefaultValue" EnablePaging="False"
                           GridName="<%$CPResource:Features%>">
                
                <Columns>
                    <controls:GeneralGridColumn DataFieldName="Id" HeaderText="<%$CPResource:Id%>" 
                                                Key="Id" Hidden="True" />
                    <controls:GeneralGridColumn DataFieldName="Name" HeaderText="<%$CPResource:Name%>" 
                                                SearchColumnType="Text" Key="Name" Width="100%" />
                    <controls:GeneralGridColumn DataFieldName="DefaultValue" HeaderText="<%$CPResource:DialerValue%>"
                                                SearchColumnType="Text" Key="DefaultValue" Width="110"/>
                    <controls:GeneralGridColumn DataFieldName="OverridenValue" HeaderText="<%$CPResource:OverridenValue%>"
                                                SearchColumnType="Text" Key="OverridenValue" Width="110" />
                </Columns>

                <Commands>
                    <controls:OverlayCommand Key="OverrideDefaultValue" RefreshOwner="true"
                                             Title="<%$CPResource:OverrideDefaultValue%>" 
                                             Caption="<%$CPResource:OverrideDefaultValue%>"
                                             Url="Resources/DialerFeatureEdit.aspx" 
                                             IDColumnName="Name" IDName="Name" 
                                             Image="edit" 
                                             Width="450" Top="120" Height="240" 
                                             SelectMode="SingleRow" ShowInCurrentFrame="True" 
                                             InlineParams="" />
                    <controls:Command Key="DeleteOverriddenValue" Caption="<%$CPResource:DeleteOverriddenValue%>"
                                      SelectMode="SingleRow" OnServerClick="DeleteOverriddenValue"
                                      Confirmation="cnfr_DeleteOverriddenValue" Image="remove" />
                </Commands>
                <ToolbarItems>
                    <controls:ToolbarCommandButton Key="OverrideDefaultValue" />
                    <controls:ToolbarCommandButton Key="DeleteOverriddenValue" />
                </ToolbarItems>
                <DataMenuItems>
                    <controls:DataMenuItem Key="OverrideDefaultValue" />
                    <controls:DataMenuItem Key="DeleteOverriddenValue" />
                </DataMenuItems>

            </controls:Grid>
        </Content>
    </controls:Dialog>
</asp:Content>
