<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="ChangeLocation.aspx.cs"
    Inherits="Confirmit.CATI.Supervisor.Persons.ChangeLocation" %>

<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
    <controls:Dialog ID="dialog" runat="server" Mode="Modal" HideHeader="true">
        <OKButton OnClick="OKButtonClick" ResName="ChangeButtonText" />
        <Content>
            <main class="content-panel">
                <table class="settings-table settings-table--default-columns settings-table--no-min-width">
                    <tr>
                        <td>
                            <asp:Label ID="lblLocation" runat="server" Text="<%$CPResource:Location%>" />
                            <controls:TextFieldValidator ID="tfxvLocation" ControlToValidate="tbLocation"
                                IsRequired="false"
                                ValidationErrorMessage="ErrorIncorrectValue" Text="*" runat="server" />
                        </td>
                        <td>
                            <controls:TextBox ID="tbLocation" runat="server" AutoPostBack="False" Width="100%" MaxLength="255" /></td>
                    </tr>
                </table>
            </main>
        </Content>
    </controls:Dialog>
</asp:Content>
