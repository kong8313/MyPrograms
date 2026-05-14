<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="ExternalTransferProperties.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Resources.ExternalTransferProperties" %>

<%@ Register TagPrefix="controls" TagName="Dg" Src="../SurveysInterviewersSelection/Controls/DoubleSurveysGrid.ascx" %>
<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="server">
    <controls:Dialog ID="dialog" Mode="Modal" runat="server" HideHeader="true" PutActionButtonsInsideGridIfPossible="False">
        <OKButton OnClick="OKButtonClick" CausesValidation="True" runat="server" />
        <Content>
            <div class="flex-panel flex-panel-column double-surveys-grid">
                <controls:UpdatePanel runat="server" ID="controlsPanel" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="flex-panel flex-panel-row flex-panel-row--justify" style="margin: 0px 20px;">
                            <asp:Label ID="lblTelephoneNumber" Text="<%$CPResource:TelNumber%>" runat="server" />
                            <controls:TextFieldValidator ID="telephoneValidator" ControlToValidate="tbTelephoneNumber" 
                                IsRequired="true" FieldRequredErrorMessage="Err_EmptyTelephoneNumber" ValidationErrorMessage="ErrorIncorrectValue"
                                Text="*" runat="server" ValidInputExpression="^[0-9]{1,255}$" />
                            <controls:TextBox ID="tbTelephoneNumber" runat="server" Style="width: 240px;" />
                            <asp:Label ID="lblDescription" Text="<%$CPResource:Description%>" runat="server" />
                            <controls:TextFieldValidator ID="descriptionValidator" ControlToValidate="tbDescription"
                                IsRequired="true" FieldRequredErrorMessage="Err_EmptyDescription"
                                Text="*" runat="server" />
                            <controls:TextBox ID="tbDescription" runat="server" Style="width: 240px;" />
                        </div>
                        <div class="flex-panel flex-panel-row" style="margin: 10px 20px 0 20px;">
                            <controls:CheckBox ID="cbIsHidden" runat="server" />
                            <asp:Label ID="lblIsHidden" AssociatedControlId="cbIsHidden" Text="<%$CPResource:HideTelephoneNumber%>" runat="server" />
                        </div>
                    </ContentTemplate>
                </controls:UpdatePanel>

                <div style="flex: 1 1 auto; margin: 20px;">
                    <controls:UpdatePanel runat="server" ChildrenAsTriggers="True" UpdateMode="Always" style="height: 100%">
                        <ContentTemplate>
                            <controls:Dg runat="server" ID="doubleGrid" />
                        </ContentTemplate>
                    </controls:UpdatePanel>
                </div>
            </div>
        </Content>
    </controls:Dialog>
</asp:Content>
