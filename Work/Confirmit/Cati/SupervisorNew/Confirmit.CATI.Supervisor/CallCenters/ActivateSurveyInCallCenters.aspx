<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true"
    CodeBehind="ActivateSurveyInCallCenters.aspx.cs" Inherits="Confirmit.CATI.Supervisor.CallCenters.ActivateSurveyInCallCenters" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="server">
    <controls:Dialog runat="server" ID="dialogControl" HideHeader="True" PutActionButtonsInsideGridIfPossible="False" Mode="Modal">
        <OKButton OnClick="Activate" Text="<%$CPResource:ActivateInCallCenter%>" />
        <Content>
            <div class="flex-panel flex-panel-column double-general-grid">
                <controls:Hint ID="_hint" runat="server" Text="<%$CPResource:ActivateSurveyInCallCenterHint%>" />
                <div style="flex: 1 1 auto; margin: 0px 20px 20px 20px;">
                    <controls:UpdatePanel ID="_updatePanel" runat="server" ChildrenAsTriggers="True"
                        UpdateMode="Always" style="height: 100%;">
                        <ContentTemplate>
                            <controls:DoubleGrid runat="server" ID="_doubleGrid">
                                <AddButton OnClick="AddCallCenters" />
                                <RemoveButton OnClick="RemoveCallCenters" />
                                <LeftGridContent>
                                    <controls:Grid ID="_allCallCentersGrid" runat="server" GridName="<%$CPResource:AllCallCenters%>"
                                        PrimaryKeyColumn="Id" OnDblClickCommand="Add" EnablePaging="False">
                                        <Commands>
                                            <controls:Command Key="Add" SelectMode="SingleRow" OnServerClick="AddCallCenters"
                                                IDColumnName="Id" />
                                        </Commands>
                                        <Columns>
                                            <controls:GeneralGridColumn Hidden="True" Key="Id" DataFieldName="ID" />
                                            <controls:GeneralGridColumn HeaderText="<%$CPResource:CallCenterName%>" Key="Name"
                                                Width="100px" DataFieldName="Name" SearchColumnType="Text" />
                                            <controls:GeneralGridColumn HeaderText="<%$CPResource:CallCenterDescription%>" Key="Description"
                                                DataFieldName="Description" Width="100%" SearchColumnType="Text" />
                                        </Columns>
                                    </controls:Grid>
                                </LeftGridContent>
                                <RightGridContent>
                                    <controls:Grid ID="_selectedCallCentersGrid" runat="server" GridName="<%$CPResource:ActiveCallCenters%>"
                                        PrimaryKeyColumn="Id" OnDblClickCommand="Remove" EnablePaging="False">
                                        <Commands>
                                            <controls:Command Key="Remove" SelectMode="SingleRow" OnServerClick="RemoveCallCenters"
                                                IDColumnName="Id" />
                                        </Commands>
                                        <Columns>
                                            <controls:GeneralGridColumn Hidden="True" Key="Id" DataFieldName="ID" />
                                            <controls:GeneralGridColumn HeaderText="<%$CPResource:CallCenterName%>" Key="Name"
                                                Width="100px" DataFieldName="Name" SearchColumnType="Text" />
                                            <controls:GeneralGridColumn HeaderText="<%$CPResource:CallCenterDescription%>" Key="Description"
                                                DataFieldName="Description" Width="100%" SearchColumnType="Text" />
                                        </Columns>
                                    </controls:Grid>
                                </RightGridContent>
                            </controls:DoubleGrid>
                        </ContentTemplate>
                    </controls:UpdatePanel>
                </div>
            </div>
        </Content>
    </controls:Dialog>
</asp:Content>
