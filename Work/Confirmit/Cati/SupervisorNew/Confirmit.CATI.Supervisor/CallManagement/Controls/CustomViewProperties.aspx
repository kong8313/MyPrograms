<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/Main.Master" ValidateRequest="false"
    CodeBehind="CustomViewProperties.aspx.cs" Inherits="Confirmit.CATI.Supervisor.CallManagement.Controls.CustomViewProperties" %>

<%@ Import Namespace="Confirmit.CATI.Supervisor.Resources" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="server">
    <style>
        tr.hidden_quota td {
            color: rgb(210, 210, 210);
        }
    </style>
    <controls:Dialog ID="dialog" runat="server" Mode="Modal" HideHeader="true">
        <OKButton OnClick="OKButtonClick" Text="Save" />
        <Content>
            <controls:UpdatePanel runat="server" ChildrenAsTriggers="True" UpdateMode="Always" style="height: 100%">
                <ContentTemplate>
                    <div class="flex-panel flex-panel-column" style="height: 100%;">
                        <table align="left" cellspacing="0" cellpadding="0" width="100%">
                            <tr>
                                <td style="padding-top: 10px;">
                                    <table width="60%">
                                        <tr>
                                            <td style="padding-left: 15px;">
                                                <asp:Label runat="server" Text="<%$CPResource:Name%>" Font-Bold="true" />
                                            </td>
                                            <td>
                                                <controls:TextBox ID="tbxCusomViewName" runat="server" Width="150px" MaxLength="20" />
                                            </td>
                                            <td style="text-align: right">
                                                <controls:CheckBox ID="cbxIsDefault" runat="server" Text="<%$CPResource:SetAsDefault%>" Font-Bold="True" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>&nbsp;
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <div class="flex-panel--all-awailable-space" style="height: 0px;">
                        <controls:Grid ID="columnNamesGrid" runat="server" PrimaryKeyColumn="Name" GridNameWidth="100%" HideRefreshButton="True" HideResetButton="True"
                            SortedColumnName="Priority" EnablePaging="False" EnableSorting="False" KeepSelection="True">
                            <Commands>
                                <controls:Command Key="MoveUp" Caption="MoveUp" Image="expand_less" OnServerClick="MoveUp" SelectMode="SingleRow" />
                                <controls:Command Key="MoveDown" Caption="MoveDown" Image="expand_more" OnServerClick="MoveDown" SelectMode="SingleRow" />
                            </Commands>
                            <ToolbarItems>
                                <controls:ToolbarCommandButton Key="MoveUp" />
                                <controls:ToolbarCommandButton Key="MoveDown" />
                            </ToolbarItems>
                            <DataMenuItems>
                                <controls:DataMenuItem Key="MoveUp" />
                                <controls:DataMenuItem Key="MoveDown" />
                            </DataMenuItems>
                            <Columns>
                                <controls:GeneralGridColumn HeaderText="Priority" Key="Priority" DataFieldName="Priority"
                                    Width="20" Hidden="True" />
                                <controls:GeneralGridColumn HeaderText="<%$CPResource:ColumnName%>" Key="Name"
                                    DataFieldName="Name" Width="100%" />
                                <controls:GeneralGridColumn HeaderText="Key" Key="Key" DataFieldName="Key"
                                    Width="20" Hidden="True" />
                            </Columns>
                        </controls:Grid>
                        </div>
                    </div>
                </ContentTemplate>
            </controls:UpdatePanel>
        </Content>
    </controls:Dialog>
</asp:Content>
