<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="StateProperties.aspx.cs"
    Inherits="Confirmit.CATI.Supervisor.Resources.StateProperties" %>

<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
    <controls:Dialog ID="dialog" runat="server" Mode="Modal" HideHeader="true">
        <OKButton OnClick="OKButtonClick" ResName="ChangeButtonText" />
        <Content>
            <main class="content-panel">
                <table class="settings-table settings-table--default-columns settings-table--no-min-width">
                    <tr>
                        <td>
                            <asp:Label ID="lblID" Text="<%$CPResource:StateID%>" runat="server" />
                        </td>
                        <td>
                            <controls:TextBox ID="tbxID" runat="server" columnKey="ID" Width="100%" Enabled="false" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblAAPOR" Text="<%$CPResource:AaporCode%>" runat="server" />
                            <controls:TextFieldValidator ID="tfxvAAPOR" ControlToValidate="tbxAAPOR"
                                ValidationErrorMessage="Err_IncorrectAAPOR" Text="*" ValidInputExpression="^(?=.{1,10}$)(?=[^.]*\.[^.]*$)\d{1,8}\.\d{1,8}$" runat="server" />
                        </td>
                        <td>
                            <controls:TextBox ID="tbxAAPOR" runat="server" columnKey="AaporCode" Width="100%" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblName" Text="<%$CPResource:Name%>" runat="server" />
                            <controls:TextFieldValidator ID="tfxvLogin" ControlToValidate="tbxName"
                                IsRequired="true" FieldRequredErrorMessage="Err_EmptyName" ValidationErrorMessage="Err_EmptyName" Text="*" runat="server" />
                        </td>
                        <td>
                            <controls:TextBox ID="tbxName" runat="server" Width="100%" MaxLength="255" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblPriority" Text="<%$CPResource:Priority%>" runat="server" />
                        </td>
                        <td style="display: flex">
                            <controls:NumericEdit ID="tbxPriority" runat="server" Nullable="False" columnKey="Priority"
                                Width="100%" HorizontalAlign="left" MinValue="1" DataMode="Int">
                            </controls:NumericEdit>
                            <controls:HelpTextViewer ID="hvPriority" runat="server" HelpTextId="StatePriorityHelpText" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblDA" Text="<%$CPResource:DA%>" runat="server" />
                        </td>
                        <td>
                            <controls:CheckBox ID="cbDA" runat="server" columnkey="DisallowActivation" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblFcdAction" Text="<%$CPResource:FcdAction%>" runat="server" />
                        </td>
                        <td>
                            <controls:CheckBox runat="server" ID="cbFcdAction" columnkey="FcdAction" />
                        </td>
                    </tr>
                </table>
            </main>
        </Content>
    </controls:Dialog>
</asp:Content>
