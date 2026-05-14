<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="QuotaClusteringParameters.aspx.cs"
    MasterPageFile="~/MasterPages/Main.Master" Inherits="Confirmit.CATI.Supervisor.Surveys.QuotaClusteringParameters" %>

<%@ Import Namespace="Confirmit.CATI.Supervisor.Resources" %>
<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="server">
    <style>
        .leftPaddedTd {
            padding-left: 11px;
        }
    </style>
    <controls:Dialog ID="dialog" runat="server" Mode="Modal" HideHeader="true">
        <OKButton Text="Save" OnClick="OkButtonClicked" />
        <Content>
            <main class="content-panel">
                <controls:Hint ID="hintQuotaClustering" Text="<%$CPResource:HintQuotaClustering%>"
                    runat="server" />
                <table class="settings-table settings-table--default-columns settings-table--no-min-width">
                    <tr>
                        <td nowrap="nowrap">
                            <%=Strings.QuotaForClustering%>
                        </td>
                        <td class="leftPaddedTd">
                            <controls:DropDownList ID="ddlQuotas" runat="server">
                            </controls:DropDownList>
                        </td>
                        <td>
                            <controls:HelpTextViewer runat="server" ID="helpQuotaForClustering" HelpTextId="HelpQuotaForClustering"
                                TitleTextId="QuotaForClustering"></controls:HelpTextViewer>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblThreshold" Text="<%$CPResource:ClusterQuotaThreshold%>" runat="server"/>
                        </td>
                        <td class="leftPaddedTd">
                            <controls:NumericEdit ID="neThreshold" runat="server" Nullable="False" ValueText="10"
                                MinValue="1" MaxValue="100">
                                <Buttons SpinButtonsDisplay="OnRight"></Buttons>
                            </controls:NumericEdit>
                        </td>
                        <td>
                            <controls:HelpTextViewer runat="server" ID="helpThreshold" HelpTextId="HelpClusterQuotaThreshold"
                                TitleTextId="ThresholdTitle"></controls:HelpTextViewer>
                        </td>
                    </tr>
                </table>
            </main>
        </Content>
    </controls:Dialog>
</asp:Content>
