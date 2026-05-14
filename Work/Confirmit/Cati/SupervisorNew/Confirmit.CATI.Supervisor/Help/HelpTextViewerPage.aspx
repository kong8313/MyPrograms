<%@ Page Language="c#" Inherits="Confirmit.CATI.Supervisor.Help.HelpTextViewerPage"
    CodeBehind="HelpTextViewerPage.aspx.cs" AutoEventWireup="true" MasterPageFile="~/MasterPages/Main.Master" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="server">
    <style>
        .divWithAutoScrolling
        {
            width: 100%;
            height: 100%;
            position: absolute;
            top: 0;
            bottom: 0;
            left: 0;
            right: 0;
            overflow: auto;
        }
        .pagetitle
        {
            font-weight: bold;
        }
    </style>
    <div class="divWithAutoScrolling">
        <div style="padding: 7px;">
            <span runat="server" id="pagetitle" class="pagetitle"></span>
            <br>
            <br>
            <span runat="server" id="helpSpan" class="plain_text"></span>
        </div>
    </div>
</asp:Content>
