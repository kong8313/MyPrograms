<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true"
    CodeBehind="Export.aspx.cs" Inherits="Confirmit.CATI.Supervisor.CallManagement.Export" %>

<%@ Import Namespace="Confirmit.CATI.Supervisor.Resources" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="server">
    <controls:Dialog ID="dialog" runat="server" HideHeader="true" Mode="Modal">
        <OKButton OnClick="OnExportClick" Text="Export" />
        <Content>
            <main class="content-panel">
                <script language="javascript" type="text/javascript">
                    function pageLoad(sender, args) {
                        radioButtonClicked();
                    }

                    function numericEditValueChange(oEdit, oldValue, oEvent) {
                        calcPagesCalls();
                    }

                    function radioButtonClicked() {

                        var rbRangePages = document.getElementById("<%=rbRangePages.ClientID%>");
                        var neStart = $IG.WebTextEditor.find("<%=neStart.ClientID%>");
                        var neEnd = $IG.WebTextEditor.find("<%=neEnd.ClientID%>");;
                        if (rbRangePages.checked) {
                            neStart.set_enabled(true);
                            neEnd.set_enabled(true);
                        }
                        else {
                            neStart.set_enabled(false);
                            neEnd.set_enabled(false);

                        }
                        calcPagesCalls();
                    }

                    function calcPagesCalls() {
                        var pages = 0;
                        var calls = 0;
                        var rbCurrentPage = document.getElementById("<%=rbCurrentPage.ClientID%>");
                        var rbAllPages = document.getElementById("<%=rbAllPages.ClientID%>");
                        var rbRangePages = document.getElementById("<%=rbRangePages.ClientID%>");
                        var pageIndex = parseInt(document.getElementById("<%=pageIndex.ClientID%>").value);
                        var pageSize = parseInt(document.getElementById("<%=pageSize.ClientID%>").value);
                        var totalCount = parseInt(document.getElementById("<%=totalCount.ClientID%>").value);

                        if (rbCurrentPage.checked) {
                            pages = 1;
                            if (pageIndex * pageSize > totalCount) {
                                // contains last page
                                calls = totalCount - (pageIndex - 1) * pageSize;
                            }
                            else {
                                calls = pageSize;
                            }
                        }
                        else if (rbAllPages.checked) {
                            pages = Math.ceil(totalCount / pageSize);
                            calls = totalCount;
                        }
                        else if (rbRangePages.checked) {
                            var startIndex = $IG.WebTextEditor.find("<%=neStart.ClientID%>").get_number();
                            var endIndex = $IG.WebTextEditor.find("<%=neEnd.ClientID%>").get_number();
                            if (startIndex <= endIndex) {
                                if (endIndex * pageSize > totalCount) {
                                    // contains last page
                                    calls = totalCount - (startIndex - 1) * pageSize;
                                }
                                else {
                                    calls = (endIndex - startIndex + 1) * pageSize;
                                }
                                pages = endIndex - startIndex + 1;
                            }
                        }

                        document.getElementById("<%=lbSelectedPages.ClientID%>").innerHTML = pages;
                        document.getElementById("<%=lbSelectedCalls.ClientID%>").innerHTML = calls;
                        document.getElementById("<%=currentCallsCount.ClientID%>").value = calls;
                    }
                </script>

                <input runat="server" type="hidden" id="totalCount" value="0" />
                <input runat="server" type="hidden" id="pageIndex" value="0" />
                <input runat="server" type="hidden" id="pageSize" value="0" />
                <input runat="server" type="hidden" id="currentCallsCount" value="0" />
                <table class="settings-table--default-columns settings-table settings-table--no-min-width">
                    <tr>
                        <td colspan="2">
                            <asp:Label runat="server" ID="lbSelectPage" Text="<%$CPResource:ExportSelectPage%>"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <div class="flex-panel">
                                <controls:RadioButton ID="rbCurrentPage" Checked="true" runat="server" GroupName="SelectPages"
                                    Text=" " AutoPostBack="false" />
                                <%= Strings.ExportCurrentPage%>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <div class="flex-panel">
                                <controls:RadioButton ID="rbAllPages" Checked="false" runat="server" GroupName="SelectPages"
                                    Text=" " AutoPostBack="false" />
                                <%=Strings.ExportAllPages %>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <div class="flex-panel">
                                <controls:RadioButton ID="rbRangePages" Checked="false" runat="server" GroupName="SelectPages"
                                    Text=" " AutoPostBack="false" />
                                <%=Strings.ExportRangeOfPages %>
                            </div>
                        </td>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Label runat="server" ID="lbStartRange" Text="<%$CPResource:ExportRangeFrom%>"></asp:Label>
                                    </td>
                                    <td>
                                        <controls:NumericEdit runat="server" ID="neStart" MinValue="1" ValueText="1" Enabled="false">
                                            <ClientEvents ValueChanged="numericEditValueChange" />
                                            <Buttons SpinButtonsDisplay="OnRight"></Buttons>
                                        </controls:NumericEdit>
                                    </td>
                                    <td>
                                        <asp:Label runat="server" ID="lbTo" Text="<%$CPResource:ExportRangeTo%>"></asp:Label>
                                    </td>
                                    <td>
                                        <controls:NumericEdit runat="server" ID="neEnd" MinValue="1" ValueText="1" Enabled="false">
                                            <ClientEvents ValueChanged="numericEditValueChange" />
                                            <Buttons SpinButtonsDisplay="OnRight"></Buttons>
                                        </controls:NumericEdit>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <table class="settings-table--default-columns settings-table settings-table--no-min-width">
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="lbSelectedPagesTitle" Text="<%$CPResource:ExportSelectedPages%>"></asp:Label>
                        </td>
                        <td>
                            <asp:Label runat="server" ID="lbSelectedPages">1</asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="lbSelectedCallsTitle" Text="<%$CPResource:ExportSelectedCalls%>"></asp:Label>
                        </td>
                        <td>
                            <asp:Label runat="server" ID="lbSelectedCalls">10</asp:Label>
                        </td>
                    </tr>
                </table>
            </main>
        </Content>
    </controls:Dialog>
</asp:Content>
