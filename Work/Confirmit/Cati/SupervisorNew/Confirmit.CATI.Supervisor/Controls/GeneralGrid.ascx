<%@ Control Language="c#" AutoEventWireup="True" CodeBehind="GeneralGrid.ascx.cs"
    Inherits="Confirmit.CATI.Supervisor.Controls.GeneralGrid" %>
<%@ Register TagPrefix="controls" TagName="Hint" Src="~/Controls/Hint.ascx" %>
<%@ Register TagPrefix="controls" TagName="GridToolbar" Src="~/Controls/GeneralToolbar.ascx" %>
<input type="hidden" runat="server" id="hSelected" name="hSelected" />
<input type="hidden" runat="server" id="hHighlighted" name="hHighlighted" />
<input type="hidden" runat="server" id="hTotalCount" name="hTotalCount" />
<input type="hidden" runat="server" id="hSortColumnKey" name="hSortColumnKey" />
<input type="hidden" runat="server" id="hDateValues" />

<div id="<%=ClientID %>" class="general-grid-control <%=CssClass %>">
    <div class="general-grid-control__header">
        <div id="trTopTitle" runat="server" class="general-grid-control__header-title">
            <asp:Label runat="server" ID="topTitle" />
            <div runat="server" class="general-grid-control__header-links" ID="links">
            </div>
        </div>
        <div id="trHint" runat="server" class="general-grid-control__header-hint">
            <controls:Hint ID="gridHint" runat="server" />
        </div>
        <div runat="server" id="topToolbarRow" class="general-grid-control__header-toolbar">
            <controls:GridToolbar runat="server" ID="topToolbar" />
        </div>
    </div>
    <div runat="server" id="gridHolder" visible="true" class="gridHolder">

        <controls:DataGrid runat="server" ID="dataGrid" EnableViewState="False" EnableAjax="False"
            TabIndex="-1" Height="100%" Width="100%">

            <EmptyRowsTemplate>
                <div style="margin-top: 20px; text-align: center; font-weight: bold; position: absolute; left: 0px; right: 0px;">
                    <%=HttpUtility.HtmlEncode(GetResString(NoDataMessage))%>
                </div>
            </EmptyRowsTemplate>
            <Behaviors>
                <iggrid:ColumnResizing Enabled="True" />
                <iggrid:Selection RowSelectType="Single" Enabled="true" EnableCrossPageSelection="True"
                    CellClickAction="Row">
                </iggrid:Selection>
            </Behaviors>
            <Columns>
            </Columns>
        </controls:DataGrid>
    </div>
    <div class="general-grid-control__alternative hidden" runat="server" ID="divAlternative">
        <asp:PlaceHolder ID="phAlternativeControls" runat="server" />
    </div>
    <div class="XpMenu clearfix bottom-status-bar" id="statusBarDiv">
        <div id="labelDiv" class="total-records-count" runat="server">
            <asp:Label ID="lblRecordCount" runat="server" Text="" CssClass="boldLabel" />
        </div>
        <div runat="server" class="total-records-count-extra" id="extraInfoDiv">
            <asp:Label runat="server" ID="lblExtraInfo" CssClass="boldLabel" />
        </div>
        <div runat="server" id="rightMenuDiv">
            <controls:XpMenu ID="rightMenu" runat="server" BorderWidth="0">
                <controls:XpMenuItem runat="server" ImageName="fast_rewind" ID="btnTopPage"
                    TextId="FirstPage" />
                <controls:XpMenuItem runat="server" ImageName="prev" ID="btnPrevPage"
                    TextId="PrevPage" />
                <controls:XpMenuItem runat="server" ButtonType="Generic">
                    <asp:Panel runat="server" CssClass="pages-of-counter">
                        <controls:NumericEdit ID="wnePageIndex" runat="server" Nullable="False" MaxValue="1" autocomplete="off"
                            MinValue="1" Height="16px">
                            <AutoPostBackFlags EnterKeyDown="On"></AutoPostBackFlags>
                        </controls:NumericEdit>
                        <asp:Label runat="server" Text="/" />
                        <asp:Label runat="server" ID="lblPageCount" />
                    </asp:Panel>
                </controls:XpMenuItem>
                <controls:XpMenuItem runat="server" ImageName="next" ID="btnNextPage"
                    TextId="NextPage" />
                <controls:XpMenuItem runat="server" ImageName="fast_forward" ID="btnBottomPage"
                    TextId="LastPage" />
            </controls:XpMenu>
        </div>
        <div class="general-grid-control__dialog-buttons" runat="server" ID="divDialogButtons">
            <asp:PlaceHolder ID="phDialogButtons" runat="server" />
        </div>

    </div>
</div>
<asp:PlaceHolder runat="server" ID="menuPlaceholder"></asp:PlaceHolder>
<controls:DataMenu runat="server" ID="gridContextMenu" EnableViewState="False">
    <Items>
    </Items>
</controls:DataMenu>
