<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true"
    CodeBehind="ErrorPage.aspx.cs" Inherits="Confirmit.CATI.Supervisor.ErrorPage" %>

<%@ Import Namespace="Confirmit.CATI.Supervisor.Resources" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content" runat="server">
    <div class="co-error">
        <div class="co-error__content">
            <h1 class="co-error__heading">Sorry about that...</h1>
            <p class="co-error__text">
                <label id="errorMessage" runat="server" />
            </p>
            <div class="co-error__link-block">
                
            </div>
        </div>
        <div class="co-error__controls">
            <input type="button" class="plain_button" id="btnTryAgain" value="<%=Strings.TryAgain %>" onclick="javascript: location.reload();">&nbsp;&nbsp;&nbsp;
            <input type="button" class="plain_button" id="btnGoBack" value="<%=Strings.GoBack %>" onclick="javascript: history.go(-1);">
        </div>
    </div>
</asp:Content>
