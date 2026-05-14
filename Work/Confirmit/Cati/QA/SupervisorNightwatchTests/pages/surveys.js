var SELECTORS = {
    surveyTab: '#Content_manuSplitter_tmpl0_mainMenu_mainMenuBar_tree > ul > li:nth-child(1) > div',
    firstRowSurvey: '#Content_listSplitter_tmpl0_RightFrameContent_SrvList_m_grid_dataGrid > table > tbody > tr:nth-child(1) > td:nth-child(1) > table > tbody:nth-child(2) > tr > td > div:nth-child(2) > table > tbody > tr:nth-child(1) > td:nth-child(4)',
    firstRowSurveyCheckBox: '#ctl00_ctl00_Content_listSplitter_tmpl0_RightFrameContent_SrvList_m_grid_dataGrid_it0_0_cbxSelection',
    firstRowSurveyID: '#Content_listSplitter_tmpl0_RightFrameContent_SrvList_m_grid_dataGrid > table > tbody > tr:nth-child(1) > td:nth-child(1) > table > tbody:nth-child(2) > tr > td > div:nth-child(2) > table > tbody > tr:nth-child(1) > td:nth-child(3)',
    viewButton: '#Content_listSplitter_tmpl0_RightFrameContent_SrvList_m_grid_topToolbar_rightMenu_ctl04',
    surveyIdInGeneralName: '#Content_General_m_trName > td:nth-child(1)',
    surveyNameSort: '#Content_listSplitter_tmpl0_RightFrameContent_SrvList_m_grid_dataGrid_Description_ValueControl',
    refreshButton: '#Content_listSplitter_tmpl0_RightFrameContent_SrvList_m_grid_topToolbar_rightMenu_ctl00',
    summaryTab: {
        selector: '//*[@id="Content_dialog_tabs"]/span/span/span[span="Summary"]',
        locateStrategy: 'xpath'
    },
    idColumnNameInSummary: '#dataGrid_columnheader_0',
    assignmentsTab: {
        selector: '//*[@id="Content_dialog_tabs"]/span/span/span[span="Assignments"]',
        locateStrategy: 'xpath'
    },
    assignmentsIdColumnName: '#dataGrid_columnheader_1 > div.gridHeaderLabel > div',
    interviewerSearchTab: {
        selector: '//*[@id="Content_dialog_tabs"]/span/span/span[span="Interviewer Search"]',
        locateStrategy: 'xpath'
    },
    interviewerSearchNameColumn: '#dataGrid_columnheader_2 > div > div',
    quotasTab: {
        selector: '//*[@id="Content_dialog_tabs"]/span/span/span[span="Quotas"]',
        locateStrategy: 'xpath'
    },
    quotasFirstColumnName: '#dataGrid_columnheader_2 > div.gridHeaderLabel > div',
    shedulingTab: {
        selector: '//*[@id="Content_dialog_tabs"]/span/span/span[span="Scheduling Parameters"]',
        locateStrategy: 'xpath'
    },
    shedulingIdColumnName: '#dataGrid_columnheader_0 > div > div',
    filtersTab: {
        selector: '//*[@id="Content_dialog_tabs"]/span/span/span[span="Filters"]',
        locateStrategy: 'xpath'
    },
    filtersIdColumnName: '#dataGrid_columnheader_1 > div.gridHeaderLabel > div',
    dialerTab: {
        selector: '//*[@id="Content_dialog_tabs"]/span/span/span[span="Dialer Settings"]',
        locateStrategy: 'xpath'
    },
    dialerAbandonColumnName: '#Content_SrvInfoDialerSetting_ParametersArea_ctl00_ParameterName',
    frame: '#listFrame',
    infoFrame: '#infoFrame'
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