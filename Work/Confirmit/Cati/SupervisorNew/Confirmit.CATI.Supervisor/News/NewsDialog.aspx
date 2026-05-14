<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NewsDialog.aspx.cs" Inherits="Confirmit.CATI.Supervisor.News.NewsDialog" MasterPageFile="~/MasterPages/Main.Master" %>

<asp:Content ID="Content" runat="server" ContentPlaceHolderID="Content">
    <style type="text/css">
        .container {
            display: flex;
            flex-direction: column;
            flex-wrap: nowrap;
            justify-content: space-between;
            width: 100%;
            height: 100%;
        }

        .container-content {
            flex: 1 0 auto;
        }

        .container-bottom {
            flex: 0 0 auto;
        }

        .new-tile {
            align-self: left;
            width: 100%;
            border-bottom-width: 1px;
            margin-bottom: 10px;
            border: 1px solid #ccc;
            border-radius: 2px;
        }

        .new-title {
            display: inline-block;
            font-weight: bold;
            width: 100%;
            padding: 7px 12px;
            margin-top: 0;
            margin-bottom: 0;
            background-color: rgba(0,0,0,.03);
            border-bottom: 1px solid rgba(0,0,0,.125);
        }

        .new-ingress {
            margin-top: 0;
            margin-bottom: 7px;
        }

        .new-body {
            padding: 12px;
            padding-bottom: 5px;
            overflow-x: auto;
        }

        .pull-left {
            float: left;
            width: 80%
        }

        .pull-right {
            float: right;
            width: 20%;
            text-align: right;
        }
    </style>
    <script>
        Y.on('load', function () {
            document.getElementById("<%=cbMarkAllAsRead.ClientID%>").addEventListener('change', function (event) {
                if (event.target.checked) {
                    var newIds = getSelectedNews();
                    document.getElementById("<%=selectedNews.ClientID%>").value = newIds.join(";");
                } else {
                    document.getElementById("<%=selectedNews.ClientID%>").value = "";
                }
            });
        });

        function getSelectedNews() {
            var selectedNews = [];
            var elements = document.getElementsByClassName('new-tile');
            Array.prototype.forEach.call(elements, function (element) {
                if (element.dataset)
                    selectedNews.push(element.dataset.newid);
                else {
                    selectedNews.push(element.getAttribute('data-newId'));
                }
            });

            return selectedNews;
        }
    </script>

    <controls:Dialog ID="dialog" runat="server" Mode="Modal" HideHeader="True"  >
        <OKButton Text="Continue to Supervisor" OnClick="ConfirmClick" TabIndex="1" />
        <CancelButton Visible="False"></CancelButton>
        <Content>
            <input runat="server" type="hidden" id="selectedNews" value="" />
            <div class="container">
                <div class="container-content">
                    <% foreach (var newsModel in News) { %>
                        <div class="new-tile" data-newId="<%=newsModel.Id %>">
                            <div class="new-title"><div class="pull-left"><%= newsModel.Title %></div><div class="pull-right"><%= newsModel.Date %></div></div>
                            <div class="new-body">
                                <h4 class="new-ingress"><%= newsModel.Ingress %></h4>
                                <p class="new-text"><%= newsModel.Body %></p>
                            </div>
                        </div>
                    <% } //foreach %>  
                </div>
                <div class="container-bottom">
                    <controls:CheckBox runat="server" ID="cbMarkAllAsRead" Checked="false" Text="<%$CPResource:MarkNewsRead%>" />
                </div> 
            </div>
        </Content>
    </controls:Dialog>
</asp:Content>
