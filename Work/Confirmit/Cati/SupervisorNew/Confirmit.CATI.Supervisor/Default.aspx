<%@ Page Language="c#" CodeBehind="Default.aspx.cs" AutoEventWireup="True" Inherits="Confirmit.CATI.Supervisor.Main"
    MasterPageFile="~/MasterPages/Main.Master" %>

<%@ Register TagPrefix="Controls" TagName="MainHeader" Src="~/Controls/MainHeader.ascx" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        html, body, .mainForm, #listFrame, #infoFrame
        {
            height: 100%;
            width: 100%;
            padding: 0px;
            margin: 0px;
            border: 0px;
        }
        #container
        {
            height: 100%;
            width: 100%;
            overflow: hidden;
        }
        #header
        {
            width: 100%;
            height: 45px;
            position: fixed;
            border-bottom: solid 1px #3A6EA5;
        }
    </style>
    <script type="text/javascript">

        window.name = "catiWindow";
        window.topCatiWindow = true;
        window.listSplitterInitialized = false;
        var blankPageUrl = 'about:blank';

        function setSessionFrameUrl(url) {
            document.getElementById("sessionFrame").src = url;
        }       
        function setTitle(text) {
            document.getElementById("TopTitle").innerHTML = text;
        }
        
        function getInfoFrame() {
            if (document.getElementById("listFrame").contentWindow.getInfoFrame)
                return document.getElementById("listFrame").contentWindow.getInfoFrame();
            else return null;
        }
        function openInfoFrame() {
            if (document.getElementById("listFrame").contentWindow.openInfoFrame)
                document.getElementById("listFrame").contentWindow.openInfoFrame();
        }
        function setInfoFrameUrl(url) {
            if (document.getElementById("listFrame").contentWindow.setInfoFrameUrl)
            {
                document.getElementById("listFrame").contentWindow.setInfoFrameUrl(url);
            }
        }
        function openAndSetInfoFrame(url)
        {
            openInfoFrame();
            setInfoFrameUrl(url);
        }
        function refreshListFrame() {
            if (document.getElementById("listFrame").contentWindow.refreshListFrame)
            //async refresh
                document.getElementById("listFrame").contentWindow.refreshListFrame();
            else {                
                document.getElementById("listFrame").src += "";
            }
        }
        function refreshInfoFrame() {
            if (document.getElementById("listFrame").contentWindow.refreshInfoFrame)
                document.getElementById("listFrame").contentWindow.refreshInfoFrame();
        }
        function closeInfoFrame() {
            if (document.getElementById("listFrame").contentWindow.closeInfoFrame)
                document.getElementById("listFrame").contentWindow.closeInfoFrame();
        }
        function closeAndClearInfoFrame()
        {
            try
            {
                closeInfoFrame();
                setInfoFrameUrl(blankPageUrl);
            }
            catch(e)
            {}
        }
       
        function setListFrameUrl(url) {
            document.getElementById("listFrame").src = url;
        }

        function openNewsDialog() {
            var settings = { width: "1006", height:"800", top:"40" };
            top.overlay.show("News", "News/NewsDialog.aspx", null, settings, null);
        }

        Y.on("domready", function () {
            window.wm = new WindowManager();
        });
    </script>
</asp:Content>
<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="server">
    <div id="container">
        <div id="header">
            <Controls:MainHeader runat="server" ID="mainHeader" />
        </div>
        <div style="height: 100%; padding-top: 44px;">
            <ig_layout:WebSplitter runat="server" ID="manuSplitter" Orientation="Vertical" Height="100%"
                Width="100%" DynamicResize="true" ResizeWithBrowser="true" EnableViewState="False">
                <Panes>
                    <ig_layout:SplitterPane runat="server" Size="100%" CollapsedDirection="None" ScrollBars="Hidden">
                        <Template>
                            <iframe src="about:blank" id="listFrame" frameborder="0"></iframe>
                        </Template>
                    </ig_layout:SplitterPane>
                </Panes>
            </ig_layout:WebSplitter>
        </div>
    </div>
    <iframe src="about:blank" id="sessionFrame" style="display: none"></iframe>
</asp:Content>
