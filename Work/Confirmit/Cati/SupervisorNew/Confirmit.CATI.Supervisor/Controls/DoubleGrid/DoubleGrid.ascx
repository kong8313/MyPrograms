<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DoubleGrid.ascx.cs"
    Inherits="Confirmit.CATI.Supervisor.Controls.DoubleGrid.DoubleGrid" %>
    
<div class="double-grid--container">
    <div class="double-grid--leftPane">
        <asp:PlaceHolder runat="server" ID="phLeftGrid"></asp:PlaceHolder>
    </div>
    <div class="double-grid--middlePane">
        <div class="flex-panel flex-panel-column">
            <controls:ImageButton ID="btnAdd" runat="server" ImageName="add_selected" CssClass="button-style" />
            <div id="divAdd" class="div-text">Add</div>
            <controls:ImageButton ID="btnRemove" runat="server" ImageName="remove_selected" CssClass="button-style"  />
            <div id="divRemove" class="div-text">Remove</div>
            <controls:ImageButton ID="btnRemoveAll" runat="server" ImageName="remove_all" CssClass="button-style"  />
            <asp:Label id="labelRemoveAll" class="div-text" runat="server" Text="Remove All" />
        </div>
    </div>
    <div class="double-grid--rightPane">
        <asp:PlaceHolder runat="server" ID="phRightGrid"></asp:PlaceHolder>
    </div>
</div>
<style>

    .div-text{
        align-self: center;
        color: gray;
        margin-bottom: 20px;
    }
    
    .button-style{
        align-self: center;
    }
    
    .double-grid--container{
        height: 100%;
        display: flex;
        flex-direction: row;
    }
    
    .double-grid--leftPane,
    .double-grid--rightPane {
        border: 1px solid #e2e3e4;
        flex: 1;
    }
    
    .double-grid--middlePane{
        width: 85px;
        display: flex;
        align-items: center;
        justify-content: center;
    }
    
    .double-grid--container .general-grid-control__header-toolbar .XpMenu {
        margin-top: 0;
    }

    .double-grid--container .gridHolder {
        padding: 0;
    }

    .double-grid--container .bottom-status-bar {
        padding-top: 10px;
        margin-top: 0;
        padding-left: 5px;
    }
    
</style>
