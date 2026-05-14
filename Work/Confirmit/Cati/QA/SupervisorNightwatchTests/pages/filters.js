var SELECTORS = {
    filtersTab: {
        selector: '//*[@id="Content_dialog_tabs"]/span/span/span[span="Filters"]',
        locateStrategy: 'xpath'
    },
    idColumnName: {
        selector: '//*[@id="dataGrid_columnheader_1"]/div/*[contains(text(),"ID")]',
        locateStrategy: 'xpath'
    },
    nameColumnName: {
        selector: '//*[@id="dataGrid_columnheader_2"]/div/div',
        locateStrategy: 'xpath'
    },
    firstRowInterviewersCheckbox: '#ctl00_ctl00_Content_listSplitter_tmpl0_RightFrameContent_m_grid_dataGrid_it0_0_cbxSelection', 
    propertiesButton: '#Content_listSplitter_tmpl0_RightFrameContent_m_grid_topToolbar_rightMenu_ctl05',
    idRowInProperties: '#Content_dialogControl_tabs_tmpl0_trIdRow',
    membershipTab: {
        selector: '//*[@id="Content_dialogControl_tabs"]/span/span/span[span="Membership"]', 
        locateStrategy: 'xpath'
    },
    memberOfNameColumns: '#Content_dialogControl_tabs > div > div:nth-child(2) > table > tbody > tr:nth-child(1)',
    assignmentsTab: {
        selector: '//*[@id="Content_dialogControl_tabs"]/span/span/span[span="Assignments"]', 
        locateStrategy: 'xpath'
    },
    surveyIdColumnNameInAssignments: '#dataGrid_columnheader_2 > div.gridHeaderLabel',
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