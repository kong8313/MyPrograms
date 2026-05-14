<%@ Page Language="c#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="LinkedInterviewChain.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Surveys.LinkedInterviewChain" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="Server">
    <style type="text/css">
        tbody tr.CurrentInterview {
            color:crimson;
        }
    </style>

    <controls:Dialog runat="server" ID="dialog" EnableViewState="true" HideHeader="True" HideButtons="True" Mode="Modal">
        <okbutton visible="false" />
        <content>
    		<Controls:Grid id="grid" HintText ="<%$CPResource:LinkedInterviewsHint %>" runat="server" PrimaryKeyColumn="InterviewsOrder" HideSelectedColumn="true" EnablePaging="false" EnableSorting="False">
				<Columns>
					<controls:GeneralGridColumn
						HeaderText="<%$CPResource:Order%>" 
						Key="InterviewsOrder" 
						DataFieldName="InterviewsOrder"
						Width="50"/>
					<controls:GeneralGridColumn
						HeaderText="<%$CPResource:SurveyID%>" 
						Key="SurveyId" 
						DataFieldName="SurveyId" 
						Width="126"
                        Hidden="True"/>
					<controls:GeneralGridColumn
						HeaderText="<%$CPResource:ProjectId%>" 
						Key="ProjectId" 
						DataFieldName="ProjectId"
						Width="100"/>
				    <controls:GeneralGridColumn
				        HeaderText="<%$CPResource:ProjectName%>" 
				        Key="SurveyName" 
				        DataFieldName="SurveyName"
				        Width="126"/>
					<controls:GeneralGridColumn
						HeaderText="<%$CPResource:InterviewId%>" 
						Key="InterviewId" 
						DataFieldName="InterviewId" 
						Width="135"/>
					<controls:GeneralGridColumn
						HeaderText="LinkedInterviewSessionId" 
						Key="LinkedInterviewSessionId" 
						DataFieldName="LinkedInterviewSessionId" 
						Width="100"
                        Hidden="True"/>
				</Columns>
			</Controls:Grid>
		</content>
    </controls:Dialog>

</asp:Content>
