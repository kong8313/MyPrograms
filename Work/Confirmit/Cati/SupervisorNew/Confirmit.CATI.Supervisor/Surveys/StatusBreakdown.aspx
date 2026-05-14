<%@ Page Language="c#" MasterPageFile="~/MasterPages/Main.Master"
    CodeBehind="StatusBreakdown.aspx.cs" AutoEventWireup="True" Inherits="Confirmit.CATI.Supervisor.Surveys.StatusBreakdown" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="Server">
    <controls:Dialog runat="server" ID="dialogControl" EnableViewState="true" HideHeader="True" HideButtons="True">
        <content>
			<Controls:Grid id="grid" runat="server" PrimaryKeyColumn="ItsId" EnableSorting="true" SortIndicator="Ascending"  HideSelectedColumn=true EnablePaging="false">
				<Columns>
					<controls:GeneralGridColumn 
						HeaderTextId="ID"
						Key="ItsId" 
						DataFieldName="ItsId"
						Hidden="false"
						Width="77px" />
					<controls:GeneralGridColumn 
						HeaderTextId="Status" 
						Key="StatusName" 
						DataFieldName="StatusName"
						Width="100%" />
					<controls:GeneralGridColumn
						HeaderTextId="Count"
						Key="Count" 
						DataFieldName="Count" 
						Width=120px />					
				</Columns>
			</Controls:Grid>
		</content>
    </controls:Dialog>
</asp:Content>
