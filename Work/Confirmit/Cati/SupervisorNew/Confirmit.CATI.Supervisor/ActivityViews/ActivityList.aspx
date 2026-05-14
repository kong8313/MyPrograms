<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true"
    Codebehind="ActivityList.aspx.cs" Inherits="Confirmit.CATI.Supervisor.ActivityViews.ActivityList" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="server">
    <controls:Grid ID="grid" runat="server" EnableSorting="false"
        EnablePaging="false" HideRefreshButton="true"
        HideSelectedColumn="true" ShowFullToolbarBorders="False" PrimaryKeyColumn="Key">
        <Columns>
            <controls:GeneralGridColumn DataFieldName="Key" HeaderText="Key" Key="Key"
                Width="20" Hidden="true"/>
            <controls:GeneralGridColumn HeaderText="Name" Key="Name" DataFieldName="Name"
                Width="250" />
            <controls:GeneralGridColumn HeaderText="Description" Key="Description"
                DataFieldName="Description" Width="100%" />
        </Columns>
    </controls:Grid>
</asp:Content>
