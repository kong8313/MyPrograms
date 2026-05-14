<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/Main.Master" CodeBehind="ActivityColumnSettings.aspx.cs" Inherits="Confirmit.CATI.Supervisor.ActivityViews.Controls.SurveyActivityColumnSettings" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="server">
    <controls:Dialog ID="dialog" runat="server" HideHeader="true" Mode="Modal" PutActionButtonsInsideGridIfPossible="False">
        <OKButton OnClick="SaveButtonClick" Text="Save" />
        <Content>
            <div class="content-panel">
                <div class="content-panel__scroll-pane">
                    <asp:GridView ID="grid" runat="server" AutoGenerateColumns="false" Width="100%" AllowSorting="false" DataKeyNames="Key" CssClass="generic-grid generic-grid--no-borders">
                        <RowStyle CssClass="row" />
                        <Columns>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <controls:CheckBox runat="server" ID="cbIsActive" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="<%$CPResource:ColumnName%>">
                                <ItemTemplate>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </Content>
    </controls:Dialog>
</asp:Content>
