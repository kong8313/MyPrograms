<%@ Control Language="c#" AutoEventWireup="True" CodeBehind="CustomTimezoneAddControl.ascx.cs"
            Inherits="Confirmit.CATI.Supervisor.CustomTimezone.Controls.CustomTimezoneAdd" %>

<%@ Import Namespace="Confirmit.CATI.Supervisor.Resources" %>
<style type="text/css">
    </style>
<controls:Dialog runat="server" ID="dialogControl" EnableViewState="true" HideHeader="True" Mode="Modal" ShowBottomBorder="True">
    <OKButton runat="server" OnClick="BtnSave_ServerClick" />
    <Content>
        <main class="content-panel flex-panel flex-panel-column">
            <div>
                <table id="inputs" class="settings-table">
                    <tr>
                        <td>
                            <%=Strings.Name %>
                            <controls:TextFieldValidator ID="tfvName" ControlToValidate="EdtName" IsRequired="true"
                                                         FieldRequredErrorMessage="Err_EmptyName" ValidationErrorMessage="ErrorIncorrectValue"
                                                         Text="*" runat="server" />
                        </td>
                        <td>
                            <controls:TextBox ID="EdtName" runat="server" Width="100%" MaxLength="255" />
                        </td>
                    </tr>
                </table>
            </div>
        </main>
    </Content>
</controls:Dialog>>
