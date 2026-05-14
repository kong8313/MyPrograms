<%@ Page AutoEventWireup="true" CodeBehind="CallsPromotionHistory.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Surveys.CallsPromotionHistory"
    Language="C#" MasterPageFile="~/MasterPages/Main.Master" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="Server">
    <style>
        tbody tr.Alert td {
            background-color: rgb(255, 150, 125) !important;
        }
    </style>
    <controls:Dialog runat="server" ID="dialog" HideButtons="True">
        <OKButton Visible="false" />
        <CancelButton text="Close" />
        <Content>
            <controls:Grid ID="grid" runat="server" IncludeGridName="false" HideSelectedColumn="true" EnablePaging="false"
                RightToolbarButtons="CloseWindow" HideResetButton="true" TopToolbarLayout="DoubleMenu" PrimaryKeyColumn="FiredTime">
                <LeftToolbarItems>
                    <controls:XpMenuItem runat="server" ButtonType="Generic" Style="white-space: nowrap">
                        <table cellpadding="0" cellspacing="0">
                            <tr>
                                <td>
                                    <asp:Label runat="server" ID="lblPeriod" Text="<%$CPResource:PeriodHeader%>" Width="50" />
                                </td>
                                <td>
                                    <controls:DateTimeRangeSelect ID="dtrsDates" runat="server" AutoPostBack="false" />
                                </td>
                            </tr>
                        </table>
                    </controls:XpMenuItem>
                </LeftToolbarItems>
                <Columns>
                    <controls:GeneralGridColumn HeaderText="<%$CPResource:PromotionDateTime%>" Key="FiredTime" DataFieldName="FiredTime" Width="180" />
                    <controls:GeneralGridColumn HeaderText="<%$CPResource:NumberOfCallsToPromote%>" Key="CallsToPromoteCount" DataFieldName="CallsToPromoteCount" />
                    <controls:GeneralGridColumn HeaderText="<%$CPResource:NumberOfPromotedCalls%>" Key="PromotedCallsCount" DataFieldName="PromotedCallsCount" />
                    <controls:GeneralGridColumn HeaderText="<%$CPResource:CellInfo%>" Key="CellInfo" DataFieldName="CellInfo" />
                    <controls:GeneralGridColumn HeaderText="<%$CPResource:QuotaName%>" Key="QuotaName" DataFieldName="QuotaName" />
                    <controls:GeneralGridColumn HeaderText="<%$CPResource:CellID%>" Key="CellId" DataFieldName="CellId" />
                </Columns>
            </controls:Grid>
        </Content>
    </controls:Dialog>
</asp:Content>
