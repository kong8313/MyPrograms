<%@ Page Language="c#" MasterPageFile="~/MasterPages/Main.Master"
CodeBehind="InterviewQuotaStatus.aspx.cs" AutoEventWireup="True" Inherits="Confirmit.CATI.Supervisor.CallManagement.InterviewQuotaStatus" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="Server">
    <style>
        .dialog-property{
            padding-left: 20px;
        }
        .modalDialog .gridHolder{
            padding: 0 20px;
        }
        .closedCell{
            font-weight: bold;
        }
    </style>
    <controls:Dialog runat="server" ID="dialog" EnableViewState="true" HideHeader="True" HideButtons="True" Mode="Modal">
        <content>
            <controls:Hint runat="server" ID="hint"/>
            <div class="dialog-property">
                <div class="dialog-property__label">
                    <asp:Label ID="lblCallStateLabel" runat="server" Text="Call State"/>
                </div>
                <div class="dialog-property__edit">
                    <asp:Label ID="lblCallStateValue" runat="server" />
                </div>
            </div>

            <Controls:Grid id="grid" runat="server" HideSelectedColumn="true" EnablePaging="false" EnableSorting="False" HideToolBar="False">
                <Columns>
                    <controls:GeneralGridColumn
                        HeaderText="Quota"
                        Key="Quota"
                        DataFieldName="QuotaName"
                        Width="120"/>
                    <controls:UnboundGeneralGridColumn
                        Header-Text="Cell"
                        Key="Cell"/>
                    <controls:GeneralGridColumn
                        HeaderText="Status"
                        Key="Status"
                        DataFieldName="IsOpen"
                        Width="70"/>
                </Columns>
            </Controls:Grid>
        </content>
    </controls:Dialog>

</asp:Content>