<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CustomScriptPage.ascx.cs"
    Inherits="Confirmit.CATI.Supervisor.Script.Controls.CustomScriptPage" %>

<%@ Register TagPrefix="Controls" TagName="CodeEditor" Src="~/Script/Controls/CodeEditor.ascx" %>

<div class="flex-panel flex-panel-column" style="height: 100%; overflow: hidden;">
    <controls:GeneralToolbar ID="toolBar" runat="server" LeftLabel="Custom script">
        <RightMenuItems>
            <controls:XpMenuItem ID="btnReference" AutoPostBack="false" ImageName="help"
                runat="server" Text="Reference Guide" />
            <controls:XpMenuItem runat="server" ButtonType="Separator" />
            <controls:XpMenuItem TextId="SaveAndLaunch" ImageName="play_circle" OnClick="LaunchClick"
                runat="server" ID="btnLaunch" />
            <controls:XpMenuItem TextId="Save" ImageName="save" OnClick="SaveClick" runat="server"
                ID="btnSave" />
        </RightMenuItems>
    </controls:GeneralToolbar>
    <div style="flex: 1 1 auto; padding: 0 10px 10px 10px; height: 150px">
        <controls:CodeEditor runat="server" ID="codeEditor" LargeScriptFeatures="True" />
    </div>
</div>

<asp:PlaceHolder runat="server" ID="placeholder"></asp:PlaceHolder>
