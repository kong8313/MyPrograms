<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SrvInfoQuotas.ascx.cs"
            Inherits="Confirmit.CATI.Supervisor.Surveys.Controls.SrvInfoQuotas" %>
<%@ Register TagPrefix="Controls" TagName="SingleQuotaBoard" Src="Quota/SingleQuotaBoard.ascx" %>
<%@ Register TagPrefix="Controls" TagName="AllQuotasBoard" Src="Quota/AllQuotasBoard.ascx" %>
<div class="tab-content">
<Controls:SingleQuotaBoard runat="server" ID="SingleQuotaBoard" Visible="True"/>
<Controls:AllQuotasBoard runat="server" ID="AllQuotasBoard" Visible="False"/>
    </div>