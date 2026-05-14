<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="QuotaBalancingParameters.aspx.cs"
    MasterPageFile="~/MasterPages/Main.Master" Inherits="Confirmit.CATI.Supervisor.Surveys.QuotaBalancingParameters" %>

<%@ Import Namespace="Confirmit.CATI.Supervisor.Resources" %>
<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="server">
    <style>
        .leftPaddedTd {
            padding-left: 11px;
        }

        .checkboxlist {
            overflow: auto;
            height: 150px;
            width: 100%;
            border: solid 1px gray;
        }

            .checkboxlist ul {
                padding: 5px;
                margin: 0;
            }

            .checkboxlist li {
                list-style: none;
            }

        .label-disabled {
            color: gray
        }
    </style>
    <input type="hidden" name="config" id="config" runat="server" />
    <controls:Dialog ID="dialog" runat="server" Mode="Modal" HideHeader="true">
        <OKButton Text="Save" OnClick="OkButtonClicked" OnClientClick="saveConfiguration()" />
        <Content>
            <main class="content-panel">
                <controls:Hint ID="hintQuotaBalancing" Text="<%$CPResource:HintQuotaBalancing%>" runat="server" />

                <div class="flex-panel flex-panel-row">
                    <div class="flex-panel flex-panel-column" style="flex: 1 1 50%;">
                        <div class="flex-panel flex-panel-row">
                            <h4>
                                <asp:Label Text="<%$CPResource:Quotas%>" runat="server" /></h4>
                            <controls:HelpTextViewer runat="server" ID="helpQuotaForBalancing" HelpTextId="HelpQuotaForBalancing"
                                TitleTextId="QuotaForBalancing" />
                        </div>
                        <div style="height: 25px; padding-top: 10px; padding-bottom: 5px;">
                            <a id="lnkSelectAllQuotas" onclick="onSelectAllQuotas(true)" href="#">Select All</a>
                            <a id="lnkDeselectAllQuotas" onclick="onSelectAllQuotas(false)" href="#">Deselect All</a>
                        </div>
                        <div class="checkboxlist">
                            <ul id="quotaList">
                            </ul>
                        </div>
                    </div>
                    <div class="flex-panel flex-panel-column" style="flex: 1 1 50%;">
                        <div class="flex-panel flex-panel-row">
                            <h4>
                                <asp:Label Text="<%$CPResource:Filter%>" runat="server" /></h4>
                            <controls:HelpTextViewer runat="server" ID="helpFilter" HelpTextId="HelpFilter" TitleTextId="Filter" />
                        </div>
                        <div style="height: 25px;"></div>
                        <div class="checkboxlist">
                            <ul id="fieldList">
                            </ul>
                        </div>
                    </div>
                </div>
                <div class="flex-panel flex-panel-row" style="margin-top: 10px;">
                    <div class="flex-panel flex-panel-row" style="flex: 1 1 50%;">
                        <asp:Label ID="lblThreshold" Text="<%$CPResource:Threshold%>" runat="server" />
                        <controls:NumericEdit ID="neThreshold" runat="server" Nullable="False" ValueText="10"  style="padding-left: 10px;"
                            MinValue="1" MaxValue="100">
                            <Buttons SpinButtonsDisplay="OnRight"></Buttons>
                        </controls:NumericEdit>
                        <controls:HelpTextViewer runat="server" ID="helpThreshold" HelpTextId="HelpThreshold"
                            TitleTextId="ThresholdTitle"></controls:HelpTextViewer>
                    </div>
                    <div class="flex-panel flex-panel-row" style="flex: 1 1 50%;">
                        <asp:Label ID="lblPriority" Text="<%$CPResource:Priority%>" runat="server" />
                        <controls:NumericEdit ID="nePriority" runat="server" Nullable="False" ValueText="500" style="padding-left: 10px;"
                            MinValue="1" MaxValue="1000">
                            <Buttons SpinButtonsDisplay="OnRight"></Buttons>
                        </controls:NumericEdit>
                        <controls:HelpTextViewer runat="server" ID="helpPriority" HelpTextId="HelpPriority"
                            TitleTextId="Priority"></controls:HelpTextViewer>
                    </div>
                </div>
            </main>
        </Content>
    </controls:Dialog>
    <script>
        var g_config;

        init(Y.JSON.parse(Y.one('#<%=config.ClientID%>').get('value')));

        function init(config) {
            g_config = config;

            var quotaListNode = Y.one('#quotaList');
            var fieldListNode = Y.one('#fieldList');

            var isAllQuotaEnabled = true;

            config.Quotas.forEach(function (quota, i, arr) {
                var quotaElementId = getQuotaElementId(quota);
                var fieldList = quota.QuotaFieldIds
                    .map(function (fieldId) {
                        var result = Y.Array.find(config.Fields, function (field) { return field.FieldId === fieldId });
                        return result.FieldName;
                    })
                    .join(', ');
                var quotaDisplayName = quota.QuotaName + ' (' + fieldList + ')';
                isAllQuotaEnabled = isAllQuotaEnabled && quota.IsEnabled;
                Y.DOM.addHTML(quotaListNode,
                    '<li><div class="checkbox-selector-wrapper"><input type="checkbox" onchange="updateFieldsState(true)" id="' + quotaElementId + '"' + (quota.IsEnabled ? ' checked' : '') + '><label>' + quotaDisplayName + '</label></div></li>');
            });

            config.Fields.forEach(function (field, i, arr) {
                var fieldElementId = getFieldElementId(field);
                var fieldLabelElementId = getFieldLabelElementId(field);
                var quotaList = config.Quotas.filter(function (quota) {
                    return quota.QuotaFieldIds.some(function (fieldId) { return fieldId === field.FieldId })
                }).map(function (q) {
                    return q.QuotaName
                }).join(", ");
                var fieldDisplayName = field.FieldName + ' (' + quotaList + ')';
                Y.DOM.addHTML(fieldListNode,
                    '<li><div class="checkbox-selector-wrapper"><input type="checkbox" id="' + fieldElementId + '"' + (field.IsEnabled ? ' checked' : '') + '><label id="' + fieldLabelElementId + '">' + fieldDisplayName + '</label></div></li>');
            });

            setSelectAllState(isAllQuotaEnabled);

            updateFieldsState(false);
        }

        function setSelectAllState(isAllQuotaEnabled) {
            if (isAllQuotaEnabled) {
                Y.one('#lnkSelectAllQuotas').hide();
                Y.one('#lnkDeselectAllQuotas').show();
            } else {
                Y.one('#lnkSelectAllQuotas').show();
                Y.one('#lnkDeselectAllQuotas').hide();
            }
        }

        function onSelectAllQuotas(checkState) {
            g_config.Quotas.forEach(function (quota, i, arr) {
                var quotaNode = Y.one('#' + getQuotaElementId(quota));
                quotaNode.set('checked', checkState);
                quota.IsEnabled = checkState;
            });
            updateFieldsState(true);
            setSelectAllState(checkState);
        }

        function saveConfiguration() {
            g_config.Quotas.forEach(function (quota, i, arr) {
                var quotaNode = Y.one('#' + getQuotaElementId(quota));
                quota.IsEnabled = quotaNode.get('checked');
            });

            g_config.Fields.forEach(function (field, i, arr) {
                var fieldNode = Y.one('#' + getFieldElementId(field));
                field.IsEnabled = fieldNode.get('checked') && !fieldNode.get('disabled');
            });

            Y.one('#<%=config.ClientID%>').set('value', Y.JSON.stringify(g_config));
        }

        function updateFieldsState(automaticalyEnableFields) {
            var availableFieldIds = g_config.Quotas.filter(function (quota) {
                var quotaNode = Y.one('#' + getQuotaElementId(quota));
                return quotaNode.get('checked');
            }).map(function (quota) { return quota.QuotaFieldIds })
                .reduce(function (a, b) { return a.concat(b) }, [])
                .filter(function (fieldIds, index, self) {
                    return self.indexOf(fieldIds) === index;
                });

            g_config.Fields.forEach(function (field, i, arr) {
                var fieldNode = Y.one('#' + getFieldElementId(field));
                var fieldlabelNode = Y.one('#' + getFieldLabelElementId(field));

                var isCheckboxDisabled = fieldNode.get('disabled');

                var isFieldAvaialble = availableFieldIds.some(function (fieldId) { return fieldId === field.FieldId });

                fieldNode.set('disabled', !isFieldAvaialble);
                fieldNode.set('readonly', !isFieldAvaialble);
                fieldlabelNode.toggleClass("label-disabled", !isFieldAvaialble);

                if (isFieldAvaialble && isCheckboxDisabled && automaticalyEnableFields) {
                    fieldNode.set('checked', true);
                }

            });
        }

        function getQuotaElementId(quota) {
            return "quota_checkbox_" + quota.QuotaId;
        }
        function getFieldLabelElementId(field) {
            return "field_label_" + field.FieldId;
        }
        function getFieldElementId(field) {
            return "field_checkbox_" + field.FieldId;
        }
    </script>
</asp:Content>
