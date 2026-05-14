<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/Main.Master"
    CodeBehind="AudioPlayer.aspx.cs" Inherits="Confirmit.CATI.Supervisor.CallManagement.AudioPlayer" %>

<%@ Register Src="~/Controls/HierarchicalGridEx.ascx" TagName="HierarchicalGridEx"
    TagPrefix="controls" %>
<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="server">

    <script type="text/javascript">
        function playRecording(url, resetVolume) {
            if ($get("html5audio")) {
                if (resetVolume) {
                    $get("html5audio").volume = 0.5;
                }
                
                $get("html5audio").setAttribute("src", url);
            }
            else {
                $get('mediaPlayer').URL = url;
            }
        }
    </script>

    <controls:Dialog runat="server" ID="dialogControl" Mode="Floating" HideButtons="True">
        <OKButton Visible="false" />
        <CancelButton onclick="window.close()" />
        <Content>
            <div id="ActivexPlayer" runat="server">
                <%--This class ID is for Windows Media Player 7 and later.--%>
                <object id="mediaPlayer" width="100%" height="44" classid="CLSID:6BF52A52-394A-11d3-B153-00C04F79FAA6">
                    <param name="autoStart" value="True">
                    <param name="URL" value="">
                    <param name="enabled" value="True">
                    <param name="balance" value="0">
                    <param name="currentPosition" value="0">
                    <param name="enableContextMenu" value="False">
                    <param name="fullScreen" value="False">
                    <param name="mute" value="False">
                    <param name="playCount" value="1">
                    <param name="rate" value="1">
                    <param name="stretchToFit" value="False">
                    <param name="uiMode" value="full">
                </object>
            </div>
            <div id="Html5Player" runat="server" style="width: 100%; padding-top: 1px">
                <audio style="width: 100%;" id="html5audio" controls autoplay>
                </audio>
            </div>
            <div style="height: calc(100vh - 95px); overflow: auto;">
                <controls:HierarchicalGridEx ID="m_grid" runat="server" CssClass="generic-grid" GridLines="Both"
                                             HideToggleColumn="true" >
                    <HeaderStyle CssClass="header" Wrap="false" />
                    <RowStyle CssClass="row" Wrap="false" />
                    <AlternatingRowStyle CssClass="altrow" Wrap="false" />
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <controls:ImageButton ID="ibPlay" runat="server" OnClientClick="" AlternateText="<%$CPResource:Play%>"
                                                 CausesValidation="false" ImageName="play_circle" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="DateTime" HeaderText="<%$CPResource:RecordingTime%>" />
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:Label runat="server" Text="<%$CPResource:Link%>"/>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:HyperLink runat="server" ID="lbDownload"/>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </controls:HierarchicalGridEx>
            </div>
        </Content>
    </controls:Dialog>
</asp:Content>
