<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true"
    CodeBehind="AddCallGroupStatuses.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Resources.AddCallGroupStatuses" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="server">
       <controls:Dialog ID="dialog" Mode="Modal" runat="server" HideHeader="true">
        <OKButton OnClick="SaveHandler" ResName = "AddStatuses" />
        <Content>
            <Controls:Grid id="grid" runat="server" PrimaryKeyColumn="Key" EnableSorting="true" SortIndicator="Ascending"  HideSelectedColumn="false" EnablePaging="false" 
                HintText="<%$CPResource:CallGroupAddStatusesWarning%>" GridName="Statuses">
				<Columns>
					<controls:GeneralGridColumn 
						HeaderTextId="ID"
						Key="Key" 
						DataFieldName="Key"
						Hidden="false"
						Width="77px" />
					<controls:GeneralGridColumn 
						HeaderTextId="StatusName" 
						Key="Value" 
						DataFieldName="Value"
						Width="100%" />					
				</Columns>
			</Controls:Grid>
                                                       
        </Content>
    </controls:Dialog>
        
</asp:Content>