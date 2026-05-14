<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/Main.Master"
    Codebehind="ChangeLimit.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Surveys.ChangeLimit" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="server">
    <controls:dialog id="dialog" runat="server" mode="Modal" hideheader="true">
	<OKButton OnClick="OKButtonClick" Text="Change limit" />
		<Content>
            <main class="content-panel">
			    <table class="settings-table settings-table--default-columns settings-table--no-min-width">
			        <tr>
					    <td colspan="2">
						    <asp:Label ID="lblSelectLimit" Text="<%$CPResource:SelectNewLimit%>" runat="server" />
					    </td>
				    </tr>
				    <tr>
					    <td>
						    <asp:Label ID="lblLimit" runat="server" Text="<%$CPResource:QuotaLimit%>" />
					    </td>
					    <td>
						    <controls:NumericEdit ID="wneLimit" runat="server" Width="105" Nullable="False"
							    ValueText="0" MinValue="0">
							    <Buttons SpinButtonsDisplay="OnRight"></Buttons>
						    </controls:NumericEdit>
					    </td>
				    </tr>
			    </table>
            </main>
		</Content>
	</controls:dialog>
</asp:Content>
