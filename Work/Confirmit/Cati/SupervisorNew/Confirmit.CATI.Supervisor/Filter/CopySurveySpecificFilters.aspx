<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CopySurveySpecificFilters.aspx.cs"
    Inherits="Confirmit.CATI.Supervisor.Filter.CopySurveySpecificFilters" MasterPageFile="~/MasterPages/Main.Master" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="Server">
    <script type="text/javascript">
        function confirmMove() {
            if (!checkSelection()) {
                return false;
            }

            if (confirm('Are you sure you want to move all of the survey specific filters? They will no longer be available in the selected survey.')) {
                return true;
            }
            else {
                return false;
            }
        }

        function confirmCopy() {
            if (!checkSelection()) {
                return false;
            }

            return true;
        }

        function checkSelection() {
            var row = <%=m_grid.ClientGetCurrentRow()%>;
            if (row == null) {
                alert("Please select a survey.");
                return false;
            }

            return true;
        }
    </script>
    <controls:Dialog runat="server" ID="dialogControl" Mode="Modal" HideHeader="true" ShowSaveButton="True">
        <OKButton Text="Move filters" OnClientClick="if(!confirmMove()) return;" OnClick="moveButton_Click" />
        <SaveButton Text="Copy filters" OnClientClick="if(!confirmCopy()) return;" OnClick="copyButton_Click" />
        <Content>
            <controls:Grid ID="m_grid" runat="server" PrimaryKeyColumn="SurveySid" SortedColumnName="SurveySid"
                SortIndicator="Descending" GridName="Select survey to copy/move filters from"
                HideSelectedColumn="true">
                <Columns>
                    <controls:GeneralGridColumn HeaderText="SurveyId" Key="SurveySid" DataFieldName="SurveySid"
                        Hidden="True" />
                    <controls:GeneralGridColumn HeaderTextId="ProjectId" Key="ProjectID" DataFieldName="ProjectID"
                        SearchColumnType="Text" Width="120" />
                    <controls:GeneralGridColumn HeaderTextId="ProjectName" Key="ProjectName" DataFieldName="ProjectName"
                        SearchColumnType="Text" Width="100%" />
                    <controls:GeneralGridColumn HeaderText="Count of survey specific filters" Key="FiltersCount"
                        DataFieldName="FiltersCount" SearchColumnType="Number" Width="200" />
                </Columns>
            </controls:Grid>
        </Content>
    </controls:Dialog>
</asp:Content>
