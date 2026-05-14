<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MainHeader.ascx.cs"
    Inherits="Confirmit.CATI.Supervisor.Controls.MainHeader" %>
<%@ Import Namespace="Confirmit.CATI.Supervisor.Resources" %>
<%@ Register TagPrefix="Controls" TagName="CallCenterSwitch" Src="~/Controls/CallCenterSwitch.ascx" %>

<style>
    #headerContainer
    {
        height: 44px;
        background: linear-gradient(to bottom, #2152AD, #739CD6);
        filter: progid:DXImageTransform.Microsoft.gradient(startColorstr='#2152AD', endColorstr='#739CD6'); /* for IE */
        background: -webkit-gradient(linear, left top, left bottom, from(#2152AD), to(#739CD6)); /* for webkit browsers */
        background: -moz-linear-gradient(top,  #2152AD,  #739CD6); /* for firefox 3.6+ */
        background: -ms-linear-gradient(top,  #2152AD,  #739CD6); /* for IE9+ */
        border-bottom: 1px solid #739CD6;
    }
</style>
<div id="headerContainer">
    <div style="float: left; padding-left: 3px; color: white; white-space: nowrap">
        <div style="font-weight: bold; font-size: 16px; padding-top: 4px;">
            <%=Strings.DefPageTitle %>
        </div>
        <div id="TopTitle" style="font-weight: bold; margin-top: 4px">
        </div>
    </div>
    <div style="float: right; padding-right: 3px;">
       
        <div style="float: left; margin-right: 15px; height: 44px">
            <Controls:CallCenterSwitch runat="server" />
        </div>

        <img src="images/confirmit-logo-prof.png" style="height: 35px; width: 142px; margin-top: 5px" />
    </div>
</div>
