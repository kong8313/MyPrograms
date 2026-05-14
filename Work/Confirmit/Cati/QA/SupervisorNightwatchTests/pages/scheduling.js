var SELECTORS = {
    schedulingTab: {
        selector: '//*[@id="Content_dialog_tabs"]/span/span/span[span="Scheduling Parameters"]',
        locateStrategy: 'xpath'
    },
    schedulingExistenceInformation: {
        selector: '//div[contains(text(), "No scheduling parameters are associated with this scheduling script")]',
        locateStrategy: 'xpath'
    },
    idColumnName: '#dataGrid_columnheader_1 > div.gridHeaderLabel > div',
    firstRowScheduling: '#Content_listSplitter_tmpl0_RightFrameContent_ScrList_Scripts_dataGrid tr[role="row"] td:nth-child(3)',
    viewButton: '#Content_listSplitter_tmpl0_RightFrameContent_ScrList_Scripts_topToolbar_rightMenu_ctl06',
    rulesColumnName: 'th[key="Number"]',
    shiftsTab: {
        selector: '//*[@id="Content_dialog_tabs"]/span/span/span[span="Shifts"]',
        locateStrategy: 'xpath'
    },
    displayCheckBoxInShifts: '#Content_ctl00_m_grid_topToolbar_rightMenu_ctl01_ddlShowShifts',
    shiftTypeTab: {
        selector: '//*[@id="Content_dialog_tabs"]/span/span/span[span="Shift Types"]',
        locateStrategy: 'xpath'
    },
    shiftTypeNameColumn: '#dataGrid_columnheader_1 > div > div',
    parametersTab: {
        selector: '//*[@id="Content_dialog_tabs"]/span/span/span[span="Parameters"]',
        locateStrategy: 'xpath'
    },
    idColumnInParametersTab: '#dataGrid_columnheader_0 > div > div',
    customScriptTab: {
        selector: '//*[@id="Content_dialog_tabs"]/span/span/span[span="Custom Script"]',
        locateStrategy: 'xpath'
    },
    saveButtonInParametersTab: '#Content_ctrlCustomScript_toolBar_rightMenu_btnSave',
    frame: '#listFrame'
};

var commands = {
};

module.exports = {
    url: function () {
        return this.api.launchUrl;
    },
    commands: [commands],
    elements: SELECTORS
};