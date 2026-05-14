<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true"
    CodeBehind="AsyncOperationProgress.aspx.cs" Inherits="Confirmit.CATI.Supervisor.AsyncOperations.AsyncOperationProgress" %>

<%@ Import Namespace="Confirmit.CATI.Supervisor.Resources" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content" runat="server">
    <controls:Dialog ID="dialog" runat="server" HideHeader="true" Mode="Modal" HideButtons="True">
        <OKButton Visible="false" />
        <CancelButton />
        <Content>
            <main class="content-panel">
                <header>
                    <h3 class="flex-panel flex-panel-row" runat="server" ID="operationTitle">
                        <controls:SvgImage runat="server" ImageName="assignment_turned_in" style="margin-right: 10px;"></controls:SvgImage>
                        <asp:Label ID="lblOperationTitle" runat="server" Text="" CssClass="progress-bar__title" />
                    </h3>
                </header>
                <div id="divProgressBar" class="pbar">
                    <table style="width: 360px;" class="progressbar" cellpadding="0" cellspacing="0">
                        <tbody>
                            <tr>
                                <td class="progresscell" id="progresscell" style="width: 0;" />
                                <td class="progressbarbg" id="bgcell" style="width: 100%;" />
                            </tr>
                        </tbody>
                    </table>
                    <table style="width: 360px;" class="progressbarscale">
                        <tbody>
                            <tr>
                                <td style="text-align: left;">0%
                                </td>
                                <td>25%
                                </td>
                                <td style="width: 33.33%;">50%
                                </td>
                                <td>75%
                                </td>
                                <td style="text-align: right;">100%
                                </td>
                            </tr>
                        </tbody>
                    </table>

                </div>
                <div style="margin-top: 20px;" ID="divCloseOnFinish" runat="server">
                    <controls:CheckBox runat="server" ID="cbCloseOnFinish" Checked="true" Text="Close when operation finish" />
                </div>
                <div class="scrollable-container">
                    <table class="settings-table settings-table--default-columns settings-table--no-min-width">
                        <tr>
                            <td nowrap="true">
                                <%=Strings.AsyncOperationProgress_OperationId%>:
                            </td>
                            <td>
                                <asp:Label ID="lblOperationId" runat="server" Text="135" Font-Bold="true" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <%=Strings.AsyncOperationProgress_Status%>
                            </td>
                            <td>
                                <asp:Label runat="server" ID="lblStatus" Font-Bold="true" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <%=Strings.AsyncOperationProgress_StartTime%>
                            </td>
                            <td>
                                <asp:Label ID="lblStartTime" runat="server" Text="-" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <%=Strings.AsyncOperationProgress_EndTime%>
                            </td>
                            <td>
                                <asp:Label ID="lblEndTime" runat="server" Text="-" />
                            </td>
                        </tr>
                        <tr>
                            <td style="vertical-align: top">
                                <%=Strings.AsyncOperationProgress_Progress%>
                            </td>
                            <td>
                                <asp:Label ID="lblText" runat="server" Text="" />
                            </td>
                        </tr>
                    </table>
                </div>
            </main>
        </Content>
    </controls:Dialog>
    <script type="text/javascript">

        function CloseButtonHandler() {
            BeforeDialogCloseHandler();
            window.close();
        }

        function BeforeDialogCloseHandler() {
            if (AsyncOperationProgress.getCurrentState() == OperationStatus.Completed ||
                AsyncOperationProgress.getCurrentState() == OperationStatus.PartiallyCompleted) {
                top.overlay.closeLast(true);
            }
        }

    </script>
</asp:Content>
