<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/Main.Master"
    CodeBehind="QuotaProperties.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Surveys.Controls.Quota.QuotaProperties" %>

<%@ Import Namespace="Confirmit.CATI.Supervisor.Resources" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="server">
    <style>
        tr.hidden_quota td {
            color: rgb(210, 210, 210);
        }
    </style>
    <controls:Dialog ID="dialog" runat="server" Mode="Modal" HideHeader="true" PutActionButtonsInsideGridIfPossible="False">
        <OKButton OnClick="OKButtonClick" Text="Save" />
        <Content>
            <controls:UpdatePanel runat="server" ChildrenAsTriggers="True" UpdateMode="Always" style="height: 100%"  >
                <ContentTemplate>
                    <table align="left" cellspacing="0" cellpadding="0" width="100%">
                        <tr><td style="padding: 0 20px;">
                            <table class="settings-table settings-table--default-columns settings-table--no-min-width">
                                <tr>
                                    <td><asp:Label ID="QuotaColumnsLabel" runat="server" Text="<%$CPResource:QuotaColumns%>" />
                                    </td>
                                    <td>
                                        <controls:TextBox ID="tbxColumns" runat="server" Width="30px" /></td>
                                </tr>
                            </table>
                        </td></tr>
                        <tr>
                            <td width="100%" height="390px">
                                <controls:Grid ID="quotasGrid" runat="server" PrimaryKeyColumn="Name" GridNameWidth="100%" HideResetButton="True" HideRefreshButton="True"
                                    SortedColumnName="Priority" EnablePaging="False" EnableSorting="False" KeepSelection ="True" GridName="Quotas">
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
                                        <controls:GeneralGridColumn HeaderText="<%$CPResource:QuotaPageTitle%>" Key="Name"
                                            DataFieldName="Name" Width="100%" />
                                    </Columns>
                                </controls:Grid>
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </controls:UpdatePanel>
        </Content>
    </controls:Dialog>
</asp:Content>
