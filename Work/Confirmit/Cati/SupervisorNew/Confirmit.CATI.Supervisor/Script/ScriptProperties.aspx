<%@ Page Language="c#" MasterPageFile="~/MasterPages/Main.Master"
    CodeBehind="ScriptProperties.aspx.cs" AutoEventWireup="True" Inherits="Confirmit.CATI.Supervisor.Script.ScriptProperties" %>

<%@ Register TagPrefix="Controls" TagName="ScriptInfoGeneral" Src="~/Script/Controls/ScriptInfo.General.ascx" %>
<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="Server">
    <controls:Dialog runat="server" id="dialogControl" HideHeader="True" Mode="Modal">
        <okbutton onclick="SaveButtonClick" Text="Save"/>
        <content>
            <main class="content-panel">
			<Controls:ScriptInfoGeneral runat="server" ID="pGeneral"/>
            </main>
	    </content>
    </controls:Dialog>
</asp:Content>
