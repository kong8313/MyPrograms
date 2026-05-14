<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/Main.Master"
    CodeBehind="SelectConfirmitVariables.aspx.cs" Inherits="Confirmit.CATI.Supervisor.CallManagement.SelectConfirmitVariables" %>
    
<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
        <script type="text/javascript">
            function okButtonClicked() {
                var maxSelectedRows = <%= MaxSelectedRows %>;
                var grid_id = "<%=m_grid.ClientID%>";
                var hSelected = document.getElementById(grid_id + "_hSelected");
                var selected = hSelected.value;

                if (selected.split(",").length > maxSelectedRows) {
                    alert('<%=String.Format(Confirmit.CATI.Supervisor.Resources.Strings.MaxSelectedRowsAlert, MaxSelectedRows) %>');
                    return false;
                }
                else {
                    return true;
                }
            }
    </script>
</asp:Content>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="server">
    <controls:Dialog runat="server" ID="dialog" HideHeader="True" Mode="Modal">
        <okbutton onclientclick="if(!okButtonClicked()) return false;" Text="Save" />
        <content>
            <Controls:Grid id="m_grid" GridName="Select questions to add" HintText="<%$CPResource:SelectVariablesHint %>" NoDataMessage="NoCatiVariables" runat="server" PrimaryKeyColumn="Key" Width="100%" EnablePaging="false">
	            <Columns>
	                <controls:GeneralGridColumn Key="Key" DataFieldName="Key" Hidden="true" />
			        <controls:GeneralGridColumn HeaderText="<%$CPResource:Name%>" Key="Name" DataFieldName="Name" Width="100%" />
			        <controls:GeneralGridColumn HeaderText="<%$CPResource:QuestionType%>" Key="ConfirmitVariableType" DataFieldName="ConfirmitVariableTypeLocalizedString" Width="150" />
		        </Columns>
	        </Controls:Grid>
		</content>
    </controls:Dialog>
</asp:Content>

